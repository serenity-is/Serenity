let blockUICount: number = 0;
let pendingBlockTimer: any = null;

/**
 * Tries to block the page
 */
export function blockUI(options?: { zIndex?: number, useTimeout?: boolean }) {

    function block() {
        pendingBlockTimer = null;
        if (blockUICount++ > 0 ||
            typeof document === "undefined")
            return;

        const div = document.createElement("div");
        div.className = "blockUI blockOverlay";
        Object.assign(div.style, {
            zIndex: (options?.zIndex ?? 2000).toString(),
            border: "none",
            margin: "0px",
            padding: "0px",
            width: "100%",
            height: "100%",
            top: "0px",
            left: "0px",
            opacity: "0",
            cursor: "wait",
            position: "fixed"
        });
        document.body.appendChild(div);
    }

    if (options?.useTimeout) {
        if (pendingBlockTimer != null)
            clearTimeout(pendingBlockTimer);
        pendingBlockTimer = window.setTimeout(block, 0);
    }
    else {
        block();
    }
}

/**
 * Unblocks the page. 
 */
export function blockUndo() {
    if (pendingBlockTimer != null) {
        clearTimeout(pendingBlockTimer);
        pendingBlockTimer = null;
    }
    if (blockUICount < 1)
        return;
    if (--blockUICount === 0 && typeof document !== "undefined")
        document.body.querySelector(':scope > .blockUI.blockOverlay')?.remove();
}