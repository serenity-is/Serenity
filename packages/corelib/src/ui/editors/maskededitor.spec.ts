import { afterEach, describe, expect, it, vi } from "vitest";
import * as base from "../../base";
import { mockJQuery, unmockBSAndJQuery } from "../../test/mocks";
import { MaskedEditor } from "./maskededitor";

afterEach(() => {
    unmockBSAndJQuery();
});

describe("MaskedEditor", () => {
    it("uses the jquery mask plugin when available", () => {
        const $ = mockJQuery({ mask: vi.fn().mockImplementation(function () { return this; }) });
        const editor = new MaskedEditor({ element: el => document.body.appendChild(el), mask: "99/99/9999" } as any);
        expect($.fn.mask).toHaveBeenCalledWith("99/99/9999", { placeholder: "_" });
        (editor as any).set_value("abc");
        expect((editor as any).get_value()).toBe("abc");
        editor.destroy();
    });

    it("uses a custom placeholder with the mask plugin", () => {
        const $ = mockJQuery({ mask: vi.fn().mockImplementation(function () { return this; }) });
        const editor = new MaskedEditor({ element: el => document.body.appendChild(el), mask: "999", placeholder: "#" } as any);
        expect($.fn.mask).toHaveBeenCalledWith("999", { placeholder: "#" });
        editor.destroy();
    });

    it("notifies when the mask plugin is missing", () => {
        const notifyError = vi.spyOn(base, "notifyError").mockImplementation(() => { });
        const editor = new MaskedEditor({ element: el => document.body.appendChild(el) } as any);
        expect(notifyError).toHaveBeenCalled();
        editor.value = "value";
        expect(editor.value).toBe("value");
        editor.destroy();
        notifyError.mockRestore();
    });
});
