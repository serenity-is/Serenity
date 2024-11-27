import { Decorators, Fluent, PropertyGrid, TabsExtensions, Toolbar, Validator, WidgetProps, first, getForm, gridPageInit, isEmptyOrNull, localText, notifySuccess, reloadLookup, validateOptions } from "@serenity-is/corelib";
import { CustomerForm, CustomerRow, CustomerService, OrderDialog, OrderGrid, OrderRow } from "@serenity-is/demo.northwind";

export default () => gridPageInit(OtherFormInTabGrid);

/**
 * Subclass of OrderGrid to override dialog type to OtherFormInTabDialog
 */
@Decorators.registerClass('Serenity.Demo.BasicSamples.OtherFormInTabGrid')
export class OtherFormInTabGrid<P = {}> extends OrderGrid<P> {
    protected override getDialogType() { return OtherFormInTabDialog; }
}

/**
 * Our custom order dialog subclass that will have a tab to display and edit selected customer details.
 */
@Decorators.registerClass('Serenity.Demo.BasicSamples.OtherFormInTabDialog')
export class OtherFormInTabDialog<P = {}> extends OrderDialog<P> {

    declare private customerValidator: any;
    declare private customerPropertyGrid: PropertyGrid;
    declare private selfChange: number;

    constructor(props: WidgetProps<P>) {
        super(props);

        this.selfChange = 0;
        this.form.CustomerID.change(e => {
            if (this.selfChange)
                return;

            (async () => {
                var customerID = await this.getCustomerID();

                TabsExtensions.setDisabled(this.tabs, 'Customer', !customerID);

                if (!customerID) {
                    // no customer is selected, just load an empty entity
                    this.customerPropertyGrid.load({});
                    return;
                }

                // load selected customer into customer form by calling CustomerService
                CustomerService.Retrieve({
                    EntityId: customerID
                }, response => {
                    this.customerPropertyGrid.load(response.Entity);
                });
            })();

        });
    }

    async getCustomerID() {
        var customerID = this.form.CustomerID.value;

        if (isEmptyOrNull(customerID))
            return null;

        // unfortunately, CustomerID (a string) used in this form and the ID (auto increment ID) are different, so we need to 
        // find numeric ID from customer lookups. you'll probably won't need this step.
        return first((await CustomerRow.getLookupAsync()).items,
            x => x.CustomerID == customerID).ID;
    }

    loadEntity(entity: OrderRow) {
        super.loadEntity(entity);

        (async () => {
            TabsExtensions.setDisabled(this.tabs, 'Customer',
                !(await this.getCustomerID()));
        })();
    }

    private async customerSaveClick() {
        var id = await this.getCustomerID();
        if (!id)
            return;

        if (!this.customerValidator.form())
            return;

        // prepare an empty entity to serialize customer details into
        var c: CustomerRow = {};
        this.customerPropertyGrid.save(c);

        CustomerService.Update({
            EntityId: id,
            Entity: c
        }, () => {
            // reload customer list just in case
            reloadLookup(CustomerRow.lookupKey);

            // set flag that we are triggering customer select change event
            // otherwise active tab will change to first one
            this.selfChange++;
            try {
                // trigger change so that customer select updates its text
                // in case if Company Name is changed
                Fluent.trigger(this.form.CustomerID.domNode, "change");
            }
            finally {
                this.selfChange--;
            }

            notifySuccess("Saved customer details");
        });
    }

    renderContents() {
        const id = this.useIdPrefix();
        return (
            <div id={id.Tabs} class="s-DialogContent">
                <ul>
                    <li><a href={'#' + id.TabOrder}><span>Order</span></a></li>
                    <li><a href={'#' + id.TabCustomer}><span>Customer</span></a></li>
                </ul>
                <div id={id.TabOrder} class="tab-pane">
                    <div id={id.Toolbar} class="s-DialogToolbar"></div>
                    <form id={id.Form} action="" class="s-Form">
                        <div id={id.PropertyGrid}></div>
                    </form>
                </div>
                <div id={id.TabCustomer} class="tab-pane">
                    <Toolbar class="s-DialogToolbar" buttons={[{
                        cssClass: "apply-changes-button",
                        title: localText("Controls.EntityDialog.SaveButton"),
                        onClick: () => this.customerSaveClick()
                    }]} />
                    <form action="" class="s-Form" ref={el => this.customerValidator = new Validator(el, validateOptions())}>
                        <PropertyGrid ref={pg => this.customerPropertyGrid = pg} idPrefix={id.Customer_}
                            items={getForm(CustomerForm.formKey).filter(x => x.name != CustomerRow.Fields.CustomerID)} />
                    </form>
                </div>
            </div>
        );
    }
}