import { addLocalText, getGlobalObject, localText } from "../base";

/** @deprecated prefer localText for better discoverability */
export const text = localText;

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