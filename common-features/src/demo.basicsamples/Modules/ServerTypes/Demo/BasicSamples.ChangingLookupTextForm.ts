import { DecimalEditor, IntegerEditor, PrefixedContext, initFormType } from "@serenity-is/corelib";
import { ChangingLookupTextEditor } from "../../Editors/ChangingLookupText/ChangingLookupTextPage";

export interface ChangingLookupTextForm {
    ProductID: ChangingLookupTextEditor;
    UnitPrice: DecimalEditor;
    Quantity: IntegerEditor;
    Discount: DecimalEditor;
}

export class ChangingLookupTextForm extends PrefixedContext {
    static readonly formKey = 'BasicSamples.ChangingLookupText';
    private static init: boolean;

    constructor(prefix: string) {
        super(prefix);

        if (!ChangingLookupTextForm.init)  {
            ChangingLookupTextForm.init = true;

            var w0 = ChangingLookupTextEditor;
            var w1 = DecimalEditor;
            var w2 = IntegerEditor;

            initFormType(ChangingLookupTextForm, [
                'ProductID', w0,
                'UnitPrice', w1,
                'Quantity', w2,
                'Discount', w1
            ]);
        }
    }
}