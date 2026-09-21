import * as corelib from "@serenity-is/corelib";
import { PasswordEditor } from "@serenity-is/corelib";
import { afterEach, describe, expect, it, vi } from "vitest";
import { addPasswordStrengthValidation, getPasswordStrengthRules } from "../../../Modules/Membership/PasswordStrength/PasswordStrengthValidation";

function rules(overrides: any = {}) {
    return {
        MinPasswordLength: 6,
        RequireDigit: true,
        RequireLowercase: true,
        RequireUppercase: true,
        RequireNonAlphanumeric: true,
        ...overrides
    } as any;
}

async function setup(ruleValues = rules()) {
    const getRemoteDataAsync = vi.spyOn(corelib, "getRemoteDataAsync").mockResolvedValue(ruleValues);
    const editor = new PasswordEditor({});
    const addRuleSpy = vi.spyOn(editor, "addValidationRule");
    addPasswordStrengthValidation(editor, "unique");
    await vi.waitFor(() => expect(addRuleSpy).toHaveBeenCalled());
    return { editor, addRuleSpy, getRemoteDataAsync };
}

describe("getPasswordStrengthRules", () => {
    afterEach(() => vi.restoreAllMocks());

    it("loads rules from remote data", async () => {
        const spy = vi.spyOn(corelib, "getRemoteDataAsync").mockResolvedValue(rules());
        const result = await getPasswordStrengthRules();
        expect(spy).toHaveBeenCalledWith("PasswordStrengthRules");
        expect(result).toBeTruthy();
    });
});

describe("addPasswordStrengthValidation", () => {
    afterEach(() => vi.restoreAllMocks());

    it("adds all rules when all requirements are enabled", async () => {
        const { editor, addRuleSpy } = await setup();
        expect(addRuleSpy).toHaveBeenCalledTimes(5);

        editor.value = "abcdef1A!";
        for (const call of addRuleSpy.mock.calls) {
            expect((call[0] as any)()).toBeUndefined();
        }
        editor.destroy();
    });

    it("returns messages when requirements are not met", async () => {
        const { editor, addRuleSpy } = await setup();
        editor.value = "a";
        const messages = addRuleSpy.mock.calls.map(c => (c[0] as any)());
        expect(messages.filter(m => m != null).length).toBeGreaterThanOrEqual(4);
        editor.destroy();
    });

    it("adds only the minimum length rule when other requirements are disabled", async () => {
        const { editor, addRuleSpy } = await setup(rules({
            RequireDigit: false,
            RequireLowercase: false,
            RequireUppercase: false,
            RequireNonAlphanumeric: false
        }));
        expect(addRuleSpy).toHaveBeenCalledTimes(1);
        editor.value = "abcdef";
        expect((addRuleSpy.mock.calls[0][0] as any)()).toBeUndefined();
        editor.destroy();
    });

    it("uses the provided unique name", async () => {
        const { editor, addRuleSpy } = await setup();
        for (const call of addRuleSpy.mock.calls)
            expect(call[1]).toBe("unique");
        editor.destroy();
    });

    it("covers lowercase, uppercase and non-alphanumeric failures", async () => {
        const { editor, addRuleSpy } = await setup();
        editor.value = "abcdef"; // no digit, uppercase, non-alnum
        const results = addRuleSpy.mock.calls.map(c => (c[0] as any)());
        expect(results.some(r => r != null)).toBe(true);
        editor.destroy();
    });
});
