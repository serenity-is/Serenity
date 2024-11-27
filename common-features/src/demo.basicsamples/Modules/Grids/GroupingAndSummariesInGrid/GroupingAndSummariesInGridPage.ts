import { Aggregators, Decorators, EntityGrid, formatNumber, gridPageInit } from "@serenity-is/corelib";
import { ProductColumns, ProductDialog, ProductRow, ProductService } from "@serenity-is/demo.northwind";
import { GroupItemMetadataProvider } from "@serenity-is/sleekgrid";

export default () => gridPageInit(GroupingAndSummariesInGrid);

@Decorators.registerClass('Serenity.Demo.BasicSamples.GroupingAndSummariesInGrid')
export class GroupingAndSummariesInGrid<P = {}> extends EntityGrid<ProductRow, P> {

    protected getColumnsKey() { return ProductColumns.columnsKey; }
    protected getDialogType() { return ProductDialog; }
    protected getRowDefinition() { return ProductRow; }
    protected getService() { return ProductService.baseUrl; }

    protected createSlickGrid() {
        var grid = super.createSlickGrid();

        // need to register this plugin for grouping or you'll have errors
        grid.registerPlugin(new GroupItemMetadataProvider());

        this.view.setSummaryOptions({
            aggregators: [
                new Aggregators.Avg('UnitPrice'),
                new Aggregators.Sum('UnitsInStock'),
                new Aggregators.Max('UnitsOnOrder'),
                new Aggregators.Avg('ReorderLevel')
            ]
        });

        return grid;
    }

    protected getColumns() {
        var columns = new ProductColumns(super.getColumns());

        columns.UnitsOnOrder && (columns.UnitsOnOrder.groupTotalsFormatter = (totals, col) =>
            (totals.max ? ('max: ' + (totals.max[col.field] ?? '')) : ''));

        columns.ReorderLevel && (columns.ReorderLevel.groupTotalsFormatter = (totals, col) =>
            (totals.avg ? ('avg: ' + (formatNumber(totals.avg[col.field], '0.') ?? '')) : ''));

        return columns.valueOf();
    }

    protected getSlickOptions() {
        var opt = super.getSlickOptions();
        opt.showFooterRow = true;
        return opt;
    }

    protected usePager() {
        return false;
    }

    protected getButtons() {
        return [{
            title: 'Group By Category',
            separator: true,
            cssClass: 'expand-all-button',
            onClick: () => this.view.setGrouping(
                [{
                    getter: 'CategoryName'
                }])
        },
        {
            title: 'Group By Category and Supplier',
            separator: true,
            cssClass: 'expand-all-button',
            onClick: () => this.view.setGrouping(
                [{
                    formatter: x => 'Category: ' + x.value + ' (' + x.count + ' items)',
                    getter: 'CategoryName'
                }, {
                    formatter: x => 'Supplier: ' + x.value + ' (' + x.count + ' items)',
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