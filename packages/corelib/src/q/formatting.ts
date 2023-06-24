import { padLeft, startsWith, trim, trimToNull, zeroPad } from "./strings";

/**
 * Interface for number formatting, similar to .NET's NumberFormatInfo
 */
export interface NumberFormat {
    /** Decimal separator */
    decimalSeparator: string;
    /** Group separator */
    groupSeparator?: string;
    /** Number of digits after decimal separator */
    decimalDigits?: number;
    /** Positive sign */
    positiveSign?: string;
    /** Negative sign */
    negativeSign?: string;
    /** Zero symbol */
    nanSymbol?: string;
    /** Percentage symbol */
    percentSymbol?: string;
    /** Currency symbol */
    currencySymbol?: string;
}

/** Interface for date formatting, similar to .NET's DateFormatInfo */
export interface DateFormat {
    /** Date separator */
    dateSeparator?: string;
    /** Default date format string */
    dateFormat?: string;
    /** Date order, like dmy, or ymd */
    dateOrder?: string;
    /** Default date time format string */
    dateTimeFormat?: string;
    /** AM designator */
    amDesignator?: string;
    /** PM designator */
    pmDesignator?: string;
    /** Time separator */
    timeSeparator?: string;
    /** First day of week, 0 = Sunday, 1 = Monday */
    firstDayOfWeek?: number;
    /** Array of day names */
    dayNames?: string[];
    /** Array of short day names */
    shortDayNames?: string[];
    /** Array of two letter day names */
    minimizedDayNames?: string[];
    /** Array of month names */
    monthNames?: string[];
    /** Array of short month names */
    shortMonthNames?: string[];
}

/** Interface for a locale, similar to .NET's CultureInfo */
export interface Locale extends NumberFormat, DateFormat {
    /** Locale string comparison function, similar to .NET's StringComparer */
    stringCompare?: (a: string, b: string) => number;
    /** Locale string to upper case function */
    toUpper?: (a: string) => string;
}

/** Invariant locale (e.g. CultureInfo.InvariantCulture) */
export let Invariant: Locale = {
    decimalSeparator: '.',
    groupSeparator: ',',
    decimalDigits: 2,
    negativeSign: '-',
    positiveSign: '+',
    percentSymbol: '%',
    currencySymbol: '$',
    dateSeparator: '/',
    dateOrder: 'mdy',
    dateFormat: 'MM/dd/yyyy',
    dateTimeFormat: 'MM/dd/yyyy HH:mm:ss',
    amDesignator: 'AM',
    pmDesignator: 'PM',
    timeSeparator: ':',
    firstDayOfWeek: 0,
    dayNames: ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'],
    shortDayNames: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
    minimizedDayNames: ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'],
    monthNames: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December', ''],
    shortMonthNames: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec', ''],
    stringCompare: (a, b) => a < b ? -1 : (a > b ? 1 : 0)
}

/** 
 * Factory for a function that compares two strings, based on a character order 
 * passed in the `order` argument.
 */
export function compareStringFactory(order: string): ((a: string, b: string) => number) {

    var o: { [key: string]: number } = {};
    for (let z = 0; z < order.length; z++) {
        o[order.charAt(z)] = z + 1;
    }

    return function (a: string, b: string) {
        a = a || "";
        b = b || "";
        if (a == b)
            return 0;

        let c: number;
        for (let i = 0, _len = Math.min(a.length, b.length); i < _len; i++) {
            let x = a.charAt(i), y = b.charAt(i);
            if (x === y) {
                continue;
            }
            let ix = o[x], iy = o[y];
            if (ix != null && iy != null)
                return ix < iy ? -1 : 1;
            c = x.localeCompare(y);
            if (c == 0)
                continue;
            return c;
        }
        if (c != null)
            return c;
        return a.localeCompare(b);
    }
}

/**
 * Current culture, e.g. CultureInfo.CurrentCulture. This is overridden by
 * settings passed from a `<script>` element in the page with id `ScriptCulture`
 * containing a JSON object if available. This element is generally created in 
 * the _LayoutHead.cshtml file for Serenity applications, so that the culture
 * settings determined server, can be passed to the client.
 */
export let Culture: Locale = {
    decimalSeparator: '.',
    groupSeparator: ',',
    dateSeparator: '/',
    dateOrder: 'dmy',
    dateFormat: 'dd/MM/yyyy',
    dateTimeFormat: 'dd/MM/yyyy HH:mm:ss',
    stringCompare: compareStringFactory("AaBbCcÇçFfGgĞğHhIıİiJjKkLlMmNnOoÖöPpRrSsŞşTtUuÜüVvYyZz")
};

