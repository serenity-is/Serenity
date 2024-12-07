/**
 * Tests if any of array elements matches given predicate. Prefer Array.some() over this function (e.g. `[1, 2, 3].some(predicate)`).
 * @param array Array to test.
 * @param predicate Predicate to test elements.
 * @returns True if any element matches.
 */
export function any<TItem>(array: TItem[], predicate: (x: TItem) => boolean): boolean {
    return array.some(predicate);
}

/**
 * Counts number of array elements that matches a given predicate.
 * @param array Array to test.
 * @param predicate Predicate to test elements.
 */
export function count<TItem>(array: TItem[], predicate: (x: TItem) => boolean): number {
    let count = 0;
    for (let x of array)
        if (predicate(x))
            count++;

    return count;
}

/**
 * Gets first element in an array that matches given predicate similar to LINQ's First.
 * Throws an error if no match is found.
 * @param array Array to test.
 * @param predicate Predicate to test elements.
 * @returns First element that matches.
 */
export function first<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem {
    for (let x of array)
        if (predicate(x))
            return x;

    throw new Error("first:No element satisfies the condition.!");
}

/**
 * A group item returned by `groupBy()`.
 */
export type GroupByElement<TItem> = {
    /** index of the item in `inOrder` array */
    order: number;
    /** key of the group */
    key: string;
    /** the items in the group */
    items: TItem[];
    /** index of the first item of this group in the original array */
    start: number;
}

/**
 * Return type of the `groupBy` function.
 */
export type GroupByResult<TItem> = {
    byKey: { [key: string]: GroupByElement<TItem> };
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
export function groupBy<TItem>(items: TItem[], getKey: (x: TItem) => any): GroupByResult<TItem> {
    let result: GroupByResult<TItem> = {
        byKey: Object.create(null),
        inOrder: []
    };

    for (var index = 0; index < items.length; index++) {
        var item = items[index];
        let key = getKey(item) ?? "";
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
 * Gets index of first element in an array that matches given predicate.
 * @param array Array to test.
 * @param predicate Predicate to test elements.
 */
export function indexOf<TItem>(array: TItem[], predicate: (x: TItem) => boolean): number {
    for (var i = 0; i < array.length; i++)
        if (predicate(array[i]))
            return i;

    return -1;
}

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
export function insert(obj: any, index: number, item: any): void {
    if (obj.insert)
        obj.insert(index, item);
    else if (Array.isArray(obj))
        obj.splice(index, 0, item);
    else
        throw new Error("Object does not support insert!");
}

/**
 * Determines if the object is an array. Prefer Array.isArray over this function (e.g. `Array.isArray(obj)`).
 * @param obj Object to test.
 * @returns True if the object is an array.
 * @example
 * isArray([1, 2, 3]); // true
 * isArray({}); // false
 */
export const isArray = Array.isArray;

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

export type Grouping<TItem> = { [key: string]: TItem[] };

/**
 * Maps an array into a dictionary with keys determined by specified getKey() callback,
 * and values that are arrays containing elements for a particular key.
 * @param items Array to map.
 * @param getKey Function that returns key for each item.
 * @returns Grouping object.
 * @example
 * toGrouping([1, 2, 3], x => x % 2 == 0 ? "even" : "odd"); // { odd: [1, 3], even: [2] }
 */
export function toGrouping<TItem>(items: TItem[], getKey: (x: TItem) => any): Grouping<TItem> {
    let lookup: Grouping<TItem> = {};
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
 * Gets first element in an array that matches given predicate (similar to LINQ's FirstOrDefault).
 * Returns null if no match is found.
 * @param array Array to test.
 * @param predicate Predicate to test elements.
 * @returns First element that matches.
 * @example
 * tryFirst([1, 2, 3], x => x == 2); // 2
 * tryFirst([1, 2, 3], x => x == 4); // null
 */
export function tryFirst<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem {
    for (let x of array)
        if (predicate(x))
            return x;
}