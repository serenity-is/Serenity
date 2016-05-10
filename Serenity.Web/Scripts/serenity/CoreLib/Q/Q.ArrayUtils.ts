/// <reference path="../Imports/SS.ts" />
/// <reference path="Q.CommonTypes.ts" />

namespace Q {
    export function arrayClone<T>(a: T[]): T[] {
        return (ss as any).arrayClone(a);
    }

    export function any<TItem>(array: TItem[], predicate: (x: TItem) => boolean): boolean {
        for (let x of array)
            if (predicate(x))
                return true;

        return false;
    }

    export function count<TItem>(array: TItem[], predicate: (x: TItem) => boolean): number {
        let count = 0;
        for (let x of array)
            if (predicate(x))
                count++;

        return count;
    }

    export function first<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem {
        for (let x of array)
            if (predicate(x))
                return x;

        throw new Error("first:No element satisfies the condition.!");
    }

    export function indexOf<TItem>(array: TItem[], predicate: (x: TItem) => boolean): number {
        for (var i = 0; i < array.length; i++)
            if (predicate(array[i]))
                return i;

        return -1;
    }

    export function insert(obj: any, index: number, item: any): void {
        (ss as any).insert(obj, index, item);
    }

    export function isArray(a: any): boolean {
        return (ss as any).isArray(a);
    }

    export function single<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem {
        let match;
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

    export function tryFirst<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem {
        for (let x of array)
            if (predicate(x))
                return x;
    }
}