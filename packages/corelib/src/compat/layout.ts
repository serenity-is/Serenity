import { addDisposingListener } from "@serenity-is/domwise";
import { Fluent, getjQuery, isArrayLike } from "../base";
import { type CreateWidgetParams, type Widget, type WidgetProps } from "../ui/widgets/widget";
import { executeEverytimeWhenVisible, LayoutTimer } from "./layouttimer";
import { Router } from "./router";

function initWidgetPage<TWidget extends Widget<P>, P>(widgetOrType: (CreateWidgetParams<TWidget, P>["type"]) | TWidget,
    props?: WidgetProps<P>, defaultElement?: string, noRoute?: boolean): TWidget {
    let widget: TWidget;

    if ((widgetOrType as Widget)?.domNode) {
        if (props && typeof props.element === "function") {
            props.element((widgetOrType as Widget).domNode);
        }
        widget = widgetOrType as TWidget;
    }
    else {
        props ??= {} as any;
        let oldFunction: (el: HTMLElement) => void;
        if (defaultElement) {
            if (typeof props.element === "function") {
                oldFunction = props.element;
                props.element = defaultElement;
            }
            else {
                props.element ??= defaultElement;
            }
        }

        widget = new (widgetOrType as CreateWidgetParams<TWidget, P>["type"])(props) as TWidget
        oldFunction?.(widget.domNode);
        widget.init();
    }
    initFullHeightGridPage(widget.domNode, { setHeight: false, noRoute: noRoute });
    return widget;
}

/**
 * Initializes a full-height grid page from a widget class and props object.
 * Compat shim for the legacy `GridPageInit` global; wraps {@link initWidgetPage} with `#GridDiv` as the default container.
 * @param type - The widget class to instantiate.
 * @param props - Optional widget properties passed to the widget constructor.
 * @returns The root {@link HTMLElement} (`domNode`) of the initialized grid widget.
 * @deprecated Prefer calling `initWidgetPage` / `gridPageInit` directly or using the modern `Fluent` / widget APIs. Kept for backward compatibility with pre-corelib page scripts.
 */
export function GridPageInit<TGrid extends Widget<P>, P>({ type, props }: { type: CreateWidgetParams<TGrid, P>["type"], props?: WidgetProps<P> }) {
    return initWidgetPage(type, props, "#GridDiv").domNode;
}

/**
 * Initializes a full-height panel page from a widget class and props object.
 * Compat shim for the legacy `PanelPageInit` global; wraps {@link initWidgetPage} with `#Panel` as the default container.
 * @param type - The panel widget class to instantiate.
 * @param props - Optional widget properties.
 * @returns The root {@link HTMLElement} of the initialized panel widget.
 * @deprecated Prefer `panelPageInit` or direct widget construction. Kept for backward compatibility.
 */
export function PanelPageInit<TPanel extends Widget<P>, P>({ type, props }: { type: CreateWidgetParams<TPanel, P>["type"], props?: WidgetProps<P> }) {
    return initWidgetPage(type, props, "#Panel", false).domNode;
}

/**
 * Initializes a Serenity grid page that fills the available viewport height.
 * Compat shim for the legacy `Q.gridPageInit` / `Serenity.gridPageInit` API. Accepts either an existing widget instance or a widget class + props.
 * @param grid - An existing grid widget instance (must expose `domNode`).
 * @returns The same grid widget after full-height layout initialization.
 * @deprecated Use widget construction with {@link initFullHeightGridPage} or modern layout components. Kept for legacy page scripts.
 */
export function gridPageInit<TGrid extends Widget<P>, P>(grid: TGrid & { domNode: HTMLElement }): TGrid;
/**
 * Initializes a Serenity grid page that fills the available viewport height.
 * @param type - Grid widget class to instantiate.
 * @param props - Optional widget properties (supports `element` as selector or callback).
 * @returns The newly created and initialized grid widget.
 */
