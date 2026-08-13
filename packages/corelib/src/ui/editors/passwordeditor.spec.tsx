import { describe, it, expect } from "vitest";
import { PasswordEditor } from "./passwordeditor";

describe("PasswordEditor", () => {
    it("creates a password input and manages value", () => {
        const editor = new PasswordEditor({});
        expect(editor.domNode.type).toBe("password");
        editor.value = "secret";
        expect(editor.value).toBe("secret");
        expect((editor as any).get_value()).toBe("secret");
        editor.destroy();
    });
});