import { getjQuery } from "./environment";
import { EventHandler, triggerRemoveAndClearAll } from "./eventhandler";
import { toggleClass as toggleCls } from "./html";

export interface Fluent<TElement extends HTMLElement = HTMLElement> extends ArrayLike<TElement> {
    addClass(value: string | boolean | (string | boolean)[]): this;
    append(child: string | Node | Fluent<any>): this;
    appendTo(parent: Element | Fluent<any>): this;
    attr(name: string): string;
    attr(name: string, value: string | number | boolean | null | undefined): this;
    children(selector?: string): HTMLElement[];
    class(value: string | boolean | (string | boolean)[]): this;
    closest(selector: string): Fluent<HTMLElement>;
    data(name: string): string;
    data(name: string, value: string): this;
    each(callback: (el: TElement) => void): this;
    getNode(): TElement;
    empty(): this;
    findFirst<TElement extends HTMLElement = HTMLElement>(selector: string): Fluent<TElement>;
    findAll<TElement extends HTMLElement = HTMLElement>(selector: string): TElement[];
    hasClass(klass: string): boolean;
    hide(): this;
    getWidget<TWidget>(type?: { new(...args: any[]): TWidget }): TWidget;
    insertAfter(referenceNode: HTMLElement | Fluent<HTMLElement>): this;
    insertBefore(referenceNode: HTMLElement | Fluent<HTMLElement>): this;
    [Symbol.iterator]: TElement[];
    readonly [n: number]: TElement;
    readonly length: number;
    off<K extends keyof HTMLElementEventMap>(type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): this;
    off(type: string): this;
    off(type: string, listener: EventListener): this;
    off(type: string, selector: string, delegationHandler: Function): this;
    on<K extends keyof HTMLElementEventMap>(type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): this;
    on(type: string, listener: EventListener): this;
    on(type: string, selector: string, delegationHandler: Function): this;
    one<K extends keyof HTMLElementEventMap>(type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): this;
    one(type: string, listener: EventListener): this;
    one(type: string, selector: string, delegationHandler: Function): this;
    matches(selector?: string): boolean;
    nextSibling(selector?: string): Fluent<any>;
    parent(): Fluent<HTMLElement>;
    prepend(child: string | Node | Fluent<any>): this;
    prependTo(parent: Element | Fluent<any>): this;
    prevSibling(selector?: string): Fluent<any>;
    remove(): this;
    removeAttr(name: string): this;
    removeClass(value: string | boolean | (string | boolean)[]): this;
    show(): this;
    style(callback: (css: CSSStyleDeclaration) => void): this;
    text(): string;
    text(value: string): this;
    toggle(flag?: boolean): this;
    toggleClass(value: (string | boolean | (string | boolean)[]), add?: boolean): this;
    trigger(type: string, args?: any): this;
    tryGetWidget<TWidget>(type?: { new(...args: any[]): TWidget }): TWidget;
    val(value: string): this;
    val(): string;
}

export function Fluent<K extends keyof HTMLElementTagNameMap>(tag: K): Fluent<HTMLElementTagNameMap[K]>;
export function Fluent<TElement extends HTMLElement>(element: TElement): Fluent<TElement>;
export function Fluent(element: EventTarget): Fluent<HTMLElement>;
export function Fluent<K extends keyof HTMLElementTagNameMap>(tagOrElement: K | HTMLElementTagNameMap[K]): Fluent<HTMLElementTagNameMap[K]> {
    if (!(this instanceof Fluent)) {
        if (typeof tagOrElement === "string")
            return new (Fluent as any)(document.createElement(tagOrElement));

        return new (Fluent as any)(tagOrElement);
    }

    this.el = tagOrElement;
    return this;
}

export namespace Fluent {
    export const off = EventHandler.off;
    export const on = EventHandler.on;
    export const one = EventHandler.one;
    export const trigger = EventHandler.trigger;

    export function addClass(el: Element, value: string | boolean | (string | boolean)[]) {
        toggleCls(el, toClassName(value), true);
    }

