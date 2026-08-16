import {
    absBox,
    applyColumnWidths,
    applyLegacyHeightOptions,
    createCssRules,
    findStylesheetByUID,
    getInnerWidth,
    getMaxSupportedCssHeight,
    getScrollBarDimensions,
    setStyleProp
} from '../../src/grid/style-utils';

describe('getInnerWidth', () => {
    const style = {
        boxSizing: 'content-box',
        ['border-left-width']: '1px',
        ['border-right-width']: '2px',
        ['padding-left']: '4px',
        ['padding-right']: '8px',
        width: '100px',
        getPropertyValue: (property: string) => style[property]
    };
    const totalHBoxDelta = 1 + 2 + 4 + 8;

    let oldGetComputedStyle: any;
    beforeAll(() => {
        oldGetComputedStyle = window.getComputedStyle;
        window.getComputedStyle = () => style as any;
    });

    afterAll(() => {
        window.getComputedStyle = oldGetComputedStyle;
    });

    it('should return width if box-sizing is not border-box', () => {
        const element = document.createElement('div');
        style.boxSizing = 'content-box';

        expect(getInnerWidth(element)).toBe(100);
    });

    it('should return width - horizontal padding + border if box-sizing is border-box', () => {
        const element = document.createElement('div');
        style.boxSizing = 'border-box';

        expect(getInnerWidth(element)).toBe(100 - totalHBoxDelta);
    });

    it('should return 0 if width is not set', () => {
        const element = document.createElement('div');
        style.width = '';

        expect(getInnerWidth(element)).toBe(0);
    });

    it('should return 0 if width is not a number', () => {
        const element = document.createElement('div');
        style.width = 'foo';

        expect(getInnerWidth(element)).toBe(0);
    });

    it('should return 0 if width is negative', () => {
        const element = document.createElement('div');
        style.width = '-1px';

        expect(getInnerWidth(element)).toBe(0);
    });
});

describe('getMaxSupportedCssHeight', () => {
    it('should return 4000000 for gecko', () => {
        const oldNavigator = window.navigator;
        Object.defineProperty(window, 'navigator', {
            value: {
                userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:95.0) Gecko/20100101 Firefox/95.0',
            },
            configurable: true,
            writable: true
        });

        expect(getMaxSupportedCssHeight(true)).toBe(4000000);

        Object.defineProperty(window, 'navigator', {
            value: oldNavigator,
            configurable: true,
            writable: true
        });
    });

    it('should return 32000000 for other browsers', () => {
        const userAgents = [
            'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36', // Chrome Generic Windows 10
            'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.3', // Chrome Generic macOs
            'Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36', // Chrome Generic Linux
            'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/16.1 Safari/605.1.15', // Safari 16.1 macOs
        ];

        userAgents.forEach(userAgent => {
            const oldNavigator = window.navigator;
            Object.defineProperty(window, 'navigator', {
                value: {
                    userAgent
                },
                configurable: true,
                writable: true
            });

            expect(userAgent + ": " + getMaxSupportedCssHeight(true)).toBe(userAgent + ": " + 32000000); // concat to make it easier to debug

            Object.defineProperty(window, 'navigator', {
                value: oldNavigator,
                configurable: true,
                writable: true
            });
        });
    });
});

