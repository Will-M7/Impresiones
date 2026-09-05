const test = require('node:test');
const assert = require('node:assert/strict');
const {
    installSafeConsoleFilter,
    isInstalled,
    isSensitiveMessage,
    resetFilter,
    SIGNAL_PREFIXES
} = require('./safe-console-filter');

test('blocks exact libsignal session prefixes case-insensitively', () => {
    assert.equal(isSensitiveMessage(['Closing open session in favor of incoming prekey bundle']), true);
    assert.equal(isSensitiveMessage(['Closing session:']), true);
    assert.equal(isSensitiveMessage(['closing OPEN session IN favor OF incoming prekey bundle']), true);
    assert.equal(isSensitiveMessage(['CLOSING SESSION:']), true);
});

test('does not block normal operational messages with broad crypto words', () => {
    const normalCases = [
        ['[RECHAZADO] Archivo no imprimible: File extension is not allowed.'],
        ['[✅ GUARDADO] documento.pdf'],
        ['Normal session status for operator dashboard'],
        ['Encryption setting documented for future support'],
        ['Error al descargar archivo: timeout']
    ];

    for (const args of normalCases) {
        assert.equal(isSensitiveMessage(args), false, `Expected not sensitive: ${args[0]}`);
    }
});

test('console info blocks full libsignal call without forwarding sensitive object', () => {
    resetFilter();

    const captured = [];
    const originalInfo = console.info;
    console.info = (...args) => captured.push(args);
    const fakeSensitiveObject = { fakePrivateKey: 'fake-secret-info' };

    installSafeConsoleFilter();
    console.info('Closing open session in favor of incoming prekey bundle', fakeSensitiveObject);

    console.info = originalInfo;
    resetFilter();

    assert.equal(captured.length, 0);
    assert.equal(JSON.stringify(captured).includes('fake-secret-info'), false);
});

test('console warn blocks full libsignal call without forwarding sensitive object', () => {
    resetFilter();

    const captured = [];
    const originalWarn = console.warn;
    console.warn = (...args) => captured.push(args);
    const fakeSensitiveObject = { fakeSessionEntry: 'fake-secret-warn' };

    installSafeConsoleFilter();
    console.warn('Closing session:', fakeSensitiveObject);

    console.warn = originalWarn;
    resetFilter();

    assert.equal(captured.length, 0);
    assert.equal(JSON.stringify(captured).includes('fake-secret-warn'), false);
});

test('console filter preserves normal warn info and error channels', () => {
    resetFilter();

    const capturedInfo = [];
    const capturedWarn = [];
    const capturedError = [];
    const originalInfo = console.info;
    const originalWarn = console.warn;
    const originalError = console.error;
    console.info = (...args) => capturedInfo.push(args);
    console.warn = (...args) => capturedWarn.push(args);
    console.error = (...args) => capturedError.push(args);

    installSafeConsoleFilter();
    console.info('[✅ GUARDADO] documento.pdf');
    console.warn('[RECHAZADO] Archivo no imprimible');
    console.error('Error operativo seguro');

    console.info = originalInfo;
    console.warn = originalWarn;
    console.error = originalError;
    resetFilter();

    assert.equal(capturedInfo.length, 1);
    assert.equal(capturedWarn.length, 1);
    assert.equal(capturedError.length, 1);
});

test('installation is idempotent', () => {
    resetFilter();
    assert.equal(isInstalled(), false);

    installSafeConsoleFilter();
    assert.equal(isInstalled(), true);

    installSafeConsoleFilter();
    assert.equal(isInstalled(), true);

    resetFilter();
});

test('prefix list is concrete and limited to known libsignal messages', () => {
    assert.deepEqual(SIGNAL_PREFIXES, [
        'Closing open session in favor of incoming prekey bundle',
        'Closing session:',
        'Session already closed',
        'Session already open',
        'Decrypted message with closed session.'
    ]);
});
