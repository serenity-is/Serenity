import { EntityGrid } from "@serenity-is/corelib";
import { CategoryColumns, CategoryRow, CategoryService } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";
import { CategoryDialog } from "./CategoryDialog";

export class CategoryGrid<P = {}> extends EntityGrid<CategoryRow, P> {
    static override[Symbol.typeInfo] = this.registerClass(nsDemoNorthwind);

    protected override getColumnsKey() { return CategoryColumns.columnsKey; }
    protected override getDialogType() { return <any>CategoryDialog; }
    protected override getRowDefinition() { return CategoryRow; }
    protected override getService() { return CategoryService.baseUrl; }
}
