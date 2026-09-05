const test = require('node:test');
const assert = require('node:assert/strict');
const path = require('path');

const {
    buildPrintableFileDecision,
    sanitizeFileName,
    shouldDownloadPrintableMessage,
    validatePrintableFile
} = require('./printable-file-validator');

const allowedFiles = [
    ['documento.pdf', 'Pdf'],
    ['documento.doc', 'Word'],
    ['documento.docx', 'Word'],
    ['documento.docm', 'Word'],
    ['presentacion.ppt', 'PowerPoint'],
    ['presentacion.pptx', 'PowerPoint'],
    ['presentacion.pptm', 'PowerPoint'],
    ['imagen.jpg', 'Image'],
    ['imagen.jpeg', 'Image'],
    ['imagen.png', 'Image'],
    ['imagen.webp', 'Image'],
    ['imagen.bmp', 'Image']
];

test('accepts all printable extensions', () => {
    for (const [fileName, documentType] of allowedFiles) {
        const result = validatePrintableFile(fileName);

        assert.equal(result.isValid, true, fileName);
        assert.equal(result.documentType, documentType);
        assert.equal(result.fileName, fileName);
        assert.equal(result.rejectionReason, null);
    }
});

test('compares extensions without casing differences', () => {
    const result = validatePrintableFile('DOCUMENTO.Final.V2.PDF');

    assert.equal(result.isValid, true);
    assert.equal(result.documentType, 'Pdf');
    assert.equal(result.fileName, 'DOCUMENTO.Final.V2.PDF');
});

test('rejects files without extension', () => {
    const result = validatePrintableFile('documento');

    assert.equal(result.isValid, false);
    assert.equal(result.rejectionReason, 'File extension is required.');
});

test('rejects double extensions by the last extension', () => {
    const result = validatePrintableFile('archivo.pdf.exe');

    assert.equal(result.isValid, false);
    assert.equal(result.rejectionReason, 'File extension is not allowed.');
});

test('rejects excel csv txt bin executable audio video and unknown extensions', () => {
    const rejectedFiles = [
        'hoja.xls',
        'hoja.xlsx',
        'hoja.xlsm',
        'datos.csv',
        'nota.txt',
        'archivo.bin',
        'programa.exe',
        'audio.mp3',
        'audio.wav',
        'video.mp4',
        'video.mov',
        'desconocido.xyz'
    ];

    for (const fileName of rejectedFiles) {
        const result = validatePrintableFile(fileName);

        assert.equal(result.isValid, false, fileName);
        assert.equal(result.documentType, null);
    }
});

test('builds safe fallback names for unnamed images', () => {
    const result = buildPrintableFileDecision('imageMessage', { mimetype: 'image/png; width=100' }, 1234);

    assert.equal(result.isValid, true);
    assert.equal(result.fileName, 'archivo_1234.png');
});

test('rejects audio video and stickers before download', () => {
    assert.equal(shouldDownloadPrintableMessage('audioMessage', { mimetype: 'audio/ogg; codecs=opus' }), false);
    assert.equal(shouldDownloadPrintableMessage('videoMessage', { fileName: 'video.mp4', mimetype: 'video/mp4' }), false);
    assert.equal(shouldDownloadPrintableMessage('stickerMessage', { mimetype: 'image/webp' }), false);
});

test('rejects mp4 delivered as document before download', () => {
    const result = buildPrintableFileDecision('documentMessage', {
        fileName: 'video.mp4',
        mimetype: 'application/octet-stream'
    });

    assert.equal(result.isValid, false);
    assert.equal(result.rejectionReason, 'File extension is not allowed.');
});

test('does not use mime incompatibility as current validity proof', () => {
    const result = buildPrintableFileDecision('documentMessage', {
        fileName: 'documento.pdf',
        mimetype: 'video/mp4'
    });

    assert.equal(result.isValid, true);
    assert.equal(result.fileName, 'documento.pdf');
});

test('rejected content does not reach download callback', async () => {
    let downloaded = false;

    async function downloadWhenPrintable(messageType, fileData, download) {
        const decision = buildPrintableFileDecision(messageType, fileData);
        if (!decision.isValid) return decision;

        downloaded = true;
        await download();
        return decision;
    }

    const decision = await downloadWhenPrintable('documentMessage', {
        fileName: 'video.mp4',
        mimetype: 'video/mp4'
    }, async () => {});

    assert.equal(decision.isValid, false);
    assert.equal(downloaded, false);
});

test('sanitizes route components from received names', () => {
    const unsafeNames = [
        '../../../archivo.pdf',
        '..\\..\\..\\archivo.pdf',
        'carpeta/subcarpeta/archivo.pdf',
        'C:\\temporal\\archivo.pdf',
        'a-../../../archivo.pdf'
    ];

    for (const fileName of unsafeNames) {
        const decision = buildPrintableFileDecision('documentMessage', { fileName });

        assert.equal(decision.isValid, true, fileName);
        assert.equal(decision.fileName, 'archivo.pdf');
        assert.equal(decision.fileName.includes('/'), false);
        assert.equal(decision.fileName.includes('\\'), false);
        assert.equal(decision.fileName.split('.').includes('..'), false);
    }
});

test('rejects invalid names after sanitizing before download', () => {
    const invalidNames = ['../../../', '..\\..\\', '.', '..', '   '];

    for (const fileName of invalidNames) {
        const decision = buildPrintableFileDecision('documentMessage', { fileName });

        assert.equal(decision.isValid, false, fileName);
    }
});

test('accepted final name cannot escape inbox when joined', () => {
    const inbox = 'D:\\Impresiones001\\data\\Inbox';
    const decision = buildPrintableFileDecision('documentMessage', {
        fileName: '..\\..\\..\\archivo.pdf'
    });
    const destination = path.resolve(inbox, `999999999-${decision.fileName}`);

    assert.equal(decision.isValid, true);
    assert.equal(destination.startsWith(path.resolve(inbox) + path.sep), true);
});

test('sanitizeFileName returns only a safe base name', () => {
    assert.equal(sanitizeFileName('C:\\temporal\\archivo.pdf'), 'archivo.pdf');
    assert.equal(sanitizeFileName('../../../archivo.pdf'), 'archivo.pdf');
    assert.equal(sanitizeFileName('   '), null);
});
