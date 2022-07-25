export type Handler<TArgs, TEventData extends IEventData = IEventData> = (e: TEventData, args: TArgs) => void;

export interface IEventData {
    readonly type?: string;
    currentTarget?: EventTarget;
    target?: EventTarget;
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
 * @class EventData
 * @constructor
 */
export class EventData implements IEventData {
    private _isPropagationStopped = false;
    private _isImmediatePropagationStopped = false;

    /***
     * Stops event from propagating up the DOM tree.
     * @method stopPropagation
     */
    stopPropagation() {
        this._isPropagationStopped = true;
    }

    /***
     * Returns whether stopPropagation was called on this event object.
     * @method isPropagationStopped
     * @return {Boolean}
     */
    isPropagationStopped(): boolean {
        return this._isPropagationStopped;
    }

    /***
     * Prevents the rest of the handlers from being executed.
     * @method stopImmediatePropagation
     */
    stopImmediatePropagation() {
        this._isImmediatePropagationStopped = true;
    }

    /***
     * Returns whether stopImmediatePropagation was called on this event object.\
     * @method isImmediatePropagationStopped
     * @return {Boolean}
     */
    isImmediatePropagationStopped(): boolean {
        return this._isImmediatePropagationStopped;
    }
}

/***
 * A simple publisher-subscriber implementation.
 * @class Event
 * @constructor
 */
export class Event<TArgs = any, TEventData extends IEventData = IEventData> {

    private _handlers: Handler<TArgs, TEventData>[] = [];

    /***
     * Adds an event handler to be called when the event is fired.
     * <p>Event handler will receive two arguments - an <code>EventData</code> and the <code>data</code>
     * object the event was fired with.<p>
     * @method subscribe
     * @param fn {Function} Event handler.
     */
    subscribe(fn: Handler<TArgs, TEventData>) {
        this._handlers.push(fn);
    }

    /***
     * Removes an event handler added with <code>subscribe(fn)</code>.
     * @method unsubscribe
     * @param fn {Function} Event handler to be removed.
     */
    unsubscribe(fn: Handler<TArgs, TEventData>) {
        for (var i = this._handlers.length - 1; i >= 0; i--) {
            if (this._handlers[i] === fn) {
                this._handlers.splice(i, 1);
            }
        }
    }

    /***
     * Fires an event notifying all subscribers.
     * @method notify
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
        e = e || new EventData() as any;
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

interface EventHandlerEntry<TArgs = any, TEventData extends IEventData = IEventData> {
    event: Event<TArgs, TEventData>;
    handler: Handler<TArgs, TEventData>;
}

export class EventHandler<TArgs = any, TEventData extends IEventData = IEventData>  {
    private _handlers: EventHandlerEntry<TArgs, TEventData>[] = [];

    subscribe(event: Event<TArgs, TEventData>, handler: Handler<TArgs, TEventData>): this {
        this._handlers.push({
            event: event,
            handler: handler
        });
        event.subscribe(handler);

        return this;
    }

    unsubscribe(event: Event<TArgs, TEventData>, handler: Handler<TArgs, TEventData>): this {
        var i = this._handlers.length;
        while (i--) {
            if (this._handlers[i].event === event &&
                this._handlers[i].handler === handler) {
                this._handlers.splice(i, 1);
                event.unsubscribe(handler);
                return;
            }
        }

        return this;
    }

    unsubscribeAll(): EventHandler<TArgs, TEventData> {
        var i = this._handlers.length;
        while (i--) {
            this._handlers[i].event.unsubscribe(this._handlers[i].handler);
        }
        this._handlers = [];

        return this;  // allow chaining
    }
}

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