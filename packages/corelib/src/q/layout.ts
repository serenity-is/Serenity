import { Config } from "./config";
import { executeEverytimeWhenVisible } from "./layouttimer";
import { Router } from "./router";
import { getNested, getGlobalThis, initializeTypes } from "./system";
import $ from "@optionaldeps/jquery";

export function autoFullHeight(element: JQuery) {
    element.css('height', '100%');
    triggerLayoutOnShow(element);
}

export function initFullHeightGridPage(gridDiv: JQuery, opt?: { noRoute?: boolean }) {
    $('body').addClass('full-height-page');
    gridDiv.addClass('responsive-height');

    let layout = function () {
        let inPageContent = gridDiv.parent().hasClass('page-content') ||
            gridDiv.parent().is('section.content');

        if (inPageContent) {
            gridDiv.css('height', '1px').css('overflow', 'hidden');
        }

        layoutFillHeight(gridDiv);

        if (inPageContent) {
            gridDiv.css('overflow', '');
        }

        gridDiv.triggerHandler('layout');
    };

    if ($('body').hasClass('has-layout-event')) {
        $('body').bind('layout', layout);
    }
    else if ((window as any).Metronic) {
        (window as any).Metronic.addResizeHandler(layout);
    }
    else {
        $(window).resize(layout);
    }

    layout();

    gridDiv.one('remove', () => {
        $(window).off('layout', layout);
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

export function setMobileDeviceMode() {
    let isMobile = typeof window !== 'undefined' &&
        (window.matchMedia?.('(max-width: 767px)')?.matches ||
         window.innerWidth < 768);

    if (typeof document === "undefined" || !document.documentElement)
        return;

    if (document.documentElement.classList.contains('mobile-device')) {
        if (!isMobile) {
            document.documentElement.classList.remove('mobile-device');
        }
    }
    else if (isMobile) {
        document.documentElement.classList.add('mobile-device');
    }
}

setMobileDeviceMode();

function initOnLoad() {
    var globalObj = getGlobalThis();
    if (globalObj && Config.rootNamespaces) {
        for (var ns of Config.rootNamespaces) {
            var obj = getNested(globalObj, ns);
            if (obj != null)
                initializeTypes(obj, ns + ".", 3);
        }
    }

    if (typeof window !== "undefined") {
        if (typeof $ !== "undefined")
            $(window).bind('resize', setMobileDeviceMode);
        else
            window.addEventListener('resize', setMobileDeviceMode);
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
    if (!el.hasClass("ui-dialog"))
        el = el.closest(".ui-dialog");

    (el as any).position?.({ at: 'center center', of: window });
    let pos = el.position();
    if (pos.left < 0)
        el.css("left", "0px");
    if (pos.top < 0)
        el.css("top", "0px");
}