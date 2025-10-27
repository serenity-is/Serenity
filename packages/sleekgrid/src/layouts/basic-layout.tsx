import { FooterRow, Header, HeaderRow, TopPanel, Viewport } from "./layout-components";
import { LayoutEngine } from "./layout-engine";
import type { LayoutHost } from "./layout-host";
import { type GridBandRefs, type GridLayoutRefs } from "./layout-refs";

export class BasicLayout implements LayoutEngine {
    protected canvasWidth: number;
    protected headersWidth: number;
    protected host: LayoutHost;
    protected mainRefs: GridBandRefs;
    protected refs: GridLayoutRefs;

    init(host: LayoutHost) {
        this.host = host;
        const signals = host.getSignals();
        const refs = this.refs = host.refs;
        this.mainRefs = refs.main;

        this.host.getContainerNode().append(<>
            <Header band="main" refs={refs} signals={signals} />
            <HeaderRow band="main" refs={refs} signals={signals} />
            <TopPanel refs={refs} signals={signals} />
            <Viewport band="main" pane="body" refs={refs} />
            <FooterRow band="main" refs={refs} signals={signals} />
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
        this.host = this.refs = this.mainRefs = null;
    }

    public getCanvasWidth(): number {
        return this.canvasWidth;
    }

    public getTopPanel(): HTMLElement {
        return this.refs.topPanel;
    }

    public afterHeaderColumnDrag(): void { }
    public afterRenderRows(): void { }
    public afterSetOptions(): void { }

    readonly layoutName = "basic";
}
