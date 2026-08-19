import { nsSerenity } from "../../base";
import { StringEditor } from "./stringeditor";

/**
 * An editor that renders a password input.
 * @typeParam TOptions - Widget options type.
 */
export class PasswordEditor<TOptions = {}> extends StringEditor<TOptions> {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity);

    static override createDefaultElement() { return <input type="password" /> as HTMLInputElement; }
}