export function endsWith(s: string, suffix: string): boolean {
    if (suffix == null || !suffix.length)
        return true;
    if (suffix.length > s.length)
        return false;
    return (s.substr(s.length - suffix.length) == suffix);
}

export function isEmptyOrNull(s: string) {
    return s == null || s.length === 0;
}

export function isTrimmedEmpty(s: string) {
    return trimToNull(s) == null;
}

export function padLeft(s: string | number, len: number, ch: string = ' ') {
    if (s["padStart"])
        return s["padStart"](len, ch);
    s = s.toString();
    while (s.length < len)
        s = ch + s;
    return s;
}

export function startsWith(s: string, prefix: string): boolean {
    if (prefix == null || !prefix.length)
        return true;
    if (prefix.length > s.length)
        return false;
    return (s.substr(0, prefix.length) == prefix);
}

export function toSingleLine(str: string) {
    return replaceAll(replaceAll(trimToEmpty(str), '\r\n', ' '), '\n', ' ').trim();
}

export function today(): Date {
    var d = new Date();
    return new Date(d.getFullYear(), d.getMonth(), d.getDate());
}

export var trimEnd = function(s: string) {
    return s.replace(/\s*$/, '');
};

export var trimStart = function(s: string) {
    return s.replace(/^\s*/, '');
};

export function trim(s: string) {
    if (s == null)
        return '';
    return s.replace(new RegExp('^\\s+|\\s+$', 'g'), '');
}

export function trimToEmpty(s: string) {
    if (s == null || s.length === 0)
        return '';

    return trim(s);
}

export function trimToNull(s: string) {
    s = trim(s);
    if (s.length === 0)
        return null;
    return s;
}

export function replaceAll(s: string, f: string, r: string): string {
    s = s || '';
    return s.split(f).join(r);
}

export function zeroPad(n: number, digits: number): string {
    let s = n.toString();
    while (s.length < digits)
        s = "0" + s;
    return s;
}