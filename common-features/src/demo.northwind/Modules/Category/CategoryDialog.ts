import { CategoryForm, CategoryRow, CategoryService } from "../ServerTypes/Demo";
import { Decorators, EntityDialog } from "@serenity-is/corelib";

@Decorators.registerClass('Serenity.Demo.Northwind.CategoryDialog')
export class CategoryDialog<P = {}> extends EntityDialog<CategoryRow, P> {
    protected getFormKey() { return CategoryForm.formKey; }
    protected getRowDefinition() { return CategoryRow; }
    protected getService() { return CategoryService.baseUrl; }

    protected form = new CategoryForm(this.idPrefix);
}