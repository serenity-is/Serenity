/// <reference path="../Imports/SS.ts" />
/// <reference path="Q.CommonTypes.ts" />

namespace Q {

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

    /**
     * Gets first element in an array that matches given predicate.
     * Returns null if no match is found.
     */
    export function tryFirst<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem {
        for (let x of array)
            if (predicate(x))
                return x;
    }
}