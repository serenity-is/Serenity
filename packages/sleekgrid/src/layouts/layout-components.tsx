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
 * @param props.band - Target band key.
 * @param props.refs - Layout refs owning the `headerCols` node.
 * @param props.signals - Visibility/pinning signals.
 */
export const Header = ({ band, refs, signals }: {
    band: BandKey,
    refs: GridLayoutRefs,
    signals: Pick<GridSignals, "hideColumnHeader" | "pinnedStartCols" | "pinnedEndCols">
}): JSXElement => {
    const bandRefs = refs[band];
    return <div hidden={bandHidden(band, signals.hideColumnHeader, signals)} class={`sg-${band} slick-header`}>
        <div class={`sg-${band} slick-header-columns`} ref={el => { bandRefs.headerCols = el }} />
    </div>;
}

/**
 * Header-row (filter row) shell for a single band.
 * @param props.band - Target band key.
 * @param props.refs - Layout refs owning the `headerRowCols` node.
 * @param props.signals - Visibility/pinning signals.
 */
export const HeaderRow = ({ band, refs, signals }: {
    band: BandKey,
    refs: GridLayoutRefs,
    signals: Pick<GridSignals, "hideHeaderRow" | "pinnedStartCols" | "pinnedEndCols">
}): JSXElement => {
    const bandRefs = refs[band];
    return <div hidden={bandHidden(band, signals.hideHeaderRow, signals)} class={`sg-${band} slick-headerrow`}>
        <div class={`sg-${band} slick-headerrow-columns`} ref={el => bandRefs.headerRowCols = el} />
    </div>
}

/**
 * Top panel container attached to the main band; hidden when `hideTopPanel` is true.
 * @param props.refs - Layout refs owning `topPanel`.
 * @param props.signals - Visibility signals.
 */
export const TopPanel = ({ refs, signals }: {
    refs: GridLayoutRefs,
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
 * @param props.band - Horizontal band key.
 * @param props.pane - Vertical pane key.
 * @param props.refs - Layout refs owning `canvas[pane]`.
 * @param props.signals - Pinning/frozen count signals.
 */
export const Viewport = ({ band, pane, refs, signals }: {
    band: BandKey,
    pane: PaneKey,
    refs: GridLayoutRefs,
    signals: Pick<GridSignals, "frozenTopRows" | "frozenBottomRows" | "pinnedStartCols" | "pinnedEndCols">
}): JSXElement => {
    const bandRefs = refs[band];
    return <div hidden={paneBandHidden(pane, band, signals)} class={`sg-${pane} sg-${band} slick-viewport`} tabindex="0">
        <div class={`sg-${pane} sg-${band} grid-canvas`} tabindex="0" ref={el => bandRefs.canvas[pane] = el} />
    </div>;
}

/**
 * Footer row shell for a single band.
 * @param props.band - Target band key.
 * @param props.refs - Layout refs owning the `footerRowCols` node.
 * @param props.signals - Visibility/pinning signals.
 */
export const FooterRow = ({ band, refs, signals }: {
    band: BandKey, refs: GridLayoutRefs,
    signals: Pick<GridSignals, "hideFooterRow" | "pinnedStartCols" | "pinnedEndCols">
}): JSXElement => {
    const bandRefs = refs[band];
    return <div hidden={bandHidden(band, signals.hideFooterRow, signals)} class={`sg-${band} slick-footerrow`}>
        <div class={`sg-${band} slick-footerrow-columns`} ref={el => bandRefs.footerRowCols = el} />
    </div>;
}
