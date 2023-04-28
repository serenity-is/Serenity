export function endsWith(s: string, suffix: string): boolean {
    return s.endsWith(suffix);
}

export function isEmptyOrNull(s: string) {
    return s == null || s.length === 0;
}

export function isTrimmedEmpty(s: string) {
    return trimToNull(s) == null;
}

export function padLeft(s: string | number, len: number, ch: string = ' ') {
    s = s == null ? '' : s.toString();
    if ((s as any).padStart)
        return (s as any).padStart(len, ch);
    while (s.length < len)
        s = ch + s;
    return s;
}

export function startsWith(s: string, prefix: string): boolean {
    return s.startsWith(prefix);
}

export function toSingleLine(str: string) {
    return replaceAll(replaceAll(trimToEmpty(str), '\r\n', ' '), '\n', ' ').trim();
}

export var trimEnd = function(s: string) {
    return (s ?? "" as any).trimEnd?.() ?? s.replace(/\s*$/, '');
};

export var trimStart = function(s: string) {
    return (s ?? "" as any).trimStart?.() ?? s.replace(/^\s*/, '');
};

export function trim(s: string) {
    return s?.trim() ?? '';
}

export function trimToEmpty(s: string) {
    return (s ?? "").trim();
}

export function trimToNull(s: string) {
    if (s == null)
        return null;
    s = trim(s);
    return s.length === 0 ? null : s;
}

export function replaceAll(str: string, find: string, replace: string): string {
    str = str || '';
    return (str as any).replaceAll?.(find, replace) ?? str.split(find).join(replace);
}

export function zeroPad(n: number, digits: number): string {
    if (n == null)
        return "";
    let s = n.toString();
    while (s.length < digits)
        s = "0" + s;
    return s;
}