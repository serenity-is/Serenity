import { isArrayLike } from "../base";

export { };

interface LayoutTimerReg {
    handler: () => void;
    element: () => (HTMLElement | Window);
    width: boolean;
    height: boolean;
    storedWidth?: number;
    storedHeight?: number;
    debounceTimes?: number;
    debouncedTimes?: number;
}

/**
 * Legacy polling-based layout timer that detects size and visibility changes.
 * Compat shim for the old `Q.LayoutTimer` / `Serenity.LayoutTimer` API. Polls registered elements every ~100 ms,
 * supports optional debouncing, and fires handlers when width, height, or visibility transitions occur.
 * Prefer `ResizeObserver` or `Fluent.on(..., 'layout')` with CSS-based layouts for new code.
 * @deprecated Kept for backward compatibility with legacy `layoutFillHeight` and `triggerLayoutOnShow` callers. Use `ResizeObserver` instead.
 */
export namespace LayoutTimer {

    let timeout: number;
    let nextKey: number = 0;
    let regCount: number = 0;
    const regs: { [key: number]: LayoutTimerReg; } = {};

    function startTimer() {
        if (timeout == null && regCount > 0) {
            timeout = setTimeout(onTimeout, 100);
        }
    }

    function clearTimer() {
        if (timeout != null) {
            clearTimeout(timeout);
            timeout = null;
        }
    }

    function getSize(el: HTMLElement | Window): { width: number, height: number } {
        if (el === window) {
            return {
                width: window.innerWidth,
                height: window.innerHeight
            };
        }
        else {
            return {
                width: (el as HTMLElement).offsetWidth,
                height: (el as HTMLElement).offsetHeight
            };
        }
    }

    function onTimeout() {
        for (const key in regs) {
            const reg = regs[key];
            try {
                const el = reg.element();
                if (!el)
                    continue;

                let { width: w, height: h } = getSize(el);
                if (w <= 0 || h <= 0) {
                    reg.storedWidth = w;
                    reg.storedHeight = h;
                    reg.debouncedTimes = 0;
                    continue;
                }

                let debounced = false;
                try {

                    if ((reg.width && reg.storedWidth !== w) ||
                        (reg.height && reg.storedHeight !== h) ||
                        (!reg.width && !reg.height && (!w !== !reg.storedWidth || !h !== !reg.storedHeight))) {
                        if (reg.debounceTimes > 0 &&
                            ++reg.debouncedTimes <= reg.debounceTimes) {
                            debounced = true;
                            continue;
                        }

                        try {
                            reg.debouncedTimes = 0;
                            reg.handler();
                        }
                        finally {
                            ({ width: w, height: h } = getSize(el));
                        }
                    }
                }
                finally {
                    if (!debounced) {
                        reg.storedWidth = w;
                        reg.storedHeight = h;
                        reg.debouncedTimes = 0;
                    }
                }
            }
            catch (e) {
                console.log(e);
            }
        }
        clearTimer();
        startTimer();
    }

    /**
     * Captures and stores the current size of a registered element without firing its handler.
     * Used to reset the baseline so the next poll compares against the current dimensions.
     * @param key - Registration key returned by {@link onSizeChange} / {@link onShown} / etc.
     */
    export function store(key: number) {
        const reg = regs[key];
        if (!reg)
            return;

        const el = reg.element();
        if (!el)
            return;

        const { width: w, height: h } = getSize(el);
        reg.storedWidth = w;
        reg.storedHeight = h;
        reg.debouncedTimes = 0;
    }

    /**
     * Manually triggers the handler for a registration if the element is currently visible (positive width and height).
     * Re-stores the baseline before and after invoking the handler.
     * @param key - Registration key returned by {@link onSizeChange}.
     */
    export function trigger(key: number) {
        const reg = regs[key];
        if (!reg)
            return;
        store(key);
        if (reg.storedWidth > 0 &&
            reg.storedHeight > 0) {
            reg.handler();
            reg.debouncedTimes = 0;
        }
        store(key);
    }

    /**
     * Registers a handler invoked when the size of the element returned by `element()` changes.
     * Polls via an internal timer; supports filtering by width / height and optional debouncing.
     * @param element - Factory returning the target `HTMLElement` or `Window` to watch.
     * @param handler - Callback invoked when a matching size change is detected.
     * @param opt - Watch options.
     * @param opt.width - When `false`, width changes are ignored. Defaults to `true`.
     * @param opt.height - When `false`, height changes are ignored. Defaults to `true`.
     * @param opt.debounceTimes - Number of polls to debounce before firing. `0` fires immediately. Defaults to `0`.
     * @returns A numeric registration key that can be passed to {@link store}, {@link trigger}, or {@link off}.
     */
    export function onSizeChange(element: () => (HTMLElement | Window), handler: () => void, opt?: { width?: boolean, height?: boolean, debounceTimes?: number }): number {
        if (handler == null)
            throw new Error("Layout handler can't be null!");

        regs[++nextKey] = {
            element: element,
            handler: handler,
            width: opt?.width !== false,
            height: opt?.height !== false,
            debounceTimes: opt?.debounceTimes || 0,
            debouncedTimes: 0
        }
        regCount++;
        store(nextKey)
        startTimer();
        return nextKey;
    }

