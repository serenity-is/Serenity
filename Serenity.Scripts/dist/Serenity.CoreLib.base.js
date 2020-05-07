var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    // @ts-ignore check for Reflect
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function")
        r = Reflect.decorate(decorators, target, key, desc);
    else
        for (var i = decorators.length - 1; i >= 0; i--)
            if (d = decorators[i])
                r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __skipExtends = {
    "__metadata": true,
    "__typeName": true,
    "__componentFactory": true
};
var __extends = function (d, b) {
    for (var p in b)
        if (b.hasOwnProperty(p) && __skipExtends[p] !== true)
            d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var __assign = Object.assign || function (t) {
    for (var s, i = 1, n = arguments.length; i < n; i++) {
        s = arguments[i];
        for (var p in s)
            if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
    }
    return t;
};
var __rest = function (s, e) {
    var t = {};
    for (var p in s)
        if (Object.prototype.hasOwnProperty.call(s, p) && e.indexOf(p) < 0)
            t[p] = s[p];
    if (s != null && typeof Object.getOwnPropertySymbols === "function")
        for (var i = 0, p = Object.getOwnPropertySymbols(s); i < p.length; i++)
            if (e.indexOf(p[i]) < 0)
                t[p[i]] = s[p[i]];
    return t;
};
var __spreadArrays = function () {
    for (var s = 0, i = 0, il = arguments.length; i < il; i++)
        s += arguments[i].length;
    for (var r = Array(s), k = 0, i = 0; i < il; i++)
        for (var a = arguments[i], j = 0, jl = a.length; j < jl; j++, k++)
            r[k] = a[j];
    return r;
};
/**
 * Represents the completion of an asynchronous operation
 */
if (typeof Promise === "undefined") {
    // @ts-ignore check for global
    if (typeof (RSVP) !== "undefined")
        Promise = RSVP;
    else if (typeof ($) !== "undefined") {
        Promise = $.Deferred;
        Promise.resolve = function (value) {
            return $.Deferred().resolveWith(value);
        };
    }
}
var Serenity;
(function (Serenity) {
    var ISlickFormatter = /** @class */ (function () {
        function ISlickFormatter() {
        }
        return ISlickFormatter;
    }());
    Serenity.ISlickFormatter = ISlickFormatter;
})(Serenity || (Serenity = {}));
// @ts-ignore check for global
var globalObj = typeof (global) !== "undefined" ? global : (typeof (window) !== "undefined" ? window : (typeof (self) !== "undefined" ? self : this));
var Q;
(function (Q) {
    Q.types = {};
    function getNested(from, name) {
        var a = name.split('.');
        for (var i = 0; i < a.length; i++) {
            from = from[a[i]];
            if (from == null)
                return null;
        }
        return from;
    }
    Q.getNested = getNested;
    Q.getType = function (name, target) {
        if (target == null)
            return Q.types[name];
        target = getNested(target, name);
        if (typeof target !== 'function')
            return null;
        return target;
    };
    Q.getTypeFullName = function (type) {
        return type.__typeName || type.name ||
            (type.toString().match(/^\s*function\s*([^\s(]+)/) || [])[1] || 'Object';
    };
    Q.getTypeName = function (type) {
        var fullName = Q.getTypeFullName(type);
        var bIndex = fullName.indexOf('[');
        var nsIndex = fullName.lastIndexOf('.', bIndex >= 0 ? bIndex : fullName.length);
        return nsIndex > 0 ? fullName.substr(nsIndex + 1) : fullName;
    };
    Q.getInstanceType = function (instance) {
        if (instance == null)
            throw new NullReferenceException('Cannot get type of null');
        // Have to catch as constructor cannot be looked up on native COM objects
        try {
            return instance.constructor;
        }
        catch (ex) {
            return Object;
        }
    };
    Q.isAssignableFrom = function (target, type) {
        return target === type ||
            (typeof (target.isAssignableFrom) === 'function' && target.isAssignableFrom(type)) ||
            type.prototype instanceof target;
    };
    Q.isInstanceOfType = function (instance, type) {
        if (instance == null)
            return false;
        if (typeof (type.isInstanceOfType) === 'function')
            return type.isInstanceOfType(instance);
        return Q.isAssignableFrom(type, Q.getInstanceType(instance));
    };
    Q.safeCast = function (instance, type) {
        return Q.isInstanceOfType(instance, type) ? instance : null;
    };
    Q.cast = function (instance, type) {
        if (instance == null)
            return instance;
        else if (Q.isInstanceOfType(instance, type))
            return instance;
        throw new InvalidCastException('Cannot cast object to type ' + Q.getTypeFullName(type));
    };
    Q.getBaseType = function (type) {
        if (type === Object || type.__interface) {
            return null;
        }
        else if (Object.getPrototypeOf) {
            return Object.getPrototypeOf(type.prototype).constructor;
        }
        else {
            var p = type.prototype;
            if (Object.prototype.hasOwnProperty.call(p, 'constructor')) {
                try {
                    var ownValue = p.constructor;
                    delete p.constructor;
                    return p.constructor;
                }
                finally {
                    p.constructor = ownValue;
                }
            }
            return p.constructor;
        }
    };
    Q.getAttributes = function (type, attrType, inherit) {
        var result = [];
        if (inherit) {
            var b = Q.getBaseType(type);
            if (b) {
                var a = Q.getAttributes(b, attrType, true);
                for (var i = 0; i < a.length; i++) {
                    var t = Q.getInstanceType(a[i]);
                    result.push(a[i]);
                }
            }
        }
        if (type.__metadata && type.__metadata.attr) {
            for (var i = 0; i < type.__metadata.attr.length; i++) {
                var a = type.__metadata.attr[i];
                if (attrType == null || Q.isInstanceOfType(a, attrType)) {
                    var t = Q.getInstanceType(a);
                    for (var j = result.length - 1; j >= 0; j--) {
                        if (Q.isInstanceOfType(result[j], t))
                            result.splice(j, 1);
                    }
                    result.push(a);
                }
            }
        }
        return result;
    };
    Q.getMembers = function (type, memberTypes) {
        var result = [];
        var b = Q.getBaseType(type);
        if (b)
            result = Q.getMembers(b, memberTypes & ~1);
        if (type.__metadata && type.__metadata.members) {
            for (var _i = 0, _a = type.__metadata.members; _i < _a.length; _i++) {
                var m = _a[_i];
                if (memberTypes & m.type)
                    result.push(m);
            }
        }
        return result;
    };
    Q.getTypes = function (from) {
        var result = [];
        if (!from) {
            for (var t in Q.types) {
                if (Q.types.hasOwnProperty(t))
                    result.push(Q.types[t]);
            }
        }
        else {
            var traverse = function (s, n) {
                for (var c in s) {
                    if (s.hasOwnProperty(c))
                        traverse(s[c], c);
                }
                if (typeof (s) === 'function' &&
                    n.charAt(0).toUpperCase() === n.charAt(0) &&
                    n.charAt(0).toLowerCase() !== n.charAt(0))
                    result.push(s);
            };
            traverse(from, '');
        }
        return result;
    };
    var Exception = /** @class */ (function (_super) {
        __extends(Exception, _super);
        function Exception(message) {
            var _this = _super.call(this, message) || this;
            _this.name = "Exception";
            return _this;
        }
        return Exception;
    }(Error));
    Q.Exception = Exception;
    var NullReferenceException = /** @class */ (function (_super) {
        __extends(NullReferenceException, _super);
        function NullReferenceException(message) {
            var _this = _super.call(this, message || 'Object is null.') || this;
            _this.name = "NullReferenceException";
            return _this;
        }
        return NullReferenceException;
    }(Exception));
    Q.NullReferenceException = NullReferenceException;
    var ArgumentNullException = /** @class */ (function (_super) {
        __extends(ArgumentNullException, _super);
        function ArgumentNullException(paramName, message) {
            var _this = _super.call(this, (message || 'Value cannot be null.') + '\nParameter name: ' + paramName) || this;
            _this.name = "ArgumentNullException";
            return _this;
        }
        return ArgumentNullException;
    }(Exception));
    Q.ArgumentNullException = ArgumentNullException;
    var ArgumentOutOfRangeException = /** @class */ (function (_super) {
        __extends(ArgumentOutOfRangeException, _super);
        function ArgumentOutOfRangeException(paramName, message) {
            var _this = _super.call(this, (message !== null && message !== void 0 ? message : 'Value is out of range.') +
                (paramName ? ('\nParameter name: ' + paramName) : "")) || this;
            _this.name = "ArgumentNullException";
            return _this;
        }
        return ArgumentOutOfRangeException;
    }(Exception));
    Q.ArgumentOutOfRangeException = ArgumentOutOfRangeException;
    var InvalidCastException = /** @class */ (function (_super) {
        __extends(InvalidCastException, _super);
        function InvalidCastException(message) {
            var _this = _super.call(this, message) || this;
            _this.name = "InvalidCastException";
            return _this;
        }
        return InvalidCastException;
    }(Exception));
    Q.InvalidCastException = InvalidCastException;
    Q.clearKeys = function (d) {
        for (var n in d) {
            if (d.hasOwnProperty(n))
                delete d[n];
        }
    };
    Q.delegateCombine = function (delegate1, delegate2) {
        if (!delegate1) {
            if (!delegate2._targets) {
                return delegate2;
            }
            return delegate2;
        }
        if (!delegate2) {
            if (!delegate1._targets) {
                return delegate1;
            }
            return delegate1;
        }
        var targets1 = delegate1._targets ? delegate1._targets : [null, delegate1];
        var targets2 = delegate2._targets ? delegate2._targets : [null, delegate2];
        return _mkdel(targets1.concat(targets2));
    };
    var Enum;
    (function (Enum) {
        Enum.toString = function (enumType, value) {
            var values = enumType.prototype;
            if (value === 0 || !enumType.__metadata && !enumType.__metadata.enumFlags) {
                for (var i in values) {
                    if (values[i] === value) {
                        return i;
                    }
                }
                return value == null ? "" : value.toString();
            }
            else {
                var parts = [];
                for (var i in values) {
                    if (values[i] & value) {
                        parts.push(i);
                    }
                    else
                        parts.push(value == null ? "" : value.toString());
                }
                return parts.join(' | ');
            }
        };
        Enum.getValues = function (enumType) {
            var parts = [];
            var values = enumType.prototype;
            for (var i in values) {
                if (values.hasOwnProperty(i))
                    parts.push(values[i]);
            }
            return parts;
        };
    })(Enum = Q.Enum || (Q.Enum = {}));
    function delegateContains(targets, object, method) {
        for (var i = 0; i < targets.length; i += 2) {
            if (targets[i] === object && targets[i + 1] === method) {
                return true;
            }
        }
        return false;
    }
    ;
    var _mkdel = function (targets) {
        var delegate = function () {
            if (targets.length == 2) {
                return targets[1].apply(targets[0], arguments);
            }
            else {
                var clone = targets.slice();
                for (var i = 0; i < clone.length; i += 2) {
                    if (delegateContains(targets, clone[i], clone[i + 1])) {
                        clone[i + 1].apply(clone[i], arguments);
                    }
                }
                return null;
            }
        };
        delegate._targets = targets;
        return delegate;
    };
    Q.delegateRemove = function (delegate1, delegate2) {
        if (!delegate1 || (delegate1 === delegate2)) {
            return null;
        }
        if (!delegate2) {
            return delegate1;
        }
        var targets = delegate1._targets;
        var object = null;
        var method;
        if (delegate2._targets) {
            object = delegate2._targets[0];
            method = delegate2._targets[1];
        }
        else {
            method = delegate2;
        }
        for (var i = 0; i < targets.length; i += 2) {
            if ((targets[i] === object) && (targets[i + 1] === method)) {
                if (targets.length == 2) {
                    return null;
                }
                var t = targets.slice();
                t.splice(i, 2);
                return _mkdel(t);
            }
        }
        return delegate1;
    };
    Q.isEnum = function (type) {
        return !!type.__enum;
    };
    function initFormType(typ, nameWidgetPairs) {
        for (var i = 0; i < nameWidgetPairs.length - 1; i += 2) {
            (function (name, widget) {
                Object.defineProperty(typ.prototype, name, {
                    get: function () {
                        return this.w(name, widget);
                    },
                    enumerable: true,
                    configurable: true
                });
            })(nameWidgetPairs[i], nameWidgetPairs[i + 1]);
        }
    }
    Q.initFormType = initFormType;
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
    function initializeTypes(root, pre, limit) {
        if (!root)
            return;
        for (var _i = 0, _a = Object.keys(root); _i < _a.length; _i++) {
            var k = _a[_i];
            if (k.charAt(0) < 'A' || k.charAt(0) > 'Z')
                continue;
            if (k.indexOf('$') >= 0)
                continue;
            if (k == "prototype")
                continue;
            if (!root.hasOwnProperty(k))
                continue;
            var obj = root[k];
            if ($.isArray(obj) ||
                root instanceof Date)
                continue;
            var t = typeof (obj);
            if (t == "string" || t == "number")
                continue;
            if ($.isFunction(obj) || (obj.__enum && obj.__register)) {
                if (obj.hasOwnProperty("__typeName") &&
                    !obj.__register)
                    continue;
                if (!obj.__interfaces &&
                    obj.prototype.format &&
                    k.substr(-9) == "Formatter") {
                    obj.__class = true;
                    obj.__interfaces = [Serenity.ISlickFormatter];
                }
                if (!obj.__class) {
                    var baseType = Q.getBaseType(obj);
                    if (baseType && baseType.__class)
                        obj.__class = true;
                }
                if (obj.__class || obj.__enum || obj.__interface) {
                    obj.__typeName = pre + k;
                    Q.types[pre + k] = obj;
                }
                delete obj.__register;
            }
            if (limit > 0)
                initializeTypes(obj, pre + k + ".", limit - 1);
        }
    }
    Q.initializeTypes = initializeTypes;
})(Q || (Q = {}));
var Q;
(function (Q) {
    function coalesce(a, b) {
        return a != null ? a : b;
    }
    Q.coalesce = coalesce;
    function isValue(a) {
        return a != null;
    }
    Q.isValue = isValue;
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
    function insert(obj, index, item) {
        if (obj.insert)
            obj.insert(index, item);
        else if (Object.prototype.toString.call(obj) === '[object Array]')
            obj.splice(index, 0, item);
        else
            throw new Error("Object does not support insert!");
    }
    Q.insert = insert;
    /**
     * Determines if the object is an array
     */
    function isArray(obj) {
        return Object.prototype.toString.call(obj) === '[object Array]';
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
     * Groups an array with keys determined by specified getKey() callback.
     * Resulting object contains group objects in order and a dictionary to access by key.
     */
    function groupBy(items, getKey) {
        var result = {
            byKey: Object.create(null),
            inOrder: []
        };
        for (var index = 0; index < items.length; index++) {
            var item = items[index];
            var key = Q.coalesce(getKey(item), "");
            var group = result.byKey[key];
            if (group === undefined) {
                group = {
                    order: result.inOrder.length,
                    key: key,
                    items: [item],
                    start: index
                };
                result.byKey[key] = group;
                result.inOrder.push(group);
            }
            else {
                group.items.push(item);
            }
        }
        return result;
    }
    Q.groupBy = groupBy;
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
    function endsWith(s, suffix) {
        if (suffix == null || !suffix.length)
            return true;
        if (suffix.length > s.length)
            return false;
        return (s.substr(s.length - suffix.length) == suffix);
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
    function padLeft(s, len, ch) {
        if (ch === void 0) { ch = ' '; }
        if (s["padStart"])
            return s["padStart"](len, ch);
        s = s.toString();
        while (s.length < len)
            s = ch + s;
        return s;
    }
    Q.padLeft = padLeft;
    function startsWith(s, prefix) {
        if (prefix == null || !prefix.length)
            return true;
        if (prefix.length > s.length)
            return false;
        return (s.substr(0, prefix.length) == prefix);
    }
    Q.startsWith = startsWith;
    function toSingleLine(str) {
        return Q.replaceAll(Q.replaceAll(trimToEmpty(str), '\r\n', ' '), '\n', ' ').trim();
    }
    Q.toSingleLine = toSingleLine;
    Q.today = function () {
        var d = new Date();
        return new Date(d.getFullYear(), d.getMonth(), d.getDate());
    };
    Q.trimEnd = function (s) {
        return s.replace(/\s*$/, '');
    };
    Q.trimStart = function (s) {
        return s.replace(/^\s*/, '');
    };
    function trim(s) {
        if (s == null)
            return '';
        return s.replace(new RegExp('^\\s+|\\s+$', 'g'), '');
    }
    Q.trim = trim;
    function trimToEmpty(s) {
        if (s == null || s.length === 0)
            return '';
        return trim(s);
    }
    Q.trimToEmpty = trimToEmpty;
    function trimToNull(s) {
        s = trim(s);
        if (s.length === 0)
            return null;
        return s;
    }
    Q.trimToNull = trimToNull;
    function replaceAll(s, f, r) {
        s = s || '';
        return s.split(f).join(r);
    }
    Q.replaceAll = replaceAll;
    function zeroPad(n, digits) {
        var s = n.toString();
        while (s.length < digits)
            s = "0" + s;
        return s;
    }
    Q.zeroPad = zeroPad;
    /**
     * Returns a function, that, as long as it continues to be invoked, will not
     * be triggered. The function will be called after it stops being called for
     * N milliseconds. If `immediate` is passed, trigger the function on the
     * leading edge, instead of the trailing. The function also has a property 'clear'
     * that is a function which will clear the timer to prevent previously scheduled executions.
     *
     * @source underscore.js
     */
    function debounce(func, wait, immediate) {
        var timeout, args, context, timestamp, result;
        if (null == wait)
            wait = 100;
        function later() {
            var last = Date.now() - timestamp;
            if (last < wait && last >= 0) {
                timeout = setTimeout(later, wait - last);
            }
            else {
                timeout = null;
                if (!immediate) {
                    result = func.apply(context, args);
                    context = args = null;
                }
            }
        }
        ;
        var debounced = function () {
            context = this;
            args = arguments;
            timestamp = Date.now();
            var callNow = immediate && !timeout;
            if (!timeout)
                timeout = setTimeout(later, wait);
            if (callNow) {
                result = func.apply(context, args);
                context = args = null;
            }
            return result;
        };
        debounced.clear = function () {
            if (timeout) {
                clearTimeout(timeout);
                timeout = null;
            }
        };
        debounced.flush = function () {
            if (timeout) {
                result = func.apply(context, args);
                context = args = null;
                clearTimeout(timeout);
                timeout = null;
            }
        };
        return debounced;
    }
    Q.debounce = debounce;
    ;
    function extend(a, b) {
        for (var key in b)
            if (b.hasOwnProperty(key))
                a[key] = b[key];
        return a;
    }
    Q.extend = extend;
    function deepClone(a) {
        if (!a)
            return a;
        var v;
        var b = Array.isArray(a) ? [] : {};
        for (var k in a) {
            v = a[k];
            b[k] = (typeof v === "object") ? deepClone(v) : v;
        }
        return b;
    }
    Q.deepClone = deepClone;
})(Q || (Q = {}));
var Q;
(function (Q) {
    Q.Invariant = {
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
        stringCompare: function (a, b) { return a < b ? -1 : (a > b ? 1 : 0); }
    };
    function compareStringFactory(order) {
        var o = {};
        for (var z = 0; z < order.length; z++) {
            o[order.charAt(z)] = z + 1;
        }
        return function (a, b) {
            a = a || "";
            b = b || "";
            if (a == b)
                return 0;
            for (var i = 0, _len = Math.min(a.length, b.length); i < _len; i++) {
                var x = a.charAt(i), y = b.charAt(i);
                if (x === y)
                    continue;
                var ix = o[x], iy = o[y];
                if (ix != null && iy != null)
                    return ix < iy ? -1 : 1;
                var c = x.localeCompare(y);
                if (c == 0)
                    continue;
                return c;
            }
            return a.localeCompare(b);
        };
    }
    Q.compareStringFactory = compareStringFactory;
    Q.Culture = {
        decimalSeparator: '.',
        groupSeparator: ',',
        dateSeparator: '/',
        dateOrder: 'dmy',
        dateFormat: 'dd/MM/yyyy',
        dateTimeFormat: 'dd/MM/yyyy HH:mm:ss',
        stringCompare: compareStringFactory("AaBbCcÇçFfGgĞğHhIıİiJjKkLlMmNnOoÖöPpRrSsŞşTtUuÜüVvYyZz")
    };
    (function () {
        var k;
        for (k in Q.Invariant)
            if (Q.Culture[k] === undefined && Q.Invariant.hasOwnProperty(k))
                Q.Culture[k] = Q.Invariant[k];
        if (typeof $ != "undefined" && (k = Q.trimToNull($('script#ScriptCulture').html())) != null) {
            var sc = $.parseJSON(k);
            if (sc.DecimalSeparator != null)
                Q.Culture.decimalSeparator = sc.DecimalSeparator;
            if (sc.GroupSeparator != null && sc.GroupSeparator != Q.Culture.decimalSeparator)
                Q.Culture.groupSeparator = sc.GroupSeparator;
            else if (Q.Culture.groupSeparator == Q.Culture.decimalSeparator)
                Q.Culture.groupSeparator = Q.Culture.decimalSeparator == '.' ? ',' : '.';
            delete sc.groupSeparator;
            delete sc.decimal;
            for (k in sc) {
                if (Q.Culture[k] === undefined && sc.hasOwnProperty(k))
                    Q.Culture[k.charAt(0).toLowerCase() + k.substr(1)] = sc[k];
            }
        }
    })();
    function turkishLocaleToUpper(a) {
        if (!a)
            return a;
        return a.replace(/i/g, 'İ').replace(/ı/g, 'I').toUpperCase();
    }
    Q.turkishLocaleToUpper = turkishLocaleToUpper;
    function insertGroupSeperator(num, dec, grp, neg) {
        var decPart = null;
        var decIndex = num.indexOf(dec);
        if (decIndex > 0) {
            decPart = num.substr(decIndex);
            num = num.substr(0, decIndex);
        }
        var negative = Q.startsWith(num, neg);
        if (negative) {
            num = num.substr(1);
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
            var part = num.substr(startIndex, length);
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
    function _formatString(format, l, values, from) {
        return format.replace(_formatRE, function (m) {
            if (m === '{{' || m === '}}')
                return m.charAt(0);
            var index = parseInt(m.substr(1), 10);
            var value = values[index + from];
            if (value == null) {
                return '';
            }
            var type = Q.getInstanceType(value);
            if (type == Number || type == Date) {
                var formatSpec = null;
                var formatIndex = m.indexOf(':');
                if (formatIndex > 0) {
                    formatSpec = m.substring(formatIndex + 1, m.length - 1);
                }
                return _formatObject(value, formatSpec, l);
            }
            else {
                return value.toString();
            }
        });
    }
    ;
    function format(format) {
        var prm = [];
        for (var _i = 1; _i < arguments.length; _i++) {
            prm[_i - 1] = arguments[_i];
        }
        return _formatString(format, Q.Culture, arguments, 1);
    }
    Q.format = format;
    function localeFormat(format, l) {
        var prm = [];
        for (var _i = 2; _i < arguments.length; _i++) {
            prm[_i - 2] = arguments[_i];
        }
        return _formatString(format, l, arguments, 2);
    }
    Q.localeFormat = localeFormat;
    function _formatObject(obj, format, fmt) {
        if (typeof (obj) === 'number')
            return Q.formatNumber(obj, format, fmt);
        else if (Object.prototype.toString.call(obj) === '[object Date]')
            return Q.formatDate(obj, format, fmt);
        else
            return obj.format(format);
    }
    ;
    Q.round = function (n, d, rounding) {
        var m = Math.pow(10, d || 0);
        n *= m;
        var sign = (n > 0) | -(n < 0);
        if (n % 1 === 0.5 * sign) {
            var f = Math.floor(n);
            return (f + (rounding ? (sign > 0) : (f % 2 * sign))) / m;
        }
        return Math.round(n) / m;
    };
    Q.trunc = function (n) { return n != null ? (n > 0 ? Math.floor(n) : Math.ceil(n)) : null; };
    function formatNumber(num, format, decOrLoc, grp) {
        var _a, _b, _c, _d, _e, _f;
        if (num == null)
            return "";
        var fmt = typeof decOrLoc !== "string" ? (decOrLoc !== null && decOrLoc !== void 0 ? decOrLoc : Q.Culture) : {
            decimalSeparator: decOrLoc,
            groupSeparator: grp !== null && grp !== void 0 ? grp : (decOrLoc == "," ? "." : ",")
        };
        if (isNaN(num)) {
            return (_a = fmt.nanSymbol) !== null && _a !== void 0 ? _a : Q.Culture.nanSymbol;
        }
        if (format == null || (format.length == 0) || (format == 'i')) {
            return num.toString();
        }
        var dec = (_b = fmt.decimalSeparator) !== null && _b !== void 0 ? _b : Q.Culture.decimalSeparator;
        grp = (_c = grp !== null && grp !== void 0 ? grp : fmt.groupSeparator) !== null && _c !== void 0 ? _c : Q.Culture.groupSeparator;
        var neg = (_d = fmt.negativeSign) !== null && _d !== void 0 ? _d : Q.Culture.negativeSign;
        var s = '';
        var precision = -1;
        if (format.length > 1) {
            precision = parseInt(format.substr(1), 10);
        }
        var fs = format.charAt(0);
        switch (fs) {
            case 'd':
            case 'D':
                s = parseInt(Math.abs(num)).toString();
                if (precision != -1)
                    s = Q.padLeft(s, precision, '0');
                if (num < 0)
                    s = neg + s;
                break;
            case 'x':
            case 'X':
                s = parseInt(Math.abs(num)).toString(16);
                if (fs == 'X')
                    s = s.toUpperCase();
                if (precision != -1)
                    s = Q.padLeft(s, precision, '0');
                break;
            case 'e':
            case 'E':
                if (precision == -1)
                    s = num.toExponential();
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
                    precision = (_e = fmt.decimalDigits) !== null && _e !== void 0 ? _e : Q.Culture.decimalDigits;
                }
                s = num.toFixed(precision).toString();
                if (precision && (dec != '.')) {
                    var index = s.indexOf('.');
                    s = s.substr(0, index) + dec + s.substr(index + 1);
                }
                if ((fs == 'n') || (fs == 'N')) {
                    s = insertGroupSeperator(s, dec, grp, neg);
                }
                break;
            case 'c':
            case 'C':
            case 'p':
            case 'P':
                if (precision == -1) {
                    precision = (_f = fmt.decimalDigits) !== null && _f !== void 0 ? _f : Q.Culture.decimalDigits;
                }
                if (fs === 'p' || fs == 'P')
                    num *= 100;
                s = Math.abs(num).toFixed(precision).toString();
                if (precision && (dec != '.')) {
                    var index = s.indexOf('.');
                    s = s.substr(0, index) + dec + s.substr(index + 1);
                }
                s = insertGroupSeperator(s, dec, grp, neg);
                s = localeFormat("0", fmt, s);
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
                var r = "";
                if (format.indexOf(".") > -1) {
                    var dp = dec;
                    var df = format.substring(format.lastIndexOf(".") + 1);
                    num = roundNumber(num, df.length);
                    var dv = num % 1;
                    var ds = new String(dv.toFixed(df.length));
                    ds = ds.substring(ds.lastIndexOf(".") + 1);
                    for (var i_1 = 0; i_1 < df.length; i_1++) {
                        if (df.charAt(i_1) == '#' && ds.charAt(i_1) != '0') {
                            dp += ds.charAt(i_1);
                            continue;
                        }
                        else if (df.charAt(i_1) == '#' && ds.charAt(i_1) == '0') {
                            var notParsed = ds.substring(i_1);
                            if (notParsed.match('[1-9]')) {
                                dp += ds.charAt(i_1);
                                continue;
                            }
                            else
                                break;
                        }
                        else if (df.charAt(i_1) == "0")
                            dp += ds.charAt(i_1);
                        else
                            dp += df.charAt(i_1);
                    }
                    r += dp;
                }
                else
                    num = Math.round(num);
                var ones = Math.floor(num);
                if (num < 0)
                    ones = Math.ceil(num);
                var of = "";
                if (format.indexOf(".") == -1)
                    of = format;
                else
                    of = format.substring(0, format.indexOf("."));
                var op = "";
                if (!(ones == 0 && of.substr(of.length - 1) == '#')) {
                    // find how many digits are in the group
                    var oneText = new String(Math.abs(ones));
                    var gl = 9999;
                    if (of.lastIndexOf(",") != -1)
                        gl = of.length - of.lastIndexOf(",") - 1;
                    var gc = 0;
                    for (var i_2 = oneText.length - 1; i_2 > -1; i_2--) {
                        op = oneText.charAt(i_2) + op;
                        gc++;
                        if (gc == gl && i_2 != 0) {
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
                if (num < 0)
                    r = neg + r;
                if (r.lastIndexOf(dec) == r.length - 1) {
                    r = r.substring(0, r.length - 1);
                }
                return prefix + r + suffix;
        }
        return s;
    }
    Q.formatNumber = formatNumber;
    function parseInteger(s) {
        s = Q.trim(s.toString());
        var ts = Q.Culture.groupSeparator;
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
        var ts = Q.Culture.groupSeparator;
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
    var _dateFormatRE = /'.*?[^\\]'|dddd|ddd|dd|d|MMMM|MMM|MM|M|yyyy|yy|y|hh|h|HH|H|mm|m|ss|s|tt|t|fff|ff|f|zzz|zz|z|\//g;
    function formatDate(d, format, locale) {
        var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o;
        if (!d)
            return '';
        var date;
        if (typeof d == "string") {
            var res = Q.parseDate(d, locale == null ? null : locale.dateOrder);
            if (!res)
                return d;
            date = res;
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
            locale = Q.Culture;
        if (format == null || format == "d")
            format = (_a = locale.dateFormat) !== null && _a !== void 0 ? _a : Q.Culture.dateFormat;
        else if (format.length == 1) {
            switch (format) {
                case "g":
                    format = ((_b = locale.dateTimeFormat) !== null && _b !== void 0 ? _b : Q.Culture.dateTimeFormat).replace(":ss", "");
                    break;
                case "G":
                    format = ((_c = locale.dateTimeFormat) !== null && _c !== void 0 ? _c : Q.Culture.dateTimeFormat);
                    break;
                case "s":
                    format = "yyyy-MM-ddTHH:mm:ss";
                    break;
                case 'd': format = ((_d = locale.dateFormat) !== null && _d !== void 0 ? _d : Q.Culture.dateFormat);
                case 't':
                    format = (locale.dateTimeFormat && locale.dateFormat) ? locale.dateTimeFormat.replace(locale.dateFormat + " ", "") : "HH:mm";
                    break;
                case 'u':
                case 'U':
                    format = format == 'u' ? 'yyyy-MM-ddTHH:mm:ss.fffZ' : (_e = locale.dateTimeFormat) !== null && _e !== void 0 ? _e : Q.Culture.dateTimeFormat;
                    date = new Date(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(), date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds(), date.getUTCMilliseconds());
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
            var part = fs;
            switch (fs) {
                case '/':
                    part = (_f = locale.dateSeparator) !== null && _f !== void 0 ? _f : Q.Culture.dateSeparator;
                    break;
                case 'dddd':
                    part = ((_g = locale.dayNames) !== null && _g !== void 0 ? _g : Q.Culture.dayNames)[date.getDay()];
                    break;
                case 'ddd':
                    part = ((_h = locale.shortDayNames) !== null && _h !== void 0 ? _h : Q.Culture.shortDayNames)[date.getDay()];
                    break;
                case 'dd':
                    part = Q.padLeft(date.getDate().toString(), 2, '0');
                    break;
                case 'd':
                    part = date.getDate();
                    break;
                case 'MMMM':
                    part = ((_j = locale.monthNames) !== null && _j !== void 0 ? _j : Q.Culture.monthNames)[date.getMonth()];
                    break;
                case 'MMM':
                    part = ((_k = locale.shortMonthNames) !== null && _k !== void 0 ? _k : Q.Culture.shortMonthNames)[date.getMonth()];
                    break;
                case 'MM':
                    part = Q.padLeft(date.getMonth() + 1, 2, '0');
                    break;
                case 'M':
                    part = (date.getMonth() + 1);
                    break;
                case 'yyyy':
                    part = Q.padLeft(date.getFullYear(), 4, '0');
                    break;
                case 'yy':
                    part = Q.padLeft(date.getFullYear() % 100, 2, '0');
                    break;
                case 'y':
                    part = date.getFullYear() % 100;
                    break;
                case 'h':
                case 'hh':
                    part = date.getHours() % 12;
                    if (!part) {
                        part = '12';
                    }
                    else if (fs == 'hh') {
                        part = Q.padLeft(part, 2, '0');
                    }
                    break;
                case 'HH':
                    part = Q.padLeft(date.getHours(), 2, '0');
                    break;
                case 'H':
                    part = date.getHours();
                    break;
                case 'mm':
                    part = Q.padLeft(date.getMinutes(), 2, '0');
                    break;
                case 'm':
                    part = date.getMinutes();
                    break;
                case 'ss':
                    part = Q.padLeft(date.getSeconds().toString(), 2, '0');
                    break;
                case 's':
                    part = date.getSeconds();
                    break;
                case 't':
                case 'tt':
                    part = (date.getHours() < 12) ? ((_l = locale.amDesignator) !== null && _l !== void 0 ? _l : Q.Culture.amDesignator) : ((_m = locale.pmDesignator) !== null && _m !== void 0 ? _m : Q.Culture.pmDesignator);
                    if (fs == 't') {
                        part = part.charAt(0);
                    }
                    break;
                case 'fff':
                    part = Q.padLeft(date.getMilliseconds(), 3, '0');
                    break;
                case 'ff':
                    part = Q.padLeft(date.getMilliseconds(), 3).substr(0, 2);
                    break;
                case 'f':
                    part = Q.padLeft(date.getMilliseconds(), 3).charAt(0);
                    break;
                case 'z':
                case 'Z':
                    part = date.getTimezoneOffset() / 60;
                    part = ((part >= 0) ? '-' : '+') + Math.floor(Math.abs(part));
                    break;
                case 'zz':
                case 'zzz':
                    part = date.getTimezoneOffset() / 60;
                    part = ((part >= 0) ? '-' : '+') +
                        Math.floor(Q.padLeft(Math.abs(part), 2, '0'));
                    if (fs == 'zzz') {
                        part += ((_o = locale.timeSeparator) !== null && _o !== void 0 ? _o : Q.Culture.timeSeparator) +
                            Math.abs(Q.padLeft(date.getTimezoneOffset() % 60, 2, '0'));
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
            days = parseInteger(p[0]);
            if (!isNaN(days))
                return days * 24 * 60;
            return parseHourAndMin(p[0]);
        }
        else {
            days = parseInteger(p[0]);
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
    function dbText(prefix) {
        return function (key) {
            return text("Db." + prefix + "." + key);
        };
    }
    Q.dbText = dbText;
    function prefixedText(prefix) {
        return function (text, key) {
            if (text != null && !Q.startsWith(text, '`')) {
                var local = Q.tryGetText(text);
                if (local != null) {
                    return local;
                }
            }
            if (text != null && Q.startsWith(text, '`')) {
                text = text.substr(1);
            }
            if (!Q.isEmptyOrNull(prefix)) {
                var textKey = typeof (key) == "function" ? key(prefix) : (prefix + key);
                var localText = Q.tryGetText(textKey);
                if (localText != null) {
                    return localText;
                }
            }
            return text;
        };
    }
    Q.prefixedText = prefixedText;
    function tryGetText(key) {
        return LT.$table[key];
    }
    Q.tryGetText = tryGetText;
    function dbTryText(prefix) {
        return function (key) {
            return text("Db." + prefix + "." + key);
        };
    }
    Q.dbTryText = dbTryText;
    function proxyTexts(o, p, t) {
        if (typeof window != 'undefined' && window['Proxy']) {
            return new window['Proxy'](o, {
                get: function (x, y) {
                    var tv = t[y];
                    if (tv == null)
                        return;
                    if (typeof tv == 'number')
                        return Q.text(p + y);
                    else {
                        var z = o[y];
                        if (z != null)
                            return z;
                        o[y] = z = proxyTexts({}, p + y + '.', tv);
                        return z;
                    }
                },
                ownKeys: function (x) { return Object.keys(t); }
            });
        }
        else {
            for (var _i = 0, _a = Object.keys(t); _i < _a.length; _i++) {
                var k = _a[_i];
                if (typeof t[k] == 'number')
                    Object.defineProperty(o, k, {
                        get: function () { return Q.text(p + k); }
                    });
                else
                    o[k] = proxyTexts({}, p + k + '.', t[k]);
            }
            return o;
        }
    }
    Q.proxyTexts = proxyTexts;
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
        if ($.blockUI) {
            $.blockUI && $.blockUI(opt);
        }
        else {
            $('<div class="blockUI blockOverlay" style="z-index: 2000; border: none; margin: 0px; padding: 0px; width: 100%; height: 100%; top: 0px; left: 0px; opacity: 0; cursor: wait; position: fixed;"></div>')
                .appendTo(document.body);
        }
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
        options = Q.extend({
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
        if ($.unblockUI)
            $.unblockUI({ fadeOut: 0 });
        else
            $(document.body).children('.blockUI.blockOverlay').remove();
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
            throw new Q.Exception('Please make sure that jsrender.js is included in the page!');
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
        if (typeof console !== "undefined" && console.log)
            console.log(m);
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
    // if both jQuery UI and bootstrap button exists, prefer jQuery UI button as UI dialog needs them
    if (typeof $ !== "undefined" && $.fn && $.fn.button && $.ui && $.ui.button && $.fn.button.noConflict) {
        $.fn.btn = $.fn.button.noConflict();
    }
    function toIconClass(icon) {
        if (!icon)
            return null;
        if (Q.startsWith(icon, 'fa-'))
            return 'fa ' + icon;
        if (Q.startsWith(icon, 'glyphicon-'))
            return 'glyphicon ' + icon;
        return icon;
    }
    function uiDialog(options, message, dialogClass) {
        var opt = Q.extend({
            modal: true,
            width: '40%',
            maxWidth: 450,
            minWidth: 180,
            resizable: false,
            open: function () {
                if (options.onOpen)
                    options.onOpen.call(this);
            },
            close: function () {
                $(this).dialog('destroy');
                if (options.onClose)
                    options.onClose.call(this, options.result);
            }
        }, options);
        opt.dialogClass = 's-MessageDialog' + (dialogClass ? (' ' + dialogClass) : '');
        if (options.buttons) {
            opt.buttons = options.buttons.map(function (x) {
                var text = x.htmlEncode == null || x.htmlEncode ? Q.htmlEncode(x.title) : x.title;
                var iconClass = toIconClass(x.icon);
                if (iconClass != null)
                    text = '<i class="' + iconClass + "><i> ";
                return {
                    text: text,
                    click: function (e) {
                        options.result = x.result;
                        $(this).dialog('close');
                        x.onClick && x.onClick.call(this, e);
                    },
                    attr: !x.cssClass ? undefined : {
                        "class": x.cssClass
                    }
                };
            });
        }
        return $('<div>' + message + '</div>').dialog(opt);
    }
    var _isBS3;
    function isBS3() {
        if (_isBS3 != null)
            return _isBS3;
        // @ts-ignore
        return (_isBS3 = !!($.fn.modal && $.fn.modal.Constructor && $.fn.modal.Constructor.VERSION && ($.fn.modal.Constructor.VERSION + "").charAt(0) == '3'));
    }
    Q.isBS3 = isBS3;
    var defaultTxt = {
        AlertTitle: 'Alert',
        InformationTitle: 'Information',
        WarningTitle: 'Warning',
        ConfirmationTitle: 'Confirm',
        OkButton: 'OK',
        YesButton: 'Yes',
        NoButton: 'No',
        CancelButton: 'Cancel',
        CloseButton: 'Close'
    };
    function txt(k) {
        var _a;
        return (_a = Q.tryGetText("Dialogs." + k)) !== null && _a !== void 0 ? _a : defaultTxt[k];
    }
    function bsModal(options, message, modalClass) {
        var closeButton = "<button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"" + txt('CloseButton') + "\">" +
            "<span aria-hidden=\"true\">&times;</span></button>";
        var div = $("<div class=\"modal s-MessageModal " + modalClass + "\" tabindex=\"-1\" role=\"dialog\">\n    <div class=\"modal-dialog\" role=\"document\">\n        <div class=\"modal-content\">\n            <div class=\"modal-header\">\n                " + (isBS3() ? closeButton : "") + "<h5 class=\"modal-title\">" + options.title + "</h5>" + (isBS3() ? "" : closeButton) + "\n            </div>\n            <div class=\"modal-body\">\n                " + message + "\n            </div>\n            <div class=\"modal-footer\"></div>\n        </div>\n    </div>\n</div>").eq(0).appendTo(document.body);
        if (options.onOpen)
            div.one('shown.bs.modal', options.onOpen);
        if (options.onClose)
            div.one('hidden.bs.modal', function (e) { return options.onClose(options.result); });
        var footer = div.find('.modal-footer');
        function createButton(x) {
            var _this = this;
            var text = x.htmlEncode == null || x.htmlEncode ? Q.htmlEncode(x.title) : x.title;
            var iconClass = toIconClass(x.icon);
            if (iconClass != null)
                text = '<i class="' + iconClass + "><i>" + (text ? (" " + text) : "");
            $("<button class=\"btn " + (x.cssClass ? x.cssClass : '') + "\"" + (x.hint ? (' title="' + Q.attrEncode(x.hint) + '"') : '') + ">" + text + "</button>")
                .appendTo(footer)
                .click(function (e) {
                options.result = x.result;
                div.modal('hide');
                x.onClick && x.onClick.call(_this, e);
            });
        }
        if (options.buttons)
            for (var _i = 0, _a = options.buttons; _i < _a.length; _i++) {
                var button = _a[_i];
                createButton(button);
            }
        div.modal({
            backdrop: false,
            show: true
        });
    }
    var _useBrowserDialogs;
    function useBrowserDialogs() {
        if (_useBrowserDialogs == null) {
            _useBrowserDialogs = typeof $ === 'undefined' || ((!$.ui || !$.ui.dialog) && (!$.fn || !$.fn.modal));
        }
        return _useBrowserDialogs;
    }
    function useBSModal(options) {
        return !!((!$.ui || !$.ui.dialog) || Q.Config.bootstrapModals || (options && options.bootstrap));
    }
    function messageHtml(message, options) {
        var htmlEncode = options == null || options.htmlEncode == null || options.htmlEncode;
        if (htmlEncode)
            message = Q.htmlEncode(message);
        var preWrap = options == null || (options.preWrap == null && htmlEncode) || options.preWrap;
        return '<div class="message"' + (preWrap ? ' style="white-space: pre-wrap">' : '>') + message + '</div>';
    }
    function alert(message, options) {
        var _a, _b;
        if (useBrowserDialogs()) {
            window.alert(message);
            return;
        }
        var useBS = useBSModal(options);
        options = Q.extend({
            htmlEncode: true,
            okButton: txt('OkButton'),
            title: txt('AlertTitle')
        }, options);
        if (options.buttons == null) {
            options.buttons = [];
            if (options.okButton == null || options.okButton) {
                options.buttons.push({
                    title: typeof options.okButton == "boolean" ? txt('OkButton') : options.okButton,
                    cssClass: useBS ? 'btn-default' : undefined,
                    result: 'ok'
                });
            }
        }
        message = messageHtml(message, options);
        if (useBS)
            bsModal(options, message, (_a = options.modalClass) !== null && _a !== void 0 ? _a : "s-AlertModal");
        else
            uiDialog(options, message, (_b = options.dialogClass) !== null && _b !== void 0 ? _b : "s-AlertDialog");
    }
    Q.alert = alert;
    function confirm(message, onYes, options) {
        var _a, _b;
        if (useBrowserDialogs()) {
            if (window.confirm(message))
                onYes && onYes();
            return;
        }
        var useBS = useBSModal(options);
        options = Q.extend({
            htmlEncode: true,
            yesButton: txt('YesButton'),
            noButton: txt('NoButton'),
            title: txt('ConfirmationTitle')
        }, options);
        if (options.buttons == null) {
            options.buttons = [];
            if (options.yesButton == null || options.yesButton) {
                options.buttons.push({
                    title: typeof options.yesButton == "boolean" ? txt('YesButton') : options.yesButton,
                    cssClass: useBS ? 'btn-primary' : undefined,
                    result: 'yes',
                    onClick: onYes
                });
            }
            if (options.noButton == null || options.noButton) {
                options.buttons.push({
                    title: typeof options.noButton == "boolean" ? txt('NoButton') : options.noButton,
                    cssClass: useBS ? 'btn-default' : undefined,
                    result: 'no',
                    onClick: options.onNo
                });
            }
            if (options.cancelButton) {
                options.buttons.push({
                    title: typeof options.cancelButton == "boolean" ? txt('CancelButton') : options.cancelButton,
                    cssClass: useBS ? 'btn-default' : undefined,
                    result: 'cancel',
                    onClick: options.onCancel
                });
            }
        }
        message = messageHtml(message, options);
        if (useBS)
            bsModal(options, message, (_a = options.modalClass) !== null && _a !== void 0 ? _a : "s-ConfirmModal");
        else
            uiDialog(options, message, (_b = options.dialogClass) !== null && _b !== void 0 ? _b : "s-ConfirmDialog");
    }
    Q.confirm = confirm;
    function iframeDialog(options) {
        if (useBrowserDialogs()) {
            window.alert(options.html);
            return;
        }
        if (useBSModal(options)) {
            bsModal({
                title: txt('AlertTitle'),
                modalClass: 'modal-lg',
                onOpen: function () {
                    doc = ($(this).find('iframe').css({
                        border: 'none',
                        width: '100%',
                        height: '100%'
                    })[0]).contentDocument;
                    doc.open();
                    doc.write(settings.html);
                    doc.close();
                }
            }, '<div style="overflow: hidden"><iframe></iframe></div>', 's-IFrameModal');
            return;
        }
        var doc;
        var e = $('<div style="overflow: hidden"><iframe></iframe></div>');
        var settings = Q.extend({
            autoOpen: true,
            modal: true,
            width: '60%',
            height: '400',
            title: txt('AlertTitle'),
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
        if (useBrowserDialogs()) {
            window.alert(message);
            onOk && onOk();
            return;
        }
        confirm(message, onOk, Q.extend({
            title: txt("InformationTitle"),
            dialogClass: "s-InformationDialog",
            modalClass: "s-InformationModal",
            yesButton: txt("OkButton"),
            noButton: false,
        }, options));
    }
    Q.information = information;
    function warning(message, options) {
        alert(message, Q.extend({
            title: txt("WarningTitle"),
            dialogClass: "s-WarningDialog",
            modalClass: "s-WarningModal"
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
        options = Q.extend(Q.extend({}, Q.defaultNotifyOptions), options);
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
        function runtimeErrorHandler(message, filename, lineno, colno, error) {
            try {
                var host = (window.location.host || "").toLowerCase();
                if (host.indexOf("localhost") < 0 &&
                    host.indexOf("127.0.0.1") < 0)
                    return;
                if (!window['toastr'])
                    return;
                var errorInfo = JSON.stringify(error || {});
                message =
                    '<p></p><p>Message: ' + Q.htmlEncode(message) +
                        '</p><p>File: ' + Q.htmlEncode(filename) +
                        ', Line: ' + lineno + ', Column: ' + colno +
                        (errorInfo != "{}" ? '</p><p>Error: ' : "") + '</p>';
                window.setTimeout(function () {
                    try {
                        Q.notifyError(message, "SCRIPT ERROR! See browser console (F12) for details.", {
                            escapeHtml: false,
                            timeOut: 15000
                        });
                    }
                    catch (_a) {
                    }
                });
            }
            catch (_a) {
            }
        }
        ErrorHandling.runtimeErrorHandler = runtimeErrorHandler;
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
        if (typeof $ !== 'undefined') {
            var pathLink = $('link#ApplicationPath');
            if (pathLink.length > 0) {
                Config.applicationPath = pathLink.attr('href');
            }
        }
        /**
         * Set this to true, to enable responsive dialogs by default, without having to add Serenity.Decorators.responsive()"
         * on dialog classes manually. It's false by default for backward compability.
         */
        Config.responsiveDialogs = false;
        /**
         * Set this to true, to prefer bootstrap dialogs over jQuery UI dialogs by default
         */
        Config.bootstrapModals = false;
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
    function getCookie(name) {
        if ($.cookie)
            return $.cookie(name);
        name += '=';
        for (var ca = document.cookie.split(/;\s*/), i = ca.length - 1; i >= 0; i--)
            if (!ca[i].indexOf(name))
                return ca[i].replace(name, '');
    }
    Q.getCookie = getCookie;
    typeof $ != 'undefined' && $.ajaxSetup && $.ajaxSetup({
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
            url = resolveUrl("~/services/") + url;
        options = Q.extend({
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
                        if (!xhr.status) {
                            if (xhr.statusText != "abort")
                                Q.alert("An unknown AJAX connection error occurred! Check browser console for details.");
                        }
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
        return serviceCall(Q.extend({
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
        function loadOptions(name, async) {
            return {
                async: async,
                cache: true,
                type: 'GET',
                url: Q.resolveUrl('~/DynJS.axd/') + name + '.js?' + registered[name],
                data: null,
                dataType: 'text',
                converters: {
                    "text script": function (text) {
                        return text;
                    }
                },
                success: function (data, textStatus, jqXHR) {
                    $.globalEval(data);
                },
                error: function (xhr, textStatus, errorThrown) {
                    var isLookup = Q.startsWith(name, "Lookup.");
                    if (xhr.status == 403 && isLookup) {
                        Q.notifyError('<p>Access denied while trying to load the lookup: "<b>' +
                            name.substr(7) + '</b>". Please check if current user has required permissions for this lookup.</p> ' +
                            '<p><em>Lookups use the ReadPermission of their row by default. You may override that for the lookup ' +
                            'like [LookupScript("Some.Lookup", Permission = "?")] to grant all ' +
                            'authenticated users to read it (or use "*" for public).</em></p>' +
                            '<p><em>Note that this might be a security risk if the lookup contains sensitive data, ' +
                            'so it could be better to set a separate permission for lookups, like "MyModule:Lookups".</em></p>', null, {
                            timeOut: 10000,
                            escapeHtml: false
                        });
                        return;
                    }
                    Q.notifyError("An error occurred while trying to load " +
                        (isLookup ? ' the lookup: "' + name.substr(7) :
                            ' dynamic script: "' + name) +
                        '"!. Please check the error message displayed in the dialog below for more info.');
                    var html = xhr.responseText;
                    if (!html) {
                        if (!xhr.status)
                            Q.alert("An unknown connection error occurred! Check browser console for details.");
                        else if (xhr.status == 500)
                            Q.alert("HTTP 500: Connection refused! Check browser console for details.");
                        else
                            Q.alert("HTTP " + xhr.status + ' error! Check browser console for details.');
                    }
                    else
                        Q.iframeDialog({ html: html });
                }
            };
        }
        function loadScriptAsync(name) {
            return Promise.resolve().then(function () {
                Q.blockUI(null);
                return Promise.resolve($.ajax(loadOptions(name, true))
                    .always(function () {
                    Q.blockUndo();
                }));
            }, null);
        }
        function loadScriptData(name) {
            if (registered[name] == null) {
                throw new Error(Q.format('Script data {0} is not found in registered script list!', name));
            }
            $.ajax(loadOptions(name, false));
        }
        function loadScriptDataAsync(name) {
            return Promise.resolve().then(function () {
                if (registered[name] == null) {
                    throw new Error(Q.format('Script data {0} is not found in registered script list!', name));
                }
                return loadScriptAsync(name);
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
        var name = 'Lookup.' + key;
        if (!ScriptData.canLoad(name)) {
            var message = 'No lookup with key "' + key + '" is registered. Please make sure you have a' +
                ' [LookupScript("' + key + '")] attribute in server side code on top of a row / custom lookup and ' +
                ' its key is exactly the same.';
            Q.notifyError(message);
            throw new Error(message);
        }
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
    if (typeof $ !== "undefined" && $.validator && $.validator.methods && $.validator.addMethod) {
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
                if ($.isFunction(handlers[i].handler)) {
                    var message = handlers[i].handler(el);
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
        function addMsg(m, k) {
            var txt = Q.tryGetText("Validation." + k);
            if (txt)
                $.validator.messages[m] = txt;
            else if (!$.validator.messages[m])
                $.validator.messages[m] = k + "?";
        }
        addMsg("required", "Required");
        addMsg("email", "Email");
        addMsg("minlength", "MinLength");
        addMsg("maxlength", "MaxLength");
        addMsg("digits", "Digits");
        addMsg("range", "Range");
        addMsg("xss", "Xss");
        addMsg("dateQ", "DateInvalid");
        addMsg("decimalQ", "Decimal");
        addMsg("integerQ", "Integer");
        addMsg("url", "Url");
    }
    function setTooltip(el, val) {
        if (Q.isBS3())
            el.attr('data-original-title', val || '').tooltip('fixTitle');
        else
            el.attr('title', val || '').tooltip('_fixTitle');
        return el;
    }
    var valOpt = {
        ignore: ':hidden, .no-validate',
        showErrors: function (errorMap, errorList) {
            $.each(this.validElements(), function (index, element) {
                var $element = $(element);
                setTooltip($element
                    .removeClass("error")
                    .addClass("valid"), '')
                    .tooltip('hide');
            });
            $.each(errorList, function (index, error) {
                var $element = $(error.element);
                setTooltip($element
                    .addClass("error"), error.message);
                if (index == 0)
                    $element.tooltip('show');
            });
        },
        normalizer: function (value) {
            return $.trim(value);
        }
    };
    function validateTooltip(form, opt) {
        return form.validate(Q.extend(Q.extend({}, valOpt), opt));
    }
    Q.validateTooltip = validateTooltip;
})(Q || (Q = {}));
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
    var SummaryType;
    (function (SummaryType) {
        SummaryType[SummaryType["Disabled"] = -1] = "Disabled";
        SummaryType[SummaryType["None"] = 0] = "None";
        SummaryType[SummaryType["Sum"] = 1] = "Sum";
        SummaryType[SummaryType["Avg"] = 2] = "Avg";
        SummaryType[SummaryType["Min"] = 3] = "Min";
        SummaryType[SummaryType["Max"] = 4] = "Max";
    })(SummaryType = Serenity.SummaryType || (Serenity.SummaryType = {}));
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var Decorators;
    (function (Decorators) {
        function distinct(arr) {
            return arr.filter(function (item, pos) { return arr.indexOf(item) === pos; });
        }
        function merge(arr1, arr2) {
            if (!arr1 || !arr2)
                return (arr1 || arr2 || []).slice();
            return distinct(arr1.concat(arr2));
        }
        function registerType(target, name, intf) {
            if (name != null) {
                target.__typeName = name;
                Q.types[name] = target;
            }
            else if (!target.__typeName)
                target.__register = true;
            if (intf)
                target.__interfaces = merge(target.__interfaces, intf);
        }
        function registerClass(nameOrIntf, intf2) {
            return function (target) {
                if (typeof nameOrIntf == "string")
                    registerType(target, nameOrIntf, intf2);
                else
                    registerType(target, null, nameOrIntf);
                target.__class = true;
            };
        }
        Decorators.registerClass = registerClass;
        function registerInterface(nameOrIntf, intf2) {
            return function (target) {
                if (typeof nameOrIntf == "string")
                    registerType(target, nameOrIntf, intf2);
                else
                    registerType(target, null, nameOrIntf);
                target.__interface = true;
                target.isAssignableFrom = function (type) {
                    return type.__interfaces != null && type.__interfaces.indexOf(this) >= 0;
                };
            };
        }
        Decorators.registerInterface = registerInterface;
        function addAttribute(type, attr) {
            type.__metadata = type.__metadata || {};
            type.__metadata.attr = type.__metadata.attr || [];
            type.__metadata.attr.push(attr);
        }
        Decorators.addAttribute = addAttribute;
    })(Decorators = Serenity.Decorators || (Serenity.Decorators = {}));
    Serenity.Decorators.registerInterface('Serenity.ISlickFormatter')(Serenity.ISlickFormatter);
    function Attr(name) {
        return Decorators.registerClass('Serenity.' + name + 'Attribute');
    }
    (function (Decorators) {
        function enumKey(value) {
            return function (target) {
                Decorators.addAttribute(target, new EnumKeyAttribute(value));
            };
        }
        Decorators.enumKey = enumKey;
        function registerEnum(target, enumKey, name) {
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
                if (name != null) {
                    target.__typeName = name;
                    Q.types[name] = target;
                }
                else if (!target.__typeName)
                    target.__register = true;
                if (enumKey)
                    Decorators.addAttribute(target, new EnumKeyAttribute(enumKey));
            }
        }
        Decorators.registerEnum = registerEnum;
        function registerEnumType(target, name, enumKey) {
            registerEnum(target, Q.coalesce(enumKey, name), name);
        }
        Decorators.registerEnumType = registerEnumType;
    })(Decorators = Serenity.Decorators || (Serenity.Decorators = {}));
    var EnumKeyAttribute = /** @class */ (function () {
        function EnumKeyAttribute(value) {
            this.value = value;
        }
        EnumKeyAttribute = __decorate([
            Attr('EnumKey')
        ], EnumKeyAttribute);
        return EnumKeyAttribute;
    }());
    Serenity.EnumKeyAttribute = EnumKeyAttribute;
    Decorators.registerEnum(Serenity.SummaryType, 'Serenity.SummaryType');
})(Serenity || (Serenity = {}));
// @ts-ignore try to make it work in common js for tests
typeof module !== "undefined" && (module.exports = { Q: Q, Serenity: Serenity, __extends: __extends });
//# sourceMappingURL=Serenity.CoreLib.base.js.map