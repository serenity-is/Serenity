import * as Q from './CoreLibQ';
import * as Serenity from './CoreLibSerenity';
import * as Slick from "./CoreLibSlick";
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

export { Q, Serenity, Slick };