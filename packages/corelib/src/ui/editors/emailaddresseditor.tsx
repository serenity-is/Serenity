import { Decorators } from "../../types/decorators";
import { EditorProps } from "./editorwidget";
import { StringEditor } from "./stringeditor";

@Decorators.registerEditor('Serenity.EmailAddressEditor')
export class EmailAddressEditor<P = {}> extends StringEditor<P> {

    static override createDefaultElement() { return <input type="email" /> as HTMLInputElement; }

    constructor(props: EditorProps<P>) {
        super(props);
        this.domNode?.classList.add('email');
    }
}