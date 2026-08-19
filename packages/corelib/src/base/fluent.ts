import { getjQuery } from "./environment";
import { addListener, notifyDisposingNode, removeListener, triggerEvent } from "./fluent-events";
import { toggleClass as toggleCls } from "./html";

/**
 * Represents a Fluent object, which is similar to jQuery but works for only one element.
 * It implements the `ArrayLike` interface and can have 0 (null) or 1 element.
 */
export interface Fluent<TElement extends HTMLElement = HTMLElement> extends ArrayLike<TElement> {

    /**
     * Adds one or more classes to the element. Any falsy value is ignored.
     *
     * @param value The class or classes to add. It can be a string, boolean, or an array of strings or booleans.
     * @returns The Fluent object itself.
     */
    addClass(value: string | boolean | (string | boolean)[]): this;

    /**
     * Appends content to the element.
     *
     * @param child The content to append. It can be a string, a Node object, or another Fluent object.
     * @returns The Fluent object itself.
     */
    append(child: string | Node | Fluent<any>): this;

    /**
     * Inserts content after the element.
     *
     * @param content The content to insert. It can be a string, a Node object, or another Fluent object.
     * @returns The Fluent object itself.
     */
    after(content: string | Node | Fluent<any>): this;

    /**
     * Appends the element to the specified parent element.
     *
     * @param parent The parent element to append to. It can be an Element object or another Fluent object.
     * @returns The Fluent object itself.
     */
    appendTo(parent: Element | Fluent<any>): this;

    /**
     * Gets the value of the specified attribute. 
     *
     * @param name The name of the attribute.
     * @returns The value of the attribute.
     */
    attr(name: string): string;

    /**
     * Sets the value of the specified attribute. 
     *
     * @param name The name of the attribute.
     * @param value The value of the attribute. If the value is falsy the attribute is removed.
     * @returns The Fluent object itself if a value is provided.
     */
    attr(name: string, value: string | number | boolean | null | undefined): this;

    /**
     * Inserts content before the element.
     *
     * @param content The content to insert. It can be a string, a Node object, or another Fluent object.
     * @returns The Fluent object itself.
     */
    before(content: string | Node | Fluent<any>): this;

    /**
     * Gets the children of the element as an array (not Fluent)
     *
     * @param selector Optional. A CSS selector to filter the children.
     * @returns An array of HTMLElement objects representing the children.
     */
    children<TElement extends HTMLElement = HTMLElement>(selector?: string): TElement[];

    /**
     * Sets (overrides) the class attribute of the element. Any falsy value is ignored.
     *
     * @param value The class or classes to add. It can be a string, boolean, or an array of strings or booleans.
     * @returns The Fluent object itself.
     */
    class(value: string | boolean | (string | boolean)[]): this;

    /**
     * Triggers a click event on the element.
     *
     * @returns The Fluent object itself.
     */
    click(): this;
    /**
     * Adds a click event listener on the element.
     *
     * @param listener A callback function to execute when the click event is triggered.
     * @returns The Fluent object itself.
     */
    click(listener: (e: MouseEvent) => void): this;

    /**
     * Gets the closest ancestor of the element that matches the specified selector.
     *
     * @param selector A CSS selector to match against.
     * @returns A Fluent object representing the closest ancestor element.
     */
    closest<TElement extends HTMLElement = HTMLElement>(selector: string): Fluent<TElement>;

    /**
     * Gets or sets the value of the specified data attribute.
     *
     * @param name The name of the data attribute.
     * @returns The value of the data attribute if no value is provided, or the Fluent object itself if a value is provided.
     */
    data(name: string): string;
    data(name: string, value: string): this;

    /**
     * Executes a callback function for the element in the Fluent object if it is not null.
     *
     * @param callback The callback function to execute for each element.
     * @returns The Fluent object itself.
     */
    each(callback: (el: TElement) => void): this;

