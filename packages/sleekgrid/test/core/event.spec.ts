import * as deprecatedWorkaround from "../../src/core/event";
import { EventDataWrapper, EventEmitter, EventSubscriber, type EventData } from "../../src/core/event";

describe('EventDataWrapper', () => {
    it('stopPropagation stops event propagation', () => {
        const eventData = new EventDataWrapper();

        expect(eventData.isPropagationStopped()).toBeFalsy();

        eventData.stopPropagation();
        expect(eventData.isPropagationStopped()).toBeTruthy();
    });

    it('stopImmediatePropagation stops event propagation', () => {
        const eventData = new EventDataWrapper();

        expect(eventData.isImmediatePropagationStopped()).toBeFalsy();

        eventData.stopImmediatePropagation();
        expect(eventData.isImmediatePropagationStopped()).toBeTruthy();
    });
});

describe('EventEmitter', () => {
    it('can subscribe to an event', () => {
        const event = new EventEmitter();
        let isEventCalled = false;

        event.subscribe(() => {
            isEventCalled = true;
        });

        event.notify(null, new EventDataWrapper(), null);

        expect(isEventCalled).toBeTruthy();
    });

    it('can subscribe to an event more than once', () => {
        const event = new EventEmitter();
        let isEventCalled = [false, false];

        event.subscribe(() => {
            isEventCalled[0] = true;
        });

        event.subscribe(() => {
            isEventCalled[1] = true;
        });

        event.notify(null, new EventDataWrapper(), null)

        expect(isEventCalled[0]).toBeTruthy();
        expect(isEventCalled[1]).toBeTruthy();
    });

    it('can send event data to subscribers', () => {
        const event = new EventEmitter();
        let eventArgs: any;

        event.subscribe((eventData, args) => {
            eventArgs = args;
        });

        event.notify({
            foo: 'bar',
        }, new EventDataWrapper(), null)

        expect(eventArgs).toBeDefined();
        expect(eventArgs.foo).toBe('bar');
    });

    it('can unsubscribe from an event', () => {
        const event = new EventEmitter();
        let callCount = 0;

        const handler = () => {
            callCount++;
        };

        event.subscribe(handler);
        event.unsubscribe(handler);

        event.notify(null, new EventDataWrapper(), null)

        expect(callCount).toBe(0);
    });

    it('can unsubscribe from an specific event', () => {
        const event = new EventEmitter();
        let callCount = 0;

        const handler = () => {
            callCount += 2;
        };

        const otherHandler = () => {
            callCount++;
        };

        event.subscribe(handler);
        event.subscribe(otherHandler);
        event.unsubscribe(handler);

        event.notify(null, new EventDataWrapper(), null)

        expect(callCount).toBe(1);
    });

    it('can send the scope to an event', () => {
        const event = new EventEmitter();
        let callCount = 0;

        const scope = {
            incrementCallCount: () => {
                callCount++;
            }
        };

        const handler = function () {
            this.incrementCallCount();
        };

        event.subscribe(handler);
        event.notify(null, new EventDataWrapper(), scope)

        expect(callCount).toBe(1);
    });

    it('doesnt notify subscribers when event propagation is stopped', () => {
        const event = new EventEmitter();
        let callCount = 0;

        const handler1 = (e) => {
            callCount++;
            e.stopPropagation();
        };

        const handler2 = () => {
            callCount++;
        }

        event.subscribe(handler1);
        event.subscribe(handler2);

        event.notify(null, null, null)

        expect(callCount).toBe(1);
    });

    it('doesnt notify remaining subscribers when event immediate propagation is stopped', () => {
        const event = new EventEmitter();
        let callCount = 0;

        const handler1 = (e) => {
            callCount++;
            e.stopImmediatePropagation();
        };

        const handler2 = () => {
            callCount++;
        }

        event.subscribe(handler1);
        event.subscribe(handler2);

        event.notify(null, null, null)

        expect(callCount).toBe(1);
    });

    it('sends EventDate as an new instance if it is not passed', () => {
        const event = new EventEmitter();
        let eventData: any;

        event.subscribe((data, _) => {
            eventData = data;
        });

        event.notify(null, null, null);

        expect(eventData).toBeDefined();
        expect(eventData.isPropagationStopped()).toBeFalsy();
        expect(eventData.isImmediatePropagationStopped()).toBeFalsy();
    });

    it('sends scope as the Event itself if it is not passed', () => {
        const event = new EventEmitter();
        let callCount = 0;

        const handler = function () {
            callCount++;
            this.unsubscribe(handler);
        }

        event.subscribe(handler);
        event.notify(null, null, null);
        event.notify(null, null, null);

        expect(callCount).toBe(1);
    });

    it('notify returns last handlers returned value for compatibility', () => {
        const event = new EventEmitter();
        let callCount = 0;

        const normalReturnHandler = () => {
            callCount++;
            return callCount;
        }

        const doubleReturnHandler = () => {
            callCount++;
            return callCount * 2;
        }

        event.subscribe(normalReturnHandler); // callCount will be 1
        event.subscribe(doubleReturnHandler); // callCount will be 2
        const firstValue = event.notify(null, null, null).getReturnValue(); // double return handler will return 4

        event.unsubscribe(doubleReturnHandler);
        const secondValue = event.notify(null, null, null).getReturnValue(); // callCount will be 3 - normal return handler will return 3

        expect(firstValue).toBe(4);
        expect(secondValue).toBe(3);
    });

    it('unsubscribe removes all handlers of the same reference', () => {
        const event = new EventEmitter();
        let callCount = 0;

        const handler = () => {
            callCount++;
        };

        event.subscribe(() => handler());

        for (let i = 0; i < 10; i++)
            event.subscribe(handler);

        event.unsubscribe(handler);

        event.notify(null, null, null);
        expect(callCount).toBe(1);
    });

    it('clear removes all handlers', () => {
        const event = new EventEmitter();
        let callCount = 0;

        const handler = () => {
            callCount++;
        };

        for (let i = 0; i < 10; i++)
            event.subscribe((() => handler()));

        event.clear();

        event.notify(null, null, null);
        expect(callCount).toBe(0);
    });
});

