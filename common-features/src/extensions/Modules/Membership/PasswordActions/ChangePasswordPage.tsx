import { ChangePasswordFormTexts } from "../../ServerTypes/Texts";
import { PropertyPanel, WidgetProps, informationDialog, localText, resolveUrl, serviceCall } from "@serenity-is/corelib";
import { ChangePasswordForm } from "../../ServerTypes/Extensions/ChangePasswordForm";
import { ChangePasswordRequest } from "../../ServerTypes/Extensions/ChangePasswordRequest";
import { addPasswordStrengthValidation } from "../PasswordStrength/PasswordStrengthValidation";

export default function pageInit() {
    new ChangePasswordPanel({ element: "#PanelDiv", class: 's-container-tight mt-5 s-Form flex-grow-0' });
}

class ChangePasswordPanel<P = {}> extends PropertyPanel<ChangePasswordRequest, P> {

    protected getFormKey() { return ChangePasswordForm.formKey; }

    private form = new ChangePasswordForm(this.idPrefix);

    constructor(props: WidgetProps<P>) {
        super(props);

        addPasswordStrengthValidation(this.form.NewPassword, this.uniqueName);

        this.form.ConfirmPassword.addValidationRule(this.uniqueName, () => {
            if (this.form.ConfirmPassword.value !== this.form.NewPassword.value) {
                return localText("Validation.PasswordConfirmMismatch");
            }
        });
    }

    protected submitClick() {
        if (!this.validateForm())
            return;

        var request = this.getSaveEntity();
        serviceCall({
            url: resolveUrl('~/Account/ChangePassword'),
            request: request,
            onSuccess: () => {
                informationDialog(ChangePasswordFormTexts.Success, () => {
                    window.location.href = resolveUrl('~/');
                })
            }
        })
    }

    renderContents(): any {
        const id = this.useIdPrefix();
        return (
            <div class="s-Panel">
                <h3 class="page-title mb-4 text-center">{ChangePasswordFormTexts.FormTitle}</h3>
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
        );
    }
}