    /**
     * Gets the underlying HTML element.
     *
     * @returns The underlying HTML element.
     */
    getNode(): TElement;

    /**
     * Removes all child nodes from the element. It also clears event handlers attached via Fluent, and disposes any attached widgets.
     *
     * @returns The Fluent object itself.
     */
    empty(): this;

    /**
     * Finds all elements that match the specified selector within the element.
     *
     * @param selector A CSS selector to match against.
     * @returns An array of elements that match the selector.
     */
    findAll<TElement extends HTMLElement = HTMLElement>(selector: string): TElement[];

    /**
     * Finds each element that matches the specified selector within the element and executes a callback function for each found element as a Fluent object.
     *
     * @param selector A CSS selector to match against.
     * @param callback The callback function to execute for each found element. It receives a Fluent object for each element.
     * @returns The Fluent object itself.
     */
    findEach<TElement extends HTMLElement = HTMLElement>(selector: string, callback: (el: Fluent<TElement>, index: number) => void): this;

    /**
     * Finds the first element that matches the specified selector within the element.
     *
     * @param selector A CSS selector to match against.
     * @returns A Fluent object representing the first element that matches the selector.
     */
    findFirst<TElement extends HTMLElement = HTMLElement>(selector: string): Fluent<TElement>;

    /**
     * Sets focus on the element.
     *
     * @returns The Fluent object itself.
     */
    focus(): this;

    /**
     * Checks if the element has the specified class.
     *
     * @param klass The class to check for.
     * @returns `true` if the element has the class, `false` otherwise.
     */
    hasClass(klass: string): boolean;

    /**
     * Gets the value of the hidden attribute/property. 
     *
     * @returns The value of the hidden attribute/property
     */
    hidden(name: string): boolean;

    
    /**
     * Sets the value of the hidden property/attribute.
     *
     * @param value The value of the attribute. If the value is falsy the attribute is removed.
     * @returns The Fluent object itself if a value is provided.
     */
    hidden(value: boolean): this;

    /**
     * Hides the element by setting its hidden property to true.
     *
     * @returns The Fluent object itself.
     */
    hide(): this;

    /**
     * Gets the widget associated with the element.
     *
     * @param type Optional. The constructor function of the widget.
     * @returns The widget associated with the element.
     */
    getWidget<TWidget>(type?: { new(...args: any[]): TWidget }): TWidget;

    /**
     * Inserts the element after the specified reference element.
     *
     * @param referenceNode The reference element to insert after. It can be an HTMLElement object or another Fluent object.
     * @returns The Fluent object itself.
     */
    insertAfter(referenceNode: HTMLElement | Fluent<HTMLElement>): this;

    /**
     * Inserts the element before the specified reference element.
     *
     * @param referenceNode The reference element to insert before. It can be an HTMLElement object or another Fluent object.
     * @returns The Fluent object itself.
     */
    insertBefore(referenceNode: HTMLElement | Fluent<HTMLElement>): this;

    /**
     * Gets an iterator for the elements in the Fluent object.
     *
     * @returns An iterator for the elements in the Fluent object.
     */
    [Symbol.iterator]: TElement[];

    /**
     * Gets the element at the specified index.
     *
     * @param n The index of the element.
     * @returns The element at the specified index.
     */
    readonly [n: number]: TElement;

    /**
     * Gets the number of elements in the Fluent object. Can only be 1 or 0.
     */
    readonly length: number;

    /**
     * Removes an event listener from the element.
     *
     * @param type The type of the event. It can include a ".namespace" similar to jQuery.
     * @param listener The event listener to remove.
     * @returns The Fluent object itself.
     */
    off<K extends keyof HTMLElementEventMap>(type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): this;
    off(type: string): this;
    off(type: string, listener: EventListener): this;
    off(type: string, selector: string, delegationHandler: Function): this;

