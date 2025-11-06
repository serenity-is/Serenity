import { Aggregators, EntityGrid, formatNumber, gridPageInit } from "@serenity-is/corelib";
import { ProductColumns, ProductDialog, ProductRow, ProductService } from "@serenity-is/demo.northwind";
import { GroupItemMetadataProvider } from "@serenity-is/sleekgrid";
import { nsDemoBasicSamples } from "../../ServerTypes/Namespaces";

export default () => gridPageInit(GroupingAndSummariesInGrid);

export class GroupingAndSummariesInGrid<P = {}> extends EntityGrid<ProductRow, P> {
    static override[Symbol.typeInfo] = this.registerClass(nsDemoBasicSamples);

    protected override getColumnsKey() { return ProductColumns.columnsKey; }
    protected override getDialogType() { return ProductDialog; }
    protected override getRowDefinition() { return ProductRow; }
    protected override getService() { return ProductService.baseUrl; }

    protected override initSleekGrid() {
        super.initSleekGrid();

        // need to register this plugin for grouping or you'll have errors
        this.sleekGrid.registerPlugin(new GroupItemMetadataProvider());

        this.view.setSummaryOptions({
            aggregators: [
                new Aggregators.Avg('UnitPrice'),
                new Aggregators.Sum('UnitsInStock'),
                new Aggregators.Max('UnitsOnOrder'),
                new Aggregators.Avg('ReorderLevel')
            ]
        });
    }

    protected override createColumns() {
        var columns = new ProductColumns(super.createColumns());

        columns.UnitsOnOrder && (columns.UnitsOnOrder.groupTotalsFormat = (ctx) =>
            (ctx.item.max ? ('max: ' + ctx.escape(ctx.item.max[columns.UnitsOnOrder.field] ?? '')) : ''));

        columns.ReorderLevel && (columns.ReorderLevel.groupTotalsFormat = (ctx) =>
            (ctx.item.avg ? ('avg: ' + ctx.escape(formatNumber(ctx.item.avg[columns.ReorderLevel.field], '0.') ?? '')) : ''));

        return columns.valueOf();
    }

    protected override getSlickOptions() {
        var opt = super.getSlickOptions();
        opt.showFooterRow = true;
        return opt;
    }

    protected override usePager() {
        return false;
    }

    protected override getButtons() {
        return [{
            title: 'Group By Category',
            separator: true,
            cssClass: 'expand-all-button',
            onClick: () => this.view.setGrouping(
                [{
                    getter: 'CategoryName',
                    format: ctx => `Category: ${ctx.escape(ctx.item.value)} (${ctx.escape(ctx.item.count)} items)`,
                }])
        },
        {
            title: 'Group By Category and Supplier',
            separator: true,
            cssClass: 'expand-all-button',
            onClick: () => this.view.setGrouping(
                [{
                    format: ctx => `Category: ${ctx.escape(ctx.item.value)} (${ctx.escape(ctx.item.count)} items)`,
                    getter: 'CategoryName'
                }, {
                    format: ctx => `Supplier: ${ctx.escape(ctx.item.value)} (${ctx.escape(ctx.item.count)} items)`,
                    getter: 'SupplierCompanyName'
                }])
        }, {
            title: 'No Grouping',
            separator: true,
            cssClass: 'collapse-all-button',
            onClick: () => this.view.setGrouping([])
        }];
    }
}