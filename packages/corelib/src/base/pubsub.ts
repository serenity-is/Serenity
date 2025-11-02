
/** A simple publisher-subscriber implementation. */
export class PubSub<TEvent = {}> {

    private handlers: ((e: TEvent) => void)[] = [];

    subscribe(fn: (e: TEvent) => void) {
        this.handlers.push(fn);
    }

    unsubscribe(fn: (e: TEvent) => void) {
        for (let i = this.handlers.length - 1; i >= 0; i--) {
            if (this.handlers[i] === fn) {
                this.handlers.splice(i, 1);
            }
        }
    }

    notify(e: TEvent): void {
        for (let handler of this.handlers)
            handler(e);
    }

    clear() {
        this.handlers = [];
    }
}