    /**
     * Adds an event listener to the element. It is possible to use delegated events like jQuery.
     *
     * @param type The type of the event. It can include a ".namespace" similar to jQuery.
     * @param listener The event listener to add.
     * @returns The Fluent object itself.
     */
    on<K extends keyof HTMLElementEventMap>(type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): this;
    on(type: string, listener: EventListener): this;
    on(type: string, selector: string, delegationHandler: Function): this;

    /**
     * Adds a one-time event listener to the element. It is possible to use delegated events like jQuery.
     *
     * @param type The type of the event. It can include a ".namespace" similar to jQuery.
     * @param listener The event listener to add.
     * @returns The Fluent object itself.
     */
    one<K extends keyof HTMLElementEventMap>(type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): this;
    one(type: string, listener: EventListener): this;
    one(type: string, selector: string, delegationHandler: Function): this;

    /**
     * Checks if the element matches the specified selector.
     *
     * @param selector A CSS selector to match against.
     * @returns `true` if the element matches the selector, `false` otherwise.
     */
    matches(selector: string): boolean;

    /**
     * Gets the next sibling element that matches the specified selector, or the first sibling if no selector is provided..
     *
     * @param selector Optional. A CSS selector to filter the next sibling.
     * @returns A Fluent object representing the next sibling element.
     */
    nextSibling(selector?: string): Fluent<any>;

    /**
     * Gets the parent element of the element.
     *
     * @returns A Fluent object representing the parent element.
     */
    parent<TElement extends HTMLElement = HTMLElement>(): Fluent<TElement>;

    /**
     * Prepends content to the element.
     *
     * @param child The content to prepend. It can be a string, a Node object, or another Fluent object.
     * @returns The Fluent object itself.
     */
    prepend(child: string | Node | Fluent<any>): this;

    /**
     * Prepends the element to the specified parent element.
     *
     * @param parent The parent element to prepend to. It can be an Element object or another Fluent object.
     * @returns The Fluent object itself.
     */
    prependTo(parent: Element | Fluent<any>): this;

    /**
     * Gets the previous sibling element that matches the specified selector, or the first sibling if no selector is provided.
     *
     * @param selector Optional. A CSS selector to filter the previous sibling.
     * @returns A Fluent object representing the previous sibling element.
     */
    prevSibling(selector?: string): Fluent<any>;

    /**
     * Removes the element from the DOM. It also removes event handlers and disposes widgets by calling "disposing" event handlers.
     *
     * @returns The Fluent object itself.
     */
    remove(): this;

    /**
     * Removes the specified attribute from the element.
     *
     * @param name The name of the attribute to remove.
     * @returns The Fluent object itself.
     */
    removeAttr(name: string): this;

    /**
     * Removes one or more classes from the element. Any falsy value is ignored.
     *
     * @param value The class or classes to remove. It can be a string, boolean, or an array of strings or booleans.
     * @returns The Fluent object itself.
     */
    removeClass(value: string | boolean | (string | boolean)[]): this;

    /**
     * Shows the element by setting its hidden property to false.
     *
     * @returns The Fluent object itself.
     */
    show(): this;

    /**
     * Executes a callback function to modify the inline style of the element.
     *
     * @param callback The callback function to modify the inline style.
     * @returns The Fluent object itself.
     */
    style(callback: (css: CSSStyleDeclaration) => void): this;

    /**
     * Gets or sets the text content of the element.
     *
     * @returns The text content of the element if no value is provided, or the Fluent object itself if a value is provided.
     */
    text(): string;
    text(value: string): this;

    /**
     * Toggles the visibility of the element.
     *
     * @param flag Optional. A flag indicating whether to show or hide the element. If not provided, the visibility will be toggled.
     * @returns The Fluent object itself.
     */
    toggle(flag?: boolean): this;

    /**
     * Toggles one or more classes on the element. If the class exists, it is removed; otherwise, it is added.
     *
     * @param value The class or classes to toggle. It can be a string, boolean, or an array of strings or booleans.
     * @returns The Fluent object itself.
     */
    toggleClass(value: (string | boolean | (string | boolean)[]), add?: boolean): this;