describe('absBox', () => {
    let htmlProtoGetSet = ["offsetLeft", "offsetTop", "offsetWidth", "offsetHeight", "offsetParent", "scrollHeight", "scrollWidth", "scrollLeft", "scrollTop", "clientHeight"];
    const oldProperties: PropertyDescriptor[] = []
    beforeAll(() => {
        htmlProtoGetSet.forEach(prop => {
            oldProperties[prop] = Object.getOwnPropertyDescriptor(HTMLElement.prototype, prop); // probably will be undefined due to jsdom

            Object.defineProperty(window.HTMLElement.prototype, prop, {
                configurable: true,
                get: function () {
                    return this[`_${prop}`] ?? 0;
                },
                set(val) {
                    this[`_${prop}`] = val;
                }
            });
        });
    });

    afterAll(() => {
        htmlProtoGetSet.forEach(prop => {
            window.HTMLElement.prototype[prop] = oldProperties[prop];
        });
    });

    it('should return the correct box', () => {
        const element = document.createElement('div');

        element["_offsetTop"] = 11;
        element["_offsetHeight"] = 88;
        element["_offsetLeft"] = 22;
        element["_offsetWidth"] = 44;

        const box = absBox(element);
        expect(box.top).toBe(11);
        expect(box.height).toBe(88);
        expect(box.left).toBe(22);
        expect(box.width).toBe(44);

        expect(box.bottom).toBe(99); // 11 (top) + 88 (height)
        expect(box.right).toBe(66); // 22 (left) + 44 (width)

        expect(box.visible).toBe(true);
    });

    it('should return not visible when the box bottom is smaller than parents scrollTop', () => {
        const element = document.createElement('div');

        element["_offsetTop"] = 10;
        element["_offsetHeight"] = 10;

        const parentElement = document.createElement('div');
        parentElement.appendChild(element);
        parentElement.style.overflowY = 'hidden';
        parentElement["_scrollTop"] = 21;
        parentElement["_scrollHeight"] = 20;

        const box = absBox(element);
        expect(box.visible).toBe(false);
    });

    it('should return visible when the box bottom is bigger to parents scrollTop', () => {
        const element = document.createElement('div');

        element["_offsetTop"] = 10;
        element["_offsetHeight"] = 10;

        const parentElement = document.createElement('div');
        parentElement.appendChild(element);
        parentElement["_scrollTop"] = 9;
        parentElement["_clientHeight"] = 100;

        const box = absBox(element);
        expect(box.visible).toBe(true);
    });

    it('should return not visible when the box top is bigger than parents scrollTop + clientHeight', () => {
        const element = document.createElement('div');

        element["_offsetTop"] = 30;
        element["_offsetHeight"] = 10;

        const parentElement = document.createElement('div');
        parentElement.appendChild(element);
        parentElement.style.overflowY = 'hidden';
        parentElement["_scrollTop"] = 21;
        parentElement["_scrollHeight"] = 20;

        const box = absBox(element);
        expect(box.visible).toBe(false);
    });

    it('should return visible when the box top is smaller than parents scrollTop + clientHeight', () => {
        const element = document.createElement('div');

        element["_offsetTop"] = 20;
        element["_offsetHeight"] = 10;

        const parentElement = document.createElement('div');
        parentElement.appendChild(element);
        parentElement["_scrollTop"] = 21;
        parentElement["_clientHeight"] = 100;

        const box = absBox(element);
        expect(box.visible).toBe(true);
    });

    it('should return not visible when the box right is smaller than parents scrollLeft', () => {
        const element = document.createElement('div');

        element["_offsetLeft"] = 10;
        element["_offsetWidth"] = 10;

        const parentElement = document.createElement('div');
        parentElement.appendChild(element);
        parentElement.style.overflowX = 'hidden';
        parentElement["_scrollLeft"] = 21;
        parentElement["_scrollWidth"] = 20;

        const box = absBox(element);
        expect(box.visible).toBe(false);
    });

    it('should return visible when the box right is bigger to parents scrollLeft', () => {
        const element = document.createElement('div');

        element["_offsetLeft"] = 10;
        element["_offsetWidth"] = 10;

        const parentElement = document.createElement('div');
        parentElement.appendChild(element);
        parentElement["_scrollLeft"] = 9;
        parentElement["_clientWidth"] = 100;

        const box = absBox(element);
        expect(box.visible).toBe(true);
    });

    it('should return not visible when the box left is bigger than parents scrollLeft + clientWidth', () => {
        const element = document.createElement('div');

        element["_offsetLeft"] = 30;
        element["_offsetWidth"] = 10;

        const parentElement = document.createElement('div');
        parentElement.style.overflowX = 'hidden';
        parentElement.appendChild(element);
        parentElement["_scrollLeft"] = 21;
        parentElement["_scrollWidth"] = 20;

        const box = absBox(element);
        expect(box.visible).toBe(false);
    });

    it('should return visible when the box left is smaller than parents scrollLeft + clientWidth', () => {
        const element = document.createElement('div');

        element["_offsetLeft"] = 20;
        element["_offsetWidth"] = 10;

        const parentElement = document.createElement('div');
        parentElement.style.overflowX = 'hidden';
        parentElement.appendChild(element);
        parentElement["_scrollLeft"] = 21;
        parentElement["_clientWidth"] = 100;

        const box = absBox(element);
        expect(box.visible).toBe(true);
    });

    it('should return the correct box when the parent element is scrolled', () => {
        const element = document.createElement('div');

        element["_offsetTop"] = 10;
        element["_offsetHeight"] = 20;
        element["_offsetLeft"] = 40;
        element["_offsetWidth"] = 80;

        const parentElement = document.createElement('div');
        parentElement.appendChild(element);
        parentElement["_scrollTop"] = 10;
        parentElement["_scrollLeft"] = 20;
        parentElement["_clientHeight"] = 80;
        parentElement["_clientWidth"] = 160;

        const box = absBox(element);
        expect(box.top).toBe(0); // 10 (element offsetTop) - 10 (parent scrollTop)
        expect(box.height).toBe(20);
        expect(box.left).toBe(20); // 40 (element offsetLeft) - 20 (parent scrollLeft)
        expect(box.width).toBe(80);

        expect(box.bottom).toBe(20);
        expect(box.right).toBe(100);

        expect(box.visible).toBe(true);
    });

    it('should return the correct box when the parent element is scrolled and we have an offsetParent', () => {
        const element = document.createElement('div');

        element["_offsetTop"] = 10;
        element["_offsetHeight"] = 20;
        element["_offsetLeft"] = 40;
        element["_offsetWidth"] = 80;

        const parentElement = document.createElement('div');
        parentElement.appendChild(element);
        parentElement["_scrollTop"] = 10;
        parentElement["_scrollLeft"] = 20;
        parentElement["_clientHeight"] = 80;
        parentElement["_clientWidth"] = 160;

        const offsetParent = document.createElement('td');
        offsetParent.appendChild(parentElement);
        element["_offsetParent"] = offsetParent;
        offsetParent["_scrollTop"] = 3;
        offsetParent["_scrollLeft"] = 3;
        offsetParent["_offsetTop"] = 15;
        offsetParent["_offsetLeft"] = 25;
        offsetParent["_clientHeight"] = 45;
        offsetParent["_clientWidth"] = 85;

        const box = absBox(element);
        expect(box.top).toBe(12); // 10 (element offsetTop) - 10 (parent offsetTop) - 3 (offsetParent scrollTop) + 15 (offsetParent offsetTop)
        expect(box.height).toBe(20);
        expect(box.left).toBe(42); // 40 (element offsetLeft) - 20 (parent offsetLeft) - 3 (offsetParent scrollLeft) + 25 (offsetParent offsetLeft)
        expect(box.width).toBe(80);

        expect(box.bottom).toBe(32);
        expect(box.right).toBe(122);

        expect(box.visible).toBe(true);
    });
});

