import { DecimalEditor, initFormType, IntegerEditor, PrefixedContext, ServiceLookupEditor } from "@serenity-is/corelib";

export interface OrderDetailForm {
    OrderID: IntegerEditor;
    ProductID: ServiceLookupEditor;
    UnitPrice: DecimalEditor;
    Quantity: IntegerEditor;
    Discount: DecimalEditor;
}

export class OrderDetailForm extends PrefixedContext {
    static readonly formKey = 'Northwind.OrderDetail';
    declare private static init: boolean;

    constructor(...args: ConstructorParameters<typeof PrefixedContext>) {
        super(...args);

        if (!OrderDetailForm.init) {
            OrderDetailForm.init = true;

            initFormType(OrderDetailForm, [
                'OrderID', IntegerEditor,
                'ProductID', ServiceLookupEditor,
                'UnitPrice', DecimalEditor,
                'Quantity', IntegerEditor,
                'Discount', DecimalEditor
            ]);
        }
    }
}