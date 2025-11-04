import type { GridPlugin, HeaderColumnEvent, ISleekGrid } from "../core";

export interface AutoTooltipsOptions {
    enableForCells?: boolean;
    enableForHeaderCells?: boolean;
    maxToolTipLength?: number;
    replaceExisting?: boolean;
}

export class AutoTooltips implements GridPlugin {

    declare private grid: ISleekGrid;
    declare private options: AutoTooltipsOptions;

    constructor(options?: AutoTooltipsOptions) {
        this.options = Object.assign({}, AutoTooltips.defaults, options);
    }

    public static readonly defaults: AutoTooltipsOptions = {
        enableForCells: true,
        enableForHeaderCells: false,
        maxToolTipLength: null,
        replaceExisting: true
    }

    init(grid: ISleekGrid) {
        this.grid = grid;

        if (this.options.enableForCells)
            this.grid.onMouseEnter.subscribe(this.handleMouseEnter);

        if (this.options.enableForHeaderCells)
            this.grid.onHeaderMouseEnter.subscribe(this.handleHeaderMouseEnter);
    }

    destroy() {
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

    public pluginName = "AutoTooltips";
}
