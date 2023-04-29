import $ from "@optionaldeps/jquery";

/** Options for the BlockUI plugin. */
export interface JQBlockUIOptions {
    useTimeout?: boolean;
}

let blockUICount: number = 0;

function blockUIWithCheck(opt: JQBlockUIOptions) {
    if (blockUICount > 0) {
        blockUICount++;
        return;
    }

    if (($ as any)?.blockUI) {
        ($ as any).blockUI(opt);
    }
    else if (typeof document !== "undefined") {
        var div = document.createElement("div");
        div.className = "blockUI blockOverlay";
        div.setAttribute("style", "z-index: 2000; border: none; margin: 0px; padding: 0px; width: 100%; height: 100%; top: 0px; left: 0px; opacity: 0; cursor: wait; position: fixed;");
        document.body.appendChild(div);
    }
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
    options = Object.assign(<any>{
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

/**
 * Unblocks the page. 
 */
export function blockUndo() {
    if (blockUICount > 1) {
        blockUICount--;
        return;
    }

    blockUICount--;
    if (($ as any)?.unblockUI)
        ($ as any).unblockUI({ fadeOut: 0 });
    else if (typeof document !== "undefined")
        document.body.querySelector(':scope > .blockUI.blockOverlay')?.remove();
}