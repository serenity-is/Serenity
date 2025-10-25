import { Column } from "../core";
import { FooterRow, Header, HeaderRow, TopPanel, Viewport } from "./layout-components";
import { LayoutEngine } from "./layout-engine";
import type { LayoutHost } from "./layout-host";
import type { GridLayoutRefs } from "./layout-refs";

export class BasicLayout implements LayoutEngine {
    protected canvasWidth: number;
    protected headersWidth: number;
    protected host: LayoutHost;
    protected bodyRefs: GridLayoutRefs["main"]["body"] = {};
    protected mainRefs: GridLayoutRefs["main"] = { body: this.bodyRefs };
    protected refs: GridLayoutRefs = { main: this.mainRefs, pinnedStartLast: -Infinity, pinnedEndFirst: Infinity, frozenTopLast: -Infinity, frozenBottomFirst: Infinity };

    init(host: LayoutHost) {
        this.host = host;
        const signals = host.getSignals();
        const refs = this.refs;
        this.host.getContainerNode().append(<>
            <Header hband="main" refs={refs} signals={signals} />
            <TopPanel refs={refs} signals={signals} />
            <HeaderRow hband="main" refs={refs} signals={signals} />
            <Viewport hband="main" vband="body" refs={refs} />
            <FooterRow hband="main" refs={refs} signals={signals} />
        </>);
        this.updateHeadersWidth();
    }

    public appendCachedRow(_: number, _rowNodeS: HTMLDivElement, rowNodeC: HTMLDivElement, _rowNodeE: HTMLDivElement): void {
        rowNodeC && this.bodyRefs.canvas.appendChild(rowNodeC);
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

        this.headersWidth = Math.max(this.headersWidth, this.host.getViewportInfo().width);
        this.mainRefs.headerCols.style.width = this.headersWidth + "px";
    }

    public destroy(): void {
        this.host = null;
        this.refs = this.bodyRefs = this.mainRefs = null;
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
    public reorderViewColumns(_: Column[]): Column[] { return null; }
    public setPaneVisibility(): void { }
    public setScroller(): void { }

    readonly layoutName = "basic";
}
