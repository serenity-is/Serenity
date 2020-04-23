
namespace Q {

    interface LayoutTimerReg {
        key: string;
        handler: () => void;
    }

    export namespace LayoutTimer {

        var timeout: number;
        var regs: LayoutTimerReg[] = [];

        function startTimer() {
            if (timeout == null && regs.length) {
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
            for (var reg of regs) {
                try {
                    reg.handler();
                }
                catch (e) {
                    console.log(e);
                }
            }

            clearTimer();
            startTimer();
        }

        export function on(key: string, handler: () => void): () => void {
            if (handler == null)
                throw "Layout handler can't be null!";

            if (key != null && Q.any(regs, x => x.key === key))
                throw "There is already a registered layout handler with key: " + key;

            regs.push({
                key: key,
                handler: handler,
            });

            startTimer();

            return handler;
        }

        export function onSizeChange(key: string, element: HTMLElement, handler: () => void): () => void {
            var oldWidth = element.offsetWidth;
            var oldHeight = element.offsetHeight;

            on(key, () => {
                var offsetWidth = element.offsetWidth;
                var offsetHeight = element.offsetHeight;
                if (offsetWidth !== oldWidth ||
                    offsetHeight !== oldHeight) {
                    oldWidth = offsetWidth;
                    oldHeight = offsetHeight;
                    handler();
                }
            });

            return handler;
        }

        export function onWidthChange(key: string, element: HTMLElement, handler: () => void): () => void {
            var oldWidth = element.offsetWidth;

            on(key, () => {
                var offsetWidth = element.offsetWidth;
                if (offsetWidth !== oldWidth) {
                    oldWidth = offsetWidth;
                    handler();
                }
            });

            return handler;
        }

        export function onHeightChange(key: string, element: HTMLElement, handler: () => void): () => void {
            var oldHeight = element.offsetHeight;

            on(key, () => {
                var offsetHeight = element.offsetHeight;
                if (offsetHeight !== oldHeight) {
                    oldHeight = offsetHeight;
                    handler();
                }
            });

            return handler;
        }

        export function off(key: string, handler?: () => void): void {
            if (key != null)
                regs = regs.filter(x => x.key !== key);
            if (handler != null)
                regs = regs.filter(x => x.handler === handler);
            !regs.length && this.clearTimer();
        }
    }
}