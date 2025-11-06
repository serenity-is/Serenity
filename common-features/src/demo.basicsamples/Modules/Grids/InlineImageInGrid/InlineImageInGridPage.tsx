import { EntityGrid, Formatter, IInitializeColumn, formatterTypeInfo, gridPageInit, registerType, resolveUrl } from "@serenity-is/corelib";
import { ProductDialog, ProductRow, ProductService } from "@serenity-is/demo.northwind";
import { Column, FormatterContext, FormatterResult, GridOptions } from "@serenity-is/sleekgrid";
import { InlineImageInGridColumns } from "../../ServerTypes/Demo";
import { nsDemoBasicSamples } from "../../ServerTypes/Namespaces";

export default () => gridPageInit(InlineImageInGrid);

export class InlineImageFormatter implements Formatter, IInitializeColumn {
    static [Symbol.typeInfo] = formatterTypeInfo(nsDemoBasicSamples); static { registerType(this); }

    constructor(public readonly props: { fileProperty?: string, thumb?: boolean } = {}) {
    }

    format(ctx: FormatterContext): FormatterResult {

        var file = (this.props?.fileProperty ? ctx.item[this.props.fileProperty] : ctx.value) as string;
        if (!file || !file.length)
            return "";

        if (this.props?.thumb) {
            var parts = file.split('.');
            file = parts.slice(0, parts.length - 1).join('.') + '_t.jpg';
        }

        return <a class="inline-image" target="_blank" href={resolveUrl("~/upload/" + file)}>
            <img src={resolveUrl('~/upload/' + file)} style="max-height: 145px; max-width: 100%" /></a>;
    }

    initializeColumn(column: Column): void {
        if (this.props?.fileProperty) {
            column.referencedFields = column.referencedFields || [];
            column.referencedFields.push(this.props.fileProperty);
        }
    }
}

export class InlineImageInGrid<P = {}> extends EntityGrid<ProductRow, P> {
    static override[Symbol.typeInfo] = this.registerClass(nsDemoBasicSamples);

    protected override getColumnsKey() { return InlineImageInGridColumns.columnsKey; }
    protected override getDialogType() { return ProductDialog; }
    protected override getRowDefinition() { return ProductRow; }
    protected override getService() { return ProductService.baseUrl; }

    protected override getSlickOptions(): GridOptions {
        let opt = super.getSlickOptions();
        opt.rowHeight = 150;
        return opt;
    }
}