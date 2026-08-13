import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { Fluent } from "../../base";
import { DateEditor } from "../editors/dateeditor";
import { EditorUtils } from "../editors/editorutils";
import { Widget } from "../widgets/widget";
import { getWidgetFrom, tryGetWidget } from "../widgets/widgetutils";
import { QuickFilterBar } from "./quickfilterbar";
import { invokeDisposingListeners } from "@serenity-is/domwise";

class TestWidget extends Widget<any> {
    static override [Symbol.typeInfo] = this.registerClass("Test.TestWidget");
    static override createDefaultElement() { return document.createElement("input"); }
}

function createBar(filters?: any[]) {
    const el = document.createElement("div");
    document.body.appendChild(el);
    const bar = new QuickFilterBar({ filters: filters ?? [], element: el } as any);
    return { bar, el };
}

describe("QuickFilterBar", () => {
    beforeEach(() => { document.body.innerHTML = ""; });
    afterEach(() => { document.body.innerHTML = ""; vi.restoreAllMocks(); });

    it("constructor adds class and sets idPrefix", () => {
        const { bar } = createBar();
        expect(bar.domNode.classList.contains("quick-filters-bar")).toBe(true);
        expect(bar["options"].idPrefix).toBeTruthy();
        bar.destroy();
    });

    it("addSeparator appends an hr", () => {
        const { bar } = createBar();
        bar.addSeparator();
        expect(bar.domNode.querySelector("hr")).toBeTruthy();
        bar.destroy();
    });

    it("add throws when opt is null", () => {
        const { bar } = createBar();
        expect(() => (bar as any).add(null)).toThrow();
        bar.destroy();
    });

    it("add creates widget and adds separator when opt.separator", () => {
        const { bar } = createBar();
        const widget = bar.add({ field: "Name", title: "Name", type: TestWidget, separator: true } as any);
        expect(widget).toBeInstanceOf(TestWidget);
        expect(bar.domNode.querySelector("hr")).toBeTruthy();
        expect(bar.domNode.querySelector(".quick-filter-label")).toBeTruthy();
        bar.destroy();
    });

    it("add falls back to field as title when no title", () => {
        const { bar } = createBar();
        bar.add({ field: "FieldX", type: TestWidget } as any);
        expect(bar.domNode.querySelector(".quick-filter-label")!.textContent).toBe("FieldX");
        bar.destroy();
    });

    it("add uses getTitle option when no title", () => {
        const el = document.createElement("div");
        document.body.appendChild(el);
        const bar = new QuickFilterBar({
            filters: [],
            element: el,
            getTitle: (f: any) => "T-" + f.field
        } as any);
        bar.add({ field: "A", type: TestWidget } as any);
        expect(bar.domNode.querySelector(".quick-filter-label")!.textContent).toBe("T-A");
        bar.destroy();
    });

    it("constructor adds configured filters", () => {
        const el = document.createElement("div");
        document.body.appendChild(el);
        const bar = new QuickFilterBar({ filters: [{ field: "A", title: "A", type: TestWidget }], element: el } as any);
        expect(bar.domNode.querySelector(".quick-filter-item")).toBeTruthy();
        bar.destroy();
    });

    it("add stores item data and applies cssClass", () => {
        const { bar } = createBar();
        const widget = bar.add({
            field: "A", title: "A", type: TestWidget, cssClass: "my-css",
            displayText: () => "x", saveState: () => "s", loadState: () => { }
        } as any);
        const item = bar.domNode.querySelector(".quick-filter-item");
        expect(item!.classList.contains("my-css")).toBe(true);
        expect(QuickFilterBar.getItemData(item!)).toBeDefined();
        expect(widget).toBeInstanceOf(TestWidget);
        bar.destroy();
    });

    it("change event on widget triggers onChange", () => {
        vi.useFakeTimers();
        const { bar } = createBar();
        const onChange = vi.fn();
        bar.onChange = onChange;
        const widget = bar.add({ field: "A", title: "A", type: TestWidget } as any);
        widget.domNode.dispatchEvent(new Event("change"));
        vi.advanceTimersByTime(0);
        expect(onChange).toHaveBeenCalled();
        vi.useRealTimers();
        bar.destroy();
    });

    it("onSubmit invokes handler and marks active", () => {
        const { bar } = createBar();
        const handler = vi.fn((args: any) => { args.handled = true; });
        const widget = bar.add({ field: "Name", title: "Name", type: TestWidget, handler } as any);
        (widget.domNode as any).value = "abc";
        const request: any = {};
        bar.onSubmit(request);
        expect(handler).toHaveBeenCalled();
        expect(request.EqualityFilter).toBeDefined();
        bar.destroy();
    });

    it("onSubmit sets EqualityFilter when no handler", () => {
        const { bar } = createBar();
        const widget = bar.add({ field: "Name", title: "Name", type: TestWidget } as any);
        (widget.domNode as any).value = "xyz";
        const request: any = {};
        bar.onSubmit(request);
        expect(request.EqualityFilter).toEqual({ Name: "xyz" });
        bar.destroy();
    });

    it("onSubmit skips ignored filter items", () => {
        const { bar } = createBar();
        const widget = bar.add({ field: "Name", title: "Name", type: TestWidget } as any);
        (widget.domNode as HTMLElement).closest(".quick-filter-item")?.classList.add("ignore");
        (widget.domNode as any).value = "xyz";
        const request: any = {};
        bar.onSubmit(request);
        expect(request.EqualityFilter).toBeUndefined();
        bar.destroy();
    });

    it("find and tryFind locate widgets by idPrefix and field", () => {
        const { bar } = createBar();
        const widget = bar.add({ field: "Name", title: "Name", type: TestWidget } as any);
        expect(bar.find(TestWidget, "Name")).toBe(widget);
        expect(bar.tryFind(TestWidget, "Name")).toBe(widget);
        bar.destroy();
    });

    it("destroy clears submitHandlers", () => {
        const { bar } = createBar();
        bar.destroy();
        expect(bar["submitHandlers"]).toBeNull();
    });

    it("boolean static returns config and handler sets equality filter", () => {
        const qf = QuickFilterBar.boolean("Active");
        expect(qf.field).toBe("Active");
        expect(qf.type).toBeDefined();
        const args = { field: "Active", value: "1", equalityFilter: {} } as any;
        qf.handler!(args);
        expect(args.equalityFilter.Active).toBe(true);
    });

    it("boolean handler clears equality filter for empty value", () => {
        const qf = QuickFilterBar.boolean("Active");
        const args = { field: "Active", value: "", equalityFilter: {} } as any;
        qf.handler!(args);
        expect(args.equalityFilter.Active).toBeNull();
    });

    it("dateRange static returns config with range helpers", () => {
        const qf = QuickFilterBar.dateRange("Date", "Date");
        expect(qf.field).toBe("Date");
        expect(qf.type).toBeDefined();
        expect(typeof qf.handler).toBe("function");
        expect(typeof qf.displayText).toBe("function");
        expect(typeof qf.saveState).toBe("function");
        expect(typeof qf.loadState).toBe("function");
    });

    it("dateTimeRange static returns config", () => {
        const qf = QuickFilterBar.dateTimeRange("Date", "Date", true);
        expect(qf.field).toBe("Date");
        expect(qf.type).toBeDefined();
    });

    it("addDateRange / addDateTimeRange / addBoolean create widgets", () => {
        const { bar } = createBar();
        expect(() => bar.addDateRange("Date", "Date")).not.toThrow();
        expect(() => bar.addDateTimeRange("Date", "Date")).not.toThrow();
        expect(() => bar.addBoolean("Active", "Active")).not.toThrow();
        bar.destroy();
    });

    it("getItemData returns undefined for unknown node", () => {
        const node = document.createElement("div");
        expect(QuickFilterBar.getItemData(node)).toBeUndefined();
    });

    it("dateRange element callback creates editors and separator", () => {
        const qf = QuickFilterBar.dateRange("Date", "Date");
        const container = document.createElement("div");
        document.body.appendChild(container);
        const el = document.createElement("input");
        container.appendChild(el);
        qf.element!(Fluent(el) as any);
        expect(container.querySelector(".range-separator")).toBeTruthy();
        expect(container.querySelectorAll("input").length).toBeGreaterThanOrEqual(1);
        document.body.innerHTML = "";
    });

    it("dateRange handler builds criteria for valid date", () => {
        const qf = QuickFilterBar.dateRange("Date", "Date");
        const container = document.createElement("div");
        document.body.appendChild(container);
        const el = document.createElement("input");
        container.appendChild(el);
        qf.element!(Fluent(el) as any);
        const widget = { value: "2020-01-01", domNode: { value: "" } } as any;
        const args: any = { field: "Date", request: {}, widget };
        qf.handler!(args);
        expect(args.active).toBe(true);
        expect(args.request.Criteria).toBeTruthy();
        document.body.innerHTML = "";
    });

    it("dateRange handler clears invalid date", () => {
        const qf = QuickFilterBar.dateRange("Date", "Date");
        const container = document.createElement("div");
        document.body.appendChild(container);
        const el = document.createElement("input");
        container.appendChild(el);
        qf.element!(Fluent(el) as any);
        const widget = { value: "invalid", domNode: { value: "x" } } as any;
        const args: any = { field: "Date", request: {}, widget };
        qf.handler!(args);
        expect(widget.domNode.value).toBe("");
        document.body.innerHTML = "";
    });

    it("dateRange saveState and loadState round-trip", () => {
        const qf = QuickFilterBar.dateRange("Date", "Date");
        const container = document.createElement("div");
        document.body.appendChild(container);
        const el = document.createElement("input");
        container.appendChild(el);
        qf.element!(Fluent(el) as any);
        // Find the end editor widget created by the element callback
        const inputs = container.querySelectorAll<HTMLInputElement>("input");
        const endWidget = inputs.length > 1 ? getWidgetFrom(inputs[1], DateEditor) : null;
        const w1 = { get_value: () => "2020-01-01", domNode: { value: "" } } as any;
        if (endWidget) {
            const state = qf.saveState!(w1);
            expect(Array.isArray(state)).toBe(true);
            expect(() => qf.loadState!(w1, state)).not.toThrow();
        }
        document.body.innerHTML = "";
    });

    function setEditorInputValue(editor: any, value: string) {
        const input = editor.domNode as HTMLInputElement;
        input.value = value;
        input.dispatchEvent(new Event("change"));
    }

    it("dateRange handler and helpers with both editor values", () => {
        const qf = QuickFilterBar.dateRange("Date", "Date");
        const container = document.createElement("div");
        document.body.appendChild(container);
        const el = document.createElement("input");
        container.appendChild(el);
        qf.element!(Fluent(el) as any);
        const endInput = container.querySelectorAll<HTMLInputElement>("input")[1];
        endInput.value = "2020-01-05";
        endInput.dispatchEvent(new Event("change"));

        const mainWidget = new DateEditor({ element: el2 => Fluent(el2).insertAfter(el) });
        setEditorInputValue(mainWidget, "2020-01-01");

        const args: any = { field: "Date", request: {}, widget: mainWidget };
        qf.handler!(args);
        expect(args.active).toBe(true);
        expect(args.request.Criteria).toBeTruthy();

        expect(qf.displayText!(mainWidget as any, "Date")).toBeTruthy();
        const state = qf.saveState!(mainWidget as any);
        expect(Array.isArray(state)).toBe(true);
        expect(() => qf.loadState!(mainWidget as any, [null, null])).not.toThrow();
        expect(() => qf.loadState!(mainWidget as any, null)).not.toThrow();
        document.body.innerHTML = "";
    });

    it("dateRange handler covers invalid date", () => {
        const qf = QuickFilterBar.dateRange("Date", "Date");
        const container = document.createElement("div");
        document.body.appendChild(container);
        const el = document.createElement("input");
        container.appendChild(el);
        qf.element!(Fluent(el) as any);
        const mainWidget = new DateEditor({ element: el2 => Fluent(el2).insertAfter(el) });
        (mainWidget.domNode as HTMLInputElement).value = "invalid";
        (mainWidget.domNode as HTMLInputElement).dispatchEvent(new Event("change"));
        const args: any = { field: "Date", request: {}, widget: mainWidget };
        expect(() => qf.handler!(args)).not.toThrow();
        document.body.innerHTML = "";
    });

    it("dateTimeRange handler and helpers with both editor values", () => {
        const qf = QuickFilterBar.dateTimeRange("Date", "Date");
        const container = document.createElement("div");
        document.body.appendChild(container);
        const el = document.createElement("input");
        container.appendChild(el);
        qf.element!(Fluent(el) as any);
        const endInput = container.querySelectorAll<HTMLInputElement>("input")[1];
        endInput.value = "2020-01-05 18:00";
        endInput.dispatchEvent(new Event("change"));

        const mainWidget = new DateEditor({ element: el2 => Fluent(el2).insertAfter(el) });
        setEditorInputValue(mainWidget, "2020-01-01 10:00");

        const args: any = { field: "Date", request: {}, widget: mainWidget };
        qf.handler!(args);
        expect(args.active).toBe(true);
        expect(args.request.Criteria).toBeTruthy();

        expect(qf.displayText!(mainWidget as any, "Date")).toBeTruthy();
        const state = qf.saveState!(mainWidget as any);
        expect(Array.isArray(state)).toBe(true);
        expect(() => qf.loadState!(mainWidget as any, state)).not.toThrow();
        document.body.innerHTML = "";
    });

    it("dateTimeRange element callback creates editors", () => {
        const qf = QuickFilterBar.dateTimeRange("Date", "Date");
        const container = document.createElement("div");
        document.body.appendChild(container);
        const el = document.createElement("input");
        container.appendChild(el);
        qf.element!(Fluent(el) as any);
        expect(container.querySelector(".range-separator")).toBeTruthy();
        document.body.innerHTML = "";
    });

    it("dateTimeRange handler builds criteria", () => {
        const qf = QuickFilterBar.dateTimeRange("Date", "Date");
        const container = document.createElement("div");
        document.body.appendChild(container);
        const el = document.createElement("input");
        container.appendChild(el);
        qf.element!(Fluent(el) as any);
        const widget = { value: "2020-01-01 10:00", domNode: { value: "" } } as any;
        const args: any = { field: "Date", request: {}, widget };
        qf.handler!(args);
        expect(args.active).toBe(true);
        document.body.innerHTML = "";
    });

    it("disposing event removes the submit handler", () => {
        const { bar } = createBar();
        const widget = bar.add({ field: "A", title: "A", type: TestWidget } as any);
        const domNode = widget.domNode as HTMLElement;
        invokeDisposingListeners(domNode);
        (domNode as any).value = "x";
        const req: any = {};
        bar.onSubmit(req);
        expect(req.EqualityFilter).toBeUndefined();
        bar.destroy();
    });

    it("dateTimeRange init callback runs", () => {
        const qf = QuickFilterBar.dateTimeRange("Date", "Date", true);
        const w = { domNode: document.createElement("input") };
        expect(() => qf.init!(w as any)).not.toThrow();
        expect(qf.options).toEqual({ useUtc: true });
    });

    it("dateTimeRange options is null when useUtc undefined", () => {
        const qf = QuickFilterBar.dateTimeRange("Date", "Date");
        expect(qf.options).toBeNull();
    });

    it("handler that does not set handled writes equality filter", () => {
        const { bar } = createBar();
        const widget = bar.add({
            field: "B", title: "B", type: TestWidget,
            handler: (args: any) => { args.equalityFilter.B = args.value; }
        } as any);
        (widget.domNode as any).value = "v";
        const req: any = {};
        bar.onSubmit(req);
        expect(req.EqualityFilter.B).toBe("v");
        bar.destroy();
    });

    it("dateRange displayText v1-only branch", () => {
        const qf = QuickFilterBar.dateRange("Date", "Date");
        const container = document.createElement("div");
        document.body.appendChild(container);
        const el = document.createElement("input");
        container.appendChild(el);
        qf.element!(Fluent(el) as any);
        const mainWidget = new DateEditor({ element: el2 => Fluent(el2).insertAfter(el) });
        setEditorInputValue(mainWidget, "2020-01-01");
        const text = qf.displayText!(mainWidget as any, "Date");
        expect(text).toContain(">=");
        document.body.innerHTML = "";
    });

    it("dateRange displayText v2-only branch", () => {
        const qf = QuickFilterBar.dateRange("Date", "Date");
        const container = document.createElement("div");
        document.body.appendChild(container);
        const el = document.createElement("input");
        container.appendChild(el);
        qf.element!(Fluent(el) as any);
        const endInput = container.querySelectorAll<HTMLInputElement>("input")[1];
        endInput.value = "2020-01-05";
        endInput.dispatchEvent(new Event("change"));
        const mainWidget = new DateEditor({ element: el2 => Fluent(el2).insertAfter(el) });
        const text = qf.displayText!(mainWidget as any, "Date");
        expect(text).toContain("<=");
        document.body.innerHTML = "";
    });

    it("dateRange loadState handles invalid state", () => {
        const qf = QuickFilterBar.dateRange("Date", "Date");
        const container = document.createElement("div");
        document.body.appendChild(container);
        const el = document.createElement("input");
        container.appendChild(el);
        qf.element!(Fluent(el) as any);
        const mainWidget = new DateEditor({ element: el2 => Fluent(el2).insertAfter(el) });
        expect(() => qf.loadState!(mainWidget as any, null)).not.toThrow();
        expect(() => qf.loadState!(mainWidget as any, [1])).not.toThrow();
        document.body.innerHTML = "";
    });

    it("add works for filter without a field", () => {
        const { bar } = createBar();
        const widget = bar.add({ title: "NoField", type: TestWidget } as any);
        expect(widget).toBeInstanceOf(TestWidget);
        expect(bar.domNode.querySelector(".quick-filter-item")).toBeTruthy();
        bar.destroy();
    });

    it("boolean handler converts zero value to false", () => {
        const qf = QuickFilterBar.boolean("Active");
        const args = { field: "Active", value: "0", equalityFilter: {} } as any;
        qf.handler!(args);
        expect(args.equalityFilter.Active).toBe(false);
    });
});
