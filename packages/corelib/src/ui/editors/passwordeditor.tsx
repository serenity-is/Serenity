import { Decorators } from "../../types/decorators";
import { StringEditor } from "./stringeditor";

@Decorators.registerType()
export class PasswordEditor<TOptions = {}> extends StringEditor<TOptions> {
    static override typeInfo = Decorators.editorType("Serenity.PasswordEditor")

    static override createDefaultElement() { return <input type="password" /> as HTMLInputElement; }
}