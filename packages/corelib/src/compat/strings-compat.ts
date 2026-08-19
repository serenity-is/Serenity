/**
 * Determines whether a string ends with the specified suffix.
 * @deprecated Use {@link String.prototype.endsWith} directly — e.g. `s.endsWith(suffix)`.
 * @param s - The string to test.
 * @param suffix - The suffix to look for at the end of `s`.
 * @returns `true` if `s` ends with `suffix`; otherwise `false`.
 */
export function endsWith(s: string, suffix: string): boolean {
    return s.endsWith(suffix);
}

/**
 * Determines whether a string is `null`, `undefined`, or empty (`""`).
 * @deprecated Prefer a direct falsy check `!s` or `s == null || s.length === 0` over this shim.
 * @param s - The string to test; may be `null` or `undefined`.
 * @returns `true` if `s` is `null`/`undefined` or has zero length.
 */
export function isEmptyOrNull(s: string) {
    return s == null || s.length === 0;
}

/**
 * Determines whether a string is `null`, `undefined`, empty, or whitespace-only.
 * @deprecated Prefer `!s?.trim()` over this shim.
 * @param s - The string to test; may be `null` or `undefined`.
 * @returns `true` if `s` is `null`/`undefined`, empty, or contains only whitespace.
 */
export function isTrimmedEmpty(s: string) {
    return !s?.trim();
}

/**
 * Pads the string representation of `s` on the left to reach `len` characters.
 * @deprecated Use {@link String.prototype.padStart} directly — e.g. `String(s ?? "").padStart(len, ch)`.
 * @param s - The value to pad; `null`/`undefined` is treated as an empty string.
 * @param len - The desired total length after padding.
 * @param ch - The character to pad with. Defaults to a single space.
 * @returns The left-padded string; already-longer strings are returned unchanged.
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
 * Determines whether a string starts with the specified prefix.
 * @deprecated Use {@link String.prototype.startsWith} directly — e.g. `s.startsWith(prefix)`.
 * @param s - The string to test.
 * @param prefix - The prefix to look for at the start of `s`.
 * @returns `true` if `s` starts with `prefix`; otherwise `false`.
 */
export function startsWith(s: string, prefix: string): boolean {
    return s.startsWith(prefix);
}

/**
 * Collapses a string to a single line by replacing CR/LF and LF with spaces and trimming the result.
 * @param str - The input string; `null`/`undefined` is treated as an empty string.
 * @returns The single-line, trimmed string.
 */
export function toSingleLine(str: string) {
    return replaceAll(replaceAll(trimToEmpty(str), '\r\n', ' '), '\n', ' ').trim();
}

/**
 * Removes trailing whitespace from a string.
 * @deprecated Use {@link String.prototype.trimEnd} / `trimRight` directly.
 * @param s - The input string; `null`/`undefined` yields `""`.
 * @returns The string without trailing whitespace.
 */
export const trimEnd = function (s: string) {
    return s == null ? "" : ((s as any).trimEnd?.() ?? s.replace(/(?<!\s)\s+$/, ''));
};

/**
 * Removes leading whitespace from a string.
 * @deprecated Use {@link String.prototype.trimStart} / `trimLeft` directly.
 * @param s - The input string; `null`/`undefined` yields `""`.
 * @returns The string without leading whitespace.
 */
export const trimStart = function (s: string) {
    return s == null ? "" : ((s as any).trimStart?.() ?? s.replace(/^\s*/, ''));
};

/**
 * Removes leading and trailing whitespace from a string.
 * @deprecated Use {@link String.prototype.trim} directly — this shim exists only for legacy `Q.trim` call sites.
 * @param s - The input string; `null`/`undefined` yields `undefined` (optional-chain semantics).
 * @returns The trimmed string, or `undefined` if `s` is `null`/`undefined`.
 */
export function trim(s: string) {
    return s?.trim();
}

/**
 * Trims leading and trailing whitespace, coercing `null`/`undefined` to an empty string.
 * @param s - The input string; `null`/`undefined` is treated as `""`.
 * @returns The trimmed string, or `""` if the input is `null`/`undefined`.
 */
export function trimToEmpty(s: string) {
    return (s ?? "").trim();
}

/**
 * Trims leading and trailing whitespace, returning `null` for empty or whitespace-only results.
 * @param s - The input string; `null`/`undefined` yields `null` directly.
 * @returns The trimmed string, or `null` if the input is `null`/`undefined` or trims to `""`.
 */
export function trimToNull(s: string) {
    if (s == null)
        return null;
    s = trim(s);
    return s.length === 0 ? null : s;
}

/**
 * Replaces all occurrences of `find` in `str` with `replace`.
 * @deprecated Prefer {@link String.prototype.replaceAll} when targeting modern runtimes; this shim falls back to `split/join`.
 * @param str - The source string; `null`/`undefined` is treated as `""`.
 * @param find - The substring to search for. Must be a non-empty string.
 * @param replace - The replacement string.
 * @returns A new string with all occurrences replaced.
 */
export function replaceAll(str: string, find: string, replace: string): string {
    str = String(str ?? '');
    return (str as any).replaceAll?.(find, replace) ?? str.split(find).join(replace);
}

/**
 * Left-pads the decimal representation of `n` with `"0"` to reach `len` characters.
 * @param n - The number to format; `null`/`undefined` yields `""`.
 * @param len - The desired total length of the resulting string.
 * @returns The zero-padded string.
 */
export function zeroPad(n: number, len: number): string {
    if (n == null)
        return "";
    let s = n.toString();
    while (s.length < len)
        s = "0" + s;
    return s;
}