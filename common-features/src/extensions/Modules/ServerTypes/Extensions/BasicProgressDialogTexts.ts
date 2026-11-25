import { proxyTexts } from "@serenity-is/corelib";

namespace texts {
    export declare namespace Site {
        export function asKey(): typeof Site;
        export function asTry(): typeof Site;
        namespace BasicProgressDialog {
            export function asKey(): typeof BasicProgressDialog;
            export function asTry(): typeof BasicProgressDialog;
            export const CancelTitle: string;
            export const PleaseWait: string;
        }
    }
}

export const BasicProgressDialogTexts: typeof texts.Site.BasicProgressDialog = /*#__PURE__*/ proxyTexts({}, 'Site.BasicProgressDialog.', {}) as any;