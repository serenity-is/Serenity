/// <reference types="jquery" />
const $: JQueryStatic = typeof jQuery === "function" ? jQuery : typeof (globalThis as any).$ === "function" ? (globalThis as any).$ :
    typeof window !== "undefined" ? ((window as any).jQuery ?? (window as any).$) : undefined;

export default $;