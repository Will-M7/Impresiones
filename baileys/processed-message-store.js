const fs = require('fs');
const path = require('path');
const { createHash } = require('node:crypto');

const PROJECT_ROOT = path.resolve(__dirname, '..');
const STORE_DIR = path.join(PROJECT_ROOT, 'data', 'Temp');
const STORE_FILE = path.join(STORE_DIR, 'baileys-processed-messages.json');
const MAX_ENTRIES = 10_000;

const SECRET_SALT = 'impresiones-processed-message-v1';

function buildKey(messageKey) {
    const composite = `${messageKey.id || ''}:${messageKey.remoteJid || ''}:${messageKey.participant || ''}:${messageKey.fromMe || false}`;
    return createHash('sha256').update(`${SECRET_SALT}:${composite}`).digest('hex');
}

function createProcessedMessageStore(options = {}) {
    const fileSystem = options.fileSystem || fs;
    const storeFile = options.storeFile || STORE_FILE;
    const storeDir = path.dirname(storeFile);
    const tempFile = `${storeFile}.tmp`;
    const inMemory = new Set();
    const inProcessing = new Set();
    let persisted = [];

    function load() {
        try {
            if (!fileSystem.existsSync(storeFile)) {
                persisted = [];
                return;
            }

            const raw = fileSystem.readFileSync(storeFile, 'utf-8');
            const parsed = JSON.parse(raw);

            if (!Array.isArray(parsed)) {
                console.warn('[DEDUP] Persisted store is not an array. Initializing empty.');
                persisted = [];
                return;
            }

            persisted = limitToLatestUnique(parsed.filter(entry => typeof entry === 'string'));
        } catch {
            console.warn('[DEDUP] Persisted store is corrupted. Initializing empty.');
            persisted = [];
        }

        inMemory.clear();
        for (const hash of persisted) {
            inMemory.add(hash);
        }
    }

    function persist(nextPersisted) {
        const limited = limitToLatestUnique(nextPersisted);

        try {
            if (!fileSystem.existsSync(storeDir)) {
                fileSystem.mkdirSync(storeDir, { recursive: true });
            }

            fileSystem.writeFileSync(tempFile, JSON.stringify(limited), 'utf-8');
            fileSystem.renameSync(tempFile, storeFile);
        } catch (err) {
            try {
                if (fileSystem.existsSync(tempFile)) {
                    fileSystem.unlinkSync(tempFile);
                }
            } catch {
                // Preserve the original persistence failure for the caller.
            }

            throw err;
        }

        return limited;
    }

    function isProcessed(key) {
        return inMemory.has(key);
    }

    function markProcessing(key) {
        inProcessing.add(key);
    }

    function isProcessing(key) {
        return inProcessing.has(key);
    }

    function markCompleted(key) {
        try {
            if (inMemory.has(key)) {
                return;
            }

            const nextPersisted = persist([...persisted, key]);
            replaceCompletedState(nextPersisted);
        } finally {
            inProcessing.delete(key);
        }
    }

    function releaseProcessing(key) {
        inProcessing.delete(key);
    }

    function processedCount() {
        return inMemory.size;
    }

    function clear() {
        inMemory.clear();
        inProcessing.clear();
        persisted = [];
    }

    function replaceCompletedState(nextPersisted) {
        persisted = nextPersisted;
        inMemory.clear();

        for (const hash of persisted) {
            inMemory.add(hash);
        }
    }

    load();

    return {
        buildKey,
        isProcessed,
        markProcessing,
        isProcessing,
        markCompleted,
        releaseProcessing,
        processedCount,
        clear
    };
}

function limitToLatestUnique(entries) {
    const seen = new Set();
    const limited = [];

    for (let index = entries.length - 1; index >= 0 && limited.length < MAX_ENTRIES; index--) {
        const key = entries[index];
        if (!seen.has(key)) {
            seen.add(key);
            limited.unshift(key);
        }
    }

    return limited;
}

module.exports = {
    buildKey,
    createProcessedMessageStore,
    MAX_ENTRIES,
    STORE_FILE
};
