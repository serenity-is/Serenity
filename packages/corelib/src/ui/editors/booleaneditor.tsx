import { nsSerenity } from "../../base";
import { IBooleanValue } from "../../interfaces";
import { EditorWidget } from "./editorwidget";

/**
 * An editor that renders a checkbox for boolean values.
 * @typeParam P - Widget props type.
 */
export class BooleanEditor<P = {}> extends EditorWidget<P> {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity, [IBooleanValue]);

    static override createDefaultElement() { return <input type="checkbox" /> as HTMLInputElement; }

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

    /** Sets the boolean value. */
    public set value(value: boolean) {
        (this.domNode as HTMLInputElement).checked = !!value;
    }

    /** Sets the boolean value. */
    protected set_value(value: boolean): void {
        this.value = value;
    }
}
