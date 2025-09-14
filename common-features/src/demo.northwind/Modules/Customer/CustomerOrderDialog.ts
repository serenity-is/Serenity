import { OrderDialog } from "../Order/OrderDialog";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";

export class CustomerOrderDialog extends OrderDialog {
    static [Symbol.typeInfo] = this.registerClass(nsDemoNorthwind);

    updateInterface() {
        super.updateInterface();

        this.form.CustomerID.readOnly = true;
    }
}
