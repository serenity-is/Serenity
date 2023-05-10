export {}

interface LayoutTimerReg {
    handler: () => void;
    element: () => HTMLElement;
    width: boolean;
    height: boolean;
    storedWidth?: number;
    storedHeight?: number;
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
            window.clearTimeout(timeout);
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
                try {

                    if ((reg.width && reg.storedWidth !== w) ||
                        (reg.height && reg.storedHeight !== h) ||
                        (!reg.width && !reg.height && (!w !== !reg.storedWidth || !h !== !reg.storedHeight))) {
                            if (w > 0 && h > 0) {
                                try {
                                    reg.handler();
                                }
                                finally {
                                    w = el.offsetWidth;
                                    h = el.offsetHeight;
                                }
                            }
                    }
                }
                finally {
                    reg.storedWidth = w;
                    reg.storedHeight = h;
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
    }

    export function trigger(key: number) {
        var reg = regs[key];
        if (!reg)
            return;
        store(key);
        if (reg.storedWidth >= 0 &&
            reg.storedHeight >= 0) {
            reg.handler();
        }
        store(key);
    }

    export function onSizeChange(element: () => HTMLElement, handler: () => void, width?: boolean, height?: boolean): number {
        if (handler == null)
            throw "Layout handler can't be null!";

        regs[++nextKey] = {
            element: element,
            handler: handler,
            width: width !== false,
            height: height !== false
        }
        regCount++;
        store(nextKey)
        startTimer();
        return nextKey;
    }

    export function onWidthChange(element: () => HTMLElement, handler: () => void) {
        return onSizeChange(element, handler, true, false);
    }

    export function onHeightChange(element: () => HTMLElement, handler: () => void) {
        return onSizeChange(element, handler, false, true);
    }

    export function onShown(element: () => HTMLElement, handler: () => void) {
        return onSizeChange(element, handler, false, false);
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

export function executeOnceWhenVisible(element: JQuery, callback: Function): void {
    var el = element && element[0];
    if (!el)
        return;
    
    if (el.offsetWidth > 0 && el.offsetHeight > 0) {
        callback();
        return;
    }

    var timer = LayoutTimer.onShown(() => el, () => {
        LayoutTimer.off(timer);
        callback();
    });
}

export function executeEverytimeWhenVisible(element: JQuery, callback: Function, callNowIfVisible: boolean): void {
    var el = element && element[0];
    if (!el)
        return;
    
    if (callNowIfVisible && el.offsetWidth > 0 && el.offsetHeight > 0) {
        callback();
    }

    LayoutTimer.onShown(() => el, () => {
        callback();
    });
}
