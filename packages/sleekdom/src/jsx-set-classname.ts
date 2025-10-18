import { className } from "./classname";
import { isArrayLike, isObject, isSignalLike, isString, isVisibleChild, observeSignal } from "./util";

function clearPrevClass(node: Element & HTMLOrSVGElement, prev?: any): void {
    if (prev == null || prev === false || prev === true)
        return;

    if (isString(prev)) {
        for (let cls of prev.split(" ")) {
            cls && node.classList.remove(cls);
        }
        return;
    }

    if (isObject(prev)) {
        Object.entries(prev).forEach(([key, val]) => {
            if (isSignalLike(val))
                val = val.peek();
            if (Boolean(val))
                node.classList.remove(key);
        });
    }

    if (isArrayLike(prev)) {
        Array.from(prev).forEach(item => {
            if (isSignalLike(item))
                item = item.peek();
            if (isVisibleChild(item)) {
                const cls = String(item);
                if (cls)
                    node.classList.remove(cls);
            }
        });
    }

    if (typeof prev === "function")
        node.setAttribute("class", "");
}

export function setClassName(node: Element & HTMLOrSVGElement, value?: any, prev?: any): void {
    if (value == null || value === false) {
        clearPrevClass(node, prev);
        return;
    }

    if (typeof value === "function") {
        if (prev !== value) {
            clearPrevClass(node, prev);
            value(node);
        }
        return;
    }

    if (isArrayLike(prev) && prev !== value) {
        prev = Array.from(prev).map(x => {
            if (isSignalLike(x)) {
                return x.peek();
            }
            return x;
        });
    }

    if (isArrayLike(value) && Array.from(value).some(x => isSignalLike(x))) {
        value = Array.from(value).filter(x => {
            if (isSignalLike(x)) {
                const dispose = observeSignal(x, (newVal, prev) => {
                    prev && Boolean(prev) && prev !== newVal && node.classList.remove(String(prev));
                    Boolean(newVal) && node.classList.add(String(newVal));
                });
                if (dispose) {
                    node.addEventListener("disposing", dispose, { once: true });
                }
                return false;
            }
            return true;
        });
    }

    if (isObject(prev) && prev !== value && Object.values(prev).some(v => isSignalLike(v))) {
        prev = { ...prev };
        Object.entries(prev).forEach(([key, val]) => {
            if (isSignalLike(val)) {
                delete prev[key];
            }
        });
    }

    if (isObject(value) && Object.values(value).some(v => isSignalLike(v))) {
        value = { ...value };
        Object.entries(value).forEach(([key, val]) => {
            if (isSignalLike(val)) {
                const dispose = observeSignal(val, (newVal) => {
                    node.classList.toggle(key, Boolean(newVal));
                });
                if (dispose) {
                    node.addEventListener("disposing", dispose, { once: true });
                }
                delete value[key];
            }
        });
    }

    const classNames = (className(value) ?? "").split(" ");
    const oldClassNames = (className(prev) ?? "").split(" ");
    for (let cls of oldClassNames) {
        if (cls && !classNames.includes(cls)) {
            node.classList.remove(cls);
        }
    }
    for (let cls of classNames) {
        cls && node.classList.add(cls);
    }
}