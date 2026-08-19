import type { Column } from "../core";
import type { Position } from "../core/editing";
import { parsePx } from "../core/util";
import { mapBands, type GridLayoutRefs } from "../layouts/layout-refs";

// shared across all grids on the page
let maxSupportedCssHeight: number;  // browser's breaking point
let scrollbarDimensions: { width: number, height: number };

/**
 * Computes the absolute bounding box of `elem` with aggregated offsets and
 * visibility, walking up through offset parents.
 * @param elem - Element to measure.
 * @returns Absolute box with `top`/`left`/`right`/`bottom`/`width`/`height`/`visible`.
 */
export function absBox(elem: HTMLElement): Position {
    var box: Position = {
        top: elem.offsetTop,
        left: elem.offsetLeft,
        bottom: 0,
        right: 0,
        width: elem.offsetWidth,
        height: elem.offsetHeight,
        visible: true
    };

    box.bottom = box.top + box.height;
    box.right = box.left + box.width;

    // walk up the tree
    var offsetParent = elem.offsetParent;
    while ((elem = elem.parentNode as HTMLElement) != document.body && elem != null) {
        if (box.visible && elem.scrollHeight != elem.offsetHeight && getComputedStyle(elem).overflowY !== "visible") {
            box.visible = box.bottom > elem.scrollTop && box.top < elem.scrollTop + elem.clientHeight;
        }

        if (box.visible && elem.scrollWidth != elem.offsetWidth && getComputedStyle(elem).overflowX != "visible") {
            box.visible = box.right > elem.scrollLeft && box.left < elem.scrollLeft + elem.clientWidth;
        }

        box.left -= elem.scrollLeft;
        box.top -= elem.scrollTop;

        if (elem === offsetParent) {
            box.left += elem.offsetLeft;
            box.top += elem.offsetTop;
            offsetParent = elem.offsetParent;
        }

        box.bottom = box.top + box.height;
        box.right = box.left + box.width;
    }

    return box;
}

/**
 * Returns the content width of `el` (outside of `border-box` padding/borders).
 * @param el - Element to measure.
 * @returns Inner width in pixels, clamped to `>= 0`.
 */
export function getInnerWidth(el: HTMLElement): number {
    if (!el)
        return 0;
    var style = getComputedStyle(el);
    var width = parsePx(style.width) ?? 0;
    if (style.boxSizing != 'border-box')
        return Math.max(0, width);

    var p = ["border-left-width", "border-right-width", "padding-left", "padding-right"];
    for (var val of p)
        width -= parsePx(style.getPropertyValue(val)) || 0;

    return Math.max(width, 0);
}

/**
 * Returns the browser's maximum supported CSS height before virtual paging is required.
 * @param recalc - When `true`, forces re-computation.
 * @returns Maximum supported pixel height.
 */