describe('getInnerWidth - null element', () => {
    it('returns 0 for a null element', () => {
        expect(getInnerWidth(null)).toBe(0);
    });
});

describe('getScrollBarDimensions', () => {
    it('computes scrollbar dimensions from a probe element', () => {
        const appendSpy = vi.spyOn(document.body, "appendChild");
        const probe = document.createElement("div");
        Object.defineProperty(probe, "offsetWidth", { get: () => 100 });
        Object.defineProperty(probe, "clientWidth", { get: () => 85 });
        Object.defineProperty(probe, "offsetHeight", { get: () => 100 });
        Object.defineProperty(probe, "clientHeight", { get: () => 90 });
        appendSpy.mockImplementation(() => probe as any);

        const dims = getScrollBarDimensions(true);
        expect(dims.width).toBe(15);   // 100 - 85
        expect(dims.height).toBe(10);  // 100 - 90 (offsetHeight, bug fix)

        appendSpy.mockRestore();
    });

    it('caches the dimensions between calls', () => {
        const appendSpy = vi.spyOn(document.body, "appendChild");
        getScrollBarDimensions(true);
        appendSpy.mockClear();
        getScrollBarDimensions();
        expect(appendSpy).not.toHaveBeenCalled();
        appendSpy.mockRestore();
    });
});

describe('setStyleProp', () => {
    it('sets the property when it differs', () => {
        const el = document.createElement("div");
        setStyleProp(el.style, "--x", "1px");
        expect(el.style.getPropertyValue("--x")).toBe("1px");
    });

    it('does not set the property when it is already the same', () => {
        const el = document.createElement("div");
        el.style.setProperty("--x", "1px");
        const spy = vi.spyOn(el.style, "setProperty");
        setStyleProp(el.style, "--x", "1px");
        expect(spy).not.toHaveBeenCalled();
    });
});

