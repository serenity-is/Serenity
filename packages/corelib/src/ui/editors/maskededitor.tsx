import { Fluent, getjQuery, notifyError, nsSerenity } from "../../base";
import { IStringValue } from "../../interfaces";
import { EditorProps, EditorWidget } from "./editorwidget";

// http://digitalbush.com/projects/masked-input-plugin/
export class MaskedEditor<P extends MaskedEditorOptions = MaskedEditorOptions> extends EditorWidget<P> {
    static override [Symbol.typeInfo] = this.registerEditor(nsSerenity, [IStringValue]);

    static override createDefaultElement() { return <input type="text" /> as HTMLInputElement; }
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
            notifyError('MaskedInput requires jQuery masked input plugin (jquery.maskedinput.js, https://www.npmjs.com/package/jquery.maskedinput) to be loaded in the page along with jQuery!');
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