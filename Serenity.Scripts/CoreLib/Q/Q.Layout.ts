namespace Q {

    export function autoFullHeight(element: JQuery) {
        element.css('height', '100%');
        triggerLayoutOnShow(element);
    }

    export function initFullHeightGridPage(gridDiv: JQuery) {
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

        // ugly, but to it is to make old pages work without having to add this
        Q.Router.resolve();
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
        let isMobile = navigator.userAgent.indexOf('Mobi') >= 0 ||
            window.matchMedia('(max-width: 767px)').matches;

        let body = $(document.body);
        if (body.hasClass('mobile-device')) {
            if (!isMobile) {
                body.removeClass('mobile-device');
            }
        }
        else if (isMobile) {
            body.addClass('mobile-device');
        }
    }

    setMobileDeviceMode();
    $(function() {
        globalObj && $(globalObj).bind('resize', function () {
            setMobileDeviceMode();
        });
    });

    export function triggerLayoutOnShow(element: JQuery) {
        Serenity.LazyLoadHelper.executeEverytimeWhenShown(element, function () {
            element.triggerHandler('layout');
        }, true);
    }

    export function centerDialog(el: JQuery) {
        if (!el.hasClass("ui-dialog"))
            el = el.closest(".ui-dialog");

        el.position({ at: 'center center', of: window });
        let pos = el.position();
        if (pos.left < 0)
            el.css("left", "0px");
        if (pos.top < 0)
            el.css("top", "0px");
    }
}