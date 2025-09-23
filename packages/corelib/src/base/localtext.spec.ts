import { localTextTableSymbol } from './symbols';
import { getGlobalObject } from './system';

const mockLocalTextTable = (localTexts: Record<string, any>) => {
    return (getGlobalObject()[localTextTableSymbol] = localTexts ?? {});
}

afterEach(() => {
    mockLocalTextTable({});
});

describe('proxyTexts', () => {
    it('proxies simple object', async () => {
        mockLocalTextTable({
            'a.b': 'Abc',
        });

        const proxyTexts = (await import('./localtext')).proxyTexts;
        const texts = proxyTexts({}, '', { a: {} }) as any;

        expect(texts.a.b).toEqual('Abc');
    });

    it('proxies nested object', async () => {
        mockLocalTextTable({
            'a.b.c': 'Ab12c',
            'a.c': 'A1c',
        });

        const proxyTexts = (await import('./localtext')).proxyTexts;
        const texts = proxyTexts({}, '', {
            a: {
                b: {}
            }
        }) as any;

        expect(texts.a.b.c).toEqual('Ab12c');
        expect(texts.a.c).toEqual('A1c');
    });

    it('proxies multiple nested object', async () => {
        mockLocalTextTable({
            'a.b.c': 'Ab12c',
            'a.c': 'A1c',
            'b.c.d': 'B1cd',
        });

        const proxyTexts = (await import('./localtext')).proxyTexts;
        const texts = proxyTexts({}, '', {
            a: {
                b: {}
            },
            b: {
                c: {}
            }
        }) as any;

        expect(texts.b.c.d).toEqual('B1cd');
    });

    it('proxies single level object', async () => {
        mockLocalTextTable({
            'a': 'Abc',
        });

        const proxyTexts = (await import('./localtext')).proxyTexts;
        const texts = proxyTexts({}, '', {}) as any;

        expect(texts.a).toEqual('Abc');
    });

    it('returns own keys', async () => {
        mockLocalTextTable({
            'a': 'Abc',
        });
        const proxyTexts = (await import('./localtext')).proxyTexts;
        const texts = proxyTexts({}, '', { a: {} }) as any;
        expect(Object.getOwnPropertyNames(texts)).toEqual(['a']);
    });

    it('asTry returns proxy that uses tryGetText', async () => {
        mockLocalTextTable({
            'a.b': 'Abc',
            'a.c': 'Acd',
        });
        const proxyTexts = (await import('./localtext')).proxyTexts;
        const texts = proxyTexts({}, '', {
            a: {}
        }) as any;

        const tryTexts = texts.a.asTry();
        expect(tryTexts.b).toEqual('Abc');
        expect(tryTexts.c).toEqual('Acd');
        expect(tryTexts.d).toEqual(undefined);
    });

    it('asKey returns proxy that returns raw keys', async () => {
        mockLocalTextTable({
            'a.b': 'Abc',
        });
        const proxyTexts = (await import('./localtext')).proxyTexts;
        const texts = proxyTexts({}, '', {
            a: {}
        }) as any;

        const keyTexts = texts.a.asKey();
        expect(keyTexts.b).toEqual('a.b');
        expect(keyTexts.c).toEqual('a.c');
    });

    it('asTry returns the same proxy instance on multiple calls', async () => {
        const proxyTexts = (await import('./localtext')).proxyTexts;
        const texts = proxyTexts({}, '', {
            a: {}
        }) as any;

        const tryTexts1 = texts.a.asTry();
        const tryTexts2 = texts.a.asTry();
        expect(tryTexts1).toBe(tryTexts2);
    });

    it('asKey returns the same proxy instance on multiple calls', async () => {
        const proxyTexts = (await import('./localtext')).proxyTexts;
        const texts = proxyTexts({}, '', {
            a: {}
        }) as any;

        const keyTexts1 = texts.a.asKey();
        const keyTexts2 = texts.a.asKey();
        expect(keyTexts1).toBe(keyTexts2);
    });

    it('asTry on asTry proxy returns itself', async () => {
        const proxyTexts = (await import('./localtext')).proxyTexts;
        const texts = proxyTexts({}, '', {
            a: {}
        }) as any;

        const tryTexts = texts.a.asTry();
        expect(tryTexts.asTry()).toBe(tryTexts);
    });

    it('asKey on asKey proxy returns itself', async () => {
        const proxyTexts = (await import('./localtext')).proxyTexts;
        const texts = proxyTexts({}, '', {
            a: {}
        }) as any;

        const keyTexts = texts.a.asKey();
        expect(keyTexts.asKey()).toBe(keyTexts);
    });

    it('sub-objects of asTry proxy also have asTry and asKey methods', async () => {
        mockLocalTextTable({
            'a.b.c': 'Abc',
        });
        const proxyTexts = (await import('./localtext')).proxyTexts;
        const texts = proxyTexts({}, '', {
            a: {
                b: {}
            }
        }) as any;

        const tryTexts = texts.a.asTry();
        expect(tryTexts.b.c).toEqual('Abc');
        expect(tryTexts.b.d).toEqual(undefined);

        // Sub-object should also have asTry/asKey
        const subTryTexts = tryTexts.b.asTry();
        expect(subTryTexts.c).toEqual('Abc');
        expect(subTryTexts.d).toEqual(undefined);

        const subKeyTexts = tryTexts.b.asKey();
        expect(subKeyTexts.c).toEqual('a.b.c');
        expect(subKeyTexts.d).toEqual('a.b.d');
    });

    it('sub-objects of asKey proxy also have asTry and asKey methods', async () => {
        const proxyTexts = (await import('./localtext')).proxyTexts;
        const texts = proxyTexts({}, '', {
            a: {
                b: {}
            }
        }) as any;

        const keyTexts = texts.a.asKey();
        expect(keyTexts.b.c).toEqual('a.b.c');

        // Sub-object should also have asTry/asKey
        const subTryTexts = keyTexts.b.asTry();
        expect(subTryTexts.c).toBeUndefined(); // no translation

        const subKeyTexts = keyTexts.b.asKey();
        expect(subKeyTexts.c).toEqual('a.b.c');
    });

    it('asTry and asKey proxies cache independently', async () => {
        const proxyTexts = (await import('./localtext')).proxyTexts;
        const texts = proxyTexts({}, '', {
            a: {}
        }) as any;

        const tryTexts = texts.a.asTry();
        const keyTexts = texts.a.asKey();

        // They should be different objects
        expect(tryTexts).not.toBe(keyTexts);

        // But each should return itself on repeated calls
        expect(texts.a.asTry()).toBe(tryTexts);
        expect(texts.a.asKey()).toBe(keyTexts);
    });

    it('nested proxies maintain correct behavior', async () => {
        mockLocalTextTable({
            'a.b.c': 'AbcValue',
            'a.b.d': 'AbdValue',
        });
        const proxyTexts = (await import('./localtext')).proxyTexts;
        const texts = proxyTexts({}, '', {
            a: {
                b: {}
            }
        }) as any;

        // Normal proxy
        expect(texts.a.b.c).toEqual('AbcValue');
        expect(texts.a.b.d).toEqual('AbdValue');
        expect(texts.a.b.e).toEqual('a.b.e'); // fallback to key

        // Try proxy
        const tryTexts = texts.a.asTry();
        expect(tryTexts.b.c).toEqual('AbcValue');
        expect(tryTexts.b.d).toEqual('AbdValue');
        expect(tryTexts.b.e).toEqual(undefined);

        // Key proxy
        const keyTexts = texts.a.asKey();
        expect(keyTexts.b.c).toEqual('a.b.c');
        expect(keyTexts.b.d).toEqual('a.b.d');
        expect(keyTexts.b.e).toEqual('a.b.e');
    });
});

