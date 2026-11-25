import { proxyTexts } from "@serenity-is/corelib";

namespace texts {
    export declare namespace Forms {
        export function asKey(): typeof Forms;
        export function asTry(): typeof Forms;
        namespace Membership {
            export function asKey(): typeof Membership;
            export function asTry(): typeof Membership;
            namespace ResetPassword {
                export function asKey(): typeof ResetPassword;
                export function asTry(): typeof ResetPassword;
                export const EmailSubject: string;
                export const FormTitle: string;
                export const SubmitButton: string;
                export const Success: string;
            }
        }
    }
}

export const ResetPasswordFormTexts: typeof texts.Forms.Membership.ResetPassword = /*#__PURE__*/ proxyTexts({}, 'Forms.Membership.ResetPassword.', {}) as any;