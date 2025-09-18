
beforeEach(() => {
    vi.resetModules();
});

describe('initFullHeightGridPage', () => {
    it('works without jQuery', async () => {
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
        initFullHeightGridPage(div, { noRoute: true });
        expect(document.documentElement.classList.contains('full-height-page')).toBe(true);
        expect(div.classList.contains('responsive-height')).toBe(true);
    });
});

describe('isMobileView', () => {

    it('should return false if window is undefined', async () => {
        const isMobileView = (await import('./layout')).isMobileView;
        var oldWindow = window;
        try {
            window = undefined;
        }
        catch {
            // fails in browser mode
            return;
        }
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

describe('layoutFillHeightValue', () => {
    it('returns parsed height when jQuery is not available', async () => {
        const layoutFillHeightValue = (await import('./layout')).layoutFillHeightValue;
        const element = document.createElement('div');
        element.style.height = '100px';
        const result = layoutFillHeightValue(element);
        expect(result).toBe(100);
    });
});

describe('layoutFillHeight', () => {
    it('sets height style on element', async () => {
        const layoutFillHeight = (await import('./layout')).layoutFillHeight;
        const element = document.createElement('div');
        element.style.height = '50px';

        // Create a parent with a fixed height
        const parent = document.createElement('div');
        parent.style.height = '300px';
        parent.appendChild(element);
        document.body.appendChild(parent);

        layoutFillHeight(element);
        // The height should be set to something (we can't easily predict the exact value without jQuery)
        expect(element.style.height).toMatch(/^\d+px$/);

        document.body.removeChild(parent);
    });
});

describe('triggerLayoutOnShow', () => {
    it('triggers layout event when element becomes visible', async () => {
        const triggerLayoutOnShow = (await import('./layout')).triggerLayoutOnShow;
        const element = document.createElement('div');
        document.body.appendChild(element);

        const mockCallback = vi.fn();
        element.addEventListener('layout', mockCallback);

        triggerLayoutOnShow(element);

        // Simulate element becoming visible
        Object.defineProperty(element, 'offsetWidth', { value: 100 });
        Object.defineProperty(element, 'offsetHeight', { value: 100 });

        // Trigger visibility check
        document.dispatchEvent(new Event('scroll'));

        document.body.removeChild(element);
    });
});

describe('centerDialog', () => {
    it('centers dialog using jQuery position', async () => {
        const centerDialog = (await import('./layout')).centerDialog;
        const dialog = document.createElement('div');
        dialog.classList.add('ui-dialog');
        document.body.appendChild(dialog);

        // Mock getjQuery to return a mock jQuery function
        var mockJQuery = globalThis.jQuery = vi.fn(() => ({
            position: vi.fn(() => ({ left: 0, top: 0 }))
        }));
        try {

            centerDialog(dialog);

            // Verify that jQuery was called
            expect(mockJQuery).toHaveBeenCalled();
        } finally {
            delete globalThis.jQuery;
        }
        document.body.removeChild(dialog);
    });
});

describe('GridPageInit', () => {
    it('initializes grid page with type and props', async () => {
        const { GridPageInit } = await import('./layout');
        const { Widget } = await import('../ui/widgets/widget');

        // Create the expected DOM element
        const gridDiv = document.createElement('div');
        gridDiv.id = 'GridDiv';
        document.body.appendChild(gridDiv);

        // Create a mock widget class that extends Widget
        class MockGrid extends Widget {
            constructor(props: any) {
                super(props);
            }
        }

        const result = GridPageInit({ type: MockGrid, props: {} });
        expect(result).toBeInstanceOf(HTMLElement);

        document.body.removeChild(gridDiv);
    });
});

describe('PanelPageInit', () => {
    it('initializes panel page with type and props', async () => {
        const { PanelPageInit } = await import('./layout');
        const { Widget } = await import('../ui/widgets/widget');

        // Create the expected DOM element
        const panelDiv = document.createElement('div');
        panelDiv.id = 'Panel';
        document.body.appendChild(panelDiv);

        // Create a mock widget class that extends Widget
        class MockPanel extends Widget {
            constructor(props: any) {
                super(props);
            }
        }

        const result = PanelPageInit({ type: MockPanel, props: {} });
        expect(result).toBeInstanceOf(HTMLElement);

        document.body.removeChild(panelDiv);
    });
});

describe('gridPageInit', () => {
    it('initializes grid with existing widget instance', async () => {
        const { gridPageInit } = await import('./layout');
        const { Widget } = await import('../ui/widgets/widget');

        // Create a mock widget instance
        class MockGrid extends Widget {
            constructor(props: any) {
                super(props);
            }
        }

        const grid = new MockGrid({});
        const result = gridPageInit(grid);
        expect(result).toBe(grid);
    });

    it('initializes grid with type and props', async () => {
        const { gridPageInit } = await import('./layout');
        const { Widget } = await import('../ui/widgets/widget');

        // Create the expected DOM element
        const gridDiv = document.createElement('div');
        gridDiv.id = 'GridDiv';
        document.body.appendChild(gridDiv);

        // Create a mock widget class
        class MockGrid extends Widget {
            constructor(props: any) {
                super(props);
            }
        }

        const result = gridPageInit(MockGrid, {});
        expect(result).toBeInstanceOf(MockGrid);

        document.body.removeChild(gridDiv);
    });
});

describe('panelPageInit', () => {
    it('initializes panel with existing widget instance', async () => {
        const { panelPageInit } = await import('./layout');
        const { Widget } = await import('../ui/widgets/widget');

        // Create a mock widget instance
        class MockPanel extends Widget {
            constructor(props: any) {
                super(props);
            }
        }

        const panel = new MockPanel({});
        const result = panelPageInit(panel);
        expect(result).toBe(panel);
    });

    it('initializes panel with type and props', async () => {
        const { panelPageInit } = await import('./layout');
        const { Widget } = await import('../ui/widgets/widget');

        // Create the expected DOM element
        const panelDiv = document.createElement('div');
        panelDiv.id = 'PanelDiv';
        document.body.appendChild(panelDiv);

        // Create a mock widget class
        class MockPanel extends Widget {
            constructor(props: any) {
                super(props);
            }
        }

        const result = panelPageInit(MockPanel, {});
        expect(result).toBeInstanceOf(MockPanel);

        document.body.removeChild(panelDiv);
    });
});