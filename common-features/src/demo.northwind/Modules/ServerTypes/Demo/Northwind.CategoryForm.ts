import { initFormType, PrefixedContext, StringEditor } from "@serenity-is/corelib";

export interface CategoryForm {
    CategoryName: StringEditor;
    Description: StringEditor;
}

export class CategoryForm extends PrefixedContext {
    static readonly formKey = 'Northwind.Category';
    declare private static init: boolean;

    constructor(...args: ConstructorParameters<typeof PrefixedContext>) {
        super(...args);

        if (!CategoryForm.init) {
            CategoryForm.init = true;

            initFormType(CategoryForm, [
                'CategoryName', StringEditor,
                'Description', StringEditor
            ]);
        }
    }
}