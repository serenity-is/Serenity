import { proxyTexts } from "@serenity-is/corelib";

namespace texts {
    export declare namespace Forms {
        export function asKey(): typeof Forms;
        export function asTry(): typeof Forms;
        namespace Membership {
            export function asKey(): typeof Membership;
            export function asTry(): typeof Membership;
            namespace ChangePassword {
                export function asKey(): typeof ChangePassword;
                export function asTry(): typeof ChangePassword;
                export const FormTitle: string;
                export const SubmitButton: string;
                export const Success: string;
            }
        }
    }
}

export const ChangePasswordFormTexts: typeof texts.Forms.Membership.ChangePassword = /*#__PURE__*/ proxyTexts({}, 'Forms.Membership.ChangePassword.', {}) as any;