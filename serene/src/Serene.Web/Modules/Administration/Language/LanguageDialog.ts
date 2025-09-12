import { EntityDialog } from "@serenity-is/corelib";
import { LanguageForm, LanguageRow, LanguageService } from "../../ServerTypes/Administration";

export class LanguageDialog extends EntityDialog<LanguageRow, any> {
    static override typeInfo = this.registerClass("Serene.Administration.LanguageDialog");

    protected getFormKey() { return LanguageForm.formKey; }
    protected getIdProperty() { return LanguageRow.idProperty; }
    protected getLocalTextPrefix() { return LanguageRow.localTextPrefix; }
    protected getNameProperty() { return LanguageRow.nameProperty; }
    protected getService() { return LanguageService.baseUrl; }

    protected form = new LanguageForm(this.idPrefix);
}