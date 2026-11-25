import { proxyTexts } from "@serenity-is/corelib";

namespace texts {
    export declare namespace Site {
        export function asKey(): typeof Site;
        export function asTry(): typeof Site;
        namespace RolePermissionDialog {
            export function asKey(): typeof RolePermissionDialog;
            export function asTry(): typeof RolePermissionDialog;
            export const DialogTitle: string;
            export const EditButton: string;
            export const SaveSuccess: string;
        }
    }
}

export const RolePermissionDialogTexts: typeof texts.Site.RolePermissionDialog = /*#__PURE__*/ proxyTexts({}, 'Site.RolePermissionDialog.', {}) as any;