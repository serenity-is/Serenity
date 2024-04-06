import { localTextTableSymbol } from "./symbols";
import { getGlobalObject } from "./system";

function getTable(): { [key: string]: string } {
    let localTextTable = getGlobalObject()[localTextTableSymbol];
    if (!localTextTable)
        getGlobalObject()[localTextTableSymbol] = localTextTable = {};
    return localTextTable;
}

export function addLocalText(obj: string | Record<string, string | Record<string, any>> | string, pre?: string) {
    if (!obj)
        return;
    
    let table = getTable();

    if (typeof obj === "string") {
        table[obj] = pre;
        return;
    }

    pre ??= '';
    for (let k of Object.keys(obj)) {
        let actual = pre + k;
        let o = obj[k];
        if (typeof (o) === 'object') {
            addLocalText(o, actual + '.');
        }
        else {
            table[actual] = o;
        }
    }
}

export function localText(key: string, defaultText?: string): string {
    return getTable()[key] ?? defaultText ?? key ?? '';
}

export function tryGetText(key: string): string {
    return getTable()[key];
}

export function proxyTexts(o: Record<string, any>, p: string, t: Record<string, any>): Object {
    return new Proxy(o, {
        get: (_: Object, y: string) => {
            if (typeof y === "symbol")
                return;
            var tv = t[y];
            if (tv == null)
                return localText(p + y);
            else {
                var z = o[y];
                if (z != null)
                    return z;
                o[y] = z = proxyTexts({}, p + y + '.', tv);
                return z;
            }
        },
        ownKeys: (_: Object) => Object.keys(t)
    });
}

let global = getGlobalObject();
(global.Serenity || (global.Serenity = {})).addLocalText = addLocalText;