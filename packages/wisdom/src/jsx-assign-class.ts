import { className } from "./classname";
import { isSignalLike, observeSignal } from "./signal-util";
import { isArrayLike, isObject } from "./util";

function unsignalizePrevClass(prev: any): any {
    if (prev == null)
        return prev;

    if (isArrayLike(prev)) {
        prev = Array.from(prev).map(item => {
            if (isSignalLike(item))
                return item.peek();
            return item;
        });
    }
    else if (isObject(prev)) {
        prev = { ...prev };
        Object.entries(prev).forEach(([key, val]) => {
            if (isSignalLike(val))
                prev[key] = val.peek();
        });
    }

    return prev;
}

function clearPrevClass(node: Element & HTMLOrSVGElement, prev?: any): void {
    if (prev == null || prev === false || prev === true)
        return;

    if (typeof prev === "function") {
        node.setAttribute("class", "");
        return;
    }

    prev = unsignalizePrevClass(prev);

    const prevClassNames = (className(prev) ?? "").split(" ");
    for (let cls of prevClassNames) {
        if (cls) {
            node.classList.remove(cls);
        }
    }
}

export function assignClass(node: Element & HTMLOrSVGElement, value?: any, prev?: any): void {
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

    prev = unsignalizePrevClass(prev);

    if (isArrayLike(value)) {
        value = Array.from(value).map(x => {
            let val = x;
            if (isSignalLike(x)) {
                observeSignal(x, args => {
                    if (args.isInitial) {
                        val = args.newValue;
                        return;
                    }
                    applyClassName(node, args.newValue, args.prevValue);
                }, {
                    lifecycleNode: node
                });
            }
            return val;
        });
    }
    else if (isObject(value)) {
        value = { ...value };
        Object.entries(value).forEach(([key, val]) => {
            if (isSignalLike(val)) {
                observeSignal(val, args => {
                    if (args.isInitial) {
                        value[key] = args.newValue;
                        return;
                    }
                    applyClassName(node, Boolean(args.newValue) && key, Boolean(args.prevValue) && key)
                }, {
                    lifecycleNode: node
                });
            }
        });
    }

    applyClassName(node, value, prev);
}

function applyClassName(node: Element & HTMLOrSVGElement, value: any, prev?: any): void {
    const newList = (className(value) ?? "").split(" ");
    if (prev) {
        const prevList = (className(prev) ?? "").split(" ");
        for (let cls of prevList) {
            if (cls && !newList.includes(cls)) {
                node.classList.remove(cls);
            }
        }
    }
    for (let cls of newList) {
        cls && node.classList.add(cls);
    }
}