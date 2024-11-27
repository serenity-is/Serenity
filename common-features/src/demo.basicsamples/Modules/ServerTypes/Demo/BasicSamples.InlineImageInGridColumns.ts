import { ColumnsBase, fieldsProxy } from "@serenity-is/corelib";
import { ProductRow } from "@serenity-is/demo.northwind";
import { Column } from "@serenity-is/sleekgrid";
import { InlineImageFormatter } from "../../Grids/InlineImageInGrid/InlineImageInGridPage";

export interface InlineImageInGridColumns {
    ProductID: Column<ProductRow>;
    ProductName: Column<ProductRow>;
    ProductImage: Column<ProductRow>;
    ProductThumbnail: Column<ProductRow>;
}

export class InlineImageInGridColumns extends ColumnsBase<ProductRow> {
    static readonly columnsKey = 'BasicSamples.InlineImageInGrid';
    static readonly Fields = fieldsProxy<InlineImageInGridColumns>();
}

[InlineImageFormatter]; // referenced types