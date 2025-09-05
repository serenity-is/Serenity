import { Decorators } from "@serenity-is/corelib";
import { OrderDialog } from "../Order/OrderDialog";

@Decorators.registerClass('Serenity.Demo.Northwind.CustomerOrderDialog')
export class CustomerOrderDialog extends OrderDialog {

    updateInterface() {
        super.updateInterface();

        this.form.CustomerID.readOnly = true;
    }
}
