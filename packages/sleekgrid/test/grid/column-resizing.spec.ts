import { autosizeColumns, setupColumnResize } from "../../src/grid/column-resizing";

function makeHeaderBand(colCount: number): HTMLElement {
    const band = document.createElement("div");
    for (let i = 0; i < colCount; i++) {
        const col = document.createElement("div");
        col.className = "slick-header-column";
        col.dataset.c = String(i);
        band.appendChild(col);
    }
    return band;
}

function makeCols(count: number, overrides: any = {}) {
    const cols: any[] = [];
    for (let i = 0; i < count; i++) {
        cols.push({ id: "c" + i, width: 100, resizable: true, ...overrides });
    }
    return cols;
}

function fireMouse(type: string, target: EventTarget, pageX: number) {
    const ev = new MouseEvent(type, { bubbles: true, cancelable: true });
    Object.defineProperty(ev, "pageX", { value: pageX, configurable: true });
    target.dispatchEvent(ev);
    return ev;
}

function mockColumnWidths(band: HTMLElement, width: () => number) {
    band.querySelectorAll(".slick-header-column").forEach(el => {
        Object.defineProperty(el, "offsetWidth", { configurable: true, get: width });
    });
}

describe('setupColumnResize', () => {
    function setup(overrides: any = {}) {
        const container = document.createElement("div");
        const band = makeHeaderBand(2);
        container.appendChild(band);
        const cols = overrides.cols ?? makeCols(2);
        const disposer = new AbortController();
        const colResizing = vi.fn();
        const colResized = vi.fn();
        const removeNode = vi.fn();
        setupColumnResize({
            absoluteColMinWidth: 30,
            container,
            cols,
            colResizing,
            colResized,
            disposer,
            headerColsElements: [band],
            getEditorLock: (() => ({ commitCurrentEdit: () => true })) as any,
            options: { forceFitColumns: false, rtl: false },
            removeNode,
            ...overrides
        });
        return { container, band, cols, disposer, colResizing, colResized, removeNode };
    }

    it('adds resize handles to resizable columns', () => {
        const { band } = setup();
        expect(band.querySelectorAll(".slick-resizable-handle").length).toBe(2);
    });

    it('performs a drag resize flow', () => {
        const { band, cols, colResizing, colResized, container } = setup();
        const handle = band.querySelector(".slick-resizable-handle") as HTMLElement;

        fireMouse("mousedown", handle, 100);
        expect(container.classList.contains("slick-column-resizing")).toBe(true);

        fireMouse("mousemove", document, 150);
        expect(colResizing).toHaveBeenCalledWith(0);
        expect(cols[0].width).toBe(50); // previousWidth(0) + dist(50)

        fireMouse("mouseup", document, 150);
        expect(colResized).toHaveBeenCalledWith(false, 0);
        expect(container.classList.contains("slick-column-resizing")).toBe(false);
    });

    it('returns early when no columns are resizable', () => {
        const container = document.createElement("div");
        const band = makeHeaderBand(2);
        container.appendChild(band);
        const cols = makeCols(2, { resizable: false });
        setupColumnResize({
            absoluteColMinWidth: 30, container, cols, colResizing: vi.fn(), colResized: vi.fn(),
            disposer: new AbortController(), headerColsElements: [band],
            getEditorLock: (() => ({ commitCurrentEdit: () => true })) as any,
            options: { forceFitColumns: false, rtl: false }, removeNode: vi.fn()
        });
        expect(band.querySelectorAll(".slick-resizable-handle").length).toBe(0);
    });

    it('does not add a handle to the last column when forceFitColumns is set', () => {
        const container = document.createElement("div");
        const band = makeHeaderBand(2);
        container.appendChild(band);
        const cols = makeCols(2);
        setupColumnResize({
            absoluteColMinWidth: 30, container, cols, colResizing: vi.fn(), colResized: vi.fn(),
            disposer: new AbortController(), headerColsElements: [band],
            getEditorLock: (() => ({ commitCurrentEdit: () => true })) as any,
            options: { forceFitColumns: true, rtl: false }, removeNode: vi.fn()
        });
        expect(band.querySelectorAll(".slick-resizable-handle").length).toBe(1);
    });

    it('does not start resizing when the editor lock cannot commit', () => {
        const container = document.createElement("div");
        const band = makeHeaderBand(1);
        container.appendChild(band);
        const cols = makeCols(1);
        setupColumnResize({
            absoluteColMinWidth: 30, container, cols, colResizing: vi.fn(), colResized: vi.fn(),
            disposer: new AbortController(), headerColsElements: [band],
            getEditorLock: (() => ({ commitCurrentEdit: () => false })) as any,
            options: { forceFitColumns: false, rtl: false }, removeNode: vi.fn()
        });
        const handle = band.querySelector(".slick-resizable-handle") as HTMLElement;
        fireMouse("mousedown", handle, 100);
        expect(container.classList.contains("slick-column-resizing")).toBe(false);
    });

    it('removes existing resize handles when re-setup', () => {
        const { band, removeNode } = setup();
        const container = document.createElement("div");
        container.appendChild(band);
        const cols = makeCols(2);
        setupColumnResize({
            absoluteColMinWidth: 30, container, cols, colResizing: vi.fn(), colResized: vi.fn(),
            disposer: new AbortController(), headerColsElements: [band],
            getEditorLock: (() => ({ commitCurrentEdit: () => true })) as any,
            options: { forceFitColumns: false, rtl: false }, removeNode
        });
        expect(removeNode).toHaveBeenCalled();
    });

    it('shrinks a column when dragging left', () => {
        const { band, cols, colResizing, colResized } = setup({ cols: makeCols(2, { minWidth: 30 }) });
        mockColumnWidths(band, () => 100);
        const handle = band.querySelector(".slick-resizable-handle") as HTMLElement;

        fireMouse("mousedown", handle, 150);
        fireMouse("mousemove", document, 50); // dist = -70
        expect(colResizing).toHaveBeenCalledWith(0);
        expect(cols[0].width).toBe(30); // 100 - 70, clamped at minWidth 30

        fireMouse("mouseup", document, 50);
        expect(colResized).toHaveBeenCalledWith(false, 0);
    });

    it('applies forceFit stretch to the columns to the right', () => {
        const { band, cols } = setup({ options: { forceFitColumns: true, rtl: false } });
        mockColumnWidths(band, () => 100);
        const handle = band.querySelector(".slick-resizable-handle") as HTMLElement;

        fireMouse("mousedown", handle, 100);
        fireMouse("mousemove", document, 150); // dist = 50
        expect(cols[0].width).toBe(150);
        expect(cols[1].width).toBe(50); // forceFit shrinks the right neighbor

        fireMouse("mouseup", document, 150);
    });

    it('negates the drag distance in rtl mode', () => {
        const { band, cols } = setup({ options: { forceFitColumns: false, rtl: true } });
        mockColumnWidths(band, () => 100);
        const handle = band.querySelector(".slick-resizable-handle") as HTMLElement;

        fireMouse("mousedown", handle, 100);
        fireMouse("mousemove", document, 120); // dist = 20, negated to -20
        expect(cols[0].width).toBe(80);

        fireMouse("mouseup", document, 120);
    });

    it('signals invalidateAll when a rerenderOnResize column changes during resize', () => {
        const { band, colResized } = setup({ cols: makeCols(2, { rerenderOnResize: true }) });
        let width = 100;
        mockColumnWidths(band, () => width);
        const handle = band.querySelector(".slick-resizable-handle") as HTMLElement;

        fireMouse("mousedown", handle, 100);
        width = 120; // the header element grew during the drag
        fireMouse("mouseup", document, 100);

        expect(colResized).toHaveBeenCalledWith(true, 0);
    });

    it('signals no invalidateAll when nothing changed during resize', () => {
        const { band, colResized } = setup({ cols: makeCols(2, { rerenderOnResize: true }) });
        mockColumnWidths(band, () => 100);
        const handle = band.querySelector(".slick-resizable-handle") as HTMLElement;

        fireMouse("mousedown", handle, 100);
        fireMouse("mouseup", document, 100);

        expect(colResized).toHaveBeenCalledWith(false, 0);
    });

    it('ignores mousemove events that happen outside a drag', () => {
        const { colResizing } = setup();
        fireMouse("mousemove", document, 100);
        expect(colResizing).not.toHaveBeenCalled();
    });

    it('ignores mousemove events with a non-numeric distance', () => {
        const { band, colResizing } = setup();
        mockColumnWidths(band, () => 100);
        const handle = band.querySelector(".slick-resizable-handle") as HTMLElement;

        fireMouse("mousedown", handle, 100);
        const ev = new MouseEvent("mousemove", { bubbles: true, cancelable: true });
        Object.defineProperty(ev, "pageX", { value: NaN, configurable: true });
        document.dispatchEvent(ev);

        expect(colResizing).not.toHaveBeenCalled();
    });

    it('accounts for a maxWidth on the right columns when force-fitting', () => {
        const { band, cols } = setup({
            options: { forceFitColumns: true, rtl: false },
            cols: makeCols(2, { maxWidth: 120 })
        });
        mockColumnWidths(band, () => 100);
        const handle = band.querySelector(".slick-resizable-handle") as HTMLElement;

        fireMouse("mousedown", handle, 150);
        fireMouse("mousemove", document, 60); // shrink with forceFit
        expect(cols[0].width).toBe(80);
        expect(cols[1].width).toBe(120); // grows to maxWidth under forceFit

        fireMouse("mouseup", document, 60);
    });
});

