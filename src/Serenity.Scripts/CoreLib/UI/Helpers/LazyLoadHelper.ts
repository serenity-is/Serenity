namespace Serenity {
    export namespace LazyLoadHelper {
        let autoIncrement = 0;

        export function executeOnceWhenShown(element: JQuery, callback: Function) {
            var el = element && element[0];
            if (!el)
                return;
            
            if (el.offsetWidth > 0 && el.offsetHeight > 0) {
                callback();
                return;
            }

            var timer = Q.LayoutTimer.onShown(() => el, () => {
                Q.LayoutTimer.off(timer);
                callback();
            });
        }

        export function executeEverytimeWhenShown(element: JQuery, callback: Function, callNowIfVisible: boolean) {
            var el = element && element[0];
            if (!el)
                return;
            
            if (callNowIfVisible && el.offsetWidth > 0 && el.offsetHeight > 0) {
                callback();
            }

            Q.LayoutTimer.onShown(() => el, () => {
                callback();
            });
        }
    }
}