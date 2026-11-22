import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { SelectEditor } from "../editors/selecteditor";
import { QuickFilterBar } from "./quickfilterbar";

let observers: { callback: ResizeObserverCallback, element: Element }[];

class FakeResizeObserver {
    constructor(private callback: ResizeObserverCallback) { }
    observe(element: Element) { observers.push({ callback: this.callback, element }); }
    disconnect() { observers = observers.filter(x => x.callback !== this.callback); }
    unobserve() { }
}

/** Resizes the bar to the width, as the browser would tell its observer. */
function resize(bar: QuickFilterBar, width: number) {
    Object.defineProperty(bar.domNode, "clientWidth", { configurable: true, value: width });
    observers.filter(x => x.element === bar.domNode).forEach(x => x.callback([], null));
}

function createBar(options: any = {}, filters?: any[]) {
    const element = document.body.appendChild(document.createElement("div"));
    return new QuickFilterBar({
        filters: filters ?? [
            { field: "Category", title: "Category", type: SelectEditor, options: { items: ["A", "B"] }, pinned: true },
            { field: "Supplier", title: "Supplier", type: SelectEditor, options: { items: ["X", "Y"] } },
            { field: "Region", title: "Region", type: SelectEditor, options: { items: ["N", "S"] }, cssClass: "hidden-xs" }
        ],
        element,
        ...options
    } as any);
}

const toggle = (bar: QuickFilterBar) => bar.domNode.querySelector<HTMLButtonElement>(".quick-filters-toggle");
const item = (bar: QuickFilterBar, field: string) => bar.domNode.querySelector<HTMLElement>(`[data-qffield=${field}]`);

beforeEach(() => {
    observers = [];
    vi.stubGlobal("ResizeObserver", FakeResizeObserver);
    // the bar is collapsed in the next frame, which is now here
    vi.stubGlobal("requestAnimationFrame", (callback: FrameRequestCallback) => { callback(0); return 0; });
    vi.stubGlobal("cancelAnimationFrame", () => { });
});

afterEach(() => {
    vi.unstubAllGlobals();
    QuickFilterBar.defaults.collapsible = true;
    QuickFilterBar.defaults.collapseWidth = 768;
    document.body.innerHTML = "";
});

