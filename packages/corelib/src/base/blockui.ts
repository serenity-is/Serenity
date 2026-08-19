let blockUICount: number = 0;
let pendingBlockTimer: any = null;

/**
 * Blocks user interaction by overlaying the page with a transparent, wait-cursor layer.
 *
 * @remarks
 * Maintains an internal reference count so nested `blockUI` / {@link blockUndo} pairs
 * are balanced — the overlay is only removed when the count returns to zero.
 * No-ops on the server (when `document` is undefined). The overlay is a fixed-position
 * `div.blockUI.blockOverlay` appended to `document.body`.
 * @param options - Optional behavior tuning.
 * @param options.zIndex - CSS `z-index` for the overlay. Defaults to `2000`.
 * @param options.useTimeout - When `true`, defers insertion via `setTimeout(…, 0)` and
 * coalesces multiple synchronous calls so only one overlay is created in the next tick.
 * When `false`/`undefined`, the overlay is inserted synchronously.
 * @example
 * blockUI(); // block immediately
 * try {\n *   await save();\n * } finally {\n *   blockUndo();\n * }
 * @example
 * blockUI({ zIndex: 3000, useTimeout: true }); // coalesced, higher z-index
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
 * Decrements the block-UI reference count and removes the overlay when it reaches zero.
 *
 * @remarks
 * Also cancels any pending deferred {@link blockUI} timer scheduled with `useTimeout: true`.
 * No-ops if the count is already zero. Removes the `:scope > .blockUI.blockOverlay` element
 * from `document.body` when the last blocker is undone.
 * @example
 * blockUndo(); // paired with a prior blockUI()
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