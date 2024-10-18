import { StringEditor, DateEditor, LookupEditor, PrefixedContext, initFormType } from "@serenity-is/corelib";
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
    private static init: boolean;

    constructor(prefix: string) {
        super(prefix);

        if (!PopulateLinkedDataForm.init)  {
            PopulateLinkedDataForm.init = true;

            var w0 = CustomerEditor;
            var w1 = StringEditor;
            var w2 = DateEditor;
            var w3 = LookupEditor;
            var w4 = OrderDetailsEditor;

            initFormType(PopulateLinkedDataForm, [
                'CustomerID', w0,
                'CustomerContactName', w1,
                'CustomerContactTitle', w1,
                'CustomerCity', w1,
                'CustomerRegion', w1,
                'CustomerCountry', w1,
                'CustomerPhone', w1,
                'CustomerFax', w1,
                'OrderDate', w2,
                'RequiredDate', w2,
                'EmployeeID', w3,
                'DetailList', w4
            ]);
        }
    }
}