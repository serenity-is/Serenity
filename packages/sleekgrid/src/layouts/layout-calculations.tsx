import type { Column } from "../core/column";
import { setStyleProp } from "../grid/style-utils";
import { mapBands, type GridLayoutRefs } from "./layout-refs";

function isStartOfMainOrEnd(colIndex: number, refs: GridLayoutRefs): boolean {
    return (refs.pinnedStartLast + 1 === colIndex) ||
        (refs.pinnedEndFirst === colIndex);
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
        style.setProperty("--sg-row-height", opt.rowHeight + "px");
        style.setProperty("--sg-cell-height", cellHeight + "px");
        style.setProperty("--sg-scrollbar-w", scrollDims.width + "px");
        style.setProperty("--sg-scrollbar-h", scrollDims.height + "px");
        return;
    }

    const styleNode = document.createElement('style');
    styleNode.dataset.uid = uid;
    var rules = ["." + uid + "{ " +
        "--sg-cell-height: " + opt.rowHeight + "px;" +
        "--sg-scrollbar-w: " + scrollDims.width + "px;",
        "--sg-scrollbar-h: " + scrollDims.height + "px;",
        " .slick-cell { height:" + cellHeight + "px;",
        " .slick-row { height:" + opt.rowHeight + "px;" +
        " }",
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

export function applyColumnWidths(this: void, { cols, cssColRulesL, cssColRulesR, opts, container, refs }: {
    cols: Column[],
    container: HTMLElement,
    cssColRulesL?: Record<number, CSSStyleRule>,
    cssColRulesR?: Record<number, CSSStyleRule>,
    opts: { rtl?: boolean },
    refs: GridLayoutRefs,
}): void {
    let x = 0, w, start = opts.rtl ? 'right' : 'left', end = opts.rtl ? 'left' : 'right',
        styles = container.style;

    for (let c = 0; c < cols.length; c++) {
        if (isStartOfMainOrEnd(c, refs)) {
            x = 0;
        }
        w = cols[c].width;
        let startVal = x + "px";
        let endVal = (c <= refs.pinnedStartLast ? refs.start.canvasWidth : c >= refs.pinnedEndFirst ? refs.end.canvasWidth : refs.main.canvasWidth) - x - w + "px";
        if (!cssColRulesL) {
            setStyleProp(styles, "--l" + c, startVal);
            setStyleProp(styles, "--r" + c, endVal);
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

export function applyLegacyHeightOptions(this: void, { groupingPanel, opt, refs }: {
    groupingPanel: HTMLElement,
    opt: { topPanelHeight?: number, groupingPanelHeight?: number, headerRowHeight?: number, footerRowHeight?: number },
    refs: GridLayoutRefs,
}) {
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
