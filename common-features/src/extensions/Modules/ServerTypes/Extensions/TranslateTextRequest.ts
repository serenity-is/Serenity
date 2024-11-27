import { ServiceRequest } from "@serenity-is/corelib";
import { TranslateTextInput } from "./TranslateTextInput";

export interface TranslateTextRequest extends ServiceRequest {
    SourceLanguageID?: string;
    Inputs?: TranslateTextInput[];
}

