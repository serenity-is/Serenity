import { Widget } from "../widgets/widget";
import { EditorFiltering } from "./editorfiltering";

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