export function getMaxSupportedCssHeight(recalc?: boolean): number {
    if (!recalc && maxSupportedCssHeight != null)
        return maxSupportedCssHeight;
    return (maxSupportedCssHeight = ((navigator.userAgent.toLowerCase().match(/gecko\//) ? 4000000 : 32000000)));
}

/**
 * Measures scrollbar thickness by inserting a scrolled test node.
 * Results are cached until `recalc` is set.
 * @param recalc - When `true`, forces re-measurement.
 * @returns Object with `width` and `height` scrollbar dimensions in pixels.
 */
export function getScrollBarDimensions(recalc?: boolean): { width: number; height: number; } {
    if (!scrollbarDimensions || recalc) {
        const c = document.body.appendChild(<div style={{ position: "absolute", top: "-10000px", left: "-10000px", width: "100px", height: "100px", overflow: "scroll", border: "0" }} /> as HTMLElement);
        scrollbarDimensions = {
            width: Math.round(c.offsetWidth - c.clientWidth),
            height: Math.round(c.offsetHeight - c.clientHeight)
        };
        c.remove();
    }
    return scrollbarDimensions;
}

/**
 * Sets a CSS property only when the new value differs (avoids style recalc churn).
 * @param styles - Target `CSSStyleDeclaration`.
 * @param prop - CSS custom property name.
 * @param value - Value to set.
 */
export function setStyleProp(this: void, styles: CSSStyleDeclaration, prop: string, value: string): void {
    if (styles.getPropertyValue(prop) !== value)
        styles.setProperty(prop, value);
}


function isStartOfMainOrEnd(colIndex: number, refs: GridLayoutRefs): boolean {
    return (refs.pinnedStartLast + 1 === colIndex) ||
        (refs.pinnedEndFirst === colIndex);
}

/**
 * Creates dynamic grid styles (row/cell heights, column slot rules or CSS vars)
 * and optionally honors CSP `nonce`. Falls back to CSS variables when `useCssVars` is enabled.
 * @param args.cellHeightDiff - Difference between row and cell heights (borders/padding).
 * @param args.colCount - Number of columns to reserve slot rules for.
 * @param args.container - Grid container to toggle `sleek-vars` onto.
 * @param args.opt - Grid options relevant to styling (`rowHeight`, `useCssVars`, `styleNonce`).
 * @param args.scrollDims - Measured scrollbar dimensions.
 * @param args.uid - Grid UID used as the CSS namespace.
 * @returns Object with the created `styleNode` (or `undefined` when CSS vars are used).
 */
export function createCssRules(this: void, { opt, cellHeightDiff, colCount, container, scrollDims, uid }: {
    cellHeightDiff: number,
    colCount: number,
    container: HTMLElement,
    opt: { styleNonce?: string, useCssVars?: boolean | number, rowHeight?: number },
    scrollDims: { width: number, height: number },
    uid: string
}): {
    styleNode: HTMLStyleElement
} {
    const cellHeight = (opt.rowHeight - cellHeightDiff);
    const useCssVars = typeof opt.useCssVars === 'number' ? (colCount <= opt.useCssVars) :
        opt.useCssVars ? colCount <= 100 : false;

    container.classList.toggle('sleek-vars', useCssVars);

    if (useCssVars) {
        const style = container.style;
        style.setProperty("--sg-row-height", opt.rowHeight + "px");
        style.setProperty("--sg-cell-height", cellHeight + "px");
        style.setProperty("--sg-scrollbar-w", scrollDims.width + "px");
        style.setProperty("--sg-scrollbar-h", scrollDims.height + "px");
        return;
    }

    const styleNode = document.createElement('style');
    const nonce = opt.styleNonce ??
        document.head?.querySelector('meta[name="csp-nonce"]')?.getAttribute('content') ??
        document.head?.querySelector('style[nonce]')?.getAttribute('nonce') ??
        document.head?.querySelector('script[nonce]')?.getAttribute('nonce');

    styleNode.dataset.uid = uid;
    if (nonce)
        styleNode.nonce = nonce;

    const rules = [
        "." + uid + " { " +
        "--sg-row-height: " + opt.rowHeight + "px; " +
        "--sg-cell-height: " + cellHeight + "px; " +
        "--sg-scrollbar-w: " + scrollDims.width + "px; " +
        "--sg-scrollbar-h: " + scrollDims.height + "px; " +
        "}",
        "." + uid + " .slick-cell { height: " + cellHeight + "px; }",
        "." + uid + " .slick-row { height: " + opt.rowHeight + "px; }"
    ];

    for (let i = 0; i < colCount; i++) {
        rules.push("." + uid + " .l" + i + " { }");
        rules.push("." + uid + " .r" + i + " { }");
    }
    styleNode.appendChild(document.createTextNode(rules.join("\n")));
    document.head.appendChild(styleNode);
    return {
        styleNode
    }
}

/**
 * Finds the stylesheet created by {@link createCssRules} for `uid` and indexes
 * the column slot rules.
 * @param uid - Grid UID namespace.
 * @param styleNode - Style element created for the grid.
 * @returns Object with `stylesheet`, `colCssRulesL`/`colCssRulesR` maps and the root `varRule`.
 */
export function findStylesheetByUID(this: void, uid: string, styleNode: HTMLElement): {
    stylesheet: CSSStyleSheet,
    colCssRulesL: Record<number, CSSStyleRule>,
    colCssRulesR: Record<number, CSSStyleRule>,
    varRule: CSSStyleRule
} {
    let stylesheet: CSSStyleSheet;
    const stylesheetFromUid = document.querySelector("style[data-uid='" + uid + "']") as HTMLStyleElement;
    if (stylesheetFromUid && stylesheetFromUid.sheet) {
        stylesheet = stylesheetFromUid.sheet;
    } else {
        const sheets = document.styleSheets;
        for (let i = 0; i < sheets.length; i++) {
            if (sheets[i].ownerNode == styleNode) {
                stylesheet = sheets[i];
                break;
            }
        }
    }

    if (!stylesheet) {
        throw new Error(`Cannot find stylesheet for ${uid}`);
    }

    const colCssRulesL: Record<number, CSSStyleRule> = Object.create(null);
    const colCssRulesR: Record<number, CSSStyleRule> = Object.create(null);
    let varRule: CSSStyleRule = null;
    const cssRules = stylesheet.cssRules;
    let matches, columnIdx;
    for (let i = 0; i < cssRules.length; i++) {
        const rule = cssRules[i] as CSSStyleRule;
        const selector = rule.selectorText;
        if (matches = /\.l\d+/.exec(selector)) {
            columnIdx = parseInt(matches[0].substring(2, matches[0].length), 10);
            colCssRulesL[columnIdx] = rule;
        } else if (matches = /\.r\d+/.exec(selector)) {
            columnIdx = parseInt(matches[0].substring(2, matches[0].length), 10);
            colCssRulesR[columnIdx] = rule;
        } else if (selector.startsWith("--")) {
            varRule ??= rule;
        }
    }
    return {
        stylesheet,
        colCssRulesL,
        colCssRulesR,
        varRule
    }
}

/**
 * Applies pixel offsets/widths for each column via either stylesheet rules or
 * CSS custom properties (depending on the rendering mode).
 * @param args.cols - Ordered columns with resolved `.width`.
 * @param args.container - Grid container (used to set CSS vars in var-mode).
 * @param args.cssColRulesL - Left-anchor stylesheet rules per column (when not using vars).
 * @param args.cssColRulesR - Right-anchor rules per column.
 * @param args.opts - Options carrying `rtl`.
 * @param args.refs - Layout refs for per-band width calculations.
 */
export function applyColumnWidths(this: void, { cols, cssColRulesL, cssColRulesR, opts, container, refs }: {
    cols: Column[],
    container: HTMLElement,
    cssColRulesL?: Record<number, CSSStyleRule>,
    cssColRulesR?: Record<number, CSSStyleRule>,
    opts: { rtl?: boolean },
    refs: GridLayoutRefs,
}): void {
    let x = 0, w, start = opts.rtl ? 'right' : 'left', end = opts.rtl ? 'left' : 'right',
        startVar = opts.rtl ? '--r' : '--l', endVar = opts.rtl ? '--l' : '--r',
        styles = container.style;

    for (let c = 0; c < cols.length; c++) {
        if (isStartOfMainOrEnd(c, refs)) {
            x = 0;
        }
        w = cols[c].width;
        let startVal = x + "px";
        let endVal = (c <= refs.pinnedStartLast ? refs.start.canvasWidth : c >= refs.pinnedEndFirst ? refs.end.canvasWidth : refs.main.canvasWidth) - x - w + "px";
        if (!cssColRulesL) {
            setStyleProp(styles, startVar + c, startVal);
            setStyleProp(styles, endVar + c, endVal);
        }
        else {
            const ruleL = cssColRulesL[c];
            const ruleR = cssColRulesR[c];
            ruleL.style.setProperty(start, startVal);
            ruleL.style.setProperty(end, null);
            ruleR.style.setProperty(end, endVal);
            ruleR.style.setProperty(start, null);
        }
        x += w;
    }
}

/**
 * Applies optional fixed heights for top panel, grouping panel, header row and
 * footer row when those heights are provided via options.
 * @param args.groupingPanel - Optional grouping panel element.
 * @param args.opt - Optional heights from `GridOptions`.
 * @param args.refs - Layout refs owning the header/footer row containers.
 */
export function applyLegacyHeightOptions(this: void, { groupingPanel, opt, refs }: {
    groupingPanel: HTMLElement,
    opt: { topPanelHeight?: number, groupingPanelHeight?: number, headerRowHeight?: number, footerRowHeight?: number },
    refs: GridLayoutRefs,
}): void {
    if (opt.topPanelHeight != null) {
        const topPanel = refs.topPanel
        topPanel && (topPanel.style.height = opt.topPanelHeight + "px");
    }
    if (opt.groupingPanelHeight != null) {
        groupingPanel && (groupingPanel.style.height = opt.groupingPanelHeight + "px");
    }
    if (opt.headerRowHeight != null) {
        mapBands(refs, band => band.headerRowCols).forEach(hrc => hrc.style.height = opt.headerRowHeight + "px");
    }
    if (opt.footerRowHeight != null) {
        mapBands(refs, band => band.footerRowCols).forEach(frc => frc.style.height = opt.footerRowHeight + "px");
    }
}
