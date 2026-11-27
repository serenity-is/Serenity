import { proxyTexts } from "@serenity-is/corelib";

namespace texts {
    export declare namespace Forms {
        export function asKey(): typeof Forms;
        export function asTry(): typeof Forms;
        namespace Membership {
            export function asKey(): typeof Membership;
            export function asTry(): typeof Membership;
            namespace MyAccount {
                export function asKey(): typeof MyAccount;
                export function asTry(): typeof MyAccount;
                export const ChangePasswordDescription: string;
                export const ChangePasswordRequired: string;
                export const PasswordTitle: string;
                export const SetPasswordDescription: string;
            }
        }
    }
}

export const MyAccountTexts: typeof texts.Forms.Membership.MyAccount = /*#__PURE__*/ proxyTexts({}, 'Forms.Membership.MyAccount.', {}) as any;