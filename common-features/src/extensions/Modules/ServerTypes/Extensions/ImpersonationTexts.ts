import { proxyTexts } from "@serenity-is/corelib";

namespace texts {
    export declare namespace Forms {
        export function asKey(): typeof Forms;
        export function asTry(): typeof Forms;
        namespace Membership {
            export function asKey(): typeof Membership;
            export function asTry(): typeof Membership;
            namespace Impersonation {
                export function asKey(): typeof Impersonation;
                export function asTry(): typeof Impersonation;
                export const Restricted: string;
            }
        }
    }
}

export const ImpersonationTexts: typeof texts.Forms.Membership.Impersonation = /*#__PURE__*/ proxyTexts({}, 'Forms.Membership.Impersonation.', {}) as any;