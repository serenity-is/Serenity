import { nsSerenity } from "../../base";
import { IBooleanValue } from "../../interfaces";
import { EditorWidget } from "./editorwidget";

/**
 * An editor that renders a checkbox for boolean values.
 * @typeParam P - Widget props type.
 */
export class BooleanEditor<P = {}> extends EditorWidget<P> {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity, [IBooleanValue]);

    /** Creates the default checkbox input element.
     * @returns The checkbox input element. */
    static override createDefaultElement() { return <input type="checkbox" /> as HTMLInputElement; }

    /** The checkbox input element that backs the editor. */
    declare public readonly domNode: HTMLInputElement;

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
