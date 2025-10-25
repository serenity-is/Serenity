import { Column, GridOptions, ViewRange } from "../core";
import { BodyPane, BottomPane, HeaderPane, TopPane, TopPanel } from "./layout-components";
import type { LayoutEngine } from "./layout-engine";
import type { LayoutHost } from "./layout-host";
import type { GridLayoutHRefs, GridLayoutRefs } from "./layout-refs";

export const FrozenLayout: { new(): LayoutEngine } = function (): LayoutEngine {
    let canvasWidth: number;
    let canvasWidthS: number;
    let canvasWidthC: number;
    let viewportTopH: number;

    var host: LayoutHost;
    let startRefs: GridLayoutHRefs
    let mainRefs: GridLayoutHRefs
    let refs: GridLayoutRefs;

    function init(hostGrid: LayoutHost) {
        host = hostGrid;
        refs = host.refs;
        startRefs = refs.start;
        mainRefs = refs.main;
        const options = host.getOptions();
        const signals = host.getSignals();

        host.getContainerNode().append(<>
            <HeaderPane hband="start" refs={refs} signals={signals} />
            <HeaderPane hband="main" refs={refs} signals={signals} />
            <TopPanel refs={refs} signals={signals} />
            <TopPane hband="start" refs={refs} signals={signals} />
            <TopPane hband="main" refs={refs} signals={signals} />
            <BodyPane hband="start" refs={refs} signals={signals} />
            <BodyPane hband="main" refs={refs} signals={signals} />
            <BottomPane hband="start" refs={refs} signals={signals} />
            <BottomPane hband="main" refs={refs} signals={signals} />
        </>);

        adjustFrozenRowsOption();
    }

    const calcCanvasWidth = () => {

        var cols = host.getColumns(), i = cols.length;
        canvasWidthS = canvasWidthS = 0;
        const { pinnedStartLast } = host.refs;

        while (i--) {
            if (i <= pinnedStartLast) {
                canvasWidthS += cols[i].width;
            } else {
                canvasWidthC += cols[i].width;
            }
        }

        var totalRowWidth = canvasWidthS + canvasWidthC;
        return host.getOptions().fullWidthRows ? Math.max(totalRowWidth, host.getAvailableWidth()) : totalRowWidth;
    }

    function getCanvasWidth() {
        return canvasWidth;
    }

    const updateCanvasWidth = () => {
        var oldCanvasWidth = canvasWidth;
        var oldCanvasWidthS = canvasWidthS;
        var oldCanvasWidthC = canvasWidthC;
        var widthChanged;
        canvasWidth = calcCanvasWidth();
        var scrollWidth = host.getScrollDims().width;

        widthChanged = canvasWidth !== oldCanvasWidth || canvasWidthS !== oldCanvasWidthS || canvasWidthC !== oldCanvasWidthC;
        var vpi = host.getViewportInfo();
        const { frozenTopLast, pinnedStartLast } = refs;
        const frozenCols = pinnedStartLast >= 0;
        const frozenRows = frozenTopLast >= 0;

        if (widthChanged || frozenCols || frozenRows) {
            var cwsPX = canvasWidthS + "px"
            var cwcPX = canvasWidthC + "px";
            var vpminusPX = (vpi.width - canvasWidthS) + "px";

            calcHeaderWidths();

            if (!frozenCols) {
                //mainRefs.headerCols.parentElement.parentElement.style.width = "100%";
                //mainRefs.headerRowCols.parentElement.style.width = "100%";
                //mainRefs.headerRowCols.style.width = canvasWidth + "px";
                //mainRefs.footerRowCols.parentElement.style.width = "100%";
                //mainRefs.footerRowCols.style.width = canvasWidth + "px";
                mainRefs.body.pane.style.width = "100%";
                mainRefs.body.viewport.style.width = "100%";
                mainRefs.body.canvas.style.width = cwcPX;
//
                //if (frozenRows) {
                //    mainRefs.top.pane.style.width = "100%";
                //    mainRefs.top.viewport.style.width = "100%";
                //    mainRefs.top.canvas.style.width = cwcPX;
                //}
            }
            else {
                const rtl = host.getOptions().rtl;
                const startKey = rtl ? "right" : "left";

                mainRefs.body.canvas.style.width = cwcPX;
                startRefs.headerCols.parentElement.parentElement.style.width = cwsPX;
                mainRefs.headerCols.parentElement.parentElement.style[startKey] = cwsPX;
                mainRefs.headerCols.parentElement.parentElement.style.width = vpminusPX;

                startRefs.body.pane.style.width = cwsPX;
                mainRefs.body.pane.style[startKey] = cwsPX;
                mainRefs.body.pane.style.width = vpminusPX;

                startRefs.headerRowCols.style.width = cwsPX;
                startRefs.headerRowCols.parentElement.style.width = cwsPX;
                mainRefs.headerRowCols.style.width = cwcPX;
                mainRefs.headerRowCols.parentElement.style.width = vpminusPX;

                //startRefs.footerRowCols.style.width = cwsPX;
                //startRefs.footerRowCols.parentElement.style.width = cwsPX;
                //mainRefs.footerRowCols.style.width = cwcPX;
                //mainRefs.footerRowCols.parentElement.style.width = vpminusPX;

                //startRefs.viewportTop.style.width = cwsPX;
                //mainRefs.viewportTop.style.width = vpminusPX;
//
                //if (frozenRows) {
                //    startRefs.paneBottom.style.width = cwsPX;
                //    mainRefs.paneBottom.style[startKey] = cwsPX;
//
                //    startRefs.viewportBottom.style.width = cwsPX;
                //    mainRefs.viewportBottom.style.width = vpminusPX;
//
                //    startRefs.canvasBottom.style.width = cwsPX;
                //    mainRefs.canvasBottom.style.width = cwcPX;
                //}

                vpi.hasHScroll = (canvasWidth > vpi.width - scrollWidth);
            }

            var w = (canvasWidth + (vpi.hasHScroll ? scrollWidth : 0)) + 'px';

            return widthChanged;
        }
    }

    const calcHeaderWidths = () => {
        var headersWidthL = 0, headersWidthR = 0;

        var scrollWidth = host.getScrollDims().width;
        var cols = host.getColumns();
        for (var i = 0, ii = cols.length; i < ii; i++) {
            var width = cols[i].width;

            //if (frozenCols > 0 && i >= frozenCols) {
            //    headersWidthR += width;
            //} else {
            //    headersWidthL += width;
            //}
        }

        const vs = host.getViewportInfo();

        //if (frozenCols > 0) {
        //    headersWidthL = headersWidthL + 1000;
        //    headersWidthR = Math.max(headersWidthR, vs.width) + headersWidthL;
        //    headersWidthR += scrollWidth;
        //} else {
        //    headersWidthL += scrollWidth;
        //    headersWidthL = Math.max(headersWidthL, vs.width) + 1000;
        //}

        //headerColsL.style.width = headersWidthL + 'px';
        //headerColsR.style.width = headersWidthR + 'px';
    }

    const setPaneVisibility = () => {
    }

    const setOverflow = () => {
        const options = host.getOptions();
        var alwaysHS = options.alwaysAllowHorizontalScroll;
        var alwaysVS = options.alwaysShowVerticalScroll;

        //viewportTopL.style.overflowX = viewportTopR.style.overflowX = (frozenRows && !alwaysHS) ? 'hidden' : (frozenCols ? 'scroll' : 'auto');
        //viewportTopL.style.overflowY = viewportBottomL.style.overflowY = (!frozenCols && alwaysVS) ? 'scroll' :
        //    (frozenCols ? 'hidden' : (frozenRows ? 'scroll' : (options.autoHeight ? 'hidden' : 'auto')));
        //viewportTopR.style.overflowY = (alwaysVS || frozenRows) ? 'scroll' : (options.autoHeight ? 'hidden' : 'auto');
        //viewportBottomL.style.overflowX = viewportBottomR.style.overflowX = (frozenCols && !alwaysHS) ? 'scroll' : 'auto';
        //viewportBottomR.style.overflowY = (alwaysVS) ? 'scroll' : 'auto';
    }

    const afterHeaderColumnDrag = () => {
        //const oldCanvasWidthL = canvasWidthL;
        //canvasWidth = calcCanvasWidth();
        //if (frozenCols && canvasWidthL != oldCanvasWidthL) {
        //    headerColsL.style.width = canvasWidthL + 1000 + 'px';
        //    paneHeaderR.style[host.getOptions().rtl ? 'right' : 'left'] = canvasWidthL + 'px';
        //}
    }

    const resizeCanvas = () => {
        //var _paneTopH = 0
        //var _paneBottomH = 0
        //const vs = host.getViewportInfo();
        //const options = host.getOptions();
//
        //// Account for Frozen Rows
        //if (frozenRows) {
        //    const frozenRowsHeight = frozenRows * options.rowHeight;
        //    if (frozenBottom) {
        //        _paneTopH = vs.height - frozenRowsHeight;
        //        _paneBottomH = frozenRowsHeight + host.getScrollDims().height;
        //    } else {
        //        _paneTopH = frozenRowsHeight;
        //        _paneBottomH = vs.height - frozenRowsHeight;
        //    }
        //} else {
        //    _paneTopH = vs.height;
        //}
//
        //// The top pane includes the the header row and the footer row
        //_paneTopH += vs.headerRowHeight + vs.footerRowHeight;
//
        //// The top viewport does not contain the header row or the footer row
        //viewportTopH = _paneTopH - vs.headerRowHeight - vs.footerRowHeight;
//
        //if (options.autoHeight) {
        //    host.getContainerNode().style.height = _paneTopH + vs.groupingPanelHeight + vs.topPanelHeight + vs.headerHeight + 'px';
        //}
//
        //paneTopL.style.top = vs.groupingPanelHeight + vs.topPanelHeight + vs.headerHeight + "px";
        //paneTopL.style.height = _paneTopH + 'px';
//
        //var paneBottomTop = paneTopL.offsetTop + _paneTopH;
//
        //if (options.autoHeight) {
        //    viewportTopL.style.height = '';
        //}
        //else {
        //    viewportTopL.style.height = viewportTopH + 'px'
        //}
//
        //if (frozenCols) {
        //    paneTopR.style.top = paneTopL.style.top;
        //    paneTopR.style.height = paneTopL.style.height;
//
        //    viewportTopR.style.height = viewportTopL.style.height;
//
        //    if (frozenRows) {
        //        paneBottomL.style.top = paneBottomR.style.top = paneBottomTop + 'px';
        //        paneBottomL.style.height = paneBottomR.style.height = viewportBottomR.style.height = _paneBottomH + 'px';
        //    }
        //} else {
        //    if (frozenRows) {
        //        paneBottomL.style.width = '100%';
        //        paneBottomL.style.height = _paneBottomH + 'px';
        //        paneBottomL.style.top = paneBottomTop + 'px';
        //    }
        //}
//
        //if (frozenRows) {
        //    viewportBottomL.style.height = _paneBottomH + 'px';
        //    const frozenRowsHeight = frozenRows * options.rowHeight;
        //    if (frozenBottom) {
        //        canvasBottomL.style.height = frozenRowsHeight + 'px';
//
        //        if (frozenCols) {
        //            canvasBottomR.style.height = frozenRowsHeight + 'px';
        //        }
        //    } else {
        //        canvasTopL.style.height = frozenRowsHeight + 'px';
//
        //        if (frozenCols) {
        //            canvasTopR.style.height = frozenRowsHeight + 'px';
        //        }
        //    }
        //} else {
        //    viewportTopR.style.height = viewportTopH + 'px';
        //}
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

    function realScrollHeightChange() {
        //const h = host.getViewportInfo().realScrollHeight;
        //if (frozenRows && !frozenBottom) {
        //    canvasBottomL.style.height = h + 'px';
//
        //    if (frozenCols) {
        //        canvasBottomR.style.height = h + 'px';
        //    }
        //} else {
        //    canvasTopL.style.height = h + 'px'
        //    canvasTopR.style.height = h + 'px'
        //}
    }

    function beforeCleanupAndRenderCells(rendered: ViewRange) {
        //if (frozenRows) {
//
        //    var renderedFrozenRows = Object.assign({}, rendered);
//
        //    if (frozenBottom) {
        //        renderedFrozenRows.top = frozenRowIdx;
        //        renderedFrozenRows.bottom = host.getDataLength() - 1;
        //    }
        //    else {
//
        //        renderedFrozenRows.top = 0;
        //        renderedFrozenRows.bottom = frozenRowIdx;
        //    }
//
        //    host.cleanUpAndRenderCells(renderedFrozenRows);
        //}
    }

    function getRowFromCellNode(cellNode: HTMLElement, clientX: number, clientY: number): number {
        var row = host.getRowFromNode(cellNode.parentNode as HTMLElement);

        //if (frozenRows) {
//
        //    var bcr = cellNode.closest('.grid-canvas').getBoundingClientRect();
//
        //    var rowOffset = 0;
        //    var isBottom = cellNode.closest('.grid-canvas-bottom') != null;
//
        //    if (isBottom) {
        //        rowOffset = frozenBottom ? Math.round(parsePx(getComputedStyle(canvasTopL).height)) : (frozenRows * host.getOptions().rowHeight);
        //    }
//
        //    return host.getCellFromPoint(clientX - bcr[host.getOptions().rtl ? 'right' : 'left'] - document.body.scrollLeft, clientY - bcr.top + document.body.scrollTop + rowOffset + document.body.scrollTop).row;
        //}

        return row;
    }

    function destroy(): void {
        host = startRefs = mainRefs = null;
    }

    return {
        afterHeaderColumnDrag,
        afterSetOptions,
        beforeCleanupAndRenderCells,
        calcCanvasWidth,
        updateHeadersWidth: calcHeaderWidths,
        destroy,
        getCanvasWidth,
        getRowFromCellNode,
        init,
        layoutName: "frozen",
        realScrollHeightChange,
        reorderViewColumns,
        resizeCanvas,
        setPaneVisibility,
        setOverflow,
        updateCanvasWidth
    }
} as any;

