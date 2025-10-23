import { currentLifecycleRoot } from "@serenity-is/sleekdom";
import { Column } from "../core";
import { LayoutEngine } from "./layout-engine";
import type { LayoutHost } from "./layout-host";
import type { GridLayoutRefs } from "./layout-refs";

export class BasicLayout implements LayoutEngine {
    protected canvasWidth: number;
    protected headersWidth: number;
    protected host: LayoutHost;
    protected bodyRefs: GridLayoutRefs["main"]["body"] = {};
    protected mainRefs: GridLayoutRefs["main"] = { body: this.bodyRefs };
    protected refs: GridLayoutRefs = { main: this.mainRefs };

    init(hostGrid: LayoutHost) {
        this.host = hostGrid;
        const optSignals = this.host.getSignals();
        const prevLifecycleRoot = currentLifecycleRoot(this.host.getContainerNode());
        const mainRefs = this.mainRefs;
        const bodyRefs = this.bodyRefs;
        try {
            this.host.getContainerNode().append(<>
                <div class={{ "slick-header": true, "slick-hidden": optSignals.hideColumnHeader }}>
                    <div class="slick-header-columns" ref={el => mainRefs.headerCols = el} />
                </div>
                <div class={{ "slick-headerrow": true, "slick-hidden": optSignals.hideHeaderRow }}>
                    <div class="slick-headerrow-columns" ref={el => mainRefs.headerRowCols = el} />
                </div>
                <div class={{ "slick-top-panel-container": true, "slick-hidden": optSignals.hideTopPanel }}>
                    <div class="slick-top-panel" ref={el => mainRefs.topPanel = el} />
                </div>
                <div class="slick-viewport" tabindex="0" ref={el => bodyRefs.viewport = el}>
                    <div class="grid-canvas" tabindex="0" ref={el => bodyRefs.canvas = el} />
                </div>
                <div class={{ "slick-footerrow": true, "slick-hidden": optSignals.hideFooterRow }}>
                    <div class="slick-footerrow-columns" ref={el => mainRefs.footerRowCols = el} />
                </div>
            </>);

            this.updateHeadersWidth();
        }
        finally {
            currentLifecycleRoot(prevLifecycleRoot);
        }
    }

    public appendCachedRow(_: number, _rowNodeS: HTMLDivElement, rowNodeC: HTMLDivElement, _rowNodeE: HTMLDivElement): void {
        rowNodeC && this.bodyRefs.canvas.appendChild(rowNodeC);
    }

    public applyColumnWidths(): void {
        var x = 0, w, rule, cols = this.host.getColumns(), opts = this.host.getOptions(), rtl = opts.rtl;

        if (opts.useCssVars) {
            var styles = this.host.getContainerNode().style;
            for (var i = 0; i < cols.length; i++) {
                w = cols[i].width;
                var prop = "--l" + i;
                var oldVal = styles.getPropertyValue(prop);
                var newVal = x + "px";
                if (oldVal !== newVal)
                    styles.setProperty(prop, newVal);
                prop = "--r" + i;
                oldVal = styles.getPropertyValue(prop);
                newVal = (this.canvasWidth - x - w) + "px"
                if (oldVal !== newVal)
                    styles.setProperty(prop, newVal);
                x += w;
            }
        }
        else {
            for (var i = 0; i < cols.length; i++) {
                w = cols[i].width;
                rule = this.host.getColumnCssRules(i);
                rule[rtl ? "right" : "left"].style[rtl ? "right" : "left"] = x + "px";
                rule[rtl ? "left" : "right"].style[rtl ? "left" : "right"] = (this.canvasWidth - x - w) + "px";
                x += w;
            }
        }
    }

    public calcCanvasWidth(): number {
        var cols = this.host.getColumns(), i = cols.length;
        var rowWidth = 0;
        while (i--) {
            rowWidth += cols[i].width;
        }

        return this.host.getOptions().fullWidthRows ? Math.max(rowWidth,
            this.host.getAvailableWidth()) : rowWidth;
    }

    public updateHeadersWidth(): void {
        this.headersWidth = 0;

        var scrollWidth = this.host.getScrollDims().width;
        var cols = this.host.getColumns();
        for (var i = 0, ii = cols.length; i < ii; i++) {
            this.headersWidth += cols[i].width;
        }

        this.headersWidth += scrollWidth;
        this.headersWidth = Math.max(this.headersWidth, this.host.getViewportInfo().width) + 1000;
        this.mainRefs.headerCols.style.width = this.headersWidth + "px";
    }

    public destroy(): void {
        this.host = null;
        this.refs = this.bodyRefs = this.mainRefs = null;
    }

