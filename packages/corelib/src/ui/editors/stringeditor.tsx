import { nsSerenity } from "../../base";
import { IStringValue } from "../../interfaces";
import { EditorWidget } from "./editorwidget";

/**
 * An editor that renders a text input for string values.
 * @typeParam P - Widget props type.
 */
export class StringEditor<P = {}> extends EditorWidget<P> {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity, [IStringValue]);

    /** The text input element that backs the editor. */
    declare readonly domNode: HTMLInputElement;

    /** Creates the default text input element.
     * @returns The text input element. */
    static override createDefaultElement() { return <input type="text" /> as HTMLInputElement; }

    /**
     * Returns the current string value.
     * @returns The input value.
     */
    public get value(): string {
        return (this.domNode as HTMLInputElement).value;
    }

    /**
     * Returns the current string value.
     * @returns The input value.
     */
    protected get_value(): string {
        return this.value;
    }

    /** Sets the string value.
     * @param value - The string value to set. */
    public set value(value: string) {
        (this.domNode as HTMLInputElement).value = value ?? '';
    }

    /** Sets the string value.
     * @param value - The string value to set. */
    protected set_value(value: string): void {
        this.value = value;
    }

}
