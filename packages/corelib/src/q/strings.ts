/**
 * Checks if the string ends with the specified substring.
 * @param s String to check.
 * @param suffix Suffix to check.
 * @returns True if the string ends with the specified substring.
 */
export function endsWith(s: string, suffix: string): boolean {
    return s.endsWith(suffix);
}

/**
 * Checks if the string is empty or null.
 * @param s String to check.
 * @returns True if the string is empty or null.
 */
export function isEmptyOrNull(s: string) {
    return s == null || s.length === 0;
}

/**
 * Checks if the string is empty or null or whitespace.
 * @param s String to check.
 * @returns True if the string is empty or null or whitespace.
 */
export function isTrimmedEmpty(s: string) {
    return trimToNull(s) == null;
}

/**
 * Pads the string to the left with the specified character.
 * @param s String to pad.
 * @param len Target length of the string.
 * @param ch Character to pad with.
 * @returns Padded string.
 */
export function padLeft(s: string | number, len: number, ch: string = ' ') {
    s = s == null ? '' : s.toString();
    if ((s as any).padStart)
        return (s as any).padStart(len, ch);
    while (s.length < len)
        s = ch + s;
    return s;
}

/**
 * Checks if the string starts with the prefix
 * @param s String to check.
 * @param prefix Prefix to check.
 * @returns True if the string starts with the prefix.
 */
export function startsWith(s: string, prefix: string): boolean {
    return s.startsWith(prefix);
}

/**
 * Converts the string to single line by removing line end characters
 * @param str String to convert.
 */
export function toSingleLine(str: string) {
    return replaceAll(replaceAll(trimToEmpty(str), '\r\n', ' '), '\n', ' ').trim();
}

/**
 * Trims the whitespace characters from the end of the string
 */
export var trimEnd = function(s: string) {
    return (s ?? "" as any).trimEnd?.() ?? s.replace(/\s*$/, '');
};

/**
 * Trims the whitespace characters from the start of the string
 */
export var trimStart = function(s: string) {
    return (s ?? "" as any).trimStart?.() ?? s.replace(/^\s*/, '');
};

/**
 * Trims the whitespace characters from the start and end of the string
 * This returns empty string even when the string is null or undefined.
 */
export function trim(s: string) {
    return s?.trim() ?? '';
}

/**
 * Trims the whitespace characters from the start and end of the string
 * Returns empty string if the string is null or undefined.
 */
export function trimToEmpty(s: string) {
    return (s ?? "").trim();
}

/**
 * Trims the whitespace characters from the start and end of the string
 * Returns null if the string is null, undefined or whitespace.
 */
export function trimToNull(s: string) {
    if (s == null)
        return null;
    s = trim(s);
    return s.length === 0 ? null : s;
}

/**
 * Replaces all occurrences of the search string with the replacement string.
 * @param str String to replace.
 * @param find String to find.
 * @param replace String to replace with.
 * @returns Replaced string.
 */
export function replaceAll(str: string, find: string, replace: string): string {
    str = str || '';
    return (str as any).replaceAll?.(find, replace) ?? str.split(find).join(replace);
}

/**
 * Pads the start of string to make it the specified length.
 * @param s String to pad.
 * @param len Target length of the string.
 */
export function zeroPad(n: number, len: number): string {
    if (n == null)
        return "";
    let s = n.toString();
    while (s.length < len)
        s = "0" + s;
    return s;
}