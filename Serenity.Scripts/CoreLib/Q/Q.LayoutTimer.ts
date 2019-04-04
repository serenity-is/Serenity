
namespace Q {

    export namespace LayoutTimer {

        var timeout: number;
        var regs: (() => void)[] = [];
        var regByKey: Q.Dictionary<number> = {};

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
                    reg();
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

            if (regByKey[key] !== undefined)
                throw "There is already a registered layout handler with key: " + key;

            if (key != null)
                regByKey[key] = regs.length;

            regs.push(handler);

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
            if (key != null) {
                var index = regByKey[key];
                if (index !== undefined) {
                    delete regByKey[key];
                    if (handler == null || handler === regs[index]) {
                        regs.splice(index, 1);
                        !regs.length && this.clearTimer();
                        return;
                    }
                }
            }

            if (handler != null) {
                for (var l = regs.length - 1; l >= 0; l++) {
                    if (regs[l] === handler) {
                        regs.splice(l, 1);
                        !regs.length && this.clearTimer();
                        break;
                    }
                }
            }
        }
    }
}