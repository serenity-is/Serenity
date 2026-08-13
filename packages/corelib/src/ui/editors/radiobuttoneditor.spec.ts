import { Fluent } from "../../base";
import { RadioButtonEditor } from "./radiobuttoneditor";

describe("RadioButtonEditor", () => {

    enum TestEnum {
        Option1 = 1,
        Option2 = 2,
        Option3 = 3
    }

    it('sets disabled of each radio button', () => {
        const editor = new RadioButtonEditor({
            element: Fluent("input").appendTo(document.body),
            enumType: TestEnum
        });

        try {
            const inputs = editor.element.findAll<HTMLInputElement>('input[type=radio]');
            expect(inputs.length).toBe(3);

            inputs.forEach(input => {
                expect(input.disabled).toBeFalsy();
            });

            editor.set_readOnly(true);
            inputs.forEach(input => {
                expect(input.disabled).toBeTruthy();
            });

            editor.set_readOnly(false);
            inputs.forEach(input => {
                expect(input.disabled).toBeFalsy();
            });
        }
        finally {
            editor.element.remove();
        }
    });

    it("early returns when no enum or lookup is provided", () => {
        const editor = new RadioButtonEditor({ element: Fluent("input").appendTo(document.body) });
        expect(editor.element.findAll("input").length).toBe(0);
        editor.element.remove();
    });

    it("get_value, value and set_value work", () => {
        const editor = new RadioButtonEditor({
            element: Fluent("input").appendTo(document.body),
            enumType: TestEnum
        });
        try {
            editor.set_value("2");
            expect(editor.get_value()).toBe("2");
            expect(editor.value).toBe("2");
            expect(editor.element.findAll<HTMLInputElement>("input:checked").length).toBe(1);
            expect(editor.element.findAll<HTMLInputElement>("input:checked")[0].value).toBe("2");

            // unknown value unchecks everything
            editor.set_value("99");
            expect(editor.element.findAll("input:checked").length).toBe(0);

            // null value unchecks everything
            editor.set_value("2");
            editor.set_value(null as any);
            expect(editor.element.findAll("input:checked").length).toBe(0);
        }
        finally {
            editor.element.remove();
        }
    });

    it("builds radios from a lookup", async () => {
        vi.spyOn(await import("../../compat"), "getLookup").mockReturnValue({
            items: [{ Id: 1, Text: "One" }, { Id: 2, Text: "Two" }],
            textField: "Text",
            idField: "Id"
        } as any);
        const editor = new RadioButtonEditor({
            element: Fluent("input").appendTo(document.body),
            lookupKey: "Test.Lookup"
        });
        expect(editor.element.findAll("input").length).toBe(2);
        editor.element.remove();
        vi.restoreAllMocks();
    });
});