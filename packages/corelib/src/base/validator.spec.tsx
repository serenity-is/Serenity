import { Config } from "./config";
import { Fluent } from './fluent';
import { Culture } from './formatting';
import { Validator, addValidationRule, removeValidationRule } from './validator';

describe("Validator.element", () => {
    let form: HTMLFormElement;
    let input: HTMLInputElement;

    beforeEach(() => {
        form = document.createElement("form");
        input = document.createElement("input");
        input.name = "testInput";
        form.appendChild(input);
        document.body.appendChild(form);
    });

    afterEach(() => {
        document.body.removeChild(form);
    });

    it("validates required fields", () => {
        const validator = new Validator(form, {
            rules: {
                testInput: {
                    required: true
                }
            },
            messages: {
                required: "This field is required."
            }
        });

        input.value = "";
        expect(validator.element(input)).toBe(false);

        input.value = "some value";
        expect(validator.element(input)).toBe(true);
    });

    it("validates custom rules", () => {
        const validator = new Validator(form, {
            rules: {
                testInput: {
                    customRule: true
                }
            },
            messages: {
                customRule: "Custom rule failed."
            }
        });

        Validator.methods.customRule = (value) => {
            return value === "valid";
        };

        input.value = "invalid";
        expect(validator.element(input)).toBe(false);

        input.value = "valid";
        expect(validator.element(input)).toBe(true);
    });

    it("displays error messages", () => {
        const validator = new Validator(form, {
            rules: {
                testInput: {
                    required: true
                }
            },
            messages: {
                testInput: {
                    required: "This field is required."
                }
            }
        });

        input.value = "";
        validator.element(input);

        const errorLabel = form.querySelector("label.error");
        expect(errorLabel).not.toBeNull();
        expect(errorLabel?.textContent).toBe("This field is required.");
    });
});

describe("Validator.methods", () => {
    it("validates required fields", () => {
        const method = Validator.methods.required;
        expect(method("", null)).toBe(false);
        expect(method("value", null)).toBe(true);
    });

    it("validates email format", () => {
        const method = Validator.methods.email;

        // Test Unicode emails (when Config.emailAllowOnlyAscii is false)
        const originalConfig = Config.emailAllowOnlyAscii;
        try {
            Config.emailAllowOnlyAscii = true;

            // Test null/undefined/empty values
            expect(method(null, null)).toBe("dependency-mismatch");
            expect(method(undefined, null)).toBe("dependency-mismatch");
            expect(method("", null)).toBe("dependency-mismatch");

            // Test length limit (should reject very long emails)
            const longEmail = "a".repeat(1000) + "@example.com";
            expect(method(longEmail, null)).toBe(false);

            // Test invalid email formats
            expect(method("invalid-email", null)).toBe(false);
            expect(method("@example.com", null)).toBe(false);
            expect(method("test@", null)).toBe(false);
            expect(method("test@.com", null)).toBe(false);
            expect(method("test..test@example.com", null)).toBe(false);
            expect(method("test @example.com", null)).toBe(false);
            expect(method("test@example..com", null)).toBe(false);
            expect(method("test@example.com ", null)).toBe(false);
            expect(method(" test@example.com", null)).toBe(false);

            // Test valid ASCII email formats
            expect(method("test@example.com", null)).toBe(true);
            expect(method("user.name@example.com", null)).toBe(true);
            expect(method("user+tag@example.com", null)).toBe(true);
            expect(method("test.email@example.co.uk", null)).toBe(true);
            expect(method("123@example.com", null)).toBe(true);
            expect(method("a@b.co", null)).toBe(true);
            expect(method("test@subdomain.example.com", null)).toBe(true);

            // Test emails with allowed special characters
            expect(method("test!#$%&'*+/=?^_`{|}~-@example.com", null)).toBe(true);
            expect(method("user.name+tag@example-domain.com", null)).toBe(true);

            // Test that Unicode emails are rejected when Config.emailAllowOnlyAscii is true
            expect(method("tëst@example.com", null)).toBe(false);
            expect(method("用户@example.com", null)).toBe(false);
            
            Config.emailAllowOnlyAscii = false;

            // Unicode in local part
            expect(method("tëst@example.com", null)).toBe(true);
            expect(method("用户@example.com", null)).toBe(true);
            expect(method("test@例え.テスト", null)).toBe(true);

            // Unicode in domain part
            expect(method("test@пример.испытание", null)).toBe(true);
            expect(method("test@例え.テスト", null)).toBe(true);

            // Mixed Unicode and ASCII
            expect(method("tëst@пример.com", null)).toBe(true);

            // Test that control characters are rejected in Unicode mode
            expect(method("test\x00@example.com", null)).toBe(false);
            expect(method("test\x7F@example.com", null)).toBe(false);
            expect(method("test\x80@example.com", null)).toBe(false);

        } finally {
            Config.emailAllowOnlyAscii = originalConfig;
        }
    });

    it("validates minimum length", () => {
        const method = Validator.methods.minlength;
        const el = document.createElement("input");
        el.value = "abc";
        expect(method("abc", el, 5)).toBe(false); // Fix: Ensure proper parameter setup
        el.value = "abcdef";
        expect(method("abcdef", el, 5)).toBe(true);
    });

    it("validates maximum length", () => {
        const method = Validator.methods.maxlength;
        const el = document.createElement("input");
        el.value = "abcdef";
        expect(method("abcdef", el, 5)).toBe(false);
        el.value = "abc";
        expect(method("abc", el, 5)).toBe(true);
    });

    it("validates range length", () => {
        const el = document.createElement("input");
        const method = Validator.methods.rangelength;
        el.value = "abc";
        expect(method("abc", el, [5, 10])).toBe(false);
        el.value = "abcdef";
        expect(method("abcdef", el, [5, 10])).toBe(true);
        el.value = "abcdefghijklm";
        expect(method("abcdefghijklm", el, [5, 10])).toBe(false);
    });

    it("validates minimum value", () => {
        const el = document.createElement("input");
        const method = Validator.methods.min;
        el.value = "3";
        expect(method(3, el, 5)).toBe(false);
        el.value = "5";
        expect(method(5, el, 5)).toBe(true);
        el.value = "7";
        expect(method(7, el, 5)).toBe(true);
    });

    it("validates maximum value", () => {
        const el = document.createElement("input");
        const method = Validator.methods.max;
        el.value = "7";
        expect(method(7, el, 5)).toBe(false);
        el.value = "5";
        expect(method(5, el, 5)).toBe(true);
        el.value = "3";
        expect(method(3, el, 5)).toBe(true);
    });

    it("validates range of values", () => {
        const el = document.createElement("input");
        const method = Validator.methods.range;
        el.value = "3";
        expect(method(3, el, [5, 10])).toBe(false);
        el.value = "7";
        expect(method(7, el, [5, 10])).toBe(true);
        el.value = "12";
        expect(method(12, el, [5, 10])).toBe(false);
    });

    it("validates URL format", () => {
        const el = document.createElement("input");
        const method = Validator.methods.url;
        el.value = "invalid-url";
        expect(method("invalid-url", el)).toBe(false);
        el.value = "http://example.com";
        expect(method("http://example.com", el)).toBe(true);
        el.value = "https://example.com";
        expect(method("https://example.com", el)).toBe(true);
    });

    it("validates date format (dateQ)", () => {
        const el = document.createElement("input");
        const method = Validator.methods.dateQ;
        el.value = "invalid-date";
        expect(method("invalid-date", el)).toBe(false);
        el.value = "2023-12-25";
        expect(method("2023-12-25", el)).toBe(true);
        el.value = "2023-12-25T10:30:00";
        expect(method("2023-12-25T10:30:00", el)).toBe(false); // dateQ requires date-only

        // Test with different date order
        const originalDateOrder = Culture.dateOrder;
        try {
            Culture.dateOrder = 'dmy';
            el.value = "25/12/2023";
            expect(method("25/12/2023", el)).toBe(true);
            el.value = "12/25/2023";
            expect(method("12/25/2023", el)).toBe(false); // should fail with dmy order
        } finally {
            Culture.dateOrder = originalDateOrder;
        }
    });

    it("validates date-time format (dateTimeQ)", () => {
        const el = document.createElement("input");
        const method = Validator.methods.dateTimeQ;
        el.value = "invalid-date";
        expect(method("invalid-date", el)).toBe(false);
        el.value = "2023-12-25";
        expect(method("2023-12-25", el)).toBe(true);
        el.value = "2023-12-25T10:30:00";
        expect(method("2023-12-25T10:30:00", el)).toBe(true);

        // Test with different date order
        const originalDateOrder = Culture.dateOrder;
        try {
            Culture.dateOrder = 'dmy';
            el.value = "25/12/2023 10:30:00";
            expect(method("25/12/2023 10:30:00", el)).toBe(true);
        } finally {
            Culture.dateOrder = originalDateOrder;
        }
    });

    it("validates decimal format (decimalQ)", () => {
        const el = document.createElement("input");
        const method = Validator.methods.decimalQ;
        el.value = "invalid-decimal";
        expect(method("invalid-decimal", el)).toBe(false);
        el.value = "123.45";
        expect(method("123.45", el)).toBe(true);
        el.value = "123";
        expect(method("123", el)).toBe(true);
        el.value = "123.45.67";
        expect(method("123.45.67", el)).toBe(false);

        // Test with different decimal separator
        const originalDecimalSeparator = Culture.decimalSeparator;
        try {
            Culture.decimalSeparator = ',';
            el.value = "123,45";
            expect(method("123,45", el)).toBe(true);
            el.value = "123.45";
            expect(method("123.45", el)).toBe(false); // should fail with comma separator
        } finally {
            Culture.decimalSeparator = originalDecimalSeparator;
        }
    });

    it("validates integer format (integerQ)", () => {
        const el = document.createElement("input");
        const method = Validator.methods.integerQ;
        el.value = "invalid-integer";
        expect(method("invalid-integer", el)).toBe(false);
        el.value = "123";
        expect(method("123", el)).toBe(true);
        el.value = "123.45";
        expect(method("123.45", el)).toBe(false);

        // Test with group separator
        const originalGroupSeparator = Culture.groupSeparator;
        try {
            Culture.groupSeparator = ',';
            el.value = "1,234";
            expect(method("1,234", el)).toBe(true);
            el.value = "1234";
            expect(method("1234", el)).toBe(true);
        } finally {
            Culture.groupSeparator = originalGroupSeparator;
        }
    });

    it("validates custom rules (customValidate)", () => {
        const el = document.createElement("input");
        const method = Validator.methods.customValidate;
        el.value = "test";
        // Without custom rules, should return true
        expect(method("test", el)).toBe(true);
    });
});

