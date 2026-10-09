import { describe, expect, it } from "vitest";
import { Int64Editor } from "../editors/int64editor";
import { FilteringTypeRegistry } from "./filteringtyperegistry";
import { Int64Filtering } from "./int64filtering";

describe("Int64Filtering", () => {
    it("is registered with typeInfo", () => {
        expect(Int64Filtering[Symbol.for("Serenity.typeInfo")]).toBeDefined();
    });

    it("is constructed with Int64Editor type", () => {
        const filtering = new Int64Filtering();
        expect((filtering as any).editorTypeRef).toBe(Int64Editor);
    });

    it("getOperators returns comparison operators and nullable", () => {
        const filtering = new Int64Filtering();
        filtering.set_field({ name: "Test" });
        const ops = filtering.getOperators();
        expect(ops.map(o => o.key)).toEqual(["eq", "ne", "lt", "le", "gt", "ge", "isnotnull", "isnull"]);
    });

    it("is resolvable by the filtering type registry as Int64", () => {
        expect(FilteringTypeRegistry.get("Int64")).toBe(Int64Filtering);
    });
});
