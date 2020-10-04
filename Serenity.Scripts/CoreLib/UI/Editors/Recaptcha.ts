namespace Serenity {

    export interface RecaptchaOptions {
        siteKey?: string;
        language?: string;
    }

    @Decorators.registerEditor('Serenity.Recaptcha', [IStringValue])
    @Decorators.element("<div/>")
    export class Recaptcha extends Widget<RecaptchaOptions> implements IStringValue {
        constructor(div: JQuery, opt: RecaptchaOptions) {
            super(div, opt);

            this.element.addClass('g-recaptcha').attr('data-sitekey', this.options.siteKey);
            if (!!(window['grecaptcha'] == null && $('script#RecaptchaInclude').length === 0)) {
                var src = 'https://www.google.com/recaptcha/api.js';
                var lng = this.options.language;
                if (lng == null) {
                    lng = Q.coalesce($('html').attr('lang'), '');
                }
                src += '?hl=' + lng;
                $('<script/>').attr('id', 'RecaptchaInclude').attr('src', src).appendTo(document.body);
            }

            var valInput = $('<input />').insertBefore(this.element)
                .attr('id', this.uniqueName + '_validate').val('x');

            var gro = {};
            gro['visibility'] = 'hidden';
            gro['width'] = '0px';
            gro['height'] = '0px';
            gro['padding'] = '0px';

            var input = valInput.css(gro);
            var self = this;

            Q.addValidationRule(input, this.uniqueName, e => {
                if (Q.isEmptyOrNull(this.get_value())) {
                    return Q.text('Validation.Required');
                }
                return null;
            });
        }

        get_value(): string {
            return this.element.find('.g-recaptcha-response').val();
        }

        set_value(value: string): void {
            // ignore
        }
    }
}