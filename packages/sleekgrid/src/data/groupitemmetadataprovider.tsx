import { applyFormatterResultToCellNode, Column, ColumnFormat, CompatFormatter, convertCompatFormatter, FormatterContext, FormatterResult, Group, IGroupTotals, ItemMetadata, type CellKeyboardEvent, type CellMouseEvent, type GridPlugin, type ISleekGrid } from "../core";

/**
 * Options controlling how {@link GroupItemMetadataProvider} renders group and totals rows.
 */
export interface GroupItemMetadataProviderOptions {
    /** Whether group rows show an expand/collapse toggle and respond to clicks/keys. Defaults to `true`. */
    enableExpandCollapse?: boolean;
    /** CSS class applied to the group cell (the spanned cell). Defaults to `"slick-group-cell"`. */
    groupCellCssClass?: string;
    /** CSS class applied to the entire group row. Defaults to `"slick-group"`. */
    groupCssClass?: string;
    /** Indentation in pixels per grouping level for the toggle. Defaults to `15`. */
    groupIndentation?: number;
    /** Whether group rows can receive focus. Defaults to `true`. */
    groupFocusable?: boolean;
    /** Modern formatter for the group title/aggregated content. */
    groupFormat?: ColumnFormat<Group>;
    /**
     * Legacy formatter for group rows.
     * @deprecated Use {@link GroupItemMetadataProviderOptions.groupFormat} instead.
     */
    groupFormatter?: CompatFormatter<Group>;
    /** CSS class prefix for grouping level (appended with level number). Defaults to `"slick-group-level-"`. */
    groupLevelPrefix?: string;
    /** Whether totals rows should be considered part of the group row span calculation. */
    groupRowTotals?: boolean;
    /** CSS class applied to the title span inside the group cell. Defaults to `"slick-group-title"`. */
    groupTitleCssClass?: string;
    /**
     * Predicate determining whether a column has a summary/aggregate.
     * Used to locate the spanned group cell position.
     * @param column - Column to test.
     * @returns `true` if the column contributes a total/summary.
     */
    hasSummaryType?: (column: Column) => boolean;
    /** CSS class for the expand/collapse toggle element. Defaults to `"slick-group-toggle"`. */
    toggleCssClass?: string;
    /** CSS class added when the toggle represents an expanded group. Defaults to `"expanded"`. */
    toggleExpandedCssClass?: string;
    /** CSS class added when the toggle represents a collapsed group. Defaults to `"collapsed"`. */
    toggleCollapsedCssClass?: string;
    /** CSS class applied to totals rows. Defaults to `"slick-group-totals"`. */
    totalsCssClass?: string;
    /** Whether totals rows can receive focus. Defaults to `false`. */
    totalsFocusable?: boolean;
    /** Modern formatter for totals rows. */
    totalsFormat?: ColumnFormat<IGroupTotals>;
    /**
     * Legacy formatter for totals rows.
     * @deprecated Use {@link GroupItemMetadataProviderOptions.totalsFormat} instead.
     */
    totalsFormatter?: CompatFormatter<IGroupTotals>;
}

/**
 * Grid plugin that provides row metadata and formatters for group headers and
 * group totals rows. Handles expand/collapse UI via click and keyboard
 * (Space, `+`, `-`) and delegates metadata through `getGroupRowMetadata` /
 * `getTotalsRowMetadata` for use by `DataView`.
 */
export class GroupItemMetadataProvider implements GridPlugin {
    /** Host grid instance set during {@link GroupItemMetadataProvider.init}. */
    declare protected grid: ISleekGrid;
    /** Resolved options merged with {@link GroupItemMetadataProvider.defaults}. */
    declare private options: GroupItemMetadataProviderOptions;

    /**
     * Creates a new provider.
     * @param opt - Partial options merged with {@link GroupItemMetadataProvider.defaults}.
     */
    constructor(opt?: GroupItemMetadataProviderOptions) {
        this.options = Object.assign({}, GroupItemMetadataProvider.defaults, opt);
        this.options.groupFormat ??= (opt as any)?.groupFormatter ? convertCompatFormatter((opt as any).groupFormatter) :
            ctx => GroupItemMetadataProvider.defaultGroupFormat(ctx, this.options);
        this.options.totalsFormat ??= (opt as any)?.totalsFormatter ? convertCompatFormatter((opt as any).totalsFormatter) :
            ctx => GroupItemMetadataProvider.defaultTotalsFormat(ctx, this.grid);
    }

