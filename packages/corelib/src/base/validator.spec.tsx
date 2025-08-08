import { Validator } from './validator';

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

