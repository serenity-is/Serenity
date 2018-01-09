var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function")
        r = Reflect.decorate(decorators, target, key, desc);
    else
        for (var i = decorators.length - 1; i >= 0; i--)
            if (d = decorators[i])
                r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b)
        if (b.hasOwnProperty(p) && p !== '__metadata')
            d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
/**
 * Represents the completion of an asynchronous operation
 */
if (typeof Promise === "undefined") {
    if (typeof (RSVP) !== "undefined") {
        Promise = RSVP;
    }
    else if (typeof (jQuery) !== "undefined") {
        Promise = $.Deferred;
        Promise.resolve = function (value) {
            return jQuery.Deferred().resolveWith(value);
        };
    }
}
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
var Q;
(function (Q) {
    var Culture;
    (function (Culture) {
        Culture.decimalSeparator = '.';
        Culture.dateSeparator = '/';
        Culture.dateOrder = 'dmy';
        Culture.dateFormat = 'dd/MM/yyyy';
        Culture.dateTimeFormat = 'dd/MM/yyyy HH:mm:ss';
        function get_groupSeparator() {
            return ((Culture.decimalSeparator === ',') ? '.' : ',');
        }
        Culture.get_groupSeparator = get_groupSeparator;
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
var Q;
(function (Q) {
    function formatNumber(n, fmt, dec, grp) {
        var neg = '-';
        if (isNaN(n)) {
            return null;
        }
        dec = dec || Q.Culture.decimalSeparator;
        grp = grp || Q.Culture.get_groupSeparator();
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
        var ts = Q.Culture.get_groupSeparator();
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
        var ts = Q.Culture.get_groupSeparator();
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
        if (id.length >= 15 || !(/^-?\d+$/.test(id)))
            return id;
        var v = parseInt(id, 10);
        if (isNaN(v))
            return id;
        return v;
    }
    Q.toId = toId;
})(Q || (Q = {}));
var Q;
(function (Q) {
    function formatDate(d, format) {
        if (!d) {
            return '';
        }
        var date;
        if (typeof d == "string") {
            var res = Q.parseDate(d);
            if (!res)
                return d;
            date = res;
        }
        else
            date = d;
        if (format == null || format == "d") {
            format = Q.Culture.dateFormat;
        }
        else {
            switch (format) {
                case "g":
                    format = Q.Culture.dateTimeFormat.replace(":ss", "");
                    break;
                case "G":
                    format = Q.Culture.dateTimeFormat;
                    break;
                case "s":
                    format = "yyyy-MM-ddTHH:mm:ss";
                    break;
                case "u": return Q.formatISODateTimeUTC(date);
            }
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
        if (s.length >= 10 && s.charAt(4) === '-' && s.charAt(7) === '-' &&
            (s.length === 10 || (s.length > 10 && s.charAt(10) === 'T'))) {
            var res = Q.parseISODateTime(s);
            if (res == null)
                return false;
            return res;
        }
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
    var LT = /** @class */ (function () {
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
                    LT.$table[key] = lt.key;
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
    var Lookup = /** @class */ (function () {
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
        var result = $('#' + elementId + relativeId);
        if (result.length > 0) {
            return result;
        }
        result = $('#' + elementId + '_' + relativeId);
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
    function attrEncoder(a) {
        switch (a) {
            case '&': return '&amp;';
            case '>': return '&gt;';
            case '<': return '&lt;';
            case '\'': return '&apos;';
            case '\"': return '&quot;';
        }
        return a;
    }
    var attrRegex = /[><&'"]/g;
    /**
     * Html attribute encodes a string (encodes quotes in addition to &, > and <)
     * @param s String to be HTML attribute encoded
     */
    function attrEncode(s) {
        var text = (s == null ? '' : s.toString());
        if (attrRegex.test(text)) {
            return text.replace(attrRegex, attrEncoder);
        }
        return text;
    }
    Q.attrEncode = attrEncode;
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
        var csrfToken = Q.getCookie('CSRF-TOKEN');
        if (csrfToken) {
            $('<input/>').attr('type', 'hidden').attr('name', '__RequestVerificationToken')
                .appendTo(div).val(csrfToken);
        }
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
        var csrfToken = Q.getCookie('CSRF-TOKEN');
        if (csrfToken) {
            $('<input/>').attr('type', 'hidden').attr('name', '__RequestVerificationToken')
                .appendTo(div).val(csrfToken);
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
var Q;
(function (Q) {
    function getCookie(name) {
        if ($.cookie)
            return $.cookie(name);
        name += '=';
        for (var ca = document.cookie.split(/;\s*/), i = ca.length - 1; i >= 0; i--)
            if (!ca[i].indexOf(name))
                return ca[i].replace(name, '');
    }
    Q.getCookie = getCookie;
    $.ajaxSetup({
        beforeSend: function (xhr) {
            var token = Q.getCookie('CSRF-TOKEN');
            if (token)
                xhr.setRequestHeader('X-CSRF-TOKEN', token);
        }
    });
    function serviceCall(options) {
        var handleError = function (response) {
            if (Q.Config.notLoggedInHandler != null &&
                response &&
                response.Error &&
                response.Error.Code == 'NotLoggedIn' &&
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
                    if (!html) {
                        if (!xhr.status)
                            Q.alert("An unknown AJAX connection error occured! Check browser console for details.");
                        else if (xhr.status == 500)
                            Q.alert("HTTP 500: Connection refused! Check browser console for details.");
                        else
                            Q.alert("HTTP " + xhr.status + ' error! Check browser console for details.');
                    }
                    else
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
        gridDiv.one('remove', function () {
            $(window).off('layout', layout);
            $('body').off('layout', layout);
        });
        // ugly, but to it is to make old pages work without having to add this
        Q.Router.resolve();
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
        var n = Math.round(h) + 'px';
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
            return Promise.resolve().then(function () {
                Q.blockUI(null);
                return Promise.resolve($.ajax({ async: true, cache: true, type: 'GET', url: url, data: null, dataType: 'script' }).always(function () {
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
            return Promise.resolve().then(function () {
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
            return Promise.resolve().then(function () {
                var data = loadedData[name];
                if (data != null) {
                    return Promise.resolve(data);
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
            return Promise.resolve().then(function () {
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
            var t = new Date().getTime();
            for (var k in scripts) {
                registered[k] = scripts[k] || t;
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
    //all browsers seem to show some unhandled exception message so don't enable this for now
    //window.addEventListener('unhandledrejection', function (e: any) {
    //    var error = e.reason || e;
    //    log(e);
    //    log((error.get_stack && error.get_stack()) || error.stack);
    //});
    //window.addEventListener('error', function (e: any) {
    //    var error = (e.error | e) as any;
    //    log(e);
    //    log((error.get_stack && error.get_stack()) || error.stack);
    //});
    (function (global) {
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
                    if (baseType && baseType.__class)
                        obj.__class = true;
                }
                if (obj.__class || obj.__enum || obj.__interface) {
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
var Q;
(function (Q) {
    var Authorization;
    (function (Authorization) {
        function hasPermission(permission) {
            if (permission == null)
                return false;
            if (permission == "*")
                return true;
            if (permission == "" || permission == "?")
                return Authorization.isLoggedIn;
            var ud = Authorization.userDefinition;
            if (ud && ud.IsAdmin)
                return true;
            if (ud && ud.Permissions) {
                var p = ud.Permissions;
                if (p[permission])
                    return true;
                var orParts = permission.split('|');
                for (var _i = 0, orParts_1 = orParts; _i < orParts_1.length; _i++) {
                    var r = orParts_1[_i];
                    if (!r)
                        continue;
                    var andParts = r.split('&');
                    if (!andParts.length)
                        continue;
                    var fail = false;
                    for (var _a = 0, andParts_1 = andParts; _a < andParts_1.length; _a++) {
                        var n = andParts_1[_a];
                        if (!p[n]) {
                            fail = true;
                            break;
                        }
                    }
                    if (!fail)
                        return true;
                }
            }
            return false;
        }
        Authorization.hasPermission = hasPermission;
        function validatePermission(permission) {
            if (!hasPermission(permission)) {
                Q.notifyError(Q.text("Authorization.AccessDenied"));
                throw new Error(Q.text("Authorization.AccessDenied"));
            }
        }
        Authorization.validatePermission = validatePermission;
    })(Authorization = Q.Authorization || (Q.Authorization = {}));
    Object.defineProperty(Q.Authorization, "userDefinition", {
        get: function () {
            return Q.getRemoteData("UserData");
        }
    });
    Object.defineProperty(Q.Authorization, "isLoggedIn", {
        get: function () {
            var ud = Authorization.userDefinition;
            return ud && !!ud.Username;
        }
    });
    Object.defineProperty(Q.Authorization, "username", {
        get: function () {
            var ud = Authorization.userDefinition;
            if (ud)
                return ud.Username;
            return null;
        }
    });
})(Q || (Q = {}));
var Q;
(function (Q) {
    var Router;
    (function (Router) {
        var oldURL;
        var resolving = 0;
        var autoinc = 0;
        var listenerTimeout;
        var ignoreHash = 0;
        var ignoreTime = 0;
        Router.enabled = true;
        function isEqual(url1, url2) {
            return url1 == url2 || url1 == url2 + '#' || url2 == url1 + '#';
        }
        function navigate(hash, tryBack, silent) {
            if (!Router.enabled || resolving > 0)
                return;
            hash = hash || '';
            hash = hash.replace(/^#/, '');
            hash = (!hash ? "" : '#' + hash);
            var newURL = window.location.href.replace(/#$/, '')
                .replace(/#.*$/, '') + hash;
            if (newURL != window.location.href) {
                if (tryBack && oldURL != null && isEqual(oldURL, newURL)) {
                    if (silent)
                        ignoreChange();
                    var prior = window.location.href;
                    oldURL = null;
                    window.history.back();
                    return;
                }
                if (silent)
                    ignoreChange();
                oldURL = window.location.href;
                window.location.hash = hash;
            }
        }
        Router.navigate = navigate;
        function replace(hash, tryBack) {
            navigate(hash, tryBack, true);
        }
        Router.replace = replace;
        function replaceLast(hash, tryBack) {
            if (!Router.enabled)
                return;
            var current = window.location.hash || '';
            if (current.charAt(0) == '#')
                current = current.substr(1, current.length - 1);
            var parts = current.split('/+/');
            if (parts.length > 1) {
                if (hash && hash.length) {
                    parts[parts.length - 1] = hash;
                    hash = parts.join("/+/");
                }
                else {
                    parts.splice(parts.length - 1, 1);
                    hash = parts.join("/+/");
                }
            }
            replace(hash, tryBack);
        }
        Router.replaceLast = replaceLast;
        function visibleDialogs() {
            return $('.ui-dialog-content:visible, .ui-dialog.panel-hidden>.ui-dialog-content, .s-Panel').toArray().sort(function (a, b) {
                return ($(a).data('qrouterorder') || 0) - ($(b).data('qrouterorder') || 0);
            });
        }
        function dialogOpen(owner, element, hash) {
            var route = [];
            var isDialog = owner.hasClass(".ui-dialog-content") || owner.hasClass('.s-Panel');
            var dialog = isDialog ? owner :
                owner.closest('.ui-dialog-content, .s-Panel');
            var value = hash();
            var idPrefix;
            if (dialog.length) {
                var dialogs = visibleDialogs();
                var index = dialogs.indexOf(dialog[0]);
                for (var i = 0; i <= index; i++) {
                    var q = $(dialogs[i]).data("qroute");
                    if (q && q.length)
                        route.push(q);
                }
                if (!isDialog) {
                    idPrefix = dialog.attr("id");
                    if (idPrefix) {
                        idPrefix += "_";
                        var id = owner.attr("id");
                        if (id && Q.startsWith(id, idPrefix))
                            value = id.substr(idPrefix.length) + '@' + value;
                    }
                }
            }
            else {
                var id = owner.attr("id");
                if (id && (!owner.hasClass("route-handler") ||
                    $('.route-handler').first().attr("id") != id))
                    value = id + "@" + value;
            }
            route.push(value);
            element.data("qroute", value);
            replace(route.join("/+/"));
            element.bind("dialogclose.qrouter panelclose.qrouter", function (e) {
                element.data("qroute", null);
                element.unbind(".qrouter");
                var prhash = element.data("qprhash");
                var tryBack = $(e.target).closest('.s-MessageDialog').length > 0 || (e && e.originalEvent &&
                    ((e.originalEvent.type == "keydown" && e.originalEvent.keyCode == 27) ||
                        $(e.originalEvent.target).hasClass("ui-dialog-titlebar-close") ||
                        $(e.originalEvent.target).hasClass("panel-titlebar-close")));
                if (prhash != null)
                    replace(prhash, tryBack);
                else
                    replaceLast('', tryBack);
            });
        }
        function dialog(owner, element, hash) {
            if (!Router.enabled)
                return;
            element.on("dialogopen.qrouter panelopen.qrouter", function (e) {
                dialogOpen(owner, element, hash);
            });
        }
        Router.dialog = dialog;
        function resolve(hash) {
            if (!Router.enabled)
                return;
            resolving++;
            try {
                hash = Q.coalesce(Q.coalesce(hash, window.location.hash), '');
                if (hash.charAt(0) == '#')
                    hash = hash.substr(1, hash.length - 1);
                var dialogs = visibleDialogs();
                var newParts = hash.split("/+/");
                var oldParts = dialogs.map(function (el) { return $(el).data('qroute'); });
                var same = 0;
                while (same < dialogs.length &&
                    same < newParts.length &&
                    oldParts[same] == newParts[same]) {
                    same++;
                }
                for (var i = same; i < dialogs.length; i++) {
                    var d = $(dialogs[i]);
                    if (d.hasClass('ui-dialog-content'))
                        d.dialog('close');
                    else if (d.hasClass('s-Panel'))
                        Serenity.TemplatedDialog.closePanel(d);
                }
                for (var i = same; i < newParts.length; i++) {
                    var route = newParts[i];
                    var routeParts = route.split('@');
                    var handler;
                    if (routeParts.length == 2) {
                        var dialog = i > 0 ? $(dialogs[i - 1]) : $([]);
                        if (dialog.length) {
                            var idPrefix = dialog.attr("id");
                            if (idPrefix) {
                                handler = $('#' + idPrefix + "_" + routeParts[0]);
                                if (handler.length) {
                                    route = routeParts[1];
                                }
                            }
                        }
                        if (!handler || !handler.length) {
                            handler = $('#' + routeParts[0]);
                            if (handler.length) {
                                route = routeParts[1];
                            }
                        }
                    }
                    if (!handler || !handler.length) {
                        handler = i > 0 ? $(dialogs[i - 1]) :
                            $('.route-handler').first();
                    }
                    handler.triggerHandler("handleroute", {
                        handled: false,
                        route: route,
                        parts: newParts,
                        index: i
                    });
                }
            }
            finally {
                resolving--;
            }
        }
        Router.resolve = resolve;
        function hashChange(e, o) {
            if (ignoreHash > 0) {
                if (new Date().getTime() - ignoreTime > 1000) {
                    ignoreHash = 0;
                }
                else {
                    ignoreHash--;
                    return;
                }
            }
            resolve();
        }
        function ignoreChange() {
            ignoreHash++;
            ignoreTime = new Date().getTime();
        }
        window.addEventListener("hashchange", hashChange, false);
        var routerOrder = 1;
        $(document).on("dialogopen panelopen", ".ui-dialog-content, .s-Panel", function (event, ui) {
            if (!Router.enabled)
                return;
            var dlg = $(event.target);
            dlg.data("qrouterorder", routerOrder++);
            if (dlg.data("qroute"))
                return;
            dlg.data("qprhash", window.location.hash);
            var owner = $(visibleDialogs).not(dlg).last();
            if (!owner.length)
                owner = $('html');
            dialogOpen(owner, dlg, function () {
                return "!" + (++autoinc).toString(36);
            });
        });
    })(Router = Q.Router || (Q.Router = {}));
})(Q || (Q = {}));
var Serenity;
(function (Serenity) {
    var ColumnsKeyAttribute = /** @class */ (function () {
        function ColumnsKeyAttribute(value) {
            this.value = value;
        }
        return ColumnsKeyAttribute;
    }());
    Serenity.ColumnsKeyAttribute = ColumnsKeyAttribute;
    var DialogTypeAttribute = /** @class */ (function () {
        function DialogTypeAttribute(value) {
            this.value = value;
        }
        return DialogTypeAttribute;
    }());
    Serenity.DialogTypeAttribute = DialogTypeAttribute;
    var EditorAttribute = /** @class */ (function () {
        function EditorAttribute() {
        }
        return EditorAttribute;
    }());
    Serenity.EditorAttribute = EditorAttribute;
    var ElementAttribute = /** @class */ (function () {
        function ElementAttribute(value) {
            this.value = value;
        }
        return ElementAttribute;
    }());
    Serenity.ElementAttribute = ElementAttribute;
    var EntityTypeAttribute = /** @class */ (function () {
        function EntityTypeAttribute(value) {
            this.value = value;
        }
        return EntityTypeAttribute;
    }());
    Serenity.EntityTypeAttribute = EntityTypeAttribute;
    var EnumKeyAttribute = /** @class */ (function () {
        function EnumKeyAttribute(value) {
            this.value = value;
        }
        return EnumKeyAttribute;
    }());
    Serenity.EnumKeyAttribute = EnumKeyAttribute;
    var FlexifyAttribute = /** @class */ (function () {
        function FlexifyAttribute(value) {
            if (value === void 0) { value = true; }
            this.value = value;
        }
        return FlexifyAttribute;
    }());
    Serenity.FlexifyAttribute = FlexifyAttribute;
    var FormKeyAttribute = /** @class */ (function () {
        function FormKeyAttribute(value) {
            this.value = value;
        }
        return FormKeyAttribute;
    }());
    Serenity.FormKeyAttribute = FormKeyAttribute;
    var GeneratedCodeAttribute = /** @class */ (function () {
        function GeneratedCodeAttribute(origin) {
            this.origin = origin;
        }
        return GeneratedCodeAttribute;
    }());
    Serenity.GeneratedCodeAttribute = GeneratedCodeAttribute;
    var IdPropertyAttribute = /** @class */ (function () {
        function IdPropertyAttribute(value) {
            this.value = value;
        }
        return IdPropertyAttribute;
    }());
    Serenity.IdPropertyAttribute = IdPropertyAttribute;
    var IsActivePropertyAttribute = /** @class */ (function () {
        function IsActivePropertyAttribute(value) {
            this.value = value;
        }
        return IsActivePropertyAttribute;
    }());
    Serenity.IsActivePropertyAttribute = IsActivePropertyAttribute;
    var ItemNameAttribute = /** @class */ (function () {
        function ItemNameAttribute(value) {
            this.value = value;
        }
        return ItemNameAttribute;
    }());
    Serenity.ItemNameAttribute = ItemNameAttribute;
    var LocalTextPrefixAttribute = /** @class */ (function () {
        function LocalTextPrefixAttribute(value) {
            this.value = value;
        }
        return LocalTextPrefixAttribute;
    }());
    Serenity.LocalTextPrefixAttribute = LocalTextPrefixAttribute;
    var MaximizableAttribute = /** @class */ (function () {
        function MaximizableAttribute(value) {
            if (value === void 0) { value = true; }
            this.value = value;
        }
        return MaximizableAttribute;
    }());
    Serenity.MaximizableAttribute = MaximizableAttribute;
    var NamePropertyAttribute = /** @class */ (function () {
        function NamePropertyAttribute(value) {
            this.value = value;
        }
        return NamePropertyAttribute;
    }());
    Serenity.NamePropertyAttribute = NamePropertyAttribute;
    var OptionAttribute = /** @class */ (function () {
        function OptionAttribute() {
        }
        return OptionAttribute;
    }());
    Serenity.OptionAttribute = OptionAttribute;
    var OptionsTypeAttribute = /** @class */ (function () {
        function OptionsTypeAttribute(value) {
            this.value = value;
        }
        return OptionsTypeAttribute;
    }());
    Serenity.OptionsTypeAttribute = OptionsTypeAttribute;
    var PanelAttribute = /** @class */ (function () {
        function PanelAttribute(value) {
            if (value === void 0) { value = true; }
            this.value = value;
        }
        return PanelAttribute;
    }());
    Serenity.PanelAttribute = PanelAttribute;
    var ResizableAttribute = /** @class */ (function () {
        function ResizableAttribute(value) {
            if (value === void 0) { value = true; }
            this.value = value;
        }
        return ResizableAttribute;
    }());
    Serenity.ResizableAttribute = ResizableAttribute;
    var ResponsiveAttribute = /** @class */ (function () {
        function ResponsiveAttribute(value) {
            if (value === void 0) { value = true; }
            this.value = value;
        }
        return ResponsiveAttribute;
    }());
    Serenity.ResponsiveAttribute = ResponsiveAttribute;
    var ServiceAttribute = /** @class */ (function () {
        function ServiceAttribute(value) {
            this.value = value;
        }
        return ServiceAttribute;
    }());
    Serenity.ServiceAttribute = ServiceAttribute;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var Decorators;
    (function (Decorators) {
        function addAttribute(type, attr) {
            type.__metadata = type.__metadata || {};
            type.__metadata.attr = type.__metadata.attr || [];
            type.__metadata.attr.push(attr);
        }
        Decorators.addAttribute = addAttribute;
        function addMemberAttr(type, memberName, attr) {
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
        function distinct(arr) {
            return arr.filter(function (item, pos) { return arr.indexOf(item) === pos; });
        }
        function merge(arr1, arr2) {
            if (!arr1 || !arr2)
                return (arr1 || arr2 || []).slice();
            return distinct(arr1.concat(arr2));
        }
        function registerClass(nameOrIntf, intf2) {
            return function (target) {
                if (typeof nameOrIntf == "string") {
                    target.__typeName = nameOrIntf;
                    if (intf2)
                        target.__interfaces = merge(target.__interfaces, intf2);
                }
                else {
                    target.__register = true;
                    if (nameOrIntf)
                        target.__interfaces = merge(target.__interfaces, nameOrIntf);
                }
                target.__class = true;
                target.__assembly = ss.__assemblies['App'];
            };
        }
        Decorators.registerClass = registerClass;
        function registerInterface(nameOrIntf, intf2) {
            return function (target) {
                if (typeof nameOrIntf == "string") {
                    target.__typeName = nameOrIntf;
                    if (intf2)
                        target.__interfaces = intf2;
                }
                else {
                    target.__register = true;
                    if (nameOrIntf)
                        target.__interfaces = merge(target.__interfaces, nameOrIntf);
                }
                target.__interface = true;
                target.__assembly = ss.__assemblies['App'];
                target.isAssignableFrom = function (type) {
                    return ss.contains(ss.getInterfaces(type), this);
                };
            };
        }
        Decorators.registerInterface = registerInterface;
        function registerEnum(target, enumKey, typeName, asm) {
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
                if (Q.coalesce(typeName, enumKey))
                    target.__typeName = Q.coalesce(typeName, enumKey);
                else
                    target.__register = true;
                if (enumKey)
                    addAttribute(target, new Serenity.EnumKeyAttribute(enumKey));
            }
        }
        Decorators.registerEnum = registerEnum;
        function registerEditor(nameOrIntf, intf2) {
            return registerClass(nameOrIntf, intf2);
        }
        Decorators.registerEditor = registerEditor;
        function registerFormatter(nameOrIntf, intf2) {
            if (nameOrIntf === void 0) { nameOrIntf = [Serenity.ISlickFormatter]; }
            if (intf2 === void 0) { intf2 = [Serenity.ISlickFormatter]; }
            return registerClass(nameOrIntf, intf2);
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
                var bsTabs = element.closest('.nav-tabs');
                if (bsTabs.length > 0) {
                    bsTabs.one('shown.bs.tab', function (e) {
                        if (!executed && element.is(':visible')) {
                            executed = true;
                            callback();
                        }
                    });
                }
                var dialog;
                if (element.hasClass('ui-dialog')) {
                    dialog = element.children('.ui-dialog-content');
                }
                else {
                    dialog = element.closest('.ui-dialog-content, .s-TemplatedDialog');
                }
                if (dialog.length > 0) {
                    dialog.bind('dialogopen.' + eventClass + ' panelopen.' + eventClass, function () {
                        dialog.unbind('dialogopen.' + eventClass);
                        dialog.unbind('panelopen.' + eventClass);
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
            var dialog = element.closest('.ui-dialog-content, .s-TemplatedDialog');
            if (dialog.length > 0) {
                dialog.bind('dialogopen.' + eventClass + ' panelopen.' + eventClass, check);
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
        function selectTab(tabs, tabKey) {
            var ibk = indexByKey(tabs);
            if (!ibk)
                return;
            var index = ibk[tabKey];
            if (index == null) {
                return;
            }
            if (index !== tabs.tabs('option', 'active')) {
                tabs.tabs('option', 'active', index);
            }
        }
        TabsExtensions.selectTab = selectTab;
    })(TabsExtensions = Serenity.TabsExtensions || (Serenity.TabsExtensions = {}));
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var Widget = /** @class */ (function () {
        function Widget(element, options) {
            var _this = this;
            this.element = element;
            this.options = options || {};
            this.widgetName = Widget_1.getWidgetName(ss.getInstanceType(this));
            this.uniqueName = this.widgetName + (Widget_1.nextWidgetNumber++).toString();
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
        Widget_1 = Widget;
        Widget.prototype.destroy = function () {
            this.element.removeClass('s-' + ss.getTypeName(ss.getInstanceType(this)));
            this.element.unbind('.' + this.widgetName).unbind('.' + this.uniqueName).removeData(this.widgetName);
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
            return Promise.resolve();
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
                var e = Widget_1.elementFor(params.type);
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
                return Promise.resolve();
            }
            if (!this.asyncPromise) {
                this.asyncPromise = this.initializeAsync();
            }
            return this.asyncPromise;
        };
        Widget.nextWidgetNumber = 0;
        Widget = Widget_1 = __decorate([
            Serenity.Decorators.registerClass()
        ], Widget);
        return Widget;
        var Widget_1;
    }());
    Serenity.Widget = Widget;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var TemplatedWidget = /** @class */ (function (_super) {
        __extends(TemplatedWidget, _super);
        function TemplatedWidget(container, options) {
            var _this = _super.call(this, container, options) || this;
            _this.idPrefix = _this.uniqueName + '_';
            var widgetMarkup = _this.getTemplate().replace(new RegExp('~_', 'g'), _this.idPrefix);
            // for compability with older templates based on JsRender
            var end = 0;
            while (true) {
                var idx = widgetMarkup.indexOf('{{text:"', end);
                if (idx < 0)
                    break;
                var end = widgetMarkup.indexOf('"}}', idx);
                if (end < 0)
                    break;
                var key = widgetMarkup.substr(idx + 8, end - idx - 8);
                var text = Q.text(key);
                widgetMarkup = widgetMarkup.substr(0, idx) + text + widgetMarkup.substr(end + 3);
                end = idx + text.length;
            }
            _this.element.html(widgetMarkup);
            return _this;
        }
        TemplatedWidget_1 = TemplatedWidget;
        TemplatedWidget.prototype.byId = function (id) {
            return $('#' + this.idPrefix + id);
        };
        TemplatedWidget.prototype.byID = function (id, type) {
            return Serenity.WX.getWidget(this.byId(id), type);
        };
        TemplatedWidget.noGeneric = function (s) {
            var dollar = s.indexOf('$');
            if (dollar >= 0) {
                return s.substr(0, dollar);
            }
            return s;
        };
        TemplatedWidget.prototype.getDefaultTemplateName = function () {
            return TemplatedWidget_1.noGeneric(ss.getTypeName(ss.getInstanceType(this)));
        };
        TemplatedWidget.prototype.getTemplateName = function () {
            var type = ss.getInstanceType(this);
            var fullName = ss.getTypeFullName(type);
            var templateNames = TemplatedWidget_1.templateNames;
            var cachedName = TemplatedWidget_1.templateNames[fullName];
            if (cachedName != null) {
                return cachedName;
            }
            while (type && type !== Serenity.Widget) {
                var name = TemplatedWidget_1.noGeneric(ss.getTypeFullName(type));
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
                name = TemplatedWidget_1.noGeneric(ss.getTypeName(type));
                if (Q.canLoadScriptData('Template.' + name) ||
                    $('script#Template_' + name).length > 0) {
                    TemplatedWidget_1.templateNames[fullName] = name;
                    return name;
                }
                type = ss.getBaseType(type);
            }
            templateNames[fullName] = cachedName = this.getDefaultTemplateName();
            return cachedName;
        };
        TemplatedWidget.prototype.getFallbackTemplate = function () {
            return null;
        };
        TemplatedWidget.prototype.getTemplate = function () {
            var templateName = this.getTemplateName();
            var script = $('script#Template_' + templateName);
            if (script.length > 0) {
                return script.html();
            }
            var template;
            if (!Q.canLoadScriptData('Template.' + templateName) &&
                this.getDefaultTemplateName() == templateName) {
                template = this.getFallbackTemplate();
                if (template != null)
                    return template;
            }
            template = Q.getTemplate(templateName);
            if (template == null) {
                throw new Error(Q.format("Can't locate template for widget '{0}' with name '{1}'!", ss.getTypeName(ss.getInstanceType(this)), templateName));
            }
            return template;
        };
        TemplatedWidget.templateNames = {};
        TemplatedWidget = TemplatedWidget_1 = __decorate([
            Serenity.Decorators.registerClass()
        ], TemplatedWidget);
        return TemplatedWidget;
        var TemplatedWidget_1;
    }(Serenity.Widget));
    Serenity.TemplatedWidget = TemplatedWidget;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var IBooleanValue = /** @class */ (function () {
        function IBooleanValue() {
        }
        IBooleanValue = __decorate([
            Serenity.Decorators.registerInterface()
        ], IBooleanValue);
        return IBooleanValue;
    }());
    Serenity.IBooleanValue = IBooleanValue;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var IDoubleValue = /** @class */ (function () {
        function IDoubleValue() {
        }
        IDoubleValue = __decorate([
            Serenity.Decorators.registerInterface()
        ], IDoubleValue);
        return IDoubleValue;
    }());
    Serenity.IDoubleValue = IDoubleValue;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var IGetEditValue = /** @class */ (function () {
        function IGetEditValue() {
        }
        IGetEditValue = __decorate([
            Serenity.Decorators.registerInterface()
        ], IGetEditValue);
        return IGetEditValue;
    }());
    Serenity.IGetEditValue = IGetEditValue;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var ISetEditValue = /** @class */ (function () {
        function ISetEditValue() {
        }
        ISetEditValue = __decorate([
            Serenity.Decorators.registerInterface()
        ], ISetEditValue);
        return ISetEditValue;
    }());
    Serenity.ISetEditValue = ISetEditValue;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var IStringValue = /** @class */ (function () {
        function IStringValue() {
        }
        IStringValue = __decorate([
            Serenity.Decorators.registerInterface()
        ], IStringValue);
        return IStringValue;
    }());
    Serenity.IStringValue = IStringValue;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var IReadOnly = /** @class */ (function () {
        function IReadOnly() {
        }
        IReadOnly = __decorate([
            Serenity.Decorators.registerInterface('Serenity.IReadOnly')
        ], IReadOnly);
        return IReadOnly;
    }());
    Serenity.IReadOnly = IReadOnly;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var BooleanEditor = /** @class */ (function (_super) {
        __extends(BooleanEditor, _super);
        function BooleanEditor(input) {
            var _this = _super.call(this, input) || this;
            input.removeClass("flexify");
            return _this;
        }
        Object.defineProperty(BooleanEditor.prototype, "value", {
            get: function () {
                return this.element.is(":checked");
            },
            set: function (value) {
                this.element.prop("checked", !!value);
            },
            enumerable: true,
            configurable: true
        });
        BooleanEditor.prototype.get_value = function () {
            return this.value;
        };
        BooleanEditor.prototype.set_value = function (value) {
            this.value = value;
        };
        BooleanEditor = __decorate([
            Serenity.Decorators.element('<input type="checkbox"/>'),
            Serenity.Decorators.registerEditor([Serenity.IBooleanValue])
        ], BooleanEditor);
        return BooleanEditor;
    }(Serenity.Widget));
    Serenity.BooleanEditor = BooleanEditor;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var Select2Editor = /** @class */ (function (_super) {
        __extends(Select2Editor, _super);
        function Select2Editor(hidden, opt) {
            var _this = _super.call(this, hidden, opt) || this;
            _this.pageSize = 100;
            _this.items = [];
            _this.itemById = {};
            var emptyItemText = _this.emptyItemText();
            if (emptyItemText != null) {
                hidden.attr('placeholder', emptyItemText);
            }
            var select2Options = _this.getSelect2Options();
            _this.multiple = !!select2Options.multiple;
            hidden.select2(select2Options);
            hidden.attr('type', 'text');
            // jquery validate to work
            hidden.bind('change.' + _this.uniqueName, function (e, x) {
                if (!!(Serenity.WX.hasOriginalEvent(e) || !x)) {
                    if (Serenity.ValidationHelper.getValidator(hidden) != null) {
                        hidden.valid();
                    }
                }
            });
            return _this;
        }
        ;
        Select2Editor.prototype.destroy = function () {
            if (this.element != null) {
                this.element.select2('destroy');
            }
            _super.prototype.destroy.call(this);
        };
        Select2Editor.prototype.emptyItemText = function () {
            return Q.coalesce(this.element.attr('placeholder'), Q.text('Controls.SelectEditor.EmptyItemText'));
        };
        Select2Editor.prototype.getSelect2Options = function () {
            var _this = this;
            var emptyItemText = this.emptyItemText();
            return {
                data: this.items,
                placeHolder: (!Q.isEmptyOrNull(emptyItemText) ? emptyItemText : null),
                allowClear: emptyItemText != null,
                createSearchChoicePosition: 'bottom',
                query: function (query) {
                    var term = (Q.isEmptyOrNull(query.term) ? '' : Select2.util.stripDiacritics(Q.coalesce(query.term, '')).toUpperCase());
                    var results = _this.items.filter(function (item) {
                        return term == null || Q.startsWith(Select2.util.stripDiacritics(Q.coalesce(item.text, '')).toUpperCase(), term);
                    });
                    results.push.apply(results, _this.items.filter(function (item1) {
                        return term != null && !Q.startsWith(Select2.util.stripDiacritics(Q.coalesce(item1.text, '')).toUpperCase(), term) &&
                            Select2.util.stripDiacritics(Q.coalesce(item1.text, ''))
                                .toUpperCase().indexOf(term) >= 0;
                    }));
                    query.callback({
                        results: results.slice((query.page - 1) * _this.pageSize, query.page * _this.pageSize),
                        more: results.length >= query.page * _this.pageSize
                    });
                },
                initSelection: function (element, callback) {
                    var val = element.val();
                    var isAutoComplete = _this.isAutoComplete();
                    if (_this.multiple) {
                        var list = [];
                        var $t1 = val.split(',');
                        for (var $t2 = 0; $t2 < $t1.length; $t2++) {
                            var z = $t1[$t2];
                            var item2 = _this.itemById[z];
                            if (item2 == null && isAutoComplete) {
                                item2 = { id: z, text: z };
                                _this.addItem(item2);
                            }
                            if (item2 != null) {
                                list.push(item2);
                            }
                        }
                        callback(list);
                        return;
                    }
                    var it = _this.itemById[val];
                    if (it == null && isAutoComplete) {
                        it = { id: val, text: val };
                        _this.addItem(it);
                    }
                    callback(it);
                }
            };
        };
        Select2Editor.prototype.get_delimited = function () {
            return !!!!this.options['delimited'];
        };
        Select2Editor.prototype.clearItems = function () {
            ss.clear(this.items);
            this.itemById = {};
        };
        Select2Editor.prototype.addItem = function (item) {
            this.items.push(item);
            this.itemById[item.id] = item;
        };
        Select2Editor.prototype.addOption = function (key, text, source, disabled) {
            this.addItem({
                id: key,
                text: text,
                source: source,
                disabled: disabled
            });
        };
        Select2Editor.prototype.addInplaceCreate = function (addTitle, editTitle) {
            var _this = this;
            var self = this;
            addTitle = Q.coalesce(addTitle, Q.text('Controls.SelectEditor.InplaceAdd'));
            editTitle = Q.coalesce(editTitle, Q.text('Controls.SelectEditor.InplaceEdit'));
            var inplaceButton = $('<a><b/></a>')
                .addClass('inplace-button inplace-create')
                .attr('title', addTitle)
                .insertAfter(this.element).click(function (e) {
                self.inplaceCreateClick(e);
            });
            this.get_select2Container().add(this.element).addClass('has-inplace-button');
            Serenity.WX.change(this, function (e1) {
                var isNew = _this.multiple || Q.isEmptyOrNull(_this.get_value());
                inplaceButton.attr('title', (isNew ? addTitle : editTitle)).toggleClass('edit', !isNew);
            });
            Serenity.WX.changeSelect2(this, function (e2) {
                if (_this.multiple) {
                    var values = _this.get_values();
                    if (values.length > 0 && values[values.length - 1] == (-2147483648).toString()) {
                        _this.set_values(values.slice(0, values.length - 1));
                        _this.inplaceCreateClick(e2);
                    }
                }
                else if (_this.get_value() == (-2147483648).toString()) {
                    _this.set_value(null);
                    _this.inplaceCreateClick(e2);
                }
            });
            if (this.multiple) {
                this.get_select2Container().on('dblclick.' + this.uniqueName, '.select2-search-choice', function (e3) {
                    var q = $(e3.target);
                    if (!q.hasClass('select2-search-choice')) {
                        q = q.closest('.select2-search-choice');
                    }
                    var index = q.index();
                    var values1 = _this.get_values();
                    if (index < 0 || index >= _this.get_values().length) {
                        return;
                    }
                    e3['editItem'] = values1[index];
                    _this.inplaceCreateClick(e3);
                });
            }
        };
        Select2Editor.prototype.inplaceCreateClick = function (e) {
        };
        Select2Editor.prototype.isAutoComplete = function () {
            return false;
        };
        Select2Editor.prototype.getCreateSearchChoice = function (getName) {
            var _this = this;
            return function (s) {
                _this.lastCreateTerm = s;
                s = Q.coalesce(Select2.util.stripDiacritics(s), '').toLowerCase();
                if (Q.isTrimmedEmpty(s)) {
                    return null;
                }
                if (Q.any(_this.get_items(), function (x) {
                    var text = getName ? getName(x.source) : x.text;
                    return Select2.util.stripDiacritics(Q.coalesce(text, '')).toLowerCase() == s;
                }))
                    return null;
                if (!Q.any(_this.get_items(), function (x1) {
                    return Q.coalesce(Select2.util.stripDiacritics(x1.text), '').toLowerCase().indexOf(s) !== -1;
                })) {
                    if (_this.isAutoComplete()) {
                        return {
                            id: _this.lastCreateTerm,
                            text: _this.lastCreateTerm
                        };
                    }
                    return {
                        id: (-2147483648).toString(),
                        text: Q.text('Controls.SelectEditor.NoResultsClickToDefine')
                    };
                }
                if (_this.isAutoComplete()) {
                    return {
                        id: _this.lastCreateTerm,
                        text: _this.lastCreateTerm
                    };
                }
                return {
                    id: (-2147483648).toString(),
                    text: Q.text('Controls.SelectEditor.ClickToDefine')
                };
            };
        };
        Select2Editor.prototype.setEditValue = function (source, property) {
            var val = source[property.name];
            if (Q.isArray(val)) {
                this.set_values(val);
            }
            else {
                this.set_value((val == null ? null : val.toString()));
            }
        };
        Select2Editor.prototype.getEditValue = function (property, target) {
            if (!this.multiple || this.get_delimited()) {
                target[property.name] = this.get_value();
            }
            else {
                target[property.name] = this.get_values();
            }
        };
        Select2Editor.prototype.get_select2Container = function () {
            return this.element.prevAll('.select2-container');
        };
        Select2Editor.prototype.get_items = function () {
            return this.items;
        };
        Select2Editor.prototype.get_itemByKey = function () {
            return this.itemById;
        };
        Select2Editor.prototype.get_value = function () {
            var val;
            if (this.element.data('select2')) {
                val = this.element.select2('val');
                if (val != null && Q.isArray(val)) {
                    return val.join(',');
                }
            }
            else
                val = this.element.val();
            return val;
        };
        Object.defineProperty(Select2Editor.prototype, "value", {
            get: function () {
                return this.get_value();
            },
            set: function (v) {
                this.set_value(v);
            },
            enumerable: true,
            configurable: true
        });
        Select2Editor.prototype.set_value = function (value) {
            if (value != this.get_value()) {
                var val = value;
                if (!Q.isEmptyOrNull(value) && this.multiple) {
                    val = value.split(String.fromCharCode(44)).map(function (x) {
                        return Q.trimToNull(x);
                    }).filter(function (x1) {
                        return x1 != null;
                    });
                }
                this.element.select2('val', val)
                    .triggerHandler('change', [true]);
            }
        };
        Select2Editor.prototype.get_values = function () {
            var val = this.element.select2('val');
            if (val == null) {
                return [];
            }
            if (Q.isArray(val)) {
                return val;
            }
            var str = val;
            if (Q.isEmptyOrNull(str)) {
                return [];
            }
            return [str];
        };
        Object.defineProperty(Select2Editor.prototype, "values", {
            get: function () {
                return this.get_values();
            },
            set: function (value) {
                this.set_values(value);
            },
            enumerable: true,
            configurable: true
        });
        Select2Editor.prototype.set_values = function (value) {
            if (value == null || value.length === 0) {
                this.set_value(null);
                return;
            }
            this.set_value(value.join(','));
        };
        Select2Editor.prototype.get_text = function () {
            return Q.coalesce(this.element.select2('data'), {}).text;
        };
        Object.defineProperty(Select2Editor.prototype, "text", {
            get: function () {
                return this.get_text();
            },
            enumerable: true,
            configurable: true
        });
        Select2Editor.prototype.get_readOnly = function () {
            return !Q.isEmptyOrNull(this.element.attr('readonly'));
        };
        Object.defineProperty(Select2Editor.prototype, "readOnly", {
            get: function () {
                return this.get_readOnly();
            },
            set: function (value) {
                this.set_readOnly(value);
            },
            enumerable: true,
            configurable: true
        });
        Select2Editor.prototype.set_readOnly = function (value) {
            if (value !== this.get_readOnly()) {
                Serenity.EditorUtils.setReadonly(this.element, value);
                this.element.nextAll('.inplace-create')
                    .attr('disabled', (value ? 'disabled' : ''))
                    .css('opacity', (value ? '0.1' : ''))
                    .css('cursor', (value ? 'default' : ''));
            }
        };
        Select2Editor = __decorate([
            Serenity.Decorators.registerClass('Serenity.Select2Editor', [Serenity.ISetEditValue, Serenity.IGetEditValue, Serenity.IStringValue, Serenity.IReadOnly]),
            Serenity.Decorators.element("<input type=\"hidden\"/>")
        ], Select2Editor);
        return Select2Editor;
    }(Serenity.Widget));
    Serenity.Select2Editor = Select2Editor;
    var SelectEditor = /** @class */ (function (_super) {
        __extends(SelectEditor, _super);
        function SelectEditor(hidden, opt) {
            var _this = _super.call(this, hidden, opt) || this;
            _this.updateItems();
            return _this;
        }
        SelectEditor.prototype.getItems = function () {
            return this.options.items || [];
        };
        SelectEditor.prototype.emptyItemText = function () {
            if (!Q.isEmptyOrNull(this.options.emptyOptionText)) {
                return this.options.emptyOptionText;
            }
            return _super.prototype.emptyItemText.call(this);
        };
        SelectEditor.prototype.updateItems = function () {
            var items = this.getItems();
            this.clearItems();
            if (items.length > 0) {
                var isStrings = typeof (items[0]) === 'string';
                for (var $t1 = 0; $t1 < items.length; $t1++) {
                    var item = items[$t1];
                    var key = isStrings ? item : item[0];
                    var text = isStrings ? item : Q.coalesce(item[1], item[0]);
                    this.addOption(key, text, item, false);
                }
            }
        };
        SelectEditor = __decorate([
            Serenity.Decorators.registerClass('Serenity.SelectEditor')
        ], SelectEditor);
        return SelectEditor;
    }(Select2Editor));
    Serenity.SelectEditor = SelectEditor;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var LookupEditorBase = /** @class */ (function (_super) {
        __extends(LookupEditorBase, _super);
        function LookupEditorBase(input, opt) {
            var _this = _super.call(this, input, opt) || this;
            _this.setCascadeFrom(_this.options.cascadeFrom);
            var self = _this;
            if (!_this.isAsyncWidget()) {
                _this.updateItems();
                Q.ScriptData.bindToChange('Lookup.' + _this.getLookupKey(), _this.uniqueName, function () {
                    self.updateItems();
                });
            }
            if (!_this.options.autoComplete &&
                _this.options.inplaceAdd &&
                (_this.options.inplaceAddPermission == null ||
                    Q.Authorization.hasPermission(_this.options.inplaceAddPermission))) {
                _this.addInplaceCreate(Q.text('Controls.SelectEditor.InplaceAdd'), null);
            }
            return _this;
        }
        LookupEditorBase.prototype.initializeAsync = function () {
            var _this = this;
            return this.updateItemsAsync().then(function () {
                Q.ScriptData.bindToChange('Lookup.' + _this.getLookupKey(), _this.uniqueName, function () {
                    _this.updateItemsAsync();
                });
            }, null);
        };
        LookupEditorBase.prototype.destroy = function () {
            Q.ScriptData.unbindFromChange(this.uniqueName);
            Serenity.Select2Editor.prototype.destroy.call(this);
        };
        LookupEditorBase.prototype.getLookupKey = function () {
            if (this.options.lookupKey != null) {
                return this.options.lookupKey;
            }
            var key = ss.getTypeFullName(ss.getInstanceType(this));
            var idx = key.indexOf('.');
            if (idx >= 0) {
                key = key.substring(idx + 1);
            }
            if (Q.endsWith(key, 'Editor')) {
                key = key.substr(0, key.length - 6);
            }
            return key;
        };
        LookupEditorBase.prototype.getLookup = function () {
            return Q.getLookup(this.getLookupKey());
        };
        LookupEditorBase.prototype.getLookupAsync = function () {
            var _this = this;
            return Promise.resolve().then(function () {
                var key = _this.getLookupKey();
                return Q.getLookupAsync(key);
            }, null);
        };
        LookupEditorBase.prototype.getItems = function (lookup) {
            return this.filterItems(this.cascadeItems(lookup.items));
        };
        LookupEditorBase.prototype.getItemText = function (item, lookup) {
            var textValue = lookup.textFormatter ? lookup.textFormatter(item) : item[lookup.textField];
            return textValue == null ? '' : textValue.toString();
        };
        LookupEditorBase.prototype.getItemDisabled = function (item, lookup) {
            return false;
        };
        LookupEditorBase.prototype.updateItems = function () {
            var lookup = this.getLookup();
            this.clearItems();
            var items = this.getItems(lookup);
            for (var $t1 = 0; $t1 < items.length; $t1++) {
                var item = items[$t1];
                var text = this.getItemText(item, lookup);
                var disabled = this.getItemDisabled(item, lookup);
                var idValue = item[lookup.idField];
                var id = (idValue == null ? '' : idValue.toString());
                this.addItem({
                    id: id,
                    text: text,
                    source: item,
                    disabled: disabled
                });
            }
        };
        LookupEditorBase.prototype.updateItemsAsync = function () {
            var _this = this;
            return this.getLookupAsync().then(function (lookup) {
                _this.clearItems();
                var items = _this.getItems(lookup);
                for (var $t1 = 0; $t1 < items.length; $t1++) {
                    var item = items[$t1];
                    var text = _this.getItemText(item, lookup);
                    var disabled = _this.getItemDisabled(item, lookup);
                    var idValue = item[lookup.idField];
                    var id = (idValue == null ? '' : idValue.toString());
                    _this.addItem({ id: id, text: text, source: item, disabled: disabled });
                }
            }, null);
        };
        LookupEditorBase.prototype.getDialogTypeKey = function () {
            if (this.options.dialogType != null) {
                return this.options.dialogType;
            }
            return this.getLookupKey();
        };
        LookupEditorBase.prototype.createEditDialog = function (callback) {
            var dialogTypeKey = this.getDialogTypeKey();
            var dialogType = Serenity.DialogTypeRegistry.get(dialogTypeKey);
            Serenity.Widget.create({
                type: dialogType,
                init: function (x) { return callback(x); }
            });
        };
        LookupEditorBase.prototype.initNewEntity = function (entity) {
            if (!Q.isEmptyOrNull(this.get_cascadeField())) {
                entity[this.get_cascadeField()] = this.get_cascadeValue();
            }
            if (!Q.isEmptyOrNull(this.get_filterField())) {
                entity[this.get_filterField()] = this.get_filterValue();
            }
            if (this.onInitNewEntity != null) {
                this.onInitNewEntity(entity);
            }
        };
        LookupEditorBase.prototype.inplaceCreateClick = function (e) {
            var _this = this;
            if (this.get_readOnly()) {
                return;
            }
            var self = this;
            this.createEditDialog(function (dialog) {
                Serenity.SubDialogHelper.bindToDataChange(dialog, _this, function (x, dci) {
                    Q.reloadLookup(_this.getLookupKey());
                    self.updateItems();
                    _this.lastCreateTerm = null;
                    if ((dci.type === 'create' || dci.type === 'update') &&
                        dci.entityId != null) {
                        var id = dci.entityId.toString();
                        if (_this.multiple) {
                            var values = self.get_values().slice();
                            if (values.indexOf(id) < 0) {
                                values.push(id);
                            }
                            self.set_values(null);
                            self.set_values(values.slice());
                        }
                        else {
                            self.set_value(null);
                            self.set_value(id);
                        }
                    }
                    else if (_this.multiple && dci.type === 'delete' &&
                        dci.entityId != null) {
                        var id1 = dci.entityId.toString();
                        var values1 = self.get_values().slice();
                        var idx1 = values1.indexOf(id1);
                        if (idx1 >= 0)
                            values1.splice(idx1, 1);
                        self.set_values(values1.slice());
                    }
                    else if (!_this.multiple) {
                        self.set_value(null);
                    }
                }, true);
                var editItem = e['editItem'];
                if (editItem != null) {
                    dialog.load(editItem, function () {
                        dialog.dialogOpen(_this.openDialogAsPanel);
                    }, null);
                }
                else if (_this.multiple || Q.isEmptyOrNull(_this.get_value())) {
                    var entity = {};
                    entity[_this.getLookup().textField] = Q.trimToEmpty(_this.lastCreateTerm);
                    _this.initNewEntity(entity);
                    dialog.load(entity, function () {
                        dialog.dialogOpen(_this.openDialogAsPanel);
                    }, null);
                }
                else {
                    dialog.load(_this.get_value(), function () {
                        dialog.dialogOpen(_this.openDialogAsPanel);
                    }, null);
                }
            });
        };
        LookupEditorBase.prototype.cascadeItems = function (items) {
            var val = this.get_cascadeValue();
            if (val == null || val === '') {
                if (!Q.isEmptyOrNull(this.get_cascadeField())) {
                    return [];
                }
                return items;
            }
            var key = val.toString();
            var fld = this.get_cascadeField();
            return items.filter(function (x) {
                var itemKey = Q.coalesce(x[fld], Serenity.ReflectionUtils.getPropertyValue(x, fld));
                return !!(itemKey != null && itemKey.toString() === key);
            });
        };
        LookupEditorBase.prototype.filterItems = function (items) {
            var val = this.get_filterValue();
            if (val == null || val === '') {
                return items;
            }
            var key = val.toString();
            var fld = this.get_filterField();
            return items.filter(function (x) {
                var itemKey = Q.coalesce(x[fld], Serenity.ReflectionUtils.getPropertyValue(x, fld));
                return !!(itemKey != null && itemKey.toString() === key);
            });
        };
        LookupEditorBase.prototype.getCascadeFromValue = function (parent) {
            return Serenity.EditorUtils.getValue(parent);
        };
        LookupEditorBase.prototype.setCascadeFrom = function (value) {
            var _this = this;
            if (Q.isEmptyOrNull(value)) {
                if (this.cascadeLink != null) {
                    this.cascadeLink.set_parentID(null);
                    this.cascadeLink = null;
                }
                this.options.cascadeFrom = null;
                return;
            }
            this.cascadeLink = new Serenity.CascadedWidgetLink(Serenity.Widget, this, function (p) {
                _this.set_cascadeValue(_this.getCascadeFromValue(p));
            });
            this.cascadeLink.set_parentID(value);
            this.options.cascadeFrom = value;
        };
        LookupEditorBase.prototype.isAutoComplete = function () {
            return this.options != null && this.options.autoComplete;
        };
        LookupEditorBase.prototype.getSelect2Options = function () {
            var opt = _super.prototype.getSelect2Options.call(this);
            if (this.options.minimumResultsForSearch != null)
                opt.minimumResultsForSearch = this.options.minimumResultsForSearch;
            if (this.options.autoComplete)
                opt.createSearchChoice = this.getCreateSearchChoice(null);
            else if (this.options.inplaceAdd && (this.options.inplaceAddPermission == null ||
                Q.Authorization.hasPermission(this.options.inplaceAddPermission))) {
                opt.createSearchChoice = this.getCreateSearchChoice(null);
            }
            if (this.options.multiple)
                opt.multiple = true;
            opt.allowClear = Q.coalesce(this.options.allowClear, true);
            return opt;
        };
        LookupEditorBase.prototype.get_cascadeFrom = function () {
            return this.options.cascadeFrom;
        };
        Object.defineProperty(LookupEditorBase.prototype, "cascadeFrom", {
            get: function () {
                return this.get_cascadeFrom();
            },
            set: function (value) {
                this.set_cascadeFrom(value);
            },
            enumerable: true,
            configurable: true
        });
        LookupEditorBase.prototype.set_cascadeFrom = function (value) {
            if (value !== this.options.cascadeFrom) {
                this.setCascadeFrom(value);
                this.updateItems();
            }
        };
        LookupEditorBase.prototype.get_cascadeField = function () {
            return Q.coalesce(this.options.cascadeField, this.options.cascadeFrom);
        };
        Object.defineProperty(LookupEditorBase.prototype, "cascadeField", {
            get: function () {
                return this.get_cascadeField();
            },
            set: function (value) {
                this.set_cascadeField(value);
            },
            enumerable: true,
            configurable: true
        });
        LookupEditorBase.prototype.set_cascadeField = function (value) {
            this.options.cascadeField = value;
        };
        LookupEditorBase.prototype.get_cascadeValue = function () {
            return this.options.cascadeValue;
        };
        Object.defineProperty(LookupEditorBase.prototype, "cascadeValue", {
            get: function () {
                return this.get_cascadeValue();
            },
            set: function (value) {
                this.set_cascadeValue(value);
            },
            enumerable: true,
            configurable: true
        });
        LookupEditorBase.prototype.set_cascadeValue = function (value) {
            if (this.options.cascadeValue !== value) {
                this.options.cascadeValue = value;
                this.set_value(null);
                this.updateItems();
            }
        };
        LookupEditorBase.prototype.get_filterField = function () {
            return this.options.filterField;
        };
        Object.defineProperty(LookupEditorBase.prototype, "filterField", {
            get: function () {
                return this.get_filterField();
            },
            set: function (value) {
                this.set_filterField(value);
            },
            enumerable: true,
            configurable: true
        });
        LookupEditorBase.prototype.set_filterField = function (value) {
            this.options.filterField = value;
        };
        LookupEditorBase.prototype.get_filterValue = function () {
            return this.options.filterValue;
        };
        Object.defineProperty(LookupEditorBase.prototype, "filterValue", {
            get: function () {
                return this.get_filterValue();
            },
            set: function (value) {
                this.set_filterValue(value);
            },
            enumerable: true,
            configurable: true
        });
        LookupEditorBase.prototype.set_filterValue = function (value) {
            if (this.options.filterValue !== value) {
                this.options.filterValue = value;
                this.set_value(null);
                this.updateItems();
            }
        };
        LookupEditorBase = __decorate([
            Serenity.Decorators.registerEditor("Serenity.LookupEditorBase")
        ], LookupEditorBase);
        return LookupEditorBase;
    }(Serenity.Select2Editor));
    Serenity.LookupEditorBase = LookupEditorBase;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var LookupEditor = /** @class */ (function (_super) {
        __extends(LookupEditor, _super);
        function LookupEditor(hidden, opt) {
            return _super.call(this, hidden, opt) || this;
        }
        return LookupEditor;
    }(Serenity.LookupEditorBase));
    Serenity.LookupEditor = LookupEditor;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    // http://digitalbush.com/projects/masked-input-plugin/
    var MaskedEditor = /** @class */ (function (_super) {
        __extends(MaskedEditor, _super);
        function MaskedEditor(input, opt) {
            var _this = _super.call(this, input, opt) || this;
            input.mask(_this.options.mask || '', {
                placeholder: Q.coalesce(_this.options.placeholder, '_')
            });
            return _this;
        }
        Object.defineProperty(MaskedEditor.prototype, "value", {
            get: function () {
                this.element.triggerHandler("blur.mask");
                return this.element.val();
            },
            set: function (value) {
                this.element.val(value);
            },
            enumerable: true,
            configurable: true
        });
        MaskedEditor.prototype.get_value = function () {
            return this.value;
        };
        MaskedEditor.prototype.set_value = function (value) {
            this.value = value;
        };
        MaskedEditor = __decorate([
            Serenity.Decorators.element("<input type=\"text\"/>"),
            Serenity.Decorators.registerEditor([Serenity.IStringValue])
        ], MaskedEditor);
        return MaskedEditor;
    }(Serenity.Widget));
    Serenity.MaskedEditor = MaskedEditor;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var StringEditor = /** @class */ (function (_super) {
        __extends(StringEditor, _super);
        function StringEditor(input) {
            return _super.call(this, input) || this;
        }
        Object.defineProperty(StringEditor.prototype, "value", {
            get: function () {
                return this.element.val();
            },
            set: function (value) {
                this.element.val(value);
            },
            enumerable: true,
            configurable: true
        });
        StringEditor.prototype.get_value = function () {
            return this.value;
        };
        StringEditor.prototype.set_value = function (value) {
            this.value = value;
        };
        StringEditor = __decorate([
            Serenity.Decorators.element("<input type=\"text\"/>"),
            Serenity.Decorators.registerEditor([Serenity.IStringValue])
        ], StringEditor);
        return StringEditor;
    }(Serenity.Widget));
    Serenity.StringEditor = StringEditor;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var TextAreaEditor = /** @class */ (function (_super) {
        __extends(TextAreaEditor, _super);
        function TextAreaEditor(input, opt) {
            var _this = _super.call(this, input, opt) || this;
            if (_this.options.cols !== 0) {
                input.attr('cols', Q.coalesce(_this.options.cols, 80));
            }
            if (_this.options.rows !== 0) {
                input.attr('rows', Q.coalesce(_this.options.rows, 6));
            }
            return _this;
        }
        Object.defineProperty(TextAreaEditor.prototype, "value", {
            get: function () {
                return this.element.val();
            },
            set: function (value) {
                this.element.val(value);
            },
            enumerable: true,
            configurable: true
        });
        TextAreaEditor.prototype.get_value = function () {
            return this.value;
        };
        TextAreaEditor.prototype.set_value = function (value) {
            this.value = value;
        };
        TextAreaEditor = __decorate([
            Serenity.Decorators.element("<textarea />"),
            Serenity.Decorators.registerEditor([Serenity.IStringValue])
        ], TextAreaEditor);
        return TextAreaEditor;
    }(Serenity.Widget));
    Serenity.TextAreaEditor = TextAreaEditor;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var TimeEditor = /** @class */ (function (_super) {
        __extends(TimeEditor, _super);
        function TimeEditor(input, opt) {
            var _this = _super.call(this, input, opt) || this;
            input.addClass('editor s-TimeEditor hour');
            if (!_this.options.noEmptyOption) {
                Q.addOption(input, '', '--');
            }
            for (var h = (_this.options.startHour || 0); h <= (_this.options.endHour || 23); h++) {
                Q.addOption(input, h.toString(), ((h < 10) ? ('0' + h) : h.toString()));
            }
            _this.minutes = $('<select/>').addClass('editor s-TimeEditor minute').insertAfter(input);
            for (var m = 0; m <= 59; m += (_this.options.intervalMinutes || 5)) {
                Q.addOption(_this.minutes, m.toString(), ((m < 10) ? ('0' + m) : m.toString()));
            }
            return _this;
        }
        Object.defineProperty(TimeEditor.prototype, "value", {
            get: function () {
                var hour = Q.toId(this.element.val());
                var minute = Q.toId(this.minutes.val());
                if (hour == null || minute == null) {
                    return null;
                }
                return hour * 60 + minute;
            },
            set: function (value) {
                if (!value) {
                    if (this.options.noEmptyOption) {
                        this.element.val(this.options.startHour);
                        this.minutes.val('0');
                    }
                    else {
                        this.element.val('');
                        this.minutes.val('0');
                    }
                }
                else {
                    this.element.val(Math.floor(value / 60).toString());
                    this.minutes.val(value % 60);
                }
            },
            enumerable: true,
            configurable: true
        });
        TimeEditor.prototype.get_value = function () {
            return this.value;
        };
        TimeEditor.prototype.set_value = function (value) {
            this.value = value;
        };
        TimeEditor = __decorate([
            Serenity.Decorators.element("<select />"),
            Serenity.Decorators.registerEditor([Serenity.IDoubleValue])
        ], TimeEditor);
        return TimeEditor;
    }(Serenity.Widget));
    Serenity.TimeEditor = TimeEditor;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var URLEditor = /** @class */ (function (_super) {
        __extends(URLEditor, _super);
        function URLEditor(input) {
            var _this = _super.call(this, input) || this;
            input.addClass("url").attr("title", "URL should be entered in format: 'http://www.site.com/page'.");
            input.on("blur." + _this.uniqueName, function (e) {
                var validator = input.closest("form").data("validator");
                if (validator == null)
                    return;
                if (!input.hasClass("error"))
                    return;
                var value = Q.trimToNull(input.val());
                if (!value)
                    return;
                value = "http://" + value;
                if ($.validator.methods['url'].call(validator, value, input[0]) == true) {
                    input.val(value);
                    validator.element(input);
                }
            });
            return _this;
        }
        URLEditor = __decorate([
            Serenity.Decorators.registerEditor([Serenity.IStringValue])
        ], URLEditor);
        return URLEditor;
    }(Serenity.StringEditor));
    Serenity.URLEditor = URLEditor;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var IFiltering = /** @class */ (function () {
        function IFiltering() {
        }
        IFiltering = __decorate([
            Serenity.Decorators.registerInterface('Serenity.IQuickFiltering')
        ], IFiltering);
        return IFiltering;
    }());
    Serenity.IFiltering = IFiltering;
    var IQuickFiltering = /** @class */ (function () {
        function IQuickFiltering() {
        }
        IQuickFiltering = __decorate([
            Serenity.Decorators.registerInterface('Serenity.IQuickFiltering')
        ], IQuickFiltering);
        return IQuickFiltering;
    }());
    Serenity.IQuickFiltering = IQuickFiltering;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var BaseFiltering = /** @class */ (function () {
        function BaseFiltering() {
        }
        BaseFiltering.prototype.get_field = function () {
            return this.field;
        };
        BaseFiltering.prototype.set_field = function (value) {
            this.field = value;
        };
        BaseFiltering.prototype.get_container = function () {
            return this.container;
        };
        BaseFiltering.prototype.set_container = function (value) {
            this.container = value;
        };
        BaseFiltering.prototype.get_operator = function () {
            return this.operator;
        };
        BaseFiltering.prototype.set_operator = function (value) {
            this.operator = value;
        };
        BaseFiltering.prototype.appendNullableOperators = function (list) {
            if (!this.isNullable()) {
                return list;
            }
            list.push({ key: Serenity.FilterOperators.isNotNull });
            list.push({ key: Serenity.FilterOperators.isNull });
            return list;
        };
        BaseFiltering.prototype.appendComparisonOperators = function (list) {
            list.push({ key: Serenity.FilterOperators.EQ });
            list.push({ key: Serenity.FilterOperators.NE });
            list.push({ key: Serenity.FilterOperators.LT });
            list.push({ key: Serenity.FilterOperators.LE });
            list.push({ key: Serenity.FilterOperators.GT });
            list.push({ key: Serenity.FilterOperators.GE });
            return list;
        };
        BaseFiltering.prototype.isNullable = function () {
            return this.get_field().required !== true;
        };
        BaseFiltering.prototype.createEditor = function () {
            switch (this.get_operator().key) {
                case 'true':
                case 'false':
                case 'isnull':
                case 'isnotnull': {
                    return;
                }
                case 'contains':
                case 'startswith':
                case 'eq':
                case 'ne':
                case 'lt':
                case 'le':
                case 'gt':
                case 'ge': {
                    this.get_container().html('<input type="text"/>');
                    return;
                }
            }
            throw new ss.Exception(Q.format("Filtering '{0}' has no editor for '{1}' operator", ss.getTypeName(ss.getInstanceType(this)), this.get_operator().key));
        };
        BaseFiltering.prototype.operatorFormat = function (op) {
            return Q.coalesce(op.format, Q.coalesce(Q.tryGetText('Controls.FilterPanel.OperatorFormats.' + op.key), op.key));
        };
        BaseFiltering.prototype.getTitle = function (field) {
            return Q.coalesce(Q.tryGetText(field.title), Q.coalesce(field.title, field.name));
        };
        BaseFiltering.prototype.displayText = function (op, values) {
            if (!values || values.length === 0) {
                return Q.format(this.operatorFormat(op), this.getTitle(this.field));
            }
            else if (values.length === 1) {
                return Q.format(this.operatorFormat(op), this.getTitle(this.field), values[0]);
            }
            else {
                return Q.format(this.operatorFormat(op), this.getTitle(this.field), values[0], values[1]);
            }
        };
        BaseFiltering.prototype.getCriteriaField = function () {
            return this.field.name;
        };
        BaseFiltering.prototype.getCriteria = function () {
            var result = {};
            var text;
            switch (this.get_operator().key) {
                case 'true': {
                    result.displayText = this.displayText(this.get_operator(), []);
                    result.criteria = [[this.getCriteriaField()], '=', true];
                    return result;
                }
                case 'false': {
                    result.displayText = this.displayText(this.get_operator(), []);
                    result.criteria = [[this.getCriteriaField()], '=', false];
                    return result;
                }
                case 'isnull': {
                    result.displayText = this.displayText(this.get_operator(), []);
                    result.criteria = ['is null', [this.getCriteriaField()]];
                    return result;
                }
                case 'isnotnull': {
                    result.displayText = this.displayText(this.get_operator(), []);
                    result.criteria = ['is not null', [this.getCriteriaField()]];
                    return result;
                }
                case 'contains': {
                    text = this.getEditorText();
                    result.displayText = this.displayText(this.get_operator(), [text]);
                    result.criteria = [[this.getCriteriaField()], 'like', '%' + text + '%'];
                }
                case 'startswith': {
                    text = this.getEditorText();
                    result.displayText = this.displayText(this.get_operator(), [text]);
                    result.criteria = [[this.getCriteriaField()], 'like', text + '%'];
                    return result;
                }
                case 'eq':
                case 'ne':
                case 'lt':
                case 'le':
                case 'gt':
                case 'ge': {
                    text = this.getEditorText();
                    result.displayText = this.displayText(this.get_operator(), [text]);
                    result.criteria = [[this.getCriteriaField()], Serenity.FilterOperators.toCriteriaOperator[this.get_operator().key], this.getEditorValue()];
                    return result;
                }
            }
            throw new ss.Exception(Q.format("Filtering '{0}' has no handler for '{1}' operator", ss.getTypeName(ss.getInstanceType(this)), this.get_operator().key));
        };
        BaseFiltering.prototype.loadState = function (state) {
            var input = this.get_container().find(':input').first();
            input.val(state);
        };
        BaseFiltering.prototype.saveState = function () {
            switch (this.get_operator().key) {
                case 'contains':
                case 'startswith':
                case 'eq':
                case 'ne':
                case 'lt':
                case 'le':
                case 'gt':
                case 'ge': {
                    var input = this.get_container().find(':input').first();
                    return input.val();
                }
            }
            return null;
        };
        BaseFiltering.prototype.argumentNull = function () {
            return new ss.ArgumentNullException('value', Q.text('Controls.FilterPanel.ValueRequired'));
        };
        BaseFiltering.prototype.validateEditorValue = function (value) {
            if (value.length === 0) {
                throw this.argumentNull();
            }
            return value;
        };
        BaseFiltering.prototype.getEditorValue = function () {
            var input = this.get_container().find(':input').not('.select2-focusser').first();
            if (input.length !== 1) {
                throw new ss.Exception(Q.format("Couldn't find input in filter container for {0}", Q.coalesce(this.field.title, this.field.name)));
            }
            var value;
            if (input.data('select2') != null) {
                value = input.select2('val');
            }
            else {
                value = input.val();
            }
            value = Q.coalesce(value, '').trim();
            return this.validateEditorValue(value);
        };
        BaseFiltering.prototype.getEditorText = function () {
            var input = this.get_container().find(':input').not('.select2-focusser').not('.select2-input').first();
            if (input.length === 0) {
                return this.get_container().text().trim();
            }
            var value;
            if (input.data('select2') != null) {
                value = Q.coalesce(input.select2('data'), {}).text;
            }
            else {
                value = input.val();
            }
            return value;
        };
        BaseFiltering.prototype.initQuickFilter = function (filter) {
            filter.field = this.getCriteriaField();
            filter.type = Serenity.StringEditor;
            filter.title = this.getTitle(this.field);
            filter.options = Q.deepClone({}, this.get_field().quickFilterParams);
        };
        BaseFiltering = __decorate([
            Serenity.Decorators.registerClass('Serenity.BaseFiltering', [Serenity.IFiltering, Serenity.IQuickFiltering])
        ], BaseFiltering);
        return BaseFiltering;
    }());
    Serenity.BaseFiltering = BaseFiltering;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var BaseEditorFiltering = /** @class */ (function (_super) {
        __extends(BaseEditorFiltering, _super);
        function BaseEditorFiltering(editorType) {
            var _this = _super.call(this) || this;
            _this.editorType = editorType;
            return _this;
        }
        BaseEditorFiltering.prototype.useEditor = function () {
            switch (this.get_operator().key) {
                case 'eq':
                case 'ne':
                case 'lt':
                case 'le':
                case 'gt':
                case 'ge':
                    return true;
            }
            return false;
        };
        BaseEditorFiltering.prototype.createEditor = function () {
            if (this.useEditor()) {
                this.editor = Serenity.Widget.create({
                    type: this.editorType,
                    container: this.get_container(),
                    options: this.getEditorOptions(),
                    init: null
                });
                return;
            }
            this.editor = null;
            _super.prototype.createEditor.call(this);
        };
        BaseEditorFiltering.prototype.useIdField = function () {
            return false;
        };
        BaseEditorFiltering.prototype.getCriteriaField = function () {
            if (this.useEditor() &&
                this.useIdField() &&
                !Q.isEmptyOrNull(this.get_field().filteringIdField)) {
                return this.get_field().filteringIdField;
            }
            return _super.prototype.getCriteriaField.call(this);
        };
        BaseEditorFiltering.prototype.getEditorOptions = function () {
            var opt = Q.deepClone({}, this.get_field().editorParams);
            delete opt['cascadeFrom'];
            // currently can't support cascadeFrom in filtering
            return Q.deepClone(opt, this.get_field().filteringParams);
        };
        BaseEditorFiltering.prototype.loadState = function (state) {
            if (this.useEditor()) {
                if (state == null) {
                    return;
                }
                Serenity.EditorUtils.setValue(this.editor, state);
                return;
            }
            _super.prototype.loadState.call(this, state);
        };
        BaseEditorFiltering.prototype.saveState = function () {
            if (this.useEditor()) {
                return Serenity.EditorUtils.getValue(this.editor);
            }
            return _super.prototype.saveState.call(this);
        };
        BaseEditorFiltering.prototype.getEditorValue = function () {
            if (this.useEditor()) {
                var value = Serenity.EditorUtils.getValue(this.editor);
                if (value == null || (typeof value == "string" && value.trim().length === 0))
                    throw this.argumentNull();
                return value;
            }
            return _super.prototype.getEditorValue.call(this);
        };
        BaseEditorFiltering.prototype.initQuickFilter = function (filter) {
            _super.prototype.initQuickFilter.call(this, filter);
            filter.type = this.editorType;
            filter.options = Q.deepClone({}, this.getEditorOptions(), this.get_field().quickFilterParams);
        };
        BaseEditorFiltering = __decorate([
            Serenity.Decorators.registerClass('Serenity.BaseEditorFiltering')
        ], BaseEditorFiltering);
        return BaseEditorFiltering;
    }(Serenity.BaseFiltering));
    Serenity.BaseEditorFiltering = BaseEditorFiltering;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var TemplatedDialog = /** @class */ (function (_super) {
        __extends(TemplatedDialog, _super);
        function TemplatedDialog(options) {
            var _this = _super.call(this, Q.newBodyDiv().addClass('s-TemplatedDialog hidden'), options) || this;
            _this.element.attr("id", _this.uniqueName);
            _this.initValidator();
            _this.initTabs();
            _this.initToolbar();
            return _this;
        }
        TemplatedDialog_1 = TemplatedDialog;
        Object.defineProperty(TemplatedDialog.prototype, "isMarkedAsPanel", {
            get: function () {
                var panelAttr = ss.getAttributes(ss.getInstanceType(this), Serenity.PanelAttribute, true);
                return panelAttr.length > 0 && panelAttr[panelAttr.length - 1].value !== false;
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(TemplatedDialog.prototype, "isResponsive", {
            get: function () {
                return Q.Config.responsiveDialogs ||
                    ss.getAttributes(ss.getInstanceType(this), Serenity.ResponsiveAttribute, true).length > 0;
            },
            enumerable: true,
            configurable: true
        });
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
                size = TemplatedDialog_1.getCssSize(sizeHelper, 'minWidth');
                if (size != null)
                    opt.minWidth = size;
                size = TemplatedDialog_1.getCssSize(sizeHelper, 'width');
                if (size != null)
                    opt.width = size;
                size = TemplatedDialog_1.getCssSize(sizeHelper, 'height');
                if (size != null)
                    opt.height = size;
                size = TemplatedDialog_1.getCssSize(sizeHelper, 'minHeight');
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
            if (this.element != null &&
                this.element.hasClass('ui-dialog-content')) {
                this.element.dialog('destroy');
                this.element.removeClass('ui-dialog-content');
            }
            $(window).unbind('.' + this.uniqueName);
            _super.prototype.destroy.call(this);
        };
        TemplatedDialog.prototype.initDialog = function () {
            var _this = this;
            if (this.element.hasClass('ui-dialog-content'))
                return;
            this.element.removeClass('hidden');
            this.element.dialog(this.getDialogOptions());
            this.element.closest('.ui-dialog').on('resize', function (e) { return _this.arrange(); });
            var type = ss.getInstanceType(this);
            if (this.isResponsive) {
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
                if (_this.isResponsive) {
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
        TemplatedDialog.prototype.dialogOpen = function (asPanel) {
            var _this = this;
            asPanel = Q.coalesce(asPanel, this.isMarkedAsPanel);
            if (asPanel) {
                if (!this.element.hasClass('s-Panel')) {
                    // so that panel title is created if needed
                    this.element.on('panelopen.' + this.uniqueName, function () {
                        _this.onDialogOpen();
                    });
                    this.element.on('panelclose.' + this.uniqueName, function () {
                        _this.onDialogClose();
                    });
                }
                TemplatedDialog_1.openPanel(this.element, this.uniqueName);
                this.setupPanelTitle();
            }
            else {
                if (!this.element.hasClass('ui-dialog-content'))
                    this.initDialog();
                this.element.dialog('open');
            }
        };
        TemplatedDialog.openPanel = function (element, uniqueName) {
            var container = $('.panels-container');
            if (!container.length)
                container = $('section.content');
            element.data('paneluniquename', uniqueName);
            if (container.length) {
                container = container.last();
                container.children()
                    .not(element)
                    .not('.panel-hidden')
                    .addClass('panel-hidden panel-hidden-' + uniqueName);
                if (element[0].parentElement !== container[0])
                    element.appendTo(container);
            }
            $('.ui-dialog:visible, .ui-widget-overlay:visible')
                .not(element)
                .addClass('panel-hidden panel-hidden-' + uniqueName);
            element
                .removeClass('hidden')
                .removeClass('panel-hidden')
                .addClass('s-Panel')
                .trigger('panelopen');
        };
        TemplatedDialog.closePanel = function (element, e) {
            if (!element.hasClass('s-Panel') || element.hasClass('hidden'))
                return;
            var query = $.Event(e);
            query.type = 'panelbeforeclose';
            query.target = element[0];
            element.trigger(query);
            if (query.isDefaultPrevented())
                return;
            element.addClass('hidden');
            var uniqueName = element.data('paneluniquename') || new Date().getTime();
            var klass = 'panel-hidden-' + uniqueName;
            $('.' + klass).removeClass(klass).removeClass('panel-hidden');
            $(window).triggerHandler('resize');
            $('.require-layout:visible').triggerHandler('layout');
            var e = $.Event(e);
            e.type = 'panelclose';
            e.target = element[0];
            element.trigger(e);
        };
        TemplatedDialog.prototype.onDialogOpen = function () {
            if (!$(document.body).hasClass('mobile-device'))
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
            if (this.isMarkedAsPanel) {
                _super.prototype.addCssClass.call(this);
                if (this.isResponsive)
                    this.element.addClass("flex-layout");
            }
        };
        TemplatedDialog.prototype.getDialogOptions = function () {
            var opt = {};
            var dialogClass = 's-Dialog ' + this.getCssClass();
            opt.dialogClass = dialogClass;
            opt.width = 920;
            TemplatedDialog_1.applyCssSizes(opt, dialogClass);
            opt.autoOpen = false;
            var type = ss.getInstanceType(this);
            opt.resizable = ss.getAttributes(type, Serenity.ResizableAttribute, true).length > 0;
            opt.modal = true;
            opt.position = { my: 'center', at: 'center', of: $(window.window) };
            opt.title = Q.coalesce(this.element.data('dialogtitle'), this.getDialogTitle()) || '';
            return opt;
        };
        TemplatedDialog.prototype.getDialogTitle = function () {
            return "";
        };
        TemplatedDialog.prototype.dialogClose = function () {
            if (this.element.hasClass('ui-dialog-content'))
                this.element.dialog().dialog('close');
            else if (this.element.hasClass('s-Panel') && !this.element.hasClass('hidden')) {
                TemplatedDialog_1.closePanel(this.element);
            }
        };
        Object.defineProperty(TemplatedDialog.prototype, "dialogTitle", {
            get: function () {
                if (this.element.hasClass('ui-dialog-content'))
                    return this.element.dialog('option', 'title');
                return this.element.data('dialogtitle');
            },
            set: function (value) {
                var oldTitle = this.dialogTitle;
                this.element.data('dialogtitle', value);
                if (this.element.hasClass('ui-dialog-content'))
                    this.element.dialog('option', 'title', value);
                else if (this.element.hasClass('s-Panel')) {
                    if (oldTitle != this.dialogTitle) {
                        this.setupPanelTitle();
                        this.arrange();
                    }
                }
            },
            enumerable: true,
            configurable: true
        });
        TemplatedDialog.prototype.setupPanelTitle = function () {
            var _this = this;
            var value = Q.coalesce(this.dialogTitle, this.getDialogTitle());
            var pt = this.element.children('.panel-titlebar');
            if (Q.isEmptyOrNull(value)) {
                pt.remove();
            }
            else {
                if (!this.element.children('.panel-titlebar').length) {
                    pt = $("<div class='panel-titlebar'><div class='panel-titlebar-text'></div></div>")
                        .prependTo(this.element);
                }
                pt.children('.panel-titlebar-text').text(value);
                if (this.element.hasClass('s-Panel')) {
                    if (!pt.children('.panel-titlebar-close').length) {
                        $('<button class="panel-titlebar-close">&nbsp;</button>')
                            .prependTo(pt)
                            .click(function (e) {
                            TemplatedDialog_1.closePanel(_this.element, e);
                        });
                    }
                }
            }
        };
        TemplatedDialog.prototype.set_dialogTitle = function (value) {
            this.dialogTitle = value;
        };
        TemplatedDialog.prototype.initTabs = function () {
            var _this = this;
            var tabsDiv = this.byId('Tabs');
            if (tabsDiv.length === 0) {
                return;
            }
            this.tabs = tabsDiv.tabs({});
            this.tabs.bind('tabsactivate', function () { return _this.arrange(); });
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
                    data.position = dlg.css('position');
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
                uiDialog.css({ left: '0px', top: '0px', width: $(window).width() + 'px', height: $(window).height() + 'px', position: 'fixed' });
                $(document.body).scrollTop(0);
                Q.layoutFillHeight(this.element);
            }
            else {
                var d = this.element.data('responsiveData');
                if (d) {
                    dlg.dialog('option', 'draggable', d.draggable);
                    dlg.dialog('option', 'resizable', d.resizable);
                    this.element.closest('.ui-dialog').css({ left: '0px', top: '0px', width: d.width + 'px', height: d.height + 'px', position: d.position });
                    this.element.height(d.contentHeight);
                    uiDialog.removeClass('mobile-layout');
                    this.element.removeData('responsiveData');
                }
            }
        };
        TemplatedDialog = TemplatedDialog_1 = __decorate([
            Serenity.Decorators.registerClass([Serenity.IDialog])
        ], TemplatedDialog);
        return TemplatedDialog;
        var TemplatedDialog_1;
    }(Serenity.TemplatedWidget));
    Serenity.TemplatedDialog = TemplatedDialog;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var IDataGrid = /** @class */ (function () {
        function IDataGrid() {
        }
        IDataGrid = __decorate([
            Serenity.Decorators.registerInterface('Serenity.IDataGrid')
        ], IDataGrid);
        return IDataGrid;
    }());
    Serenity.IDataGrid = IDataGrid;
})(Serenity || (Serenity = {}));
(function (Serenity) {
    var DataGrid = /** @class */ (function (_super) {
        __extends(DataGrid, _super);
        function DataGrid(container, options) {
            var _this = _super.call(this, container, options) || this;
            _this.restoringSettings = 0;
            var self = _this;
            _this.element.addClass('s-DataGrid').html('');
            _this.element.addClass('s-' + ss.getTypeName(ss.getInstanceType(_this)));
            _this.element.addClass('require-layout').bind('layout.' + _this.uniqueName, function () {
                self.layout();
            });
            _this.setTitle(_this.getInitialTitle());
            var buttons = _this.getButtons();
            if (buttons != null) {
                _this.createToolbar(buttons);
            }
            _this.slickContainer = _this.createSlickContainer();
            _this.view = _this.createView();
            _this.slickGrid = _this.createSlickGrid();
            if (_this.enableFiltering()) {
                _this.createFilterBar();
            }
            if (_this.usePager()) {
                _this.createPager();
            }
            _this.bindToSlickEvents();
            _this.bindToViewEvents();
            if (buttons != null) {
                _this.createToolbarExtensions();
            }
            _this.createQuickFilters();
            _this.updateDisabledState();
            if (!_this.isAsyncWidget()) {
                _this.initialSettings = _this.getCurrentSettings(null);
                _this.restoreSettings(null, null);
                window.setTimeout(function () { return _this.initialPopulate(); }, 0);
            }
            return _this;
        }
        DataGrid.prototype.add_submitHandlers = function (action) {
            this.submitHandlers = ss.delegateCombine(this.submitHandlers, action);
        };
        DataGrid.prototype.remove_submitHandlers = function (action) {
            this.submitHandlers = ss.delegateRemove(this.submitHandlers, action);
        };
        DataGrid.prototype.layout = function () {
            if (!this.element.is(':visible')) {
                return;
            }
            if (this.slickContainer == null) {
                return;
            }
            Q.layoutFillHeight(this.slickContainer);
            if (this.element.hasClass('responsive-height')) {
                if (this.slickGrid != null && this.slickGrid.getOptions().autoHeight) {
                    this.slickContainer.children('.slick-viewport').css('height', 'auto');
                    this.slickGrid.setOptions({ autoHeight: false });
                }
                if (this.slickGrid != null && (this.slickContainer.height() < 200 || $(window.window).width() < 768)) {
                    this.element.css('height', 'auto');
                    this.slickContainer.css('height', 'auto').children('.slick-viewport').css('height', 'auto');
                    this.slickGrid.setOptions({ autoHeight: true });
                }
            }
            if (this.slickGrid != null) {
                this.slickGrid.resizeCanvas();
                this.slickGrid.invalidate();
            }
        };
        DataGrid.prototype.getInitialTitle = function () {
            return null;
        };
        DataGrid.prototype.createToolbarExtensions = function () {
        };
        DataGrid.prototype.createQuickFilters = function () {
            var filters = this.getQuickFilters();
            for (var f = 0; f < filters.length; f++) {
                var filter = filters[f];
                this.addQuickFilter(filter);
            }
        };
        DataGrid.prototype.getQuickFilters = function () {
            var list = [];
            var columns = this.allColumns.filter(function (x) {
                return x.sourceItem && x.sourceItem.quickFilter === true;
            });
            for (var _i = 0, columns_1 = columns; _i < columns_1.length; _i++) {
                var column = columns_1[_i];
                var item = column.sourceItem;
                var quick = {};
                var name = item.name;
                var title = Q.tryGetText(item.title);
                if (title == null) {
                    title = item.title;
                    if (title == null) {
                        title = name;
                    }
                }
                var filteringType = Serenity.FilteringTypeRegistry.get(Q.coalesce(item.filteringType, 'String'));
                if (filteringType === Serenity.DateFiltering) {
                    quick = this.dateRangeQuickFilter(name, title);
                }
                else if (filteringType === Serenity.DateTimeFiltering) {
                    quick = this.dateTimeRangeQuickFilter(name, title);
                }
                else if (filteringType === Serenity.BooleanFiltering) {
                    var q = item.quickFilterParams || {};
                    var f = item.filteringParams || {};
                    var trueText = q['trueText'];
                    if (trueText == null) {
                        trueText = f['trueText'];
                    }
                    var falseText = q['falseText'];
                    if (falseText == null) {
                        falseText = f['falseText'];
                    }
                    quick = this.booleanQuickFilter(name, title, trueText, falseText);
                }
                else {
                    var filtering = new filteringType();
                    if (filtering && ss.isInstanceOfType(filtering, Serenity.IQuickFiltering)) {
                        Serenity.ReflectionOptionsSetter.set(filtering, item.filteringParams);
                        filtering.set_field(item);
                        filtering.set_operator({ key: Serenity.FilterOperators.EQ });
                        filtering.initQuickFilter(quick);
                        quick.options = Q.deepClone(quick.options, item.quickFilterParams);
                    }
                    else {
                        continue;
                    }
                }
                if (!!item.quickFilterSeparator) {
                    quick.separator = true;
                }
                quick.cssClass = item.quickFilterCssClass;
                list.push(quick);
            }
            return list;
        };
        DataGrid.prototype.findQuickFilter = function (type, field) {
            return $('#' + this.uniqueName + '_QuickFilter_' + field).getWidget(type);
        };
        DataGrid.prototype.tryFindQuickFilter = function (type, field) {
            var el = $('#' + this.uniqueName + '_QuickFilter_' + field);
            if (!el.length)
                return null;
            return el.tryGetWidget(type);
        };
        DataGrid.prototype.createIncludeDeletedButton = function () {
            if (!Q.isEmptyOrNull(this.getIsActiveProperty())) {
                Serenity.GridUtils.addIncludeDeletedToggle(this.toolbar.element, this.view, null, false);
            }
        };
        DataGrid.prototype.getQuickSearchFields = function () {
            return null;
        };
        DataGrid.prototype.createQuickSearchInput = function () {
            Serenity.GridUtils.addQuickSearchInput(this.toolbar.element, this.view, this.getQuickSearchFields());
        };
        DataGrid.prototype.destroy = function () {
            this.submitHandlers = null;
            if (this.toolbar) {
                this.toolbar.destroy();
                this.toolbar = null;
            }
            if (this.slickGrid) {
                this.slickGrid.onClick.clear();
                this.slickGrid.onSort.clear();
                this.slickGrid.onColumnsResized.clear();
                this.slickGrid.onColumnsReordered.clear();
                this.slickGrid.destroy();
                this.slickGrid = null;
            }
            if (this.view) {
                this.view.onDataChanged.clear();
                this.view.onSubmit = null;
                this.view.setFilter(null);
                this.view = null;
            }
            this.titleDiv = null;
            _super.prototype.destroy.call(this);
        };
        DataGrid.prototype.getItemCssClass = function (item, index) {
            var activeFieldName = this.getIsActiveProperty();
            if (Q.isEmptyOrNull(activeFieldName)) {
                return null;
            }
            var value = item[activeFieldName];
            if (value == null) {
                return null;
            }
            if (typeof (value) === 'number') {
                if (value < 0) {
                    return 'deleted';
                }
                else if (value === 0) {
                    return 'inactive';
                }
            }
            else if (typeof (value) === 'boolean') {
                if (value === false) {
                    return 'deleted';
                }
            }
            return null;
        };
        DataGrid.prototype.getItemMetadata = function (item, index) {
            var itemClass = this.getItemCssClass(item, index);
            if (Q.isEmptyOrNull(itemClass)) {
                return new Object();
            }
            return { cssClasses: itemClass };
        };
        DataGrid.prototype.postProcessColumns = function (columns) {
            Serenity.SlickHelper.setDefaults(columns, this.getLocalTextDbPrefix());
            return columns;
        };
        DataGrid.prototype.initialPopulate = function () {
            var self = this;
            if (this.populateWhenVisible()) {
                Serenity.LazyLoadHelper.executeEverytimeWhenShown(this.element, function () {
                    self.refreshIfNeeded();
                }, false);
                if (this.element.is(':visible') && this.view) {
                    this.view.populate();
                }
            }
            else if (this.view) {
                this.view.populate();
            }
        };
        DataGrid.prototype.initializeAsync = function () {
            var _this = this;
            return _super.prototype.initializeAsync.call(this)
                .then(function () { return _this.getColumnsAsync(); })
                .then(function (columns) {
                _this.allColumns = columns;
                _this.postProcessColumns(_this.allColumns);
                var self = _this;
                if (_this.filterBar) {
                    _this.filterBar.set_store(new Serenity.FilterStore(_this.allColumns.filter(function (x) {
                        return x.sourceItem && x.sourceItem.notFilterable !== true;
                    }).map(function (x1) {
                        return x1.sourceItem;
                    })));
                    _this.filterBar.get_store().add_changed(function (s, e) {
                        if (_this.restoringSettings <= 0) {
                            self.persistSettings(null);
                            self.refresh();
                        }
                    });
                }
                var visibleColumns = _this.allColumns.filter(function (x2) {
                    return x2.visible !== false;
                });
                if (_this.slickGrid) {
                    _this.slickGrid.setColumns(visibleColumns);
                }
                _this.setInitialSortOrder();
                _this.initialSettings = _this.getCurrentSettings(null);
                _this.restoreSettings(null, null);
                _this.initialPopulate();
            }, null);
        };
        DataGrid.prototype.createSlickGrid = function () {
            var visibleColumns;
            if (this.isAsyncWidget()) {
                visibleColumns = [];
            }
            else {
                this.allColumns = this.getColumns();
                visibleColumns = this.postProcessColumns(this.allColumns).filter(function (x) {
                    return x.visible !== false;
                });
            }
            var slickOptions = this.getSlickOptions();
            var grid = new Slick.Grid(this.slickContainer, this.view, visibleColumns, slickOptions);
            grid.registerPlugin(new Slick.AutoTooltips({
                enableForHeaderCells: true
            }));
            this.slickGrid = grid;
            this.rows = this.slickGrid;
            if (!this.isAsyncWidget()) {
                this.setInitialSortOrder();
            }
            return grid;
        };
        DataGrid.prototype.setInitialSortOrder = function () {
            var sortBy = this.getDefaultSortBy();
            if (this.view) {
                this.view.sortBy = Array.prototype.slice.call(sortBy);
            }
            var mapped = sortBy.map(function (s) {
                var x = {};
                if (s && Q.endsWith(s.toLowerCase(), ' desc')) {
                    x.columnId = ss.trimEndString(s.substr(0, s.length - 5));
                    x.sortAsc = false;
                }
                else {
                    x.columnId = s;
                    x.sortAsc = true;
                }
                return x;
            });
            this.slickGrid.setSortColumns(mapped);
        };
        DataGrid.prototype.itemAt = function (row) {
            return this.slickGrid.getDataItem(row);
        };
        DataGrid.prototype.rowCount = function () {
            return this.slickGrid.getDataLength();
        };
        DataGrid.prototype.getItems = function () {
            return this.view.getItems();
        };
        DataGrid.prototype.setItems = function (value) {
            this.view.setItems(value, true);
        };
        DataGrid.prototype.bindToSlickEvents = function () {
            var _this = this;
            var self = this;
            this.slickGridOnSort = function (e, p) {
                self.view.populateLock();
                try {
                    var sortBy = [];
                    var col;
                    if (!!p.multiColumnSort) {
                        for (var i = 0; !!(i < p.sortCols.length); i++) {
                            var x = p.sortCols[i];
                            col = x.sortCol;
                            if (col == null) {
                                col = {};
                            }
                            sortBy.push(col.field + (!!x.sortAsc ? '' : ' DESC'));
                        }
                    }
                    else {
                        var col = p.sortCol;
                        if (col == null) {
                            col = {};
                        }
                        sortBy.push(col.field + (!!p.sortAsc ? '' : ' DESC'));
                    }
                    self.view.sortBy = sortBy;
                }
                finally {
                    self.view.populateUnlock();
                }
                self.view.populate();
                _this.persistSettings(null);
            };
            this.slickGrid.onSort.subscribe(this.slickGridOnSort);
            this.slickGridOnClick = function (e1, p1) {
                self.onClick(e1, p1.row, p1.cell);
            };
            this.slickGrid.onClick.subscribe(this.slickGridOnClick);
            this.slickGrid.onColumnsReordered.subscribe(function (e2, p2) {
                return _this.persistSettings(null);
            });
            this.slickGrid.onColumnsResized.subscribe(function (e3, p3) {
                return _this.persistSettings(null);
            });
        };
        DataGrid.prototype.getAddButtonCaption = function () {
            return Q.coalesce(Q.tryGetText('Controls.DataGrid.NewButton'), 'New');
        };
        DataGrid.prototype.getButtons = function () {
            return [];
        };
        DataGrid.prototype.editItem = function (entityOrId) {
            throw new ss.NotImplementedException();
        };
        DataGrid.prototype.editItemOfType = function (itemType, entityOrId) {
            if (itemType === this.getItemType()) {
                this.editItem(entityOrId);
                return;
            }
            throw new ss.NotImplementedException();
        };
        DataGrid.prototype.onClick = function (e, row, cell) {
            if (e.isDefaultPrevented()) {
                return;
            }
            var target = $(e.target);
            if (!target.hasClass('s-EditLink')) {
                target = target.closest('a');
            }
            if (target.hasClass('s-EditLink')) {
                e.preventDefault();
                this.editItemOfType(Serenity.SlickFormatting.getItemType(target), Serenity.SlickFormatting.getItemId(target));
            }
        };
        DataGrid.prototype.viewDataChanged = function (e, rows) {
            this.markupReady();
        };
        DataGrid.prototype.bindToViewEvents = function () {
            var self = this;
            this.view.onDataChanged.subscribe(function (e, d) {
                return self.viewDataChanged(e, d);
            });
            this.view.onSubmit = function (view) {
                return self.onViewSubmit();
            };
            this.view.setFilter(function (item, view1) {
                return self.onViewFilter(item);
            });
            this.view.onProcessData = function (response, view2) {
                return self.onViewProcessData(response);
            };
        };
        DataGrid.prototype.onViewProcessData = function (response) {
            return response;
        };
        DataGrid.prototype.onViewFilter = function (item) {
            return true;
        };
        DataGrid.prototype.getIncludeColumns = function (include) {
            var columns = this.slickGrid.getColumns();
            for (var _i = 0, columns_2 = columns; _i < columns_2.length; _i++) {
                var column = columns_2[_i];
                if (column.field) {
                    include[column.field] = true;
                }
                if (column.referencedFields) {
                    for (var _a = 0, _b = column.referencedFields; _a < _b.length; _a++) {
                        var x = _b[_a];
                        include[x] = true;
                    }
                }
            }
        };
        DataGrid.prototype.setCriteriaParameter = function () {
            delete this.view.params['Criteria'];
            if (this.filterBar) {
                var criteria = this.filterBar.get_store().get_activeCriteria();
                if (!Serenity.Criteria.isEmpty(criteria)) {
                    this.view.params.Criteria = criteria;
                }
            }
        };
        DataGrid.prototype.setEquality = function (field, value) {
            Q.setEquality(this.view.params, field, value);
        };
        DataGrid.prototype.setIncludeColumnsParameter = function () {
            var include = {};
            this.getIncludeColumns(include);
            var array = [];
            for (var _i = 0, _a = Object.keys(include); _i < _a.length; _i++) {
                var key = _a[_i];
                array.push(key);
            }
            this.view.params.IncludeColumns = array;
        };
        DataGrid.prototype.onViewSubmit = function () {
            if (this.isDisabled || !this.getGridCanLoad()) {
                return false;
            }
            this.setCriteriaParameter();
            this.setIncludeColumnsParameter();
            this.invokeSubmitHandlers();
            return true;
        };
        DataGrid.prototype.markupReady = function () {
        };
        DataGrid.prototype.createSlickContainer = function () {
            return $('<div class="grid-container"></div>').appendTo(this.element);
        };
        DataGrid.prototype.createView = function () {
            var opt = this.getViewOptions();
            return new Slick.RemoteView(opt);
        };
        DataGrid.prototype.getDefaultSortBy = function () {
            if (this.slickGrid) {
                var columns = this.slickGrid.getColumns().filter(function (x) {
                    return x.sortOrder && x.sortOrder !== 0;
                });
                if (columns.length > 0) {
                    columns.sort(function (x1, y) {
                        return ss.compare(Math.abs(x1.sortOrder), Math.abs(y.sortOrder));
                    });
                    var list = [];
                    for (var i = 0; i < columns.length; i++) {
                        var col = columns[i];
                        list.push(col.field + ((col.sortOrder < 0) ? ' DESC' : ''));
                    }
                    return list;
                }
            }
            return [];
        };
        DataGrid.prototype.usePager = function () {
            return false;
        };
        DataGrid.prototype.enableFiltering = function () {
            var attr = ss.getAttributes(ss.getInstanceType(this), Serenity.FilterableAttribute, true);
            return attr.length > 0 && attr[0].value;
        };
        DataGrid.prototype.populateWhenVisible = function () {
            return false;
        };
        DataGrid.prototype.createFilterBar = function () {
            var _this = this;
            var filterBarDiv = $('<div/>').appendTo(this.element);
            var self = this;
            this.filterBar = new Serenity.FilterDisplayBar(filterBarDiv);
            if (!this.isAsyncWidget()) {
                this.filterBar.set_store(new Serenity.FilterStore(this.allColumns.filter(function (x) {
                    return (x.sourceItem != null) && x.sourceItem.notFilterable !== true;
                }).map(function (x1) {
                    return x1.sourceItem;
                })));
                this.filterBar.get_store().add_changed(function (s, e) {
                    if (_this.restoringSettings <= 0) {
                        self.persistSettings(null);
                        self.refresh();
                    }
                });
            }
        };
        DataGrid.prototype.getPagerOptions = function () {
            return {
                view: this.view,
                rowsPerPage: 20,
                rowsPerPageOptions: [20, 100, 500, 2500]
            };
        };
        DataGrid.prototype.createPager = function () {
            var pagerDiv = $('<div></div>').appendTo(this.element);
            pagerDiv.slickPager(this.getPagerOptions());
        };
        DataGrid.prototype.getViewOptions = function () {
            var _this = this;
            var opt = {};
            opt.idField = this.getIdProperty();
            opt.sortBy = this.getDefaultSortBy();
            if (!this.usePager()) {
                opt.rowsPerPage = 0;
            }
            else if (this.element.hasClass('responsive-height')) {
                opt.rowsPerPage = (($(window.window).width() < 768) ? 20 : 100);
            }
            else {
                opt.rowsPerPage = 100;
            }
            opt.getItemMetadata = function (item, index) {
                return _this.getItemMetadata(item, index);
            };
            return opt;
        };
        DataGrid.prototype.createToolbar = function (buttons) {
            var toolbarDiv = $('<div class="grid-toolbar"></div>').appendTo(this.element);
            this.toolbar = new Serenity.Toolbar(toolbarDiv, { buttons: buttons, hotkeyContext: this.element[0] });
        };
        DataGrid.prototype.getTitle = function () {
            if (!this.titleDiv) {
                return null;
            }
            return this.titleDiv.children().text();
        };
        DataGrid.prototype.setTitle = function (value) {
            if (value !== this.getTitle()) {
                if (value == null) {
                    if (this.titleDiv) {
                        this.titleDiv.remove();
                        this.titleDiv = null;
                    }
                }
                else {
                    if (!this.titleDiv) {
                        this.titleDiv = $('<div class="grid-title"><div class="title-text"></div></div>')
                            .prependTo(this.element);
                    }
                    this.titleDiv.children().text(value);
                }
                this.layout();
            }
        };
        DataGrid.prototype.getItemType = function () {
            return 'Item';
        };
        DataGrid.prototype.itemLink = function (itemType, idField, text, cssClass, encode) {
            if (encode === void 0) { encode = true; }
            if (itemType == null) {
                itemType = this.getItemType();
            }
            if (idField == null) {
                idField = this.getIdProperty();
            }
            return Serenity.SlickFormatting.itemLink(itemType, idField, text, cssClass, encode);
        };
        DataGrid.prototype.getColumnsKey = function () {
            var attr = ss.getAttributes(ss.getInstanceType(this), Serenity.ColumnsKeyAttribute, true);
            if (attr && attr.length > 0) {
                return attr[0].value;
            }
            return null;
        };
        DataGrid.prototype.getPropertyItemsAsync = function () {
            var _this = this;
            return Promise.resolve()
                .then(function () {
                var columnsKey = _this.getColumnsKey();
                if (!Q.isEmptyOrNull(columnsKey)) {
                    return Q.getColumnsAsync(columnsKey);
                }
                return Promise.resolve([]);
            }, null);
        };
        DataGrid.prototype.getPropertyItems = function () {
            var attr = ss.getAttributes(ss.getInstanceType(this), Serenity.ColumnsKeyAttribute, true);
            var columnsKey = this.getColumnsKey();
            if (!Q.isEmptyOrNull(columnsKey)) {
                return Q.getColumns(columnsKey);
            }
            return [];
        };
        DataGrid.prototype.getColumns = function () {
            var propertyItems = this.getPropertyItems();
            return this.propertyItemsToSlickColumns(propertyItems);
        };
        DataGrid.prototype.propertyItemsToSlickColumns = function (propertyItems) {
            var columns = Serenity.PropertyItemSlickConverter.toSlickColumns(propertyItems);
            for (var i = 0; i < propertyItems.length; i++) {
                var item = propertyItems[i];
                var column = columns[i];
                if (item.editLink === true) {
                    var oldFormat = { $: column.format };
                    var css = { $: (item.editLinkCssClass) != null ? item.editLinkCssClass : null };
                    column.format = this.itemLink(item.editLinkItemType != null ? item.editLinkItemType : null, item.editLinkIdField != null ? item.editLinkIdField : null, ss.mkdel({ oldFormat: oldFormat }, function (ctx) {
                        if (this.oldFormat.$ !== null) {
                            return this.oldFormat.$(ctx);
                        }
                        return Q.htmlEncode(ctx.value);
                    }), ss.mkdel({ css: css }, function (ctx1) {
                        return Q.coalesce(this.css.$, '');
                    }), false);
                    if (!Q.isEmptyOrNull(item.editLinkIdField)) {
                        column.referencedFields = column.referencedFields || [];
                        column.referencedFields.push(item.editLinkIdField);
                    }
                }
            }
            return columns;
        };
        DataGrid.prototype.getColumnsAsync = function () {
            var _this = this;
            return this.getPropertyItemsAsync().then(function (propertyItems) {
                return _this.propertyItemsToSlickColumns(propertyItems);
            }, null);
        };
        DataGrid.prototype.getSlickOptions = function () {
            var opt = {};
            opt.multiSelect = false;
            opt.multiColumnSort = true;
            opt.enableCellNavigation = false;
            opt.headerRowHeight = Serenity.DataGrid.defaultHeaderHeight;
            opt.rowHeight = Serenity.DataGrid.defaultRowHeight;
            return opt;
        };
        DataGrid.prototype.populateLock = function () {
            this.view.populateLock();
        };
        DataGrid.prototype.populateUnlock = function () {
            this.view.populateUnlock();
        };
        DataGrid.prototype.getGridCanLoad = function () {
            return true;
        };
        DataGrid.prototype.refresh = function () {
            if (!this.populateWhenVisible()) {
                this.internalRefresh();
                return;
            }
            if (this.slickContainer.is(':visible')) {
                this.slickContainer.data('needsRefresh', false);
                this.internalRefresh();
                return;
            }
            this.slickContainer.data('needsRefresh', true);
        };
        DataGrid.prototype.refreshIfNeeded = function () {
            if (!!this.slickContainer.data('needsRefresh')) {
                this.slickContainer.data('needsRefresh', false);
                this.internalRefresh();
            }
        };
        DataGrid.prototype.internalRefresh = function () {
            this.view.populate();
        };
        DataGrid.prototype.setIsDisabled = function (value) {
            if (this.isDisabled !== value) {
                this.isDisabled = value;
                if (this.isDisabled) {
                    this.view.setItems([], true);
                }
                this.updateDisabledState();
            }
        };
        DataGrid.prototype.getLocalTextDbPrefix = function () {
            if (this.localTextDbPrefix == null) {
                this.localTextDbPrefix = Q.coalesce(this.getLocalTextPrefix(), '');
                if (this.localTextDbPrefix.length > 0 && !Q.endsWith(this.localTextDbPrefix, '.')) {
                    this.localTextDbPrefix = 'Db.' + this.localTextDbPrefix + '.';
                }
            }
            return this.localTextDbPrefix;
        };
        DataGrid.prototype.getLocalTextPrefix = function () {
            var attributes = ss.getAttributes(ss.getInstanceType(this), Serenity.LocalTextPrefixAttribute, true);
            if (attributes.length >= 1)
                return attributes[0].value;
            return '';
        };
        DataGrid.prototype.getIdProperty = function () {
            if (this.idProperty == null) {
                var attributes = ss.getAttributes(ss.getInstanceType(this), Serenity.IdPropertyAttribute, true);
                if (attributes.length === 1) {
                    this.idProperty = attributes[0].value;
                }
                else {
                    this.idProperty = 'ID';
                }
            }
            return this.idProperty;
        };
        DataGrid.prototype.getIsActiveProperty = function () {
            if (this.isActiveProperty == null) {
                var attributes = ss.getAttributes(ss.getInstanceType(this), Serenity.IsActivePropertyAttribute, true);
                if (attributes.length === 1) {
                    this.isActiveProperty = attributes[0].value;
                }
                else {
                    this.isActiveProperty = '';
                }
            }
            return this.isActiveProperty;
        };
        DataGrid.prototype.updateDisabledState = function () {
            this.slickContainer.toggleClass('ui-state-disabled', !!this.isDisabled);
        };
        DataGrid.prototype.resizeCanvas = function () {
            this.slickGrid.resizeCanvas();
        };
        DataGrid.prototype.subDialogDataChange = function () {
            this.refresh();
        };
        DataGrid.prototype.addFilterSeparator = function () {
            if (this.quickFiltersDiv) {
                this.quickFiltersDiv.append($('<hr/>'));
            }
        };
        DataGrid.prototype.determineText = function (getKey) {
            var localTextPrefix = this.getLocalTextDbPrefix();
            if (!Q.isEmptyOrNull(localTextPrefix)) {
                var local = Q.tryGetText(getKey(localTextPrefix));
                if (local != null) {
                    return local;
                }
            }
            return null;
        };
        DataGrid.prototype.addQuickFilter = function (opt) {
            var _this = this;
            if (opt == null) {
                throw new ss.ArgumentNullException('opt');
            }
            if (this.quickFiltersDiv == null) {
                $('<div/>').addClass('clear').appendTo(this.toolbar.element);
                this.quickFiltersDiv = $('<div/>').addClass('quick-filters-bar').appendTo(this.toolbar.element);
            }
            if (opt.separator) {
                this.addFilterSeparator();
            }
            var item = $("<div class='quick-filter-item'><span class='quick-filter-label'></span></div>")
                .appendTo(this.quickFiltersDiv)
                .data('qffield', opt.field).children();
            var title = opt.title;
            if (title == null) {
                title = this.determineText(function (pre) {
                    return pre + opt.field;
                });
                if (title == null) {
                    title = opt.field;
                }
            }
            var quickFilter = item.text(title).parent();
            if (opt.displayText != null) {
                quickFilter.data('qfdisplaytext', opt.displayText);
            }
            if (opt.saveState != null) {
                quickFilter.data('qfsavestate', opt.saveState);
            }
            if (opt.loadState != null) {
                quickFilter.data('qfloadstate', opt.loadState);
            }
            if (!Q.isEmptyOrNull(opt.cssClass)) {
                quickFilter.addClass(opt.cssClass);
            }
            var widget = Serenity.Widget.create({
                type: opt.type,
                element: function (e) {
                    if (!Q.isEmptyOrNull(opt.field)) {
                        e.attr('id', _this.uniqueName + '_QuickFilter_' + opt.field);
                    }
                    e.attr('placeholder', ' ');
                    e.appendTo(quickFilter);
                    if (opt.element != null) {
                        opt.element(e);
                    }
                },
                options: opt.options,
                init: opt.init
            });
            var submitHandler = function () {
                if (quickFilter.hasClass('ignore')) {
                    return;
                }
                var request = _this.view.params;
                request.EqualityFilter = request.EqualityFilter || {};
                var value = Serenity.EditorUtils.getValue(widget);
                var active = value != null && !Q.isEmptyOrNull(value.toString());
                if (opt.handler != null) {
                    var args = {
                        field: opt.field,
                        request: request,
                        equalityFilter: request.EqualityFilter,
                        value: value,
                        active: active,
                        widget: widget,
                        handled: true
                    };
                    opt.handler(args);
                    quickFilter.toggleClass('quick-filter-active', args.active);
                    if (!args.handled) {
                        request.EqualityFilter[opt.field] = value;
                    }
                }
                else {
                    request.EqualityFilter[opt.field] = value;
                    quickFilter.toggleClass('quick-filter-active', active);
                }
            };
            Serenity.WX.changeSelect2(widget, function (e1) {
                _this.quickFilterChange(e1);
            });
            this.add_submitHandlers(submitHandler);
            widget.element.bind('remove.' + this.uniqueName, function (x) {
                _this.remove_submitHandlers(submitHandler);
            });
            return widget;
        };
        DataGrid.prototype.addDateRangeFilter = function (field, title) {
            return this.addQuickFilter(this.dateRangeQuickFilter(field, title));
        };
        DataGrid.prototype.dateRangeQuickFilter = function (field, title) {
            var end = null;
            return {
                field: field,
                type: Serenity.DateEditor,
                title: title,
                element: function (e1) {
                    end = Serenity.Widget.create({
                        type: Serenity.DateEditor,
                        element: function (e2) {
                            e2.insertAfter(e1);
                        },
                        options: null,
                        init: null
                    });
                    end.element.change(function (x) {
                        e1.triggerHandler('change');
                    });
                    $('<span/>').addClass('range-separator').text('-').insertAfter(e1);
                },
                handler: function (args) {
                    var active1 = !Q.isTrimmedEmpty(args.widget.value);
                    var active2 = !Q.isTrimmedEmpty(end.value);
                    if (active1 && !Q.parseDate(args.widget.element.val())) {
                        active1 = false;
                        Q.notifyWarning(Q.text('Validation.DateInvalid'), '', null);
                        args.widget.element.val('');
                    }
                    if (active2 && !Q.parseDate(end.element.val())) {
                        active2 = false;
                        Q.notifyWarning(Q.text('Validation.DateInvalid'), '', null);
                        end.element.val('');
                    }
                    args.active = active1 || active2;
                    if (active1) {
                        args.request.Criteria = Serenity.Criteria.join(args.request.Criteria, 'and', [[args.field], '>=', args.widget.value]);
                    }
                    if (active2) {
                        var next = new Date(end.valueAsDate.valueOf());
                        next.setDate(next.getDate() + 1);
                        args.request.Criteria = Serenity.Criteria.join(args.request.Criteria, 'and', [[args.field], '<', Q.formatDate(next, 'yyyy-MM-dd')]);
                    }
                },
                displayText: function (w, l) {
                    var v1 = Serenity.EditorUtils.getDisplayText(w);
                    var v2 = Serenity.EditorUtils.getDisplayText(end);
                    if (Q.isEmptyOrNull(v1) && Q.isEmptyOrNull(v2)) {
                        return null;
                    }
                    var text1 = l + ' >= ' + v1;
                    var text2 = l + ' <= ' + v2;
                    if (!Q.isEmptyOrNull(v1) && !Q.isEmptyOrNull(v2)) {
                        return text1 + ' ' + Q.coalesce(Q.tryGetText('Controls.FilterPanel.And'), 'and') + ' ' + text2;
                    }
                    else if (!Q.isEmptyOrNull(v1)) {
                        return text1;
                    }
                    else {
                        return text2;
                    }
                },
                saveState: function (w1) {
                    return [Serenity.EditorUtils.getValue(w1), Serenity.EditorUtils.getValue(end)];
                },
                loadState: function (w2, state) {
                    if (state == null || !Q.isArray(state) || state.length !== 2) {
                        state = [null, null];
                    }
                    Serenity.EditorUtils.setValue(w2, state[0]);
                    Serenity.EditorUtils.setValue(end, state[1]);
                }
            };
        };
        DataGrid.prototype.addDateTimeRangeFilter = function (field, title) {
            return this.addQuickFilter(this.dateTimeRangeQuickFilter(field, title));
        };
        DataGrid.prototype.dateTimeRangeQuickFilter = function (field, title) {
            var end = null;
            return {
                field: field,
                type: Serenity.DateTimeEditor,
                title: title,
                element: function (e1) {
                    end = Serenity.Widget.create({
                        type: Serenity.DateTimeEditor,
                        element: function (e2) {
                            e2.insertAfter(e1);
                        },
                        options: null,
                        init: null
                    });
                    end.element.change(function (x) {
                        e1.triggerHandler('change');
                    });
                    $('<span/>').addClass('range-separator').text('-').insertAfter(e1);
                },
                init: function (i) {
                    i.element.parent().find('.time').change(function (x1) {
                        i.element.triggerHandler('change');
                    });
                },
                handler: function (args) {
                    var active1 = !Q.isTrimmedEmpty(args.widget.value);
                    var active2 = !Q.isTrimmedEmpty(end.value);
                    if (active1 && !Q.parseDate(args.widget.element.val())) {
                        active1 = false;
                        Q.notifyWarning(Q.text('Validation.DateInvalid'), '', null);
                        args.widget.element.val('');
                    }
                    if (active2 && !Q.parseDate(end.element.val())) {
                        active2 = false;
                        Q.notifyWarning(Q.text('Validation.DateInvalid'), '', null);
                        end.element.val('');
                    }
                    args.active = active1 || active2;
                    if (active1) {
                        args.request.Criteria = Serenity.Criteria.join(args.request.Criteria, 'and', [[args.field], '>=', args.widget.value]);
                    }
                    if (active2) {
                        args.request.Criteria = Serenity.Criteria.join(args.request.Criteria, 'and', [[args.field], '<=', end.value]);
                    }
                },
                displayText: function (w, l) {
                    var v1 = Serenity.EditorUtils.getDisplayText(w);
                    var v2 = Serenity.EditorUtils.getDisplayText(end);
                    if (Q.isEmptyOrNull(v1) && Q.isEmptyOrNull(v2)) {
                        return null;
                    }
                    var text1 = l + ' >= ' + v1;
                    var text2 = l + ' <= ' + v2;
                    if (!Q.isEmptyOrNull(v1) && !Q.isEmptyOrNull(v2)) {
                        return text1 + ' ' + Q.coalesce(Q.tryGetText('Controls.FilterPanel.And'), 'and') + ' ' + text2;
                    }
                    else if (!Q.isEmptyOrNull(v1)) {
                        return text1;
                    }
                    else {
                        return text2;
                    }
                },
                saveState: function (w1) {
                    return [Serenity.EditorUtils.getValue(w1), Serenity.EditorUtils.getValue(end)];
                },
                loadState: function (w2, state) {
                    if (state == null || !Q.isArray(state) || state.length !== 2) {
                        state = [null, null];
                    }
                    Serenity.EditorUtils.setValue(w2, state[0]);
                    Serenity.EditorUtils.setValue(end, state[1]);
                }
            };
        };
        DataGrid.prototype.addBooleanFilter = function (field, title, yes, no) {
            return this.addQuickFilter(this.booleanQuickFilter(field, title, yes, no));
        };
        DataGrid.prototype.booleanQuickFilter = function (field, title, yes, no) {
            var opt = {};
            var items = [];
            var trueText = yes;
            if (trueText == null) {
                trueText = Q.text('Controls.FilterPanel.OperatorNames.true');
            }
            items.push(['1', trueText]);
            var falseText = no;
            if (falseText == null) {
                falseText = Q.text('Controls.FilterPanel.OperatorNames.false');
            }
            items.push(['0', falseText]);
            opt.items = items;
            return {
                field: field,
                type: Serenity.SelectEditor,
                title: title,
                options: opt,
                handler: function (args) {
                    args.equalityFilter[args.field] = args.value == null || Q.isEmptyOrNull(args.value.toString()) ?
                        null : !!Q.toId(args.value);
                }
            };
        };
        DataGrid.prototype.invokeSubmitHandlers = function () {
            if (this.submitHandlers != null) {
                this.submitHandlers();
            }
        };
        DataGrid.prototype.quickFilterChange = function (e) {
            this.persistSettings(null);
            this.refresh();
        };
        DataGrid.prototype.getPersistanceStorage = function () {
            return Serenity.DataGrid.defaultPersistanceStorage;
        };
        DataGrid.prototype.getPersistanceKey = function () {
            var key = 'GridSettings:';
            var path = window.location.pathname;
            if (!Q.isEmptyOrNull(path)) {
                key += path.substr(1).split(String.fromCharCode(47)).slice(0, 2).join('/') + ':';
            }
            key += ss.getTypeFullName(ss.getInstanceType(this));
            return key;
        };
        DataGrid.prototype.gridPersistanceFlags = function () {
            return {};
        };
        DataGrid.prototype.canShowColumn = function (column) {
            if (column == null) {
                return false;
            }
            var item = column.sourceItem;
            if (item == null) {
                return true;
            }
            if (item.filterOnly === true) {
                return false;
            }
            if (item.readPermission == null) {
                return true;
            }
            return Q.Authorization.hasPermission(item.readPermission);
        };
        DataGrid.prototype.restoreSettings = function (settings, flags) {
            var _this = this;
            if (settings == null) {
                var storage = this.getPersistanceStorage();
                if (storage == null) {
                    return;
                }
                var json = Q.trimToNull(storage.getItem(this.getPersistanceKey()));
                if (json != null && Q.startsWith(json, '{') && Q.endsWith(json, '}')) {
                    settings = JSON.parse(json);
                }
                else {
                    return;
                }
            }
            if (!this.slickGrid) {
                return;
            }
            var columns = this.slickGrid.getColumns();
            var colById = null;
            var updateColById = function (cl) {
                colById = {};
                for (var $t1 = 0; $t1 < cl.length; $t1++) {
                    var c = cl[$t1];
                    colById[c.id] = c;
                }
            };
            this.view.beginUpdate();
            this.restoringSettings++;
            try {
                flags = flags || this.gridPersistanceFlags();
                if (settings.columns != null) {
                    if (flags.columnVisibility !== false) {
                        var visible = {};
                        updateColById(this.allColumns);
                        var newColumns = [];
                        for (var $t2 = 0; $t2 < settings.columns.length; $t2++) {
                            var x = settings.columns[$t2];
                            if (x.id != null && x.visible === true) {
                                var column = colById[x.id];
                                if (this.canShowColumn(column)) {
                                    column.visible = true;
                                    newColumns.push(column);
                                    delete colById[x.id];
                                }
                            }
                        }
                        for (var $t3 = 0; $t3 < this.allColumns.length; $t3++) {
                            var c1 = this.allColumns[$t3];
                            if (colById[c1.id] != null) {
                                c1.visible = false;
                                newColumns.push(c1);
                            }
                        }
                        this.allColumns = newColumns;
                        columns = this.allColumns.filter(function (x1) {
                            return x1.visible === true;
                        });
                    }
                    if (flags.columnWidths !== false) {
                        updateColById(columns);
                        for (var $t4 = 0; $t4 < settings.columns.length; $t4++) {
                            var x2 = settings.columns[$t4];
                            if (x2.id != null && x2.width != null && x2.width !== 0) {
                                var column1 = colById[x2.id];
                                if (column1 != null) {
                                    column1.width = x2.width;
                                }
                            }
                        }
                    }
                    if (flags.sortColumns !== false) {
                        updateColById(columns);
                        var list = [];
                        var sortColumns = settings.columns.filter(function (x3) {
                            return x3.id != null && Q.coalesce(x3.sort, 0) !== 0;
                        });
                        sortColumns.sort(function (a, b) {
                            return a.sort - b.sort;
                        });
                        for (var $t5 = 0; $t5 < sortColumns.length; $t5++) {
                            var x4 = sortColumns[$t5];
                            var column2 = colById[x4.id];
                            if (column2 != null) {
                                list.push({
                                    columnId: x4.id,
                                    sortAsc: x4.sort > 0
                                });
                            }
                        }
                        this.view.sortBy = list.map(function (x5) {
                            return x5.columnId + ((x5.sortAsc === false) ? ' DESC' : '');
                        });
                        this.slickGrid.setSortColumns(list);
                    }
                    this.slickGrid.setColumns(columns);
                    this.slickGrid.invalidate();
                }
                if (settings.filterItems != null &&
                    flags.filterItems !== false &&
                    this.filterBar != null &&
                    this.filterBar.get_store() != null) {
                    ss.clear(this.filterBar.get_store().get_items());
                    ss.arrayAddRange(this.filterBar.get_store().get_items(), settings.filterItems);
                    this.filterBar.get_store().raiseChanged();
                }
                if (settings.includeDeleted != null &&
                    flags.includeDeleted !== false) {
                    var includeDeletedToggle = this.element.find('.s-IncludeDeletedToggle');
                    if (!!settings.includeDeleted !== includeDeletedToggle.hasClass('pressed')) {
                        includeDeletedToggle.children('a').click();
                    }
                }
                if (settings.quickFilters != null &&
                    flags.quickFilters !== false &&
                    this.quickFiltersDiv != null &&
                    this.quickFiltersDiv.length > 0) {
                    this.quickFiltersDiv.find('.quick-filter-item').each(function (i, e) {
                        var field = $(e).data('qffield');
                        if (Q.isEmptyOrNull(field)) {
                            return;
                        }
                        var widget = $('#' + _this.uniqueName + '_QuickFilter_' + field).tryGetWidget(Serenity.Widget);
                        if (widget == null) {
                            return;
                        }
                        var state = settings.quickFilters[field];
                        var loadState = $(e).data('qfloadstate');
                        if (loadState != null) {
                            loadState(widget, state);
                        }
                        else {
                            Serenity.EditorUtils.setValue(widget, state);
                        }
                    });
                }
                if (flags.quickSearch === true && (settings.quickSearchField != null || settings.quickSearchText != null)) {
                    var qsInput = this.toolbar.element.find('.s-QuickSearchInput').first();
                    if (qsInput.length > 0) {
                        var qsWidget = qsInput.tryGetWidget(Serenity.QuickSearchInput);
                        if (qsWidget != null) {
                            this.view.populateLock();
                            try {
                                qsWidget.element.addClass('ignore-change');
                                try {
                                    if (settings.quickSearchField != null) {
                                        qsWidget.set_field(settings.quickSearchField);
                                    }
                                    if (settings.quickSearchText != null &&
                                        Q.trimToNull(settings.quickSearchText) !== Q.trimToNull(qsWidget.element.val())) {
                                        qsWidget.element.val(settings.quickSearchText);
                                    }
                                }
                                finally {
                                    qsWidget.element.removeClass('ignore-change');
                                    qsWidget.element.triggerHandler('execute-search');
                                }
                            }
                            finally {
                                this.view.populateUnlock();
                            }
                        }
                    }
                }
            }
            finally {
                this.restoringSettings--;
                this.view.endUpdate();
            }
        };
        DataGrid.prototype.persistSettings = function (flags) {
            var storage = this.getPersistanceStorage();
            if (!storage) {
                return;
            }
            var settings = this.getCurrentSettings(flags);
            storage.setItem(this.getPersistanceKey(), $.toJSON(settings));
        };
        DataGrid.prototype.getCurrentSettings = function (flags) {
            var _this = this;
            flags = flags || this.gridPersistanceFlags();
            var settings = {};
            if (flags.columnVisibility !== false || flags.columnWidths !== false || flags.sortColumns !== false) {
                settings.columns = [];
                var sortColumns = this.slickGrid.getSortColumns();
                var $t1 = this.slickGrid.getColumns();
                for (var $t2 = 0; $t2 < $t1.length; $t2++) {
                    var column = { $: $t1[$t2] };
                    var p = {
                        id: column.$.id
                    };
                    if (flags.columnVisibility !== false) {
                        p.visible = true;
                    }
                    if (flags.columnWidths !== false) {
                        p.width = column.$.width;
                    }
                    if (flags.sortColumns !== false) {
                        var sort = Q.indexOf(sortColumns, ss.mkdel({ column: column }, function (x) {
                            return x.columnId !== this.column.$.id;
                        }));
                        p.sort = ((sort >= 0) ? ((sortColumns[sort].sortAsc !== false) ? (sort + 1) : (-sort - 1)) : 0);
                    }
                    settings.columns.push(p);
                }
            }
            if (flags.includeDeleted !== false) {
                settings.includeDeleted = this.element.find('.s-IncludeDeletedToggle').hasClass('pressed');
            }
            if (flags.filterItems !== false && (this.filterBar != null) && (this.filterBar.get_store() != null)) {
                settings.filterItems = this.filterBar.get_store().get_items().slice();
            }
            if (flags.quickSearch === true) {
                var qsInput = this.toolbar.element.find('.s-QuickSearchInput').first();
                if (qsInput.length > 0) {
                    var qsWidget = qsInput.tryGetWidget(Serenity.QuickSearchInput);
                    if (qsWidget != null) {
                        settings.quickSearchField = qsWidget.get_field();
                        settings.quickSearchText = qsWidget.element.val();
                    }
                }
            }
            if (flags.quickFilters !== false && (this.quickFiltersDiv != null) && this.quickFiltersDiv.length > 0) {
                settings.quickFilters = {};
                this.quickFiltersDiv.find('.quick-filter-item').each(function (i, e) {
                    var field = $(e).data('qffield');
                    if (Q.isEmptyOrNull(field)) {
                        return;
                    }
                    var widget = $('#' + _this.uniqueName + '_QuickFilter_' + field).tryGetWidget(Serenity.Widget);
                    if (widget == null) {
                        return;
                    }
                    var saveState = $(e).data('qfsavestate');
                    var state = (saveState != null) ? saveState(widget) : Serenity.EditorUtils.getValue(widget);
                    settings.quickFilters[field] = state;
                    if (flags.quickFilterText === true && $(e).hasClass('quick-filter-active')) {
                        var getDisplayText = $(e).data('qfdisplaytext');
                        var filterLabel = $(e).find('.quick-filter-label').text();
                        var displayText;
                        if (getDisplayText != null) {
                            displayText = getDisplayText(widget, filterLabel);
                        }
                        else {
                            displayText = filterLabel + ' = ' + Serenity.EditorUtils.getDisplayText(widget);
                        }
                        if (!Q.isEmptyOrNull(displayText)) {
                            if (!Q.isEmptyOrNull(settings.quickFilterText)) {
                                settings.quickFilterText += ' ' + Q.coalesce(Q.tryGetText('Controls.FilterPanel.And'), 'and') + ' ';
                                settings.quickFilterText += displayText;
                            }
                            else {
                                settings.quickFilterText = displayText;
                            }
                        }
                    }
                });
            }
            return settings;
        };
        DataGrid.prototype.getElement = function () {
            return this.element;
        };
        DataGrid.prototype.getGrid = function () {
            return this.slickGrid;
        };
        DataGrid.prototype.getView = function () {
            return this.view;
        };
        DataGrid.prototype.getFilterStore = function () {
            return (this.filterBar == null) ? null : this.filterBar.get_store();
        };
        DataGrid = __decorate([
            Serenity.Decorators.registerClass('Serenity.DataGrid', [Serenity.IDataGrid])
        ], DataGrid);
        return DataGrid;
    }(Serenity.Widget));
    Serenity.DataGrid = DataGrid;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var CheckTreeEditor = /** @class */ (function (_super) {
        __extends(CheckTreeEditor, _super);
        function CheckTreeEditor(div, opt) {
            var _this = _super.call(this, div, opt) || this;
            div.addClass('s-CheckTreeEditor');
            _this.updateItems();
            return _this;
        }
        CheckTreeEditor.prototype.getIdProperty = function () {
            return "id";
        };
        CheckTreeEditor.prototype.getTreeItems = function () {
            return [];
        };
        CheckTreeEditor.prototype.updateItems = function () {
            var items = this.getTreeItems();
            var itemById = {};
            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                item.children = [];
                if (!Q.isEmptyOrNull(item.id)) {
                    itemById[item.id] = item;
                }
                if (!Q.isEmptyOrNull(item.parentId)) {
                    var parent = itemById[item.parentId];
                    if (parent != null) {
                        parent.children.push(item);
                    }
                }
            }
            this.view.addData({ Entities: items, Skip: 0, Take: 0, TotalCount: items.length });
            this.updateSelectAll();
            this.updateFlags();
        };
        CheckTreeEditor.prototype.getEditValue = function (property, target) {
            target[property.name] = this.get_value();
        };
        CheckTreeEditor.prototype.setEditValue = function (source, property) {
            var value = source[property.name];
            if (Q.isArray(value)) {
                this.set_value(value);
            }
        };
        CheckTreeEditor.prototype.getButtons = function () {
            var _this = this;
            var selectAllText = this.getSelectAllText();
            if (Q.isEmptyOrNull(selectAllText)) {
                return null;
            }
            var self = this;
            var buttons = [];
            buttons.push(Serenity.GridSelectAllButtonHelper.define(function () {
                return self;
            }, function (x) {
                return x.id;
            }, function (x1) {
                return x1.isSelected;
            }, function (x2, v) {
                if (x2.isSelected !== v) {
                    x2.isSelected = v;
                    _this.itemSelectedChanged(x2);
                }
            }, null, function () {
                _this.updateFlags();
            }));
            return buttons;
        };
        CheckTreeEditor.prototype.itemSelectedChanged = function (item) {
        };
        CheckTreeEditor.prototype.getSelectAllText = function () {
            return Q.coalesce(Q.tryGetText('Controls.CheckTreeEditor.SelectAll'), 'Select All');
        };
        CheckTreeEditor.prototype.isThreeStateHierarchy = function () {
            return false;
        };
        CheckTreeEditor.prototype.createSlickGrid = function () {
            this.element.addClass('slick-no-cell-border').addClass('slick-no-odd-even');
            var result = _super.prototype.createSlickGrid.call(this);
            this.element.addClass('slick-hide-header');
            result.resizeCanvas();
            return result;
        };
        CheckTreeEditor.prototype.onViewFilter = function (item) {
            if (!_super.prototype.onViewFilter.call(this, item)) {
                return false;
            }
            var items = this.view.getItems();
            var self = this;
            return Serenity.SlickTreeHelper.filterCustom(item, function (x) {
                if (x.parentId == null) {
                    return null;
                }
                if (self.byId == null) {
                    self.byId = {};
                    for (var i = 0; i < items.length; i++) {
                        var o = items[i];
                        if (o.id != null) {
                            self.byId[o.id] = o;
                        }
                    }
                }
                return self.byId[x.parentId];
            });
        };
        CheckTreeEditor.prototype.getInitialCollapse = function () {
            return false;
        };
        CheckTreeEditor.prototype.onViewProcessData = function (response) {
            response = _super.prototype.onViewProcessData.call(this, response);
            this.byId = null;
            Serenity.SlickTreeHelper.setIndents(response.Entities, function (x) {
                return x.id;
            }, function (x1) {
                return x1.parentId;
            }, this.getInitialCollapse());
            return response;
        };
        CheckTreeEditor.prototype.onClick = function (e, row, cell) {
            _super.prototype.onClick.call(this, e, row, cell);
            if (!e.isDefaultPrevented()) {
                Serenity.SlickTreeHelper.toggleClick(e, row, cell, this.view, function (x) {
                    return x.id;
                });
            }
            if (e.isDefaultPrevented()) {
                return;
            }
            var target = $(e.target);
            if (target.hasClass('check-box')) {
                var checkedOrPartial = target.hasClass('checked') || target.hasClass('partial');
                var item = this.itemAt(row);
                var anyChanged = item.isSelected !== !checkedOrPartial;
                this.view.beginUpdate();
                try {
                    if (item.isSelected !== !checkedOrPartial) {
                        item.isSelected = !checkedOrPartial;
                        this.view.updateItem(item.id, item);
                        this.itemSelectedChanged(item);
                    }
                    anyChanged = this.setAllSubTreeSelected(item, item.isSelected) || anyChanged;
                    this.updateSelectAll();
                    this.updateFlags();
                }
                finally {
                    this.view.endUpdate();
                }
                if (anyChanged) {
                    this.element.triggerHandler('change');
                }
            }
        };
        CheckTreeEditor.prototype.updateSelectAll = function () {
            Serenity.GridSelectAllButtonHelper.update(this, function (x) {
                return x.isSelected;
            });
        };
        CheckTreeEditor.prototype.updateFlags = function () {
            var view = this.view;
            var items = view.getItems();
            var threeState = this.isThreeStateHierarchy();
            if (!threeState) {
                return;
            }
            view.beginUpdate();
            try {
                for (var i = 0; i < items.length; i++) {
                    var item = items[i];
                    if (item.children == null || item.children.length === 0) {
                        var allsel = this.getDescendantsSelected(item);
                        if (allsel !== item.isAllDescendantsSelected) {
                            item.isAllDescendantsSelected = allsel;
                            view.updateItem(item.id, item);
                        }
                        continue;
                    }
                    var allSelected = this.allDescendantsSelected(item);
                    var selected = allSelected || this.anyDescendantsSelected(item);
                    if (allSelected !== item.isAllDescendantsSelected || selected !== item.isSelected) {
                        var selectedChange = item.isSelected !== selected;
                        item.isAllDescendantsSelected = allSelected;
                        item.isSelected = selected;
                        view.updateItem(item.id, item);
                        if (selectedChange) {
                            this.itemSelectedChanged(item);
                        }
                    }
                }
            }
            finally {
                view.endUpdate();
            }
        };
        CheckTreeEditor.prototype.getDescendantsSelected = function (item) {
            return true;
        };
        CheckTreeEditor.prototype.setAllSubTreeSelected = function (item, selected) {
            var result = false;
            for (var i = 0; i < item.children.length; i++) {
                var sub = item.children[i];
                if (sub.isSelected !== selected) {
                    result = true;
                    sub.isSelected = selected;
                    this.view.updateItem(sub.id, sub);
                    this.itemSelectedChanged(sub);
                }
                if (sub.children.length > 0) {
                    result = this.setAllSubTreeSelected(sub, selected) || result;
                }
            }
            return result;
        };
        CheckTreeEditor.prototype.allItemsSelected = function () {
            for (var i = 0; i < this.rowCount(); i++) {
                var row = this.itemAt(i);
                if (!row.isSelected) {
                    return false;
                }
            }
            return this.rowCount() > 0;
        };
        CheckTreeEditor.prototype.allDescendantsSelected = function (item) {
            if (item.children.length > 0) {
                for (var i = 0; i < item.children.length; i++) {
                    var sub = item.children[i];
                    if (!sub.isSelected) {
                        return false;
                    }
                    if (!this.allDescendantsSelected(sub)) {
                        return false;
                    }
                }
            }
            return true;
        };
        CheckTreeEditor.prototype.anyDescendantsSelected = function (item) {
            if (item.children.length > 0) {
                for (var i = 0; i < item.children.length; i++) {
                    var sub = item.children[i];
                    if (sub.isSelected) {
                        return true;
                    }
                    if (this.anyDescendantsSelected(sub)) {
                        return true;
                    }
                }
            }
            return false;
        };
        CheckTreeEditor.prototype.getColumns = function () {
            var _this = this;
            var self = this;
            var columns = [];
            columns.push({
                field: 'text', name: 'Kayıt', width: 80, format: Serenity.SlickFormatting.treeToggle(function () {
                    return self.view;
                }, function (x) {
                    return x.id;
                }, function (ctx) {
                    var cls = 'check-box';
                    var item = ctx.item;
                    if (item.hideCheckBox) {
                        return _this.getItemText(ctx);
                    }
                    var threeState = _this.isThreeStateHierarchy();
                    if (item.isSelected) {
                        if (threeState && !item.isAllDescendantsSelected) {
                            cls += ' partial';
                        }
                        else {
                            cls += ' checked';
                        }
                    }
                    return '<span class="' + cls + '"></span>' + _this.getItemText(ctx);
                })
            });
            return columns;
        };
        CheckTreeEditor.prototype.getItemText = function (ctx) {
            return Q.htmlEncode(ctx.value);
        };
        CheckTreeEditor.prototype.getSlickOptions = function () {
            var opt = _super.prototype.getSlickOptions.call(this);
            opt.forceFitColumns = true;
            return opt;
        };
        CheckTreeEditor.prototype.sortItems = function () {
            if (!this.moveSelectedUp()) {
                return;
            }
            var oldIndexes = {};
            var list = this.view.getItems();
            var i = 0;
            for (var $t1 = 0; $t1 < list.length; $t1++) {
                var x = list[$t1];
                oldIndexes[x.id] = i++;
            }
            list.sort(function (x1, y) {
                if (x1.isSelected && !y.isSelected) {
                    return -1;
                }
                if (y.isSelected && !x1.isSelected) {
                    return 1;
                }
                var c = Q.turkishLocaleCompare(x1.text, y.text);
                if (c !== 0) {
                    return c;
                }
                return ss.compare(oldIndexes[x1.id], oldIndexes[y.id]);
            });
            this.view.setItems(list, true);
        };
        CheckTreeEditor.prototype.moveSelectedUp = function () {
            return false;
        };
        CheckTreeEditor.prototype.get_value = function () {
            var list = [];
            var items = this.view.getItems();
            for (var i = 0; i < items.length; i++) {
                if (items[i].isSelected) {
                    list.push(items[i].id);
                }
            }
            return list;
        };
        Object.defineProperty(CheckTreeEditor.prototype, "value", {
            get: function () {
                return this.get_value();
            },
            set: function (v) {
                this.set_value(v);
            },
            enumerable: true,
            configurable: true
        });
        CheckTreeEditor.prototype.set_value = function (value) {
            var selected = {};
            if (value != null) {
                for (var i = 0; i < value.length; i++) {
                    selected[value[i]] = true;
                }
            }
            this.view.beginUpdate();
            try {
                var items = this.view.getItems();
                for (var i1 = 0; i1 < items.length; i1++) {
                    var item = items[i1];
                    var select = selected[item.id];
                    if (select !== item.isSelected) {
                        item.isSelected = select;
                        this.view.updateItem(item.id, item);
                    }
                }
                this.updateSelectAll();
                this.updateFlags();
                this.sortItems();
            }
            finally {
                this.view.endUpdate();
            }
        };
        CheckTreeEditor = __decorate([
            Serenity.Decorators.registerEditor('Serenity.CheckTreeEditor', [Serenity.IGetEditValue, Serenity.ISetEditValue]),
            Serenity.Decorators.element("<div/>")
        ], CheckTreeEditor);
        return CheckTreeEditor;
    }(Serenity.DataGrid));
    Serenity.CheckTreeEditor = CheckTreeEditor;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var ColumnPickerDialog = /** @class */ (function (_super) {
        __extends(ColumnPickerDialog, _super);
        function ColumnPickerDialog() {
            var _this = _super.call(this) || this;
            new Serenity.QuickSearchInput(_this.byId("Search"), {
                onSearch: function (fld, txt, done) {
                    txt = Q.trimToNull(txt);
                    if (txt != null)
                        txt = Select2.util.stripDiacritics(txt.toLowerCase());
                    _this.element.find('li').each(function (x, e) {
                        $(e).toggle(!txt || Select2.util.stripDiacritics($(e).text().toLowerCase()).indexOf(txt) >= 0);
                    });
                }
            });
            _this.ulVisible = _this.byId("VisibleCols");
            _this.ulHidden = _this.byId("HiddenCols");
            return _this;
        }
        ColumnPickerDialog.createToolButton = function (grid) {
            function onClick() {
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
                Q.Router.dialog(grid.element, picker.element, function () { return "columns"; });
                picker.dialogOpen();
            }
            grid.element.on('handleroute.' + grid.uniqueName, function (e, arg) {
                if (arg && !arg.handled && arg.route == "columns") {
                    onClick();
                }
            });
            return {
                hint: Q.text("Controls.ColumnPickerDialog.Title"),
                cssClass: "column-picker-button",
                onClick: onClick
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
            return $("\n<li data-key=\"" + col.id + "\">\n  <span class=\"drag-handle\">\u2630</span>\n  " + Q.htmlEncode(this.getTitle(col)) + "\n  <i class=\"js-hide\" title=\"" + Q.text("Controls.ColumnPickerDialog.HideHint") + "\">\u2716</i>\n  <i class=\"js-show fa fa-eye\" title=\"" + Q.text("Controls.ColumnPickerDialog.ShowHint") + "\"></i>\n</li>");
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
                if (!visible[c_2.id] && (!c_2.sourceItem ||
                    (c_2.sourceItem.filterOnly !== true &&
                        (c_2.sourceItem.readPermission == null || Q.Authorization.hasPermission(c_2.sourceItem.readPermission))))) {
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
            return "\n<div class=\"search\"><input id=\"~_Search\" type=\"text\" disabled /></div>\n<div class=\"columns-container\">\n<div class=\"column-list visible-list bg-success\">\n  <h5><i class=\"fa fa-eye\"></i> " + Q.text("Controls.ColumnPickerDialog.VisibleColumns") + "</h5>\n  <ul id=\"~_VisibleCols\"></ul>\n</div>\n<div class=\"column-list hidden-list bg-info\">\n  <h5><i class=\"fa fa-list\"></i> " + Q.text("Controls.ColumnPickerDialog.HiddenColumns") + "</h5>\n  <ul id=\"~_HiddenCols\"></ul>\n</div>\n</div>";
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
var Serenity;
(function (Serenity) {
    /**
     * A mixin that can be applied to a DataGrid for tree functionality
     */
    var TreeGridMixin = /** @class */ (function () {
        function TreeGridMixin(options) {
            this.options = options;
            var dg = this.dataGrid = options.grid;
            var idProperty = dg.getIdProperty();
            var getId = this.getId = function (item) { return item[idProperty]; };
            dg.element.find('.grid-container').on('click', function (e) {
                if ($(e.target).hasClass('s-TreeToggle')) {
                    var src = dg.slickGrid.getCellFromEvent(e);
                    if (src.cell >= 0 &&
                        src.row >= 0) {
                        Serenity.SlickTreeHelper.toggleClick(e, src.row, src.row, dg.view, getId);
                    }
                }
            });
            var oldViewFilter = dg.onViewFilter;
            dg.onViewFilter = function (item) {
                if (!oldViewFilter.apply(this, [item]))
                    return false;
                return Serenity.SlickTreeHelper.filterById(item, dg.view, options.getParentId);
            };
            var oldProcessData = dg.onViewProcessData;
            dg.onViewProcessData = function (response) {
                response = oldProcessData.apply(this, [response]);
                response.Entities = TreeGridMixin.applyTreeOrdering(response.Entities, getId, options.getParentId);
                Serenity.SlickTreeHelper.setIndents(response.Entities, getId, options.getParentId, (options.initialCollapse && options.initialCollapse()) || false);
                return response;
            };
            if (options.toggleField) {
                var col = Q.first(dg.getGrid().getColumns(), function (x) { return x.field == options.toggleField; });
                col.format = Serenity.SlickFormatting.treeToggle(function () { return dg.view; }, getId, col.format || (function (ctx) { return Q.htmlEncode(ctx.value); }));
                col.formatter = Serenity.SlickHelper.convertToFormatter(col.format);
            }
        }
        /**
         * Expands / collapses all rows in a grid automatically
         */
        TreeGridMixin.prototype.toggleAll = function () {
            Serenity.SlickTreeHelper.setCollapsed(this.dataGrid.view.getItems(), !this.dataGrid.view.getItems().every(function (x) { return x._collapsed == true; }));
            this.dataGrid.view.setItems(this.dataGrid.view.getItems(), true);
        };
        /**
         * Reorders a set of items so that parents comes before their children.
         * This method is required for proper tree ordering, as it is not so easy to perform with SQL.
         * @param items list of items to be ordered
         * @param getId a delegate to get ID of a record (must return same ID with grid identity field)
         * @param getParentId a delegate to get parent ID of a record
         */
        TreeGridMixin.applyTreeOrdering = function (items, getId, getParentId) {
            var result = [];
            var byId = Q.toGrouping(items, getId);
            var byParentId = Q.toGrouping(items, getParentId);
            var visited = {};
            function takeChildren(theParentId) {
                if (visited[theParentId])
                    return;
                visited[theParentId] = true;
                for (var _i = 0, _a = (byParentId[theParentId] || []); _i < _a.length; _i++) {
                    var child = _a[_i];
                    result.push(child);
                    takeChildren(getId(child));
                }
            }
            for (var _i = 0, items_2 = items; _i < items_2.length; _i++) {
                var item = items_2[_i];
                var parentId = getParentId(item);
                if (parentId == null ||
                    !((byId[parentId] || []).length)) {
                    result.push(item);
                    takeChildren(getId(item));
                }
            }
            return result;
        };
        return TreeGridMixin;
    }());
    Serenity.TreeGridMixin = TreeGridMixin;
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
(function (Slick) {
    var RemoteView = /** @class */ (function () {
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
                if (suspend) {
                    recalc(items);
                }
                else {
                    refresh();
                }
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
                    //"}"
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
                    //"}"
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
            var o = this.optional(element);
            if (o)
                return o;
            var d = Q.parseDate(value);
            if (!d)
                return false;
            var z = new Date(d);
            z.setHours(0, 0, 0, 0);
            return z.getTime() === d.getTime();
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
            invalidHandler: function (event, validator) {
                Q.notifyError(Q.text("Validation.InvalidFormMessage"));
                $(validator.errorList.map(function (x) { return x.element; }))
                    .closest('.category.collapsed')
                    .children('.category-title')
                    .each(function (i, x) {
                    $(x).click();
                    return true;
                });
                if (validator.errorList.length) {
                    var el = validator.errorList[0].element;
                    if (el) {
                        var bsPane = $(el).closest('.tab-pane');
                        if (!bsPane.hasClass("active") &&
                            bsPane.parent().hasClass('tab-content')) {
                            var bsPaneId = bsPane.attr('id');
                            if (bsPaneId) {
                                $('a[href="#' + bsPaneId + '"]').click();
                            }
                        }
                        if ($.fn.tooltip) {
                            $.fn.tooltip && $(el).tooltip({
                                title: validator.errorList[0].message,
                                trigger: 'manual'
                            }).tooltip('show');
                            window.setTimeout(function () {
                                $(el).tooltip('destroy');
                            }, 1500);
                        }
                    }
                }
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
            buttonImage: 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNvyMY98AAAHNSURBVEhLtZU/S8NAGIf7FfpJHDoX136ADp3s4qDgUpeCDlJEEIcubk7ZFJ06OBRcdJAOdhCx6CAWodJFrP8KFexrnhcv3sVLVYyBp/fLXe75hbSkGRFxaLVaEgSBMMbXkpi0Rz8eVlbkrlhUdnM56XQ6Opq57/DtwRkVMCFMhJzn89JoNHQ0c9/h26NOU3BdKMh4eVneFhflcGpK1rNZHTn/CfE9uHA6BSyMFhZkODsrt2E7I+c/Ib4Hl1NwOT0dXcgx2N6Wh+FQx16vp/O/ARdO3FrAs2PhcWZGm3l+9sj8b8Cl34ddwMKgVEoFXE5BUK9zkurx4fwseBuPUyWxYKfZTCQIf+txtvb2HLwFm7WaTg5fX1V0cnWlxPPR2ZmC2GSkB+22QsaBC6dTwMLzaPTnAhy4vhSwcP/yEl1s2D8+ngjFNjhwJRZwt9wB2Jm79WVuws4TC3qDgUrJYGekvozUzjjITsFGtarNpsBw2u0654ghnhEbcODCmVjAewjsjMyXkdo5seDm6Uku+n2VcgHYGakvI7UzDlxOwWql4hT4QOoDqY0pwBkVLM3NRQVpgAtnVFApl3UyTXBGBWvhX9x8+JpNE5xRwf8hmXe+7B9dZrOuOwAAAABJRU5ErkJggg==',
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
    function jQueryUIInitialization() {
        $.ui.dialog.prototype._allowInteraction = function (event) {
            if ($(event.target).closest(".ui-dialog").length) {
                return true;
            }
            return !!$(event.target).closest(".ui-datepicker, .select2-drop, .cke, .cke_dialog, #support-modal").length;
        };
        (function (orig) {
            $.ui.dialog.prototype._focusTabbable = function () {
                if ($(document.body).hasClass('mobile-device')) {
                    this.uiDialog && this.uiDialog.focus();
                    return;
                }
                orig.call(this);
            };
        })($.ui.dialog.prototype._focusTabbable);
        (function (orig) {
            $.ui.dialog.prototype._createTitlebar = function () {
                orig.call(this);
                this.uiDialogTitlebar.find('.ui-dialog-titlebar-close').html('<i class="fa fa-times" />');
            };
        })($.ui.dialog.prototype._createTitlebar);
    }
    ;
    if (jQuery.ui) {
        jQueryUIInitialization();
    }
    else {
        jQuery(function () {
            if (jQuery.ui)
                jQueryUIInitialization();
        });
    }
    jQuery.cleanData = (function (orig) {
        return function (elems) {
            var events, elem, i, e;
            var cloned = elems;
            for (i = 0; (elem = cloned[i]) != null; i++) {
                try {
                    events = $._data(elem, "events");
                    if (events && events.remove) {
                        // html collection might change during remove event, so clone it!
                        if (cloned === elems)
                            cloned = Array.prototype.slice.call(elems);
                        $(elem).triggerHandler("remove");
                        delete events.remove;
                    }
                }
                catch (e) { }
            }
            orig(elems);
        };
    })(jQuery.cleanData);
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
    function vueInitialization() {
        Vue.component('editor', {
            props: {
                type: {
                    type: String,
                    required: true,
                },
                id: {
                    type: String,
                    required: false
                },
                name: {
                    type: String,
                    required: false
                },
                placeholder: {
                    type: String,
                    required: false
                },
                value: {
                    required: false
                },
                options: {
                    required: false
                },
                maxLength: {
                    required: false
                }
            },
            render: function (createElement) {
                var editorType = Serenity.EditorTypeRegistry.get(this.type);
                var elementAttr = ss.getAttributes(editorType, Serenity.ElementAttribute, true);
                var elementHtml = ((elementAttr.length > 0) ? elementAttr[0].value : '<input/>');
                var domProps = {};
                var element = $(elementHtml)[0];
                var attrs = element.attributes;
                for (var i = 0; i < attrs.length; i++) {
                    var attr = attrs.item(i);
                    domProps[attr.name] = attr.value;
                }
                if (this.id != null)
                    domProps.id = this.id;
                if (this.name != null)
                    domProps.name = this.name;
                if (this.placeholder != null)
                    domProps.placeholder = this.placeholder;
                var editorParams = this.options;
                var optionsType = null;
                var self = this;
                var el = createElement(element.tagName, {
                    domProps: domProps
                });
                this.$editorType = editorType;
                return el;
            },
            watch: {
                value: function (v) {
                    Serenity.EditorUtils.setValue(this.$widget, v);
                }
            },
            mounted: function () {
                var self = this;
                this.$widget = new this.$editorType($(this.$el), this.options);
                this.$widget.initialize();
                if (this.maxLength) {
                    Serenity.PropertyGrid.$setMaxLength(this.$widget, this.maxLength);
                }
                if (this.options)
                    Serenity.ReflectionOptionsSetter.set(this.$widget, this.options);
                if (this.value != null)
                    Serenity.EditorUtils.setValue(this.$widget, this.value);
                if ($(this.$el).data('select2'))
                    Serenity.WX.changeSelect2(this.$widget, function () {
                        self.$emit('input', Serenity.EditorUtils.getValue(self.$widget));
                    });
                else
                    Serenity.WX.change(this.$widget, function () {
                        self.$emit('input', Serenity.EditorUtils.getValue(self.$widget));
                    });
            },
            destroyed: function () {
                if (this.$widget) {
                    this.$widget.destroy();
                    this.$widget = null;
                }
            }
        });
    }
    window['Vue'] ? vueInitialization() : $(function () { window['Vue'] && vueInitialization(); });
})(Q || (Q = {}));
var Serenity;
(function (Serenity) {
    var AsyncLookupEditor = /** @class */ (function (_super) {
        __extends(AsyncLookupEditor, _super);
        function AsyncLookupEditor(hidden, opt) {
            return _super.call(this, hidden, opt) || this;
        }
        AsyncLookupEditor.prototype.getLookupKey = function () {
            return Q.coalesce(this.options.lookupKey, _super.prototype.getLookupKey.call(this));
        };
        AsyncLookupEditor = __decorate([
            Serenity.Decorators.registerEditor('Serenity.AsyncLookupEditor', [Serenity.ISetEditValue, Serenity.IGetEditValue, Serenity.IStringValue, Serenity.IReadOnly, Serenity.IAsyncInit])
        ], AsyncLookupEditor);
        return AsyncLookupEditor;
    }(Serenity.LookupEditorBase));
    Serenity.AsyncLookupEditor = AsyncLookupEditor;
})(Serenity || (Serenity = {}));
(function (Serenity) {
    var FilterPanels;
    (function (FilterPanels) {
        var FieldSelect = /** @class */ (function (_super) {
            __extends(FieldSelect, _super);
            function FieldSelect(hidden, fields) {
                var _this = _super.call(this, hidden) || this;
                for (var _i = 0, fields_1 = fields; _i < fields_1.length; _i++) {
                    var field = fields_1[_i];
                    _this.addOption(field.name, Q.coalesce(Q.tryGetText(field.title), Q.coalesce(field.title, field.name)), field);
                }
                return _this;
            }
            FieldSelect.prototype.emptyItemText = function () {
                if (Q.isEmptyOrNull(this.value)) {
                    return Q.text('Controls.FilterPanel.SelectField');
                }
                return null;
            };
            FieldSelect.prototype.getSelect2Options = function () {
                var opt = _super.prototype.getSelect2Options.call(this);
                opt.allowClear = false;
                return opt;
            };
            FieldSelect = __decorate([
                Serenity.Decorators.registerClass('Serenity.FilterPanels.FieldSelect', [Serenity.ISetEditValue, Serenity.IGetEditValue, Serenity.IStringValue, Serenity.IReadOnly])
            ], FieldSelect);
            return FieldSelect;
        }(Serenity.Select2Editor));
        FilterPanels.FieldSelect = FieldSelect;
    })(FilterPanels = Serenity.FilterPanels || (Serenity.FilterPanels = {}));
})(Serenity || (Serenity = {}));
(function (Serenity) {
    var FilterPanels;
    (function (FilterPanels) {
        var OperatorSelect = /** @class */ (function (_super) {
            __extends(OperatorSelect, _super);
            function OperatorSelect(hidden, source) {
                var _this = _super.call(this, hidden) || this;
                for (var _i = 0, source_1 = source; _i < source_1.length; _i++) {
                    var op = source_1[_i];
                    var title = Q.coalesce(op.title, Q.coalesce(Q.tryGetText("Controls.FilterPanel.OperatorNames." + op.key), op.key));
                    _this.addOption(op.key, title, op);
                }
                if (source.length && source[0])
                    _this.value = source[0].key;
                return _this;
            }
            OperatorSelect.prototype.emptyItemText = function () {
                return null;
            };
            OperatorSelect.prototype.getSelect2Options = function () {
                var opt = _super.prototype.getSelect2Options.call(this);
                opt.allowClear = false;
                return opt;
            };
            OperatorSelect = __decorate([
                Serenity.Decorators.registerClass('Serenity.FilterPanels.FieldSelect', [Serenity.ISetEditValue, Serenity.IGetEditValue, Serenity.IStringValue, Serenity.IReadOnly])
            ], OperatorSelect);
            return OperatorSelect;
        }(Serenity.Select2Editor));
        FilterPanels.OperatorSelect = OperatorSelect;
    })(FilterPanels = Serenity.FilterPanels || (Serenity.FilterPanels = {}));
})(Serenity || (Serenity = {}));
(function (Serenity) {
    var BooleanFiltering = /** @class */ (function (_super) {
        __extends(BooleanFiltering, _super);
        function BooleanFiltering() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        BooleanFiltering.prototype.getOperators = function () {
            return this.appendNullableOperators([
                { key: Serenity.FilterOperators.isTrue },
                { key: Serenity.FilterOperators.isFalse }
            ]);
        };
        BooleanFiltering = __decorate([
            Serenity.Decorators.registerClass('Serenity.BooleanFiltering')
        ], BooleanFiltering);
        return BooleanFiltering;
    }(Serenity.BaseFiltering));
    Serenity.BooleanFiltering = BooleanFiltering;
})(Serenity || (Serenity = {}));
(function (Serenity) {
    var BooleanFormatter = /** @class */ (function () {
        function BooleanFormatter() {
        }
        BooleanFormatter.prototype.format = function (ctx) {
            if (ctx.value == null) {
                return '';
            }
            var text;
            if (!!ctx.value) {
                text = Q.tryGetText(this.trueText);
                if (text == null) {
                    text = this.trueText;
                    if (text == null) {
                        text = Q.coalesce(Q.tryGetText('Dialogs.YesButton'), 'Yes');
                    }
                }
            }
            else {
                text = Q.tryGetText(this.falseText);
                if (text == null) {
                    text = this.falseText;
                    if (text == null) {
                        text = Q.coalesce(Q.tryGetText('Dialogs.NoButton'), 'No');
                    }
                }
            }
            return Q.htmlEncode(text);
        };
        __decorate([
            Serenity.Decorators.option()
        ], BooleanFormatter.prototype, "falseText", void 0);
        __decorate([
            Serenity.Decorators.option()
        ], BooleanFormatter.prototype, "trueText", void 0);
        BooleanFormatter = __decorate([
            Serenity.Decorators.registerFormatter('Serenity.BooleanFormatter')
        ], BooleanFormatter);
        return BooleanFormatter;
    }());
    Serenity.BooleanFormatter = BooleanFormatter;
})(Serenity || (Serenity = {}));
(function (Serenity) {
    var CascadedWidgetLink = /** @class */ (function () {
        function CascadedWidgetLink(parentType, widget, parentChange) {
            var _this = this;
            this.parentType = parentType;
            this.widget = widget;
            this.parentChange = parentChange;
            this.bind();
            this.widget.element.bind('remove.' + widget.uniqueName + 'cwh', function (e) {
                _this.unbind();
                _this.widget = null;
                _this.parentChange = null;
            });
        }
        CascadedWidgetLink.prototype.bind = function () {
            var _this = this;
            if (Q.isEmptyOrNull(this._parentID)) {
                return null;
            }
            var parent = Q.findElementWithRelativeId(this.widget.element, this._parentID)
                .tryGetWidget(this.parentType);
            if (parent != null) {
                parent.element.bind('change.' + this.widget.uniqueName, function () {
                    _this.parentChange(parent);
                });
                return parent;
            }
            else {
                Q.notifyError("Can't find cascaded parent element with ID: " + this._parentID + '!', '', null);
                return null;
            }
        };
        CascadedWidgetLink.prototype.unbind = function () {
            if (Q.isEmptyOrNull(this._parentID)) {
                return null;
            }
            var parent = Q.findElementWithRelativeId(this.widget.element, this._parentID).tryGetWidget(this.parentType);
            if (parent != null) {
                parent.element.unbind('.' + this.widget.uniqueName);
            }
            return parent;
        };
        CascadedWidgetLink.prototype.get_parentID = function () {
            return this._parentID;
        };
        CascadedWidgetLink.prototype.set_parentID = function (value) {
            if (this._parentID !== value) {
                this.unbind();
                this._parentID = value;
                this.bind();
            }
        };
        CascadedWidgetLink = __decorate([
            Serenity.Decorators.registerClass('Serenity.CascadedWidgetLink')
        ], CascadedWidgetLink);
        return CascadedWidgetLink;
    }());
    Serenity.CascadedWidgetLink = CascadedWidgetLink;
})(Serenity || (Serenity = {}));
(function (Serenity) {
    interface;
})(Serenity || (Serenity = {}));
(function (Serenity) {
    var CategoryAttribute = /** @class */ (function () {
        function CategoryAttribute(category) {
            this.category = category;
        }
        CategoryAttribute = __decorate([
            Serenity.Decorators.registerClass('Serenity.CategoryAttribute')
        ], CategoryAttribute);
        return CategoryAttribute;
    }());
    Serenity.CategoryAttribute = CategoryAttribute;
})(Serenity || (Serenity = {}));
(function (Serenity) {
    var CheckboxFormatter = /** @class */ (function () {
        function CheckboxFormatter() {
        }
        CheckboxFormatter.prototype.format = function (ctx) {
            return '<span class="check-box no-float readonly ' + (!!ctx.value ? ' checked' : '') + '"></span>';
        };
        CheckboxFormatter = __decorate([
            Serenity.Decorators.registerFormatter('Serenity.CheckboxFormatter')
        ], CheckboxFormatter);
        return CheckboxFormatter;
    }());
    Serenity.CheckboxFormatter = CheckboxFormatter;
})(Serenity || (Serenity = {}));
(function (Serenity) {
    var CollapsibleAttribute = /** @class */ (function () {
        function CollapsibleAttribute(value) {
            this.value = value;
        }
        CollapsibleAttribute = __decorate([
            Serenity.Decorators.registerClass('Serenity.CollapsibleAttribute')
        ], CollapsibleAttribute);
        return CollapsibleAttribute;
    }());
    Serenity.CollapsibleAttribute = CollapsibleAttribute;
})(Serenity || (Serenity = {}));
(function (Serenity) {
    var CssClassAttribute = /** @class */ (function () {
        function CssClassAttribute(cssClass) {
            this.cssClass = cssClass;
        }
        CssClassAttribute = __decorate([
            Serenity.Decorators.registerClass('Serenity.CssClassAttribute')
        ], CssClassAttribute);
        return CssClassAttribute;
    }());
    Serenity.CssClassAttribute = CssClassAttribute;
})(Serenity || (Serenity = {}));
//# sourceMappingURL=Serenity.CoreLib.js.map