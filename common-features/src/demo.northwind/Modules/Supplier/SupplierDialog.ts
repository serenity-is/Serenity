import { Decorators, EntityDialog } from "@serenity-is/corelib";
import { SupplierForm, SupplierRow, SupplierService } from "../ServerTypes/Demo";

@Decorators.registerClass('Serenity.Demo.Northwind.SupplierDialog')
export class SupplierDialog<P = {}> extends EntityDialog<SupplierRow, P> {
    protected getFormKey() { return SupplierForm.formKey; }
    protected getRowDefinition() { return SupplierRow; }
    protected getService() { return SupplierService.baseUrl; }

    protected form = new SupplierForm(this.idPrefix);
}
