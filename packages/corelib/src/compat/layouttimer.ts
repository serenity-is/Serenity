import { debounce, isArrayLike } from "../base";

export { };

interface LayoutTimerReg {
    handler: () => void;
    element: () => HTMLElement;
    width: boolean;
    height: boolean;
    storedWidth?: number;
    storedHeight?: number;
    debounceTimes?: number;
    debouncedTimes?: number;
}

export namespace LayoutTimer {

    var timeout: number;
    var nextKey: number = 0;
    var regCount: number = 0;
    var regs: { [key: number]: LayoutTimerReg; } = {};

    function startTimer() {
        if (timeout == null && regCount > 0) {
            timeout = window.setTimeout(onTimeout, 100);
        }
    }

    function clearTimer() {
        if (timeout != null) {
            clearTimeout(timeout);
            timeout = null;
        }
    }

    function onTimeout() {
        for (var key in regs) {
            var reg = regs[key];
            try {
                var el = reg.element();
                if (!el)
                    continue;

                var w = el.offsetWidth;
                var h = el.offsetHeight;
                if (w <= 0 || h <= 0)
                    continue;

                var debounced = false;
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
                            w = el.offsetWidth;
                            h = el.offsetHeight;
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

    export function store(key: number) {
        var reg = regs[key];
        if (!reg)
            return;

        var el = reg.element();
        if (!el)
            return;

        reg.storedWidth = el.offsetWidth;
        reg.storedHeight = el.offsetHeight;
        reg.debouncedTimes = 0;
    }

    export function trigger(key: number) {
        var reg = regs[key];
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

    export function onSizeChange(element: () => HTMLElement, handler: () => void, opt?: { width?: boolean, height?: boolean, debounceTimes?: number }): number {
        if (handler == null)
            throw "Layout handler can't be null!";

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

    export function onWidthChange(element: () => HTMLElement, handler: () => void, opt?: { debounceTimes?: number }) {
        return onSizeChange(element, handler, { width: true, height: false, debounceTimes: opt?.debounceTimes });
    }

    export function onHeightChange(element: () => HTMLElement, handler: () => void, opt?: { debounceTimes?: number }) {
        return onSizeChange(element, handler, { width: false, height: true, debounceTimes: opt?.debounceTimes });
    }

    export function onShown(element: () => HTMLElement, handler: () => void, opt?: { debounceTimes?: number }) {
        return onSizeChange(element, handler, { width: false, height: false, debounceTimes: opt?.debounceTimes });
    }

    export function off(key: number): number {
        var reg = regs[key];
        if (!reg)
            return 0;

        delete regs[key];
        regCount--;
        if (regCount <= 0)
            clearTimer();

        return 0;
    }
}

export function executeOnceWhenVisible(el: HTMLElement | ArrayLike<HTMLElement>, callback: Function): void {
    el = isArrayLike(el) ? el[0] : el;
    if (!el)
        return;
    if (el.offsetWidth > 0 && el.offsetHeight > 0) {
        callback();
        return;
    }

    var timer = LayoutTimer.onShown(() => el as HTMLElement, () => {
        LayoutTimer.off(timer);
        callback();
    });
}

export function executeEverytimeWhenVisible(el: HTMLElement | ArrayLike<HTMLElement>, callback: Function, callNowIfVisible: boolean): void {
    el = isArrayLike(el) ? el[0] : el;
    if (!el)
        return;

    if (callNowIfVisible && el.offsetWidth > 0 && el.offsetHeight > 0) {
        callback();
    }

    LayoutTimer.onShown(() => el as HTMLElement, () => {
        callback();
    });
}
