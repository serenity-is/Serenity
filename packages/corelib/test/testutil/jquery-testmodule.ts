import $, { jQuery } from "../../../../src/Serenity.Assets/wwwroot/jquery/jquery.js";
const _$ = (window as any).$ = (window as any).jQuery = (window as any).$ ?? (window as any).jQuery ?? (typeof jQuery === "function" ? jQuery : $);
export default _$;
