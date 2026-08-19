import { Culture, parseInteger, stringFormat, stringFormatLocale } from "../base";
/**
 * Lowercases a string with Turkish-specific handling (`İ` → `i`, `I` → `ı`).
 * @param a - Input string; if falsy, returned as-is.
 * @returns Lowercased string with Turkish dotted/dotless-I mapping preserved.
 * @remarks Compat shim retained because native `String.prototype.toLocaleLowerCase('tr')` behaves differently across engines; prefer locale-aware APIs for new code.
 * @deprecated Retained for legacy `Q.turkishLocaleToLower` call sites.
 * @example
 * turkishLocaleToLower("İSTANBUL"); // "istanbul" with ı handling
 */
export function turkishLocaleToLower(a: string): string {
    if (!a)
        return a;
    return a.replace(/İ/g, 'i').replace(/I/g, 'ı').toLowerCase();
}

/**
 * Uppercases a string with Turkish-specific handling (`i` → `İ`, `ı` → `I`).
 * @param a - Input string; if falsy, returned as-is.
 * @returns Uppercased string with Turkish dotted/dotless-I mapping preserved.
 * @remarks Compat shim; for new code prefer `toLocaleUpperCase('tr')`.
 * @deprecated Retained for legacy `Q.turkishLocaleToUpper` call sites.
 * @example
 * turkishLocaleToUpper("istanbul"); // handles dotted i
 */
export function turkishLocaleToUpper(a: string): string {
    if (!a)
        return a;
    return a.replace(/i/g, 'İ').replace(/ı/g, 'I').toUpperCase();
}

/**
 * Legacy alias for {@link Culture.stringCompare}.
 * @deprecated Use `Culture.stringCompare` directly.
 * @see {@link Culture.stringCompare}
 */
export let turkishLocaleCompare = Culture.stringCompare;

/**
 * Legacy alias for {@link stringFormat} (`Q.format`).
 * @deprecated Use {@link stringFormat} directly.
 * @see {@link stringFormat}
 */
export let format = stringFormat;

/**
 * Legacy alias for {@link stringFormatLocale}.
 * @deprecated Use {@link stringFormatLocale} directly.
 * @see {@link stringFormatLocale}
 */
export let localeFormat = stringFormatLocale;

/**
 * Formats a duration given in minutes as `"d.hh:mm"` (days, hours, minutes).
 * @param n - Total minutes; `null`/`undefined` yields `""`, `0` yields `"0"`.
 * @returns Formatted string — e.g. `1500` → `"1.01:00"`, `90` → `"01:30"`.
 * @remarks Days are omitted when zero; minutes part `"00:00"` is omitted when zero unless days is also zero. Compat helper from `Q.formatDayHourAndMin`.
 * @example
 * formatDayHourAndMin(1500); // "1.01:00"
 * formatDayHourAndMin(0);    // "0"
 */
export function formatDayHourAndMin(n: number): string {
    if (n == null)
        return "";
    if (n === 0)
        return '0';
    let days = Math.floor(n / 24 / 60);
    let txt = "";
    if (days > 0) {
        txt += days.toString();
    }
    let mins = Math.floor((n % (24 * 60)) / (60)).toString().padStart(2, '0') + ':' + (n % 60).toString().padStart(2, '0');
    if (mins != '00:00') {
        if (days > 0)
            txt += ".";
        txt += mins;
    }
    return txt;
}

/**
 * Parses a `"hh:mm"` time string into total minutes.
 * @param value - String to parse (accepts `h:mm` or `hh:mm`; surrounding whitespace is trimmed).
 * @returns Total minutes (`h*60+m`), `null` for empty/whitespace input, or `NaN` if the format or range is invalid (hours must be 0–23, minutes 0–59, length 4–5 chars).
 * @remarks Compat helper from `Q.parseHourAndMin`.
 * @example
 * parseHourAndMin("02:30"); // 150
 * parseHourAndMin("2:05");  // 125
 */
export function parseHourAndMin(value: string): number {
    let v = value?.trim() ?? '';
    if (!v.length)
        return null;
    if (v.length < 4 || v.length > 5)
        return NaN;
    let h: number, m: number;
    if (v.charAt(1) == ':') {
        h = parseInteger(v.substring(0, 1));
        m = parseInteger(v.substring(2, 4));
    }
    else {
        if (v.charAt(2) != ':')
            return NaN;
        h = parseInteger(v.substring(0, 2));
        m = parseInteger(v.substring(3, 5));
    }
    if (isNaN(h) || isNaN(m) || h < 0 || h > 23 || m < 0 || m > 59)
        return NaN;
    return h * 60 + m;
}

/**
 * Parses a `"d.hh:mm"` duration string into total minutes (also accepts plain `"hh:mm"` or day count).
 * @param s - String to parse; whitespace is trimmed.
 * @returns Total minutes, `null` for empty input, or `NaN` for invalid format/range (hours 0–23, minutes 0–59).
 * @remarks Accepts `"d"` (days), `"hh:mm"`, or `"d.hh:mm"` (two-part split on `.`). Delegates the time part to {@link parseHourAndMin}. Compat helper from `Q.parseDayHourAndMin`.
 * @example
 * parseDayHourAndMin("1.01:00"); // 1500
 * parseDayHourAndMin("01:30");   // 90
 */
export function parseDayHourAndMin(s: string): number {
    let days: number;
    let v = s?.trim() ?? '';
    if (!v.length)
        return null;
    let p = v.split('.');
    if (p.length == 1) {
        days = parseInteger(p[0]);
        if (!isNaN(days))
            return days * 24 * 60;
        return parseHourAndMin(p[0]);
    }
    else if (p.length == 2) {
        days = parseInteger(p[0]);
        let hm = parseHourAndMin(p[1]);
        if (isNaN(days) || hm == null || isNaN(hm))
            return NaN;
        return days * 24 * 60 + hm;
    }
    else
        return NaN;
}