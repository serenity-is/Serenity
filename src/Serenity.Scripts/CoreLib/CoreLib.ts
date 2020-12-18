import * as Q from './CoreLib.Q';
import * as Serenity from './CoreLib.Serenity';
import * as Slick from "./CoreLib.Slick";
export * from "./SlickGrid/Imports";
import { jQueryPatch } from './Patch/jQueryPatch'
//import { vuePatch } from './Patch/VuePatch'

if (typeof jQuery === "function") {
    jQueryPatch(jQuery);
}

// @ts-ignore
if (typeof Vue == "function")
// @ts-ignore
    vuePatch(Vue);

export { Q, Serenity, Slick };