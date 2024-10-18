import { ListResponse } from "@serenity-is/corelib";
import { TranslationItem } from "./TranslationItem";

export interface TranslationListResponse extends ListResponse<TranslationItem> {
    KeysByAssembly?: { [key: string]: string[] };
}