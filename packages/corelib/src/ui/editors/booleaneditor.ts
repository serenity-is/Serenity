import { Decorators } from "../../decorators";
import { IBooleanValue } from "../../interfaces";
import { EditorWidget } from "../widgets/widget";

@Decorators.registerEditor('Serenity.BooleanEditor', [IBooleanValue])
@Decorators.element('<input type="checkbox"/>')
export class BooleanEditor<P = {}> extends EditorWidget<P> {

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
