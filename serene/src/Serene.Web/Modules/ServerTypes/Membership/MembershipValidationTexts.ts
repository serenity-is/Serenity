import { proxyTexts } from "@serenity-is/corelib";

namespace texts {
    export declare namespace Validation {
        export function asKey(): typeof Validation;
        export function asTry(): typeof Validation;
        export const AuthenticationError: string;
        export const CurrentPasswordMismatch: string;
        export const EmailConfirm: string;
        export const EmailInUse: string;
        export const InvalidActivateToken: string;
        export const InvalidResetToken: string;
        export const MinRequiredPasswordLength: string;
        export const PasswordConfirmMismatch: string;
    }
}

export const MembershipValidationTexts: typeof texts.Validation = /*#__PURE__*/ proxyTexts({}, 'Validation.', {}) as any;