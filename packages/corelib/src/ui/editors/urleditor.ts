import { Fluent, Validator } from "../../base";
import { IStringValue } from "../../interfaces";
import { ValidationHelper } from "../../q";
import { Decorators } from "../../types/decorators";
import { EditorProps } from "../widgets/widget";
import { StringEditor } from "./stringeditor";

@Decorators.registerEditor('Serenity.URLEditor', [IStringValue])
export class URLEditor<P = {}> extends StringEditor<P> {

    constructor(props: EditorProps<P>) {
        super(props);

        this.domNode.classList.add("url");
        this.domNode.setAttribute("title", "URL should be entered in format: 'http://www.site.com/page'.");

        Fluent.on(this.domNode, "blur." + this.uniqueName, e => {
            var validator = ValidationHelper.getValidator(this.domNode);
            if (validator == null)
                return;

            if (!this.domNode.classList.contains("error"))
                return;

            var value = this.domNode.value?.trim();
            if (!value)
                return;

            value = "http://" + value;

            if (Validator.methods.url(value, this.domNode)) {
                this.domNode.value = value;
                validator.element(this.domNode);
            }
        });
    }
}