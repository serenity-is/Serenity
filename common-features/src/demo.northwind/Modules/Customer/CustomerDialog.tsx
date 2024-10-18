import { CustomerForm, CustomerRow, CustomerService } from "../ServerTypes/Demo";
import { Decorators, EntityDialog, TabsExtensions, WidgetProps, htmlEncode, localText, reloadLookup } from "@serenity-is/corelib";
import { DialogUtils } from "@serenity-is/extensions";
import { CustomerOrdersGrid } from "./CustomerOrdersGrid";
import "./CustomerDialog.css";

@Decorators.registerClass('Serenity.Demo.Northwind.CustomerDialog')
export class CustomerDialog<P = {}> extends EntityDialog<CustomerRow, P> {
    protected getFormKey() { return CustomerForm.formKey; }
    protected getRowDefinition() { return CustomerRow; }
    protected getService() { return CustomerService.baseUrl; }

    protected form = new CustomerForm(this.idPrefix);

    declare private ordersGrid: CustomerOrdersGrid;
    declare private loadedState: string;

    constructor(props: WidgetProps<P>) {
        super(props);

        this.byId('NoteList').closest('.field').hide();
        this.byId('NoteList').appendTo(this.byId('TabNotes'));
    }

    override initDialog() {
        super.initDialog();
        DialogUtils.pendingChangesConfirmation(this.domNode, () => this.getSaveState() != this.loadedState);
    }

    getSaveState() {
        try {
            return JSON.stringify(this.getSaveEntity());
        }
        catch (e) {
            return null;
        }
    }

    loadResponse(data: any) {
        super.loadResponse(data);
        this.loadedState = this.getSaveState();
    }

    loadEntity(entity: CustomerRow) {
        super.loadEntity(entity);

        TabsExtensions.setDisabled(this.tabs, 'Orders', this.isNewOrDeleted());

        this.ordersGrid.customerID = entity.CustomerID;
    }

    onSaveSuccess(response) {
        super.onSaveSuccess(response);

        reloadLookup('Northwind.Customer');
    }

    renderContents(): any {
        const id = this.useIdPrefix();
        return (
            <div id={id.Tabs} class="s-DialogContent">
                <ul>
                    <li><a href={'#' + id.TabInfo}><span>{localText("Db.Northwind.Customer.EntitySingular")}</span></a></li>
                    <li><a href={'#' + id.TabNotes}><span>{localText("Db.Northwind.Note.EntityPlural")}</span></a></li>
                    <li><a href={'#' + id.TabOrders}><span>{localText("Db.Northwind.Order.EntityPlural")}</span></a></li>
                </ul>
                <div id={id.TabInfo} class="tab-pane s-TabInfo">
                    <div id={id.Toolbar} class="s-DialogToolbar">
                    </div>
                    <form id={id.Form} action="" class="s-Form">
                        <div id={id.PropertyGrid}></div>
                    </form>
                </div>
                <div id={id.TabNotes} class="tab-pane s-TabNotes">
                </div>
                <div id={id.TabOrders} class="tab-pane s-TabOrders">
                    <CustomerOrdersGrid id={id.OrdersGrid} ref={grid => {
                        this.ordersGrid = grid;
                        // force order dialog to open in Dialog mode instead of Panel mode
                        // which is set as default on OrderDialog with @panelAttribute
                        this.ordersGrid.openDialogsAsPanel = false;
                    }} />
                </div>
            </div>
        )
    }
}