describe("a quick filter bar that is narrow", () => {
    it("is not collapsed while it is wide", () => {
        const bar = createBar();
        resize(bar, 1000);
        expect(bar.domNode.classList.contains("quick-filters-narrow")).toBe(false);
        expect(toggle(bar)).toBeNull();
        bar.destroy();
    });

    it("collapses its filters, except the pinned ones, behind a button at its start", () => {
        const bar = createBar();
        resize(bar, 400);
        expect(bar.domNode.classList.contains("quick-filters-narrow")).toBe(true);
        expect(bar.domNode.firstElementChild).toBe(toggle(bar));
        expect(toggle(bar).hidden).toBe(false);
        expect(toggle(bar).getAttribute("aria-expanded")).toBe("false");
        expect(item(bar, "Category").classList.contains("quick-filter-pinned")).toBe(true);
        expect(item(bar, "Supplier").classList.contains("quick-filter-pinned")).toBe(false);

        resize(bar, 1000);
        expect(bar.domNode.classList.contains("quick-filters-narrow")).toBe(false);
        expect(toggle(bar).hidden).toBe(true);
        bar.destroy();
    });

    it("collapses the filters that were hidden on a small screen, so that they can be opened", () => {
        const bar = createBar();
        resize(bar, 400);
        expect(item(bar, "Region").classList.contains("hidden-xs")).toBe(false);
        expect(item(bar, "Region").classList.contains("quick-filter-hidden-xs")).toBe(true);

        // also one that is added while it is narrow
        bar.add({ field: "Other", title: "Other", type: SelectEditor, options: { items: ["Z"] }, cssClass: "hidden-xs" } as any);
        expect(item(bar, "Other").classList.contains("hidden-xs")).toBe(false);

        resize(bar, 1000);
        expect(item(bar, "Region").classList.contains("hidden-xs")).toBe(true);
        expect(item(bar, "Other").classList.contains("hidden-xs")).toBe(true);
        expect(item(bar, "Region").classList.contains("quick-filter-hidden-xs")).toBe(false);
        bar.destroy();
    });

    it("opens and closes its filters with the button", () => {
        const bar = createBar();
        resize(bar, 400);
        toggle(bar).click();
        expect(bar.domNode.classList.contains("quick-filters-expanded")).toBe(true);
        expect(toggle(bar).getAttribute("aria-expanded")).toBe("true");
        toggle(bar).click();
        expect(bar.domNode.classList.contains("quick-filters-expanded")).toBe(false);
        expect(toggle(bar).getAttribute("aria-expanded")).toBe("false");
        bar.destroy();
    });

    it("shows how many of the collapsed filters are applied", () => {
        const bar = createBar();
        resize(bar, 400);
        const count = () => toggle(bar).querySelector<HTMLElement>(".quick-filters-count");
        bar.onSubmit({} as any);
        expect(count().hidden).toBe(true);

        bar.find(SelectEditor, "Category").value = "A";
        bar.find(SelectEditor, "Supplier").value = "X";
        bar.find(SelectEditor, "Region").value = "N";
        bar.onSubmit({} as any);
        // the pinned filter is seen, so it is not counted
        expect(count().hidden).toBe(false);
        expect(count().textContent).toBe("2");
        bar.destroy();
    });

    it("has its clear button beside the button, and after the filters again when it is wide", () => {
        const bar = createBar();
        bar.find(SelectEditor, "Supplier").value = "X";
        bar.onSubmit({} as any);
        const clear = bar.domNode.querySelector(".quick-filters-clear");

        resize(bar, 400);
        expect(toggle(bar).nextElementSibling).toBe(clear);

        // a filter that is added meanwhile goes after the others
        bar.add({ field: "Other", title: "Other", type: SelectEditor, options: { items: ["Z"] } } as any);
        bar.onSubmit({} as any);
        expect(toggle(bar).nextElementSibling).toBe(clear);
        expect(bar.domNode.querySelector(".quick-filter-item:last-of-type").getAttribute("data-qffield")).toBe("Other");

        resize(bar, 1000);
        expect(clear.previousElementSibling.getAttribute("data-qffield")).toBe("Other");
        bar.destroy();
    });

    it("has no button when all its filters are pinned", () => {
        const bar = createBar({}, [{ field: "Category", title: "Category", type: SelectEditor, options: { items: ["A"] }, pinned: true }]);
        resize(bar, 400);
        expect(toggle(bar).hidden).toBe(true);
        bar.destroy();
    });

    it("keeps how it is while it is not shown", () => {
        const bar = createBar();
        resize(bar, 400);
        resize(bar, 0);
        expect(bar.domNode.classList.contains("quick-filters-narrow")).toBe(true);
        bar.destroy();
    });

    it("collapses below the width that the defaults give", () => {
        QuickFilterBar.defaults.collapseWidth = 500;
        const bar = createBar();
        resize(bar, 600);
        expect(bar.domNode.classList.contains("quick-filters-narrow")).toBe(false);
        resize(bar, 450);
        expect(bar.domNode.classList.contains("quick-filters-narrow")).toBe(true);
        bar.destroy();
    });

    it("is not collapsed when the bar, or the defaults, say so", () => {
        const own = createBar({ collapsible: false });
        resize(own, 400);
        expect(own.domNode.classList.contains("quick-filters-narrow")).toBe(false);
        own.setNarrow(true);
        expect(own.domNode.classList.contains("quick-filters-narrow")).toBe(false);
        own.destroy();

        QuickFilterBar.defaults.collapsible = false;
        const byDefault = createBar();
        resize(byDefault, 400);
        expect(byDefault.domNode.classList.contains("quick-filters-narrow")).toBe(false);
        byDefault.destroy();
    });

    it("is collapsed in the next frame, and not while the browser notifies its observer", () => {
        const frames: FrameRequestCallback[] = [];
        vi.stubGlobal("requestAnimationFrame", (callback: FrameRequestCallback) => frames.push(callback));
        const bar = createBar();
        resize(bar, 400);
        expect(bar.domNode.classList.contains("quick-filters-narrow")).toBe(false);
        frames.forEach(f => f(0));
        expect(bar.domNode.classList.contains("quick-filters-narrow")).toBe(true);
        bar.destroy();
    });

    it("stops watching its width when it is destroyed", () => {
        const bar = createBar();
        expect(observers.length).toBe(1);
        bar.destroy();
        expect(observers.length).toBe(0);
    });

    it("is not collapsed where the width can not be watched", () => {
        vi.stubGlobal("ResizeObserver", undefined);
        const bar = createBar();
        expect(bar.domNode.classList.contains("quick-filters-narrow")).toBe(false);
        bar.destroy();
    });
});
