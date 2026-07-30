import { DateEditor, initFormType, LookupEditor, PrefixedContext, StringEditor } from "@serenity-is/corelib";
import { CustomerEditor, OrderDetailsEditor } from "@serenity-is/demo.northwind";

export interface PopulateLinkedDataForm {
    CustomerID: CustomerEditor;
    CustomerContactName: StringEditor;
    CustomerContactTitle: StringEditor;
    CustomerCity: StringEditor;
    CustomerRegion: StringEditor;
    CustomerCountry: StringEditor;
    CustomerPhone: StringEditor;
    CustomerFax: StringEditor;
    OrderDate: DateEditor;
    RequiredDate: DateEditor;
    EmployeeID: LookupEditor;
    DetailList: OrderDetailsEditor;
}

export class PopulateLinkedDataForm extends PrefixedContext {
    static readonly formKey = 'BasicSamples.PopulateLinkedData';
    declare private static init: boolean;

    constructor(...args: ConstructorParameters<typeof PrefixedContext>) {
        super(...args);

        if (!PopulateLinkedDataForm.init) {
            PopulateLinkedDataForm.init = true;

            initFormType(PopulateLinkedDataForm, [
                'CustomerID', CustomerEditor,
                'CustomerContactName', StringEditor,
                'CustomerContactTitle', StringEditor,
                'CustomerCity', StringEditor,
                'CustomerRegion', StringEditor,
                'CustomerCountry', StringEditor,
                'CustomerPhone', StringEditor,
                'CustomerFax', StringEditor,
                'OrderDate', DateEditor,
                'RequiredDate', DateEditor,
                'EmployeeID', LookupEditor,
                'DetailList', OrderDetailsEditor
            ]);
        }
    }
}