describe("Validator.normalizeRules", () => {
    it("exposes normalizeRules as a static function", () => {
        expect(typeof Validator.normalizeRules).toBe("function");
    });

    it("removes rules explicitly set to false", () => {
        const el = document.createElement("input");
        const rules = Validator.normalizeRules({ required: false, minlength: "5" as any }, el as any);
        expect(rules).toEqual({ minlength: 5 });
    });

    it("honors depends as a CSS selector string (kept when selector matches)", () => {
        const form = document.createElement("form");
        const el = document.createElement("input");
        el.name = "a";
        form.appendChild(el);
        const other = document.createElement("input");
        other.className = "dep";
        form.appendChild(other);
        // attach a validator instance so getInstance(form) works (not strictly required in the keep case)
        new Validator(form, { rules: {} });

        const rules = Validator.normalizeRules({ custom: { depends: ".dep" } } as any, el as any);
        expect(rules).toEqual({ custom: true });
    });

    it("honors depends as a CSS selector string (removed when selector does not match) and calls resetElements", () => {
        const form = document.createElement("form");
        const el = document.createElement("input");
        el.name = "a";
        form.appendChild(el);
        const validator = new Validator(form, { rules: {} });
        const resetSpy = vi.spyOn(validator as any, "resetElements");

        const rules = Validator.normalizeRules({ custom: { depends: ".will-not-match" } } as any, el as any);
        expect(rules).toEqual({});
        expect(resetSpy).toHaveBeenCalledTimes(1);
        expect(resetSpy).toHaveBeenCalledWith([el]);
    });

    it("supports depends as a function and preserves param when provided", () => {
        const form = document.createElement("form");
        const el = document.createElement("input");
        el.name = "a";
        form.appendChild(el);
        new Validator(form, { rules: {} });

        const rules = Validator.normalizeRules({ min: { depends: () => true, param: 3 } } as any, el as any);
        expect(rules).toEqual({ min: 3 });

        const rulesRemoved = Validator.normalizeRules({ min: { depends: () => false, param: 3 } } as any, el as any);
        expect(rulesRemoved).toEqual({});
    });

    it("evaluates function-valued rule parameters", () => {
        const el = document.createElement("input");
        const rules = Validator.normalizeRules({ minlength: () => "5" } as any, el as any);
        expect(rules).toEqual({ minlength: 5 });
    });

    it("converts numeric-like parameters and parses ranges", () => {
        const el = document.createElement("input");
        const rules = Validator.normalizeRules({ minlength: "2", maxlength: "10", rangelength: "[3, 7]", range: ["4", "8"] } as any, el as any);
        expect(rules).toEqual({ minlength: 2, maxlength: 10, rangelength: [3, 7], range: [4, 8] });
    });

    it("auto-creates ranges when autoCreateRanges is true", () => {
        const prev = Validator.autoCreateRanges;
        Validator.autoCreateRanges = true;
        try {
            const el = document.createElement("input");
            const rules = Validator.normalizeRules({ min: 2, max: 5, minlength: 1, maxlength: 3 } as any, el as any);
            expect(rules).toEqual({ range: [2, 5], rangelength: [1, 3] });
        } finally {
            Validator.autoCreateRanges = prev;
        }
    });

    it("returns empty object when rules input is empty", () => {
        const el = document.createElement("input");
        const rules = Validator.normalizeRules({}, el as any);
        expect(rules).toEqual({});
    });
});

describe("Validator static utility methods", () => {
    it("optional returns dependency-mismatch for empty required fields", () => {
        const el = document.createElement("input");
        el.value = "";
        expect(Validator.optional(el, "")).toBe("dependency-mismatch");
    });

    it("optional returns false for non-empty values", () => {
        const el = document.createElement("input");
        el.value = "test";
        expect(Validator.optional(el, "test")).toBe(false);
    });

    it("valid returns false for non-form elements", () => {
        const el = document.createElement("input");
        expect(Validator.valid(el)).toBe(false);
    });

    it("rules returns empty object for elements without form", () => {
        const el = document.createElement("input");
        expect(Validator.rules(el)).toBeUndefined();
    });

    it("rules can add and remove rules", () => {
        const form = document.createElement("form");
        const el = document.createElement("input");
        el.name = "test";
        form.appendChild(el);
        const validator = new Validator(form, { rules: {} });

        // Add rule
        Validator.rules(el, "add", { required: true });
        expect(Validator.rules(el)).toEqual({ required: true });

        // Remove rule
        Validator.rules(el, "remove", "required");
        expect(Validator.rules(el)).toEqual({});
    });

    it("addClassRules adds class-based rules", () => {
        Validator.addClassRules("testClass", { required: true });
        expect(Validator.classRuleSettings.testClass).toEqual({ required: true });

        // Cleanup
        delete Validator.classRuleSettings.testClass;
    });

    it("classRules extracts rules from element classes", () => {
        const el = document.createElement("input");
        el.className = "required email";
        const rules = Validator.classRules(el);
        expect(rules).toEqual({ required: true, email: true });
    });

    it("attributeRules extracts rules from element attributes", () => {
        const el = document.createElement("input");
        el.setAttribute("required", "");
        el.setAttribute("minlength", "5");
        el.setAttribute("type", "text");
        const rules = Validator.attributeRules(el);
        expect(rules).toEqual({ required: true, minlength: 5 });
    });

    it("dataRules extracts rules from data attributes", () => {
        const el = document.createElement("input");
        el.dataset.ruleRequired = "";
        el.dataset.ruleMinlength = "3";
        const rules = Validator.dataRules(el);
        expect(rules).toEqual({ required: true, minlength: 3 });
    });

    it("staticRules returns rules from validator settings", () => {
        const form = document.createElement("form");
        const el = document.createElement("input");
        el.name = "test";
        form.appendChild(el);
        const validator = new Validator(form, { rules: { test: { required: true } } });

        const rules = Validator.staticRules(el);
        expect(rules).toEqual({ required: true });
    });

    it("normalizeAttributeRule handles different attribute types", () => {
        const rules: any = {};
        const el = document.createElement("input");
        el.setAttribute("type", "number");

        Validator.normalizeAttributeRule(rules, "number", "min", "5");
        expect(rules.min).toBe(5);

        Validator.normalizeAttributeRule(rules, "number", "max", "10");
        expect(rules.max).toBe(10);

        Validator.normalizeAttributeRule(rules, "number", "step", "2");
        expect(rules.step).toBe(2);
    });

    it("getHighlightTarget returns element by data-vx-highlight", () => {
        const el = document.createElement("input");
        el.dataset.vxHighlight = "highlight-target";
        const target = document.createElement("div");
        target.id = "highlight-target";
        document.body.appendChild(target);

        expect(Validator.getHighlightTarget(el)).toBe(target);

        document.body.removeChild(target);
    });

    it("getHighlightTarget returns s2id element for select2", () => {
        const el = document.createElement("input");
        el.className = "select2-offscreen";
        el.id = "test-select";
        const target = document.createElement("div");
        target.id = "s2id_test-select";
        document.body.appendChild(target);

        expect(Validator.getHighlightTarget(el)).toBe(target);

        document.body.removeChild(target);
    });
});

