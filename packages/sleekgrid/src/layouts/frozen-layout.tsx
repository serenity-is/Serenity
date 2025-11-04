import { Column, GridOptions } from "../core";
import { FooterRow, Header, HeaderRow, TopPanel, Viewport } from "./layout-components";
import type { LayoutEngine } from "./layout-engine";
import type { LayoutHost } from "./layout-host";
import type { GridLayoutRefs } from "./layout-refs";

export class FrozenLayout implements LayoutEngine {
    private host: LayoutHost;
    private refs: GridLayoutRefs;

    init(host: LayoutHost) {
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

    public reorderViewColumns(viewCols: Column[], refs: GridLayoutRefs): Column[] {
        const pinnedStartCols = viewCols.filter(x => x.frozen && x.frozen !== 'end');
        refs.pinnedStartLast = pinnedStartCols.length > 0 ? pinnedStartCols.length - 1 : -Infinity;
        if (pinnedStartCols.length > 0)
            return pinnedStartCols.concat(viewCols.filter(x => !x.frozen));
        return null;
    }

    public afterSetOptions(arg: GridOptions) {
        if (arg.frozenRows != null || arg.frozenBottom != null)
            this.adjustFrozenRowsOption();
        if (arg.frozenColumns != null && arg.columns == null) {
            const columns = this.reorderViewColumns(this.host.getColumns(true), this.refs);
            if (columns != null)
                arg.columns = columns;
        }
    }

    public adjustFrozenRowsOption(): void {
        const { autoHeight, frozenRows, frozenBottom } = this.host.getOptions();
        let frozenTopLast = -Infinity;
        if (!autoHeight) {
            let availRows = this.host.getViewportInfo().numVisibleRows;
            let frozenTopRows = frozenBottom === true ? 0 : (frozenRows ?? 0);
            frozenTopRows = (frozenTopRows > 0 && frozenRows <= availRows) ? frozenTopRows : 0;
            frozenTopLast = frozenTopRows > 0 ? frozenTopRows - 1 : -Infinity;
        }
        this.refs.frozenTopLast = frozenTopLast;
    }

    public destroy(): void {
        this.host = null;
    }

    readonly layoutName = "FrozenLayout";
}
