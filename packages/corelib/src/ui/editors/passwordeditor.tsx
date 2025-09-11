import { StringEditor } from "./stringeditor";

export class PasswordEditor<TOptions = {}> extends StringEditor<TOptions> {
    static override typeInfo = this.editorType("Serenity.PasswordEditor");

    static override createDefaultElement() { return <input type="password" /> as HTMLInputElement; }
}