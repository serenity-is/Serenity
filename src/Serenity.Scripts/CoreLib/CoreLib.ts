import * as Q from './CoreLibQ';
import * as Serenity from './CoreLibSerenity';
import { jQueryPatch } from './Patch/jQueryPatch'
//import { vuePatch } from './Patch/VuePatch'

if (typeof jQuery === "function") {
    jQueryPatch(jQuery);
}

// @ts-ignore
//if (typeof Vue == "function")
// @ts-ignore
//    vuePatch(Vue, typeof jQuery == "function" ? jQuery : null);

export { Q, Serenity };