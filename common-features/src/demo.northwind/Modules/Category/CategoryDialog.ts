import { Decorators, EntityDialog } from "@serenity-is/corelib";
import { CategoryForm, CategoryRow, CategoryService } from "../ServerTypes/Demo";

export class CategoryDialog<P = {}> extends EntityDialog<CategoryRow, P> {
    static override typeInfo = this.classTypeInfo("Serenity.Demo.Northwind.CategoryDialog");

    protected getFormKey() { return CategoryForm.formKey; }
    protected getRowDefinition() { return CategoryRow; }
    protected getService() { return CategoryService.baseUrl; }

    protected form = new CategoryForm(this.idPrefix);
}