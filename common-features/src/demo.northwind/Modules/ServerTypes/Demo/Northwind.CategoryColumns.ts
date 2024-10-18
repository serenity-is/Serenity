import { ColumnsBase, fieldsProxy } from "@serenity-is/corelib";
import { Column } from "@serenity-is/sleekgrid";
import { CategoryRow } from "./Northwind.CategoryRow";

export interface CategoryColumns {
    CategoryID: Column<CategoryRow>;
    CategoryName: Column<CategoryRow>;
    Description: Column<CategoryRow>;
}

export class CategoryColumns extends ColumnsBase<CategoryRow> {
    static readonly columnsKey = 'Northwind.Category';
    static readonly Fields = fieldsProxy<CategoryColumns>();
}