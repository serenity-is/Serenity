import type { GridPlugin, HeaderColumnEvent, ISleekGrid } from "../core";

/**
 * Options for {@link AutoTooltips}.
 */
export interface AutoTooltipsOptions {
    /** Auto-assign tooltips for body cells when text overflows. Defaults to `true`. */
    enableForCells?: boolean;
    /** Auto-assign tooltips for header cells when text overflows. Defaults to `false`. */
    enableForHeaderCells?: boolean;
    /** Maximum tooltip length before truncation with `"..."`; `null` means no limit. */
    maxToolTipLength?: number;
    /** When `true`, overwrites existing `title` attributes. */
    replaceExisting?: boolean;
}

/**
 * Grid plugin that automatically sets `title` tooltips for truncated cell content.
 * Handles overflow detection via `clientWidth < scrollWidth` and optional truncation.
 */
export class AutoTooltips implements GridPlugin {

    /** Host grid set during {@link AutoTooltips.init}. */
    declare private grid: ISleekGrid;
    /** Resolved options merged with {@link AutoTooltips.defaults}. */
    declare private options: AutoTooltipsOptions;

    /**
     * Creates the plugin.
     * @param options - Partial options merged with {@link AutoTooltips.defaults}.
     */
    constructor(options?: AutoTooltipsOptions) {
        this.options = Object.assign({}, AutoTooltips.defaults, options);
    }

    /** Default option values. */
    public static readonly defaults: AutoTooltipsOptions = {
        enableForCells: true,
        enableForHeaderCells: false,
        maxToolTipLength: null,
        replaceExisting: true
    }

    /**
     * Attaches overflow-tooltip handlers based on current options.
     * @param grid - Host grid instance.
     */
    init(grid: ISleekGrid): void {
        this.grid = grid;

        if (this.options.enableForCells)
            this.grid.onMouseEnter.subscribe(this.handleMouseEnter);

        if (this.options.enableForHeaderCells)
            this.grid.onHeaderMouseEnter.subscribe(this.handleHeaderMouseEnter);
    }

    /**
     * Detaches handlers installed by {@link AutoTooltips.init}.
     */
    destroy(): void {
        if (this.options.enableForCells)
            this.grid.onMouseEnter.unsubscribe(this.handleMouseEnter);

        if (this.options.enableForHeaderCells)
            this.grid.onHeaderMouseEnter.unsubscribe(this.handleHeaderMouseEnter);
    }

    private handleMouseEnter = (e: MouseEvent) => {
        var cell = this.grid.getCellFromEvent(e);
        if (!cell)
            return;
        var node = this.grid.getCellNode(cell.row, cell.cell);
        if (!node)
            return;
        var text;
        if (!node.title || this.options.replaceExisting) {
            if (node.clientWidth < node.scrollWidth) {
                text = node.textContent?.trim() ?? "";
                if (this.options.maxToolTipLength &&
                    text.length > this.options.maxToolTipLength) {
                    text = text.substring(0, this.options.maxToolTipLength - 3) + "...";
                }
            } else {
                text = "";
            }
            node.title = text;
        }
        node = null;
    }

    private handleHeaderMouseEnter = (e: HeaderColumnEvent) => {
        var column = e.column;
        if (column && !column.toolTip) {
            var node = (e.target as HTMLElement).closest(".slick-header-column") as HTMLElement;
            node && (node.title = (node.clientWidth < node.scrollWidth ? (typeof column.name === "string" ? column.name : "") : ""));
        }
    }

    /** Plugin name for lookup via `grid.getPluginByName()`. */
    public pluginName = "AutoTooltips";
}
