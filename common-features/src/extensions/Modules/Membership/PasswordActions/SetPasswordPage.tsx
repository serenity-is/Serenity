import { SetPasswordFormTexts } from "../../ServerTypes/Texts";
import { BasePanel, informationDialog, parseQueryString, resolveUrl, serviceCall } from "@serenity-is/corelib";
import { SendResetPasswordResponse } from "../../ServerTypes/Extensions/SendResetPasswordResponse";

export default function pageInit() {
    new SetPasswordPage({ element: "#PanelDiv", class: "s-container-tight mt-5 s-Form" });
}

class SetPasswordPage extends BasePanel {

    protected submitClick() {
        if (!this.validateForm())
            return;

        serviceCall({
            url: resolveUrl('~/Account/SendResetPassword'),
            onSuccess: (response: SendResetPasswordResponse) => {
                if (response.DemoLink) {
                    informationDialog("If this wasn't a demo we would send you a reset password email. " +
                        "Since this is a demo we will just redirect you to set your password.", () => {
                            window.location.href = resolveUrl(response.DemoLink);
                        });
                }
                else {
                    informationDialog(SetPasswordFormTexts.EmailSentMessage, () => {
                        window.location.href = resolveUrl('~/');
                    });
                }
            }
        })
    }

    renderContents(): any {
        return (
            <div class="s-Panel">
                <h3 class="page-title mb-4 text-center">{SetPasswordFormTexts.PageTitle}</h3>
                {
                    parseQueryString()["reason"] == "elevate" ?
                        <p>{SetPasswordFormTexts.ElevatedActionsMessage}</p> :
                        <p>{SetPasswordFormTexts.EmailToSetPasswordMessage}</p>
                }
                <form>
                    <button class="btn btn-primary w-100" onClick={e => {
                        e.preventDefault();
                        this.submitClick()
                    }}>{SetPasswordFormTexts.SendEmailButton}</button>
                </form>
            </div>
        );
    }
}

