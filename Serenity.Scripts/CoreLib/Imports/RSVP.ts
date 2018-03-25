declare class RSVP<TResult> {
    constructor(constructor: (p1: (p1: any) => void, p2: any) => void);
}

declare module RSVP {
    function on(handler: (e: any) => void): void;
    function resolve(): PromiseLike<any>;
}

// Type definitions for RSVP 3.0.9
// Project: github.com/tildeio/rsvp.js 3.0.9
// Definitions by: Taylor Brown <https://github.com/Taytay>
// Definitions: https://github.com/borisyankov/DefinitelyTyped

// Some of this file was taken from the type definitions for es6-promise https://github.com/borisyankov/DefinitelyTyped/blob/master/es6-promise/es6-promise.d.ts
// Credit for that file goes to: François de Campredon <https://github.com/fdecampredon/>

// Some of this file was taken from the type definitions for Q : https://github.com/borisyankov/DefinitelyTyped/blob/master/q/Q.d.ts
// Credit for that file goes to: Barrie Nemetchek <https://github.com/bnemetchek>, Andrew Gaspar <https://github.com/AndrewGaspar/>, John Reilly <https://github.com/johnnyreilly>

declare module RSVP {

    class Promise<R> implements PromiseLike<R> {
        /**
         * If you call resolve in the body of the callback passed to the constructor,
         * your promise is fulfilled with result object passed to resolve.
         * If you call reject your promise is rejected with the object passed to resolve.
         * For consistency and debugging (eg stack traces), obj should be an instanceof Error.
         * Any errors thrown in the constructor callback will be implicitly passed to reject().
         */
        constructor(callback: (resolve: (result?: R) => void, reject: (error: any) => void) => void, label?: string);

        /**
         * If you call resolve in the body of the callback passed to the constructor,
         * your promise will be fulfilled/rejected with the outcome of thenable passed to resolve.
         * If you call reject your promise is rejected with the object passed to resolve.
         * For consistency and debugging (eg stack traces), obj should be an instanceof Error.
         * Any errors thrown in the constructor callback will be implicitly passed to reject().
         */
        constructor(callback: (resolve: (thenable?: PromiseLike<R>) => void, reject: (error: any) => void) => void, label?: string);

        /**
         * onFulfilled is called when/if "promise" resolves. onRejected is called when/if "promise" rejects.
         * Both are optional, if either/both are omitted the next onFulfilled/onRejected in the chain is called.
         * Both callbacks have a single parameter , the fulfillment value or rejection reason.
         * "then" returns a new promise equivalent to the value you return from onFulfilled/onRejected after being passed through Promise.resolve.
         * If an error is thrown in the callback, the returned promise rejects with that error.
         *
         * @param onFulfilled called when/if "promise" resolves
         * @param onRejected called when/if "promise" rejects
         */
        then<U>(onFulfilled?: (value: R) => PromiseLike<U>, onRejected?: (error: any) => PromiseLike<U>): Promise<U>;

        /**
         * onFulfilled is called when/if "promise" resolves. onRejected is called when/if "promise" rejects.
         * Both are optional, if either/both are omitted the next onFulfilled/onRejected in the chain is called.
         * Both callbacks have a single parameter , the fulfillment value or rejection reason.
         * "then" returns a new promise equivalent to the value you return from onFulfilled/onRejected after being passed through Promise.resolve.
         * If an error is thrown in the callback, the returned promise rejects with that error.
         *
         * @param onFulfilled called when/if "promise" resolves
         * @param onRejected called when/if "promise" rejects
         */
        then<U>(onFulfilled?: (value: R) => PromiseLike<U>, onRejected?: (error: any) => U): Promise<U>;

        /**
         * onFulfilled is called when/if "promise" resolves. onRejected is called when/if "promise" rejects.
         * Both are optional, if either/both are omitted the next onFulfilled/onRejected in the chain is called.
         * Both callbacks have a single parameter , the fulfillment value or rejection reason.
         * "then" returns a new promise equivalent to the value you return from onFulfilled/onRejected after being passed through Promise.resolve.
         * If an error is thrown in the callback, the returned promise rejects with that error.
         *
         * @param onFulfilled called when/if "promise" resolves
         * @param onRejected called when/if "promise" rejects
         */
        then<U>(onFulfilled?: (value: R) => PromiseLike<U>, onRejected?: (error: any) => void): Promise<U>;

        /**
         * onFulfilled is called when/if "promise" resolves. onRejected is called when/if "promise" rejects.
         * Both are optional, if either/both are omitted the next onFulfilled/onRejected in the chain is called.
         * Both callbacks have a single parameter , the fulfillment value or rejection reason.
         * "then" returns a new promise equivalent to the value you return from onFulfilled/onRejected after being passed through Promise.resolve.
         * If an error is thrown in the callback, the returned promise rejects with that error.
         *
         * @param onFulfilled called when/if "promise" resolves
         * @param onRejected called when/if "promise" rejects
         */
        then<U>(onFulfilled?: (value: R) => U, onRejected?: (error: any) => PromiseLike<U>): Promise<U>;

        /**
         * onFulfilled is called when/if "promise" resolves. onRejected is called when/if "promise" rejects.
         * Both are optional, if either/both are omitted the next onFulfilled/onRejected in the chain is called.
         * Both callbacks have a single parameter , the fulfillment value or rejection reason.
         * "then" returns a new promise equivalent to the value you return from onFulfilled/onRejected after being passed through Promise.resolve.
         * If an error is thrown in the callback, the returned promise rejects with that error.
         *
         * @param onFulfilled called when/if "promise" resolves
         * @param onRejected called when/if "promise" rejects
         */
        then<U>(onFulfilled?: (value: R) => U, onRejected?: (error: any) => U): Promise<U>;

