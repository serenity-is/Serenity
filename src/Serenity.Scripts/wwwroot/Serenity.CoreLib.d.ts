/// <reference types="jquery" />
/// <reference types="jquery.validation" />

/******************************************************************************
Copyright (c) Microsoft Corporation.

Permission to use, copy, modify, and/or distribute this software for any
purpose with or without fee is hereby granted.

THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH
REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY
AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT,
INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM
LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR
OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR
PERFORMANCE OF THIS SOFTWARE.
***************************************************************************** */

/**
 * Used to shim class extends.
 *
 * @param d The derived class.
 * @param b The base class.
 */
declare function __extends(d: Function, b: Function): void;

/**
 * Copy the values of all of the enumerable own properties from one or more source objects to a
 * target object. Returns the target object.
 *
 * @param t The target object to copy to.
 * @param sources One or more source objects from which to copy properties
 */
declare function __assign(t: any, ...sources: any[]): any;

/**
 * Performs a rest spread on an object.
 *
 * @param t The source value.
 * @param propertyNames The property names excluded from the rest spread.
 */
declare function __rest(t: any, propertyNames: (string | symbol)[]): any;

/**
 * Applies decorators to a target object
 *
 * @param decorators The set of decorators to apply.
 * @param target The target object.
 * @param key If specified, the own property to apply the decorators to.
 * @param desc The property descriptor, defaults to fetching the descriptor from the target object.
 * @experimental
 */
declare function __decorate(decorators: Function[], target: any, key?: string | symbol, desc?: any): any;

/**
 * Creates an observing function decorator from a parameter decorator.
 *
 * @param paramIndex The parameter index to apply the decorator to.
 * @param decorator The parameter decorator to apply. Note that the return value is ignored.
 * @experimental
 */
declare function __param(paramIndex: number, decorator: Function): Function;

/**
 * Creates a decorator that sets metadata.
 *
 * @param metadataKey The metadata key
 * @param metadataValue The metadata value
 * @experimental
 */
declare function __metadata(metadataKey: any, metadataValue: any): Function;

/**
 * Converts a generator function into a pseudo-async function, by treating each `yield` as an `await`.
 *
 * @param thisArg The reference to use as the `this` value in the generator function
 * @param _arguments The optional arguments array
 * @param P The optional promise constructor argument, defaults to the `Promise` property of the global object.
 * @param generator The generator function
 */
declare function __awaiter(thisArg: any, _arguments: any, P: Function, generator: Function): any;

/**
 * Creates an Iterator object using the body as the implementation.
 *
 * @param thisArg The reference to use as the `this` value in the function
 * @param body The generator state-machine based implementation.
 *
 * @see [./docs/generator.md]
 */
declare function __generator(thisArg: any, body: Function): any;

/**
 * Creates bindings for all enumerable properties of `m` on `exports`
 *
 * @param m The source object
 * @param exports The `exports` object.
 */
declare function __exportStar(m: any, o: any): void;

/**
 * Creates a value iterator from an `Iterable` or `ArrayLike` object.
 *
 * @param o The object.
 * @throws {TypeError} If `o` is neither `Iterable`, nor an `ArrayLike`.
 */
declare function __values(o: any): any;

/**
 * Reads values from an `Iterable` or `ArrayLike` object and returns the resulting array.
 *
 * @param o The object to read from.
 * @param n The maximum number of arguments to read, defaults to `Infinity`.
 */
declare function __read(o: any, n?: number): any[];

/**
 * Creates an array from iterable spread.
 *
 * @param args The Iterable objects to spread.
 * @deprecated since TypeScript 4.2 - Use `__spreadArray`
 */
declare function __spread(...args: any[][]): any[];

/**
 * Creates an array from array spread.
 *
 * @param args The ArrayLikes to spread into the resulting array.
 * @deprecated since TypeScript 4.2 - Use `__spreadArray`
 */
declare function __spreadArrays(...args: any[][]): any[];

/**
 * Spreads the `from` array into the `to` array.
 *
 * @param pack Replace empty elements with `undefined`.
 */
declare function __spreadArray(to: any[], from: any[], pack?: boolean): any[];

/**
 * Creates an object that signals to `__asyncGenerator` that it shouldn't be yielded,
 * and instead should be awaited and the resulting value passed back to the generator.
 *
 * @param v The value to await.
 */
declare function __await(v: any): any;

/**
 * Converts a generator function into an async generator function, by using `yield __await`
 * in place of normal `await`.
 *
 * @param thisArg The reference to use as the `this` value in the generator function
 * @param _arguments The optional arguments array
 * @param generator The generator function
 */
declare function __asyncGenerator(thisArg: any, _arguments: any, generator: Function): any;

/**
 * Used to wrap a potentially async iterator in such a way so that it wraps the result
 * of calling iterator methods of `o` in `__await` instances, and then yields the awaited values.
 *
 * @param o The potentially async iterator.
 * @returns A synchronous iterator yielding `__await` instances on every odd invocation
 *          and returning the awaited `IteratorResult` passed to `next` every even invocation.
 */
declare function __asyncDelegator(o: any): any;

/**
 * Creates a value async iterator from an `AsyncIterable`, `Iterable` or `ArrayLike` object.
 *
 * @param o The object.
 * @throws {TypeError} If `o` is neither `AsyncIterable`, `Iterable`, nor an `ArrayLike`.
 */
declare function __asyncValues(o: any): any;

/**
 * Creates a `TemplateStringsArray` frozen object from the `cooked` and `raw` arrays.
 *
 * @param cooked The cooked possibly-sparse array.
 * @param raw The raw string content.
 */
declare function __makeTemplateObject(cooked: string[], raw: string[]): TemplateStringsArray;

/**
 * Used to shim default and named imports in ECMAScript Modules transpiled to CommonJS.
 *
 * ```js
 * import Default, { Named, Other } from "mod";
 * // or
 * import { default as Default, Named, Other } from "mod";
 * ```
 *
 * @param mod The CommonJS module exports object.
 */
declare function __importStar<T>(mod: T): T;

/**
 * Used to shim default imports in ECMAScript Modules transpiled to CommonJS.
 *
 * ```js
 * import Default from "mod";
 * ```
 *
 * @param mod The CommonJS module exports object.
 */
declare function __importDefault<T>(mod: T): T | { default: T };

/**
 * Emulates reading a private instance field.
 *
 * @param receiver The instance from which to read the private field.
 * @param state A WeakMap containing the private field value for an instance.
 * @param kind Either `"f"` for a field, `"a"` for an accessor, or `"m"` for a method.
 *
 * @throws {TypeError} If `state` doesn't have an entry for `receiver`.
 */
declare function __classPrivateFieldGet<T extends object, V>(
    receiver: T,
    state: { has(o: T): boolean, get(o: T): V | undefined },
    kind?: "f"
): V;

/**
 * Emulates reading a private static field.
 *
 * @param receiver The object from which to read the private static field.
 * @param state The class constructor containing the definition of the static field.
 * @param kind Either `"f"` for a field, `"a"` for an accessor, or `"m"` for a method.
 * @param f The descriptor that holds the static field value.
 *
 * @throws {TypeError} If `receiver` is not `state`.
 */
declare function __classPrivateFieldGet<T extends new (...args: any[]) => unknown, V>(
    receiver: T,
    state: T,
    kind: "f",
    f: { value: V }
): V;

/**
 * Emulates evaluating a private instance "get" accessor.
 *
 * @param receiver The instance on which to evaluate the private "get" accessor.
 * @param state A WeakSet used to verify an instance supports the private "get" accessor.
 * @param kind Either `"f"` for a field, `"a"` for an accessor, or `"m"` for a method.
 * @param f The "get" accessor function to evaluate.
 *
 * @throws {TypeError} If `state` doesn't have an entry for `receiver`.
 */
declare function __classPrivateFieldGet<T extends object, V>(
    receiver: T,
    state: { has(o: T): boolean },
    kind: "a",
    f: () => V
): V;

/**
 * Emulates evaluating a private static "get" accessor.
 *
 * @param receiver The object on which to evaluate the private static "get" accessor.
 * @param state The class constructor containing the definition of the static "get" accessor.
 * @param kind Either `"f"` for a field, `"a"` for an accessor, or `"m"` for a method.
 * @param f The "get" accessor function to evaluate.
 *
 * @throws {TypeError} If `receiver` is not `state`.
 */
declare function __classPrivateFieldGet<T extends new (...args: any[]) => unknown, V>(
    receiver: T,
    state: T,
    kind: "a",
    f: () => V
): V;

/**
 * Emulates reading a private instance method.
 *
 * @param receiver The instance from which to read a private method.
 * @param state A WeakSet used to verify an instance supports the private method.
 * @param kind Either `"f"` for a field, `"a"` for an accessor, or `"m"` for a method.
 * @param f The function to return as the private instance method.
 *
 * @throws {TypeError} If `state` doesn't have an entry for `receiver`.
 */
declare function __classPrivateFieldGet<T extends object, V extends (...args: any[]) => unknown>(
    receiver: T,
    state: { has(o: T): boolean },
    kind: "m",
    f: V
): V;

/**
 * Emulates reading a private static method.
 *
 * @param receiver The object from which to read the private static method.
 * @param state The class constructor containing the definition of the static method.
 * @param kind Either `"f"` for a field, `"a"` for an accessor, or `"m"` for a method.
 * @param f The function to return as the private static method.
 *
 * @throws {TypeError} If `receiver` is not `state`.
 */
declare function __classPrivateFieldGet<T extends new (...args: any[]) => unknown, V extends (...args: any[]) => unknown>(
    receiver: T,
    state: T,
    kind: "m",
    f: V
): V;

/**
 * Emulates writing to a private instance field.
 *
 * @param receiver The instance on which to set a private field value.
 * @param state A WeakMap used to store the private field value for an instance.
 * @param value The value to store in the private field.
 * @param kind Either `"f"` for a field, `"a"` for an accessor, or `"m"` for a method.
 *
 * @throws {TypeError} If `state` doesn't have an entry for `receiver`.
 */
declare function __classPrivateFieldSet<T extends object, V>(
    receiver: T,
    state: { has(o: T): boolean, set(o: T, value: V): unknown },
    value: V,
    kind?: "f"
): V;

/**
 * Emulates writing to a private static field.
 *
 * @param receiver The object on which to set the private static field.
 * @param state The class constructor containing the definition of the private static field.
 * @param value The value to store in the private field.
 * @param kind Either `"f"` for a field, `"a"` for an accessor, or `"m"` for a method.
 * @param f The descriptor that holds the static field value.
 *
 * @throws {TypeError} If `receiver` is not `state`.
 */
declare function __classPrivateFieldSet<T extends new (...args: any[]) => unknown, V>(
    receiver: T,
    state: T,
    value: V,
    kind: "f",
    f: { value: V }
): V;

/**
 * Emulates writing to a private instance "set" accessor.
 *
 * @param receiver The instance on which to evaluate the private instance "set" accessor.
 * @param state A WeakSet used to verify an instance supports the private "set" accessor.
 * @param value The value to store in the private accessor.
 * @param kind Either `"f"` for a field, `"a"` for an accessor, or `"m"` for a method.
 * @param f The "set" accessor function to evaluate.
 *
 * @throws {TypeError} If `state` doesn't have an entry for `receiver`.
 */
declare function __classPrivateFieldSet<T extends object, V>(
    receiver: T,
    state: { has(o: T): boolean },
    value: V,
    kind: "a",
    f: (v: V) => void
): V;

/**
 * Emulates writing to a private static "set" accessor.
 *
 * @param receiver The object on which to evaluate the private static "set" accessor.
 * @param state The class constructor containing the definition of the static "set" accessor.
 * @param value The value to store in the private field.
 * @param kind Either `"f"` for a field, `"a"` for an accessor, or `"m"` for a method.
 * @param f The "set" accessor function to evaluate.
 *
 * @throws {TypeError} If `receiver` is not `state`.
 */
declare function __classPrivateFieldSet<T extends new (...args: any[]) => unknown, V>(
    receiver: T,
    state: T,
    value: V,
    kind: "a",
    f: (v: V) => void
): V;

/**
 * Checks for the existence of a private field/method/accessor.
 *
 * @param state The class constructor containing the static member, or the WeakMap or WeakSet associated with a private instance member.
 * @param receiver The object for which to test the presence of the private member.
 */
declare function __classPrivateFieldIn(
    state: (new (...args: any[]) => unknown) | { has(o: any): boolean },
    receiver: unknown,
): boolean;

/**
 * Creates a re-export binding on `object` with key `objectKey` that references `target[key]`.
 *
 * @param object The local `exports` object.
 * @param target The object to re-export from.
 * @param key The property key of `target` to re-export.
 * @param objectKey The property key to re-export as. Defaults to `key`.
 */
declare function __createBinding(object: object, target: object, key: PropertyKey, objectKey?: PropertyKey): void;


declare namespace Q {
    /**
     * Tests if any of array elements matches given predicate. Prefer Array.some() over this function (e.g. `[1, 2, 3].some(predicate)`).
     * @param array Array to test.
     * @param predicate Predicate to test elements.
     * @returns True if any element matches.
     */
    function any<TItem>(array: TItem[], predicate: (x: TItem) => boolean): boolean;
    /**
     * Counts number of array elements that matches a given predicate.
     * @param array Array to test.
     * @param predicate Predicate to test elements.
     */
    function count<TItem>(array: TItem[], predicate: (x: TItem) => boolean): number;
    /**
     * Gets first element in an array that matches given predicate similar to LINQ's First.
     * Throws an error if no match is found.
     * @param array Array to test.
     * @param predicate Predicate to test elements.
     * @returns First element that matches.
     */
    function first<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem;
    /**
     * A group item returned by `groupBy()`.
     */
    type GroupByElement<TItem> = {
        /** index of the item in `inOrder` array */
        order: number;
        /** key of the group */
        key: string;
        /** the items in the group */
        items: TItem[];
        /** index of the first item of this group in the original array */
        start: number;
    };
    /**
     * Return type of the `groupBy` function.
     */
    type GroupByResult<TItem> = {
        byKey: {
            [key: string]: GroupByElement<TItem>;
        };
        inOrder: GroupByElement<TItem>[];
    };
    /**
     * Groups an array with keys determined by specified getKey() callback.
     * Resulting object contains group objects in order and a dictionary to access by key.
     * This is similar to LINQ's ToLookup function with some additional details like start index.
     * @param items Array to group.
     * @param getKey Function that returns key for each item.
     * @returns GroupByResult object.
     */
    function groupBy<TItem>(items: TItem[], getKey: (x: TItem) => any): GroupByResult<TItem>;
    /**
     * Gets index of first element in an array that matches given predicate.
     * @param array Array to test.
     * @param predicate Predicate to test elements.
     */
    function indexOf<TItem>(array: TItem[], predicate: (x: TItem) => boolean): number;
    /**
     * Inserts an item to the array at specified index. Prefer Array.splice unless
     * you need to support IE.
     * @param obj Array or array like object to insert to.
     * @param index Index to insert at.
     * @param item Item to insert.
     * @throws Error if object does not support insert.
     * @example
     * insert([1, 2, 3], 1, 4); // [1, 4, 2, 3]
     * insert({ insert: (index, item) => { this.splice(index, 0, item); } }
     */
    function insert(obj: any, index: number, item: any): void;
    /**
     * Determines if the object is an array. Prefer Array.isArray over this function (e.g. `Array.isArray(obj)`).
     * @param obj Object to test.
     * @returns True if the object is an array.
     * @example
     * isArray([1, 2, 3]); // true
     * isArray({}); // false
     */
    const isArray: (arg: any) => arg is any[];
    /**
    * Gets first element in an array that matches given predicate.
    * Throws an error if no matches is found, or there are multiple matches.
    * @param array Array to test.
    * @param predicate Predicate to test elements.
    * @returns First element that matches.
    * @example
    * first([1, 2, 3], x => x == 2); // 2
    * first([1, 2, 3], x => x == 4); // throws error.
    */
    function single<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem;
    type Grouping<TItem> = {
        [key: string]: TItem[];
    };
    /**
     * Maps an array into a dictionary with keys determined by specified getKey() callback,
     * and values that are arrays containing elements for a particular key.
     * @param items Array to map.
     * @param getKey Function that returns key for each item.
     * @returns Grouping object.
     * @example
     * toGrouping([1, 2, 3], x => x % 2 == 0 ? "even" : "odd"); // { odd: [1, 3], even: [2] }
     */
    function toGrouping<TItem>(items: TItem[], getKey: (x: TItem) => any): Grouping<TItem>;
    /**
     * Gets first element in an array that matches given predicate (similar to LINQ's FirstOrDefault).
     * Returns null if no match is found.
     * @param array Array to test.
     * @param predicate Predicate to test elements.
     * @returns First element that matches.
     * @example
     * tryFirst([1, 2, 3], x => x == 2); // 2
     * tryFirst([1, 2, 3], x => x == 4); // null
     */
    function tryFirst<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem;

    interface UserDefinition {
        /**
         * Username of the logged user
         */
        Username?: string;
        /**
         * Display name of the logged user
         */
        DisplayName?: string;
        /**
         * This indicates that the user is a super "admin", e.g. assumed to have all the permissions available.
         * It does not mean a member of Administrators, who might not have some of the permissions */
        IsAdmin?: boolean;
        /**
         * A hashset of permission keys that the current user have, explicitly assigned or via its
         * roles. Note that client side permission checks should only be used for UI enable/disable etc.
         * You should not rely on client side permission checks and always re-check permissions server side.
         */
        Permissions?: {
            [key: string]: boolean;
        };
    }

    /**
     * Contains permission related functions.
     *
     * ## Note
     * We use a namespace here both for compatibility and for allowing users to override
     * these functions easily in ES modules environment, which is normally hard to do.
     */
    namespace Authorization {
        /**
         * Checks if the current user has the permission specified.
         * This should only be used for UI purposes and it is strongly recommended to check permissions server side.
         *
         * > Please prefer the `hasPermissionAsync` variant as this may block the UI thread if the `UserData` script is not already loaded.
         * @param permission Permission key. It may contain logical operators like A&B|C.
         * @returns `false` for "null or undefined", true for "*", `IsLoggedIn` for "?". For other permissions,
         * if the user has the permission or if the user has the `IsAdmin` flag (super admin) `true`, otherwise `false`.
         */
        function hasPermission(permission: string): boolean;
        /**
         * Checks if the current user has the permission specified.
         * This should only be used for UI purposes and it is strongly recommended to check permissions server side.
         *
         * @param permission Permission key. It may contain logical operators like A&B|C.
         * @returns `false` for "null or undefined", true for "*", `IsLoggedIn` for "?". For other permissions,
         * if the user has the permission or if the user has the `IsAdmin` flag (super admin) `true`, otherwise `false`.
         */
        function hasPermissionAsync(permission: string): Promise<boolean>;
        /**
         * Checks if the hashset contains the specified permission, also handling logical "|" and "&" operators
         * @param permissionSet Set of permissions
         * @param permission Permission key or a permission expression containing & | operators
         * @returns true if set contains permission
         */
        function isPermissionInSet(permissionSet: {
            [key: string]: boolean;
        }, permission: string): boolean;
        /**
         * Throws an error if the current user does not have the specified permission.
         * Prefer `await validatePermissionAsync()` as this one might block the UI if the `UserData`
         * is not already loaded.
         * @param permission Permission key. It may contain logical operators like A&B|C.
         */
        function validatePermission(permission: string): void;
        /**
        * Throws an error if the current user does not have the specified permission.
        * @param permission Permission key. It may contain logical operators like A&B|C.
        * @example
        * await Authorization.validatePermissionAsync("A&B|C");
        */
        function validatePermissionAsync(permission: string): Promise<void>;
    }
    namespace Authorization {
        /**
         * Checks if the current user is logged in. Prefer `isLoggedInAsync` as this one might block the UI if the `UserData`
         * is not already loaded.
         * @returns `true` if the user is logged in, `false` otherwise.
         * @example
         * if (Authorization.isLoggedIn) {
         *     // do something
         * }
         */
        let isLoggedIn: boolean;
        /**
         * Checks if the current user is logged in.
         * @returns `true` if the user is logged in, `false` otherwise.
         * @example
         * if (await Authorization.isLoggedInAsync) {
         *     // do something
         * }
         */
        let isLoggedInAsync: Promise<boolean>;
        /** Returns the username for currently logged user. Prefer `usernameAsync` as this one might block the UI if the `UserData`
         * is not already loaded.
         * @returns Username for currently logged user.
         * @example
         * if (Authorization.username) {
         *     // do something
         * }
         */
        let username: string;
        /** Returns the username for currently logged user.
         * @returns Username for currently logged user.
         * @example
         * if (await Authorization.usernameAsync) {
         *     // do something
         * }
         */
        let usernameAsync: Promise<string>;
        /** Returns the user data for currently logged user. Prefer `userDefinitionAsync` as this one might block the UI if the `UserData`
         * is not already loaded.
         * @returns User data for currently logged user.
         * @example
         * if (Authorization.userDefinition.IsAdmin) {
         *     // do something
         * }
         */
        let userDefinition: UserDefinition;
        /** Returns the user data for currently logged user.
         * @returns User data for currently logged user.
         * @example
         * if ((await Authorization.userDefinitionAsync).IsAdmin) {
         *     // do something
         * }
         */
        let userDefinitionAsync: Promise<UserDefinition>;
    }

    /** Options for the BlockUI plugin. */
    interface JQBlockUIOptions {
        useTimeout?: boolean;
    }
    /**
     * Uses jQuery BlockUI plugin to block access to whole page (default) or
     * a part of it, by using a transparent overlay covering the whole area.
     * @param options Parameters for the BlockUI plugin
     * @remarks If options are not specified, this function blocks
     * whole page with a transparent overlay. Default z-order of the overlay
     * div is 2000, so a higher z-order shouldn't be used in page.
     */
    function blockUI(options: JQBlockUIOptions): void;
    /**
     * Unblocks the page.
     */
    function blockUndo(): void;

    var Config: {
        /**
         * This is the root path of your application. If your application resides under http://localhost/mysite/,
         * your root path is "mysite/". This variable is automatically initialized by reading from a <link> element
         * with ID "ApplicationPath" from current page, which is usually located in your _LayoutHead.cshtml file
         */
        applicationPath: string;
        /**
         * Email validation by default only allows ASCII characters. Set this to true if you want to allow unicode.
         */
        emailAllowOnlyAscii: boolean;
        /**
         * @Obsolete defaulted to false before for backward compatibility, now its true by default
         */
        responsiveDialogs: boolean;
        /**
         * Set this to true, to prefer bootstrap dialogs over jQuery UI dialogs by default for message dialogs
         */
        bootstrapMessages: boolean;
        /**
         * This is the list of root namespaces that may be searched for types. For example, if you specify an editor type
         * of "MyEditor", first a class with name "MyEditor" will be searched, if not found, search will be followed by
         * "Serenity.MyEditor" and "MyApp.MyEditor" if you added "MyApp" to the list of root namespaces.
         *
         * You should usually add your application root namespace to this list in ScriptInitialization.ts file.
         */
        rootNamespaces: string[];
        /**
         * This is an optional method for handling when user is not logged in. If a users session is expired
         * and when a NotAuthorized response is received from a service call, Serenity will call this handler, so
         * you may intercept it and notify user about this situation and ask if she wants to login again...
         */
        notLoggedInHandler: Function;
    };

    interface DebouncedFunction<T extends (...args: any[]) => any> {
        /**
         * Call the original function, but applying the debounce rules.
         *
         * If the debounced function can be run immediately, this calls it and returns its return
         * value.
         *
         * Otherwise, it returns the return value of the last invocation, or undefined if the debounced
         * function was not invoked yet.
         */
        (...args: Parameters<T>): ReturnType<T> | undefined;
        /**
         * Throw away any pending invocation of the debounced function.
         */
        clear(): void;
        /**
         * If there is a pending invocation of the debounced function, invoke it immediately and return
         * its return value.
         *
         * Otherwise, return the value from the last invocation, or undefined if the debounced function
         * was never invoked.
         */
        flush(): ReturnType<T> | undefined;
    }
    /**
     * Returns a function, that, as long as it continues to be invoked, will not
     * be triggered. The function also has a property 'clear' that can be used
     * to clear the timer to prevent previously scheduled executions, and flush method
     * to invoke scheduled executions now if any.
     * @param wait The function will be called after it stops being called for
     * N milliseconds.
     * @param immediate If passed, trigger the function on the leading edge, instead of the trailing.
     *
     * @source underscore.js
     */
    function debounce<T extends (...args: any) => any>(func: T, wait?: number, immediate?: boolean): DebouncedFunction<T>;

