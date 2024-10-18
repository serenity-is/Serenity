import { LanguageDialog } from "./LanguageDialog";
import { LanguageColumns, LanguageRow, LanguageService } from "../";
import { Decorators, EntityGrid } from "@serenity-is/corelib"

@Decorators.registerClass('Serene.Administration.LanguageGrid')
export class LanguageGrid extends EntityGrid<LanguageRow, any> {
    protected useAsync() { return true; }
    protected getColumnsKey() { return LanguageColumns.columnsKey; }
    protected getDialogType() { return LanguageDialog; }
    protected getIdProperty() { return LanguageRow.idProperty; }
    protected getLocalTextPrefix() { return LanguageRow.localTextPrefix; }
    protected getService() { return LanguageService.baseUrl; }

    protected afterInit() {
        super.afterInit();
    }
    protected getDefaultSortBy() {
        return [LanguageRow.Fields.LanguageName];
    }
}