import { proxyTexts } from "@serenity-is/corelib";

namespace texts {
    export declare namespace Site {
        export function asKey(): typeof Site;
        export function asTry(): typeof Site;
        namespace Translation {
            export function asKey(): typeof Translation;
            export function asTry(): typeof Translation;
            export const AllTextsAlreadyTranslated: string;
            export const Assembly: string;
            export const CopyFailMessage: string;
            export const CopySourceTranslations: string;
            export const CopySuccessMessage: string;
            export const CopyTargetTranslations: string;
            export const CustomText: string;
            export const EntityPlural: string;
            export const HasTranslation: string;
            export const Key: string;
            export const OverrideConfirmation: string;
            export const SaveChangesButton: string;
            export const SaveSuccessMessage: string;
            export const SourceLanguage: string;
            export const SourceTargetLanguageSame: string;
            export const SourceText: string;
            export const TargetLanguage: string;
            export const TargetLanguageRequired: string;
            export const TargetText: string;
            export const TranslateAllText: string;
            export const TranslateText: string;
            export const TranslateTextConfirmation: string;
            export const TranslateTextDisabled: string;
            export const UserTranslated: string;
        }
    }
}

export const TranslationTexts: typeof texts.Site.Translation = /*#__PURE__*/ proxyTexts({}, 'Site.Translation.', {}) as any;