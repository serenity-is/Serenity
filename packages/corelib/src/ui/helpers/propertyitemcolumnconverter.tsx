import { Column } from "@serenity-is/sleekgrid";
import { Authorization, isPromiseLike, SummaryType, tryGetText, type PropertyItem } from "../../base";
import { FormatterType } from "../../types/formattertype";
import { FormatterTypeRegistry } from "../../types/formattertyperegistry";
import { IInitializeColumn } from "../formatters/iinitializecolumn";
import { ReflectionOptionsSetter } from "../widgets/reflectionoptionssetter";

export namespace PropertyItemColumnConverter {

    export function toColumns(items: PropertyItem[]): Column[] {
        var result: Column[] = [];
        if (items == null) {
            return result;
        }
        for (var i = 0; i < items.length; i++) {
            result.push(PropertyItemColumnConverter.toColumn(items[i]));
        }
        return result;
    }

    export function toColumn(item: PropertyItem): Column {
        const isAlwaysHidden = item.filterOnly === true ||
            (item.readPermission != null && !Authorization.hasPermission(item.readPermission));

        var result: Column = {
            field: item.unbound ? null : item.name,
            id: item.unbound ? item.name : null,
            sourceItem: item,
            cssClass: item.cssClass,
            headerCssClass: item.headerCssClass,
            sortable: item.sortable !== false,
            sortOrder: item.sortOrder ?? 0,
            width: item.width != null ? item.width : 80,
            summaryType: item.summaryType,
            minWidth: item.minWidth ?? 30,
            maxWidth: (item.maxWidth == null || item.maxWidth === 0) ? null : item.maxWidth,
            resizable: item.resizable == null || !!item.resizable,
            togglable: (isAlwaysHidden || item.allowHide === false) ? false : void 0,
            visible: !isAlwaysHidden && item.visible !== false
        };

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

        if (item.pin != null && item.pin as any !== false) {
            result.frozen = item.pin === "end" ? "end" : "start";
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
                if (ctx.row != null && ctx.cell != null && ctx.grid) {
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
