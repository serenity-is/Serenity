import { initFormType, PrefixedContext, StringEditor } from "@serenity-is/corelib";
import { PhoneEditor } from "../../Shared/PhoneEditor";

export interface ShipperForm {
    CompanyName: StringEditor;
    Phone: PhoneEditor;
}

export class ShipperForm extends PrefixedContext {
    static readonly formKey = 'Northwind.Shipper';
    declare private static init: boolean;

    constructor(...args: ConstructorParameters<typeof PrefixedContext>) {
        super(...args);

        if (!ShipperForm.init) {
            ShipperForm.init = true;

            initFormType(ShipperForm, [
                'CompanyName', StringEditor,
                'Phone', PhoneEditor
            ]);
        }
    }
}