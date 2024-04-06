import { Fluent, getjQuery, notifyError } from "../../base";
import { IStringValue } from "../../interfaces";
import { Decorators } from "../../types/decorators";
import { EditorProps, EditorWidget } from "../widgets/widget";

// http://digitalbush.com/projects/masked-input-plugin/
@Decorators.registerEditor('Serenity.MaskedEditor', [IStringValue])
export class MaskedEditor<P extends MaskedEditorOptions = MaskedEditorOptions> extends EditorWidget<P> {

    static override createDefaultElement() { return Fluent("input").attr("type", "text").getNode(); }
    declare readonly domNode: HTMLInputElement;

    constructor(props: EditorProps<P>) {
        super(props);

        let $ = getjQuery();
        if ($?.fn?.mask) {
            $(this.domNode).mask(this.options.mask || '', {
                placeholder: (this.options.placeholder ?? '_')
            });
        }
        else {
            notifyError('MaskedInput requires jQuery masked input plugin ("~/Serenity.Assets/Scripts/jquery.maskedinput.js") to be loaded in the page along with jQuery!');
        }
    }

    public get value(): string {
        Fluent.trigger(this.domNode, "blur.mask");
        return this.domNode.value;
    }

    protected get_value(): string {
        return this.value;
    }

    public set value(value: string) {
        this.domNode.value = value;
    }

    protected set_value(value: string): void {
        this.value = value;
    }
}

export interface MaskedEditorOptions {
    mask?: string;
    placeholder?: string;
}