    /**
     * Default option values. Override per instance via constructor or {@link GroupItemMetadataProvider.setOptions}.
     */
    public static readonly defaults: GroupItemMetadataProviderOptions = {
        enableExpandCollapse: true,
        groupCellCssClass: "slick-group-cell",
        groupCssClass: "slick-group",
        groupFocusable: true,
        groupIndentation: 15,
        groupLevelPrefix: "slick-group-level-",
        groupTitleCssClass: "slick-group-title",
        hasSummaryType: (col: any) => col.summaryType && col.summaryType != -1,
        totalsCssClass: "slick-group-totals",
        toggleCssClass: "slick-group-toggle",
        toggleCollapsedCssClass: "collapsed",
        toggleExpandedCssClass: "expanded",
        totalsFocusable: false
    }

    /**
     * Default group row formatter. Renders the group title with an optional
     * expand/collapse toggle indented by `group.level`.
     * @param ctx - Formatter context whose `item` is the {@link Group} to render.
     * @param opt - Options controlling indentation and toggle classes; defaults to {@link GroupItemMetadataProvider.defaults}.
     * @returns Rendered group row content as DOM/JSX.
     */
    public static defaultGroupFormat(ctx: FormatterContext, opt?: GroupItemMetadataProviderOptions): FormatterResult {
        // note that grid calls the format function provided via getGroupRowMetadata
        // so the ctx.item is always a Group and value of the group is in item.value, not ctx.value
        // as ctx.value is set by the grid to ctx.item["__groupdisplaycolumnfield__"],
        // so never use or rely on ctx.value here!
        opt ??= GroupItemMetadataProvider.defaults;
        let group = ctx.item as Group;
        let fmtResultTitle: FormatterResult;
        if (group?.formatValue) {
            fmtResultTitle = group.formatValue(ctx);
        }
        else {
            fmtResultTitle = ctx.escape(group?.value);
        }

        if (!opt.enableExpandCollapse) {
            return fmtResultTitle;
        }

        let indentation = group.level * opt.groupIndentation;
        const titleSpan = <span class={opt.groupTitleCssClass} data-level={group.level.toString()}>
            {fmtResultTitle}
        </span> as HTMLElement;
        applyFormatterResultToCellNode(ctx, fmtResultTitle, titleSpan, contentOnly);
        return <>
            <span class={opt.toggleCssClass + " " + (group.collapsed ? opt.toggleCollapsedCssClass : opt.toggleExpandedCssClass)}
                style={{ marginLeft: indentation + "px" }}></span>
            {titleSpan}
        </>
    }

    /**
     * Default totals row formatter. Delegates to the grid's column totals formatter
     * (or the column's own `groupTotalsFormat`/`groupTotalsFormatter`).
     * @param ctx - Formatter context whose `item` is the {@link IGroupTotals} row.
     * @param grid - Optional grid fallback when `ctx.grid` is unavailable.
     * @returns Rendered totals content, or empty string when no formatter is found.
     */
    public static defaultTotalsFormat(ctx: FormatterContext, grid?: ISleekGrid): FormatterResult {
        let item = ctx.item as IGroupTotals;
        if (!item.__groupTotals && (item as any).totals)
            ctx.item = item = (item as any).totals;

        grid = ctx.grid ?? grid;
        const formatter = grid ? grid.getTotalsFormatter(ctx.column) :
            (ctx.column as any)?.groupTotalsFormatter ? convertCompatFormatter((ctx.column as any)?.groupTotalsFormatter) :
                ctx.column.groupTotalsFormat;

        if (formatter)
            return formatter(ctx);

        return "";
    }

    /**
     * Initializes the plugin, attaching click and key handlers for expand/collapse.
     * @param grid - Host grid instance.
     */
    init(grid: ISleekGrid): void {
        this.grid = grid;
        grid.onClick.subscribe(this.handleGridClick);
        grid.onKeyDown.subscribe(this.handleGridKeyDown);
    }

    /** Plugin name used for lookup via `grid.getPluginByName()`. */
    readonly pluginName = "GroupItemMetadataProvider";

    /**
     * Detaches event handlers added during {@link GroupItemMetadataProvider.init}.
     */
    destroy(): void {
        if (this.grid) {
            this.grid.onClick?.unsubscribe(this.handleGridClick);
            this.grid.onKeyDown?.unsubscribe(this.handleGridKeyDown);
        }
    }

    /**
     * Returns the current resolved options.
     * @returns Current options object.
     */
    getOptions(): GroupItemMetadataProviderOptions {
        return this.options;
    }

    /**
     * Merges the given values into the current options.
     * @param value - Partial options to apply.
     */
    setOptions(value: GroupItemMetadataProviderOptions): void {
        Object.assign(this.options, value);
    }

