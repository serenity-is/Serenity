import { proxyTexts } from "@serenity-is/corelib";

namespace texts {
    export declare namespace Validation {
        export function asKey(): typeof Validation;
        export function asTry(): typeof Validation;
        namespace PermissionGrant {
            export function asKey(): typeof PermissionGrant;
            export function asTry(): typeof PermissionGrant;
            export const NotHeld: string;
            export const NotListed: string;
            export const SuperAdminOnly: string;
            export const UserNotHeld: string;
            export const UserSuperAdmin: string;
        }
    }
}

export const PermissionGrantTexts: typeof texts.Validation.PermissionGrant = /*#__PURE__*/ proxyTexts({}, 'Validation.PermissionGrant.', {}) as any;