(function () {
    let k: string;
    for (k in Invariant)
        if ((Culture as any)[k] === undefined && Object.prototype.hasOwnProperty.call(Invariant, k))
            (Culture as any)[k] = (Invariant as any)[k];

    if (typeof document !== "undefined" && (k = trimToNull(document.querySelector('script#ScriptCulture')?.textContent)) != null) {
        var sc = JSON.parse(k);
        if (sc.DecimalSeparator != null)
            Culture.decimalSeparator = sc.DecimalSeparator;
        if (sc.GroupSeparator != null && sc.GroupSeparator != Culture.decimalSeparator)
            Culture.groupSeparator = sc.GroupSeparator;
        else if (Culture.groupSeparator == Culture.decimalSeparator)
            Culture.groupSeparator = Culture.decimalSeparator == '.' ? ',' : '.';

        delete sc.GroupSeparator;
        delete sc.DecimalSeparator;
        for (k in sc) {
            if ((Culture as any)[k] === undefined && Object.prototype.hasOwnProperty.call(sc, k))
                (Culture as any)[k.charAt(0).toLowerCase() + k.substr(1)] = sc[k];
        }
    }
})();

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

function insertGroupSeperator(num: string, dec: string, grp: string, neg: string) {
    var decPart = null;
    var decIndex = num.indexOf(dec);
    if (decIndex > 0) {
        decPart = num.substring(decIndex);
        num = num.substr(0, decIndex);
    }

    var negative = startsWith(num, neg);
    if (negative) {
        num = num.substring(1);
    }

    var groupSize = 3;
    if (num.length < groupSize) {
        return (negative ? neg : '') + (decPart ? num + decPart : num);
    }

    var index = num.length;
    var s = '';
    var done = false;
    while (!done) {
        var length = groupSize;
        var startIndex = index - length;
        if (startIndex < 0) {
            groupSize += startIndex;
            length += startIndex;
            startIndex = 0;
            done = true;
        }

        if (!length)
            break;

        var part = num.substring(startIndex, startIndex + length);
        if (s.length)
            s = part + grp + s;
        else
            s = part;
        index -= length;
    }

    if (negative)
        s = '-' + s;
    return decPart ? s + decPart : s;
}

var _formatRE = /\{\{|\}\}|\{[^\}\{]+\}/g;

function _formatString(format: string, l: Locale, values: IArguments, from: number) {

    return format.replace(_formatRE,
        function (m) {
            if (m === '{{' || m === '}}')
                return m.charAt(0);
            var index = parseInt(m.substr(1), 10);
            var value = values[index + from];
            if (value == null) {
                return '';
            }
            var formatSpec = null;
            var formatIndex = m.indexOf(':');
            if (formatIndex > 0) {
                formatSpec = m.substring(formatIndex + 1, m.length - 1);
            }
            return _formatObject(value, formatSpec, l);
        });
};


/**
 * Formats a string with parameters similar to .NET's String.Format function
 * using current `Culture` locale settings.
 */
export function format(format: string, ...prm: any[]): string {
    return _formatString(format, Culture, arguments, 1);
}

/**
 * Formats a string with parameters similar to .NET's String.Format function
 * using the locale passed as the first argument.
 */
export function localeFormat(l: Locale, format: string, ...prm: any[]): string {
    return _formatString(format, l, arguments, 2);
}

function _formatObject(obj: any, format: string, fmt?: Locale): string {
    if (typeof (obj) === 'number')
        return formatNumber(obj, format, fmt);
    else if (Object.prototype.toString.call(obj) === '[object Date]')
        return formatDate(obj, format, fmt);
    else if (obj.format)
        return obj.format(format, fmt ?? Culture);
    return String(obj);
};

/** 
 * Rounds a number to specified digits or an integer number if digits are not specified.
 * @param n the number to round 
 * @param d the number of digits to round to. default is zero.
 * @param rounding whether to use banker's rounding
 * @returns the rounded number 
 */
export let round = (n: number, d?: number, rounding?: boolean) => {
    var m = Math.pow(10, d || 0);
    n *= m;
    var sign = ((n > 0) as any) | -(n < 0);
    if (n % 1 === 0.5 * sign) {
        var f = Math.floor(n);
        return (f + (rounding ? (sign > 0) as any : (f % 2 * sign))) / m;
    }

    return Math.round(n) / m;
};

