import { EntityGrid, ListResponse, gridPageInit } from "@serenity-is/corelib";
import { SalesByCategoryColumns, SalesByCategoryRow, SalesByCategoryService } from "@serenity-is/demo.northwind";
import { nsDemoBasicSamples } from "../../ServerTypes/Namespaces";


export default () => gridPageInit(ViewWithoutIDGrid)

export class ViewWithoutIDGrid<P = {}> extends EntityGrid<SalesByCategoryRow, P> {
    static override[Symbol.typeInfo] = this.registerClass(nsDemoBasicSamples);

    protected override getColumnsKey() { return SalesByCategoryColumns.columnsKey; }
    protected override getIdProperty() { return "__id"; }
    protected getNameProperty() { return SalesByCategoryRow.nameProperty; }
    protected override getLocalTextPrefix() { return SalesByCategoryRow.localTextPrefix; }
    protected override getService() { return SalesByCategoryService.baseUrl; }

    // this is our autoincrementing counter
    private nextId = 1;

    /**
     * This method is called to preprocess data returned from the list service
     */
    protected override onViewProcessData(response: ListResponse<SalesByCategoryRow>) {
        response = super.onViewProcessData(response);

        // there is no __id property in SalesByCategoryRow but 
        // this is javascript and we can set any property of an object
        for (var x of response.Entities) {
            (x as any).__id = this.nextId++;
        }
        return response;
    }

    protected override getButtons() {
        return [];
    }
}