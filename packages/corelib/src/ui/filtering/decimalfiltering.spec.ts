import { DecimalEditor } from "../editors/decimaleditor";
import { DecimalFiltering } from "./decimalfiltering";

describe("DecimalFiltering", () => {
    it("is registered with typeInfo", () => {
        expect(DecimalFiltering[Symbol.for("Serenity.typeInfo")]).toBeDefined();
    });

    it("is constructed with DecimalEditor type", () => {
        const filtering = new DecimalFiltering();
        expect((filtering as any).editorTypeRef).toBe(DecimalEditor);
    });

    it("getOperators returns comparison operators and nullable", () => {
        const filtering = new DecimalFiltering();
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
        const filtering = new DecimalFiltering();
        filtering.set_field({ name: "Test", required: true });
        const ops = filtering.getOperators();
        expect(ops).toHaveLength(6);
    });
});
