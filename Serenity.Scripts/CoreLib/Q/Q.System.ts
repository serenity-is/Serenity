// @ts-ignore check for global
let globalObj: any = typeof (global) !== "undefined" ? global : (typeof (window) !== "undefined" ? window : (typeof (self) !== "undefined" ? self : null));

namespace Q {

    export let types: { [key: string]: Function } = {};

    function _makeGenericTypeName(genericType: any, typeArguments: any[]) {
        var result = genericType.__typeName;
        for (var i = 0; i < typeArguments.length; i++)
            result += (i === 0 ? '[' : ',') + '[' + Q.getTypeFullName(typeArguments[i]) + ']';
        result += ']';
        return result;
    };

    let __genericCache = {};

    function makeGenericType(genericType: any, typeArguments: any[]) {
        var name = _makeGenericTypeName(genericType, typeArguments);
        return __genericCache[name] || genericType.apply(null, typeArguments);
    };

    export let isGenericTypeDefinition = (type: any) => {
        return type.__isGenericTypeDefinition || false;
    };

    export let getType = (name: string, target?: any): any => {
        if (target == null) {
            var type = types[name];
            if (type != null)
                return;
        }

        var a = name.split('.');
        type = target;
        for (var i = 0; i < a.length; i++) {
            type = type[a[i]];
            if (type == null)
                return null;
        }
        if (typeof type !== 'function')
            return null;
        return type;
    }

