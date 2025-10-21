import { computed, IfElse, signal } from "@serenity-is/signals";
import { Column, parsePx } from "../core";
import { LayoutEngine, LayoutHost } from "./layout";

export const BasicLayout: { new(): LayoutEngine } = function (): LayoutEngine {
    let host: LayoutHost;
    let canvasWidth: number;
    let headersWidth: number;

    let headerCols: HTMLElement;
    let headerRowCols: HTMLElement;
    let canvas: HTMLElement;
    let topPanel: HTMLElement;
    let viewport: HTMLElement;
    let footerRowCols: HTMLElement;

    function init(hostGrid: LayoutHost) {
        host = hostGrid;
        const options = host.getOptions();
        const optSignals = host.getOptionSignals();
        const hideColumnHeader = computed(() => !optSignals.showColumnHeader.value);

        host.getContainerNode().append(<>
            <div class={{ "slick-header": true, "slick-hidden": hideColumnHeader }}>
                <div class="slick-header-columns" ref={el => headerCols = el} />
            </div>
            <IfElse when={optSignals.showHeaderRow} else={__("headerrow")}>
                <div class="slick-headerrow slick-spacer-h">
                    <div class="slick-headerrow-columns" ref={el => headerRowCols = el} />
                </div>
            </IfElse>
            <IfElse when={optSignals.showTopPanel} else={__("toppanel")}>
                <div class="slick-top-panel-container">
                    <div class="slick-top-panel" ref={el => topPanel = el} />
                </div>
            </IfElse>
            <div class="slick-viewport" tabindex="0" ref={el => viewport = el}>
                <div class="grid-canvas" tabindex="0" ref={el => canvas = el} />
            </div>
            <IfElse when={optSignals.showFooterRow} else={__("footerrow")}>
                <div class="slick-footerrow">
                    <div class="slick-footerrow-columns" ref={el => footerRowCols = el} />
                </div>
            </IfElse>
        </>);

        updateHeadersWidth();
    }

    function appendCachedRow(_: number, _rowNodeS: HTMLDivElement, rowNodeC: HTMLDivElement, _rowNodeE: HTMLDivElement): void {
        rowNodeC && canvas.appendChild(rowNodeC);
    }

    function applyColumnWidths() {
        var x = 0, w, rule, cols = host.getColumns(), opts = host.getOptions(), rtl = opts.rtl;

        if (opts.useCssVars) {
            var styles = host.getContainerNode().style;
            for (var i = 0; i < cols.length; i++) {
                w = cols[i].width;
                var prop = "--l" + i;
                var oldVal = styles.getPropertyValue(prop);
                var newVal = x + "px";
                if (oldVal !== newVal)
                    styles.setProperty(prop, newVal);
                prop = "--r" + i;
                oldVal = styles.getPropertyValue(prop);
                newVal = (canvasWidth - x - w) + "px"
                if (oldVal !== newVal)
                    styles.setProperty(prop, newVal);
                x += w;
            }
        }
        else {
            for (var i = 0; i < cols.length; i++) {
                w = cols[i].width;
                rule = host.getColumnCssRules(i);
                rule[rtl ? "right" : "left"].style[rtl ? "right" : "left"] = x + "px";
                rule[rtl ? "left" : "right"].style[rtl ? "left" : "right"] = (canvasWidth - x - w) + "px";
                x += w;
            }
        }
    }

    function bindAncestorScrollEvents(): void {
        var elem: HTMLElement = canvas;
        while ((elem = elem.parentNode as HTMLElement) != document.body && elem != null) {
            // bind to scroll containers only
            if (elem == viewport || elem.scrollWidth != elem.clientWidth || elem.scrollHeight != elem.clientHeight) {
                host.bindAncestorScroll(elem);
            }
        }
    }

    function calcCanvasWidth() {
        var cols = host.getColumns(), i = cols.length;
        var rowWidth = 0;
        while (i--) {
            rowWidth += cols[i].width;
        }

        return host.getOptions().fullWidthRows ? Math.max(rowWidth,
            host.getAvailableWidth()) : rowWidth;
    }

    function updateHeadersWidth() {
        headersWidth = 0;

        var scrollWidth = host.getScrollDims().width;
        var cols = host.getColumns();
        for (var i = 0, ii = cols.length; i < ii; i++) {
            headersWidth += cols[i].width;
        }

        headersWidth += scrollWidth;
        headersWidth = Math.max(headersWidth, host.getViewportInfo().width) + 1000;
        headerCols.style.width = headersWidth + "px";
    }

    const destroy = () => {
        host = null;
    }

    function getCanvasNodeFor(): HTMLElement {
        return canvas;
    }

    function getCanvasNodes(): HTMLElement[] {
        return [canvas];
    }

    function getCanvasWidth(): number {
        return canvasWidth;
    }

    function getHeaderCols(): HTMLElement[] {
        return [headerCols];
    }

    function getHeaderColumn(cell: number): HTMLElement {
        return headerCols.children.item(cell) as HTMLElement;
    }

    function getHeaderRowCols(): HTMLElement[] {
        return exceptNull([headerRowCols])
    }

    function getHeaderRowColumn(cell: number): HTMLElement {
        return headerRowCols?.childNodes.item(cell) as HTMLElement;
    }

    function getHeaderRowColsFor(): HTMLElement {
        return headerRowCols;
    }

    function getFooterRowColumn(cell: number): HTMLElement {
        return footerRowCols?.childNodes.item(cell) as HTMLElement;
    }

    function getFooterRowColsFor(): HTMLElement {
        return footerRowCols;
    }

    function getHeaderColsFor(): HTMLElement {
        return headerCols;
    }

    function getFooterRowCols(): HTMLElement[] {
        return exceptNull([footerRowCols]);
    }

    function getRowFromCellNode(cellNode: HTMLElement): number {
        return host.getRowFromNode(cellNode.parentElement);
    }

    function getTopPanelFor(): HTMLElement {
        return topPanel;
    }

    function getViewportNodeFor(): HTMLElement {
        return viewport;
    }

    function getViewportNodes(): HTMLElement[] {
        return [viewport];
    }

    function handleScrollH(): void {
        headerCols && (headerCols.parentElement.scrollLeft = host.getScrollLeft());
        topPanel && (topPanel.parentElement.scrollLeft = host.getScrollLeft());
        headerRowCols && (headerRowCols.parentElement.scrollLeft = host.getScrollLeft());
        footerRowCols && (footerRowCols.parentElement.scrollLeft = host.getScrollLeft());
    }


    function realScrollHeightChange(): void {
        canvas.style.height = host.getViewportInfo().realScrollHeight + "px"
    }

    function reorderViewColumns(_: Column[]): Column[] {
        return null;
    }

    function setOverflow(): void {
        var alwaysVS = host.getOptions().alwaysShowVerticalScroll;

        viewport.style.overflowX = "auto";
        viewport.style.overflowY = alwaysVS ? "scroll" : (host.getOptions().autoHeight ? "hidden" : "auto");
    }

    function updateCanvasWidth(): boolean {
        var oldCanvasWidth = canvasWidth;
        canvasWidth = calcCanvasWidth();
        var scrollWidth = host.getScrollDims().width;

        const vpi = host.getViewportInfo();
        var canvasWidthPx = canvasWidth + "px"
        canvas.style.width = canvasWidthPx;
        headerRowCols.style.width = canvasWidthPx;
        footerRowCols.style.width = canvasWidthPx;
        updateHeadersWidth();
        vpi.hasHScroll = (canvasWidth > host.getViewportInfo().width - scrollWidth);

        return canvasWidth != oldCanvasWidth;
    }

    const resizeCanvas = () => {
        var vs = host.getViewportInfo();
        var _paneTopH = vs.height + vs.topPanelHeight + vs.headerRowHeight + vs.footerRowHeight;
        const options = host.getOptions();
        if (options.autoHeight) {
            host.getContainerNode().style.height = (_paneTopH + vs.groupingPanelHeight +
                parsePx(getComputedStyle(headerCols.parentElement).height)) + "px";
            viewport.style.height = "";
        }
        else
            viewport.style.height = vs.height + "px";
    }

    function returnZero() {
        return 0;
    }

    const intf: LayoutEngine = {
        afterHeaderColumnDrag: noop,
        afterRenderRows: noop,
        afterSetOptions: noop,
        appendCachedRow,
        applyColumnWidths,
        beforeCleanupAndRenderCells: noop,
        bindAncestorScrollEvents,
        calcCanvasWidth,
        updateHeadersWidth,
        isFrozenRow: returnFalse,
        destroy,
        getCanvasNodeFor,
        getCanvasNodes,
        getCanvasWidth,
        getFooterRowCols,
        getFooterRowColsFor,
        getFooterRowColumn,
        getHeaderCols,
        getHeaderColsFor,
        getHeaderColumn,
        getHeaderRowCols,
        getHeaderRowColsFor,
        getHeaderRowColumn,
        getRowFromCellNode,
        getFrozenTopLastRow: () => -1,
        getFrozenBottomFirstRow: () => Infinity,
        getPinnedStartLastCol: () => -1,
        getPinnedEndFirstCol: () => Infinity,
        getFrozenRowOffset: returnZero,
        getScrollCanvasY: getCanvasNodeFor,
        getScrollContainerX: getViewportNodeFor,
        getScrollContainerY: getViewportNodeFor,
        getTopPanelFor,
        getViewportNodeFor,
        getViewportNodes,
        handleScrollH,
        handleScrollV: noop,
        init,
        layoutName: "basic",
        realScrollHeightChange,
        reorderViewColumns,
        resizeCanvas,
        setOverflow,
        setPaneVisibility: noop,
        setScroller: noop,
        updateCanvasWidth
    }

    return intf;
} as any;

function exceptNull<T>(x: T[]) { return x.filter(y => y != null); }
function noop(): void { }
function returnFalse(): boolean { return false; }
function __(comment?: string): HTMLElement { return new Comment("placeholder:" + comment) as unknown as HTMLElement; }