    public getCanvasNodeFor(cell: number, row: number): HTMLElement {
        return this.bodyRefs.canvas;
    }

    public getCanvasNodes(): HTMLElement[] {
        return [this.bodyRefs.canvas];
    }

    public getCanvasWidth(): number {
        return this.canvasWidth;
    }

    public getHeaderColumn(cell: number): HTMLElement {
        return this.mainRefs.headerCols.children.item(cell) as HTMLElement;
    }

    public getHeaderRowColumn(cell: number): HTMLElement {
        return this.mainRefs.headerRowCols?.childNodes.item(cell) as HTMLElement;
    }

    public getHeaderRowColsFor(): HTMLElement {
        return this.mainRefs.headerRowCols;
    }

    public getFooterRowColumn(cell: number): HTMLElement {
        return this.mainRefs.footerRowCols?.childNodes.item(cell) as HTMLElement;
    }

    public getFooterRowColsFor(): HTMLElement {
        return this.mainRefs.footerRowCols;
    }

    public getHeaderColsFor(): HTMLElement {
        return this.mainRefs.headerCols;
    }

    public getRefs(): GridLayoutRefs {
        return this.refs;
    }

    public getRowFromCellNode(cellNode: HTMLElement): number {
        return this.host.getRowFromNode(cellNode.parentElement);
    }

    public getTopPanel(): HTMLElement {
        return this.mainRefs.topPanel;
    }

    public getViewportNodeFor(_row: number, _cell: number): HTMLElement {
        return this.bodyRefs.viewport;
    }

    public handleScrollH(): void {
        this.mainRefs.headerCols && (this.mainRefs.headerCols.parentElement.scrollLeft = this.host.getScrollLeft());
        this.mainRefs.topPanel && (this.mainRefs.topPanel.parentElement.scrollLeft = this.host.getScrollLeft());
        this.mainRefs.headerRowCols && (this.mainRefs.headerRowCols.parentElement.scrollLeft = this.host.getScrollLeft());
        this.mainRefs.footerRowCols && (this.mainRefs.footerRowCols.parentElement.scrollLeft = this.host.getScrollLeft());
    }

    public realScrollHeightChange(): void {
        this.bodyRefs.canvas.style.height = this.host.getViewportInfo().realScrollHeight + "px"
    }


    public setOverflow(): void {
        var alwaysVS = this.host.getOptions().alwaysShowVerticalScroll;

        this.bodyRefs.viewport.style.overflowX = "auto";
        this.bodyRefs.viewport.style.overflowY = alwaysVS ? "scroll" : (this.host.getOptions().autoHeight ? "hidden" : "auto");
    }

    public updateCanvasWidth(): boolean {
        var oldCanvasWidth = this.canvasWidth;
        this.canvasWidth = this.calcCanvasWidth();
        var scrollWidth = this.host.getScrollDims().width;

        const vpi = this.host.getViewportInfo();
        var canvasWidthPx = this.canvasWidth + "px"
        this.bodyRefs.canvas.style.width = canvasWidthPx;
        this.mainRefs.headerRowCols && (this.mainRefs.headerRowCols.style.width = canvasWidthPx);
        this.mainRefs.footerRowCols && (this.mainRefs.footerRowCols.style.width = canvasWidthPx);
        this.updateHeadersWidth();
        vpi.hasHScroll = (this.canvasWidth > this.host.getViewportInfo().width - scrollWidth);

        return this.canvasWidth != oldCanvasWidth;
    }

    public resizeCanvas(): void {
        var vs = this.host.getViewportInfo();
        const options = this.host.getOptions();
        if (options.autoHeight) {
            const totalHeight = vs.groupingPanelHeight + vs.topPanelHeight + vs.headerRowHeight + vs.footerRowHeight +
                vs.headerHeight + "px";

            this.host.getContainerNode().style.height = totalHeight;
        }
        else
            this.bodyRefs.viewport.style.height = vs.height + "px";
    }

    public afterHeaderColumnDrag(): void { }
    public afterRenderRows(): void { }
    public afterSetOptions(): void { }
    public beforeCleanupAndRenderCells(): void { }
    public getFrozenBottomFirstRow(): number { return Infinity; }
    public getFrozenRowOffset(): number { return 0; }
    public getFrozenTopLastRow(): number { return -1; }
    public getPinnedEndFirstCol(): number { return Infinity; }
    public getPinnedStartLastCol(): number { return -1; }
    public handleScrollV(): void { }
    public isFrozenRow(): boolean { return false; }
    public reorderViewColumns(_: Column[]): Column[] { return null;}
    public setPaneVisibility(): void { }
    public setScroller(): void { }

    readonly layoutName = "basic";
}
