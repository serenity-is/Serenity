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
        let eventArgs;

        event.subscribe((eventData, args) => {
            eventArgs = args;
        });

        event.notify({
            foo: 'bar',
        }, new Slick.EventData(), null)

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
        let eventData;

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

describe('Slick.EventHandler', () => {
    it('automatically subscribes handler to the event', function () {
        let isEventCalled = false;
        
        const event = new Slick.Event();
        
        const eventHandler = new Slick.EventHandler();
        eventHandler.subscribe(event, () => {
            isEventCalled = true;
        });
        
        event.notify(null, null, null);
        
        expect(isEventCalled).toBeTruthy();
    });

    it('automatically subscribes multiple handlers to the event', function () {
        let isEventCalled = [false, false];

        const event = new Slick.Event();
        const eventHandler = new Slick.EventHandler();
        
        eventHandler.subscribe(event, () => isEventCalled[0] = true);
        eventHandler.subscribe(event, () => isEventCalled[1] = true);

        event.notify(null, null, null);

        expect(isEventCalled[0]).toBeTruthy();
        expect(isEventCalled[1]).toBeTruthy();
    });

    it('unsubscribes handler from event', function () {
        let eventCallCount = 0;
        
        const event = new Slick.Event();
        const eventHandler = new Slick.EventHandler();
        
        const handler = () => eventCallCount++;
        
        eventHandler.subscribe(event, handler);
        event.notify(null, null, null);
        
        expect(eventCallCount).toBe(1);
        
        eventHandler.unsubscribe(event, handler);
        event.notify(null, null, null);

        expect(eventCallCount).toBe(1);
    });

    it('unsubscribes all handlers of the same reference from event', function () {
        let eventCallCount = 0;

        const event = new Slick.Event();
        const eventHandler = new Slick.EventHandler();

        const handler = () => eventCallCount++;

        eventHandler.subscribe(event, handler);
        eventHandler.subscribe(event, handler);
        eventHandler.subscribe(event, handler);
        event.notify(null, null, null);

        expect(eventCallCount).toBe(3);

        eventHandler.unsubscribe(event, handler);
        event.notify(null, null, null);

        expect(eventCallCount).toBe(3);
    });

    it('unsubscribes handler from an specific event', function () {
        let eventCallCount = 0;

        const event = new Slick.Event();
        const otherEvent = new Slick.Event();
        
        const eventHandler = new Slick.EventHandler();

        const handler = () => eventCallCount++;

        eventHandler.subscribe(event, handler);
        eventHandler.subscribe(otherEvent, handler);

        event.notify(null, null, null);
        otherEvent.notify(null, null, null);

        expect(eventCallCount).toBe(2);

        eventHandler.unsubscribe(event, handler);
        event.notify(null, null, null);
        otherEvent.notify(null, null, null);

        expect(eventCallCount).toBe(3);
    });

    it('unsubscribes all handlers of the same reference from an specific event', function () {
        let eventCallCount = 0;

        const event = new Slick.Event();
        const otherEvent = new Slick.Event();

        const eventHandler = new Slick.EventHandler();

        const handler = () => eventCallCount++;

        eventHandler.subscribe(event, handler);
        eventHandler.subscribe(event, handler);
        eventHandler.subscribe(event, handler);
        eventHandler.subscribe(otherEvent, handler);

        event.notify(null, null, null);
        otherEvent.notify(null, null, null);

        expect(eventCallCount).toBe(4);

        eventHandler.unsubscribe(event, handler);
        event.notify(null, null, null);
        otherEvent.notify(null, null, null);

        expect(eventCallCount).toBe(5);
    });

    it('unsubscribes from all event and handlers', function () {
        let eventCallCount = 0;

        const event = new Slick.Event();
        const otherEvent = new Slick.Event();

        const eventHandler = new Slick.EventHandler();

        const handler = () => eventCallCount++;

        eventHandler.subscribe(event, handler);
        eventHandler.subscribe(event, handler);
        eventHandler.subscribe(event, handler);
        eventHandler.subscribe(otherEvent, handler);
        eventHandler.subscribe(otherEvent, handler);
        eventHandler.subscribe(otherEvent, handler);

        event.notify(null, null, null);
        otherEvent.notify(null, null, null);

        expect(eventCallCount).toBe(6);

        eventHandler.unsubscribeAll();
        event.notify(null, null, null);
        otherEvent.notify(null, null, null);

        expect(eventCallCount).toBe(6);
    });
});

describe('Slick.Range', () => {
    it('sets toRow to fromRow if toRow is undefined', () => {
        const range = new Slick.Range(1, 2);
        
        expect(range.toRow).toBe(range.fromRow);
    });

    it('sets toCell to fromCell if toRow is undefined', () => {
        const range = new Slick.Range(1, 2);

        expect(range.toCell).toBe(range.fromCell);
    });

    it('isSingleRow returns true if toRow and fromRow is the same', () => {
        const range = new Slick.Range(1, 2);

        expect(range.isSingleRow()).toBeTruthy();
    });

    it('isSingleRow returns false if toRow and fromRow is not the same', () => {
        const range = new Slick.Range(1, 2, 3, 2);

        expect(range.isSingleRow()).toBeFalsy();
    });

    it('isSingleCell returns true if row and the cell is the same ', () => {
        const range = new Slick.Range(1, 2, 1, 2);

        expect(range.isSingleCell()).toBeTruthy();
    });

    it('isSingleCell returns false if row is not the same ', () => {
        const range = new Slick.Range(1, 2, 2, 2);

        expect(range.isSingleCell()).toBeFalsy();
    });

    it('isSingleCell returns false if cell is not the same ', () => {
        const range = new Slick.Range(1, 2, 1, 3);

        expect(range.isSingleCell()).toBeFalsy();
    });

    it('isSingleCell returns false if row and cell is not the same ', () => {
        const range = new Slick.Range(1, 2, 2, 3);

        expect(range.isSingleCell()).toBeFalsy();
    });

    it('contains returns true if range contains it', () => {
        const [fromRow, fromCell, toRow, toCell] = [2, 5, 3, 6];
        const range = new Slick.Range(fromRow, fromCell, toRow, toCell);
        
        for (let currentRow = 0; currentRow < toRow + 4; currentRow++) {
            for (let currentCell = 0; currentCell < toCell + 4; currentCell++) {
                
                const expected = currentRow >= fromRow && currentRow <= toRow &&
                    currentCell >= fromCell && currentCell <= toCell;
                
                expect(range.contains(currentRow, currentCell)).toBe(expected);
            }
        }
    });

    it('toString should return only (fromRow:fromCell) if range contains only one cell', () => {
        const range = new Slick.Range(1, 2);
        
        expect(range.toString()).toBe('(1:2)');
    });

    it('toString should return (fromRow:fromCell - toRow:toCell) if range is not same row', () => {
        const range = new Slick.Range(1, 2, 2, 2);

        expect(range.toString()).toBe('(1:2 - 2:2)');
    });

    it('toString should return (fromRow:fromCell - toRow:toCell) if range is not same cell', () => {
        const range = new Slick.Range(1, 2, 1, 3);

        expect(range.toString()).toBe('(1:2 - 1:3)');
    });

    it('toString should return (fromRow:fromCell - toRow:toCell) if range is not single cell', () => {
        const range = new Slick.Range(1, 2, 2, 3);

        expect(range.toString()).toBe('(1:2 - 2:3)');
    });
});