export function gridPageInit<TGrid extends Widget<P>, P>(type: CreateWidgetParams<TGrid, P>["type"], props?: WidgetProps<P>): TGrid;
export function gridPageInit<TGrid extends Widget<P>, P>(gridOrType: (CreateWidgetParams<TGrid, P>["type"]) | TGrid, props?: WidgetProps<P>): TGrid {
    return initWidgetPage(gridOrType, props, "#GridDiv");
}

/**
 * Initializes a Serenity panel page without hash-router integration.
 * Compat shim for the legacy `Q.panelPageInit` / `Serenity.panelPageInit` API. Accepts either an existing panel instance or a widget class + props.
 * @param panel - An existing panel widget instance (must expose `domNode`).
 * @returns The same panel widget after layout initialization (`noRoute: true`).
 * @deprecated Use direct widget construction with {@link initFullHeightGridPage}. Kept for legacy compatibility.
 */
export function panelPageInit<TGrid extends Widget<P>, P>(panel: TGrid & { domNode: HTMLElement }): TGrid;
/**
 * Initializes a Serenity panel page without hash-router integration.
 * @param type - Panel widget class to instantiate.
 * @param props - Optional widget properties.
 * @returns The newly created and initialized panel widget.
 */
export function panelPageInit<TGrid extends Widget<P>, P>(type: CreateWidgetParams<TGrid, P>["type"], props?: WidgetProps<P>): TGrid;
export function panelPageInit<TGrid extends Widget<P>, P>(panelOrType: (CreateWidgetParams<TGrid, P>["type"]) | TGrid, props?: WidgetProps<P>): TGrid {
    return initWidgetPage(panelOrType, props, "#PanelDiv", true);
}

/**
 * Configures a full-height page layout for a grid or panel container.
 * Compat shim for the legacy `Q.initFullHeightGridPage`. Adds `full-height-page` / `responsive-height` classes, wires resize or `layout` events, and optionally resolves the hash router.
 * @param gridDiv - Target container: an {@link HTMLElement}, array-like collection, or an object with a `domNode` property.
 * @param opt - Layout options.
 * @param opt.noRoute - When `true`, skips the one-time {@link Router}.`resolve()` call on initial page load. Defaults to `false`.
 * @param opt.setHeight - When `true` forces height filling via {@link layoutFillHeight}; when `false` disables it; when omitted auto-detects via jQuery and element classes. Defaults to auto.
 * @deprecated Prefer CSS flex / grid layouts or `Fluent` responsive utilities. Kept for legacy full-height pages.
 */
export function initFullHeightGridPage(gridDiv: HTMLElement | ArrayLike<HTMLElement> | { domNode: HTMLElement }, opt?: { noRoute?: boolean, setHeight?: boolean }) {
    const el: HTMLElement = isArrayLike(gridDiv) ? gridDiv[0] : gridDiv instanceof HTMLElement ? gridDiv : gridDiv.domNode;
    document.documentElement.classList.add('full-height-page');
    el.classList.add('responsive-height');

    let setHeight = opt?.setHeight ?? (getjQuery() && (!el.classList.contains('s-DataGrid') &&
        !el.classList.contains('s-Panel')));

    let layout = function () {
        setHeight && layoutFillHeight(el);
        Fluent.trigger(el, 'layout');
    };
    let layoutTimerKey: number;

    if (document.body.classList.contains('has-layout-event')) {
        Fluent.on(document.body, 'layout', layout);
    }
    else if ((window as any).Metronic?.addResizeHandler) {
        (window as any).Metronic.addResizeHandler(layout);
    }
    else {
        layoutTimerKey = LayoutTimer.onSizeChange(() => window, layout, { debounceTimes: 1 });
    }

    layout();

    addDisposingListener(el, () => {
        LayoutTimer.off(layoutTimerKey);
        Fluent.off(document.body, 'layout', layout);
    });

    if (!opt?.noRoute &&
        typeof document !== "undefined" &&
        !document.body?.getAttribute?.('data-fhrouteinit')) {
        document.body?.setAttribute?.('data-fhrouteinit', 'true');
        // ugly, but to it is to make old pages work without having to add this
        typeof Router !== "undefined" && Router.resolve?.();
    }
}

