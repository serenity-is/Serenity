import { type JSXElement, type SignalLike, computed } from "@serenity-is/domwise";
import type { GridSignals } from "../core";
import type { BandKey, GridLayoutRefs, PaneKey } from "./layout-refs";

function bandHidden(band: BandKey, hide: SignalLike<boolean>, signals: Pick<GridSignals, "pinnedStartCols" | "pinnedEndCols">): SignalLike<boolean> {
    if (band === "main")
        return hide;
    return computed(() => hide.value ||
        (band === "start" && signals.pinnedStartCols.value < 0) ||
        (band === "end" && signals.pinnedEndCols.value == Infinity)
    );
}

function paneBandHidden(pane: PaneKey, band: BandKey, signals: Pick<GridSignals, "pinnedStartCols" | "pinnedEndCols" | "frozenTopRows" | "frozenBottomRows">): boolean | SignalLike<boolean> {
    if (pane === "body" && band === "main")
        return false;

    return computed(() =>
        (pane === "top" && signals.frozenTopRows.value <= 0) ||
        (pane === "bottom" && signals.frozenBottomRows.value <= 0) ||
        (band === "start" && signals.pinnedStartCols.value <= 0) ||
        (band === "end" && signals.pinnedEndCols.value <= 0));
}

/**
 * Header shell component for a single band. Hosts the column-header container
 * and hides automatically when the band is empty or the header is hidden.
 * @param props - Component props containing band, refs, and signals.
 */
export const Header = ({ band, refs, signals }: {
    /* Target band key. */
    band: BandKey,
    /* Layout refs owning the `headerCols` node. */
    refs: GridLayoutRefs,
    /* Visibility/pinning signals. */
    signals: Pick<GridSignals, "hideColumnHeader" | "pinnedStartCols" | "pinnedEndCols">
}): JSXElement => {
    const bandRefs = refs[band];
    return <div hidden={bandHidden(band, signals.hideColumnHeader, signals)} class={`sg-${band} slick-header`}>
        <div class={`sg-${band} slick-header-columns`} ref={el => { bandRefs.headerCols = el }} />
    </div>;
}

/**
 * Header-row (filter row) shell for a single band.
 * @param props - Component props containing band, refs, and signals.
 */
export const HeaderRow = ({ band, refs, signals }: {
    /* Target band key. */
    band: BandKey,
    /* Layout refs owning the `headerRowCols` node. */
    refs: GridLayoutRefs,
    /* Visibility/pinning signals. */
    signals: Pick<GridSignals, "hideHeaderRow" | "pinnedStartCols" | "pinnedEndCols">
}): JSXElement => {
    const bandRefs = refs[band];
    return <div hidden={bandHidden(band, signals.hideHeaderRow, signals)} class={`sg-${band} slick-headerrow`}>
        <div class={`sg-${band} slick-headerrow-columns`} ref={el => bandRefs.headerRowCols = el} />
    </div>
}

/**
 * Top panel container attached to the main band; hidden when `hideTopPanel` is true.
 * @param props - Component props containing refs and signals.
 */
export const TopPanel = ({ refs, signals }: {
    /* Layout refs owning `topPanel`. */
    refs: GridLayoutRefs,
    /* Visibility signals. */
    signals: Pick<GridSignals, "hideTopPanel">
}): JSXElement => {
    const bandRefs = refs["main"];
    return <div hidden={signals.hideTopPanel} class={`slick-top-panel-container`}>
        <div class="slick-top-panel" ref={el => refs.topPanel = el} />
    </div>;
}

/**
 * Scrollable viewport + canvas pair for a single `band`/`pane` cell.
 * Hidden when the corresponding frozen/pinned count is `0`.
 * @param props - Component props containing band, pane, refs, and signals.
 */
export const Viewport = ({ band, pane, refs, signals }: {
    /* Horizontal band key. */
    band: BandKey,
    /* Vertical pane key. */
    pane: PaneKey,
    /* Layout refs owning `canvas[pane]`. */
    refs: GridLayoutRefs,
    /* Pinning/frozen count signals. */
    signals: Pick<GridSignals, "frozenTopRows" | "frozenBottomRows" | "pinnedStartCols" | "pinnedEndCols">
}): JSXElement => {
    const bandRefs = refs[band];
    return <div hidden={paneBandHidden(pane, band, signals)} class={`sg-${pane} sg-${band} slick-viewport`} tabindex="0">
        <div class={`sg-${pane} sg-${band} grid-canvas`} tabindex="0" ref={el => bandRefs.canvas[pane] = el} />
    </div>;
}

/**
 * Footer row shell for a single band.
 * @param props - Component props containing band, refs, and signals.
 */
export const FooterRow = ({ band, refs, signals }: {
    /* Target band key. */
    band: BandKey,
    /* Layout refs owning the `footerRowCols` node. */
    refs: GridLayoutRefs,
    /* Visibility/pinning signals. */
    signals: Pick<GridSignals, "hideFooterRow" | "pinnedStartCols" | "pinnedEndCols">
}): JSXElement => {
    const bandRefs = refs[band];
    return <div hidden={bandHidden(band, signals.hideFooterRow, signals)} class={`sg-${band} slick-footerrow`}>
        <div class={`sg-${band} slick-footerrow-columns`} ref={el => bandRefs.footerRowCols = el} />
    </div>;
}
