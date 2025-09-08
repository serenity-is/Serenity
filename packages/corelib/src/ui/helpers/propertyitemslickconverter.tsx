import { Column, Grid } from "@serenity-is/sleekgrid";
import { Authorization, isPromiseLike, tryGetText, type PropertyItem } from "../../base";
import { FormatterType } from "../../types/formattertype";
import { FormatterTypeRegistry } from "../../types/formattertyperegistry";
import { IInitializeColumn } from "../formatters/formatters";
import { ReflectionOptionsSetter } from "../widgets/reflectionoptionssetter";

export namespace PropertyItemSlickConverter {

    export function toSlickColumns(items: PropertyItem[]): Column[] {
        var result: Column[] = [];
        if (items == null) {
            return result;
        }
        for (var i = 0; i < items.length; i++) {
            result.push(PropertyItemSlickConverter.toSlickColumn(items[i]));
        }
        return result;
    }

    export function toSlickColumn(item: PropertyItem): Column {
        var result: Column = {
            field: item.name,
            sourceItem: item,
            cssClass: item.cssClass,
            headerCssClass: item.headerCssClass,
            sortable: item.sortable !== false,
            sortOrder: item.sortOrder ?? 0,
            width: item.width != null ? item.width : 80,
            minWidth: item.minWidth ?? 30,
            maxWidth: (item.maxWidth == null || item.maxWidth === 0) ? null : item.maxWidth,
            resizable: item.resizable == null || !!item.resizable
        };

        result.visible = item.visible !== false && item.filterOnly !== true &&
            (item.readPermission == null || Authorization.hasPermission(item.readPermission));

        var name = tryGetText(item.title);
        if (name == null)
            name = item.title;
        result.name = name;

        if (item.alignment != null && item.alignment.length > 0) {
            if (result.cssClass) {
                result.cssClass += ' align-' + item.alignment;
            }
            else {
                result.cssClass = 'align-' + item.alignment;
            }
        }

        if (!item.formatterType)
            return result;

        const formatterType = (isPromiseLike(item.formatterType) || typeof item.formatterType === "function")
            ? item.formatterType : (FormatterTypeRegistry.getOrLoad(item.formatterType));

        const then = (formatterType: FormatterType) => {
            var formatter = new formatterType(item.formatterParams ?? {});

            if (item.formatterParams != null) {
                ReflectionOptionsSetter.set(formatter, item.formatterParams);
            }

            (formatter as unknown as IInitializeColumn)?.initializeColumn?.(result);

            result.format = (ctx) => formatter.format(ctx);
        }
        if (isPromiseLike(formatterType)) {
            result.format = (ctx) => {
                if (ctx.row != null && ctx.cell != null && ctx.grid instanceof Grid) {
                    const grid = ctx.grid;
                    const row = ctx.row;
                    const cell = ctx.cell;
                    formatterType.then(() => {
                        grid.updateCell?.(row, cell);
                    });
                }
                return "";
            }
            formatterType.then(then);
        }
        else {
            then(formatterType);
        }

        return result;
    }
}
