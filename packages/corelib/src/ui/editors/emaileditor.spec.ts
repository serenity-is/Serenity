import { afterEach, describe, expect, it, vi } from "vitest";
import { Validator } from "../../base";
import { EmailEditor } from "./emaileditor";

afterEach(() => {
    document.body.innerHTML = "";
});

function domainOf(editor: EmailEditor<any>): HTMLInputElement {
    return editor.domNode.nextElementSibling?.nextElementSibling as HTMLInputElement;
}

describe("EmailEditor", () => {
    it("builds the user and domain inputs", () => {
        const editor = new EmailEditor({ element: el => document.body.appendChild(el), domain: "test.com" } as any);
        expect(editor.domNode.classList.contains("emailuser")).toBe(true);
        const domain = domainOf(editor);
        expect(domain.classList.contains("emaildomain")).toBe(true);
        expect(domain.value).toBe("test.com");
        editor.destroy();
    });

    it("makes the domain readonly when configured", () => {
        const editor = new EmailEditor({ element: el => document.body.appendChild(el), domain: "test.com", readOnlyDomain: true } as any);
        const domain = domainOf(editor);
        expect(domain.readOnly).toBe(true);
        expect(domain.classList.contains("readonly")).toBe(true);
        expect(domain.classList.contains("disabled")).toBe(true);
        expect(domain.tabIndex).toBe(-1);
        editor.destroy();
    });

    it("reads and writes email values", () => {
        const editor = new EmailEditor({ element: el => document.body.appendChild(el), domain: "test.com" } as any);
        editor.set_value("user@example.com");
        expect(editor.domNode.value).toBe("user");
        expect(domainOf(editor).value).toBe("example.com");
        expect(editor.value).toBe("user@example.com");
        editor.set_value("");
        expect(editor.value).toBe("");
        editor.destroy();
    });

    it("handles readonly domain values", () => {
        const editor = new EmailEditor({ element: el => document.body.appendChild(el), domain: "test.com", readOnlyDomain: true } as any);
        editor.set_value("user@test.com");
        expect(editor.domNode.value).toBe("user");
        expect(editor.value).toBe("user@test.com");
        editor.set_value("other@example.com");
        expect(editor.domNode.value).toBe("other@example.com");
        editor.destroy();
    });

    it("splits the value without a configured domain when readonly", () => {
        const editor = new EmailEditor({ element: el => document.body.appendChild(el), readOnlyDomain: true } as any);
        editor.set_value("user@example.com");
        expect(editor.domNode.value).toBe("user");
        editor.destroy();
    });

    it("registers the emailuser validation method", () => {
        const editor = new EmailEditor({ element: el => document.body.appendChild(el), domain: "test.com" } as any);
        expect(typeof (Validator as any).methods.emailuser).toBe("function");
        editor.destroy();
    });

    it("blocks the @ key on the user input and focuses the domain", () => {
        const editor = new EmailEditor({ element: el => document.body.appendChild(el) } as any);
        const domain = domainOf(editor);
        const focusSpy = vi.spyOn(domain, "focus");
        const event = new KeyboardEvent("keypress", { key: "@", cancelable: true, bubbles: true });
        editor.domNode.dispatchEvent(event);
        expect(event.defaultPrevented).toBe(true);
        expect(focusSpy).toHaveBeenCalled();
        editor.destroy();
    });

    it("blocks the @ key on the domain input", () => {
        const editor = new EmailEditor({ element: el => document.body.appendChild(el) } as any);
        const domain = domainOf(editor);
        const event = new KeyboardEvent("keypress", { key: "@", cancelable: true, bubbles: true });
        domain.dispatchEvent(event);
        expect(event.defaultPrevented).toBe(true);
        editor.destroy();
    });

    it("updates the value from the user input on change", () => {
        const editor = new EmailEditor({ element: el => document.body.appendChild(el) } as any);
        const domain = domainOf(editor);
        domain.value = "test.com";
        editor.domNode.value = "user";
        editor.domNode.dispatchEvent(new Event("change"));
        expect(editor.value).toBe("user@test.com");
        editor.destroy();
    });

    it("returns an @-prefixed domain when only the domain is set", () => {
        const editor = new EmailEditor({ element: el => document.body.appendChild(el) } as any);
        editor.domNode.value = "";
        domainOf(editor).value = "test.com";
        expect(editor.value).toBe("@test.com");
        editor.destroy();
    });

    it("validates emailuser values through the registered method", () => {
        const editor = new EmailEditor({ element: el => document.body.appendChild(el), domain: "test.com" } as any);
        const method = (Validator as any).methods.emailuser;
        expect(method("user", editor.domNode)).toBe(true);
        expect(method("", editor.domNode)).toBe(false);
        editor.domNode.value = "";
        domainOf(editor).value = "";
        expect(method("", editor.domNode)).toBe(true);
        editor.destroy();
    });

    it("validates emailuser values against a fixed domain when readonly", () => {
        const editor = new EmailEditor({ element: el => document.body.appendChild(el), domain: "test.com", readOnlyDomain: true } as any);
        const method = (Validator as any).methods.emailuser;
        expect(method("user", editor.domNode)).toBe(true);
        editor.destroy();
    });

    it("uses the value property setter", () => {
        const editor = new EmailEditor({ element: el => document.body.appendChild(el), domain: "test.com" } as any);
        editor.value = "a@b.com";
        expect(editor.domNode.value).toBe("a");
        editor.destroy();
    });

    it("toggles readonly state", () => {
        const editor = new EmailEditor({ element: el => document.body.appendChild(el) } as any);
        expect(editor.get_readOnly()).toBe(false);
        editor.set_readOnly(true);
        expect(editor.get_readOnly()).toBe(true);
        expect(editor.domNode.hasAttribute("readonly")).toBe(true);
        editor.set_readOnly(false);
        expect(editor.get_readOnly()).toBe(false);
        editor.destroy();
    });

    it("validates the domain on blur", () => {
        const editor = new EmailEditor({ element: el => document.body.appendChild(el) } as any);
        domainOf(editor).dispatchEvent(new FocusEvent("blur"));
        editor.destroy();
    });
});
