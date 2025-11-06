import { EntityDialog } from "@serenity-is/corelib";
import { LanguageForm, LanguageRow, LanguageService } from "../../ServerTypes/Administration";
import { nsAdministration } from "../../ServerTypes/Namespaces";

export class LanguageDialog extends EntityDialog<LanguageRow, any> {
    static override[Symbol.typeInfo] = this.registerClass(nsAdministration);

    protected override getFormKey() { return LanguageForm.formKey; }
    protected override getIdProperty() { return LanguageRow.idProperty; }
    protected override getLocalTextPrefix() { return LanguageRow.localTextPrefix; }
    protected override getNameProperty() { return LanguageRow.nameProperty; }
    protected override getService() { return LanguageService.baseUrl; }

    protected form = new LanguageForm(this.idPrefix);
}