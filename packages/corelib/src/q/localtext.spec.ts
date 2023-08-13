beforeEach(() => {
    jest.resetModules();
    jest.unmock('./system');
});

const mockLocalTextStore = (localTexts: Record<string, any>) => {
    jest.mock('./system', () => ({
        getStateStore: jest.fn(() => localTexts)
    }));
}



describe('proxyTexts', () => {
    it('proxies simple object', async () => {
        mockLocalTextStore({
            'a.b': 'Abc',
        });

        const proxyTexts = (await import('./localtext')).proxyTexts;
        const texts = proxyTexts({}, '', {a: {}}) as any;

        expect(texts.a.b).toEqual('Abc');
    });

    it('proxies nested object', async () => {
        mockLocalTextStore({
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
        mockLocalTextStore({
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
        mockLocalTextStore({
            'a': 'Abc',
        });

        const proxyTexts = (await import('./localtext')).proxyTexts;
        const texts = proxyTexts({}, '', {}) as any;

        expect(texts.a).toEqual('Abc');
    });
});

export {}