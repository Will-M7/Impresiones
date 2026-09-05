const SIGNAL_PREFIXES = [
    'Closing open session in favor of incoming prekey bundle',
    'Closing session:',
    'Session already closed',
    'Session already open',
    'Decrypted message with closed session.'
];

function isSensitiveMessage(args) {
    if (!args || args.length === 0) return false;

    const first = args[0];
    if (typeof first !== 'string') return false;

    const normalized = first.trim().toLowerCase();
    return SIGNAL_PREFIXES.some(prefix => normalized.startsWith(prefix.toLowerCase()));
}

let installed = false;

function installSafeConsoleFilter() {
    if (installed) return;

    const originalInfo = console.info;
    const originalWarn = console.warn;

    console.info = function (...args) {
        if (isSensitiveMessage(args)) return;
        originalInfo.apply(console, args);
    };

    console.warn = function (...args) {
        if (isSensitiveMessage(args)) return;
        originalWarn.apply(console, args);
    };

    installed = true;
}

function isInstalled() {
    return installed;
}

function resetFilter() {
    installed = false;
}

module.exports = {
    installSafeConsoleFilter,
    isInstalled,
    isSensitiveMessage,
    resetFilter,
    SIGNAL_PREFIXES
};
