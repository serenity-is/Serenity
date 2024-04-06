import { Culture, parseInteger, stringFormat, stringFormatLocale } from "../base";
/**
 * A string to lowercase function that handles special Turkish
 * characters like 'ı'. Left in for compatibility reasons.
 */
export function turkishLocaleToLower(a: string): string {
    if (!a)
        return a;
    return a.replace(/i/g, 'İ').replace(/ı/g, 'I').toLowerCase();
}

/**
 * A string to uppercase function that handles special Turkish
 * characters like 'ı'. Left in for compatibility reasons.
 */
export function turkishLocaleToUpper(a: string): string {
    if (!a)
        return a;
    return a.replace(/i/g, 'İ').replace(/ı/g, 'I').toUpperCase();
}

/**
 * This is an alias for Culture.stringCompare, left in for compatibility reasons.
 */
export let turkishLocaleCompare = Culture.stringCompare;

/** @deprecated Use stringFormat */
export let format = stringFormat;

/** @deprecated Use stringFormatLocale */
export let localeFormat = stringFormatLocale;

/**
 * Formats a number containing number of minutes into a string in the format "d.hh:mm".
 * @param n The number of minutes.
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
 * Parses a time string in the format "hh:mm" into a number containing number of minutes.
 * Returns NaN if the hours not in range 0-23 or minutes not in range 0-59.
 * @param value The string to parse.
 */
export function parseHourAndMin(value: string): number {
    let v = value?.trim() ?? '';
    if (!v.length)
        return null;
    if (v.length < 4 || v.length > 5)
        return NaN;
    let h: number, m: number;
    if (v.charAt(1) == ':') {
        h = parseInteger(v.substr(0, 1));
        m = parseInteger(v.substr(2, 2));
    }
    else {
        if (v.charAt(2) != ':')
            return NaN;
        h = parseInteger(v.substr(0, 2));
        m = parseInteger(v.substr(3, 2));
    }
    if (isNaN(h) || isNaN(m) || h < 0 || h > 23 || m < 0 || m > 59)
        return NaN;
    return h * 60 + m;
}

/**
 * Parses a string in the format "d.hh:mm" into a number containing number of minutes.
 * Returns NaN if the hours not in range 0-23 or minutes not in range 0-59.
 * Returns NULL if the string is empty or whitespace.
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
        if (isNaN(days) || !hm || isNaN(hm))
            return NaN;
        return days * 24 * 60 + hm;
    }
    else
        return NaN;
}