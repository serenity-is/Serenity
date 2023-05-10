/// <reference types="jquery" />
const $: JQueryStatic = typeof jQuery === "function" ? jQuery : typeof globalThis.$ === "function" ? globalThis.$ :
    typeof window !== "undefined" ? (window.jQuery ?? window.$) : undefined;

export default $;