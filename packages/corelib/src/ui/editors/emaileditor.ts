import { Decorators } from "../../decorators";
import { IReadOnly, IStringValue } from "../../interfaces";
import { isEmptyOrNull, trimToNull, tryGetText } from "../../q";
import { Widget } from "../widgets/widget";

export interface EmailEditorOptions {
    domain?: string;
    readOnlyDomain?: boolean;
}

@Decorators.registerEditor('Serenity.EmailEditor', [IStringValue, IReadOnly])
@Decorators.element('<input type="text"/>')
export class EmailEditor extends Widget<EmailEditorOptions> {

    constructor(input: JQuery, opt: EmailEditorOptions) {
        super(input, opt);

        EmailEditor.registerValidationMethods();

        input.addClass('emailuser');

        var spanAt = $('<span/>').text('@').addClass('emailat').insertAfter(input);

        var domain = $('<input type="text"/>').addClass('emaildomain').insertAfter(spanAt);
        domain.bind('blur.' + this.uniqueName, function () {
            var validator = domain.closest('form').data('validator');
            if (validator != null) {
                validator.element(input[0]);
            }
        });

        if (!isEmptyOrNull(this.options.domain)) {
            domain.val(this.options.domain);
        }

        if (this.options.readOnlyDomain) {
            domain.attr('readonly', 'readonly').addClass('disabled').attr('tabindex', '-1');
        }

        input.bind('keypress.' + this.uniqueName, e => {
            if (e.which === 64) {
                e.preventDefault();
                if (!this.options.readOnlyDomain) {
                    domain.focus();
                    domain.select();
                }
            }
        });

        domain.bind('keypress.' + this.uniqueName, function (e1) {
            if (e1.which === 64) {
                e1.preventDefault();
            }
        });

        if (!this.options.readOnlyDomain) {
            input.change(e2 => this.set_value(input.val()));
        }
    }

    static registerValidationMethods(): void {
        if (!$.validator || !$.validator.methods || $.validator.methods['emailuser'] != null)
            return;

        $.validator.addMethod('emailuser', function (value, element) {

            var domain = $(element).nextAll('.emaildomain');
            if (domain.length > 0 && domain.attr('readonly') == null) {

                if (this.optional(element) && this.optional(domain[0])) {
                    return true;
                }

                return $.validator.methods.email.call(this, value + '@' + domain.val(), element);
            }
            else {
                return $.validator.methods.email.call(this, value + '@dummy.com', element);
            }
        }, tryGetText("Validation.Email") ?? $.validator.messages.email);
    }

    get_value(): string {
        var domain = this.element.nextAll('.emaildomain');
        var value = this.element.val();
        var domainValue = domain.val();
        if (isEmptyOrNull(value)) {
            if (this.options.readOnlyDomain || isEmptyOrNull(domainValue)) {
                return '';
            }
            return '@' + domainValue;
        }
        return value + '@' + domainValue;
    }

    get value(): string {
        return this.get_value();
    }

    set_value(value: string): void {
        var domain = this.element.nextAll('.emaildomain');
        value = trimToNull(value);
        if (value == null) {
            if (!this.options.readOnlyDomain)
                domain.val('');
            this.element.val('');
        }
        else {
            var parts = value.split('@');
            if (parts.length > 1) {
                if (!this.options.readOnlyDomain) {
                    domain.val(parts[1]);
                    this.element.val(parts[0]);
                }
                else if (!isEmptyOrNull(this.options.domain)) {
                    if (parts[1] !== this.options.domain)
                        this.element.val(value);
                    else 
                        this.element.val(parts[0]);
                }
                else 
                    this.element.val(parts[0]);
            }
            else
                this.element.val(parts[0]);
        }
    }

    set value(v: string) {
        this.set_value(v);
    }

    get_readOnly(): boolean {
        var domain = this.element.nextAll('.emaildomain');
        return !(this.element.attr('readonly') == null &&
            (!this.options.readOnlyDomain || domain.attr('readonly') == null));
    }

    set_readOnly(value: boolean): void {
        var domain = this.element.nextAll('.emaildomain');
        if (value) {
            this.element.attr('readonly', 'readonly').addClass('readonly');
            if (!this.options.readOnlyDomain) {
                domain.attr('readonly', 'readonly').addClass('readonly');
            }
        }
        else {
            this.element.removeAttr('readonly').removeClass('readonly');
            if (!this.options.readOnlyDomain) {
                domain.removeAttr('readonly').removeClass('readonly');
            }
        }
    }
}