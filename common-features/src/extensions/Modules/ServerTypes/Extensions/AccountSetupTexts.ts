import { proxyTexts } from "@serenity-is/corelib";

namespace texts {
    export declare namespace Forms {
        export function asKey(): typeof Forms;
        export function asTry(): typeof Forms;
        namespace Membership {
            export function asKey(): typeof Membership;
            export function asTry(): typeof Membership;
            namespace AccountSetup {
                export function asKey(): typeof AccountSetup;
                export function asTry(): typeof AccountSetup;
                export const PasswordChangeRequired: string;
                export const PasswordChangeRequiredHint: string;
                export const PasswordChangeRequiredTitle: string;
            }
        }
    }
}

export const AccountSetupTexts: typeof texts.Forms.Membership.AccountSetup = /*#__PURE__*/ proxyTexts({}, 'Forms.Membership.AccountSetup.', {}) as any;