describe('EventSubscriber', () => {

    it('unsubcribe method can be chained', function () {
        const event = new EventEmitter();
        const subscriber = new EventSubscriber();
        const handler = () => { };
        subscriber.subscribe(event, handler);
        const actual = subscriber.unsubscribe(event, handler);
        expect(actual).toBe(subscriber);
        const actual2 = subscriber.unsubscribe(event, handler);
        expect(actual2).toBe(subscriber);
    });

    it('automatically subscribes handler to the event', function () {
        let isEventCalled = false;

        const event = new EventEmitter();

        const subscriber = new EventSubscriber();
        subscriber.subscribe(event, () => {
            isEventCalled = true;
        });

        event.notify(null, null, null);

        expect(isEventCalled).toBeTruthy();
    });

    it('automatically subscribes multiple handlers to the event', function () {
        let isEventCalled = [false, false];

        const event = new EventEmitter();
        const subscriber = new EventSubscriber();

        subscriber.subscribe(event, () => isEventCalled[0] = true);
        subscriber.subscribe(event, () => isEventCalled[1] = true);

        event.notify(null, null, null);

        expect(isEventCalled[0]).toBeTruthy();
        expect(isEventCalled[1]).toBeTruthy();
    });

    it('unsubscribes handler from event', function () {
        let eventCallCount = 0;

        const event = new EventEmitter();
        const subscriber = new EventSubscriber();

        const handler = () => eventCallCount++;

        subscriber.subscribe(event, handler);
        event.notify(null, null, null);

        expect(eventCallCount).toBe(1);

        subscriber.unsubscribe(event, handler);
        event.notify(null, null, null);

        expect(eventCallCount).toBe(1);
    });

    it('unsubscribes all handlers of the same reference from event', function () {
        let eventCallCount = 0;

        const event = new EventEmitter();
        const subscriber = new EventSubscriber();

        const handler = () => eventCallCount++;

        subscriber.subscribe(event, handler);
        subscriber.subscribe(event, handler);
        subscriber.subscribe(event, handler);
        event.notify(null, null, null);

        expect(eventCallCount).toBe(3);

        subscriber.unsubscribe(event, handler);
        event.notify(null, null, null);

        expect(eventCallCount).toBe(3);
    });

    it('unsubscribes handler from an specific event', function () {
        let eventCallCount = 0;

        const event = new EventEmitter();
        const otherEvent = new EventEmitter();

        const subscriber = new EventSubscriber();

        const handler = () => eventCallCount++;

        subscriber.subscribe(event, handler);
        subscriber.subscribe(otherEvent, handler);

        event.notify(null, null, null);
        otherEvent.notify(null, null, null);

        expect(eventCallCount).toBe(2);

        subscriber.unsubscribe(event, handler);
        event.notify(null, null, null);
        otherEvent.notify(null, null, null);

        expect(eventCallCount).toBe(3);
    });

    it('unsubscribes all handlers of the same reference from an specific event', function () {
        let eventCallCount = 0;

        const event = new EventEmitter();
        const otherEvent = new EventEmitter();

        const subscriber = new EventSubscriber();

        const handler = () => eventCallCount++;

        subscriber.subscribe(event, handler);
        subscriber.subscribe(event, handler);
        subscriber.subscribe(event, handler);
        subscriber.subscribe(otherEvent, handler);

        event.notify(null, null, null);
        otherEvent.notify(null, null, null);

        expect(eventCallCount).toBe(4);

        subscriber.unsubscribe(event, handler);
        event.notify(null, null, null);
        otherEvent.notify(null, null, null);

        expect(eventCallCount).toBe(5);
    });

    it('unsubscribes from all event and handlers', function () {
        let eventCallCount = 0;

        const event = new EventEmitter();
        const otherEvent = new EventEmitter();

        const subscriber = new EventSubscriber();

        const handler = () => eventCallCount++;

        subscriber.subscribe(event, handler);
        subscriber.subscribe(event, handler);
        subscriber.subscribe(event, handler);
        subscriber.subscribe(otherEvent, handler);
        subscriber.subscribe(otherEvent, handler);
        subscriber.subscribe(otherEvent, handler);

        event.notify(null, null, null);
        otherEvent.notify(null, null, null);

        expect(eventCallCount).toBe(6);

        subscriber.unsubscribeAll();
        event.notify(null, null, null);
        otherEvent.notify(null, null, null);

        expect(eventCallCount).toBe(6);
    });
});

