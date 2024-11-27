import { ResetPasswordForm } from "../../ServerTypes/Extensions/ResetPasswordForm";
import { ResetPasswordRequest } from "../../ServerTypes/Extensions/ResetPasswordRequest";
import { ExtensionsTexts, ResetPasswordFormTexts } from "../../ServerTypes/Texts";
import { PropertyPanel, TransformInclude, WidgetProps, informationDialog, resolveUrl, serviceCall } from "@serenity-is/corelib";
import { ResetPasswordResponse } from "../../ServerTypes/Extensions/ResetPasswordResponse";
import { AccountPanelTitle } from "../AccountPanelTitle";
import { addPasswordStrengthValidation } from "../PasswordStrength/PasswordStrengthValidation";

export default function pageInit(opt: ResetPasswordOptions) {
    new ResetPasswordPanel({ element: '#PanelDiv', class: 's-full-page justify-content-center s-Form', ...opt });
}

export interface ResetPasswordOptions extends TransformInclude {
    token: string;
    minPasswordLength: number;
}

export class ResetPasswordPanel extends PropertyPanel<ResetPasswordRequest, ResetPasswordOptions> {

    protected getFormKey() { return ResetPasswordForm.formKey; }

    private form = new ResetPasswordForm(this.idPrefix);
    declare private tokenInput: HTMLInputElement;

    constructor(props: WidgetProps<ResetPasswordOptions>) {
        super(props);

        addPasswordStrengthValidation(this.form.NewPassword, this.uniqueName);

        this.form.ConfirmPassword.addValidationRule(this.uniqueName, e => {
            if (this.form.ConfirmPassword.value !== this.form.NewPassword.value)
                return ExtensionsTexts.Validation.PasswordConfirmMismatch;
        });
    }

    submitClick() {
        if (!this.validateForm())
            return;

        var request = this.getSaveEntity();
        request.Token = this.tokenInput.value;
        serviceCall({
            url: resolveUrl('~/Account/ResetPassword'),
            request: request,
            onSuccess: (response: ResetPasswordResponse) => {
                if (response.RedirectHome)
                    window.location.href = resolveUrl('~/')
                else
                    informationDialog(ResetPasswordFormTexts.Success, () => {
                        window.location.href = resolveUrl('~/Account/Login');
                    });
            }
        });
    }

    renderContents(): any {
        const id = this.useIdPrefix();
        return (
            <div class="s-container-tight">
                <AccountPanelTitle />
                <div class="s-Panel p-4">
                    <h5 class="text-center mb-4">{ResetPasswordFormTexts.FormTitle}</h5>
                    <form id={id.Form} action="">
                        <div id={id.PropertyGrid}></div>
                        <button id={id.SubmitButton} type="submit" class="btn btn-primary mx-8 w-100"
                            onClick={e => { e.preventDefault(); this.submitClick(); }}>
                            {ResetPasswordFormTexts.SubmitButton}
                        </button>
                        <input type="hidden" value={this.options.token} ref={el => this.tokenInput = el} />
                    </form>
                </div>
            </div>
        );
    }
}