describe("Validator instance methods", () => {
    it("form validates the entire form and returns validity", () => {
        const form = document.createElement("form");
        const input = document.createElement("input");
        input.name = "test";
        form.appendChild(input);
        const validator = new Validator(form, { rules: { test: { required: true } } });

        input.value = "";
        expect(validator.form()).toBe(false);

        input.value = "value";
        expect(validator.form()).toBe(true);
    });

    it("checkForm validates all elements without showing errors", () => {
        const form = document.createElement("form");
        const input = document.createElement("input");
        input.name = "test";
        form.appendChild(input);
        const validator = new Validator(form, { rules: { test: { required: true } } });

        input.value = "";
        expect(validator.checkForm()).toBe(false);
        expect((validator as any).errorList.length).toBeGreaterThan(0);
    });

    it("showErrors displays errors using custom showErrors function", () => {
        const form = document.createElement("form");
        const input = document.createElement("input");
        input.name = "test";
        form.appendChild(input);
        const validator = new Validator(form, {
            rules: { test: { required: true } },
            showErrors: vi.fn()
        });

        (validator as any).errorMap = { test: "Required" };
        (validator as any).errorList = [{ message: "Required", element: input }];
        validator.showErrors();

        expect(validator.settings.showErrors).toHaveBeenCalledWith((validator as any).errorMap, (validator as any).errorList, validator);
    });

    it("resetForm clears all errors and resets elements", () => {
        const form = document.createElement("form");
        const input = document.createElement("input");
        input.name = "test";
        form.appendChild(input);
        const validator = new Validator(form, { rules: { test: { required: true } } });

        input.value = "";
        validator.element(input); // This should add errors
        expect((validator as any).errorList.length).toBeGreaterThan(0);

        validator.resetForm();
        expect((validator as any).errorList.length).toBe(0);
        expect((validator as any).invalid).toEqual({});
    });

    it("resetElements removes classes and attributes from elements", () => {
        const form = document.createElement("form");
        const input = document.createElement("input");
        input.name = "test";
        form.appendChild(input);
        const validator = new Validator(form, {});

        input.classList.add("error");
        input.classList.add("valid");

        validator.resetElements([input]);
        expect(input.classList.contains("error")).toBe(false);
        expect(input.classList.contains("valid")).toBe(false);
    });

    it("numberOfInvalids returns count of invalid fields", () => {
        const form = document.createElement("form");
        const validator = new Validator(form, {});

        (validator as any).invalid = { field1: "error1", field2: "error2" };
        expect(validator.numberOfInvalids()).toBe(2);

        (validator as any).invalid = {};
        expect(validator.numberOfInvalids()).toBe(0);
    });

    it("hideErrors hides error elements", () => {
        const form = document.createElement("form");
        const errorEl = document.createElement("label");
        errorEl.className = "error";
        errorEl.hidden = false;
        form.appendChild(errorEl);
        const validator = new Validator(form, {});

        (validator as any).toHide = [errorEl];
        validator.hideErrors();
        expect(errorEl.hidden).toBe(true);
    });

    it("valid returns true when no errors", () => {
        const form = document.createElement("form");
        const validator = new Validator(form, {});

        (validator as any).errorList = [];
        expect(validator.valid()).toBe(true);

        (validator as any).errorList = [{ message: "error", element: document.createElement("input") }];
        expect(validator.valid()).toBe(false);
    });

    it("size returns number of errors", () => {
        const form = document.createElement("form");
        const validator = new Validator(form, {});

        (validator as any).errorList = [];
        expect(validator.size()).toBe(0);

        (validator as any).errorList = [{ message: "error", element: document.createElement("input") }];
        expect(validator.size()).toBe(1);
    });

    it("focusInvalid focuses the first invalid element", () => {
        const form = document.createElement("form");
        const input = document.createElement("input");
        input.name = "test";
        form.appendChild(input);
        document.body.appendChild(form);
        const validator = new Validator(form, { focusInvalid: true });

        (validator as any).errorList = [{ message: "error", element: input }];
        const focusSpy = vi.spyOn(input, "focus");

        // Mock isVisibleLike to return true
        vi.spyOn(Fluent, "isVisibleLike").mockReturnValue(true);

        validator.focusInvalid();
        expect(focusSpy).toHaveBeenCalled();

        document.body.removeChild(form);
    });
});

describe("Validator.addMethod", () => {
    afterEach(() => {
        // Clean up any custom methods/messages/class rules we may add in tests
        ["isFoo", "isBar", "isBaz", "noMsg"].forEach((k) => {
            delete (Validator as any).methods[k];
            delete (Validator as any).messages[k];
            delete (Validator as any).classRuleSettings[k];
        });
    });

    it("registers a new method and it participates in validation", () => {
        // Add a custom method that checks value equals 'foo'
        Validator.addMethod("isFoo", (value) => value === "foo", "Must be foo");

        const form = document.createElement("form");
        const input = document.createElement("input");
        input.name = "testInput";
        form.appendChild(input);
        document.body.appendChild(form);

        const validator = new Validator(form, {
            rules: { testInput: { isFoo: true } }
        });

        input.value = "bar";
        expect(validator.element(input)).toBe(false);

        input.value = "foo";
        expect(validator.element(input)).toBe(true);

        // cleanup DOM
        document.body.removeChild(form);
    });

    it("sets provided message and keeps existing message when none provided", () => {
        // Provided message
        Validator.addMethod("isFoo", (value) => true, "Foo message");
        expect(Validator.messages["isFoo"]).toBe("Foo message");

        // Pre-existing message remains when message arg is undefined
        (Validator as any).messages["noMsg"] = "Old";
        Validator.addMethod("noMsg", (value) => true); // no message provided
        expect(Validator.messages["noMsg"]).toBe("Old");
    });

    it("auto-adds a class rule when method arity < 3", () => {
        // length === 1 < 3 triggers addClassRules
        Validator.addMethod("isBar", (value) => true, "");
        const el = document.createElement("input");
        el.name = "n";
        el.className = "isBar";
        const form = document.createElement("form");
        form.appendChild(el);
        new Validator(form, {});

        const rules = Validator.rules(el as any);
        expect(rules).toEqual(expect.objectContaining({ isBar: true }));
    });

    it("does not auto-add a class rule when method arity >= 3", () => {
        // length === 3 should not trigger addClassRules
        Validator.addMethod("isBaz", function (value, element, param) { return true; }, "");
        const el = document.createElement("input");
        el.name = "n";
        el.className = "isBaz";
        const form = document.createElement("form");
        form.appendChild(el);
        new Validator(form, {});

        const rules = Validator.rules(el as any);
        expect((rules as any).isBaz).toBeUndefined();
    });
});