    /**
     * Triggers a specified event on the element.
     *
     * @param type The type of the event to trigger.
     * @param args Optional. An object that specifies event-specific initialization properties.
     * @returns The Fluent object itself.
     */
    trigger(type: string, args?: any): this;

    /**
     * Tries to get the widget associated with the element.
     *
     * @param type Optional. The constructor function of the widget.
     * @returns The widget associated with the element, or `null` if no widget is found.
     */
    tryGetWidget<TWidget>(type?: { new(...args: any[]): TWidget }): TWidget;

    /**
     * Gets or sets the value of the element.
     *
     * @param value The value to set. If no value is provided, returns the current value of the element.
     * @returns The value of the element if no value is provided, or the Fluent object itself if a value is provided.
     */
    val(value: string): this;
    val(): string;
}

const validTagRegex = /^[a-zA-Z][a-zA-Z0-9\-]*$/;

/**
 * Creates a {@link Fluent} wrapper from a tag name or an existing element.
 * @param tag - Tag name to create (e.g. `"div"`). Must match `^[a-zA-Z][a-zA-Z0-9\\-]*$`; otherwise an empty wrapper is returned.
 * @returns A {@link Fluent} wrapping the newly created element.
 */
export function Fluent<K extends keyof HTMLElementTagNameMap>(tag: K): Fluent<HTMLElementTagNameMap[K]>;
/**
 * Wraps an existing element in a {@link Fluent} instance.
 * @param element - Element to wrap; `null` / `undefined` yields an empty wrapper.
 * @returns A {@link Fluent} wrapping `element`.
 */
export function Fluent<TElement extends HTMLElement>(element: TElement): Fluent<TElement>;
/**
 * Wraps an `EventTarget` (typically an `HTMLElement`) in a {@link Fluent} instance.
 * @param element - Target to wrap.
 * @returns A {@link Fluent} wrapping `element`.
 */
export function Fluent(element: EventTarget): Fluent<HTMLElement>;
/**
 * Factory / constructor for {@link Fluent} wrappers. Also usable with `new`.
 * @param tagOrElement - Tag name to create, or an element / `EventTarget` to wrap.
 * @returns A {@link Fluent} instance. When called without `new`, delegates to the constructor path.
 */
export function Fluent<K extends keyof HTMLElementTagNameMap>(tagOrElement: K | HTMLElementTagNameMap[K]): Fluent<HTMLElementTagNameMap[K]> {
    if (!(this instanceof Fluent)) {
        if (typeof tagOrElement === "string") {
            if (validTagRegex.test(tagOrElement))
                return new (Fluent as any)(document.createElement(tagOrElement));
            return new (Fluent as any)(null);
        }

        return new (Fluent as any)(tagOrElement);
    }

    this.el = tagOrElement;
    return this;
}

/**
 * Static helpers that operate on raw DOM elements without requiring a {@link Fluent} wrapper.
 * @remarks Supports jQuery-style namespaced and delegated events via the shared `fluent-events` module.
 */
export namespace Fluent {
    /**
     * Adds an event listener, with optional delegation and namespace support.
     * @param element - Target element to listen on.
     * @param type - Event type; may include a `.namespace` suffix (e.g. `"click.myNs"`).
     * @param listener - Callback to invoke when the event fires.
     * @returns `void`.
     */
    export function on<K extends keyof HTMLElementEventMap>(element: EventTarget, type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): void;
    export function on(element: EventTarget, type: string, listener: EventListener): void;
    export function on(element: EventTarget, type: string, selector: string, delegationHandler: Function): void;
    export function on(element: EventTarget, type: string, handler: any, delegationHandler?: Function): void {
        addListener(element, type, handler, delegationHandler, /*oneOff*/ false);
    }

