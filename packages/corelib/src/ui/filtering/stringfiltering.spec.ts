import { StringFiltering } from "./stringfiltering";

describe("StringFiltering", () => {
    it("is registered with typeInfo", () => {
        expect(StringFiltering[Symbol.for("Serenity.typeInfo")]).toBeDefined();
    });

    it("getOperators returns contains, startsWith, EQ, NE and nullable operators", () => {
        const filtering = new StringFiltering();
        filtering.set_field({ name: "Test" });
        const ops = filtering.getOperators();
        expect(ops).toHaveLength(6);
        expect(ops[0].key).toBe("contains");
        expect(ops[1].key).toBe("startswith");
        expect(ops[2].key).toBe("eq");
        expect(ops[3].key).toBe("ne");
        expect(ops[4].key).toBe("isnotnull");
        expect(ops[5].key).toBe("isnull");
    });

    it("getOperators without nullable when field is required", () => {
        const filtering = new StringFiltering();
        filtering.set_field({ name: "Test", required: true });
        const ops = filtering.getOperators();
        expect(ops).toHaveLength(4);
        expect(ops[0].key).toBe("contains");
        expect(ops[1].key).toBe("startswith");
        expect(ops[2].key).toBe("eq");
        expect(ops[3].key).toBe("ne");
    });

    it("override validateEditorValue allows empty string", () => {
        const filtering = new StringFiltering();
        filtering.set_field({ name: "Test" });
        filtering.set_container(document.createElement("div"));
        filtering.set_operator({ key: "eq" });

        expect(() => filtering.validateEditorValue("")).not.toThrow();
        expect(filtering.validateEditorValue("")).toBe("");
        expect(filtering.validateEditorValue("test")).toBe("test");
    });

    it("getCriteria for contains operator creates input and returns criteria", () => {
        const filtering = new StringFiltering();
        filtering.set_field({ name: "SomeField", title: "Some Field" });
        const container = document.createElement("div");
        filtering.set_container(container);
        filtering.set_operator({ key: "contains" });
        filtering.createEditor();

        const input = container.querySelector("input");
        expect(input).toBeTruthy();
        input.value = "testValue";

        const result = filtering.getCriteria();
        expect(result.criteria).toEqual([["SomeField"], "like", "%testValue%"]);
        expect(result.displayText).toBeTruthy();
    });

    it("getCriteria for startsWith operator", () => {
        const filtering = new StringFiltering();
        filtering.set_field({ name: "SomeField", title: "Some Field" });
        const container = document.createElement("div");
        filtering.set_container(container);
        filtering.set_operator({ key: "startswith" });
        filtering.createEditor();

        const input = container.querySelector("input");
        input.value = "prefix";

        const result = filtering.getCriteria();
        expect(result.criteria).toEqual([["SomeField"], "like", "prefix%"]);
    });

    it("getCriteria for eq operator", () => {
        const filtering = new StringFiltering();
        filtering.set_field({ name: "SomeField" });
        const container = document.createElement("div");
        filtering.set_container(container);
        filtering.set_operator({ key: "eq" });
        filtering.createEditor();

        const input = container.querySelector("input");
        input.value = "exact";

        const result = filtering.getCriteria();
        expect(result.criteria).toEqual([["SomeField"], "=", "exact"]);
    });

    it("getCriteria for ne operator", () => {
        const filtering = new StringFiltering();
        filtering.set_field({ name: "SomeField" });
        const container = document.createElement("div");
        filtering.set_container(container);
        filtering.set_operator({ key: "ne" });
        filtering.createEditor();

        const input = container.querySelector("input");
        input.value = "notThis";

        const result = filtering.getCriteria();
        expect(result.criteria).toEqual([["SomeField"], "!=", "notThis"]);
    });

    it("getCriteria for isNull operator", () => {
        const filtering = new StringFiltering();
        filtering.set_field({ name: "SomeField" });
        filtering.set_operator({ key: "isnull" });
        const result = filtering.getCriteria();
        expect(result.criteria).toEqual(["is null", ["SomeField"]]);
    });

    it("getCriteria for isNotNull operator", () => {
        const filtering = new StringFiltering();
        filtering.set_field({ name: "SomeField" });
        filtering.set_operator({ key: "isnotnull" });
        const result = filtering.getCriteria();
        expect(result.criteria).toEqual(["is not null", ["SomeField"]]);
    });

    it("loadState sets input value", () => {
        const filtering = new StringFiltering();
        filtering.set_field({ name: "Test" });
        const container = document.createElement("div");
        filtering.set_container(container);
        filtering.set_operator({ key: "eq" });
        filtering.createEditor();

        filtering.loadState("loadedValue");
        const input = container.querySelector("input");
        expect(input.value).toBe("loadedValue");
    });

    it("saveState returns input value", () => {
        const filtering = new StringFiltering();
        filtering.set_field({ name: "Test" });
        const container = document.createElement("div");
        filtering.set_container(container);
        filtering.set_operator({ key: "eq" });
        filtering.createEditor();

        const input = container.querySelector("input");
        input.value = "saveMe";
        expect(filtering.saveState()).toBe("saveMe");
    });

    it("saveState returns null for isNull/isNotNull operators", () => {
        const filtering = new StringFiltering();
        filtering.set_field({ name: "Test" });
        filtering.set_operator({ key: "isnull" });
        expect(filtering.saveState()).toBeNull();
    });

    it("createEditor throws for unknown operator", () => {
        const filtering = new StringFiltering();
        filtering.set_field({ name: "Test" });
        filtering.set_container(document.createElement("div"));
        filtering.set_operator({ key: "invalid_op" });
        expect(() => filtering.createEditor()).toThrow();
    });

    it("getCriteria throws for unknown operator", () => {
        const filtering = new StringFiltering();
        filtering.set_field({ name: "Test" });
        filtering.set_container(document.createElement("div"));
        filtering.set_operator({ key: "invalid_op" });
        expect(() => filtering.getCriteria()).toThrow();
    });
});
