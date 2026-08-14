import { Fluent } from "../../base";
import { ScriptData } from "../../compat";
import { LookupEditor } from "./lookupeditor";

let oldWindowAlert: any;
beforeAll(() => {
    oldWindowAlert = window.alert;
    window.alert = () => { };
});

afterAll(() => {
    window.alert = oldWindowAlert;
});

afterEach(() => {
    document.body.innerHTML = "";
});

describe("LookupEditor", () => {

    it('throws an error if lookupKey is not registered', () => {
        ScriptData.set("Lookup.Test", null);
        var logSpy = vi.spyOn(window.console, 'log').mockImplementation(() => { });
        var oldXHR = window.XMLHttpRequest
        try {
            window.XMLHttpRequest = class {
                open() { }
                send() {
                    (this as any).status = 404;
                }
            } as any;
            expect(() => new LookupEditor({
                lookupKey: "Test"
            })).toThrow('No lookup with key "Test" is registered. Please make sure you have a [LookupScript("Test")] attribute in server side code on top of a row / custom lookup and its key is exactly the same.');
        }
        finally {
            logSpy.mockRestore();
            window.XMLHttpRequest = oldXHR;
        }
    });

    it('doesn\'t throw an error if lookupKey is registered', () => {
        ScriptData.set("Lookup.Test", {
            items: [{ id: 1, text: "Test" }]
        });

        new LookupEditor({
            lookupKey: "Test"
        });
    });

    it('sets placeholder to default if its null', () => {
        ScriptData.set("Lookup.Test", {
            items: []
        });

        const editor = new LookupEditor({
            lookupKey: "Test"
        });

        expect(editor.domNode.getAttribute("placeholder")).toBe("Controls.SelectEditor.EmptyItemText");
    });

    it('doesn\'t set placeholder if its not null', () => {
        ScriptData.set("Lookup.Test", {
            items: []
        });

        const editor = new LookupEditor({
            lookupKey: "Test",
            element: el => el.setAttribute("placeholder", "test")
        });

        expect(editor.domNode.getAttribute("placeholder")).toBe("test");
    });

    it('creates inplaceAdd button if its enabled', () => {
        ScriptData.set("Lookup.Test", {
            items: []
        });

        const editor = new LookupEditor({
            lookupKey: "Test",
            inplaceAdd: true
        });

        expect(editor.domNode.classList.contains("has-inplace-button")).toBe(true);
    });

    it('doesn\'t create inplaceAdd button if its disabled', () => {
        ScriptData.set("Lookup.Test", {
            items: []
        });

        const editor = new LookupEditor({
            lookupKey: "Test"
        });

        expect(editor.domNode.classList.contains("has-inplace-button")).toBe(false);
    });

    it('inplaceAdd button has correct text depending on selection', () => {
        ScriptData.set("Lookup.Test", {
            idField: "id",
            items: [
                { id: 1, text: "Test" }
            ]
        });

        const select2Container = Fluent(<div class="select2-container" />);
        document.body.appendChild(select2Container[0]);

        const editor = new LookupEditor({
            lookupKey: "Test",
            inplaceAdd: true,
            element: el => select2Container.append(el)
        });

        expect(select2Container.findFirst(".inplace-button").attr('title')).toBe("Controls.SelectEditor.InplaceAdd");
        editor.value = "1";
        expect(select2Container.findFirst(".inplace-button").attr('title')).toBe("Controls.SelectEditor.InplaceEdit");
    });

    it('can load empty lookup', () => {
        ScriptData.set("Lookup.Test", {
            items: []
        });

        const editor = new LookupEditor({
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(0);
    });

    it('can load lookup with items', () => {
        ScriptData.set("Lookup.Test", {
            items: [{ id: 1, text: "Test" }]
        });

        const editor = new LookupEditor({
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(1);
    });

    it('can load lookup with multiple items', () => {
        ScriptData.set("Lookup.Test", {
            items: [
                { id: 1, text: "Test" },
                { id: 2, text: "Test2" }
            ]
        });

        const editor = new LookupEditor({
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(2);
    });

    it('appends original lookup item to source', () => {
        ScriptData.set("Lookup.Test", {
            items: [{ id: 1, text: "Test" }]
        });

        const editor = new LookupEditor({
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(1);
        expect(editor.items[0].source).toStrictEqual({ id: 1, text: "Test" });
    });

    it('doesn\'t load id if idField is not set', () => {
        ScriptData.set("Lookup.Test", {
            items: [{ id: 1, text: "Test" }]
        });

        const editor = new LookupEditor({
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(1);
        expect(editor.items[0].id).toBe("");
    });

    it('doesn\'t load text if textField is not set', () => {
        ScriptData.set("Lookup.Test", {
            items: [{ id: 1, text: "Test" }]
        });

        const editor = new LookupEditor({
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(1);
        expect(editor.items[0].text).toBe("");
    });

    it('can load lookup with idField', () => {
        ScriptData.set("Lookup.Test", {
            items: [{ id: 1, text: "Test" }],
            idField: "id"
        });

        const editor = new LookupEditor({
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(1);
        expect(editor.items[0].id).toBe("1");
    });

    it('can load lookup with textField', () => {
        ScriptData.set("Lookup.Test", {
            items: [{ id: 1, text: "Test" }],
            textField: "text"
        });

        const editor = new LookupEditor({
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(1);
        expect(editor.items[0].text).toBe("Test");
    });

    it('can load lookup with idField and textField', () => {
        ScriptData.set("Lookup.Test", {
            items: [{ id: 1, text: "Test" }],
            idField: "id",
            textField: "text"
        });

        const editor = new LookupEditor({
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(1);
        expect(editor.items[0].id).toBe("1");
        expect(editor.items[0].text).toBe("Test");
    });

    it('can cascade lookup with cascadeField', () => {
        ScriptData.set("Lookup.Test", {
            items: [
                { id: 1, text: "Test", parentId: 1 },
                { id: 2, text: "Test", parentId: 2 }
            ]
        });

        const editor = new LookupEditor({
            lookupKey: "Test",
            cascadeField: "parentId",
            cascadeValue: 1,
        });

        expect(editor.items).toHaveLength(1);
    });

    it('can cascade lookup with cascadeFrom', () => {
        ScriptData.set("Lookup.Test", {
            idField: "id",
            items: [
                { id: 1, parentId: 10 },
                { id: 2, parentId: 20 }
            ]
        });

        ScriptData.set("Lookup.TestParent", {
            idField: "id",
            items: [
                { id: 10 },
                { id: 20 }
            ]
        });

        const parentIdEditor = new LookupEditor({
            lookupKey: "TestParent",
            id: "parentId",
            element: el => document.body.append(el)
        });

        const editor = new LookupEditor({
            lookupKey: "Test",
            cascadeFrom: "parentId"
        });

        expect(editor.items).toHaveLength(0);

        parentIdEditor.value = "10";

        expect(editor.items).toHaveLength(1);
        expect(editor.items[0].id).toBe("1");
    });

    it('can filter lookup', () => {
        ScriptData.set("Lookup.Test", {
            idField: "id",
            items: [
                { id: 1, text: "Test" },
                { id: 2, text: "Test2" }
            ]
        });

        const editor = new LookupEditor({
            lookupKey: "Test",
            filterField: "text",
            filterValue: "Test2"
        });

        expect(editor.items).toHaveLength(1);
        expect(editor.items[0].id).toBe("2");
    });

    it('can update items when scriptData changes', () => {
        ScriptData.set("Lookup.Test", {
            idField: "id",
            items: [
                { id: 1, text: "Test" },
                { id: 2, text: "Test2" }
            ]
        });

        const editor = new LookupEditor({
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(2);

        ScriptData.set("Lookup.Test", {
            idField: "id",
            items: [
                { id: 1, text: "Test" },
                { id: 2, text: "Test2" },
                { id: 3, text: "Test3" }
            ]
        });

        expect(editor.items).toHaveLength(3);
    });

    it('can update items when scriptData changes and cascadeFrom is set', () => {
        ScriptData.set("Lookup.Test", {
            idField: "id",
            items: [
                { id: 1, text: "Test", parentId: 1 },
                { id: 2, text: "Test", parentId: 2 }
            ]
        });

        ScriptData.set("Lookup.TestParent", {
            idField: "id",
            items: [
                { id: 1 },
                { id: 2 }
            ]
        });

        const parentIdEditor = new LookupEditor({
            id: "parentId",
            lookupKey: "TestParent",
            element: el => document.body.appendChild(el)
        });

        const editor = new LookupEditor({
            lookupKey: "Test",
            cascadeFrom: "parentId",
            element: el => document.body.appendChild(el)
        })

        expect(editor.items).toHaveLength(0);

        parentIdEditor.value = "1";

        expect(editor.items).toHaveLength(1);

        ScriptData.set("Lookup.Test", {
            idField: "id",
            items: [
                { id: 1, text: "Test", parentId: 1 },
                { id: 2, text: "Test", parentId: 2 },
                { id: 3, text: "Test", parentId: 1 }
            ]
        });

        expect(editor.items).toHaveLength(2);
    });

    //    it('can filter items when input value changes', () => {
    //        ScriptData.set("Lookup.Test", {
    //            idField: "id",
    //            items: [
    //                { id: 1, text: "Test" },
    //                { id: 2, text: "Test2" }
    //            ]
    //        });//

    //        const editor = new LookupEditor({
    //            lookupKey: "Test",
    //            element: el => document.body.appendChild(el)
    //        });//

    //        expect(editor.items).toHaveLength(2);//

    //        Fluent.trigger(document.body.querySelector(".select2-choice"), "mousedown");
    //        
    //        let options = Array.from(document.body.querySelectorAll(".select2-results li"));
    //        expect(options).toHaveLength(2);//

    //        const input = document.body.querySelector(".select2-input") as HTMLInputElement
    //        input.value = "Test2";
    //        Fluent(input).trigger("input");//

    //        expect(document.body.querySelector(".select2-active")).toBeNull();//

    //        options = Array.from(document.body.querySelectorAll(".select2-results li"));
    //        expect(options).toHaveLength(1);
    //    });

    it('correctly unbinds from scriptData change event on destroy', () => {
        ScriptData.set("Lookup.Test", {
            idField: "id",
            items: [
                { id: 1, text: "Test" },
                { id: 2, text: "Test2" }
            ]
        });

        const editor = new LookupEditor({
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(2);
        editor.destroy();

        ScriptData.set("Lookup.Test", {
            items: []
        });

        // Should not throw error
    });

    it('throws error if editor is async and items getter or setter is accessed', () => {
        ScriptData.set("Lookup.Test", { items: [] });

        const editor = new LookupEditor({
            lookupKey: "Test",
            async: true
        });

        expect(() => editor.items).toThrow("Can't read items property of an async select editor!");
        expect(() => { editor.items = [] }).toThrow("Can't set items of an async select editor!");
    });

});

describe("LookupEditor additional behavior", () => {
    it("asyncSearch filters by ids, text, and paging", async () => {
        ScriptData.set("Lookup.AsyncTest", {
            idField: "Id",
            textField: "Name",
            items: [
                { Id: 1, Name: "Alpha" },
                { Id: 2, Name: "Beta" },
                { Id: 3, Name: "Alphabet" }
            ]
        });

        const editor = new LookupEditor({ lookupKey: "AsyncTest", async: true } as any);
        const result = await (editor as any).asyncSearch({
            idList: ["1", "3"],
            searchTerm: "alp",
            skip: 0,
            take: 1
        });
        expect(result.items).toHaveLength(1);
        expect(result.items[0]).toEqual({ Id: 1, Name: "Alpha" });
        expect(result.more).toBe(true);
        editor.destroy();
    });

    it("maps lookup items and handles null text", () => {
        ScriptData.set("Lookup.MapTest", {
            idField: "Id",
            textField: "Name",
            items: [{ Id: 1, Name: null }]
        });
        const editor = new LookupEditor({ lookupKey: "MapTest" } as any);
        expect(editor.items[0].id).toBe("1");
        expect(editor.items[0].text).toBe("");
        expect(editor.items[0].disabled).toBe(false);
        editor.destroy();
    });

    it("sets create term on the lookup text field", () => {
        ScriptData.set("Lookup.CreateTest", {
            idField: "Id",
            textField: "Name",
            items: []
        });
        const editor = new LookupEditor({ lookupKey: "CreateTest" } as any);
        const entity: any = {};
        (editor as any).setCreateTermOnNewEntity(entity, "Created");
        expect(entity.Name).toBe("Created");
        editor.destroy();
    });

    it("derives lookup key from a registered editor type", () => {
        ScriptData.set("Lookup.Explicit", { items: [] });
        class CustomLookupEditor extends LookupEditor<any> {
            static [Symbol.typeInfo] = this.registerEditor("Test.CustomLookupEditor");
        }
        const editor = new CustomLookupEditor({ lookupKey: "Explicit" } as any);
        expect((editor as any).getLookupKey()).toBe("Explicit");
        editor.destroy();
    });

    it("derives lookup key without an explicit option", () => {
        ScriptData.set("Lookup.Products", { idField: "Id", textField: "Name", items: [] });
        class ProductsEditor extends LookupEditor<any> {
            static [Symbol.typeInfo] = this.registerEditor("Test.ProductsEditor");
        }
        const editor = new ProductsEditor({} as any);
        expect((editor as any).getLookupKey()).toBe("Products");
        editor.destroy();
    });

    it("returns early from updateItems for async sources", () => {
        ScriptData.set("AsyncEarly", { items: [{ id: 1, text: "One" }] });
        const editor = new LookupEditor({ lookupKey: "AsyncEarly", async: true } as any);
        expect(() => editor.updateItems()).not.toThrow();
        editor.destroy();
    });

    it("supports asyncSearch without ids or a search term", async () => {
        class AsyncEditor extends LookupEditor<any> {
            protected override getLookupAsync() {
                return Promise.resolve({
                    idField: "Id",
                    textField: "Name",
                    items: [{ Id: 1, Name: "One" }, { Id: 2, Name: "Two" }]
                } as any);
            }
        }
        const editor = new AsyncEditor({ async: true } as any);
        const result = await (editor as any).asyncSearch({ skip: 0 });
        expect(result.items).toHaveLength(2);
        expect(result.more).toBeUndefined();
        editor.destroy();
    });

    it("uses base item text when lookup is null", () => {
        ScriptData.set("Lookup.BaseText", { items: [] });
        const editor = new LookupEditor({ lookupKey: "BaseText" } as any);
        expect((editor as any).getItemText({ Name: "Base" }, null)).toBe("");
        expect((editor as any).getIdField()).toBeUndefined();
        editor.destroy();
    });

    it("uses lookup key as dialog type fallback", () => {
        ScriptData.set("Lookup.DialogKey", { items: [] });
        const editor = new LookupEditor({ lookupKey: "DialogKey" } as any);
        expect((editor as any).getDialogTypeKey()).toBe("DialogKey");
        editor.destroy();
    });

    it("reloads lookup when edit dialog data changes", async () => {
        ScriptData.set("Lookup.ReloadKey", { items: [] });
        const reloadSpy = vi.spyOn(await import("../../compat"), "reloadLookup").mockImplementation((() => { }) as any);
        const editor = new LookupEditor({ lookupKey: "ReloadKey" } as any);
        (editor as any).editDialogDataChange();
        expect(reloadSpy).toHaveBeenCalledWith("ReloadKey");
        reloadSpy.mockRestore();
        editor.destroy();
    });
});