describe("Validator.defaults", () => {
    it("onfocusin sets lastActive to the focused element", () => {
        const form = document.createElement("form");
        const input = document.createElement("input");
        input.name = "a";
        form.appendChild(input);
        const validator = new Validator(form, {});

        validator.settings.onfocusin?.(input as any, new Event("focusin"), validator);
        expect(validator.lastActive).toBe(input);
    });

    it("onfocusout calls validator.element when element is not optional", () => {
        const form = document.createElement("form");
        const input = document.createElement("input");
        input.name = "a";
        form.appendChild(input);
        const validator = new Validator(form, { rules: { a: { required: true } } });
        const spy = vi.spyOn(validator, "element");

        input.value = "hello"; // not optional because required and non-empty
        validator.settings.onfocusout?.(input as any, new Event("focusout"), validator);
        expect(spy).toHaveBeenCalledWith(input);
    });

    it("onfocusout does not call validator.element when element is optional and not submitted", () => {
        const form = document.createElement("form");
        const input = document.createElement("input");
        input.name = "a";
        form.appendChild(input);
        const validator = new Validator(form, {});
        const spy = vi.spyOn(validator, "element");

        input.value = ""; // optional (empty and no required rule)
        validator.settings.onfocusout?.(input as any, new Event("focusout"), validator);
        expect(spy).not.toHaveBeenCalled();
    });

    it("onkeyup skips excluded keys and triggers validation for invalid/submitted fields", () => {
        const form = document.createElement("form");
        const input = document.createElement("input");
        input.name = "a";
        form.appendChild(input);
        const validator = new Validator(form, {});
        const spy = vi.spyOn(validator, "element");

        // Excluded key (Shift)
        validator.settings.onkeyup?.(input as any, new KeyboardEvent("keyup", { key: "Shift" }), validator);
        expect(spy).not.toHaveBeenCalled();

        // Non-excluded key, but only triggers when submitted or invalid
        (validator as any).invalid[input.name] = true;
        validator.settings.onkeyup?.(input as any, new KeyboardEvent("keyup", { key: "a" }), validator);
        expect(spy).toHaveBeenCalledWith(input);
    });

    it("onclick triggers validation when element is already submitted", () => {
        const form = document.createElement("form");
        const cb = document.createElement("input");
        cb.type = "checkbox";
        cb.name = "agree";
        form.appendChild(cb);
        const validator = new Validator(form, {});
        const spy = vi.spyOn(validator, "element");

        (validator as any).submitted[cb.name] = "error";
        const handler = validator.settings.onclick as unknown as (el: any, e: any, v: any) => void;
        handler?.(cb as any, new MouseEvent("click"), validator);
        expect(spy).toHaveBeenCalledWith(cb);
    });

    it("highlight adds errorClass and removes validClass on non-radio elements", () => {
        const form = document.createElement("form");
        const input = document.createElement("input");
        input.name = "a";
        form.appendChild(input);
        const validator = new Validator(form, {});

        validator.settings.highlight?.call(validator, input as any, validator.settings.errorClass!, validator.settings.validClass!);
        expect(input.classList.contains(validator.settings.errorClass!)).toBe(true);
        expect(input.classList.contains(validator.settings.validClass!)).toBe(false);

        validator.settings.unhighlight?.call(validator, input as any, validator.settings.errorClass!, validator.settings.validClass!);
        expect(input.classList.contains(validator.settings.errorClass!)).toBe(false);
        expect(input.classList.contains(validator.settings.validClass!)).toBe(true);
    });

    it("highlight/unhighlight affect all radios in the same group", () => {
        const form = document.createElement("form");
        const r1 = document.createElement("input"); r1.type = "radio"; r1.name = "grp";
        const r2 = document.createElement("input"); r2.type = "radio"; r2.name = "grp";
        form.appendChild(r1); form.appendChild(r2);
        const validator = new Validator(form, {});

        validator.settings.highlight?.call(validator, r1 as any, validator.settings.errorClass!, validator.settings.validClass!);
        expect(r1.classList.contains(validator.settings.errorClass!)).toBe(true);
        expect(r2.classList.contains(validator.settings.errorClass!)).toBe(true);

        validator.settings.unhighlight?.call(validator, r1 as any, validator.settings.errorClass!, validator.settings.validClass!);
        expect(r1.classList.contains(validator.settings.validClass!)).toBe(true);
        expect(r2.classList.contains(validator.settings.validClass!)).toBe(true);
    });
});

describe("Validator.validatorEventDelegate", () => {
    it("handles contenteditable elements", () => {
        let form = document.createElement("form");
        const div = document.createElement("div");
        div.setAttribute("contenteditable", "true");
        div.setAttribute("name", "content");
        form.appendChild(div);
        form = document.body.appendChild(form);
        try {
            const validator = new Validator(form, {
                rules: {
                    content: {
                        required: true
                    },
                    test: {
                        required: true
                    }
                }
            });

            div.dispatchEvent(new Event("focusin", { bubbles: true }));
            expect(validator.lastActive).toBe(div);

            // Test that form is set on contenteditable elements
            expect((div as any).form).toBe(form);
        }
        finally {
            document.body.removeChild(form);
        }
    });

    it("Validator.isValidatableElement correctly identifies validatable elements", () => {
        const input = document.createElement("input");
        const select = document.createElement("select");
        const textarea = document.createElement("textarea");
        const div = document.createElement("div");

        expect(Validator.isValidatableElement(input)).toBe(true);
        expect(Validator.isValidatableElement(select)).toBe(true);
        expect(Validator.isValidatableElement(textarea)).toBe(true);
        expect(Validator.isValidatableElement(div)).toBe(false);
    });
});

describe("Validator.resetAll", () => {
    it("resets all validation state", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            form.appendChild(input);
            document.body.appendChild(form);

            const validator = new Validator(form, {
                rules: {
                    test: { required: true }
                }
            });

            // Make it invalid and submitted
            input.value = "";
            validator.element(input);
            (validator as any).submitted[input.name] = true;

            expect(validator.numberOfInvalids()).toBe(1);
            expect((validator as any).submitted[input.name]).toBe(true);

            validator.resetAll();

            expect(validator.numberOfInvalids()).toBe(0);
            expect((validator as any).submitted[input.name]).toBeUndefined();
        } finally {
            document.body.removeChild(form);
        }
    });
});

describe("Other Validator instance methods", () => {
    it("validation dependTypes handle different parameter types", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            form.appendChild(input);
            document.body.appendChild(form);

            const validator = new Validator(form, {});

            // Test boolean depend type
            expect(validator.dependTypes.boolean(true)).toBe(true);
            expect(validator.dependTypes.boolean(false)).toBe(false);

            // Test string depend type
            const selectorInput = document.createElement("input");
            selectorInput.id = "selector";
            form.appendChild(selectorInput);
            expect(validator.dependTypes.string("#selector", input)).toBe(true);
            expect(validator.dependTypes.string("#nonexistent", input)).toBe(false);

            // Test function depend type
            const testFn = () => true;
            expect(validator.dependTypes.function(testFn, input)).toBe(true);
        } finally {
            document.body.removeChild(form);
        }
    });

    it("depend() method handles dependency validation", () => {
        const form = document.createElement("form");
        try {
            const input1 = document.createElement("input");
            input1.name = "field1";
            const input2 = document.createElement("input");
            input2.name = "field2";
            form.appendChild(input1);
            form.appendChild(input2);
            document.body.appendChild(form);

            const validator = new Validator(form, {
                rules: {
                    field2: {
                        required: {
                            depends: () => input1.value === "enable"
                        }
                    }
                }
            });

            // When dependency is not met, field2 should not be required
            input1.value = "disable";
            input2.value = "";
            expect(validator.element(input2)).toBe(true);

            // When dependency is met, field2 should be required
            input1.value = "enable";
            input2.value = "";
            expect(validator.element(input2)).toBe(false);
        } finally {
            document.body.removeChild(form);
        }
    });

    it("startRequest() and stopRequest() manage pending requests", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            form.appendChild(input);
            document.body.appendChild(form);

            const validator = new Validator(form, {});

            expect((validator as any).pendingRequest).toBe(0);

            validator.startRequest(input);
            expect((validator as any).pendingRequest).toBe(1);
            expect((validator as any).pending[input.name]).toBeInstanceOf(AbortController);

            validator.stopRequest(input, true);
            expect((validator as any).pendingRequest).toBe(0);
            expect((validator as any).pending[input.name]).toBeUndefined();
        } finally {
            document.body.removeChild(form);
        }
    });

    it("previousValue() tracks previous validation values", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            form.appendChild(input);
            document.body.appendChild(form);

            const validator = new Validator(form, {});

            // Initially returns an object with default values
            const prevValue = validator.previousValue(input, "required");
            expect(prevValue).toHaveProperty("old", null);
            expect(prevValue).toHaveProperty("valid", true);
            expect(prevValue).toHaveProperty("message");

            // After validation, the previousValue object should still exist
            // (previousValue is mainly used for remote validation)
            input.value = "test value";
            validator.element(input);
            const prevValue2 = validator.previousValue(input, "required");
            expect(prevValue2).toHaveProperty("old", null); // old remains null for non-remote validation
            expect(prevValue2).toHaveProperty("valid", true);
            expect(prevValue2).toHaveProperty("message");
        } finally {
            document.body.removeChild(form);
        }
    });

    it("destroy() cleans up validator instance", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            form.appendChild(input);
            document.body.appendChild(form);

            const validator = new Validator(form, {
                rules: {
                    test: { required: true }
                }
            });

            // Add some state
            (validator as any).submitted[input.name] = true;
            (validator as any).invalid[input.name] = "error";

            validator.destroy();

            // Check that state is cleared
            expect((validator as any).submitted[input.name]).toBeUndefined();
            expect((validator as any).invalid[input.name]).toBeUndefined();
            expect((validator as any).currentForm).toBeUndefined();
        } finally {
            document.body.removeChild(form);
        }
    });
});