/**
 * Truncates a number to an integer number.
 */
export let trunc = (n: number): number => n != null ? (n > 0 ? Math.floor(n) : Math.ceil(n)) : null;

/**
 * Formats a number using the current `Culture` locale (or the passed locale) settings.
 * It supports format specifiers similar to .NET numeric formatting strings.
 * @param num the number to format
 * @param format the format specifier. default is 'g'.
 * See .NET numeric formatting strings documentation for more information.
 */
export function formatNumber(num: number, format?: string, decOrLoc?: string | NumberFormat, grp?: string): string {

    if (num == null)
        return "";

    var fmt: NumberFormat = typeof decOrLoc !== "string" ? (decOrLoc ?? Culture) : {
        decimalSeparator: decOrLoc,
        groupSeparator: grp ?? (decOrLoc == "," ? "." : ",")
    }

    if (isNaN(num)) {
        return fmt.nanSymbol ?? Culture.nanSymbol;
    }

    if (format === 'i') {
        return num.toString();
    }

    if (format == null || format == '') {
        format = 'g';
    }

    var dec = fmt.decimalSeparator ?? Culture.decimalSeparator;
    grp = grp ?? fmt.groupSeparator ?? Culture.groupSeparator;
    var neg = fmt.negativeSign ?? Culture.negativeSign;

    var s = '';
    var precision = -1;

    if (format.length > 1) {
        precision = parseInt(format.substring(1), 10);
    }

    var fs = format.charAt(0);
    switch (fs) {
        case 'g':
        case 'G':
            if (precision != -1)
                s = num.toFixed(precision);
            else
                s = num.toString();
            if (dec != '.')
                s = s.replace('.', dec);
            break;
        case 'd':
        case 'D':
            s = parseInt(Math.abs(num) as any).toString();
            if (precision != -1)
                s = padLeft(s, precision, '0');
            if (num < 0)
                s = neg + s;
            break;
        case 'x': case 'X':
            s = parseInt(Math.abs(num) as any).toString(16);
            if (fs == 'X')
                s = s.toUpperCase();
            if (precision != -1)
                s = padLeft(s, precision, '0');
            break;
        case 'e':
        case 'E':
            if (precision == -1)
                s = num.toExponential(6);
            else
                s = num.toExponential(precision);
            if (fs == 'E')
                s = s.toUpperCase();
            break;
        case 'f':
        case 'F':
        case 'n':
        case 'N':
            if (precision == -1) {
                precision = fmt.decimalDigits ?? Culture.decimalDigits;
            }
            s = num.toFixed(precision).toString();
            if (precision && (dec != '.')) {
                var index = s.indexOf('.');
                s = s.substring(0, index) + dec + s.substring(index + 1);
            }
            if ((fs == 'n') || (fs == 'N')) {
                s = insertGroupSeperator(s, dec, grp, neg);
            }
            break;
        case 'c': case 'C':
        case 'p': case 'P':
            if (precision == -1) {
                precision = fmt.decimalDigits ?? Culture.decimalDigits;
            }
            var symbol: string;
            if (fs === 'p' || fs == 'P') {
                num *= 100;
                symbol = fmt.percentSymbol ?? Culture.percentSymbol;
            }
            else {
                symbol = fmt.currencySymbol ?? Culture.currencySymbol;
            }
            s = num.toFixed(precision).toString();
            if (precision && (dec != '.')) {
                var index = s.indexOf('.');
                s = s.substring(0, index) + dec + s.substring(index + 1);
            }
            s = insertGroupSeperator(s, dec, grp, neg) + symbol;
            break;

        default:
            var prefix = '';
            var mid = '';
            var suffix = '';
            var endPrefix = false;
            var inQuote = false;
            for (var i = 0; i < format.length; i++) {
                var c = format.charAt(i);
                if (c == "'") {
                    inQuote = !inQuote;
                    continue;
                }
                else if (!inQuote) {
                    if (c == '\\') {
                        var c = (format.charAt(i + 1) || '');
                        i++;
                    }
                    else if (c == '#' || c == ',' || c == '.' || c == '0') {
                        endPrefix = true;
                        mid += c;
                        continue;
                    }
                }
                endPrefix ? (suffix += c) : (prefix += c);
            }

            format = mid;

            let r = "";
            if (format.indexOf(".") > -1) {
                let dp = dec;
                let df = format.substring(format.lastIndexOf(".") + 1);
                num = roundNumber(num, df.length);
                let dv = num % 1;
                let ds = new String(dv.toFixed(df.length));
                ds = ds.substring(ds.lastIndexOf(".") + 1);
                for (let i = 0; i < df.length; i++) {
                    if (df.charAt(i) == '#' && ds.charAt(i) != '0') {
                        dp += ds.charAt(i);
                        continue;
                    }
                    else if (df.charAt(i) == '#' && ds.charAt(i) == '0') {
                        let notParsed = ds.substring(i);
                        if (notParsed.match('[1-9]')) {
                            dp += ds.charAt(i);
                            continue;
                        }
                        else
                            break;
                    }
                    else if (df.charAt(i) == "0")
                        dp += ds.charAt(i);
                    else
                        dp += df.charAt(i);
                }
                r += dp;
            }
            else
                num = Math.round(num);

            let ones = Math.floor(num);
            if (num < 0)
                ones = Math.ceil(num);
            let of = "";
            if (format.indexOf(".") == -1)
                of = format;
            else
                of = format.substring(0, format.indexOf("."));

            let op = "";
            if (!(ones == 0 && of.substr(of.length - 1) == '#')) {
                // find how many digits are in the group
                let oneText = new String(Math.abs(ones));
                let gl = 9999;
                if (of.lastIndexOf(",") != -1)
                    gl = of.length - of.lastIndexOf(",") - 1;
                let gc = 0;
                for (let i = oneText.length - 1; i > -1; i--) {
                    op = oneText.charAt(i) + op;
                    gc++;
                    if (gc == gl && i != 0) {
                        op = grp + op;
                        gc = 0;
                    }
                }

                // account for any pre-data padding
                if (of.length > op.length) {
                    let padStart = of.indexOf('0');
                    if (padStart != -1) {
                        let padLen = of.length - padStart;
                        // pad to left with 0's or group char
                        let pos = of.length - op.length - 1;
                        while (op.length < padLen) {
                            let pc = of.charAt(pos);
                            // replace with real group char if needed
                            if (pc == ',')
                                pc = grp;
                            op = pc + op;
                            pos--;
                        }
                    }
                }
            }

            if (!op && of.indexOf('0', of.length - 1) !== -1)
                op = '0';

            r = op + r;
            if (num < 0)
                r = neg + r;

            if (r.lastIndexOf(dec) == r.length - 1) {
                r = r.substring(0, r.length - 1);
            }

            return prefix + r + suffix;
    }

    return s;
}