    export function empty(el: Element) {
        if (!el)
            return;

        if (typeof el.hasChildNodes === "function" && el.hasChildNodes()) {
            let $ = getjQuery();
            if ($)
                $(el).empty();
            else {
                cleanContents(el);
                el.innerHTML = "";
            }
        }
        else
            el.innerHTML = "";
    }

    /** For compatibility with jQuery's :visible selector, e.g. has offsetWidth or offsetHeight or any client rect */
    export function isVisibleLike(el: Element) {
        return !!(el && ((el as any).offsetWidth || (el as any).offsetHeight || el.getClientRects().length));
    }

    export function remove(el: Element) {
        if (!el)
            return;
        let $ = getjQuery();
        if ($)
            $(el).remove();
        else {
            cleanContents(el);
            triggerRemoveAndClearAll(el);
            el.remove();
        }
        return this;
    }

    export function removeClass(el: Element, value: string | boolean | (string | boolean)[]) {
        toggleCls(el, toClassName(value), false);
    }

    export function toggle(el: Element, flag?: boolean): void {
        el && (el as any).style && ((el as any).style.display = flag ? "" : (flag != null && !flag) ? "none" : (el as any).style.display == "none" ? "" : "none");
    }

    export function toggleClass(el: Element, value: string | boolean | (string | boolean)[], add?: boolean): void {
        el && toggleCls(el, toClassName(value), add);
    }

    export function toClassName(value: string | boolean | (string | boolean)[]): string {
        if (typeof value === "string")
            return value;
        if (Array.isArray(value))
            return value.map(toClassName).filter(Boolean).join(" ");
        if (typeof value !== "boolean" && value != null)
            return "" + value;
        return "";
    }

    export function isInputLike(node: Element): node is (HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement | HTMLButtonElement) {
        return isInputTag(node?.nodeName);
    }

    export const inputLikeSelector = "input,select,textarea,button";

    export function isInputTag(tag: string) {
        return /^(?:input|select|textarea|button)$/i.test(tag);
    }

    export function isDefaultPrevented(e: { defaultPrevented?: boolean, isDefaultPrevented?: () => boolean }) {
        return e != null && (!!e.defaultPrevented ||
            (typeof e.isDefaultPrevented === "function" && !!e.isDefaultPrevented()));
    }

    export function eventProp(e: any, prop: string) {
        if (!e)
            return void 0;

        if (typeof e[prop] !== "undefined")
            return e[prop];

        if (typeof e.originalEvent === "object" && typeof e.originalEvent[prop] !== "undefined")
            return e.originalEvent[prop];

        if (typeof e.detail === "object")
            return e.detail[prop];
    }
}

type FluentThis<TElement extends HTMLElement = HTMLElement> = Fluent<TElement> & {
    el?: HTMLElement;
}

function extractNode(element: string | Node | FluentThis): (string | Node) {
    return (element instanceof EventTarget || typeof element === "string") ? element : element?.el;
}

function extractElement(element: Element | FluentThis<HTMLElement>): HTMLElement {
    return (element instanceof EventTarget || typeof element === "string") ? element as HTMLElement : element?.el;
}

Fluent.prototype.addClass = function (this: FluentThis, value: string | boolean | (string | boolean)[]) {
    Fluent.addClass(this.el, value);
    return this;
}

Fluent.prototype.append = function (this: FluentThis, child: string | Node | Fluent<HTMLElement>) {
    this.el && this.el.append(extractNode(child));
    return this;
}

Fluent.prototype.appendTo = function (this: FluentThis, parent: Element | Fluent<HTMLElement>) {
    if (this.el) {
        parent = extractElement(parent);
        if (!parent)
            this.el.remove();
        else
            parent.appendChild(this.el);
    }
    return this;
}

Fluent.prototype.attr = function (this: FluentThis<any>, name: string, value?: string | boolean | number | null | undefined) {
    if (value === void 0 && arguments.length < 2)
        return this.el?.getAttribute(name);

    if (this.el) {
        if (value == null)
            this.el.removeAttribute(name);
        else if (typeof value === "string")
            this.el.setAttribute(name, value);
        else if (typeof value === "number")
            this.el.setAttribute(name, "" + value);
        else if (value != null && value !== false)
            this.el.setAttribute(name, "true");
    }

    return this;
}