describe('tryGetText', () => {
    it('returns undefined for non-existing keys', async () => {
        mockLocalTextTable({
            'a': 'Abc',
        });
        const tryGetText = (await import('./localtext')).tryGetText;
        expect(tryGetText("a")).toEqual('Abc');
        expect(tryGetText(null)).toEqual(undefined);
        expect(tryGetText(undefined)).toEqual(undefined);
        expect(tryGetText('')).toEqual(undefined);
        expect(tryGetText('b')).toEqual(undefined);
        expect(tryGetText('b.c')).toEqual(undefined);
    });

    it('returns the value for existing keys', async () => {
        mockLocalTextTable({
            'a': 'A',
            'b.c': "BC"
        });
        const tryGetText = (await import('./localtext')).tryGetText;
        expect(tryGetText("a")).toEqual('A');
        expect(tryGetText('b')).toEqual(undefined);
        expect(tryGetText('b.c')).toEqual("BC");
        expect(tryGetText('d')).toEqual(undefined);
    });
});

describe('localText', () => {
    it('returns empty string for null and undefined', async () => {
        mockLocalTextTable({
            'a': 'Abc',
        });
        const localText = (await import('./localtext')).localText;
        expect(localText(null)).toEqual('');
        expect(localText(undefined)).toEqual('');
    });

    it('returns the value for existing keys and the key for non existing keys', async () => {
        mockLocalTextTable({
            'a': 'A',
            'b.c': "BC"
        });
        const localText = (await import('./localtext')).localText;
        expect(localText("a")).toEqual('A');
        expect(localText('b')).toEqual('b');
        expect(localText('b.c')).toEqual("BC");
        expect(localText('d')).toEqual('d');
    });

    it('returns the second parameter for non existent keys if specified and not null', async () => {
        mockLocalTextTable({
        });
        const localText = (await import('./localtext')).localText;
        expect(localText('a', 'X')).toEqual('X');
        expect(localText('b', 'Y')).toEqual('Y');
        expect(localText('b.c', 'Z')).toEqual('Z');
        expect(localText('d', '')).toEqual('');
        expect(localText('d', null)).toEqual('d');
        expect(localText('d', undefined)).toEqual('d');
    });
});

