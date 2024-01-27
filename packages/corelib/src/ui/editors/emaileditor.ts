import { Fluent, getjQuery, tryGetText } from "@serenity-is/base";
import { Decorators } from "../../types/decorators";
import { IReadOnly, IStringValue } from "../../interfaces";
import { EditorWidget, EditorProps } from "../widgets/widget";
import { ValidationHelper } from "../../q";

export interface EmailEditorOptions {
    domain?: string;
    readOnlyDomain?: boolean;
}

@Decorators.registerEditor('Serenity.EmailEditor', [IStringValue, IReadOnly])
export class EmailEditor<P extends EmailEditorOptions = EmailEditorOptions> extends EditorWidget<P> {

    static override createDefaultElement() { return Fluent("input").attr("type", "text").getNode(); }   
    declare readonly domNode: HTMLInputElement;
    private readonly domain: HTMLInputElement;

    constructor(props: EditorProps<P>) {
        super(props);
        EmailEditor.registerValidationMethods();

        this.domNode.classList.add('emailuser');

        var spanAt = Fluent("span").text('@').addClass('emailat').insertAfter(this.domNode);

        var domain = Fluent("input").attr("type", "text").addClass('emaildomain').insertAfter(spanAt);
        domain.on('blur.' + this.uniqueName, function () {
            ValidationHelper.validateElement(this.domNode);
        });
        this.domain = domain.getNode();

        if (this.options.domain) {
            domain.val(this.options.domain);
        }

        if (this.options.readOnlyDomain) {
            domain.attr('readonly', 'readonly').addClass('disabled').attr('tabindex', '-1');
        }

        Fluent(this.domNode).on('keypress.' + this.uniqueName, (e: KeyboardEvent) => {
            if (e.key === "@") {
                e.preventDefault();
                if (!this.options.readOnlyDomain) {
                    domain.getNode().focus();
                    domain.getNode().select();
                }
            }
        });

        domain.on('keypress.' + this.uniqueName, function (e: KeyboardEvent) {
            if (e.key === "@")
                e.preventDefault();
        });

        if (!this.options.readOnlyDomain) {
            Fluent.on(this.domNode, "change", e => this.set_value(this.domNode.value));
        }
    }

    static registerValidationMethods(): void {
        let $ = getjQuery();
        if (!$?.validator?.methods || $.validator.methods.emailuser)
            return;

        $.validator.addMethod('emailuser', function (value: string, element: any) {

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
        var value = this.domNode.value;
        var domainValue = this.domain.value;
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
        value = value?.trim();
        if (!value) {
            if (!this.options.readOnlyDomain)
                this.domain.value = '';
            this.domNode.value = "";
        }
        else {
            var parts = value.split('@');
            if (parts.length > 1) {
                if (!this.options.readOnlyDomain) {
                    this.domain.value = parts[1];
                    this.domNode.value = parts[0];
                }
                else if (this.options.domain) {
                    if (parts[1] !== this.options.domain)
                        this.domNode.value = value;
                    else
                        this.domNode.value = parts[0];
                }
                else
                    this.domNode.value = parts[0];
            }
            else
                this.domNode.value = parts[0];
        }
    }

    set value(v: string) {
        this.set_value(v);
    }

    get_readOnly(): boolean {
        return !(this.domNode.getAttribute("readonly") == null &&
            (!this.options.readOnlyDomain || this.domain.getAttribute('readonly') == null));
    }

    set_readOnly(value: boolean): void {
        if (value) {
            this.domNode.setAttribute('readonly', 'readonly');
            this.domNode.classList.add('readonly');
            if (!this.options.readOnlyDomain) {
                this.domain.setAttribute('readonly', 'readonly');
                this.domain.classList.add('readonly');
            }
        }
        else {
            this.domNode.removeAttribute('readonly');
            this.domNode.classList.remove('readonly');
            if (!this.options.readOnlyDomain) {
                this.domain.removeAttribute('readonly');
                this.domain.classList.remove('readonly');
            }
        }
    }
}