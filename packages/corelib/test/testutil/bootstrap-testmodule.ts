const _bootstrap: {
    Modal?: {
        new (div: HTMLElement, opt?: { backdrop?: boolean }): { show: () => void, hide: () => void };
        VERSION?: string;
        getInstance(el: HTMLElement): { hide: () => void, show: () => void };
    }
// @ts-ignore
} = typeof bootstrap !== "undefined" ? bootstrap : undefined;

export default _bootstrap;