Fluent.prototype.children = function (this: FluentThis, selector?: string): Element[] {
    if (selector == null)
        return Array.from(this.el?.children || []);
    return Array.from(this.el?.children || []).filter(x => x.matches(selector));
}

Fluent.prototype.class = function (this: FluentThis, value: string | boolean | (string | boolean)[]): Fluent<HTMLElement> {
    this.el && (this.el.className = Fluent.toClassName(value));
    return this;
}

Fluent.prototype.closest = function (this: FluentThis, selector: string): Fluent<HTMLElement> {
    return new (Fluent as any)(this.el?.closest<HTMLElement>(selector));
}

Fluent.prototype.each = function (this: FluentThis, callback: (el: HTMLElement) => void) {
    this.el && callback(this.el);
    return this;
}

Fluent.prototype.empty = function (this: FluentThis<any>) {
    Fluent.empty(this.el);
    return this;
}

Fluent.prototype.getNode = function (this: FluentThis<any>) {
    return this.el;
}

Fluent.prototype.hasClass = function (this: FluentThis<any>, klass: string) {
    return !!this.el?.classList.contains(klass);
}

Fluent.prototype.insertAfter = function (this: FluentThis, referenceNode: HTMLElement | Fluent<HTMLElement>) {
    if (!this.el)
        return this;
    referenceNode = extractElement(referenceNode);
    let parent = referenceNode?.parentNode;
    if (!parent)
        this.el.remove();
    else
        parent.insertBefore(this.el, referenceNode.nextSibling);
    return this;
}

Fluent.prototype.isVisibleLike = function (this: FluentThis) {
    return Fluent.isVisibleLike(this.el);
}

Fluent.prototype.insertBefore = function (this: FluentThis, referenceNode: HTMLElement | Fluent<HTMLElement>) {
    if (!this.el)
        return this;
    referenceNode = extractElement(referenceNode);
    let parent = referenceNode?.parentElement;
    if (!parent)
        this.el.remove();
    else
        parent.insertBefore(this.el, referenceNode);
    return this;
}

Fluent.prototype.prependTo = function (this: FluentThis, parent: HTMLElement | Fluent<HTMLElement>) {
    if (this.el) {
        parent = extractElement(parent);
        if (!parent)
            this.el.remove();
        else
            parent.prepend(this.el);
    }
    return this;
}

Fluent.prototype.removeAttr = function (this: FluentThis, name: string) {
    this.el?.removeAttribute(name);
    return this;
}

Fluent.prototype.data = function (this: FluentThis, name: string, value?: string) {
    if (value === void 0 && arguments.length < 2)
        return this.attr("data-" + name);
    return this.attr("data-" + name, value);
}

Fluent.prototype.off = function (this: FluentThis<any>, type: string, handler: any, delegationHandler?: Function) {
    this.el && EventHandler.off(this.el, type, handler, delegationHandler);
    return this;
}

Fluent.prototype.on = function (this: FluentThis<any>, type: string, handler: any, delegationHandler?: Function) {
    this.el && EventHandler.on(this.el, type, handler, delegationHandler);
    return this;
}

Fluent.prototype.one = function (this: FluentThis<any>, type: string, handler: any, delegationHandler?: Function) {
    this.el && EventHandler.one(this.el, type, handler, delegationHandler);
    return this;
}

Fluent.prototype.removeClass = function (this: FluentThis, value: string | boolean | (string | boolean)[]) {
    Fluent.removeClass(this.el, value);
    return this;
}

Fluent.prototype.findFirst = function (this: FluentThis, selector: string): Fluent<HTMLElement> {
    return new (Fluent as any)(this.el?.querySelector<HTMLElement>(selector));
}

Fluent.prototype.findAll = function (this: FluentThis, selector: string): HTMLElement[] {
    if (!this.el)
        return [];
    return Array.from(this.el.querySelectorAll<HTMLElement>(selector));
}

Fluent.prototype.hide = function (this: FluentThis) {
    this.el && (this.el.style.display = "none");
    return this;
}

Fluent.prototype.matches = function (this: FluentThis, selector?: string): boolean {
    return !!this.el && typeof this.el.matches === "function" && this.el.matches(selector);
}

