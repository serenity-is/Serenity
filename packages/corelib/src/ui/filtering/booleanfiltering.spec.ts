import { BooleanFiltering } from "./booleanfiltering";

describe("BooleanFiltering", () => {
    it("is registered with typeInfo", () => {
        expect(BooleanFiltering[Symbol.for("Serenity.typeInfo")]).toBeDefined();
    });

    it("getOperators returns isTrue, isFalse and nullable operators", () => {
        const filtering = new BooleanFiltering();
        filtering.set_field({ name: "Test" });
        const ops = filtering.getOperators();
        expect(ops).toHaveLength(4);
        expect(ops[0].key).toBe("true");
        expect(ops[1].key).toBe("false");
        expect(ops[2].key).toBe("isnotnull");
        expect(ops[3].key).toBe("isnull");
    });

    it("getOperators returns isTrue and isFalse when field is required", () => {
        const filtering = new BooleanFiltering();
        filtering.set_field({ name: "TestField", required: true });
        const ops = filtering.getOperators();
        expect(ops).toHaveLength(2);
        expect(ops[0].key).toBe("true");
        expect(ops[1].key).toBe("false");
    });

    it("getCriteria for isTrue operator", () => {
        const filtering = new BooleanFiltering();
        filtering.set_field({ name: "IsActive", title: "Is Active" });
        filtering.set_operator({ key: "true" });
        const result = filtering.getCriteria();
        expect(result.criteria).toEqual([["IsActive"], "=", true]);
        expect(result.displayText).toBeTruthy();
    });

    it("getCriteria for isFalse operator", () => {
        const filtering = new BooleanFiltering();
        filtering.set_field({ name: "IsActive", title: "Is Active" });
        filtering.set_operator({ key: "false" });
        const result = filtering.getCriteria();
        expect(result.criteria).toEqual([["IsActive"], "=", false]);
        expect(result.displayText).toBeTruthy();
    });

    it("getCriteria for isNull operator", () => {
        const filtering = new BooleanFiltering();
        filtering.set_field({ name: "IsActive" });
        filtering.set_operator({ key: "isnull" });
        const result = filtering.getCriteria();
        expect(result.criteria).toEqual(["is null", ["IsActive"]]);
    });

    it("getCriteria for isNotNull operator", () => {
        const filtering = new BooleanFiltering();
        filtering.set_field({ name: "IsActive" });
        filtering.set_operator({ key: "isnotnull" });
        const result = filtering.getCriteria();
        expect(result.criteria).toEqual(["is not null", ["IsActive"]]);
    });

    it("createEditor does nothing for isTrue/isFalse operators (no editor needed)", () => {
        const filtering = new BooleanFiltering();
        filtering.set_field({ name: "Test" });
        const container = document.createElement("div");
        filtering.set_container(container);

        filtering.set_operator({ key: "true" });
        filtering.createEditor();
        expect(container.children.length).toBe(0);

        filtering.set_operator({ key: "false" });
        filtering.createEditor();
        expect(container.children.length).toBe(0);
    });

    it("saveState returns null for isTrue/isFalse/isnull/isnotnull operators", () => {
        const filtering = new BooleanFiltering();
        filtering.set_field({ name: "Test" });
        filtering.set_operator({ key: "true" });
        expect(filtering.saveState()).toBeNull();

        filtering.set_operator({ key: "false" });
        expect(filtering.saveState()).toBeNull();

        filtering.set_operator({ key: "isnull" });
        expect(filtering.saveState()).toBeNull();

        filtering.set_operator({ key: "isnotnull" });
        expect(filtering.saveState()).toBeNull();
    });
});
