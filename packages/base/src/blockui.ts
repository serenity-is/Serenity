let blockUICount: number = 0;

/**
 * Tries to block the page
 */
export function blockUI(options?: { zIndex?: number, useTimeout?: boolean }) {

    function block() {
        if (blockUICount++ > 0 ||
            typeof document === "undefined")
            return;
    
        var div = document.createElement("div");
        div.className = "blockUI blockOverlay";
        div.setAttribute("style", `z-index: ${options?.zIndex ?? 2000}; border: none; margin: 0px; padding: 0px; width: 100%; height: 100%; top: 0px; left: 0px; opacity: 0; cursor: wait; position: fixed;`);
        document.body.appendChild(div);
    }
    
    (options?.useTimeout && (window.setTimeout(block, 0))) || block();
}

/**
 * Unblocks the page. 
 */
export function blockUndo() {
    if (blockUICount < 1)
        return;
    if (--blockUICount === 0 && typeof document !== "undefined")
        document.body.querySelector(':scope > .blockUI.blockOverlay')?.remove();
}