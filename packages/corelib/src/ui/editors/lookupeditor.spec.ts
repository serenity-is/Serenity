import { Lookup } from "../../q/lookup";
import { ScriptData } from "../../q/scriptdata";
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
    test('throws an error if element is null', () => {
        ScriptData.set("Lookup.Test", { items: [] });

        expect(() => new LookupEditor(null, {
            lookupKey: "Test"
        })).toThrowError();
    });

    test('throws an error if lookupKey is not registered', () => {
        ScriptData.set("Lookup.Test", null);

        expect(() => new LookupEditor($("<input />"), {
            lookupKey: "Test"
        })).toThrowError('No lookup with key "Test" is registered. Please make sure you have a [LookupScript("Test")] attribute in server side code on top of a row / custom lookup and  its key is exactly the same.');
    });

    test('doesn\'t throw an error if lookupKey is registered', () => {
        ScriptData.set("Lookup.Test", {
            items: [{ id: 1, text: "Test" }]
        });

        new LookupEditor($("<input />"), {
            lookupKey: "Test"
        });
    });

    test('sets input type to text', () => {
        ScriptData.set("Lookup.Test", {
            items: []
        });

        const editor = new LookupEditor($("<input type='number' />"), {
            lookupKey: "Test"
        });

        expect(editor.element.attr("type")).toBe("text");
    });

    test('sets placeholder to default if its null', () => {
        ScriptData.set("Lookup.Test", {
            items: []
        });

        const editor = new LookupEditor($("<input />"), {
            lookupKey: "Test"
        });

        expect(editor.element.attr("placeholder")).toBe("Controls.SelectEditor.EmptyItemText");
    });

    test('doesnt set placeholder if its not null', () => {
        ScriptData.set("Lookup.Test", {
            items: []
        });

        const editor = new LookupEditor($("<input placeholder='test' />"), {
            lookupKey: "Test"
        });

        expect(editor.element.attr("placeholder")).toBe("test");
    });

    test('creates inplaceAdd button if its enabled', () => {
        ScriptData.set("Lookup.Test", {
            items: []
        });

        const editor = new LookupEditor($("<input />"), {
            lookupKey: "Test",
            inplaceAdd: true
        });

        expect(editor.element.hasClass("has-inplace-button")).toBe(true);
    });

    test('doesn\'t create inplaceAdd button if its disabled', () => {
        ScriptData.set("Lookup.Test", {
            items: []
        });

        const editor = new LookupEditor($("<input />"), {
            lookupKey: "Test"
        });

        expect(editor.element.hasClass("has-inplace-button")).toBe(false);
    });

    test('inplaceAdd button has correct text depending on selection', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{
            idField: "id",
            items: [
                { id: 1, text: "Test" }
            ]
        });

        const select2Container = $(`<div class="select2-container"><input /></div>`);
        document.body.appendChild(select2Container[0]);
        const editorInput = select2Container.find("input");

        const editor = new LookupEditor(editorInput, {
            lookupKey: "Test",
            inplaceAdd: true
        });

        expect(select2Container.find(".inplace-button").attr('title')).toBe("Controls.SelectEditor.InplaceAdd");

        editor.value = "1";
        expect(select2Container.find(".inplace-button").attr('title')).toBe("Controls.SelectEditor.InplaceEdit");
    });

    test('can load empty lookup', () => {
        ScriptData.set("Lookup.Test", {
            items: []
        });

        const editor = new LookupEditor($("<input />"), {
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(0);
    });

    test('can load lookup with items', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{
            items: [{ id: 1, text: "Test" }]
        });

        const editor = new LookupEditor($("<input />"), {
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(1);
    });

    test('can load lookup with multiple items', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{
            items: [
                { id: 1, text: "Test" },
                { id: 2, text: "Test2" }
            ]
        });

        const editor = new LookupEditor($("<input />"), {
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(2);
    });

    test('appends original lookup item to source', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{
            items: [{ id: 1, text: "Test" }]
        });

        const editor = new LookupEditor($("<input />"), {
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(1);
        expect(editor.items[0].source).toStrictEqual({ id: 1, text: "Test" });
    });

    test('doesn\'t load id if idField is not set', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{
            items: [{ id: 1, text: "Test" }]
        });

        const editor = new LookupEditor($("<input />"), {
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(1);
        expect(editor.items[0].id).toBe("");
    });

    test('doesn\'t load text if textField is not set', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{
            items: [{ id: 1, text: "Test" }]
        });

        const editor = new LookupEditor($("<input />"), {
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(1);
        expect(editor.items[0].text).toBe("");
    });

    test('can load lookup with idField', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{
            items: [{ id: 1, text: "Test" }],
            idField: "id"
        });

        const editor = new LookupEditor($("<input />"), {
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(1);
        expect(editor.items[0].id).toBe("1");
    });

    test('can load lookup with textField', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{
            items: [{ id: 1, text: "Test" }],
            textField: "text"
        });

        const editor = new LookupEditor($("<input />"), {
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(1);
        expect(editor.items[0].text).toBe("Test");
    });

    test('can load lookup with idField and textField', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{
            items: [{ id: 1, text: "Test" }],
            idField: "id",
            textField: "text"
        });

        const editor = new LookupEditor($("<input />"), {
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(1);
        expect(editor.items[0].id).toBe("1");
        expect(editor.items[0].text).toBe("Test");
    });

    test('can cascade lookup with cascadeField', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{
            items: [
                { id: 1, text: "Test", parentId: 1 },
                { id: 2, text: "Test", parentId: 2 }
            ]
        });

        const editor = new LookupEditor($("<input />"), {
            lookupKey: "Test",
            cascadeField: "parentId",
            cascadeValue: 1,
        });

        expect(editor.items).toHaveLength(1);
    });

    test('can cascade lookup with cascadeFrom', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{
            idField: "id",
            items: [
                { id: 1, parentId: 10 },
                { id: 2, parentId: 20 }
            ]
        });

        ScriptData.set("Lookup.TestParent", <Lookup<any>>{
            idField: "id",
            items: [
                { id: 10 },
                { id: 20 }
            ]
        });

        const parentIdInput = $("<input id='parentId' />");
        document.body.appendChild(parentIdInput[0]);

        const parentIdEditor = new LookupEditor(parentIdInput, {
            lookupKey: "TestParent",
        });

        const editor = new LookupEditor($("<input />"), {
            lookupKey: "Test",
            cascadeFrom: "parentId"
        });

        expect(editor.items).toHaveLength(0);

        parentIdEditor.value = "10";

        expect(editor.items).toHaveLength(1);
        expect(editor.items[0].id).toBe("1");
    });

    test('can filter lookup', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{
            idField: "id",
            items: [
                { id: 1, text: "Test" },
                { id: 2, text: "Test2" }
            ]
        });

        const editor = new LookupEditor($("<input />"), {
            lookupKey: "Test",
            filterField: "text",
            filterValue: "Test2"
        });

        expect(editor.items).toHaveLength(1);
        expect(editor.items[0].id).toBe("2");
    });

    test('can show text using textFormatter', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{
            idField: "id",
            textFormatter: (item) => item.text + "!",
            items: [
                { id: 1, text: "Test" },
                { id: 2, text: "Test2" }
            ]
        });

        const editor = new LookupEditor($("<input />"), {
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(2);
        expect(editor.items[0].text).toBe("Test!");
        expect(editor.items[1].text).toBe("Test2!");
    });

    test('can update items when scriptData changes', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{
            idField: "id",
            items: [
                { id: 1, text: "Test" },
                { id: 2, text: "Test2" }
            ]
        });

        const editor = new LookupEditor($("<input />"), {
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(2);

        ScriptData.set("Lookup.Test", <Lookup<any>>{
            idField: "id",
            items: [
                { id: 1, text: "Test" },
                { id: 2, text: "Test2" },
                { id: 3, text: "Test3" }
            ]
        });

        expect(editor.items).toHaveLength(3);
    });

    test('can update items when scriptData changes and cascadeFrom is set', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{
            idField: "id",
            items: [
                { id: 1, text: "Test", parentId: 1 },
                { id: 2, text: "Test", parentId: 2 }
            ]
        });

        ScriptData.set("Lookup.TestParent", <Lookup<any>>{
            idField: "id",
            items: [
                { id: 1 },
                { id: 2 }
            ]
        });

        const parentIdInput = $("<input id='parentId' />");
        document.body.appendChild(parentIdInput[0]);

        const parentIdEditor = new LookupEditor(parentIdInput, {
            lookupKey: "TestParent",
        });

        const editor = new LookupEditor($("<input />"), {
            lookupKey: "Test",
            cascadeFrom: "parentId"
        });

        expect(editor.items).toHaveLength(0);

        parentIdEditor.value = "1";

        expect(editor.items).toHaveLength(1);

        ScriptData.set("Lookup.Test", <Lookup<any>>{
            idField: "id",
            items: [
                { id: 1, text: "Test", parentId: 1 },
                { id: 2, text: "Test", parentId: 2 },
                { id: 3, text: "Test", parentId: 1 }
            ]
        });

        expect(editor.items).toHaveLength(2);
    });

    test('can filter items when input value changes', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{
            idField: "id",
            items: [
                { id: 1, text: "Test" },
                { id: 2, text: "Test2" }
            ]
        });

        const select2Container = $(`<div class="select2-container"><input /></div>`);
        document.body.appendChild(select2Container[0]);
        const editorInput = select2Container.find("input");

        document.body.appendChild(editorInput[0]);
        const editor = new LookupEditor(editorInput, {
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(2);

        $(document.body.querySelector(".select2-choice")).trigger("mousedown");

        let options = Array.from(document.body.querySelectorAll(".select2-results li"));
        expect(options).toHaveLength(2);

        const input = document.body.querySelector(".select2-input") as HTMLInputElement
        input.value = "Test2";
        $(input).trigger("input");

        expect(document.body.querySelector(".select2-active")).toBeNull();

        options = Array.from(document.body.querySelectorAll(".select2-results li"));
        expect(options).toHaveLength(1);
    });

    test('correctly unbinds from scriptData change event on destroy', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{
            idField: "id",
            items: [
                { id: 1, text: "Test" },
                { id: 2, text: "Test2" }
            ]
        });

        const editor = new LookupEditor($("<input />"), {
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(2);
        editor.destroy();

        ScriptData.set("Lookup.Test", <Lookup<any>>{
            items: []
        });

        // Should not throw error
    });

    test('throws error if editor is async and items getter or setter is accessed', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{ items: [] });

        const editor = new LookupEditor($("<input />"), {
            lookupKey: "Test",
            async: true
        });

        expect(() => editor.items).toThrowError("Can't read items property of an async select editor!");
        expect(() => editor.items = []).toThrowError("Can't set items of an async select editor!");
    });

});