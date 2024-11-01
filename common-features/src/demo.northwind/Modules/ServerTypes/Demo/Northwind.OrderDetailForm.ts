import { IntegerEditor, LookupEditor, DecimalEditor, PrefixedContext, initFormType } from "@serenity-is/corelib";

export interface OrderDetailForm {
    OrderID: IntegerEditor;
    ProductID: LookupEditor;
    UnitPrice: DecimalEditor;
    Quantity: IntegerEditor;
    Discount: DecimalEditor;
}

export class OrderDetailForm extends PrefixedContext {
    static readonly formKey = 'Northwind.OrderDetail';
    private static init: boolean;

    constructor(prefix: string) {
        super(prefix);

        if (!OrderDetailForm.init)  {
            OrderDetailForm.init = true;

            var w0 = IntegerEditor;
            var w1 = LookupEditor;
            var w2 = DecimalEditor;

            initFormType(OrderDetailForm, [
                'OrderID', w0,
                'ProductID', w1,
                'UnitPrice', w2,
                'Quantity', w0,
                'Discount', w2
            ]);
        }
    }
}