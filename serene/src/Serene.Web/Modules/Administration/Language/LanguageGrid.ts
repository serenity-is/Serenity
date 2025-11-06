import { EntityGrid } from "@serenity-is/corelib";
import { LanguageColumns, LanguageRow, LanguageService } from "../../ServerTypes/Administration";
import { nsAdministration } from "../../ServerTypes/Namespaces";
import { LanguageDialog } from "./LanguageDialog";

export class LanguageGrid extends EntityGrid<LanguageRow, any> {
    static override[Symbol.typeInfo] = this.registerClass(nsAdministration);

    protected useAsync() { return true; }
    protected override getColumnsKey() { return LanguageColumns.columnsKey; }
    protected override getDialogType() { return LanguageDialog; }
    protected override getIdProperty() { return LanguageRow.idProperty; }
    protected override getLocalTextPrefix() { return LanguageRow.localTextPrefix; }
    protected override getService() { return LanguageService.baseUrl; }

    protected afterInit() {
        super.afterInit();
    }
    protected getDefaultSortBy() {
        return [LanguageRow.Fields.LanguageName];
    }
}