/**
 * Converts a string to an integer. The difference between parseInt and parseInteger 
 * is that parseInteger will return null if the string is empty or null, whereas
 * parseInt will return NaN and parseInteger will use the current culture's group
 * and decimal separators.
 * @param s the string to parse
 */
export function parseInteger(s: string): number {
    if (s == null)
        return null;
    s = trim(s.toString());
    if (!s.length)
        return null;
    let ts = Culture.groupSeparator;
    if (s && s.length && s.indexOf(ts) > 0) {
        s = s.replace(new RegExp("(\\b\\d{1,3})\\" + ts + "(?=\\d{3}(\\D|$))", "g"), '$1');
    }
    if (!(/^[-\+]?\d+$/.test(s)))
        return NaN;
    return parseInt(s, 10);
}

/**
 * Converts a string to a decimal. The difference between parseFloat and parseDecimal
 * is that parseDecimal will return null if the string is empty or null, whereas
 * parseFloat will return NaN and parseDecimal will use the current culture's group
 * and decimal separators.
 * @param s the string to parse
 */
export function parseDecimal(s: string): number {
    if (s == null)
        return null;

    s = trim(s.toString());
    if (s.length == 0)
        return null;

    let ts = Culture.groupSeparator;

    if (s && s.length && s.indexOf(ts) > 0) {
        s = s.replace(new RegExp("(\\b\\d{1,3})\\" + ts + "(?=\\d{3}(\\D|$))", "g"), '$1');
    }

    if (!(new RegExp("^\\s*([-\\+])?(\\d*)\\" + Culture.decimalSeparator + "?(\\d*)\\s*$").test(s)))
        return NaN;

    return parseFloat(s.toString().replace(Culture.decimalSeparator, '.'));
}

