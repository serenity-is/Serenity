import { ServiceResponse } from "@serenity-is/corelib";
import { TranslateTextOutput } from "./TranslateTextOutput";

export interface TranslateTextResponse extends ServiceResponse {
    Translations?: TranslateTextOutput[];
}