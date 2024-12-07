import { Fluent, ValidatableElement, Validator, setElementReadOnly, tryGetText } from "../../base";
import { IReadOnly, IStringValue } from "../../interfaces";
import { ValidationHelper } from "../../compat";
import { Decorators } from "../../types/decorators";
import { EditorProps, EditorWidget } from "./editorwidget";

export interface EmailEditorOptions {
    domain?: string;
    readOnlyDomain?: boolean;
}

@Decorators.registerEditor('Serenity.EmailEditor', [IStringValue, IReadOnly])
export class EmailEditor<P extends EmailEditorOptions = EmailEditorOptions> extends EditorWidget<P> {

    static override createDefaultElement() { return <input type="text" /> as HTMLInputElement; }
    declare readonly domNode: HTMLInputElement;
    declare private readonly domain: HTMLInputElement;

    constructor(props: EditorProps<P>) {
        super(props);
        EmailEditor.registerValidationMethods();

        this.domNode.classList.add('emailuser');

        this.domain = <input type="text" class="emaildomain" /> as HTMLInputElement;
        this.domNode.after(<>
            <span class="emailat">@</span>
            {this.domain}
        </>)

        Fluent.on(this.domain, 'blur.' + this.uniqueName, function () {
            ValidationHelper.validateElement(this.domNode);
        });

        if (this.options.domain) {
            this.domain.value = this.options.domain;
        }

        if (this.options.readOnlyDomain) {
            this.domain.readOnly = true;
            this.domain.classList.add("readonly", "disabled");
            this.domain.tabIndex = -1;
        }

        this.element.on('keypress.' + this.uniqueName, (e: KeyboardEvent) => {
            if (e.key === "@") {
                e.preventDefault();
                if (!this.options.readOnlyDomain) {
                    this.domain.focus();
                    this.domain.select();
                }
            }
        });

        Fluent.on(this.domain, 'keypress.' + this.uniqueName, function (e: KeyboardEvent) {
            if (e.key === "@")
                e.preventDefault();
        });

        if (!this.options.readOnlyDomain) {
            Fluent.on(this.domNode, "change", e => this.set_value(this.domNode.value));
        }
    }

    static registerValidationMethods(): void {

        Validator.addMethod('emailuser', function (value, element) {

            var domain = Fluent(element).nextSibling(".emaildomain").getNode();
            if (domain && domain.getAttribute('readonly') == null) {

                if (Validator.optional(element) && Validator.optional(domain as ValidatableElement)) {
                    return true;
                }

                return Validator.methods.email(value + '@' + (domain as any).value, element);
            }
            else {
                return Validator.methods.email(value + '@dummy.com', element);
            }
        }, tryGetText("Validation.Email"));
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
        setElementReadOnly(this.domNode, value);
        if (!this.options.readOnlyDomain) {
            setElementReadOnly(this.domain, value)
        }
    }
}