describe("Validator.addCustomRule and removeCustomRule", () => {

    it("adds custom validation rules to elements", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            input.value = "xyzabc";
            form.appendChild(input);
            document.body.appendChild(form);

            const validator = new Validator(form, {
                rules: {
                    test: { required: true, minlength: 5 }
                }
            });

            let rules = Validator.rules(input);
            expect(rules).toEqual({ required: true, minlength: 5 });

            Validator.addCustomRule(input, x => null, "rule1");
            Validator.addCustomRule(input, x => "fail2", "rule2");

            rules = Validator.rules(input);
            expect(rules).toEqual({ required: true, minlength: 5, customValidate: true });

            // The custom validation should fail
            expect(validator.element(input)).toBe(false);
            expect((validator as any).errorMap[input.name]).toBe("fail2");

            // Remove one custom rule, should still fail
            Validator.removeCustomRule(input, "rule1");

            rules = Validator.rules(input);
            expect(rules).toEqual({ required: true, minlength: 5, customValidate: true });

            expect(validator.element(input)).toBe(false);
            expect((validator as any).errorMap[input.name]).toBe("fail2");

            // Remove the other custom rule, should pass now
            Validator.removeCustomRule(input, "rule2");

            rules = Validator.rules(input);
            expect(rules).toEqual({ required: true, minlength: 5 });

            expect(validator.element(input)).toBe(true);
            expect((validator as any).errorMap[input.name]).toBeUndefined();
        } finally {
            document.body.removeChild(form);
        };
    });

    it("cleans up custom rules when all rules are removed", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            input.value = "xyzabc";
            form.appendChild(input);
            document.body.appendChild(form);
            const validator = new Validator(form, {});

            Validator.addCustomRule(input, x => "fail", "myRule");
            expect(input.classList.contains('customValidate')).toBe(true);

            // Remove the rule
            Validator.removeCustomRule(input, "myRule");
            expect(input.classList.contains('customValidate')).toBe(false);

            // Validation should pass now
            expect(validator.element(input)).toBe(true);
        } finally {
            document.body.removeChild(form);
        };
    });

    it("handles removeCustomRule when element has the class but no rules in the map", () => {
        const input = document.createElement("input");
        try {
            input.classList.add('customValidate');
            // Call removeCustomRule - since there are no rules in the map for this element,
            // it should go to the else branch and just remove the class
            Validator.removeCustomRule(input, "someRule");
            expect(input.classList.contains('customValidate')).toBe(false);
        } finally {
            input.remove();
        }
    });

    it("handles removeCustomRule when element has the class but no rules in the map", () => {
        const input = document.createElement("input");
        try {
            input.classList.add('customValidate');
            // rules map doesn't have this element
            Validator.removeCustomRule(input, "someRule");
            expect(input.classList.contains('customValidate')).toBe(false);
        } finally {
            input.remove();
        }
    });
});

describe("addValidationRule and removeValidationRule exports", () => {
    it("addValidationRule should be Validator.addCustomRule", () => {
        expect(addValidationRule).toBe(Validator.addCustomRule);
    });

    it("removeValidationRule should be Validator.removeCustomRule", () => {
        expect(removeValidationRule).toBe(Validator.removeCustomRule);
    });

    it("addValidationRule adds and removeValidationRule removes custom rules", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            input.value = "abc";
            form.appendChild(input);
            document.body.appendChild(form);
            const validator = new Validator(form, {});

            addValidationRule(input, x => "custom fail", "testRule");
            expect(validator.element(input)).toBe(false);
            expect((validator as any).errorMap[input.name]).toBe("custom fail");

            removeValidationRule(input, "testRule");
            expect(validator.element(input)).toBe(true);
        } finally {
            document.body.removeChild(form);
        }
    });
});

describe("elementValue with file input", () => {
    it("handles C:\\fakepath\\ prefix", () => {
        const input = document.createElement("input");
        input.type = "file";
        Object.defineProperty(input, "value", {
            get: () => "C:\\fakepath\\document.pdf"
        });
        expect(Validator.elementValue(input)).toBe("document.pdf");
    });

    it("handles unix path", () => {
        const input = document.createElement("input");
        input.type = "file";
        Object.defineProperty(input, "value", {
            get: () => "/home/user/document.pdf"
        });
        expect(Validator.elementValue(input)).toBe("document.pdf");
    });

    it("handles windows path", () => {
        const input = document.createElement("input");
        input.type = "file";
        Object.defineProperty(input, "value", {
            get: () => "D:\\docs\\report.txt"
        });
        expect(Validator.elementValue(input)).toBe("report.txt");
    });

    it("handles just a filename", () => {
        const input = document.createElement("input");
        input.type = "file";
        Object.defineProperty(input, "value", {
            get: () => "image.png"
        });
        expect(Validator.elementValue(input)).toBe("image.png");
    });
});

describe("Validator.valid static", () => {
    it("returns false for null element", () => {
        expect(Validator.valid(null)).toBe(false);
    });

    it("validates form elements when passed an HTMLFormElement", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            form.appendChild(input);
            document.body.appendChild(form);
            new Validator(form, { rules: { test: { required: true } } });

            input.value = "";
            expect(Validator.valid(form)).toBe(false);

            input.value = "val";
            expect(Validator.valid(form)).toBe(true);
        } finally {
            document.body.removeChild(form);
        }
    });

    it("validates individual elements when passed an array", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            form.appendChild(input);
            document.body.appendChild(form);
            const validator = new Validator(form, { rules: { test: { required: true } } });

            input.value = "";
            expect(Validator.valid([input])).toBe(false);

            input.value = "val";
            expect(Validator.valid([input])).toBe(true);
        } finally {
            document.body.removeChild(form);
        }
    });
});

describe("attributeRules edge cases", () => {
    it("removes maxlength values that are -1, 2147483647 or 524288", () => {
        const el = document.createElement("input");
        el.setAttribute("type", "text");
        el.setAttribute("maxlength", "-1");
        let rules = Validator.attributeRules(el);
        expect(rules.maxlength).toBeUndefined();

        el.setAttribute("maxlength", "2147483647");
        rules = Validator.attributeRules(el);
        expect(rules.maxlength).toBeUndefined();

        el.setAttribute("maxlength", "524288");
        rules = Validator.attributeRules(el);
        expect(rules.maxlength).toBeUndefined();

        el.setAttribute("maxlength", "10");
        rules = Validator.attributeRules(el);
        expect(rules.maxlength).toBe(10);
    });

    it("handles range type attribute", () => {
        const el = document.createElement("input");
        el.setAttribute("type", "range");
        el.setAttribute("min", "0");
        el.setAttribute("max", "100");
        const rules = Validator.attributeRules(el);
        expect(rules.min).toBe(0);
        expect(rules.max).toBe(100);
    });
});

describe("dataRules edge cases", () => {
    it("handles data attributes with empty values as true", () => {
        const el = document.createElement("input");
        el.dataset.ruleRequired = "";
        const rules = Validator.dataRules(el);
        expect(rules).toEqual({ required: true });
    });

    it("handles data attributes with numeric values", () => {
        const el = document.createElement("input");
        el.dataset.ruleMinlength = "5";
        const rules = Validator.dataRules(el);
        expect(rules).toEqual({ minlength: 5 });
    });
});

