import { EntityDialog } from "@serenity-is/corelib";
import { TerritoryForm, TerritoryRow, TerritoryService } from "../ServerTypes/Demo";

export class TerritoryDialog<P = {}> extends EntityDialog<TerritoryRow, P> {
    static override typeInfo = this.registerClass("Serenity.Demo.Northwind.TerritoryDialog");

    protected getFormKey() { return TerritoryForm.formKey; }
    protected getRowDefinition() { return TerritoryRow; }
    protected getService() { return TerritoryService.baseUrl; }

    protected form = new TerritoryForm(this.idPrefix);
}