    /**
     * Adds a one-time event listener that is automatically removed after the first invocation.
     * @param element - Target element to listen on.
     * @param type - Event type; may include a `.namespace` suffix.
     * @param listener - Callback to invoke once when the event fires.
     * @returns `void`.
     */
    export function one<K extends keyof HTMLElementEventMap>(element: EventTarget, type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): void;
    export function one(element: EventTarget, type: string, listener: EventListener): void;
    export function one(element: EventTarget, type: string, selector: string, delegationHandler: Function): void;
    export function one(element: EventTarget, type: string, handler: any, delegationHandler?: Function): void {
        addListener(element, type, handler, delegationHandler, true);
    }

    /**
     * Removes an event listener (or all listeners for a namespaced type).
     * @param element - Target element.
     * @param type - Event type; may include a `.namespace`. When only a namespace is handled, all matching listeners are removed.
     * @param listener - Specific callback to remove. When omitted, all listeners for `type` are removed.
     * @returns `void`.
     */
    export function off<K extends keyof HTMLElementEventMap>(element: EventTarget, type: K, listener?: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): void;
    export function off(element: EventTarget, type: string, listener?: EventListener): void;
    export function off(element: EventTarget, type: string, selector?: string, delegationHandler?: Function): void;
    export function off(element: EventTarget, originalTypeEvent: string, handler?: any, delegationHandler?: Function): void {
        return removeListener(element, originalTypeEvent, handler, delegationHandler);
    }

    /**
     * Dispatches a synthetic event on the element.
     * @param element - Target element to dispatch on.
     * @param type - Event type to trigger (e.g. `"click"`, `"change"`).
     * @param args - Optional properties merged into the created `Event` / `CustomEvent` (`detail`, `bubbles`, etc.).
     * @returns The dispatched event. Use {@link Fluent.isDefaultPrevented} to test whether `preventDefault()` was called.
     */
    export function trigger(element: EventTarget, type: string, args?: any): Event & { isDefaultPrevented?(): boolean } {
        return triggerEvent(element, type, args);
    }

    /**
     * Adds one or more classes to the element.
     * @param element - Target element.
     * @param value - Class name(s) to add. Strings are split on whitespace; arrays are flattened; falsy entries are ignored.
     * @returns `void`.
     */
    export function addClass(element: Element, value: string | boolean | (string | boolean)[]): void {
        toggleCls(element, toClassName(value), true);
    }

    /**
     * Removes all child nodes from the element, notifying `disposing` handlers and clearing Fluent event listeners.
     * @param element - Element to empty. No-op when `null` / `undefined`.
     * @returns `void`.
     */
    export function empty(element: Element): void {
        if (!element)
            return;

        if (typeof element.hasChildNodes === "function" && element.hasChildNodes()) {
            let $ = getjQuery();
            if ($)
                $(element).empty();
            else {
                notifyDisposingNode(element, { descendants: true, excludeSelf: true });
                element.innerHTML = "";
            }
        }
        else
            element.innerHTML = "";
    }

    /**
     * Tests whether the element is considered visible (jQuery `:visible` semantics).
     * @param element - Element to test.
     * @returns `true` when the element has non-zero `offsetWidth` / `offsetHeight` or any client rects.
     */
    export function isVisibleLike(element: Element): boolean {
        return !!(element && ((element as any).offsetWidth || (element as any).offsetHeight || element.getClientRects().length));
    }

    /**
     * Removes the element from the DOM, clearing Fluent event handlers and firing `disposing` notifications for the element and its descendants.
     * @param element - Element to remove. No-op when `null` / `undefined`.
     * @returns `void`.
     */
    export function remove(element: Element): void {
        if (!element)
            return;
        let $ = getjQuery();
        if ($) {
            $(element).remove();
        }
        else {
            notifyDisposingNode(element, { descendants: true });
            element.remove();
        }
    }

    /**
     * Removes one or more classes from the element.
     * @param element - Target element.
     * @param value - Class name(s) to remove. Falsy entries are ignored.
     * @returns `void`.
     */
    export function removeClass(element: Element, value: string | boolean | (string | boolean)[]): void {
        toggleCls(element, toClassName(value), false);
    }

