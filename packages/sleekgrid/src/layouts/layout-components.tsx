import { computed } from "@serenity-is/signals";
import type { SignalLike, SignalOrValue } from "@serenity-is/sleekdom";
import type { GridSignals } from "../core";
import type { HBand, VBand } from "./layout-consts";
import type { GridLayoutRefs } from "./layout-refs";

type PaneSignalSet = Pick<GridSignals, "hideColumnHeader" | "hideHeaderRow" | "hideFooterRow" | "pinnedStartLast" | "pinnedEndFirst" | "frozenTopLast" | "frozenBottomFirst">;

function hiddenH(hband: HBand, hide: SignalLike<boolean>, signals: Pick<GridSignals, "pinnedStartLast" | "pinnedEndFirst">): SignalLike<boolean> {
    if (hband === "main")
        return hide;
    return computed(() => hide.value ||
        (hband === "start" && signals.pinnedStartLast.value < 0) ||
        (hband === "end" && signals.pinnedEndFirst.value == Infinity)
    );
}

function hiddenVH(vband: VBand, hband: HBand, signals: Pick<GridSignals, "pinnedStartLast" | "pinnedEndFirst" | "frozenTopLast" | "frozenBottomFirst">): boolean | SignalLike<boolean> {
    if (vband === "body" && hband === "main")
        return false;

    return computed(() =>
        (vband === "top" && signals.frozenTopLast.value < 0) ||
        (vband === "body" && signals.frozenBottomFirst.value === Infinity) ||
        (hband === "start" && signals.pinnedStartLast.value < 0) ||
        (hband === "end" && signals.pinnedEndFirst.value == Infinity));
}

export const Header = ({ hband, refs, signals }: {
    hband: HBand,
    refs: GridLayoutRefs,
    signals: Pick<GridSignals, "hideColumnHeader" | "pinnedStartLast" | "pinnedEndFirst">
}) => {
    const hRefs = refs[hband];
    return <div class={{ [`slick-header slick-header-${hband}`]: true, "slick-hidden": hiddenH(hband, signals.hideColumnHeader, signals) }}>
        <div class={`slick-header-columns slick-header-columns-${hband}`}
            ref={el => { hRefs.headerCols = el }} />
    </div>;
}

export const HeaderRow = ({ hband, refs, signals }: {
    hband: HBand,
    refs: GridLayoutRefs,
    signals: Pick<GridSignals, "hideHeaderRow" | "pinnedStartLast" | "pinnedEndFirst">
}) => {
    const hRefs = refs[hband];
    return <div class={{ [`slick-headerrow slick-headerrow-${hband}`]: true, "slick-hidden": hiddenH(hband, signals.hideHeaderRow, signals) }}>
        <div class={`slick-headerrow-columns slick-headerrow-columns-${hband}`} ref={el => hRefs.headerRowCols = el} />
    </div>
}

export const TopPanel = ({ refs, signals }: {
    refs: GridLayoutRefs,
    signals: Pick<GridSignals, "hideTopPanel">
}) => {
    const hRefs = refs["main"];
    return <div class={{ [`slick-top-panel-container`]: true, "slick-hidden": signals.hideTopPanel }}>
        <div class="slick-top-panel" ref={el => hRefs.topPanel = el} />
    </div>;
}

export const Viewport = ({ hband, vband, refs }: {
    hband: HBand,
    vband: VBand,
    refs: GridLayoutRefs
}) => {
    const pRefs = refs[hband][vband];
    return <div class={`slick-viewport slick-viewport-${vband} slick-viewport-${hband}`} tabindex="0" ref={el => pRefs.viewport = el}>
        <div class={`grid-canvas grid-canvas-${vband} grid-canvas-${hband}`} tabindex="0" ref={el => pRefs.canvas = el} />
    </div>;
}

export const FooterRow = ({ hband, refs, signals }: {
    hband: HBand, refs: GridLayoutRefs,
    signals: Pick<GridSignals, "hideFooterRow" | "pinnedStartLast" | "pinnedEndFirst">
}) => {
    const hRefs = refs[hband];
    return <div class={{ [`slick-footerrow slick-footerrow-${hband}`]: true, "slick-hidden": hiddenH(hband, signals.hideFooterRow, signals) }}>
        <div class={`slick-footerrow-columns slick-footerrow-columns-${hband}`} ref={el => hRefs.footerRowCols = el} />
    </div>
}

export const Pane = ({ hband, vband, children, refs, signals }: {
    hband: HBand,
    vband: VBand | "header",
    children?: any,
    refs: GridLayoutRefs,
    signals: PaneSignalSet
}) => {
    const hidden = vband == "header" ? hiddenH(hband, signals.hideColumnHeader, signals) : hiddenVH(vband, hband, signals);
    const hRefs = refs[hband];
    return <div class={{ [`slick-pane slick-pane-${vband} slick-pane-${hband}`]: true, "slick-hidden": hidden }} tabindex="0"
        ref={el => vband !== "header" && (hRefs[vband].pane = el)}>
        {children}
    </div>;
}

export const HeaderPane = ({ hband, children, refs, signals }: {
    hband: HBand,
    children?: any,
    refs: GridLayoutRefs,
    signals: PaneSignalSet
}) =>
    <Pane hband={hband} vband="header" refs={refs} signals={signals}>
        {children == null || children.length === 0 ? <Header hband={hband} refs={refs} signals={signals} /> : children}
    </Pane>;


export const TopPane = ({ hband, headerRow, refs, signals }: {
    hband: HBand,
    headerRow?: any,
    refs: GridLayoutRefs,
    signals: PaneSignalSet, hidden?: SignalOrValue<boolean>
}) =>
    <Pane hband={hband} vband="top" refs={refs} signals={signals}>
        {headerRow ?? <HeaderRow hband={hband} refs={refs} signals={signals} />}
        <Viewport hband={hband} vband="top" refs={refs} />
    </Pane>;

export const BodyPane = ({ hband, children, refs, signals }: {
    hband: HBand,
    children?: any,
    refs: GridLayoutRefs,
    signals: PaneSignalSet
}) =>
    <Pane hband={hband} vband="body" refs={refs} signals={signals}>
        {children ?? <Viewport hband={hband} vband="body" refs={refs} />}
    </Pane>;

export const BottomPane = ({ hband, footerRow, refs, signals }: {
    hband: HBand,
    footerRow?: any,
    refs: GridLayoutRefs,
    signals: PaneSignalSet
}) =>
    <Pane hband={hband} vband="bottom" refs={refs} signals={signals}>
        {footerRow ?? <FooterRow hband={hband} refs={refs} signals={signals} />}
        <Viewport hband={hband} vband="bottom" refs={refs} />
    </Pane>;