describe('createCssRules', () => {
    afterEach(() => {
        document.head.querySelectorAll("style[data-uid]").forEach(n => n.remove());
    });

    it('uses css vars when useCssVars is true', () => {
        const container = document.createElement("div");
        const result = createCssRules({
            opt: { useCssVars: true, rowHeight: 25 },
            cellHeightDiff: 2,
            colCount: 3,
            container,
            scrollDims: { width: 10, height: 12 },
            uid: "u1"
        });
        expect(container.classList.contains("sleek-vars")).toBe(true);
        expect(container.style.getPropertyValue("--sg-row-height")).toBe("25px");
        expect(container.style.getPropertyValue("--sg-cell-height")).toBe("23px");
        expect(container.style.getPropertyValue("--sg-scrollbar-w")).toBe("10px");
        expect(container.style.getPropertyValue("--sg-scrollbar-h")).toBe("12px");
        expect(result).toBeUndefined();
    });

    it('uses css vars when colCount is within a numeric useCssVars limit', () => {
        const container = document.createElement("div");
        createCssRules({ opt: { useCssVars: 5, rowHeight: 25 }, cellHeightDiff: 0, colCount: 3, container, scrollDims: { width: 0, height: 0 }, uid: "u" });
        expect(container.classList.contains("sleek-vars")).toBe(true);
    });

    it('does not use css vars when colCount exceeds a numeric limit', () => {
        const container = document.createElement("div");
        createCssRules({ opt: { useCssVars: 5, rowHeight: 25 }, cellHeightDiff: 0, colCount: 10, container, scrollDims: { width: 0, height: 0 }, uid: "u" });
        expect(container.classList.contains("sleek-vars")).toBe(false);
    });

    it('creates a style node with rules when not using css vars', () => {
        const container = document.createElement("div");
        const result = createCssRules({ opt: { rowHeight: 25 }, cellHeightDiff: 2, colCount: 2, container, scrollDims: { width: 10, height: 12 }, uid: "u2" });
        expect(result.styleNode).toBeTruthy();
        expect(result.styleNode.dataset.uid).toBe("u2");
        expect(document.head.contains(result.styleNode)).toBe(true);
        const text = result.styleNode.textContent;
        expect(text).toContain("--sg-row-height: 25px");
        expect(text).toContain("--sg-cell-height: 23px");
        expect(text).toContain(".u2 .slick-cell { height: 23px; }");
        expect(text).toContain(".u2 .slick-row { height: 25px; }");
        expect(text).toContain(".u2 .l0 { }");
        expect(text).toContain(".u2 .r1 { }");
    });

    it('applies the style nonce when provided', () => {
        const container = document.createElement("div");
        const result = createCssRules({ opt: { rowHeight: 25, styleNonce: "abc" }, cellHeightDiff: 0, colCount: 1, container, scrollDims: { width: 0, height: 0 }, uid: "u3" });
        expect(result.styleNode.nonce).toBe("abc");
    });

    it('reads the nonce from a csp-nonce meta tag', () => {
        const meta = document.createElement("meta");
        meta.name = "csp-nonce";
        meta.content = "meta-nonce";
        document.head.appendChild(meta);
        try {
            const container = document.createElement("div");
            const result = createCssRules({ opt: { rowHeight: 25 }, cellHeightDiff: 0, colCount: 1, container, scrollDims: { width: 0, height: 0 }, uid: "u4" });
            expect(result.styleNode.nonce).toBe("meta-nonce");
        } finally {
            meta.remove();
        }
    });
});

describe('findStylesheetByUID', () => {
    afterEach(() => {
        document.head.querySelectorAll("style[data-uid]").forEach(n => n.remove());
    });

    it('finds the stylesheet and column rules by uid', () => {
        const container = document.createElement("div");
        const { styleNode } = createCssRules({ opt: { rowHeight: 25 }, cellHeightDiff: 0, colCount: 2, container, scrollDims: { width: 0, height: 0 }, uid: "u5" });
        const res = findStylesheetByUID("u5", styleNode);
        expect(res.stylesheet).toBeTruthy();
        expect(res.colCssRulesL[0]).toBeTruthy();
        expect(res.colCssRulesL[1]).toBeTruthy();
        expect(res.colCssRulesR[0]).toBeTruthy();
        expect(res.colCssRulesR[1]).toBeTruthy();
        expect(res.varRule).toBeNull();
    });

    it('throws when the stylesheet cannot be found', () => {
        const styleNode = document.createElement("style");
        expect(() => findStylesheetByUID("missing", styleNode)).toThrow("Cannot find stylesheet");
    });
});

