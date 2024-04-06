import { Fluent, Lookup } from "../../base";
import { ScriptData } from "../../q";
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

    test('throws an error if lookupKey is not registered', () => {
        ScriptData.set("Lookup.Test", null);
        var logSpy = jest.spyOn(window.console, 'log').mockImplementation(() => {});
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
            })).toThrow('No lookup with key "Test" is registered. Please make sure you have a [LookupScript("Test")] attribute in server side code on top of a row / custom lookup and  its key is exactly the same.');
        }
        finally {
            logSpy.mockRestore();
            window.XMLHttpRequest = oldXHR;
        }
    });

    test('doesn\'t throw an error if lookupKey is registered', () => {
        ScriptData.set("Lookup.Test", {
            items: [{ id: 1, text: "Test" }]
        });

        new LookupEditor({
            lookupKey: "Test"
        });
    });

    test('sets placeholder to default if its null', () => {
        ScriptData.set("Lookup.Test", {
            items: []
        });

        const editor = new LookupEditor({
            lookupKey: "Test"
        });

        expect(editor.domNode.getAttribute("placeholder")).toBe("Controls.SelectEditor.EmptyItemText");
    });

    test('doesn\'t set placeholder if its not null', () => {
        ScriptData.set("Lookup.Test", {
            items: []
        });

        const editor = new LookupEditor({
            lookupKey: "Test",
            element: el => el.setAttribute("placeholder", "test")
        });

        expect(editor.domNode.getAttribute("placeholder")).toBe("test");
    });

    test('creates inplaceAdd button if its enabled', () => {
        ScriptData.set("Lookup.Test", {
            items: []
        });

        const editor = new LookupEditor({
            lookupKey: "Test",
            inplaceAdd: true
        });

        expect(editor.domNode.classList.contains("has-inplace-button")).toBe(true);
    });

    test('doesn\'t create inplaceAdd button if its disabled', () => {
        ScriptData.set("Lookup.Test", {
            items: []
        });

        const editor = new LookupEditor({
            lookupKey: "Test"
        });

        expect(editor.domNode.classList.contains("has-inplace-button")).toBe(false);
    });

    test('inplaceAdd button has correct text depending on selection', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{
            idField: "id",
            items: [
                { id: 1, text: "Test" }
            ]
        });

        const select2Container = Fluent("div").class("select2-container");
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

    test('can load empty lookup', () => {
        ScriptData.set("Lookup.Test", {
            items: []
        });

        const editor = new LookupEditor({
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(0);
    });

    test('can load lookup with items', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{
            items: [{ id: 1, text: "Test" }]
        });

        const editor = new LookupEditor({
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

        const editor = new LookupEditor({
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(2);
    });

    test('appends original lookup item to source', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{
            items: [{ id: 1, text: "Test" }]
        });

        const editor = new LookupEditor({
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(1);
        expect(editor.items[0].source).toStrictEqual({ id: 1, text: "Test" });
    });

    test('doesn\'t load id if idField is not set', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{
            items: [{ id: 1, text: "Test" }]
        });

        const editor = new LookupEditor({
            lookupKey: "Test"
        });

        expect(editor.items).toHaveLength(1);
        expect(editor.items[0].id).toBe("");
    });

    test('doesn\'t load text if textField is not set', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{
            items: [{ id: 1, text: "Test" }]
        });

        const editor = new LookupEditor({
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

        const editor = new LookupEditor({
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

        const editor = new LookupEditor({
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

        const editor = new LookupEditor({
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

        const editor = new LookupEditor({
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

    test('can filter lookup', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{
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

    test('can update items when scriptData changes', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{
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

//    test('can filter items when input value changes', () => {
//        ScriptData.set("Lookup.Test", <Lookup<any>>{
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

    test('correctly unbinds from scriptData change event on destroy', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{
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

        ScriptData.set("Lookup.Test", <Lookup<any>>{
            items: []
        });

        // Should not throw error
    });

    test('throws error if editor is async and items getter or setter is accessed', () => {
        ScriptData.set("Lookup.Test", <Lookup<any>>{ items: [] });

        const editor = new LookupEditor({
            lookupKey: "Test",
            async: true
        });

        expect(() => editor.items).toThrow("Can't read items property of an async select editor!");
        expect(() => editor.items = []).toThrow("Can't set items of an async select editor!");
    });

});