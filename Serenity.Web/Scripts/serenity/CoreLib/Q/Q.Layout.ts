namespace Q {

    export function addFullHeightResizeHandler(handler: (n: number) => void) {
        $('body').addClass('full-height-page');
        let layout = function () {
            let avail: number;
            try {
                avail = parseInt($('.page-content').css('min-height') || '0')
                    - parseInt($('.page-content').css('padding-top') || '0')
                    - parseInt($('.page-content').css('padding-bottom') || '0');
            }
            catch ($t1) {
                avail = 100;
            }
            handler(avail);
        };

        if ((window as any).Metronic) {
            (window as any).Metronic.addResizeHandler(layout);
        }
        else {
            $(window).resize(layout);
        }

        layout();
    }

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
        let n = h + 'px';
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

    export function triggerLayoutOnShow(element: JQuery) {
        Serenity.LazyLoadHelper.executeEverytimeWhenShown(element, function () {
            element.triggerHandler('layout');
        }, true);
    }
}