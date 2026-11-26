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
        export const TooManyRequests: string;
    }
}

export const AccountRateLimitTexts: typeof texts.Validation = /*#__PURE__*/ proxyTexts({}, 'Validation.', {}) as any;