describe('applyColumnWidths', () => {
    const refs = {
        pinnedStartLast: -1,
        pinnedEndFirst: Infinity,
        start: { canvasWidth: 100 },
        main: { canvasWidth: 500 },
        end: { canvasWidth: 100 }
    } as any;

    it('sets css var widths when no css rules are provided', () => {
        const container = document.createElement("div");
        const cols = [{ id: "a", width: 100 }, { id: "b", width: 200 }];
        applyColumnWidths({ cols, container, opts: {}, refs });
        expect(container.style.getPropertyValue("--l0")).toBe("0px");
        expect(container.style.getPropertyValue("--r0")).toBe("400px"); // 500 - 0 - 100
        expect(container.style.getPropertyValue("--l1")).toBe("100px");
        expect(container.style.getPropertyValue("--r1")).toBe("200px"); // 500 - 100 - 200
    });

    it('sets rule styles when css rules are provided', () => {
        const container = document.createElement("div");
        const cols = [{ id: "a", width: 100 }];
        const ruleL = { style: { setProperty: vi.fn() } } as any;
        const ruleR = { style: { setProperty: vi.fn() } } as any;
        applyColumnWidths({ cols, container, opts: {}, refs, cssColRulesL: { 0: ruleL }, cssColRulesR: { 0: ruleR } });
        expect(ruleL.style.setProperty).toHaveBeenCalledWith("left", "0px");
        expect(ruleL.style.setProperty).toHaveBeenCalledWith("right", null);
        expect(ruleR.style.setProperty).toHaveBeenCalledWith("right", "400px");
        expect(ruleR.style.setProperty).toHaveBeenCalledWith("left", null);
    });

    it('uses right/left and --r/--l vars in rtl mode', () => {
        const container = document.createElement("div");
        const cols = [{ id: "a", width: 100 }];
        applyColumnWidths({ cols, container, opts: { rtl: true }, refs });
        expect(container.style.getPropertyValue("--r0")).toBe("0px");
        expect(container.style.getPropertyValue("--l0")).toBe("400px");
    });

    it('resets x at band boundaries', () => {
        const container = document.createElement("div");
        const cols = [{ id: "a", width: 100 }, { id: "b", width: 50 }, { id: "c", width: 30 }];
        const refs2 = { pinnedStartLast: 0, pinnedEndFirst: 2, start: { canvasWidth: 100 }, main: { canvasWidth: 500 }, end: { canvasWidth: 100 } } as any;
        applyColumnWidths({ cols, container, opts: {}, refs: refs2 });
        expect(container.style.getPropertyValue("--l0")).toBe("0px");
        expect(container.style.getPropertyValue("--r0")).toBe("0px"); // 100 - 0 - 100
        expect(container.style.getPropertyValue("--l1")).toBe("0px");
        expect(container.style.getPropertyValue("--r1")).toBe("450px"); // 500 - 0 - 50
        expect(container.style.getPropertyValue("--l2")).toBe("0px");
        expect(container.style.getPropertyValue("--r2")).toBe("70px"); // 100 - 0 - 30
    });
});

describe('applyLegacyHeightOptions', () => {
    function makeRefs() {
        const topPanel = document.createElement("div");
        const hrc1 = document.createElement("div");
        const hrc2 = document.createElement("div");
        const frc1 = document.createElement("div");
        const refs = {
            topPanel,
            start: { headerRowCols: hrc1, footerRowCols: frc1 },
            main: { headerRowCols: hrc2, footerRowCols: frc1 },
            end: { headerRowCols: null, footerRowCols: null }
        } as any;
        return { topPanel, hrc1, hrc2, frc1, refs };
    }

    it('applies top panel, grouping panel, header row and footer row heights', () => {
        const { topPanel, hrc1, hrc2, frc1, refs } = makeRefs();
        const groupingPanel = document.createElement("div");
        applyLegacyHeightOptions({ groupingPanel, opt: { topPanelHeight: 30, groupingPanelHeight: 40, headerRowHeight: 25, footerRowHeight: 20 }, refs });
        expect(topPanel.style.height).toBe("30px");
        expect(groupingPanel.style.height).toBe("40px");
        expect(hrc1.style.height).toBe("25px");
        expect(hrc2.style.height).toBe("25px");
        expect(frc1.style.height).toBe("20px");
    });

    it('does nothing when the options are not set', () => {
        const { topPanel, refs } = makeRefs();
        const groupingPanel = document.createElement("div");
        applyLegacyHeightOptions({ groupingPanel, opt: {}, refs });
        expect(topPanel.style.height).toBe("");
        expect(groupingPanel.style.height).toBe("");
    });
});
