describe('Slick.EventData', () => {
    it('stopPropagation stops event propagation', () => {
        const eventData = new Slick.EventData();

        expect(eventData.isPropagationStopped()).toBeFalsy();

        eventData.stopPropagation();
        expect(eventData.isPropagationStopped()).toBeTruthy();
    });

    it('stopImmediatePropagation stops event propagation', () => {
        const eventData = new Slick.EventData();

        expect(eventData.isImmediatePropagationStopped()).toBeFalsy();

        eventData.stopImmediatePropagation();
        expect(eventData.isImmediatePropagationStopped()).toBeTruthy();
    });
});

describe('Slick.Event', () => {
    it('can subscribe to an event', () => {
        const event = new Slick.Event();
        let isEventCalled = false;

        event.subscribe(() => {
            isEventCalled = true;
        });

        event.notify(null, new Slick.EventData(), null);

        expect(isEventCalled).toBeTruthy();
    });

    it('can subscribe to an event more than once', () => {
        const event = new Slick.Event();
        let isEventCalled = [false, false];

        event.subscribe(() => {
            isEventCalled[0] = true;
        });

        event.subscribe(() => {
            isEventCalled[1] = true;
        });

        event.notify(null, new Slick.EventData(), null)

        expect(isEventCalled[0]).toBeTruthy();
        expect(isEventCalled[1]).toBeTruthy();
    });

    it('can send event data to subscribers', () => {
        const event = new Slick.Event();
        let eventArgs: { foo?: string };

        event.subscribe((eventData, args) => {
            eventArgs = args;
        });

        event.notify({
            foo: 'bar',
        } as typeof eventArgs, new Slick.EventData(), null)

        expect(eventArgs).toBeDefined();
        expect(eventArgs.foo).toBe('bar');
    });

    it('can unsubscribe from an event', () => {
        const event = new Slick.Event();
        let callCount = 0;

        const handler = () => {
            callCount++;
        };

        event.subscribe(handler);
        event.unsubscribe(handler);

        event.notify(null, new Slick.EventData(), null)

        expect(callCount).toBe(0);
    });

    it('can unsubscribe from an specific event', () => {
        const event = new Slick.Event();
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

        event.notify(null, new Slick.EventData(), null)

        expect(callCount).toBe(1);
    });

    it('can send the scope to an event', () => {
        const event = new Slick.Event();
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
        event.notify(null, new Slick.EventData(), scope)

        expect(callCount).toBe(1);
    });

    it('doesnt notify subscribers when event propagation is stopped', () => {
        const event = new Slick.Event();
        const eventData = new Slick.EventData();
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
        const event = new Slick.Event();
        const eventData = new Slick.EventData();
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
        const event = new Slick.Event();
        let eventData: Slick.EventData;

        const handler = (data, _) => {
            eventData = data;
        };

        event.subscribe(handler);
        event.notify(null, null, null);

        expect(eventData).toBeDefined();
        expect(eventData.isPropagationStopped()).toBeFalsy();
        expect(eventData.isImmediatePropagationStopped()).toBeFalsy();
    });

    it('sends scope as the Event itself if it is not passed', () => {
        const event = new Slick.Event();
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
        const event = new Slick.Event();
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
        const event = new Slick.Event();
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
        const event = new Slick.Event();
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