    /**
     * Options for a message dialog button
     */
    interface DialogButton {
        /** Button text */
        text?: string;
        /** Button hint */
        hint?: string;
        /** Button icon */
        icon?: string;
        /** Click handler */
        click?: (e: MouseEvent) => void;
        /** CSS class for button */
        cssClass?: string;
        /** HTML encode button text. Default is true. */
        htmlEncode?: boolean;
        /** The code that is returned from message dialog function when this button is clicked */
        result?: string;
    }
    /**
     * Options that apply to all message dialog types
     */
    interface CommonDialogOptions {
        /** Event handler that is called when dialog is opened */
        onOpen?: () => void;
        /** Event handler that is called when dialog is closed */
        onClose?: (result: string) => void;
        /** Dialog title */
        title?: string;
        /** HTML encode the message, default is true */
        htmlEncode?: boolean;
        /** Wrap the message in a `<pre>` element, so that line endings are preserved, default is true */
        preWrap?: boolean;
        /** Dialog css class. Default is based on the message dialog type */
        dialogClass?: string;
        /** List of buttons to show on the dialog */
        buttons?: DialogButton[];
        /** Class to use for the modal element for Bootstrap dialogs */
        modalClass?: string;
        /** True to use Bootstrap dialogs even when jQuery UI  present, default is based on `Q.Config.bootstrapMessages */
        bootstrap?: boolean;
        /** The result code of the button used to close the dialog is returned via this variable in the options object */
        result?: string;
    }
    /** Returns true if Bootstrap 3 is loaded */
    function isBS3(): boolean;
    /** Returns true if Bootstrap 5+ is loaded */
    function isBS5Plus(): boolean;
    /**
     * Builds HTML DIV element for a Bootstrap modal dialog
     * @param title Modal title
     * @param body Modal body, it will not be HTML encoded, so make sure it is encoded
     * @param modalClass Optional class to add to the modal element
     * @param escapeHtml True to html encode body, default is true
     * @returns
     */
    function bsModalMarkup(title: string, body: string, modalClass?: string, escapeHtml?: boolean): HTMLDivElement;
    function dialogButtonToBS(x: DialogButton): HTMLButtonElement;
    function dialogButtonToUI(x: DialogButton): any;
    /**
     * Additional options for Alert dialogs
     */
    interface AlertOptions extends CommonDialogOptions {
        /** The title of OK button, or false to hide the OK button */
        okButton?: string | boolean;
        /** CSS class for OK button */
        okButtonClass?: string;
    }
    /**
     * Displays an alert dialog
     * @param message The message to display
     * @param options Additional options.
     * @see AlertOptions
     * @example
     * alertDialog("An error occured!"); }
     */
    function alertDialog(message: string, options?: AlertOptions): void;
    /** @obsolete use alertDialog */
    const alert: typeof alertDialog;
    /** Additional options for confirm dialog */
    interface ConfirmOptions extends CommonDialogOptions {
        /** Title of the Yes button, or false to hide the Yes button. Default is value of local text: "Dialogs.YesButton" */
        yesButton?: string | boolean;
        /** CSS class for the Yes button. */
        yesButtonClass?: string;
        /** Title of the NO button, or false to hide the No button. Default is value of local text: "Dialogs.NoButton" */
        noButton?: string | boolean;
        /** Title of the CANCEL button, or false to hide the Cancel button. Default is value of local text: "Dialogs.NoButton" */
        cancelButton?: string | boolean;
        /** Event handler for cancel button click */
        onCancel?: () => void;
        /** Event handler for no button click */
        onNo?: () => void;
    }
    /**
     * Display a confirmation dialog
     * @param message The message to display
     * @param onYes Callback for Yes button click
     * @param options Additional options.
     * @see ConfirmOptions
     * @example
     * confirmDialog("Are you sure you want to delete?", () => {
     *     // do something when yes is clicked
     * }
     */
    function confirmDialog(message: string, onYes: () => void, options?: ConfirmOptions): void;
    /** @obsolete use confirmDialog */
    const confirm: typeof confirmDialog;
    /** Options for `iframeDialog` **/
    interface IFrameDialogOptions {
        html?: string;
    }
    /**
     * Display a dialog that shows an HTML block in an IFRAME, which is usually returned from server callbacks
     * @param options The options
     */
    function iframeDialog(options: IFrameDialogOptions): void;
    /**
     * Display an information dialog
     * @param message The message to display
     * @param onOk Callback for OK button click
     * @param options Additional options.
     * @see ConfirmOptions
     * @example
     * informationDialog("Operation complete", () => {
     *     // do something when OK is clicked
     * }
     */
    function informationDialog(message: string, onOk?: () => void, options?: ConfirmOptions): void;
    /** @obsolete use informationDialog */
    const information: typeof informationDialog;
    /**
     * Display a success dialog
     * @param message The message to display
     * @param onOk Callback for OK button click
     * @param options Additional options.
     * @see ConfirmOptions
     * @example
     * successDialog("Operation complete", () => {
     *     // do something when OK is clicked
     * }
     */
    function successDialog(message: string, onOk?: () => void, options?: ConfirmOptions): void;
    /** @obsolete use successDialog */
    const success: typeof successDialog;
    /**
     * Display a warning dialog
     * @param message The message to display
     * @param options Additional options.
     * @see AlertOptions
     * @example
     * warningDialog("Something is odd!");
     */
    function warningDialog(message: string, options?: AlertOptions): void;
    /** @obsolete use warningDialog */
    const warning: typeof warningDialog;
    /**
     * Closes a panel, triggering panelbeforeclose and panelclose events.
     * If the panelbeforeclose prevents the default, the operation is cancelled.
     * @param element The panel element
     * @param e  The event triggering the close
     */
    function closePanel(element: JQuery | HTMLElement, e?: Event): void;
    function openPanel(element: JQuery | HTMLElement, uniqueName?: string): void;

    interface ServiceError {
        Code?: string;
        Arguments?: string;
        Message?: string;
        Details?: string;
        ErrorId?: string;
    }
    interface ServiceResponse {
        Error?: ServiceError;
    }
    interface ServiceRequest {
    }
    interface ServiceOptions<TResponse extends ServiceResponse> extends JQueryAjaxSettings {
        request?: any;
        service?: string;
        blockUI?: boolean;
        onError?(response: TResponse): void;
        onSuccess?(response: TResponse): void;
        onCleanup?(): void;
    }
    interface SaveRequest<TEntity> extends ServiceRequest {
        EntityId?: any;
        Entity?: TEntity;
        Localizations?: any;
    }
    interface SaveRequestWithAttachment<TEntity> extends SaveRequest<TEntity> {
        Attachments?: any[];
    }
    interface SaveResponse extends ServiceResponse {
        EntityId?: any;
    }
    interface SaveWithLocalizationRequest<TEntity> extends SaveRequest<TEntity> {
        Localizations?: {
            [key: string]: TEntity;
        };
    }
    interface DeleteRequest extends ServiceRequest {
        EntityId?: any;
    }
    interface DeleteResponse extends ServiceResponse {
    }
    interface UndeleteRequest extends ServiceRequest {
        EntityId?: any;
    }
    interface UndeleteResponse extends ServiceResponse {
    }
    enum ColumnSelection {
        List = 0,
        KeyOnly = 1,
        Details = 2,
        None = 3,
        IdOnly = 4,
        Lookup = 5
    }
    enum RetrieveColumnSelection {
        details = 0,
        keyOnly = 1,
        list = 2,
        none = 3,
        idOnly = 4,
        lookup = 5
    }
    interface ListRequest extends ServiceRequest {
        Skip?: number;
        Take?: number;
        Sort?: string[];
        ContainsText?: string;
        ContainsField?: string;
        Criteria?: any[];
        EqualityFilter?: any;
        IncludeDeleted?: boolean;
        ExcludeTotalCount?: boolean;
        ColumnSelection?: ColumnSelection;
        IncludeColumns?: string[];
        ExcludeColumns?: string[];
        ExportColumns?: string[];
        DistinctFields?: string[];
    }
    interface ListResponse<TEntity> extends ServiceResponse {
        Entities?: TEntity[];
        Values?: any[];
        TotalCount?: number;
        Skip?: number;
        Take?: number;
    }
    interface RetrieveRequest extends ServiceRequest {
        EntityId?: any;
        ColumnSelection?: RetrieveColumnSelection;
        IncludeColumns?: string[];
        ExcludeColumns?: string[];
    }
    interface RetrieveResponse<TEntity> extends ServiceResponse {
        Entity?: TEntity;
    }
    interface RetrieveLocalizationRequest extends RetrieveRequest {
    }
    interface RetrieveLocalizationResponse<TEntity> extends ServiceResponse {
        Entities?: {
            [key: string]: TEntity;
        };
    }

    namespace ErrorHandling {
        function showServiceError(error: ServiceError): void;
        function runtimeErrorHandler(message: string, filename?: string, lineno?: number, colno?: number, error?: Error): void;
    }

    interface NumberFormat {
        decimalSeparator: string;
        groupSeparator?: string;
        decimalDigits?: number;
        positiveSign?: string;
        negativeSign?: string;
        nanSymbol?: string;
        percentSymbol?: string;
        currencySymbol?: string;
    }
    interface DateFormat {
        dateSeparator?: string;
        dateFormat?: string;
        dateOrder?: string;
        dateTimeFormat?: string;
        amDesignator?: string;
        pmDesignator?: string;
        timeSeparator?: string;
        firstDayOfWeek?: number;
        dayNames?: string[];
        shortDayNames?: string[];
        minimizedDayNames?: string[];
        monthNames?: string[];
        shortMonthNames?: string[];
    }
    interface Locale extends NumberFormat, DateFormat {
        stringCompare?: (a: string, b: string) => number;
        toUpper?: (a: string) => string;
    }
    let Invariant: Locale;
    function compareStringFactory(order: string): ((a: string, b: string) => number);
    let Culture: Locale;
    function turkishLocaleToUpper(a: string): string;
    let turkishLocaleCompare: (a: string, b: string) => number;
    function format(format: string, ...prm: any[]): string;
    function localeFormat(format: string, l: Locale, ...prm: any[]): string;
    let round: (n: number, d?: number, rounding?: boolean) => number;
    let trunc: (n: number) => number;
    function formatNumber(num: number, format?: string, decOrLoc?: string | NumberFormat, grp?: string): string;
    function parseInteger(s: string): number;
    function parseDecimal(s: string): number;
    function toId(id: any): any;
    function formatDate(d: Date | string, format?: string, locale?: Locale): string;
    function formatDayHourAndMin(n: number): string;
    function formatISODateTimeUTC(d: Date): string;
    function parseISODateTime(s: string): Date;
    function parseHourAndMin(value: string): number;
    function parseDayHourAndMin(s: string): number;
    function parseDate(s: string, dateOrder?: string): Date;
    function splitDateString(s: string): string[];

    /**
     * Adds an empty option to the select.
     * @param select the select element
     */
    function addEmptyOption(select: JQuery | HTMLSelectElement): void;
    /**
     * Adds an option to the select.
     */
    function addOption(select: JQuery | HTMLSelectElement, key: string, text: string): void;
    /** @obsolete use htmlEncode as it also encodes quotes */
    const attrEncode: typeof htmlEncode;
    /** Clears the options in the select element */
    function clearOptions(select: JQuery): void;
    /**
     * Finds the first element with the given relative id to the source element.
     * It can handle underscores in the source element id.
     * @param element the source element
     * @param relativeId the relative id to the source element
     * @param context the context element (optional)
     * @returns the element with the given relative id to the source element.
     */
    function findElementWithRelativeId(element: JQuery, relativeId: string, context?: HTMLElement): JQuery;
    /**
     * Finds the first element with the given relative id to the source element.
     * It can handle underscores in the source element id.
     * @param element the source element
     * @param relativeId the relative id to the source element
     * @param context the context element (optional)
     * @returns the element with the given relative id to the source element.
     */
    function findElementWithRelativeId(element: HTMLElement, relativeId: string, context?: HTMLElement): HTMLElement;
    /**
     * Html encodes a string (encodes single and double quotes, & (ampersand), > and < characters)
     * @param s String (or number etc.) to be HTML encoded
     */
    function htmlEncode(s: any): string;
    /**
     * Creates a new DIV and appends it to the body.
     * @returns the new DIV element.
     */
    function newBodyDiv(): JQuery;
    /**
     * Returns the outer HTML of the element.
     */
    function outerHtml(element: JQuery): string;
    /**
     * Toggles the class on the element handling spaces like jQuery addClass does.
     * @param el the element
     * @param cls the class to toggle
     * @param remove if true, the class will be added, if false the class will be removed, otherwise it will be toggled.
     */
    function toggleClass(el: Element, cls: string, remove?: boolean): void;

    function autoFullHeight(element: JQuery): void;
    function initFullHeightGridPage(gridDiv: JQuery, opt?: {
        noRoute?: boolean;
    }): void;
    function layoutFillHeightValue(element: JQuery): number;
    function layoutFillHeight(element: JQuery): void;
    function setMobileDeviceMode(): void;
    function triggerLayoutOnShow(element: JQuery): void;
    function centerDialog(el: JQuery): void;

    namespace LayoutTimer {
        function store(key: number): void;
        function trigger(key: number): void;
        function onSizeChange(element: () => HTMLElement, handler: () => void, width?: boolean, height?: boolean): number;
        function onWidthChange(element: () => HTMLElement, handler: () => void): number;
        function onHeightChange(element: () => HTMLElement, handler: () => void): number;
        function onShown(element: () => HTMLElement, handler: () => void): number;
        function off(key: number): number;
    }
    function executeOnceWhenVisible(element: JQuery, callback: Function): void;
    function executeEverytimeWhenVisible(element: JQuery, callback: Function, callNowIfVisible: boolean): void;

    function localText(key: string): string;
    /** @obsolete prefer localText for better discoverability */
    const text: typeof localText;
    function dbText(prefix: string): ((key: string) => string);
    function prefixedText(prefix: string): (text: string, key: string | ((p?: string) => string)) => string;
    function tryGetText(key: string): string;
    function dbTryText(prefix: string): ((key: string) => string);
    function proxyTexts(o: Record<string, any>, p: string, t: Record<string, any>): Object;
    class LT {
        private key;
        static empty: LT;
        constructor(key: string);
        static add(key: string, value: string): void;
        get(): string;
        toString(): string;
        static initializeTextClass: (type: any, prefix: string) => void;
        static getDefault: (key: string, defaultText: string) => string;
    }

    interface LookupOptions<TItem> {
        idField?: string;
        parentIdField?: string;
        textField?: string;
        textFormatter?(item: TItem): string;
    }
    interface Lookup<TItem> {
        items: TItem[];
        itemById: {
            [key: string]: TItem;
        };
        idField: string;
        parentIdField: string;
        textField: string;
        textFormatter: (item: TItem) => string;
    }
    class Lookup<TItem> {
        items: TItem[];
        itemById: {
            [key: string]: TItem;
        };
        idField: string;
        parentIdField: string;
        textField: string;
        textFormatter: (item: TItem) => string;
        constructor(options: LookupOptions<TItem>, items?: TItem[]);
        update?(value: TItem[]): void;
    }

    type ToastContainerOptions = {
        containerId?: string;
        positionClass?: string;
        target?: string;
    };
    type ToastrOptions = ToastContainerOptions & {
        tapToDismiss?: boolean;
        toastClass?: string;
        showDuration?: number;
        onShown?: () => void;
        hideDuration?: number;
        onHidden?: () => void;
        closeMethod?: boolean;
        closeDuration?: number | false;
        closeEasing?: boolean;
        closeOnHover?: boolean;
        extendedTimeOut?: number;
        iconClass?: string;
        positionClass?: string;
        timeOut?: number;
        titleClass?: string;
        messageClass?: string;
        escapeHtml?: boolean;
        target?: string;
        closeHtml?: string;
        closeClass?: string;
        newestOnTop?: boolean;
        preventDuplicates?: boolean;
        onclick?: (event: MouseEvent) => void;
        onCloseClick?: (event: Event) => void;
        closeButton?: boolean;
        rtl?: boolean;
    };
    type NotifyMap = {
        type: string;
        iconClass: string;
        title?: string;
        message?: string;
    };
    class Toastr {
        private listener;
        private toastId;
        private previousToast;
        options: ToastrOptions;
        constructor(options?: ToastrOptions);
        private createContainer;
        getContainer(options?: ToastContainerOptions, create?: boolean): HTMLElement;
        error(message?: string, title?: string, opt?: ToastrOptions): HTMLElement | null;
        warning(message?: string, title?: string, opt?: ToastrOptions): HTMLElement | null;
        success(message?: string, title?: string, opt?: ToastrOptions): HTMLElement | null;
        info(message?: string, title?: string, opt?: ToastrOptions): HTMLElement | null;
        subscribe(callback: (response: Toastr) => void): void;
        publish(args: Toastr): void;
        clear(toastElement?: HTMLElement | null, clearOptions?: {
            force?: boolean;
        }): void;
        remove(toastElement?: HTMLElement | null): void;
        removeToast(toastElement: HTMLElement, options?: ToastrOptions): void;
        private clearContainer;
        private clearToast;
        private notify;
    }

    let defaultNotifyOptions: ToastrOptions;
    function notifyWarning(message: string, title?: string, options?: ToastrOptions): void;
    function notifySuccess(message: string, title?: string, options?: ToastrOptions): void;
    function notifyInfo(message: string, title?: string, options?: ToastrOptions): void;
    function notifyError(message: string, title?: string, options?: ToastrOptions): void;
    function positionToastContainer(create: boolean, options?: ToastrOptions): void;

    interface PropertyItem {
        name?: string;
        title?: string;
        hint?: string;
        placeholder?: string;
        editorType?: string;
        editorParams?: any;
        category?: string;
        collapsible?: boolean;
        collapsed?: boolean;
        tab?: string;
        cssClass?: string;
        headerCssClass?: string;
        formCssClass?: string;
        maxLength?: number;
        required?: boolean;
        insertable?: boolean;
        insertPermission?: string;
        hideOnInsert?: boolean;
        updatable?: boolean;
        updatePermission?: string;
        hideOnUpdate?: boolean;
        readOnly?: boolean;
        readPermission?: string;
        oneWay?: boolean;
        defaultValue?: any;
        localizable?: boolean;
        visible?: boolean;
        allowHide?: boolean;
        formatterType?: string;
        formatterParams?: any;
        displayFormat?: string;
        alignment?: string;
        width?: number;
        widthSet?: boolean;
        minWidth?: number;
        maxWidth?: number;
        labelWidth?: string;
        resizable?: boolean;
        sortable?: boolean;
        sortOrder?: number;
        groupOrder?: number;
        summaryType?: SummaryType;
        editLink?: boolean;
        editLinkItemType?: string;
        editLinkIdField?: string;
        editLinkCssClass?: string;
        filteringType?: string;
        filteringParams?: any;
        filteringIdField?: string;
        notFilterable?: boolean;
        filterOnly?: boolean;
        quickFilter?: boolean;
        quickFilterParams?: any;
        quickFilterSeparator?: boolean;
        quickFilterCssClass?: string;
    }
    interface PropertyItemsData {
        items: PropertyItem[];
        additionalItems: PropertyItem[];
    }
    enum SummaryType {
        Disabled = -1,
        None = 0,
        Sum = 1,
        Avg = 2,
        Min = 3,
        Max = 4
    }

    interface HandleRouteEventArgs {
        handled: boolean;
        route: string;
        parts: string[];
        index: number;
    }
    namespace Router {
        let enabled: boolean;
        function navigate(hash: string, tryBack?: boolean, silent?: boolean): void;
        function replace(hash: string, tryBack?: boolean): void;
        function replaceLast(hash: string, tryBack?: boolean): void;
        function dialog(owner: JQuery, element: JQuery, hash: () => string): void;
        function resolve(hash?: string): void;
    }

    namespace ScriptData {
        function bindToChange(name: string, regClass: string, onChange: () => void): void;
        function triggerChange(name: string): void;
        function unbindFromChange(regClass: string): void;
        function ensure<TData = any>(name: string): TData;
        function ensureAsync<TData = any>(name: string): Promise<TData>;
        function reload<TData = any>(name: string): TData;
        function reloadAsync<TData = any>(name: string): Promise<TData>;
        function canLoad(name: string): boolean;
        function setRegisteredScripts(scripts: any[]): void;
        function set(name: string, value: any): void;
    }
    function getRemoteData<TData = any>(key: string): TData;
    function getRemoteDataAsync<TData = any>(key: string): Promise<TData>;
    function getLookup<TItem>(key: string): Lookup<TItem>;
    function getLookupAsync<TItem>(key: string): Promise<Lookup<TItem>>;
    function reloadLookup<TItem = any>(key: string): Lookup<TItem>;
    function reloadLookupAsync<TItem = any>(key: string): Promise<Lookup<TItem>>;
    function getColumns(key: string): PropertyItem[];
    function getColumnsData(key: string): PropertyItemsData;
    function getColumnsAsync(key: string): Promise<PropertyItem[]>;
    function getColumnsDataAsync(key: string): Promise<PropertyItemsData>;
    function getForm(key: string): PropertyItem[];
    function getFormData(key: string): PropertyItemsData;
    function getFormAsync(key: string): Promise<PropertyItem[]>;
    function getFormDataAsync(key: string): Promise<PropertyItemsData>;
    function getTemplate(key: string): string;
    function getTemplateAsync(key: string): Promise<string>;
    function canLoadScriptData(name: string): boolean;

    function getCookie(name: string): any;
    function serviceCall<TResponse extends ServiceResponse>(options: ServiceOptions<TResponse>): JQueryXHR;
    function serviceRequest<TResponse extends ServiceResponse>(service: string, request?: any, onSuccess?: (response: TResponse) => void, options?: ServiceOptions<TResponse>): JQueryXHR;
    function setEquality(request: ListRequest, field: string, value: any): void;
    interface PostToServiceOptions {
        url?: string;
        service?: string;
        target?: string;
        request: any;
    }
    interface PostToUrlOptions {
        url?: string;
        target?: string;
        params: any;
    }
    function parseQueryString(s?: string): {};
    function postToService(options: PostToServiceOptions): void;
    function postToUrl(options: PostToUrlOptions): void;
    function resolveUrl(url: string): string;

    /**
     * Checks if the string ends with the specified substring.
     * @param s String to check.
     * @param suffix Suffix to check.
     * @returns True if the string ends with the specified substring.
     */
    function endsWith(s: string, suffix: string): boolean;
    /**
     * Checks if the string is empty or null.
     * @param s String to check.
     * @returns True if the string is empty or null.
     */
    function isEmptyOrNull(s: string): boolean;
    /**
     * Checks if the string is empty or null or whitespace.
     * @param s String to check.
     * @returns True if the string is empty or null or whitespace.
     */
    function isTrimmedEmpty(s: string): boolean;
    /**
     * Pads the string to the left with the specified character.
     * @param s String to pad.
     * @param len Target length of the string.
     * @param ch Character to pad with.
     * @returns Padded string.
     */
    function padLeft(s: string | number, len: number, ch?: string): any;
    /**
     * Checks if the string starts with the prefix
     * @param s String to check.
     * @param prefix Prefix to check.
     * @returns True if the string starts with the prefix.
     */
    function startsWith(s: string, prefix: string): boolean;
    /**
     * Converts the string to single line by removing line end characters
     * @param str String to convert.
     */
    function toSingleLine(str: string): string;
    /**
     * Trims the whitespace characters from the end of the string
     */
    var trimEnd: (s: string) => any;
    /**
     * Trims the whitespace characters from the start of the string
     */
    var trimStart: (s: string) => any;
    /**
     * Trims the whitespace characters from the start and end of the string
     * This returns empty string even when the string is null or undefined.
     */
    function trim(s: string): string;
    /**
     * Trims the whitespace characters from the start and end of the string
     * Returns empty string if the string is null or undefined.
     */
    function trimToEmpty(s: string): string;
    /**
     * Trims the whitespace characters from the start and end of the string
     * Returns null if the string is null, undefined or whitespace.
     */
    function trimToNull(s: string): string;
    /**
     * Replaces all occurrences of the search string with the replacement string.
     * @param str String to replace.
     * @param find String to find.
     * @param replace String to replace with.
     * @returns Replaced string.
     */
    function replaceAll(str: string, find: string, replace: string): string;
    /**
     * Pads the start of string to make it the specified length.
     * @param s String to pad.
     * @param len Target length of the string.
     */
    function zeroPad(n: number, len: number): string;

