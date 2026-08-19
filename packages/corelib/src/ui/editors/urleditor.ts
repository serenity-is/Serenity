import { Fluent, nsSerenity, Validator } from "../../base";
import { ValidationHelper } from "../../compat";
import { IStringValue } from "../../interfaces";
import { EditorProps } from "./editorwidget";
import { StringEditor } from "./stringeditor";

/**
 * An editor that renders a URL input and auto-prefixes missing schemes on blur.
 * @typeParam P - Widget props type.
 */
export class URLEditor<P = {}> extends StringEditor<P> {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity, [IStringValue]);

    /**
     * Creates a URL editor.
     * @param props - Widget props.
     */
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