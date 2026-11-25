import { proxyTexts } from "@serenity-is/corelib";

namespace texts {
    export declare namespace Site {
        export function asKey(): typeof Site;
        export function asTry(): typeof Site;
        namespace UserDialog {
            export function asKey(): typeof UserDialog;
            export function asTry(): typeof UserDialog;
            export const EditPermissionsButton: string;
            export const EditRolesButton: string;
        }
    }
}

export const UserDialogTexts: typeof texts.Site.UserDialog = /*#__PURE__*/ proxyTexts({}, 'Site.UserDialog.', {}) as any;