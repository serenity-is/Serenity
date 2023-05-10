import $, { jQuery } from "../../../../src/Serenity.Assets/wwwroot/jquery/jquery.js";
const _$ = window.$ = window.jQuery = window.$ ?? window.jQuery ?? (typeof jQuery === "function" ? jQuery : $);
export default _$;
