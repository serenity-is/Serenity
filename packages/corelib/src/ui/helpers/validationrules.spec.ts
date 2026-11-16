import { afterEach, describe, expect, it, vi } from "vitest";
import { addLocalText, Config, ValidationRule } from "../../base";
import { ValidationRuleTypeRegistry } from "../../types/validationruletyperegistry";
import { checkValidationRules, getValidationRuleMessage, isValidationRuleRegistered, loadValidationRules, registerValidationRule } from "./validationrules";

describe("checkValidationRules", () => {
    afterEach(() => vi.restoreAllMocks());

    const check = (rule: string, params: any, value: any, getValue?: (field: string) => any) =>
        checkValidationRules([{ rule, params }], value, { getValue });

    it("checks a range of numbers and of dates as texts, including its limits", () => {
        expect(check("Range", [1, 10], 1)).toBeNull();
        expect(check("Range", [1, 10], 10)).toBeNull();
        expect(check("Range", [1, 10], "5")).toBeNull();
        expect(check("Range", [1, 10], 11)).toBe("Please enter a value between 1 and 10.");
        expect(check("Range", [1.5, 2.5], 3)).not.toBeNull();
        expect(check("Range", ["2026-01-01", "2026-12-31"], "2026-06-15")).toBeNull();
        expect(check("Range", ["2026-01-01", "2026-12-31"], "2027-01-01")).not.toBeNull();
    });

    it("checks the lengths of texts and lists, with the message of what it has, a maximum, a minimum or both", () => {
        expect(check("Length", [0, 3], "abc")).toBeNull();
        expect(check("Length", [0, 3], "abcd")).toBe("Please enter no more than 3 characters.");
        expect(check("Length", [2, null], "a")).toBe("Please enter no less than 2 characters.");
        expect(check("Length", [2, null], "a long text")).toBeNull();
        expect(check("Length", [2, 4], "abcde")).toBe("Please enter a value between 2 and 4 characters long.");
        expect(check("Length", [0, 1], ["a", "b"])).not.toBeNull();
    });

    it("finds the built-in rules by their short keys and their full names", () => {
        expect(isValidationRuleRegistered("Range")).toBe(true);
        expect(isValidationRuleRegistered("Serenity.RangeValidationRule")).toBe(true);
        expect(check("Serenity.LengthValidationRule", [0, 3], "abcd")).not.toBeNull();
    });

    it("matches a pattern against the whole text", () => {
        expect(check("Pattern", "[A-Z]{2}[0-9]{2}", "AB12")).toBeNull();
        expect(check("Pattern", "[A-Z]{2}[0-9]{2}", "AB123")).toBe("Please enter a value in the expected format.");
        expect(check("Pattern", "a|b", "ab")).not.toBeNull();
    });

    it("checks emails and urls as the validator does", () => {
        expect(check("Email", null, "user@example.com")).toBeNull();
        expect(check("Email", null, "invalid")).toBe("Please enter a valid email address.");
        expect(check("Url", null, "https://example.com/a")).toBeNull();
        expect(check("Url", null, "example")).not.toBeNull();
    });

    it("compares with the value of another field, an empty one being the same as another empty one", () => {
        const values: Record<string, any> = { Password: "secret", Empty: null };
        expect(check("Compare", "Password", "secret", f => values[f])).toBeNull();
        expect(check("Compare", "Password", "other", f => values[f])).toBe("Please enter the same value again.");
        expect(check("Compare", "Password", 1, () => "1")).toBeNull();
    });

    it("doesn't check an empty value, as being required is a rule of its own", () => {
        for (const value of [null, undefined, "", []])
            expect(check("Range", [1, 10], value)).toBeNull();
    });

    it("stops at the first rule the value is not valid by", () => {
        expect(checkValidationRules([{ rule: "Length", params: [0, 10] }, { rule: "Pattern", params: "[0-9]+" }, { rule: "Length", params: [5, null] }], "ab"))
            .toBe("Please enter a value in the expected format.");
    });

    it("skips a rule that is not registered with a warning, as the server still checks it", () => {
        const warn = vi.spyOn(console, "warn").mockImplementation(() => { });
        expect(check("notRegisteredAnywhere", null, "x")).toBeNull();
        expect(warn).toHaveBeenCalledOnce();
    });

    it("checks a registered custom rule with its parameters and the default message", () => {
        registerValidationRule("multipleOf", (value, params) => Number(value) % params === 0, { textKey: "Test.MultipleOf", defaultText: "Must be a multiple of {0}." });
        expect(isValidationRuleRegistered("multipleOf")).toBe(true);
        expect(check("multipleOf", 5, 10)).toBeNull();
        expect(check("multipleOf", 5, 12)).toBe("Must be a multiple of 5.");
    });
});

