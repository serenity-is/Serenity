/**
 * Tests whether any element in the array satisfies the predicate.
 * @param array - Array to test.
 * @param predicate - Function invoked per element; should return `true` for a match.
 * @returns `true` if at least one element matches, otherwise `false`.
 * @deprecated Prefer native `Array.prototype.some` — e.g. `array.some(predicate)`. Retained as a `Q.any` compat shim.
 * @example
 * any([1, 2, 3], x => x > 2); // true
 */
export function any<TItem>(array: TItem[], predicate: (x: TItem) => boolean): boolean {
    return array.some(predicate);
}

/**
 * Counts elements that satisfy the predicate.
 * @param array - Array to count over.
 * @param predicate - Function invoked per element; return `true` to count the element.
 * @returns Number of matching elements.
 * @deprecated Prefer `array.filter(predicate).length` or a manual loop. Retained as a `Q.count` compat shim.
 * @example
 * count([1, 2, 3], x => x % 2 === 1); // 2
 */
export function count<TItem>(array: TItem[], predicate: (x: TItem) => boolean): number {
    let count = 0;
    for (let x of array)
        if (predicate(x))
            count++;

    return count;
}

/**
 * Returns the first element that satisfies the predicate (LINQ `First` semantics).
 * @param array - Array to search.
 * @param predicate - Function invoked per element; return `true` for the desired element.
 * @returns The first matching element.
 * @throws {Error} If no element satisfies the predicate (`"first:No element satisfies the condition."`).
 * @deprecated Prefer `array.find(predicate)` with explicit not-found handling. Retained as a `Q.first` compat shim.
 * @example
 * first([1, 2, 3], x => x > 1); // 2
 */
export function first<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem {
    for (let x of array)
        if (predicate(x))
            return x;

    throw new Error("first:No element satisfies the condition.");
}

/**
 * Single group produced by {@link groupBy}.
 * @typeParam TItem - Element type of the source array.
 * @example
 * const g = groupBy(users, u => u.department);
 * g.inOrder[0].key; // department key
 */
export type GroupByElement<TItem> = {
    /** Zero-based position of this group in the {@link GroupByResult.inOrder} array. */
    order: number;
    /** Group key as returned by the `getKey` callback (normalized to string). */
    key: string;
    /** Elements belonging to this group, in original encounter order. */
    items: TItem[];
    /** Index of the first element of this group in the original source array. */
    start: number;
}

/**
 * Result returned by {@link groupBy}.
 * @typeParam TItem - Element type of the source array.
 * @remarks Provides both dictionary (`byKey`) and ordered (`inOrder`) access to groups.
 */
export type GroupByResult<TItem> = {
    /** Dictionary mapping stringified key to its {@link GroupByElement}. */
    byKey: { [key: string]: GroupByElement<TItem> };
    /** Groups in order of first encounter in the source array. */
    inOrder: GroupByElement<TItem>[];
};

/**
 * Groups an array with keys determined by specified getKey() callback.
 * Resulting object contains group objects in order and a dictionary to access by key.
 * This is similar to LINQ's ToLookup function with some additional details like start index.
 * @param items Array to group.
 * Groups an array by keys derived from each element.
 * @param items - Array to group.
 * @param getKey - Callback returning the group key for an element; `null`/`undefined` is normalized to `""`.
 * @returns A {@link GroupByResult} with `byKey` dictionary and `inOrder` array. Each group records its `order`, `key`, `items`, and `start` index.
 * @remarks Similar to LINQ `ToLookup` with extra `order`/`start` metadata. Uses `Object.create(null)` so prototype keys are safe.
 * @deprecated Kept as a `Q.groupBy` compat shim; for new code consider `Map`-based grouping or `toGrouping`.
 * @example
 * groupBy([{k:'a'}, {k:'b'}, {k:'a'}], x => x.k).inOrder.length; // 2
 */