    type Dictionary<TItem> = {
        [key: string]: TItem;
    };
    function coalesce(a: any, b: any): any;
    function isValue(a: any): boolean;
    let today: () => Date;
    function extend<T = any>(a: T, b: T): T;
    function deepClone<T = any>(a: T, a2?: any, a3?: any): T;
    type Type = Function | Object;
    interface TypeMember {
        name: string;
        type: MemberType;
        attr?: any[];
        getter?: string;
        setter?: string;
    }
    function getNested(from: any, name: string): any;
    function getGlobalThis(): any;
    function getType(name: string, target?: any): Type;
    function getTypeNameProp(type: Type): string;
    function setTypeNameProp(type: Type, value: string): void;
    function getTypeFullName(type: Type): string;
    function getTypeShortName(type: Type): string;
    function getInstanceType(instance: any): any;
    function isAssignableFrom(target: any, type: Type): boolean;
    function isInstanceOfType(instance: any, type: Type): boolean;
    function safeCast(instance: any, type: Type): any;
    function cast(instance: any, type: Type): any;
    function getBaseType(type: any): any;
    function getAttributes(type: any, attrType: any, inherit?: boolean): any[];
    enum MemberType {
        field = 4,
        property = 16
    }
    function getMembers(type: any, memberTypes: MemberType): TypeMember[];
    function addTypeMember(type: any, member: TypeMember): TypeMember;
    function getTypes(from?: any): any[];
    function clearKeys(d: any): void;
    function delegateCombine(delegate1: any, delegate2: any): any;
    function getStateStore(key?: string): any;
    namespace Enum {
        let toString: (enumType: any, value: number) => string;
        let getValues: (enumType: any) => any[];
    }
    let delegateRemove: (delegate1: any, delegate2: any) => any;
    let isEnum: (type: any) => boolean;
    function initFormType(typ: Function, nameWidgetPairs: any[]): void;
    function prop(type: any, name: string, getter?: string, setter?: string): void;
    function fieldsProxy<TRow>(): Readonly<Record<keyof TRow, string>>;
    function keyOf<T>(prop: keyof T): keyof T;
    function registerClass(type: any, name: string, intf?: any[]): void;
    function registerEditor(type: any, name: string, intf?: any[]): void;
    function registerEnum(type: any, name: string, enumKey?: string): void;
    function registerInterface(type: any, name: string, intf?: any[]): void;
    function addAttribute(type: any, attr: any): void;
    class ISlickFormatter {
    }
    class EditorAttribute {
    }
    function initializeTypes(root: any, pre: string, limit: number): void;
    class Exception extends Error {
        constructor(message: string);
    }
    class ArgumentNullException extends Exception {
        constructor(paramName: string, message?: string);
    }
    class InvalidCastException extends Exception {
        constructor(message: string);
    }

    function validatorAbortHandler(validator: any): void;
    function validateOptions(options?: JQueryValidation.ValidationOptions): JQueryValidation.ValidationOptions;

    function loadValidationErrorMessages(): void;
    function getHighlightTarget(el: HTMLElement): HTMLElement;
    function baseValidateOptions(): JQueryValidation.ValidationOptions;
    function validateForm(form: JQuery, opt: JQueryValidation.ValidationOptions): JQueryValidation.Validator;
    function addValidationRule(element: JQuery, eventClass: string, rule: (p1: JQuery) => string): JQuery;
    function removeValidationRule(element: JQuery, eventClass: string): JQuery;

    /**
     * CriteriaBuilder is a class that allows to build unary or binary criteria with completion support.
     */
    class CriteriaBuilder extends Array {
        /**
         * Creates a between criteria.
         * @param fromInclusive from value
         * @param toInclusive to value
         */
        bw(fromInclusive: any, toInclusive: any): Array<any>;
        /**
         * Creates a contains criteria
         * @param value contains value
         */
        contains(value: string): Array<any>;
        /**
         * Creates a endsWith criteria
         * @param value endsWith value
         */
        endsWith(value: string): Array<any>;
        /**
         * Creates an equal (=) criteria
         * @param value equal value
         */
        eq(value: any): Array<any>;
        /**
         * Creates a greater than criteria
         * @param value greater than value
         */
        gt(value: any): Array<any>;
        /**
         * Creates a greater than or equal criteria
         * @param value greater than or equal value
         */
        ge(value: any): Array<any>;
        /**
         * Creates a in criteria
         * @param values in values
         */
        in(values: any[]): Array<any>;
        /**
         * Creates a IS NULL criteria
         */
        isNull(): Array<any>;
        /**
         * Creates a IS NOT NULL criteria
         */
        isNotNull(): Array<any>;
        /**
         * Creates a less than or equal to criteria
         * @param value less than or equal to value
         */
        le(value: any): Array<any>;
        /**
         * Creates a less than criteria
         * @param value less than value
         */
        lt(value: any): Array<any>;
        /**
         * Creates a not equal criteria
         * @param value not equal value
         */
        ne(value: any): Array<any>;
        /**
         * Creates a LIKE criteria
         * @param value like value
         */
        like(value: any): Array<any>;
        /**
         * Creates a STARTS WITH criteria
         * @param value startsWith value
         */
        startsWith(value: string): Array<any>;
        /**
         * Creates a NOT IN criteria
         * @param values array of NOT IN values
         */
        notIn(values: any[]): Array<any>;
        /**
         * Creates a NOT LIKE criteria
         * @param value not like value
         */
        notLike(value: any): Array<any>;
    }
    /**
     * Parses a criteria expression to Serenity Criteria array format.
     * The string may optionally contain parameters like `A >= @p1 and B < @p2`.
     * @param expression The criteria expression.
     * @param params The dictionary containing parameter values like { p1: 10, p2: 20 }.
     * @example
     * parseCriteria('A >= @p1 and B < @p2', { p1: 5, p2: 4 }) // [[[a], '>=' 5], 'and', [[b], '<', 4]]
     */
    function parseCriteria(expression: string, params?: any): any[];
    /**
     * Parses a criteria expression to Serenity Criteria array format.
     * The expression may contain parameter placeholders like `A >= ${p1}`
     * where p1 is a variable in the scope.
     * @param strings The string fragments.
     * @param values The tagged template arguments.
     * @example
     * var a = 5, b = 4;
     * parseCriteria`A >= ${a} and B < ${b}` // [[[a], '>=' 5], 'and', [[b], '<', 4]]
     */
    function parseCriteria(strings: TemplateStringsArray, ...values: any[]): any[];
    enum CriteriaOperator {
        paren = "()",
        not = "not",
        isNull = "is null",
        isNotNull = "is not null",
        exists = "exists",
        and = "and",
        or = "or",
        xor = "xor",
        eq = "=",
        ne = "!=",
        gt = ">",
        ge = ">=",
        lt = "<",
        le = "<=",
        in = "in",
        notIn = "not in",
        like = "like",
        notLike = "not like"
    }
    /**
     * Creates a new criteria builder containg the passed field name.
     * @param field The field name.
     */
    function Criteria(field: string): CriteriaBuilder;
    namespace Criteria {
        var and: (c1: any[], c2: any[], ...rest: any[][]) => any[];
        var Operator: typeof CriteriaOperator;
        var isEmpty: (c: any[]) => boolean;
        var join: (c1: any[], op: string, c2: any[]) => any[];
        var not: (c: any[]) => (string | any[])[];
        var or: (c1: any[], c2: any[], ...rest: any[][]) => any[];
        var paren: (c: any[]) => any[];
        var parse: typeof parseCriteria;
    }
}


