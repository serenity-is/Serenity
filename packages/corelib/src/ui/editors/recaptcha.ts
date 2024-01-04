import sQuery from "@optionaldeps/squery";
import { localText } from "@serenity-is/base";
import { Decorators } from "../../decorators";
import { IStringValue } from "../../interfaces";
import { addValidationRule } from "../../q";
import { EditorWidget, EditorProps } from "../widgets/widget";

export interface RecaptchaOptions {
    siteKey?: string;
    language?: string;
}

@Decorators.registerEditor('Serenity.Recaptcha', [IStringValue])
@Decorators.element("<div/>")
export class Recaptcha<P extends RecaptchaOptions = RecaptchaOptions> extends EditorWidget<P> implements IStringValue {
    constructor(props: EditorProps<P>) {
        super(props);

        sQuery(this.domNode).addClass('g-recaptcha').attr('data-sitekey', this.options.siteKey);
        if (!!((window as any)['grecaptcha'] == null && sQuery('script#RecaptchaInclude').length === 0)) {
            var src = 'https://www.google.com/recaptcha/api.js';
            var lng = this.options.language;
            if (lng == null) {
                lng = sQuery('html').attr('lang') ?? '';
            }
            src += '?hl=' + lng;
            sQuery('<script/>').attr('id', 'RecaptchaInclude').attr('src', src).appendTo(document.body);
        }

        var valInput = sQuery('<input />').insertBefore(this.domNode)
            .attr('id', this.uniqueName + '_validate').val('x');

        var gro: Record<string, string> = {};
        gro['visibility'] = 'hidden';
        gro['width'] = '0px';
        gro['height'] = '0px';
        gro['padding'] = '0px';

        var input = valInput.css(gro);
        var self = this;

        addValidationRule(input, this.uniqueName, e => {
            if (!this.get_value()) {
                return localText('Validation.Required');
            }
            return null;
        });
    }

    get_value(): string {
        return sQuery(this.domNode).find('.g-recaptcha-response').val() as string;
    }

    set_value(value: string): void {
        // ignore
    }
}