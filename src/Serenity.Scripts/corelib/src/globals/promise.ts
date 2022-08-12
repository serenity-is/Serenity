declare global {
    /**
     * Represents the completion of an asynchronous operation
     */

    interface PromiseConstructor {
        /**
         * Creates a new Promise.
         * @param executor A callback used to initialize the promise. This callback is passed two arguments:
         * a resolve callback used resolve the promise with a value or the result of another promise,
         * and a reject callback used to reject the promise with a provided reason or error.
         */
        new <T>(executor: (resolve: (value?: T | PromiseLike<T>) => void, reject: (reason?: any) => void) => void): PromiseLike<T>;

        /**
         * Creates a new rejected promise for the provided reason.
         * @param reason The reason the promise was rejected.
         * @returns A new rejected Promise.
         */
        reject(reason: any): PromiseLike<never>;

        /**
         * Creates a new rejected promise for the provided reason.
         * @param reason The reason the promise was rejected.
         * @returns A new rejected Promise.
         */
        reject<T>(reason: any): PromiseLike<T>;

        /**
         * Creates a new resolved promise for the provided value.
         * @param value A promise.
         * @returns A promise whose internal state matches the provided promise.
         */
        resolve<T>(value: T | PromiseLike<T>): PromiseLike<T>;

        /**
         * Creates a new resolved promise .
         * @returns A resolved promise.
         */
        resolve(): PromiseLike<void>;
    }

    var Promise: PromiseConstructor;
}

export {}