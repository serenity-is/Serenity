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

    protected override async validateEntity(row: OrderDetailRow, id: any) {
        row.ProductID = toId(row.ProductID);

        var sameProduct = this.view.getItems().find(x => x.ProductID === row.ProductID);
        if (sameProduct && this.itemId(sameProduct) !== id) {
            alertDialog('This product is already in order details!');
            return false;
        }

        if (this.connectedMode)
            return true;

        const lookup = await ProductRow.getLookupAsync();
        row.ProductName = lookup.itemById[row.ProductID].ProductName;
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

    declare private _orderId: number;
    
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
