import type { Column } from "../core/column";
import { mapBands, type GridLayoutRefs } from "./layout-refs";

export function isStartOfMainOrEnd(colIndex: number, refs: GridLayoutRefs): boolean {
    return (refs.pinnedStartLast + 1 === colIndex) ||
        (refs.pinnedEndFirst === colIndex);
}

function setStyleVar(this: void, styles: CSSStyleDeclaration, prop: string, value: string): void {
    if (styles.getPropertyValue(prop) !== value)
        styles.setProperty(prop, value);
}

export function applyColumnWidths(this: void, { cols, colCSSRulesL, colCSSRulesR, opts, container, refs }: {
    cols: Column[],
    container: HTMLElement,
    colCSSRulesL?: Record<number, CSSStyleRule>,
    colCSSRulesR?: Record<number, CSSStyleRule>,
    opts: { rtl?: boolean },
    refs: GridLayoutRefs
}): void {
    let x = 0, w, rule, start = opts.rtl ? 'right' : 'left', end = opts.rtl ? 'left' : 'right',
        cwStart = refs.start.canvas.body?.clientWidth || 0,
        cwMain = refs.main.canvas.body?.clientWidth || 0,
        cwEnd = refs.end.canvas.body?.clientWidth || 0,
        styles = container.style;


    for (let c = 0; c < cols.length; c++) {
        if (isStartOfMainOrEnd(c, refs)) {
            x = 0;
        }
        w = cols[c].width;
        let startVal = x + "px";
        let endVal = (c <= refs.pinnedStartLast ? cwStart : c >= refs.pinnedEndFirst ? cwEnd : cwMain) - x - w + "px";
        if (!colCSSRulesL) {
            setStyleVar(styles, "--l" + c, startVal);
            setStyleVar(styles, "--r" + c, endVal);
        }
        else {
            const ruleL = colCSSRulesL[c];
            const ruleR = colCSSRulesR[c];
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
