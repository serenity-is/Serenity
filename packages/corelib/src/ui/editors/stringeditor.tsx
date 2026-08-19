import { nsSerenity } from "../../base";
import { IStringValue } from "../../interfaces";
import { EditorWidget } from "./editorwidget";

/**
 * An editor that renders a text input for string values.
 * @typeParam P - Widget props type.
 */
export class StringEditor<P = {}> extends EditorWidget<P> {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity, [IStringValue]);

    declare readonly domNode: HTMLInputElement;

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

    /** Sets the string value. */
    public set value(value: string) {
        (this.domNode as HTMLInputElement).value = value ?? '';
    }

    /** Sets the string value. */
    protected set_value(value: string): void {
        this.value = value;
    }

}
