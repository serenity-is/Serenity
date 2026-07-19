import { EnumEditor } from "../editors/enumeditor";
import { EnumFiltering } from "./enumfiltering";

describe("EnumFiltering", () => {
    it("is registered with typeInfo", () => {
        expect(EnumFiltering[Symbol.for("Serenity.typeInfo")]).toBeDefined();
    });

    it("is constructed with EnumEditor type", () => {
        const filtering = new EnumFiltering();
        expect((filtering as any).editorTypeRef).toBe(EnumEditor);
    });

    it("getOperators returns EQ, NE and nullable operators", () => {
        const filtering = new EnumFiltering();
        filtering.set_field({ name: "Test" });
        const ops = filtering.getOperators();
        expect(ops).toHaveLength(4);
        expect(ops[0].key).toBe("eq");
        expect(ops[1].key).toBe("ne");
        expect(ops[2].key).toBe("isnotnull");
        expect(ops[3].key).toBe("isnull");
    });

    it("getOperators without nullable when field is required", () => {
        const filtering = new EnumFiltering();
        filtering.set_field({ name: "Test", required: true });
        const ops = filtering.getOperators();
        expect(ops).toHaveLength(2);
    });

    it("getEditorText returns editor.text when useEditor is true and editor exists", () => {
        const filtering = new EnumFiltering();
        filtering.set_field({ name: "Test" });
        const container = document.createElement("div");
        filtering.set_container(container);
        filtering.set_operator({ key: "eq" });
        (filtering as any).editor = { text: "Option1" };
        expect(filtering.getEditorText()).toBe("Option1");
        expect((filtering as any).useEditor()).toBe(true);
    });

    it("getEditorText calls super when useEditor is false", () => {
        const filtering = new EnumFiltering();
        filtering.set_field({ name: "Test" });
        const container = document.createElement("div");
        filtering.set_container(container);
        filtering.set_operator({ key: "contains" });
        filtering.createEditor();

        const input = container.querySelector("input");
        input.value = "fallbackText";

        expect(filtering.getEditorText()).toBe("fallbackText");
        expect((filtering as any).useEditor()).toBeFalsy();
    });
});