        /**
         * onFulfilled is called when/if "promise" resolves. onRejected is called when/if "promise" rejects.
         * Both are optional, if either/both are omitted the next onFulfilled/onRejected in the chain is called.
         * Both callbacks have a single parameter , the fulfillment value or rejection reason.
         * "then" returns a new promise equivalent to the value you return from onFulfilled/onRejected after being passed through Promise.resolve.
         * If an error is thrown in the callback, the returned promise rejects with that error.
         *
         * @param onFulfilled called when/if "promise" resolves
         * @param onRejected called when/if "promise" rejects
         */
        then<U>(onFulfilled?: (value: R) => U, onRejected?: (error: any) => void): Promise<U>;

        /**
         * Sugar for promise.then(undefined, onRejected)
         *
         * @param onRejected called when/if "promise" rejects
         */
        catch<U>(onRejected?: (error: any) => PromiseLike<U>): Promise<U>;

        /**
         * Sugar for promise.then(undefined, onRejected)
         *
         * @param onRejected called when/if "promise" rejects
         */
        catch<U>(onRejected?: (error: any) => U): Promise<U>;

        /**
         * Sugar for promise.then(undefined, onRejected)
         *
         * @param onRejected called when/if "promise" rejects
         */
        catch<U>(onRejected?: (error: any) => void): Promise<U>;

        finally(finallyCallback: () => any): Promise<R>;

        static all<T>(promises: PromiseLike<T>[]): Promise<T[]>;
        static all<T>(promises: any[]): Promise<T[]>;
        static race<R>(promises: Promise<R>[]): Promise<R>;

        /**
         @method resolve
         @param {Any} value value that the returned promise will be resolved with
         @param {String} label optional string for identifying the returned promise.
         Useful for tooling.
         @return {Promise} a promise that will become fulfilled with the given
         `value`
         */
        static resolve<T>(object: PromiseLike<T>): Promise<T>;
        static resolve<T>(object: T): Promise<T>;

        /**
         @method cast (Deprecated in favor of resolve
         @param {Any} value value that the returned promise will be resolved with
         @param {String} label optional string for identifying the returned promise.
         Useful for tooling.
         @return {Promise} a promise that will become fulfilled with the given
         `value`
         */
        static cast<T>(object: PromiseLike<T>, label?: string): Promise<T>;
        static cast<T>(object: T, label?: string): Promise<T>;

        /**
         `RSVP.Promise.reject` returns a promise rejected with the passed `reason`.
         */
        static reject(reason?: any): Promise<any>;
    }

    interface PromiseState<T> {
        state: string /* "fulfilled", "rejected", "pending" */;
        value?: T;
        reason?: any;
    }

    interface InstrumentEvent {
        guid: string;      // guid of promise. Must be globally unique, not just within the implementation
        childGuid: string; // child of child promise (for chained via `then`)
        eventName: string; // one of ['created', 'chained', 'fulfilled', 'rejected']
        detail: any;    // fulfillment value or rejection reason, if applicable
        label: string;     // label passed to promise's constructor
        timeStamp: number; // milliseconds elapsed since 1 January 1970 00:00:00 UTC up until now
    }

    export function on(eventName: string, callback: (value: any) => void): void;
    export function on(eventName: "error", errorHandler: (reason: any) => void): void;
    export function on(eventName: "created", listener: (event: InstrumentEvent) => void): void;
    export function on(eventName: "chained", listener: (event: InstrumentEvent) => void): void;
    export function on(eventName: "fulfilled", listener: (event: InstrumentEvent) => void): void;
    export function on(eventName: "rejected", listener: (event: InstrumentEvent) => void): void;

    export function configure(configName: string, value: any): void;
    export function configure(configName: "instrument", shouldInstrument: boolean): void;

    /**
     * configure('onerror', handler) is deprecated in favor of on('error', handler)
     * @param configName
     * @param errorHandler
     */
    export function configure(configName: "onerror", errorHandler: (reason: any) => void): void;

    /**
     * Make a promise that fulfills when every item in the array fulfills, and rejects if (and when) any item rejects.
     * the array passed to all can be a mixture of promise-like objects and other objects.
     * The fulfillment value is an array (in order) of fulfillment values. The rejection value is the first rejection value.
     */
    export function all<T>(promises: PromiseLike<T>[]): Promise<T[]>;
    export function all<T>(promises: any[]): Promise<T[]>;

    function race<R>(promises: Promise<R>[]): Promise<R>;

    /**
     `RSVP.Promise.reject` returns a promise rejected with the passed `reason`.
     */
    export function reject(reason?: any): Promise<any>;

    /**
     `RSVP.Promise.resolve` returns a promise that will become resolved with the
     passed `value`.
     */
    export function resolve<T>(object: PromiseLike<T>): Promise<T>;
    export function resolve<T>(object: T): Promise<T>;

    /**
     `RSVP.rethrow` will rethrow an error on the next turn of the JavaScript event
     loop in order to aid debugging.
     Promises A+ specifies that any exceptions that occur with a promise must be
     caught by the promises implementation and bubbled to the last handler. For
     this reason, it is recommended that you always specify a second rejection
     handler function to `then`. However, `RSVP.rethrow` will throw the exception
     outside of the promise, so it bubbles up to your console if in the browser,
     or domain/cause uncaught exception in Node. `rethrow` will also throw the
     error again so the error can be handled by the promise per the spec.
     */
    export function rethrow(reason: any): void;
}