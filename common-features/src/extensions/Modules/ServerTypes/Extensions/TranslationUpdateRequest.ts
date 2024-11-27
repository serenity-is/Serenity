import { ServiceRequest } from "@serenity-is/corelib";

export interface TranslationUpdateRequest extends ServiceRequest {
    TargetLanguageID?: string;
    Translations?: { [key: string]: string };
}