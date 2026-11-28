import { getWindowLocation, informationDialog, PropertyPanel, resolveUrl, serviceCall, WidgetProps } from "@serenity-is/corelib";
import { ChangePasswordForm } from "../../ServerTypes/Extensions/ChangePasswordForm";
import { ChangePasswordFormTexts } from "../../ServerTypes/Extensions/ChangePasswordFormTexts";
import { ChangePasswordRequest } from "../../ServerTypes/Extensions/ChangePasswordRequest";
import { ChangePasswordValidationTexts } from "../../ServerTypes/Extensions/ChangePasswordValidationTexts";
import { AccountSetupTexts } from "../../ServerTypes/Extensions/AccountSetupTexts";
import { addPasswordStrengthValidation } from "../PasswordStrength/PasswordStrengthValidation";

export interface ChangePasswordPageOptions {
    /** Whether the user must change the password before using the application, e.g. one that an administrator set. */
    passwordChangeRequired?: boolean;
}

export default function pageInit(options?: ChangePasswordPageOptions) {
    // in the middle of the page, as tall as its content, like the other pages of the account
    new ChangePasswordPanel({ element: "#PanelDiv", class: "s-Form", ...options }).domNode?.classList.add("d-flex", "flex-grow-1", "align-items-center");
}

class ChangePasswordPanel<P extends ChangePasswordPageOptions = ChangePasswordPageOptions> extends PropertyPanel<ChangePasswordRequest, P> {

    protected override getFormKey() { return ChangePasswordForm.formKey; }

    private form = new ChangePasswordForm(this);

    constructor(props: WidgetProps<P>) {
        super(props);

        addPasswordStrengthValidation(this.form.NewPassword, this.uniqueName);

        this.form.ConfirmPassword.addValidationRule(this.uniqueName, () => {
            if (this.form.ConfirmPassword.value !== this.form.NewPassword.value) {
                return ChangePasswordValidationTexts.PasswordConfirmMismatch;
            }
        });
    }

    protected submitClick() {
        if (!this.validateForm())
            return;

        const request = this.getSaveEntity();
        serviceCall({
            url: resolveUrl('~/Account/ChangePassword'),
            request: request,
            onSuccess: () => {
                informationDialog(ChangePasswordFormTexts.Success, () => {
                    getWindowLocation().href = resolveUrl('~/');
                })
            }
        })
    }

    protected override renderContents(): any {
        const id = this.useIdPrefix();
        return (
            <div class="s-container-tight">
                <div class="s-Panel p-4">
                    <h5 class="text-center mb-4">{ChangePasswordFormTexts.FormTitle}</h5>
                    {this.options.passwordChangeRequired ? <div class="alert alert-warning s-password-change-required" role="alert">
                        <b>{AccountSetupTexts.PasswordChangeRequiredTitle}</b>
                        <p class="mb-0">{AccountSetupTexts.PasswordChangeRequiredHint}</p>
                    </div> : null}
                    <form id={id.Form} action="">
                        <div id={id.PropertyGrid}></div>
                        <div class="px-field mt-4">
                            <button id={id.SubmitButton} type="submit" class="btn btn-primary w-100"
                                onClick={e => { e.preventDefault(); this.submitClick() }}>
                                {ChangePasswordFormTexts.SubmitButton}
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        );
    }
}

