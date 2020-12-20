export * as Q from './Q/indexAll';
export * as Serenity from './Serenity/indexAll';
export * as Slick from "./SlickGrid";
export * from "./Globals/Select2";
export * from "./Globals/Validate";
import { jQueryPatch } from './Patch/jQueryPatch'
//import { vuePatch } from './Patch/VuePatch'

if (typeof jQuery === "function") {
    jQueryPatch(jQuery);
}

// @ts-ignore
if (typeof Vue == "function")
// @ts-ignore
    vuePatch(Vue);