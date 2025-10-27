import type { Position } from "../core/editing";
import { parsePx } from "../core/util";

// shared across all grids on the page
let maxSupportedCssHeight: number;  // browser's breaking point
let scrollbarDimensions: { width: number, height: number };

export function absBox(elem: HTMLElement): Position {
    var box: Position = {
        top: elem.offsetTop,
        left: elem.offsetLeft,
        bottom: 0,
        right: 0,
        width: elem.offsetWidth,
        height: elem.offsetHeight,
        visible: true
    };

    box.bottom = box.top + box.height;
    box.right = box.left + box.width;

    // walk up the tree
    var offsetParent = elem.offsetParent;
    while ((elem = elem.parentNode as HTMLElement) != document.body && elem != null) {
        if (box.visible && elem.scrollHeight != elem.offsetHeight && getComputedStyle(elem).overflowY !== "visible") {
            box.visible = box.bottom > elem.scrollTop && box.top < elem.scrollTop + elem.clientHeight;
        }

        if (box.visible && elem.scrollWidth != elem.offsetWidth && getComputedStyle(elem).overflowX != "visible") {
            box.visible = box.right > elem.scrollLeft && box.left < elem.scrollLeft + elem.clientWidth;
        }

        box.left -= elem.scrollLeft;
        box.top -= elem.scrollTop;

        if (elem === offsetParent) {
            box.left += elem.offsetLeft;
            box.top += elem.offsetTop;
            offsetParent = elem.offsetParent;
        }

        box.bottom = box.top + box.height;
        box.right = box.left + box.width;
    }

    return box;
}

export function getInnerWidth(el: HTMLElement): number {
    var style = getComputedStyle(el);
    var width = parsePx(style.width) ?? 0;
    if (style.boxSizing != 'border-box')
        return Math.max(0, width);

    var p = ["border-left-width", "border-right-width", "padding-left", "padding-right"];
    for (var val of p)
        width -= parsePx(style.getPropertyValue(val)) || 0;

    return Math.max(width, 0);
}

export function getMaxSupportedCssHeight(recalc?: boolean): number {
    if (!recalc && maxSupportedCssHeight != null)
        return maxSupportedCssHeight;
    return (maxSupportedCssHeight = ((navigator.userAgent.toLowerCase().match(/gecko\//) ? 4000000 : 32000000)));
}

export function getScrollBarDimensions(recalc?: boolean): { width: number; height: number; } {
    if (!scrollbarDimensions || recalc) {
        const c = document.body.appendChild(<div style="position:absolute;top:-10000px;left:-10000px;width:100px;height:100px;overflow: scroll;border:0" /> as HTMLElement);
        scrollbarDimensions = {
            width: Math.round(c.offsetWidth - c.clientWidth),
            height: Math.round(c.offsetWidth - c.clientHeight)
        };
        c.remove();
    }
    return scrollbarDimensions;
}

export function getVBoxDelta(el: HTMLElement): number {
    if (!el)
        return 0;

    var style = getComputedStyle(el);
    if (style.boxSizing === 'border-box')
        return 0;

    var p = ["border-top-width", "border-bottom-width", "padding-top", "padding-bottom"];
    var delta = 0;
    for (var val of p)
        delta += parsePx(style.getPropertyValue(val)) || 0;
    return delta;
}

export function setStyleProp(this: void, styles: CSSStyleDeclaration, prop: string, value: string): void {
    if (styles.getPropertyValue(prop) !== value)
        styles.setProperty(prop, value);
}
