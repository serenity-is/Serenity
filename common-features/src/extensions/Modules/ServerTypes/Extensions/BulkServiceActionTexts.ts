import { proxyTexts } from "@serenity-is/corelib";

namespace texts {
    export declare namespace Site {
        export function asKey(): typeof Site;
        export function asTry(): typeof Site;
        namespace BulkServiceAction {
            export function asKey(): typeof BulkServiceAction;
            export function asTry(): typeof BulkServiceAction;
            export const AllHadErrorsFormat: string;
            export const AllSuccessFormat: string;
            export const ConfirmationFormat: string;
            export const ErrorCount: string;
            export const NothingToProcess: string;
            export const SomeHadErrorsFormat: string;
            export const SuccessCount: string;
        }
    }
}

export const BulkServiceActionTexts: typeof texts.Site.BulkServiceAction = /*#__PURE__*/ proxyTexts({}, 'Site.BulkServiceAction.', {}) as any;