describe('autosizeColumns', () => {
    it('grows columns to fill the available width', () => {
        const cols = [{ id: "a", width: 100, resizable: true }, { id: "b", width: 100, resizable: true }];
        const reRender = autosizeColumns(cols, 300, 30);
        expect(cols[0].width + cols[1].width).toBe(300);
        expect(reRender).toBe(false);
    });

    it('shrinks columns when the total exceeds the available width', () => {
        const cols = [{ id: "a", width: 200, resizable: true, minWidth: 50 }, { id: "b", width: 200, resizable: true, minWidth: 50 }];
        autosizeColumns(cols, 200, 30);
        expect(cols[0].width + cols[1].width).toBe(200);
        expect(cols[0].width).toBeGreaterThanOrEqual(50);
        expect(cols[1].width).toBeGreaterThanOrEqual(50);
    });

    it('respects maxWidth when growing', () => {
        const cols = [{ id: "a", width: 100, resizable: true, maxWidth: 120 }, { id: "b", width: 100, resizable: true }];
        autosizeColumns(cols, 400, 30);
        expect(cols[0].width).toBe(120);
        expect(cols[1].width).toBe(280);
    });

    it('does not shrink non-resizable columns', () => {
        const cols = [{ id: "a", width: 100, resizable: false }, { id: "b", width: 100, resizable: true, minWidth: 80 }];
        autosizeColumns(cols, 100, 30);
        expect(cols[0].width).toBe(100);
        expect(cols[1].width).toBe(80);
    });

    it('returns true when a rerenderOnResize column changes', () => {
        const cols = [{ id: "a", width: 100, resizable: true, rerenderOnResize: true }, { id: "b", width: 100, resizable: true }];
        const reRender = autosizeColumns(cols, 300, 30);
        expect(reRender).toBe(true);
    });

    it('returns false when no rerenderOnResize column changes', () => {
        const cols = [{ id: "a", width: 100, resizable: true }, { id: "b", width: 100, resizable: true }];
        const reRender = autosizeColumns(cols, 300, 30);
        expect(reRender).toBe(false);
    });
});
