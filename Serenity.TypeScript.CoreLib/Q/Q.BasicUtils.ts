
namespace Q {
    export type Dictionary<TItem> = { [key: string]: TItem };
    export type Grouping<TItem> = { [key: string]: TItem[] };

    export function coalesce(a: any, b: any): any {
        return (ss as any).coalesce(a, b);
    }

    export function isValue(a: any): boolean {
        return (ss as any).isValue(a);
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
    export function insert<TItem>(array: TItem[], index: number, item: TItem): void {
        (ss as any).insert(array, index, item);
    }

    /**
     * Determines if the object is an array
     */
    export function isArray(obj: any): boolean {
        return (ss as any).isArray(obj);
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

    export function endsWith(s: string, search: string): boolean {
        return (ss as any).endsWithString(s, search);
    }

    export function isEmptyOrNull(s: string) {
        return s == null || s.length === 0;
    }

    export function isTrimmedEmpty(s: string) {
        return trimToNull(s) == null;
    }

    export function format(msg: string, ...prm: any[]): string {
        return (ss as any).formatString(msg, ...prm);
    }

    export function padLeft(s: string, len: number, ch: string = ' ') {
        while (s.length < len)
            s = "0" + s;
        return s;
    }

    export function startsWith(s: string, search: string): boolean {
        return (ss as any).startsWithString(s, search);
    }

    export function toSingleLine(str: string) {
        return Q.replaceAll(Q.replaceAll(trimToEmpty(str), '\r\n', ' '), '\n', ' ').trim();
    }

    export function trim(s: string) {
        return (s == null ? '' : s).replace(new RegExp('^\\s+|\\s+$', 'g'), '');
    }

    export function trimToEmpty(s: string) {
        if (s == null || s.length === 0) {
            return '';
        }

        return trim(s);
    }

    export function trimToNull(s: string) {
        if (s == null || s.length === 0) {
            return null;
        }

        s = trim(s);
        if (s.length === 0) {
            return null;
        }
        else {
            return s;
        }
    }

    let turkishOrder: {};

    export function turkishLocaleCompare(a: string, b: string): number {
        let alphabet = "AaBbCcÇçFfGgĞğHhIıİiJjKkLlMmNnOoÖöPpRrSsŞşTtUuÜüVvYyZz";
        a = a || "";
        b = b || "";
        if (a == b)
            return 0;
        if (!turkishOrder) {
            turkishOrder = {};
            for (let z = 0; z < alphabet.length; z++) {
                turkishOrder[alphabet.charAt(z)] = z + 1;
            }
        }
        for (let i = 0, _len = Math.min(a.length, b.length); i < _len; i++) {
            let x = a.charAt(i), y = b.charAt(i);
            if (x === y)
                continue;
            let ix = turkishOrder[x], iy = turkishOrder[y];
            if (ix != null && iy != null)
                return ix < iy ? -1 : 1;
            let c = x.localeCompare(y);
            if (c == 0)
                continue;
            return c;
        }
        return a.localeCompare(b);
    }

    export function turkishLocaleToUpper(a: string): string {
        if (!a)
            return a;
        return a.replace(/i/g, 'İ').replace(/ı/g, 'I').toUpperCase();
    }

    export function replaceAll(s: string, f: string, r: string): string {
        return (ss as any).replaceAllString(s, f, r);
    }

    export function zeroPad(n: number, digits: number): string {
        let s = n.toString();
        while (s.length < digits)
            s = "0" + s;
        return s;
    }

    // derived from https://github.com/mistic100/jQuery.extendext/blob/master/jQuery.extendext.js
    export function deepClone<TItem>(arg1: TItem, ...args: TItem[]): TItem {
        let options: any,
            name: string,
            src: any,
            copy: any,
            copyIsArray: boolean,
            clone: any,
            target = arguments[0] || {},
            i = 1,
            length = arguments.length;

        // Handle case when target is a string or something (possible in deep copy)
        if (typeof target !== "object" && !$.isFunction(target)) {
            target = {};
        }
        if (i === length) {
            target = {};
            i = 0;
        }
        for (; i < length; i++) {
            // Only deal with non-null/undefined values
            if ((options = arguments[i]) !== null) {
                // Special operations for arrays
                if ($.isArray(options)) {
                    target = $.extend(true, [], options);
                }
                else {
                    // Extend the base object
                    for (name in options) {
                        src = target[name];
                        copy = options[name];
                        // Prevent never-ending loop
                        if (target === copy) {
                            continue;
                        }
                        // Recurse if we're merging plain objects or arrays
                        if (copy && ($.isPlainObject(copy) ||
                            (copyIsArray = $.isArray(copy)))) {
                            if (copyIsArray) {
                                copyIsArray = false;
                                clone = src && $.isArray(src) ? src : [];
                            }
                            else {
                                clone = src && $.isPlainObject(src) ? src : {};
                            }
                            // Never move original objects, clone them
                            target[name] = deepClone(clone, copy);
                        }
                        else if (copy !== undefined) {
                            target[name] = copy;
                        }
                    }
                }
            }
        }
        // Return the modified object
        return target;
    }
}