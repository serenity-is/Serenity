import $_, { $, jQuery } from "../../../../Serenity.Assets/wwwroot/jquery/jquery.min.js";
window.$ = window.jQuery = window.jQuery ?? $ ?? jQuery ?? $_;
export default window.$;
