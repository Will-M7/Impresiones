const { 
    default: makeWASocket, 
    useMultiFileAuthState, 
    DisconnectReason, 
    fetchLatestBaileysVersion, 
    downloadMediaMessage 
} = require('@whiskeysockets/baileys');
const qrcode = require('qrcode-terminal');
const fs = require('fs');
const path = require('path');
const pino = require('pino');
const { buildPrintableFileDecision } = require('./printable-file-validator');
const { evaluateEvent, evaluateMessage } = require('./message-ingestion-policy');
const { createProcessedMessageStore } = require('./processed-message-store');
const { installSafeConsoleFilter } = require('./safe-console-filter');

installSafeConsoleFilter();

const PROJECT_ROOT = path.resolve(__dirname, '..');
const DOWNLOAD_DIR = path.join(PROJECT_ROOT, 'data', 'Inbox');

[DOWNLOAD_DIR].forEach(dir => {
    if (!fs.existsSync(dir)) fs.mkdirSync(dir, { recursive: true });
});

const processedStore = createProcessedMessageStore();
const lidToPhoneMap = new Map();

function limpiarNumero(numeroRaw) {
    if (!numeroRaw) return null;
    let numero = numeroRaw.split('@')[0].split(':')[0].replace(/\D/g, '');
    if (numero.startsWith('51') && numero.length > 9) {
        numero = numero.slice(2);
    }
    return numero || null;
}

function obtenerNumeroTelefonoReal(senderJid) {
    if (!senderJid) return 'desconocido';

    if (senderJid.includes('@lid')) {
        const lidClean = senderJid.split('@')[0].split(':')[0];
        if (lidToPhoneMap.has(lidClean)) {
            return lidToPhoneMap.get(lidClean);
        }
    } else {
        const numLimpio = limpiarNumero(senderJid);
        if (numLimpio) return numLimpio;
    }

    const fallbackNum = limpiarNumero(senderJid);
    return fallbackNum || 'desconocido';
}

function unwrapMessage(msg) {
    let message = msg.message;
    if (!message) return null;

    if (message.ephemeralMessage) message = message.ephemeralMessage.message;
    if (message.viewOnceMessage) message = message.viewOnceMessage.message;
    if (message.viewOnceMessageV2) message = message.viewOnceMessageV2.message;

    return message;
}

async function connectToWhatsApp() {
    const { state, saveCreds } = await useMultiFileAuthState('baileys_auth_info');
    const { version } = await fetchLatestBaileysVersion();

    const sock = makeWASocket({
        version,
        auth: state,
        printQRInTerminal: false,
        syncFullHistory: false,
        logger: pino({ level: 'silent' })
    });

    sock.ev.on('creds.update', saveCreds);

    sock.ev.on('contacts.upsert', (contacts) => {
        for (const contact of contacts) {
            if (contact.id && contact.lid) {
                const phone = limpiarNumero(contact.id);
                const lid = contact.lid.split('@')[0].split(':')[0];
                if (phone && lid) {
                    lidToPhoneMap.set(lid, phone);
                }
            }
        }
    });

    sock.ev.on('connection.update', (update) => {
        const { connection, lastDisconnect, qr } = update;

        if (qr) {
            console.log('\n======================================================');
            console.log('📌 ESCANEA EL SIGUIENTE CÓDIGO QR EN TU WHATSAPP:');
            console.log('======================================================\n');
            qrcode.generate(qr, { small: true });
        }

        if (connection === 'close') {
            const shouldReconnect = (lastDisconnect?.error?.output?.statusCode !== DisconnectReason.loggedOut);
            if (shouldReconnect) {
                connectToWhatsApp();
            }
        } else if (connection === 'open') {
            console.log('\n======================================================');
            console.log('✅ ¡CONECTADO EXITOSAMENTE A WHATSAPP (BAILEYS)!');
            console.log(`📁 Descargas generales: ${DOWNLOAD_DIR}`);
            console.log('======================================================\n');
        }
    });

    sock.ev.on('messages.upsert', async (upsertPayload) => {
        const eventResult = evaluateEvent(upsertPayload);
        if (!eventResult.accepted) return;

        for (const msg of eventResult.messages) {
            const msgResult = evaluateMessage(msg);
            if (!msgResult.accepted) continue;

            const dedupKey = processedStore.buildKey(msg.key);

            if (processedStore.isProcessed(dedupKey)) continue;

            if (processedStore.isProcessing(dedupKey)) continue;

            processedStore.markProcessing(dedupKey);

            try {
                const unwrapped = unwrapMessage(msg);
                if (!unwrapped) {
                    processedStore.markCompleted(dedupKey);
                    continue;
                }

                const messageType = Object.keys(unwrapped)[0];

                if (!['documentMessage', 'imageMessage', 'audioMessage', 'videoMessage', 'stickerMessage'].includes(messageType)) {
                    processedStore.markCompleted(dedupKey);
                    continue;
                }

                const fileData = unwrapped[messageType];
                const decision = buildPrintableFileDecision(messageType, fileData);

                if (!decision.isValid) {
                    console.log(`[RECHAZADO] Archivo no imprimible: ${decision.rejectionReason}`);
                    processedStore.markCompleted(dedupKey);
                    continue;
                }

                const phoneNumber = obtenerNumeroTelefonoReal(msgResult.senderJid);
                const buffer = await downloadMediaMessage(msg, 'buffer', {});
                const newFilename = `${phoneNumber}-${decision.fileName}`;
                const filePath = path.join(DOWNLOAD_DIR, newFilename);

                fs.writeFileSync(filePath, buffer);
                console.log(`[✅ GUARDADO] ${newFilename}`);

                processedStore.markCompleted(dedupKey);
            } catch (err) {
                console.error('❌ Error al descargar archivo:', err.message);
                processedStore.releaseProcessing(dedupKey);
            }
        }
    });
}

connectToWhatsApp();
