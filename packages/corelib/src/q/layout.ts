import sQuery from "@optionaldeps/squery";
import { Config, getGlobalObject, getNested, isArrayLike } from "@serenity-is/base";
import { executeEverytimeWhenVisible } from "./layouttimer";
import { Router } from "./router";
import { initializeTypes } from "./system-compat";
   
export function initFullHeightGridPage(gridDiv: HTMLElement | ArrayLike<HTMLElement> | { node: HTMLElement }, opt?: { noRoute?: boolean, setHeight?: boolean }) {
    var el: HTMLElement = isArrayLike(gridDiv) ? gridDiv[0] : gridDiv instanceof HTMLElement ? gridDiv : gridDiv.node;
    document.documentElement.classList.add('full-height-page');
    el.classList.add('responsive-height');

    let setHeight = opt?.setHeight ?? (!sQuery.isMock && (!el.classList.contains('s-DataGrid') &&
        !el.classList.contains('s-Panel')));

    let layout = function () {
        setHeight && layoutFillHeight(sQuery(el));
        !sQuery.isMock ? sQuery(el).triggerHandler('layout') : el.dispatchEvent(new Event('layout'));
    };

    if (document.body.classList.contains('has-layout-event')) {
        !sQuery.isMock ? sQuery(document.body).on('layout', layout) : el.addEventListener('layout', layout);
    }
    else if ((window as any).Metronic?.addResizeHandler) {
        (window as any).Metronic.addResizeHandler(layout);
    }
    else {
        !sQuery.isMock ? sQuery(window).resize(layout) : window.addEventListener('resize', layout);
    }

    layout();

    !sQuery.isMock && sQuery(el).one('remove', () => {
        sQuery(window).off('resize', layout);
        sQuery('body').off('layout', layout);
    });

    if (!opt?.noRoute && 
        typeof document !== "undefined" &&
        !document.body?.getAttribute?.('data-fhrouteinit')) {
            document.body?.setAttribute?.('data-fhrouteinit', 'true');
            // ugly, but to it is to make old pages work without having to add this
            typeof Router !== "undefined" && Router.resolve?.();
    }
}

export function layoutFillHeightValue(element: JQuery) {
    let h = 0;
    element.parent().children().not(element).each(function (i, e) {
        let q = sQuery(e);
        if (q.is(':visible')) {
            h += q.outerHeight(true);
        }
    });
    h = element.parent().height() - h;
    if (element.css('box-sizing') !== 'border-box') {
        h = h - (element.outerHeight(true) - element.height());
    }
    return h;
}

export function layoutFillHeight(element: JQuery) {
    let h = layoutFillHeightValue(element);
    let n = Math.round(h) + 'px';
    if (element.css('height') != n) {
        element.css('height', n);
    }
}

export function isMobileView() {
    return typeof window !== 'undefined' &&
        (window.matchMedia?.('(max-width: 767px)')?.matches ??
         window.innerWidth < 768);
}

function initOnLoad() {
    if (!Config.rootNamespaces)
        return;
    for (var ns of Config.rootNamespaces) {
        var obj = getNested(getGlobalObject(), ns);
        if (obj != null)
            initializeTypes(obj, ns + ".", 3);
    }
}

sQuery(initOnLoad);

export function triggerLayoutOnShow(element: JQuery) {
    executeEverytimeWhenVisible(element, function () {
        element.triggerHandler('layout');
    }, true);
}

export function centerDialog(el: JQuery) {
    el = el.closest(".ui-dialog");
    (el as any).position?.({ at: 'center center', of: window });
    let pos = el.position();
    if (pos.left < 0)
        el.css("left", "0px");
    if (pos.top < 0)
        el.css("top", "0px");
}