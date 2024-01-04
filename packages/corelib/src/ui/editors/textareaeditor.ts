import sQuery from "@optionaldeps/squery";
import { Decorators } from "../../decorators";
import { IStringValue } from "../../interfaces";
import { EditorWidget, EditorProps } from "../widgets/widget";

export interface TextAreaEditorOptions {
    cols?: number;
    rows?: number;
}

@Decorators.registerEditor('Serenity.TextAreaEditor', [IStringValue])
@Decorators.element("<textarea />")
export class TextAreaEditor<P extends TextAreaEditorOptions = TextAreaEditorOptions> extends EditorWidget<P> {

    constructor(props: EditorProps<P>) {
        super(props);
        let input = sQuery(this.domNode);
        if (this.options.cols !== 0) {
            input.attr('cols', (this.options.cols ?? 80));
        }
        if (this.options.rows !== 0) {
            input.attr('rows', (this.options.rows ?? 6));
        }
    }

    public get value(): string {
        return sQuery(this.domNode).val() as string;
    }

    protected get_value(): string {
        return this.value;
    }

    public set value(value: string) {
        sQuery(this.domNode).val(value);
    }

    protected set_value(value: string): void {
        this.value = value;
    }
}