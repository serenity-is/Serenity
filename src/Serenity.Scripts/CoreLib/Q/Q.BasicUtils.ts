
namespace Q {
    export type Dictionary<TItem> = { [key: string]: TItem };
    export type Grouping<TItem> = { [key: string]: TItem[] };

    export function coalesce(a: any, b: any): any {
        return a != null ? a : b;
    }

    export function isValue(a: any): boolean {
        return a != null;
    }

    /**
         * Tests if any of array elements matches given predicate
         */
    export function any<TItem>(array: TItem[], predicate: (x: TItem) => boolean): boolean {
        for (let x of array)
            if (predicate(x))
                return true;

        return false;
    }

    /**
     * Counts number of array elements that matches a given predicate
     */
    export function count<TItem>(array: TItem[], predicate: (x: TItem) => boolean): number {
        let count = 0;
        for (let x of array)
            if (predicate(x))
                count++;

        return count;
    }

    /**
     * Gets first element in an array that matches given predicate.
     * Throws an error if no match is found.
     */
    export function first<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem {
        for (let x of array)
            if (predicate(x))
                return x;

        throw new Error("first:No element satisfies the condition.!");
    }

    /**
     * Gets index of first element in an array that matches given predicate
     */
    export function indexOf<TItem>(array: TItem[], predicate: (x: TItem) => boolean): number {
        for (var i = 0; i < array.length; i++)
            if (predicate(array[i]))
                return i;

        return -1;
    }

    /**
     * Inserts an item to the array at specified index
     */
    export function insert(obj: any, index: number, item: any): void {
        if (obj.insert)
            obj.insert(index, item);
        else if (Object.prototype.toString.call(obj) === '[object Array]')
            obj.splice(index, 0, item);
        else
            throw new Error("Object does not support insert!");
    }

    /**
     * Determines if the object is an array
     */
    export function isArray(obj: any): boolean {
        return Object.prototype.toString.call(obj) === '[object Array]';
    }

    /**
    * Gets first element in an array that matches given predicate.
    * Throws an error if no matches is found, or there are multiple matches.
    */
    export function single<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem {
        let match: any;
        let found = false;
        for (let x of array)
            if (predicate(x)) {
                if (found)
                    throw new Error("single:sequence contains more than one element.");

                found = true;
                match = x;
            }

        if (!found)
            throw new Error("single:No element satisfies the condition.");

        return match;
    }

    /**
     * Maps an array into a dictionary with keys determined by specified getKey() callback,
     * and values that are arrays containing elements for a particular key.
     */
    export function toGrouping<TItem>(items: TItem[], getKey: (x: TItem) => any): Q.Grouping<TItem> {
        let lookup: Grouping<TItem> = {};
        for (let x of items) {
            let key = getKey(x) || "";
            let d = lookup[key];
            if (!d) {
                d = lookup[key] = [];
            }

            d.push(x);
        }
        return lookup;
    }


    export type Group<TItem> = {
        order: number;
        key: string;
        items: TItem[];
        start: number;
    }

    export type Groups<TItem> = {
        byKey: Q.Dictionary<Group<TItem>>;
        inOrder: Group<TItem>[];
    };

    /**
     * Groups an array with keys determined by specified getKey() callback.
     * Resulting object contains group objects in order and a dictionary to access by key.
     */
    export function groupBy<TItem>(items: TItem[], getKey: (x: TItem) => any): Q.Groups<TItem> {
        let result: Groups<TItem> = {
            byKey: Object.create(null),
            inOrder: []
        };

        for (var index = 0; index < items.length; index++) {
            var item = items[index];
            let key = Q.coalesce(getKey(item), "");
            var group = result.byKey[key];
            if (group === undefined) {
                group = {
                    order: result.inOrder.length,
                    key: key,
                    items: [item],
                    start: index
                }
                result.byKey[key] = group;
                result.inOrder.push(group);
            }
            else {
                group.items.push(item);
            }
        }

        return result;
    }

    /**
     * Gets first element in an array that matches given predicate.
     * Returns null if no match is found.
     */
    export function tryFirst<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem {
        for (let x of array)
            if (predicate(x))
                return x;
    }
    
    export let today = (): Date => {
        var d = new Date();
        return new Date(d.getFullYear(), d.getMonth(), d.getDate());
    }

    /**
     * Returns a function, that, as long as it continues to be invoked, will not
     * be triggered. The function will be called after it stops being called for
     * N milliseconds. If `immediate` is passed, trigger the function on the
     * leading edge, instead of the trailing. The function also has a property 'clear'
     * that is a function which will clear the timer to prevent previously scheduled executions.
     *
     * @source underscore.js
     */
    export function debounce(func: Function, wait?: number, immediate?: boolean) {
        var timeout: number, args: IArguments, context: any, timestamp: number, result: any;
        if (null == wait) wait = 100;

        function later() {
            var last = Date.now() - timestamp;

            if (last < wait && last >= 0) {
                timeout = setTimeout(later, wait - last);
            } else {
                timeout = null;
                if (!immediate) {
                    result = func.apply(context, args);
                    context = args = null;
                }
            }
        };

        var debounced = function () {
            context = this;
            args = arguments;
            timestamp = Date.now();
            var callNow = immediate && !timeout;
            if (!timeout) timeout = setTimeout(later, wait);
            if (callNow) {
                result = func.apply(context, args);
                context = args = null;
            }

            return result;
        };

        (debounced as any).clear = function () {
            if (timeout) {
                clearTimeout(timeout);
                timeout = null;
            }
        };

        (debounced as any).flush = function () {
            if (timeout) {
                result = func.apply(context, args);
                context = args = null;

                clearTimeout(timeout);
                timeout = null;
            }
        };

        return debounced;
    };

    export function extend<T = any>(a: T, b: T): T {
        for (var key in b)
            if (b.hasOwnProperty(key))
                a[key] = b[key];
        return a;
    }

    export function deepClone<T = any>(a: T, a2?: any, a3?: any): T {
        // for backward compatibility
        if (a2 != null || a3 != null) {
            return Q.extend(Q.extend(Q.deepClone(a || {}), Q.deepClone(a2 || {})), Q.deepClone(a3 || {}));
        }

        if (!a)
            return a;
      
        let v;
        let b: T = Array.isArray(a) ? [] : {} as any;
        for (const k in a) {
            v = a[k];
            b[k] = (typeof v === "object") ? deepClone(v) : v;
        }
      
        return b;
    }
}