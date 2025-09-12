import { nsSerenity } from "../../base";
import { StringEditor } from "./stringeditor";

export class PasswordEditor<TOptions = {}> extends StringEditor<TOptions> {
    static override typeInfo = this.registerEditor(nsSerenity);

    static override createDefaultElement() { return <input type="password" /> as HTMLInputElement; }
}