import { OrderDetailColumns, OrderDetailRow, OrderDetailService, ProductRow } from "../ServerTypes/Demo";
import { Decorators, alertDialog, toId } from "@serenity-is/corelib";
import { GridEditorBase } from "@serenity-is/extensions";
import { OrderDetailDialog } from "./OrderDetailDialog";

@Decorators.registerEditor('Serenity.Demo.Northwind.OrderDetailsEditor')
export class OrderDetailsEditor<P = {}> extends GridEditorBase<OrderDetailRow, P> {
    protected getColumnsKey() { return OrderDetailColumns.columnsKey; }
    protected getDialogType() { return OrderDetailDialog; }
    protected getRowDefinition() { return OrderDetailRow; }
    protected getService() { return OrderDetailService.baseUrl; }

    protected override validateEntity(row: OrderDetailRow, id: any) {
        row.ProductID = toId(row.ProductID);

        var sameProduct = this.view.getItems().find(x => x.ProductID === row.ProductID);
        if (sameProduct && this.id(sameProduct) !== id) {
            alertDialog('This product is already in order details!');
            return false;
        }

        id ??= row[this.getIdProperty()];

        ProductRow.getLookupAsync().then(lookup => {
            var item = this.view?.getItemById?.(id);
            if (item) {
                item.ProductName = lookup.itemById[row.ProductID].ProductName;
                this.view.updateItem(id, item);
            }
        });

        row.LineTotal = (row.Quantity || 0) * (row.UnitPrice || 0) - (row.Discount || 0);
        return true;
    }

    protected override getGridCanLoad() {
        return super.getGridCanLoad() && this.orderId != null;
    }

    protected override getNewEntity() {
        return {
            OrderID: this.orderId
        };
    }

    private _orderId: number;
    
    public get orderId() { 
        return this._orderId; 
    }

    public set orderId(value: number) {
        if (this._orderId !== toId(value)) {
            this.setEquality(OrderDetailRow.Fields.OrderID, this._orderId = toId(value));
            this.connectedMode = this._orderId != null;
            this.refresh();
        }
    }
}
