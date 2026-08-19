/**
 * A debounced wrapper around a function `T` with helper methods.
 *
 * @typeParam T - The original function type being debounced.
 * @remarks
 * The callable signature applies debounce timing; {@link DebouncedFunction.clear}
 * cancels a pending invocation and {@link DebouncedFunction.flush} forces it to run now.
 * @example
 * const onResize = debounce(() => layout(), 150);
 * window.addEventListener("resize", onResize);
 * onResize.clear(); // cancel pending call
 */
export interface DebouncedFunction<T extends (...args: any[]) => any> {
    /**
     * Invokes the debounced function, applying debounce timing rules.
     *
     * @param args - Arguments forwarded to the original function `T`.
     * @returns Return value of the last immediate invocation, or `undefined` if the call was deferred / never invoked.
     */
    (...args: Parameters<T>): ReturnType<T> | undefined;

    /**
     * Cancels any pending (not yet fired) invocation.
     *
     * @example
     * const fn = debounce(save, 300);
     * fn(); fn.clear(); // save will not run
     */
    clear(): void;

    /**
     * Immediately invokes the pending debounced call (if any) and returns its result.
     *
     * @returns Return value of the flushed invocation, or the last invocation's return value if nothing was pending, or `undefined` if never invoked.
     * @example
     * const fn = debounce(save, 300);
     * fn(); fn.flush(); // save runs now instead of after 300 ms
     */
    flush(): ReturnType<T> | undefined;
}

/**
 * Creates a debounced function that delays invoking `func` until after `wait` ms have elapsed
 * since the last time it was invoked.
 *
 * @remarks
 * When `immediate` is `false` (default), `func` is invoked on the trailing edge after the quiet period.
 * When `immediate` is `true`, `func` is invoked on the leading edge and subsequent calls within
 * `wait` ms are ignored. The returned function exposes {@link DebouncedFunction.clear} to cancel
 * a pending trailing call and {@link DebouncedFunction.flush} to run it immediately. `wait` defaults to `100` ms.
 * @typeParam T - Type of the function to debounce.
 * @param func - Function to debounce.
 * @param wait - Delay in milliseconds to wait after the last call before invoking `func`. Defaults to `100`.
 * @param immediate - If `true`, trigger on the leading edge instead of the trailing edge. Defaults to `false`.
 * @returns Debounced wrapper with `clear` and `flush` helpers.
 * @example
 * const save = debounce(() => api.save(data), 500);
 * save(); save(); // only the last call triggers after 500 ms of quiet
 * @example
 * const track = debounce(() => analytics.send(), 200, true); // leading-edge
 * @example
 * const fn = debounce(() => console.log("hi"), 300);
 * fn(); fn.clear(); // cancels
 * fn(); fn.flush(); // forces immediate invocation
 */
export function debounce<T extends (...args: any) => any>(func: T, wait?: number, immediate?: boolean): DebouncedFunction<T> {
    let timeout: any, args: any, context: any, timestamp: number, result: any;
    if (null == wait) wait = 100;

    const later = function () {
        const last = Date.now() - timestamp;

        if (last < wait && last >= 0) {
            timeout = setTimeout(later, wait - last);
        } else {
            timeout = null;
            if (!immediate)
                result = func.apply(context, args);
            if (!timeout)
                context = args = null;
        }
    };

    const debounced = function () {
        context = this;
        args = arguments;
        timestamp = Date.now();
        if (!timeout) {
            timeout = setTimeout(later, wait);
            if (immediate)
                result = func.apply(context, args);
        }
        return result;
    };

    (debounced as any).clear = function () {
        if (timeout) {
            clearTimeout(timeout);
            timeout = null;
        }
    };

    (debounced as any).flush = function () {
        if (timeout) {
            result = func.apply(context, args);
            context = args = null;

            clearTimeout(timeout);
            timeout = null;
        }

        return result;
    };

    return debounced as DebouncedFunction<T>;
};