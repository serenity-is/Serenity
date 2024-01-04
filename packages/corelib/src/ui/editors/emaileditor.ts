import sQuery from "@optionaldeps/squery";
import { tryGetText } from "@serenity-is/base";
import { Decorators } from "../../decorators";
import { IReadOnly, IStringValue } from "../../interfaces";
import { EditorWidget, EditorProps } from "../widgets/widget";

export interface EmailEditorOptions {
    domain?: string;
    readOnlyDomain?: boolean;
}

@Decorators.registerEditor('Serenity.EmailEditor', [IStringValue, IReadOnly])
@Decorators.element('<input type="text"/>')
export class EmailEditor<P extends EmailEditorOptions = EmailEditorOptions> extends EditorWidget<P> {

    constructor(props: EditorProps<P>) {
        super(props);
        let input = sQuery(this.domNode);
        EmailEditor.registerValidationMethods();

        input.addClass('emailuser');

        var spanAt = sQuery('<span/>').text('@').addClass('emailat').insertAfter(input);

        var domain = sQuery('<input type="text"/>').addClass('emaildomain').insertAfter(spanAt);
        domain.on('blur.' + this.uniqueName, function () {
            var validator = domain.closest('form').data('validator');
            if (validator != null) {
                validator.element(input[0]);
            }
        });

        if (this.options.domain) {
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
            input.change(e2 => this.set_value(input.val() as string));
        }
    }

    static registerValidationMethods(): void {
        if (!sQuery.validator || !sQuery.validator.methods || sQuery.validator.methods['emailuser'] != null)
            return;

        sQuery.validator.addMethod('emailuser', function (value, element) {

            var domain = sQuery(element).nextAll('.emaildomain');
            if (domain.length > 0 && domain.attr('readonly') == null) {

                if (this.optional(element) && this.optional(domain[0])) {
                    return true;
                }

                return sQuery.validator.methods.email.call(this, value + '@' + domain.val(), element);
            }
            else {
                return sQuery.validator.methods.email.call(this, value + '@dummy.com', element);
            }
        }, tryGetText("Validation.Email") ?? sQuery.validator.messages.email);
    }

    get_value(): string {
        var domain = sQuery(this.domNode).nextAll('.emaildomain');
        var value = sQuery(this.domNode).val();
        var domainValue = domain.val();
        if (!value) {
            if (this.options.readOnlyDomain || !domainValue) {
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
        var domain = sQuery(this.domNode).nextAll('.emaildomain');
        value = value?.trim();
        if (!value) {
            if (!this.options.readOnlyDomain)
                domain.val('');
            sQuery(this.domNode).val('');
        }
        else {
            var parts = value.split('@');
            if (parts.length > 1) {
                if (!this.options.readOnlyDomain) {
                    domain.val(parts[1]);
                    sQuery(this.domNode).val(parts[0]);
                }
                else if (this.options.domain) {
                    if (parts[1] !== this.options.domain)
                        sQuery(this.domNode).val(value);
                    else
                        sQuery(this.domNode).val(parts[0]);
                }
                else
                    sQuery(this.domNode).val(parts[0]);
            }
            else
                sQuery(this.domNode).val(parts[0]);
        }
    }

    set value(v: string) {
        this.set_value(v);
    }

    get_readOnly(): boolean {
        var domain = sQuery(this.domNode).nextAll('.emaildomain');
        return !(this.domNode.getAttribute("readonly") == null &&
            (!this.options.readOnlyDomain || domain.attr('readonly') == null));
    }

    set_readOnly(value: boolean): void {
        var domain = sQuery(this.domNode).nextAll('.emaildomain');
        if (value) {
            sQuery(this.domNode).attr('readonly', 'readonly').addClass('readonly');
            if (!this.options.readOnlyDomain) {
                domain.attr('readonly', 'readonly').addClass('readonly');
            }
        }
        else {
            sQuery(this.domNode).removeAttr('readonly').removeClass('readonly');
            if (!this.options.readOnlyDomain) {
                domain.removeAttr('readonly').removeClass('readonly');
            }
        }
    }
}