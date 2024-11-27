import { ForgotPasswordFormTexts } from "../../ServerTypes/Texts";
import { PropertyPanel, informationDialog, resolveUrl, serviceCall, toggleClass } from "@serenity-is/corelib";
import { ForgotPasswordForm } from "../../ServerTypes/Extensions/ForgotPasswordForm";
import { ForgotPasswordRequest } from "../../ServerTypes/Extensions/ForgotPasswordRequest";
import { AccountPanelTitle } from "../AccountPanelTitle";

export default function pageInit() {
    toggleClass(new ForgotPasswordPanel({ element: "#PanelDiv" }).domNode, 
        "s-full-page justify-content-center s-Form", true);
}

export class ForgotPasswordPanel<P = {}> extends PropertyPanel<ForgotPasswordRequest, P> {

    protected getFormKey() { return ForgotPasswordForm.formKey; }

    protected submitClick() {
        if (!this.validateForm())
            return;

        var request = this.getSaveEntity();
        serviceCall({
            url: resolveUrl('~/Account/ForgotPassword'),
            request: request,
            onSuccess: () => {
                informationDialog(ForgotPasswordFormTexts.SuccessMessage, () => {
                    window.location.href = resolveUrl('~/');
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
                    <h5 class="text-center mb-4">{ForgotPasswordFormTexts.FormTitle}</h5>
                    <p class="text-center">{ForgotPasswordFormTexts.FormInfo}</p>
                    <form id={id.Form} action="">
                        <div id={id.PropertyGrid}></div>
                        <button id={id.SubmitButton} type="submit" class="btn btn-primary mx-8 w-100"
                            onClick={e => { e.preventDefault(); this.submitClick() }}>
                            {ForgotPasswordFormTexts.SubmitButton}
                        </button>
                    </form>
                </div>
            </div>
        );
    }
}