export function groupBy<TItem>(items: TItem[], getKey: (x: TItem) => any): GroupByResult<TItem> {
    let result: GroupByResult<TItem> = {
        byKey: Object.create(null),
        inOrder: []
    };

    for (let index = 0; index < items.length; index++) {
        const item = items[index];
        let key = getKey(item) ?? "";
        let group = result.byKey[key];
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
 * Returns the index of the first element satisfying the predicate.
 * @param array - Array to search.
 * @param predicate - Function invoked per element; return `true` for the target element.
 * @returns Zero-based index of the first match, or `-1` if none matches.
 * @deprecated Prefer `Array.prototype.findIndex` — `array.findIndex(predicate)`. Retained as a `Q.indexOf` compat shim (note the predicate overload differs from `Array.indexOf`).
 * @example
 * indexOf([1, 2, 3], x => x === 2); // 1
 */
export function indexOf<TItem>(array: TItem[], predicate: (x: TItem) => boolean): number {
    for (let i = 0; i < array.length; i++)
        if (predicate(array[i]))
            return i;

    return -1;
}

/**
 * Inserts an item into an array at the given index.
 * @param obj - Target array or array-like object with an `insert(index, item)` method.
 * @param index - Zero-based index at which to insert.
 * @param item - Item to insert.
 * @throws {Error} If `obj` is neither an array nor exposes `insert`.
 * @remarks If `obj.insert` exists it is delegated to; otherwise `Array.prototype.splice` is used. No return value.
 * @deprecated Prefer `array.splice(index, 0, item)` directly. Retained as a `Q.insert` compat shim.
 * @example
 * insert([1, 2, 3], 1, 4); // [1, 4, 2, 3]
 */
export function insert(obj: any, index: number, item: any): void {
    if (obj.insert)
        obj.insert(index, item);
    else if (Array.isArray(obj))
        obj.splice(index, 0, item);
    else
        throw new Error("Object does not support insert!");
}

/**
 * Tests whether a value is an array.
 * @remarks Thin re-export of `Array.isArray` for legacy `Q.isArray` call sites.
 * @deprecated Use `Array.isArray` directly.
 * @example
 * isArray([1, 2, 3]); // true
 * isArray({}); // false
 */
export const isArray = Array.isArray;

/**
 * Returns the single element satisfying the predicate (LINQ `Single` semantics).
 * @param array - Array to search.
 * @param predicate - Function invoked per element; exactly one element must return `true`.
 * @returns The sole matching element.
 * @throws {Error} If no element matches (`"single:No element satisfies the condition."`) or more than one matches (`"single:sequence contains more than one element."`).
 * @deprecated Retained as a `Q.single` compat shim; prefer explicit `filter` + length check for clarity.
 * @example
 * single([1, 2, 3], x => x == 2); // 2
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
 * Dictionary mapping a stringified key to the array of items sharing that key.
 * Produced by {@link toGrouping}.
 * @typeParam TItem - Element type of the source array.
 */
export type Grouping<TItem> = { [key: string]: TItem[] };

/**
 * Groups an array into a dictionary keyed by `getKey`.
 * @param items - Array to group.
 * @param getKey - Callback returning the group key for an element; `null`/`undefined` is normalized to `""`.
 * @returns A {@link Grouping} dictionary whose values are arrays of matching elements. Uses a null-prototype object.
 * @remarks Lighter alternative to {@link groupBy} when ordered metadata is not needed.
 * @deprecated Retained as a `Q.toGrouping` compat shim; new code may prefer `Map`-grouping.
 * @example
 * toGrouping([1, 2, 3], x => x % 2 == 0 ? "even" : "odd"); // { odd: [1, 3], even: [2] }
 */
export function toGrouping<TItem>(items: TItem[], getKey: (x: TItem) => any): Grouping<TItem> {
    let lookup: Grouping<TItem> = Object.create(null) as Grouping<TItem>;
    for (let x of items) {
        let key = getKey(x) ?? "";
        let d = lookup[key];
        if (!d) {
            d = lookup[key] = [];
        }

        d.push(x);
    }
    return lookup;
}

/**
 * Returns the first element satisfying the predicate, or `undefined` if none matches (LINQ `FirstOrDefault`).
 * @param array - Array to search.
 * @param predicate - Function invoked per element; return `true` for the desired element.
 * @returns The first matching element, or `undefined` when no match is found.
 * @deprecated Prefer `Array.prototype.find` — `array.find(predicate)`. Retained as a `Q.tryFirst` compat shim.
 * @example
 * tryFirst([1, 2, 3], x => x == 2); // 2
 * tryFirst([1, 2, 3], x => x == 4); // undefined
 */
export function tryFirst<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem {
    for (let x of array)
        if (predicate(x))
            return x;
}