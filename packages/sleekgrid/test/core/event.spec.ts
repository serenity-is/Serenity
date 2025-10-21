import * as deprecatedWorkaround from "../../src/core/event";
import { EventData, EventEmitter, EventSubscriber, IEventData, patchEvent } from "../../src/core/event";

describe('EventData', () => {
    it('stopPropagation stops event propagation', () => {
        const eventData = new EventData();

        expect(eventData.isPropagationStopped()).toBeFalsy();

        eventData.stopPropagation();
        expect(eventData.isPropagationStopped()).toBeTruthy();
    });

    it('stopImmediatePropagation stops event propagation', () => {
        const eventData = new EventData();

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

        event.notify(null, new EventData(), null);

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

        event.notify(null, new EventData(), null)

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
        }, new EventData(), null)

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

        event.notify(null, new EventData(), null)

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

        event.notify(null, new EventData(), null)

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
        event.notify(null, new EventData(), scope)

        expect(callCount).toBe(1);
    });

    it('doesnt notify subscribers when event propagation is stopped', () => {
        const event = new EventEmitter();
        const eventData = new EventData();
        let callCount = 0;

        const handler = () => {
            callCount++;
        };

        event.subscribe(handler);

        eventData.stopPropagation();
        event.notify(null, eventData, null)

        expect(callCount).toBe(0);
    });

    it('doesnt notify subscribers when event immediate propagation is stopped', () => {
        const event = new EventEmitter();
        const eventData = new EventData();
        let callCount = 0;

        const handler = () => {
            callCount++;
        };

        event.subscribe(handler);

        eventData.stopImmediatePropagation();
        event.notify(null, eventData, null)

        expect(callCount).toBe(0);
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

    it('notify returns last handlers returned value', () => {
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
        const firstValue = event.notify(null, null, null); // double return handler will return 4

        event.unsubscribe(doubleReturnHandler);
        const secondValue = event.notify(null, null, null); // callCount will be 3 - normal return handler will return 3

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
        const handler = () => {};
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

describe('patchEvent', () => {
    it('returns undefined when e is undefined', () => {
        const result = patchEvent(undefined);
        expect(result).toBeUndefined();
    });

    it('returns null when e is null', () => {
        const result = patchEvent(null);
        expect(result).toBeNull();
    });

    it('returns original object when it is empty', () => {
        const e = {};
        var result = patchEvent(e);
        expect(result).toBe(e);
    });


    it('adds isDefaultPrevented when it has preventDefault', () => {
        let calls = 0;
        const e: IEventData = {
            defaultPrevented: false,
            preventDefault: function() { this.defaultPrevented = true; calls++; }
        };

        const result = patchEvent(e);
        expect(result).toBe(e);
        expect(e.stopPropagation).toBeUndefined();
        expect(e.stopImmediatePropagation).toBeUndefined();

        expect(e.isDefaultPrevented != null).toBe(true);
        expect(e.isDefaultPrevented()).toBe(false);
        expect(e.defaultPrevented).toBe(false);
        expect(calls).toBe(0);

        e.preventDefault();
        expect(e.isDefaultPrevented()).toBe(true);
        expect(e.defaultPrevented).toBe(true);

        e.preventDefault();
        expect(e.isDefaultPrevented()).toBe(true);
        expect(e.defaultPrevented).toBe(true);
        expect(calls).toBe(2);
    });

    it('adds isPropagationStopped when it has stopPropagation', () => {
        let calls = 0;
        const e: IEventData = {
            stopPropagation: function() { calls++; }
        };

        const result = patchEvent(e);
        expect(result).toBe(e);
        expect(e.preventDefault).toBeUndefined();
        expect(e.stopImmediatePropagation).toBeUndefined();
        expect(e.defaultPrevented).toBeUndefined();
        expect(e.isPropagationStopped != null).toBe(true);
        expect(e.isPropagationStopped()).toBe(false);
        expect(calls).toBe(0);

        result.stopPropagation();
        expect(e.isPropagationStopped()).toBe(true);

        result.stopPropagation();
        expect(e.isPropagationStopped()).toBe(true);
        expect(calls).toBe(2);
    });

    it('adds isImmediatePropagationStopped when it has stopImmediatePropagation', () => {
        let calls = 0;
        const e: IEventData = {
            stopImmediatePropagation: function() { calls++; }
        };

        const result = patchEvent(e);
        expect(result).toBe(e);
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
