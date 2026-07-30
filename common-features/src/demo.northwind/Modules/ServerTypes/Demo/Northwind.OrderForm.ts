import { DateEditor, DecimalEditor, initFormType, LookupEditor, PrefixedContext, StringEditor } from "@serenity-is/corelib";
import { CustomerEditor } from "../../Customer/CustomerEditor";
import { OrderDetailsEditor } from "../../OrderDetail/OrderDetailsEditor";

export interface OrderForm {
    CustomerID: CustomerEditor;
    OrderDate: DateEditor;
    RequiredDate: DateEditor;
    EmployeeID: LookupEditor;
    DetailList: OrderDetailsEditor;
    ShippedDate: DateEditor;
    ShipVia: LookupEditor;
    Freight: DecimalEditor;
    ShipName: StringEditor;
    ShipAddress: StringEditor;
    ShipCity: StringEditor;
    ShipRegion: StringEditor;
    ShipPostalCode: StringEditor;
    ShipCountry: StringEditor;
}

export class OrderForm extends PrefixedContext {
    static readonly formKey = 'Northwind.Order';
    declare private static init: boolean;

    constructor(...args: ConstructorParameters<typeof PrefixedContext>) {
        super(...args);

        if (!OrderForm.init) {
            OrderForm.init = true;

            initFormType(OrderForm, [
                'CustomerID', CustomerEditor,
                'OrderDate', DateEditor,
                'RequiredDate', DateEditor,
                'EmployeeID', LookupEditor,
                'DetailList', OrderDetailsEditor,
                'ShippedDate', DateEditor,
                'ShipVia', LookupEditor,
                'Freight', DecimalEditor,
                'ShipName', StringEditor,
                'ShipAddress', StringEditor,
                'ShipCity', StringEditor,
                'ShipRegion', StringEditor,
                'ShipPostalCode', StringEditor,
                'ShipCountry', StringEditor
            ]);
        }
    }
}