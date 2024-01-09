import { Fluent } from "@serenity-is/base";
import { Decorators } from "../../decorators";
import { StringEditor } from "./stringeditor";

@Decorators.registerEditor('Serenity.PasswordEditor')
export class PasswordEditor<TOptions = {}> extends StringEditor<TOptions> {
    static override createDefaultElement() { return Fluent("input").attr("type", "password").getNode(); }
}