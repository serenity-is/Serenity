import { Column } from "@serenity-is/sleekgrid";
import { Authorization, faIcon, isPromiseLike, tryGetText, type PropertyItem } from "../../base";
import { FormatterType } from "../../types/formattertype";
import { FormatterTypeRegistry } from "../../types/formattertyperegistry";
import { IInitializeColumn } from "../formatters/iinitializecolumn";
import { GridRowSelectionMixin } from "./gridrowselectionmixin";
import { UtilityColumns } from "./utilitycolumns";
import { ReflectionOptionsSetter } from "../widgets/reflectionoptionssetter";

/**
 * Converts {@link PropertyItem} definitions into sleek grid column definitions.
 */
export namespace PropertyItemColumnConverter {

    /**
     * Converts an array of property items into column definitions.
     * @param items - The property items to convert.
     * @returns The resulting column definitions.
     */
    export function toColumns(items: PropertyItem[]): Column[] {
        const result: Column[] = [];
        if (items == null) {
            return result;
        }
        for (let i = 0; i < items.length; i++) {
            result.push(PropertyItemColumnConverter.toColumn(items[i]));
        }
        return result;
    }

    /**
     * Converts a single property item into a column definition.
     * @param item - The property item to convert.
     * @returns The resulting column definition.
     */
    export function toColumn(item: PropertyItem): Column {
        const isAlwaysHidden = item.filterOnly === true ||
            (item.readPermission != null && !Authorization.hasPermission(item.readPermission));

        const result: Column = {
            field: item.unbound ? null : item.name,
            id: item.unbound ? item.name : null,
            sourceItem: item,
            cssClass: item.cssClass,
            focusable: item.focusable !== false,
            headerCssClass: item.headerCssClass,
            sortable: item.sortable !== false,
            selectable: item.showSelection !== false,
            tabbable: item.tabbable !== false,
            sortOrder: item.sortOrder ?? 0,
            width: item.width != null ? item.width : 80,
            summaryType: item.summaryType,
            minWidth: item.minWidth ?? 30,
            maxWidth: (item.maxWidth == null || item.maxWidth === 0) ? null : item.maxWidth,
            resizable: item.resizable == null || !!item.resizable,
            togglable: (isAlwaysHidden || item.allowHide === false) ? false : void 0,
            visible: !isAlwaysHidden && item.visible !== false
        };

        let name = tryGetText(item.title);
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

        if (item.pin != null && item.pin !== false) {
            result.frozen = item.pin === "end" ? "end" : "start";
        }

        // the column of the checkboxes that select the rows is not a data column, so its cell, its header
        // and its group cell are set by the mixin, e.g. its select all checkbox, and the server only tells
        // that it is there, with the RowSelectionColumn attribute of the columns class
        if (item.name === "RowSelection") {
            const template = GridRowSelectionMixin.createSelectColumn();
            return {
                ...template,
                width: result.width,
                minWidth: result.minWidth,
                maxWidth: result.maxWidth,
                visible: result.visible,
                frozen: result.frozen,
                sourceItem: item,
                name: result.name ?? template.name
            };
        }

        // the column of the buttons of the row actions is not a data column either: the server tells where it
        // is, with the RowActionsColumn attribute of the columns class, and the RowActionsMixin of
        // Serenity.Pro.Extensions renders its buttons, finding the column by its id
        if (item.name === "RowActions") {
            return {
                id: UtilityColumns.actionsId,
                name: "",
                field: null,
                width: result.width,
                minWidth: result.minWidth,
                maxWidth: result.maxWidth,
                visible: result.visible,
                frozen: result.frozen,
                sortable: false,
                resizable: false,
                groupable: false,
                focusable: true,
                selectable: false,
                skipGroupCell: true,
                cssClass: "s-row-actions-cell",
                headerCssClass: "s-row-actions-header",
                sourceItem: item,
                format: () => ""
            };
        }

        if (!item.formatterType)
            return result;

        const formatterType = (isPromiseLike(item.formatterType) || typeof item.formatterType === "function")
            ? item.formatterType : (FormatterTypeRegistry.getOrLoad(item.formatterType));

        let loadError: string;

        const then = (formatterType: FormatterType) => {
            const formatter = new formatterType(item.formatterParams ?? {});

            if (item.formatterParams != null) {
                ReflectionOptionsSetter.set(formatter, item.formatterParams);
            }

            (formatter as unknown as IInitializeColumn)?.initializeColumn?.(result);

            result.format = (ctx) => formatter.format(ctx);
        }
        if (isPromiseLike(formatterType)) {

            result.format = (ctx) => {
                if (loadError != null) {
                    return formatLoadError(loadError);
                }

                if (ctx.row != null && ctx.cell != null && ctx.grid) {
                    const grid = ctx.grid;
                    const row = ctx.row;
                    const cell = ctx.cell;
                    formatterType.then(() => {
                        grid.updateCell?.(row, cell);
                    }, () => {
                        grid.updateCell?.(row, cell);
                    });
                }
                return "";
            }

            formatterType.then(then, (err) => {
                loadError = "Failed to load formatter type: " + String(item.formatterType);
                result.format = formatLoadError.bind(null, loadError);
                console.error(loadError, err);
            });
        }
        else {
            then(formatterType);
        }

        return result;
    }

    function formatLoadError(error: string) {
        return <span class="s-FormatterLoadError" title={error}>
            <i class={faIcon("exclamation-triangle", "warning")} /></span>;            
    }
}
