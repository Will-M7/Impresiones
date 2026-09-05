const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('fs');
const os = require('os');
const path = require('path');
const { buildKey, createProcessedMessageStore, MAX_ENTRIES, STORE_FILE } = require('./processed-message-store');

function makeKey(id, remoteJid = 'jid@a', participant, fromMe = false) {
    return buildKey({ id, remoteJid, participant, fromMe });
}

function createIsolatedStoreFile() {
    const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'impresiones-store-'));
    return {
        directory,
        storeFile: path.join(directory, 'baileys-processed-messages.json')
    };
}

function readPersisted(storeFile) {
    return JSON.parse(fs.readFileSync(storeFile, 'utf-8'));
}

function isInsideDirectory(filePath, directory) {
    const relative = path.relative(path.resolve(directory), path.resolve(filePath));
    return relative.length > 0 && !relative.startsWith('..') && !path.isAbsolute(relative);
}

test('buildKey produces a sha256 hex string', () => {
    const key = makeKey('msg-1', '51999999999@s.whatsapp.net');

    assert.equal(typeof key, 'string');
    assert.equal(key.length, 64);
    assert.match(key, /^[a-f0-9]{64}$/);
});

test('buildKey does not contain phone numbers or JIDs', () => {
    const key = makeKey('msg-1', '51999999999@s.whatsapp.net', '51888888888@s.whatsapp.net');

    assert.equal(key.includes('51999'), false);
    assert.equal(key.includes('51888'), false);
    assert.equal(key.includes('@s.whatsapp.net'), false);
});

test('buildKey is deterministic', () => {
    assert.equal(makeKey('msg-1'), makeKey('msg-1'));
});

test('buildKey differs for different messages', () => {
    assert.notEqual(makeKey('msg-1'), makeKey('msg-2'));
});

test('new ID is not processed', () => {
    const { storeFile } = createIsolatedStoreFile();
    const store = createProcessedMessageStore({ storeFile });

    assert.equal(store.isProcessed(makeKey('fresh-001')), false);
});

test('after marking completed the ID is recognized as processed', () => {
    const { storeFile } = createIsolatedStoreFile();
    const store = createProcessedMessageStore({ storeFile });
    const key = makeKey('mark-001');

    store.markCompleted(key);

    assert.equal(store.isProcessed(key), true);
});

test('new store instance recognizes previously persisted IDs', () => {
    const { storeFile } = createIsolatedStoreFile();
    const key = makeKey('persist-001');

    createProcessedMessageStore({ storeFile }).markCompleted(key);
    const store = createProcessedMessageStore({ storeFile });

    assert.equal(store.isProcessed(key), true);
});

test('two concurrent receives do not download twice', () => {
    const { storeFile } = createIsolatedStoreFile();
    const store = createProcessedMessageStore({ storeFile });
    const key = makeKey('concurrent-001');

    store.markProcessing(key);
    const secondWouldProcess = !store.isProcessed(key) && !store.isProcessing(key);

    assert.equal(secondWouldProcess, false);
});

test('download failure allows retry by releasing processing', () => {
    const { storeFile } = createIsolatedStoreFile();
    const store = createProcessedMessageStore({ storeFile });
    const key = makeKey('fail-001');

    store.markProcessing(key);
    store.releaseProcessing(key);

    assert.equal(store.isProcessed(key), false);
    assert.equal(store.isProcessing(key), false);
});

test('store keeps in-memory and persisted state limited to 10000 entries', () => {
    const { storeFile } = createIsolatedStoreFile();
    const store = createProcessedMessageStore({ storeFile });

    for (let i = 0; i < MAX_ENTRIES + 50; i++) {
        store.markCompleted(makeKey(`limit-${i}`));
    }

    const persisted = readPersisted(storeFile);
    assert.equal(persisted.length, MAX_ENTRIES);
    assert.equal(store.processedCount(), MAX_ENTRIES);
});

test('store keeps newest entries and evicts oldest entries after exceeding limit', () => {
    const { storeFile } = createIsolatedStoreFile();
    const store = createProcessedMessageStore({ storeFile });
    const oldest = makeKey('limit-0');
    const newest = makeKey(`limit-${MAX_ENTRIES + 9}`);

    for (let i = 0; i < MAX_ENTRIES + 10; i++) {
        store.markCompleted(makeKey(`limit-${i}`));
    }

    assert.equal(store.isProcessed(oldest), false);
    assert.equal(store.isProcessed(newest), true);
    assert.equal(readPersisted(storeFile).includes(oldest), false);
    assert.equal(readPersisted(storeFile).includes(newest), true);
});

test('repeated key is not added twice to persisted order', () => {
    const { storeFile } = createIsolatedStoreFile();
    const store = createProcessedMessageStore({ storeFile });
    const key = makeKey('repeat-001');

    store.markCompleted(key);
    store.markCompleted(key);

    assert.deepEqual(readPersisted(storeFile), [key]);
    assert.equal(store.processedCount(), 1);
});

