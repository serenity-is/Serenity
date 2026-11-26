import { nsSerenity, registerEnum } from "../../base";
import { IBooleanValue } from "../../interfaces";
import { EditorProps, EditorWidget } from "./editorwidget";

/**
 * Where the caption of a checkbox is, when it is rendered in a form field.
 */
export enum CheckLabelPosition {
    /** The default: the caption is at the start (left in LTR) in the caption area, and the checkbox is after it, where the other editors start. */
    Start = 0,
    /** The checkbox is first and the caption is after it, on the end side (right in LTR). It saves horizontal space, and lets several checkboxes share a row. */
    End = 1,
    /** Like `End`, but the caption area before the checkbox is left empty, so the checkbox starts where the other editors do. For a checkbox alone in its row. */
    EndAligned = 2
}

registerEnum(CheckLabelPosition, "Serenity.ComponentModel.CheckLabelPosition");

/**
 * Options for the {@link BooleanEditor}.
 */
export interface BooleanEditorOptions {
    /**
     * Where the field caption is, when the editor is rendered in a form field. At the start in the caption area by default.
     * With `End`, the checkbox is placed first and the caption is shown after it, on the end side (right in LTR), which
     * saves horizontal space and lets several checkboxes share a row. `EndAligned` leaves the caption area empty, so the
     * checkbox lines up with the other editors, for one that is alone in its row.
     */
    labelPosition?: CheckLabelPosition;

    /**
     * Shows a switch instead of a checkbox, e.g. for a setting that turns something on or off. The value is the same.
     */
    switch?: boolean;
}

/**
 * An editor that renders a checkbox, or a switch with the `switch` option, for boolean values.
 * @typeParam P - Widget props type.
 */
export class BooleanEditor<P extends BooleanEditorOptions = BooleanEditorOptions> extends EditorWidget<P> {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity, [IBooleanValue]);

    /** Creates the default checkbox input element.
     * @returns The checkbox input element. */
    static override createDefaultElement() { return <input type="checkbox" /> as HTMLInputElement; }

    /** The checkbox input element that backs the editor. */
    declare public readonly domNode: HTMLInputElement;

    /**
     * Creates a boolean editor.
     * @param props - Widget props.
     */
    constructor(props: EditorProps<P>) {
        super(props);
        // the form field layout is handled in CSS via .field:has(> .label-on-end)
        // an enum editor saves its value as a string
        const position = Number(this.options?.labelPosition ?? CheckLabelPosition.Start);
        if (position === CheckLabelPosition.End || position === CheckLabelPosition.EndAligned)
            this.domNode.classList.add("label-on-end");
        if (position === CheckLabelPosition.EndAligned)
            this.domNode.classList.add("align-with-editors");
        if (this.options?.switch) {
            this.domNode.classList.add("form-check-input", "s-switch");
            this.domNode.setAttribute("role", "switch");
        }
    }

    /**
     * Returns the current boolean value.
     * @returns True when the checkbox is checked.
     */
    public get value(): boolean {
        return !!(this.domNode as HTMLInputElement).checked;
    }

    /**
     * Returns the current boolean value.
     * @returns True when the checkbox is checked.
     */
    protected get_value(): boolean {
        return this.value;
    }

    /** Sets the boolean value.
     * @param value - The boolean value to set. */
    public set value(value: boolean) {
        (this.domNode as HTMLInputElement).checked = !!value;
    }

    /** Sets the boolean value.
     * @param value - The boolean value to set. */
    protected set_value(value: boolean): void {
        this.value = value;
    }
}
