import { afterEach, describe, expect, it } from "vitest";
import { Config } from "../../base";
import { StringEditor } from "../editors/stringeditor";
import { Widget } from "../widgets/widget";
import { EditorFiltering } from "./editorfiltering";

describe("EditorFiltering with an editor type that has to be loaded", () => {
    const orgLoader = Config.lazyTypeLoader;
    let keyId = 0;

    afterEach(() => {
        Config.lazyTypeLoader = orgLoader;
    });

    class LazyEditor extends StringEditor {
    }

    function setup(operator = "eq") {
        // types stay registered after they are loaded, so each test needs its own key
        const key = "Test.LazyFilteringEditor" + (++keyId);
        let resolve: () => void;
        const promise = new Promise(r => resolve = () => r(LazyEditor));
        Config.lazyTypeLoader = k => k === key ? promise : null;

        const filtering = new EditorFiltering({ editorType: key, useLike: true });
        const container = document.createElement("div");
        filtering.set_field({ name: "Test", title: "Test" });
        filtering.set_container(container);
        filtering.set_operator({ key: operator });

        return { filtering, container, key, load: async () => { resolve(); await new Promise(r => setTimeout(r)); } };
    }

    it("creates the editor when the type is loaded, where it was requested", async () => {
        const { filtering, container, load } = setup();
        container.append(document.createElement("hr"));
        filtering.createEditor();
        container.append(document.createElement("br"));

        expect((filtering as any).editor).toBeNull();
        expect(container.querySelector("input")).toBeNull();

        await load();

        expect((filtering as any).editor).toBeInstanceOf(LazyEditor);
        expect([...container.children].map(x => x.tagName)).toEqual(["HR", "INPUT", "BR"]);
        expect(container.childNodes.length).toBe(3);
    });

    it("applies the state loaded meanwhile to the editor", async () => {
        const { filtering, container, load } = setup();
        filtering.createEditor();
        filtering.loadState("saved");

        await load();

        expect(container.querySelector("input").value).toBe("saved");
        expect(filtering.saveState()).toBe("saved");
    });

    it("works with the state loaded meanwhile, so a search doesn't lose it", () => {
        const { filtering } = setup();
        filtering.createEditor();
        filtering.loadState("saved");

        expect(filtering.saveState()).toBe("saved");
        expect(filtering.getEditorValue()).toBe("saved");
        expect(filtering.getEditorText()).toBe("saved");
        expect(filtering.getCriteria().criteria).toEqual([["Test"], "=", "saved"]);
    });

    it.each([undefined, null, "", "  "])("reports a missing value while loading with state %j", state => {
        const { filtering } = setup();
        filtering.createEditor();
        filtering.loadState(state);

        expect(() => filtering.getEditorValue()).toThrow();
        expect(filtering.getEditorText()).toBe(state ?? "");
    });

    it("doesn't create the editor if the container is emptied meanwhile", async () => {
        const { filtering, container, load } = setup();
        filtering.createEditor();
        filtering.loadState("saved");
        container.innerHTML = "";

        await load();

        expect(container.childNodes.length).toBe(0);
        expect((filtering as any).editor).toBeNull();
    });

    it("creates one editor when called again meanwhile, without the state of the previous one", async () => {
        const { filtering, container, load } = setup();
        filtering.createEditor();
        filtering.loadState("old");
        filtering.createEditor();

        await load();

        expect(container.querySelectorAll("input").length).toBe(1);
        expect(container.querySelector("input").value).toBe("");
    });

    it("doesn't create the editor if the operator no longer uses one", async () => {
        const { filtering, container, load } = setup();
        filtering.createEditor();
        filtering.set_operator({ key: "isnull" });
        filtering.createEditor();

        await load();

        expect(container.childNodes.length).toBe(0);
        expect((filtering as any).editor).toBeNull();
    });

    it("uses the regular behavior for an operator not using the editor", () => {
        const { filtering, container } = setup("contains");
        filtering.createEditor();

        const input = container.querySelector("input");
        expect(input).toBeTruthy();
        filtering.loadState("text");
        expect(input.value).toBe("text");
        expect(filtering.saveState()).toBe("text");
    });

    it("creates the editor right away once the type is loaded", async () => {
        const { filtering, container, key, load } = setup();
        filtering.createEditor();
        await load();

        const other = new EditorFiltering({ editorType: key });
        const otherContainer = document.createElement("div");
        other.set_field({ name: "Test" });
        other.set_container(otherContainer);
        other.set_operator({ key: "eq" });
        other.createEditor();

        expect((other as any).editor).toBeInstanceOf(LazyEditor);
        expect(otherContainer.querySelector("input")).toBeTruthy();
        expect(container.querySelector("input")).toBeTruthy();
    });
});