Fluent.prototype.nextSibling = function (this: FluentThis, selector?: string): Fluent<HTMLElement> {
    var sibling = this.el?.nextElementSibling;
    while (sibling && selector != null && !sibling.matches(selector))
        sibling = sibling.nextElementSibling;
    return new (Fluent as any)(sibling);
}

Fluent.prototype.parent = function (this: FluentThis): Fluent<HTMLElement> {
    return new (Fluent as any)(this.el?.parentNode);
}

Fluent.prototype.prepend = function (this: FluentThis, child: string | Node | Fluent<HTMLElement>) {
    this.el && this.el.prepend(extractNode(child));
    return this;
}

Fluent.prototype.prevSibling = function (this: FluentThis, selector?: string): Fluent<HTMLElement> {
    var sibling = this.el?.previousElementSibling;
    while (sibling && selector != null && !sibling.matches(selector))
        sibling = sibling.previousElementSibling;
    return new (Fluent as any)(sibling);
}

function cleanContents(element: Element) {
    element.querySelectorAll("*").forEach(node => triggerRemoveAndClearAll(node));
}

Fluent.prototype.remove = function (this: FluentThis<any>) {
    Fluent.remove(this.el);
    return this;
}

Fluent.prototype.show = function (this: FluentThis) {
    this.el && (this.el.style.display = "");
    return this;
}

Fluent.prototype.style = function (this: FluentThis, callback: (css: CSSStyleDeclaration) => void) {
    if (this.el && this.el.style instanceof CSSStyleDeclaration)
        callback(this.el.style);
    return this;
}

Fluent.prototype.trigger = function (this: FluentThis, type: string, args?: any) {
    this.el && EventHandler.trigger(this.el, type, args);
    return this;
}

Fluent.prototype.val = function (this: FluentThis<any>, value?: string) {
    if (value === void 0 && !arguments.length)
        return Fluent.isInputLike(this.el) ? this.el.value : "";
    if (Fluent.isInputLike(this.el))
        this.el.value = value ?? "";
    return this as any;
}

Fluent.prototype.text = function (this: FluentThis<any>, value?: string) {
    if (value === void 0 && !arguments.length)
        return this.el?.textContent;

    if (!this.el)
        return this;

    if (typeof this.el.hasChildNodes === "function" && this.el.hasChildNodes()) {
        let $ = getjQuery();
        if ($)
            $(this.el).text(value ?? "");
        else {
            cleanContents(this.el);
            this.el.textContent = value ?? "";
        }
    }
    else
        this.el.textContent = value ?? "";

    return this;
}

Fluent.prototype.toggle = function (this: FluentThis, flag?: boolean) {
    Fluent.toggle(this.el, flag);
    return this;
}

Fluent.prototype.toggleClass = function (this: FluentThis, value: string | boolean | (string | boolean)[], add?: boolean) {
    Fluent.toggleClass(this.el, value, add);
    return this;
}

Object.defineProperty(Fluent.prototype, "length", { get: function () { return this.el ? 1 : 0 } });
Object.defineProperty(Fluent.prototype, 0, { get: function () { return this.el; } });
Object.defineProperty(Fluent.prototype, Symbol.iterator, { get: function () { return (this.el ? [this.el] : [])[Symbol.iterator]; } });


Fluent.ready = function (callback: () => void) {
    if (!callback)
        return;
    let $ = getjQuery();
    if ($) {
        $(callback);
    }
    else if (typeof document !== "undefined" && document.readyState === 'loading') {
        const loaded = () => {
            document.removeEventListener('DOMContentLoaded', loaded);
            callback();
        }
        document.addEventListener('DOMContentLoaded', loaded);
        return;
    }
    else {
        setTimeout(callback, 0);
    }
}

Fluent.byId = function <TElement extends HTMLElement>(id: string): Fluent<TElement> {
    return Fluent<TElement>(document.getElementById(id) as TElement);
}

export function H<K extends keyof HTMLElementTagNameMap>(tag: K): Fluent<HTMLElementTagNameMap[K]> {
    return new (Fluent as any)(document.createElement(tag));
}