describe('addLocalTexts', () => {
    it('ignores if first parameter is null or undefined', async () => {
        const texts = mockLocalTextTable({
            'a': 'A',
        });
        const addLocalText = (await import('./localtext')).addLocalText;
        addLocalText(undefined, "Y");
        expect(texts).toEqual({
            'a': 'A'
        });
        addLocalText(null, "Y");
        expect(texts).toEqual({
            'a': 'A'
        });

    });

    it('adds single text if first parameter is string', async () => {
        const texts = mockLocalTextTable({
            'a': 'A',
        });
        const addLocalText = (await import('./localtext')).addLocalText;
        addLocalText("x", "Y");
        expect(texts).toEqual({
            'a': 'A',
            'x': 'Y'
        });
    });

    it('overrides text if first parameter is string', async () => {
        const texts = mockLocalTextTable({
            'a': 'A',
            'b': 'C'
        });
        const addLocalText = (await import('./localtext')).addLocalText;
        addLocalText("a", "B");
        expect(texts).toEqual({
            'a': 'B',
            'b': 'C'
        });
    });

    it('adds text if first parameter is object', async () => {
        const texts = mockLocalTextTable({
            'a': 'A',
            'b.c': 'D'
        });
        const addLocalText = (await import('./localtext')).addLocalText;
        addLocalText({
            x: 'Y',
            b: {
                d: 'E'
            },
            'z': {
                u: 'W',
                v: 'Z'
            }
        });
        expect(texts).toEqual({
            'a': 'A',
            'b.c': 'D',
            'b.d': 'E',
            'x': 'Y',
            'z.u': 'W',
            'z.v': 'Z'
        });
    });

    it('adds text with prefix if second parameter is passed and first is object', async () => {
        const texts = mockLocalTextTable({
            'a': 'A',
            'b.c': 'D'
        });
        const addLocalText = (await import('./localtext')).addLocalText;
        addLocalText({
            x: 'Y',
            d: 'DD',
            z: {
                u: 'W',
                v: 'Z'
            }
        }, 'b.');
        expect(texts).toEqual({
            'a': 'A',
            'b.c': 'D',
            'b.d': 'DD',
            'b.x': 'Y',
            'b.z.u': 'W',
            'b.z.v': 'Z'
        });
    });

});