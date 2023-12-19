import { Decorators } from "../../decorators";
import { IBooleanValue } from "../../interfaces";
import { EditorComponent } from "../widgets/widget";

@Decorators.registerEditor('Serenity.BooleanEditor', [IBooleanValue])
@Decorators.element('<input type="checkbox"/>')
export class BooleanEditor<P = {}> extends EditorComponent<P> {

    public get value(): boolean {
        return this.element.is(":checked");
    }

    protected get_value(): boolean {
        return this.value;
    }

    public set value(value: boolean) {
        this.element.prop("checked", !!value);
    }

    protected set_value(value: boolean): void {
        this.value = value;
    }
}
