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

});