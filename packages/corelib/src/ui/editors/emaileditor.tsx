import { Fluent, FormValidationTexts, ValidatableElement, Validator, nsSerenity, setElementReadOnly } from "../../base";
import { ValidationHelper } from "../../compat";
import { IReadOnly, IStringValue } from "../../interfaces";
import { EditorProps, EditorWidget } from "./editorwidget";

/**
 * Options for the {@link EmailEditor}.
 */
export interface EmailEditorOptions {
    /** Fixed domain appended to the user part. */
    domain?: string;
    /** Whether the domain part is read-only. */
    readOnlyDomain?: boolean;
}

/**
 * An editor that renders user and domain parts of an email address separately.
 * @typeParam P - Widget props type.
 */
export class EmailEditor<P extends EmailEditorOptions = EmailEditorOptions> extends EditorWidget<P> {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity, [IStringValue, IReadOnly]);

    static override createDefaultElement() { return <input type="text" /> as HTMLInputElement; }
    declare readonly domNode: HTMLInputElement;
    declare private readonly domain: HTMLInputElement;

    /**
     * Creates an email editor.
     * @param props - Widget props.
     */
    constructor(props: EditorProps<P>) {
        super(props);
        EmailEditor.registerValidationMethods();

        this.domNode.classList.add('emailuser');

        this.domain = <input type="text" class="emaildomain" /> as HTMLInputElement;
        this.domNode.after(<>
            <span class="emailat">@</span>
            {this.domain}
        </>)

        Fluent.on(this.domain, 'blur.' + this.uniqueName, () => {
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

    /**
     * Registers the custom email validation method.
     */
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
        }, FormValidationTexts.Email);
    }

    /**
     * Returns the full email address.
     * @returns The email value.
     */
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

    /**
     * Returns the full email address.
     * @returns The email value.
     */
    get value(): string {
        return this.get_value();
    }

    /**
     * Sets the email address, splitting it into user and domain parts.
     * @param value - The email value.
     */
    set_value(value: string): void {
        value = value?.trim();
        if (!value) {
            if (!this.options.readOnlyDomain)
                this.domain.value = '';
            this.domNode.value = "";
        }
        else {
            var idx = value.indexOf('@');
            if (idx >= 0) {
                var user = value.substring(0, idx);
                var domain = value.substring(idx + 1);
                if (!this.options.readOnlyDomain) {
                    this.domain.value = domain;
                    this.domNode.value = user;
                }
                else if (this.options.domain) {
                    if (domain !== this.options.domain)
                        this.domNode.value = value;
                    else
                        this.domNode.value = user;
                }
                else
                    this.domNode.value = user;
            }
            else
                this.domNode.value = value;
        }
    }

    /** Sets the email address. */
    set value(v: string) {
        this.set_value(v);
    }

    /**
     * Returns whether the editor is read-only.
     * @returns True when read-only.
     */
    get_readOnly(): boolean {
        return !(this.domNode.getAttribute("readonly") == null &&
            (this.options.readOnlyDomain || this.domain.getAttribute('readonly') == null));
    }

    /**
     * Sets whether the editor is read-only.
     * @param value - True to enable read-only mode.
     */
    set_readOnly(value: boolean): void {
        setElementReadOnly(this.domNode, value);
        if (!this.options.readOnlyDomain) {
            setElementReadOnly(this.domain, value)
        }
    }
}