describe('EventDataWrapper', () => {
    it('returns undefined when e is undefined', () => {
        const result = new EventDataWrapper(undefined);
        expect(result.nativeEvent).toBeUndefined();
    });

    it('returns null when e is null', () => {
        const result = new EventDataWrapper(null);
        expect(result.nativeEvent).toBeNull();
    });

    it('returns original object when it is empty', () => {
        const e = {};
        var result = new EventDataWrapper(e);
        expect(result.nativeEvent).toBe(e);
    });


    it('adds isDefaultPrevented when it has preventDefault', () => {
        let calls = 0;
        const e: EventData = {
            defaultPrevented: false,
            preventDefault: function () { this.defaultPrevented = true; calls++; }
        } as any;

        const result = new EventDataWrapper(e);
        expect(result.nativeEvent).toBe(e);
        expect(e.stopPropagation).toBeUndefined();
        expect(e.stopImmediatePropagation).toBeUndefined();

        expect(result.isDefaultPrevented != null).toBe(true);
        expect(result.isDefaultPrevented()).toBe(false);
        expect(result.defaultPrevented).toBe(false);
        expect(calls).toBe(0);

        e.preventDefault();
        expect(result.isDefaultPrevented()).toBe(true);
        expect(e.defaultPrevented).toBe(true);

        e.preventDefault();
        expect(result.isDefaultPrevented()).toBe(true);
        expect(e.defaultPrevented).toBe(true);
        expect(calls).toBe(2);
    });

    it('adds isPropagationStopped when it has stopPropagation', () => {
        let calls = 0;
        const e: EventData = {
            stopPropagation: function () { calls++; }
        } as any;

        const result = new EventDataWrapper(e);
        expect(result.nativeEvent).toBe(e);
        expect(e.preventDefault).toBeUndefined();
        expect(e.stopImmediatePropagation).toBeUndefined();
        expect(e.defaultPrevented).toBeUndefined();
        expect(result.isPropagationStopped != null).toBe(true);
        expect(result.isPropagationStopped()).toBe(false);
        expect(calls).toBe(0);

        result.stopPropagation();
        expect(result.isPropagationStopped()).toBe(true);

        result.stopPropagation();
        expect(result.isPropagationStopped()).toBe(true);
        expect(calls).toBe(2);
    });

    it('adds isImmediatePropagationStopped when it has stopImmediatePropagation', () => {
        let calls = 0;
        const e: EventData = {
            stopImmediatePropagation: function () { calls++; }
        } as any;

        const result = new EventDataWrapper(e);
        expect(result.nativeEvent).toBe(e);
        expect(e.preventDefault).toBeUndefined();
        expect(e.stopPropagation).toBeUndefined();
        expect(e.defaultPrevented).toBe(undefined);
        expect(result.isImmediatePropagationStopped != null).toBe(true);
        expect(result.isImmediatePropagationStopped()).toBe(false);
        expect(calls).toBe(0);

        result.stopImmediatePropagation();
        expect(result.isImmediatePropagationStopped()).toBe(true);

        result.stopImmediatePropagation();
        expect(result.isImmediatePropagationStopped()).toBe(true);
        expect(calls).toBe(2);
    });

});

describe('keyCode', () => {
    it("has correct codes", () => {
        expect((deprecatedWorkaround as any).keyCode.BACKSPACE).toBe(8);
    });
});
