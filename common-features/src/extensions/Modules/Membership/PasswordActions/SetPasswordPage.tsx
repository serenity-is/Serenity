import { BasePanel, getWindowLocation, informationDialog, parseQueryString, resolveUrl, serviceCall } from "@serenity-is/corelib";
import { SendResetPasswordResponse } from "../../ServerTypes/Extensions/SendResetPasswordResponse";
import { SetPasswordFormTexts } from "../../ServerTypes/Extensions/SetPasswordFormTexts";

export default function pageInit() {
    // in the middle of the page, as tall as its content, like the other pages of the account
    new SetPasswordPage({ element: "#PanelDiv", class: "s-Form" }).domNode?.classList.add("d-flex", "flex-grow-1", "align-items-center");
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
                            getWindowLocation().href = resolveUrl(response.DemoLink);
                        });
                }
                else {
                    informationDialog(SetPasswordFormTexts.EmailSentMessage, () => {
                        getWindowLocation().href = resolveUrl('~/');
                    });
                }
            }
        })
    }

    protected override renderContents(): any {
        return (
            <div class="s-container-tight">
                <div class="s-Panel p-4">
                    <h5 class="text-center mb-4">{SetPasswordFormTexts.PageTitle}</h5>
                    {
                        parseQueryString()["reason"] == "elevate" ?
                            <p class="text-center">{SetPasswordFormTexts.ElevatedActionsMessage}</p> :
                            <p class="text-center">{SetPasswordFormTexts.EmailToSetPasswordMessage}</p>
                    }
                    <form>
                        <button class="btn btn-primary w-100" onClick={e => {
                            e.preventDefault();
                            this.submitClick()
                        }}>{SetPasswordFormTexts.SendEmailButton}</button>
                    </form>
                </div>
            </div>
        );
    }
}

