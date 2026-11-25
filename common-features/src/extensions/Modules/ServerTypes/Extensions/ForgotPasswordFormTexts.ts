import { proxyTexts } from "@serenity-is/corelib";

namespace texts {
    export declare namespace Forms {
        export function asKey(): typeof Forms;
        export function asTry(): typeof Forms;
        namespace Membership {
            export function asKey(): typeof Membership;
            export function asTry(): typeof Membership;
            namespace ForgotPassword {
                export function asKey(): typeof ForgotPassword;
                export function asTry(): typeof ForgotPassword;
                export const FormInfo: string;
                export const FormTitle: string;
                export const SubmitButton: string;
                export const SuccessMessage: string;
            }
        }
    }
}

export const ForgotPasswordFormTexts: typeof texts.Forms.Membership.ForgotPassword = /*#__PURE__*/ proxyTexts({}, 'Forms.Membership.ForgotPassword.', {}) as any;