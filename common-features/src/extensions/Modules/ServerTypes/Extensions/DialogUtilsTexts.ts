import { proxyTexts } from "@serenity-is/corelib";

namespace texts {
    export declare namespace Site {
        export function asKey(): typeof Site;
        export function asTry(): typeof Site;
        namespace Dialogs {
            export function asKey(): typeof Dialogs;
            export function asTry(): typeof Dialogs;
            export const PendingChangesConfirmation: string;
            export const PendingChangesUnloadWarning: string;
        }
    }
}

export const DialogUtilsTexts: typeof texts.Site.Dialogs = /*#__PURE__*/ proxyTexts({}, 'Site.Dialogs.', {}) as any;