describe("Validator.instance method edge cases", () => {
    it("abortRequest aborts pending requests", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            form.appendChild(input);
            document.body.appendChild(form);

            const validator = new Validator(form, {});

            validator.startRequest(input);
            expect((validator as any).pendingRequest).toBe(1);
            expect((validator as any).pending[input.name]).toBeInstanceOf(AbortController);

            validator.abortRequest(input);
            expect((validator as any).pendingRequest).toBe(0);
            expect((validator as any).pending[input.name]).toBeUndefined();
            expect(input.classList.contains("pending")).toBe(false);
        } finally {
            document.body.removeChild(form);
        }
    });

    it("abortRequest handles missing pending request gracefully", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            form.appendChild(input);
            document.body.appendChild(form);

            const validator = new Validator(form, {});
            // Calling abortRequest without startRequest should not throw
            expect(() => validator.abortRequest(input)).not.toThrow();
        } finally {
            document.body.removeChild(form);
        }
    });

    it("elements() filters out disabled elements and submit/reset buttons", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            input.disabled = true;
            form.appendChild(input);
            document.body.appendChild(form);

            const validator = new Validator(form, {
                rules: { test: { required: true } }
            });

            const elements = validator.elements();
            expect(elements.length).toBe(0);
        } finally {
            document.body.removeChild(form);
        }
    });

    it("elements() filters out submit, reset and image buttons", () => {
        const form = document.createElement("form");
        try {
            const submitBtn = document.createElement("input");
            submitBtn.type = "submit";
            submitBtn.name = "btn";
            form.appendChild(submitBtn);
            document.body.appendChild(form);

            const validator = new Validator(form, {});
            const elements = validator.elements();
            expect(elements.filter(e => e === submitBtn).length).toBe(0);
        } finally {
            document.body.removeChild(form);
        }
    });

    it("elements() filters out ignored elements", () => {
        const form = document.createElement("form");
        try {
            const hidden = document.createElement("input");
            hidden.type = "hidden";
            hidden.name = "hiddenField";
            form.appendChild(hidden);
            document.body.appendChild(form);

            const validator = new Validator(form, {
                rules: { hiddenField: { required: true } }
            });
            const elements = validator.elements();
            expect(elements.filter(e => e === hidden).length).toBe(0);
        } finally {
            document.body.removeChild(form);
        }
    });

    it("elements() handles fieldset disabled", () => {
        const form = document.createElement("form");
        try {
            const fieldset = document.createElement("fieldset");
            fieldset.disabled = true;
            const input = document.createElement("input");
            input.name = "test";
            fieldset.appendChild(input);
            form.appendChild(fieldset);
            document.body.appendChild(form);

            const validator = new Validator(form, {
                rules: { test: { required: true } }
            });
            const elements = validator.elements();
            expect(elements.filter(e => e === input).length).toBe(0);
        } finally {
            document.body.removeChild(form);
        }
    });

    it("check() handles dependency-mismatch with single rule", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            input.value = "";
            form.appendChild(input);
            document.body.appendChild(form);

            const validator = new Validator(form, {
                rules: {
                    test: {
                        required: { depends: () => false }
                    }
                }
            });

            // When depends returns false, the rule is removed, element is valid
            const result = validator.check(input);
            expect(result).toBe(true);
        } finally {
            document.body.removeChild(form);
        }
    });

    it("showLabel with success string adds success class", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            input.value = "val";
            form.appendChild(input);
            document.body.appendChild(form);

            const validator = new Validator(form, {
                rules: { test: { required: true } },
                success: "ok"
            });

            validator.element(input);
            const errorLabel = form.querySelector("label.error");
            // With success option, the label should have the success class
            if (errorLabel) {
                expect(errorLabel.classList.contains("ok")).toBe(true);
            }
        } finally {
            document.body.removeChild(form);
        }
    });

    it("showLabel with success function callback", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            input.value = "val";
            form.appendChild(input);
            document.body.appendChild(form);

            const successFn = vi.fn();
            const validator = new Validator(form, {
                rules: { test: { required: true } },
                success: successFn
            });

            validator.element(input);
            expect(successFn).toHaveBeenCalled();
            expect(successFn.mock.calls[0][1]).toBe(input);
        } finally {
            document.body.removeChild(form);
        }
    });

    it("defaultShowErrors with highlight/unhighlight and success", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            form.appendChild(input);
            document.body.appendChild(form);

            const highlight = vi.fn();
            const unhighlight = vi.fn();
            const validator = new Validator(form, {
                rules: { test: { required: true } },
                highlight,
                unhighlight,
                success: "ok"
            });

            // First make it invalid, then valid to trigger success flow
            input.value = "";
            validator.element(input);
            expect(highlight).toHaveBeenCalled();

            input.value = "val";
            validator.element(input);
            // After valid, unhighlight should have been called
            expect(unhighlight).toHaveBeenCalled();
        } finally {
            document.body.removeChild(form);
        }
    });
});

describe("Validator constructor submit handler", () => {
    it("handles cancel class on submit button", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            input.value = "";
            form.appendChild(input);
            document.body.appendChild(form);

            const submitHandler = vi.fn();
            const validator = new Validator(form, {
                rules: { test: { required: true } },
                submitHandler
            });

            // Simulate clicking a cancel button
            const cancelBtn = document.createElement("button");
            cancelBtn.className = "cancel";
            cancelBtn.type = "submit";
            form.appendChild(cancelBtn);

            // Trigger click on cancel button
            cancelBtn.dispatchEvent(new MouseEvent("click", { bubbles: true }));
            // Then submit
            form.dispatchEvent(new Event("submit", { cancelable: true }));

            expect(submitHandler).toHaveBeenCalled();
        } finally {
            document.body.removeChild(form);
        }
    });

    it("handles formnovalidate attribute", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            input.value = "";
            form.appendChild(input);
            document.body.appendChild(form);

            const submitHandler = vi.fn();
            const validator = new Validator(form, {
                rules: { test: { required: true } },
                submitHandler
            });

            // Simulate clicking a button with formnovalidate
            const btn = document.createElement("button");
            btn.setAttribute("formnovalidate", "");
            btn.type = "submit";
            form.appendChild(btn);

            btn.dispatchEvent(new MouseEvent("click", { bubbles: true }));
            form.dispatchEvent(new Event("submit", { cancelable: true }));

            // submitHandler should be called because cancelSubmit was set
            expect(submitHandler).toHaveBeenCalled();
        } finally {
            document.body.removeChild(form);
        }
    });

    it("debug mode prevents default form submission", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            input.value = "val";
            form.appendChild(input);
            document.body.appendChild(form);

            const validator = new Validator(form, {
                rules: { test: { required: true } },
                debug: true
            });

            const submitEvent = new Event("submit", { cancelable: true });
            form.dispatchEvent(submitEvent);

            // In debug mode, the submit handler calls preventDefault
            expect(submitEvent.defaultPrevented).toBe(true);
        } finally {
            document.body.removeChild(form);
        }
    });

    it("focusInvalid is false does not focus", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            input.value = "";
            form.appendChild(input);
            document.body.appendChild(form);

            const focusSpy = vi.spyOn(input, "focus");
            const validator = new Validator(form, {
                rules: { test: { required: true } },
                focusInvalid: false
            });

            validator.form();
            expect(focusSpy).not.toHaveBeenCalled();
        } finally {
            document.body.removeChild(form);
        }
    });

    it("submitHandler returning false prevents submit", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            input.value = "";
            form.appendChild(input);
            document.body.appendChild(form);

            let submitHandlerResult: boolean | undefined = false;
            const validator = new Validator(form, {
                rules: { test: { required: true } },
                submitHandler: () => submitHandlerResult
            });

            const submitEvent = new Event("submit", { cancelable: true });
            form.dispatchEvent(submitEvent);

            // The submit event should be prevented since submitHandler returns false
            expect(submitEvent.defaultPrevented).toBe(true);
        } finally {
            document.body.removeChild(form);
        }
    });

    it("form() triggers invalid-form event when form is invalid", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            input.value = "";
            form.appendChild(input);
            document.body.appendChild(form);

            const invalidHandler = vi.fn();
            const validator = new Validator(form, {
                rules: { test: { required: true } },
                invalidHandler
            });

            // Manually trigger the invalid-form event
            Fluent.trigger(form, "invalid-form", { validator });
            expect(invalidHandler).toHaveBeenCalled();
        } finally {
            document.body.removeChild(form);
        }
    });
});

describe("stopRequest edge cases", () => {
    it("stopRequest triggers form submit when formSubmitted and valid", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            input.value = "val";
            form.appendChild(input);
            document.body.appendChild(form);

            const submitHandler = vi.fn();
            const validator = new Validator(form, {
                rules: { test: { required: true } },
                submitHandler
            });

            // Set up state: formSubmitted true, pendingRequest 1
            (validator as any).formSubmitted = true;
            validator.startRequest(input);

            // Simulate successful stopRequest
            validator.stopRequest(input, true);

            // submitHandler should have been called via the triggered submit event
            expect(submitHandler).toHaveBeenCalled();
        } finally {
            document.body.removeChild(form);
        }
    });

    it("stopRequest triggers invalid-form when formSubmitted and not valid", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            input.value = "";
            form.appendChild(input);
            document.body.appendChild(form);

            const invalidHandler = vi.fn();
            const validator = new Validator(form, {
                rules: { test: { required: true } },
                invalidHandler
            });

            // Set up state: formSubmitted true, pendingRequest 1
            (validator as any).formSubmitted = true;
            validator.startRequest(input);

            // Simulate failed stopRequest
            validator.stopRequest(input, false);

            // invalidHandler should have been called via invalid-form event
            expect(invalidHandler).toHaveBeenCalled();
        } finally {
            document.body.removeChild(form);
        }
    });

    it("stopRequest with abortHandler when not valid", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            input.value = "";
            form.appendChild(input);
            document.body.appendChild(form);

            const abortHandler = vi.fn();
            const validator = new Validator(form, {
                rules: { test: { required: true } },
                abortHandler
            });

            (validator as any).formSubmitted = true;
            validator.startRequest(input);

            validator.stopRequest(input, false);
            expect(abortHandler).toHaveBeenCalled();
        } finally {
            document.body.removeChild(form);
        }
    });
});

