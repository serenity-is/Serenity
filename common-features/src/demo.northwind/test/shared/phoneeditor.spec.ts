import { mockDynamicData } from "test-utils";
import { beforeAll, describe, expect, it } from "vitest";
import { PhoneEditor } from "../../Modules/Shared/PhoneEditor";

beforeAll(() => {
    mockDynamicData();
});

describe("PhoneEditor static validation", () => {
    it("isValidPhone rejects empty and short values", () => {
        expect(PhoneEditor.isValidPhone(null)).toBe(false);
        expect(PhoneEditor.isValidPhone("")).toBe(false);
        expect(PhoneEditor.isValidPhone("123")).toBe(false);
        expect(PhoneEditor.isValidPhone("012345678")).toBe(false);
    });

    it("isValidPhone accepts ten digit numbers", () => {
        expect(PhoneEditor.isValidPhone("1234567890")).toBe(true);
        expect(PhoneEditor.isValidPhone("123 456-7890")).toBe(true);
        expect(PhoneEditor.isValidPhone("(123) 456-7890")).toBe(true);
    });

    it("isValidPhone rejects non numeric values", () => {
        expect(PhoneEditor.isValidPhone("abcdefghij")).toBe(false);
        expect(PhoneEditor.isValidPhone("123456789a")).toBe(false);
    });

    it("isValidPhone handles leading zeros and parenthesis", () => {
        expect(PhoneEditor.isValidPhone("(023) 456-7890")).toBe(false);
        expect(PhoneEditor.isValidPhone("(234) 567-890")).toBe(false);
        expect(PhoneEditor.isValidPhone("(234) 567-8901")).toBe(true);
    });

    it("formatPhone formats valid values and returns invalid as-is", () => {
        expect(PhoneEditor.formatPhone("1234567890")).toBe("(123) 456-7890");
        expect(PhoneEditor.formatPhone("(123) 456-7890")).toBe("(123) 456-7890");
        expect(PhoneEditor.formatPhone("123")).toBe("123");
    });

    it("formatMulti splits on semicolons and commas", () => {
        expect(PhoneEditor.formatMulti("1234567890;9876543210", PhoneEditor.formatPhone))
            .toBe("(123) 456-7890, (987) 654-3210");
        expect(PhoneEditor.formatMulti("1234567890, ,9876543210", PhoneEditor.formatPhone))
            .toBe("(123) 456-7890, (987) 654-3210");
        expect(PhoneEditor.formatMulti(null, PhoneEditor.formatPhone)).toBe("");
    });

    it("isValidMulti validates every entry", () => {
        expect(PhoneEditor.isValidMulti("1234567890;9876543210", PhoneEditor.isValidPhone)).toBe(true);
        expect(PhoneEditor.isValidMulti("1234567890;123", PhoneEditor.isValidPhone)).toBe(false);
        expect(PhoneEditor.isValidMulti("", PhoneEditor.isValidPhone)).toBe(false);
        expect(PhoneEditor.isValidMulti(" , ;", PhoneEditor.isValidPhone)).toBe(false);
    });

    it("validate returns null for valid and a message for invalid", () => {
        expect(PhoneEditor.validate("1234567890", false)).toBeNull();
        expect(PhoneEditor.validate("123", false)).toBeTruthy();
        expect(PhoneEditor.validate("123", true)).toBeTruthy();
    });
});

describe("PhoneEditor instance", () => {
    it("formats a single value on change", () => {
        const editor = new PhoneEditor({});
        editor.value = "1234567890";
        expect((editor as any).get_value()).toBe("(123) 456-7890");
        editor.domNode.dispatchEvent(new Event("change"));
        expect((editor.domNode as HTMLInputElement).value).toBe("(123) 456-7890");
        editor.destroy();
    });

    it("formats multiple values", () => {
        const editor = new PhoneEditor({ multiple: true });
        editor.value = "1234567890;9876543210";
        expect((editor as any).get_value()).toBe("(123) 456-7890, (987) 654-3210");
        editor.destroy();
    });

    it("handles empty values", () => {
        const editor = new PhoneEditor({});
        editor.value = "";
        expect((editor as any).get_value()).toBeNull();
        editor.destroy();
    });

    it("formats on blur when validated", () => {
        const editor = new PhoneEditor({});
        editor.value = "1234567890";
        editor.domNode.classList.add("valid");
        editor.domNode.dispatchEvent(new Event("blur"));
        expect((editor.domNode as HTMLInputElement).value).toBe("(123) 456-7890");
        editor.destroy();
    });

    it("exposes a registered editor type", () => {
        expect(PhoneEditor[Symbol.typeInfo]).toBeTruthy();
    });
});
