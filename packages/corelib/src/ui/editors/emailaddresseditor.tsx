import { nsSerenity } from "../../base";
import { EditorProps } from "./editorwidget";
import { StringEditor } from "./stringeditor";

/**
 * An editor that renders an email address input.
 * @typeParam P - Widget props type.
 */
export class EmailAddressEditor<P = {}> extends StringEditor<P> {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity);

    static override createDefaultElement() { return <input type="email" /> as HTMLInputElement; }

    /**
     * Creates an email address editor.
     * @param props - Widget props.
     */
    constructor(props: EditorProps<P>) {
        super(props);
        this.domNode?.classList.add('email');
    }
}