declare namespace Slick {
    /***
     * A base class that all special / non-data rows (like Group and GroupTotals) derive from.
     */
    class NonDataRow {
    	__nonDataRow: boolean;
    }
    const preClickClassName = "slick-edit-preclick";
    interface FormatterContext<TItem = any> {
    	addAttrs?: {
    		[key: string]: string;
    	};
    	addClass?: string;
    	cell?: number;
    	column?: Column<TItem>;
    	/** returns html escaped ctx.value if called without arguments. prefer this over ctx.value to avoid html injection attacks! */
    	readonly escape: ((value?: any) => string);
    	grid?: any;
    	item?: TItem;
    	row?: number;
    	tooltip?: string;
    	/** when returning a formatter result, prefer ctx.escape() to avoid html injection attacks! */
    	value?: any;
    }
    export type ColumnFormat<TItem = any> = (ctx: FormatterContext<TItem>) => string;
    interface CompatFormatterResult {
    	addClasses?: string;
    	text?: string;
    	toolTip?: string;
    }
    export type CompatFormatter<TItem = any> = (row: number, cell: number, value: any, column: Column<TItem>, item: TItem, grid?: any) => string | CompatFormatterResult;
    interface FormatterFactory<TItem = any> {
    	getFormat?(column: Column<TItem>): ColumnFormat<TItem>;
    	getFormatter?(column: Column<TItem>): CompatFormatter<TItem>;
    }
    export type AsyncPostRender<TItem = any> = (cellNode: HTMLElement, row: number, item: TItem, column: Column<TItem>, reRender: boolean) => void;
    export type AsyncPostCleanup<TItem = any> = (cellNode: HTMLElement, row?: number, column?: Column<TItem>) => void;
    export type CellStylesHash = {
    	[row: number]: {
    		[columnId: string]: string;
    	};
    };
    function defaultColumnFormat(ctx: FormatterContext): any;
    function convertCompatFormatter(compatFormatter: CompatFormatter): ColumnFormat;
    function applyFormatterResultToCellNode(ctx: FormatterContext, html: string, node: HTMLElement): void;
    /***
     * Information about a group of rows.
     */
    class Group<TEntity = any> extends NonDataRow {
    	readonly __group = true;
    	/**
    	 * Grouping level, starting with 0.
    	 * @property level
    	 * @type {Number}
    	 */
    	level: number;
    	/***
    	 * Number of rows in the group.
    	 * @property count
    	 * @type {Number}
    	 */
    	count: number;
    	/***
    	 * Grouping value.
    	 * @property value
    	 * @type {Object}
    	 */
    	value: any;
    	/***
    	 * Formatted display value of the group.
    	 * @property title
    	 * @type {String}
    	 */
    	title: string;
    	/***
    	 * Whether a group is collapsed.
    	 * @property collapsed
    	 * @type {Boolean}
    	 */
    	collapsed: boolean;
    	/***
    	 * GroupTotals, if any.
    	 * @property totals
    	 * @type {GroupTotals}
    	 */
    	totals: GroupTotals<TEntity>;
    	/**
    	 * Rows that are part of the group.
    	 * @property rows
    	 * @type {Array}
    	 */
    	rows: TEntity[];
    	/**
    	 * Sub-groups that are part of the group.
    	 * @property groups
    	 * @type {Array}
    	 */
    	groups: Group<TEntity>[];
    	/**
    	 * A unique key used to identify the group.  This key can be used in calls to DataView
    	 * collapseGroup() or expandGroup().
    	 * @property groupingKey
    	 * @type {Object}
    	 */
    	groupingKey: string;
    	/***
    	 * Compares two Group instances.
    	 * @method equals
    	 * @return {Boolean}
    	 * @param group {Group} Group instance to compare to.
    	 */
    	equals(group: Group): boolean;
    }
    /***
     * Information about group totals.
     * An instance of GroupTotals will be created for each totals row and passed to the aggregators
     * so that they can store arbitrary data in it.  That data can later be accessed by group totals
     * formatters during the display.
     * @class GroupTotals
     * @extends NonDataRow
     * @constructor
     */
    class GroupTotals<TEntity = any> extends NonDataRow {
    	readonly __groupTotals = true;
    	/***
    	 * Parent Group.
    	 * @param group
    	 * @type {Group}
    	 */
    	group: Group<TEntity>;
    	/***
    	 * Whether the totals have been fully initialized / calculated.
    	 * Will be set to false for lazy-calculated group totals.
    	 * @param initialized
    	 * @type {Boolean}
    	 */
    	initialized: boolean;
    	/**
    	 * Contains sum
    	 */
    	sum?: number;
    	/**
    	 * Contains avg
    	 */
    	avg?: number;
    	/**
    	 * Contains min
    	 */
    	min?: any;
    	/**
    	 * Contains max
    	 */
    	max?: any;
    }
    export type EventListener<TArgs, TEventData extends IEventData = IEventData> = (e: TEventData, args: TArgs) => void;
    interface IEventData {
    	readonly type?: string;
    	currentTarget?: EventTarget | null;
    	target?: EventTarget | null;
    	originalEvent?: any;
    	defaultPrevented?: boolean;
    	preventDefault?(): void;
    	stopPropagation?(): void;
    	stopImmediatePropagation?(): void;
    	isDefaultPrevented?(): boolean;
    	isImmediatePropagationStopped?(): boolean;
    	isPropagationStopped?(): boolean;
    }
    /***
     * An event object for passing data to event handlers and letting them control propagation.
     * <p>This is pretty much identical to how W3C and jQuery implement events.</p>
     */
    class EventData implements IEventData {
    	private _isPropagationStopped;
    	private _isImmediatePropagationStopped;
    	/***
    	 * Stops event from propagating up the DOM tree.
    	 * @method stopPropagation
    	 */
    	stopPropagation(): void;
    	/***
    	 * Returns whether stopPropagation was called on this event object.
    	 */
    	isPropagationStopped(): boolean;
    	/***
    	 * Prevents the rest of the handlers from being executed.
    	 */
    	stopImmediatePropagation(): void;
    	/***
    	 * Returns whether stopImmediatePropagation was called on this event object.\
    	 */
    	isImmediatePropagationStopped(): boolean;
    }
    /***
     * A simple publisher-subscriber implementation.
     */
    class EventEmitter<TArgs = any, TEventData extends IEventData = IEventData> {
    	private _handlers;
    	/***
    	 * Adds an event handler to be called when the event is fired.
    	 * <p>Slick.Event handler will receive two arguments - an <code>EventData</code> and the <code>data</code>
    	 * object the event was fired with.<p>
    	 * @method subscribe
    	 * @param fn {Function} Event handler.
    	 */
    	subscribe(fn: EventListener<TArgs, TEventData>): void;
    	/***
    	 * Removes an event handler added with <code>subscribe(fn)</code>.
    	 * @method unsubscribe
    	 * @param fn {Function} Event handler to be removed.
    	 */
    	unsubscribe(fn: EventListener<TArgs, TEventData>): void;
    	/***
    	 * Fires an event notifying all subscribers.
    	 * @param args {Object} Additional data object to be passed to all handlers.
    	 * @param e {EventData}
    	 *      Optional.
    	 *      An <code>EventData</code> object to be passed to all handlers.
    	 *      For DOM events, an existing W3C/jQuery event object can be passed in.
    	 * @param scope {Object}
    	 *      Optional.
    	 *      The scope ("this") within which the handler will be executed.
    	 *      If not specified, the scope will be set to the <code>Slick.Event</code> instance.
    	 */
    	notify(args?: any, e?: TEventData, scope?: object): any;
    	clear(): void;
    }
    class EventSubscriber<TArgs = any, TEventData extends IEventData = IEventData> {
    	private _handlers;
    	subscribe(event: EventEmitter<TArgs, TEventData>, handler: EventListener<TArgs, TEventData>): this;
    	unsubscribe(event: EventEmitter<TArgs, TEventData>, handler: EventListener<TArgs, TEventData>): this;
    	unsubscribeAll(): EventSubscriber<TArgs, TEventData>;
    }
    /** @deprecated */
    const keyCode: {
    	BACKSPACE: number;
    	DELETE: number;
    	DOWN: number;
    	END: number;
    	ENTER: number;
    	ESCAPE: number;
    	HOME: number;
    	INSERT: number;
    	LEFT: number;
    	PAGEDOWN: number;
    	PAGEUP: number;
    	RIGHT: number;
    	TAB: number;
    	UP: number;
    };
    function patchEvent(e: IEventData): IEventData;
    interface Position {
    	bottom?: number;
    	height?: number;
    	left?: number;
    	right?: number;
    	top?: number;
    	visible?: boolean;
    	width?: number;
    }
    interface ValidationResult {
    	valid: boolean;
    	msg?: string;
    }
    interface RowCell {
    	row: number;
    	cell: number;
    }
    interface EditorHost {
    	getActiveCell(): RowCell;
    	navigateNext(): boolean;
    	navigatePrev(): boolean;
    	onCompositeEditorChange: EventEmitter<any>;
    }
    interface CompositeEditorOptions {
    	formValues: any;
    }
    interface EditorOptions {
    	grid: EditorHost;
    	gridPosition?: Position;
    	position?: Position;
    	editorCellNavOnLRKeys?: boolean;
    	column?: Column;
    	columnMetaData?: ColumnMetadata<any>;
    	compositeEditorOptions?: CompositeEditorOptions;
    	container?: HTMLElement;
    	item?: any;
    	event?: IEventData;
    	commitChanges?: () => void;
    	cancelChanges?: () => void;
    }
    interface EditorFactory {
    	getEditor(column: Column, row?: number): EditorClass;
    }
    interface EditCommand {
    	row: number;
    	cell: number;
    	editor: Editor;
    	serializedValue: any;
    	prevSerializedValue: any;
    	execute: () => void;
    	undo: () => void;
    }
    interface EditorClass {
    	new (options: EditorOptions): Editor;
    	suppressClearOnEdit?: boolean;
    }
    interface Editor {
    	destroy(): void;
    	applyValue(item: any, value: any): void;
    	focus(): void;
    	isValueChanged(): boolean;
    	keyCaptureList?: number[];
    	loadValue(value: any): void;
    	serializeValue(): any;
    	position?(pos: Position): void;
    	preClick?(): void;
    	hide?(): void;
    	show?(): void;
    	validate?(): ValidationResult;
    }
    interface EditController {
    	commitCurrentEdit(): boolean;
    	cancelCurrentEdit(): boolean;
    }
    /***
     * A locking helper to track the active edit controller and ensure that only a single controller
     * can be active at a time.  This prevents a whole class of state and validation synchronization
     * issues.  An edit controller (such as SleekGrid) can query if an active edit is in progress
     * and attempt a commit or cancel before proceeding.
     * @class EditorLock
     * @constructor
     */
    class EditorLock {
    	private activeEditController;
    	/***
    	 * Returns true if a specified edit controller is active (has the edit lock).
    	 * If the parameter is not specified, returns true if any edit controller is active.
    	 * @method isActive
    	 * @param editController {EditController}
    	 * @return {Boolean}
    	 */
    	isActive(editController?: EditController): boolean;
    	/***
    	 * Sets the specified edit controller as the active edit controller (acquire edit lock).
    	 * If another edit controller is already active, and exception will be thrown.
    	 * @method activate
    	 * @param editController {EditController} edit controller acquiring the lock
    	 */
    	activate(editController: EditController): void;
    	/***
    	 * Unsets the specified edit controller as the active edit controller (release edit lock).
    	 * If the specified edit controller is not the active one, an exception will be thrown.
    	 * @method deactivate
    	 * @param editController {EditController} edit controller releasing the lock
    	 */
    	deactivate(editController: EditController): void;
    	/***
    	 * Attempts to commit the current edit by calling "commitCurrentEdit" method on the active edit
    	 * controller and returns whether the commit attempt was successful (commit may fail due to validation
    	 * errors, etc.).  Edit controller's "commitCurrentEdit" must return true if the commit has succeeded
    	 * and false otherwise.  If no edit controller is active, returns true.
    	 * @method commitCurrentEdit
    	 * @return {Boolean}
    	 */
    	commitCurrentEdit(): boolean;
    	/***
    	 * Attempts to cancel the current edit by calling "cancelCurrentEdit" method on the active edit
    	 * controller and returns whether the edit was successfully cancelled.  If no edit controller is
    	 * active, returns true.
    	 * @method cancelCurrentEdit
    	 * @return {Boolean}
    	 */
    	cancelCurrentEdit(): boolean;
    }
    /***
     * A global singleton editor lock.
     * @class GlobalEditorLock
     * @static
     * @constructor
     */
    const GlobalEditorLock: EditorLock;
    interface Column<TItem = any> {
    	asyncPostRender?: AsyncPostRender<TItem>;
    	asyncPostRenderCleanup?: AsyncPostCleanup<TItem>;
    	behavior?: any;
    	cannotTriggerInsert?: boolean;
    	cssClass?: string;
    	defaultSortAsc?: boolean;
    	editor?: EditorClass;
    	editorFixedDecimalPlaces?: number;
    	field?: string;
    	frozen?: boolean;
    	focusable?: boolean;
    	footerCssClass?: string;
    	format?: ColumnFormat<TItem>;
    	/** @deprecated */
    	formatter?: CompatFormatter<TItem>;
    	groupTotalsFormatter?: (p1?: GroupTotals<TItem>, p2?: Column<TItem>, grid?: unknown) => string;
    	headerCssClass?: string;
    	id?: string;
    	maxWidth?: any;
    	minWidth?: number;
    	name?: string;
    	nameIsHtml?: boolean;
    	previousWidth?: number;
    	referencedFields?: string[];
    	rerenderOnResize?: boolean;
    	resizable?: boolean;
    	selectable?: boolean;
    	sortable?: boolean;
    	sortOrder?: number;
    	toolTip?: string;
    	validator?: (value: any, editorArgs?: any) => ValidationResult;
    	visible?: boolean;
    	width?: number;
    }
    const columnDefaults: Partial<Column>;
    interface ColumnMetadata<TItem = any> {
    	colspan: number | "*";
    	cssClasses?: string;
    	editor?: EditorClass;
    	format?: ColumnFormat<TItem>;
    	/** @deprecated */
    	formatter?: CompatFormatter<TItem>;
    }
    interface ColumnSort {
    	columnId: string;
    	sortAsc?: boolean;
    }
    interface ItemMetadata<TItem = any> {
    	cssClasses?: string;
    	columns?: {
    		[key: string]: ColumnMetadata<TItem>;
    	};
    	focusable?: boolean;
    	format?: ColumnFormat<TItem>;
    	/** @deprecated */
    	formatter?: CompatFormatter<TItem>;
    	selectable?: boolean;
    }
    function initializeColumns(columns: Column[], defaults: Partial<Column<any>>): void;
    function titleize(str: string): string;
    class Range {
    	fromRow: number;
    	fromCell: number;
    	toRow: number;
    	toCell: number;
    	constructor(fromRow: number, fromCell: number, toRow?: number, toCell?: number);
    	/***
    	 * Returns whether a range represents a single row.
    	 */
    	isSingleRow(): boolean;
    	/***
    	 * Returns whether a range represents a single cell.
    	 */
    	isSingleCell(): boolean;
    	/***
    	 * Returns whether a range contains a given cell.
    	 */
    	contains(row: number, cell: number): boolean;
    	/***
    	 * Returns a readable representation of a range.
    	 */
    	toString(): string;
    }
    function addClass(el: Element, cls: string): void;
    function escape(s: any): any;
    function disableSelection(target: HTMLElement): void;
    function removeClass(el: Element, cls: string): void;
    function H<K extends keyof HTMLElementTagNameMap>(tag: K, attr?: {
    	ref?: (el?: HTMLElementTagNameMap[K]) => void;
    	[key: string]: string | number | boolean | ((el?: HTMLElementTagNameMap[K]) => void) | null | undefined;
    }, ...children: (string | Node)[]): HTMLElementTagNameMap[K];
    function spacerDiv(width: string): HTMLDivElement;
    function parsePx(str: string): number;
    interface IPlugin {
    	init(grid: Grid): void;
    	pluginName?: string;
    	destroy?: () => void;
    }
    interface ViewportInfo {
    	height: number;
    	width: number;
    	hasVScroll: boolean;
    	hasHScroll: boolean;
    	headerHeight: number;
    	groupingPanelHeight: number;
    	virtualHeight: number;
    	realScrollHeight: number;
    	topPanelHeight: number;
    	headerRowHeight: number;
    	footerRowHeight: number;
    	numVisibleRows: number;
    }
    interface SelectionModel extends IPlugin {
    	setSelectedRanges(ranges: Range[]): void;
    	onSelectedRangesChanged: EventEmitter<Range[]>;
    	refreshSelections?(): void;
    }
    interface ViewRange {
    	top?: number;
    	bottom?: number;
    	leftPx?: number;
    	rightPx?: number;
    }
    interface LayoutHost {
    	bindAncestorScroll(el: HTMLElement): void;
    	cleanUpAndRenderCells(range: ViewRange): void;
    	getAvailableWidth(): number;
    	getCellFromPoint(x: number, y: number): RowCell;
    	getColumnCssRules(idx: number): {
    		right: any;
    		left: any;
    	};
    	getColumns(): Column[];
    	getContainerNode(): HTMLElement;
    	getDataLength(): number;
    	getOptions(): GridOptions;
    	getRowFromNode(rowNode: HTMLElement): number;
    	getScrollDims(): {
    		width: number;
    		height: number;
    	};
    	getScrollLeft(): number;
    	getScrollTop(): number;
    	getViewportInfo(): ViewportInfo;
    	renderRows(range: ViewRange): void;
    }
    interface LayoutEngine {
    	appendCachedRow(row: number, rowNodeL: HTMLElement, rowNodeR: HTMLElement): void;
    	afterHeaderColumnDrag(): void;
    	afterSetOptions(args: GridOptions): void;
    	applyColumnWidths(): void;
    	beforeCleanupAndRenderCells(rendered: ViewRange): void;
    	afterRenderRows(rendered: ViewRange): void;
    	bindAncestorScrollEvents(): void;
    	calcCanvasWidth(): number;
    	updateHeadersWidth(): void;
    	isFrozenRow(row: number): boolean;
    	destroy(): void;
    	getCanvasNodeFor(cell: number, row: number): HTMLElement;
    	getCanvasNodes(): HTMLElement[];
    	getCanvasWidth(): number;
    	getRowFromCellNode(cellNode: HTMLElement, clientX: number, clientY: number): number;
    	getFooterRowCols(): HTMLElement[];
    	getFooterRowColsFor(cell: number): HTMLElement;
    	getFooterRowColumn(cell: number): HTMLElement;
    	getFrozenCols(): number;
    	getFrozenRowOffset(row: number): number;
    	getFrozenRows(): number;
    	getHeaderCols(): HTMLElement[];
    	getHeaderColsFor(cell: number): HTMLElement;
    	getHeaderColumn(cell: number): HTMLElement;
    	getHeaderRowCols(): HTMLElement[];
    	getHeaderRowColsFor(cell: number): HTMLElement;
    	getHeaderRowColumn(cell: number): HTMLElement;
    	getScrollCanvasY(): HTMLElement;
    	getScrollContainerX(): HTMLElement;
    	getScrollContainerY(): HTMLElement;
    	getTopPanelFor(arg0: number): HTMLElement;
    	getTopPanelNodes(): HTMLElement[];
    	getViewportNodeFor(cell: number, row: number): HTMLElement;
    	getViewportNodes(): HTMLElement[];
    	handleScrollH(): void;
    	handleScrollV(): void;
    	init(host: LayoutHost): void;
    	layoutName: string;
    	realScrollHeightChange(): void;
    	/** this might be called before init, chicken egg situation */
    	reorderViewColumns(viewCols: Column[], options?: GridOptions): Column[];
    	resizeCanvas(): void;
    	setPaneVisibility(): void;
    	setScroller(): void;
    	setOverflow(): void;
    	updateCanvasWidth(): boolean;
    }
    interface GridOptions<TItem = any> {
    	addNewRowCssClass?: string;
    	alwaysAllowHorizontalScroll?: boolean;
    	alwaysShowVerticalScroll?: boolean;
    	asyncEditorLoadDelay?: number;
    	asyncEditorLoading?: boolean;
    	asyncPostCleanupDelay?: number;
    	asyncPostRenderDelay?: number;
    	autoEdit?: boolean;
    	autoHeight?: boolean;
    	cellFlashingCssClass?: string;
    	cellHighlightCssClass?: string;
    	columns?: Column<TItem>[];
    	createPreHeaderPanel?: boolean;
    	dataItemColumnValueExtractor?: (item: TItem, column: Column<TItem>) => void;
    	defaultColumnWidth?: number;
    	defaultFormat?: ColumnFormat<TItem>;
    	defaultFormatter?: CompatFormatter<TItem>;
    	editable?: boolean;
    	editCommandHandler?: (item: TItem, column: Column<TItem>, command: EditCommand) => void;
    	editorCellNavOnLRKeys?: boolean;
    	editorFactory?: EditorFactory;
    	editorLock?: EditorLock;
    	enableAddRow?: boolean;
    	enableAsyncPostRender?: boolean;
    	enableAsyncPostRenderCleanup?: boolean;
    	enableCellNavigation?: boolean;
    	enableCellRangeSelection?: boolean;
    	enableColumnReorder?: boolean;
    	enableRowReordering?: boolean;
    	enableTabKeyNavigation?: boolean;
    	enableTextSelectionOnCells?: boolean;
    	explicitInitialization?: boolean;
    	footerRowHeight?: number;
    	forceFitColumns?: boolean;
    	forceSyncScrolling?: boolean;
    	forceSyncScrollInterval?: number;
    	formatterFactory?: FormatterFactory;
    	frozenBottom?: boolean;
    	frozenColumns?: number;
    	frozenRows?: number;
    	fullWidthRows?: boolean;
    	groupingPanel?: boolean;
    	groupingPanelHeight?: number;
    	groupTotalsFormatter?: (p1?: GroupTotals<TItem>, p2?: Column<TItem>, grid?: any) => string;
    	headerRowHeight?: number;
    	jQuery?: {
    		ready: any;
    		fn: any;
    	};
    	leaveSpaceForNewRows?: boolean;
    	layoutEngine?: LayoutEngine;
    	minBuffer?: number;
    	multiColumnSort?: boolean;
    	multiSelect?: boolean;
    	preHeaderPanelHeight?: number;
    	renderAllCells?: boolean;
    	rowHeight?: number;
    	rtl?: boolean;
    	selectedCellCssClass?: string;
    	showCellSelection?: boolean;
    	showColumnHeader?: boolean;
    	showFooterRow?: boolean;
    	showGroupingPanel?: boolean;
    	showHeaderRow?: boolean;
    	showPreHeaderPanel?: boolean;
    	showTopPanel?: boolean;
    	slickCompat?: boolean;
    	suppressActiveCellChangeOnEdit?: boolean;
    	syncColumnCellResize?: boolean;
    	topPanelHeight?: number;
    	useLegacyUI?: boolean;
    	useCssVars?: boolean;
    	viewportClass?: string;
    }
    const gridDefaults: GridOptions;
    class Grid<TItem = any> implements EditorHost {
    	private _absoluteColMinWidth;
    	private _activeCanvasNode;
    	private _activeCell;
    	private _activeCellNode;
    	private _activePosX;
    	private _activeRow;
    	private _activeViewportNode;
    	private _cellCssClasses;
    	private _cellHeightDiff;
    	private _cellWidthDiff;
    	private _cellNavigator;
    	private _colById;
    	private _colDefaults;
    	private _colLeft;
    	private _colRight;
    	private _cols;
    	private _columnCssRulesL;
    	private _columnCssRulesR;
    	private _currentEditor;
    	private _data;
    	private _editController;
    	private _headerColumnWidthDiff;
    	private _hEditorLoader;
    	private _hPostRender;
    	private _hPostRenderCleanup;
    	private _hRender;
    	private _ignoreScrollUntil;
    	private _initColById;
    	private _initCols;
    	private _initialized;
    	private _jQuery;
    	private _jumpinessCoefficient;
    	private _lastRenderTime;
    	private _layout;
    	private _numberOfPages;
    	private _options;
    	private _page;
    	private _pageHeight;
    	private _pageOffset;
    	private _pagingActive;
    	private _pagingIsLastPage;
    	private _plugins;
    	private _postProcessCleanupQueue;
    	private _postProcessedRows;
    	private _postProcessFromRow;
    	private _postProcessGroupId;
    	private _postProcessToRow;
    	private _rowsCache;
    	private _scrollDims;
    	private _scrollLeft;
    	private _scrollLeftPrev;
    	private _scrollLeftRendered;
    	private _scrollTop;
    	private _scrollTopPrev;
    	private _scrollTopRendered;
    	private _selectedRows;
    	private _selectionModel;
    	private _serializedEditorValue;
    	private _sortColumns;
    	private _styleNode;
    	private _stylesheet;
    	private _tabbingDirection;
    	private _uid;
    	private _viewportInfo;
    	private _vScrollDir;
    	private _boundAncestorScroll;
    	private _container;
    	private _focusSink1;
    	private _focusSink2;
    	private _groupingPanel;
    	readonly onActiveCellChanged: EventEmitter<ArgsCell, IEventData>;
    	readonly onActiveCellPositionChanged: EventEmitter<ArgsGrid, IEventData>;
    	readonly onAddNewRow: EventEmitter<ArgsAddNewRow, IEventData>;
    	readonly onBeforeCellEditorDestroy: EventEmitter<ArgsEditorDestroy, IEventData>;
    	readonly onBeforeDestroy: EventEmitter<ArgsGrid, IEventData>;
    	readonly onBeforeEditCell: EventEmitter<ArgsCellEdit, IEventData>;
    	readonly onBeforeFooterRowCellDestroy: EventEmitter<ArgsColumnNode, IEventData>;
    	readonly onBeforeHeaderCellDestroy: EventEmitter<ArgsColumnNode, IEventData>;
    	readonly onBeforeHeaderRowCellDestroy: EventEmitter<ArgsColumnNode, IEventData>;
    	readonly onCellChange: EventEmitter<ArgsCellChange, IEventData>;
    	readonly onCellCssStylesChanged: EventEmitter<ArgsCssStyle, IEventData>;
    	readonly onClick: EventEmitter<ArgsCell, MouseEvent>;
    	readonly onColumnsReordered: EventEmitter<ArgsGrid, IEventData>;
    	readonly onColumnsResized: EventEmitter<ArgsGrid, IEventData>;
    	readonly onCompositeEditorChange: EventEmitter<ArgsGrid, IEventData>;
    	readonly onContextMenu: EventEmitter<ArgsGrid, UIEvent>;
    	readonly onDblClick: EventEmitter<ArgsCell, MouseEvent>;
    	readonly onDrag: EventEmitter<ArgsGrid, UIEvent>;
    	readonly onDragEnd: EventEmitter<ArgsGrid, UIEvent>;
    	readonly onDragInit: EventEmitter<ArgsGrid, UIEvent>;
    	readonly onDragStart: EventEmitter<ArgsGrid, UIEvent>;
    	readonly onFooterRowCellRendered: EventEmitter<ArgsColumnNode, IEventData>;
    	readonly onHeaderCellRendered: EventEmitter<ArgsColumnNode, IEventData>;
    	readonly onHeaderClick: EventEmitter<ArgsColumn, IEventData>;
    	readonly onHeaderContextMenu: EventEmitter<ArgsColumn, IEventData>;
    	readonly onHeaderMouseEnter: EventEmitter<ArgsColumn, MouseEvent>;
    	readonly onHeaderMouseLeave: EventEmitter<ArgsColumn, MouseEvent>;
    	readonly onHeaderRowCellRendered: EventEmitter<ArgsColumnNode, IEventData>;
    	readonly onKeyDown: EventEmitter<ArgsCell, KeyboardEvent>;
    	readonly onMouseEnter: EventEmitter<ArgsGrid, MouseEvent>;
    	readonly onMouseLeave: EventEmitter<ArgsGrid, MouseEvent>;
    	readonly onScroll: EventEmitter<ArgsScroll, IEventData>;
    	readonly onSelectedRowsChanged: EventEmitter<ArgsSelectedRowsChange, IEventData>;
    	readonly onSort: EventEmitter<ArgsSort, IEventData>;
    	readonly onValidationError: EventEmitter<ArgsValidationError, IEventData>;
    	readonly onViewportChanged: EventEmitter<ArgsGrid, IEventData>;
    	constructor(container: HTMLElement | {
    		jquery: string;
    		length: number;
    	}, data: any, columns: Column<TItem>[], options: GridOptions<TItem>);
    	private createGroupingPanel;
    	private bindAncestorScroll;
    	init(): void;
    	private hasFrozenColumns;
    	private hasFrozenRows;
    	registerPlugin(plugin: IPlugin): void;
    	unregisterPlugin(plugin: IPlugin): void;
    	getPluginByName(name: string): IPlugin;
    	setSelectionModel(model: SelectionModel): void;
    	private unregisterSelectionModel;
    	getScrollBarDimensions(): {
    		width: number;
    		height: number;
    	};
    	getDisplayedScrollbarDimensions(): {
    		width: number;
    		height: number;
    	};
    	getAbsoluteColumnMinWidth(): number;
    	getSelectionModel(): SelectionModel;
    	private colIdOrIdxToCell;
    	getCanvasNode(columnIdOrIdx?: string | number, row?: number): HTMLElement;
    	getCanvases(): any | HTMLElement[];
    	getActiveCanvasNode(e?: IEventData): HTMLElement;
    	getViewportNode(columnIdOrIdx?: string | number, row?: number): HTMLElement;
    	private getViewports;
    	getActiveViewportNode(e?: IEventData): HTMLElement;
    	private getAvailableWidth;
    	private updateCanvasWidth;
    	private unbindAncestorScrollEvents;
    	updateColumnHeader(columnId: string, title?: string, toolTip?: string): void;
    	getHeader(): HTMLElement;
    	getHeaderColumn(columnIdOrIdx: string | number): HTMLElement;
    	getGroupingPanel(): HTMLElement;
    	getPreHeaderPanel(): HTMLElement;
    	getHeaderRow(): HTMLElement;
    	getHeaderRowColumn(columnIdOrIdx: string | number): HTMLElement;
    	getFooterRow(): HTMLElement;
    	getFooterRowColumn(columnIdOrIdx: string | number): HTMLElement;
    	private createColumnFooters;
    	private createColumnHeaders;
    	private setupColumnSort;
    	private setupColumnReorder;
    	private setupColumnResize;
    	private setOverflow;
    	private measureCellPaddingAndBorder;
    	private createCssRules;
    	private getColumnCssRules;
    	private removeCssRules;
    	destroy(): void;
    	private trigger;
    	getEditorLock(): EditorLock;
    	getEditController(): EditController;
    	getColumnIndex(id: string): number;
    	getInitialColumnIndex(id: string): number;
    	autosizeColumns(): void;
    	private applyColumnHeaderWidths;
    	setSortColumn(columnId: string, ascending: boolean): void;
    	setSortColumns(cols: ColumnSort[]): void;
    	getSortColumns(): ColumnSort[];
    	private handleSelectedRangesChanged;
    	getColumns(): Column<TItem>[];
    	getInitialColumns(): Column<TItem>[];
    	private updateViewColLeftRight;
    	private setInitialCols;
    	setColumns(columns: Column<TItem>[]): void;
    	getOptions(): GridOptions<TItem>;
    	setOptions(args: GridOptions<TItem>, suppressRender?: boolean, suppressColumnSet?: boolean, suppressSetOverflow?: boolean): void;
    	private validateAndEnforceOptions;
    	private viewOnRowCountChanged;
    	private viewOnRowsChanged;
    	private viewOnDataChanged;
    	private bindToData;
    	private unbindFromData;
    	setData(newData: any, scrollToTop?: boolean): void;
    	getData(): any;
    	getDataLength(): number;
    	private getDataLengthIncludingAddNew;
    	getDataItem(i: number): TItem;
    	getTopPanel(): HTMLElement;
    	setTopPanelVisibility(visible: boolean): void;
    	setColumnHeaderVisibility(visible: boolean, animate?: boolean): void;
    	setFooterRowVisibility(visible: boolean): void;
    	setGroupingPanelVisibility(visible: boolean): void;
    	setPreHeaderPanelVisibility(visible: boolean): void;
    	setHeaderRowVisibility(visible: boolean): void;
    	getContainerNode(): HTMLElement;
    	getUID(): string;
    	private getRowTop;
    	private getRowFromPosition;
    	private scrollTo;
    	getFormatter(row: number, column: Column<TItem>): ColumnFormat<TItem>;
    	getFormatterContext(row: number, cell: number): FormatterContext;
    	private getEditor;
    	getDataItemValueForColumn(item: TItem, columnDef: Column<TItem>): any;
    	private appendRowHtml;
    	private appendCellHtml;
    	private cleanupRows;
    	invalidate(): void;
    	invalidateAllRows(): void;
    	private queuePostProcessedRowForCleanup;
    	private queuePostProcessedCellForCleanup;
    	private removeRowFromCache;
    	invalidateRows(rows: number[]): void;
    	invalidateRow(row: number): void;
    	updateCell(row: number, cell: number): void;
    	private updateCellWithFormatter;
    	updateRow(row: number): void;
    	private calcViewportSize;
    	resizeCanvas: () => void;
    	updatePagingStatusFromView(pagingInfo: {
    		pageSize: number;
    		pageNum: number;
    		totalPages: number;
    	}): void;
    	private updateRowCount;
    	/**
    	 * @param viewportTop optional viewport top
    	 * @param viewportLeft optional viewport left
    	 * @returns viewport range
    	 */
    	getViewport(viewportTop?: number, viewportLeft?: number): ViewRange;
    	getVisibleRange(viewportTop?: number, viewportLeft?: number): ViewRange;
    	getRenderedRange(viewportTop?: number, viewportLeft?: number): ViewRange;
    	private ensureCellNodesInRowsCache;
    	private cleanUpCells;
    	private cleanUpAndRenderCells;
    	private renderRows;
    	private startPostProcessing;
    	private startPostProcessingCleanup;
    	private invalidatePostProcessingResults;
    	private updateRowPositions;
    	private updateGrandTotals;
    	groupTotalsFormatter(p1?: GroupTotals<TItem>, p2?: Column<TItem>, grid?: any): string;
    	render: () => void;
    	private handleHeaderRowScroll;
    	private handleFooterRowScroll;
    	private handleMouseWheel;
    	private handleScroll;
    	private asyncPostProcessRows;
    	private asyncPostProcessCleanupRows;
    	private updateCellCssStylesOnRenderedRows;
    	addCellCssStyles(key: string, hash: CellStylesHash): void;
    	removeCellCssStyles(key: string): void;
    	setCellCssStyles(key: string, hash: CellStylesHash): void;
    	getCellCssStyles(key: string): CellStylesHash;
    	flashCell(row: number, cell: number, speed?: number): void;
    	private handleDragInit;
    	private handleDragStart;
    	private handleDrag;
    	private handleDragEnd;
    	private handleKeyDown;
    	private handleClick;
    	private handleContextMenu;
    	private handleDblClick;
    	private handleHeaderMouseEnter;
    	private handleHeaderMouseLeave;
    	private handleHeaderContextMenu;
    	private handleHeaderClick;
    	private handleMouseEnter;
    	private handleMouseLeave;
    	private cellExists;
    	getCellFromPoint(x: number, y: number): {
    		row: number;
    		cell: number;
    	};
    	getCellFromNode(cellNode: Element): number;
    	getColumnFromNode(cellNode: Element): Column<TItem>;
    	getRowFromNode(rowNode: Element): number;
    	getCellFromEvent(e: any): {
    		row: number;
    		cell: number;
    	};
    	getCellNodeBox(row: number, cell: number): {
    		top: number;
    		right: number;
    		bottom: number;
    		left: number;
    	};
    	resetActiveCell(): void;
    	focus(): void;
    	private setFocus;
    	scrollCellIntoView(row: number, cell: number, doPaging?: boolean): void;
    	scrollColumnIntoView(cell: number): void;
    	private internalScrollColumnIntoView;
    	private setActiveCellInternal;
    	clearTextSelection(): void;
    	private isCellPotentiallyEditable;
    	private makeActiveCellNormal;
    	editActiveCell(editor?: EditorClass): void;
    	private makeActiveCellEditable;
    	private commitEditAndSetFocus;
    	private cancelEditAndSetFocus;
    	private getActiveCellPosition;
    	getGridPosition(): Position;
    	private handleActiveCellPositionChange;
    	getCellEditor(): Editor;
    	getActiveCell(): RowCell;
    	getActiveCellNode(): HTMLElement;
    	scrollActiveCellIntoView(): void;
    	scrollRowIntoView(row: number, doPaging?: boolean): void;
    	scrollRowToTop(row: number): void;
    	private scrollPage;
    	navigatePageDown(): void;
    	navigatePageUp(): void;
    	navigateTop(): void;
    	navigateBottom(): void;
    	navigateToRow(row: number): boolean;
    	getColspan(row: number, cell: number): number;
    	navigateRight(): boolean;
    	navigateLeft(): boolean;
    	navigateDown(): boolean;
    	navigateUp(): boolean;
    	navigateNext(): boolean;
    	navigatePrev(): boolean;
    	navigateRowStart(): boolean;
    	navigateRowEnd(): boolean;
    	/**
    	 * @param {string} dir Navigation direction.
    	 * @return {boolean} Whether navigation resulted in a change of active cell.
    	 */
    	navigate(dir: string): boolean;
    	getCellNode(row: number, cell: number): HTMLElement;
    	setActiveCell(row: number, cell: number): void;
    	setActiveRow(row: number, cell: number, suppressScrollIntoView?: boolean): void;
    	private canCellBeActive;
    	canCellBeSelected(row: number, cell: number): any;
    	gotoCell(row: number, cell: number, forceEdit?: boolean): void;
    	commitCurrentEdit(): boolean;
    	private cancelCurrentEdit;
    	private rowsToRanges;
    	getSelectedRows(): number[];
    	setSelectedRows(rows: number[]): void;
    }
    interface ArgsGrid {
    	grid?: Grid;
    }
    interface ArgsColumn extends ArgsGrid {
    	column: Column;
    }
    interface ArgsColumnNode extends ArgsColumn {
    	node: HTMLElement;
    }
    export type ArgsSortCol = {
    	sortCol: Column;
    	sortAsc: boolean;
    };
    interface ArgsSort extends ArgsGrid {
    	multiColumnSort: boolean;
    	sortAsc?: boolean;
    	sortCol?: Column;
    	sortCols?: ArgsSortCol[];
    }
    interface ArgsSelectedRowsChange extends ArgsGrid {
    	rows: number[];
    	changedSelectedRows?: number[];
    	changedUnselectedRows?: number[];
    	previousSelectedRows?: number[];
    	caller: any;
    }
    interface ArgsScroll extends ArgsGrid {
    	scrollLeft: number;
    	scrollTop: number;
    }
    interface ArgsCssStyle extends ArgsGrid {
    	key: string;
    	hash: CellStylesHash;
    }
    interface ArgsCell extends ArgsGrid {
    	row: number;
    	cell: number;
    }
    interface ArgsCellChange extends ArgsCell {
    	item: any;
    }
    interface ArgsCellEdit extends ArgsCellChange {
    	column: Column;
    }
    interface ArgsAddNewRow extends ArgsColumn {
    	item: any;
    }
    interface ArgsEditorDestroy extends ArgsGrid {
    	editor: Editor;
    }
    interface ArgsValidationError extends ArgsCell {
    	editor: Editor;
    	column: Column;
    	cellNode: HTMLElement;
    	validationResults: ValidationResult;
    }
    const BasicLayout: {
    	new (): LayoutEngine;
    };
    const FrozenLayout: {
    	new (): LayoutEngine;
    };
    function PercentCompleteFormatter(ctx: FormatterContext): string;
    function PercentCompleteBarFormatter(ctx: FormatterContext): string;
    function YesNoFormatter(ctx: FormatterContext): "Yes" | "No";
    function CheckboxFormatter(ctx: FormatterContext): string;
    function CheckmarkFormatter(ctx: FormatterContext): "" | "<i class=\"slick-checkmark\"></i>";
    namespace Formatters {
    	function PercentComplete(_row: number, _cell: number, value: any): string;
    	function PercentCompleteBar(_row: number, _cell: number, value: any): string;
    	function YesNo(_row: number, _cell: number, value: any): "Yes" | "No";
    	function Checkbox(_row: number, _cell: number, value: any): string;
    	function Checkmark(_row: number, _cell: number, value: any): "" | "<i class=\"slick-checkmark\"></i>";
    }
    abstract class BaseEditor {
    	protected _input: HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement;
    	protected _defaultValue: any;
    	protected _args: EditorOptions;
    	constructor(args: EditorOptions);
    	abstract init(): void;
    	destroy(): void;
    	focus(): void;
    	getValue(): string;
    	setValue(val: string): void;
    	loadValue(item: any): void;
    	serializeValue(): any;
    	applyValue(item: any, state: any): void;
    	isValueChanged(): boolean;
    	validate(): ValidationResult;
    }
    class TextEditor extends BaseEditor {
    	_input: HTMLInputElement;
    	init(): void;
    }
    class IntegerEditor extends TextEditor {
    	serializeValue(): number;
    	validate(): ValidationResult;
    }
    class FloatEditor extends TextEditor {
    	static AllowEmptyValue: boolean;
    	static DefaultDecimalPlaces: number;
    	getDecimalPlaces(): number;
    	loadValue(item: any): void;
    	serializeValue(): any;
    	validate(): ValidationResult;
    }
    class DateEditor extends TextEditor {
    	private _calendarOpen;
    	init(): void;
    	destroy(): void;
    	show(): void;
    	hide(): void;
    	position(position: Position): void;
    }
    class YesNoSelectEditor extends BaseEditor {
    	_input: HTMLSelectElement;
    	init(): void;
    	loadValue(item: any): void;
    	serializeValue(): boolean;
    	isValueChanged(): boolean;
    	validate(): {
    		valid: boolean;
    		msg: string;
    	};
    }
    class CheckboxEditor extends BaseEditor {
    	_input: HTMLInputElement;
    	init(): void;
    	loadValue(item: any): void;
    	preClick(): void;
    	serializeValue(): boolean;
    	applyValue(item: any, state: any): void;
    	isValueChanged(): boolean;
    	validate(): {
    		valid: boolean;
    		msg: string;
    	};
    }
    class PercentCompleteEditor extends IntegerEditor {
    	protected _picker: HTMLDivElement;
    	init(): void;
    	destroy(): void;
    }
    class LongTextEditor extends BaseEditor {
    	_input: HTMLTextAreaElement;
    	protected _container: HTMLElement;
    	protected _wrapper: HTMLDivElement;
    	init(): void;
    	handleKeyDown(e: KeyboardEvent): void;
    	save(): void;
    	cancel(): void;
    	hide(): void;
    	show(): void;
    	position(position: Position): void;
    	destroy(): void;
    }
    namespace Editors {
    	const Text: typeof TextEditor;
    	const Integer: typeof IntegerEditor;
    	const Float: typeof FloatEditor;
    	const Date: typeof DateEditor;
    	const YesNoSelect: typeof YesNoSelectEditor;
    	const Checkbox: typeof CheckboxEditor;
    	const PercentComplete: typeof PercentCompleteEditor;
    	const LongText: typeof LongTextEditor;
    }
    interface GroupItemMetadataProviderOptions {
    	enableExpandCollapse?: boolean;
    	groupCellCssClass?: string;
    	groupCssClass?: string;
    	groupIndentation?: number;
    	groupFocusable?: boolean;
    	groupFormat?: ColumnFormat<Group>;
    	groupFormatter?: CompatFormatter<Group>;
    	groupLevelPrefix?: string;
    	groupRowTotals?: boolean;
    	groupTitleCssClass?: string;
    	hasSummaryType?: (column: Column) => boolean;
    	toggleCssClass?: string;
    	toggleExpandedCssClass?: string;
    	toggleCollapsedCssClass?: string;
    	totalsCssClass?: string;
    	totalsFocusable?: boolean;
    	totalsFormat?: ColumnFormat<GroupTotals>;
    	totalsFormatter?: CompatFormatter<GroupTotals>;
    }
    class GroupItemMetadataProvider {
    	protected grid: Pick<Grid, "getActiveCell" | "getColumns" | "getData" | "getDataItem" | "getRenderedRange" | "onClick" | "onKeyDown" | "groupTotalsFormatter">;
    	private options;
    	constructor(opt?: GroupItemMetadataProviderOptions);
    	static readonly defaults: GroupItemMetadataProviderOptions;
    	static defaultGroupFormat(ctx: FormatterContext, opt?: GroupItemMetadataProviderOptions): string;
    	static defaultTotalsFormat(ctx: FormatterContext, grid?: typeof this.prototype["grid"]): string;
    	init(grid: typeof this.grid): void;
    	readonly pluginName = "GroupItemMetadataProvider";
    	destroy(): void;
    	getOptions(): GroupItemMetadataProviderOptions;
    	setOptions(value: GroupItemMetadataProviderOptions): void;
    	handleGridClick: (e: MouseEvent, args: ArgsCell) => void;
    	handleGridKeyDown: (e: KeyboardEvent, args: ArgsCell) => void;
    	groupCellPosition: () => {
    		cell: number;
    		colspan: number | "*";
    	};
    	getGroupRowMetadata: ((item: Group) => ItemMetadata);
    	getTotalsRowMetadata: ((item: GroupTotals) => ItemMetadata);
    }
    interface AutoTooltipsOptions {
    	enableForCells?: boolean;
    	enableForHeaderCells?: boolean;
    	maxToolTipLength?: number;
    	replaceExisting?: boolean;
    }
    class AutoTooltips implements IPlugin {
    	private grid;
    	private options;
    	constructor(options?: AutoTooltipsOptions);
    	static readonly defaults: AutoTooltipsOptions;
    	init(grid: Grid): void;
    	destroy(): void;
    	private handleMouseEnter;
    	private handleHeaderMouseEnter;
    	pluginName: string;
    }
}


