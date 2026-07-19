import { FilterLine } from "./filterline";

describe("FilterLine", () => {
    it("can be created with field and operator", () => {
        const line: FilterLine = {
            field: "SomeField",
            operator: "eq"
        };
        expect(line.field).toBe("SomeField");
        expect(line.operator).toBe("eq");
    });

    it("can have all properties set", () => {
        const line: FilterLine = {
            field: "SomeField",
            operator: "contains",
            isOr: true,
            leftParen: false,
            rightParen: true,
            validationError: "error",
            criteria: ["field", "contains", "test"],
            displayText: "field contains test",
            state: { value: "test" }
        };
        expect(line.isOr).toBe(true);
        expect(line.rightParen).toBe(true);
        expect(line.validationError).toBe("error");
        expect(line.criteria).toEqual(["field", "contains", "test"]);
        expect(line.state).toEqual({ value: "test" });
    });
});
