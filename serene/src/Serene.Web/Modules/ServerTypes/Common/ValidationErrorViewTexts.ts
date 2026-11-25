import { proxyTexts } from "@serenity-is/corelib";

namespace texts {
    export declare namespace Site {
        export function asKey(): typeof Site;
        export function asTry(): typeof Site;
        namespace ValidationError {
            export function asKey(): typeof ValidationError;
            export function asTry(): typeof ValidationError;
            export const Title: string;
        }
    }
}

export const ValidationErrorViewTexts: typeof texts.Site.ValidationError = /*#__PURE__*/ proxyTexts({}, 'Site.ValidationError.', {}) as any;