declare namespace Slick {
    interface Column<TItem = any> {
        referencedFields?: string[];
        sourceItem?: Q.PropertyItem;
    }
}

declare namespace Slick {
    namespace Aggregators {
        function Avg(field: string): void;
        function WeightedAvg(field: string, weightedField: string): void;
        function Min(field: string): void;
        function Max(field: string): void;
        function Sum(field: string): void;
    }
    namespace AggregateFormatting {
        function formatMarkup<TItem = any>(totals: GroupTotals, column: Column<TItem>, aggType: string): string;
        function formatValue(column: Column, value: number): string;
        function groupTotalsFormatter<TItem = any>(totals: GroupTotals, column: Column<TItem>): string;
    }

    type Format<TItem = any> = (ctx: FormatterContext<TItem>) => string;

    interface Formatter {
        format(ctx: FormatterContext): string;
    }
    interface GroupInfo<TItem> {
        getter?: any;
        formatter?: (p1: Group<TItem>) => string;
        comparer?: (a: Group<TItem>, b: Group<TItem>) => number;
        aggregators?: any[];
        aggregateCollapsed?: boolean;
        lazyTotalsCalculation?: boolean;
    }
    interface PagerOptions {
        view?: any;
        showRowsPerPage?: boolean;
        rowsPerPage?: number;
        rowsPerPageOptions?: number[];
        onChangePage?: (newPage: number) => void;
        onRowsPerPageChange?: (n: number) => void;
    }
    interface SummaryOptions {
        aggregators: any[];
    }
    interface PagingOptions {
        rowsPerPage?: number;
        page?: number;
    }

    interface RemoteViewOptions {
        autoLoad?: boolean;
        idField?: string;
        contentType?: string;
        dataType?: string;
        filter?: any;
        params?: any;
        onSubmit?: CancellableViewCallback<any>;
        url?: string;
        localSort?: boolean;
        sortBy?: any;
        rowsPerPage?: number;
        seekToPage?: number;
        onProcessData?: RemoteViewProcessCallback<any>;
        method?: string;
        inlineFilters?: boolean;
        groupItemMetadataProvider?: GroupItemMetadataProvider;
        onAjaxCall?: RemoteViewAjaxCallback<any>;
        getItemMetadata?: (p1?: any, p2?: number) => any;
        errorMsg?: string;
    }
    interface PagingInfo {
        rowsPerPage: number;
        page: number;
        totalCount: number;
        loading: boolean;
        error: string;
        dataView: RemoteView<any>;
    }
    type CancellableViewCallback<TEntity> = (view: RemoteView<TEntity>) => boolean | void;
    type RemoteViewAjaxCallback<TEntity> = (view: RemoteView<TEntity>, options: JQueryAjaxSettings) => boolean | void;
    type RemoteViewFilter<TEntity> = (item: TEntity, view: RemoteView<TEntity>) => boolean;
    type RemoteViewProcessCallback<TEntity> = (data: Q.ListResponse<TEntity>, view: RemoteView<TEntity>) => Q.ListResponse<TEntity>;
    interface RemoteView<TEntity> {
        onSubmit: CancellableViewCallback<TEntity>;
        onDataChanged: EventEmitter;
        onDataLoading: EventEmitter;
        onDataLoaded: EventEmitter;
        onPagingInfoChanged: EventEmitter;
        onRowCountChanged: EventEmitter;
        onRowsChanged: EventEmitter;
        onRowsOrCountChanged: EventEmitter;
        getPagingInfo(): PagingInfo;
        onGroupExpanded: EventEmitter;
        onGroupCollapsed: EventEmitter;
        onAjaxCall: RemoteViewAjaxCallback<TEntity>;
        onProcessData: RemoteViewProcessCallback<TEntity>;
        addData(data: Q.ListResponse<TEntity>): void;
        beginUpdate(): void;
        endUpdate(): void;
        deleteItem(id: any): void;
        getItems(): TEntity[];
        setFilter(filter: RemoteViewFilter<TEntity>): void;
        getFilter(): RemoteViewFilter<TEntity>;
        getFilteredItems(): any;
        getGroupItemMetadataProvider(): GroupItemMetadataProvider;
        setGroupItemMetadataProvider(value: GroupItemMetadataProvider): void;
        fastSort: any;
        setItems(items: any[], newIdProperty?: boolean | string): void;
        getIdPropertyName(): string;
        getItemById(id: any): TEntity;
        getGrandTotals(): any;
        getGrouping(): GroupInfo<TEntity>[];
        getGroups(): any[];
        getRowById(id: any): number;
        getRowByItem(item: any): number;
        getRows(): any[];
        mapItemsToRows(itemArray: any[]): any[];
        mapRowsToIds(rowArray: number[]): any[];
        mapIdsToRows(idAray: any[]): number[];
        setFilterArgs(args: any): void;
        setRefreshHints(hints: any[]): void;
        insertItem(insertBefore: number, item: any): void;
        sortedAddItem(item: any): void;
        sortedUpdateItem(id: any, item: any): void;
        syncGridSelection(grid: any, preserveHidden?: boolean, preserveHiddenOnSelectionChange?: boolean): void;
        syncGridCellCssStyles(grid: any, key: string): void;
        getItemMetadata(i: number): any;
        updateItem(id: any, item: TEntity): void;
        addItem(item: TEntity): void;
        getIdxById(id: any): any;
        getItemByIdx(index: number): any;
        setGrouping(groupInfo: GroupInfo<TEntity>[]): void;
        collapseAllGroups(level: number): void;
        expandAllGroups(level: number): void;
        expandGroup(keys: any[]): void;
        collapseGroup(keys: any[]): void;
        setSummaryOptions(options: SummaryOptions): void;
        setPagingOptions(options: PagingOptions): void;
        refresh(): void;
        populate(): void;
        populateLock(): void;
        populateUnlock(): void;
        getItem(row: number): any;
        getLength(): number;
        rowsPerPage: number;
        errormsg: string;
        params: any;
        getLocalSort(): boolean;
        setLocalSort(value: boolean): void;
        sort(comparer?: (a: any, b: any) => number, ascending?: boolean): void;
        reSort(): void;
        sortBy: string[];
        url: string;
        method: string;
        idField: string;
        seekToPage?: number;
    }
    class RemoteView<TEntity> {
        constructor(options: RemoteViewOptions);
    }
}


declare namespace Serenity {
    export import ColumnSelection = Q.ColumnSelection;
    export import Criteria = Q.Criteria;
    export import DeleteRequest = Q.DeleteRequest;
    export import DeleteResponse = Q.DeleteResponse;
    export import ISlickFormatter = Q.ISlickFormatter;
    export import ListRequest = Q.ListRequest;
    export import ListResponse = Q.ListResponse;
    export import PropertyItem = Q.PropertyItem;
    export import PropertyItemsData = Q.PropertyItemsData;
    export import RetrieveColumnSelection = Q.RetrieveColumnSelection;
    export import RetrieveLocalizationRequest = Q.RetrieveLocalizationRequest;
    export import RetrieveLocalizationResponse = Q.RetrieveLocalizationResponse;
    export import RetrieveRequest = Q.RetrieveRequest;
    export import RetrieveResponse = Q.RetrieveResponse;
    export import SaveRequest = Q.SaveRequest;
    export import SaveRequestWithAttachment = Q.SaveRequestWithAttachment;
    export import SaveResponse = Q.SaveResponse;
    export import SaveWithLocalizationRequest = Q.SaveWithLocalizationRequest;
    export import ServiceError = Q.ServiceError;
    export import ServiceOptions = Q.ServiceOptions;
    export import ServiceRequest = Q.ServiceRequest;
    export import ServiceResponse = Q.ServiceResponse;
    export import SummaryType = Q.SummaryType;
    export import UndeleteRequest = Q.UndeleteRequest;
    export import UndeleteResponse = Q.UndeleteResponse;
    export import Formatter = Slick.Formatter;


    class IBooleanValue {
    }
    interface IBooleanValue {
        get_value(): boolean;
        set_value(value: boolean): void;
    }

    class IDoubleValue {
    }
    interface IDoubleValue {
        get_value(): any;
        set_value(value: any): void;
    }

    class IDialog {
    }
    interface IDialog {
        dialogOpen(asPanel?: boolean): void;
    }

    class IEditDialog {
    }
    interface IEditDialog {
        load(entityOrId: any, done: () => void, fail?: (p1: any) => void): void;
    }

    class IGetEditValue {
    }
    interface IGetEditValue {
        getEditValue(property: Q.PropertyItem, target: any): void;
    }

    interface IReadOnly {
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
    }
    class IReadOnly {
    }

    class ISetEditValue {
    }
    interface ISetEditValue {
        setEditValue(source: any, property: Q.PropertyItem): void;
    }

    class IStringValue {
    }
    interface IStringValue {
        get_value(): string;
        set_value(value: string): void;
    }

    interface IValidateRequired {
        get_required(): boolean;
        set_required(value: boolean): void;
    }
    class IValidateRequired {
    }

    enum CaptureOperationType {
        Before = 0,
        Delete = 1,
        Insert = 2,
        Update = 3
    }

    interface DataChangeInfo {
        type: string;
        entityId: any;
        entity: any;
    }

    namespace ReflectionUtils {
        function getPropertyValue(o: any, property: string): any;
        function setPropertyValue(o: any, property: string, value: any): void;
        function makeCamelCase(s: string): string;
    }

    namespace DialogTypeRegistry {
        function get(key: string): any;
        function reset(): void;
        function tryGet(key: string): any;
    }

    namespace EditorTypeRegistry {
        function get(key: string): any;
        function reset(): void;
        function tryGet(key: string): any;
    }

    namespace EnumTypeRegistry {
        function get(key: string): Function;
        function reset(): void;
        function tryGet(key: string): any;
    }

    interface IRowDefinition {
        readonly deletePermission?: string;
        readonly idProperty?: string;
        readonly insertPermission?: string;
        readonly isActiveProperty?: string;
        readonly isDeletedProperty?: string;
        readonly localTextPrefix?: string;
        readonly nameProperty?: string;
        readonly readPermission?: string;
        readonly updatePermission?: string;
    }

    class EnumKeyAttribute {
        value: string;
        constructor(value: string);
    }
    class DisplayNameAttribute {
        displayName: string;
        constructor(displayName: string);
    }
    class CategoryAttribute {
        category: string;
        constructor(category: string);
    }
    class ColumnsKeyAttribute {
        value: string;
        constructor(value: string);
    }
    class CssClassAttribute {
        cssClass: string;
        constructor(cssClass: string);
    }
    class DefaultValueAttribute {
        value: any;
        constructor(value: any);
    }
    class DialogTypeAttribute {
        value: any;
        constructor(value: any);
    }
    class EditorOptionAttribute {
        key: string;
        value: any;
        constructor(key: string, value: any);
    }
    class EditorTypeAttributeBase {
        editorType: string;
        constructor(editorType: string);
        setParams(editorParams: any): void;
    }
    class EditorTypeAttribute extends EditorTypeAttributeBase {
        constructor(editorType: string);
    }
    class ElementAttribute {
        value: string;
        constructor(value: string);
    }
    class EntityTypeAttribute {
        value: string;
        constructor(value: string);
    }
    class FilterableAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class FlexifyAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class FormKeyAttribute {
        value: string;
        constructor(value: string);
    }
    class GeneratedCodeAttribute {
        origin?: string;
        constructor(origin?: string);
    }
    class HiddenAttribute {
        constructor();
    }
    class HintAttribute {
        hint: string;
        constructor(hint: string);
    }
    class IdPropertyAttribute {
        value: string;
        constructor(value: string);
    }
    class InsertableAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class IsActivePropertyAttribute {
        value: string;
        constructor(value: string);
    }
    class ItemNameAttribute {
        value: string;
        constructor(value: string);
    }
    class LocalTextPrefixAttribute {
        value: string;
        constructor(value: string);
    }
    class MaximizableAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class MaxLengthAttribute {
        maxLength: number;
        constructor(maxLength: number);
    }
    class NamePropertyAttribute {
        value: string;
        constructor(value: string);
    }
    class OneWayAttribute {
    }
    class OptionAttribute {
    }
    class OptionsTypeAttribute {
        value: Function;
        constructor(value: Function);
    }
    class PanelAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class PlaceholderAttribute {
        value: string;
        constructor(value: string);
    }
    class ReadOnlyAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class RequiredAttribute {
        isRequired: boolean;
        constructor(isRequired?: boolean);
    }
    class ResizableAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class ResponsiveAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class ServiceAttribute {
        value: string;
        constructor(value: string);
    }
    class UpdatableAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    namespace Decorators {
        function registerClass(nameOrIntf?: string | any[], intf2?: any[]): (target: Function) => void;
        function registerInterface(nameOrIntf?: string | any[], intf2?: any[]): (target: Function) => void;
        function registerEditor(nameOrIntf?: string | any[], intf2?: any[]): (target: Function) => void;
        function registerEnum(target: any, enumKey?: string, name?: string): void;
        function registerEnumType(target: any, name?: string, enumKey?: string): void;
        function registerFormatter(nameOrIntf?: string | any[], intf2?: any[]): (target: Function) => void;
        function enumKey(value: string): (target: Function) => void;
        function option(): (target: Object, propertyKey: string) => void;
        function dialogType(value: any): (target: Function) => void;
        function editor(): (target: Function) => void;
        function element(value: string): (target: Function) => void;
        function filterable(value?: boolean): (target: Function) => void;
        function flexify(value?: boolean): (target: Function) => void;
        function itemName(value: string): (target: Function) => void;
        function maximizable(value?: boolean): (target: Function) => void;
        function optionsType(value: Function): (target: Function) => void;
        function panel(value?: boolean): (target: Function) => void;
        function resizable(value?: boolean): (target: Function) => void;
        function responsive(value?: boolean): (target: Function) => void;
        function service(value: string): (target: Function) => void;
    }

    namespace LazyLoadHelper {
        const executeOnceWhenShown: typeof Q.executeOnceWhenVisible;
        const executeEverytimeWhenShown: typeof Q.executeEverytimeWhenVisible;
    }

    class PrefixedContext {
        readonly idPrefix: string;
        constructor(idPrefix: string);
        byId(id: string): JQuery;
        w<TWidget>(id: string, type: {
            new (...args: any[]): TWidget;
        }): TWidget;
    }

    interface WidgetClass<TOptions = object> {
        new (element: JQuery, options?: TOptions): Widget<TOptions>;
        element: JQuery;
    }
    interface WidgetDialogClass<TOptions = object> {
        new (options?: TOptions): Widget<TOptions> & IDialog;
        element: JQuery;
    }
    type AnyWidgetClass<TOptions = object> = WidgetClass<TOptions> | WidgetDialogClass<TOptions>;
    function reactPatch(): void;
    interface CreateWidgetParams<TWidget extends Widget<TOptions>, TOptions> {
        type?: new (element: JQuery, options?: TOptions) => TWidget;
        options?: TOptions;
        container?: JQuery;
        element?: (e: JQuery) => void;
        init?: (w: TWidget) => void;
    }
    interface WidgetComponentProps<W extends Widget<any>> {
        id?: string;
        name?: string;
        className?: string;
        maxLength?: number;
        placeholder?: string;
        setOptions?: any;
        required?: boolean;
        readOnly?: boolean;
        oneWay?: boolean;
        onChange?: (e: JQueryEventObject) => void;
        onChangeSelect2?: (e: JQueryEventObject) => void;
        value?: any;
        defaultValue?: any;
    }
    class Widget<TOptions = any> {
        private static nextWidgetNumber;
        element: JQuery;
        protected options: TOptions;
        protected widgetName: string;
        protected uniqueName: string;
        readonly idPrefix: string;
        constructor(element: JQuery, options?: TOptions);
        destroy(): void;
        protected addCssClass(): void;
        protected getCssClass(): string;
        static getWidgetName(type: Function): string;
        static elementFor<TWidget>(editorType: {
            new (...args: any[]): TWidget;
        }): JQuery;
        addValidationRule(eventClass: string, rule: (p1: JQuery) => string): JQuery;
        getGridField(): JQuery;
        static create<TWidget extends Widget<TOpt>, TOpt>(params: CreateWidgetParams<TWidget, TOpt>): TWidget;
        initialize(): void;
        init(action?: (widget: any) => void): this;
        protected renderContents(): void;
        private static __isWidgetType;
    }
    interface Widget<TOptions> {
        change(handler: (e: JQueryEventObject) => void): void;
        changeSelect2(handler: (e: JQueryEventObject) => void): void;
    }

    interface ToolButton {
        action?: string;
        title?: string;
        hint?: string;
        cssClass?: string;
        icon?: string;
        onClick?: any;
        htmlEncode?: any;
        hotkey?: string;
        hotkeyAllowDefault?: boolean;
        hotkeyContext?: any;
        separator?: (false | true | 'left' | 'right' | 'both');
        visible?: boolean | (() => boolean);
        disabled?: boolean | (() => boolean);
    }
    interface PopupMenuButtonOptions {
        menu?: JQuery;
        onPopup?: () => void;
        positionMy?: string;
        positionAt?: string;
    }
    class PopupMenuButton extends Widget<PopupMenuButtonOptions> {
        constructor(div: JQuery, opt: PopupMenuButtonOptions);
        destroy(): void;
    }
    interface PopupToolButtonOptions extends PopupMenuButtonOptions {
    }
    class PopupToolButton extends PopupMenuButton {
        constructor(div: JQuery, opt: PopupToolButtonOptions);
    }
    interface ToolbarOptions {
        buttons?: ToolButton[];
        hotkeyContext?: any;
    }
    class Toolbar extends Widget<ToolbarOptions> {
        constructor(div: JQuery, options: ToolbarOptions);
        destroy(): void;
        protected mouseTrap: any;
        protected createButtons(): void;
        protected createButton(container: JQuery, b: ToolButton): void;
        findButton(className: string): JQuery;
        updateInterface(): void;
    }

    class TemplatedWidget<TOptions> extends Widget<TOptions> {
        private static templateNames;
        protected byId(id: string): JQuery;
        private byID;
        private static noGeneric;
        private getDefaultTemplateName;
        protected getTemplateName(): string;
        protected getFallbackTemplate(): string;
        protected getTemplate(): string;
        protected renderContents(): void;
        protected useIdPrefix(): IdPrefixType;
    }
    type IdPrefixType = {
        [key: string]: string;
        Form: string;
        Tabs: string;
        Toolbar: string;
        PropertyGrid: string;
    };
    function useIdPrefix(prefix: string): IdPrefixType;

    class TemplatedDialog<TOptions> extends TemplatedWidget<TOptions> {
        protected tabs: JQuery;
        protected toolbar: Toolbar;
        protected validator: JQueryValidation.Validator;
        constructor(options?: TOptions);
        private get isMarkedAsPanel();
        private get isResponsive();
        private static getCssSize;
        private static applyCssSizes;
        destroy(): void;
        protected initDialog(): void;
        protected getModalOptions(): ModalOptions;
        protected initModal(): void;
        protected initToolbar(): void;
        protected getToolbarButtons(): ToolButton[];
        protected getValidatorOptions(): JQueryValidation.ValidationOptions;
        protected initValidator(): void;
        protected resetValidation(): void;
        protected validateForm(): boolean;
        dialogOpen(asPanel?: boolean): void;
        private useBSModal;
        static bootstrapModal: boolean;
        static openPanel(element: JQuery, uniqueName: string): void;
        static closePanel(element: JQuery, e?: JQueryEventObject): void;
        protected onDialogOpen(): void;
        arrange(): void;
        protected onDialogClose(): void;
        protected addCssClass(): void;
        protected getDialogButtons(): Q.DialogButton[];
        protected getDialogOptions(): any;
        protected getDialogTitle(): string;
        dialogClose(): void;
        get dialogTitle(): string;
        private setupPanelTitle;
        set dialogTitle(value: string);
        set_dialogTitle(value: string): void;
        protected initTabs(): void;
        protected handleResponsive(): void;
    }
    interface ModalOptions {
        backdrop?: boolean | 'static';
        keyboard?: boolean;
        size?: 'lg' | 'sm';
        modalClass?: string;
    }

    class TemplatedPanel<TOptions> extends TemplatedWidget<TOptions> {
        constructor(container: JQuery, options?: TOptions);
        destroy(): void;
        protected tabs: JQuery;
        protected toolbar: Toolbar;
        protected validator: JQueryValidation.Validator;
        protected isPanel: boolean;
        protected responsive: boolean;
        arrange(): void;
        protected getToolbarButtons(): ToolButton[];
        protected getValidatorOptions(): JQueryValidation.ValidationOptions;
        protected initTabs(): void;
        protected initToolbar(): void;
        protected initValidator(): void;
        protected resetValidation(): void;
        protected validateForm(): boolean;
    }

    namespace ValidationHelper {
        function asyncSubmit(form: JQuery, validateBeforeSave: () => boolean, submitHandler: () => void): boolean;
        function submit(form: JQuery, validateBeforeSave: () => boolean, submitHandler: () => void): boolean;
        function getValidator(element: JQuery): JQueryValidation.Validator;
    }
    namespace VX {
        function addValidationRule(element: JQuery, eventClass: string, rule: (p1: JQuery) => string): JQuery;
        function removeValidationRule(element: JQuery, eventClass: string): JQuery;
        function validateElement(validator: JQueryValidation.Validator, widget: Widget<any>): boolean;
    }

    class CascadedWidgetLink<TParent extends Widget<any>> {
        private parentType;
        private widget;
        private parentChange;
        constructor(parentType: {
            new (...args: any[]): TParent;
        }, widget: Widget<any>, parentChange: (p1: TParent) => void);
        private _parentID;
        bind(): TParent;
        unbind(): TParent;
        get_parentID(): string;
        set_parentID(value: string): void;
    }

    namespace TabsExtensions {
        function setDisabled(tabs: JQuery, tabKey: string, isDisabled: boolean): void;
        function toggle(tabs: JQuery, tabKey: string, visible: boolean): void;
        function activeTabKey(tabs: JQuery): string;
        function indexByKey(tabs: JQuery): any;
        function selectTab(tabs: JQuery, tabKey: string): void;
    }

    namespace ReflectionOptionsSetter {
        function set(target: any, options: any): void;
    }