/** 
 * Rounds a number to a specified number of decimal places.
 * @param n the number to round
 * @param dec the number of decimal places to round to
 * @returns the rounded number
 */
function roundNumber(n: number, dec?: number): number {
    let power = Math.pow(10, dec || 0);
    let value = (Math.round(n * power) / power).toString();
    // ensure the decimal places are there
    if (dec > 0) {
        let dp = value.indexOf(".");
        if (dp == -1) {
            value += '.';
            dp = 0;
        }
        else {
            dp = value.length - (dp + 1);
        }
        while (dp < dec) {
            value += '0';
            dp++;
        }
    }
    return parseFloat(value);
}

/**
 * Converts a string to an ID. If the string is a number, it is returned as-is.
 * If the string is empty, null or whitespace, null is returned.
 * Otherwise, it is converted to a number if possible. If the string is not a
 * valid number or longer than 14 digits, the trimmed string is returned as-is.
 * @param id the string to convert to an ID
 */
export function toId(id: any): any {
    if (id == null)
        return null;
    if (typeof id == "number")
        return id;
    if (typeof id == "string")
        id = trim(id);
    if (!id.length)
        return null;
    if (id.length >= 15 || !(/^-?\d+$/.test(id)))
        return id;
    return parseInt(id, 10);
}

var _dateFormatRE = /'.*?[^\\]'|dddd|ddd|dd|d|MMMM|MMM|MM|M|yyyy|yy|y|hh|h|HH|H|mm|m|ss|s|tt|t|fff|ff|f|zzz|zz|z|\//g;

/** 
 * Formats a date using the specified format string and optional culture.
 * Supports .NET style format strings including custom formats.
 * See .NET documentation for supported formats.
 * @param d the date to format. If null, it returns empty string.
 * @param format the format string to use. If null, it uses the current culture's default format.
 * 'G' uses the culture's datetime format.
 * 'g' uses the culture's datetime format with secs removed. 
 * 'd' uses the culture's date format.
 * 't' uses the culture's time format.
 * 'u' uses the sortable ISO format with UTC time.
 * 'U' uses the culture's date format with UTC time.
 * @param locale the locale to use
 * @returns the formatted date
 * @example
 * // returns "2019-01-01"
 * formatDate(new Date(2019, 0, 1), "yyyy-MM-dd");
 * @example
 * // returns "2019-01-01 12:00:00"
 * formatDate(new Date(2019, 0, 1, 12), "yyyy-MM-dd HH:mm:ss");
 * @example
 * // returns "2019-01-01 12:00:00.000"
 * formatDate(new Date(2019, 0, 1, 12), "yyyy-MM-dd HH:mm:ss.fff");
 * @example
 * // returns "2019-01-01 12:00:00.000 AM"
 * formatDate(new Date(2019, 0, 1, 12), "yyyy-MM-dd HH:mm:ss.fff tt");
 */
