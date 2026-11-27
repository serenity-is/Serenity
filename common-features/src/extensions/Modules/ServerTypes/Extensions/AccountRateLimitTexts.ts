import { proxyTexts } from "@serenity-is/corelib";

namespace texts {
    export declare namespace Validation {
        export function asKey(): typeof Validation;
        export function asTry(): typeof Validation;
        export const InvalidResetToken: string;
        export const MinRequiredPasswordLength: string;
        export const PasswordConfirmMismatch: string;
        export const PasswordStrengthRequireDigit: string;
        export const PasswordStrengthRequireLowercase: string;
        export const PasswordStrengthRequireNonAlphanumeric: string;
        export const PasswordStrengthRequireUppercase: string;
        namespace PermissionGrant {
            export function asKey(): typeof PermissionGrant;
            export function asTry(): typeof PermissionGrant;
            export const NotHeld: string;
            export const NotListed: string;
            export const SuperAdminOnly: string;
            export const UserNotHeld: string;
            export const UserSuperAdmin: string;
        }
        export const TooManyRequests: string;
    }
}

export const AccountRateLimitTexts: typeof texts.Validation = /*#__PURE__*/ proxyTexts({}, 'Validation.', {
    PermissionGrant: {}
}) as any;