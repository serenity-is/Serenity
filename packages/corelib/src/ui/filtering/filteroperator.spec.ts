import { FilterOperator, FilterOperators } from "./filteroperator";

describe("FilterOperators", () => {
    it("has all expected constants", () => {
        expect(FilterOperators.isTrue).toBe("true");
        expect(FilterOperators.isFalse).toBe("false");
        expect(FilterOperators.contains).toBe("contains");
        expect(FilterOperators.startsWith).toBe("startswith");
        expect(FilterOperators.EQ).toBe("eq");
        expect(FilterOperators.NE).toBe("ne");
        expect(FilterOperators.GT).toBe("gt");
        expect(FilterOperators.GE).toBe("ge");
        expect(FilterOperators.LT).toBe("lt");
        expect(FilterOperators.LE).toBe("le");
        expect(FilterOperators.BW).toBe("bw");
        expect(FilterOperators.IN).toBe("in");
        expect(FilterOperators.isNull).toBe("isnull");
        expect(FilterOperators.isNotNull).toBe("isnotnull");
    });

    it("toCriteriaOperator maps correctly", () => {
        expect(FilterOperators.toCriteriaOperator).toEqual({
            eq: "=",
            ne: "!=",
            gt: ">",
            ge: ">=",
            lt: "<",
            le: "<="
        });
    });

    it("FilterOperator interface allows partial options", () => {
        const op: FilterOperator = { key: "eq" };
        expect(op.key).toBe("eq");
        expect(op.title).toBeUndefined();
        expect(op.format).toBeUndefined();
    });

    it("FilterOperator can have title and format", () => {
        const op: FilterOperator = { key: "eq", title: "Equals", format: "{0} = {1}" };
        expect(op.title).toBe("Equals");
        expect(op.format).toBe("{0} = {1}");
    });
});
