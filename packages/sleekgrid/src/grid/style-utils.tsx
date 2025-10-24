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
