import { proxyTexts } from "@serenity-is/corelib";

namespace texts {
    export declare namespace Forms {
        export function asKey(): typeof Forms;
        export function asTry(): typeof Forms;
        namespace Membership {
            export function asKey(): typeof Membership;
            export function asTry(): typeof Membership;
            namespace SetPassword {
                export function asKey(): typeof SetPassword;
                export function asTry(): typeof SetPassword;
                export const ElevatedActionsMessage: string;
                export const EmailSentMessage: string;
                export const EmailToSetPasswordMessage: string;
                export const PageTitle: string;
                export const SendEmailButton: string;
            }
        }
    }
}

export const SetPasswordFormTexts: typeof texts.Forms.Membership.SetPassword = /*#__PURE__*/ proxyTexts({}, 'Forms.Membership.SetPassword.', {}) as any;