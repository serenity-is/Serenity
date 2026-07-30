import { BooleanEditor, DecimalEditor, ImageUploadEditor, initFormType, IntegerEditor, LookupEditor, PrefixedContext, ServiceLookupEditor, StringEditor } from "@serenity-is/corelib";
import { CategoryDialog } from "../../Category/CategoryDialog";
import { SupplierDialog } from "../../Supplier/SupplierDialog";

export interface ProductForm {
    ProductName: StringEditor;
    ProductImage: ImageUploadEditor;
    Discontinued: BooleanEditor;
    SupplierID: LookupEditor;
    CategoryID: ServiceLookupEditor;
    QuantityPerUnit: StringEditor;
    UnitPrice: DecimalEditor;
    UnitsInStock: IntegerEditor;
    UnitsOnOrder: IntegerEditor;
    ReorderLevel: IntegerEditor;
}

export class ProductForm extends PrefixedContext {
    static readonly formKey = 'Northwind.Product';
    declare private static init: boolean;

    constructor(...args: ConstructorParameters<typeof PrefixedContext>) {
        super(...args);

        if (!ProductForm.init) {
            ProductForm.init = true;

            initFormType(ProductForm, [
                'ProductName', StringEditor,
                'ProductImage', ImageUploadEditor,
                'Discontinued', BooleanEditor,
                'SupplierID', LookupEditor,
                'CategoryID', ServiceLookupEditor,
                'QuantityPerUnit', StringEditor,
                'UnitPrice', DecimalEditor,
                'UnitsInStock', IntegerEditor,
                'UnitsOnOrder', IntegerEditor,
                'ReorderLevel', IntegerEditor
            ]);
        }
    }
}

queueMicrotask(() => [SupplierDialog, CategoryDialog]); // referenced dialogs