import { EntityGrid, EnumEditor, FilterableAttribute, Fluent, LookupEditor, ToolButton, faIcon, toId } from "@serenity-is/corelib";
import { ExcelExportHelper, PdfExportHelper, ReportHelper } from "@serenity-is/extensions";
import { OrderColumns, OrderListRequest, OrderRow, OrderService, ProductRow } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";
import { OrderDialog } from "./OrderDialog";
import "./OrderGrid.css";

export class OrderGrid<P = {}> extends EntityGrid<OrderRow, P> {
    static [Symbol.typeInfo] = this.registerClass(nsDemoNorthwind, [new FilterableAttribute()]);

    protected getColumnsKey() { return OrderColumns.columnsKey; }
    protected getDialogType() { return OrderDialog as any; }
    protected getRowDefinition() { return OrderRow; }
    protected getService() { return OrderService.baseUrl; }

    declare protected shippingStateFilter: EnumEditor;

    protected getQuickFilters() {
        var filters = super.getQuickFilters();

        filters.push({
            type: LookupEditor,
            options: {
                lookupKey: ProductRow.lookupKey,
                async: true
            },
            field: 'ProductID',
            title: 'Contains Product in Details',
            handler: w => {
                (this.view.params as OrderListRequest).ProductID = toId(w.value);
            },
            cssClass: 'hidden-xs'
        });

        return filters;
    }

    protected createQuickFilters() {
        super.createQuickFilters();

        this.shippingStateFilter = this.findQuickFilter(EnumEditor, OrderRow.Fields.ShippingState);
    }

    protected getButtons(): ToolButton[] {
        var buttons = super.getButtons();

        buttons.push(ExcelExportHelper.createToolButton({
            grid: this,
            service: OrderService.baseUrl + '/ListExcel',
            onViewSubmit: () => this.onViewSubmit(),
            separator: true
        }));

        buttons.push(PdfExportHelper.createToolButton({
            grid: this,
            onViewSubmit: () => this.onViewSubmit()
        }));

        return buttons;
    }

    protected getColumns() {
        var columns = new OrderColumns(super.getColumns());

        columns.PrintInvoice.format = () => <a class="inline-action" data-action="print-invoice" title="invoice">
            <i class={faIcon("file-pdf", "red")}></i></a>;

        return columns.valueOf();
    }

    protected onClick(e: Event, row: number, cell: number) {
        super.onClick(e, row, cell);

        if (Fluent.isDefaultPrevented(e))
            return;

        var item = this.itemAt(row);
        let action = (e.target as HTMLElement)?.closest(".inline-action")?.getAttribute("data-action");
        if (action) {
            e.preventDefault();
            if (action == "print-invoice") {
                ReportHelper.execute({
                    reportKey: 'Northwind.OrderDetail',
                    params: {
                        OrderID: item.OrderID
                    }
                });
            }
        }
    }

    public set_shippingState(value: number): void {
        this.shippingStateFilter.value = value == null ? '' : value.toString();
    }

    protected addButtonClick() {
        var eq = this.view.params.EqualityFilter;
        this.editItem({
            CustomerID: eq ? eq.CustomerID : null
        });

    }
}
