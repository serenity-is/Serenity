import { Decorators } from "../../decorators";
import { StringEditor } from "./stringeditor";

@Decorators.registerEditor('Serenity.PasswordEditor')
@Decorators.element("<input type=\"password\"/>")
export class PasswordEditor<TOptions = {}> extends StringEditor<TOptions> {
}