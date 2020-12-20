export * as Q from './Q/indexAll';
export * as Serenity from './Serenity/indexAll';
export * as Slick from "./Slick";
export * from "./Globals";
import { jQueryPatch } from './Patch/jQueryPatch'
//import { vuePatch } from './Patch/VuePatch'

if (typeof jQuery === "function") {
    jQueryPatch(jQuery);
}

// @ts-ignore
if (typeof Vue == "function")
// @ts-ignore
    vuePatch(Vue);