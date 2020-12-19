import { registerEditor } from "../../Decorators";
import { StringEditor } from "./StringEditor";

@registerEditor('Serenity.EmailAddressEditor')
export class EmailAddressEditor extends StringEditor {
    constructor(input: JQuery) {
        super(input);

        input.attr('type', 'email')
            .addClass('email');
    }
}