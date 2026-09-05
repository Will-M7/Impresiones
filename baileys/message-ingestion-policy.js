const UPsertABLE_MESSAGE_TYPES = ['documentMessage', 'imageMessage', 'audioMessage', 'videoMessage', 'stickerMessage'];

const ALLOWED_CHAT_SUFFIXES = ['@s.whatsapp.net', '@lid', '@g.us'];

const BLOCKED_CHAT_PATTERNS = ['@broadcast', '@newsletter'];

function evaluateEvent(upsertPayload) {
    if (!upsertPayload || typeof upsertPayload !== 'object') {
        return { accepted: false, reason: 'Invalid event payload.' };
    }

    if (upsertPayload.type !== 'notify') {
        return { accepted: false, reason: 'Event type is not notify.' };
    }

    const messages = upsertPayload.messages;
    if (!Array.isArray(messages)) {
        return { accepted: false, reason: 'No messages array in event.' };
    }

    return { accepted: true, reason: null, messages };
}

function evaluateMessage(message) {
    if (!message || typeof message !== 'object') {
        return { accepted: false, reason: 'Invalid message object.' };
    }

    const key = message.key;
    if (!key || typeof key !== 'object') {
        return { accepted: false, reason: 'Message has no key.' };
    }

    if (key.fromMe === true) {
        return { accepted: false, reason: 'Message is from self.' };
    }

    if (!key.id || typeof key.id !== 'string' || key.id.length === 0) {
        return { accepted: false, reason: 'Message has no id.' };
    }

    const remoteJid = key.remoteJid || '';
    if (!remoteJid) {
        return { accepted: false, reason: 'Message has no remoteJid.' };
    }

    if (remoteJid === 'status@broadcast') {
        return { accepted: false, reason: 'Status broadcast is not supported.' };
    }

    if (remoteJid.endsWith('@broadcast')) {
        return { accepted: false, reason: 'Broadcast messages are not supported.' };
    }

    if (remoteJid.endsWith('@newsletter')) {
        return { accepted: false, reason: 'Newsletter messages are not supported.' };
    }

    const isDirect = remoteJid.endsWith('@s.whatsapp.net') || remoteJid.endsWith('@lid');
    const isGroup = remoteJid.endsWith('@g.us');

    if (!isDirect && !isGroup) {
        return { accepted: false, reason: 'Unknown chat type.' };
    }

    if (isGroup && !key.participant) {
        return { accepted: false, reason: 'Group message without participant.' };
    }

    const senderJid = isGroup ? key.participant : remoteJid;

    return { accepted: true, reason: null, senderJid };
}

module.exports = {
    evaluateEvent,
    evaluateMessage,
    UPsertABLE_MESSAGE_TYPES,
    ALLOWED_CHAT_SUFFIXES,
    BLOCKED_CHAT_PATTERNS
};
