import { EntityDialog } from "@serenity-is/corelib";
import { RegionForm, RegionRow, RegionService } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";

export class RegionDialog<P = {}> extends EntityDialog<RegionRow, P> {
    static override [Symbol.typeInfo] = this.registerClass(nsDemoNorthwind);
    
    protected getFormKey() { return RegionForm.formKey; }
    protected getRowDefinition() { return RegionRow; }
    protected getService() { return RegionService.baseUrl; }

    protected form = new RegionForm(this.idPrefix);
}
