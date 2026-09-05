const test = require('node:test');
const assert = require('node:assert/strict');

const { evaluateEvent, evaluateMessage } = require('./message-ingestion-policy');

function makeEvent(messages, type = 'notify') {
    return { type, messages };
}

function makeMessage(overrides = {}) {
    return {
        key: {
            id: 'msg-001',
            remoteJid: '51999999999@s.whatsapp.net',
            fromMe: false,
            ...overrides
        }
    };
}

test('evaluateEvent accepts notify type', () => {
    const result = evaluateEvent(makeEvent([makeMessage()]));

    assert.equal(result.accepted, true);
    assert.equal(result.messages.length, 1);
});

test('evaluateEvent rejects append type', () => {
    const result = evaluateEvent(makeEvent([makeMessage()], 'append'));

    assert.equal(result.accepted, false);
    assert.equal(result.reason, 'Event type is not notify.');
});

test('evaluateEvent evaluates all messages in the array', () => {
    const msg1 = makeMessage({ id: 'msg-001' });
    const msg2 = makeMessage({ id: 'msg-002' });
    const result = evaluateEvent(makeEvent([msg1, msg2]));

    assert.equal(result.accepted, true);
    assert.equal(result.messages.length, 2);
    assert.equal(result.messages[0].key.id, 'msg-001');
    assert.equal(result.messages[1].key.id, 'msg-002');
});

test('evaluateMessage rejects self messages', () => {
    const result = evaluateMessage(makeMessage({ fromMe: true }));

    assert.equal(result.accepted, false);
    assert.equal(result.reason, 'Message is from self.');
});

test('evaluateMessage rejects message without id', () => {
    const result = evaluateMessage(makeMessage({ id: '' }));

    assert.equal(result.accepted, false);
    assert.equal(result.reason, 'Message has no id.');
});

test('evaluateMessage rejects null id', () => {
    const result = evaluateMessage(makeMessage({ id: null }));

    assert.equal(result.accepted, false);
    assert.equal(result.reason, 'Message has no id.');
});

test('evaluateMessage rejects status broadcast', () => {
    const result = evaluateMessage(makeMessage({ remoteJid: 'status@broadcast' }));

    assert.equal(result.accepted, false);
    assert.equal(result.reason, 'Status broadcast is not supported.');
});

test('evaluateMessage rejects other broadcasts', () => {
    const result = evaluateMessage(makeMessage({ remoteJid: '12345@broadcast' }));

    assert.equal(result.accepted, false);
    assert.equal(result.reason, 'Broadcast messages are not supported.');
});

test('evaluateMessage rejects newsletters', () => {
    const result = evaluateMessage(makeMessage({ remoteJid: '12345@newsletter' }));

    assert.equal(result.accepted, false);
    assert.equal(result.reason, 'Newsletter messages are not supported.');
});

test('evaluateMessage accepts direct chat with s.whatsapp.net', () => {
    const result = evaluateMessage(makeMessage({ remoteJid: '51999999999@s.whatsapp.net' }));

    assert.equal(result.accepted, true);
    assert.equal(result.senderJid, '51999999999@s.whatsapp.net');
});

test('evaluateMessage accepts direct chat with LID', () => {
    const result = evaluateMessage(makeMessage({ remoteJid: 'abc123@lid' }));

    assert.equal(result.accepted, true);
    assert.equal(result.senderJid, 'abc123@lid');
});

test('evaluateMessage accepts group using participant', () => {
    const result = evaluateMessage(makeMessage({
        remoteJid: '51999999999-1234@g.us',
        participant: '51888888888@s.whatsapp.net'
    }));

    assert.equal(result.accepted, true);
    assert.equal(result.senderJid, '51888888888@s.whatsapp.net');
});

test('evaluateMessage rejects group without participant', () => {
    const result = evaluateMessage(makeMessage({
        remoteJid: '51999999999-1234@g.us',
        participant: undefined
    }));

    assert.equal(result.accepted, false);
    assert.equal(result.reason, 'Group message without participant.');
});

test('evaluateMessage never uses group JID as sender', () => {
    const groupJid = '51999999999-1234@g.us';
    const participantJid = '51888888888@s.whatsapp.net';
    const result = evaluateMessage(makeMessage({
        remoteJid: groupJid,
        participant: participantJid
    }));

    assert.equal(result.accepted, true);
    assert.notEqual(result.senderJid, groupJid);
    assert.equal(result.senderJid, participantJid);
});

test('evaluateMessage rejects unknown chat types', () => {
    const result = evaluateMessage(makeMessage({ remoteJid: 'unknown@something.com' }));

    assert.equal(result.accepted, false);
    assert.equal(result.reason, 'Unknown chat type.');
});

test('evaluateMessage rejects invalid message object', () => {
    assert.equal(evaluateMessage(null).accepted, false);
    assert.equal(evaluateMessage(undefined).accepted, false);
    assert.equal(evaluateMessage('string').accepted, false);
});

test('evaluateMessage rejects message without key', () => {
    const result = evaluateMessage({});

    assert.equal(result.accepted, false);
    assert.equal(result.reason, 'Message has no key.');
});

test('evaluateEvent rejects null and invalid payloads', () => {
    assert.equal(evaluateEvent(null).accepted, false);
    assert.equal(evaluateEvent(undefined).accepted, false);
    assert.equal(evaluateEvent('string').accepted, false);
    assert.equal(evaluateEvent({}).accepted, false);
    assert.equal(evaluateEvent({ type: 'notify' }).accepted, false);
});