export function formatDate(d: Date | string, format?: string, locale?: Locale) {
    if (!d)
        return '';

    let date: Date;
    if (typeof d == "string") {
        date = parseDate(d, locale?.dateOrder);
        if (!date)
            return '';

        if (isNaN(date.valueOf()))
            return d;
    }
    else
        date = d;

    if (format == 'i')
        return date.toString();
    if (format == 'id')
        return date.toDateString();
    if (format == 'it')
        return date.toTimeString();

    if (locale == null)
        locale = Culture;

    if (format == null || format == "d")
        format = locale.dateFormat ?? Culture.dateFormat;
    else if (format.length == 1) {
        switch (format) {
            case "g": format = (locale.dateTimeFormat ?? Culture.dateTimeFormat).replace(":ss", ""); break;
            case "G": format = (locale.dateTimeFormat ?? Culture.dateTimeFormat); break;
            case "s": format = "yyyy-MM-ddTHH:mm:ss"; break;
            case 't': format = (locale.dateTimeFormat && locale.dateFormat) ? locale.dateTimeFormat.replace(locale.dateFormat + " ", "") : "HH:mm"; break;
            case 'u':
            case 'U':
                format = format == 'u' ? 'yyyy-MM-ddTHH:mm:ss.fffZ' : locale.dateTimeFormat ?? Culture.dateTimeFormat;
                date = new Date(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(),
                    date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds(), date.getUTCMilliseconds());
                break;
        }
    }

    if (format.charAt(0) == '%') {
        format = format.substr(1);
    }

    var re = _dateFormatRE;
    var sb = [];

    re.lastIndex = 0;
    while (true) {
        var index = re.lastIndex;
        var match = re.exec(format);

        sb.push(format.slice(index, match ? match.index : format.length));
        if (!match) {
            break;
        }

        var fs = match[0];
        var part: any = fs;
        switch (fs) {
            case '/':
                part = locale.dateSeparator ?? Culture.dateSeparator;
                break;
            case 'dddd':
                part = (locale.dayNames ?? Culture.dayNames)[date.getDay()];
                break;
            case 'ddd':
                part = (locale.shortDayNames ?? Culture.shortDayNames)[date.getDay()];
                break;
            case 'dd':
                part = padLeft(date.getDate().toString(), 2, '0');
                break;
            case 'd':
                part = date.getDate();
                break;
            case 'MMMM':
                part = (locale.monthNames ?? Culture.monthNames)[date.getMonth()];
                break;
            case 'MMM':
                part = (locale.shortMonthNames ?? Culture.shortMonthNames)[date.getMonth()];
                break;
            case 'MM':
                part = padLeft(date.getMonth() + 1, 2, '0');
                break;
            case 'M':
                part = (date.getMonth() + 1);
                break;
            case 'yyyy':
                part = padLeft(date.getFullYear(), 4, '0');
                break;
            case 'yy':
                part = padLeft(date.getFullYear() % 100, 2, '0');
                break;
            case 'y':
                part = date.getFullYear() % 100;
                break;
            case 'h': case 'hh':
                part = date.getHours() % 12;
                if (!part) {
                    part = '12';
                }
                else if (fs == 'hh') {
                    part = padLeft(part, 2, '0');
                }
                break;
            case 'HH':
                part = padLeft(date.getHours(), 2, '0');
                break;
            case 'H':
                part = date.getHours();
                break;
            case 'mm':
                part = padLeft(date.getMinutes(), 2, '0');
                break;
            case 'm':
                part = date.getMinutes();
                break;
            case 'ss':
                part = padLeft(date.getSeconds().toString(), 2, '0');
                break;
            case 's':
                part = date.getSeconds();
                break;
            case 't': case 'tt':
                part = (date.getHours() < 12) ? (locale.amDesignator ?? Culture.amDesignator) : (locale.pmDesignator ?? Culture.pmDesignator);
                if (fs == 't') {
                    part = part.charAt(0);
                }
                break;
            case 'fff':
                part = padLeft(date.getMilliseconds(), 3, '0');
                break;
            case 'ff':
                part = padLeft(date.getMilliseconds(), 3, '0').substr(0, 2);
                break;
            case 'f':
                part = padLeft(date.getMilliseconds(), 3, '0').charAt(0);
                break;
            case 'z':
                part = date.getTimezoneOffset() / 60;
                part = ((part >= 0) ? '-' : '+') + Math.floor(Math.abs(part));
                break;
            case 'zz':
            case 'zzz':
                part = date.getTimezoneOffset() / 60;
                part = ((part >= 0) ? '-' : '+') +
                    padLeft(Math.floor(Math.abs(part)), 2, '0');
                if (fs == 'zzz') {
                    part += (locale.timeSeparator ?? Culture.timeSeparator) +
                        padLeft(Math.abs(date.getTimezoneOffset() % 60), 2, '0');
                }
                break;
            default:
                if (part.charAt(0) == '\'') {
                    part = part.substr(1, part.length - 2).replace(/\\'/g, '\'');
                }
                break;
        }
        sb.push(part);
    }

    return sb.join('');
}

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
    let mins = zeroPad(Math.floor((n % (24 * 60)) / (60)), 2) + ':' + zeroPad(n % 60, 2);
    if (mins != '00:00') {
        if (days > 0)
            txt += ".";
        txt += mins;
    }
    return txt;
}

/**
 * Formats a date as the ISO 8601 UTC date/time format.
 * @param n The number of minutes.
 */
export function formatISODateTimeUTC(d: Date): string {
    if (d == null)
        return "";
    let zeropad = function (num: number) { return ((num < 10) ? '0' : '') + num; };
    let str = d.getUTCFullYear() + "-" +
        zeropad(d.getUTCMonth() + 1) + "-" +
        zeropad(d.getUTCDate()) + "T" +
        zeropad(d.getUTCHours()) + ":" +
        zeropad(d.getUTCMinutes());
    let secs = Number(d.getUTCSeconds() + "." +
        ((d.getUTCMilliseconds() < 100) ? '0' : '') +
        zeropad(d.getUTCMilliseconds()));
    str += ":" + zeropad(secs) + "Z";
    return str;
}

