/// <reference types="jquery" />
const $: JQueryStatic = typeof jQuery === "function" ? jQuery : typeof window !== "undefined" ? (window as any).jQuery : undefined;
export default $;