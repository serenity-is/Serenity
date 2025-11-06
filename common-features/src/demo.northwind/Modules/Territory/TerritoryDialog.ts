import { EntityDialog } from "@serenity-is/corelib";
import { TerritoryForm, TerritoryRow, TerritoryService } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";

export class TerritoryDialog<P = {}> extends EntityDialog<TerritoryRow, P> {
    static override[Symbol.typeInfo] = this.registerClass(nsDemoNorthwind);

    protected override getFormKey() { return TerritoryForm.formKey; }
    protected override getRowDefinition() { return TerritoryRow; }
    protected override getService() { return TerritoryService.baseUrl; }

    protected form = new TerritoryForm(this.idPrefix);
}
