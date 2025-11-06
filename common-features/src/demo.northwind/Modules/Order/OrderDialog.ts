import { EntityDialog } from "@serenity-is/corelib";
import { ReportHelper } from "@serenity-is/extensions";
import { OrderForm, OrderRow, OrderService } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";
import "./OrderDialog.css";

export class OrderDialog<P = {}> extends EntityDialog<OrderRow, P> {
    static override[Symbol.typeInfo] = this.registerClass(nsDemoNorthwind);

    protected override getFormKey() { return OrderForm.formKey; }
    protected override getRowDefinition() { return OrderRow; }
    protected override getService() { return OrderService.baseUrl; }

    protected form = new OrderForm(this.idPrefix);

    protected override getToolbarButtons() {
        var buttons = super.getToolbarButtons();

        buttons.push(ReportHelper.createToolButton({
            title: 'Invoice',
            cssClass: 'export-pdf-button',
            reportKey: 'Northwind.OrderDetail',
            getParams: () => ({
                OrderID: this.entityId
            })
        }));

        return buttons;
    }

    protected override updateInterface() {
        super.updateInterface();

        this.toolbar.findButton('export-pdf-button').toggle(this.isEditMode());
    }

    protected override afterLoadEntity() {
        super.afterLoadEntity();
        this.form.DetailList.orderId = this.entityId;
    }
}