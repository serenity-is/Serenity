import { nsSerenity } from "../../base";
import { IStringValue } from "../../interfaces";
import { EditorProps, EditorWidget } from "./editorwidget";

/**
 * Options for the {@link TextAreaEditor}.
 */
export interface TextAreaEditorOptions {
    /** Number of columns; 0 disables the attribute. */
    cols?: number;
    /** Number of rows; 0 disables the attribute. */
    rows?: number;
}

/**
 * An editor that renders a textarea for multi-line string values.
 * @typeParam P - Widget props type.
 */
export class TextAreaEditor<P extends TextAreaEditorOptions = TextAreaEditorOptions> extends EditorWidget<P> {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity, [IStringValue]);

    /** Creates the default textarea element.
     * @returns The textarea element. */
    static override createDefaultElement() { return document.createElement("textarea"); }

    /**
     * Creates a textarea editor.
     * @param props - Widget props.
     */
    constructor(props: EditorProps<P>) {
        super(props);
        let input = this.element;
        if (this.options.cols !== 0) {
            input.attr('cols', this.options.cols ?? 80);
        }
        if (this.options.rows !== 0) {
            input.attr('rows', this.options.rows ?? 6);
        }
    }

    /**
     * Returns the current textarea value.
     * @returns The value.
     */
    public get value(): string {
        return this.element.val() as string;
    }

    /**
     * Returns the current textarea value.
     * @returns The value.
     */
    protected get_value(): string {
        return this.value;
    }

    /** Sets the textarea value.
     * @param value - The textarea value to set. */
    public set value(value: string) {
        this.element.val(value ?? "");
    }

    /** Sets the textarea value.
     * @param value - The textarea value to set. */
    protected set_value(value: string): void {
        this.value = value;
    }
}