    class PropertyGrid extends Widget<PropertyGridOptions> {
        private editors;
        private items;
        readonly idPrefix: string;
        constructor(div: JQuery, opt: PropertyGridOptions);
        destroy(): void;
        private createItems;
        private createCategoryDiv;
        private categoryLinkClick;
        private determineText;
        private createField;
        private getCategoryOrder;
        private createCategoryLinks;
        get_editors(): Widget<any>[];
        get_items(): Q.PropertyItem[];
        get_idPrefix(): string;
        get_mode(): PropertyGridMode;
        set_mode(value: PropertyGridMode): void;
        static loadEditorValue(editor: Widget<any>, item: Q.PropertyItem, source: any): void;
        static saveEditorValue(editor: Widget<any>, item: Q.PropertyItem, target: any): void;
        private static setReadOnly;
        private static setReadonly;
        private static setRequired;
        private static setMaxLength;
        load(source: any): void;
        save(target?: any): any;
        get value(): any;
        set value(val: any);
        private canModifyItem;
        updateInterface(): void;
        enumerateItems(callback: (p1: Q.PropertyItem, p2: Widget<any>) => void): void;
    }
    enum PropertyGridMode {
        insert = 1,
        update = 2
    }
    interface PropertyGridOptions {
        idPrefix?: string;
        items?: Q.PropertyItem[];
        useCategories?: boolean;
        categoryOrder?: string;
        defaultCategory?: string;
        localTextPrefix?: string;
        mode?: PropertyGridMode;
    }

    class PropertyPanel<TItem, TOptions> extends TemplatedPanel<TOptions> {
        private _entity;
        private _entityId;
        constructor(container: JQuery, options?: TOptions);
        destroy(): void;
        protected initPropertyGrid(): void;
        protected loadInitialEntity(): void;
        protected getFormKey(): string;
        protected getPropertyGridOptions(): PropertyGridOptions;
        protected getPropertyItems(): Q.PropertyItem[];
        protected getSaveEntity(): TItem;
        protected get_entity(): TItem;
        protected get_entityId(): any;
        protected set_entity(value: TItem): void;
        protected set_entityId(value: any): void;
        protected validateBeforeSave(): boolean;
        protected propertyGrid: PropertyGrid;
    }

    namespace SubDialogHelper {
        function bindToDataChange(dialog: any, owner: Widget<any>, dataChange: (p1: any, p2: DataChangeInfo) => void, useTimeout?: boolean): any;
        function triggerDataChange(dialog: Widget<any>): any;
        function triggerDataChanged(element: JQuery): JQuery;
        function bubbleDataChange(dialog: any, owner: Widget<any>, useTimeout?: boolean): any;
        function cascade(cascadedDialog: any, ofElement: JQuery): any;
        function cascadedDialogOffset(element: JQuery): any;
    }

    namespace DialogExtensions {
        function dialogResizable(dialog: JQuery, w?: any, h?: any, mw?: any, mh?: any): JQuery;
        function dialogMaximizable(dialog: JQuery): JQuery;
        function dialogFlexify(dialog: JQuery): JQuery;
    }

    class PropertyDialog<TItem, TOptions> extends TemplatedDialog<TOptions> {
        protected entity: TItem;
        protected entityId: any;
        protected propertyItemsData: Q.PropertyItemsData;
        constructor(opt?: TOptions);
        internalInit(): void;
        protected initSync(): void;
        protected initAsync(): Promise<void>;
        protected afterInit(): void;
        protected useAsync(): boolean;
        destroy(): void;
        protected getDialogOptions(): any;
        protected getDialogButtons(): Q.DialogButton[];
        protected okClick(): void;
        protected okClickValidated(): void;
        protected cancelClick(): void;
        protected initPropertyGrid(): void;
        protected getFormKey(): string;
        protected getPropertyGridOptions(): PropertyGridOptions;
        protected getPropertyItems(): Q.PropertyItem[];
        protected getPropertyItemsData(): Q.PropertyItemsData;
        protected getPropertyItemsDataAsync(): Promise<Q.PropertyItemsData>;
        protected getSaveEntity(): TItem;
        protected loadInitialEntity(): void;
        protected get_entity(): TItem;
        protected set_entity(value: TItem): void;
        protected get_entityId(): any;
        protected set_entityId(value: any): void;
        protected validateBeforeSave(): boolean;
        protected updateTitle(): void;
        protected propertyGrid: PropertyGrid;
        protected getFallbackTemplate(): string;
    }

    namespace EditorUtils {
        function getDisplayText(editor: Widget<any>): string;
        function getValue(editor: Widget<any>): any;
        function saveValue(editor: Widget<any>, item: Q.PropertyItem, target: any): void;
        function setValue(editor: Widget<any>, value: any): void;
        function loadValue(editor: Widget<any>, item: Q.PropertyItem, source: any): void;
        function setReadonly(elements: JQuery, isReadOnly: boolean): JQuery;
        function setReadOnly(widget: Widget<any>, isReadOnly: boolean): void;
        function setRequired(widget: Widget<any>, isRequired: boolean): void;
        function setContainerReadOnly(container: JQuery, readOnly: boolean): void;
    }

    class StringEditor extends Widget<any> {
        constructor(input: JQuery);
        get value(): string;
        protected get_value(): string;
        set value(value: string);
        protected set_value(value: string): void;
    }

    class PasswordEditor extends StringEditor {
        constructor(input: JQuery);
    }

    interface TextAreaEditorOptions {
        cols?: number;
        rows?: number;
    }
    class TextAreaEditor extends Widget<TextAreaEditorOptions> {
        constructor(input: JQuery, opt?: TextAreaEditorOptions);
        get value(): string;
        protected get_value(): string;
        set value(value: string);
        protected set_value(value: string): void;
    }

    class BooleanEditor extends Widget<any> {
        get value(): boolean;
        protected get_value(): boolean;
        set value(value: boolean);
        protected set_value(value: boolean): void;
    }

    interface DecimalEditorOptions {
        minValue?: string;
        maxValue?: string;
        decimals?: any;
        padDecimals?: any;
        allowNegatives?: boolean;
    }
    class DecimalEditor extends Widget<DecimalEditorOptions> implements IDoubleValue {
        constructor(input: JQuery, opt?: DecimalEditorOptions);
        get_value(): number;
        get value(): number;
        set_value(value: number): void;
        set value(v: number);
        get_isValid(): boolean;
        static defaultAutoNumericOptions(): any;
    }

    interface IntegerEditorOptions {
        minValue?: number;
        maxValue?: number;
        allowNegatives?: boolean;
    }
    class IntegerEditor extends Widget<IntegerEditorOptions> implements IDoubleValue {
        constructor(input: JQuery, opt?: IntegerEditorOptions);
        get_value(): number;
        get value(): number;
        set_value(value: number): void;
        set value(v: number);
        get_isValid(): boolean;
    }

    let datePickerIconSvg: string;
    class DateEditor extends Widget<any> implements IStringValue, IReadOnly {
        private minValue;
        private maxValue;
        constructor(input: JQuery);
        static useFlatpickr: boolean;
        static flatPickrOptions(input: JQuery): {
            clickOpens: boolean;
            allowInput: boolean;
            dateFormat: string;
            onChange: () => void;
        };
        get_value(): string;
        get value(): string;
        set_value(value: string): void;
        set value(v: string);
        private get_valueAsDate;
        get valueAsDate(): Date;
        private set_valueAsDate;
        set valueAsDate(v: Date);
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        yearRange: string;
        get_minValue(): string;
        set_minValue(value: string): void;
        get_maxValue(): string;
        set_maxValue(value: string): void;
        get_minDate(): Date;
        set_minDate(value: Date): void;
        get_maxDate(): Date;
        set_maxDate(value: Date): void;
        get_sqlMinMax(): boolean;
        set_sqlMinMax(value: boolean): void;
        static dateInputChange: (e: JQueryEventObject) => void;
        static flatPickrTrigger(input: JQuery): JQuery;
        static dateInputKeyup(e: JQueryEventObject): void;
        static uiPickerZIndexWorkaround(input: JQuery): void;
    }

    class DateTimeEditor extends Widget<DateTimeEditorOptions> implements IStringValue, IReadOnly {
        private minValue;
        private maxValue;
        private time;
        private lastSetValue;
        private lastSetValueGet;
        constructor(input: JQuery, opt?: DateTimeEditorOptions);
        getFlatpickrOptions(): any;
        get_value(): string;
        get value(): string;
        set_value(value: string): void;
        private getInplaceNowText;
        private getDisplayFormat;
        set value(v: string);
        private get_valueAsDate;
        get valueAsDate(): Date;
        private set_valueAsDate;
        set valueAsDate(value: Date);
        get_minValue(): string;
        set_minValue(value: string): void;
        get_maxValue(): string;
        set_maxValue(value: string): void;
        get_minDate(): Date;
        set_minDate(value: Date): void;
        get_maxDate(): Date;
        set_maxDate(value: Date): void;
        get_sqlMinMax(): boolean;
        set_sqlMinMax(value: boolean): void;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        static roundToMinutes(date: Date, minutesStep: number): Date;
        static getTimeOptions: (fromHour: number, fromMin: number, toHour: number, toMin: number, stepMins: number) => string[];
    }
    interface DateTimeEditorOptions {
        startHour?: any;
        endHour?: any;
        intervalMinutes?: any;
        yearRange?: string;
        useUtc?: boolean;
        seconds?: boolean;
        inputOnly?: boolean;
    }

    interface TimeEditorOptions {
        noEmptyOption?: boolean;
        startHour?: any;
        endHour?: any;
        intervalMinutes?: any;
    }
    class TimeEditor extends Widget<TimeEditorOptions> {
        private minutes;
        constructor(input: JQuery, opt?: TimeEditorOptions);
        get value(): number;
        protected get_value(): number;
        set value(value: number);
        protected set_value(value: number): void;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
    }

    interface EmailEditorOptions {
        domain?: string;
        readOnlyDomain?: boolean;
    }
    class EmailEditor extends Widget<EmailEditorOptions> {
        constructor(input: JQuery, opt: EmailEditorOptions);
        static registerValidationMethods(): void;
        get_value(): string;
        get value(): string;
        set_value(value: string): void;
        set value(v: string);
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
    }

    class EmailAddressEditor extends StringEditor {
        constructor(input: JQuery);
    }

    class URLEditor extends StringEditor {
        constructor(input: JQuery);
    }

    interface RadioButtonEditorOptions {
        enumKey?: string;
        enumType?: any;
        lookupKey?: string;
    }
    class RadioButtonEditor extends Widget<RadioButtonEditorOptions> implements IReadOnly {
        constructor(input: JQuery, opt: RadioButtonEditorOptions);
        protected addRadio(value: string, text: string): void;
        get_value(): string;
        get value(): string;
        set_value(value: string): void;
        set value(v: string);
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
    }

    interface Select2CommonOptions {
        allowClear?: boolean;
        delimited?: boolean;
        minimumResultsForSearch?: any;
        multiple?: boolean;
    }
    interface Select2FilterOptions {
        cascadeFrom?: string;
        cascadeField?: string;
        cascadeValue?: any;
        filterField?: string;
        filterValue?: any;
    }
    interface Select2InplaceAddOptions {
        inplaceAdd?: boolean;
        inplaceAddPermission?: string;
        dialogType?: string;
        autoComplete?: boolean;
    }
    interface Select2EditorOptions extends Select2FilterOptions, Select2InplaceAddOptions, Select2CommonOptions {
    }
    interface Select2SearchPromise {
        abort?(): void;
        catch?(callback: () => void): void;
        fail?(callback: () => void): void;
    }
    interface Select2SearchQuery {
        searchTerm?: string;
        idList?: string[];
        skip?: number;
        take?: number;
        checkMore?: boolean;
    }
    interface Select2SearchResult<TItem> {
        items: TItem[];
        more: boolean;
    }
    class Select2Editor<TOptions, TItem> extends Widget<TOptions> implements ISetEditValue, IGetEditValue, IStringValue, IReadOnly {
        private _items;
        private _itemById;
        protected lastCreateTerm: string;
        constructor(hidden: JQuery, opt?: any);
        destroy(): void;
        protected hasAsyncSource(): boolean;
        protected asyncSearch(query: Select2SearchQuery, results: (result: Select2SearchResult<TItem>) => void): Select2SearchPromise;
        protected getTypeDelay(): any;
        protected emptyItemText(): string;
        protected getPageSize(): number;
        protected getIdField(): any;
        protected itemId(item: TItem): string;
        protected getTextField(): any;
        protected itemText(item: TItem): string;
        protected itemDisabled(item: TItem): boolean;
        protected mapItem(item: TItem): Select2Item;
        protected mapItems(items: TItem[]): Select2Item[];
        protected allowClear(): boolean;
        protected isMultiple(): boolean;
        private initSelectionPromise;
        private queryPromise;
        private typeTimeout;
        protected abortPendingQuery(): void;
        protected getSelect2Options(): Select2Options;
        get_delimited(): boolean;
        get items(): Select2Item[];
        set items(value: Select2Item[]);
        protected get itemById(): {
            [key: string]: Select2Item;
        };
        protected set itemById(value: {
            [key: string]: Select2Item;
        });
        clearItems(): void;
        addItem(item: Select2Item): void;
        addOption(key: string, text: string, source?: any, disabled?: boolean): void;
        protected addInplaceCreate(addTitle: string, editTitle: string): void;
        protected useInplaceAdd(): boolean;
        protected isAutoComplete(): boolean;
        getCreateSearchChoice(getName: (z: any) => string): (s: string) => {
            id: string;
            text: string;
        };
        setEditValue(source: any, property: Q.PropertyItem): void;
        getEditValue(property: Q.PropertyItem, target: any): void;
        protected get_select2Container(): JQuery;
        protected get_items(): Select2Item[];
        protected get_itemByKey(): {
            [key: string]: Select2Item;
        };
        static filterByText<TItem>(items: TItem[], getText: (item: TItem) => string, term: string): TItem[];
        get_value(): any;
        get value(): string;
        set_value(value: string): void;
        set value(v: string);
        get selectedItem(): TItem;
        get selectedItems(): TItem[];
        protected get_values(): string[];
        get values(): string[];
        protected set_values(value: string[]): void;
        set values(value: string[]);
        protected get_text(): string;
        get text(): string;
        get_readOnly(): boolean;
        get readOnly(): boolean;
        private updateInplaceReadOnly;
        set_readOnly(value: boolean): void;
        set readOnly(value: boolean);
        protected getCascadeFromValue(parent: Widget<any>): any;
        protected cascadeLink: CascadedWidgetLink<Widget<any>>;
        protected setCascadeFrom(value: string): void;
        protected get_cascadeFrom(): string;
        get cascadeFrom(): string;
        protected set_cascadeFrom(value: string): void;
        set cascadeFrom(value: string);
        protected get_cascadeField(): string;
        get cascadeField(): string;
        protected set_cascadeField(value: string): void;
        set cascadeField(value: string);
        protected get_cascadeValue(): any;
        get cascadeValue(): any;
        protected set_cascadeValue(value: any): void;
        set cascadeValue(value: any);
        protected get_filterField(): string;
        get filterField(): string;
        protected set_filterField(value: string): void;
        set filterField(value: string);
        protected get_filterValue(): any;
        get filterValue(): any;
        protected set_filterValue(value: any): void;
        set filterValue(value: any);
        protected cascadeItems(items: TItem[]): TItem[];
        protected filterItems(items: TItem[]): TItem[];
        protected updateItems(): void;
        protected getDialogTypeKey(): string;
        protected createEditDialog(callback: (dlg: IEditDialog) => void): void;
        onInitNewEntity: (entity: TItem) => void;
        protected initNewEntity(entity: TItem): void;
        protected setEditDialogReadOnly(dialog: any): void;
        protected editDialogDataChange(): void;
        protected setTermOnNewEntity(entity: TItem, term: string): void;
        protected inplaceCreateClick(e: JQueryEventObject): void;
        openDialogAsPanel: boolean;
    }

    class SelectEditor extends Select2Editor<SelectEditorOptions, Select2Item> {
        constructor(hidden: JQuery, opt?: SelectEditorOptions);
        getItems(): any[];
        protected emptyItemText(): string;
        updateItems(): void;
    }
    interface SelectEditorOptions extends Select2CommonOptions {
        items?: any[];
        emptyOptionText?: string;
    }

    class DateYearEditor extends SelectEditor {
        constructor(hidden: JQuery, opt: DateYearEditorOptions);
        getItems(): any[];
    }
    interface DateYearEditorOptions extends SelectEditorOptions {
        minYear?: string;
        maxYear?: string;
        descending?: boolean;
    }

    interface EnumEditorOptions extends Select2CommonOptions {
        enumKey?: string;
        enumType?: any;
    }
    class EnumEditor extends Select2Editor<EnumEditorOptions, Select2Item> {
        constructor(hidden: JQuery, opt: EnumEditorOptions);
        protected updateItems(): void;
        protected allowClear(): boolean;
    }

    interface LookupEditorOptions extends Select2EditorOptions {
        lookupKey?: string;
        async?: boolean;
    }
    abstract class LookupEditorBase<TOptions extends LookupEditorOptions, TItem> extends Select2Editor<TOptions, TItem> {
        constructor(input: JQuery, opt?: TOptions);
        hasAsyncSource(): boolean;
        destroy(): void;
        protected getLookupKey(): string;
        protected lookup: Q.Lookup<TItem>;
        protected getLookupAsync(): PromiseLike<Q.Lookup<TItem>>;
        protected getLookup(): Q.Lookup<TItem>;
        protected getItems(lookup: Q.Lookup<TItem>): TItem[];
        protected getIdField(): any;
        protected getItemText(item: TItem, lookup: Q.Lookup<TItem>): any;
        protected mapItem(item: TItem): Select2Item;
        protected getItemDisabled(item: TItem, lookup: Q.Lookup<TItem>): boolean;
        updateItems(): void;
        protected asyncSearch(query: Select2SearchQuery, results: (result: Select2SearchResult<TItem>) => void): Select2SearchPromise;
        protected getDialogTypeKey(): string;
        protected setCreateTermOnNewEntity(entity: TItem, term: string): void;
        protected editDialogDataChange(): void;
    }
    class LookupEditor extends LookupEditorBase<LookupEditorOptions, any> {
        constructor(hidden: JQuery, opt?: LookupEditorOptions);
    }

    interface ServiceLookupEditorOptions extends Select2EditorOptions {
        service?: string;
        idField: string;
        textField: string;
        pageSize?: number;
        minimumResultsForSearch?: any;
        sort: string[];
        columnSelection?: Q.ColumnSelection;
        includeColumns?: string[];
        excludeColumns?: string[];
        includeDeleted?: boolean;
        containsField?: string;
        equalityFilter?: any;
        criteria?: any[];
    }
    abstract class ServiceLookupEditorBase<TOptions extends ServiceLookupEditorOptions, TItem> extends Select2Editor<TOptions, TItem> {
        constructor(input: JQuery, opt?: TOptions);
        protected getDialogTypeKey(): string;
        protected getService(): string;
        protected getServiceUrl(): string;
        protected getIncludeColumns(): string[];
        protected getSort(): any[];
        protected getCascadeCriteria(): any[];
        protected getFilterCriteria(): any[];
        protected getIdListCriteria(idList: any[]): any[];
        protected getCriteria(query: Select2SearchQuery): any[];
        protected getListRequest(query: Select2SearchQuery): Q.ListRequest;
        protected getServiceCallOptions(query: Select2SearchQuery, results: (result: Select2SearchResult<TItem>) => void): Q.ServiceOptions<Q.ListResponse<TItem>>;
        protected hasAsyncSource(): boolean;
        protected asyncSearch(query: Select2SearchQuery, results: (result: Select2SearchResult<TItem>) => void): Select2SearchPromise;
    }
    class ServiceLookupEditor extends ServiceLookupEditorBase<ServiceLookupEditorOptions, any> {
        constructor(hidden: JQuery, opt?: ServiceLookupEditorOptions);
    }

    interface HtmlContentEditorOptions {
        cols?: any;
        rows?: any;
    }
    interface CKEditorConfig {
    }
    class HtmlContentEditor extends Widget<HtmlContentEditorOptions> implements IStringValue, IReadOnly {
        private _instanceReady;
        constructor(textArea: JQuery, opt?: HtmlContentEditorOptions);
        protected instanceReady(x: any): void;
        protected getLanguage(): string;
        protected getConfig(): CKEditorConfig;
        protected getEditorInstance(): any;
        destroy(): void;
        get_value(): string;
        get value(): string;
        set_value(value: string): void;
        set value(v: string);
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        static CKEditorVer: string;
        static CKEditorBasePath: string;
        static getCKEditorBasePath(): string;
        static includeCKEditor(): void;
    }
    class HtmlNoteContentEditor extends HtmlContentEditor {
        constructor(textArea: JQuery, opt?: HtmlContentEditorOptions);
        protected getConfig(): CKEditorConfig;
    }
    class HtmlReportContentEditor extends HtmlContentEditor {
        constructor(textArea: JQuery, opt?: HtmlContentEditorOptions);
        protected getConfig(): CKEditorConfig;
    }

    class MaskedEditor extends Widget<MaskedEditorOptions> {
        constructor(input: JQuery, opt?: MaskedEditorOptions);
        get value(): string;
        protected get_value(): string;
        set value(value: string);
        protected set_value(value: string): void;
    }
    interface MaskedEditorOptions {
        mask?: string;
        placeholder?: string;
    }

    interface RecaptchaOptions {
        siteKey?: string;
        language?: string;
    }
    class Recaptcha extends Widget<RecaptchaOptions> implements IStringValue {
        constructor(div: JQuery, opt: RecaptchaOptions);
        get_value(): string;
        set_value(value: string): void;
    }

    namespace UploadHelper {
        function addUploadInput(options: UploadInputOptions): JQuery;
        function checkImageConstraints(file: UploadResponse, opt: FileUploadConstraints): boolean;
        function fileNameSizeDisplay(name: string, bytes: number): string;
        function fileSizeDisplay(bytes: number): string;
        function hasImageExtension(filename: string): boolean;
        function thumbFileName(filename: string): string;
        function dbFileUrl(filename: string): string;
        function colorBox(link: JQuery, options: any): void;
        function populateFileSymbols(container: JQuery, items: UploadedFile[], displayOriginalName?: boolean, urlPrefix?: string): void;
    }
    interface UploadedFile {
        Filename?: string;
        OriginalName?: string;
    }
    interface UploadInputOptions {
        container?: JQuery;
        zone?: JQuery;
        progress?: JQuery;
        inputName?: string;
        allowMultiple?: boolean;
        uploadIntent?: string;
        uploadUrl?: string;
        fileDone?: (p1: UploadResponse, p2: string, p3: any) => void;
    }
    interface UploadResponse {
        TemporaryFile: string;
        Size: number;
        IsImage: boolean;
        Width: number;
        Height: number;
    }
    interface FileUploadConstraints {
        minWidth?: number;
        maxWidth?: number;
        minHeight?: number;
        maxHeight?: number;
        minSize?: number;
        maxSize?: number;
        allowNonImage?: boolean;
        originalNameProperty?: string;
    }

    interface FileUploadEditorOptions extends FileUploadConstraints {
        displayFileName?: boolean;
        uploadIntent?: string;
        uploadUrl?: string;
        urlPrefix?: string;
    }
    interface ImageUploadEditorOptions extends FileUploadEditorOptions {
    }
    class FileUploadEditor extends Widget<FileUploadEditorOptions> implements IReadOnly, IGetEditValue, ISetEditValue, IValidateRequired {
        constructor(div: JQuery, opt: FileUploadEditorOptions);
        protected getUploadInputOptions(): UploadInputOptions;
        protected addFileButtonText(): string;
        protected getToolButtons(): ToolButton[];
        protected populate(): void;
        protected updateInterface(): void;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        get_required(): boolean;
        set_required(value: boolean): void;
        get_value(): UploadedFile;
        get value(): UploadedFile;
        set_value(value: UploadedFile): void;
        set value(v: UploadedFile);
        getEditValue(property: Q.PropertyItem, target: any): void;
        setEditValue(source: any, property: Q.PropertyItem): void;
        protected entity: UploadedFile;
        protected toolbar: Toolbar;
        protected progress: JQuery;
        protected fileSymbols: JQuery;
        protected uploadInput: JQuery;
        protected hiddenInput: JQuery;
    }
    class ImageUploadEditor extends FileUploadEditor {
        constructor(div: JQuery, opt: ImageUploadEditorOptions);
    }
    class MultipleFileUploadEditor extends Widget<FileUploadEditorOptions> implements IReadOnly, IGetEditValue, ISetEditValue, IValidateRequired {
        private entities;
        private toolbar;
        private fileSymbols;
        private uploadInput;
        protected progress: JQuery;
        protected hiddenInput: JQuery;
        constructor(div: JQuery, opt: ImageUploadEditorOptions);
        protected getUploadInputOptions(): UploadInputOptions;
        protected addFileButtonText(): string;
        protected getToolButtons(): ToolButton[];
        protected populate(): void;
        protected updateInterface(): void;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        get_required(): boolean;
        set_required(value: boolean): void;
        get_value(): UploadedFile[];
        get value(): UploadedFile[];
        set_value(value: UploadedFile[]): void;
        set value(v: UploadedFile[]);
        getEditValue(property: Q.PropertyItem, target: any): void;
        setEditValue(source: any, property: Q.PropertyItem): void;
        jsonEncodeValue: boolean;
    }
    class MultipleImageUploadEditor extends MultipleFileUploadEditor {
        constructor(div: JQuery, opt: ImageUploadEditorOptions);
    }

