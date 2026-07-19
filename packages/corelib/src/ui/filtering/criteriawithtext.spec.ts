import { CriteriaWithText } from "./criteriawithtext";

describe("CriteriaWithText", () => {
    it("can be created with no properties", () => {
        const c: CriteriaWithText = {};
        expect(c.criteria).toBeUndefined();
        expect(c.displayText).toBeUndefined();
    });

    it("can be created with criteria and displayText", () => {
        const c: CriteriaWithText = {
            criteria: ["field", "=", 1],
            displayText: "field = 1"
        };
        expect(c.criteria).toEqual(["field", "=", 1]);
        expect(c.displayText).toBe("field = 1");
    });
});
