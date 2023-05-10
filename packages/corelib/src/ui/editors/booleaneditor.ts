import { Decorators } from "../../decorators";
import { IBooleanValue } from "../../interfaces";
import { Widget } from "../widgets/widget";

@Decorators.registerEditor('Serenity.BooleanEditor', [IBooleanValue])
@Decorators.element('<input type="checkbox"/>')
export class BooleanEditor extends Widget<any> {

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