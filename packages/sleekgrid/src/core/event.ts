export interface IEventData {
    readonly type?: string;
    currentTarget?: EventTarget | null;
    target?: EventTarget | null;
    originalEvent?: any;
    defaultPrevented?: boolean;
    preventDefault?(): void;
    stopPropagation?(): void;
    stopImmediatePropagation?(): void;
    isDefaultPrevented?(): boolean;
    isImmediatePropagationStopped?(): boolean;
    isPropagationStopped?(): boolean;
}

/***
 * An event object for passing data to event handlers and letting them control propagation.
 * <p>This is pretty much identical to how W3C and jQuery implement events.</p>
 */
export class EventData implements IEventData {
    private _isPropagationStopped = false;
    private _isImmediatePropagationStopped = false;

    /***
     * Stops event from propagating up the DOM tree.
     */
    stopPropagation() {
        this._isPropagationStopped = true;
    }

    /***
     * Returns whether stopPropagation was called on this event object.
     */
    isPropagationStopped(): boolean {
        return this._isPropagationStopped;
    }

    /***
     * Prevents the rest of the handlers from being executed.
     */
    stopImmediatePropagation() {
        this._isImmediatePropagationStopped = true;
    }

    /***
     * Returns whether stopImmediatePropagation was called on this event object.\
     */
    isImmediatePropagationStopped(): boolean {
        return this._isImmediatePropagationStopped;
    }
}

/***
 * A simple publisher-subscriber implementation.
 */
export class EventEmitter<TArgs = any, TEventData extends IEventData = IEventData> {

    private _handlers: ((e: TEventData, args: TArgs) => void)[] = [];

    /***
     * Adds an event handler to be called when the event is fired.
     * <p>Event handler will receive two arguments - an <code>EventData</code> and the <code>data</code>
     * object the event was fired with.<p>
     * @param fn {Function} Event handler.
     */
    subscribe(fn: ((e: TEventData, args: TArgs) => void)) {
        this._handlers.push(fn);
    }

    /***
     * Removes an event handler added with <code>subscribe(fn)</code>.
     * @param fn {Function} Event handler to be removed.
     */
    unsubscribe(fn: ((e: TEventData, args: TArgs) => void)) {
        for (var i = this._handlers.length - 1; i >= 0; i--) {
            if (this._handlers[i] === fn) {
                this._handlers.splice(i, 1);
            }
        }
    }

    /***
     * Fires an event notifying all subscribers.
     * @param args {Object} Additional data object to be passed to all handlers.
     * @param e {EventData}
     *      Optional.
     *      An <code>EventData</code> object to be passed to all handlers.
     *      For DOM events, an existing W3C/jQuery event object can be passed in.
     * @param scope {Object}
     *      Optional.
     *      The scope ("this") within which the handler will be executed.
     *      If not specified, the scope will be set to the <code>Event</code> instance.
     */
    notify(args?: any, e?: TEventData, scope?: object) {
        e = patchEvent(e) || new EventData() as any;
        scope = scope || this;

        var returnValue;
        for (var i = 0; i < this._handlers.length && !(e.isPropagationStopped() || e.isImmediatePropagationStopped()); i++) {
            returnValue = this._handlers[i].call(scope, e, args);
        }

        return returnValue;
    }

    clear() {
        this._handlers = [];
    }
}

interface EventSubscriberEntry<TArgs = any, TEventData extends IEventData = IEventData> {
    event: EventEmitter<TArgs, TEventData>;
    handler: ((e: TEventData, args: TArgs) => void);
}

export class EventSubscriber<TArgs = any, TEventData extends IEventData = IEventData> {
    private _handlers: EventSubscriberEntry<TArgs, TEventData>[] = [];

    subscribe(event: EventEmitter<TArgs, TEventData>, handler: ((e: TEventData, args: TArgs) => void)): this {
        this._handlers.push({
            event: event,
            handler: handler
        });
        event.subscribe(handler);

        return this;
    }

    unsubscribe(event: EventEmitter<TArgs, TEventData>, handler: ((e: TEventData, args: TArgs) => void)): this {
        var i = this._handlers.length;
        while (i--) {
            if (this._handlers[i].event === event &&
                this._handlers[i].handler === handler) {
                this._handlers.splice(i, 1);
                event.unsubscribe(handler);
                return this;
            }
        }

        return this;
    }

    unsubscribeAll(): EventSubscriber<TArgs, TEventData> {
        var i = this._handlers.length;
        while (i--) {
            this._handlers[i].event.unsubscribe(this._handlers[i].handler);
        }
        this._handlers = [];

        return this;  // allow chaining
    }
}

/** @deprecated */
export const keyCode = {
    BACKSPACE: 8,
    DELETE: 46,
    DOWN: 40,
    END: 35,
    ENTER: 13,
    ESCAPE: 27,
    HOME: 36,
    INSERT: 45,
    LEFT: 37,
    PAGEDOWN: 34,
    PAGEUP: 33,
    RIGHT: 39,
    TAB: 9,
    UP: 38
}

function returnTrue() {
    return true;
}

function returnFalse() {
    return false;
}

// patches event so that it has methods jQuery event objects provides, for backward compatibility when jQuery is not loaded
export function patchEvent(e: IEventData) {
    if (e == null)
        return e;

    if (!e.isDefaultPrevented && e.preventDefault)
        e.isDefaultPrevented = function () { return this.defaultPrevented; }

    var org1: () => void, org2: () => void;
    if (!e.isImmediatePropagationStopped && (org1 = e.stopImmediatePropagation)) {
        e.isImmediatePropagationStopped = returnFalse;
        e.stopImmediatePropagation = function () { this.isImmediatePropagationStopped = returnTrue; org1.call(this); }
    }

    if (!e.isPropagationStopped && (org2 = e.stopPropagation)) {
        e.isPropagationStopped = returnFalse;
        e.stopPropagation = function () { this.isPropagationStopped = returnTrue; org2.call(this); }
    }

    return e;
}
