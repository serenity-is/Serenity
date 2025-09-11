import { EntityDialog } from "@serenity-is/corelib";
import { RegionForm, RegionRow, RegionService } from "../ServerTypes/Demo";

export class RegionDialog<P = {}> extends EntityDialog<RegionRow, P> {
    static override typeInfo = this.classTypeInfo("Serenity.Demo.Northwind.RegionDialog");
    
    protected getFormKey() { return RegionForm.formKey; }
    protected getRowDefinition() { return RegionRow; }
    protected getService() { return RegionService.baseUrl; }

    protected form = new RegionForm(this.idPrefix);
}