    /**
     * Click handler that toggles group collapse when the toggle element is clicked.
     * @param e - Cell mouse event from the grid's `onClick`.
     */
    handleGridClick = (e: CellMouseEvent): void => {
        let grid = e?.grid ?? this.grid;
        if (!grid)
            return;
        var item = grid.getDataItem(e.row);
        if (!item ||
            !(item instanceof Group) ||
            !this.options.toggleCssClass ||
            !(e.target as HTMLElement).classList.contains(this.options.toggleCssClass))
            return;

        e.stopImmediatePropagation();
        e.preventDefault();

        var range = grid.getRenderedRange();
        grid.getData().setRefreshHints?.({
            ignoreDiffsBefore: range.top,
            ignoreDiffsAfter: range.bottom + 1
        });

        if (item.collapsed)
            grid.getData().expandGroup?.(item.groupingKey);
        else
            grid.getData().collapseGroup?.(item.groupingKey);
    }

    /**
     * Key handler that toggles group collapse on Space / `+` / `-` when a group row is active.
     * @param e - Cell keyboard event from the grid's `onKeyDown`.
     */
    handleGridKeyDown = (e: CellKeyboardEvent): void => {
        if (!this.options.enableExpandCollapse ||
            (e.key !== " " && e.key !== "-" && e.key !== "+"))
            return;

        let grid = e?.grid ?? this.grid;
        if (!grid)
            return;

        var activeCell = grid.getActiveCell();
        if (!activeCell)
            return;

        var item = grid.getDataItem(activeCell.row);
        if (!item || !(item instanceof Group))
            return;

        e.stopImmediatePropagation();
        e.preventDefault();

        if ((e.key == "+" && !item.collapsed) ||
            (e.key == "-" && item.collapsed))
            return;

        var range = (grid.getRenderedRange as any)();
        grid.getData().setRefreshHints?.({
            ignoreDiffsBefore: range.top,
            ignoreDiffsAfter: range.bottom + 1
        });

        if (item.collapsed)
            grid.getData().expandGroup?.(item.groupingKey);
        else
            grid.getData().collapseGroup?.(item.groupingKey);
    }

    /**
     * Computes the cell index and colspan for the spanned group cell, taking
     * summary columns and frozen columns into account.
     * @returns Object with `cell` start index and `colspan` span width (`"*"` means full row when no totals).
     */
    groupCellPosition = (): {
        cell: number;
        colspan: (number | "*");
    } => {

        const result = {
            cell: 0,
            colspan: "*" as (number | "*")
        }

        if (!this.options.groupRowTotals ||
            !this.grid) {
            return result;
        }

        var cols = this.grid.getColumns();
        var col1: Column;
        for (var idx = 0; idx < cols.length; idx++) {
            col1 = cols[idx];
            if (!this.options.hasSummaryType?.(cols[idx])) {
                result.cell = idx;
                break;
            }
        }

        result.colspan = 0;
        for (var idx = result.cell + 1; idx < cols.length; idx++) {
            var col2 = cols[idx];
            if (!this.options.hasSummaryType?.(col2) &&
                (!!(col1?.frozen) === !!(col2?.frozen))) {
                result.colspan++;
            }
            else
                break;
        }

        result.colspan = Math.max(1, result.colspan);
        return result;
    }

    /**
     * Returns row metadata for a group header row. The grid/DataView calls this
     * to obtain CSS classes, focusability and the spanned column formatter.
     * @param item - Group row item.
     * @returns Metadata describing how the group row should be rendered.
     */
    getGroupRowMetadata: ((item: Group) => ItemMetadata) = (item) => {

        const opt = this.options;
        const gcp = this.groupCellPosition();
        const result: ItemMetadata = {
            selectable: false,
            focusable: opt.groupFocusable,
            cssClasses: opt.groupCssClass + " " + opt.groupLevelPrefix + item?.level,
            columns: {
                [gcp.cell]: {
                    colspan: gcp.colspan,
                    cssClasses: opt.groupCellCssClass,
                    format: opt.groupFormat,
                    editor: null
                }
            }
        };

        if (opt.groupRowTotals)
            result.format = opt.totalsFormat;

        return result;
    }

    /**
     * Returns row metadata for a group totals row.
     * @param item - Totals row item.
     * @returns Metadata describing how the totals row should be rendered.
     */
    getTotalsRowMetadata: ((item: IGroupTotals) => ItemMetadata) = (item) => {
        const opt = this.options;
        return {
            selectable: false,
            focusable: opt.totalsFocusable,
            cssClasses: opt.totalsCssClass + " " + opt.groupLevelPrefix + item?.group?.level,
            format: opt.totalsFormat,
            editor: null
        };
    }

}

const contentOnly = {
    contentOnly: true
}
