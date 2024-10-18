import { Decorators, EntityDialog } from "@serenity-is/corelib";
import { TerritoryForm, TerritoryRow, TerritoryService } from "../ServerTypes/Demo";

@Decorators.registerClass('Serenity.Demo.Northwind.TerritoryDialog')
export class TerritoryDialog<P = {}> extends EntityDialog<TerritoryRow, P> {
    protected getFormKey() { return TerritoryForm.formKey; }
    protected getRowDefinition() { return TerritoryRow; }
    protected getService() { return TerritoryService.baseUrl; }

    protected form = new TerritoryForm(this.idPrefix);
}
