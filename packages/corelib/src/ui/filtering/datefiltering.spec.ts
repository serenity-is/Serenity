import { DateEditor } from "../editors/dateeditor";
import { DateFiltering } from "./datefiltering";

describe("DateFiltering", () => {
    it("is registered with typeInfo", () => {
        expect(DateFiltering[Symbol.for("Serenity.typeInfo")]).toBeDefined();
    });

    it("is constructed with DateEditor type", () => {
        const filtering = new DateFiltering();
        expect((filtering as any).editorTypeRef).toBe(DateEditor);
    });

    it("getOperators returns comparison operators and nullable", () => {
        const filtering = new DateFiltering();
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

    it("getOperators without nullable when field is required", () => {
        const filtering = new DateFiltering();
        filtering.set_field({ name: "Test", required: true });
        const ops = filtering.getOperators();
        expect(ops).toHaveLength(6);
    });
});
