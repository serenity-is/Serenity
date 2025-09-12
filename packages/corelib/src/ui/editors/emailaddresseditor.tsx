import { EditorProps } from "./editorwidget";
import { StringEditor } from "./stringeditor";

export class EmailAddressEditor<P = {}> extends StringEditor<P> {
    static override typeInfo = this.registerEditor("Serenity.EmailAddressEditor");

    static override createDefaultElement() { return <input type="email" /> as HTMLInputElement; }

    constructor(props: EditorProps<P>) {
        super(props);
        this.domNode?.classList.add('email');
    }
}