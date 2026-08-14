import { describe, expect, it } from "vitest";
import { DateYearEditor } from "./dateyeareditor";

describe("DateYearEditor", () => {
    it("builds a relative year list by default", () => {
        const editor = new DateYearEditor({ element: el => document.body.appendChild(el) } as any);
        const items = editor.getItems();
        const current = new Date().getFullYear();
        expect(items.length).toBe(21);
        expect(items[0]).toBe((current - 10).toString());
        expect(items[items.length - 1]).toBe((current + 10).toString());
        editor.destroy();
    });

    it("supports absolute year bounds", () => {
        const editor = new DateYearEditor({ element: el => document.body.appendChild(el), minYear: "2000", maxYear: "2003" } as any);
        expect(editor.getItems()).toEqual(["2000", "2001", "2002", "2003"]);
        editor.destroy();
    });

    it("supports descending order and explicit items", () => {
        const desc = new DateYearEditor({ element: el => document.body.appendChild(el), minYear: "2000", maxYear: "2002", descending: true } as any);
        expect(desc.getItems()).toEqual(["2002", "2001", "2000"]);
        desc.destroy();

        const custom = new DateYearEditor({ element: el => document.body.appendChild(el), items: ["2020", "2021"] } as any);
        expect(custom.getItems()).toEqual(["2020", "2021"]);
        custom.destroy();
    });
});
