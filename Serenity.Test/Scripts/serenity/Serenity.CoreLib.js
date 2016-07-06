var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
/// <reference path="../../../typings/rsvp/rsvp.d.ts" />
/// <reference path="ServiceError.ts" />
/// <reference path="../../../typings/jquery/jquery.d.ts" />
/// <reference path="../Imports/SS.ts" />
var Q;
(function (Q) {
    function coalesce(a, b) {
        return ss.coalesce(a, b);
    }
    Q.coalesce = coalesce;
    function isValue(a) {
        return ss.isValue(a);
    }
    Q.isValue = isValue;
    // derived from https://github.com/mistic100/jQuery.extendext/blob/master/jQuery.extendext.js
    function deepClone(arg1) {
        var args = [];
        for (var _i = 1; _i < arguments.length; _i++) {
            args[_i - 1] = arguments[_i];
        }
        var options, name, src, copy, copyIsArray, clone, target = arguments[0] || {}, i = 1, length = arguments.length;
        // Handle case when target is a string or something (possible in deep copy)
        if (typeof target !== "object" && !$.isFunction(target)) {
            target = {};
        }
        if (i === length) {
            target = {};
            i = 0;
        }
        for (; i < length; i++) {
            // Only deal with non-null/undefined values
            if ((options = arguments[i]) !== null) {
                // Special operations for arrays
                if ($.isArray(options)) {
                    target = $.extend(true, [], options);
                }
                else {
                    // Extend the base object
                    for (name in options) {
                        src = target[name];
                        copy = options[name];
                        // Prevent never-ending loop
                        if (target === copy) {
                            continue;
                        }
                        // Recurse if we're merging plain objects or arrays
                        if (copy && ($.isPlainObject(copy) ||
                            (copyIsArray = $.isArray(copy)))) {
                            if (copyIsArray) {
                                copyIsArray = false;
                                clone = src && $.isArray(src) ? src : [];
                            }
                            else {
                                clone = src && $.isPlainObject(src) ? src : {};
                            }
                            // Never move original objects, clone them
                            target[name] = deepClone(clone, copy);
                        }
                        else if (copy !== undefined) {
                            target[name] = copy;
                        }
                    }
                }
            }
        }
        // Return the modified object
        return target;
    }
    Q.deepClone = deepClone;
})(Q || (Q = {}));
/// <reference path="../Imports/SS.ts" />
/// <reference path="Q.CommonTypes.ts" />
var Q;
(function (Q) {
    /**
     * Tests if any of array elements matches given predicate
     */
    function any(array, predicate) {
        for (var _i = 0, array_1 = array; _i < array_1.length; _i++) {
            var x = array_1[_i];
            if (predicate(x))
                return true;
        }
        return false;
    }
    Q.any = any;
    /**
     * Counts number of array elements that matches a given predicate
     */
    function count(array, predicate) {
        var count = 0;
        for (var _i = 0, array_2 = array; _i < array_2.length; _i++) {
            var x = array_2[_i];
            if (predicate(x))
                count++;
        }
        return count;
    }
    Q.count = count;
    /**
     * Gets first element in an array that matches given predicate.
     * Throws an error if no match is found.
     */
    function first(array, predicate) {
        for (var _i = 0, array_3 = array; _i < array_3.length; _i++) {
            var x = array_3[_i];
            if (predicate(x))
                return x;
        }
        throw new Error("first:No element satisfies the condition.!");
    }
    Q.first = first;
    /**
     * Gets index of first element in an array that matches given predicate
     */
    function indexOf(array, predicate) {
        for (var i = 0; i < array.length; i++)
            if (predicate(array[i]))
                return i;
        return -1;
    }
    Q.indexOf = indexOf;
    /**
     * Inserts an item to the array at specified index
     */
    function insert(array, index, item) {
        ss.insert(array, index, item);
    }
    Q.insert = insert;
    /**
     * Determines if the object is an array
     */
    function isArray(obj) {
        return ss.isArray(obj);
    }
    Q.isArray = isArray;
    /**
    * Gets first element in an array that matches given predicate.
    * Throws an error if no matches is found, or there are multiple matches.
    */
    function single(array, predicate) {
        var match;
        var found = false;
        for (var _i = 0, array_4 = array; _i < array_4.length; _i++) {
            var x = array_4[_i];
            if (predicate(x)) {
                if (found)
                    throw new Error("single:sequence contains more than one element.");
                found = true;
                match = x;
            }
        }
        if (!found)
            throw new Error("single:No element satisfies the condition.");
        return match;
    }
    Q.single = single;
    /**
     * Maps an array into a dictionary with keys determined by specified getKey() callback,
     * and values that are arrays containing elements for a particular key.
     */
    function toGrouping(items, getKey) {
        var lookup = {};
        for (var _i = 0, items_1 = items; _i < items_1.length; _i++) {
            var x = items_1[_i];
            var key = getKey(x) || "";
            var d = lookup[key];
            if (!d) {
                d = lookup[key] = [];
            }
            d.push(x);
        }
        return lookup;
    }
    Q.toGrouping = toGrouping;
    /**
     * Gets first element in an array that matches given predicate.
     * Returns null if no match is found.
     */
    function tryFirst(array, predicate) {
        for (var _i = 0, array_5 = array; _i < array_5.length; _i++) {
            var x = array_5[_i];
            if (predicate(x))
                return x;
        }
    }
    Q.tryFirst = tryFirst;
})(Q || (Q = {}));
/// <reference path="../Imports/SS.ts" />
var Q;
(function (Q) {
    function endsWith(s, search) {
        return ss.endsWithString(s, search);
    }
    Q.endsWith = endsWith;
    function isEmptyOrNull(s) {
        return s == null || s.length === 0;
    }
    Q.isEmptyOrNull = isEmptyOrNull;
    function isTrimmedEmpty(s) {
        return trimToNull(s) == null;
    }
    Q.isTrimmedEmpty = isTrimmedEmpty;
    function format(msg) {
        var prm = [];
        for (var _i = 1; _i < arguments.length; _i++) {
            prm[_i - 1] = arguments[_i];
        }
        return (_a = ss).formatString.apply(_a, [msg].concat(prm));
        var _a;
    }
    Q.format = format;
    function padLeft(s, len, ch) {
        if (ch === void 0) { ch = ' '; }
        while (s.length < len)
            s = "0" + s;
        return s;
    }
    Q.padLeft = padLeft;
    function startsWith(s, search) {
        return ss.startsWithString(s, search);
    }
    Q.startsWith = startsWith;
    function toSingleLine(str) {
        return Q.replaceAll(Q.replaceAll(trimToEmpty(str), '\r\n', ' '), '\n', ' ').trim();
    }
    Q.toSingleLine = toSingleLine;
    function trim(s) {
        return (s == null ? '' : s).replace(new RegExp('^\\s+|\\s+$', 'g'), '');
    }
    Q.trim = trim;
    function trimToEmpty(s) {
        if (s == null || s.length === 0) {
            return '';
        }
        return trim(s);
    }
    Q.trimToEmpty = trimToEmpty;
    function trimToNull(s) {
        if (s == null || s.length === 0) {
            return null;
        }
        s = trim(s);
        if (s.length === 0) {
            return null;
        }
        else {
            return s;
        }
    }
    Q.trimToNull = trimToNull;
    var turkishOrder;
    function turkishLocaleCompare(a, b) {
        var alphabet = "AaBbCcÇçFfGgĞğHhIıİiJjKkLlMmNnOoÖöPpRrSsŞşTtUuÜüVvYyZz";
        a = a || "";
        b = b || "";
        if (a == b)
            return 0;
        if (!turkishOrder) {
            turkishOrder = {};
            for (var z = 0; z < alphabet.length; z++) {
                turkishOrder[alphabet.charAt(z)] = z + 1;
            }
        }
        for (var i = 0, _len = Math.min(a.length, b.length); i < _len; i++) {
            var x = a.charAt(i), y = b.charAt(i);
            if (x === y)
                continue;
            var ix = turkishOrder[x], iy = turkishOrder[y];
            if (ix != null && iy != null)
                return ix < iy ? -1 : 1;
            var c = x.localeCompare(y);
            if (c == 0)
                continue;
            return c;
        }
        return a.localeCompare(b);
    }
    Q.turkishLocaleCompare = turkishLocaleCompare;
    function turkishLocaleToUpper(a) {
        if (!a)
            return a;
        return a.replace(/i/g, 'İ').replace(/ı/g, 'I').toUpperCase();
    }
    Q.turkishLocaleToUpper = turkishLocaleToUpper;
    function replaceAll(s, f, r) {
        return ss.replaceAllString(s, f, r);
    }
    Q.replaceAll = replaceAll;
    function zeroPad(n, digits) {
        var s = n.toString();
        while (s.length < digits)
            s = "0" + s;
        return s;
    }
    Q.zeroPad = zeroPad;
})(Q || (Q = {}));
/// <reference path="Q.StringHelpers.ts" />
var Q;
(function (Q) {
    var Culture;
    (function (Culture) {
        Culture.decimalSeparator = '.';
        Culture.dateSeparator = '/';
        Culture.dateOrder = 'dmy';
        Culture.dateFormat = 'dd/MM/yyyy';
        Culture.dateTimeFormat = 'dd/MM/yyyy HH:mm:ss';
        function get_groupSeperator() {
            return ((Culture.decimalSeparator === ',') ? '.' : ',');
        }
        Culture.get_groupSeperator = get_groupSeperator;
        ;
        var s = Q.trimToNull($('script#ScriptCulture').html());
        if (s != null) {
            var sc = $.parseJSON(s);
            if (sc.DecimalSeparator != null)
                Culture.decimalSeparator = sc.DecimalSeparator;
            if (sc.DateSeparator != null)
                Culture.dateSeparator = sc.DateSeparator;
            if (sc.DateOrder != null)
                Culture.dateOrder = sc.DateOrder;
            if (sc.DateFormat != null)
                Culture.dateFormat = sc.DateFormat;
            if (sc.DateTimeFormat != null)
                Culture.dateTimeFormat = sc.DateTimeFormat;
        }
    })(Culture = Q.Culture || (Q.Culture = {}));
})(Q || (Q = {}));
/// <reference path="Q.StringHelpers.ts" />
/// <reference path="Q.Culture.ts" />
var Q;
(function (Q) {
    function formatNumber(n, fmt, dec, grp) {
        var neg = '-';
        if (isNaN(n)) {
            return null;
        }
        dec = dec || Q.Culture.decimalSeparator;
        grp = grp || Q.Culture.get_groupSeperator();
        var r = "";
        if (fmt.indexOf(".") > -1) {
            var dp = dec;
            var df = fmt.substring(fmt.lastIndexOf(".") + 1);
            n = roundNumber(n, df.length);
            var dv = n % 1;
            var ds = new String(dv.toFixed(df.length));
            ds = ds.substring(ds.lastIndexOf(".") + 1);
            for (var i = 0; i < df.length; i++) {
                if (df.charAt(i) == '#' && ds.charAt(i) != '0') {
                    dp += ds.charAt(i);
                    continue;
                }
                else if (df.charAt(i) == '#' && ds.charAt(i) == '0') {
                    var notParsed = ds.substring(i);
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
            n = Math.round(n);
        var ones = Math.floor(n);
        if (n < 0)
            ones = Math.ceil(n);
        var of = "";
        if (fmt.indexOf(".") == -1)
            of = fmt;
        else
            of = fmt.substring(0, fmt.indexOf("."));
        var op = "";
        if (!(ones == 0 && of.substr(of.length - 1) == '#')) {
            // find how many digits are in the group
            var oneText = new String(Math.abs(ones));
            var gl = 9999;
            if (of.lastIndexOf(",") != -1)
                gl = of.length - of.lastIndexOf(",") - 1;
            var gc = 0;
            for (var i = oneText.length - 1; i > -1; i--) {
                op = oneText.charAt(i) + op;
                gc++;
                if (gc == gl && i != 0) {
                    op = grp + op;
                    gc = 0;
                }
            }
            // account for any pre-data padding
            if (of.length > op.length) {
                var padStart = of.indexOf('0');
                if (padStart != -1) {
                    var padLen = of.length - padStart;
                    // pad to left with 0's or group char
                    var pos = of.length - op.length - 1;
                    while (op.length < padLen) {
                        var pc = of.charAt(pos);
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
        if (n < 0)
            r = neg + r;
        if (r.lastIndexOf(dec) == r.length - 1) {
            r = r.substring(0, r.length - 1);
        }
        return r;
    }
    Q.formatNumber = formatNumber;
    var isoRegexp = /(\d{4,})(?:-(\d{1,2})(?:-(\d{1,2})(?:[T ](\d{1,2}):(\d{1,2})(?::(\d{1,2})(?:\.(\d+))?)?(?:(Z)|([+-])(\d{1,2})(?::(\d{1,2}))?)?)?)?)?/;
    function parseInteger(s) {
        s = Q.trim(s.toString());
        var ts = Q.Culture.get_groupSeperator();
        if (s && s.length && s.indexOf(ts) > 0) {
            s = s.replace(new RegExp("(\\b\\d{1,3})\\" + ts + "(?=\\d{3}(\\D|$))", "g"), '$1');
        }
        if (!(/^[-\+]?\d+$/.test(s)))
            return NaN;
        return parseInt(s, 10);
    }
    Q.parseInteger = parseInteger;
    function parseDecimal(s) {
        if (s == null)
            return null;
        s = Q.trim(s.toString());
        if (s.length == 0)
            return null;
        var ts = Q.Culture.get_groupSeperator();
        if (s && s.length && s.indexOf(ts) > 0) {
            s = s.replace(new RegExp("(\\b\\d{1,3})\\" + ts + "(?=\\d{3}(\\D|$))", "g"), '$1');
        }
        if (!(new RegExp("^\\s*([-\\+])?(\\d*)\\" + Q.Culture.decimalSeparator + "?(\\d*)\\s*$").test(s)))
            return NaN;
        return parseFloat(s.toString().replace(Q.Culture.decimalSeparator, '.'));
    }
    Q.parseDecimal = parseDecimal;
    function roundNumber(n, dec) {
        var power = Math.pow(10, dec || 0);
        var value = (Math.round(n * power) / power).toString();
        // ensure the decimal places are there
        if (dec > 0) {
            var dp = value.indexOf(".");
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
    function toId(id) {
        if (id == null)
            return null;
        if (typeof id == "number")
            return id;
        id = $.trim(id);
        if (id == null || !id.length)
            return null;
        if (id.length >= 15 || !(/^\d+$/.test(id)))
            return id;
        var v = parseInt(id, 10);
        if (isNaN(v))
            return id;
        return v;
    }
    Q.toId = toId;
})(Q || (Q = {}));
/// <reference path="Q.StringHelpers.ts" />
/// <reference path="Q.Culture.ts" />
/// <reference path="Q.Number.ts" />
var Q;
(function (Q) {
    function formatDate(date, format) {
        if (!date) {
            return '';
        }
        if (format == null) {
            format = Q.Culture.dateFormat;
        }
        var pad = function (i) {
            return Q.zeroPad(i, 2);
        };
        return format.replace(new RegExp('dd?|MM?|yy?y?y?|hh?|HH?|mm?|ss?|tt?|fff|zz?z?|\\/', 'g'), function (fmt) {
            switch (fmt) {
                case '/': return Q.Culture.dateSeparator;
                case 'hh': return pad(((date.getHours() < 13) ? date.getHours() : (date.getHours() - 12)));
                case 'h': return ((date.getHours() < 13) ? date.getHours() : (date.getHours() - 12));
                case 'HH': return pad(date.getHours());
                case 'H': return date.getHours();
                case 'mm': return pad(date.getMinutes());
                case 'm': return date.getMinutes();
                case 'ss': return pad(date.getSeconds());
                case 's': return date.getSeconds();
                case 'yyyy': return date.getFullYear();
                case 'yy': return date.getFullYear().toString().substr(2, 4);
                case 'dd': return pad(date.getDate());
                case 'd': return date.getDate().toString();
                case 'MM': return pad(date.getMonth() + 1);
                case 'M': return date.getMonth() + 1;
                case 't': return ((date.getHours() < 12) ? 'A' : 'P');
                case 'tt': return ((date.getHours() < 12) ? 'AM' : 'PM');
                case 'fff': return Q.zeroPad(date.getMilliseconds(), 3);
                case 'zzz':
                case 'zz':
                case 'z': return '';
                default: return fmt;
            }
        });
    }
    Q.formatDate = formatDate;
    function formatDayHourAndMin(n) {
        if (n === 0)
            return '0';
        else if (!n)
            return '';
        var days = Math.floor(n / 24 / 60);
        var txt = "";
        if (days > 0) {
            txt += days.toString();
        }
        var mins = Q.zeroPad(Math.floor((n % (24 * 60)) / (60)), 2) + ':' + Q.zeroPad(n % 60, 2);
        if (mins != '00:00') {
            if (days > 0)
                txt += ".";
            txt += mins;
        }
        return txt;
    }
    Q.formatDayHourAndMin = formatDayHourAndMin;
    function formatISODateTimeUTC(d) {
        if (d == null)
            return "";
        var zeropad = function (num) { return ((num < 10) ? '0' : '') + num; };
        var str = d.getUTCFullYear() + "-" +
            zeropad(d.getUTCMonth() + 1) + "-" +
            zeropad(d.getUTCDate()) + "T" +
            zeropad(d.getUTCHours()) + ":" +
            zeropad(d.getUTCMinutes());
        var secs = Number(d.getUTCSeconds() + "." +
            ((d.getUTCMilliseconds() < 100) ? '0' : '') +
            zeropad(d.getUTCMilliseconds()));
        str += ":" + zeropad(secs) + "Z";
        return str;
    }
    Q.formatISODateTimeUTC = formatISODateTimeUTC;
    var isoRegexp = /(\d{4,})(?:-(\d{1,2})(?:-(\d{1,2})(?:[T ](\d{1,2}):(\d{1,2})(?::(\d{1,2})(?:\.(\d+))?)?(?:(Z)|([+-])(\d{1,2})(?::(\d{1,2}))?)?)?)?)?/;
    function parseISODateTime(s) {
        if (!s || !s.length)
            return null;
        var timestamp = Date.parse(s);
        if (!isNaN(timestamp) && typeof timestamp == "Date")
            return timestamp;
        s = s + "";
        if (typeof (s) != "string" || s.length === 0) {
            return null;
        }
        var res = s.match(isoRegexp);
        if (typeof (res) == "undefined" || res === null) {
            return null;
        }
        var year, month, day, hour, min, sec, msec;
        year = parseInt(res[1], 10);
        if (typeof (res[2]) == "undefined" || res[2] === '') {
            return new Date(year);
        }
        month = parseInt(res[2], 10) - 1;
        day = parseInt(res[3], 10);
        if (typeof (res[4]) == "undefined" || res[4] === '') {
            return new Date(year, month, day);
        }
        hour = parseInt(res[4], 10);
        min = parseInt(res[5], 10);
        sec = (typeof (res[6]) != "undefined" && res[6] !== '') ? parseInt(res[6], 10) : 0;
        if (typeof (res[7]) != "undefined" && res[7] !== '') {
            msec = Math.round(1000.0 * parseFloat("0." + res[7]));
        }
        else {
            msec = 0;
        }
        if ((typeof (res[8]) == "undefined" || res[8] === '') && (typeof (res[9]) == "undefined" || res[9] === '')) {
            return new Date(year, month, day, hour, min, sec, msec);
        }
        var ofs;
        if (typeof (res[9]) != "undefined" && res[9] !== '') {
            ofs = parseInt(res[10], 10) * 3600000;
            if (typeof (res[11]) != "undefined" && res[11] !== '') {
                ofs += parseInt(res[11], 10) * 60000;
            }
            if (res[9] == "-") {
                ofs = -ofs;
            }
        }
        else {
            ofs = 0;
        }
        return new Date(Date.UTC(year, month, day, hour, min, sec, msec) - ofs);
    }
    Q.parseISODateTime = parseISODateTime;
    function parseHourAndMin(value) {
        var v = Q.trim(value);
        if (v.length < 4 || v.length > 5)
            return NaN;
        var h, m;
        if (v.charAt(1) == ':') {
            h = Q.parseInteger(v.substr(0, 1));
            m = Q.parseInteger(v.substr(2, 2));
        }
        else {
            if (v.charAt(2) != ':')
                return NaN;
            h = Q.parseInteger(v.substr(0, 2));
            m = Q.parseInteger(v.substr(3, 2));
        }
        if (isNaN(h) || isNaN(m) || h < 0 || h > 23 || m < 0 || m > 59)
            return NaN;
        return h * 60 + m;
    }
    Q.parseHourAndMin = parseHourAndMin;
    function parseDayHourAndMin(s) {
        var days;
        var v = Q.trim(s);
        if (!v)
            return NaN;
        var p = v.split('.');
        if (p.length == 0 || p.length > 2)
            return NaN;
        if (p.length == 1) {
            days = Q.parseInteger(p[0]);
            if (!isNaN(days))
                return days * 24 * 60;
            return parseHourAndMin(p[0]);
        }
        else {
            days = Q.parseInteger(p[0]);
            var hm = parseHourAndMin(p[1]);
            if (isNaN(days) || isNaN(hm))
                return NaN;
            return days * 24 * 60 + hm;
        }
    }
    Q.parseDayHourAndMin = parseDayHourAndMin;
    function parseDate(s, dateOrder) {
        if (!s || !s.length)
            return null;
        var dateVal;
        var dArray;
        var d, m, y;
        dArray = splitDateString(s);
        if (!dArray)
            return false;
        if (dArray.length == 3) {
            dateOrder = dateOrder || Q.Culture.dateOrder;
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
                return false;
            if (y < 100) {
                var fullYear = new Date().getFullYear();
                var shortYearCutoff = (fullYear % 100) + 10;
                y += fullYear - fullYear % 100 + (y <= shortYearCutoff ? 0 : -100);
            }
            try {
                dateVal = new Date(y, m, d);
                if (isNaN(dateVal.getFullYear()))
                    return false;
            }
            catch (e) {
                return false;
            }
        }
        else if (dArray.length == 1) {
            try {
                dateVal = new Date(dArray[0]);
                if (isNaN(dateVal.getFullYear()))
                    return false;
            }
            catch (e) {
                return false;
            }
        }
        return dateVal;
    }
    Q.parseDate = parseDate;
    function splitDateString(s) {
        s = Q.trim(s);
        if (!s.length)
            return;
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
    Q.splitDateString = splitDateString;
})(Q || (Q = {}));
/// <reference path="Q.ArrayUtils.ts" />
var Q;
(function (Q) {
    function text(key) {
        var t = LT.$table[key];
        if (t == null) {
            t = key || '';
        }
        return t;
    }
    Q.text = text;
    function tryGetText(key) {
        return LT.$table[key];
    }
    Q.tryGetText = tryGetText;
    var LT = (function () {
        function LT(key) {
            this.key = key;
        }
        LT.add = function (obj, pre) {
            if (!obj) {
                return;
            }
            pre = pre || '';
            for (var _i = 0, _a = Object.keys(obj); _i < _a.length; _i++) {
                var k = _a[_i];
                var actual = pre + k;
                var o = obj[k];
                if (typeof (o) === 'object') {
                    LT.add(o, actual + '.');
                }
                else {
                    LT.$table[actual] = o;
                }
            }
        };
        LT.prototype.get = function () {
            var t = LT.$table[this.key];
            if (t == null) {
                t = this.key || '';
            }
            return t;
        };
        LT.prototype.toString = function () {
            var t = LT.$table[this.key];
            if (t == null) {
                t = this.key || '';
            }
            return t;
        };
        LT.$table = {};
        LT.empty = new LT('');
        LT.initializeTextClass = function (type, prefix) {
            var $t1 = Object.keys(type).slice();
            for (var $t2 = 0; $t2 < $t1.length; $t2++) {
                var member = $t1[$t2];
                var value = type[member];
                if (value instanceof LT) {
                    var lt = value;
                    var key = prefix + member;
                    LT.$table[key] = lt.$key;
                    type[member] = new LT(key);
                }
            }
        };
        LT.getDefault = function (key, defaultText) {
            var t = LT.$table[key];
            if (t == null) {
                t = defaultText;
                if (t == null) {
                    t = key || '';
                }
            }
            return t;
        };
        return LT;
    }());
    Q.LT = LT;
})(Q || (Q = {}));
var Q;
(function (Q) {
    var Lookup = (function () {
        function Lookup(options, items) {
            this.items = [];
            this.itemById = {};
            options = options || {};
            this.textFormatter = options.textFormatter;
            this.idField = options.idField;
            this.parentIdField = options.parentIdField;
            this.textField = options.textField;
            this.textFormatter = options.textFormatter;
            if (items != null)
                this.update(items);
        }
        Lookup.prototype.update = function (value) {
            this.items = [];
            this.itemById = {};
            if (value) {
                for (var _i = 0, value_1 = value; _i < value_1.length; _i++) {
                    var k = value_1[_i];
                    this.items.push(k);
                }
            }
            var idField = this.idField;
            if (!Q.isEmptyOrNull(idField)) {
                for (var _a = 0, _b = this.items; _a < _b.length; _a++) {
                    var r = _b[_a];
                    var v = r[idField];
                    if (v != null) {
                        this.itemById[v] = r;
                    }
                }
            }
        };
        Lookup.prototype.get_idField = function () {
            return this.idField;
        };
        Lookup.prototype.get_parentIdField = function () {
            return this.parentIdField;
        };
        Lookup.prototype.get_textField = function () {
            return this.textField;
        };
        Lookup.prototype.get_textFormatter = function () {
            return this.textFormatter;
        };
        Lookup.prototype.get_itemById = function () {
            return this.itemById;
        };
        Lookup.prototype.get_items = function () {
            return this.items;
        };
        return Lookup;
    }());
    Q.Lookup = Lookup;
})(Q || (Q = {}));
/// <reference path="../../../typings/jquery.blockUI/jquery.blockUI.d.ts" />
/// <reference path="../Imports/JQBlockUI.ts" />
var Q;
(function (Q) {
    var blockUICount = 0;
    function blockUIWithCheck(opt) {
        if (blockUICount > 0) {
            blockUICount++;
            return;
        }
        $.blockUI(opt);
        blockUICount++;
    }
    /**
     * Uses jQuery BlockUI plugin to block access to whole page (default) or
     * a part of it, by using a transparent overlay covering the whole area.
     * @param options Parameters for the BlockUI plugin
     * @remarks If options are not specified, this function blocks
     * whole page with a transparent overlay. Default z-order of the overlay
     * div is 2000, so a higher z-order shouldn't be used in page.
     */
    function blockUI(options) {
        options = $.extend({
            baseZ: 2000,
            message: '',
            overlayCSS: {
                opacity: '0.0',
                zIndex: 2000,
                cursor: 'wait'
            }, fadeOut: 0
        }, options);
        if (options.useTimeout) {
            window.setTimeout(function () {
                blockUIWithCheck(options);
            }, 0);
        }
        else {
            blockUIWithCheck(options);
        }
    }
    Q.blockUI = blockUI;
    function blockUndo() {
        if (blockUICount > 1) {
            blockUICount--;
            return;
        }
        blockUICount--;
        $.unblockUI({ fadeOut: 0 });
    }
    Q.blockUndo = blockUndo;
})(Q || (Q = {}));
var Q;
(function (Q) {
    function addOption(select, key, text) {
        $('<option/>').val(key).text(text).appendTo(select);
    }
    Q.addOption = addOption;
    function addEmptyOption(select) {
        addOption(select, '', Q.text("Controls.SelectEditor.EmptyItemText"));
    }
    Q.addEmptyOption = addEmptyOption;
    function clearOptions(select) {
        select.html('');
    }
    Q.clearOptions = clearOptions;
    function findElementWithRelativeId(element, relativeId) {
        var elementId = element.attr('id');
        if (Q.isEmptyOrNull(elementId)) {
            return $('#' + relativeId);
        }
        var result = $(elementId + relativeId);
        if (result.length > 0) {
            return result;
        }
        result = $(elementId + '_' + relativeId);
        if (result.length > 0) {
            return result;
        }
        while (true) {
            var idx = elementId.lastIndexOf('_');
            if (idx <= 0) {
                return $('#' + relativeId);
            }
            elementId = elementId.substr(0, idx);
            result = $('#' + elementId + '_' + relativeId);
            if (result.length > 0) {
                return result;
            }
        }
    }
    Q.findElementWithRelativeId = findElementWithRelativeId;
    function htmlEncoder(a) {
        switch (a) {
            case '&': return '&amp;';
            case '>': return '&gt;';
            case '<': return '&lt;';
        }
        return a;
    }
    /**
     * Html encodes a string
     * @param s String to be HTML encoded
     */
    function htmlEncode(s) {
        var text = (s == null ? '' : s.toString());
        if ((new RegExp('[><&]', 'g')).test(text)) {
            return text.replace(new RegExp('[><&]', 'g'), htmlEncoder);
        }
        return text;
    }
    Q.htmlEncode = htmlEncode;
    function jsRender(markup, data) {
        if (!markup || markup.indexOf('{{') < 0) {
            return markup;
        }
        if (!$.templates || !$.views) {
            throw new ss.Exception('Please make sure that jsrender.js is included in the page!');
        }
        data = data || {};
        var template = $.templates(markup);
        $.views.converters({
            text: Q.text
        }, template);
        return template.render(data);
    }
    Q.jsRender = jsRender;
    function log(m) {
        window.console && window.console.log(m);
    }
    Q.log = log;
    function newBodyDiv() {
        return $('<div/>').appendTo(document.body);
    }
    Q.newBodyDiv = newBodyDiv;
    function outerHtml(element) {
        return $('<i/>').append(element.eq(0).clone()).html();
    }
    Q.outerHtml = outerHtml;
})(Q || (Q = {}));
/// <reference path="../../../typings/jquery/jquery.d.ts" />
/// <reference path="../../../typings/jqueryui/jqueryui.d.ts" />
/// <reference path="Q.LocalText.ts" />
/// <reference path="Q.Html.ts" />
var Q;
(function (Q) {
    function alert(message, options) {
        var dialog;
        options = $.extend({
            htmlEncode: true,
            okButton: Q.text('Dialogs.OkButton'),
            title: Q.text('Dialogs.AlertTitle'),
            onClose: null,
            onOpen: null,
            autoOpen: false,
            dialogClass: 's-MessageDialog s-AlertDialog',
            modal: true,
            width: '40%',
            maxWidth: '450',
            minWidth: '180',
            resizable: false,
            open: function () {
                if (options.onOpen)
                    options.onOpen.call(this);
            },
            close: function () {
                dialog.dialog('destroy');
                if (options.onClose)
                    options.onClose();
            }
        }, options);
        if (options.htmlEncode)
            message = Q.htmlEncode(message);
        if (!options.buttons) {
            var buttons = [];
            buttons.push({
                text: options.okButton,
                click: function () {
                    dialog.dialog('close');
                }
            });
            options.buttons = buttons;
        }
        dialog = $('<div><div class="message"><\/div><\/div>')
            .dialog(options)
            .children('.message')
            .html(message)
            .parent()
            .dialog('open');
    }
    Q.alert = alert;
    function confirm(message, onYes, options) {
        var dialog;
        options = $.extend({
            htmlEncode: true,
            yesButton: Q.text('Dialogs.YesButton'),
            noButton: Q.text('Dialogs.NoButton'),
            title: Q.text('Dialogs.ConfirmationTitle'),
            onNo: null,
            onCancel: null,
            onClose: null,
            autoOpen: false,
            modal: true,
            dialogClass: 's-MessageDialog s-ConfirmDialog',
            width: '40%',
            maxWidth: '450',
            minWidth: '180',
            resizable: false,
            open: function () {
                if (options.onOpen)
                    options.onOpen.call(this);
            },
            close: function () {
                dialog.dialog('destroy');
                if (!clicked && options.onCancel)
                    options.onCancel();
            },
            overlay: {
                opacity: 0.77,
                background: "black"
            }
        }, options);
        if (options.htmlEncode)
            message = Q.htmlEncode(message);
        var clicked = false;
        if (!options.buttons) {
            var buttons = [];
            buttons.push({
                text: options.yesButton,
                click: function () {
                    clicked = true;
                    dialog.dialog('close');
                    if (onYes)
                        onYes();
                }
            });
            if (options.noButton)
                buttons.push({
                    text: options.noButton,
                    click: function () {
                        clicked = true;
                        dialog.dialog('close');
                        if (options.onNo)
                            options.onNo();
                        else if (options.onCancel)
                            options.onCancel();
                    }
                });
            options.buttons = buttons;
        }
        dialog = $('<div><div class="message"><\/div><\/div>')
            .dialog(options)
            .children('.message')
            .html(message)
            .parent()
            .dialog('open');
    }
    Q.confirm = confirm;
    function iframeDialog(options) {
        var doc;
        var e = $('<div><iframe></iframe></div>');
        var settings = $.extend({
            autoOpen: true,
            modal: true,
            width: '60%',
            height: '400',
            title: Q.text('Dialogs.AlertTitle'),
            open: function () {
                doc = (e.find('iframe').css({
                    border: 'none',
                    width: '100%',
                    height: '100%'
                })[0]).contentDocument;
                doc.open();
                doc.write(settings.html);
                doc.close();
            },
            close: function () {
                doc.open();
                doc.write('');
                doc.close();
                e.dialog('destroy').html('');
            }
        }, options);
        e.dialog(settings);
    }
    Q.iframeDialog = iframeDialog;
    function information(message, onOk, options) {
        confirm(message, onOk, $.extend({
            title: Q.text("Dialogs.InformationTitle"),
            dialogClass: "s-MessageDialog s-InformationDialog",
            yesButton: Q.text("Dialogs.OkButton"),
            noButton: null,
        }, options));
    }
    Q.information = information;
    function warning(message, options) {
        alert(message, $.extend({
            title: Q.text("Dialogs.WarningTitle"),
            dialogClass: "s-MessageDialog s-WarningDialog"
        }, options));
    }
    Q.warning = warning;
})(Q || (Q = {}));
/// <reference path="../../../typings/toastr/toastr.d.ts" />
var Q;
(function (Q) {
    Q.defaultNotifyOptions = {
        timeOut: 3000,
        showDuration: 250,
        hideDuration: 500,
        extendedTimeOut: 500,
        positionClass: 'toast-top-full-width'
    };
    function getToastrOptions(options) {
        options = $.extend({}, Q.defaultNotifyOptions, options);
        positionToastContainer(true);
        return options;
    }
    function notifyWarning(message, title, options) {
        toastr.warning(message, title, getToastrOptions(options));
    }
    Q.notifyWarning = notifyWarning;
    function notifySuccess(message, title, options) {
        toastr.success(message, title, getToastrOptions(options));
    }
    Q.notifySuccess = notifySuccess;
    function notifyInfo(message, title, options) {
        toastr.info(message, title, getToastrOptions(options));
    }
    Q.notifyInfo = notifyInfo;
    function notifyError(message, title, options) {
        toastr.error(message, title, getToastrOptions(options));
    }
    Q.notifyError = notifyError;
    function positionToastContainer(create) {
        if (typeof toastr === 'undefined') {
            return;
        }
        var dialog = $(window.document.body).children('.ui-dialog:visible').last();
        var container = toastr.getContainer(null, create);
        if (container.length === 0) {
            return;
        }
        if (dialog.length > 0) {
            var position = dialog.position();
            container.addClass('positioned-toast toast-top-full-width');
            container.css({ position: 'absolute', top: position.top + 28 + 'px', left: position.left + 6 + 'px', width: dialog.width() - 12 + 'px' });
        }
        else {
            container.addClass('toast-top-full-width');
            if (container.hasClass('positioned-toast')) {
                container.removeClass('positioned-toast');
                container.css({ position: '', top: '', left: '', width: '' });
            }
        }
    }
    Q.positionToastContainer = positionToastContainer;
})(Q || (Q = {}));
var Q;
(function (Q) {
    var Config;
    (function (Config) {
        /**
         * This is the root path of your application. If your application resides under http://localhost/mysite/,
         * your root path is "mysite/". This variable is automatically initialized by reading from a <link> element
         * with ID "ApplicationPath" from current page, which is usually located in your _LayoutHead.cshtml file
         */
        Config.applicationPath = '/';
        var pathLink = $('link#ApplicationPath');
        if (pathLink.length > 0) {
            Config.applicationPath = pathLink.attr('href');
        }
        /**
         * Email validation by default only allows ASCII characters. Set this to true if you want to allow unicode.
         */
        Config.emailAllowOnlyAscii = true;
        /**
         * Set this to true, to enable responsive dialogs by default, without having to add Serenity.Decorators.responsive()"
         * on dialog classes manually. It's false by default for backward compability.
         */
        Config.responsiveDialogs = false;
        /**
         * This is the list of root namespaces that may be searched for types. For example, if you specify an editor type
         * of "MyEditor", first a class with name "MyEditor" will be searched, if not found, search will be followed by
         * "Serenity.MyEditor" and "MyApp.MyEditor" if you added "MyApp" to the list of root namespaces.
         *
         * You should usually add your application root namespace to this list in ScriptInitialization.ts file.
         */
        Config.rootNamespaces = ['Serenity'];
        /**
         * This is an optional method for handling when user is not logged in. If a users session is expired
         * and when a NotAuthorized response is received from a service call, Serenity will call this handler, so
         * you may intercept it and notify user about this situation and ask if she wants to login again...
         */
        Config.notLoggedInHandler = null;
    })(Config = Q.Config || (Q.Config = {}));
})(Q || (Q = {}));
var Q;
(function (Q) {
    function parseQueryString(s) {
        var qs;
        if (s === undefined)
            qs = location.search.substring(1, location.search.length);
        else
            qs = s || '';
        var result = {};
        var parts = qs.split('&');
        for (var i = 0; i < parts.length; i++) {
            var pair = parts[i].split('=');
            var name_1 = decodeURIComponent(pair[0]);
            result[name_1] = (pair.length >= 2 ? decodeURIComponent(pair[1]) : name_1);
        }
        return result;
    }
    Q.parseQueryString = parseQueryString;
    function postToService(options) {
        var form = $('<form/>')
            .attr('method', 'POST')
            .attr('action', options.url ? (resolveUrl(options.url)) : resolveUrl('~/services/' + options.service))
            .appendTo(document.body);
        if (options.target)
            form.attr('target', options.target);
        var div = $('<div/>').appendTo(form);
        $('<input/>').attr('type', 'hidden').attr('name', 'request')
            .val($['toJSON'](options.request))
            .appendTo(div);
        $('<input/>').attr('type', 'submit')
            .appendTo(div);
        form.submit();
        window.setTimeout(function () { form.remove(); }, 0);
    }
    Q.postToService = postToService;
    function postToUrl(options) {
        var form = $('<form/>')
            .attr('method', 'POST')
            .attr('action', resolveUrl(options.url))
            .appendTo(document.body);
        if (options.target)
            form.attr('target', options.target);
        var div = $('<div/>').appendTo(form);
        if (options.params != null) {
            for (var k in options.params) {
                $('<input/>').attr('type', 'hidden').attr('name', k)
                    .val(options.params[k])
                    .appendTo(div);
            }
        }
        $('<input/>').attr('type', 'submit')
            .appendTo(div);
        form.submit();
        window.setTimeout(function () { form.remove(); }, 0);
    }
    Q.postToUrl = postToUrl;
    function resolveUrl(url) {
        if (url && url.substr(0, 2) === '~/') {
            return Q.Config.applicationPath + url.substr(2);
        }
        return url;
    }
    Q.resolveUrl = resolveUrl;
})(Q || (Q = {}));
/// <reference path="../Services/ServiceError.ts" />
/// <reference path="Q.Dialogs.ts" />
var Q;
(function (Q) {
    var ErrorHandling;
    (function (ErrorHandling) {
        function showServiceError(error) {
            var msg;
            if (error == null) {
                msg = '??ERROR??';
            }
            else {
                msg = error.Message;
                if (msg == null) {
                    msg = error.Code;
                }
            }
            Q.alert(msg);
        }
        ErrorHandling.showServiceError = showServiceError;
    })(ErrorHandling = Q.ErrorHandling || (Q.ErrorHandling = {}));
})(Q || (Q = {}));
/// <reference path="Q.BlockUI.ts" />
/// <reference path="Q.Config.ts" />
/// <reference path="Q.ErrorHandling.ts" />
/// <reference path="Q.Url.ts" />
/// <reference path="../Services/ServiceResponse.ts" />
var Q;
(function (Q) {
    function serviceCall(options) {
        var handleError = function (response) {
            if (Q.Config.notLoggedInHandler != null &&
                response &&
                response.Error &&
                Q.Config.notLoggedInHandler(options, response)) {
                return;
            }
            if (options.onError != null) {
                options.onError(response);
                return;
            }
            Q.ErrorHandling.showServiceError(response.Error);
        };
        var url = options.service;
        if (url && url.length && url.charAt(0) != '~' && url.charAt(0) != '/' && url.indexOf('://') < 0)
            url = Q.resolveUrl("~/services/") + url;
        options = $.extend({
            dataType: 'json',
            contentType: 'application/json',
            type: 'POST',
            cache: false,
            blockUI: true,
            url: url,
            data: $.toJSON(options.request),
            success: function (data, textStatus, request) {
                try {
                    if (!data.Error && options.onSuccess) {
                        options.onSuccess(data);
                    }
                }
                finally {
                    if (options.blockUI) {
                        Q.blockUndo();
                    }
                    options.onCleanup && options.onCleanup();
                }
            },
            error: function (xhr, status, ev) {
                try {
                    if (xhr.status === 403) {
                        var l = null;
                        try {
                            l = xhr.getResponseHeader('Location');
                        }
                        catch ($t1) {
                            l = null;
                        }
                        if (l) {
                            window.top.location.href = l;
                            return;
                        }
                    }
                    if ((xhr.getResponseHeader('content-type') || '')
                        .toLowerCase().indexOf('application/json') >= 0) {
                        var json = $.parseJSON(xhr.responseText);
                        if (json && json.Error) {
                            handleError(json);
                            return;
                        }
                    }
                    var html = xhr.responseText;
                    Q.iframeDialog({ html: html });
                }
                finally {
                    if (options.blockUI) {
                        Q.blockUndo();
                    }
                    options.onCleanup && options.onCleanup();
                }
            }
        }, options);
        if (options.blockUI) {
            Q.blockUI(null);
        }
        return $.ajax(options);
    }
    Q.serviceCall = serviceCall;
    function serviceRequest(service, request, onSuccess, options) {
        return serviceCall($.extend({
            service: service,
            request: request,
            onSuccess: onSuccess
        }, options));
    }
    Q.serviceRequest = serviceRequest;
    function setEquality(request, field, value) {
        if (request.EqualityFilter == null) {
            request.EqualityFilter = {};
        }
        request.EqualityFilter[field] = value;
    }
    Q.setEquality = setEquality;
})(Q || (Q = {}));
var Q;
(function (Q) {
    function addFullHeightResizeHandler(handler) {
        $('body').addClass('full-height-page');
        var layout = function () {
            var avail;
            try {
                avail = parseInt($('.page-content').css('min-height') || '0')
                    - parseInt($('.page-content').css('padding-top') || '0')
                    - parseInt($('.page-content').css('padding-bottom') || '0');
            }
            catch ($t1) {
                avail = 100;
            }
            handler(avail);
        };
        if (window.Metronic) {
            window.Metronic.addResizeHandler(layout);
        }
        else {
            $(window).resize(layout);
        }
        layout();
    }
    Q.addFullHeightResizeHandler = addFullHeightResizeHandler;
    function autoFullHeight(element) {
        element.css('height', '100%');
        triggerLayoutOnShow(element);
    }
    Q.autoFullHeight = autoFullHeight;
    function initFullHeightGridPage(gridDiv) {
        $('body').addClass('full-height-page');
        gridDiv.addClass('responsive-height');
        var layout = function () {
            var inPageContent = gridDiv.parent().hasClass('page-content') ||
                gridDiv.parent().is('section.content');
            if (inPageContent) {
                gridDiv.css('height', '1px').css('overflow', 'hidden');
            }
            layoutFillHeight(gridDiv);
            if (inPageContent) {
                gridDiv.css('overflow', '');
            }
            gridDiv.triggerHandler('layout');
        };
        if ($('body').hasClass('has-layout-event')) {
            $('body').bind('layout', layout);
        }
        else if (window.Metronic) {
            window.Metronic.addResizeHandler(layout);
        }
        else {
            $(window).resize(layout);
        }
        layout();
    }
    Q.initFullHeightGridPage = initFullHeightGridPage;
    function layoutFillHeightValue(element) {
        var h = 0;
        element.parent().children().not(element).each(function (i, e) {
            var q = $(e);
            if (q.is(':visible')) {
                h += q.outerHeight(true);
            }
        });
        h = element.parent().height() - h;
        if (element.css('box-sizing') !== 'border-box') {
            h = h - (element.outerHeight(true) - element.height());
        }
        return h;
    }
    Q.layoutFillHeightValue = layoutFillHeightValue;
    function layoutFillHeight(element) {
        var h = layoutFillHeightValue(element);
        var n = h + 'px';
        if (element.css('height') != n) {
            element.css('height', n);
        }
    }
    Q.layoutFillHeight = layoutFillHeight;
    function setMobileDeviceMode() {
        var isMobile = navigator.userAgent.indexOf('Mobi') >= 0 ||
            window.matchMedia('(max-width: 767px)').matches;
        var body = $(document.body);
        if (body.hasClass('mobile-device')) {
            if (!isMobile) {
                body.removeClass('mobile-device');
            }
        }
        else if (isMobile) {
            body.addClass('mobile-device');
        }
    }
    Q.setMobileDeviceMode = setMobileDeviceMode;
    function triggerLayoutOnShow(element) {
        Serenity.LazyLoadHelper.executeEverytimeWhenShown(element, function () {
            element.triggerHandler('layout');
        }, true);
    }
    Q.triggerLayoutOnShow = triggerLayoutOnShow;
    function centerDialog(el) {
        if (!el.hasClass("ui-dialog"))
            el = el.closest(".ui-dialog");
        el.position({ at: 'center center', of: window });
        var pos = el.position();
        if (pos.left < 0)
            el.css("left", "0px");
        if (pos.top < 0)
            el.css("top", "0px");
    }
    Q.centerDialog = centerDialog;
})(Q || (Q = {}));
/// <reference path="Q.BlockUI.ts" />
/// <reference path="Q.StringHelpers.ts" />
/// <reference path="Q.Url.ts" />
/// <reference path="../../../typings/rsvp/rsvp.d.ts" />
var Q;
(function (Q) {
    var ScriptData;
    (function (ScriptData) {
        var registered = {};
        var loadedData = {};
        function bindToChange(name, regClass, onChange) {
            $(document.body).bind('scriptdatachange.' + regClass, function (e, s) {
                if (s == name) {
                    onChange();
                }
            });
        }
        ScriptData.bindToChange = bindToChange;
        function triggerChange(name) {
            $(document.body).triggerHandler('scriptdatachange', [name]);
        }
        ScriptData.triggerChange = triggerChange;
        function unbindFromChange(regClass) {
            $(document.body).unbind('scriptdatachange.' + regClass);
        }
        ScriptData.unbindFromChange = unbindFromChange;
        function syncLoadScript(url) {
            $.ajax({ async: false, cache: true, type: 'GET', url: url, data: null, dataType: 'script' });
        }
        function loadScriptAsync(url) {
            return RSVP.resolve().then(function () {
                Q.blockUI(null);
                return RSVP.resolve($.ajax({ async: true, cache: true, type: 'GET', url: url, data: null, dataType: 'script' }).always(function () {
                    Q.blockUndo();
                }));
            }, null);
        }
        function loadScriptData(name) {
            if (registered[name] == null) {
                throw new Error(Q.format('Script data {0} is not found in registered script list!', name));
            }
            name = name + '.js?' + registered[name];
            syncLoadScript(Q.resolveUrl('~/DynJS.axd/') + name);
        }
        function loadScriptDataAsync(name) {
            return RSVP.resolve().then(function () {
                if (registered[name] == null) {
                    throw new Error(Q.format('Script data {0} is not found in registered script list!', name));
                }
                name = name + '.js?' + registered[name];
                return loadScriptAsync(Q.resolveUrl('~/DynJS.axd/') + name);
            }, null);
        }
        function ensure(name) {
            var data = loadedData[name];
            if (data == null) {
                loadScriptData(name);
            }
            data = loadedData[name];
            if (data == null)
                throw new Error(Q.format("Can't load script data: {0}!", name));
            return data;
        }
        ScriptData.ensure = ensure;
        function ensureAsync(name) {
            return RSVP.resolve().then(function () {
                var data = loadedData[name];
                if (data != null) {
                    return RSVP.resolve(data);
                }
                return loadScriptDataAsync(name).then(function () {
                    data = loadedData[name];
                    if (data == null) {
                        throw new Error(Q.format("Can't load script data: {0}!", name));
                    }
                    return data;
                }, null);
            }, null);
        }
        ScriptData.ensureAsync = ensureAsync;
        function reload(name) {
            if (registered[name] == null) {
                throw new Error(Q.format('Script data {0} is not found in registered script list!', name));
            }
            registered[name] = (new Date()).getTime().toString();
            loadScriptData(name);
            var data = loadedData[name];
            return data;
        }
        ScriptData.reload = reload;
        function reloadAsync(name) {
            return RSVP.resolve().then(function () {
                if (registered[name] == null) {
                    throw new Error(Q.format('Script data {0} is not found in registered script list!', name));
                }
                registered[name] = (new Date()).getTime().toString();
                return loadScriptDataAsync(name).then(function () {
                    return loadedData[name];
                }, null);
            }, null);
        }
        ScriptData.reloadAsync = reloadAsync;
        function canLoad(name) {
            return (loadedData[name] != null || registered[name] != null);
        }
        ScriptData.canLoad = canLoad;
        function setRegisteredScripts(scripts) {
            registered = {};
            for (var k in scripts) {
                registered[k] = scripts[k].toString();
            }
        }
        ScriptData.setRegisteredScripts = setRegisteredScripts;
        function set(name, value) {
            loadedData[name] = value;
            triggerChange(name);
        }
        ScriptData.set = set;
    })(ScriptData = Q.ScriptData || (Q.ScriptData = {}));
    function getRemoteData(key) {
        return ScriptData.ensure('RemoteData.' + key);
    }
    Q.getRemoteData = getRemoteData;
    function getRemoteDataAsync(key) {
        return ScriptData.ensureAsync('RemoteData.' + key);
    }
    Q.getRemoteDataAsync = getRemoteDataAsync;
    function getLookup(key) {
        return ScriptData.ensure('Lookup.' + key);
    }
    Q.getLookup = getLookup;
    function getLookupAsync(key) {
        return ScriptData.ensureAsync('Lookup.' + key);
    }
    Q.getLookupAsync = getLookupAsync;
    function reloadLookup(key) {
        ScriptData.reload('Lookup.' + key);
    }
    Q.reloadLookup = reloadLookup;
    function reloadLookupAsync(key) {
        return ScriptData.reloadAsync('Lookup.' + key);
    }
    Q.reloadLookupAsync = reloadLookupAsync;
    function getColumns(key) {
        return ScriptData.ensure('Columns.' + key);
    }
    Q.getColumns = getColumns;
    function getColumnsAsync(key) {
        return ScriptData.ensureAsync('Columns.' + key);
    }
    Q.getColumnsAsync = getColumnsAsync;
    function getForm(key) {
        return ScriptData.ensure('Form.' + key);
    }
    Q.getForm = getForm;
    function getFormAsync(key) {
        return ScriptData.ensureAsync('Form.' + key);
    }
    Q.getFormAsync = getFormAsync;
    function getTemplate(key) {
        return ScriptData.ensure('Template.' + key);
    }
    Q.getTemplate = getTemplate;
    function getTemplateAsync(key) {
        return ScriptData.ensureAsync('Template.' + key);
    }
    Q.getTemplateAsync = getTemplateAsync;
    function canLoadScriptData(name) {
        return ScriptData.canLoad(name);
    }
    Q.canLoadScriptData = canLoadScriptData;
})(Q || (Q = {}));
/// <reference path="Q.Layout.ts" />
/// <reference path="Q.Lookup.ts" />
/// <reference path="Q.ScriptData.ts" />
var Q;
(function (Q) {
    function prop(type, name, getter, setter) {
        getter = getter || "get_" + name;
        setter = setter || "set_" + name;
        Object.defineProperty(type.prototype, name, {
            get: function () {
                return this[getter]();
            },
            set: function (value) {
                return this[setter](value);
            },
            configurable: true,
            enumerable: true
        });
    }
    Q.prop = prop;
    function enumerateTypes(global, namespaces, callback) {
        function scan(root, fullName, depth) {
            if (!root)
                return;
            if ($.isArray(root) ||
                root instanceof Date)
                return;
            var t = typeof (root);
            if (t == "string" ||
                t == "number")
                return;
            if ($.isFunction(root) || (root.__enum && root.__register))
                callback(root, fullName);
            if (depth > 3)
                return;
            for (var _i = 0, _a = Object.keys(root); _i < _a.length; _i++) {
                var k = _a[_i];
                if (k.charAt(0) < 'A' || k.charAt(0) > 'Z')
                    continue;
                if (k.indexOf('$') >= 0)
                    continue;
                if (k == "prototype")
                    continue;
                scan(root[k], fullName + '.' + k, depth + 1);
            }
        }
        for (var _i = 0, namespaces_1 = namespaces; _i < namespaces_1.length; _i++) {
            var nsRoot = namespaces_1[_i];
            if (nsRoot == null || !nsRoot.length) {
                continue;
            }
            if (nsRoot.indexOf('.') >= 0) {
                var g = global;
                var parts = nsRoot.split('.');
                for (var _a = 0, parts_1 = parts; _a < parts_1.length; _a++) {
                    var p = parts_1[_a];
                    if (!p.length)
                        continue;
                    g = g[p];
                    if (!g)
                        continue;
                }
                scan(g, nsRoot, 0);
            }
            scan(global[nsRoot], nsRoot, 0);
        }
    }
    (function (global) {
        if (typeof RSVP !== undefined) {
            RSVP.on && RSVP.on(function (e) {
                Q.log(e);
                Q.log((e.get_stack && e.get_stack()) || e.stack);
            });
        }
        // fake assembly for typescript apps
        ss.initAssembly({}, 'App', {});
        // for backward compability, avoid!
        global.Q$Externals = Q;
        global.Q$Config = Q.Config;
        global.Q$Culture = Q.Culture;
        global.Q$Lookup = Q.Lookup;
        global.Q$ScriptData = Q.ScriptData;
        global.Q$LT = Q.LT;
        function initializeTypes() {
            enumerateTypes(global, Q.Config.rootNamespaces, function (obj, fullName) {
                // probably Saltaralle class
                if (obj.hasOwnProperty("__typeName") &&
                    !obj.__register)
                    return;
                if (!obj.__interfaces &&
                    obj.prototype.format &&
                    fullName.substr(-9) == "Formatter") {
                    obj.__class = true;
                    obj.__interfaces = [Serenity.ISlickFormatter];
                }
                if (!obj.__class) {
                    var baseType = ss.getBaseType(obj);
                    if (baseType.__class)
                        obj.__class = true;
                }
                if (obj.__class || obj.__enum) {
                    obj.__typeName = fullName;
                    if (!obj.__assembly) {
                        obj.__assembly = ss.__assemblies['App'];
                    }
                    obj.__assembly.__types[fullName] = obj;
                }
                delete obj.__register;
            });
        }
        $(function () {
            initializeTypes();
            Q.setMobileDeviceMode();
            $(global).bind('resize', function () {
                Q.setMobileDeviceMode();
            });
        });
    })(window || {});
})(Q || (Q = {}));
var Serenity;
(function (Serenity) {
    var ColumnsKeyAttribute = (function () {
        function ColumnsKeyAttribute(value) {
            this.value = value;
        }
        return ColumnsKeyAttribute;
    }());
    Serenity.ColumnsKeyAttribute = ColumnsKeyAttribute;
    var DialogTypeAttribute = (function () {
        function DialogTypeAttribute(value) {
            this.value = value;
        }
        return DialogTypeAttribute;
    }());
    Serenity.DialogTypeAttribute = DialogTypeAttribute;
    var EditorAttribute = (function () {
        function EditorAttribute() {
        }
        return EditorAttribute;
    }());
    Serenity.EditorAttribute = EditorAttribute;
    var ElementAttribute = (function () {
        function ElementAttribute(value) {
            this.value = value;
        }
        return ElementAttribute;
    }());
    Serenity.ElementAttribute = ElementAttribute;
    var EntityTypeAttribute = (function () {
        function EntityTypeAttribute(value) {
            this.value = value;
        }
        return EntityTypeAttribute;
    }());
    Serenity.EntityTypeAttribute = EntityTypeAttribute;
    var EnumKeyAttribute = (function () {
        function EnumKeyAttribute(value) {
            this.value = value;
        }
        return EnumKeyAttribute;
    }());
    Serenity.EnumKeyAttribute = EnumKeyAttribute;
    var FlexifyAttribute = (function () {
        function FlexifyAttribute(value) {
            if (value === void 0) { value = true; }
            this.value = value;
        }
        return FlexifyAttribute;
    }());
    Serenity.FlexifyAttribute = FlexifyAttribute;
    var FormKeyAttribute = (function () {
        function FormKeyAttribute(value) {
            this.value = value;
        }
        return FormKeyAttribute;
    }());
    Serenity.FormKeyAttribute = FormKeyAttribute;
    var GeneratedCodeAttribute = (function () {
        function GeneratedCodeAttribute(origin) {
            this.origin = origin;
        }
        return GeneratedCodeAttribute;
    }());
    Serenity.GeneratedCodeAttribute = GeneratedCodeAttribute;
    var IdPropertyAttribute = (function () {
        function IdPropertyAttribute(value) {
            this.value = value;
        }
        return IdPropertyAttribute;
    }());
    Serenity.IdPropertyAttribute = IdPropertyAttribute;
    var IsActivePropertyAttribute = (function () {
        function IsActivePropertyAttribute(value) {
            this.value = value;
        }
        return IsActivePropertyAttribute;
    }());
    Serenity.IsActivePropertyAttribute = IsActivePropertyAttribute;
    var ItemNameAttribute = (function () {
        function ItemNameAttribute(value) {
            this.value = value;
        }
        return ItemNameAttribute;
    }());
    Serenity.ItemNameAttribute = ItemNameAttribute;
    var LocalTextPrefixAttribute = (function () {
        function LocalTextPrefixAttribute(value) {
            this.value = value;
        }
        return LocalTextPrefixAttribute;
    }());
    Serenity.LocalTextPrefixAttribute = LocalTextPrefixAttribute;
    var MaximizableAttribute = (function () {
        function MaximizableAttribute(value) {
            if (value === void 0) { value = true; }
            this.value = value;
        }
        return MaximizableAttribute;
    }());
    Serenity.MaximizableAttribute = MaximizableAttribute;
    var NamePropertyAttribute = (function () {
        function NamePropertyAttribute(value) {
            this.value = value;
        }
        return NamePropertyAttribute;
    }());
    Serenity.NamePropertyAttribute = NamePropertyAttribute;
    var OptionAttribute = (function () {
        function OptionAttribute() {
        }
        return OptionAttribute;
    }());
    Serenity.OptionAttribute = OptionAttribute;
    var OptionsTypeAttribute = (function () {
        function OptionsTypeAttribute(value) {
            this.value = value;
        }
        return OptionsTypeAttribute;
    }());
    Serenity.OptionsTypeAttribute = OptionsTypeAttribute;
    var PanelAttribute = (function () {
        function PanelAttribute(value) {
            if (value === void 0) { value = true; }
            this.value = value;
        }
        return PanelAttribute;
    }());
    Serenity.PanelAttribute = PanelAttribute;
    var ResizableAttribute = (function () {
        function ResizableAttribute(value) {
            if (value === void 0) { value = true; }
            this.value = value;
        }
        return ResizableAttribute;
    }());
    Serenity.ResizableAttribute = ResizableAttribute;
    var ResponsiveAttribute = (function () {
        function ResponsiveAttribute(value) {
            if (value === void 0) { value = true; }
            this.value = value;
        }
        return ResponsiveAttribute;
    }());
    Serenity.ResponsiveAttribute = ResponsiveAttribute;
    var ServiceAttribute = (function () {
        function ServiceAttribute(value) {
            this.value = value;
        }
        return ServiceAttribute;
    }());
    Serenity.ServiceAttribute = ServiceAttribute;
})(Serenity || (Serenity = {}));
/// <reference path="Attributes.ts" />
var Serenity;
(function (Serenity) {
    var Decorators;
    (function (Decorators) {
        function addAttribute(type, attr) {
            var old = type.__metadata;
            type.__metadata = type.__metadata || {};
            type.__metadata.attr = type.__metadata.attr || [];
            type.__metadata.attr.push(attr);
        }
        Decorators.addAttribute = addAttribute;
        function addMemberAttr(type, memberName, attr) {
            var old = type.__metadata;
            type.__metadata = type.__metadata || {};
            type.__metadata.members = type.__metadata.members || [];
            var member = undefined;
            for (var _i = 0, _a = type.__metadata.members; _i < _a.length; _i++) {
                var m = _a[_i];
                if (m.name == memberName) {
                    member = m;
                    break;
                }
            }
            if (!member) {
                member = { name: memberName, attr: [], type: 4, returnType: Object, sname: memberName };
                type.__metadata.members.push(member);
            }
            member.attr = member.attr || [];
            member.attr.push(attr);
        }
        Decorators.addMemberAttr = addMemberAttr;
        function columnsKey(value) {
            return function (target) {
                addAttribute(target, new Serenity.ColumnsKeyAttribute(value));
            };
        }
        Decorators.columnsKey = columnsKey;
        function dialogType(value) {
            return function (target) {
                addAttribute(target, new Serenity.DialogTypeAttribute(value));
            };
        }
        Decorators.dialogType = dialogType;
        function editor(key) {
            return function (target) {
                var attr = new Serenity.EditorAttribute();
                if (key !== undefined)
                    attr.key = key;
                addAttribute(target, attr);
            };
        }
        Decorators.editor = editor;
        function element(value) {
            return function (target) {
                addAttribute(target, new Serenity.ElementAttribute(value));
            };
        }
        Decorators.element = element;
        function entityType(value) {
            return function (target) {
                addAttribute(target, new Serenity.EntityTypeAttribute(value));
            };
        }
        Decorators.entityType = entityType;
        function enumKey(value) {
            return function (target) {
                addAttribute(target, new Serenity.EnumKeyAttribute(value));
            };
        }
        Decorators.enumKey = enumKey;
        function flexify(value) {
            if (value === void 0) { value = true; }
            return function (target) {
                addAttribute(target, new Serenity.FlexifyAttribute(value));
            };
        }
        Decorators.flexify = flexify;
        function formKey(value) {
            return function (target) {
                addAttribute(target, new Serenity.FormKeyAttribute(value));
            };
        }
        Decorators.formKey = formKey;
        function generatedCode(origin) {
            return function (target) {
                addAttribute(target, new Serenity.GeneratedCodeAttribute(origin));
            };
        }
        Decorators.generatedCode = generatedCode;
        function idProperty(value) {
            return function (target) {
                addAttribute(target, new Serenity.IdPropertyAttribute(value));
            };
        }
        Decorators.idProperty = idProperty;
        function registerClass(intf, asm) {
            return function (target) {
                target.__register = true;
                target.__class = true;
                target.__assembly = asm || ss.__assemblies['App'];
                if (intf)
                    target.__interfaces = intf;
            };
        }
        Decorators.registerClass = registerClass;
        function registerEnum(target, enumKey, asm) {
            if (!target.__enum) {
                Object.defineProperty(target, '__enum', {
                    get: function () {
                        return true;
                    }
                });
                target.prototype = target.prototype || {};
                for (var _i = 0, _a = Object.keys(target); _i < _a.length; _i++) {
                    var k = _a[_i];
                    if (isNaN(Q.parseInteger(k)) && target[k] != null && !isNaN(Q.parseInteger(target[k])))
                        target.prototype[k] = target[k];
                }
                target.__register = true;
                if (enumKey)
                    addAttribute(target, new Serenity.EnumKeyAttribute(enumKey));
            }
        }
        Decorators.registerEnum = registerEnum;
        function registerEditor(intf, asm) {
            return registerClass(intf, asm);
        }
        Decorators.registerEditor = registerEditor;
        function registerFormatter(intf, asm) {
            if (intf === void 0) { intf = [Serenity.ISlickFormatter]; }
            return registerClass(intf, asm);
        }
        Decorators.registerFormatter = registerFormatter;
        function filterable(value) {
            if (value === void 0) { value = true; }
            return function (target) {
                addAttribute(target, new Serenity.FilterableAttribute(value));
            };
        }
        Decorators.filterable = filterable;
        function itemName(value) {
            return function (target) {
                addAttribute(target, new Serenity.ItemNameAttribute(value));
            };
        }
        Decorators.itemName = itemName;
        function isActiveProperty(value) {
            return function (target) {
                addAttribute(target, new Serenity.IsActivePropertyAttribute(value));
            };
        }
        Decorators.isActiveProperty = isActiveProperty;
        function localTextPrefix(value) {
            return function (target) {
                addAttribute(target, new Serenity.LocalTextPrefixAttribute(value));
            };
        }
        Decorators.localTextPrefix = localTextPrefix;
        function maximizable(value) {
            if (value === void 0) { value = true; }
            return function (target) {
                addAttribute(target, new Serenity.MaximizableAttribute(value));
            };
        }
        Decorators.maximizable = maximizable;
        function nameProperty(value) {
            return function (target) {
                addAttribute(target, new Serenity.NamePropertyAttribute(value));
            };
        }
        Decorators.nameProperty = nameProperty;
        function option() {
            return function (target, propertyKey) {
                addMemberAttr(target.constructor, propertyKey, new Serenity.OptionAttribute());
            };
        }
        Decorators.option = option;
        function optionsType(value) {
            return function (target) {
                addAttribute(target, new Serenity.OptionsTypeAttribute(value));
            };
        }
        Decorators.optionsType = optionsType;
        function panel(value) {
            if (value === void 0) { value = true; }
            return function (target) {
                addAttribute(target, new Serenity.PanelAttribute(value));
            };
        }
        Decorators.panel = panel;
        function resizable(value) {
            if (value === void 0) { value = true; }
            return function (target) {
                addAttribute(target, new Serenity.ResizableAttribute(value));
            };
        }
        Decorators.resizable = resizable;
        function responsive(value) {
            if (value === void 0) { value = true; }
            return function (target) {
                addAttribute(target, new Serenity.ResponsiveAttribute(value));
            };
        }
        Decorators.responsive = responsive;
        function service(value) {
            return function (target) {
                addAttribute(target, new Serenity.ServiceAttribute(value));
            };
        }
        Decorators.service = service;
    })(Decorators = Serenity.Decorators || (Serenity.Decorators = {}));
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    function Criteria(field) {
        return [field];
    }
    Serenity.Criteria = Criteria;
    var Criteria;
    (function (Criteria) {
        var C = Criteria;
        function isEmpty(c) {
            return c == null ||
                c.length === 0 ||
                (c.length === 1 &&
                    typeof c[0] == "string" &&
                    c[0].length === 0);
        }
        Criteria.isEmpty = isEmpty;
        function join(c1, op, c2) {
            if (C.isEmpty(c1))
                return c2;
            if (C.isEmpty(c2))
                return c1;
            return [c1, op, c2];
        }
        Criteria.join = join;
        function paren(c) {
            return C.isEmpty(c) ? c : ['()', c];
        }
        Criteria.paren = paren;
        function and(c1, c2) {
            var rest = [];
            for (var _i = 2; _i < arguments.length; _i++) {
                rest[_i - 2] = arguments[_i];
            }
            var result = join(c1, 'and', c2);
            if (rest) {
                for (var _a = 0, rest_1 = rest; _a < rest_1.length; _a++) {
                    var k = rest_1[_a];
                    result = join(result, 'and', k);
                }
            }
            return result;
        }
        Criteria.and = and;
        function or(c1, c2) {
            var rest = [];
            for (var _i = 2; _i < arguments.length; _i++) {
                rest[_i - 2] = arguments[_i];
            }
            var result = join(c1, 'or', c2);
            if (rest) {
                for (var _a = 0, rest_2 = rest; _a < rest_2.length; _a++) {
                    var k = rest_2[_a];
                    result = join(result, 'or', k);
                }
            }
            return result;
        }
        Criteria.or = or;
    })(Criteria = Serenity.Criteria || (Serenity.Criteria = {}));
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var LazyLoadHelper;
    (function (LazyLoadHelper) {
        var autoIncrement = 0;
        function executeOnceWhenShown(element, callback) {
            autoIncrement++;
            var eventClass = 'ExecuteOnceWhenShown' + autoIncrement;
            var executed = false;
            if (element.is(':visible')) {
                callback();
            }
            else {
                var uiTabs = element.closest('.ui-tabs');
                if (uiTabs.length > 0) {
                    uiTabs.bind('tabsshow.' + eventClass, function (e) {
                        if (element.is(':visible')) {
                            uiTabs.unbind('tabsshow.' + eventClass);
                            if (!executed) {
                                executed = true;
                                element.unbind('shown.' + eventClass);
                                callback();
                            }
                        }
                    });
                }
                var dialog;
                if (element.hasClass('ui-dialog')) {
                    dialog = element.children('.ui-dialog-content');
                }
                else {
                    dialog = element.closest('.ui-dialog-content');
                }
                if (dialog.length > 0) {
                    dialog.bind('dialogopen.' + eventClass, function () {
                        dialog.unbind('dialogopen.' + eventClass);
                        if (element.is(':visible') && !executed) {
                            executed = true;
                            element.unbind('shown.' + eventClass);
                            callback();
                        }
                    });
                }
                element.bind('shown.' + eventClass, function () {
                    if (element.is(':visible')) {
                        element.unbind('shown.' + eventClass);
                        if (!executed) {
                            executed = true;
                            callback();
                        }
                    }
                });
            }
        }
        LazyLoadHelper.executeOnceWhenShown = executeOnceWhenShown;
        function executeEverytimeWhenShown(element, callback, callNowIfVisible) {
            autoIncrement++;
            var eventClass = 'ExecuteEverytimeWhenShown' + autoIncrement;
            var wasVisible = element.is(':visible');
            if (wasVisible && callNowIfVisible) {
                callback();
            }
            var check = function (e) {
                if (element.is(':visible')) {
                    if (!wasVisible) {
                        wasVisible = true;
                        callback();
                    }
                }
                else {
                    wasVisible = false;
                }
            };
            var uiTabs = element.closest('.ui-tabs');
            if (uiTabs.length > 0) {
                uiTabs.bind('tabsactivate.' + eventClass, check);
            }
            var dialog = element.closest('.ui-dialog-content');
            if (dialog.length > 0) {
                dialog.bind('dialogopen.' + eventClass, check);
            }
            element.bind('shown.' + eventClass, check);
        }
        LazyLoadHelper.executeEverytimeWhenShown = executeEverytimeWhenShown;
    })(LazyLoadHelper = Serenity.LazyLoadHelper || (Serenity.LazyLoadHelper = {}));
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var TabsExtensions;
    (function (TabsExtensions) {
        function setDisabled(tabs, tabKey, isDisabled) {
            if (!tabs)
                return;
            var ibk = indexByKey(tabs);
            if (!ibk)
                return;
            var index = ibk[tabKey];
            if (index == null) {
                return;
            }
            if (index === tabs.tabs('option', 'active')) {
                tabs.tabs('option', 'active', 0);
            }
            tabs.tabs(isDisabled ? 'disable' : 'enable', index);
        }
        TabsExtensions.setDisabled = setDisabled;
        function activeTabKey(tabs) {
            var href = tabs.children('ul')
                .children('li')
                .eq(tabs.tabs('option', 'active'))
                .children('a')
                .attr('href')
                .toString();
            var prefix = '_Tab';
            var lastIndex = href.lastIndexOf(prefix);
            if (lastIndex >= 0) {
                href = href.substr(lastIndex + prefix.length);
            }
            return href;
        }
        TabsExtensions.activeTabKey = activeTabKey;
        function indexByKey(tabs) {
            var indexByKey = tabs.data('indexByKey');
            if (!indexByKey) {
                indexByKey = {};
                tabs.children('ul').children('li').children('a').each(function (index, el) {
                    var href = el.getAttribute('href').toString();
                    var prefix = '_Tab';
                    var lastIndex = href.lastIndexOf(prefix);
                    if (lastIndex >= 0) {
                        href = href.substr(lastIndex + prefix.length);
                    }
                    indexByKey[href] = index;
                });
                tabs.data('indexByKey', indexByKey);
            }
            return indexByKey;
        }
        TabsExtensions.indexByKey = indexByKey;
    })(TabsExtensions = Serenity.TabsExtensions || (Serenity.TabsExtensions = {}));
})(Serenity || (Serenity = {}));
/// <reference path="../../../../typings/jquery/jquery.d.ts" />
/// <reference path="../../../../typings/jquery.validation/jquery.validation.d.ts" />
var Serenity;
(function (Serenity) {
    var Widget = (function () {
        function Widget(element, options) {
            var _this = this;
            this.element = element;
            this.options = options || {};
            this.widgetName = Widget.getWidgetName(ss.getInstanceType(this));
            this.uniqueName = this.widgetName + (Widget.nextWidgetNumber++).toString();
            if (element.data(this.widgetName)) {
                throw new ss.Exception(Q.format("The element already has widget '{0}'!", this.widgetName));
            }
            element.bind('remove.' + this.widgetName, function (e) {
                if (e.bubbles || e.cancelable) {
                    return;
                }
                _this.destroy();
            }).data(this.widgetName, this);
            this.addCssClass();
            if (this.isAsyncWidget()) {
                window.setTimeout(function () {
                    if (element && !_this.asyncPromise) {
                        _this.asyncPromise = _this.initializeAsync();
                    }
                }, 0);
            }
        }
        Widget.prototype.destroy = function () {
            this.element.removeClass('s-' + ss.getTypeName(ss.getInstanceType(this)));
            this.element.unbind('.' + this.widgetName).removeData(this.widgetName);
            this.element = null;
            this.asyncPromise = null;
        };
        Widget.prototype.addCssClass = function () {
            this.element.addClass(this.getCssClass());
        };
        Widget.prototype.getCssClass = function () {
            var type = ss.getInstanceType(this);
            var klass = 's-' + ss.getTypeName(type);
            var fullClass = Q.replaceAll(ss.getTypeFullName(type), '.', '-');
            for (var _i = 0, _a = Q.Config.rootNamespaces; _i < _a.length; _i++) {
                var k = _a[_i];
                if (Q.startsWith(fullClass, k + '-')) {
                    fullClass = fullClass.substr(k.length + 1);
                    break;
                }
            }
            fullClass = 's-' + fullClass;
            if (klass === fullClass) {
                return klass;
            }
            return klass + ' ' + fullClass;
        };
        Widget.prototype.initializeAsync = function () {
            return RSVP.resolve();
        };
        Widget.prototype.isAsyncWidget = function () {
            return ss.isInstanceOfType(this, Serenity.IAsyncInit);
        };
        Widget.getWidgetName = function (type) {
            return Q.replaceAll(ss.getTypeFullName(type), '.', '_');
        };
        Widget.elementFor = function (editorType) {
            var elementAttr = ss.getAttributes(editorType, Serenity.ElementAttribute, true);
            var elementHtml = ((elementAttr.length > 0) ? elementAttr[0].value : '<input/>');
            return $(elementHtml);
        };
        ;
        Widget.create = function (params) {
            var widget;
            if (ss.isAssignableFrom(Serenity.IDialog, params.type)) {
                widget = new params.type(params.options);
                if (params.container)
                    widget.element.appendTo(params.container);
                params.element && params.element(widget.element);
            }
            else {
                var e = Widget.elementFor(params.type);
                if (params.container)
                    e.appendTo(params.container);
                params.element && params.element(e);
                widget = new params.type(e, params.options);
            }
            widget.init(params.init);
            return widget;
        };
        Widget.prototype.init = function (action) {
            var _this = this;
            var promise = this.initialize();
            if (action) {
                promise.then(function () { return action(_this); });
            }
            return this;
        };
        Widget.prototype.initialize = function () {
            if (!this.isAsyncWidget()) {
                return RSVP.resolve();
            }
            if (!this.asyncPromise) {
                this.asyncPromise = this.initializeAsync();
            }
            return this.asyncPromise;
        };
        Widget.nextWidgetNumber = 0;
        Widget = __decorate([
            Serenity.Decorators.registerClass()
        ], Widget);
        return Widget;
    }());
    Serenity.Widget = Widget;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var TemplatedWidget = (function (_super) {
        __extends(TemplatedWidget, _super);
        function TemplatedWidget(container, options) {
            _super.call(this, container, options);
            this.idPrefix = this.uniqueName + '_';
            var widgetMarkup = this.getTemplate().replace(new RegExp('~_', 'g'), this.idPrefix);
            widgetMarkup = Q.jsRender(widgetMarkup);
            this.element.html(widgetMarkup);
        }
        TemplatedWidget.prototype.byId = function (id) {
            return $('#' + this.idPrefix + id);
        };
        TemplatedWidget.prototype.byID = function (id, type) {
            return Serenity.WX.getWidget(this.byId(id), type);
        };
        TemplatedWidget.prototype.getTemplateName = function () {
            var noGeneric = function (s) {
                var dollar = s.indexOf('$');
                if (dollar >= 0) {
                    return s.substr(0, dollar);
                }
                return s;
            };
            var type = ss.getInstanceType(this);
            var fullName = ss.getTypeFullName(type);
            var templateNames = TemplatedWidget.templateNames;
            var cachedName = TemplatedWidget.templateNames[fullName];
            if (cachedName != null) {
                return cachedName;
            }
            while (type && type !== Serenity.Widget) {
                var name = noGeneric(ss.getTypeFullName(type));
                for (var _i = 0, _a = Q.Config.rootNamespaces; _i < _a.length; _i++) {
                    var k = _a[_i];
                    if (Q.startsWith(name, k + '.')) {
                        name = name.substr(k.length + 1);
                        break;
                    }
                }
                if (Q.canLoadScriptData('Template.' + name)) {
                    templateNames[fullName] = name;
                    return name;
                }
                name = Q.replaceAll(name, '.', '_');
                if (Q.canLoadScriptData('Template.' + name) ||
                    $('script#Template_' + name).length > 0) {
                    templateNames[fullName] = name;
                    return name;
                }
                name = noGeneric(ss.getTypeName(type));
                if (Q.canLoadScriptData('Template.' + name) ||
                    $('script#Template_' + name).length > 0) {
                    TemplatedWidget.templateNames[fullName] = name;
                    return name;
                }
                type = ss.getBaseType(type);
            }
            templateNames[fullName] = cachedName = noGeneric(ss.getTypeName(ss.getInstanceType(this)));
            return cachedName;
        };
        TemplatedWidget.prototype.getTemplate = function () {
            var templateName = this.getTemplateName();
            var script = $('script#Template_' + templateName);
            if (script.length > 0) {
                return script.html();
            }
            var template = Q.getTemplate(templateName);
            if (template == null) {
                throw new Error(Q.format("Can't locate template for widget '{0}' with name '{1}'!", ss.getTypeName(ss.getInstanceType(this)), templateName));
            }
            return template;
        };
        TemplatedWidget.templateNames = {};
        TemplatedWidget = __decorate([
            Serenity.Decorators.registerClass()
        ], TemplatedWidget);
        return TemplatedWidget;
    }(Serenity.Widget));
    Serenity.TemplatedWidget = TemplatedWidget;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var TemplatedDialog = (function (_super) {
        __extends(TemplatedDialog, _super);
        function TemplatedDialog(options) {
            _super.call(this, Q.newBodyDiv(), options);
            this.isPanel = ss.getAttributes(ss.getInstanceType(this), Serenity.PanelAttribute, true).length > 0;
            if (!this.isPanel) {
                this.initDialog();
            }
            this.initValidator();
            this.initTabs();
            this.initToolbar();
        }
        TemplatedDialog.getCssSize = function (element, name) {
            var cssSize = element.css(name);
            if (cssSize == null) {
                return null;
            }
            if (!Q.endsWith(cssSize, 'px')) {
                return null;
            }
            cssSize = cssSize.substr(0, cssSize.length - 2);
            var i = Q.parseInteger(cssSize);
            if (i == null || isNaN(i) || i == 0)
                return null;
            return i;
        };
        TemplatedDialog.applyCssSizes = function (opt, dialogClass) {
            var size;
            var dialog = $('<div/>').hide().addClass(dialogClass).appendTo(document.body);
            try {
                var sizeHelper = $('<div/>').addClass('size').appendTo(dialog);
                size = TemplatedDialog.getCssSize(sizeHelper, 'minWidth');
                if (size != null)
                    opt.minWidth = size;
                size = TemplatedDialog.getCssSize(sizeHelper, 'width');
                if (size != null)
                    opt.width = size;
                size = TemplatedDialog.getCssSize(sizeHelper, 'height');
                if (size != null)
                    opt.height = size;
                size = TemplatedDialog.getCssSize(sizeHelper, 'minHeight');
                if (size != null)
                    opt.minHeight = size;
            }
            finally {
                dialog.remove();
            }
        };
        ;
        TemplatedDialog.prototype.destroy = function () {
            this.tabs && this.tabs.tabs('destroy');
            this.tabs = null;
            this.toolbar && this.toolbar.destroy();
            this.toolbar = null;
            this.validator && this.byId('Form').remove();
            this.validator = null;
            !this.isPanel && this.element.dialog('destroy');
            $(window).unbind('.' + this.uniqueName);
            _super.prototype.destroy.call(this);
        };
        TemplatedDialog.prototype.initDialog = function () {
            var _this = this;
            this.element.dialog(this.getDialogOptions());
            var type = ss.getInstanceType(this);
            this.responsive = Q.Config.responsiveDialogs ||
                ss.getAttributes(type, Serenity.ResponsiveAttribute, true).length > 0;
            if (this.responsive) {
                Serenity.DialogExtensions.dialogResizable(this.element);
                $(window).bind('resize.' + this.uniqueName, function (e) {
                    if (_this.element && _this.element.is(':visible')) {
                        _this.handleResponsive();
                    }
                });
                this.element.closest('.ui-dialog').addClass('flex-layout');
            }
            else if (ss.getAttributes(type, Serenity.FlexifyAttribute, true).length > 0) {
                Serenity.DialogExtensions.dialogFlexify(this.element);
                Serenity.DialogExtensions.dialogResizable(this.element);
            }
            if (ss.getAttributes(type, Serenity.MaximizableAttribute, true).length > 0) {
                Serenity.DialogExtensions.dialogMaximizable(this.element);
            }
            var self = this;
            this.element.bind('dialogopen.' + this.uniqueName, function () {
                $(document.body).addClass('modal-dialog-open');
                if (_this.responsive) {
                    _this.handleResponsive();
                }
                self.onDialogOpen();
            });
            this.element.bind('dialogclose.' + this.uniqueName, function () {
                $(document.body).toggleClass('modal-dialog-open', $('.ui-dialog:visible').length > 0);
                self.onDialogClose();
            });
        };
        TemplatedDialog.prototype.initToolbar = function () {
            var toolbarDiv = this.byId('Toolbar');
            if (toolbarDiv.length === 0) {
                return;
            }
            var hotkeyContext = this.element.closest('.ui-dialog');
            if (hotkeyContext.length === 0) {
                hotkeyContext = this.element;
            }
            var opt = { buttons: this.getToolbarButtons(), hotkeyContext: hotkeyContext[0] };
            this.toolbar = new Serenity.Toolbar(toolbarDiv, opt);
        };
        TemplatedDialog.prototype.getToolbarButtons = function () {
            return [];
        };
        TemplatedDialog.prototype.getValidatorOptions = function () {
            return {};
        };
        TemplatedDialog.prototype.initValidator = function () {
            var form = this.byId('Form');
            if (form.length > 0) {
                var valOptions = this.getValidatorOptions();
                this.validator = form.validate(Q.validateOptions(valOptions));
            }
        };
        TemplatedDialog.prototype.resetValidation = function () {
            this.validator && this.validator.resetAll();
        };
        TemplatedDialog.prototype.validateForm = function () {
            return this.validator == null || !!this.validator.form();
        };
        TemplatedDialog.prototype.dialogOpen = function () {
            if (this.isPanel) {
                return;
            }
            this.element.dialog().dialog('open');
        };
        TemplatedDialog.prototype.onDialogOpen = function () {
            $(':input:eq(0)', this.element).focus();
            this.arrange();
            this.tabs && this.tabs.tabs('option', 'active', 0);
        };
        TemplatedDialog.prototype.arrange = function () {
            this.element.find('.require-layout').filter(':visible').each(function (i, e) {
                $(e).triggerHandler('layout');
            });
        };
        TemplatedDialog.prototype.onDialogClose = function () {
            var _this = this;
            $(document).trigger('click');
            // for tooltips etc.
            if ($.qtip) {
                $(document.body).children('.qtip').each(function (index, el) {
                    $(el).qtip('hide');
                });
            }
            window.setTimeout(function () {
                var element = _this.element;
                _this.destroy();
                element.remove();
                Q.positionToastContainer(false);
            }, 0);
        };
        TemplatedDialog.prototype.addCssClass = function () {
            var type = ss.getInstanceType(this);
            if (ss.getAttributes(type, Serenity.PanelAttribute, true).length > 0) {
                _super.prototype.addCssClass.call(this);
            }
            // will add css class to ui-dialog container, not content element
        };
        TemplatedDialog.prototype.getDialogOptions = function () {
            var opt = {};
            var dialogClass = 's-Dialog ' + this.getCssClass();
            opt.dialogClass = dialogClass;
            opt.width = 920;
            TemplatedDialog.applyCssSizes(opt, dialogClass);
            opt.autoOpen = false;
            var type = ss.getInstanceType(this);
            opt.resizable = ss.getAttributes(type, Serenity.ResizableAttribute, true).length > 0;
            opt.modal = true;
            opt.position = { my: 'center', at: 'center', of: $(window.window) };
            return opt;
        };
        TemplatedDialog.prototype.dialogClose = function () {
            if (this.isPanel) {
                return;
            }
            this.element.dialog().dialog('close');
        };
        Object.defineProperty(TemplatedDialog.prototype, "dialogTitle", {
            get: function () {
                if (this.isPanel) {
                    return null;
                }
                return this.element.dialog('option', 'title');
            },
            set: function (value) {
                if (this.isPanel) {
                    return;
                }
                this.element.dialog('option', 'title', value);
            },
            enumerable: true,
            configurable: true
        });
        TemplatedDialog.prototype.set_dialogTitle = function (value) {
            this.dialogTitle = value;
        };
        TemplatedDialog.prototype.initTabs = function () {
            var tabsDiv = this.byId('Tabs');
            if (tabsDiv.length === 0) {
                return;
            }
            this.tabs = tabsDiv.tabs({});
        };
        TemplatedDialog.prototype.handleResponsive = function () {
            var dlg = this.element.dialog();
            var uiDialog = this.element.closest('.ui-dialog');
            if ($(document.body).hasClass('mobile-device')) {
                var data = this.element.data('responsiveData');
                if (!data) {
                    data = {};
                    data.draggable = dlg.dialog('option', 'draggable');
                    data.resizable = dlg.dialog('option', 'resizable');
                    var pos = uiDialog.position();
                    data.left = pos.left;
                    data.top = pos.top;
                    data.width = uiDialog.width();
                    data.height = uiDialog.height();
                    data.contentHeight = this.element.height();
                    this.element.data('responsiveData', data);
                    dlg.dialog('option', 'draggable', false);
                    dlg.dialog('option', 'resizable', false);
                }
                uiDialog.addClass('mobile-layout');
                uiDialog.css({ left: '0px', top: '0px', width: $(window).width() + 'px', height: $(window).height() + 'px' });
                $(document.body).scrollTop(0);
                Q.layoutFillHeight(this.element);
            }
            else {
                var d = this.element.data('responsiveData');
                if (d) {
                    dlg.dialog('option', 'draggable', d.draggable);
                    dlg.dialog('option', 'resizable', d.resizable);
                    this.element.closest('.ui-dialog').css({ left: '0px', top: '0px', width: d.width + 'px', height: d.height + 'px' });
                    this.element.height(d.contentHeight);
                    uiDialog.removeClass('mobile-layout');
                    this.element.removeData('responsiveData');
                }
            }
        };
        TemplatedDialog = __decorate([
            Serenity.Decorators.registerClass([Serenity.IDialog])
        ], TemplatedDialog);
        return TemplatedDialog;
    }(Serenity.TemplatedWidget));
    Serenity.TemplatedDialog = TemplatedDialog;
})(Serenity || (Serenity = {}));
/// <reference path="../../../../typings/sortablejs/sortablejs.d.ts" />
var Serenity;
(function (Serenity) {
    var ColumnPickerDialog = (function (_super) {
        __extends(ColumnPickerDialog, _super);
        function ColumnPickerDialog() {
            var _this = this;
            _super.call(this);
            new Serenity.QuickSearchInput(this.byId("Search"), {
                onSearch: function (fld, txt, done) {
                    txt = Q.trimToNull(txt);
                    if (txt != null)
                        txt = Select2.util.stripDiacritics(txt.toLowerCase());
                    _this.element.find('li').each(function (x, e) {
                        $(e).toggle(!txt || Select2.util.stripDiacritics($(e).text().toLowerCase()).indexOf(txt) >= 0);
                    });
                }
            });
            this.ulVisible = this.byId("VisibleCols");
            this.ulHidden = this.byId("HiddenCols");
        }
        ColumnPickerDialog.createToolButton = function (grid) {
            return {
                hint: Q.text("Controls.ColumnPickerDialog.Title"),
                cssClass: "column-picker-button",
                onClick: function () {
                    var picker = new Serenity.ColumnPickerDialog();
                    picker.allColumns = grid.allColumns;
                    if (grid.initialSettings) {
                        var initialSettings = grid.initialSettings;
                        if (initialSettings.columns && initialSettings.columns.length)
                            picker.defaultColumns = initialSettings.columns.map(function (x) { return x.id; });
                    }
                    picker.visibleColumns = grid.slickGrid.getColumns().map(function (x) { return x.id; });
                    picker.done = function () {
                        grid.allColumns = picker.allColumns;
                        var visible = picker.allColumns.filter(function (x) { return x.visible === true; });
                        grid.slickGrid.setColumns(visible);
                        grid.persistSettings();
                        grid.refresh();
                    };
                    picker.dialogOpen();
                }
            };
        };
        ColumnPickerDialog.prototype.getDialogOptions = function () {
            var _this = this;
            var opt = _super.prototype.getDialogOptions.call(this);
            opt.title = Q.text("Controls.ColumnPickerDialog.Title");
            opt.width = 600;
            opt.buttons = [
                {
                    text: Q.text("Controls.ColumnPickerDialog.RestoreDefaults"),
                    click: function () {
                        var liByKey = {};
                        _this.ulVisible.children().add(_this.ulHidden.children()).each(function (x, e) {
                            var el = $(e);
                            liByKey[el.data('key')] = el;
                        });
                        var last = null;
                        for (var _i = 0, _a = _this.defaultColumns; _i < _a.length; _i++) {
                            var id = _a[_i];
                            var li = liByKey[id];
                            if (!li)
                                continue;
                            if (last == null)
                                li.prependTo(_this.ulVisible);
                            else
                                li.insertAfter(last);
                            last = li;
                            var key = li.data('key');
                            delete liByKey[key];
                        }
                        for (var key in liByKey)
                            liByKey[key].appendTo(_this.ulHidden);
                        _this.updateListStates();
                    }
                },
                {
                    text: Q.text("Dialogs.OkButton"),
                    click: function () {
                        var newColumns = [];
                        for (var _i = 0, _a = _this.allColumns; _i < _a.length; _i++) {
                            var col = _a[_i];
                            col.visible = false;
                        }
                        _this.visibleColumns = _this.ulVisible.children().toArray().map(function (x) {
                            var id = $(x).data("key");
                            var col = _this.colById[id];
                            col.visible = true;
                            newColumns.push(col);
                            return id;
                        });
                        for (var _b = 0, _c = _this.allColumns; _b < _c.length; _b++) {
                            var col = _c[_b];
                            if (!col.visible)
                                newColumns.push(col);
                        }
                        _this.allColumns = newColumns;
                        _this.done && _this.done();
                        _this.dialogClose();
                    }
                },
                {
                    text: Q.text("Dialogs.CancelButton"),
                    click: function () {
                        _this.dialogClose();
                    }
                }
            ];
            return opt;
        };
        ColumnPickerDialog.prototype.getTitle = function (col) {
            if (col.id == "__select__")
                return "[x]";
            return col.name || col.toolTip || col.id;
        };
        ColumnPickerDialog.prototype.createLI = function (col) {
            return $("\n<li data-key=\"" + col.id + "\">\n  <span class=\"drag-handle\">\u2630</span>\n  " + Q.htmlEncode(this.getTitle(col)) + "\n  <i class=\"js-hide\" title=\"" + Q.text("Controls.ColumnPickerDialog.HideHint") + "\">\u2716</i>\n  <i class=\"js-show icon-eye\" title=\"" + Q.text("Controls.ColumnPickerDialog.ShowHint") + "\"></i>\n</li>");
        };
        ColumnPickerDialog.prototype.updateListStates = function () {
            this.ulVisible.children().removeClass("bg-info").addClass("bg-success");
            this.ulHidden.children().removeClass("bg-success").addClass("bg-info");
        };
        ColumnPickerDialog.prototype.setupColumns = function () {
            var _this = this;
            this.allColumns = this.allColumns || [];
            this.visibleColumns = this.visibleColumns || [];
            var visible = {};
            for (var _i = 0, _a = this.visibleColumns; _i < _a.length; _i++) {
                var id = _a[_i];
                visible[id] = true;
            }
            this.colById = {};
            for (var _b = 0, _c = this.allColumns; _b < _c.length; _b++) {
                var c_1 = _c[_b];
                this.colById[c_1.id] = c_1;
            }
            if (this.defaultColumns == null)
                this.defaultColumns = this.visibleColumns.slice(0);
            var hidden = [];
            for (var _d = 0, _e = this.allColumns; _d < _e.length; _d++) {
                var c_2 = _e[_d];
                if (!visible[c_2.id] && (!c_2.sourceItem || c_2.sourceItem.filterOnly !== true)) {
                    hidden.push(c_2);
                }
            }
            var hiddenColumns = hidden.sort(function (a, b) { return Q.turkishLocaleCompare(_this.getTitle(a), _this.getTitle(b)); });
            for (var _f = 0, _g = this.visibleColumns; _f < _g.length; _f++) {
                var id = _g[_f];
                var c = this.colById[id];
                if (!c)
                    continue;
                this.createLI(c).appendTo(this.ulVisible);
            }
            for (var _h = 0, hiddenColumns_1 = hiddenColumns; _h < hiddenColumns_1.length; _h++) {
                var c_3 = hiddenColumns_1[_h];
                this.createLI(c_3).appendTo(this.ulHidden);
            }
            this.updateListStates();
            if (typeof Sortable !== "undefined" &&
                Sortable.create) {
                Sortable.create(this.ulVisible[0], {
                    group: this.uniqueName + "_group",
                    filter: '.js-hide',
                    onFilter: function (evt) {
                        $(evt.item).appendTo(_this.ulHidden);
                        _this.updateListStates();
                    },
                    onEnd: function (evt) { return _this.updateListStates(); }
                });
                Sortable.create(this.ulHidden[0], {
                    group: this.uniqueName + "_group",
                    sort: false,
                    filter: '.js-show',
                    onFilter: function (evt) {
                        $(evt.item).appendTo(_this.ulVisible);
                        _this.updateListStates();
                    },
                    onEnd: function (evt) { return _this.updateListStates(); }
                });
            }
        };
        ColumnPickerDialog.prototype.onDialogOpen = function () {
            _super.prototype.onDialogOpen.call(this);
            this.element.find("input").removeAttr("disabled");
            this.element.closest('.ui-dialog').children(".ui-dialog-buttonpane")
                .find('button').eq(0).addClass("restore-defaults")
                .next().focus();
            this.setupColumns();
            Q.centerDialog(this.element);
        };
        ColumnPickerDialog.prototype.getTemplate = function () {
            return "\n<div class=\"search\"><input id=\"~_Search\" type=\"text\" disabled /></div>\n<div class=\"columns-container\">\n<div class=\"column-list visible-list bg-success\">\n  <h5><i class=\"icon-eye\"></i> " + Q.text("Controls.ColumnPickerDialog.VisibleColumns") + "</h5>\n  <ul id=\"~_VisibleCols\"></ul>\n</div>\n<div class=\"column-list hidden-list bg-info\">\n  <h5><i class=\"icon-list\"></i> " + Q.text("Controls.ColumnPickerDialog.HiddenColumns") + "</h5>\n  <ul id=\"~_HiddenCols\"></ul>\n</div>\n</div>";
        };
        ColumnPickerDialog = __decorate([
            Serenity.Decorators.registerClass(),
            Serenity.Decorators.resizable(),
            Serenity.Decorators.responsive()
        ], ColumnPickerDialog);
        return ColumnPickerDialog;
    }(Serenity.TemplatedDialog));
    Serenity.ColumnPickerDialog = ColumnPickerDialog;
})(Serenity || (Serenity = {}));
if ($.fn.button && $.fn.button.noConflict) {
    var btn = $.fn.button.noConflict();
    $.fn.btn = btn;
}
$.fn.flexHeightOnly = function (flexY) {
    if (flexY === void 0) { flexY = 1; }
    return this.flexWidthHeight(0, flexY);
};
$.fn.flexWidthOnly = function (flexX) {
    if (flexX === void 0) { flexX = 1; }
    return this.flexWidthHeight(flexX, 0);
};
$.fn.flexWidthHeight = function (flexX, flexY) {
    if (flexX === void 0) { flexX = 1; }
    if (flexY === void 0) { flexY = 1; }
    return this.addClass('flexify').data('flex-x', flexX).data('flex-y', flexY);
};
$.fn.flexX = function (flexX) {
    return this.data('flex-x', flexX);
};
$.fn.flexY = function (flexY) {
    return this.data('flex-y', flexY);
};
if ($.fn.button && $.fn.button.noConflict) {
    var btn = $.fn.button.noConflict();
    $.fn.btn = btn;
}
/// <reference path="../../typings/jquery/jquery.d.ts" />
// PAGER -----
(function ($) {
    $.widget("ui.slickPager", {
        options: {
            view: null,
            showRowsPerPage: true,
            rowsPerPageOptions: [20, 100, 500, 2000],
            onRpChange: null
        },
        _create: function () {
            var self = this;
            var o = self.options;
            var v = o.view;
            if (!v)
                throw "SlickPager requires view option to be set!";
            this.element.addClass('s-SlickPager slick-pg')
                .html('<div class="slick-pg-in">' +
                '<div class="slick-pg-grp">' +
                '<div class="slick-pg-first slick-pg-btn"><span class="slick-pg-btn-span"></span></div>' +
                '<div class="slick-pg-prev slick-pg-btn"><span class="slick-pg-btn-span"></span></div>' +
                '</div><div class="slick-pg-sep"></div><div class="slick-pg-grp">' +
                '<span class="slick-pg-control">&nbsp;' + Q.text("Controls.Pager.Page") +
                '&nbsp;<input class="slick-pg-current" type="text" size="4" value="1" /> / ' +
                '<span class="slick-pg-total"> 1 </span></span>' +
                '</div><div class="slick-pg-sep"></div><div class="slick-pg-grp">' +
                '<div class="slick-pg-next slick-pg-btn"><span class="slick-pg-btn-span"></span></div>' +
                '<div class="slick-pg-last slick-pg-btn"><span class="slick-pg-btn-span"></span></div>' +
                '</div><div class="slick-pg-sep"></div><div class="slick-pg-grp">' +
                '<div class="slick-pg-reload slick-pg-btn"><span class="slick-pg-btn-span"></span></div>' +
                '</div><div class="slick-pg-sep"></div>' +
                '<div class="slick-pg-grp"><span class="slick-pg-stat"></span></div></div>');
            $('.slick-pg-reload', this.element).click(function () { v.populate(); });
            $('.slick-pg-first', this.element).click(function () { self._changePage('first'); });
            $('.slick-pg-prev', this.element).click(function () { self._changePage('prev'); });
            $('.slick-pg-next', this.element).click(function () { self._changePage('next'); });
            $('.slick-pg-last', this.element).click(function () { self._changePage('last'); });
            $('.slick-pg-current', this.element).keydown(function (e) { if (e.keyCode == 13)
                self._changePage('input'); });
            if (self.options.showRowsPerPage) {
                var opt, sel = "";
                for (var nx = 0; nx < o.rowsPerPageOptions.length; nx++) {
                    if (v.rowsPerPage == o.rowsPerPageOptions[nx])
                        sel = 'selected="selected"';
                    else
                        sel = '';
                    opt += "<option value='" + o.rowsPerPageOptions[nx] + "' " + sel + " >" + o.rowsPerPageOptions[nx] + "&nbsp;&nbsp;</option>";
                }
                ;
                $('.slick-pg-in', this.element).prepend('<div class="slick-pg-grp"><select class="slick-pg-size" name="rp">' + opt + '</select></div><div class="slick-pg-sep"></div>');
                $('select.slick-pg-size', this.element).change(function () {
                    if (o.onRowsPerPageChange)
                        o.onRowsPerPageChange(+this.value);
                    else {
                        v.newp = 1;
                        v.setPagingOptions({
                            page: 1,
                            rowsPerPage: +this.value
                        });
                    }
                });
            }
            v.onPagingInfoChanged.subscribe(function () {
                self._updatePager();
            });
        },
        destroy: function () {
            $.Widget.prototype.destroy.apply(this, arguments);
        },
        _changePage: function (ctype) {
            var view = this.options.view;
            if (!view || view.loading)
                return true;
            var info = view.getPagingInfo();
            var pages = (!info.rowsPerPage || !info.totalCount) ? 1 : Math.ceil(info.totalCount / info.rowsPerPage);
            var newp;
            switch (ctype) {
                case 'first':
                    newp = 1;
                    break;
                case 'prev':
                    if (info.page > 1)
                        newp = parseInt(info.page) - 1;
                    break;
                case 'next':
                    if (info.page < pages)
                        newp = parseInt(info.page) + 1;
                    break;
                case 'last':
                    newp = pages;
                    break;
                case 'input':
                    var nv = parseInt($('input.slick-pg-current', this.element).val());
                    if (isNaN(nv))
                        nv = 1;
                    else if (nv < 1)
                        nv = 1;
                    else if (nv > pages)
                        nv = pages;
                    $('.slick-pg-current', this.element).val(nv);
                    newp = nv;
                    break;
            }
            if (newp == info.page)
                return false;
            if (this.options.onChangePage)
                this.options.onChangePage(newp);
            else {
                view.setPagingOptions({ page: newp });
            }
        },
        _updatePager: function () {
            var view = this.options.view;
            var info = view.getPagingInfo();
            var pages = (!info.rowsPerPage || !info.totalCount) ? 1 : Math.ceil(info.totalCount / info.rowsPerPage);
            $('.slick-pg-current', this.element).val(info.page);
            $('.slick-pg-total', this.element).html(pages);
            var r1 = (info.page - 1) * info.rowsPerPage + 1;
            var r2 = r1 + info.rowsPerPage - 1;
            if (info.totalCount < r2)
                r2 = info.totalCount;
            var stat;
            if (info.loading) {
                stat = Q.text("Controls.Pager.LoadingStatus");
            }
            else if (info.error) {
                stat = info.error;
            }
            else if (info.totalCount > 0) {
                stat = Q.text("Controls.Pager.PageStatus");
                stat = stat.replace(/{from}/, r1);
                stat = stat.replace(/{to}/, r2);
                stat = stat.replace(/{total}/, info.totalCount);
            }
            else
                stat = Q.text("Controls.Pager.NoRowStatus");
            $('.slick-pg-stat', this.element).html(stat);
            $('.slick-pg-size', this.element).val((info.rowsPerPage || 0).toString());
        },
        _setOption: function (key, value) {
            $.Widget.prototype._setOption.apply(this, arguments);
        }
    });
})(jQuery);
var Slick;
(function (Slick) {
    var Data;
    (function (Data) {
        var GroupItemMetadataProvider;
    })(Data = Slick.Data || (Slick.Data = {}));
})(Slick || (Slick = {}));
var Slick;
(function (Slick) {
    var RemoteView = (function () {
        function RemoteView(options) {
            var self = this;
            var defaults = {
                groupItemMetadataProvider: null,
                inlineFilters: false
            };
            var idProperty;
            var items = [];
            var rows = [];
            var idxById = {};
            var rowsById = null;
            var filter = null;
            var updated = null;
            var suspend = 0;
            var sortAsc = true;
            var fastSortField;
            var sortComparer;
            var refreshHints = {};
            var prevRefreshHints = {};
            var filterArgs;
            var filteredItems = [];
            var compiledFilter;
            var compiledFilterWithCaching;
            var filterCache = [];
            var groupingInfoDefaults = {
                getter: null,
                formatter: null,
                comparer: function (a, b) {
                    return (a.value === b.value ? 0 :
                        (a.value > b.value ? 1 : -1));
                },
                predefinedValues: [],
                aggregateEmpty: false,
                aggregateCollapsed: false,
                aggregateChildGroups: false,
                collapsed: false,
                displayTotalsRow: true,
                lazyTotalsCalculation: false
            };
            var summaryOptions = {};
            var groupingInfos = [];
            var groups = [];
            var toggledGroupsByLevel = [];
            var groupingDelimiter = ':|:';
            var page = 1;
            var totalRows = 0;
            var onRowCountChanged = new Slick.Event();
            var onRowsChanged = new Slick.Event();
            var onPagingInfoChanged = new Slick.Event();
            var loading = false;
            var errorMessage = null;
            var populateLocks = 0;
            var populateCalls = 0;
            var contentType;
            var dataType;
            var totalCount = null;
            var onDataChanged = new Slick.Event();
            var onDataLoading = new Slick.Event();
            var onDataLoaded = new Slick.Event();
            var onClearData = new Slick.Event();
            var intf;
            function beginUpdate() {
                suspend++;
            }
            function endUpdate() {
                suspend--;
                if (suspend <= 0)
                    refresh();
            }
            function setRefreshHints(hints) {
                refreshHints = hints;
            }
            function setFilterArgs(args) {
                filterArgs = args;
            }
            function updateIdxById(startingIndex) {
                startingIndex = startingIndex || 0;
                var id;
                for (var i = startingIndex, l = items.length; i < l; i++) {
                    id = items[i][idProperty];
                    if (id === undefined) {
                        var msg = "Each data element must implement a unique '" +
                            idProperty + "' property. Object at index '" + i + "' " +
                            "has no identity value: ";
                        msg += $.toJSON(items[i]);
                        throw msg;
                    }
                    idxById[id] = i;
                }
            }
            function ensureIdUniqueness() {
                var id;
                for (var i = 0, l = items.length; i < l; i++) {
                    id = items[i][idProperty];
                    if (id === undefined || idxById[id] !== i) {
                        var msg = "Each data element must implement a unique '" +
                            idProperty + "' property. Object at index '" + i + "' ";
                        if (id == undefined)
                            msg += "has no identity value: ";
                        else
                            msg += "has repeated identity value '" + id + "': ";
                        msg += $.toJSON(items[i]);
                        throw msg;
                    }
                }
            }
            function getItems() {
                return items;
            }
            function setItems(data) {
                items = filteredItems = data;
                idxById = {};
                rowsById = null;
                summaryOptions.totals = {};
                updateIdxById();
                ensureIdUniqueness();
                refresh();
                onDataChanged.notify({ dataView: self }, null, self);
            }
            function setPagingOptions(args) {
                var anyChange = false;
                if (args.rowsPerPage != undefined &&
                    intf.rowsPerPage != args.rowsPerPage) {
                    intf.rowsPerPage = args.rowsPerPage;
                    anyChange = true;
                }
                if (args.page != undefined) {
                    var newPage;
                    if (!intf.rowsPerPage)
                        newPage = 1;
                    else if (totalCount == null)
                        newPage = args.page;
                    else
                        newPage = Math.min(args.page, Math.ceil(totalCount / intf.rowsPerPage) + 1);
                    if (newPage < 1)
                        newPage = 1;
                    if (newPage != page) {
                        intf.seekToPage = newPage;
                        anyChange = true;
                    }
                }
                if (anyChange)
                    populate();
            }
            function getPagingInfo() {
                return {
                    rowsPerPage: intf.rowsPerPage,
                    page: page,
                    totalCount: totalCount,
                    loading: loading,
                    error: errorMessage,
                    dataView: self
                };
            }
            function sort(comparer, ascending) {
                sortAsc = ascending;
                sortComparer = comparer;
                fastSortField = null;
                if (ascending === false) {
                    items.reverse();
                }
                items.sort(comparer);
                if (ascending === false) {
                    items.reverse();
                }
                idxById = {};
                updateIdxById();
                refresh();
            }
            /***
             * Provides a workaround for the extremely slow sorting in IE.
             * Does a [lexicographic] sort on a give column by temporarily overriding Object.prototype.toString
             * to return the value of that field and then doing a native Array.sort().
             */
            function fastSort(field, ascending) {
                sortAsc = ascending;
                fastSortField = field;
                sortComparer = null;
                var oldToString = Object.prototype.toString;
                Object.prototype.toString = (typeof field === "function") ? field : function () {
                    return this[field];
                };
                // an extra reversal for descending sort keeps the sort stable
                // (assuming a stable native sort implementation, which isn't true in some cases)
                if (ascending === false) {
                    items.reverse();
                }
                items.sort();
                Object.prototype.toString = oldToString;
                if (ascending === false) {
                    items.reverse();
                }
                idxById = {};
                updateIdxById();
                refresh();
            }
            function reSort() {
                if (sortComparer) {
                    sort(sortComparer, sortAsc);
                }
                else if (fastSortField) {
                    fastSort(fastSortField, sortAsc);
                }
            }
            function setFilter(filterFn) {
                filter = filterFn;
                if (options.inlineFilters) {
                    compiledFilter = compileFilter();
                    compiledFilterWithCaching = compileFilterWithCaching();
                }
                refresh();
            }
            function getGrouping() {
                return groupingInfos;
            }
            function setSummaryOptions(summary) {
                summary = summary || {};
                summaryOptions.aggregators = summary.aggregators || [];
                summaryOptions.compiledAccumulators = [];
                summaryOptions.totals = {};
                var idx = summaryOptions.aggregators.length;
                while (idx--) {
                    summaryOptions.compiledAccumulators[idx] = compileAccumulatorLoop(summaryOptions.aggregators[idx]);
                }
                setGrouping(groupingInfos || []);
            }
            function getGrandTotals() {
                summaryOptions.totals = summaryOptions.totals || {};
                if (!summaryOptions.totals.initialized) {
                    summaryOptions.aggregators = summaryOptions.aggregators || [];
                    summaryOptions.compiledAccumulators = summaryOptions.compiledAccumulators || [];
                    var agg, idx = summaryOptions.aggregators.length;
                    while (idx--) {
                        agg = summaryOptions.aggregators[idx];
                        agg.init();
                        summaryOptions.compiledAccumulators[idx].call(agg, items);
                        agg.storeResult(summaryOptions.totals);
                    }
                    summaryOptions.totals.initialized = true;
                }
                return summaryOptions.totals;
            }
            function setGrouping(groupingInfo) {
                if (!options.groupItemMetadataProvider) {
                    options.groupItemMetadataProvider = new Slick.Data.GroupItemMetadataProvider();
                }
                groups = [];
                toggledGroupsByLevel = [];
                groupingInfo = groupingInfo || [];
                groupingInfos = (groupingInfo instanceof Array) ? groupingInfo : [groupingInfo];
                for (var i = 0; i < groupingInfos.length; i++) {
                    var gi = groupingInfos[i] = $.extend(true, {}, groupingInfoDefaults, groupingInfos[i]);
                    gi.aggregators = gi.aggregators || summaryOptions.aggregators || [];
                    gi.getterIsAFn = typeof gi.getter === "function";
                    // pre-compile accumulator loops
                    gi.compiledAccumulators = [];
                    var idx = gi.aggregators.length;
                    while (idx--) {
                        gi.compiledAccumulators[idx] = compileAccumulatorLoop(gi.aggregators[idx]);
                    }
                    toggledGroupsByLevel[i] = {};
                }
                refresh();
            }
            function getItemByIdx(i) {
                return items[i];
            }
            function getIdxById(id) {
                return idxById[id];
            }
            function ensureRowsByIdCache() {
                if (!rowsById) {
                    rowsById = {};
                    for (var i = 0, l = rows.length; i < l; i++) {
                        rowsById[rows[i][idProperty]] = i;
                    }
                }
            }
            function getRowById(id) {
                ensureRowsByIdCache();
                return rowsById[id];
            }
            function getItemById(id) {
                return items[idxById[id]];
            }
            function mapIdsToRows(idArray) {
                var rows = [];
                ensureRowsByIdCache();
                for (var i = 0, l = idArray.length; i < l; i++) {
                    var row = rowsById[idArray[i]];
                    if (row != null) {
                        rows[rows.length] = row;
                    }
                }
                return rows;
            }
            function mapRowsToIds(rowArray) {
                var ids = [];
                for (var i = 0, l = rowArray.length; i < l; i++) {
                    if (rowArray[i] < rows.length) {
                        ids[ids.length] = rows[rowArray[i]][idProperty];
                    }
                }
                return ids;
            }
            function updateItem(id, item) {
                if (idxById[id] === undefined || id !== item[idProperty]) {
                    throw "Invalid or non-matching id";
                }
                items[idxById[id]] = item;
                if (!updated) {
                    updated = {};
                }
                updated[id] = true;
                refresh();
            }
            function insertItem(insertBefore, item) {
                items.splice(insertBefore, 0, item);
                updateIdxById(insertBefore);
                refresh();
            }
            function addItem(item) {
                items.push(item);
                updateIdxById(items.length - 1);
                refresh();
            }
            function deleteItem(id) {
                var idx = idxById[id];
                if (idx === undefined) {
                    throw "Invalid id";
                }
                delete idxById[id];
                items.splice(idx, 1);
                updateIdxById(idx);
                refresh();
            }
            function getRows() {
                return rows;
            }
            function getLength() {
                return rows.length;
            }
            function getItem(i) {
                var item = rows[i];
                // if this is a group row, make sure totals are calculated and update the title
                if (item && item.__group && item.totals && !item.totals.initialized) {
                    var gi = groupingInfos[item.level];
                    if (!gi.displayTotalsRow) {
                        calculateTotals(item.totals);
                        item.title = gi.formatter ? gi.formatter(item) : item.value;
                    }
                }
                else if (item && item.__groupTotals && !item.initialized) {
                    calculateTotals(item);
                }
                return item;
            }
            function getItemMetadata(i) {
                var item = rows[i];
                if (item === undefined) {
                    return null;
                }
                // overrides for grouping rows
                if (item.__group) {
                    return options.groupItemMetadataProvider.getGroupRowMetadata(item);
                }
                // overrides for totals rows
                if (item.__groupTotals) {
                    return options.groupItemMetadataProvider.getTotalsRowMetadata(item);
                }
                return (options.getItemMetadata && options.getItemMetadata(item, i)) || null;
            }
            function expandCollapseAllGroups(level, collapse) {
                if (level == null) {
                    for (var i = 0; i < groupingInfos.length; i++) {
                        toggledGroupsByLevel[i] = {};
                        groupingInfos[i].collapsed = collapse;
                    }
                }
                else {
                    toggledGroupsByLevel[level] = {};
                    groupingInfos[level].collapsed = collapse;
                }
                refresh();
            }
            /**
             * @param level {Number} Optional level to collapse.  If not specified, applies to all levels.
             */
            function collapseAllGroups(level) {
                expandCollapseAllGroups(level, true);
            }
            /**
             * @param level {Number} Optional level to expand.  If not specified, applies to all levels.
             */
            function expandAllGroups(level) {
                expandCollapseAllGroups(level, false);
            }
            function resolveLevelAndGroupingKey(args) {
                var arg0 = args[0];
                if (args.length === 1 && arg0.indexOf(groupingDelimiter) !== -1) {
                    return { level: arg0.split(groupingDelimiter).length - 1, groupingKey: arg0 };
                }
                else {
                    return { level: args.length - 1, groupingKey: args.join(groupingDelimiter) };
                }
            }
            function expandCollapseGroup(args, collapse) {
                var opts = resolveLevelAndGroupingKey(args);
                toggledGroupsByLevel[opts.level][opts.groupingKey] = groupingInfos[opts.level].collapsed ^ collapse;
                refresh();
            }
            /**
             * @param varArgs Either a Slick.Group's "groupingKey" property, or a
             *     variable argument list of grouping values denoting a unique path to the row.  For
             *     example, calling collapseGroup('high', '10%') will collapse the '10%' subgroup of
             *     the 'high' group.
             */
            function collapseGroup(varArgs) {
                var args = Array.prototype.slice.call(arguments);
                expandCollapseGroup(args, true);
            }
            /**
             * @param varArgs Either a Slick.Group's "groupingKey" property, or a
             *     variable argument list of grouping values denoting a unique path to the row.  For
             *     example, calling expandGroup('high', '10%') will expand the '10%' subgroup of
             *     the 'high' group.
             */
            function expandGroup(varArgs) {
                var args = Array.prototype.slice.call(arguments);
                expandCollapseGroup(args, false);
            }
            function getGroups() {
                return groups;
            }
            function getOrCreateGroup(groupsByVal, val, level, parentGroup, groups) {
                var group = groupsByVal[val];
                if (!group) {
                    group = new Slick.Group();
                    group.value = val;
                    group.level = level;
                    group.groupingKey = (parentGroup ? parentGroup.groupingKey + groupingDelimiter : '') + val;
                    groups[groups.length] = group;
                    groupsByVal[val] = group;
                }
                return group;
            }
            function extractGroups(rows, parentGroup) {
                var group;
                var val;
                var groups = [];
                var groupsByVal = {};
                var r;
                var level = parentGroup ? parentGroup.level + 1 : 0;
                var gi = groupingInfos[level];
                for (var i = 0, l = gi.predefinedValues.length; i < l; i++) {
                    val = gi.predefinedValues[i];
                    group = getOrCreateGroup(groupsByVal, val, level, parentGroup, groups);
                }
                for (var i = 0, l = rows.length; i < l; i++) {
                    r = rows[i];
                    val = gi.getterIsAFn ? gi.getter(r) : r[gi.getter];
                    group = getOrCreateGroup(groupsByVal, val, level, parentGroup, groups);
                    group.rows[group.count++] = r;
                }
                if (level < groupingInfos.length - 1) {
                    for (var i = 0; i < groups.length; i++) {
                        group = groups[i];
                        group.groups = extractGroups(group.rows, group);
                    }
                }
                groups.sort(groupingInfos[level].comparer);
                return groups;
            }
            function calculateTotals(totals) {
                var group = totals.group;
                var gi = groupingInfos[group.level];
                var isLeafLevel = (group.level == groupingInfos.length);
                var agg, idx = gi.aggregators.length;
                if (!isLeafLevel && gi.aggregateChildGroups) {
                    // make sure all the subgroups are calculated
                    var i = group.groups.length;
                    while (i--) {
                        if (!group.groups[i].totals.initialized) {
                            calculateTotals(group.groups[i].totals);
                        }
                    }
                }
                while (idx--) {
                    agg = gi.aggregators[idx];
                    agg.init();
                    if (!isLeafLevel && gi.aggregateChildGroups) {
                        gi.compiledAccumulators[idx].call(agg, group.groups);
                    }
                    else {
                        gi.compiledAccumulators[idx].call(agg, group.rows);
                    }
                    agg.storeResult(totals);
                }
                totals.initialized = true;
            }
            function addGroupTotals(group) {
                var gi = groupingInfos[group.level];
                var totals = new Slick.GroupTotals();
                totals.group = group;
                group.totals = totals;
                if (!gi.lazyTotalsCalculation) {
                    calculateTotals(totals);
                }
            }
            function addTotals(groups, level) {
                level = level || 0;
                var gi = groupingInfos[level];
                var groupCollapsed = gi.collapsed;
                var toggledGroups = toggledGroupsByLevel[level];
                var idx = groups.length, g;
                while (idx--) {
                    g = groups[idx];
                    if (g.collapsed && !gi.aggregateCollapsed) {
                        continue;
                    }
                    // Do a depth-first aggregation so that parent group aggregators can access subgroup totals.
                    if (g.groups) {
                        addTotals(g.groups, level + 1);
                    }
                    if (gi.aggregators.length && (gi.aggregateEmpty || g.rows.length || (g.groups && g.groups.length))) {
                        addGroupTotals(g);
                    }
                    g.collapsed = groupCollapsed ^ toggledGroups[g.groupingKey];
                    g.title = gi.formatter ? gi.formatter(g) : g.value;
                }
            }
            function flattenGroupedRows(groups, level) {
                level = level || 0;
                var gi = groupingInfos[level];
                var groupedRows = [], rows, gl = 0, g;
                for (var i = 0, l = groups.length; i < l; i++) {
                    g = groups[i];
                    groupedRows[gl++] = g;
                    if (!g.collapsed) {
                        rows = g.groups ? flattenGroupedRows(g.groups, level + 1) : g.rows;
                        for (var j = 0, jj = rows.length; j < jj; j++) {
                            groupedRows[gl++] = rows[j];
                        }
                    }
                    if (g.totals && gi.displayTotalsRow && (!g.collapsed || gi.aggregateCollapsed)) {
                        groupedRows[gl++] = g.totals;
                    }
                }
                return groupedRows;
            }
            function getFunctionInfo(fn) {
                var fnRegex = /^function[^(]*\(([^)]*)\)\s*{([\s\S]*)}$/;
                var matches = fn.toString().match(fnRegex);
                return {
                    params: matches[1].split(","),
                    body: matches[2]
                };
            }
            function compileAccumulatorLoop(aggregator) {
                var accumulatorInfo = getFunctionInfo(aggregator.accumulate);
                var fn = new Function("_items", "for (var " + accumulatorInfo.params[0] + ", _i=0, _il=_items.length; _i<_il; _i++) {" +
                    accumulatorInfo.params[0] + " = _items[_i]; " +
                    accumulatorInfo.body +
                    "}");
                fn.displayName = fn.name = "compiledAccumulatorLoop";
                return fn;
            }
            function compileFilter() {
                var filterInfo = getFunctionInfo(filter);
                var filterBody = filterInfo.body
                    .replace(/return false\s*([;}]|$)/gi, "{ continue _coreloop; }$1")
                    .replace(/return true\s*([;}]|$)/gi, "{ _retval[_idx++] = $item$; continue _coreloop; }$1")
                    .replace(/return ([^;}]+?)\s*([;}]|$)/gi, "{ if ($1) { _retval[_idx++] = $item$; }; continue _coreloop; }$2");
                // This preserves the function template code after JS compression,
                // so that replace() commands still work as expected.
                var tpl = [
                    //"function(_items, _args) { ",
                    "var _retval = [], _idx = 0; ",
                    "var $item$, $args$ = _args; ",
                    "_coreloop: ",
                    "for (var _i = 0, _il = _items.length; _i < _il; _i++) { ",
                    "$item$ = _items[_i]; ",
                    "$filter$; ",
                    "} ",
                    "return _retval; "
                ].join("");
                tpl = tpl.replace(/\$filter\$/gi, filterBody);
                tpl = tpl.replace(/\$item\$/gi, filterInfo.params[0]);
                tpl = tpl.replace(/\$args\$/gi, filterInfo.params[1]);
                var fn = new Function("_items,_args", tpl);
                fn.displayName = fn.name = "compiledFilter";
                return fn;
            }
            function compileFilterWithCaching() {
                var filterInfo = getFunctionInfo(filter);
                var filterBody = filterInfo.body
                    .replace(/return false\s*([;}]|$)/gi, "{ continue _coreloop; }$1")
                    .replace(/return true\s*([;}]|$)/gi, "{ _cache[_i] = true;_retval[_idx++] = $item$; continue _coreloop; }$1")
                    .replace(/return ([^;}]+?)\s*([;}]|$)/gi, "{ if ((_cache[_i] = $1)) { _retval[_idx++] = $item$; }; continue _coreloop; }$2");
                // This preserves the function template code after JS compression,
                // so that replace() commands still work as expected.
                var tpl = [
                    //"function(_items, _args, _cache) { ",
                    "var _retval = [], _idx = 0; ",
                    "var $item$, $args$ = _args; ",
                    "_coreloop: ",
                    "for (var _i = 0, _il = _items.length; _i < _il; _i++) { ",
                    "$item$ = _items[_i]; ",
                    "if (_cache[_i]) { ",
                    "_retval[_idx++] = $item$; ",
                    "continue _coreloop; ",
                    "} ",
                    "$filter$; ",
                    "} ",
                    "return _retval; "
                ].join("");
                tpl = tpl.replace(/\$filter\$/gi, filterBody);
                tpl = tpl.replace(/\$item\$/gi, filterInfo.params[0]);
                tpl = tpl.replace(/\$args\$/gi, filterInfo.params[1]);
                var fn = new Function("_items,_args,_cache", tpl);
                fn.displayName = fn.name = "compiledFilterWithCaching";
                return fn;
            }
            function uncompiledFilter(items, args) {
                var retval = [], idx = 0;
                for (var i = 0, ii = items.length; i < ii; i++) {
                    if (filter(items[i], args)) {
                        retval[idx++] = items[i];
                    }
                }
                return retval;
            }
            function uncompiledFilterWithCaching(items, args, cache) {
                var retval = [], idx = 0, item;
                for (var i = 0, ii = items.length; i < ii; i++) {
                    item = items[i];
                    if (cache[i]) {
                        retval[idx++] = item;
                    }
                    else if (filter(item, args)) {
                        retval[idx++] = item;
                        cache[i] = true;
                    }
                }
                return retval;
            }
            function getFilteredAndPagedItems(items) {
                if (filter) {
                    var batchFilter = options.inlineFilters ? compiledFilter : uncompiledFilter;
                    var batchFilterWithCaching = options.inlineFilters ? compiledFilterWithCaching : uncompiledFilterWithCaching;
                    if (refreshHints.isFilterNarrowing) {
                        filteredItems = batchFilter(filteredItems, filterArgs);
                    }
                    else if (refreshHints.isFilterExpanding) {
                        filteredItems = batchFilterWithCaching(items, filterArgs, filterCache);
                    }
                    else if (!refreshHints.isFilterUnchanged) {
                        filteredItems = batchFilter(items, filterArgs);
                    }
                }
                else {
                    // special case:  if not filtering and not paging, the resulting
                    // rows collection needs to be a copy so that changes due to sort
                    // can be caught
                    filteredItems = items.concat();
                }
                // get the current page
                return { totalRows: filteredItems.length, rows: filteredItems };
            }
            function getRowDiffs(rows, newRows) {
                var item, r, eitherIsNonData, diff = [];
                var from = 0, to = newRows.length;
                if (refreshHints && refreshHints.ignoreDiffsBefore) {
                    from = Math.max(0, Math.min(newRows.length, refreshHints.ignoreDiffsBefore));
                }
                if (refreshHints && refreshHints.ignoreDiffsAfter) {
                    to = Math.min(newRows.length, Math.max(0, refreshHints.ignoreDiffsAfter));
                }
                for (var i = from, rl = rows.length; i < to; i++) {
                    if (i >= rl) {
                        diff[diff.length] = i;
                    }
                    else {
                        item = newRows[i];
                        r = rows[i];
                        if ((groupingInfos.length && (eitherIsNonData = (item.__nonDataRow) || (r.__nonDataRow)) &&
                            item.__group !== r.__group ||
                            item.__group && !item.equals(r))
                            || (eitherIsNonData &&
                                // no good way to compare totals since they are arbitrary DTOs
                                // deep object comparison is pretty expensive
                                // always considering them 'dirty' seems easier for the time being
                                (item.__groupTotals || r.__groupTotals))
                            || item[idProperty] != r[idProperty]
                            || (updated && updated[item[idProperty]])) {
                            diff[diff.length] = i;
                        }
                    }
                }
                return diff;
            }
            function recalc(_items) {
                rowsById = null;
                if (refreshHints.isFilterNarrowing != prevRefreshHints.isFilterNarrowing ||
                    refreshHints.isFilterExpanding != prevRefreshHints.isFilterExpanding) {
                    filterCache = [];
                }
                var filteredItems = getFilteredAndPagedItems(_items);
                totalRows = filteredItems.totalRows;
                var newRows = filteredItems.rows;
                summaryOptions.totals = {};
                groups = [];
                if (groupingInfos.length) {
                    groups = extractGroups(newRows);
                    if (groups.length) {
                        addTotals(groups);
                        newRows = flattenGroupedRows(groups);
                    }
                }
                var diff = getRowDiffs(rows, newRows);
                rows = newRows;
                return diff;
            }
            function refresh() {
                if (suspend) {
                    return;
                }
                var countBefore = rows.length;
                var totalRowsBefore = totalRows;
                var diff = recalc(items); // pass as direct refs to avoid closure perf hit
                updated = null;
                prevRefreshHints = refreshHints;
                refreshHints = {};
                if (totalRowsBefore !== totalRows) {
                    onPagingInfoChanged.notify(getPagingInfo(), null, self);
                }
                if (countBefore !== rows.length) {
                    onRowCountChanged.notify({ previous: countBefore, current: rows.length, dataView: self }, null, self);
                }
                if (diff.length > 0) {
                    onRowsChanged.notify({ rows: diff, dataView: self }, null, self);
                }
            }
            /***
             * Wires the grid and the DataView together to keep row selection tied to item ids.
             * This is useful since, without it, the grid only knows about rows, so if the items
             * move around, the same rows stay selected instead of the selection moving along
             * with the items.
             *
             * NOTE:  This doesn't work with cell selection model.
             *
             * @param grid {Slick.Grid} The grid to sync selection with.
             * @param preserveHidden {Boolean} Whether to keep selected items that go out of the
             *     view due to them getting filtered out.
             * @param preserveHiddenOnSelectionChange {Boolean} Whether to keep selected items
             *     that are currently out of the view (see preserveHidden) as selected when selection
             *     changes.
             * @return {Slick.Event} An event that notifies when an internal list of selected row ids
             *     changes.  This is useful since, in combination with the above two options, it allows
             *     access to the full list selected row ids, and not just the ones visible to the grid.
             * @method syncGridSelection
             */
            function syncGridSelection(grid, preserveHidden, preserveHiddenOnSelectionChange) {
                var self = this;
                var inHandler;
                var selectedRowIds = self.mapRowsToIds(grid.getSelectedRows());
                var onSelectedRowIdsChanged = new Slick.Event();
                function setSelectedRowIds(rowIds) {
                    if (selectedRowIds.join(",") == rowIds.join(",")) {
                        return;
                    }
                    selectedRowIds = rowIds;
                    onSelectedRowIdsChanged.notify({
                        "grid": grid,
                        "ids": selectedRowIds,
                        "dataView": self
                    }, new Slick.EventData(), self);
                }
                function update() {
                    if (selectedRowIds.length > 0) {
                        inHandler = true;
                        var selectedRows = self.mapIdsToRows(selectedRowIds);
                        if (!preserveHidden) {
                            setSelectedRowIds(self.mapRowsToIds(selectedRows));
                        }
                        grid.setSelectedRows(selectedRows);
                        inHandler = false;
                    }
                }
                grid.onSelectedRowsChanged.subscribe(function (e, args) {
                    if (inHandler) {
                        return;
                    }
                    var newSelectedRowIds = self.mapRowsToIds(grid.getSelectedRows());
                    if (!preserveHiddenOnSelectionChange || !grid.getOptions().multiSelect) {
                        setSelectedRowIds(newSelectedRowIds);
                    }
                    else {
                        // keep the ones that are hidden
                        var existing = $.grep(selectedRowIds, function (id) { return self.getRowById(id) === undefined; });
                        // add the newly selected ones
                        setSelectedRowIds(existing.concat(newSelectedRowIds));
                    }
                });
                this.onRowsChanged.subscribe(update);
                this.onRowCountChanged.subscribe(update);
                return onSelectedRowIdsChanged;
            }
            function syncGridCellCssStyles(grid, key) {
                var hashById;
                var inHandler;
                // since this method can be called after the cell styles have been set,
                // get the existing ones right away
                storeCellCssStyles(grid.getCellCssStyles(key));
                function storeCellCssStyles(hash) {
                    hashById = {};
                    for (var row in hash) {
                        var id = rows[row][idProperty];
                        hashById[id] = hash[row];
                    }
                }
                function update() {
                    if (hashById) {
                        inHandler = true;
                        ensureRowsByIdCache();
                        var newHash = {};
                        for (var id in hashById) {
                            var row = rowsById[id];
                            if (row != undefined) {
                                newHash[row] = hashById[id];
                            }
                        }
                        grid.setCellCssStyles(key, newHash);
                        inHandler = false;
                    }
                }
                grid.onCellCssStylesChanged.subscribe(function (e, args) {
                    if (inHandler) {
                        return;
                    }
                    if (key != args.key) {
                        return;
                    }
                    if (args.hash) {
                        storeCellCssStyles(args.hash);
                    }
                });
                this.onRowsChanged.subscribe(update);
                this.onRowCountChanged.subscribe(update);
            }
            function addData(data) {
                if (intf.onProcessData && data)
                    data = intf.onProcessData(data, intf) || data;
                errorMessage = null;
                loading && loading.abort();
                loading = false;
                if (!data) {
                    errorMessage = intf.errormsg;
                    onPagingInfoChanged.notify(getPagingInfo());
                    return false;
                }
                var theData = data;
                data.TotalCount = data.TotalCount || 0;
                data.Entities = data.Entities || [];
                if (!data.Skip || (!intf.rowsPerPage && !data.Take))
                    data.Page = 1;
                else
                    data.Page = Math.ceil(data.Skip / (data.Take || intf.rowsPerPage)) + 1;
                page = data.Page;
                totalCount = data.TotalCount;
                setItems(data.Entities);
                onPagingInfoChanged.notify(getPagingInfo());
            }
            function populate() {
                if (populateLocks > 0) {
                    populateCalls++;
                    return;
                }
                populateCalls = 0;
                loading && loading.abort();
                if (intf.onSubmit) {
                    var gh = intf.onSubmit(intf);
                    if (gh === false)
                        return false;
                }
                onDataLoading.notify(this);
                if (!intf.url)
                    return false;
                // set loading event
                if (!intf.seekToPage)
                    intf.seekToPage = 1;
                var request = {};
                var skip = (intf.seekToPage - 1) * intf.rowsPerPage;
                if (skip)
                    request.Skip = skip;
                if (intf.rowsPerPage)
                    request.Take = intf.rowsPerPage;
                if (intf.sortBy && intf.sortBy.length) {
                    if ($.isArray(intf.sortBy))
                        request.Sort = intf.sortBy;
                    else {
                        request.Sort = [intf.sortBy];
                    }
                }
                if (intf.params) {
                    request = $.extend(request, intf.params);
                }
                var dt = dataType;
                var self = this;
                var ajaxOptions = {
                    cache: false,
                    type: intf.method,
                    contentType: contentType,
                    url: intf.url,
                    data: request,
                    dataType: dt,
                    success: function (response) {
                        loading = false;
                        if (response.Error)
                            Q.notifyError(response.Error.Message || response.Error.Code);
                        else
                            addData(response);
                        onDataLoaded.notify(this);
                    },
                    error: function (xhr, status, ev) {
                        loading = false;
                        if ((xhr.getResponseHeader("content-type") || '').toLowerCase().indexOf("application/json") >= 0) {
                            var json = $.parseJSON(xhr.responseText);
                            if (json != null && json.Error != null) {
                                Q.notifyError(json.Error.Message || json.Error.Code);
                                onPagingInfoChanged.notify(getPagingInfo());
                                onDataLoaded.notify(this);
                                return;
                            }
                        }
                        errorMessage = xhr.errormsg;
                        onPagingInfoChanged.notify(getPagingInfo());
                        onDataLoaded.notify(this);
                    },
                    complete: function () {
                        loading = false;
                    }
                };
                if (intf.onAjaxCall) {
                    var ah = intf.onAjaxCall(this, ajaxOptions);
                    if (ah === false) {
                        loading = false;
                        onPagingInfoChanged.notify(getPagingInfo());
                        return false;
                    }
                }
                ajaxOptions.data = $.toJSON(ajaxOptions.data);
                onPagingInfoChanged.notify(getPagingInfo());
                loading = $.ajax(ajaxOptions);
            }
            function populateLock() {
                if (populateLocks == 0)
                    populateCalls = 0;
                populateLocks++;
            }
            function populateUnlock() {
                if (populateLocks > 0) {
                    populateLocks--;
                    if (populateLocks == 0 && populateCalls > 0)
                        populate();
                }
            }
            intf = {
                // methods
                "beginUpdate": beginUpdate,
                "endUpdate": endUpdate,
                "setPagingOptions": setPagingOptions,
                "getPagingInfo": getPagingInfo,
                "getRows": getRows,
                "getItems": getItems,
                "setItems": setItems,
                "setFilter": setFilter,
                "sort": sort,
                "fastSort": fastSort,
                "reSort": reSort,
                "setSummaryOptions": setSummaryOptions,
                "getGrandTotals": getGrandTotals,
                "setGrouping": setGrouping,
                "getGrouping": getGrouping,
                "collapseAllGroups": collapseAllGroups,
                "expandAllGroups": expandAllGroups,
                "collapseGroup": collapseGroup,
                "expandGroup": expandGroup,
                "getGroups": getGroups,
                "getIdxById": getIdxById,
                "getRowById": getRowById,
                "getItemById": getItemById,
                "getItemByIdx": getItemByIdx,
                "mapRowsToIds": mapRowsToIds,
                "mapIdsToRows": mapIdsToRows,
                "setRefreshHints": setRefreshHints,
                "setFilterArgs": setFilterArgs,
                "refresh": refresh,
                "updateItem": updateItem,
                "insertItem": insertItem,
                "addItem": addItem,
                "deleteItem": deleteItem,
                "syncGridSelection": syncGridSelection,
                "syncGridCellCssStyles": syncGridCellCssStyles,
                "getLength": getLength,
                "getItem": getItem,
                "getItemMetadata": getItemMetadata,
                "onRowCountChanged": onRowCountChanged,
                "onRowsChanged": onRowsChanged,
                "onPagingInfoChanged": onPagingInfoChanged,
                "addData": addData,
                "populate": populate,
                "populateLock": populateLock,
                "populateUnlock": populateUnlock,
                "onDataChanged": onDataChanged,
                "onDataLoaded": onDataLoaded,
                "onDataLoading": onDataLoading
            };
            idProperty = options.idField || 'id';
            contentType = options.contentType || "application/json";
            dataType = options.dataType || 'json';
            filter = options.filter || null;
            intf.params = options.params || {};
            intf.onSubmit = options.onSubmit || null;
            intf.url = options.url || null;
            intf.rowsPerPage = options.rowsPerPage || 0;
            intf.seekToPage = options.seekToPage || 1;
            intf.onAjaxCall = options.onAjaxCall || null;
            intf.onProcessData = options.onProcessData || null;
            intf.method = options.method || 'POST';
            intf.errormsg = intf.errormsg || Q.text("Controls.Pager.DefaultLoadError");
            intf.sortBy = options.sortBy || [];
            intf.idField = idProperty;
            if (options.url && options.autoLoad) {
                populate();
            }
            return intf;
        }
        return RemoteView;
    }());
    Slick.RemoteView = RemoteView;
})(Slick || (Slick = {}));
var Slick;
(function (Slick) {
    var Aggregators;
    (function (Aggregators) {
        function Avg(field) {
            this.field_ = field;
            this.init = function () {
                this.count_ = 0;
                this.nonNullCount_ = 0;
                this.sum_ = 0;
            };
            this.accumulate = function (item) {
                var val = item[this.field_];
                this.count_++;
                if (val != null && val !== "" && !isNaN(val)) {
                    this.nonNullCount_++;
                    this.sum_ += parseFloat(val);
                }
            };
            this.storeResult = function (groupTotals) {
                if (!groupTotals.avg) {
                    groupTotals.avg = {};
                }
                if (this.nonNullCount_ != 0) {
                    groupTotals.avg[this.field_] = this.sum_ / this.nonNullCount_;
                }
            };
        }
        Aggregators.Avg = Avg;
        function WeightedAvg(field, weightedField) {
            this.field_ = field;
            this.weightedField_ = weightedField;
            this.init = function () {
                this.sum_ = 0;
                this.weightedSum_ = 0;
            };
            this.accumulate = function (item) {
                var val = item[this.field_];
                var valWeighted = item[this.weightedField_];
                if (this.isValid(val) && this.isValid(valWeighted)) {
                    this.weightedSum_ += parseFloat(valWeighted);
                    this.sum_ += parseFloat(val) * parseFloat(valWeighted);
                }
            };
            this.storeResult = function (groupTotals) {
                if (!groupTotals.avg) {
                    groupTotals.avg = {};
                }
                if (this.sum_ && this.weightedSum_) {
                    groupTotals.avg[this.field_] = this.sum_ / this.weightedSum_;
                }
            };
            this.isValid = function (val) {
                return val !== null && val !== "" && !isNaN(val);
            };
        }
        Aggregators.WeightedAvg = WeightedAvg;
        function Min(field) {
            this.field_ = field;
            this.init = function () {
                this.min_ = null;
            };
            this.accumulate = function (item) {
                var val = item[this.field_];
                if (val != null && val !== "" && !isNaN(val)) {
                    if (this.min_ == null || val < this.min_) {
                        this.min_ = val;
                    }
                }
            };
            this.storeResult = function (groupTotals) {
                if (!groupTotals.min) {
                    groupTotals.min = {};
                }
                groupTotals.min[this.field_] = this.min_;
            };
        }
        Aggregators.Min = Min;
        function Max(field) {
            this.field_ = field;
            this.init = function () {
                this.max_ = null;
            };
            this.accumulate = function (item) {
                var val = item[this.field_];
                if (val != null && val !== "" && !isNaN(val)) {
                    if (this.max_ == null || val > this.max_) {
                        this.max_ = val;
                    }
                }
            };
            this.storeResult = function (groupTotals) {
                if (!groupTotals.max) {
                    groupTotals.max = {};
                }
                groupTotals.max[this.field_] = this.max_;
            };
        }
        Aggregators.Max = Max;
        function Sum(field) {
            this.field_ = field;
            this.init = function () {
                this.sum_ = null;
            };
            this.accumulate = function (item) {
                var val = item[this.field_];
                if (val != null && val !== "" && !isNaN(val)) {
                    this.sum_ += parseFloat(val);
                }
            };
            this.storeResult = function (groupTotals) {
                if (!groupTotals.sum) {
                    groupTotals.sum = {};
                }
                groupTotals.sum[this.field_] = this.sum_;
            };
        }
        Aggregators.Sum = Sum;
    })(Aggregators = Slick.Aggregators || (Slick.Aggregators = {}));
})(Slick || (Slick = {}));
/// <reference path = "Q/Q.Config.ts" />
/// <reference path = "Q/Q.Culture.ts" />
/// <reference path = "Q/Q.LocalText.ts" />
/// <reference path = "Q/Q.Notify.ts" />
/// <reference path = "Q/Q.Url.ts" />
var Q;
(function (Q) {
    var oldShowLabel;
    function validateShowLabel(element, message) {
        oldShowLabel.call(this, element, message);
        this.errorsFor(element).each(function (i, e) {
            if ($(element).hasClass('error'))
                $(e).removeClass('checked');
            $(e).attr('title', $(e).text());
        });
    }
    ;
    function registerCustomValidationMethods() {
        if ($.validator.methods['customValidate'] == null) {
            $.validator.addMethod('customValidate', function (value, element) {
                var result = this.optional(element);
                if (element == null || !!result) {
                    return result;
                }
                var events = $._data(element, 'events');
                if (!events) {
                    return true;
                }
                var handlers = events.customValidate;
                if (handlers == null || handlers.length === 0) {
                    return true;
                }
                var el = $(element);
                for (var i = 0; !!(i < handlers.length); i++) {
                    var handler = ss.safeCast(handlers[i].handler, Function);
                    if (handler) {
                        var message = handler(el);
                        if (message != null) {
                            el.data('customValidationMessage', message);
                            return false;
                        }
                    }
                }
                return true;
            }, function (o, e) {
                return $(e).data('customValidationMessage');
            });
        }
    }
    function jQueryValidationInitialization() {
        registerCustomValidationMethods();
        var p = $.validator;
        p = p.prototype;
        oldShowLabel = p.showLabel;
        p.showLabel = validateShowLabel;
        $.validator.addMethod("dateQ", function (value, element) {
            return this.optional(element) || Q.parseDate(value) != false;
        });
        $.validator.addMethod("hourAndMin", function (value, element) {
            return this.optional(element) || !isNaN(Q.parseHourAndMin(value));
        });
        $.validator.addMethod("dayHourAndMin", function (value, element) {
            return this.optional(element) || !isNaN(Q.parseDayHourAndMin(value));
        });
        $.validator.addMethod("decimalQ", function (value, element) {
            return this.optional(element) || !isNaN(Q.parseDecimal(value));
        });
        $.validator.addMethod("integerQ", function (value, element) {
            return this.optional(element) || !isNaN(Q.parseInteger(value));
        });
        var oldEmail = $.validator.methods['email'];
        $.validator.addMethod("email", function (value, element) {
            if (!Q.Config.emailAllowOnlyAscii)
                return oldEmail.call(this, value, element);
            return this.optional(element) || /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/.test(value);
        });
        $.validator.addMethod("emailMultiple", function (value, element) {
            var result = this.optional(element);
            if (result)
                return result;
            if (value.indexOf(';') >= 0)
                value = value.split(';');
            else
                value = value.split(',');
            for (var i = 0; i < value.length; i++) {
                result = $.validator.methods['email'].call(this, value[i], element);
                if (!result)
                    return result;
            }
            return result;
        });
        $.validator.addMethod("anyvalue", function (value, element) {
            return true;
        });
        var d = $.validator.defaults;
        d.ignoreTitle = true;
        d.onchange = function (element) {
            this.element(element);
        };
        p.oldinit = p.init;
        p.init = function () {
            p.oldinit.call(this);
            function changeDelegate(event) {
                if (this.form == null)
                    return;
                var validator = $.data(this.form, "validator"), eventType = "on" + event.type.replace(/^validate/, "");
                validator && validator.settings[eventType] && validator.settings[eventType].call(validator, this);
            }
            function delegate(event) {
                var el = this[0];
                if (!$.data(el, 'changebound')) {
                    $(el).change(changeDelegate);
                    $.data(el, 'changebound', true);
                }
            }
            $(this.currentForm)
                .on(":text, :password, :file, select, textarea", "focusin.validate", delegate);
        };
        p.oldfocusInvalid = p.focusInvalid;
        p.focusInvalid = function () {
            if (this.settings.abortHandler)
                this.settings.abortHandler(this);
            this.oldfocusInvalid.call(this);
        };
        p.oldstopRequest = p.focusInvalid;
        p.stopRequest = function (element, valid) {
            var formSubmitted = this.formSubmitted;
            this.oldfocusInvalid.call(this, [element, valid]);
            if (!valid && this.pendingRequest == 0 && formSubmitted && this.settings.abortHandler) {
                this.settings.abortHandler(this);
            }
        };
        p.resetAll = function () {
            this.submitted = {};
            this.prepareForm();
            this.hideErrors();
            this.elements().removeClass(this.settings.errorClass);
        };
        jQuery(function () {
            $.extend($.validator.messages, {
                email: Q.text("Validation.Email"),
                required: Q.text("Validation.Required"),
                minlength: Q.text("Validation.MinLength"),
                maxlength: Q.text("Validation.MaxLength"),
                digits: Q.text("Validation.Digits"),
                range: Q.text("Validation.Range"),
                xss: Q.text("Validation.Xss"),
                dateQ: Q.text("Validation.DateInvalid"),
                decimalQ: Q.text("Validation.Decimal"),
                integerQ: Q.text("Validation.Integer"),
                url: Q.text("Validation.Url")
            });
        });
    }
    ;
    function validatorAbortHandler(validator) {
        validator.settings.abortHandler = null;
        validator.settings.submitHandler = function () {
            return false;
        };
    }
    Q.validatorAbortHandler = validatorAbortHandler;
    ;
    function validateOptions(options) {
        return $.extend({
            ignore: ":hidden",
            meta: 'v',
            errorClass: 'error',
            errorPlacement: function (error, element) {
                var field = null;
                var vx = element.attr('data-vx-id');
                if (vx) {
                    field = $('#' + vx);
                    if (!field.length)
                        field = null;
                    else
                        field = field[0];
                }
                if (field == null) {
                    field = element.parents('div.field');
                    if (field.length) {
                        var inner = $('div.vx', field[0]);
                        if (inner.length)
                            field = inner[0];
                    }
                    else
                        field = element.parent();
                }
                error.appendTo(field);
            },
            submitHandler: function () {
                return false;
            },
            invalidHandler: function () {
                Q.notifyError(Q.text("Validation.InvalidFormMessage"));
            },
            success: function (label) {
                label.addClass('checked');
            }
        }, options);
    }
    Q.validateOptions = validateOptions;
    ;
    if (window['jQuery'] && window['jQuery']['validator'])
        jQueryValidationInitialization();
    else if (window['jQuery']) {
        jQuery(function ($) {
            if ($.validator)
                jQueryValidationInitialization();
        });
    }
    function jQueryDatepickerInitialization() {
        var order = Q.Culture.dateOrder;
        var s = Q.Culture.dateSeparator;
        var culture = ($('html').attr('lang') || 'en').toLowerCase();
        if (!$.datepicker.regional[culture]) {
            culture = culture.split('-')[0];
            if (!$.datepicker.regional[culture]) {
                culture = 'en';
            }
        }
        $.datepicker.setDefaults($.datepicker.regional['en']);
        $.datepicker.setDefaults($.datepicker.regional[culture]);
        $.datepicker.setDefaults({
            dateFormat: (order == 'mdy' ? 'mm' + s + 'dd' + s + 'yy' :
                (order == 'ymd' ? 'yy' + s + 'mm' + s + 'dd' :
                    'dd' + s + 'mm' + s + 'yy')),
            buttonImage: Q.resolveUrl('~/content/serenity/images/datepicker.png'),
            buttonImageOnly: true,
            showOn: 'both',
            showButtonPanel: true,
            changeMonth: true,
            changeYear: true
        });
    }
    ;
    if (window['jQuery'] &&
        window['jQuery']['datepicker'] &&
        window['jQuery']['datepicker']['regional'] &&
        window['jQuery']['datepicker']['regional']['en']) {
        jQueryDatepickerInitialization();
    }
    else {
        jQuery(function ($) {
            if ($.datepicker)
                jQueryDatepickerInitialization();
        });
    }
    function jQuerySelect2Initialization() {
        $.ui.dialog.prototype._allowInteraction = function (event) {
            if ($(event.target).closest(".ui-dialog").length) {
                return true;
            }
            return !!$(event.target).closest(".ui-datepicker, .select2-drop, .cke, .cke_dialog, #support-modal").length;
        };
    }
    ;
    if (window['jQuery'] && window['jQuery']['ui']) {
        jQuerySelect2Initialization();
    }
    else {
        jQuery(function ($) {
            if (window['jQuery']['ui'])
                jQuerySelect2Initialization();
        });
    }
    function ssExceptionInitialization() {
        ss.Exception.prototype.toString = function () {
            return this.get_message();
        };
    }
    ;
    if (ss && ss.Exception)
        ssExceptionInitialization();
    else {
        jQuery(function ($) {
            if (ss && ss.Exception)
                ssExceptionInitialization();
        });
    }
})(Q || (Q = {}));
//# sourceMappingURL=Serenity.CoreLib.js.map