import { EntityDialog, MaximizableAttribute } from "@serenity-is/corelib";
import { ProductForm, ProductRow, ProductService } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";
import "./ProductDialog.css";

export class ProductDialog<P = {}> extends EntityDialog<ProductRow, P> {
    static override[Symbol.typeInfo] = this.registerClass(nsDemoNorthwind, [new MaximizableAttribute()] );

    protected override getFormKey() { return ProductForm.formKey; }
    protected override getRowDefinition() { return ProductRow; }
    protected override getService() { return ProductService.baseUrl; }

    protected form = new ProductForm(this.idPrefix);
}
