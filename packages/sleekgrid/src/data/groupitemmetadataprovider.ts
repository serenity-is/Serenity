import { Column, ColumnFormat, CompatFormatter, convertCompatFormatter, FormatterContext, FormatterResult, Group, IGroupTotals, ItemMetadata } from "../core";
import { ArgsCell, Grid, IPlugin } from "../grid";

export interface GroupItemMetadataProviderOptions {
    enableExpandCollapse?: boolean;
    groupCellCssClass?: string;
    groupCssClass?: string;
    groupIndentation?: number;
    groupFocusable?: boolean;
    groupFormat?: ColumnFormat<Group>;
    /** @deprecated see @use groupFormat */
    groupFormatter?: CompatFormatter<Group>;
    groupLevelPrefix?: string;
    groupRowTotals?: boolean;
    groupTitleCssClass?: string;
    hasSummaryType?: (column: Column) => boolean;
    toggleCssClass?: string;
    toggleExpandedCssClass?: string;
    toggleCollapsedCssClass?: string;
    totalsCssClass?: string;
    totalsFocusable?: boolean;
    totalsFormat?: ColumnFormat<IGroupTotals>;
    /** @deprecated see @use totalsFormat */
    totalsFormatter?: CompatFormatter<IGroupTotals>;
}

export class GroupItemMetadataProvider implements IPlugin {
    declare protected grid: Grid;
    declare private options: GroupItemMetadataProviderOptions;

    constructor(opt?: GroupItemMetadataProviderOptions) {
        this.options = Object.assign({}, GroupItemMetadataProvider.defaults, opt);
        this.options.groupFormat ??= (opt as any)?.groupFormatter ? convertCompatFormatter((opt as any).groupFormatter) :
            ctx => GroupItemMetadataProvider.defaultGroupFormat(ctx, this.options);
        this.options.totalsFormat ??= (opt as any)?.totalsFormatter ? convertCompatFormatter((opt as any).totalsFormatter) :
            ctx => GroupItemMetadataProvider.defaultTotalsFormat(ctx, this.grid);
    }

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

    public static defaultGroupFormat(ctx: FormatterContext, opt?: GroupItemMetadataProviderOptions) {
        opt ??= GroupItemMetadataProvider.defaults;
        let item = ctx.item as Group;
        if (!opt.enableExpandCollapse)
            return item?.title;
        let indentation = item.level * opt.groupIndentation;
        const span = document.createElement("span");
        span.className = opt.toggleCssClass + " " + (item.collapsed ? opt.toggleCollapsedCssClass : opt.toggleExpandedCssClass);
        span.style.marginLeft = indentation + "px";
        const titleSpan = document.createElement("span");
        titleSpan.className = opt.groupTitleCssClass;
        titleSpan.setAttribute("level", item.level.toString());
        titleSpan.textContent = item.title ?? "";
        span.appendChild(titleSpan);
        return span;
    }

    public static defaultTotalsFormat(ctx: FormatterContext, grid?: Grid): FormatterResult {
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

    init(grid: Grid) {
        this.grid = grid;
        grid.onClick.subscribe(this.handleGridClick);
        grid.onKeyDown.subscribe(this.handleGridKeyDown);
    }

    readonly pluginName = "GroupItemMetadataProvider";

    destroy() {
        if (this.grid) {
            this.grid.onClick?.unsubscribe(this.handleGridClick);
            this.grid.onKeyDown?.unsubscribe(this.handleGridKeyDown);
        }
    }

    getOptions() {
        return this.options;
    }

    setOptions(value: GroupItemMetadataProviderOptions) {
        Object.assign(this.options, value);
    }

    handleGridClick = (e: MouseEvent, args: ArgsCell) => {
        let grid = args?.grid ?? this.grid;
        if (!grid)
            return;
        var item = grid.getDataItem(args.row);
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

    handleGridKeyDown = (e: KeyboardEvent, args: ArgsCell) => {
        if (!this.options.enableExpandCollapse ||
            (e.key !== " " && e.key !== "-" && e.key !== "+"))
            return;

        let grid = args?.grid ?? this.grid;
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

    groupCellPosition = () => {

        const result = {
            cell: 0,
            colspan: <number | "*">"*"
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
