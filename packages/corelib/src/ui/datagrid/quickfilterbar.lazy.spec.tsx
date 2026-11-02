import { afterEach, describe, expect, it, vi } from "vitest";
import { Config, Fluent, type ListRequest } from "../../base";
import { EditorUtils } from "../editors/editorutils";
import { StringEditor } from "../editors/stringeditor";
import { tryGetWidget } from "../widgets/widget";
import "../filtering/editorfiltering";
import { propertyItemToQuickFilter } from "./datagrid-internal";
import { getCurrentSettings, restoreSettingsFrom } from "./datagrid-persistence";
import { QuickFilterBar } from "./quickfilterbar";
import { QuickFilterPlaceholder } from "./quickfilterplaceholder";

describe("QuickFilterBar with a widget type that has to be loaded", () => {
    const orgLoader = Config.lazyTypeLoader;
    let keyId = 0;

    afterEach(() => {
        Config.lazyTypeLoader = orgLoader;
        document.body.innerHTML = "";
    });

    class LazyEditor extends StringEditor {
    }

    const uniqueName = "Grid";
    const fieldSelector = "#Grid_QuickFilter_Field1";
    const flush = () => new Promise(r => setTimeout(r));

    function setup(override?: (filter: any) => void) {
        // types stay registered after they are loaded, so each test needs its own key
        const key = "Test.LazyQuickFilterEditor" + (++keyId);
        let resolve: () => void;
        const promise = new Promise(r => resolve = () => r(LazyEditor));
        Config.lazyTypeLoader = k => k === key ? promise : null;

        const filter = propertyItemToQuickFilter({
            name: "Field1", title: "Field 1", filteringType: "Editor", filteringParams: { editorType: key }
        });
        override?.(filter);

        const div = document.body.appendChild(document.createElement("div"));
        const bar = new QuickFilterBar({ filters: [filter], idPrefix: uniqueName + "_QuickFilter_", element: div });
        return { bar, div, key, load: async () => { resolve(); await flush(); } };
    }

    function submit(bar: QuickFilterBar) {
        const request: ListRequest = {};
        bar.onSubmit(request);
        return request;
    }

    const restore = (div: HTMLElement, quickFilters: any) => restoreSettingsFrom({
        filterStore: null, flags: {}, includeDeletedToggle: null, quickFiltersDiv: div,
        sleekGrid: { getAllColumns: () => [] } as any, settings: { quickFilters }, toolbarNode: null, uniqueName, view: null
    });

    const save = (div: HTMLElement) => getCurrentSettings({
        filterStore: null, flags: { columnPinning: false, columnWidths: false, columnVisibility: false, sortColumns: false, includeDeleted: false },
        includeDeletedToggle: null, quickFiltersDiv: div, sleekGrid: null, toolbarNode: null, uniqueName
    }).quickFilters;

    it("uses a placeholder until the type is loaded, then the actual widget in its place", async () => {
        const { bar, div, load } = setup();
        const item = div.querySelector(".quick-filter-item");

        expect(tryGetWidget(fieldSelector)).toBeInstanceOf(QuickFilterPlaceholder);
        expect(item.querySelector("input").type).toBe("hidden");

        await load();

        expect(tryGetWidget(fieldSelector)).toBeInstanceOf(LazyEditor);
        expect(item.querySelectorAll("input").length).toBe(1);
        expect(item.querySelector("input").type).not.toBe("hidden");
        expect(bar.tryFind(LazyEditor, "Field1")).toBeInstanceOf(LazyEditor);
    });

    it("doesn't add the placeholder value to the request, nor mark the filter active, when it has none", () => {
        const { bar, div } = setup();

        expect(submit(bar).EqualityFilter?.Field1).toBeUndefined();
        expect(div.querySelector(".quick-filter-active")).toBeNull();
    });

    it("restores persisted state, uses it for requests and saving while loading, then transfers it", async () => {
        const { bar, div, load } = setup();

        restore(div, { Field1: "saved" });

        expect(save(div)).toEqual({ Field1: "saved" });
        expect(submit(bar).EqualityFilter.Field1).toBe("saved");
        expect(div.querySelector(".quick-filter-active")).toBeTruthy();

        await load();

        expect(EditorUtils.getValue(tryGetWidget(fieldSelector))).toBe("saved");
        expect(save(div)).toEqual({ Field1: "saved" });
        expect(submit(bar).EqualityFilter.Field1).toBe("saved");
    });

    it("submits the value of the actual widget once it is loaded", async () => {
        const { bar, load } = setup();
        await load();

        bar.find(LazyEditor, "Field1").value = "typed";

        expect(submit(bar).EqualityFilter.Field1).toBe("typed");
    });

    it("doesn't report a change for loading, but does for later changes of the actual widget", async () => {
        const { bar, div, load } = setup();
        bar.onChange = vi.fn();
        restore(div, { Field1: "saved" });

        await load();
        await flush();

        expect(bar.onChange).not.toHaveBeenCalled();

        bar.find(LazyEditor, "Field1").domNode.dispatchEvent(new Event("change"));
        await flush();

        expect(bar.onChange).toHaveBeenCalledTimes(1);
    });

    it("calls init and element only for the actual widget", async () => {
        const init = vi.fn();
        const element = vi.fn();
        const { load } = setup(filter => { filter.init = init; filter.element = element; });

        expect(init).not.toHaveBeenCalled();
        expect(element).not.toHaveBeenCalled();

        await load();

        expect(init).toHaveBeenCalledTimes(1);
        expect(init.mock.calls[0][0]).toBeInstanceOf(LazyEditor);
        expect(element).toHaveBeenCalledTimes(1);
    });

    it("passes the options to the actual widget", async () => {
        const { div, load } = setup(filter => { filter.options = { ...filter.options, maxlength: 7 }; });

        await load();

        expect(div.querySelector("input").getAttribute("maxlength")).toBe("7");
    });

    it("calls the handler with the placeholder value while loading", () => {
        const handler = vi.fn();
        const { bar, div } = setup(filter => { filter.handler = handler; });
        restore(div, { Field1: "saved" });

        submit(bar);

        expect(handler.mock.calls[0][0].value).toBe("saved");
        expect(handler.mock.calls[0][0].widget).toBeInstanceOf(QuickFilterPlaceholder);
    });

    it("doesn't create the widget if the bar is destroyed meanwhile", async () => {
        const init = vi.fn();
        const { bar, div, load } = setup(filter => { filter.init = init; });

        bar.destroy();
        await load();

        expect(init).not.toHaveBeenCalled();
        expect(div.querySelector(".s-LazyEditor")).toBeNull();
    });

    it("removes the submit handler if the placeholder is disposed, without creating the widget", async () => {
        const handler = vi.fn();
        const init = vi.fn();
        const { bar, load } = setup(filter => { filter.handler = handler; filter.init = init; });

        Fluent.remove(tryGetWidget<QuickFilterPlaceholder>(fieldSelector).domNode);
        await load();

        submit(bar);
        expect(handler).not.toHaveBeenCalled();
        expect(init).not.toHaveBeenCalled();
    });

    it("keeps the submit handler when the placeholder is replaced, and removes it with the actual widget", async () => {
        const handler = vi.fn();
        const { bar, load } = setup(filter => { filter.handler = handler; });
        await load();

        submit(bar);
        expect(handler).toHaveBeenCalledTimes(1);

        Fluent.remove(bar.find(LazyEditor, "Field1").domNode);

        submit(bar);
        expect(handler).toHaveBeenCalledTimes(1);
    });

    it("uses the type directly once it is loaded", async () => {
        const { key, load } = setup();
        await load();

        const again = propertyItemToQuickFilter({
            name: "Field2", filteringType: "Editor", filteringParams: { editorType: key }
        });

        expect(again.type).toBe(LazyEditor);
    });
});
