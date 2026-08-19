import { Column, GridOptions } from "../core";
import { FooterRow, Header, HeaderRow, TopPanel, Viewport } from "./layout-components";
import type { LayoutEngine } from "./layout-engine";
import type { LayoutHost } from "./layout-host";
import type { GridLayoutRefs } from "./layout-refs";

/**
 * Frozen/pinned layout providing pinned columns and frozen top panes.
 * Renders `start`/`main` bands with `top`/`body` panes and handles
 * `frozenRows`/`frozenBottom` and legacy `frozenColumns` options.
 */
export class FrozenLayout implements LayoutEngine {
    /** Host provided during {@link FrozenLayout.init}. */
    private host: LayoutHost;
    /** Refs provided during {@link FrozenLayout.init}. */
    private refs: GridLayoutRefs;

    /**
     * Builds the frozen layout DOM (headers, header rows, viewports, footer rows)
     * across `start`/`main` bands and top/body panes.
     * @param host - Layout host.
     */
    init(host: LayoutHost): void {
        this.host = host;
        this.refs = host.refs;
        const signals = host.getSignals();
        const common = { refs: this.refs, signals };

        host.getContainerNode().append(<>
            <Header band="start" {...common} />
            <Header band="main" {...common} />
            <TopPanel {...common} />
            <HeaderRow band="start" {...common} />
            <HeaderRow band="main" {...common} />
            <Viewport band="start" pane="top" {...common} />
            <Viewport band="main" pane="top" {...common} />
            <Viewport band="start" pane="body" {...common} />
            <Viewport band="main" pane="body" {...common} />
            <FooterRow band="start" {...common} />
            <FooterRow band="main" {...common} />
        </>);

        this.adjustFrozenRowsOption();
    }

    /**
     * Reorders visible columns so that pinned (non-`"end"`) columns come first.
     * Also writes `refs.config.pinnedStartCols` for later layout calculations.
     * @param viewCols - Visible columns in current order.
     * @param refs - Mutable layout refs to update.
     * @returns Reordered columns when pinning exists, `null` otherwise.
     */
    public reorderViewColumns(viewCols: Column[], refs: GridLayoutRefs): Column[] {
        const pinnedStartCols = viewCols.filter(x => x.frozen && x.frozen !== 'end');
        refs.config.pinnedStartCols = pinnedStartCols.length;
        if (pinnedStartCols.length > 0)
            return pinnedStartCols.concat(viewCols.filter(x => !x.frozen || x.frozen === 'end'));
        return null;
    }

    /**
     * Reacts to grid option changes (frozen rows/columns).
     * @param arg - Options delta from `grid.setOptions()`.
     */
    public afterSetOptions(arg: GridOptions): void {
        if (arg.frozenRows != null || arg.frozenBottom != null)
            this.adjustFrozenRowsOption();
        if (arg.frozenColumns != null && arg.columns == null) {
            const columns = this.reorderViewColumns(this.host.getAllColumns(), this.refs);
            if (columns != null)
                arg.columns = columns;
        }
    }

    /**
     * Syncs `refs.config.frozenTopRows` from `frozenRows`/`frozenBottom` grid options.
     */
    public adjustFrozenRowsOption(): void {
        const { frozenRows, frozenBottom } = this.host.getOptions();
        this.refs.config.frozenTopRows = frozenBottom === true ? 0 : (frozenRows ?? 0);
    }

    /**
     * Clears the host reference.
     */
    public destroy(): void {
        this.host = null;
    }

    /** Layout identifier. */
    readonly layoutName = "FrozenLayout";

    /** Indicates this layout supports pinned columns. */
    supportPinnedCols: true = true;
    /** Indicates this layout supports top-frozen rows. */
    supportFrozenRows: true = true;
}
