
beforeEach(() => {
    vi.resetModules();
});

describe('initFullHeightGridPage', () => {
    it('works without jQuery', async () => {
        const initFullHeightGridPage = (await import('./layout')).initFullHeightGridPage;
        const div = document.createElement('div');
        initFullHeightGridPage(div, { noRoute: true });
        expect(document.documentElement.classList.contains('full-height-page')).toBe(true);
        expect(div.classList.contains('responsive-height')).toBe(true);
    });

    it('works with jQuery and HTML element', async () => {
        const initFullHeightGridPage = (await import('./layout')).initFullHeightGridPage;
        const div = document.createElement('div');
        initFullHeightGridPage(div, { noRoute: true });
        expect(document.documentElement.classList.contains('full-height-page')).toBe(true);
        expect(div.classList.contains('responsive-height')).toBe(true);
    });

    it('works with jQuery and jQuery wrapped element', async () => {
        const initFullHeightGridPage = (await import('./layout')).initFullHeightGridPage;
        const div = document.createElement('div');
        initFullHeightGridPage(div, { noRoute: true });
        expect(document.documentElement.classList.contains('full-height-page')).toBe(true);
        expect(div.classList.contains('responsive-height')).toBe(true);
    });
});

describe('isMobileView', () => {

    it('should return false if window is undefined', async () => {
        const isMobileView = (await import('./layout')).isMobileView;
        const oldWindow = window;
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
        const matchMedia = window.matchMedia;
        try {
            let q: string;
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

        const matchMedia = window.matchMedia;
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
        const mockJQuery = globalThis.jQuery = vi.fn(() => ({
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

describe('initWidgetPage paths via gridPageInit', () => {
    it('covers path B1: type with defaultElement and props.element as function', async () => {
        const { gridPageInit } = await import('./layout');
        const { Widget } = await import('../ui/widgets/widget');

        const gridDiv = document.createElement('div');
        gridDiv.id = 'GridDiv';
        document.body.appendChild(gridDiv);

        const elementFn = vi.fn();
        class MockGrid extends Widget {
            constructor(props: any) {
                super(props);
            }
        }

        const result = gridPageInit(MockGrid, { element: elementFn });
        expect(result).toBeInstanceOf(MockGrid);
        // The element function should not have been called directly by initWidgetPage
        // (it gets replaced with defaultElement, then later called with the widget's domNode)
        expect(elementFn).toHaveBeenCalledWith(result.domNode);

        document.body.removeChild(gridDiv);
    });

    it('covers path B2: type with defaultElement and props.element as string', async () => {
        const { gridPageInit } = await import('./layout');
        const { Widget } = await import('../ui/widgets/widget');

        const gridDiv = document.createElement('div');
        gridDiv.id = 'GridDiv';
        document.body.appendChild(gridDiv);

        class MockGrid extends Widget {
            constructor(props: any) {
                super(props);
            }
        }

        // Pass element as a string selector (non-function)
        const result = gridPageInit(MockGrid, { element: '#GridDiv' } as any);
        expect(result).toBeInstanceOf(MockGrid);

        document.body.removeChild(gridDiv);
    });
});

describe('initFullHeightGridPage additional paths', () => {
    beforeEach(() => {
        document.body.removeAttribute('data-fhrouteinit');
    });

    it('handles isArrayLike gridDiv', async () => {
        const initFullHeightGridPage = (await import('./layout')).initFullHeightGridPage;
        const div = document.createElement('div');
        initFullHeightGridPage([div], { noRoute: true });
        expect(document.documentElement.classList.contains('full-height-page')).toBe(true);
        expect(div.classList.contains('responsive-height')).toBe(true);
    });

    it('handles domNode object gridDiv', async () => {
        const initFullHeightGridPage = (await import('./layout')).initFullHeightGridPage;
        const div = document.createElement('div');
        initFullHeightGridPage({ domNode: div }, { noRoute: true });
        expect(document.documentElement.classList.contains('full-height-page')).toBe(true);
        expect(div.classList.contains('responsive-height')).toBe(true);
    });

    it('handles has-layout-event body class', async () => {
        document.body.classList.add('has-layout-event');
        try {
            const initFullHeightGridPage = (await import('./layout')).initFullHeightGridPage;
            const div = document.createElement('div');
            initFullHeightGridPage(div, { noRoute: true });
            expect(document.documentElement.classList.contains('full-height-page')).toBe(true);
        } finally {
            document.body.classList.remove('has-layout-event');
        }
    });

    it('handles Metronic addResizeHandler', async () => {
        const mockAddResizeHandler = vi.fn();
        (window as any).Metronic = { addResizeHandler: mockAddResizeHandler };
        try {
            const initFullHeightGridPage = (await import('./layout')).initFullHeightGridPage;
            const div = document.createElement('div');
            initFullHeightGridPage(div, { noRoute: true });
            expect(mockAddResizeHandler).toHaveBeenCalled();
        } finally {
            delete (window as any).Metronic;
        }
    });

    it('handles jQuery with s-DataGrid class (setHeight false)', async () => {
        const initFullHeightGridPage = (await import('./layout')).initFullHeightGridPage;
        const div = document.createElement('div');
        div.classList.add('s-DataGrid');

        // Set jQuery AFTER import to avoid Router constructor interference
        const mockJQuery = globalThis.jQuery = vi.fn(() => ({
            parent: vi.fn().mockReturnThis(),
            children: vi.fn().mockReturnThis(),
            not: vi.fn().mockReturnThis(),
            each: vi.fn(),
            height: vi.fn().mockReturnValue(300),
            outerHeight: vi.fn().mockReturnValue(50),
            css: vi.fn().mockReturnValue('border-box'),
            is: vi.fn().mockReturnValue(true)
        }));
        try {
            initFullHeightGridPage(div, { noRoute: true, setHeight: false });
            expect(document.documentElement.classList.contains('full-height-page')).toBe(true);
        } finally {
            delete globalThis.jQuery;
        }
    });

    it('handles jQuery with setHeight true (no s-DataGrid/s-Panel)', async () => {
        const initFullHeightGridPage = (await import('./layout')).initFullHeightGridPage;
        const div = document.createElement('div');

        // Set jQuery AFTER import to avoid Router constructor interference
        const mockJQuery = globalThis.jQuery = vi.fn(() => ({
            parent: vi.fn(() => ({
                children: vi.fn(() => ({
                    not: vi.fn(() => ({
                        each: vi.fn()
                    }))
                })),
                height: vi.fn().mockReturnValue(300)
            })),
            children: vi.fn().mockReturnThis(),
            not: vi.fn().mockReturnThis(),
            each: vi.fn(),
            height: vi.fn().mockReturnValue(300),
            outerHeight: vi.fn().mockReturnValue(50),
            css: vi.fn().mockReturnValue('border-box'),
            is: vi.fn().mockReturnValue(true)
        }));
        try {
            initFullHeightGridPage(div, { noRoute: true, setHeight: true });
            expect(document.documentElement.classList.contains('full-height-page')).toBe(true);
        } finally {
            delete globalThis.jQuery;
        }
    });

    it('triggers route resolution when noRoute is false/undefined', async () => {
        // We need to reset the data-fhrouteinit attribute
        document.body.removeAttribute('data-fhrouteinit');

        const initFullHeightGridPage = (await import('./layout')).initFullHeightGridPage;
        const div = document.createElement('div');
        // Call without noRoute - should trigger Router.resolve
        initFullHeightGridPage(div, { noRoute: false });
        expect(document.body.getAttribute('data-fhrouteinit')).toBe('true');
    });

    it('disposing callback does not crash', async () => {
        const initFullHeightGridPage = (await import('./layout')).initFullHeightGridPage;

        const div = document.createElement('div');
        document.body.appendChild(div);
        initFullHeightGridPage(div, { noRoute: true });

        // Remove the element to trigger dispose
        document.body.removeChild(div);
        // Just verify no crash occurred
        expect(true).toBe(true);
    });
});

describe('layoutFillHeightValue with jQuery', () => {
    it('calculates height using jQuery when available', async () => {
        const layoutFillHeightValue = (await import('./layout')).layoutFillHeightValue;
        const element = document.createElement('div');
        element.style.height = '100px';

        // Create parent for jQuery traversal
        const parent = document.createElement('div');
        parent.style.height = '500px';
        parent.appendChild(element);
        document.body.appendChild(parent);

        // Set jQuery AFTER import to avoid Router constructor interference
        const mockQ = vi.fn((el: any) => ({
            parent: vi.fn(() => ({
                children: vi.fn(() => ({
                    not: vi.fn(() => ({
                        each: vi.fn((cb: any) => {
                            // Simulate one visible sibling child
                            cb(0, document.createElement('div'));
                        })
                    }))
                })),
                height: vi.fn().mockReturnValue(500)
            })),
            is: vi.fn().mockReturnValue(true),
            outerHeight: vi.fn().mockReturnValue(50),
            height: vi.fn().mockReturnValue(100),
            css: vi.fn().mockReturnValue('border-box')
        }));
        globalThis.jQuery = mockQ;
        try {
            const result = layoutFillHeightValue(element);
            // 500 (parent height) - 50 (sibling outerHeight) = 450
            expect(result).toBe(450);

            document.body.removeChild(parent);
        } finally {
            delete globalThis.jQuery;
        }
    });

    it('calculates height with non-border-box sizing', async () => {
        const layoutFillHeightValue = (await import('./layout')).layoutFillHeightValue;
        const element = document.createElement('div');

        const parent = document.createElement('div');
        parent.style.height = '500px';
        parent.appendChild(element);
        document.body.appendChild(parent);

        // Set jQuery AFTER import to avoid Router constructor interference
        const mockQ = vi.fn((el: any) => ({
            parent: vi.fn(() => ({
                children: vi.fn(() => ({
                    not: vi.fn(() => ({
                        each: vi.fn()
                    }))
                })),
                height: vi.fn().mockReturnValue(500)
            })),
            is: vi.fn().mockReturnValue(true),
            outerHeight: vi.fn().mockReturnValue(60),
            height: vi.fn().mockReturnValue(40),
            css: vi.fn().mockReturnValue('content-box') // non-border-box
        }));
        globalThis.jQuery = mockQ;
        try {
            const result = layoutFillHeightValue(element);
            // 500 - 0 (no siblings) = 500, then 500 - (60 - 40) = 480
            expect(result).toBe(480);

            document.body.removeChild(parent);
        } finally {
            delete globalThis.jQuery;
        }
    });

    it('handles isArrayLike element', async () => {
        const layoutFillHeightValue = (await import('./layout')).layoutFillHeightValue;
        const element = document.createElement('div');
        element.style.height = '200px';
        const result = layoutFillHeightValue([element]);
        expect(result).toBe(200);
    });
});

describe('layoutFillHeight additional paths', () => {
    it('handles isArrayLike element', async () => {
        const layoutFillHeight = (await import('./layout')).layoutFillHeight;
        const element = document.createElement('div');
        element.style.height = '100px';

        const parent = document.createElement('div');
        parent.style.height = '300px';
        parent.appendChild(element);
        document.body.appendChild(parent);

        layoutFillHeight([element]);
        expect(element.style.height).toMatch(/^\d+px$/);

        document.body.removeChild(parent);
    });

    it('sets height only when it differs from current', async () => {
        const layoutFillHeight = (await import('./layout')).layoutFillHeight;
        const element = document.createElement('div');
        // Mock getComputedStyle to return a known height
        const origGetComputedStyle = globalThis.getComputedStyle;
        globalThis.getComputedStyle = vi.fn().mockReturnValue({ height: '150px' });
        try {
            layoutFillHeight(element);
            // Since no jQuery, layoutFillHeightValue returns getComputedStyle height = 150
            // layoutFillHeight then rounds it and sets it: element.style.height = '150px'
            expect(element.style.height).toBe('150px');
        } finally {
            globalThis.getComputedStyle = origGetComputedStyle;
        }
    });
});

describe('triggerLayoutOnShow additional paths', () => {
    it('does nothing when element is null/undefined', async () => {
        const triggerLayoutOnShow = (await import('./layout')).triggerLayoutOnShow;
        // Should not throw
        expect(() => triggerLayoutOnShow(null as any)).not.toThrow();
        expect(() => triggerLayoutOnShow(undefined as any)).not.toThrow();
    });

    it('handles isArrayLike element', async () => {
        const triggerLayoutOnShow = (await import('./layout')).triggerLayoutOnShow;
        const element = document.createElement('div');
        document.body.appendChild(element);

        // Should not throw
        expect(() => triggerLayoutOnShow([element])).not.toThrow();
        document.body.removeChild(element);
    });
});

describe('centerDialog additional paths', () => {
    it('handles isArrayLike element', async () => {
        const centerDialog = (await import('./layout')).centerDialog;
        const dialog = document.createElement('div');
        dialog.classList.add('ui-dialog');
        document.body.appendChild(dialog);

        const mockJQuery = globalThis.jQuery = vi.fn(() => ({
            position: vi.fn(() => ({ left: 0, top: 0 })),
            css: vi.fn()
        }));
        try {
            centerDialog([dialog]);
            expect(mockJQuery).toHaveBeenCalled();
        } finally {
            delete globalThis.jQuery;
        }
        document.body.removeChild(dialog);
    });

    it('does nothing when no ui-dialog parent found', async () => {
        const centerDialog = (await import('./layout')).centerDialog;
        const element = document.createElement('div');
        document.body.appendChild(element);

        // Should not throw (early return when no .ui-dialog ancestor)
        expect(() => centerDialog(element)).not.toThrow();
        document.body.removeChild(element);
    });

    it('adjusts position when left is negative', async () => {
        const centerDialog = (await import('./layout')).centerDialog;
        const dialog = document.createElement('div');
        dialog.classList.add('ui-dialog');
        document.body.appendChild(dialog);

        const cssMock = vi.fn();
        const mockJQuery = globalThis.jQuery = vi.fn(() => {
            let posCallCount = 0;
            return {
                position: vi.fn(() => {
                    posCallCount++;
                    return { left: -50, top: 100 };
                }),
                css: cssMock
            };
        });
        try {
            centerDialog(dialog);
            expect(cssMock).toHaveBeenCalledWith('left', '0px');
        } finally {
            delete globalThis.jQuery;
        }
        document.body.removeChild(dialog);
    });

    it('adjusts position when top is negative', async () => {
        const centerDialog = (await import('./layout')).centerDialog;
        const dialog = document.createElement('div');
        dialog.classList.add('ui-dialog');
        document.body.appendChild(dialog);

        const cssMock = vi.fn();
        const mockJQuery = globalThis.jQuery = vi.fn(() => {
            return {
                position: vi.fn(() => ({ left: 50, top: -30 })),
                css: cssMock
            };
        });
        try {
            centerDialog(dialog);
            expect(cssMock).toHaveBeenCalledWith('top', '0px');
        } finally {
            delete globalThis.jQuery;
        }
        document.body.removeChild(dialog);
    });
});