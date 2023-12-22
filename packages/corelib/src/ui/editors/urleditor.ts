import { Decorators } from "../../decorators";
import { IStringValue } from "../../interfaces";
import { WidgetProps } from "../widgets/widget";
import { StringEditor } from "./stringeditor";

@Decorators.registerEditor('Serenity.URLEditor', [IStringValue])
export class URLEditor<P = {}> extends StringEditor<P> {

    constructor(props: WidgetProps<P>) {
        super(props);

        let input = this.element;
        input.addClass("url").attr("title", "URL should be entered in format: 'http://www.site.com/page'.");

        input.on("blur." + this.uniqueName, e => {
            var validator = input.closest("form").data("validator") as JQueryValidation.Validator;
            if (validator == null)
                return;

            if (!input.hasClass("error"))
                return;

            var value = (input.val() as string)?.trim();
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