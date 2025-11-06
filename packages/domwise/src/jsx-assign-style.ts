import type { JSXElement } from "../types";
import { isSignalLike, observeSignal } from "./signal-util";
import { isNumber, isObject, isString } from "./util";

/**
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found on
 * https://github.com/facebook/react/blob/b87aabdfe1b7461e7331abb3601d9e6bb27544bc/LICENSE
 */

/**
 * @param prefix vendor-specific prefix, eg: Webkit
 * @param key style name, eg: transitionDuration
 * @return style name prefixed with `prefix`, properly camelCased, eg:
 * WebkitTransitionDuration
 */
function prefixKey(prefix: string, key: string) {
    return prefix + key.charAt(0).toUpperCase() + key.substring(1)
}

/**
 * CSS properties which accept numbers but are not in units of "px".
 */
let isUnitlessNumber = /*#__PURE__*/ (() => {
    let rec: Record<string, number> = {
        animationIterationCount: 0,
        borderImageOutset: 0,
        borderImageSlice: 0,
        borderImageWidth: 0,
        boxFlex: 0,
        boxFlexGroup: 0,
        boxOrdinalGroup: 0,
        columnCount: 0,
        columns: 0,
        flex: 0,
        flexGrow: 0,
        flexPositive: 0,
        flexShrink: 0,
        flexNegative: 0,
        flexOrder: 0,
        gridArea: 0,
        gridRow: 0,
        gridRowEnd: 0,
        gridRowSpan: 0,
        gridRowStart: 0,
        gridColumn: 0,
        gridColumnEnd: 0,
        gridColumnSpan: 0,
        gridColumnStart: 0,
        fontWeight: 0,
        lineClamp: 0,
        lineHeight: 0,
        opacity: 0,
        order: 0,
        orphans: 0,
        tabSize: 0,
        widows: 0,
        zIndex: 0,
        zoom: 0,

        // SVG-related properties
        fillOpacity: 0,
        floodOpacity: 0,
        stopOpacity: 0,
        strokeDasharray: 0,
        strokeDashoffset: 0,
        strokeMiterlimit: 0,
        strokeOpacity: 0,
        strokeWidth: 0,
    }

    /**
     * Support style names that may come passed in prefixed by adding permutations
     * of vendor prefixes.
     */
    const prefixes = ["Webkit", "ms", "Moz", "O"]

    Object.keys(rec).forEach((prop) => {
        prefixes.forEach((prefix) => {
            rec[prefixKey(prefix, prop)] = 0;
        });
    });

    return rec;
})();

function setStylePropValue(node: JSXElement, key: string, val: any): void {
    if (key.indexOf("-") === 0) {
        // CSS custom properties (variables) start with `-` (e.g. `--my-variable`)
        // and must be assigned via `setProperty`.
        (node as HTMLElement).style.setProperty(key, val)
    } else if (isNumber(val) && isUnitlessNumber[key] !== 0) {
        ((node as HTMLElement).style as any)[key] = val + "px"
    } else {
        ((node as HTMLElement).style as any)[key] = val
    }
}

function clearPrevStyle(node: JSXElement, prev?: any): void {
    if (prev == null || prev === false || prev === true)
        return;

    if (isString(prev)) {
        node.removeAttribute("style");
        return;
    }

    if (isObject(prev)) {
        Object.keys(prev).forEach(key => {
            setStylePropValue(node, key, "");
        });
    }
}

export function assignStyle(node: JSXElement, value?: any, prev?: boolean | any): void {
    if (value == null || value === false || value === true) {
        clearPrevStyle(node, prev);
        return;
    }

    if (isString(value)) {
        node.setAttribute("style", value);
        return;
    }

    if (isObject(value)) {
        if (isObject(prev)) {
            value = { ...value };
            Object.keys(prev).forEach(key => {
                if (!(key in value))
                    value[key] = "";
            });
        }
        else
            clearPrevStyle(node, prev);

        Object.entries(value).forEach(([key, val]) => {
            if (isSignalLike(val)) {
                observeSignal(val, args => setStylePropValue(node, key, args.newValue), {
                    lifecycleNode: node
                });
            }
            else {
                setStylePropValue(node, key, val);
            }
        });
        return;
    }
}
