import { Decorators } from "../../decorators";
import { StringEditor } from "./stringeditor";

@Decorators.registerEditor('Serenity.EmailAddressEditor')
@Decorators.element("<input type=\"email\"/>")
export class EmailAddressEditor<TOptions = {}> extends StringEditor<TOptions> {
    constructor(opt?: TOptions) {
        super(opt);
        this.node?.classList.add('email');
    }
}