    /**
     * Shows or hides the element, handling `hidden`, `display:none`, and `.hidden` class.
     * @param element - Target element.
     * @param flag - When `true`, shows the element; when `false`, hides it; when omitted, toggles the current visibility.
     * @returns `void`.
     */
    export function toggle(element: Element, flag?: boolean): void {
        if (element) {
            let hidden = !(flag != null ? flag : (
                ("hidden" in element && !!element.hidden) ||
                element.classList.contains("hidden") ||
                (element as any).style?.display === "none"));

            if ("hidden" in element)    
                element.hidden = hidden;

            if (!hidden) {
                if ("style" in element &&
                    (element as any).style?.display === "none") {
                    (element as any).style.display = "";
                }

                if (element.classList.contains("hidden"))
                    element.classList.remove("hidden");
            }
        }
    }

    /**
     * Toggles one or more classes on the element.
     * @param element - Target element.
     * @param value - Class name(s) to toggle. Falsy entries are ignored.
     * @param add - When `true`, forces addition; when `false`, forces removal; when omitted, each class is toggled.
     * @returns `void`.
     */
    export function toggleClass(element: Element, value: string | boolean | (string | boolean)[], add?: boolean): void {
        element && toggleCls(element, toClassName(value), add);
    }

    /**
     * Normalizes a class value (string, boolean flag, or nested array) to a space-separated class string.
     * @param value - Value to normalize. Non-string primitives are stringified; booleans and `null` / `undefined` yield `""`; arrays are recursively flattened and falsy entries dropped.
     * @returns The concatenated class string (may be empty).
     */
    export function toClassName(value: string | boolean | (string | boolean)[]): string {
        if (typeof value === "string")
            return value;
        if (Array.isArray(value))
            return value.map(toClassName).filter(Boolean).join(" ");
        if (typeof value !== "boolean" && value != null)
            return "" + value;
        return "";
    }

    /**
     * Tests whether the element is an input-like control (`input`, `select`, `textarea`, or `button`).
     * @param element - Element to test.
     * @returns `true` when the element's tag name matches an input-like tag.
     */
    export function isInputLike(element: Element): element is (HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement | HTMLButtonElement) {
        return isInputTag(element?.nodeName);
    }

    /** CSS selector that matches input-like elements (`input,select,textarea,button`). */
    export const inputLikeSelector = "input,select,textarea,button";

    /**
     * Tests whether a tag name is an input-like tag.
     * @param tag - Tag name to test (case-insensitive).
     * @returns `true` when `tag` is `input`, `select`, `textarea`, or `button`.
     */
    export function isInputTag(tag: string) {
        return /^(?:input|select|textarea|button)$/i.test(tag);
    }

    /**
     * Tests whether `preventDefault()` was called on the event, supporting both native and jQuery-wrapped events.
     * @param event - Event object, possibly with a jQuery `isDefaultPrevented()` method.
     * @returns `true` when `defaultPrevented` is `true` or `isDefaultPrevented()` returns `true`.
     */
    export function isDefaultPrevented(event: { defaultPrevented?: boolean, isDefaultPrevented?: () => boolean }) {
        return event != null && (!!event.defaultPrevented ||
            (typeof event.isDefaultPrevented === "function" && !!event.isDefaultPrevented()));
    }

    /**
     * Reads a property from the event, falling back to wrapped/original event containers.
     * @param event - Event object, potentially jQuery-wrapped (`originalEvent`, `nativeEvent`).
     * @param prop - Property name to read.
     * @returns The property value, or `undefined` when not found. Lookup order: `event[prop]` → `event.nativeEvent[prop]` → `event.originalEvent[prop]` / `event.nativeEvent.originalEvent[prop]` → `event.detail[prop]`.
     */
    export function eventProp(event: any, prop: string) {
        if (!event)
            return void 0;

        if (prop in event) {
            return event[prop];
        }

        if (typeof event.nativeEvent === "object" && prop in event.nativeEvent)
            return event.nativeEvent[prop];

        const originalEvent = event.originalEvent ?? event.nativeEvent?.originalEvent;

        if (typeof originalEvent === "object" && prop in originalEvent)
            return originalEvent[prop];

        if (typeof event.detail === "object" && prop in event.detail)
            return event.detail[prop];
    }
}

