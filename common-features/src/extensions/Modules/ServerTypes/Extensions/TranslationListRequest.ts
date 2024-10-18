import { ListRequest } from "@serenity-is/corelib";

export interface TranslationListRequest extends ListRequest {
    SourceLanguageID?: string;
    TargetLanguageID?: string;
}