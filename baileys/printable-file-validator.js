const path = require('path');

const allowedRules = new Map([
    ['.pdf', { documentType: 'Pdf' }],
    ['.doc', { documentType: 'Word' }],
    ['.docx', { documentType: 'Word' }],
    ['.docm', { documentType: 'Word' }],
    ['.ppt', { documentType: 'PowerPoint' }],
    ['.pptx', { documentType: 'PowerPoint' }],
    ['.pptm', { documentType: 'PowerPoint' }],
    ['.jpg', { documentType: 'Image' }],
    ['.jpeg', { documentType: 'Image' }],
    ['.png', { documentType: 'Image' }],
    ['.webp', { documentType: 'Image' }],
    ['.bmp', { documentType: 'Image' }]
]);

const fallbackImageExtensions = new Map([
    ['image/jpeg', 'jpg'],
    ['image/png', 'png'],
    ['image/webp', 'webp'],
    ['image/bmp', 'bmp'],
    ['image/x-bmp', 'bmp'],
    ['image/x-ms-bmp', 'bmp']
]);

function normalizeMimeType(mimetype) {
    if (typeof mimetype !== 'string') return '';
    return mimetype.split(';', 1)[0].trim().toLowerCase();
}

function sanitizeFileName(fileName) {
    if (typeof fileName !== 'string' || fileName.trim().length === 0) {
        return null;
    }

    let safeName = fileName.trim();
    let previousName;

    do {
        previousName = safeName;
        safeName = path.win32.basename(path.posix.basename(safeName));
    } while (safeName !== previousName);

    if (
        safeName.length === 0
        || safeName === '.'
        || safeName === '..'
        || safeName.includes('/')
        || safeName.includes('\\')
        || safeName.split('.').includes('..')
        || path.win32.isAbsolute(safeName)
        || path.posix.isAbsolute(safeName)
    ) {
        return null;
    }

    return safeName;
}

function validatePrintableFile(fileName) {
    const safeFileName = sanitizeFileName(fileName);
    if (!safeFileName) {
        return rejected('File name is required.');
    }

    const extension = path.extname(safeFileName).toLowerCase();
    if (!extension) {
        return rejected('File extension is required.');
    }

    const rule = allowedRules.get(extension);
    if (!rule) {
        return rejected('File extension is not allowed.');
    }

    return {
        isValid: true,
        documentType: rule.documentType,
        fileName: safeFileName,
        rejectionReason: null
    };
}

function buildPrintableFileDecision(messageType, fileData, fallbackTimestamp = Date.now()) {
    if (['audioMessage', 'videoMessage', 'stickerMessage'].includes(messageType)) {
        return rejected('Message type is not printable.');
    }

    if (!['documentMessage', 'imageMessage'].includes(messageType)) {
        return rejected('Message type is not supported.');
    }

    const mimetype = fileData?.mimetype || '';
    const normalizedMimeType = normalizeMimeType(mimetype);
    const fallbackExtension = messageType === 'imageMessage'
        ? fallbackImageExtensions.get(normalizedMimeType) || 'bin'
        : 'bin';
    const fileName = fileData?.fileName || `archivo_${fallbackTimestamp}.${fallbackExtension}`;
    return validatePrintableFile(fileName);
}

function shouldDownloadPrintableMessage(messageType, fileData, fallbackTimestamp) {
    return buildPrintableFileDecision(messageType, fileData, fallbackTimestamp).isValid;
}

function rejected(rejectionReason) {
    return {
        isValid: false,
        documentType: null,
        rejectionReason
    };
}

module.exports = {
    buildPrintableFileDecision,
    normalizeMimeType,
    sanitizeFileName,
    shouldDownloadPrintableMessage,
    validatePrintableFile
};
