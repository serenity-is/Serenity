import { Combobox, ComboboxSearchQuery, ComboboxSearchResult } from "./combobox";
import { ComboboxEditor } from "./comboboxeditor";
import { DialogTypeRegistry } from "../../types/dialogtyperegistry";
import { SubDialogHelper } from "../helpers/subdialoghelper";

describe("ComboboxEditor async behavior", () => {

    type TestItem = {
        ID: string,
    }

    class ReverseAsyncCombo extends ComboboxEditor<{ filter?: (x: string) => boolean }, TestItem> {
        protected override hasAsyncSource(): boolean {
            return true;
        }

        protected override isMultiple() {
            return true;
        }

        protected override getIdField() {
            return "ID";
        }

        protected asyncSearch(query: ComboboxSearchQuery): PromiseLike<ComboboxSearchResult<TestItem>> {
            if (query.initSelection) {
                return Promise.resolve({
                    items: query.idList.slice().reverse().filter(this.props.filter ?? (() => true)).map(id => ({
                        ID: id,
                        Name: "Name" + id
                    })),
                    more: false
                });
            }

            throw "Not Implemented";
        }
    }

    it("should preserve order for async initSelection", async () => {
        var combo = new ReverseAsyncCombo({
        });
        combo.values = ["3", "2", "1"];
        expect(combo.values).toStrictEqual(["3", "2", "1"]);
        await Promise.resolve();
        expect(combo.values).toStrictEqual(["3", "2", "1"]);
    });

    it("should preserve order when some items not found for async initSelection", async () => {
        vi.useFakeTimers();
        try {
            var combo = new ReverseAsyncCombo({
                filter: x => x !== "2"
            });
            combo.values = ["3", "2", "1"];
            expect(combo.values).toStrictEqual(["3", "2", "1"]);
            await vi.runAllTimersAsync();
            expect(combo.values).toStrictEqual(["3", "1"]);
        }
        finally {
            vi.useRealTimers();
        }
    });
});