let isoRegexp = /(\d{4,})(?:-(\d{1,2})(?:-(\d{1,2})(?:[T ](\d{1,2}):(\d{1,2})(?::(\d{1,2})(?:\.(\d+))?)?(?:(Z)|([+-])(\d{1,2})(?::(\d{1,2}))?)?)?)?)?/;

/**
 * Parses a string in the ISO 8601 UTC date/time format.
 * @param s The string to parse.
 */
export function parseISODateTime(s: string): Date {
    if (s == null)
        return null;

    if (typeof s !== "string")
        s = s + "";

    if (!s.length)
        return null;

    if (!isoRegexp.test(s))
        return new Date(NaN);

    return new Date(s + (s.length == 10 ? "T00:00:00" : ""));
}

/**
 * Parses a time string in the format "hh:mm" into a number containing number of minutes.
 * Returns NaN if the hours not in range 0-23 or minutes not in range 0-59.
 * @param s The string to parse.
 */
export function parseHourAndMin(value: string): number {
    let v = trim(value);
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
    let v = trim(s);
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

/**
 * Parses a string to a date. If the string is empty or whitespace, returns null.
 * Returns a NaN Date if the string is not a valid date.
 * @param s The string to parse.
 * @param dateOrder The order of the date parts in the string. Defaults to culture's default date order.
  */
export function parseDate(s: string, dateOrder?: string): Date {
    if (!s || !s.length)
        return null;

    s = trim(s);
    if (!s.length)
        return null;

    if (s.length >= 10 && s.charAt(4) === '-' && s.charAt(7) === '-' &&
        (s.length === 10 || (s.length > 10 && s.charAt(10) === 'T'))) {
        return parseISODateTime(s);
    }

    if (s.indexOf(' ') > 0 && s.indexOf(':') > s.indexOf(' ') + 1) {
        var datePart = parseDate(s.substring(0, s.indexOf(' ')));
        if (!datePart || isNaN(datePart.valueOf()))
            return new Date(NaN);
        return parseISODateTime(formatDate(datePart, 'yyyy-MM-dd') + 'T' + trim(s.substring(s.indexOf(' ') + 1)));
    }

    let d: number, m: number, y: number;
    let dArray = splitDateString(s);
    if (dArray.length == 3) {
        if (dArray.some(x => !/^[0-9]+$/.test(x)))
            return new Date(NaN);

        dateOrder = dateOrder || Culture.dateOrder;
        switch (dateOrder) {
            case "dmy":
                d = parseInt(dArray[0], 10);
                m = parseInt(dArray[1], 10) - 1;
                y = parseInt(dArray[2], 10);
                break;
            case "ymd":
                d = parseInt(dArray[2], 10);
                m = parseInt(dArray[1], 10) - 1;
                y = parseInt(dArray[0], 10);
                break;
            case "mdy":
            default:
                d = parseInt(dArray[1], 10);
                m = parseInt(dArray[0], 10) - 1;
                y = parseInt(dArray[2], 10);
                break;
        }

        if (isNaN(d) || isNaN(m) || isNaN(y) || d < 1 || d > 31 || m < 0 || m > 11 || y > 9999 || y < 0)
            return new Date(NaN);

        if (y < 100) {
            let fullYear = new Date().getFullYear();
            let shortYearCutoff = (fullYear % 100) + 10;
            y += fullYear - fullYear % 100 + (y <= shortYearCutoff ? 0 : -100);
        }

        return new Date(y, m, d);
    }
    else if (dArray.length == 1) {
        try {
            return new Date(dArray[0]);
        }
        catch (e) {
            return new Date(NaN);
        }
    }

    return new Date(NaN);
}

/**
 * Splits a date string into an array of strings, each containing a single date part.
 * It can handle separators "/", ".", "-" and "\".
 * @param s The string to split.
 */
export function splitDateString(s: string): string[] {
    s = trim(s);
    if (!s.length)
        return null;
    if (s.indexOf("/") >= 0)
        return s.split("/");
    else if (s.indexOf(".") >= 0)
        return s.split(".");
    else if (s.indexOf("-") >= 0)
        return s.split("-");
    else if (s.indexOf("\\") >= 0)
        return s.split("\\");
    else
        return [s];
}