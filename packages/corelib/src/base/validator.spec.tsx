import { Validator, addValidationRule, removeValidationRule } from './validator';
import { Fluent } from './fluent';
import { Culture } from './formatting';

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
        expect(method("invalid-email", null)).toBe(false);
        expect(method("test@example.com", null)).toBe(true);
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
        errorEl.style.display = "block";
        form.appendChild(errorEl);
        const validator = new Validator(form, {});

        (validator as any).toHide = [errorEl];
        validator.hideErrors();
        expect(errorEl.style.display).toBe("none");
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
        const form = document.createElement("form");
        const div = document.createElement("div");
        div.setAttribute("contenteditable", "true");
        div.setAttribute("name", "content");
        form.appendChild(div);
        document.body.appendChild(form);
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
});

