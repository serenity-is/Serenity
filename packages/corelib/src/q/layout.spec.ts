beforeEach(() => {
    jest.resetModules();
    jest.unmock('@optionaldeps/jquery');
});

function mockUndefinedJQuery() {
    jest.mock('@optionaldeps/jquery', () => {
        return {
            __esModule: true,
            default: undefined
        }
    });
}

describe('initFullHeightGridPage', () => {
    it('works without jQuery', async () => {
        mockUndefinedJQuery();
        const initFullHeightGridPage = (await import('./layout')).initFullHeightGridPage;
        var div = document.createElement('div');
        initFullHeightGridPage(div, { noRoute: true });
        expect(document.documentElement.classList.contains('full-height-page')).toBe(true);
        expect(div.classList.contains('responsive-height')).toBe(true);
    });

    it('works with jQuery and HTML element', async () => {
        const initFullHeightGridPage = (await import('./layout')).initFullHeightGridPage;
        var div = document.createElement('div');
        initFullHeightGridPage(div, { noRoute: true });
        expect(document.documentElement.classList.contains('full-height-page')).toBe(true);
        expect(div.classList.contains('responsive-height')).toBe(true);
    });

    it('works with jQuery and jQuery wrapped element', async () => {
        const initFullHeightGridPage = (await import('./layout')).initFullHeightGridPage;
        var div = document.createElement('div');
        const $ = (await import("@optionaldeps/jquery")).default as any;
        initFullHeightGridPage($(div), { noRoute: true });
        expect(document.documentElement.classList.contains('full-height-page')).toBe(true);
        expect(div.classList.contains('responsive-height')).toBe(true);
    });
});

describe('isMobileView', () => {

    it('should return false if window is undefined', async () => {
        const isMobileView = (await import('./layout')).isMobileView;
        var oldWindow = window;
        window = undefined;
        try {
            expect(isMobileView()).toBe(false);
        }
        finally {
            window = oldWindow;
        }
    });

    it('should return based on window.matchMedia result for "width < 768px"', async () => {
        const isMobileView = (await import('./layout')).isMobileView;
        var matchMedia = window.matchMedia;
        try {
            var q: string;
            window.matchMedia = (query: string) => {
                q = query;
                return { matches: false } as any;
            };
            expect(isMobileView()).toBe(false);
            expect(q).toBe('(max-width: 767px)');
            q = null;
            window.matchMedia = (query: string) => {
                q = query;
                return { matches: true } as any;
            };
            expect(isMobileView()).toBe(true);
            expect(q).toBe('(max-width: 767px)');
        }
        finally {
            window.matchMedia = matchMedia;
        }
    });

    it('should check window.width if window.matchMedia not available', async () => {
        const isMobileView = (await import('./layout')).isMobileView;
        
        var matchMedia = window.matchMedia;
        try {
            delete window.matchMedia;
            window.innerWidth = 100;
            expect(isMobileView()).toBe(true);
            window.innerWidth = 1024;
            expect(isMobileView()).toBe(false);
        }
        finally {
            window.matchMedia = matchMedia;
        }
    });
});

export {}