    interface QuickFilterArgs<TWidget> {
        field?: string;
        widget?: TWidget;
        request?: Q.ListRequest;
        equalityFilter?: any;
        value?: any;
        active?: boolean;
        handled?: boolean;
    }
    interface QuickFilter<TWidget extends Widget<TOptions>, TOptions> {
        field?: string;
        type?: new (element: JQuery, options: TOptions) => TWidget;
        handler?: (h: QuickFilterArgs<TWidget>) => void;
        title?: string;
        options?: TOptions;
        element?: (e: JQuery) => void;
        init?: (w: TWidget) => void;
        separator?: boolean;
        cssClass?: string;
        loadState?: (w: TWidget, state: any) => void;
        saveState?: (w: TWidget) => any;
        displayText?: (w: TWidget, label: string) => string;
    }

    interface QuickFilterBarOptions {
        filters: QuickFilter<Widget<any>, any>[];
        getTitle?: (filter: QuickFilter<Widget<any>, any>) => string;
        idPrefix?: string;
    }
    class QuickFilterBar extends Widget<QuickFilterBarOptions> {
        constructor(container: JQuery, options?: QuickFilterBarOptions);
        addSeparator(): void;
        add<TWidget extends Widget<any>, TOptions>(opt: QuickFilter<TWidget, TOptions>): TWidget;
        addDateRange(field: string, title?: string): DateEditor;
        static dateRange(field: string, title?: string): QuickFilter<DateEditor, DateTimeEditorOptions>;
        addDateTimeRange(field: string, title?: string): DateTimeEditor;
        static dateTimeRange(field: string, title?: string, useUtc?: boolean): QuickFilter<DateTimeEditor, DateTimeEditorOptions>;
        addBoolean(field: string, title?: string, yes?: string, no?: string): SelectEditor;
        static boolean(field: string, title?: string, yes?: string, no?: string): QuickFilter<SelectEditor, SelectEditorOptions>;
        onChange: (e: JQueryEventObject) => void;
        private submitHandlers;
        destroy(): void;
        onSubmit(request: Q.ListRequest): void;
        protected add_submitHandlers(action: (request: Q.ListRequest) => void): void;
        protected remove_submitHandlers(action: (request: Q.ListRequest) => void): void;
        protected clear_submitHandlers(): void;
        find<TWidget>(type: {
            new (...args: any[]): TWidget;
        }, field: string): TWidget;
        tryFind<TWidget>(type: {
            new (...args: any[]): TWidget;
        }, field: string): TWidget;
    }

    interface QuickSearchField {
        name: string;
        title: string;
    }
    interface QuickSearchInputOptions {
        typeDelay?: number;
        loadingParentClass?: string;
        filteredParentClass?: string;
        onSearch?: (p1: string, p2: string, p3: (p1: boolean) => void) => void;
        fields?: QuickSearchField[];
    }
    class QuickSearchInput extends Widget<QuickSearchInputOptions> {
        private lastValue;
        private field;
        private fieldChanged;
        private timer;
        constructor(input: JQuery, opt: QuickSearchInputOptions);
        protected checkIfValueChanged(): void;
        get_value(): string;
        get_field(): QuickSearchField;
        set_field(value: QuickSearchField): void;
        protected updateInputPlaceHolder(): void;
        restoreState(value: string, field: QuickSearchField): void;
        protected searchNow(value: string): void;
    }

    interface FilterOperator {
        key?: string;
        title?: string;
        format?: string;
    }
    namespace FilterOperators {
        const isTrue = "true";
        const isFalse = "false";
        const contains = "contains";
        const startsWith = "startswith";
        const EQ = "eq";
        const NE = "ne";
        const GT = "gt";
        const GE = "ge";
        const LT = "lt";
        const LE = "le";
        const BW = "bw";
        const IN = "in";
        const isNull = "isnull";
        const isNotNull = "isnotnull";
        const toCriteriaOperator: {
            [key: string]: string;
        };
    }

    interface FilterLine {
        field?: string;
        operator?: string;
        isOr?: boolean;
        leftParen?: boolean;
        rightParen?: boolean;
        validationError?: string;
        criteria?: any[];
        displayText?: string;
        state?: any;
    }

    class FilterStore {
        constructor(fields: Q.PropertyItem[]);
        static getCriteriaFor(items: FilterLine[]): any[];
        static getDisplayTextFor(items: FilterLine[]): string;
        private changed;
        private displayText;
        private fields;
        private fieldByName;
        private items;
        get_fields(): Q.PropertyItem[];
        get_fieldByName(): {
            [key: string]: Q.PropertyItem;
        };
        get_items(): FilterLine[];
        raiseChanged(): void;
        add_changed(value: (e: JQueryEventObject, a: any) => void): void;
        remove_changed(value: (e: JQueryEventObject, a: any) => void): void;
        get_activeCriteria(): any[];
        get_displayText(): string;
    }

    interface IFiltering {
        createEditor(): void;
        getCriteria(): CriteriaWithText;
        getOperators(): FilterOperator[];
        loadState(state: any): void;
        saveState(): any;
        get_field(): Q.PropertyItem;
        set_field(value: Q.PropertyItem): void;
        get_container(): JQuery;
        set_container(value: JQuery): void;
        get_operator(): FilterOperator;
        set_operator(value: FilterOperator): void;
    }
    class IFiltering {
    }
    interface CriteriaWithText {
        criteria?: any[];
        displayText?: string;
    }
    interface IQuickFiltering {
        initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
    }
    class IQuickFiltering {
    }
    abstract class BaseFiltering implements IFiltering, IQuickFiltering {
        private field;
        get_field(): Q.PropertyItem;
        set_field(value: Q.PropertyItem): void;
        private container;
        get_container(): JQuery;
        set_container(value: JQuery): void;
        private operator;
        get_operator(): FilterOperator;
        set_operator(value: FilterOperator): void;
        abstract getOperators(): FilterOperator[];
        protected appendNullableOperators(list: FilterOperator[]): FilterOperator[];
        protected appendComparisonOperators(list: FilterOperator[]): FilterOperator[];
        protected isNullable(): boolean;
        createEditor(): void;
        protected operatorFormat(op: FilterOperator): string;
        protected getTitle(field: Q.PropertyItem): string;
        protected displayText(op: FilterOperator, values?: any[]): string;
        protected getCriteriaField(): string;
        getCriteria(): CriteriaWithText;
        loadState(state: any): void;
        saveState(): any;
        protected argumentNull(): Q.ArgumentNullException;
        validateEditorValue(value: string): string;
        getEditorValue(): string;
        getEditorText(): any;
        initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
    }
    abstract class BaseEditorFiltering<TEditor extends Widget<any>> extends BaseFiltering {
        editorType: any;
        constructor(editorType: any);
        protected useEditor(): boolean;
        protected editor: TEditor;
        createEditor(): void;
        protected useIdField(): boolean;
        getCriteriaField(): string;
        getEditorOptions(): any;
        loadState(state: any): void;
        saveState(): any;
        getEditorValue(): any;
        initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
    }
    class DateFiltering extends BaseEditorFiltering<DateEditor> {
        constructor();
        getOperators(): FilterOperator[];
    }
    class BooleanFiltering extends BaseFiltering {
        getOperators(): FilterOperator[];
    }
    class DateTimeFiltering extends BaseEditorFiltering<DateEditor> {
        constructor();
        getOperators(): FilterOperator[];
        getCriteria(): CriteriaWithText;
    }
    class DecimalFiltering extends BaseEditorFiltering<DecimalEditor> {
        constructor();
        getOperators(): FilterOperator[];
    }
    class EditorFiltering extends BaseEditorFiltering<Widget<any>> {
        constructor();
        editorType: string;
        useRelative: boolean;
        useLike: boolean;
        getOperators(): FilterOperator[];
        protected useEditor(): boolean;
        getEditorOptions(): any;
        createEditor(): void;
        protected useIdField(): boolean;
        initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
    }
    class EnumFiltering extends BaseEditorFiltering<EnumEditor> {
        constructor();
        getOperators(): FilterOperator[];
    }
    class IntegerFiltering extends BaseEditorFiltering<IntegerEditor> {
        constructor();
        getOperators(): FilterOperator[];
    }
    class LookupFiltering extends BaseEditorFiltering<LookupEditor> {
        constructor();
        getOperators(): FilterOperator[];
        protected useEditor(): boolean;
        protected useIdField(): boolean;
        getEditorText(): string;
    }
    class ServiceLookupFiltering extends BaseEditorFiltering<ServiceLookupEditor> {
        constructor();
        getOperators(): FilterOperator[];
        protected useEditor(): boolean;
        protected useIdField(): boolean;
        getEditorText(): string;
    }
    class StringFiltering extends BaseFiltering {
        getOperators(): FilterOperator[];
        validateEditorValue(value: string): string;
    }
    namespace FilteringTypeRegistry {
        function get(key: string): Function;
    }

    class FilterWidgetBase<TOptions> extends TemplatedWidget<TOptions> {
        private store;
        private onFilterStoreChanged;
        constructor(div: JQuery, opt?: TOptions);
        destroy(): void;
        protected filterStoreChanged(): void;
        get_store(): FilterStore;
        set_store(value: FilterStore): void;
    }

    class FilterPanel extends FilterWidgetBase<any> {
        private rowsDiv;
        constructor(div: JQuery);
        private showInitialLine;
        get_showInitialLine(): boolean;
        set_showInitialLine(value: boolean): void;
        protected filterStoreChanged(): void;
        updateRowsFromStore(): void;
        private showSearchButton;
        get_showSearchButton(): boolean;
        set_showSearchButton(value: boolean): void;
        private updateStoreOnReset;
        get_updateStoreOnReset(): boolean;
        set_updateStoreOnReset(value: boolean): void;
        protected getTemplate(): string;
        protected initButtons(): void;
        protected searchButtonClick(e: JQueryEventObject): void;
        get_hasErrors(): boolean;
        search(): void;
        protected addButtonClick(e: JQueryEventObject): void;
        protected resetButtonClick(e: JQueryEventObject): void;
        protected findEmptyRow(): JQuery;
        protected addEmptyRow(popupField: boolean): JQuery;
        protected onRowFieldChange(e: JQueryEventObject): void;
        protected rowFieldChange(row: JQuery): void;
        protected removeFiltering(row: JQuery): void;
        protected populateOperatorList(row: JQuery): void;
        protected getFieldFor(row: JQuery): Q.PropertyItem;
        protected getFilteringFor(row: JQuery): IFiltering;
        protected onRowOperatorChange(e: JQueryEventObject): void;
        protected rowOperatorChange(row: JQuery): void;
        protected deleteRowClick(e: JQueryEventObject): void;
        protected updateButtons(): void;
        protected andOrClick(e: JQueryEventObject): void;
        protected leftRightParenClick(e: JQueryEventObject): void;
        protected updateParens(): void;
    }

    class FilterDialog extends TemplatedDialog<any> {
        private filterPanel;
        constructor();
        get_filterPanel(): FilterPanel;
        protected getTemplate(): string;
        protected getDialogButtons(): {
            text: string;
            click: () => void;
        }[];
    }

    class FilterDisplayBar extends FilterWidgetBase<any> {
        constructor(div: JQuery);
        protected filterStoreChanged(): void;
        protected getTemplate(): string;
    }

    class SlickPager extends Widget<Slick.PagerOptions> {
        constructor(div: JQuery, o: Slick.PagerOptions);
        _changePage(ctype: string): boolean;
        _updatePager(): void;
    }

    interface IDataGrid {
        getElement(): JQuery;
        getGrid(): Slick.Grid;
        getView(): Slick.RemoteView<any>;
        getFilterStore(): FilterStore;
    }

    interface GridRowSelectionMixinOptions {
        selectable?: (item: any) => boolean;
    }
    class GridRowSelectionMixin {
        private idField;
        private include;
        private grid;
        private options;
        constructor(grid: IDataGrid, options?: GridRowSelectionMixinOptions);
        updateSelectAll(): void;
        clear(): void;
        resetCheckedAndRefresh(): void;
        selectKeys(keys: string[]): void;
        getSelectedKeys(): string[];
        getSelectedAsInt32(): number[];
        getSelectedAsInt64(): number[];
        setSelectedKeys(keys: string[]): void;
        private isSelectable;
        static createSelectColumn(getMixin: () => GridRowSelectionMixin): Slick.Column;
    }
    interface GridRadioSelectionMixinOptions {
        selectable?: (item: any) => boolean;
    }
    class GridRadioSelectionMixin {
        private idField;
        private include;
        private grid;
        private options;
        constructor(grid: IDataGrid, options?: GridRadioSelectionMixinOptions);
        private isSelectable;
        clear(): void;
        resetCheckedAndRefresh(): void;
        getSelectedKey(): string;
        getSelectedAsInt32(): number;
        getSelectedAsInt64(): number;
        setSelectedKey(key: string): void;
        static createSelectColumn(getMixin: () => GridRadioSelectionMixin): Slick.Column;
    }
    namespace GridSelectAllButtonHelper {
        function update(grid: IDataGrid, getSelected: (p1: any) => boolean): void;
        function define(getGrid: () => IDataGrid, getId: (p1: any) => any, getSelected: (p1: any) => boolean, setSelected: (p1: any, p2: boolean) => void, text?: string, onClick?: () => void): ToolButton;
    }
    namespace GridUtils {
        function addToggleButton(toolDiv: JQuery, cssClass: string, callback: (p1: boolean) => void, hint: string, initial?: boolean): void;
        function addIncludeDeletedToggle(toolDiv: JQuery, view: Slick.RemoteView<any>, hint?: string, initial?: boolean): void;
        function addQuickSearchInput(toolDiv: JQuery, view: Slick.RemoteView<any>, fields?: QuickSearchField[], onChange?: () => void): void;
        function addQuickSearchInputCustom(container: JQuery, onSearch: (p1: string, p2: string, done: (p3: boolean) => void) => void, fields?: QuickSearchField[]): QuickSearchInput;
        function makeOrderable(grid: Slick.Grid, handleMove: (p1: any, p2: number) => void): void;
        function makeOrderableWithUpdateRequest(grid: IDataGrid, getId: (p1: any) => number, getDisplayOrder: (p1: any) => any, service: string, getUpdateRequest: (p1: number, p2: number) => Q.SaveRequest<any>): void;
    }
    namespace PropertyItemSlickConverter {
        function toSlickColumns(items: Q.PropertyItem[]): Slick.Column[];
        function toSlickColumn(item: Q.PropertyItem): Slick.Column;
    }
    namespace SlickFormatting {
        function getEnumText(enumKey: string, name: string): string;
        function treeToggle(getView: () => Slick.RemoteView<any>, getId: (x: any) => any, formatter: Slick.Format): Slick.Format;
        function date(format?: string): Slick.Format;
        function dateTime(format?: string): Slick.Format;
        function checkBox(): Slick.Format;
        function number(format: string): Slick.Format;
        function getItemType(link: JQuery): string;
        function getItemId(link: JQuery): string;
        function itemLinkText(itemType: string, id: any, text: any, extraClass: string, encode: boolean): string;
        function itemLink<TItem = any>(itemType: string, idField: string, getText: Slick.Format<TItem>, cssClass?: Slick.Format<TItem>, encode?: boolean): Slick.Format<TItem>;
    }
    namespace SlickHelper {
        function setDefaults(columns: Slick.Column[], localTextPrefix?: string): any;
    }
    namespace SlickTreeHelper {
        function filterCustom<TItem>(item: TItem, getParent: (x: TItem) => any): boolean;
        function filterById<TItem>(item: TItem, view: Slick.RemoteView<TItem>, getParentId: (x: TItem) => any): boolean;
        function setCollapsed<TItem>(items: TItem[], collapsed: boolean): void;
        function setCollapsedFlag<TItem>(item: TItem, collapsed: boolean): void;
        function setIndents<TItem>(items: TItem[], getId: (x: TItem) => any, getParentId: (x: TItem) => any, setCollapsed?: boolean): void;
        function toggleClick<TItem>(e: JQueryEventObject, row: number, cell: number, view: Slick.RemoteView<TItem>, getId: (x: TItem) => any): void;
    }

    interface IInitializeColumn {
        initializeColumn(column: Slick.Column): void;
    }
    class IInitializeColumn {
    }
    class BooleanFormatter implements Formatter {
        format(ctx: Slick.FormatterContext): string;
        falseText: string;
        trueText: string;
    }
    class CheckboxFormatter implements Formatter {
        format(ctx: Slick.FormatterContext): string;
    }
    class DateFormatter implements Formatter {
        constructor();
        static format(value: any, format?: string): any;
        displayFormat: string;
        format(ctx: Slick.FormatterContext): string;
    }
    class DateTimeFormatter extends DateFormatter {
        constructor();
    }
    class EnumFormatter implements Formatter {
        format(ctx: Slick.FormatterContext): string;
        enumKey: string;
        static format(enumType: any, value: any): string;
        static getText(enumKey: string, name: string): string;
        static getName(enumType: any, value: any): string;
    }
    class FileDownloadFormatter implements Formatter, IInitializeColumn {
        format(ctx: Slick.FormatterContext): string;
        static dbFileUrl(filename: string): string;
        initializeColumn(column: Slick.Column): void;
        displayFormat: string;
        originalNameProperty: string;
        iconClass: string;
    }
    class MinuteFormatter implements Formatter {
        format(ctx: Slick.FormatterContext): string;
        static format(value: number): string;
    }
    class NumberFormatter {
        format(ctx: Slick.FormatterContext): string;
        static format(value: any, format?: string): string;
        displayFormat: string;
    }
    class UrlFormatter implements Formatter, IInitializeColumn {
        format(ctx: Slick.FormatterContext): string;
        initializeColumn(column: Slick.Column): void;
        displayProperty: string;
        displayFormat: string;
        urlProperty: string;
        urlFormat: string;
        target: string;
    }

    namespace FormatterTypeRegistry {
        function get(key: string): any;
        function reset(): void;
        function tryGet(key: string): any;
    }

    type GroupItemMetadataProviderType = typeof GroupItemMetadataProvider;

    interface SettingStorage {
        getItem(key: string): string;
        setItem(key: string, value: string): void;
    }
    interface PersistedGridColumn {
        id: string;
        width?: number;
        sort?: number;
        visible?: boolean;
    }
    interface PersistedGridSettings {
        columns?: PersistedGridColumn[];
        filterItems?: FilterLine[];
        quickFilters?: {
            [key: string]: any;
        };
        quickFilterText?: string;
        quickSearchField?: QuickSearchField;
        quickSearchText?: string;
        includeDeleted?: boolean;
    }
    interface GridPersistanceFlags {
        columnWidths?: boolean;
        columnVisibility?: boolean;
        sortColumns?: boolean;
        filterItems?: boolean;
        quickFilters?: boolean;
        quickFilterText?: boolean;
        quickSearch?: boolean;
        includeDeleted?: boolean;
    }
    class DataGrid<TItem, TOptions> extends Widget<TOptions> implements IDataGrid, IReadOnly {
        private _isDisabled;
        private _layoutTimer;
        private _slickGridOnSort;
        private _slickGridOnClick;
        protected titleDiv: JQuery;
        protected toolbar: Toolbar;
        protected filterBar: FilterDisplayBar;
        protected quickFiltersDiv: JQuery;
        protected quickFiltersBar: QuickFilterBar;
        protected slickContainer: JQuery;
        protected allColumns: Slick.Column[];
        protected propertyItemsData: Q.PropertyItemsData;
        protected initialSettings: PersistedGridSettings;
        protected restoringSettings: number;
        view: Slick.RemoteView<TItem>;
        slickGrid: Slick.Grid;
        openDialogsAsPanel: boolean;
        static defaultRowHeight: number;
        static defaultHeaderHeight: number;
        static defaultPersistanceStorage: SettingStorage;
        static defaultColumnWidthScale: number;
        static defaultColumnWidthDelta: number;
        constructor(container: JQuery, options?: TOptions);
        protected internalInit(): void;
        protected initSync(): void;
        protected initAsync(): Promise<void>;
        protected afterInit(): void;
        protected useAsync(): boolean;
        protected useLayoutTimer(): boolean;
        protected attrs<TAttr>(attrType: {
            new (...args: any[]): TAttr;
        }): TAttr[];
        protected layout(): void;
        protected getInitialTitle(): string;
        protected createToolbarExtensions(): void;
        protected ensureQuickFilterBar(): QuickFilterBar;
        protected createQuickFilters(filters?: QuickFilter<Widget<any>, any>[]): void;
        protected getQuickFilters(): QuickFilter<Widget<any>, any>[];
        static propertyItemToQuickFilter(item: Q.PropertyItem): any;
        protected findQuickFilter<TWidget>(type: {
            new (...args: any[]): TWidget;
        }, field: string): TWidget;
        protected tryFindQuickFilter<TWidget>(type: {
            new (...args: any[]): TWidget;
        }, field: string): TWidget;
        protected createIncludeDeletedButton(): void;
        protected getQuickSearchFields(): QuickSearchField[];
        protected createQuickSearchInput(): void;
        destroy(): void;
        protected getItemCssClass(item: TItem, index: number): string;
        protected getItemMetadata(item: TItem, index: number): any;
        protected postProcessColumns(columns: Slick.Column[]): Slick.Column[];
        protected getColumnWidthDelta(): number;
        protected getColumnWidthScale(): number;
        protected initialPopulate(): void;
        protected canFilterColumn(column: Slick.Column): boolean;
        protected initializeFilterBar(): void;
        protected createSlickGrid(): Slick.Grid;
        protected setInitialSortOrder(): void;
        itemAt(row: number): TItem;
        rowCount(): number;
        getItems(): TItem[];
        setItems(value: TItem[]): void;
        protected bindToSlickEvents(): void;
        protected getAddButtonCaption(): string;
        protected getButtons(): ToolButton[];
        protected editItem(entityOrId: any): void;
        protected editItemOfType(itemType: string, entityOrId: any): void;
        protected onClick(e: JQueryEventObject, row: number, cell: number): void;
        protected viewDataChanged(e: any, rows: TItem[]): void;
        protected bindToViewEvents(): void;
        protected onViewProcessData(response: Q.ListResponse<TItem>): Q.ListResponse<TItem>;
        protected onViewFilter(item: TItem): boolean;
        protected getIncludeColumns(include: {
            [key: string]: boolean;
        }): void;
        protected setCriteriaParameter(): void;
        protected setEquality(field: string, value: any): void;
        protected setIncludeColumnsParameter(): void;
        protected onViewSubmit(): boolean;
        protected markupReady(): void;
        protected createSlickContainer(): JQuery;
        protected createView(): Slick.RemoteView<TItem>;
        protected getDefaultSortBy(): any[];
        protected usePager(): boolean;
        protected enableFiltering(): boolean;
        protected populateWhenVisible(): boolean;
        protected createFilterBar(): void;
        protected getPagerOptions(): Slick.PagerOptions;
        protected createPager(): void;
        protected getViewOptions(): Slick.RemoteViewOptions;
        protected createToolbar(buttons: ToolButton[]): void;
        getTitle(): string;
        setTitle(value: string): void;
        protected getItemType(): string;
        protected itemLink(itemType?: string, idField?: string, text?: (ctx: Slick.FormatterContext) => string, cssClass?: (ctx: Slick.FormatterContext) => string, encode?: boolean): Slick.Format<TItem>;
        protected getColumnsKey(): string;
        protected getPropertyItems(): Q.PropertyItem[];
        protected getPropertyItemsData(): Q.PropertyItemsData;
        protected getPropertyItemsDataAsync(): Promise<Q.PropertyItemsData>;
        protected getColumns(): Slick.Column[];
        protected propertyItemsToSlickColumns(propertyItems: Q.PropertyItem[]): Slick.Column[];
        protected getSlickOptions(): Slick.GridOptions;
        protected populateLock(): void;
        protected populateUnlock(): void;
        protected getGridCanLoad(): boolean;
        refresh(): void;
        protected refreshIfNeeded(): void;
        protected internalRefresh(): void;
        setIsDisabled(value: boolean): void;
        private _readonly;
        get readOnly(): boolean;
        set readOnly(value: boolean);
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        protected updateInterface(): void;
        protected getRowDefinition(): IRowDefinition;
        private _localTextDbPrefix;
        protected getLocalTextDbPrefix(): string;
        protected getLocalTextPrefix(): string;
        private _idProperty;
        protected getIdProperty(): string;
        protected getIsDeletedProperty(): string;
        private _isActiveProperty;
        protected getIsActiveProperty(): string;
        protected updateDisabledState(): void;
        protected resizeCanvas(): void;
        protected subDialogDataChange(): void;
        protected addFilterSeparator(): void;
        protected determineText(getKey: (prefix: string) => string): string;
        protected addQuickFilter<TWidget extends Widget<any>, TOptions>(opt: QuickFilter<TWidget, TOptions>): TWidget;
        protected addDateRangeFilter(field: string, title?: string): DateEditor;
        protected dateRangeQuickFilter(field: string, title?: string): QuickFilter<DateEditor, DateTimeEditorOptions>;
        protected addDateTimeRangeFilter(field: string, title?: string): DateTimeEditor;
        protected dateTimeRangeQuickFilter(field: string, title?: string): QuickFilter<DateTimeEditor, DateTimeEditorOptions>;
        protected addBooleanFilter(field: string, title?: string, yes?: string, no?: string): SelectEditor;
        protected booleanQuickFilter(field: string, title?: string, yes?: string, no?: string): QuickFilter<SelectEditor, SelectEditorOptions>;
        protected invokeSubmitHandlers(): void;
        protected quickFilterChange(e: JQueryEventObject): void;
        protected getPersistanceStorage(): SettingStorage;
        protected getPersistanceKey(): string;
        protected gridPersistanceFlags(): GridPersistanceFlags;
        protected canShowColumn(column: Slick.Column): boolean;
        protected getPersistedSettings(): PersistedGridSettings;
        protected restoreSettings(settings?: PersistedGridSettings, flags?: GridPersistanceFlags): void;
        protected persistSettings(flags?: GridPersistanceFlags): void;
        protected getCurrentSettings(flags?: GridPersistanceFlags): PersistedGridSettings;
        getElement(): JQuery;
        getGrid(): Slick.Grid;
        getView(): Slick.RemoteView<TItem>;
        getFilterStore(): FilterStore;
    }

