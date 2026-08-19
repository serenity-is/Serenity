import { Fluent, getjQuery, notifyError, nsSerenity } from "../../base";
import { IStringValue } from "../../interfaces";
import { EditorProps, EditorWidget } from "./editorwidget";

// http://digitalbush.com/projects/masked-input-plugin/
/**
 * An editor that applies a mask to the input using the jQuery masked input plugin.
 * @typeParam P - Widget props type.
 */
export class MaskedEditor<P extends MaskedEditorOptions = MaskedEditorOptions> extends EditorWidget<P> {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity, [IStringValue]);

    static override createDefaultElement() { return <input type="text" /> as HTMLInputElement; }
    declare readonly domNode: HTMLInputElement;

    /**
     * Creates a masked editor.
     * @param props - Widget props.
     */
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

    /**
     * Returns the current masked value.
     * @returns The input value.
     */
    public get value(): string {
        Fluent.trigger(this.domNode, "blur.mask");
        return this.domNode.value;
    }

    /**
     * Returns the current masked value.
     * @returns The input value.
     */
    protected get_value(): string {
        return this.value;
    }

    /** Sets the masked value. */
    public set value(value: string) {
        this.domNode.value = value;
    }

    /** Sets the masked value. */
    protected set_value(value: string): void {
        this.value = value;
    }
}

/**
 * Options for the {@link MaskedEditor}.
 */
export interface MaskedEditorOptions {
    /** The mask pattern to apply. */
    mask?: string;
    /** Placeholder character for empty mask positions. */
    placeholder?: string;
}