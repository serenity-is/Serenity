import { Decorators } from "../../decorators";
import { IStringValue } from "../../interfaces";
import { Widget } from "../widgets/widget";

// http://digitalbush.com/projects/masked-input-plugin/
@Decorators.registerEditor('Serenity.MaskedEditor', [IStringValue])
@Decorators.element("<input type=\"text\"/>")
export class MaskedEditor extends Widget<MaskedEditorOptions> {

    constructor(input: JQuery, opt?: MaskedEditorOptions) {
        super(input, opt);

        (input as any).mask(this.options.mask || '', {
            placeholder: (this.options.placeholder ?? '_')
        });
    }

    public get value(): string {
        this.element.triggerHandler("blur.mask");
        return this.element.val();
    }

    protected get_value(): string {
        return this.value;
    }

    public set value(value: string) {
        this.element.val(value);
    }

    protected set_value(value: string): void {
        this.value = value;
    }
}

export interface MaskedEditorOptions {
    mask?: string;
    placeholder?: string;
}