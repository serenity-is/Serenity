import { describe, expect, it } from "vitest";
import { Validator } from "../../base";
import { URLEditor } from "./urleditor";

describe("URLEditor", () => {
    it("adds the url class and title", () => {
        const editor = new URLEditor({ element: el => document.body.appendChild(el) } as any);
        expect(editor.domNode.classList.contains("url")).toBe(true);
        expect(editor.domNode.getAttribute("title")).toContain("http://");
        editor.destroy();
    });

    it("prepends http to an invalid url on blur", () => {
        const form = document.createElement("form");
        const editor = new URLEditor({ element: el => form.appendChild(el) } as any);
        document.body.appendChild(form);
        new Validator(form, {});
        editor.domNode.classList.add("error");
        editor.domNode.value = "www.site.com";
        editor.domNode.dispatchEvent(new FocusEvent("blur"));
        expect(editor.domNode.value).toBe("http://www.site.com");
        editor.destroy();
        form.remove();
    });

    it("leaves a valid url unchanged on blur", () => {
        const form = document.createElement("form");
        const editor = new URLEditor({ element: el => form.appendChild(el) } as any);
        document.body.appendChild(form);
        new Validator(form, {});
        editor.domNode.classList.add("error");
        editor.domNode.value = "http://www.site.com";
        editor.domNode.dispatchEvent(new FocusEvent("blur"));
        expect(editor.domNode.value).toBe("http://www.site.com");
        editor.destroy();
        form.remove();
    });

    it("does not touch the value without an error class", () => {
        const form = document.createElement("form");
        const editor = new URLEditor({ element: el => form.appendChild(el) } as any);
        document.body.appendChild(form);
        new Validator(form, {});
        editor.domNode.value = "www.site.com";
        editor.domNode.dispatchEvent(new FocusEvent("blur"));
        expect(editor.domNode.value).toBe("www.site.com");
        editor.destroy();
        form.remove();
    });

    it("leaves an empty value unchanged on blur", () => {
        const form = document.createElement("form");
        const editor = new URLEditor({ element: el => form.appendChild(el) } as any);
        document.body.appendChild(form);
        new Validator(form, {});
        editor.domNode.classList.add("error");
        editor.domNode.value = "";
        editor.domNode.dispatchEvent(new FocusEvent("blur"));
        expect(editor.domNode.value).toBe("");
        editor.destroy();
        form.remove();
    });

    it("returns early when no validator is attached", () => {
        const editor = new URLEditor({ element: el => document.body.appendChild(el) } as any);
        editor.domNode.classList.add("error");
        editor.domNode.value = "www.site.com";
        editor.domNode.dispatchEvent(new FocusEvent("blur"));
        expect(editor.domNode.value).toBe("www.site.com");
        editor.destroy();
    });
});
