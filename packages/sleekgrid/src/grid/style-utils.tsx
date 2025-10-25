import type { Position } from "../core/editing";
import { parsePx } from "../core/util";

// shared across all grids on the page
let maxSupportedCssHeight: number;  // browser's breaking point
let scrollbarDimensions: { width: number, height: number };

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

export function getInnerWidth(el: HTMLElement): number {
    var style = getComputedStyle(el);
    var width = parsePx(style.width) ?? 0;
    if (style.boxSizing != 'border-box')
        return Math.max(0, width);

    var p = ["border-left-width", "border-right-width", "padding-left", "padding-right"];
    for (var val of p)
        width -= parsePx(style.getPropertyValue(val)) || 0;

    return Math.max(width, 0);
}

export function getMaxSupportedCssHeight(recalc?: boolean): number {
    if (!recalc && maxSupportedCssHeight != null)
        return maxSupportedCssHeight;
    return (maxSupportedCssHeight = ((navigator.userAgent.toLowerCase().match(/gecko\//) ? 4000000 : 32000000)));
}

export function getScrollBarDimensions(recalc?: boolean): { width: number; height: number; } {
    if (!scrollbarDimensions || recalc) {
        const c = document.body.appendChild(<div style="position:absolute;top:-10000px;left:-10000px;width:100px;height:100px;overflow: scroll;border:0" /> as HTMLElement);
        scrollbarDimensions = {
            width: Math.round(c.offsetWidth - c.clientWidth),
            height: Math.round(c.offsetWidth - c.clientHeight)
        };
        c.remove();
    }
    return scrollbarDimensions;
}

export function getVBoxDelta(el: HTMLElement): number {
    if (!el)
        return 0;

    var style = getComputedStyle(el);
    if (style.boxSizing === 'border-box')
        return 0;

    var p = ["border-top-width", "border-bottom-width", "padding-top", "padding-bottom"];
    var delta = 0;
    for (var val of p)
        delta += parsePx(style.getPropertyValue(val)) || 0;
    return delta;
}

export function createCssRules(this: void, { opt, cellHeightDiff, colCount, container, scrollDims, uid }: {
    cellHeightDiff: number,
    colCount: number,
    container: HTMLElement,
    opt: { useCssVars?: boolean | number, rowHeight?: number },
    scrollDims: { width: number, height: number },
    uid: string
}): {
    styleNode: HTMLStyleElement
} {
    const cellHeight = (opt.rowHeight - cellHeightDiff);
    const useCssVars = typeof opt.useCssVars === 'number' ? (colCount <= opt.useCssVars) :
        opt.useCssVars ? colCount <= 50 : false;

    container.classList.toggle('sleek-vars', useCssVars);

    if (useCssVars) {
        const style = container.style;
        style.setProperty("--row-height", opt.rowHeight + "px");
        style.setProperty("--cell-height", cellHeight + "px");
        style.setProperty("--scrollbar-w", scrollDims.width + "px");
        style.setProperty("--scrollbar-h", scrollDims.height + "px");
        return;
    }

    const styleNode = document.createElement('style');
    styleNode.dataset.uid = uid;
    var rules = [
        "." + uid + " { --cell-height: " + opt.rowHeight + "px; }",
        "." + uid + " { --scrollbar-w: " + scrollDims.width + "px; }",
        "." + uid + " { --scrollbar-h: " + scrollDims.height + "px; }",
        "." + uid + " .slick-cell { height:" + cellHeight + "px; }",
        "." + uid + " .slick-row { height:" + opt.rowHeight + "px; }"
    ];

    for (var i = 0; i < colCount; i++) {
        rules.push("." + uid + " .l" + i + " { }");
        rules.push("." + uid + " .r" + i + " { }");
    }
    styleNode.appendChild(document.createTextNode(rules.join(" ")));
    document.head.appendChild(styleNode);
    return {
        styleNode
    }
}

export function findStylesheetByUID(this: void, uid: string, styleNode: HTMLElement): {
    stylesheet: CSSStyleSheet,
    colCssRulesL: Record<number, CSSStyleRule>,
    colCssRulesR: Record<number, CSSStyleRule>
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

    let colCssRulesL: Record<number, CSSStyleRule> = {};
    let colCssRulesR: Record<number, CSSStyleRule> = {};
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
        }
    }
    return {
        stylesheet,
        colCssRulesL,
        colCssRulesR
    }
}


