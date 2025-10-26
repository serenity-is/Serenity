import { computed } from "@serenity-is/signals";
import type { SignalLike, SignalOrValue } from "@serenity-is/sleekdom";
import type { GridSignals } from "../core";
import type { BandKey, GridLayoutRefs, PaneKey } from "./layout-refs";

type PaneSignalSet = Pick<GridSignals, "hideColumnHeader" | "hideHeaderRow" | "hideFooterRow" | "pinnedStartLast" | "pinnedEndFirst" | "frozenTopLast" | "frozenBottomFirst">;

function bandHidden(band: BandKey, hide: SignalLike<boolean>, signals: Pick<GridSignals, "pinnedStartLast" | "pinnedEndFirst">): SignalLike<boolean> {
    if (band === "main")
        return hide;
    return computed(() => hide.value ||
        (band === "start" && signals.pinnedStartLast.value < 0) ||
        (band === "end" && signals.pinnedEndFirst.value == Infinity)
    );
}

function paneBandHidden(pane: PaneKey, band: BandKey, signals: Pick<GridSignals, "pinnedStartLast" | "pinnedEndFirst" | "frozenTopLast" | "frozenBottomFirst" | "hideFooterRow" | "hideHeaderRow">): boolean | SignalLike<boolean> {
    if (pane === "body" && band === "main")
        return false;

    return computed(() =>
        (pane === "top" && signals.hideHeaderRow && signals.frozenTopLast.value < 0) ||
        (pane === "bottom" && signals.hideFooterRow && signals.frozenBottomFirst.value === Infinity) ||
        (band === "start" && signals.pinnedStartLast.value < 0) ||
        (band === "end" && signals.pinnedEndFirst.value == Infinity));
}

export const Header = ({ band, refs, signals }: {
    band: BandKey,
    refs: GridLayoutRefs,
    signals: Pick<GridSignals, "hideColumnHeader" | "pinnedStartLast" | "pinnedEndFirst">
}) => {
    const bandRefs = refs[band];
    return <div class={{ [`slick-header slick-header-${band}`]: true, "slick-hidden": bandHidden(band, signals.hideColumnHeader, signals) }}>
        <div class={`slick-header-columns slick-header-columns-${band}`}
            ref={el => { bandRefs.headerCols = el }} />
    </div>;
}

export const HeaderRow = ({ band, refs, signals }: {
    band: BandKey,
    refs: GridLayoutRefs,
    signals: Pick<GridSignals, "hideHeaderRow" | "pinnedStartLast" | "pinnedEndFirst">
}) => {
    const bandRefs = refs[band];
    return <div class={{ [`slick-headerrow slick-headerrow-${band}`]: true, "slick-hidden": bandHidden(band, signals.hideHeaderRow, signals) }}>
        <div class={`slick-headerrow-columns slick-headerrow-columns-${band}`} ref={el => bandRefs.headerRowCols = el} />
    </div>
}

export const TopPanel = ({ refs, signals }: {
    refs: GridLayoutRefs,
    signals: Pick<GridSignals, "hideTopPanel">
}) => {
    const bandRefs = refs["main"];
    return <div class={{ [`slick-top-panel-container`]: true, "slick-hidden": signals.hideTopPanel }}>
        <div class="slick-top-panel" ref={el => refs.topPanel = el} />
    </div>;
}

export const Viewport = ({ band, pane, refs }: {
    band: BandKey,
    pane: PaneKey,
    refs: GridLayoutRefs
}) => {
    const pRefs = refs[band][pane];
    return <div class={`slick-viewport slick-viewport-${pane} slick-viewport-${band}`} tabindex="0" ref={el => pRefs.viewport = el}>
        <div class={`grid-canvas grid-canvas-${pane} grid-canvas-${band}`} tabindex="0" ref={el => pRefs.canvas = el} />
    </div>;
}

export const FooterRow = ({ band, refs, signals }: {
    band: BandKey, refs: GridLayoutRefs,
    signals: Pick<GridSignals, "hideFooterRow" | "pinnedStartLast" | "pinnedEndFirst">
}) => {
    const bandRefs = refs[band];
    return <div class={{ [`slick-footerrow slick-footerrow-${band}`]: true, "slick-hidden": bandHidden(band, signals.hideFooterRow, signals) }}>
        <div class={`slick-footerrow-columns slick-footerrow-columns-${band}`} ref={el => bandRefs.footerRowCols = el} />
    </div>
}

export const Pane = ({ band, pane, children, refs, signals }: {
    band: BandKey,
    pane: PaneKey | "header",
    children?: any,
    refs: GridLayoutRefs,
    signals: PaneSignalSet
}) => {
    const hidden = pane == "header" ? bandHidden(band, signals.hideColumnHeader, signals) : paneBandHidden(pane, band, signals);
    const bandRefs = refs[band];
    return <div class={{ [`slick-pane slick-pane-${pane} slick-pane-${band}`]: true, "slick-hidden": hidden }} tabindex="0"
        ref={el => pane !== "header" && (bandRefs[pane].pane = el)}>
        {children}
    </div>;
}

export const HeaderPane = ({ band, children, refs, signals }: {
    band: BandKey,
    children?: any,
    refs: GridLayoutRefs,
    signals: PaneSignalSet
}) =>
    <Pane band={band} pane="header" refs={refs} signals={signals}>
        {children == null || children.length === 0 ? <Header band={band} refs={refs} signals={signals} /> : children}
    </Pane>;


export const TopPane = ({ band, headerRow, refs, signals }: {
    band: BandKey,
    headerRow?: any,
    refs: GridLayoutRefs,
    signals: PaneSignalSet, hidden?: SignalOrValue<boolean>
}) =>
    <Pane band={band} pane="top" refs={refs} signals={signals}>
        {headerRow ?? <HeaderRow band={band} refs={refs} signals={signals} />}
        <Viewport band={band} pane="top" refs={refs} />
    </Pane>;

export const BodyPane = ({ band, children, refs, signals }: {
    band: BandKey,
    children?: any,
    refs: GridLayoutRefs,
    signals: PaneSignalSet
}) =>
    <Pane band={band} pane="body" refs={refs} signals={signals}>
        {children ?? <Viewport band={band} pane="body" refs={refs} />}
    </Pane>;

export const BottomPane = ({ band, footerRow, refs, signals }: {
    band: BandKey,
    footerRow?: any,
    refs: GridLayoutRefs,
    signals: PaneSignalSet
}) =>
    <Pane band={band} pane="bottom" refs={refs} signals={signals}>
        {footerRow ?? <FooterRow band={band} refs={refs} signals={signals} />}
        <Viewport band={band} pane="bottom" refs={refs} />
    </Pane>;
