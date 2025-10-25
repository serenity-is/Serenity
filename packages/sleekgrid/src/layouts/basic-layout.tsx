import { FooterRow, Header, HeaderRow, TopPanel, Viewport } from "./layout-components";
import { LayoutEngine } from "./layout-engine";
import type { LayoutHost } from "./layout-host";
import { type GridLayoutHRefs, type GridLayoutRefs, type ViewportPaneRefs } from "./layout-refs";

export class BasicLayout implements LayoutEngine {
    protected canvasWidth: number;
    protected headersWidth: number;
    protected host: LayoutHost;
    protected bodyRefs: ViewportPaneRefs;
    protected mainRefs: GridLayoutHRefs;
    protected refs: GridLayoutRefs;

    init(host: LayoutHost) {
        this.host = host;
        const signals = host.getSignals();
        const refs = this.refs = host.refs;
        this.bodyRefs = (this.mainRefs = refs.main).body;

        this.host.getContainerNode().append(<>
            <Header hband="main" refs={refs} signals={signals} />
            <TopPanel refs={refs} signals={signals} />
            <HeaderRow hband="main" refs={refs} signals={signals} />
            <Viewport hband="main" vband="body" refs={refs} />
            <FooterRow hband="main" refs={refs} signals={signals} />
        </>);
        this.updateHeadersWidth();
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
        this.host = this.refs = this.mainRefs = this.bodyRefs = null;
    }

    public getCanvasWidth(): number {
        return this.canvasWidth;
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

    readonly layoutName = "basic";
}
