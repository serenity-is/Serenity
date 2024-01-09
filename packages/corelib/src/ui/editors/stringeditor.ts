import { Fluent } from "@serenity-is/base";
import { IStringValue } from "../../interfaces";
import { EditorWidget } from "../widgets/widget";

export class StringEditor<P={}> extends EditorWidget<P> {
    static override typeName = this.registerEditor("Serenity.StringEditor", [IStringValue])
    
    declare readonly domNode: HTMLInputElement;

    static override createDefaultElement() { return Fluent("input").attr("type", "text").getNode(); }

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
