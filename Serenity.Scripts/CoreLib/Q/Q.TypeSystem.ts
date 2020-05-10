// @ts-ignore check for global
let globalObj: any = typeof (global) !== "undefined" ? global : (typeof (window) !== "undefined" ? window : (typeof (self) !== "undefined" ? self : this));

namespace Q {

    export interface Type {
        prototype: any;
        name?: string;
        __typeName?: string;
        __metadata?: {
            __interfaces: any[];
        };
        __propByName?: { [key: string]: any },
        __fieldByName?: { [key: string]: any }
        isInstanceOfType?: (type: Function) => boolean;
    }

    export let types: { [key: string]: Type } = {};

    export function getNested(from: any, name: string) {
        var a = name.split('.');
        for (var i = 0; i < a.length; i++) {
            from = from[a[i]];
            if (from == null)
                return null;
        }
        return from;
    }

    export let getType = (name: string, target?: any): Type => {
        var type: any;
        if (target == null) {
            type = types[name];
            if (type != null || globalObj == null)
                return type;

            target = globalObj;
        }

        type = getNested(target, name)
        if (typeof type !== 'function')
            return null;

        return type;
    }

    export let getTypeFullName = (type: Type): string => {
        return type.__typeName || type.name ||
            (type.toString().match(/^\s*function\s*([^\s(]+)/) || [])[1] || 'Object';
    };

    export let getTypeName = (type: Type): string => {
        var fullName = Q.getTypeFullName(type);
        var bIndex = fullName.indexOf('[');
        var nsIndex = fullName.lastIndexOf('.', bIndex >= 0 ? bIndex : fullName.length);
        return nsIndex > 0 ? fullName.substr(nsIndex + 1) : fullName;
    };

    export let getInstanceType = (instance: any): any => {
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

    export let isAssignableFrom = (target: any, type: Type) => {
        return target === type ||
            (typeof (target.isAssignableFrom) === 'function' && target.isAssignableFrom(type)) ||
            type.prototype instanceof target;
    };

    export let isInstanceOfType = (instance: any, type: Type) => {
        if (instance == null)
            return false;

        if (typeof (type.isInstanceOfType) === 'function')
            return type.isInstanceOfType(instance);

        return Q.isAssignableFrom(type, Q.getInstanceType(instance));

    };

    export let safeCast = (instance: any, type: Type) => {
        return Q.isInstanceOfType(instance, type) ? instance : null;
    };

    export let cast = (instance: any, type: Type) => {
        if (instance == null)
            return instance;
        else if (Q.isInstanceOfType(instance, type))
            return instance;
        throw new InvalidCastException('Cannot cast object to type ' + Q.getTypeFullName(type));
    }

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
                    result.push(a[i]);
                }
            }
        }
        if (type.__metadata && type.__metadata.attr) {
            for (var i = 0; i < type.__metadata.attr.length; i++) {
                var a: any = type.__metadata.attr[i];
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

    export const enum MemberType {
        field = 4,
        property = 16
    }

    export let getMembers = (type: any, memberTypes: MemberType): any[] => {
        var result = [];
        var b = Q.getBaseType(type);
        if (b)
            result = Q.getMembers(b, memberTypes & ~1);

        if (type.__metadata && type.__metadata.members) {
            for (var m of type.__metadata.members) {
                if (memberTypes & m.type)
                    result.push(m);
            }
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

    export let delegateCombine = (delegate1: any, delegate2: any) => {
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

    export namespace Enum {
        export let toString = (enumType: any, value: number): string => {
            var values = enumType.prototype;
            if (value === 0 || !enumType.__metadata || !enumType.__metadata.enumFlags) {
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

    function delegateContains(targets: any[], object: any, method: any) {
        for (var i = 0; i < targets.length; i += 2) {
            if (targets[i] === object && targets[i + 1] === method) {
                return true;
            }
        }
        return false;
    };

    
    let _mkdel = (targets: any[]): any => {
        var delegate: any = function () {
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
                var t = targets.slice();
                t.splice(i, 2);
                return _mkdel(t);
            }
        }

        return delegate1;
    };
    
    export let isEnum = (type: any) => {
        return !!type.__enum;
    };

    export function initFormType(typ: Function, nameWidgetPairs: any[]) {
        for (var i = 0; i < nameWidgetPairs.length - 1; i += 2) {
            (function (name: string, widget: any) {
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

    export function prop(type: any, name: string, getter?: string, setter?: string) {
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

    export function initializeTypes(root: any, pre: string, limit: number) {

        if (!root)
            return;

        for (var k of Object.keys(root)) {
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
                    obj.__interfaces = [Serenity.ISlickFormatter]
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
}