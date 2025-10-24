import type { GridSignals } from "../core";
import type { HBand, VBand } from "./layout-consts";
import type { GridLayoutRefs } from "./layout-refs";

export const Header = ({ hband, refs, signals }: { hband: HBand, refs: GridLayoutRefs, signals: Pick<GridSignals, "hideColumnHeader"> }) => {
    const hRefs = refs[hband];
    return <div class={{ [`slick-header slick-header-${hband}`]: true, "slick-hidden": signals.hideColumnHeader }}>
        <div class={`slick-header-columns slick-header-columns-${hband}`}
            ref={el => { hRefs.headerCols = el }} />
    </div>;
}

export const HeaderRow = ({ hband, refs, signals }: { hband: HBand, refs: GridLayoutRefs, signals: Pick<GridSignals, "hideHeaderRow"> }) => {
    const hRefs = refs[hband];
    return <div class={{ [`slick-headerrow slick-headerrow-${hband}`]: true, "slick-hidden": signals.hideHeaderRow }}>
        <div class={`slick-headerrow-columns slick-headerrow-columns-${hband}`} ref={el => hRefs.headerRowCols = el} />
    </div>
}

export const TopPanel = ({ refs, signals }: { refs: GridLayoutRefs, signals: Pick<GridSignals, "hideTopPanel"> }) => {
    const hRefs = refs["main"];
    return <div class={{ [`slick-top-panel-container`]: true, "slick-hidden": signals.hideTopPanel }}>
        <div class="slick-top-panel" ref={el => hRefs.topPanel = el} />
    </div>;
}

export const Viewport = ({ hband, vband, refs }: { hband: HBand, vband: VBand, refs: GridLayoutRefs }) => {
    const pRefs = refs[hband][vband];
    return <div class={`slick-viewport slick-viewport-${vband} slick-viewport-${hband}`} tabindex="0" ref={el => pRefs.viewport = el}>
        <div class={`grid-canvas grid-canvas-${vband} grid-canvas-${hband}`} tabindex="0" ref={el => pRefs.canvas = el} />
    </div>;
}

export const FooterRow = ({ hband, refs, signals }: { hband: HBand, refs: GridLayoutRefs, signals: Pick<GridSignals, "hideFooterRow"> }) => {
    const hRefs = refs[hband];
    return <div class={{ [`slick-footerrow slick-footerrow-${hband}`]: true, "slick-hidden": signals.hideFooterRow }}>
        <div class={`slick-footerrow-columns slick-footerrow-columns-${hband}`} ref={el => hRefs.footerRowCols = el} />
    </div>
}
/*
export const Pane = ({ hband, vband, children, refs, hidden }: { hband: HBand, vband: VBand, children?: any, refs: PaneRefs, hidden?: SignalOrValue<boolean> }) =>
    <div class={{ "slick-pane": true, [`slick-pane-${vband}`]: true, [`slick-pane-${hband}`]: true, "slick-hidden": hidden }} tabindex="0" ref={el => refs.pane = el}>
        {children}
    </div>;

export const HeaderPane = ({ hband, hide, children, refs }: { hband: HBand, hide: SignalOrValue<boolean>, children?: any, refs: HeaderPaneRefs }) =>
    <Pane hband={hband} vband="header" refs={refs} hidden={hide}>
        {children == null || children.length === 0 ? <Header hband={hband} hide={hide} refs={refs} /> : children}
    </Pane>;



export const TopPane = ({ hband, headerRow, refs }: { hband: HBand, headerRow?: any, refs: TopPaneRefs }) =>
    <Pane hband={hband} vband="top" refs={refs}>
        {headerRow ?? <HeaderRow hband={hband} refs={refs} />}
        <Viewport hband={hband} vband="top" refs={refs} />
    </Pane>;

export const BodyPane = ({ hband, children, refs }: { hband: HBand, children?: any, refs: ViewportPaneRefs }) =>
    <Pane hband={hband} vband="body" refs={refs}>
        {children ?? <Viewport hband={hband} vband="body" refs={refs} />}
    </Pane>;


export const BottomPane = ({ hband, footerRow, refs }: { hband: HBand, footerRow?: any, refs: BottomPaneRefs }) =>
    <Pane hband={hband} vband="bottom" refs={refs}>
        {footerRow ?? <FooterRow hband={hband} refs={refs} />}
        <Viewport hband={hband} vband="bottom" refs={refs} />
    </Pane>;
*/
