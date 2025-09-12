import { EntityDialog, WidgetProps, gridPageInit, toId } from "@serenity-is/corelib";
import { CustomerRow, CustomerService, OrderGrid, OrderRow, OrderService } from "@serenity-is/demo.northwind";
import { PopulateLinkedDataForm } from "../../ServerTypes/Demo";

export default () => gridPageInit(PopulateLinkedDataGrid);

/**
 * A subclass of OrderGrid that launches PopulateLinkedDataDialog
 */
export class PopulateLinkedDataGrid extends OrderGrid {
    static override typeInfo = this.registerClass("Serenity.Demo.BasicSamples.PopulateLinkedDataGrid");

    protected getDialogType() { return PopulateLinkedDataDialog; }
}

export class PopulateLinkedDataDialog<P = {}> extends EntityDialog<OrderRow, P> {
    static override typeInfo = this.registerClass("Serenity.Demo.BasicSamples.PopulateLinkedDataDialog");

    protected getFormKey() { return PopulateLinkedDataForm.formKey; }
    protected getRowDefinition() { return OrderRow; }
    protected getService() { return OrderService.baseUrl; }

    protected form = new PopulateLinkedDataForm(this.idPrefix);

    constructor(props: WidgetProps<P>) {
        super(props);

        // "changeSelect2" is only fired when user changes the selection
        // but "change" is fired when dialog sets customer on load too
        // so we prefer "changeSelect2", as initial customer details 
        // will get populated by initial load, we don't want extra call
        this.form.CustomerID.changeSelect2(async e => {
            const customerID = toId(this.form.CustomerID.value);
            if (!customerID) {
                this.setCustomerDetails({});
                return;
            }

            CustomerService.Retrieve({
                EntityId: customerID
            }, response => {
                this.setCustomerDetails(response.Entity);
            });
        });
    }

    private setCustomerDetails(details: CustomerRow) {
        this.form.CustomerCity.value = details.City;
        this.form.CustomerContactName.value = details.ContactName;
        this.form.CustomerContactTitle.value = details.ContactTitle;
        this.form.CustomerCountry.value = details.Country;
        this.form.CustomerFax.value = details.Fax;
        this.form.CustomerPhone.value = details.Phone;
        this.form.CustomerRegion.value = details.Region;
    }

    /**
     * This dialog will have CSS class "s-PopulateLinkedDataDialog"
     * We are changing it here to "s-OrderDialog", to make it use default OrderDialog styles
     * This has no effect other than looks on populate linked data demonstration
     */
    protected getCssClass() {
        return super.getCssClass() + " s-OrderDialog s-Demo-Northwind-OrderDialog";
    }
}