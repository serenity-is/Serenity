/**
 * Gets the globally available jQuery instance, if any.
 * @remarks
 * Checks both `jQuery` and `$` globals. Returns `undefined` when jQuery is
 * not loaded or does not expose `fn`, allowing the codebase to fall back to
 * native DOM / Bootstrap 5 APIs.
 * @returns The jQuery function when available, otherwise `undefined`.
 */
export function getjQuery(): any {
    // @ts-ignore
    return typeof jQuery === "function" ? jQuery : typeof $ === "function" && ($ as any).fn ? $ : undefined;
}

/**
 * Determines whether Bootstrap 3 is loaded on the page.
 * @remarks
 * Inspects `jQuery.fn.modal.Constructor.VERSION`; the check is safe when
 * jQuery or the modal plugin is absent.
 * @returns `true` if Bootstrap 3 is detected, otherwise `false`.
 */
export function isBS3(): boolean {
    return (getjQuery()?.fn?.modal?.Constructor?.VERSION + "").charAt(0) == '3';
}

/**
 * Determines whether Bootstrap 5 or later is loaded on the page.
 * @remarks
 * Uses the global `bootstrap.Modal.VERSION` when available. Explicitly
 * excludes Bootstrap 4 (major version `"4"`) so that Bootstrap 4 is treated
 * as neither BS3 nor BS5+.
 * @returns `true` if Bootstrap 5+ is detected, otherwise `false`.
 */
export function isBS5Plus(): boolean {
    return typeof bootstrap !== "undefined" && !!bootstrap.Modal && (bootstrap.Modal.VERSION + "").charAt(0) != '4';
}
