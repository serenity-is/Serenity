import { applyFormatterResultToCellNode, Column, ColumnFormat, CompatFormatter, convertCompatFormatter, FormatterContext, FormatterResult, Group, IGroupTotals, ItemMetadata, type CellKeyboardEvent, type CellMouseEvent, type GridPlugin, type IDataView, type ISleekGrid } from "../core";

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
    /**
     * Whether the first cell of the data rows inside a group is indented according to the grouping
     * level, so that they look nested under the group rows. Defaults to `true`.
     *
     * The indentation goes to the column the group cell starts at, see {@link groupCellPosition},
     * which the provider can only find when it knows the grid and so its columns, i.e. when it is
     * registered as a plugin of the grid, which also makes the view use the same instance.
     */
    indentDataRows?: boolean;
    /** CSS class applied to the indented first cell of grouped data rows. Defaults to `"slick-group-data"`. */
    dataCellCssClass?: string;
    /**
     * Columns that have a fixed width, i.e. the same `minWidth` and `maxWidth`, and that are
     * narrower than this many pixels are skipped when finding the group cell column, like the
     * columns marked with `skipGroupCell`. It finds the narrow utility columns, e.g. a checkbox
     * column of 27px, without needing a flag. The declared width is used, not the current one, so
     * `forceFitColumns` and resizing don't move the group cell column. Set it to `0` to disable it,
     * and set `skipGroupCell` on a column to skip a column that is not fixed.
     * Defaults to `40`.
     */
    minGroupCellWidth?: number;
    /**
     * Indentation in pixels per grouping level. Defaults to `15`.
     * @deprecated It is not used to emit an inline style anymore, as that doesn't work with a
     * content security policy that disallows inline styles. The indentation is now applied by
     * the `slick-group-level-N` classes that the provider puts on group rows, toggles and data
     * cells, see `--sg-group-indent` in `css/slick.grid.css`. Set that CSS variable to change
     * the step. This option is kept for compatibility.
     */
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
     * The deepest grouping level that has an indentation rule in `css/slick.grid.css`, which
     * defines the rules for levels 1 to 8, as CSS can't multiply a custom property by a level
     * that is only known at runtime. Deeper levels are clamped to it. If you need more levels,
     * add the matching `.slick-group-level-N` rules to your own stylesheet and raise this value.
     */
    public static maxIndentLevel = 8;

    /**
     * Clamps a grouping level to {@link GroupItemMetadataProvider.maxIndentLevel}, so that a level
     * that has no CSS rule still gets the deepest indentation instead of none. Level 0, the top
     * level groups, is not indented, and has no CSS rule.
     * @param level - The grouping level.
     * @returns The level, between 0 and {@link GroupItemMetadataProvider.maxIndentLevel}.
     */
    public static clampIndentLevel(level: number): number {
        return Math.max(0, Math.min(level ?? 0, GroupItemMetadataProvider.maxIndentLevel));
    }

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
        dataCellCssClass: "slick-group-data",
        groupCellCssClass: "slick-group-cell",
        groupCssClass: "slick-group",
        groupFocusable: true,
        groupIndentation: 15,
        groupLevelPrefix: "slick-group-level-",
        groupTitleCssClass: "slick-group-title",
        hasSummaryType: (col: any) => col.summaryType && col.summaryType != -1,
        indentDataRows: true,
        minGroupCellWidth: 40,
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
        const group = ctx.item as Group;
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

        // the indentation is applied by the level class, and not by an inline style, so that it
        // works with a content security policy that disallows inline styles
        const level = GroupItemMetadataProvider.clampIndentLevel(group.level);
        const toggleCssClass = [opt.toggleCssClass,
            group.collapsed ? opt.toggleCollapsedCssClass : opt.toggleExpandedCssClass,
            opt.groupLevelPrefix ? opt.groupLevelPrefix + level : null
        ].filter(x => !!x).join(" ");

        const titleSpan = <span class={opt.groupTitleCssClass} data-level={group.level.toString()}>
            {fmtResultTitle}
        </span> as HTMLElement;
        applyFormatterResultToCellNode(ctx, fmtResultTitle, titleSpan, contentOnly);
        return <>
            <span class={toggleCssClass}></span>
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
     * Sets the grid the provider reads the columns from, e.g. to find the column a group cell starts
     * at, without attaching the event handlers of {@link init}. A view that has a grid attached, like
     * `VirtualScrollView`, calls it; a provider registered with `registerPlugin` gets the grid from
     * {@link init}.
     * @param grid - The grid that shows the rows the provider gives metadata for.
     */
    public setGrid(grid: ISleekGrid): void {
        this.grid = grid;
    }

    /**
     * Initializes the plugin, attaching click and key handlers for expand/collapse.
     * @param grid - Host grid instance.
     */
    init(grid: ISleekGrid): void {
        this.setGrid(grid);
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
        const grid = e?.grid ?? this.grid;
        if (!grid)
            return;
        const item = grid.getDataItem(e.row);
        if (!item ||
            !(item instanceof Group) ||
            !this.options.toggleCssClass ||
            !(e.target as HTMLElement).classList.contains(this.options.toggleCssClass))
            return;

        e.stopImmediatePropagation();
        e.preventDefault();

        const range = grid.getRenderedRange();
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
     * Returns the first and the last cell of a row that can be navigated to, ignoring the columns a
     * cell spans. The arrow keys are only used for the group at these ends of the row, as they don't
     * go to another cell there.
     * @param grid - The grid that shows the row.
     * @param row - The row to find the cells of.
     * @returns The first and the last navigable cell, both `null` when there is none.
     */
    protected getFocusableCellRange(grid: ISleekGrid, row: number): { first: number, last: number } {
        const colCount = grid.getColumns?.()?.length ?? 0;
        let cell = 0, first: number = null, last: number = null;

        while (cell < colCount) {
            if (grid.canCellBeActive?.(row, cell)) {
                if (first == null)
                    first = cell;
                last = cell;
            }

            cell += Math.max(1, (grid.getColspan?.(row, cell) as number) || 1);
        }

        return { first, last };
    }

    /**
     * Key handler that toggles group collapse on Space / `+` / `-` when the group cell is active, and
     * on the arrow keys at the ends of a group row, e.g. Left collapses the group on the first cell of
     * its row and Right expands it on the last one.
     * @param e - Cell keyboard event from the grid's `onKeyDown`.
     */
    handleGridKeyDown = (e: CellKeyboardEvent): void => {
        if (!this.options.enableExpandCollapse)
            return;

        const key = e?.key;
        const isLeft = key === "ArrowLeft";
        const isRight = key === "ArrowRight";
        const isArrow = isLeft || isRight;
        if (!isArrow && key !== " " && key !== "-" && key !== "+")
            return;

        const grid = e?.grid ?? this.grid;
        if (!grid)
            return;

        const activeCell = grid.getActiveCell();
        if (!activeCell)
            return;

        const item = grid.getDataItem(activeCell.row);
        if (!item || !(item instanceof Group))
            return;

        // the state the key asks for, which is the opposite of the current one for the space key
        const collapse: boolean = key === " " ? !item.collapsed : key === "-" || isLeft;

        if (isArrow) {
            // the arrows belong to the group only at the ends of the cells of its row that can be
            // navigated to, where they would not go to another cell, and only when they change its
            // state, otherwise they are left to the grid
            const range = this.getFocusableCellRange(grid, activeCell.row);
            if (range.first == null)
                return;

            // the columns are mirrored in an rtl grid, so the ends are swapped as well
            const rtl = !!grid.getOptions?.()?.rtl;
            const atStart = activeCell.cell === (rtl ? range.last : range.first);
            const atEnd = activeCell.cell === (rtl ? range.first : range.last);

            if (collapse ? !atStart : !atEnd)
                return;

            if (collapse === !!item.collapsed)
                return;
        }
        else {
            // the other keys belong to the cell the group is in, and not to e.g. a checkbox cell
            // before it, which handles them itself
            if (activeCell.cell !== this.groupCellPosition().cell)
                return;
        }

        // the keys are ours from here, even when `+` or `-` is pressed on a group that is already in
        // the state it asks for
        e.stopImmediatePropagation();
        e.preventDefault();

        if (collapse === !!item.collapsed)
            return;

        const range = grid.getRenderedRange?.();
        if (range) {
            grid.getData().setRefreshHints?.({
                ignoreDiffsBefore: range.top,
                ignoreDiffsAfter: range.bottom + 1
            });
        }

        if (collapse)
            grid.getData().collapseGroup?.(item.groupingKey);
        else
            grid.getData().expandGroup?.(item.groupingKey);
    }

    /**
     * Returns where the totals of a group of the given level are shown, which the view that groups
     * the rows knows: on a row of their own, on the row of the group itself, or not at all. It
     * defaults to a row of their own when the view doesn't tell, like the grouping defaults of a view.
     * @param level - The level of the grouping, `0` when not given.
     * @returns `true` for a totals row, `"groupheader"` for the row of the group, `false` for none.
     */
    public getTotalsDisplay(level?: number): boolean | "groupheader" {
        const grouping = (this.grid?.getData() as IDataView)?.getGrouping?.();
        return grouping?.[level ?? 0]?.displayTotalsRow ?? true;
    }

    /**
     * Returns whether a column can be the start of a group cell, and so the column the data rows
     * of a group are indented in. Summary columns are skipped, as they show the totals of a group,
     * and so are the columns marked with `skipGroupCell` and the narrow columns with a fixed width
     * (see {@link GroupItemMetadataProviderOptions.minGroupCellWidth}), like a checkbox column,
     * which stay in front of the group cell.
     * @param column - The column to test.
     * @returns `true` if the group cell can start at the column.
     */
    public isGroupCellColumn(column: Column): boolean {
        return !!column &&
            !this.isSkippedColumn(column) &&
            !this.options.hasSummaryType?.(column);
    }

    /**
     * Returns whether a column stays in front of the group cell, i.e. it is marked with
     * `skipGroupCell`, or it is a narrow column with a fixed width, like a checkbox or a row number
     * column. Such columns keep their own cell in the rows of a group, where the group cell, and the
     * indentation of the data rows, start after them.
     * @param column - The column to test.
     * @returns `true` if the group cell starts after the column.
     */
    protected isSkippedColumn(column: Column): boolean {
        return !!column?.skipGroupCell || this.isFixedNarrowColumn(column);
    }

    /**
     * Returns whether a column has a fixed width narrower than
     * {@link GroupItemMetadataProviderOptions.minGroupCellWidth}, which marks a utility column
     * like a checkbox or a row number column. The declared width is used, as the current one
     * changes with `forceFitColumns` and resizing, which would move the group cell column.
     *
     * A width of `0` is treated as not set, like the column sizing code does, e.g. the fitting of
     * the columns uses `maxWidth || Infinity`, so a column with a zero minimum and maximum is a
     * normal column that can be resized and is not skipped.
     * @param column - The column to test.
     * @returns `true` if the column is a narrow column with a fixed width.
     */
    protected isFixedNarrowColumn(column: Column): boolean {
        const limit = this.options.minGroupCellWidth;
        if (!(limit > 0))
            return false;

        const minWidth = column?.minWidth;
        const maxWidth = column?.maxWidth;

        if (typeof minWidth !== "number" ||
            typeof maxWidth !== "number" ||
            !(minWidth > 0) ||
            minWidth !== maxWidth) {
            return false;
        }

        return maxWidth < limit;
    }

    /**
     * Computes the cell index and colspan for the spanned group cell, taking summary columns, columns
     * marked with `skipGroupCell`, narrow fixed width columns and frozen columns into account.
     * @param level - The level of the grouping, which decides where the totals of a group are shown.
     * @returns Object with `cell` start index and `colspan` span width (`"*"` when the totals are not
     * on the row of the group, so that the group cell spans the rest of the row).
     */
    groupCellPosition = (level?: number): {
        cell: number;
        colspan: (number | "*");
    } => {

        const result = {
            cell: 0,
            colspan: "*" as (number | "*")
        }

        const cols = this.grid?.getColumns?.();
        if (!cols?.length) {
            return result;
        }

        // the totals of a group are in the summary columns before the group cell, which then spans
        // only through the columns that are not summaries; when the totals are on a row of their
        // own, the group cell covers the summary columns as well
        const totalsOnGroupRow = this.getTotalsDisplay(level) === "groupheader";

        // the group cell starts at the first column that is not skipped, so that the columns like a
        // checkbox stay visible in the rows of a group too
        let cell = -1;
        for (let idx = 0; idx < cols.length; idx++) {
            const column = cols[idx];
            if (!this.isSkippedColumn(column) &&
                (!totalsOnGroupRow || !this.options.hasSummaryType?.(column))) {
                cell = idx;
                break;
            }
        }

        if (cell < 0) {
            // every column is a skipped column, so there is nothing to span
            return result;
        }

        result.cell = cell;

        if (!totalsOnGroupRow) {
            return result;
        }

        // the span starts with the group cell itself, and grows through the columns after it that
        // the group cell also covers, e.g. stops at the summary columns
        const col1 = cols[cell];
        result.colspan = 1;
        for (let idx = cell + 1; idx < cols.length; idx++) {
            const col2 = cols[idx];
            if (this.isGroupCellColumn(col2) &&
                (!!(col1?.frozen) === !!(col2?.frozen))) {
                result.colspan++;
            }
            else
                break;
        }

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
        const gcp = this.groupCellPosition(item?.level);
        const result: ItemMetadata = {
            selectable: false,
            focusable: opt.groupFocusable,
            cssClasses: opt.groupCssClass + " " + opt.groupLevelPrefix + item?.level,
            columns: {
                [gcp.cell]: {
                    colspan: gcp.colspan,
                    cssClass: opt.groupCellCssClass,
                    format: opt.groupFormat,
                    editor: null
                }
            }
        };

        if (this.getTotalsDisplay(item?.level) === "groupheader")
            result.format = opt.totalsFormat;

        return result;
    }

    /**
     * Returns row metadata for a data row that is inside one or more groups. Views that
     * support grouping call this for their data rows and merge the result with their own
     * metadata (see {@link mergeMetadata}).
     *
     * When {@link GroupItemMetadataProviderOptions.indentDataRows} is `true`, the first cell
     * gets the level class (e.g. `slick-group-level-2`) and `css/slick.grid.css` indents it by
     * `level * var(--sg-group-indent)`, so the data rows of nested groups line up under their
     * group rows. No inline style is used, so that it works with a content security policy
     * that disallows inline styles.
     *
     * @param row - The row index of the data row in the view.
     * @param level - The grouping level of the data row. Data rows are normally at the leaf
     * level, i.e. the number of active groupings, so two levels of grouping indent them by
     * two steps.
     * @param baseMetadata - Metadata the view has already built for the row, e.g. from its
     * `getItemMetadata` callback. It is merged with the indentation metadata when the row is
     * indented, and returned as is otherwise.
     * @returns Metadata for the row, or `null` when there is none and the row is not indented.
     */
    public getDataRowMetadata(row: number, level: number, baseMetadata?: ItemMetadata): ItemMetadata {
        const opt = this.options;

        if (!opt.indentDataRows ||
            level == null ||
            !(level > 0))
            return baseMetadata ?? null;

        // the indentation is applied by the level class, and not by an inline style, so that it
        // works with a content security policy that disallows inline styles
        const cssClasses = [opt.dataCellCssClass,
            opt.groupLevelPrefix ? opt.groupLevelPrefix + GroupItemMetadataProvider.clampIndentLevel(level) : null
        ].filter(x => !!x).join(" ");

        // the indentation goes to the column the group cell starts at, so that the data rows of
        // the groups line up under their group rows
        return GroupItemMetadataProvider.mergeMetadata(baseMetadata, {
            columns: {
                [this.groupCellPosition(level).cell]: {
                    cssClass: cssClasses
                }
            }
        });
    }

    /**
     * Merges metadata returned for a row, e.g. the metadata a view or a column provider has
     * already built and the metadata {@link getDataRowMetadata} returns, into a single object.
     * CSS classes are concatenated, other properties of `extra` win.
     * @param base - The metadata built by the view or a user callback, if any.
     * @param extra - The metadata to apply on top of it, if any.
     * @returns The merged metadata, or `null` when both arguments are empty.
     */
    public static mergeMetadata(base: ItemMetadata, extra: ItemMetadata): ItemMetadata {
        if (!extra)
            return base ?? null;

        if (!base)
            return extra;

        const result: ItemMetadata = { ...base, ...extra };

        const cssClasses = [base.cssClasses, extra.cssClasses].filter(x => !!x).join(" ");
        if (cssClasses.length)
            result.cssClasses = cssClasses;

        if (extra.columns) {
            const columns: { [key: string]: any } = { ...(base.columns ?? {}) };
            for (const key in extra.columns) {
                const baseCol = columns[key];
                const extraCol = extra.columns[key];
                if (!baseCol) {
                    columns[key] = extraCol;
                    continue;
                }

                const merged: any = { ...baseCol, ...extraCol };
                const classes = [baseCol.cssClass ?? baseCol.cssClasses, extraCol.cssClass ?? extraCol.cssClasses]
                    .filter(x => !!x).join(" ");
                if (classes.length)
                    merged.cssClass = classes;
                delete merged.cssClasses;

                columns[key] = merged;
            }

            result.columns = columns;
        }

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
