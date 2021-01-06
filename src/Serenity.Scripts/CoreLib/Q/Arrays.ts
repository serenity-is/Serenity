export type Grouping<TItem> = { [key: string]: TItem[] };

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
export function toGrouping<TItem>(items: TItem[], getKey: (x: TItem) => any): Grouping<TItem> {
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
    byKey: { [key: string]: Group<TItem> };
    inOrder: Group<TItem>[];
};

/**
 * Groups an array with keys determined by specified getKey() callback.
 * Resulting object contains group objects in order and a dictionary to access by key.
 */
export function groupBy<TItem>(items: TItem[], getKey: (x: TItem) => any): Groups<TItem> {
    let result: Groups<TItem> = {
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
 * Gets first element in an array that matches given predicate.
 * Returns null if no match is found.
 */
export function tryFirst<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem {
    for (let x of array)
        if (predicate(x))
            return x;
}