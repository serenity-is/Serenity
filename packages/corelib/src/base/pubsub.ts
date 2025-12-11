
/** A simple publisher-subscriber implementation. */
export class PubSub<TEvent = {}> {

    #handlers: ((e: TEvent) => void)[] = [];

    subscribe(fn: (e: TEvent) => void) {
        this.#handlers.push(fn);
    }

    unsubscribe(fn: (e: TEvent) => void) {
        for (let i = this.#handlers.length - 1; i >= 0; i--) {
            if (this.#handlers[i] === fn) {
                this.#handlers.splice(i, 1);
            }
        }
    }

    notify(e: TEvent, opt?: { isCancelled: (e: TEvent) => boolean}): void {
        for (let handler of this.#handlers) {
            handler(e);
            if (opt?.isCancelled(e)) {
                break;
            }
        }
    }

    clear() {
        this.#handlers = [];
    }
}