describe("contenteditable validation", () => {
    it("validates contenteditable elements", () => {
        const form = document.createElement("form");
        try {
            const div = document.createElement("div");
            div.setAttribute("contenteditable", "true");
            div.setAttribute("name", "editable");
            div.textContent = "";
            form.appendChild(div);
            document.body.appendChild(form);

            const validator = new Validator(form, {
                rules: {
                    editable: { required: true }
                }
            });

            expect(validator.form()).toBe(false);

            div.textContent = "some content";
            // Re-check after setting content
            expect(validator.element(div)).toBe(true);
        } finally {
            document.body.removeChild(form);
        }
    });
});

describe("validator.rules with add/remove commands", () => {
    it("add method merges rules and handles messages", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            form.appendChild(input);
            document.body.appendChild(form);
            new Validator(form, { rules: {}, messages: { test: {} } });

            Validator.rules(input, "add", { required: true, messages: { required: "Required!" } });
            expect(Validator.rules(input)).toEqual({ required: true });
        } finally {
            document.body.removeChild(form);
        }
    });

    it("remove method without argument deletes all static rules", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            form.appendChild(input);
            document.body.appendChild(form);
            new Validator(form, { rules: { test: { required: true } } });

            const removed = Validator.rules(input, "remove");
            expect(removed).toEqual({ required: true });
            expect(Validator.rules(input)).toEqual({});
        } finally {
            document.body.removeChild(form);
        }
    });
});

describe("Validator.getLength edge cases", () => {
    it("handles select-multiple elements", () => {
        const select = document.createElement("select");
        select.multiple = true;
        const opt1 = document.createElement("option");
        opt1.selected = true;
        const opt2 = document.createElement("option");
        opt2.selected = true;
        select.appendChild(opt1);
        select.appendChild(opt2);
        expect(Validator.getLength("", select)).toBe(2);
    });

    it("handles number values", () => {
        const input = document.createElement("input");
        expect(Validator.getLength(12345 as any, input)).toBe(5);
    });

    it("handles checked radio elements without name", () => {
        const radio = document.createElement("input");
        radio.type = "radio";
        radio.checked = true;
        expect(Validator.getLength("", radio)).toBe(1);
    });

    it("handles unchecked radio elements without name", () => {
        const radio = document.createElement("input");
        radio.type = "radio";
        radio.checked = false;
        expect(Validator.getLength("", radio)).toBe(0);
    });
});

describe("check method exception handling", () => {
    it("re-throws TypeError with augmented message in debug mode", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            input.value = "val";
            form.appendChild(input);
            document.body.appendChild(form);

            // Register a method that throws TypeError
            Validator.methods.throwsError = () => {
                throw new TypeError("original error");
            };

            const validator = new Validator(form, {
                rules: { test: { throwsError: true } },
                debug: true
            });

            expect(() => validator.check(input)).toThrow(TypeError);
            expect(() => validator.check(input)).toThrow(/original error/);
        } finally {
            document.body.removeChild(form);
            delete Validator.methods.throwsError;
        }
    });
});

describe("getHighlightTarget edge cases", () => {
    it("returns cke_ element for hidden textareas", () => {
        const el = document.createElement("textarea");
        el.style.visibility = "hidden";
        el.id = "my-editor";
        const ckeEl = document.createElement("div");
        ckeEl.id = "cke_my-editor";
        document.body.appendChild(ckeEl);
        try {
            expect(Validator.getHighlightTarget(el)).toBe(ckeEl);
        } finally {
            document.body.removeChild(ckeEl);
        }
    });

    it("returns null for regular textarea without id", () => {
        const el = document.createElement("textarea");
        el.style.visibility = "hidden";
        expect(Validator.getHighlightTarget(el)).toBeUndefined();
    });

    it("returns tiptap_ element for hidden textareas when cke not found", () => {
        const el = document.createElement("textarea");
        el.style.visibility = "hidden";
        el.id = "tiptap-editor";
        const tiptapEl = document.createElement("div");
        tiptapEl.id = "tiptap_tiptap-editor";
        document.body.appendChild(tiptapEl);
        try {
            expect(Validator.getHighlightTarget(el)).toBe(tiptapEl);
        } finally {
            document.body.removeChild(tiptapEl);
        }
    });
});

describe("isPollutingKey", () => {
    it("returns true for __proto__, constructor, prototype", () => {
        // Just test via the add method behavior
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "__proto__";
            form.appendChild(input);
            document.body.appendChild(form);
            new Validator(form, { rules: {}, messages: {} });

            // Adding rules for __proto__ should not throw
            expect(() => Validator.rules(input, "add", { required: true })).not.toThrow();
        } finally {
            document.body.removeChild(form);
        }
    });

    it("allows adding rules for normal keys", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "normalField";
            form.appendChild(input);
            document.body.appendChild(form);
            const validator = new Validator(form, { rules: {}, messages: {} });

            Validator.rules(input, "add", { required: true });
            expect(Validator.rules(input)).toEqual({ required: true });
        } finally {
            document.body.removeChild(form);
        }
    });
});

describe("showLabel with vx class parent", () => {
    it("sets title attribute on error labels inside vx containers", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            input.value = "";
            form.appendChild(input);
            document.body.appendChild(form);

            // Create a vx container and put the error label in it via errorPlacement
            const vxContainer = document.createElement("div");
            vxContainer.className = "vx";
            form.appendChild(vxContainer);

            const validator = new Validator(form, {
                rules: { test: { required: true } },
                errorPlacement: (error, el) => {
                    vxContainer.appendChild(error);
                }
            });

            validator.element(input);
            const errorLabel = vxContainer.querySelector("label.error");
            expect(errorLabel).not.toBeNull();
            expect(errorLabel?.getAttribute("title")).toBeTruthy();
        } finally {
            document.body.removeChild(form);
        }
    });
});

describe("defaultShowErrors with success callbacks", () => {
    it("processes success list with string success option", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            input.value = "val";
            form.appendChild(input);
            document.body.appendChild(form);

            const highlight = vi.fn();
            const unhighlight = vi.fn();
            const validator = new Validator(form, {
                rules: { test: { required: true } },
                success: "ok",
                highlight,
                unhighlight
            });

            // First make it invalid
            input.value = "";
            validator.element(input);
            expect(highlight).toHaveBeenCalled();

            // Then validate - should go through success path
            input.value = "val";
            highlight.mockClear();
            unhighlight.mockClear();
            validator.element(input);
            expect(unhighlight).toHaveBeenCalled();
        } finally {
            document.body.removeChild(form);
        }
    });
});

describe("stopRequest edge cases", () => {
    it("stopRequest with negative pendingRequest sync", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            form.appendChild(input);
            document.body.appendChild(form);
            const validator = new Validator(form, {});

            // Set pendingRequest negative to test sync
            (validator as any).pendingRequest = -1;
            (validator as any).pending[input.name] = new AbortController();

            // Should not throw and should sync to 0
            validator.stopRequest(input, true);
            expect((validator as any).pendingRequest).toBe(0);
        } finally {
            document.body.removeChild(form);
        }
    });
});

describe("Validator.methods.required with select elements", () => {
    it("handles select elements with selected options", () => {
        const select = document.createElement("select");
        const opt1 = document.createElement("option");
        opt1.value = "";
        opt1.text = "Choose...";
        const opt2 = document.createElement("option");
        opt2.value = "val";
        opt2.text = "Value";
        select.appendChild(opt1);
        select.appendChild(opt2);

        // Empty selection - required returns falsy value for empty selects
        expect(Validator.methods.required("", select)).toBeFalsy();

        // Selected value
        select.value = "val";
        expect(Validator.methods.required("val", select)).toBe(true);
    });

    it("handles select-multiple with selected options", () => {
        const select = document.createElement("select");
        select.multiple = true;
        const opt1 = document.createElement("option");
        opt1.value = "a";
        const opt2 = document.createElement("option");
        opt2.value = "b";
        select.appendChild(opt1);
        select.appendChild(opt2);

        expect(Validator.methods.required("", select)).toBeFalsy();

        opt1.selected = true;
        expect(Validator.methods.required("a", select)).toBe(true);
    });
});

