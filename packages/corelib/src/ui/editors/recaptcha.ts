import { Fluent, FormValidationTexts, addValidationRule, nsSerenity } from "../../base";
import { IStringValue } from "../../interfaces";
import { EditorProps, EditorWidget } from "./editorwidget";

export interface RecaptchaOptions {
    siteKey?: string;
    language?: string;
}

export class Recaptcha<P extends RecaptchaOptions = RecaptchaOptions> extends EditorWidget<P> implements IStringValue {
    static override [Symbol.typeInfo] = this.registerEditor(nsSerenity, [IStringValue]);

    constructor(props: EditorProps<P>) {
        super(props);

        this.domNode.classList.add('g-recaptcha');
        this.domNode.setAttribute('data-sitekey', this.options.siteKey);
        if (!!((window as any)['grecaptcha'] == null && !document.querySelector('script#RecaptchaInclude'))) {
            var src = 'https://www.google.com/recaptcha/api.js';
            var lng = this.options.language;
            if (lng == null) {
                lng = document.documentElement.getAttribute('lang') ?? '';
            }
            src += '?hl=' + lng;
            var script = document.createElement("script");
            script.setAttribute('id', 'RecaptchaInclude');
            script.setAttribute('src', src);
            document.head.append(script);
        }

        var valInput = document.createElement("input");
        Fluent(valInput).insertBefore(this.domNode);
        valInput.setAttribute('id', this.uniqueName + '_validate');
        valInput.value = 'x';

        valInput.style.visibility = 'hidden';
        valInput.style.width = '0px';
        valInput.style.height = '0px';
        valInput.style.padding = '0px';

        addValidationRule(valInput, e => {
            if (!this.get_value()) {
                return FormValidationTexts.Required;
            }
            return null;
        }, this.uniqueName);
    }

    get_value(): string {
        return this.domNode.querySelector<HTMLInputElement>('.g-recaptcha-response').value;
    }

    set_value(value: string): void {
        // ignore
    }
}