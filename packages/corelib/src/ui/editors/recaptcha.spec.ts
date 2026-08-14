import { afterEach, describe, expect, it } from "vitest";
import { Validator } from "../../base";
import { Recaptcha } from "./recaptcha";

afterEach(() => {
    delete (window as any).grecaptcha;
    document.querySelector("script#RecaptchaInclude")?.remove();
    document.body.innerHTML = "";
});

describe("Recaptcha", () => {
    it("configures the container and loads the recaptcha script", () => {
        delete (window as any).grecaptcha;
        const editor = new Recaptcha({ element: el => document.body.appendChild(el), siteKey: "KEY", language: "tr" } as any);
        expect(editor.domNode.classList.contains("g-recaptcha")).toBe(true);
        expect(editor.domNode.getAttribute("data-sitekey")).toBe("KEY");
        const script = document.querySelector("script#RecaptchaInclude") as HTMLScriptElement;
        expect(script).toBeTruthy();
        expect(script.getAttribute("src")).toContain("?hl=tr");
        const valInput = editor.domNode.previousElementSibling as HTMLInputElement;
        expect(valInput.id).toBe(editor.uniqueName + "_validate");
        expect(valInput.value).toBe("x");
        editor.destroy();
    });

    it("skips script loading when grecaptcha is present", () => {
        (window as any).grecaptcha = {};
        const editor = new Recaptcha({ element: el => document.body.appendChild(el) } as any);
        expect(document.querySelector("script#RecaptchaInclude")).toBeNull();
        editor.destroy();
    });

    it("reads the response value and ignores set_value", () => {
        const editor = new Recaptcha({ element: el => document.body.appendChild(el) } as any);
        const response = document.createElement("input");
        response.className = "g-recaptcha-response";
        response.value = "TOKEN";
        editor.domNode.appendChild(response);
        expect(editor.get_value()).toBe("TOKEN");
        editor.set_value("IGNORED");
        expect(editor.get_value()).toBe("TOKEN");
        editor.destroy();
    });

    it("validates the response as required", () => {
        const editor = new Recaptcha({ element: el => document.body.appendChild(el) } as any);
        const response = document.createElement("input");
        response.className = "g-recaptcha-response";
        editor.domNode.appendChild(response);
        const valInput = editor.domNode.previousElementSibling as HTMLInputElement;
        const form = document.createElement("form");
        form.appendChild(valInput);
        form.appendChild(editor.domNode);
        document.body.appendChild(form);
        const validator = new Validator(form, {});
        response.value = "";
        expect(validator.element(valInput)).toBe(false);
        response.value = "TOKEN";
        expect(validator.element(valInput)).toBe(true);
        editor.destroy();
        form.remove();
    });
});
