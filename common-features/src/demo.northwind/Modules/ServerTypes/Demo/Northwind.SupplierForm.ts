import { initFormType, PrefixedContext, StringEditor } from "@serenity-is/corelib";

export interface SupplierForm {
    CompanyName: StringEditor;
    ContactName: StringEditor;
    ContactTitle: StringEditor;
    Address: StringEditor;
    Region: StringEditor;
    PostalCode: StringEditor;
    Country: StringEditor;
    City: StringEditor;
    Phone: StringEditor;
    Fax: StringEditor;
    HomePage: StringEditor;
}

export class SupplierForm extends PrefixedContext {
    static readonly formKey = 'Northwind.Supplier';
    declare private static init: boolean;

    constructor(...args: ConstructorParameters<typeof PrefixedContext>) {
        super(...args);

        if (!SupplierForm.init) {
            SupplierForm.init = true;

            initFormType(SupplierForm, [
                'CompanyName', StringEditor,
                'ContactName', StringEditor,
                'ContactTitle', StringEditor,
                'Address', StringEditor,
                'Region', StringEditor,
                'PostalCode', StringEditor,
                'Country', StringEditor,
                'City', StringEditor,
                'Phone', StringEditor,
                'Fax', StringEditor,
                'HomePage', StringEditor
            ]);
        }
    }
}