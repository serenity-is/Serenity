import { ColumnsBase, fieldsProxy } from "@serenity-is/corelib";
import { Column } from "@serenity-is/sleekgrid";
import { LanguageRow } from "./LanguageRow";

export interface LanguageColumns {
    LanguageId: Column<LanguageRow>;
    LanguageName: Column<LanguageRow>;
}

export class LanguageColumns extends ColumnsBase<LanguageRow> {
    static readonly columnsKey = 'Administration.Language';
    static readonly Fields = fieldsProxy<LanguageColumns>();
}