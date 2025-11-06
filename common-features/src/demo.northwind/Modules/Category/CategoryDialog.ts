import { EntityDialog } from "@serenity-is/corelib";
import { CategoryForm, CategoryRow, CategoryService } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";

export class CategoryDialog<P = {}> extends EntityDialog<CategoryRow, P> {
    static override[Symbol.typeInfo] = this.registerClass(nsDemoNorthwind);

    protected override getFormKey() { return CategoryForm.formKey; }
    protected override getRowDefinition() { return CategoryRow; }
    protected override getService() { return CategoryService.baseUrl; }

    protected form = new CategoryForm(this.idPrefix);
}