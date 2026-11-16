import { afterEach, describe, expect, it } from "vitest";
import { Fluent, Validator, type PropertyItem } from "../../base";
import { DecimalEditor } from "../editors/decimaleditor";
import { EditorUtils } from "../editors/editorutils";
import { StringEditor } from "../editors/stringeditor";
import { PropertyGrid } from "./propertygrid";

const forms: HTMLFormElement[] = [];

afterEach(() => {
    forms.splice(0).forEach(x => x.remove());
});

function createForm(items: PropertyItem[], value?: any) {
    const form = document.createElement("form");
    document.body.append(form);
    forms.push(form);
    const grid = new PropertyGrid({ items, value }).init();
    form.append(grid.domNode);
    const validator = new Validator(form, {});
    const input = (name: string) => grid.domNode.querySelector<HTMLInputElement>(`.field[data-itemname="${name}"] .editor`);
    const validate = (name: string) => {
        const valid = validator.element(input(name));
        return valid ? null : (validator as any).errorMap[input(name).name] as string;
    };
    return { grid, input, validate };
}

function setValue(grid: PropertyGrid, name: string, value: any) {
    const editor = grid.get_editors()[grid.get_items().findIndex(x => x.name === name)];
    (editor as any).value = value;
    Fluent.trigger(editor.domNode, "change");
}

describe("PropertyGrid validation rules", () => {
    it("checks the value of an editor by its rules, with their messages", () => {
        const { grid, validate } = createForm([
            { name: "Quantity", editorType: DecimalEditor, validationRules: [{ rule: "Range", params: [1, 10] }] }
        ]);

        setValue(grid, "Quantity", 5);
        expect(validate("Quantity")).toBeNull();
        setValue(grid, "Quantity", 11);
        expect(validate("Quantity")).toBe("Please enter a value between 1 and 10.");
    });

    it("uses the message of the rule when it has one, and skips an empty value", () => {
        const { grid, validate } = createForm([
            { name: "Code", editorType: StringEditor, validationRules: [{ rule: "Pattern", params: "[A-Z]+", message: "Capitals only" }] }
        ]);

        setValue(grid, "Code", "abc");
        expect(validate("Code")).toBe("Capitals only");
        setValue(grid, "Code", "");
        expect(validate("Code")).toBeNull();
    });

    it("compares with the value of another field of the grid", () => {
        const { grid, validate } = createForm([
            { name: "Password", editorType: StringEditor },
            { name: "Confirm", editorType: StringEditor, validationRules: [{ rule: "Compare", params: "Password" }] }
        ]);

        setValue(grid, "Password", "secret");
        setValue(grid, "Confirm", "other");
        expect(validate("Confirm")).toBe("Please enter the same value again.");
        setValue(grid, "Confirm", "secret");
        expect(validate("Confirm")).toBeNull();
    });

    it("doesn't check a read only field, as its value can't be changed", () => {
        const { grid, validate } = createForm([
            { name: "Quantity", editorType: DecimalEditor, readOnly: true, validationRules: [{ rule: "Range", params: [1, 10] }] }
        ]);

        const editor = grid.get_editors()[0];
        (editor as any).value = 99;
        expect(EditorUtils.getValue(editor)).toBe(99);
        expect(validate("Quantity")).toBeNull();
    });

    it("doesn't check a field hidden by a condition", () => {
        const { grid, input } = createForm([
            { name: "Action", editorType: StringEditor },
            {
                name: "Url", editorType: StringEditor, visibleWhen: [{ field: "Action", values: ["link"] }],
                validationRules: [{ rule: "Url" }]
            }
        ]);

        setValue(grid, "Url", "not a url");
        const form = input("Url").form;
        // the validator leaves out what is hidden, as it does for the required fields
        expect(Validator.getInstance(form).form()).toBe(true);
        setValue(grid, "Action", "link");
        expect(Validator.getInstance(form).form()).toBe(false);
    });
});
