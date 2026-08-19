import { getElementReadOnly, nsSerenity, setElementReadOnly } from "../../base";
import { Widget, WidgetProps } from "../widgets/widget";

/**
 * Props for editor widgets, extending widget props with editor-specific options.
 * @typeParam T - Widget props type.
 */
export type EditorProps<T> = WidgetProps<T> & {
    /** Initial value for the editor. */
    initialValue?: any;
    /** Maximum input length. */
    maxLength?: number;
    /** Field name. */
    name?: string;
    /** Placeholder text. */
    placeholder?: string;
    /** Whether the field is required. */
    required?: boolean;
    /** Whether the editor is read-only. */
    readOnly?: boolean;
}

/**
 * Base class for editor widgets, providing read-only handling.
 * @typeParam P - Widget props type.
 */
export class EditorWidget<P> extends Widget<EditorProps<P>> {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity);

    /**
     * Creates an editor widget.
     * @param props - Widget props.
     */
    constructor(props: EditorProps<P>) {
        super(props);
    }

    /**
     * Returns whether the editor is read-only.
     * @returns True when read-only.
     */
    get readOnly(): boolean {
        return typeof (this as any)?.get_readOnly === "function" ? !!(this as any).get_readOnly() :
            getElementReadOnly(this.domNode);
    }

    /** Sets whether the editor is read-only. */
    set readOnly(value: boolean) {
        if (typeof (this as any)?.set_readOnly === "function") {
            (this as any).set_readOnly(!!value);
        }
        else if (this.domNode) {
            setElementReadOnly(this.domNode, value);
        }
    }
}
