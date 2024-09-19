export function getjQuery(): any {
    // @ts-ignore
    return typeof jQuery === "function" ? jQuery : typeof $ === "function" && ($ as any).fn ? $ : undefined;
}

/** Returns true if Bootstrap 3 is loaded */
export function isBS3(): boolean {
    return (getjQuery()?.fn?.modal?.Constructor?.VERSION + "").charAt(0) == '3';
}

/** Returns true if Bootstrap 5+ is loaded */
export function isBS5Plus(): boolean {
    return typeof bootstrap !== "undefined" && !!bootstrap.Modal && (bootstrap.Modal.VERSION + "").charAt(0) != '4';
}
