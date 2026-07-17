import { PubSub } from "./pubsub";

describe("PubSub", () => {
    describe("subscribe", () => {
        it("should add a handler", () => {
            const pubsub = new PubSub<number>();
            const handler = vi.fn();
            pubsub.subscribe(handler);
            pubsub.notify(42);
            expect(handler).toHaveBeenCalledWith(42);
        });

        it("should add multiple handlers", () => {
            const pubsub = new PubSub<string>();
            const handler1 = vi.fn();
            const handler2 = vi.fn();
            pubsub.subscribe(handler1);
            pubsub.subscribe(handler2);
            pubsub.notify("test");
            expect(handler1).toHaveBeenCalledWith("test");
            expect(handler2).toHaveBeenCalledWith("test");
        });
    });

    describe("unsubscribe", () => {
        it("should remove a specific handler", () => {
            const pubsub = new PubSub<number>();
            const handler1 = vi.fn();
            const handler2 = vi.fn();
            pubsub.subscribe(handler1);
            pubsub.subscribe(handler2);
            pubsub.unsubscribe(handler1);
            pubsub.notify(99);
            expect(handler1).not.toHaveBeenCalled();
            expect(handler2).toHaveBeenCalledWith(99);
        });

        it("should not fail when unsubscribing a non-existent handler", () => {
            const pubsub = new PubSub<number>();
            const handler = vi.fn();
            pubsub.subscribe(handler);
            expect(() => pubsub.unsubscribe(() => { })).not.toThrow();
            pubsub.notify(1);
            expect(handler).toHaveBeenCalledWith(1);
        });

        it("should remove all instances of the same handler", () => {
            const pubsub = new PubSub<number>();
            const handler = vi.fn();
            pubsub.subscribe(handler);
            pubsub.subscribe(handler);
            pubsub.unsubscribe(handler);
            pubsub.notify(7);
            expect(handler).not.toHaveBeenCalled();
        });

        it("should do nothing when called on an empty PubSub", () => {
            const pubsub = new PubSub<number>();
            expect(() => pubsub.unsubscribe(() => { })).not.toThrow();
        });
    });

    describe("notify", () => {
        it("should call all handlers with the event", () => {
            const pubsub = new PubSub<{ id: number }>();
            const handler1 = vi.fn();
            const handler2 = vi.fn();
            pubsub.subscribe(handler1);
            pubsub.subscribe(handler2);
            const event = { id: 1 };
            pubsub.notify(event);
            expect(handler1).toHaveBeenCalledWith(event);
            expect(handler2).toHaveBeenCalledWith(event);
        });

        it("should work with no handlers subscribed", () => {
            const pubsub = new PubSub<string>();
            expect(() => pubsub.notify("hello")).not.toThrow();
        });

        it("should stop notifying when isCancelled returns true", () => {
            const pubsub = new PubSub<number>();
            const handler1 = vi.fn();
            const handler2 = vi.fn();
            pubsub.subscribe(handler1);
            pubsub.subscribe(handler2);

            let notifyCount = 0;
            pubsub.notify(1, {
                isCancelled: (e) => {
                    notifyCount++;
                    return true;
                }
            });
            expect(handler1).toHaveBeenCalledWith(1);
            expect(handler2).not.toHaveBeenCalled();
            expect(notifyCount).toBe(1);
        });

        it("should not stop when isCancelled returns false", () => {
            const pubsub = new PubSub<number>();
            const handler1 = vi.fn();
            const handler2 = vi.fn();
            pubsub.subscribe(handler1);
            pubsub.subscribe(handler2);
            pubsub.notify(5, {
                isCancelled: () => false
            });
            expect(handler1).toHaveBeenCalledWith(5);
            expect(handler2).toHaveBeenCalledWith(5);
        });

        it("should work without the opt parameter", () => {
            const pubsub = new PubSub<number>();
            const handler = vi.fn();
            pubsub.subscribe(handler);
            pubsub.notify(10);
            expect(handler).toHaveBeenCalledWith(10);
        });
    });

    describe("clear", () => {
        it("should remove all handlers", () => {
            const pubsub = new PubSub<number>();
            const handler1 = vi.fn();
            const handler2 = vi.fn();
            pubsub.subscribe(handler1);
            pubsub.subscribe(handler2);
            pubsub.clear();
            pubsub.notify(1);
            expect(handler1).not.toHaveBeenCalled();
            expect(handler2).not.toHaveBeenCalled();
        });

        it("should work on an already empty PubSub", () => {
            const pubsub = new PubSub<number>();
            expect(() => pubsub.clear()).not.toThrow();
        });
    });
});
