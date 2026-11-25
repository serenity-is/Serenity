import { proxyTexts } from "@serenity-is/corelib";

namespace texts {
    export declare namespace Validation {
        export function asKey(): typeof Validation;
        export function asTry(): typeof Validation;
        export const NorthwindPhone: string;
        export const NorthwindPhoneMultiple: string;
    }
}

export const NorthwindValidationTexts: typeof texts.Validation = /*#__PURE__*/ proxyTexts({}, 'Validation.', {}) as any;