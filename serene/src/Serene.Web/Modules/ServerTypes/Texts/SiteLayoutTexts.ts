import { proxyTexts } from "@serenity-is/corelib";

namespace texts {
    export declare namespace Site {
        export function asKey(): typeof Site;
        export function asTry(): typeof Site;
        namespace Layout {
            export function asKey(): typeof Layout;
            export function asTry(): typeof Layout;
            export const Language: string;
            export const Theme: string;
        }
    }
}

export const SiteLayoutTexts: typeof texts.Site.Layout = /*#__PURE__*/ proxyTexts({}, 'Site.Layout.', {}) as any;