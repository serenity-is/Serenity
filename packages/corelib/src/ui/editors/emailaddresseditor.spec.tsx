import { describe, it, expect } from "vitest";
import { EmailAddressEditor } from "./emailaddresseditor";

describe("EmailAddressEditor", () => {
    it("creates an email input with email class and manages value", () => {
        const editor = new EmailAddressEditor({});
        expect(editor.domNode.type).toBe("email");
        expect(editor.domNode.classList.contains("email")).toBe(true);
        editor.value = "a@b.com";
        expect(editor.value).toBe("a@b.com");
        editor.destroy();
    });
});