type FluentThis<TElement extends HTMLElement = HTMLElement> = Fluent<TElement> & {
    el?: HTMLElement;
}

function extractNode(element: string | Node | FluentThis): (string | Node) {
    return (element instanceof Node || typeof element === "string") ? element : element?.el;
}

function extractElement(element: Element | FluentThis<HTMLElement>): HTMLElement {
    return (element instanceof Node || typeof element === "string") ? element as HTMLElement : element?.el;
}

Fluent.prototype.addClass = function (this: FluentThis, value: string | boolean | (string | boolean)[]) {
    Fluent.addClass(this.el, value);
    return this;
}

Fluent.prototype.after = function (this: FluentThis, content: string | HTMLElement | Fluent<any>) {
    if (this.el) {
        const node = extractNode(content);
        if (node instanceof Element)
            this.el.insertAdjacentElement("afterend", node);
        else if (node instanceof DocumentFragment) {
            Fluent(node).insertAfter(this.el);
        }
        else if (node != null) {
            this.el.insertAdjacentText("afterend", "" + node);
        }
    }
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
        if (value == null || value === false)
            this.el.removeAttribute(name);
        else if (typeof value === "string")
            this.el.setAttribute(name, value);
        else if (typeof value === "number")
            this.el.setAttribute(name, "" + value);
        else
            this.el.setAttribute(name, "true");
    }

    return this;
}