/**
 * Calculates the available height for an element to fill its parent.
 * Compat shim for `Q.layoutFillHeightValue`. Sums the outer heights of visible siblings and subtracts from the parent height, adjusting for `box-sizing`.
 * @param element - Target element or array-like collection (first element is used).
 * @returns The computed fill height in pixels (rounded from computed styles). Returns `0` if the element is not found.
 * @deprecated Use CSS flexbox or `calc()` based layouts. Kept for legacy height calculations that depend on jQuery.
 */
export function layoutFillHeightValue(element: HTMLElement | ArrayLike<HTMLElement>) {
    let h = 0;
    let $ = getjQuery();
    element = isArrayLike(element) ? element[0] : element
    if (!$ || !element)
        return element ? parseInt(getComputedStyle(element).height, 10) : 0;

    $(element).parent().children().not(element).each(function (i: number, e: HTMLElement) {
        let q = $(e);
        if (q.is(':visible')) {
            h += q.outerHeight(true);
        }
    });
    h = $(element).parent().height() - h;
    if ($(element).css('box-sizing') !== 'border-box') {
        h = h - ($(element).outerHeight(true) - $(element).height());
    }
    return h;
}

/**
 * Sets an element's height to fill the remaining vertical space in its parent.
 * Compat shim for `Q.layoutFillHeight`. Computes the value via {@link layoutFillHeightValue} and applies it as an inline `height` style.
 * @param element - Target element or array-like collection (first element is used).
 * @deprecated Prefer CSS flex / grid layouts. Kept for legacy full-height grid pages.
 */
export function layoutFillHeight(element: HTMLElement | ArrayLike<HTMLElement>) {
    let h = layoutFillHeightValue(element);
    element = isArrayLike(element) ? element[0] : element;
    if (!element || !isFinite(h))
        return;
    let n = Math.round(h) + 'px';
    if (element.style.height != n)
        element.style.height = n;
}

/**
 * Determines whether the current viewport is considered a mobile view.
 * Compat helper wrapping `window.matchMedia('(max-width: 767px)')` with a fallback to `window.innerWidth < 768`.
 * @returns `true` if the viewport width is at most 767 px; otherwise `false`.
 */
export function isMobileView() {
    return typeof window !== 'undefined' &&
        (window.matchMedia?.('(max-width: 767px)')?.matches ??
            window.innerWidth < 768);
}

/**
 * Triggers a `layout` event each time the element becomes visible.
 * Compat shim for `Q.triggerLayoutOnShow`. Uses {@link executeEverytimeWhenVisible} to fire `Fluent.trigger(element, 'layout')` on visibility transitions.
 * @param element - Target element or array-like collection (first element is used). No-op if the element is missing.
 */
export function triggerLayoutOnShow(element: HTMLElement | ArrayLike<HTMLElement>) {
    element = isArrayLike(element) ? element[0] : element;
    if (!element)
        return;
    executeEverytimeWhenVisible(element, function () {
        Fluent.trigger(element as any, 'layout');
    }, true);
}

/**
 * Centers a jQuery UI dialog containing the given element within the viewport.
 * Compat shim for `Q.centerDialog`. Requires jQuery and jQuery UI `position`; clamps negative `left` / `top` to `0`.
 * @param el - An element inside the dialog (e.g., `.ui-dialog-content`) or the dialog element itself; array-like collections use the first element.
 * @deprecated Prefer native dialog centering or Bootstrap modal positioning. Kept for legacy jQuery UI dialogs.
 */
export function centerDialog(el: HTMLElement | ArrayLike<HTMLElement>) {
    el = isArrayLike(el) ? el[0] : el;
    if (!el)
        return;
    let dlg = el.closest(".ui-dialog") as any;
    if (!dlg)
        return;
    let $ = getjQuery();
    if (!$)
        return;
    dlg = $(dlg) as any;
    dlg.position?.({ at: 'center center', of: window });
    let pos = dlg.position?.();
    if (pos?.left < 0)
        dlg.css("left", "0px");
    if (pos?.top < 0)
        dlg.css("top", "0px");
}