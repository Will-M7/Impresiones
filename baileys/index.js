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

// Configuración de rutas
const PROJECT_ROOT = path.resolve(__dirname, '..');
const DOWNLOAD_DIR = path.join(PROJECT_ROOT, 'data', 'Inbox');
const AUDIOS_DIR = 'D:\\Impresiones\\Baileys\\Ordenes';
const EXECUTED_DIR = 'D:\\Impresiones\\Baileys\\ejecutados';

// Crear carpetas si no existen
[DOWNLOAD_DIR, AUDIOS_DIR, EXECUTED_DIR].forEach(dir => {
    if (!fs.existsSync(dir)) fs.mkdirSync(dir, { recursive: true });
});

// Mapa de memoria para guardar la relación entre LID y Número Real (PN)
const lidToPhoneMap = new Map();

// Función para limpiar y formatear el número
function limpiarNumero(numeroRaw) {
    if (!numeroRaw) return null;
    let numero = numeroRaw.split('@')[0].split(':')[0].replace(/\D/g, '');
    if (numero.startsWith('51') && numero.length > 9) {
        numero = numero.slice(2);
    }
    return numero || null;
}

// Extraer el número telefónico real de 9 dígitos
function obtenerNumeroTelefonoReal(msg) {
    // 1. Intentar obtener JID directo del remitente o participante
    let senderJid = msg.key.participant || msg.key.remoteJid || '';

    // 2. Si el remitente viene como LID, revisar si lo tenemos mapeado en memoria
    if (senderJid.includes('@lid')) {
        const lidClean = senderJid.split('@')[0].split(':')[0];
        if (lidToPhoneMap.has(lidClean)) {
            return lidToPhoneMap.get(lidClean);
        }
    } else {
        const numLimpio = limpiarNumero(senderJid);
        if (numLimpio) return numLimpio;
    }

    // 3. Buscar en verifiedBizName o senderAlt si Baileys lo adjuntó
    const altJid = msg.key.participantAlt || msg.key.remoteJidAlt || '';
    if (altJid && !altJid.includes('@lid')) {
        const numAlt = limpiarNumero(altJid);
        if (numAlt) return numAlt;
    }

    // Fallback: Si no se pudo traducir el LID
    const fallbackNum = limpiarNumero(senderJid);
    return fallbackNum || 'desconocido';
}

// Desenvolver mensajes efímeros o de vista única
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

    // Mapear contactos recibidos para asociar LIDs con Números Reales
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
            console.log(`🎙️ Audios (Solo grupos "Impresiones..."): ${AUDIOS_DIR}`);
            console.log('======================================================\n');
        }
    });

    sock.ev.on('messages.upsert', async (m) => {
        const msg = m.messages[0];
        if (!msg || msg.key.fromMe) return;

        const unwrapped = unwrapMessage(msg);
        if (!unwrapped) return;

        const remoteJid = msg.key.remoteJid || '';
        const messageType = Object.keys(unwrapped)[0];
        const phoneNumber = obtenerNumeroTelefonoReal(msg);

        // --- 1. MANEJO EXCLUSIVO DE AUDIOS EN GRUPOS QUE EMPIECEN CON "IMPRESIONES" ---
        if (messageType === 'audioMessage') {
            if (remoteJid.endsWith('@g.us')) {
                try {
                    const groupMetadata = await sock.groupMetadata(remoteJid);
                    const groupName = groupMetadata.subject || '';

                    if (groupName.toLowerCase().startsWith('impresiones')) {
                        const buffer = await downloadMediaMessage(msg, 'buffer', {});
                        const ext = unwrapped.audioMessage?.mimetype?.includes('ogg') ? 'ogg' : 'mp3';
                        const audioFilename = `${phoneNumber}-audio_${Date.now()}.${ext}`;
                        const filePath = path.join(AUDIOS_DIR, audioFilename);

                        fs.writeFileSync(filePath, buffer);
                        console.log(`[🎙️ AUDIO GUARDADO] ${audioFilename} (Grupo: "${groupName}")`);
                    } else {
                        console.log(`[IGNORADO] Audio de grupo "${groupName}" no coincide con "Impresiones...".`);
                    }
                } catch (err) {
                    console.error('❌ Error al obtener info del grupo para el audio:', err.message);
                }
            }
            return;
        }

        // --- 2. MANEJO DE DOCUMENTOS E IMÁGENES (SE GUARDAN EN DESCARGAS) ---
        if (['documentMessage', 'imageMessage', 'videoMessage'].includes(messageType)) {
            try {
                const buffer = await downloadMediaMessage(msg, 'buffer', {});
                const fileData = unwrapped[messageType];

                let originalName = fileData.fileName;
                if (!originalName) {
                    const mime = fileData.mimetype || '';
                    let ext = mime.split('/')[1]?.split(';')[0] || 'bin';
                    if (mime.includes('wordprocessingml')) ext = 'docx';
                    else if (mime.includes('spreadsheetml')) ext = 'xlsx';
                    else if (mime.includes('pdf')) ext = 'pdf';
                    else if (mime.includes('jpeg') || mime.includes('jpg')) ext = 'jpg';
                    else if (mime.includes('png')) ext = 'png';

                    originalName = `archivo_${Date.now()}.${ext}`;
                }

                const newFilename = `${phoneNumber}-${originalName}`;
                const filePath = path.join(DOWNLOAD_DIR, newFilename);

                fs.writeFileSync(filePath, buffer);
                console.log(`[✅ GUARDADO] ${newFilename}`);
            } catch (err) {
                console.error('❌ Error al descargar archivo:', err.message);
            }
        }
    });
}

connectToWhatsApp();
