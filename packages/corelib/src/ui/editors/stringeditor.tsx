import { Fluent } from "../../base";
import { IStringValue } from "../../interfaces";
import { Decorators } from "../../types/decorators";
import { EditorWidget } from "./editorwidget";

@Decorators.registerType()
export class StringEditor<P = {}> extends EditorWidget<P> {
    static override typeInfo = Decorators.editorType("Serenity.StringEditor", [IStringValue])

    declare readonly domNode: HTMLInputElement;

    static override createDefaultElement() { return <input type="text" /> as HTMLInputElement; }

    public get value(): string {
        return (this.domNode as HTMLInputElement).value;
    }

    protected get_value(): string {
        return this.value;
    }

    public set value(value: string) {
        (this.domNode as HTMLInputElement).value = value ?? '';
    }

    protected set_value(value: string): void {
        this.value = value;
    }
}
