import { Config } from "./config";
import { executeEverytimeWhenVisible } from "./layouttimer";
import { Router } from "./router";
import { getNested, getGlobalThis, initializeTypes } from "./system";
import $ from "@optionaldeps/jquery";
   
export function initFullHeightGridPage(gridDiv: JQuery | HTMLElement, opt?: { noRoute?: boolean, setHeight?: boolean }) {
    var el = ($ && gridDiv instanceof $) ? (gridDiv as JQuery).get(0) : gridDiv as HTMLElement
    document.documentElement.classList.add('full-height-page');
    el.classList.add('responsive-height');

    let setHeight = opt?.setHeight ?? ($ && (!el.classList.contains('s-DataGrid') &&
        !el.classList.contains('s-Panel')));

    let layout = function () {
        setHeight && layoutFillHeight($(el));
        $ ? $(el).triggerHandler('layout') : el.dispatchEvent(new Event('layout'));
    };

    if (document.body.classList.contains('has-layout-event')) {
        $ ? $(document.body).on('layout', layout) : el.addEventListener('layout', layout);
    }
    else if ((window as any).Metronic?.addResizeHandler) {
        (window as any).Metronic.addResizeHandler(layout);
    }
    else {
        $ ? $(window).resize(layout) : window.addEventListener('resize', layout);
    }

    layout();

    $ && $(el).one('remove', () => {
        $(window).off('resize', layout);
        $('body').off('layout', layout);
    });

    if (!opt?.noRoute && 
        typeof document !== "undefined" &&
        !$(document.body).attr('data-fhrouteinit')) {
            $(document.body).attr('data-fhrouteinit', 'true');
            // ugly, but to it is to make old pages work without having to add this
            typeof Router !== "undefined" && Router.resolve?.();
    }
}

export function layoutFillHeightValue(element: JQuery) {
    let h = 0;
    element.parent().children().not(element).each(function (i, e) {
        let q = $(e);
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
    var globalObj = getGlobalThis();
    if (globalObj && Config.rootNamespaces) {
        for (var ns of Config.rootNamespaces) {
            var obj = getNested(globalObj, ns);
            if (obj != null)
                initializeTypes(obj, ns + ".", 3);
        }
    }
}

if (typeof $ !== "undefined")
    $(initOnLoad);
else if (typeof document !== "undefined" && document.readyState === 'loading')
    document.addEventListener('DOMContentLoaded', initOnLoad);
else
    initOnLoad();

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