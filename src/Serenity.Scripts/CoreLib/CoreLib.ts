export * as Q from './Q/indexAll';
export * as Serenity from './Serenity';
export * as Slick from "./Slick";
import { jQueryPatch } from './Patch/jQueryPatch'
import { promisePatch } from './Patch/PromisePatch'
import { vuePatch } from './Patch/VuePatch'

if (typeof jQuery === "function") {
    jQueryPatch(jQuery);
}

promisePatch();

// @ts-ignore
if (typeof Vue == "function")
// @ts-ignore
    vuePatch(Vue);