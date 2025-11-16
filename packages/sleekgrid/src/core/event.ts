
export interface IEventData<TArgs = {}, TEvent = {}> {
    args: TArgs;
    defaultPrevented: boolean;
    preventDefault(): void;
    stopPropagation(): void;
    stopImmediatePropagation(): void;
    isDefaultPrevented(): boolean;
    isImmediatePropagationStopped(): boolean;
    isPropagationStopped(): boolean;
    getReturnValue(): any;
    getReturnValues(): any[];
    nativeEvent: TEvent | null | undefined;
}

export type MergeArgKeys = "grid" | "column" | "node" | "row" | "cell" | "item";
export type EventData<TArgs = {}, TEvent = {}> = IEventData<TArgs, TEvent> & TEvent & { [key in keyof TArgs & (MergeArgKeys)]: TArgs[key] };
export type EventCallback<TArgs = {}, TEvent = {}> = (e: EventData<TArgs, TEvent>, args?: TArgs) => void;

let eventDataInitialized = false;

function addEventDataProp(name: string, isArgs: boolean) {
    Object.defineProperty(EventDataWrapper.prototype, name, {
        enumerable: true,
        configurable: true,
        get: function (this: EventDataWrapper<any, any>) {
            if (isArgs) {
                if (typeof this.args === "object" && this.args !== null)
                    return this.args[name];
            }
            else if (this.nativeEvent) {
                return this.nativeEvent[name];
            }
        },
        set: function (value) {

            if (isArgs) {
                if (typeof this.args === "object" && this.args !== null) {
                    this.args[name] = value;
                    return;
                }
            }
            else if (this.nativeEvent) {
                this.nativeEvent[name] = value;
                return;
            }

            Object.defineProperty(this, name, {
                enumerable: true,
                configurable: true,
                writable: true,
                value: value
            });
        }
    });
}


function initializeEventDataProps() {
    for (const key of [
        'altKey', 'char', 'bubbles', 'button', 'buttons', 'cancelable', 'changedTouches',
        'charCode', 'clientX', 'clientY', 'code', 'ctrlKey', 'currentTarget', 'detail', 'eventPhase',
        'key', 'keyCode', 'metaKey', 'offsetX', 'offsetY', 'originalEvent', 'pageX', 'pageY', 'pointerId',
        'pointerType', 'screenX', 'screenY', 'shiftKey', 'relatedTarget', 'target', 'targetTouches',
        'toElement', 'touches', 'type', 'view', 'which']) {
        addEventDataProp(key, false);
    }

    for (const key of ['grid', 'column', 'node', 'row', 'cell', 'item']) {
        addEventDataProp(key, true);
    }
}

/***
 * An event object for passing data to event handlers and letting them control propagation.
 * <p>This is pretty much identical to how W3C and jQuery implement events.</p>
 */
export class EventDataWrapper<TArgs, TEvent = {}> implements IEventData<TArgs, TEvent> {
    private _args: TArgs;
    private _isPropagationStopped = false;
    private _isImmediatePropagationStopped = false;
    private _isDefaultPrevented = false;
    private _nativeEvent: Event;
    private _returnValue: any;
    private _returnValues: any[] = [];

    constructor(event?: TEvent | null, args?: TArgs) {
        this._nativeEvent = event as Event;
        this._args = args || {} as TArgs;

        if (!eventDataInitialized) {
            eventDataInitialized = true;
            initializeEventDataProps();
        }
    }

    get defaultPrevented(): boolean { return this._isDefaultPrevented; }

    preventDefault() {
        this._isDefaultPrevented = true;
        this._nativeEvent?.preventDefault?.();
    }

    isDefaultPrevented() {
        if (this._isDefaultPrevented)
            return true;

        if (this._nativeEvent && ("defaultPrevented" in this._nativeEvent))
            return !!this._nativeEvent.defaultPrevented;

        if (this._nativeEvent && typeof (this._nativeEvent as any).isDefaultPrevented === "function")
            return (this._nativeEvent as any).isDefaultPrevented();

        return false;
    }

    /***
     * Stops event from propagating up the DOM tree.
     */
    stopPropagation() {
        this._isPropagationStopped = true;
        this._nativeEvent?.stopPropagation();
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
        this._nativeEvent?.stopImmediatePropagation();
    }

    /***
     * Returns whether stopImmediatePropagation was called on this event object.\
     */
    isImmediatePropagationStopped(): boolean {
        return this._isImmediatePropagationStopped;
    }

    get args(): TArgs {
        return this._args;
    }

    addReturnValue(value: any): void {
        this._returnValues.push(value);
        if (value !== undefined)
            this._returnValue = value;
    }

    getReturnValues(): any[] {
        return this._returnValues;
    }

    getReturnValue(): any {
        return this._returnValue;
    }

    get nativeEvent(): TEvent | null | undefined {
        return this._nativeEvent as TEvent;
    }
}

/***
 * A simple publisher-subscriber implementation.
 */
export class EventEmitter<TArgs = any, TEvent = {}> {

    private _handlers: EventCallback<TArgs, TEvent>[] = [];

    /***
     * Adds an event handler to be called when the event is fired.
     * <p>Event handler will receive two arguments - an <code>EventData</code> and the <code>data</code>
     * object the event was fired with.<p>
     * @param fn {Function} Event handler.
     */
    subscribe(fn: EventCallback<TArgs, TEvent>) {
        this._handlers.push(fn);
    }

    /***
     * Removes an event handler added with <code>subscribe(fn)</code>.
     * @param fn {Function} Event handler to be removed.
     */
    unsubscribe(fn: EventCallback<TArgs, TEvent>) {
        for (var i = this._handlers.length - 1; i >= 0; i--) {
            if (this._handlers[i] === fn) {
                this._handlers.splice(i, 1);
            }
        }
    }

    /***
     * Fires an event notifying all subscribers.
     * @param args {Object} Additional data object to be passed to all handlers.
     * @param e {EventDataWrapper}
     *      Optional.
     *      An <code>EventData</code> object to be passed to all handlers.
     *      For DOM events, an existing W3C/jQuery event object can be passed in.
     * @param scope {Object}
     *      Optional.
     *      The scope ("this") within which the handler will be executed.
     *      If not specified, the scope will be set to the <code>Event</code> instance.
     */
    notify(args?: TArgs, e?: TEvent, scope?: object): EventData<TArgs, TEvent> {
        const sed = new EventDataWrapper<TArgs, TEvent>(e, args);
        scope = scope || this;
        for (var i = 0; i < this._handlers.length && !(sed.isPropagationStopped() || sed.isImmediatePropagationStopped()); i++) {
            const returnValue = this._handlers[i].call(scope, sed, args);
            sed.addReturnValue(returnValue);
        }
        return sed as unknown as EventData<TArgs, TEvent>;
    }

    clear() {
        this._handlers = [];
    }
}

interface EventSubscriberEntry {
    event: EventEmitter<any, any>;
    handler: EventCallback<any, any>;
}

export class EventSubscriber {
    private _handlers: EventSubscriberEntry[] = [];

    subscribe<TArgs, TEvent>(event: EventEmitter<TArgs, TEvent>, handler: EventCallback<TArgs, TEvent>): this {
        this._handlers.push({
            event: event,
            handler: handler
        });
        event.subscribe(handler);

        return this;
    }

    unsubscribe<TArgs, TEvent>(event: EventEmitter<TArgs, TEvent>, handler: EventCallback<TArgs, TEvent>): this {
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

    unsubscribeAll(): EventSubscriber {
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
