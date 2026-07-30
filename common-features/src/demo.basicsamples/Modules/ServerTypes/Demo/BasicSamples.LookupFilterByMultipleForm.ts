import { BooleanEditor, DecimalEditor, ImageUploadEditor, initFormType, IntegerEditor, LookupEditor, PrefixedContext, StringEditor } from "@serenity-is/corelib";
import { SupplierDialog } from "@serenity-is/demo.northwind";
import { ProduceSeafoodCategoryEditor } from "../../Editors/LookupFilterByMultipleValues/LookupFilterByMultipleValuesPage";

export interface LookupFilterByMultipleForm {
    ProductName: StringEditor;
    ProductImage: ImageUploadEditor;
    Discontinued: BooleanEditor;
    SupplierID: LookupEditor;
    CategoryID: ProduceSeafoodCategoryEditor;
    QuantityPerUnit: StringEditor;
    UnitPrice: DecimalEditor;
    UnitsInStock: IntegerEditor;
    UnitsOnOrder: IntegerEditor;
    ReorderLevel: IntegerEditor;
}

export class LookupFilterByMultipleForm extends PrefixedContext {
    static readonly formKey = 'BasicSamples.LookupFilterByMultiple';
    declare private static init: boolean;

    constructor(...args: ConstructorParameters<typeof PrefixedContext>) {
        super(...args);

        if (!LookupFilterByMultipleForm.init) {
            LookupFilterByMultipleForm.init = true;

            initFormType(LookupFilterByMultipleForm, [
                'ProductName', StringEditor,
                'ProductImage', ImageUploadEditor,
                'Discontinued', BooleanEditor,
                'SupplierID', LookupEditor,
                'CategoryID', ProduceSeafoodCategoryEditor,
                'QuantityPerUnit', StringEditor,
                'UnitPrice', DecimalEditor,
                'UnitsInStock', IntegerEditor,
                'UnitsOnOrder', IntegerEditor,
                'ReorderLevel', IntegerEditor
            ]);
        }
    }
}

queueMicrotask(() => [SupplierDialog]); // referenced dialogs