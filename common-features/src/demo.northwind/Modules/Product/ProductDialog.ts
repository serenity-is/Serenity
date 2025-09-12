import { EntityDialog, MaximizableAttribute } from "@serenity-is/corelib";
import { ProductForm, ProductRow, ProductService } from "../ServerTypes/Demo";
import "./ProductDialog.css";

export class ProductDialog<P = {}> extends EntityDialog<ProductRow, P> {
    static override typeInfo = this.registerClass("Serenity.Demo.Northwind.ProductDialog", [new MaximizableAttribute()] );

    protected getFormKey() { return ProductForm.formKey; }
    protected getRowDefinition() { return ProductRow; }
    protected getService() { return ProductService.baseUrl; }

    protected form = new ProductForm(this.idPrefix);
}
