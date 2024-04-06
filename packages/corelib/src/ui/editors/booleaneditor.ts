import { Fluent } from "../../base";
import { IBooleanValue } from "../../interfaces";
import { Decorators } from "../../types/decorators";
import { EditorWidget } from "../widgets/widget";

@Decorators.registerEditor('Serenity.BooleanEditor', [IBooleanValue])
export class BooleanEditor<P = {}> extends EditorWidget<P> {

    static override createDefaultElement() { return Fluent("input").attr("type", "checkbox").getNode(); }

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
