import { EntityDialog } from "@serenity-is/corelib";
import { TerritoryForm, TerritoryRow, TerritoryService } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";

export class TerritoryDialog<P = {}> extends EntityDialog<TerritoryRow, P> {
    static override typeInfo = this.registerClass(nsDemoNorthwind);

    protected getFormKey() { return TerritoryForm.formKey; }
    protected getRowDefinition() { return TerritoryRow; }
    protected getService() { return TerritoryService.baseUrl; }

    protected form = new TerritoryForm(this.idPrefix);
}
