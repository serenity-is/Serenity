import { Attributes, EntityGrid, EnumEditor, Fluent, LookupEditor, ToolButton, faIcon, toId } from "@serenity-is/corelib";
import { ExcelExportHelper, PdfExportHelper, ReportHelper } from "@serenity-is/extensions";
import { OrderColumns, OrderListRequest, OrderRow, OrderService, ProductRow } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";
import { OrderDialog } from "./OrderDialog";
import "./OrderGrid.css";

export class OrderGrid<P = {}> extends EntityGrid<OrderRow, P> {
    static override[Symbol.typeInfo] = this.registerClass(nsDemoNorthwind, [Attributes.advancedFiltering]);

    protected override getColumnsKey() { return OrderColumns.columnsKey; }
    protected override getDialogType() { return OrderDialog as any; }
    protected override getRowDefinition() { return OrderRow; }
    protected override getService() { return OrderService.baseUrl; }

    declare protected shippingStateFilter: EnumEditor;

    protected override getQuickFilters() {
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

    protected override createQuickFilters() {
        super.createQuickFilters();

        this.shippingStateFilter = this.findQuickFilter(EnumEditor, OrderRow.Fields.ShippingState);
    }

    protected override getButtons(): ToolButton[] {
        var buttons = super.getButtons();

        buttons.push(ExcelExportHelper.createToolButton({
            grid: this,
            service: OrderService.baseUrl + '/ListExcel',
            separator: true
        }));

        buttons.push(PdfExportHelper.createToolButton({
            grid: this
        }));

        return buttons;
    }

    protected override createColumns() {
        var columns = new OrderColumns(super.createColumns());

        columns.PrintInvoice.format = () => <a class="inline-action" data-action="print-invoice" title="invoice">
            <i class={faIcon("file-pdf", "red")}></i></a>;

        return columns.valueOf();
    }

    protected override onClick(e: Event, row: number, cell: number) {
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

    protected override addButtonClick() {
        var eq = this.view.params.EqualityFilter;
        this.editItem({
            CustomerID: eq ? eq.CustomerID : null
        });

    }
}
