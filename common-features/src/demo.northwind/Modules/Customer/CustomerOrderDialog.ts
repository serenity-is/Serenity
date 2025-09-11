import { OrderDialog } from "../Order/OrderDialog";

export class CustomerOrderDialog extends OrderDialog {
    static override typeInfo = this.classTypeInfo("Serenity.Demo.Northwind.CustomerOrderDialog");

    updateInterface() {
        super.updateInterface();

        this.form.CustomerID.readOnly = true;
    }
}
