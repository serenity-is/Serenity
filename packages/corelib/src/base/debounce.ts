export interface DebouncedFunction<T extends (...args: any[]) => any> {
    /**
     * Call the original function, but applying the debounce rules.
     *
     * If the debounced function can be run immediately, this calls it and returns its return
     * value.
     *
     * Otherwise, it returns the return value of the last invocation, or undefined if the debounced
     * function was not invoked yet.
     */
    (...args: Parameters<T>): ReturnType<T> | undefined;

    /**
     * Throw away any pending invocation of the debounced function.
     */
    clear(): void;

    /**
     * If there is a pending invocation of the debounced function, invoke it immediately and return
     * its return value.
     *
     * Otherwise, return the value from the last invocation, or undefined if the debounced function
     * was never invoked.
     */
    flush(): ReturnType<T> | undefined;
}

/**
 * Returns a function, that, as long as it continues to be invoked, will not
 * be triggered. The function also has a property 'clear' that can be used 
 * to clear the timer to prevent previously scheduled executions, and flush method
 * to invoke scheduled executions now if any.
 * @param wait The function will be called after it stops being called for
 * N milliseconds. 
 * @param immediate If passed, trigger the function on the leading edge, instead of the trailing. 
 *
 * @source underscore.js
 */
export function debounce<T extends (...args: any) => any>(func: T, wait?: number, immediate?: boolean): DebouncedFunction<T> {
    var timeout: any, args: any, context: any, timestamp: number, result: any;
    if (null == wait) wait = 100;

    var later = function () {
        var last = Date.now() - timestamp;

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

    var debounced = function () {
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
    };

    return debounced as DebouncedFunction<T>;
};