import { nsSerenity } from "../../base";
import { IBooleanValue } from "../../interfaces";
import { EditorProps, EditorWidget } from "./editorwidget";

/**
 * Options for the {@link BooleanEditor}.
 */
export interface BooleanEditorOptions {
    /**
     * When true, and the editor is rendered in a form field, the checkbox is
     * placed first and the field caption is shown after it, on the end side (right in LTR) instead of the
     * default caption-left layout. This saves horizontal space and lets
     * several checkboxes share a row.
     */
    labelOnEnd?: boolean;
}

/**
 * An editor that renders a checkbox for boolean values.
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
        if (this.options?.labelOnEnd)
            this.domNode.classList.add("label-on-end");
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
