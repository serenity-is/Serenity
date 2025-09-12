import { EntityDialog } from "@serenity-is/corelib";
import { CategoryForm, CategoryRow, CategoryService } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";

export class CategoryDialog<P = {}> extends EntityDialog<CategoryRow, P> {
    static override typeInfo = this.registerClass(nsDemoNorthwind);

    protected getFormKey() { return CategoryForm.formKey; }
    protected getRowDefinition() { return CategoryRow; }
    protected getService() { return CategoryService.baseUrl; }

    protected form = new CategoryForm(this.idPrefix);
}