describe("ComboboxEditor helpers", () => {
    it("filterByText prioritizes prefix matches and preserves contains matches", () => {
        const result = ComboboxEditor.filterByText(
            [{ text: "Beta" }, { text: "Alpha" }, { text: "Alphabet" }, { text: "Other" }],
            x => x.text,
            "alp");

        expect(result.map(x => x.text)).toEqual(["Alpha", "Alphabet"]);
    });

    it("filterByText returns original items for empty term", () => {
        const items = [{ text: "A" }, { text: "B" }];
        expect(ComboboxEditor.filterByText(items, x => x.text, "")).toBe(items);
        expect(ComboboxEditor.filterByText(items, x => x.text, null)).toBe(items);
    });

    it("maps item id and text fields", () => {
        class TestCombo extends ComboboxEditor<any, any> {
            protected getIdField() { return "Id"; }
            protected getTextField() { return "Name"; }
        }
        const combo = new TestCombo({});
        expect((combo as any).itemId({ Id: 5 })).toBe("5");
        expect((combo as any).itemText({ Name: 6 })).toBe("6");
        expect((combo as any).mapItem({ Id: 5, Name: "Five" })).toEqual({
            id: "5", text: "Five", disabled: false, source: { Id: 5, Name: "Five" }
        });
        combo.destroy();
    });

    it("maps null item fields to empty strings", () => {
        class TestCombo extends ComboboxEditor<any, any> {
            protected getIdField() { return "Id"; }
            protected getTextField() { return "Name"; }
        }
        const combo = new TestCombo({});
        expect((combo as any).itemId({ Id: null })).toBe("");
        expect((combo as any).itemText({ Name: null })).toBe("");
        combo.destroy();
    });

    it("supports edit value serialization", () => {
        class TestCombo extends ComboboxEditor<any, any> {
            protected isMultiple() { return false; }
        }
        const combo = new TestCombo({});
        combo["combobox"] = { getValue: vi.fn(() => "1"), setValue: vi.fn(), dispose: vi.fn() } as any;
        const target: any = {};
        combo.getEditValue({ name: "Field" } as any, target);
        expect(target.Field).toBe("1");
        combo.setEditValue({ Field: 2 }, { name: "Field" } as any);
        expect(combo["combobox"].setValue).toHaveBeenCalledWith("2", true);
        combo.destroy();
    });

    it("manages items and item lookup", () => {
        class TestCombo extends ComboboxEditor<any, any> { }
        const combo = new TestCombo({});
        combo.clearItems();
        combo.addItem({ id: "1", text: "One", source: { Id: 1 } });
        combo.addOption("2", "Two", { Id: 2 }, true);
        expect(combo.items).toHaveLength(2);
        expect((combo as any).itemById["1"].source).toEqual({ Id: 1 });
        combo.items = [{ id: "3", text: "Three", source: { Id: 3 } }];
        expect(combo.items[0].id).toBe("3");
        combo["itemById"] = {};
        expect((combo as any).itemById).toEqual({});
        combo.destroy();
    });

    it("runs synchronous combobox search and initSelection callbacks", () => {
        class TestCombo extends ComboboxEditor<any, any> { }
        const combo = new TestCombo({ minimumResultsForSearch: 0 });
        combo.items = [
            { id: "1", text: "Alpha", source: { Id: 1 } },
            { id: "2", text: "Beta", source: { Id: 2 } }
        ];
        const options = (combo as any).getComboboxOptions();
        expect(options.minimumResultsForSearch).toBe(0);
        const searchResult = options.search({ searchTerm: "alp", skip: 0, take: 10 });
        expect(searchResult.items).toHaveLength(1);
        expect(options.search({ initSelection: true, idList: ["2"], skip: 0 }).items[0].id).toBe("2");
        combo.destroy();
    });

    it("supports autocomplete missing ids and createSearchChoice", () => {
        class TestCombo extends ComboboxEditor<any, any> {
            protected isAutoComplete() { return true; }
        }
        const combo = new TestCombo({});
        combo.items = [{ id: "1", text: "Alpha" }];
        const options = (combo as any).getComboboxOptions();
        const result = options.search({ initSelection: true, idList: ["1", "2"], skip: 0 });
        expect(result.items.map((x: any) => x.id)).toContain("2");
        expect(options.createSearchChoice).toBeTypeOf("function");
        expect(options.createSearchChoice("New").id).toBe("New");
        expect(options.createSearchChoice("Alpha")).toBeNull();
        combo.destroy();
    });

    it("covers value, selected item, text, and readonly helpers", () => {
        class TestCombo extends ComboboxEditor<any, any> { }
        const combo = new TestCombo({});
        combo["combobox"] = {
            getValue: vi.fn(() => "1"),
            setValue: vi.fn(),
            getValues: vi.fn(() => ["1", "2"]),
            setValues: vi.fn(),
            dispose: vi.fn()
        } as any;
        combo["_itemById"] = {
            "1": { id: "1", text: "One", source: { Id: 1 } },
            "2": { id: "2", text: "Two" }
        };
        expect(combo.value).toBe("1");
        combo.value = "2";
        expect(combo["combobox"].setValue).toHaveBeenCalledWith("2", true);
        expect(combo.selectedItem).toEqual({ Id: 1 });
        expect(combo.selectedItems).toEqual([{ Id: 1 }, null]);
        expect(combo.values).toEqual(["1", "2"]);
        combo.values = ["2"];
        expect(combo["combobox"].setValues).toHaveBeenCalledWith(["2"]);
        expect(combo.text).toBe("");
        combo.readOnly = true;
        expect(combo.readOnly).toBe(true);
        combo.destroy();
    });

    it("handles edit value arrays and delimited mode", () => {
        class TestCombo extends ComboboxEditor<any, any> {
            protected isMultiple() { return true; }
        }
        const combo = new TestCombo({ delimited: false });
        combo["combobox"] = { getValue: vi.fn(() => "1,2"), getValues: vi.fn(() => ["1", "2"]), setValues: vi.fn(), setValue: vi.fn(), dispose: vi.fn() } as any;
        const target: any = {};
        combo.getEditValue({ name: "Field" } as any, target);
        expect(target.Field).toEqual(["1", "2"]);
        combo.setEditValue({ Field: ["3", "4"] }, { name: "Field" } as any);
        expect(combo["combobox"].setValues).toHaveBeenCalledWith(["3", "4"]);
        combo.destroy();
    });

    it("filters and cascades item collections", () => {
        class TestCombo extends ComboboxEditor<any, any> {
            public updateItems() { }
        }
        const combo = new TestCombo({ cascadeField: "Parent", filterField: "Kind" } as any);
        const items = [{ Parent: 1, Kind: "A" }, { Parent: 1, Kind: "B" }, { Parent: 2, Kind: "A" }];
        expect((combo as any).cascadeItems(items)).toEqual([]);
        combo.cascadeValue = 1;
        combo.filterValue = "A";
        expect((combo as any).cascadeItems(items)).toHaveLength(2);
        expect((combo as any).filterItems(items)).toEqual([{ Parent: 1, Kind: "A" }, { Parent: 2, Kind: "A" }]);
        combo.cascadeFrom = null;
        expect(combo.cascadeFrom).toBeNull();
        combo.cascadeField = "Other";
        combo.filterField = "OtherKind";
        expect(combo.cascadeField).toBe("Other");
        expect(combo.filterField).toBe("OtherKind");
        combo.destroy();
    });

    it("maps async search results and autocomplete missing ids", async () => {
        class AsyncCombo extends ComboboxEditor<any, any> {
            protected hasAsyncSource() { return true; }
            protected asyncSearch() {
                return Promise.resolve({ items: [{ Id: 1, Name: "One" }], more: false });
            }
            protected getIdField() { return "Id"; }
            protected getTextField() { return "Name"; }
            protected isAutoComplete() { return true; }
        }
        const combo = new AsyncCombo({ autoComplete: true } as any);
        const options = (combo as any).getComboboxOptions();
        const result = await options.search({ initSelection: true, idList: ["1", "2"] });
        expect(result.items.map((x: any) => x.id)).toEqual(["1", "2"]);
        expect((combo as any)._itemById["2"]).toBeDefined();
        combo.destroy();
    });

    it("resolves dialog types and initializes new entities", () => {
        class TestCombo extends ComboboxEditor<any, any> { }
        const combo = new TestCombo({ dialogType: "MyDialog", cascadeField: "Parent", cascadeValue: 4, filterField: "Kind", filterValue: "A" } as any);
        expect((combo as any).getDialogTypeKey()).toBe("MyDialog");
        const type = class { };
        const registrySpy = vi.spyOn(DialogTypeRegistry, "getOrLoad").mockReturnValue(type as any);
        expect((combo as any).getDialogType()).toBe(type);
        const entity: any = {};
        combo["onInitNewEntity"] = vi.fn((x: any) => x.Extra = true);
        (combo as any).initNewEntity(entity);
        expect(entity).toEqual({ Parent: 4, Kind: "A", Extra: true });
        registrySpy.mockRestore();
        combo.destroy();
    });

    it("sets a term on a new entity from a dialog name property", () => {
        class TestCombo extends ComboboxEditor<any, any> { }
        const combo = new TestCombo({});
        const entity: any = {};
        (combo as any).setTermOnNewEntity(entity, "New term", { getNameProperty: () => "Name" });
        expect(entity.Name).toBe("New term");
        (combo as any).setTermOnNewEntity(entity, "Ignored", {});
        combo.destroy();
    });

    it("returns null for missing dialog type and resolves object dialog type", () => {
        class TestCombo extends ComboboxEditor<any, any> { }
        const combo = new TestCombo({});
        expect((combo as any).getDialogType()).toBeNull();
        const dialogType = class { };
        const objectCombo = new TestCombo({ dialogType } as any);
        expect((objectCombo as any).getDialogType()).toBe(dialogType);
        combo.destroy();
        objectCombo.destroy();
    });

    it("creates edit dialogs for synchronous and asynchronous types", async () => {
        class DialogType {
            init() { return this; }
        }
        class TestCombo extends ComboboxEditor<any, any> { }
        const combo = new TestCombo({ dialogType: DialogType } as any);
        const callback = vi.fn();
        (combo as any).createEditDialog(callback);
        expect(callback).toHaveBeenCalled();

        const asyncCombo = new TestCombo({ dialogType: Promise.resolve(DialogType) } as any);
        const asyncCallback = vi.fn();
        (asyncCombo as any).createEditDialog(asyncCallback);
        await Promise.resolve();
        expect(asyncCallback).toHaveBeenCalled();
        combo.destroy();
        asyncCombo.destroy();
    });

    it("sets edit dialog delete button disabled", () => {
        class TestCombo extends ComboboxEditor<any, any> { }
        const combo = new TestCombo({});
        const button = document.createElement("button");
        const fluent = { addClass: vi.fn(), off: vi.fn() };
        fluent.addClass.mockReturnValue(fluent);
        const dialog = { element: {
            findFirst: vi.fn(() => fluent)
        } };
        (combo as any).setEditDialogReadOnly(dialog);
        expect(dialog.element.findFirst).toHaveBeenCalledWith(".tool-button.delete-button");
        combo.destroy();
    });

    it("inplaceCreateClick returns early for readonly empty value", () => {
        class TestCombo extends ComboboxEditor<any, any> { }
        const combo = new TestCombo({});
        combo.readOnly = true;
        const createSpy = vi.spyOn(combo as any, "createEditDialog");
        (combo as any).inplaceCreateClick(new Event("click"));
        expect(createSpy).not.toHaveBeenCalled();
        combo.destroy();
    });

    it("handles inplace create and data changes for a single value", () => {
        class TestCombo extends ComboboxEditor<any, any> { }
        const combo = new TestCombo({});
        combo["combobox"] = {
            getValue: vi.fn(() => ""),
            setValue: vi.fn(),
            getValues: vi.fn(() => []),
            setValues: vi.fn(),
            dispose: vi.fn()
        } as any;
        const dialog = {
            load: vi.fn((_entity: any, done: Function) => done()),
            dialogOpen: vi.fn(),
            element: {}
        };
        const changeHandler = vi.fn();
        const bindSpy = vi.spyOn(SubDialogHelper, "bindToDataChange").mockImplementation((_dialog, _owner, handler) => {
            changeHandler.mockImplementation(handler);
        });
        vi.spyOn(combo as any, "createEditDialog").mockImplementation((callback: Function) => callback(dialog));
        vi.spyOn(combo as any, "updateItems").mockImplementation(() => { });

        (combo as any).lastCreateTerm = "New value";
        (combo as any).inplaceCreateClick(new Event("click"));
        expect(dialog.load).toHaveBeenCalled();
        expect(dialog.dialogOpen).toHaveBeenCalled();
        expect(bindSpy).toHaveBeenCalled();

        changeHandler({ operationType: "create", entityId: 7 });
        expect(combo["combobox"].setValue).toHaveBeenCalledWith("7", true);
        changeHandler({ operationType: "delete", entityId: 7 });
        expect(combo["combobox"].setValue).toHaveBeenCalled();
        combo.destroy();
    });

    it("covers create-search-choice result variants", () => {
        class NormalCombo extends ComboboxEditor<any, any> { }
        const normal = new NormalCombo({});
        normal.items = [{ id: "1", text: "Alpha", source: { Name: "Alpha" } }];
        const normalChoice = normal.getCreateSearchChoice((x: any) => x.Name);
        expect(normalChoice("")).toBeNull();
        expect(normalChoice("Alpha")).toBeNull();
        expect(normalChoice("New").id).toBe("-2147483648");
        expect(normalChoice("Al").id).toBe("-2147483648");
        normal.destroy();

        class AutoCombo extends ComboboxEditor<any, any> {
            protected isAutoComplete() { return true; }
        }
        const auto = new AutoCombo({});
        auto.items = [{ id: "1", text: "Alpha" }];
        const autoChoice = auto.getCreateSearchChoice(null);
        expect(autoChoice("New")).toEqual({ id: "New", text: "New" });
        expect(autoChoice("Al")).toEqual({ id: "Al", text: "Al" });
        auto.destroy();
    });

    it("covers inplace add permission and change handlers", async () => {
        const authorizationSpy = vi.spyOn((await import("../../base")).Authorization, "hasPermission").mockReturnValue(false);
        class TestCombo extends ComboboxEditor<any, any> {
            protected isMultiple() { return false; }
        }
        const combo = new TestCombo({ inplaceAdd: true, inplaceAddPermission: "Create" } as any);
        expect((combo as any).useInplaceAdd()).toBe(false);
        const combo2 = new TestCombo({ inplaceAdd: true } as any);
        const inplace = combo2.domNode.nextElementSibling as HTMLElement;
        expect(inplace).toBeTruthy();
        const createSpy = vi.spyOn(combo2 as any, "inplaceCreateClick").mockImplementation(() => { });
        combo2["combobox"] = { getValue: vi.fn(() => "-2147483648"), setValue: vi.fn(), getValues: vi.fn(() => []), setValues: vi.fn(), dispose: vi.fn() } as any;
        combo2.domNode.dispatchEvent(new Event("change"));
        expect(createSpy).toHaveBeenCalled();
        combo.destroy();
        combo2.destroy();
        authorizationSpy.mockRestore();
    });

    it("covers option and field accessors", () => {
        class TestCombo extends ComboboxEditor<any, any> { }
        const combo = new TestCombo({
            typeDelay: 50,
            pageSize: 25,
            idField: "Id",
            textField: "Name",
            multiple: true,
            delimited: true,
            allowClear: false,
            cascadeField: "Parent",
            cascadeValue: 1,
            filterField: "Kind",
            filterValue: "A"
        } as any);
        expect((combo as any).getTypeDelay()).toBe(50);
        expect((combo as any).getPageSize()).toBe(25);
        expect((combo as any).getIdField()).toBe("Id");
        expect((combo as any).getTextField()).toBe("Name");
        expect((combo as any).itemDisabled({})).toBe(false);
        expect((combo as any).mapItems([{ Id: 1, Name: "One" }])[0].text).toBe("One");
        expect((combo as any).allowClear()).toBe(false);
        expect((combo as any).isMultiple()).toBe(true);
        expect((combo as any).get_delimited()).toBe(true);
        expect(combo.cascadeValue).toBe(1);
        expect(combo.filterValue).toBe("A");
        expect((combo as any).get_items()).toEqual([]);
        expect((combo as any).get_itemByKey()).toEqual({});
        expect((combo as any).getComboboxContainer()).toBeTruthy();
        combo.destroy();
    });

    it("covers cascade and filter value updates", () => {
        class TestCombo extends ComboboxEditor<any, any> {
            updateItems = vi.fn();
        }
        const combo = new TestCombo({});
        combo.cascadeField = "Parent";
        combo.cascadeValue = 2;
        combo.filterField = "Kind";
        combo.filterValue = "B";
        expect(combo.cascadeField).toBe("Parent");
        expect(combo.cascadeValue).toBe(2);
        expect(combo.filterField).toBe("Kind");
        expect(combo.filterValue).toBe("B");
        expect(combo.updateItems).toHaveBeenCalled();
        combo.set_readOnly(true);
        expect(combo.readOnly).toBe(true);
        combo.destroy();
    });

    it("handles multi-select in-place create, update, and delete changes", () => {
        class MultiCombo extends ComboboxEditor<any, any> {
            protected isMultiple() { return true; }
        }
        const combo = new MultiCombo({});
        const values = ["1"];
        combo["combobox"] = {
            getValue: vi.fn(() => values.join(",")),
            setValue: vi.fn(),
            getValues: vi.fn(() => values.slice()),
            setValues: vi.fn((next: string[]) => { values.splice(0, values.length, ...(next ?? [])); }),
            dispose: vi.fn()
        } as any;
        const dialog = { load: vi.fn((_entity: any, done: Function) => done()), dialogOpen: vi.fn(), element: {} };
        let changeHandler: Function;
        vi.spyOn(SubDialogHelper, "bindToDataChange").mockImplementation((_d, _o, handler) => { changeHandler = handler; });
        vi.spyOn(combo as any, "createEditDialog").mockImplementation((callback: Function) => callback(dialog));
        vi.spyOn(combo as any, "updateItems").mockImplementation(() => { });
        (combo as any).inplaceCreateClick(new Event("click"));

        changeHandler({ operationType: "create", entityId: 2 });
        expect(values).toEqual(["1", "2"]);
        changeHandler({ operationType: "update", entityId: 3 });
        expect(values).toContain("3");
        changeHandler({ operationType: "delete", entityId: 2 });
        expect(values).not.toContain("2");
        combo.destroy();
    });

    it("updates an in-place button when readonly changes", () => {
        class TestCombo extends ComboboxEditor<any, any> { }
        const combo = new TestCombo({});
        const button = document.createElement("a");
        button.className = "inplace-create";
        combo.domNode.after(button);
        combo.readOnly = true;
        expect(button.getAttribute("disabled")).toBe("disabled");
        expect(button.style.opacity).toBe("0.1");
        combo.readOnly = false;
        expect(button.style.opacity).toBe("");
        combo.destroy();
    });

    it("forwards dropdown opening to the combobox", () => {
        class TestCombo extends ComboboxEditor<any, any> { }
        const combo = new TestCombo({});
        const openDropdown = vi.fn();
        const instanceSpy = vi.spyOn(Combobox, "getInstance").mockReturnValue({ openDropdown } as any);
        combo.openDropdown();
        expect(openDropdown).toHaveBeenCalled();
        instanceSpy.mockRestore();
        combo.destroy();
    });

    it("returns the default empty async search result", async () => {
        class TestCombo extends ComboboxEditor<any, any> { }
        const combo = new TestCombo({});
        await expect((combo as any).asyncSearch({})).resolves.toEqual({ items: [], more: false });
        combo.destroy();
    });

    it("forwards abortPendingQuery to the combobox", () => {
        class TestCombo extends ComboboxEditor<any, any> { }
        const combo = new TestCombo({});
        const abortPendingQuery = vi.fn();
        combo["combobox"] = { abortPendingQuery, dispose: vi.fn() } as any;
        (combo as any).abortPendingQuery();
        expect(abortPendingQuery).toHaveBeenCalled();
        combo.destroy();
    });
});