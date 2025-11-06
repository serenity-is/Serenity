
import { SubDialogHelper, type Widget } from "@serenity-is/corelib";
import { Column } from "@serenity-is/sleekgrid";
import { OrderGrid } from "../Order/OrderGrid";
import { OrderRow } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";
import { CustomerOrderDialog } from "./CustomerOrderDialog";

const fld = OrderRow.Fields;

export class CustomerOrdersGrid<P = {}> extends OrderGrid<P> {
    static override[Symbol.typeInfo] = this.registerClass(nsDemoNorthwind);

    protected override getDialogType() { return CustomerOrderDialog; }

    protected override createColumns(): Column[] {
        return super.createColumns().filter(x => x.field !== fld.CustomerCompanyName);
    }

    protected override initEntityDialog(itemType: string, dialog: Widget<any>) {
        super.initEntityDialog(itemType, dialog);
        SubDialogHelper.cascade(dialog, this.domNode.closest('.ui-dialog') as HTMLElement);
    }

    protected override getButtons() {
        var buttons = super.getButtons();
        var addButton = buttons.find(x => x.action === 'add');
        if (addButton)
            addButton.disabled = () => !this.customerID;
        return buttons;
    }

    protected override addButtonClick() {
        if (!this.customerID)
            return;
        this.editItem({ CustomerID: this.customerID });
    }

    protected override getInitialTitle() {
        return null;
    }

    protected override getGridCanLoad() {
        return super.getGridCanLoad() && !!this.customerID;
    }

    declare private _customerID: string;

    get customerID() {
        return this._customerID;
    }

    set customerID(value: string) {
        if (this._customerID !== value) {
            this._customerID = value;
            this.setEquality('CustomerID', value);
            this.refresh();
            this.updateInterface();
        }
    }
}
