import { Column, GridOptions } from "../core";
import { FooterRow, Header, HeaderRow, TopPanel, Viewport } from "./layout-components";
import type { LayoutEngine } from "./layout-engine";
import type { LayoutHost } from "./layout-host";
import type { GridBandRefs, GridLayoutRefs } from "./layout-refs";

export const FrozenLayout: { new(): LayoutEngine } = function (): LayoutEngine {
    var host: LayoutHost;
    let startRefs: GridBandRefs
    let mainRefs: GridBandRefs
    let refs: GridLayoutRefs;

    function init(hostGrid: LayoutHost) {
        host = hostGrid;
        refs = host.refs;
        startRefs = refs.start;
        mainRefs = refs.main;
        const signals = host.getSignals();
        const common = { refs, signals};

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

        adjustFrozenRowsOption();
    }

    function reorderViewColumns(viewCols: Column[], refs: GridLayoutRefs): Column[] {
        const pinnedStartCols = viewCols.filter(x => x.frozen && x.frozen !== 'end');
        refs.pinnedStartLast = pinnedStartCols.length - 1;
        if (pinnedStartCols.length > 0)
            return pinnedStartCols.concat(viewCols.filter(x => !x.frozen));
        return null;
    }

    function afterSetOptions(arg: GridOptions) {
        if (arg.frozenRows != null || arg.frozenBottom != null)
            adjustFrozenRowsOption();
        if (arg.frozenColumns != null && arg.columns == null) {
            const columns = reorderViewColumns(host.getInitialColumns(), refs);
            if (columns != null)
                arg.columns = columns;
        }
    }

    function adjustFrozenRowsOption(): void {
        const options = host.getOptions();
        //if (options.autoHeight) {
        //    frozenRows = 0;
        //    return;
        //}
//
        //frozenRows = (options.frozenRows > 0 && options.frozenRows <= host.getViewportInfo().numVisibleRows) ? options.frozenRows : 0;
//
        //if (frozenRows) {
        //    frozenRowIdx = options.frozenBottom ? (host.getDataLength() - frozenRows) : (frozenRows - 1);
        //}
    }

    function destroy(): void {
        host = startRefs = mainRefs = null;
    }

    return {
        afterSetOptions,
        destroy,
        init,
        layoutName: "frozen",
        reorderViewColumns
    }
} as any;