describe("getValidationRuleMessage", () => {
    it("is the text of the key, then the message, then the localized default, with the parameters", () => {
        addLocalText({ "Test.Rules.Key": "From the key {0}", "Validation.Range": "Between {0} and {1}" });

        expect(getValidationRuleMessage({ rule: "Range", params: [1, 2], textKey: "Test.Rules.Key", message: "Own" })).toBe("From the key 1");
        expect(getValidationRuleMessage({ rule: "Range", params: [1, 2], textKey: "Test.Rules.Missing", message: "Own {1}" })).toBe("Own 2");
        expect(getValidationRuleMessage({ rule: "Range", params: [1, 2] })).toBe("Between 1 and 2");
    });

    it("has a generic message for a rule that is not registered", () => {
        expect(getValidationRuleMessage({ rule: "notRegisteredAnywhere" })).toBe("Please enter a valid value.");
    });
});

describe("validation rule classes", () => {
    afterEach(() => {
        vi.restoreAllMocks();
        Config.lazyTypeLoader = orgLoader;
        ValidationRuleTypeRegistry.reset();
    });

    const orgLoader = Config.lazyTypeLoader;

    class EvenValidationRule extends ValidationRule {
        static override [Symbol.typeInfo] = this.registerRule("Test.Rules.");
        override textKey = "Test.Rules.Even";
        override defaultText = "Please enter an even number, not {0}.";
        check(value: any) { return Number(value) % 2 === 0; }
    }

    it("finds a loaded rule class by its type key, or by its key without the ValidationRule suffix", () => {
        void EvenValidationRule;
        expect(checkValidationRules([{ rule: "Test.Rules.EvenValidationRule" }], 4)).toBeNull();
        expect(checkValidationRules([{ rule: "Test.Rules.Even", params: 3 }], 3)).toBe("Please enter an even number, not 3.");
    });

    it("loads a rule class on demand, without a warning while it is loading", async () => {
        class OddValidationRule extends ValidationRule {
            override textKey = "Test.Rules.Odd";
            override defaultText = "Odd only";
            check(value: any) { return Number(value) % 2 === 1; }
        }

        let resolve: (type: any) => void;
        const loading = new Promise(r => resolve = r);
        Config.lazyTypeLoader = ((key: string, _kind: string, availabilityCheck?: boolean) => {
            if (key !== "Test.Lazy.OddValidationRule")
                return null;
            return availabilityCheck ? true : loading;
        }) as any;
        const warn = vi.spyOn(console, "warn").mockImplementation(() => { });

        const rules = [{ rule: "Test.Lazy.OddValidationRule" }];
        const loaded = loadValidationRules(rules);
        // still loading, so it is skipped quietly, as the server checks it anyway
        expect(checkValidationRules(rules, 2)).toBeNull();
        expect(warn).not.toHaveBeenCalled();

        resolve(OddValidationRule);
        await loaded;
        expect(isValidationRuleRegistered("Test.Lazy.OddValidationRule")).toBe(true);
        expect(checkValidationRules(rules, 2)).toBe("Odd only");
    });
});
