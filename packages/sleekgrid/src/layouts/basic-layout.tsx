import { FooterRow, Header, HeaderRow, TopPanel, Viewport } from "./layout-components";
import { LayoutEngine } from "./layout-engine";
import type { LayoutHost } from "./layout-host";
import { type GridLayoutRefs } from "./layout-refs";

/**
 * Default single-pane layout. Renders header, header row, top panel, body
 * viewport and footer row in the main band without pinning or frozen panes.
 */
export class BasicLayout implements LayoutEngine {
    /** Host provided during {@link BasicLayout.init}. */
    protected host: LayoutHost;
    /** Refs snapshot provided during {@link BasicLayout.init}. */
    protected refs: GridLayoutRefs;

    /**
     * Builds the basic layout DOM inside `host.getContainerNode()`.
     * @param host - Layout host.
     */
    init(host: LayoutHost): void {
        this.host = host;
        const signals = host.getSignals();
        const refs = this.refs = host.refs;
        const common = { refs, signals };

        this.host.getContainerNode().append(<>
            <Header band="main" {...common} />
            <HeaderRow band="main" {...common} />
            <TopPanel {...common} />
            <Viewport band="main" pane="body" {...common} />
            <FooterRow band="main" {...common} />
        </>);
    }

    /**
     * Clears host and refs references.
     */
    public destroy(): void {
        this.host = this.refs = null;
    }

    /**
     * No-op for the basic layout; options require no layout-specific handling.
     */
    public afterSetOptions(): void { }

    /** Layout identifier. */
    readonly layoutName = "BasicLayout";
}
