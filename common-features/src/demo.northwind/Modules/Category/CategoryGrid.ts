import { CategoryColumns, CategoryRow, CategoryService } from "../ServerTypes/Demo";
import { Decorators, EntityGrid } from "@serenity-is/corelib";
import { CategoryDialog } from "./CategoryDialog";

@Decorators.registerClass('Serenity.Demo.Northwind.CategoryGrid')
export class CategoryGrid<P = {}> extends EntityGrid<CategoryRow, P> {
    protected getColumnsKey() { return CategoryColumns.columnsKey; }
    protected getDialogType() { return <any>CategoryDialog; }
    protected getRowDefinition() { return CategoryRow; }
    protected getService() { return CategoryService.baseUrl; }
}
