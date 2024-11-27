import { informationDialog, PropertyPanel, resolveUrl, serviceCall } from "@serenity-is/corelib";
import { SignUpForm, SignUpRequest, SignUpResponse } from "../../../ServerTypes/Membership";
import { MembershipValidationTexts, SignUpFormTexts } from "../../../ServerTypes/Texts";
import { AccountPanelTitle } from "../AccountPanelTitle";

export default function pageInit(opt: any) {
    new SignUpPanel({ element: '#SignUpPanel', ...opt});
}

class SignUpPanel extends PropertyPanel<SignUpRequest, any> {

    protected getFormKey() { return SignUpForm.formKey; }

    private form: SignUpForm;

    constructor(props: { element?: any } & any) {
        super(props);

        this.form = new SignUpForm(this.idPrefix);

        this.form.Email.element.attr("autocomplete", "off");
        this.form.Password.element.attr("autocomplete", "new-password");

        this.form.ConfirmEmail.addValidationRule(this.uniqueName, e => {
            if (this.form.ConfirmEmail.value !== this.form.Email.value) {
                return MembershipValidationTexts.EmailConfirm;
            }
        });

        this.form.ConfirmPassword.addValidationRule(this.uniqueName, e => {
            if (this.form.ConfirmPassword.value !== this.form.Password.value) {
                return MembershipValidationTexts.PasswordConfirmMismatch;
            }
        });
    }

    submitClick() {
        if (!this.validateForm()) {
            return;
        }

        var request = this.propertyGrid.save();
        delete request.ConfirmEmail;
        delete request.ConfirmPassword;

        serviceCall({
            url: resolveUrl('~/Account/SignUp'),
            request: request,
            onSuccess: (response: SignUpResponse) => {
                informationDialog(SignUpFormTexts.Success, () => {
                    window.location.href = resolveUrl('~/');
                });
            }
        });
    }

    renderContents() {
        const id = this.useIdPrefix();
        this.element.empty().append(<>
            <AccountPanelTitle />

            <div class="s-Panel p-4">
                <h5 class="text-center my-4">{SignUpFormTexts.FormTitle}</h5>
                <p id={id.FormInfo} class="text-center">{SignUpFormTexts.FormInfo}</p>

                <form id={id.Form} action="" autoComplete="off">
                    <input autoComplete="false" name="hidden" type="text" style="display:none;" />
                    <div id={id.PropertyGrid}></div>
                    <div class="px-field">
                        <button id={id.SubmitButton} type="submit" class="btn btn-primary my-4 w-100"
                            onClick={e => { e.preventDefault(); this.submitClick(); }}>
                            {SignUpFormTexts.SubmitButton}
                        </button>
                    </div>
                </form>
            </div>
        </>)
    }
}
