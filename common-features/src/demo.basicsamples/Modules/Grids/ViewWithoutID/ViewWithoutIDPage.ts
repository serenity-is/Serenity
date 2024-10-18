import { Decorators, EntityGrid, ListResponse, gridPageInit } from "@serenity-is/corelib";
import { SalesByCategoryColumns, SalesByCategoryRow, SalesByCategoryService } from "@serenity-is/demo.northwind";


export default () => gridPageInit(ViewWithoutIDGrid)

@Decorators.registerClass('Serenity.Demo.BasicSamples.ViewWithoutIDGrid')
export class ViewWithoutIDGrid<P = {}> extends EntityGrid<SalesByCategoryRow, P> {
    protected getColumnsKey() { return SalesByCategoryColumns.columnsKey; }
    protected getIdProperty() { return "__id"; }
    protected getNameProperty() { return SalesByCategoryRow.nameProperty; }
    protected getLocalTextPrefix() { return SalesByCategoryRow.localTextPrefix; }
    protected getService() { return SalesByCategoryService.baseUrl; }

    // this is our autoincrementing counter
    private nextId = 1;

    /**
     * This method is called to preprocess data returned from the list service
     */
    protected onViewProcessData(response: ListResponse<SalesByCategoryRow>) {
        response = super.onViewProcessData(response);

        // there is no __id property in SalesByCategoryRow but 
        // this is javascript and we can set any property of an object
        for (var x of response.Entities) {
            (x as any).__id = this.nextId++;
        }
        return response;
    }

    protected getButtons() {
        return [];
    }
}