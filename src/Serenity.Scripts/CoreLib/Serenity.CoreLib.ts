export * as Q from './Serenity.CoreLib.Q';
export * as Serenity from './Serenity.CoreLib.Serenity';
export * as Slick from "./Serenity.CoreLib.Slick";
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