Fluent.prototype.before = function (this: FluentThis, content: string | HTMLElement | Fluent<any>) {
    if (this.el) {
        const node = extractNode(content);
        if (node instanceof Element)
            this.el.insertAdjacentElement("beforebegin", node);
        else if (node instanceof DocumentFragment) {
            Fluent(node).insertBefore(this.el);
        }
        else if (node != null) {
            this.el.insertAdjacentText("beforebegin", "" + node);
        }
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

Fluent.prototype.click = function (this: FluentThis, listener?: (e: MouseEvent) => void) {
    if (listener === void 0 && !arguments.length) {
        this.el && typeof this.el.click === "function" && this.el.click();
        return this;
    }
    else {
        return this.on("click", listener);
    }
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

Fluent.prototype.focus = function (this: FluentThis) {
    this.el && typeof this.el.focus === "function" && this.el.focus();
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
    this.el && removeListener(this.el, type, handler, delegationHandler);
    return this;
}

Fluent.prototype.on = function (this: FluentThis<any>, type: string, handler: any, delegationHandler?: Function) {
    this.el && addListener(this.el, type, handler, delegationHandler, false);
    return this;
}

Fluent.prototype.one = function (this: FluentThis<any>, type: string, handler: any, delegationHandler?: Function) {
    this.el && addListener(this.el, type, handler, delegationHandler, true);
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

Fluent.prototype.findEach = function (this: FluentThis, selector: string, callback: (el: Fluent, idx: number) => void): Fluent<HTMLElement> {
    if (!this.el || !callback)
        return this;
    this.el.querySelectorAll<HTMLElement>(selector).forEach((x, i) => callback(Fluent(x), i));
    return this;
}

Fluent.prototype.hide = function (this: FluentThis) {
    return this.toggle(false);
}

Fluent.prototype.hidden = function (this: FluentThis<any>, value?: boolean) {
    if (value === void 0 && arguments.length < 1)
        return this.el?.hidden;

    if (this.el)
        this.el.hidden = !!value;

    return this;
}

Fluent.prototype.matches = function (this: FluentThis, selector?: string): boolean {
    return !!this.el && typeof this.el.matches === "function" && this.el.matches(selector);
}

Fluent.prototype.nextSibling = function (this: FluentThis, selector?: string): Fluent<HTMLElement> {
    let sibling = this.el?.nextElementSibling;
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
    let sibling = this.el?.previousElementSibling;
    while (sibling && selector != null && !sibling.matches(selector))
        sibling = sibling.previousElementSibling;
    return new (Fluent as any)(sibling);
}

Fluent.prototype.remove = function (this: FluentThis<any>) {
    Fluent.remove(this.el);
    return this;
}

Fluent.prototype.show = function (this: FluentThis) {
    return this.toggle(true);
}

Fluent.prototype.style = function (this: FluentThis, callback: (css: CSSStyleDeclaration) => void) {
    if (this.el && ((typeof CSSStyleDeclaration !== "undefined" && this.el.style instanceof CSSStyleDeclaration) || 
        (typeof CSSStyleDeclaration === "undefined" && typeof this.el.style === "object" && this.el.style != null)))
        callback(this.el.style);
    return this;
}

Fluent.prototype.trigger = function (this: FluentThis, type: string, args?: any) {
    this.el && triggerEvent(this.el, type, args);
    return this;
}

Fluent.prototype.val = function (this: FluentThis<any>, value?: string) {
    if (value === void 0 && !arguments.length) {
        if (!this.el)
            return void 0;
        return Fluent.isInputLike(this.el) ? this.el.value : "";
    }
    if (this.el && Fluent.isInputLike(this.el))
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
            notifyDisposingNode(this.el, { descendants: true, excludeSelf: true });
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


/**
 * Executes a callback once the DOM is ready.
 * @param callback - Function to invoke. When jQuery is available, delegated to `$(callback)`; otherwise uses `DOMContentLoaded` or an async tick when already loaded.
 * @returns `void`.
 */
Fluent.ready = function (callback: () => void) {
    if (!callback)
        return;

    let $ = getjQuery();
    if ($) {
        $(callback);
        return;
    }

    if (typeof document !== "undefined" && document.readyState === 'loading') {
        const loaded = () => {
            document.removeEventListener('DOMContentLoaded', loaded);
            callback();
        }
        document.addEventListener('DOMContentLoaded', loaded);
        return;
    }

    setTimeout(callback, 0);
}

/**
 * Finds the element with the given ID in the document.
 * @param id - Element ID to search for.
 * @returns A {@link Fluent} wrapping the found element, or an empty wrapper when not found.
 */
Fluent.byId = function <TElement extends HTMLElement>(id: string): Fluent<TElement> {
    return Fluent<TElement>(document.getElementById(id) as TElement);
}

/**
 * Finds all elements matching a selector in the document.
 * @param selector - CSS selector to query.
 * @returns An array of matching elements (empty when none match).
 */
Fluent.findAll = function <TElement extends HTMLElement>(selector: string): TElement[] {
    return Array.from(document.querySelectorAll<TElement>(selector));
}

/**
 * Iterates over all elements matching a selector, invoking a callback with a {@link Fluent} wrapper for each.
 * @param selector - CSS selector to query.
 * @param callback - Function invoked for each matching element.
 * @returns `void`.
 */
Fluent.findEach = function <TElement extends HTMLElement>(selector: string, callback: (el: Fluent<TElement>) => void): void {
    if (!callback)
        return;
    document.querySelectorAll<TElement>(selector).forEach(x => callback(Fluent(x)));
}

/**
 * Finds the first element matching a selector in the document.
 * @param selector - CSS selector to query.
 * @returns A {@link Fluent} wrapping the first match, or an empty wrapper when not found.
 */
Fluent.findFirst = function <TElement extends HTMLElement>(selector: string): Fluent<TElement> {
    return Fluent<TElement>(document.querySelector<TElement>(selector));
}