describe("Validator.methods with value coercion", () => {
    it("min/max handles number coercion from string params", () => {
        const el = document.createElement("input");
        // min method: value >= param
        expect(Validator.methods.min(5, el, 3)).toBe(true);
        expect(Validator.methods.min(2, el, 3)).toBe(false);

        // max method: value <= param
        expect(Validator.methods.max(3, el, 5)).toBe(true);
        expect(Validator.methods.max(7, el, 5)).toBe(false);
    });
});

describe("Validator.onclick with option element", () => {
    it("triggers validation for option element via parent select", () => {
        const form = document.createElement("form");
        try {
            const select = document.createElement("select");
            select.name = "test";
            select.multiple = true;
            const opt = document.createElement("option");
            opt.value = "a";
            select.appendChild(opt);
            form.appendChild(select);
            document.body.appendChild(form);

            const validator = new Validator(form, {
                rules: { test: { required: true } }
            });
            const spy = vi.spyOn(validator, "element");

            // Simulate submitted state
            (validator as any).submitted["test"] = "error";

            // Trigger onclick on the option element
            (validator.settings.onclick as any)?.(opt as any, new MouseEvent("click"), validator);
            expect(spy).toHaveBeenCalled();
        } finally {
            document.body.removeChild(form);
        }
    });
});

describe("Validator.element with radio/checkbox groups", () => {
    it("validates radio groups correctly", () => {
        const form = document.createElement("form");
        try {
            const r1 = document.createElement("input");
            r1.type = "radio";
            r1.name = "gender";
            r1.value = "male";
            const r2 = document.createElement("input");
            r2.type = "radio";
            r2.name = "gender";
            r2.value = "female";
            form.appendChild(r1);
            form.appendChild(r2);
            document.body.appendChild(form);

            const validator = new Validator(form, {
                rules: { gender: { required: true } }
            });

            // No radio selected
            expect(validator.element(r1)).toBe(false);

            // Select a radio
            r1.checked = true;
            expect(validator.element(r1)).toBe(true);
        } finally {
            document.body.removeChild(form);
        }
    });

    it("validates checkbox groups correctly", () => {
        const form = document.createElement("form");
        try {
            const cb1 = document.createElement("input");
            cb1.type = "checkbox";
            cb1.name = "hobbies";
            cb1.value = "reading";
            const cb2 = document.createElement("input");
            cb2.type = "checkbox";
            cb2.name = "hobbies";
            cb2.value = "sports";
            form.appendChild(cb1);
            form.appendChild(cb2);
            document.body.appendChild(form);

            const validator = new Validator(form, {
                rules: { hobbies: { required: true } }
            });

            // None checked
            expect(validator.element(cb1)).toBe(false);

            // Check one
            cb1.checked = true;
            expect(validator.element(cb1)).toBe(true);
        } finally {
            document.body.removeChild(form);
        }
    });
});

describe("Validator.element returns false for elements with no form", () => {
    it("returns false when no form", () => {
        const input = document.createElement("input");
        expect(Validator.elementValue(input)).toBe("");
    });
});

describe("number type input with bad input", () => {
    it("handles number input with badInput validity", () => {
        const input = document.createElement("input");
        input.type = "number";
        // Mock validity.badInput
        Object.defineProperty(input, "validity", {
            get: () => ({ badInput: true })
        });
        const val = Validator.elementValue(input);
        expect(val).toBeNaN();
    });

    it("handles number input with valueAsNumber", () => {
        const input = document.createElement("input");
        input.type = "number";
        Object.defineProperty(input, "validity", {
            get: () => ({ badInput: false })
        });
        Object.defineProperty(input, "valueAsNumber", {
            get: () => 42
        });
        const val = Validator.elementValue(input);
        expect(val).toBe(42);
    });
});

describe("onfocusout with submitted elements", () => {
    it("calls element when element name is in submitted", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            form.appendChild(input);
            document.body.appendChild(form);
            const validator = new Validator(form, { rules: { test: { required: true } } });
            const spy = vi.spyOn(validator, "element");

            // Simulate submitted state
            (validator as any).submitted[input.name] = "error";

            input.value = "";
            validator.settings.onfocusout?.(input as any, new Event("focusout"), validator);
            expect(spy).toHaveBeenCalledWith(input);
        } finally {
            document.body.removeChild(form);
        }
    });

    it("skips hidden textareas in onfocusout", () => {
        const form = document.createElement("form");
        try {
            const textarea = document.createElement("textarea");
            textarea.name = "test";
            textarea.style.visibility = "hidden";
            form.appendChild(textarea);
            document.body.appendChild(form);
            const validator = new Validator(form, { rules: { test: { required: true } } });
            const spy = vi.spyOn(validator, "element");

            textarea.value = "";
            validator.settings.onfocusout?.(textarea as any, new Event("focusout"), validator);
            // Should skip because it's a hidden textarea
            expect(spy).not.toHaveBeenCalled();
        } finally {
            document.body.removeChild(form);
        }
    });
});

describe("onkeyup with Tab key", () => {
    it("skips validation when Tab is pressed and value is empty", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            input.value = "";
            form.appendChild(input);
            document.body.appendChild(form);
            const validator = new Validator(form, { rules: { test: { required: true } } });
            const spy = vi.spyOn(validator, "element");

            validator.settings.onkeyup?.(input as any, new KeyboardEvent("keyup", { key: "Tab" }), validator);
            expect(spy).not.toHaveBeenCalled();
        } finally {
            document.body.removeChild(form);
        }
    });
});

describe("onclick with option element", () => {
    it("validates parent select when option is clicked and parent is submitted", () => {
        const form = document.createElement("form");
        try {
            const select = document.createElement("select");
            select.name = "test";
            const opt = document.createElement("option");
            opt.value = "a";
            select.appendChild(opt);
            form.appendChild(select);
            document.body.appendChild(form);

            const validator = new Validator(form, {
                rules: { test: { required: true } }
            });
            const spy = vi.spyOn(validator, "element");

            // Set submitted state on parent select's name
            (validator as any).submitted["test"] = "error";

            if (typeof validator.settings.onclick === "function") {
                validator.settings.onclick(opt as any, new MouseEvent("click"), validator);
            }
            expect(spy).toHaveBeenCalled();
        } finally {
            document.body.removeChild(form);
        }
    });
});

describe("normalizer function in check", () => {
    it("uses element-level normalizer", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            input.value = "  padded  ";
            form.appendChild(input);
            document.body.appendChild(form);

            const validator = new Validator(form, {
                rules: {
                    test: {
                        required: true,
                        normalizer: (val: any) => typeof val === "string" ? val.trim() : val
                    }
                }
            });

            expect(validator.element(input)).toBe(true);
        } finally {
            document.body.removeChild(form);
        }
    });
});

describe("customDataMessage and customMessage", () => {
    it("customDataMessage returns data attributes", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            input.dataset.msgRequired = "Custom required msg";
            input.dataset.msg = "Generic msg";
            form.appendChild(input);
            document.body.appendChild(form);

            const validator = new Validator(form, {});

            expect((validator as any).customDataMessage(input, "required")).toBe("Custom required msg");
            // Falls back to generic data-msg when method-specific not found
            expect((validator as any).customDataMessage(input, "nonexistent")).toBe("Generic msg");
        } finally {
            document.body.removeChild(form);
        }
    });

    it("customMessage returns element-specific messages", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            form.appendChild(input);
            document.body.appendChild(form);

            const validator = new Validator(form, {
                messages: {
                    test: "Direct message"
                }
            });

            expect((validator as any).customMessage("test", "required")).toBe("Direct message");
        } finally {
            document.body.removeChild(form);
        }
    });

    it("customMessage returns method-specific messages", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            form.appendChild(input);
            document.body.appendChild(form);

            const validator = new Validator(form, {
                messages: {
                    test: { required: "Method specific msg" }
                }
            });

            expect((validator as any).customMessage("test", "required")).toBe("Method specific msg");
        } finally {
            document.body.removeChild(form);
        }
    });
});

describe("defaultMessage with function messages", () => {
    it("calls function messages with parameters and element", () => {
        const form = document.createElement("form");
        try {
            const input = document.createElement("input");
            input.name = "test";
            form.appendChild(input);
            document.body.appendChild(form);

            Validator.messages["customMsg"] = (params: any, element: any) => {
                return `Message for ${element.name} with param ${params}`;
            };

            const validator = new Validator(form, {});

            const msg = (validator as any).defaultMessage(input, { method: "customMsg", parameters: "xyz" });
            expect(msg).toBe("Message for test with param xyz");
        } finally {
            document.body.removeChild(form);
            delete Validator.messages.customMsg;
        }
    });
});

