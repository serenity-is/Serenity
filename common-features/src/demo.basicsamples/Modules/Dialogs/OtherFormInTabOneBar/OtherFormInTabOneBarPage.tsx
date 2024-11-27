import { Decorators, Fluent, PropertyGrid, SaveResponse, TabsExtensions, Validator, WidgetProps, first, getForm, gridPageInit, isEmptyOrNull, reloadLookup, validateOptions } from "@serenity-is/corelib";
import { CustomerForm, CustomerRow, CustomerService, OrderDialog, OrderGrid, OrderRow } from "@serenity-is/demo.northwind";

export default () => gridPageInit(OtherFormInTabOneBarGrid);

/**
 * Subclass of OrderGrid to override dialog type to OtherFormInTabOneBarDialog
 */
@Decorators.registerClass('Serenity.Demo.BasicSamples.OtherFormInTabOneBarGrid')
export class OtherFormInTabOneBarGrid extends OrderGrid {
    protected override getDialogType() { return OtherFormOneBarDialog; }
}

/**
 * Our custom order dialog subclass that will have a tab to display and edit selected customer details.
 * With single toolbar for all forms
 */
@Decorators.registerClass('Serenity.Demo.BasicSamples.OtherFormOneBarDialog')
export class OtherFormOneBarDialog<P = {}> extends OrderDialog<P> {

    declare private customerPropertyGrid: PropertyGrid;
    declare private customerValidator: any;
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

        // unfortunately, CustomerID (a string) used in this form and 
        // the ID (auto increment ID) are different, so we need to 
        // find numeric ID from customer lookups. 
        // you'll probably won't need this step.
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

    // Save the customer and the order 
    protected async saveCustomer(callback: (response: SaveResponse) => void, onSuccess?: (response: SaveResponse) => void) {
        this.getCustomerID().then(id => {
            if (!id) {
                // If id of Customer isn't present, we save only Order entity
                onSuccess(null);
            }
            else {
                // Get current tab
                var currTab = TabsExtensions.activeTabKey(this.tabs);

                // Select the correct tab and validate to see the error message in tab
                TabsExtensions.selectTab(this.tabs, "Customer")
                if (!this.customerValidator.form()) {
                    return;
                }

                // Re-select initial tab
                TabsExtensions.selectTab(this.tabs, currTab)

                // prepare an empty entity to serialize customer details into
                var c: CustomerRow = {};
                this.customerPropertyGrid.save(c);

                CustomerService.Update({
                    EntityId: id,
                    Entity: c
                }, response => {
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

                    onSuccess(response);
                });
            }
        });
    }

    // Call super.save to save Order entity
    protected saveOrder(callback: (response: SaveResponse) => void) {
        super.save(callback);
    }

    protected saveAll(callback: (response: SaveResponse) => void) {
        this.saveCustomer(callback,
            // If customer success, save Order entity
            () => this.saveOrder(callback)
        );
    }

    // This is called when save/update button is pressed
    protected save(callback: (response: SaveResponse) => void) {
        this.saveAll(callback);
    }

    renderContents() {
        const id = this.useIdPrefix();
        return (<>
            <div id={id.Toolbar} class="s-DialogToolbar">
            </div>
            <div id={id.Tabs} class="s-DialogContent">
                <ul>
                    <li><a href={'#' + id.TabOrder}><span>Order</span></a></li>
                    <li><a href={'#' + id.TabCustomer}><span>Customer</span></a></li>
                </ul>
                <div id={id.TabOrder} class="tab-pane s-TabOrder">
                    <form id={id.Form} action="" class="s-Form">
                        <div id={id.PropertyGrid}></div>
                    </form>
                </div>
                <div id={id.TabCustomer} class="tab-pane s-TabCustomer">
                    <form id={id.CustomerForm} action="" class="s-Form" ref={el => this.customerValidator = new Validator(el, validateOptions({}))}>
                        <PropertyGrid ref={pg => this.customerPropertyGrid = pg} idPrefix={this.idPrefix + "_Customer_"}
                            items={getForm(CustomerForm.formKey).filter(x => x.name != 'CustomerID' && x.name != "NoteList")} />
                    </form>
                </div>
            </div>
        </>)
    }
}