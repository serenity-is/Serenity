import { FooterRow, Header, HeaderRow, TopPanel, Viewport } from "./layout-components";
import { LayoutEngine } from "./layout-engine";
import type { LayoutHost } from "./layout-host";
import { type GridLayoutRefs } from "./layout-refs";

export class BasicLayout implements LayoutEngine {
    protected host: LayoutHost;
    protected refs: GridLayoutRefs;

    init(host: LayoutHost) {
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

    public destroy(): void {
        this.host = this.refs = null;
    }

    public afterSetOptions(): void { }

    readonly layoutName = "BasicLayout";
}
