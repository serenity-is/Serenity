import { Decorators, EntityDialog } from "@serenity-is/corelib";
import { ProductForm, ProductRow, ProductService } from "../ServerTypes/Demo";
import "./ProductDialog.css";

@Decorators.registerClass()
@Decorators.maximizable()
export class ProductDialog<P = {}> extends EntityDialog<ProductRow, P> {
    protected getFormKey() { return ProductForm.formKey; }
    protected getRowDefinition() { return ProductRow; }
    protected getService() { return ProductService.baseUrl; }

    protected form = new ProductForm(this.idPrefix);
}
