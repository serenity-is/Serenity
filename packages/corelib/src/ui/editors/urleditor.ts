import { Decorators } from "../../decorators";
import { IStringValue } from "../../interfaces";
import { trimToNull } from "@serenity-is/corelib/q";
import { StringEditor } from "./stringeditor";

@Decorators.registerEditor('Serenity.URLEditor', [IStringValue])
export class URLEditor extends StringEditor {

    constructor(input: JQuery) {
        super(input);

        input.addClass("url").attr("title", "URL should be entered in format: 'http://www.site.com/page'.");

        input.on("blur." + this.uniqueName, e => {
            var validator = input.closest("form").data("validator") as JQueryValidation.Validator;
            if (validator == null)
                return;

            if (!input.hasClass("error"))
                return;

            var value = trimToNull(input.val());
            if (!value)
                return;

            value = "http://" + value;

            if ($.validator.methods['url'].call(validator, value, input[0]) == true) {
                input.val(value);
                validator.element(input);
            }
        });
    }
}