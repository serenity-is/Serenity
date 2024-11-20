import { StringEditor, ImageUploadEditor, BooleanEditor, LookupEditor, ServiceLookupEditor, DecimalEditor, IntegerEditor, PrefixedContext, initFormType } from "@serenity-is/corelib";
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
    private static init: boolean;

    constructor(prefix: string) {
        super(prefix);

        if (!ProductForm.init)  {
            ProductForm.init = true;

            var w0 = StringEditor;
            var w1 = ImageUploadEditor;
            var w2 = BooleanEditor;
            var w3 = LookupEditor;
            var w4 = ServiceLookupEditor;
            var w5 = DecimalEditor;
            var w6 = IntegerEditor;

            initFormType(ProductForm, [
                'ProductName', w0,
                'ProductImage', w1,
                'Discontinued', w2,
                'SupplierID', w3,
                'CategoryID', w4,
                'QuantityPerUnit', w0,
                'UnitPrice', w5,
                'UnitsInStock', w6,
                'UnitsOnOrder', w6,
                'ReorderLevel', w6
            ]);
        }
    }
}

queueMicrotask(() => [SupplierDialog, CategoryDialog]); // referenced dialogs