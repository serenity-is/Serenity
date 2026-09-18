import * as corelib from "@serenity-is/corelib";
import { Fluent } from "@serenity-is/corelib";
import { mockAdmin, mockDynamicData, mockFetch, mockGridSize, mockRowLookup, unmockFetch } from "test-utils";
import { TranslationGrid } from "../../../Modules/Administration/Translation/TranslationGrid";
import { LanguageRow, TranslationService } from "../../../Modules/ServerTypes/Administration";

beforeAll(() => {
    mockDynamicData();
    mockAdmin();
    mockGridSize();
});

beforeEach(() => {
    mockRowLookup(LanguageRow, [{ LanguageId: "en", LanguageName: "English" }, { LanguageId: "tr", LanguageName: "Turkish" }] as any);
    mockFetch({ "*": () => ({ Entities: [], TotalCount: 0 }) });
});

afterEach(() => {
    unmockFetch();
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

async function createGrid() {
    const grid = new TranslationGrid({});
    await new Promise(resolve => setTimeout(resolve, 20));
    return grid;
}

describe("TranslationGrid", () => {
    it("wires up metadata, columns and no pager", async () => {
        const grid = await createGrid();
        expect(grid["getIdProperty"]()).toBe("Key");
        expect(grid["getLocalTextPrefix"]()).toBe("Administration.Translation");
        expect(grid["getService"]()).toBe(TranslationService.baseUrl);
        expect(grid["usePager"]()).toBe(false);

        const item = { Key: "K", SourceText: "Source", TargetText: "Target", CustomText: "Custom" } as any;
        const columns = grid["createColumns"]();
        expect(columns.length).toBe(3);
        expect(columns[1].format({ value: "Source", item } as any)).toBeTruthy();
        expect(columns[2].format({ value: null, item } as any)).toBeTruthy();
        grid.destroy();
    });

    it("saves changes and notifies", async () => {
        const fetchSpy = mockFetch({ [TranslationService.Methods.Update]: () => ({}) });
        const notify = vi.spyOn(corelib, "notifySuccess");
        const grid = await createGrid();
        grid.view.setItems([{ Key: "K", CustomText: "C" }] as any, true);
        await grid["saveChanges"]("en");
        expect(fetchSpy.requests.some(x => x.url.indexOf("Update") >= 0)).toBe(true);
        expect(notify).toHaveBeenCalled();
        grid.destroy();
    });

    it("saves when the toolbar save button is clicked", async () => {
        const fetchSpy = mockFetch({ [TranslationService.Methods.Update]: () => ({}) });
        const grid = await createGrid();
        const button = grid.element.findFirst(".apply-changes-button");
        expect(button.length).toBe(1);
        button.click();
        await new Promise(resolve => setTimeout(resolve, 20));
        expect(fetchSpy.requests.some(x => x.url.indexOf("Update") >= 0)).toBe(true);
        grid.destroy();
    });

    it("sets view params from language editors", async () => {
        const grid = await createGrid();
        grid["sourceLanguage"].value = "en";
        grid["targetLanguage"].value = "tr";
        grid["setViewParams"]();
        expect(grid.view.params.SourceLanguageID).toBe("en");
        expect(grid.view.params.TargetLanguageID).toBe("tr");
        grid.destroy();
    });

    it("resets hasChanges on view submit", async () => {
        const grid = await createGrid();
        grid["hasChanges"] = true;
        expect(grid["onViewSubmit"]()).toBe(true);
        expect(grid["hasChanges"]).toBe(false);
        grid.destroy();
    });

    it("returns false from view submit when the grid cannot load", async () => {
        const grid = await createGrid();
        vi.spyOn(grid as any, "getGridCanLoad").mockReturnValue(false);
        expect(grid["onViewSubmit"]()).toBe(false);
        grid.destroy();
    });

    it("copies source text to custom text on click", async () => {
        const grid = await createGrid();
        const item = { Key: "K", SourceText: "Source", CustomText: "" } as any;
        grid.view.setItems([item], true);
        const target = document.createElement("a");
        target.classList.add("source-text");
        grid.element.append(target);
        const event: any = { target, preventDefault: vi.fn(), defaultPrevented: false };
        grid["onClick"](event, 0, 0);
        expect(item.CustomText).toBe("Source");
        grid.destroy();
    });

    it("asks confirmation before overriding custom text with source text", async () => {
        const confirm = vi.spyOn(corelib, "confirmDialog").mockImplementation(((msg: any, cb: any) => { cb(); }) as any);
        const grid = await createGrid();
        const item = { Key: "K", SourceText: "Source", CustomText: "Existing" } as any;
        grid.view.setItems([item], true);
        const target = document.createElement("a");
        target.classList.add("source-text");
        grid.element.append(target);
        grid["onClick"]({ target, preventDefault: vi.fn(), defaultPrevented: false } as any, 0, 0);
        expect(confirm).toHaveBeenCalled();
        expect(item.CustomText).toBe("Source");
        grid.destroy();
    });

    it("copies and confirms target text on click", async () => {
        const confirm = vi.spyOn(corelib, "confirmDialog").mockImplementation(((msg: any, cb: any) => { cb(); }) as any);
        const grid = await createGrid();
        const item = { Key: "K", TargetText: "Target", CustomText: "" } as any;
        grid.view.setItems([item], true);
        const target = document.createElement("a");
        target.classList.add("target-text");
        grid.element.append(target);
        grid["onClick"]({ target, preventDefault: vi.fn(), defaultPrevented: false } as any, 0, 0);
        expect(item.CustomText).toBe("Target");

        const item2 = { Key: "K2", TargetText: "Target", CustomText: "Existing" } as any;
        grid.view.setItems([item2], true);
        grid["onClick"]({ target, preventDefault: vi.fn(), defaultPrevented: false } as any, 0, 0);
        expect(confirm).toHaveBeenCalled();
        grid.destroy();
    });

    it("handles custom text input changes", async () => {
        const grid = await createGrid();
        const item = { Key: "K", CustomText: null } as any;
        grid.view.setItems([item], true);
        const el = document.createElement("input");
        el.classList.add("custom-text");
        el.dataset.key = "K";
        el.value = "NewValue";
        grid.element.append(el);
        el.dispatchEvent(new Event("keyup", { bubbles: true }));
        expect(item.CustomText).toBe("NewValue");
        expect(grid["hasChanges"]).toBe(true);
        grid.destroy();
    });

    it("filters items by key and text", async () => {
        const grid = await createGrid();
        expect(grid["onViewFilter"]({ Key: "K", SourceText: "Source" } as any)).toBe(true);
        grid["searchText"] = "sou";
        expect(grid["onViewFilter"]({ Key: "K", SourceText: "Source" } as any)).toBe(true);
        expect(grid["onViewFilter"]({ Key: "K", SourceText: "Other" } as any)).toBe(false);
        expect(grid["onViewFilter"]({ Key: "K", SourceText: null } as any)).toBe(false);
        grid.destroy();
    });

    it("ignores clicks when the default is already prevented", async () => {
        const grid = await createGrid();
        const event: any = { target: document.body, preventDefault: vi.fn(), defaultPrevented: true };
        grid["onClick"](event, 0, 0);
        expect(event.preventDefault).not.toHaveBeenCalled();
        grid.destroy();
    });

    it("clears custom text when emptied", async () => {
        const grid = await createGrid();
        const item = { Key: "K", CustomText: "Existing" } as any;
        grid.view.setItems([item], true);
        const el = document.createElement("input");
        el.classList.add("custom-text");
        el.dataset.key = "K";
        el.value = "";
        grid.element.append(el);
        el.dispatchEvent(new Event("keyup", { bubbles: true }));
        expect(item.CustomText).toBeNull();
        grid.destroy();
    });

    it("refreshes when source language changes without pending changes", async () => {
        const fetchSpy = mockFetch({ "*": () => ({ Entities: [], TotalCount: 0 }) });
        const grid = await createGrid();
        grid["hasChanges"] = false;
        Fluent(grid["sourceLanguage"].domNode).trigger("change");
        await new Promise(resolve => setTimeout(resolve, 20));
        expect(fetchSpy.requests.length).toBeGreaterThan(0);
        grid.destroy();
    });

    it("saves before refreshing when source language changes with pending changes", async () => {
        const fetchSpy = mockFetch({ [TranslationService.Methods.Update]: () => ({}) });
        const grid = await createGrid();
        grid["hasChanges"] = true;
        grid["targetLanguageKey"] = "tr";
        Fluent(grid["sourceLanguage"].domNode).trigger("change");
        await new Promise(resolve => setTimeout(resolve, 20));
        expect(fetchSpy.requests.some(x => x.url.indexOf("Update") >= 0)).toBe(true);
        grid.destroy();
    });

    it("refreshes and saves when target language changes", async () => {
        mockFetch({ [TranslationService.Methods.Update]: () => ({}) });
        const grid = await createGrid();
        grid["hasChanges"] = false;
        Fluent(grid["targetLanguage"].domNode).trigger("change");
        await new Promise(resolve => setTimeout(resolve, 10));
        grid["hasChanges"] = true;
        grid["targetLanguageKey"] = "tr";
        Fluent(grid["targetLanguage"].domNode).trigger("change");
        await new Promise(resolve => setTimeout(resolve, 10));
        grid.destroy();
    });

    it("reacts to quick search input", async () => {
        const grid = await createGrid();
        const input = grid["toolbar"].domNode.querySelector(".s-QuickSearchBar input") as HTMLInputElement;
        expect(input).toBeTruthy();
        input.value = "abc";
        input.dispatchEvent(new Event("input", { bubbles: true }));
        await new Promise(resolve => setTimeout(resolve, 600));
        expect(grid["searchText"]).toBe("abc");
        grid.destroy();
    });
});


