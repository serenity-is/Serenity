import { DecimalEditor, initFormType, IntegerEditor, PrefixedContext } from "@serenity-is/corelib";
import { ChangingLookupTextEditor } from "../../Editors/ChangingLookupText/ChangingLookupTextPage";

export interface ChangingLookupTextForm {
    ProductID: ChangingLookupTextEditor;
    UnitPrice: DecimalEditor;
    Quantity: IntegerEditor;
    Discount: DecimalEditor;
}

export class ChangingLookupTextForm extends PrefixedContext {
    static readonly formKey = 'BasicSamples.ChangingLookupText';
    declare private static init: boolean;

    constructor(...args: ConstructorParameters<typeof PrefixedContext>) {
        super(...args);

        if (!ChangingLookupTextForm.init) {
            ChangingLookupTextForm.init = true;

            initFormType(ChangingLookupTextForm, [
                'ProductID', ChangingLookupTextEditor,
                'UnitPrice', DecimalEditor,
                'Quantity', IntegerEditor,
                'Discount', DecimalEditor
            ]);
        }
    }
}