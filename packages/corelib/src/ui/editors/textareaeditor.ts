import { Decorators } from "../../decorators";
import { IStringValue } from "../../interfaces";
import { Widget } from "../widgets/widget";

export interface TextAreaEditorOptions {
    cols?: number;
    rows?: number;
}

@Decorators.registerEditor('Serenity.TextAreaEditor', [IStringValue])
@Decorators.element("<textarea />")
export class TextAreaEditor extends Widget<TextAreaEditorOptions> {

    constructor(input: JQuery, opt?: TextAreaEditorOptions) {
        super(input, opt);

        if (this.options.cols !== 0) {
            input.attr('cols', (this.options.cols ?? 80));
        }
        if (this.options.rows !== 0) {
            input.attr('rows', (this.options.rows ?? 6));
        }
    }

    public get value(): string {
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