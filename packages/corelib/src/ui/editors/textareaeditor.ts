import { Decorators } from "../../decorators";
import { IStringValue } from "../../interfaces";
import { EditorComponent, EditorProps } from "../widgets/widget";

export interface TextAreaEditorOptions {
    cols?: number;
    rows?: number;
}

@Decorators.registerEditor('Serenity.TextAreaEditor', [IStringValue])
@Decorators.element("<textarea />")
export class TextAreaEditor<P extends TextAreaEditorOptions = TextAreaEditorOptions> extends EditorComponent<P> {

    constructor(props?: EditorProps<P>) {
        super(props);
        let input = this.element;
        if (this.options.cols !== 0) {
            input.attr('cols', (this.options.cols ?? 80));
        }
        if (this.options.rows !== 0) {
            input.attr('rows', (this.options.rows ?? 6));
        }
    }

    public get value(): string {
        return this.element.val() as string;
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