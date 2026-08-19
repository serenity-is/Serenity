/**
 * Core event object passed to every grid event handler. Mirrors W3C/jQuery event
 * semantics with propagation and default-prevent controls.
 * @template TArgs - Payload specific to the event.
 * @template TEvent - Native DOM event wrapped by this object.
 */
export interface IEventData<TArgs = {}, TEvent = {}> {
    /** Payload supplied by the event emitter (e.g. `{row, cell, grid}`). */
    args: TArgs;
    /** Whether {@link IEventData.preventDefault} has been called. */
    defaultPrevented: boolean;
    /**
     * Prevents the default action associated with the event.
     */
    preventDefault(): void;
    /**
     * Stops the event from bubbling further, but remaining handlers on the same
     * emitter still run. Also calls `stopPropagation` on the native event when present.
     */
    stopPropagation(): void;
    /**
     * Prevents remaining handlers from being executed. Also stops DOM propagation.
     */
    stopImmediatePropagation(): void;
    /** Returns `true` if {@link IEventData.preventDefault} has been called or the native event is default-prevented. */
    isDefaultPrevented(): boolean;
    /** Returns `true` if {@link IEventData.stopImmediatePropagation} has been called. */
    isImmediatePropagationStopped(): boolean;
    /** Returns `true` if {@link IEventData.stopPropagation} has been called. */
    isPropagationStopped(): boolean;
    /** Returns the last non-`undefined` return value from the handlers that have run. */
    getReturnValue(): any;
    /** Returns all return values collected from handlers. */
    getReturnValues(): any[];
    /** The wrapped native DOM event, if any. */
    nativeEvent: TEvent | null | undefined;
}

/** Shorthand keys that are hoisted from `args` onto `EventData` for ergonomic access (`e.grid`, `e.row`, …). */
export type MergeArgKeys = "grid" | "column" | "node" | "row" | "cell" | "item";
/**
 * Union type representing the actual event object handlers receive. It merges
 * {@link IEventData} with the native event and hoisted arg keys so callers can
 * access `e.row`, `e.cell`, `e.grid`, etc. directly.
 */
export type EventData<TArgs = {}, TEvent = {}> = IEventData<TArgs, TEvent> & TEvent & { [key in keyof TArgs & (MergeArgKeys)]: TArgs[key] };
/**
 * Handler signature for SleekGrid events.
 * @template TArgs - Event payload type.
 * @template TEvent - Wrapped native event type.
 * @param e - Event object with propagation controls and merged fields.
 * @param args - Optional duplicate of `e.args` for convenience.
 */
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

/**
 * Wraps a native DOM event and a payload object, exposing propagation controls.
 * Property access for common DOM fields (e.g. `clientX`, `key`, `target`) and arg keys
 * (`grid`, `row`, `cell`) is dynamically proxied via getters installed by
 * `initializeEventDataProps()`.
 * @template TArgs - Event payload type.
 * @template TEvent - Wrapped native event type.
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

    preventDefault(): void {
        this._isDefaultPrevented = true;
        this._nativeEvent?.preventDefault?.();
    }

    isDefaultPrevented(): boolean {
        if (this._isDefaultPrevented)
            return true;

        if (this._nativeEvent && ("defaultPrevented" in this._nativeEvent))
            return !!this._nativeEvent.defaultPrevented;

        if (this._nativeEvent && typeof (this._nativeEvent as any).isDefaultPrevented === "function")
            return (this._nativeEvent as any).isDefaultPrevented();

        return false;
    }

    /**
     * Stops event from propagating up the DOM tree and marks it as propagation-stopped.
     */
    stopPropagation(): void {
        this._isPropagationStopped = true;
        this._nativeEvent?.stopPropagation();
    }

    /**
     * Returns whether {@link EventDataWrapper.stopPropagation} was called on this event object.
     * @returns `true` if propagation was stopped.
     */
    isPropagationStopped(): boolean {
        return this._isPropagationStopped;
    }

    /**
     * Prevents remaining handlers from being executed and stops DOM propagation.
     */
    stopImmediatePropagation(): void {
        this._isImmediatePropagationStopped = true;
        this._nativeEvent?.stopImmediatePropagation();
    }

    /**
     * Returns whether {@link EventDataWrapper.stopImmediatePropagation} was called on this event object.
     * @returns `true` if immediate propagation was stopped.
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

/**
 * Lightweight publish–subscribe implementation used for all SleekGrid events.
 * @template TArgs - Payload type.
 * @template TEvent - Wrapped native event type.
 */
export class EventEmitter<TArgs = any, TEvent = {}> {

    private _handlers: EventCallback<TArgs, TEvent>[] = [];

    /**
     * Registers an event handler to be invoked when the event is fired.
     * Handlers receive `(eventData, args)` and run in insertion order.
     * @param fn - Handler to register.
     */
    subscribe(fn: EventCallback<TArgs, TEvent>): void {
        this._handlers.push(fn);
    }

    /**
     * Removes a previously registered handler.
     * @param fn - Handler to remove; must be the exact function reference passed to {@link EventEmitter.subscribe}.
     */
    unsubscribe(fn: EventCallback<TArgs, TEvent>): void {
        for (var i = this._handlers.length - 1; i >= 0; i--) {
            if (this._handlers[i] === fn) {
                this._handlers.splice(i, 1);
            }
        }
    }

    /**
     * Fires the event, invoking all subscribers in order until propagation is stopped.
     * @param args - Payload passed to handlers as `e.args`.
     * @param e - Optional native DOM event to wrap.
     * @param scope - `this` value for handlers; defaults to the emitter itself.
     * @returns The {@link EventData} object created for this notification (carries return values and propagation flags).
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

    /**
     * Removes all registered handlers.
     */
    clear(): void {
        this._handlers = [];
    }
}

interface EventSubscriberEntry {
    event: EventEmitter<any, any>;
    handler: EventCallback<any, any>;
}

/**
 * Aggregates subscriptions across multiple emitters and allows bulk unsubscribe.
 * Useful for plugins/components that subscribe to many grid events and need
 * a single `unsubscribeAll()` on destroy.
 */
export class EventSubscriber {
    private _handlers: EventSubscriberEntry[] = [];

    /**
     * Subscribes `handler` to `event` and tracks the pair for later bulk cleanup.
     * @param event - Emitter to subscribe to.
     * @param handler - Handler to register.
     * @returns `this` for chaining.
     */
    subscribe<TArgs, TEvent>(event: EventEmitter<TArgs, TEvent>, handler: EventCallback<TArgs, TEvent>): this {
        this._handlers.push({
            event: event,
            handler: handler
        });
        event.subscribe(handler);

        return this;
    }

    /**
     * Unsubscribes a previously tracked handler.
     * @param event - Emitter the handler was subscribed to.
     * @param handler - Handler to remove.
     * @returns `this` for chaining.
     */
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

    /**
     * Unsubscribes all tracked handlers.
     * @returns `this` for chaining.
     */
    unsubscribeAll(): EventSubscriber {
        var i = this._handlers.length;
        while (i--) {
            this._handlers[i].event.unsubscribe(this._handlers[i].handler);
        }
        this._handlers = [];

        return this;  // allow chaining
    }
}

/**
 * Legacy key-code constants.
 * @deprecated Prefer `KeyboardEvent.key`/`KeyboardEvent.code` checks over numeric codes.
 */
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
