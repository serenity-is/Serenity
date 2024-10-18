import { Criteria, Decorators, EntityGrid, ListRequest, gridPageInit } from "@serenity-is/corelib";
import { ProductColumns, ProductDialog, ProductRow, ProductService } from "@serenity-is/demo.northwind";

export default () => gridPageInit(GridFilteredByCriteria);

@Decorators.registerClass('Serenity.Demo.BasicSamples.GridFilteredByCriteria')
export class GridFilteredByCriteria<P = {}> extends EntityGrid<ProductRow, P> {

    protected getColumnsKey() { return ProductColumns.columnsKey; }
    protected getDialogType() { return ProductDialog; }
    protected getRowDefinition() { return ProductRow; }
    protected getService() { return ProductService.baseUrl; }

    protected onViewSubmit() {
        // only continue if base class returns true (didn't cancel request)
        if (!super.onViewSubmit()) {
            return false;
        }

        // view object is the data source for grid (SlickRemoteView)
        // this is an EntityGrid so its Params object is a ListRequest
        var request = this.view.params as ListRequest;

        // list request has a Criteria parameter
        // we AND criteria here to existing one because 
        // otherwise we might clear filter set by 
        // an edit filter dialog if any.

        const fld = ProductRow.Fields;
        request.Criteria = Criteria.and(request.Criteria,
            Criteria(fld.UnitsInStock).gt(10),
            Criteria(fld.CategoryName).ne('Condiments'),
            Criteria(fld.Discontinued).eq(0));

        return true;
    }
}