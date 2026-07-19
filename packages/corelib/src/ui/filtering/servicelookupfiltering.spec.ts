import { ServiceLookupEditor } from "../editors/servicelookupeditor";
import { ServiceLookupFiltering } from "./servicelookupfiltering";

describe("ServiceLookupFiltering", () => {
    it("is registered with typeInfo", () => {
        expect(ServiceLookupFiltering[Symbol.for("Serenity.typeInfo")]).toBeDefined();
    });

    it("is constructed with ServiceLookupEditor type", () => {
        const filtering = new ServiceLookupFiltering();
        expect((filtering as any).editorTypeRef).toBe(ServiceLookupEditor);
    });

    it("getOperators returns EQ, NE, contains, startsWith and nullable", () => {
        const filtering = new ServiceLookupFiltering();
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

    it("useEditor returns true for eq and ne operators", () => {
        const filtering = new ServiceLookupFiltering();
        filtering.set_field({ name: "Test" });

        filtering.set_operator({ key: "eq" });
        expect((filtering as any).useEditor()).toBe(true);

        filtering.set_operator({ key: "ne" });
        expect((filtering as any).useEditor()).toBe(true);
    });

    it("useEditor returns false for contains and startsWith", () => {
        const filtering = new ServiceLookupFiltering();
        filtering.set_field({ name: "Test" });

        filtering.set_operator({ key: "contains" });
        expect((filtering as any).useEditor()).toBe(false);

        filtering.set_operator({ key: "startswith" });
        expect((filtering as any).useEditor()).toBe(false);
    });

    it("useIdField returns same as useEditor", () => {
        const filtering = new ServiceLookupFiltering();
        filtering.set_field({ name: "Test" });

        filtering.set_operator({ key: "eq" });
        expect((filtering as any).useIdField()).toBe(true);

        filtering.set_operator({ key: "contains" });
        expect((filtering as any).useIdField()).toBe(false);
    });

    it("getCriteriaField returns filteringIdField when useEditor and filteringIdField set", () => {
        const filtering = new ServiceLookupFiltering();
        filtering.set_field({ name: "Test", filteringIdField: "TestId" });
        filtering.set_operator({ key: "eq" });

        expect(filtering.getCriteriaField()).toBe("TestId");
    });

    it("getEditorText returns editor.text when useEditor is true and editor exists", () => {
        const filtering = new ServiceLookupFiltering();
        filtering.set_field({ name: "Test" });
        const container = document.createElement("div");
        filtering.set_container(container);
        filtering.set_operator({ key: "eq" });
        (filtering as any).editor = { text: "selectedItem" };
        expect(filtering.getEditorText()).toBe("selectedItem");
    });
});