describe("EditorFiltering", () => {
    it("is registered with typeInfo", () => {
        expect(EditorFiltering[Symbol.for("Serenity.typeInfo")]).toBeDefined();
    });

    it("default props are empty", () => {
        const filtering = new EditorFiltering();
        expect(filtering.editorType).toBeUndefined();
        expect(filtering.useRelative).toBeUndefined();
        expect(filtering.useLike).toBeUndefined();
    });

    it("getOperators with default props returns EQ, NE, and nullable", () => {
        const filtering = new EditorFiltering();
        filtering.set_field({ name: "Test" });
        const ops = filtering.getOperators();
        expect(ops).toHaveLength(4);
        expect(ops[0].key).toBe("eq");
        expect(ops[1].key).toBe("ne");
        expect(ops[2].key).toBe("isnotnull");
        expect(ops[3].key).toBe("isnull");
    });

    it("getOperators with useRelative adds comparison operators", () => {
        const filtering = new EditorFiltering({ useRelative: true });
        filtering.set_field({ name: "Test" });
        const ops = filtering.getOperators();
        expect(ops).toHaveLength(8);
        expect(ops[0].key).toBe("eq");
        expect(ops[1].key).toBe("ne");
        expect(ops[2].key).toBe("lt");
        expect(ops[3].key).toBe("le");
        expect(ops[4].key).toBe("gt");
        expect(ops[5].key).toBe("ge");
        expect(ops[6].key).toBe("isnotnull");
        expect(ops[7].key).toBe("isnull");
    });

    it("getOperators with useLike adds contains and startsWith", () => {
        const filtering = new EditorFiltering({ useLike: true });
        filtering.set_field({ name: "Test" });
        const ops = filtering.getOperators();
        expect(ops).toHaveLength(6);
        expect(ops[0].key).toBe("eq");
        expect(ops[1].key).toBe("ne");
        expect(ops[2].key).toBe("contains");
        expect(ops[3].key).toBe("startswith");
        expect(ops[4].key).toBe("isnotnull");
        expect(ops[5].key).toBe("isnull");
    });

    it("getOperators with both useRelative and useLike returns all operators", () => {
        const filtering = new EditorFiltering({ useRelative: true, useLike: true });
        filtering.set_field({ name: "Test" });
        const ops = filtering.getOperators();
        expect(ops).toHaveLength(10);
    });

    it("useEditor returns true for eq, ne and relative operators", () => {
        const filtering = new EditorFiltering({ useRelative: true });

        filtering.set_operator({ key: "eq" });
        expect((filtering as any).useEditor()).toBe(true);

        filtering.set_operator({ key: "ne" });
        expect((filtering as any).useEditor()).toBe(true);

        filtering.set_operator({ key: "lt" });
        expect((filtering as any).useEditor()).toBe(true);

        filtering.set_operator({ key: "gt" });
        expect((filtering as any).useEditor()).toBe(true);
    });

    it("useEditor returns falsy for contains/startswith when useLike is false", () => {
        const filtering = new EditorFiltering();

        filtering.set_operator({ key: "contains" });
        expect((filtering as any).useEditor()).toBeFalsy();

        filtering.set_operator({ key: "startswith" });
        expect((filtering as any).useEditor()).toBeFalsy();
    });

    it("useEditor returns true for eq when useLike is true", () => {
        const filtering = new EditorFiltering({ useLike: true });

        filtering.set_operator({ key: "eq" });
        expect((filtering as any).useEditor()).toBe(true);
    });

    it("useEditor returns falsy for isNull/isNotNull", () => {
        const filtering = new EditorFiltering();

        filtering.set_operator({ key: "isnull" });
        expect((filtering as any).useEditor()).toBeFalsy();

        filtering.set_operator({ key: "isnotnull" });
        expect((filtering as any).useEditor()).toBeFalsy();
    });

    it("editorType property works", () => {
        const filtering = new EditorFiltering();
        expect(filtering.editorType).toBeUndefined();
        filtering.editorType = "MyEditor";
        expect(filtering.editorType).toBe("MyEditor");
    });

    it("useRelative property works", () => {
        const filtering = new EditorFiltering();
        expect(filtering.useRelative).toBeUndefined();
        filtering.useRelative = true;
        expect(filtering.useRelative).toBe(true);
    });

    it("useLike property works", () => {
        const filtering = new EditorFiltering();
        expect(filtering.useLike).toBeUndefined();
        filtering.useLike = true;
        expect(filtering.useLike).toBe(true);
    });

    it("useIdField returns useEditor result", () => {
        const filtering = new EditorFiltering();

        filtering.set_operator({ key: "eq" });
        expect((filtering as any).useIdField()).toBe(true);

        filtering.set_operator({ key: "contains" });
        expect((filtering as any).useIdField()).toBeFalsy();
    });

    it("createEditor throws when useEditor is false and operator is unknown", () => {
        const filtering = new EditorFiltering();
        filtering.set_field({ name: "Test" });
        filtering.set_container(document.createElement("div"));
        filtering.set_operator({ key: "invalid" });
        expect(() => filtering.createEditor()).toThrow();
    });

    it("createEditor uses super.createEditor for non-editor operators", () => {
        const filtering = new EditorFiltering();
        filtering.set_field({ name: "Test" });
        const container = document.createElement("div");
        filtering.set_container(container);
        filtering.set_operator({ key: "contains" });
        filtering.createEditor();

        const input = container.querySelector("input");
        expect(input).toBeTruthy();
    });

    it("createEditor with useEditor=true and editorType='String' creates a StringEditor", () => {
        const filtering = new EditorFiltering({ editorType: "String" } as any);
        filtering.set_field({ name: "Test" });
        const container = document.createElement("div");
        filtering.set_container(container);
        filtering.set_operator({ key: "eq" });
        filtering.createEditor();

        // Editor should have been created
        expect((filtering as any).editor).toBeTruthy();
    });

    it("getEditorOptions merges editorParams when editorType matches field editorType", () => {
        const filtering = new EditorFiltering({ useRelative: true } as any);
        filtering.editorType = "String";
        filtering.set_field({ name: "Test", editorType: "String", editorParams: { maxLength: 50 }, filteringParams: { someParam: true } });
        filtering.set_operator({ key: "eq" });

        const opts = filtering.getEditorOptions();
        expect(opts.someParam).toBe(true);
        expect(opts.maxLength).toBe(50);
    });

    it("getEditorOptions does not double-merge editorParams when editorType does not match", () => {
        const filtering = new EditorFiltering({ useRelative: true } as any);
        filtering.editorType = "Integer";
        filtering.set_field({ name: "Test", editorType: "String", editorParams: { maxLength: 50 }, filteringParams: { extraParam: true } });
        filtering.set_operator({ key: "eq" });

        const opts = filtering.getEditorOptions();
        // Super.getEditorOptions already includes editorParams and filteringParams
        expect(opts.maxLength).toBe(50);
        expect(opts.extraParam).toBe(true);
    });

    it("getEditorOptions does not include cascadeFrom", () => {
        const filtering = new EditorFiltering();
        filtering.set_field({ name: "Test", editorParams: { cascadeFrom: "Parent" } });
        filtering.set_operator({ key: "eq" });

        const opts = filtering.getEditorOptions();
        expect(opts.cascadeFrom).toBeUndefined();
    });

    it("getEditorOptions never re-introduces cascadeFrom even when editorType matches", () => {
        const filtering = new EditorFiltering({ editorType: "String" } as any);
        filtering.set_field({ name: "Test", editorType: "String", editorParams: { cascadeFrom: "Parent", maxLength: 50 }, filteringParams: { someParam: true } });
        filtering.set_operator({ key: "eq" });

        const opts = filtering.getEditorOptions();
        expect(opts.cascadeFrom).toBeUndefined();
        expect(opts.maxLength).toBe(50);
        expect(opts.someParam).toBe(true);
    });

    it("getEditorOptions returns empty options with default constructor", () => {
        const filtering = new EditorFiltering();
        filtering.set_field({ name: "Test" });
        filtering.set_operator({ key: "eq" });
        const opts = filtering.getEditorOptions();
        expect(opts).toBeTruthy();
    });

    it("initQuickFilter sets filter type via EditorTypeRegistry", () => {
        const filtering = new EditorFiltering({ editorType: "String" } as any);
        filtering.set_field({ name: "Test", title: "Test" });
        filtering.set_operator({ key: "eq" });
        const filter: any = { options: {} };
        filtering.initQuickFilter(filter);
        expect(filter.type).toBeDefined();
        expect(filter.field).toBe("Test");
    });
});