    class ColumnPickerDialog extends TemplatedDialog<any> {
        private ulVisible;
        private ulHidden;
        private colById;
        allColumns: Slick.Column[];
        visibleColumns: string[];
        defaultColumns: string[];
        done: () => void;
        constructor();
        static createToolButton(grid: IDataGrid): ToolButton;
        protected getDialogButtons(): {
            text: string;
            click: () => void;
        }[];
        protected getDialogOptions(): any;
        private getTitle;
        private allowHide;
        private createLI;
        private updateListStates;
        protected setupColumns(): void;
        protected onDialogOpen(): void;
        protected getTemplate(): string;
    }

    /**
     * A mixin that can be applied to a DataGrid for tree functionality
     */
    class TreeGridMixin<TItem> {
        private options;
        private dataGrid;
        private getId;
        constructor(options: TreeGridMixinOptions<TItem>);
        /**
         * Expands / collapses all rows in a grid automatically
         */
        toggleAll(): void;
        collapseAll(): void;
        expandAll(): void;
        /**
         * Reorders a set of items so that parents comes before their children.
         * This method is required for proper tree ordering, as it is not so easy to perform with SQL.
         * @param items list of items to be ordered
         * @param getId a delegate to get ID of a record (must return same ID with grid identity field)
         * @param getParentId a delegate to get parent ID of a record
         */
        static applyTreeOrdering<TItem>(items: TItem[], getId: (item: TItem) => any, getParentId: (item: TItem) => any): TItem[];
    }
    interface TreeGridMixinOptions<TItem> {
        grid: DataGrid<TItem, any>;
        getParentId: (item: TItem) => any;
        toggleField: string;
        initialCollapse?: () => boolean;
    }

    interface CheckTreeItem<TSource> {
        isSelected?: boolean;
        hideCheckBox?: boolean;
        isAllDescendantsSelected?: boolean;
        id?: string;
        text?: string;
        parentId?: string;
        children?: CheckTreeItem<TSource>[];
        source?: TSource;
    }
    class CheckTreeEditor<TItem extends CheckTreeItem<any>, TOptions> extends DataGrid<TItem, TOptions> implements IGetEditValue, ISetEditValue, IReadOnly {
        private byId;
        constructor(div: JQuery, opt?: TOptions);
        protected getIdProperty(): string;
        protected getTreeItems(): TItem[];
        protected updateItems(): void;
        getEditValue(property: Q.PropertyItem, target: any): void;
        setEditValue(source: any, property: Q.PropertyItem): void;
        protected getButtons(): ToolButton[];
        protected itemSelectedChanged(item: TItem): void;
        protected getSelectAllText(): string;
        protected isThreeStateHierarchy(): boolean;
        protected createSlickGrid(): Slick.Grid;
        protected onViewFilter(item: TItem): boolean;
        protected getInitialCollapse(): boolean;
        protected onViewProcessData(response: Q.ListResponse<TItem>): Q.ListResponse<TItem>;
        protected onClick(e: JQueryEventObject, row: number, cell: number): void;
        protected updateSelectAll(): void;
        protected updateFlags(): void;
        protected getDescendantsSelected(item: TItem): boolean;
        protected setAllSubTreeSelected(item: TItem, selected: boolean): boolean;
        protected allItemsSelected(): boolean;
        protected allDescendantsSelected(item: TItem): boolean;
        protected getDelimited(): boolean;
        protected anyDescendantsSelected(item: TItem): boolean;
        protected getColumns(): Slick.Column[];
        protected getItemText(ctx: Slick.FormatterContext): string;
        protected getSlickOptions(): Slick.GridOptions;
        protected sortItems(): void;
        protected moveSelectedUp(): boolean;
        private _readOnly;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        private get_value;
        get value(): string[];
        private set_value;
        set value(v: string[]);
    }
    interface CheckLookupEditorOptions {
        lookupKey?: string;
        checkedOnTop?: boolean;
        showSelectAll?: boolean;
        hideSearch?: boolean;
        delimited?: boolean;
        cascadeFrom?: string;
        cascadeField?: string;
        cascadeValue?: any;
        filterField?: string;
        filterValue?: any;
    }
    class CheckLookupEditor<TItem = any> extends CheckTreeEditor<CheckTreeItem<TItem>, CheckLookupEditorOptions> {
        private searchText;
        private enableUpdateItems;
        constructor(div: JQuery, options: CheckLookupEditorOptions);
        protected updateItems(): void;
        protected getLookupKey(): string;
        protected getButtons(): ToolButton[];
        protected createToolbarExtensions(): void;
        protected getSelectAllText(): string;
        protected cascadeItems(items: TItem[]): TItem[];
        protected filterItems(items: TItem[]): TItem[];
        protected getLookupItems(lookup: Q.Lookup<TItem>): TItem[];
        protected getTreeItems(): CheckTreeItem<TItem>[];
        protected onViewFilter(item: CheckTreeItem<TItem>): boolean;
        protected moveSelectedUp(): boolean;
        protected get_cascadeFrom(): string;
        get cascadeFrom(): string;
        protected getCascadeFromValue(parent: Widget<any>): any;
        protected cascadeLink: CascadedWidgetLink<Widget<any>>;
        protected setCascadeFrom(value: string): void;
        protected set_cascadeFrom(value: string): void;
        set cascadeFrom(value: string);
        protected get_cascadeField(): string;
        get cascadeField(): string;
        protected set_cascadeField(value: string): void;
        set cascadeField(value: string);
        protected get_cascadeValue(): any;
        get cascadeValue(): any;
        protected set_cascadeValue(value: any): void;
        set cascadeValue(value: any);
        protected get_filterField(): string;
        get filterField(): string;
        protected set_filterField(value: string): void;
        set filterField(value: string);
        protected get_filterValue(): any;
        get filterValue(): any;
        protected set_filterValue(value: any): void;
        set filterValue(value: any);
    }

    class EntityGrid<TItem, TOptions> extends DataGrid<TItem, TOptions> {
        constructor(container: JQuery, options?: TOptions);
        protected handleRoute(args: Q.HandleRouteEventArgs): void;
        protected usePager(): boolean;
        protected createToolbarExtensions(): void;
        protected getInitialTitle(): string;
        protected getLocalTextPrefix(): string;
        private _entityType;
        protected getEntityType(): string;
        private _displayName;
        protected getDisplayName(): string;
        private _itemName;
        protected getItemName(): string;
        protected getAddButtonCaption(): string;
        protected getButtons(): ToolButton[];
        protected newRefreshButton(noText?: boolean): ToolButton;
        protected addButtonClick(): void;
        protected editItem(entityOrId: any): void;
        protected editItemOfType(itemType: string, entityOrId: any): void;
        private _service;
        protected getService(): string;
        protected getViewOptions(): Slick.RemoteViewOptions;
        protected getItemType(): string;
        protected routeDialog(itemType: string, dialog: Widget<any>): void;
        protected getInsertPermission(): string;
        protected hasInsertPermission(): boolean;
        protected transferDialogReadOnly(dialog: Widget<any>): void;
        protected initDialog(dialog: Widget<any>): void;
        protected initEntityDialog(itemType: string, dialog: Widget<any>): void;
        protected createEntityDialog(itemType: string, callback?: (dlg: Widget<any>) => void): Widget<any>;
        protected getDialogOptions(): any;
        protected getDialogOptionsFor(itemType: string): any;
        protected getDialogTypeFor(itemType: string): {
            new (...args: any[]): Widget<any>;
        };
        private _dialogType;
        protected getDialogType(): {
            new (...args: any[]): Widget<any>;
        };
    }

    class EntityDialog<TItem, TOptions> extends TemplatedDialog<TOptions> implements IEditDialog, IReadOnly {
        protected entity: TItem;
        protected entityId: any;
        protected propertyItemsData: Q.PropertyItemsData;
        protected propertyGrid: PropertyGrid;
        protected toolbar: Toolbar;
        protected saveAndCloseButton: JQuery;
        protected applyChangesButton: JQuery;
        protected deleteButton: JQuery;
        protected undeleteButton: JQuery;
        protected cloneButton: JQuery;
        protected editButton: JQuery;
        protected localizationGrid: PropertyGrid;
        protected localizationButton: JQuery;
        protected localizationPendingValue: any;
        protected localizationLastValue: any;
        static defaultLanguageList: () => string[][];
        constructor(opt?: TOptions);
        internalInit(): void;
        protected initSync(): void;
        protected initAsync(): Promise<void>;
        protected afterInit(): void;
        protected useAsync(): boolean;
        destroy(): void;
        protected get_entity(): TItem;
        protected set_entity(entity: any): void;
        protected get_entityId(): any;
        protected set_entityId(value: any): void;
        protected getEntityNameFieldValue(): any;
        protected getEntityTitle(): string;
        protected updateTitle(): void;
        protected isCloneMode(): boolean;
        protected isEditMode(): boolean;
        protected isDeleted(): boolean;
        protected isNew(): boolean;
        protected isNewOrDeleted(): boolean;
        protected getDeleteOptions(callback: (response: Q.DeleteResponse) => void): Q.ServiceOptions<Q.DeleteResponse>;
        protected deleteHandler(options: Q.ServiceOptions<Q.DeleteResponse>, callback: (response: Q.DeleteResponse) => void): void;
        protected doDelete(callback: (response: Q.DeleteResponse) => void): void;
        protected onDeleteSuccess(response: Q.DeleteResponse): void;
        protected attrs<TAttr>(attrType: {
            new (...args: any[]): TAttr;
        }): TAttr[];
        protected getRowDefinition(): IRowDefinition;
        private _entityType;
        protected getEntityType(): string;
        private _formKey;
        protected getFormKey(): string;
        private _localTextDbPrefix;
        protected getLocalTextDbPrefix(): string;
        protected getLocalTextPrefix(): string;
        private _entitySingular;
        protected getEntitySingular(): string;
        private _nameProperty;
        protected getNameProperty(): string;
        private _idProperty;
        protected getIdProperty(): string;
        private _isActiveProperty;
        protected getIsActiveProperty(): string;
        protected getIsDeletedProperty(): string;
        private _service;
        protected getService(): string;
        load(entityOrId: any, done: () => void, fail?: (ex: Q.Exception) => void): void;
        loadNewAndOpenDialog(asPanel?: boolean): void;
        loadEntityAndOpenDialog(entity: TItem, asPanel?: boolean): void;
        protected loadResponse(data: any): void;
        protected loadEntity(entity: TItem): void;
        protected beforeLoadEntity(entity: TItem): void;
        protected afterLoadEntity(): void;
        loadByIdAndOpenDialog(entityId: any, asPanel?: boolean): void;
        protected onLoadingData(data: Q.RetrieveResponse<TItem>): void;
        protected getLoadByIdOptions(id: any, callback: (response: Q.RetrieveResponse<TItem>) => void): Q.ServiceOptions<Q.RetrieveResponse<TItem>>;
        protected getLoadByIdRequest(id: any): Q.RetrieveRequest;
        protected reloadById(): void;
        loadById(id: any, callback?: (response: Q.RetrieveResponse<TItem>) => void, fail?: () => void): void;
        protected loadByIdHandler(options: Q.ServiceOptions<Q.RetrieveResponse<TItem>>, callback: (response: Q.RetrieveResponse<TItem>) => void, fail: () => void): void;
        protected initLocalizationGrid(): void;
        protected initLocalizationGridCommon(pgOptions: PropertyGridOptions): void;
        protected isLocalizationMode(): boolean;
        protected isLocalizationModeAndChanged(): boolean;
        protected localizationButtonClick(): void;
        protected getLanguages(): any[];
        private getLangs;
        protected loadLocalization(): void;
        protected setLocalizationGridCurrentValues(): void;
        protected getLocalizationGridValue(): any;
        protected getPendingLocalizations(): any;
        protected initPropertyGrid(): void;
        protected getPropertyItems(): Q.PropertyItem[];
        protected getPropertyItemsData(): Q.PropertyItemsData;
        protected getPropertyItemsDataAsync(): Promise<Q.PropertyItemsData>;
        protected getPropertyGridOptions(): PropertyGridOptions;
        protected validateBeforeSave(): boolean;
        protected getSaveOptions(callback: (response: Q.SaveResponse) => void): Q.ServiceOptions<Q.SaveResponse>;
        protected getSaveEntity(): TItem;
        protected getSaveRequest(): Q.SaveRequest<TItem>;
        protected onSaveSuccess(response: Q.SaveResponse): void;
        protected save_submitHandler(callback: (response: Q.SaveResponse) => void): void;
        protected save(callback?: (response: Q.SaveResponse) => void): void | boolean;
        protected saveHandler(options: Q.ServiceOptions<Q.SaveResponse>, callback: (response: Q.SaveResponse) => void): void;
        protected initToolbar(): void;
        protected showSaveSuccessMessage(response: Q.SaveResponse): void;
        protected getToolbarButtons(): ToolButton[];
        protected getCloningEntity(): TItem;
        protected updateInterface(): void;
        protected getUndeleteOptions(callback?: (response: Q.UndeleteResponse) => void): Q.ServiceOptions<Q.UndeleteResponse>;
        protected undeleteHandler(options: Q.ServiceOptions<Q.UndeleteResponse>, callback: (response: Q.UndeleteResponse) => void): void;
        protected undelete(callback?: (response: Q.UndeleteResponse) => void): void;
        private _readonly;
        get readOnly(): boolean;
        set readOnly(value: boolean);
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        protected getInsertPermission(): string;
        protected getUpdatePermission(): string;
        protected getDeletePermission(): string;
        protected hasDeletePermission(): boolean;
        protected hasInsertPermission(): boolean;
        protected hasUpdatePermission(): boolean;
        protected hasSavePermission(): boolean;
        protected editClicked: boolean;
        protected isViewMode(): boolean;
        protected useViewMode(): boolean;
        protected getFallbackTemplate(): string;
    }

    type JsxDomWidgetProps<P> = P & WidgetComponentProps<any> & {
        children?: any | undefined;
        class?: string;
    };
    interface JsxDomWidget<P = {}, TElement extends Element = HTMLElement> {
        (props: JsxDomWidgetProps<P>, context?: any): TElement | null;
    }
    function jsxDomWidget<TWidget extends Widget<TOptions>, TOptions>(type: new (element: JQuery, options?: TOptions) => TWidget): JsxDomWidget<TOptions & {
        ref?: (r: TWidget) => void;
    }>;

    namespace Reporting {
        interface ReportDialogOptions {
            reportKey?: string;
        }
        class ReportDialog extends TemplatedDialog<ReportDialogOptions> {
            constructor(opt: ReportDialogOptions);
            protected propertyGrid: PropertyGrid;
            protected propertyItems: Q.PropertyItem[];
            protected reportKey: string;
            protected createPropertyGrid(): void;
            loadReport(reportKey: string): void;
            executeReport(targetFrame: string, exportType: string): void;
            protected getToolbarButtons(): {
                title: string;
                cssClass: string;
                onClick: () => void;
            }[];
        }
        interface ReportExecuteRequest extends ServiceRequest {
            ExportType?: string;
            ReportKey?: string;
            DesignId?: string;
            Parameters?: any;
        }
        class ReportPage extends Widget<any> {
            constructor(div: JQuery);
            protected updateMatchFlags(text: string): void;
            protected categoryClick(e: JQueryEventObject): void;
            protected reportLinkClick(e: JQueryEventObject): void;
        }
        interface ReportRetrieveRequest extends ServiceRequest {
            ReportKey?: string;
        }
        interface ReportRetrieveResponse extends ServiceResponse {
            ReportKey?: string;
            Properties?: Q.PropertyItem[];
            Title?: string;
            InitialSettings?: any;
            IsDataOnlyReport?: boolean;
        }
    }

    interface ScriptContext {
    }
    class ScriptContext {
    }

    class IAsyncInit {
    }

    namespace WX {
        function getWidget<TWidget>(element: JQuery, type: any): TWidget;
        var getWidgetName: typeof Widget.getWidgetName;
        function hasOriginalEvent(e: any): boolean;
        function change(widget: any, handler: any): void;
        function changeSelect2(widget: any, handler: any): void;
        function getGridField(widget: Widget<any>): JQuery;
    }

    class Flexify extends Widget<FlexifyOptions> {
        private xDifference;
        private yDifference;
        constructor(container: JQuery, options: FlexifyOptions);
        storeInitialSize(): void;
        private getXFactor;
        private getYFactor;
        private resizeElements;
        private resizeElement;
    }
    interface FlexifyOptions {
        getXFactor?: (p1: JQuery) => any;
        getYFactor?: (p1: JQuery) => any;
        designWidth?: any;
        designHeight?: any;
    }

    interface GoogleMapOptions {
        latitude?: any;
        longitude?: any;
        zoom?: any;
        mapTypeId?: any;
        markerTitle?: string;
        markerLatitude?: any;
        markerLongitude?: any;
    }
    class GoogleMap extends Widget<GoogleMapOptions> {
        private map;
        constructor(container: JQuery, opt: GoogleMapOptions);
        get_map(): any;
    }

    class Select2AjaxEditor<TOptions, TItem> extends Widget<TOptions> implements IStringValue {
        pageSize: number;
        constructor(hidden: JQuery, opt: TOptions);
        protected emptyItemText(): string;
        protected getService(): string;
        protected query(request: Q.ListRequest, callback: (p1: Q.ListResponse<any>) => void): void;
        protected executeQuery(options: Q.ServiceOptions<Q.ListResponse<any>>): void;
        protected queryByKey(key: string, callback: (p1: any) => void): void;
        protected executeQueryByKey(options: Q.ServiceOptions<Q.RetrieveResponse<any>>): void;
        protected getItemKey(item: any): string;
        protected getItemText(item: any): string;
        protected getTypeDelay(): number;
        protected getSelect2Options(): Select2Options;
        protected addInplaceCreate(title: string): void;
        protected inplaceCreateClick(e: any): void;
        protected get_select2Container(): JQuery;
        get_value(): string;
        get value(): string;
        set_value(value: string): void;
        set value(v: string);
    }

    /**
     *
     * This is the main entry point for `@serenity-is/corelib` package.
     *
     * The types from this module are available by importing from "@serenity-is/corelib":
     *
     * ```ts
     * import { EntityGrid } from "serenity-is/corelib"
     *
     * export class MyGrid extends EntityGrid<MyRow, any> {
     * }
     * ```
     *
     * > When using classic namespaces instead of the ESM modules, the types and functions in this module are directly available from the global `Serenity` namespace.
     * > e.g. `Serenity.EntityGrid`
     * @packageDocumentation
     * @module corelib
     */

    type Constructor<T> = new (...args: any[]) => T;
}
declare namespace Select2 {
    namespace util {
        function stripDiacritics(input: string): string;
    }
}
interface Select2QueryOptions {
    element?: JQuery;
    term?: string;
    page?: number;
    context?: any;
    callback?: (p1: Select2Result) => void;
}
interface Select2Item {
    id: string;
    text: string;
    source?: any;
    disabled?: boolean;
}
interface Select2Result {
    results: any;
    more: boolean;
    context?: any;
}
interface Select2AjaxOptions {
    transport?: any;
    url?: any;
    dataType?: string;
    quietMillis?: number;
    cache?: boolean;
    jsonpCallback?: any;
    data?: (p1: string, p2: number, p3: any) => any;
    results?: (p1: any, p2: number, p3: any) => any;
    params?: any;
}
interface Select2Options {
    width?: any;
    minimumInputLength?: number;
    maximumInputLength?: number;
    minimumResultsForSearch?: number;
    maximumSelectionSize?: any;
    placeHolder?: string;
    placeHolderOption?: any;
    separator?: string;
    allowClear?: boolean;
    multiple?: boolean;
    closeOnSelect?: boolean;
    openOnEnter?: boolean;
    id?: (p1: any) => string;
    matcher?: (p1: string, p2: string, p3: JQuery) => boolean;
    sortResults?: (p1: any, p2: JQuery, p3: any) => any;
    formatSelection?: (p1: any, p2: JQuery, p3: (p1: string) => string) => string;
    formatResult?: (p1: any, p2: JQuery, p3: any, p4: (p1: string) => string) => string;
    formatResultCssClass?: (p1: any) => string;
    formatNoMatches?: (p1: string) => string;
    formatSearching?: () => string;
    formatInputTooShort?: (p1: string, p2: number) => string;
    formatSelectionTooBig?: (p1: string) => string;
    createSearchChoice?: (p1: string) => any;
    createSearchChoicePosition?: string;
    initSelection?: (p1: JQuery, p2: (p1: any) => void) => void;
    tokenizer?: (p1: string, p2: any, p3: (p1: any) => any, p4: any) => string;
    tokenSeparators?: any;
    query?: (p1: Select2QueryOptions) => void;
    ajax?: Select2AjaxOptions;
    data?: any;
    tags?: any;
    containerCss?: any;
    containerCssClass?: any;
    dropdownCss?: any;
    dropdownCssClass?: any;
    dropdownAutoWidth?: boolean;
    adaptContainerCssClass?: (p1: string) => string;
    adaptDropdownCssClass?: (p1: string) => string;
    escapeMarkup?: (p1: string) => string;
    selectOnBlur?: boolean;
    loadMorePadding?: number;
    nextSearchTerm?: (p1: any, p2: string) => string;
}
interface Select2Data {
    text?: string;
}
interface JQuery {
    select2(options: Select2Options): JQuery;
    select2(cmd: 'focus' | 'open'): JQuery;
    select2(cmd: 'destroy'): void;
    select2(cmd: 'val'): any;
    select2(cmd: 'val', value: string | string[]): JQuery;
    select2(cmd: 'data'): Select2Data;
}
interface JQueryStatic {
    extend<T>(target: T, object1?: T, ...objectN: T[]): T;
    toJSON(obj: any): string;
}
declare namespace JQueryValidation {
    interface ValidationOptions {
        normalizer?: (v: string) => string;
    }
}
interface JQuery {
    getWidget<TWidget>(widgetType: {
        new (...args: any[]): TWidget;
    }): TWidget;
    tryGetWidget<TWidget>(widgetType: {
        new (...args: any[]): TWidget;
    }): TWidget;
    flexHeightOnly(flexY?: number): JQuery;
    flexWidthOnly(flexX?: number): JQuery;
    flexWidthHeight(flexX: number, flexY: number): JQuery;
    flexX(flexX: number): JQuery;
    flexY(flexY: number): JQuery;
}
declare namespace Slick {
    namespace Data {
        /** @obsolete use the type exported from @serenity-is/sleekgrid */
        const GroupItemMetadataProvider: GroupItemMetadataProviderType;
    }
    interface RowMoveManagerOptions {
        cancelEditOnDrag: boolean;
    }
    class RowMoveManager implements IPlugin {
        constructor(options: RowMoveManagerOptions);
        init(): void;
        onBeforeMoveRows: Slick.EventEmitter;
        onMoveRows: Slick.EventEmitter;
    }
    class RowSelectionModel implements SelectionModel {
        init(grid: Slick.Grid): void;
        destroy?: () => void;
        setSelectedRanges(ranges: Slick.Range[]): void;
        onSelectedRangesChanged: Slick.EventEmitter<Slick.Range[]>;
        refreshSelections?(): void;
    }
}