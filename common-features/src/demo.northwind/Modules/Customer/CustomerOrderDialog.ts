import { OrderDialog } from "../Order/OrderDialog";

export class CustomerOrderDialog extends OrderDialog {
    static override typeInfo = this.registerClass("Serenity.Demo.Northwind.CustomerOrderDialog");

    updateInterface() {
        super.updateInterface();

        this.form.CustomerID.readOnly = true;
    }
}