    /**
     * Registers a handler invoked only when the width of the element changes.
     * Convenience wrapper around {@link onSizeChange} with `height: false`.
     * @param element - Factory returning the target `HTMLElement`.
     * @param handler - Callback invoked on width change.
     * @param opt - Optional debounce configuration.
     * @param opt.debounceTimes - Number of polls to debounce before firing.
     * @returns A numeric registration key.
     */
    export function onWidthChange(element: () => HTMLElement, handler: () => void, opt?: { debounceTimes?: number }) {
        return onSizeChange(element, handler, { width: true, height: false, debounceTimes: opt?.debounceTimes });
    }

    /**
     * Registers a handler invoked only when the height of the element changes.
     * Convenience wrapper around {@link onSizeChange} with `width: false`.
     * @param element - Factory returning the target `HTMLElement`.
     * @param handler - Callback invoked on height change.
     * @param opt - Optional debounce configuration.
     * @param opt.debounceTimes - Number of polls to debounce before firing.
     * @returns A numeric registration key.
     */
    export function onHeightChange(element: () => HTMLElement, handler: () => void, opt?: { debounceTimes?: number }) {
        return onSizeChange(element, handler, { width: false, height: true, debounceTimes: opt?.debounceTimes });
    }

    /**
     * Registers a handler invoked when the element becomes visible (transitions from zero to non-zero size).
     * Wrapper around {@link onSizeChange} with both `width` and `height` set to `false` so only hidden-to-visible transitions fire.
     * @param element - Factory returning the target `HTMLElement`.
     * @param handler - Callback invoked when the element is shown.
     * @param opt - Optional debounce configuration.
     * @param opt.debounceTimes - Number of polls to debounce before firing.
     * @returns A numeric registration key.
     */
    export function onShown(element: () => HTMLElement, handler: () => void, opt?: { debounceTimes?: number }) {
        return onSizeChange(element, handler, { width: false, height: false, debounceTimes: opt?.debounceTimes });
    }

    /**
     * Unregisters a handler previously registered with {@link onSizeChange} / {@link onWidthChange} / {@link onHeightChange} / {@link onShown}.
     * Stops the internal polling timer when no registrations remain.
     * @param key - Registration key to remove.
     * @returns `0` for compatibility with the legacy API.
     */
    export function off(key: number): number {
        const reg = regs[key];
        if (!reg)
            return 0;

        delete regs[key];
        regCount--;
        if (regCount <= 0)
            clearTimer();

        return 0;
    }
}

/**
 * Executes a callback once when the element becomes visible.
 * If the element is already visible (positive `offsetWidth` / `offsetHeight`), the callback is invoked immediately and `null` is returned.
 * Otherwise registers via {@link LayoutTimer.onShown} and auto-unregisters after the first fire.
 * @param el - Target element or array-like collection (first element is used).
 * @param callback - Function to invoke when visible.
 * @returns The {@link LayoutTimer} registration key, or `null` if already visible / element missing.
 * @deprecated Prefer `IntersectionObserver` or `ResizeObserver`. Kept for legacy `triggerLayoutOnShow` compatibility.
 */
export function executeOnceWhenVisible(el: HTMLElement | ArrayLike<HTMLElement>, callback: Function): number | null {
    el = isArrayLike(el) ? el[0] : el;
    if (!el)
        return null;
    if (el.offsetWidth > 0 && el.offsetHeight > 0) {
        callback();
        return null;
    }

    const timer = LayoutTimer.onShown(() => el as HTMLElement, () => {
        LayoutTimer.off(timer);
        callback();
    });
    return timer;
}

/**
 * Executes a callback every time the element becomes visible.
 * Unlike {@link executeOnceWhenVisible}, the registration persists and fires on each hidden-to-visible transition.
 * @param el - Target element or array-like collection (first element is used).
 * @param callback - Function to invoke each time the element is shown.
 * @param callNowIfVisible - When `true` and the element is already visible, invokes the callback immediately before registering.
 * @returns The {@link LayoutTimer} registration key, or `null` if the element is missing.
 * @deprecated Prefer `IntersectionObserver` / `ResizeObserver`. Kept for legacy `triggerLayoutOnShow` compatibility.
 */
export function executeEverytimeWhenVisible(el: HTMLElement | ArrayLike<HTMLElement>, callback: Function, callNowIfVisible: boolean): number | null {
    el = isArrayLike(el) ? el[0] : el;
    if (!el)
        return null;

    if (callNowIfVisible && el.offsetWidth > 0 && el.offsetHeight > 0) {
        callback();
    }

    const timer = LayoutTimer.onShown(() => el as HTMLElement, () => {
        callback();
    });
    return timer;
}