    export let getTypeFullName = (type: any): string => {
        return type.__typeName || type.name ||
            (type.toString().match(/^\s*function\s*([^\s(]+)/) || [])[1] || 'Object';
    };

    export let getTypeName = (type: any): string => {
        var fullName = Q.getTypeFullName(type);
        var bIndex = fullName.indexOf('[');
        var nsIndex = fullName.lastIndexOf('.', bIndex >= 0 ? bIndex : fullName.length);
        return nsIndex > 0 ? fullName.substr(nsIndex + 1) : fullName;
    };

    export let getInstanceType = (instance: any) => {
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

    export let isAssignableFrom = (target: any, type: any) => {
        return target === type ||
            (typeof (target.isAssignableFrom) === 'function' && target.isAssignableFrom(type)) ||
            type.prototype instanceof target;
    };

    export let isInstanceOfType = (instance: any, type: any) => {
        if (instance == null)
            return false;

        if (typeof (type.isInstanceOfType) === 'function')
            return type.isInstanceOfType(instance);

        return Q.isAssignableFrom(type, Q.getInstanceType(instance));

    };

    export let safeCast = (instance: any, type: any) => {
        return Q.isInstanceOfType(instance, type) ? instance : null;
    };

    export let cast = (instance: any, type: any) => {
        if (instance == null)
            return instance;
        else if (Q.isInstanceOfType(instance, type))
            return instance;
        throw new InvalidCastException('Cannot cast object to type ' + Q.getTypeFullName(type));
    }

    export let createInstance = function ss$createInstance(type: any): any {
        if (typeof (type.createInstance) === 'function')
            return type.createInstance();
        else if (type === Boolean)
            return false;
        else if (type === Date)
            return new Date(0);
        else if (type === Number)
            return 0;
        else if (type === String)
            return '';
        else
            return new type();
    };

    export let getBaseType = (type: any): any => {
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

    export let getAttributes = (type: any, attrType: any, inherit?: boolean): any[] => {
        var result = [];
        if (inherit) {
            var b = Q.getBaseType(type);
            if (b) {
                var a: any = Q.getAttributes(b, attrType, true);
                for (var i = 0; i < a.length; i++) {
                    var t = Q.getInstanceType(a[i]);
                    if (!t.__metadata || !t.__metadata.attrNoInherit)
                        result.push(a[i]);
                }
            }
        }
        if (type.__metadata && type.__metadata.attr) {
            for (var i = 0; i < type.__metadata.attr.length; i++) {
                var a: any = type.__metadata.attr[i];
                if (attrType == null || Q.isInstanceOfType(a, attrType)) {
                    var t = Q.getInstanceType(a);
                    if (!t.__metadata || !t.__metadata.attrAllowMultiple) {
                        for (var j = result.length - 1; j >= 0; j--) {
                            if (Q.isInstanceOfType(result[j], t))
                                result.splice(j, 1);
                        }
                    }
                    result.push(a);
                }
            }
        }
        return result;
    };

    export let getMembers = (type: any, memberTypes: number, bindingAttr: any, name?: string, params?: any): any[] => {
        var result = [];
        if ((bindingAttr & 72) == 72 || (bindingAttr & 6) == 4) {
            var b = Q.getBaseType(type);
            if (b)
                result = Q.getMembers(b, memberTypes & ~1, bindingAttr &
                    (bindingAttr & 64 ? 255 : 247) & (bindingAttr & 2 ? 251 : 255), name, params);
        }

        var f = function (m: any) {
            if ((memberTypes & m.type) && (((bindingAttr & 4) && !m.isStatic) ||
                    ((bindingAttr & 8) && m.isStatic)) && (!name || m.name === name)) {
                if (params) {
                    if ((m.params || []).length !== params.length)
                        return;
                    for (var i = 0; i < params.length; i++) {
                        if (params[i] !== m.params[i])
                            return;
                    }
                }
                result.push(m);
            }
        };

        if (type.__metadata && type.__metadata.members) {
            for (var i = 0; i < type.__metadata.members.length; i++) {
                var m = type.__metadata.members[i];
                f(m);
                for (var j = 0; j < 4; j++) {
                    var a = ['getter', 'setter', 'adder', 'remover'][j];
                    if (m[a])
                        f(m[a]);
                }
            }
        }

        if (bindingAttr & 256) {
            while (type) {
                var r = [];
                for (var i = 0; i < result.length; i++) {
                    if (result[i].typeDef === type)
                        r.push(result[i]);
                }
                if (r.length > 1)
                    throw new Error('Ambiguous match');
                else if (r.length === 1)
                    return r[0];
                type = Q.getBaseType(type);
            }
            return null;
        }

        return result;
    };

    export let getTypes = (from?: any): any[] => {
        var result = [];
        if (!from) {
            for (var t in types) {
                if (types.hasOwnProperty(t))
                    result.push(types[t]);
            }
        }
        else {
            var traverse = function (s: any, n: string) {
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

    export class Exception extends Error {
        constructor(message: string) {
            super(message);
            this.name = "Exception";
        }
    }

    export class NullReferenceException extends Exception {
        constructor(message?: string) {
            super(message || 'Object is null.');
            this.name = "NullReferenceException";
        }
    }

    export class ArgumentNullException extends Exception {
        constructor(paramName: string, message?: string) {
            super((message || 'Value cannot be null.') + '\nParameter name: ' + paramName);
            this.name = "ArgumentNullException";
        }
    }

    export class ArgumentOutOfRangeException extends Exception {
        constructor(paramName: string, message?: string) {
            super((message ?? 'Value is out of range.') +
                (paramName ? ('\nParameter name: ' + paramName) : ""));
            this.name = "ArgumentNullException";
        }
    }

    export class InvalidCastException extends Exception {
        constructor(message: string) {
            super(message);
            this.name = "InvalidCastException";
        }
    }

    export let clearKeys = (d: any) => {
        for (var n in d) {
            if (d.hasOwnProperty(n))
                delete d[n];
        }
    }

    export let compareValues = (a: any, b: any): number => {
        if (a == null)
            throw new NullReferenceException('Object is null');
        else if (typeof (a) === 'number' || typeof (a) === 'string' || typeof (a) === 'boolean')
            return b != null ? (a < b ? -1 : (a > b ? 1 : 0)) : 1;
        else if (Object.prototype.toString.call(a) === '[object Date]')
            return b != null ? Q.compareValues(a.valueOf(), b.valueOf()) : 1;
        else
            return a.compareTo(b);
    }

    export let compareStrings = (s1: string, s2: string, ignoreCase?: boolean) => {
        if (s1 == null)
            return s2 != null ? -1 : 0;

        if (s2 == null)
            return 1;

        if (ignoreCase) {
            s1 = s1.toUpperCase();
            s2 = s2.toUpperCase();
        }

        return s1 == s2 ? 0 : (s1 < s2 ? -1 : 1);
    };

    export let lastIndexOfAnyString = (s: string, chars: string[], startIndex?: number, count?: number) => {
        var length = s.length;
        if (!length) {
            return -1;
        }

        chars = String.fromCharCode.apply(null, chars);
        startIndex = startIndex || length - 1;
        count = count || length;

        var endIndex = startIndex - count + 1;
        if (endIndex < 0) {
            endIndex = 0;
        }

        for (var i = startIndex; i >= endIndex; i--) {
            if (chars.indexOf(s.charAt(i)) >= 0) {
                return i;
            }
        }
        return -1;
    };

    export let contains = (obj: any, item: any) => {
        if (obj.contains)
            return obj.contains(item);
        else
            return obj.indexOf(item) >= 0;
    };

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

    export let trunc = (n: number): number => n != null ? (n > 0 ? Math.floor(n) : Math.ceil(n)) : null;

    export let delegateCombine = (delegate1: any, delegate2: any) => {
        if (!delegate1) {
            if (!delegate2._targets) {
                return Q.mkdel(null, delegate2);
            }
            return delegate2;
        }
        if (!delegate2) {
            if (!delegate1._targets) {
                return Q.mkdel(null, delegate1);
            }
            return delegate1;
        }

        var targets1 = delegate1._targets ? delegate1._targets : [null, delegate1];
        var targets2 = delegate2._targets ? delegate2._targets : [null, delegate2];

        return _mkdel(targets1.concat(targets2));
    };

    export namespace Enum {
        export let toString = (enumType: any, value: number): string => {
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
                var parts: string[] = [];
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

        export let getValues = (enumType: any) => {
            var parts = [];
            var values = enumType.prototype;
            for (var i in values) {
                if (values.hasOwnProperty(i))
                    parts.push(values[i]);
            }
            return parts;
        };
    }

    export let arrayClone = (arr: any[]) => {
        if (arr.length === 1) {
            return [arr[0]];
        }
        else {
            return Array.apply(null, arr);
        }
    };

    function delegateContains(targets: any[], object: any, method: any) {
        for (var i = 0; i < targets.length; i += 2) {
            if (targets[i] === object && targets[i + 1] === method) {
                return true;
            }
        }
        return false;
    };

    export let midel = (mi: any, target: any, typeArguments?: any) => {
        if (mi.isStatic && !!target)
            throw new ArgumentNullException('Cannot specify target for static method');
        else if (!mi.isStatic && !target)
            throw new ArgumentNullException('Must specify target for instance method');

        var method;
        if (mi.fget) {
            method = function () { return (mi.isStatic ? mi.typeDef : this)[mi.fget]; };
        }
        else if (mi.fset) {
            method = function (v: any) { (mi.isStatic ? mi.typeDef : this)[mi.fset] = v; };
        }
        else {
            method = mi.def || (mi.isStatic || mi.sm ? mi.typeDef[mi.sname] : target[mi.sname]);

            if (mi.tpcount) {
                if (!typeArguments || typeArguments.length !== mi.tpcount)
                    throw new ArgumentOutOfRangeException('Wrong number of type arguments');
                method = method.apply(null, typeArguments);
            }
            else {
                if (typeArguments && typeArguments.length)
                    throw new ArgumentOutOfRangeException('Cannot specify type arguments for non-generic method');
            }
            if (mi.exp) {
                var _m1 = method;
                method = function () { return _m1.apply(this, Array.prototype.slice.call(arguments, 0, arguments.length - 1).concat(arguments[arguments.length - 1])); };
            }
            if (mi.sm) {
                var _m2 = method;
                method = function () { return _m2.apply(null, [this].concat(Array.prototype.slice.call(arguments))); };
            }
        }
        return Q.mkdel(target, method);
    };

    export function fieldAccess(fi: any, obj: any, val?: any) {
        if (fi.isStatic && !!obj)
            throw new Error('Cannot specify target for static field');
        else if (!fi.isStatic && !obj)
            throw new Error('Must specify target for instance field');
        obj = fi.isStatic ? fi.typeDef : obj;
        if (arguments.length === 3)
            obj[fi.sname] = arguments[2];
        else
            return obj[fi.sname];
    };

    let _mkdel = (targets: any[]): any => {
        var delegate: any = function () {
            if (targets.length == 2) {
                return targets[1].apply(targets[0], arguments);
            }
            else {
                var clone = Q.arrayClone(targets);
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


    export let mkdel = (object: any, method: any) => {
        if (!object) {
            return method;
        }
        if (typeof method === 'string') {
            method = object[method];
        }
        return _mkdel([object, method]);
    };

    export let delegateRemove = (delegate1: any, delegate2: any) => {
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
                var t = Q.arrayClone(targets);
                t.splice(i, 2);
                return _mkdel(t);
            }
        }

        return delegate1;
    };

    export let startsWithString = (s: string, prefix: string): boolean => {
        if (prefix == null || !prefix.length) {
            return true;
        }
        if (prefix.length > s.length) {
            return false;
        }
        return (s.substr(0, prefix.length) == prefix);
    };

    export let today = (): Date => {
        var d = new Date();
        return new Date(d.getFullYear(), d.getMonth(), d.getDate());
    }

    export var trimEndString = function(s: string, chars?: string[]) {
        return s.replace(chars ? new RegExp('[' + String.fromCharCode.apply(null, chars) + ']+$') : /\s*$/, '');
    };

    export var trimStartString = function(s: string, chars?: string[]) {
        return s.replace(chars ? new RegExp('^[' + String.fromCharCode.apply(null, chars) + ']+') : /^\s*/, '');
    };

    export let isEnum = (type: any) => {
        return !!type.__enum;
    };

}