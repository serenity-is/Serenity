import { EventHandler } from "./eventhandler";
import { getjQuery } from "./system";

const esc: Record<string, string> = {
    '<': '&lt;',
    '>': '&gt;',
    '"': '&quot;',
    "'": "&#39;",
    '&': '&amp;',
}

function escFunc(a: string): string {
    return esc[a];
}

/**
 * Html encodes a string (encodes single and double quotes, & (ampersand), > and < characters)
 * @param s String (or number etc.) to be HTML encoded
 */
export function htmlEncode(s: any): string {
    if (s == null)
        return '';

    if (typeof s !== "string")
        s = "" + s;

    return s.replace(/[<>"'&]/g, escFunc)
}

/** 
 * Toggles the class on the element handling spaces like addClass does.
 * @param el the element
 * @param cls the class to toggle
 * @param add if true, the class will be added, if false the class will be removed, otherwise it will be toggled.
 */
export function toggleClass(el: Element, cls: string, add?: boolean) {
    if (cls == null || !cls.length)
        return;

    if (cls.indexOf(' ') < 0) {
        el.classList.toggle(cls, add);
        return;
    }

    var k = cls.split(' ').map(x => x.trim()).filter(x => x.length);
    for (var a of k)
        el.classList.toggle(a, add);
}

export function addClass(el: Element, cls: string) {
    return toggleClass(el, cls, true);
}

export function removeClass(el: Element, cls: string) {
    return toggleClass(el, cls, false);
}

export function toClassName(value: string | boolean | (string | boolean)[]): string {
    return Array.isArray(value) ? value.map(x => value != null && typeof value !== "boolean" ? value : "").filter(Boolean).join(" ") :
        typeof value != null && typeof value !== "boolean" ? value : "";
}

export function isInputLike(node: Element): node is (HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement | HTMLButtonElement) {
    return isInputTag(node?.nodeName);
}

export const inputLikeSelector = "input,select,textarea,button";

export function isInputTag(tag: string) {
    return /^(?:input|select|textarea|button)$/i.test(tag);
}

export interface Fluent<TElement extends HTMLElement = HTMLElement> extends ArrayLike<TElement> {
    addClass(value: string | boolean | (string | boolean)[]): this;
    append(child: string | Node | Fluent<any>): this;
    appendTo(parent: Element | Fluent<any>): this;
    attr(name: string): string;
    attr(name: string, value: string | number | boolean | null | undefined): this;
    children(selector?: string): HTMLElement[];
    closest(selector: string): Fluent<HTMLElement>;
    data(name: string): string;
    data(name: string, value: string): this;
    getNode(): TElement;
    empty(): this;
    findFirst(selector: string): Fluent<HTMLElement>;
    findAll(selector: string): HTMLElement[];
    hasClass(klass: string): boolean;
    hide(): this;
    html(): string;
    html(value: string): this;
    insertAfter(referenceNode: HTMLElement | Fluent<HTMLElement>): this;
    insertBefore(referenceNode: HTMLElement | Fluent<HTMLElement>): this;
    readonly [n: number]: TElement;
    readonly length: number;
    off<K extends keyof HTMLElementEventMap>(type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): this;
    off(type: string, listener: EventListener): this;
    off(type: string, selector: string, delegationHandler: Function): this;
    on<K extends keyof HTMLElementEventMap>(type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): this;
    on(type: string, listener: EventListener): this;
    on(type: string, selector: string, delegationHandler: Function): this;
    one<K extends keyof HTMLElementEventMap>(type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): this;
    one(type: string, listener: EventListener): this;
    one(type: string, selector: string, delegationHandler: Function): this;
    parent(): Fluent<HTMLElement>;
    prepend(child: string | Node | Fluent<any>): this;
    prependTo(parent: Element | Fluent<any>): this;
    remove(): this;
    removeAttr(name: string): this;
    removeClass(value: string | boolean | (string | boolean)[]): this;
    show(): this;
    text(): string;
    text(value: string): this;
    toggle(flag?: boolean): this;
    toggleClass(value: (string | boolean | (string | boolean)[]), add?: boolean): this;
    trigger(type: string, args?: any): this;
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
    this.el && toggleClass(this.el, toClassName(value), true);
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
        if (name === "class")
            name = "className";
        if (typeof value === "string")
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

Fluent.prototype.closest = function (this: FluentThis, selector: string): Fluent<HTMLElement> {
    return new (Fluent as any)(this.el?.closest<HTMLElement>(selector));
}

Fluent.prototype.empty = function (this: FluentThis<any>, html?: string) {
    return this.html("");
}

Fluent.prototype.getNode = function (this: FluentThis<any>) {
    return this.el;
}

Fluent.prototype.hasClass = function (this: FluentThis<any>, klass: string) {
    return !!this.el?.classList.contains(klass);
}

Fluent.prototype.html = function (this: FluentThis<any>, html?: string) {
    if (html === void 0 && !arguments.length)
        return this.el?.innerHTML;
    if (this.el) {
        let $ = getjQuery();
        if ($)
            $(this.el).html(html ?? '');
        else
            this.el.innerHTML = html ?? '';
    }
    return this;
}

Fluent.prototype.insertAfter = function (this: FluentThis, referenceNode: HTMLElement | Fluent<HTMLElement>) {
    if (!this.el)
        return this;
    referenceNode = extractElement(referenceNode);
    let parent = referenceNode?.parentNode;
    if (!parent)
        this.el.remove();
    parent.insertBefore(this.el, referenceNode.nextSibling);
    return this;
}

Fluent.prototype.insertBefore = function (this: FluentThis, referenceNode: HTMLElement | Fluent<HTMLElement>) {
    if (!this.el)
        return this;
    referenceNode = extractElement(referenceNode);
    let parent = referenceNode?.parentElement;
    if (!parent)
        this.el.remove();
    parent.insertBefore(this.el, referenceNode);
    return this;
}

Fluent.prototype.prependTo = function (this: FluentThis, parent: HTMLElement | Fluent<HTMLElement>) {
    parent = extractElement(parent);
    if (!parent)
        this.el.remove();
    else
        parent.prepend(this.el);
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
    this.el && toggleClass(this.el, toClassName(value), false);
    return this;
}

Fluent.prototype.findFirst = function (this: FluentThis, selector: string): Fluent<HTMLElement> {
    return new (Fluent as any)(this.el?.querySelector<HTMLElement>(selector));
}

Fluent.prototype.findAll = function (this: FluentThis, selector: string): HTMLElement[] {
    return Array.from(this.el?.querySelectorAll<HTMLElement>(selector));
}

Fluent.prototype.hide = function (this: FluentThis) {
    this.el && (this.el.style.display = "none");
    return this;
}

Fluent.prototype.parent = function (this: FluentThis): Fluent<HTMLElement> {
    return new (Fluent as any)(this.el?.parentNode);
}

Fluent.prototype.prepend = function (this: FluentThis, child: string | Node | Fluent<HTMLElement>) {
    this.el && this.el.prepend(extractNode(child));
    return this;
}

Fluent.prototype.remove = function (this: FluentThis<any>) {
    if (!this.el)
        return;
    let $ = getjQuery();
    if ($)
        $(this.el).remove();
    else
        this.el.remove();
    return this;
}

Fluent.prototype.show = function (this: FluentThis) {
    this.el && (this.el.style.display = "");
    return this;
}

Fluent.prototype.trigger = function (this: FluentThis, type: string, args?: any) {
    this.el && EventHandler.trigger(this.el, type, args);
    return this;
}

Fluent.prototype.val = function (this: FluentThis<any>, value?: string) {
    if (value === void 0 && !arguments.length)
        return isInputLike(this.el) ? this.el.value : "";
    if (isInputLike(this.el))
        this.el.value = value;
    return this as any;
}

Fluent.prototype.text = function (this: FluentThis<any>, value?: string) {
    if (value === void 0 && !arguments.length)
        return this.el?.textContent;
    this.el && (this.el.textContent = value ?? '');
    return this;
}

Fluent.prototype.toggle = function (this: FluentThis, flag?: boolean) {
    if (!this.el)
        return;
    this.el.style.display = flag ? "" : (flag != null && !flag) ? "none" : (this.el.style.display == "none" ? "" : "none");
    return this;
}

Fluent.prototype.toggleClass = function (this: FluentThis, value: string | boolean | (string | boolean)[], add?: boolean) {
    this.el && toggleClass(this.el, toClassName(value), add);
    return this;
}

Object.defineProperty(Fluent.prototype, "length", { get: function () { return this.el ? 1 : 0 } });
Object.defineProperty(Fluent.prototype, 0, { get: function () { return this.el; } });

Fluent.off = EventHandler.off;
Fluent.on = EventHandler.on;
Fluent.one = EventHandler.on;
Fluent.trigger = EventHandler.trigger;

Fluent.ready = function (callback: () => void) {
    let $ = getjQuery();
    if ($) {
        $(callback);
    }
    else if (typeof document !== "undefined") {
        if (document.readyState === 'loading')
            document.addEventListener('DOMContentLoaded', callback);
        else
            setTimeout(callback, 0);
    }
}

export function H<K extends keyof HTMLElementTagNameMap>(tag: K): Fluent<HTMLElementTagNameMap[K]> {
    return new (Fluent as any)(document.createElement(tag));
}
