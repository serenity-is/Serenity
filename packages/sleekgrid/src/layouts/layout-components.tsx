import { type SignalLike, computed } from "@serenity-is/domwise";
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

export const Header = ({ band, refs, signals }: {
    band: BandKey,
    refs: GridLayoutRefs,
    signals: Pick<GridSignals, "hideColumnHeader" | "pinnedStartCols" | "pinnedEndCols">
}) => {
    const bandRefs = refs[band];
    return <div hidden={bandHidden(band, signals.hideColumnHeader, signals)} class={`sg-${band} slick-header`}>
        <div class={`sg-${band} slick-header-columns`} ref={el => { bandRefs.headerCols = el }} />
    </div>;
}

export const HeaderRow = ({ band, refs, signals }: {
    band: BandKey,
    refs: GridLayoutRefs,
    signals: Pick<GridSignals, "hideHeaderRow" | "pinnedStartCols" | "pinnedEndCols">
}) => {
    const bandRefs = refs[band];
    return <div hidden={bandHidden(band, signals.hideHeaderRow, signals)} class={`sg-${band} slick-headerrow`}>
        <div class={`sg-${band} slick-headerrow-columns`} ref={el => bandRefs.headerRowCols = el} />
    </div>
}

export const TopPanel = ({ refs, signals }: {
    refs: GridLayoutRefs,
    signals: Pick<GridSignals, "hideTopPanel">
}) => {
    const bandRefs = refs["main"];
    return <div hidden={signals.hideTopPanel} class={`slick-top-panel-container`}>
        <div class="slick-top-panel" ref={el => refs.topPanel = el} />
    </div>;
}

export const Viewport = ({ band, pane, refs, signals }: {
    band: BandKey,
    pane: PaneKey,
    refs: GridLayoutRefs,
    signals: Pick<GridSignals, "frozenTopRows" | "frozenBottomRows" | "pinnedStartCols" | "pinnedEndCols">
}) => {
    const bandRefs = refs[band];
    return <div hidden={paneBandHidden(pane, band, signals)} class={`sg-${pane} sg-${band} slick-viewport`} tabindex="0">
        <div class={`sg-${pane} sg-${band} grid-canvas`} tabindex="0" ref={el => bandRefs.canvas[pane] = el} />
    </div>;
}

export const FooterRow = ({ band, refs, signals }: {
    band: BandKey, refs: GridLayoutRefs,
    signals: Pick<GridSignals, "hideFooterRow" | "pinnedStartCols" | "pinnedEndCols">
}) => {
    const bandRefs = refs[band];
    return <div hidden={bandHidden(band, signals.hideFooterRow, signals)} class={`sg-${band} slick-footerrow`}>
        <div class={`sg-${band} slick-footerrow-columns`} ref={el => bandRefs.footerRowCols = el} />
    </div>;
}
