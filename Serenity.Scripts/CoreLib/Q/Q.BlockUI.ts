
namespace Q {
    let blockUICount: number = 0;

    function blockUIWithCheck(opt: JQBlockUIOptions) {
        if (blockUICount > 0) {
            blockUICount++;
            return;
        }

        $.blockUI(opt);
        blockUICount++;
    }

    /**
     * Uses jQuery BlockUI plugin to block access to whole page (default) or 
     * a part of it, by using a transparent overlay covering the whole area.
     * @param options Parameters for the BlockUI plugin
     * @remarks If options are not specified, this function blocks 
     * whole page with a transparent overlay. Default z-order of the overlay
     * div is 2000, so a higher z-order shouldn't be used in page.
     */
    export function blockUI(options: JQBlockUIOptions) {
        options = $.extend<JQBlockUIOptions>({
            baseZ: 2000,
            message: '',
            overlayCSS: {
                opacity: '0.0',
                zIndex: 2000,
                cursor: 'wait'
            }, fadeOut: 0
        }, options);

        if (options.useTimeout) {
            window.setTimeout(function () {
                blockUIWithCheck(options);
            }, 0);
        }
        else {
            blockUIWithCheck(options);
        }
    }

    export function blockUndo() {
        if (blockUICount > 1) {
            blockUICount--;
            return;
        }

        blockUICount--;
        $.unblockUI({ fadeOut: 0 });
    }
}