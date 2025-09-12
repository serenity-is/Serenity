import { EntityGrid } from "@serenity-is/corelib";
import { CategoryColumns, CategoryRow, CategoryService } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";
import { CategoryDialog } from "./CategoryDialog";

export class CategoryGrid<P = {}> extends EntityGrid<CategoryRow, P> {
    static override typeInfo = this.registerClass(nsDemoNorthwind);

    protected getColumnsKey() { return CategoryColumns.columnsKey; }
    protected getDialogType() { return <any>CategoryDialog; }
    protected getRowDefinition() { return CategoryRow; }
    protected getService() { return CategoryService.baseUrl; }
}
