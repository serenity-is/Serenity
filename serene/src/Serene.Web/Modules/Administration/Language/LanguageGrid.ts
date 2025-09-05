import { Decorators, EntityGrid } from "@serenity-is/corelib";
import { LanguageColumns, LanguageRow, LanguageService } from "../../ServerTypes/Administration";
import { LanguageDialog } from "./LanguageDialog";

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