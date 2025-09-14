import { nsSerenity } from "../../base";
import { IBooleanValue } from "../../interfaces";
import { EditorWidget } from "./editorwidget";

export class BooleanEditor<P = {}> extends EditorWidget<P> {
    static [Symbol.typeInfo] = this.registerEditor(nsSerenity, [IBooleanValue]);

    static override createDefaultElement() { return <input type="checkbox" /> as HTMLInputElement; }

    declare public readonly domNode: HTMLInputElement;

    public get value(): boolean {
        return !!(this.domNode as HTMLInputElement).checked;
    }

    protected get_value(): boolean {
        return this.value;
    }

    public set value(value: boolean) {
        (this.domNode as HTMLInputElement).checked = !!value;
    }

    protected set_value(value: boolean): void {
        this.value = value;
    }
}