test('loading more than 10000 entries initializes only newest entries in memory and persistence', () => {
    const { storeFile } = createIsolatedStoreFile();
    const oldKey = makeKey('loaded-0');
    const newKey = makeKey(`loaded-${MAX_ENTRIES + 19}`);
    const existing = Array.from({ length: MAX_ENTRIES + 20 }, (_, index) => makeKey(`loaded-${index}`));
    fs.writeFileSync(storeFile, JSON.stringify(existing), 'utf-8');

    const store = createProcessedMessageStore({ storeFile });

    assert.equal(store.processedCount(), MAX_ENTRIES);
    assert.equal(store.isProcessed(oldKey), false);
    assert.equal(store.isProcessed(newKey), true);
});

test('missing file initializes empty', () => {
    const { storeFile } = createIsolatedStoreFile();
    const store = createProcessedMessageStore({ storeFile });

    assert.equal(store.processedCount(), 0);
});

test('corrupted file does not expose sensitive content', () => {
    const { storeFile } = createIsolatedStoreFile();
    fs.writeFileSync(storeFile, '{ not valid json !!!', 'utf-8');
    const warnMessages = [];
    const originalWarn = console.warn;
    console.warn = (...args) => warnMessages.push(args.join(' '));

    const store = createProcessedMessageStore({ storeFile });
    console.warn = originalWarn;

    assert.equal(store.processedCount(), 0);
    assert.equal(warnMessages.some(message => message.includes('corrupted')), true);
    assert.equal(warnMessages.some(message => message.includes('{ not valid json !!!')), false);
});

test('runtime store file is exactly inside data Temp', () => {
    const testDirectory = __dirname;
    const expectedTempDirectory = path.resolve(testDirectory, '..', 'data', 'Temp');

    assert.equal(path.dirname(path.resolve(STORE_FILE)), expectedTempDirectory);
    assert.equal(isInsideDirectory(STORE_FILE, expectedTempDirectory), true);
});

test('temporary store file is exactly inside data Temp', () => {
    const testDirectory = __dirname;
    const expectedTempDirectory = path.resolve(testDirectory, '..', 'data', 'Temp');
    const tempFile = `${STORE_FILE}.tmp`;

    assert.equal(path.dirname(path.resolve(tempFile)), expectedTempDirectory);
    assert.equal(isInsideDirectory(tempFile, expectedTempDirectory), true);
});

test('store paths cannot escape data Temp by prefix similarity', () => {
    const tempDirectory = path.resolve(__dirname, '..', 'data', 'Temp');
    const maliciousSibling = path.resolve(__dirname, '..', 'data', 'TempMalicioso', 'store.json');

    assert.equal(isInsideDirectory(maliciousSibling, tempDirectory), false);
});

test('failed persistence removes temporary file and allows retry', () => {
    const { storeFile } = createIsolatedStoreFile();
    const key = makeKey('retry-001');
    let shouldFailRename = true;
    const fileSystem = {
        existsSync: fs.existsSync,
        mkdirSync: fs.mkdirSync,
        readFileSync: fs.readFileSync,
        writeFileSync: fs.writeFileSync,
        unlinkSync: fs.unlinkSync,
        renameSync(source, target) {
            if (shouldFailRename) {
                throw new Error('simulated rename failure');
            }

            return fs.renameSync(source, target);
        }
    };
    const store = createProcessedMessageStore({ storeFile, fileSystem });

    store.markProcessing(key);
    assert.throws(() => store.markCompleted(key), /simulated rename failure/);

    assert.equal(fs.existsSync(`${storeFile}.tmp`), false);
    assert.equal(store.isProcessed(key), false);
    assert.equal(store.isProcessing(key), false);

    shouldFailRename = false;
    store.markProcessing(key);
    store.markCompleted(key);

    assert.equal(store.isProcessed(key), true);
    assert.deepEqual(readPersisted(storeFile), [key]);
});

test('failed persistence preserves previous valid state', () => {
    const { storeFile } = createIsolatedStoreFile();
    const existingKey = makeKey('existing-001');
    const newKey = makeKey('new-001');
    fs.writeFileSync(storeFile, JSON.stringify([existingKey]), 'utf-8');
    const fileSystem = {
        existsSync: fs.existsSync,
        mkdirSync: fs.mkdirSync,
        readFileSync: fs.readFileSync,
        writeFileSync: fs.writeFileSync,
        unlinkSync: fs.unlinkSync,
        renameSync() {
            throw new Error('simulated rename failure');
        }
    };
    const store = createProcessedMessageStore({ storeFile, fileSystem });

    store.markProcessing(newKey);
    assert.throws(() => store.markCompleted(newKey), /simulated rename failure/);

    assert.equal(store.isProcessed(existingKey), true);
    assert.equal(store.isProcessed(newKey), false);
    assert.deepEqual(readPersisted(storeFile), [existingKey]);
});
