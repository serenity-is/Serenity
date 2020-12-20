export * as Q from './index.Q';
export * as Serenity from './index.Serenity';
export * as Slick from "./index.Slick";
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