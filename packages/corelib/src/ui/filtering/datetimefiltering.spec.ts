import { Criteria } from "../../base";
import { DateTimeEditor } from "../editors/datetimeeditor";
import { DateTimeFiltering } from "./datetimefiltering";

describe("DateTimeFiltering", () => {
    it("is registered with typeInfo", () => {
        expect(DateTimeFiltering[Symbol.for("Serenity.typeInfo")]).toBeDefined();
    });

    it("is constructed with DateTimeEditor type", () => {
        const filtering = new DateTimeFiltering();
        expect((filtering as any).editorTypeRef).toBe(DateTimeEditor);
    });

    it("getOperators returns comparison operators and nullable", () => {
        const filtering = new DateTimeFiltering();
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

    it("getCriteria for eq operator creates date range criteria", () => {
        const filtering = new DateTimeFiltering();
        filtering.set_field({ name: "DateField", title: "Date Field" });
        const container = document.createElement("div");
        filtering.set_container(container);
        filtering.set_operator({ key: "eq" });
        filtering.createEditor();

        const input = container.querySelector("input");
        input.value = "2024-01-15";

        const result = filtering.getCriteria();
        expect(result.criteria).toBeTruthy();
        expect(result.displayText).toBeTruthy();
    });

    it("getCriteria for ne operator", () => {
        const filtering = new DateTimeFiltering();
        filtering.set_field({ name: "DateField" });
        const container = document.createElement("div");
        filtering.set_container(container);
        filtering.set_operator({ key: "ne" });
        filtering.createEditor();

        const input = container.querySelector("input");
        input.value = "2024-01-15";

        const result = filtering.getCriteria();
        expect(result.criteria).toBeTruthy();
    });

    it("getCriteria for lt operator", () => {
        const filtering = new DateTimeFiltering();
        filtering.set_field({ name: "DateField" });
        const container = document.createElement("div");
        filtering.set_container(container);
        filtering.set_operator({ key: "lt" });
        filtering.createEditor();

        const input = container.querySelector("input");
        input.value = "2024-01-15";

        const result = filtering.getCriteria();
        expect(result.criteria).toBeTruthy();
    });

    it("getCriteria for le operator", () => {
        const filtering = new DateTimeFiltering();
        filtering.set_field({ name: "DateField" });
        const container = document.createElement("div");
        filtering.set_container(container);
        filtering.set_operator({ key: "le" });
        filtering.createEditor();

        const input = container.querySelector("input");
        input.value = "2024-01-15";

        const result = filtering.getCriteria();
        expect(result.criteria).toBeTruthy();
    });

    it("getCriteria for gt operator", () => {
        const filtering = new DateTimeFiltering();
        filtering.set_field({ name: "DateField" });
        const container = document.createElement("div");
        filtering.set_container(container);
        filtering.set_operator({ key: "gt" });
        filtering.createEditor();

        const input = container.querySelector("input");
        input.value = "2024-01-15";

        const result = filtering.getCriteria();
        expect(result.criteria).toBeTruthy();
    });

    it("getCriteria for ge operator", () => {
        const filtering = new DateTimeFiltering();
        filtering.set_field({ name: "DateField" });
        const container = document.createElement("div");
        filtering.set_container(container);
        filtering.set_operator({ key: "ge" });
        filtering.createEditor();

        const input = container.querySelector("input");
        input.value = "2024-01-15";

        const result = filtering.getCriteria();
        expect(result.criteria).toBeTruthy();
    });

    it("getCriteria for isNull passes through to base", () => {
        const filtering = new DateTimeFiltering();
        filtering.set_field({ name: "DateField" });
        filtering.set_operator({ key: "isnull" });
        const result = filtering.getCriteria();
        expect(result.criteria).toEqual(["is null", ["DateField"]]);
    });

    it("getCriteria for isNotNull passes through to base", () => {
        const filtering = new DateTimeFiltering();
        filtering.set_field({ name: "DateField" });
        filtering.set_operator({ key: "isnotnull" });
        const result = filtering.getCriteria();
        expect(result.criteria).toEqual(["is not null", ["DateField"]]);
    });

    it("getCriteria for contains passes through to base", () => {
        const filtering = new DateTimeFiltering();
        filtering.set_field({ name: "DateField" });
        filtering.set_operator({ key: "contains" });
        filtering.set_container(document.createElement("div"));
        const result = filtering.getCriteria();
        expect(result.criteria).toBeTruthy();
        expect(result.displayText).toBeTruthy();
    });

    it("eq logic produces range criteria", () => {
        const filtering = new DateTimeFiltering();
        filtering.set_field({ name: "OrderDate" });
        const container = document.createElement("div");
        filtering.set_container(container);
        filtering.set_operator({ key: "eq" });
        filtering.createEditor();

        const input = container.querySelector("input");
        input.value = "2024-06-15";

        const result = filtering.getCriteria();
        expect(Array.isArray(result.criteria)).toBe(true);
        expect(Array.isArray(result.criteria[0])).toBe(true);
        expect(result.displayText).toBeTruthy();
    });
});
