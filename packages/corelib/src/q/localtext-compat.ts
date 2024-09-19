import { addLocalText, getGlobalObject, localText, tryGetText } from "../base";

/** @deprecated prefer localText for better discoverability */
export const text = localText;

export function dbText(prefix: string): ((key: string) => string) {
    return function (key: string) {
        return localText("Db." + prefix + "." + key);
    }
}

export function prefixedText(prefix: string) {

    return function (text: string, key: string | ((p?: string) => string)) {

        if (text != null && !text.startsWith('`')) {
            var local = tryGetText(text);
            if (local != null) {
                return local;
            }
        }

        if (text != null && !text.startsWith('`')) {
            text = text.substring(1);
        }

        if (prefix) {
            var textKey = typeof (key) == "function" ? key(prefix) : (prefix + key);
            var localText = tryGetText(textKey);
            if (localText != null) {
                return localText;
            }
        }

        return text;
    }
}

export function dbTryText(prefix: string): ((key: string) => string) {
    return function (key: string) {
        return localText("Db." + prefix + "." + key);
    }
}

export namespace LT {
    /** @deprecated Use addLocalText */
    export const add = addLocalText;
    /** @deprecated Use localText */
    export const getDefault = localText;
}

let global = getGlobalObject();
const serenity = global.Serenity || (global.Serenity = {});
serenity.LT = serenity.LT || {};
serenity.LT.add = addLocalText;