export type Dictionary<TItem> = { [key: string]: TItem };

export function coalesce(a: any, b: any): any {
    return a != null ? a : b;
}

export function isValue(a: any): boolean {
    return a != null;
}

export let today = (): Date => {
    var d = new Date();
    return new Date(d.getFullYear(), d.getMonth(), d.getDate());
}

export function extend<T = any>(a: T, b: T): T {
    for (var key in b)
        if (Object.prototype.hasOwnProperty.call(b, key))
            a[key] = b[key];
    return a;
}

export function deepClone<T = any>(a: T, a2?: any, a3?: any): T {
    // for backward compatibility
    if (a2 != null || a3 != null) {
        return extend(extend(deepClone(a || {}), deepClone(a2 || {})), deepClone(a3 || {}));
    }

    if (!a)
        return a;
    
    let v: any;
    let b: T = Array.isArray(a) ? [] : {} as any;
    for (const k in a) {
        v = a[k];
        b[k] = (typeof v === "object") ? deepClone(v) : v;
    }
    
    return b;
}

// @ts-ignore check for global
let globalObj: any = typeof (global) !== "undefined" ? global : (typeof (window) !== "undefined" ? window : (typeof (self) !== "undefined" ? self : null));

export declare const enum TypeKind {
    Class = 0,
    Interface = 1,
    Enum = 2
}

interface TypeMetadata {
    kind?: TypeKind;
    enumFlags?: boolean;
    interfaces?: any[];
    attributes?: any[];
    members?: TypeMember[];
    propByName?: { [key: string]: any };
    fieldByName?: { [key: string]: any };
    isAssignableFrom?: (type: Function) => boolean;
    isInstanceOfType?: (type: Function) => boolean;
}

export interface Type {
    prototype: any;
    name?: string;
    __typeName?: string;
}

export interface TypeMember {
    name: string;
    type: MemberType;
    attr?: any[];
    getter?: string;
    setter?: string;
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

export function getType(name: string, target?: any): Type  {
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

export function getTypeFullName(type: Type): string {
    return type.__typeName || type.name ||
        (type.toString().match(/^\s*function\s*([^\s(]+)/) || [])[1] || 'Object';
};

export function getTypeName(type: Type): string {
    var fullName = getTypeFullName(type);
    var bIndex = fullName.indexOf('[');
    var nsIndex = fullName.lastIndexOf('.', bIndex >= 0 ? bIndex : fullName.length);
    return nsIndex > 0 ? fullName.substr(nsIndex + 1) : fullName;
};

export function getInstanceType(instance: any): any {
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

export function isAssignableFrom(target: any, type: Type) {
    if (target === type || type.prototype instanceof target)
        return true;

    var md = target.__metadata as TypeMetadata;
    if (md != null &&
        typeof md.isAssignableFrom === 'function')
        return md.isAssignableFrom.call(target, type);

    return false;
};

export function isInstanceOfType(instance: any, type: Type) {
    if (instance == null)
        return false;

    var md = (type as any).__metadata as TypeMetadata;
    if (md != null &&
        typeof (md.isInstanceOfType) === 'function')
        return md.isInstanceOfType.call(type, instance);

    return isAssignableFrom(type, getInstanceType(instance));
};

export function safeCast(instance: any, type: Type) {
    return isInstanceOfType(instance, type) ? instance : null;
};

export function cast(instance: any, type: Type) {
    if (instance == null)
        return instance;
    else if (isInstanceOfType(instance, type))
        return instance;
    throw new InvalidCastException('Cannot cast object to type ' + getTypeFullName(type));
}

export function getBaseType(type: any) {
    if (type === Object ||
        (type.__metadata as TypeMetadata)?.kind == TypeKind.Interface) {
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

export function getAttributes(type: any, attrType: any, inherit?: boolean) {
    var result = [];
    if (inherit) {
        var b = getBaseType(type);
        if (b) {
            var a: any = getAttributes(b, attrType, true);
            for (var i = 0; i < a.length; i++) {
                var t = getInstanceType(a[i]);
                result.push(a[i]);
            }
        }
    }
    var attr = (type.__metadata as TypeMetadata)?.attributes;
    if (attr != null) {
        for (var i = 0; i < attr.length; i++) {
            var a: any = attr[i];
            if (attrType == null || isInstanceOfType(a, attrType)) {
                var t = getInstanceType(a);
                for (var j = result.length - 1; j >= 0; j--) {
                    if (isInstanceOfType(result[j], t))
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

export function getMembers(type: any, memberTypes: MemberType): TypeMember[] {
    var result: TypeMember[] = [];
    var b = getBaseType(type);
    if (b)
        result = getMembers(b, memberTypes & ~1);

    var members = (type.__metadata as TypeMetadata).members;
    if (members != null) {
        for (var m of members) {
            if (memberTypes & m.type)
                result.push(m);
        }
    }

    return result;
};

export function addTypeMember(type: any, member: TypeMember): TypeMember {

    var name = member.name;
    var md = ensureMetadata(type);
    md.members = md.members || [];

    let existing: TypeMember;
    for (var m of type.__metadata.members) {
        if (m.name == name) {
            existing = m;
            break;
        }
    }

    if (existing) {
        if (member.type != null)
            existing.type = member.type;
        if (member.attr != null)
            existing.attr = merge(existing.attr, member.attr);
        if (member.getter != null)
            existing.getter = member.getter;
        if (member.setter != null)
            existing.setter = member.setter;
        return existing;
    }
    else {
        md.members.push(member);
        return member;
    }
}

export function getTypes(from?: any): any[] {
    var result = [];
    if (!from) {
        for (var t in types) {
            if (Object.prototype.hasOwnProperty.call(types, t))
                result.push(types[t]);
        }
    }
    else {
        var traverse = function (s: any, n: string) {
            for (var c in s) {
                if (Object.prototype.hasOwnProperty.call(s, c))
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

export function clearKeys(d: any) {
    for (var n in d) {
        if (Object.prototype.hasOwnProperty.call(d, n))
            delete d[n];
    }
}

export function delegateCombine(delegate1: any, delegate2: any) {
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
        if (value === 0 || !(enumType.__metadata as TypeMetadata)?.enumFlags) {
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
            if (Object.prototype.hasOwnProperty.call(values, i))
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
    return (type.__metadata as TypeMetadata)?.kind == TypeKind.Interface;
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

function ensureMetadata(target: Type): TypeMetadata {

    if (!Object.hasOwnProperty.call(target, '__metadata')) {
        Object.defineProperty(target, '__metadata', {
            get: function () { return Object.prototype.hasOwnProperty.call(this, '__metadata$') ? this.__metadata$ : void 0; },
            set: function (v) { this.__metadata$ = v; }
        });
    }
    if (!(target as any).__metadata) {
        (target as any).__metadata = Object.create(null);
    }

    return (target as any).__metadata;
}

function distinct(arr: any[]) {
    return arr.filter((item, pos) => arr.indexOf(item) === pos);
}

function merge(arr1: any[], arr2: any[]) {
    if (!arr1 || !arr2)
        return (arr1 || arr2 || []).slice();

    return distinct(arr1.concat(arr2));
}

function interfaceIsAssignableFrom(from: any) {
    var fmd = from.__metadata as TypeMetadata;
    return fmd != null && fmd.interfaces != null && fmd.interfaces.indexOf(this) >= 0;
}

export function registerType(type: any, name: string, kind?: TypeKind, intf?: any[]) {
    if (name != null) {
        setTypeName(type, name);
        types[name] = type;
    }
    else if (!type.__typeName)
        type.__register = true;
    else
        types[type.__typeName] = type;

    var md = ensureMetadata(type);
    md.kind = kind ?? TypeKind.Class;
    if (intf)
        md.interfaces = merge(md.interfaces, intf);

    if (kind == TypeKind.Interface)
        md.isAssignableFrom = interfaceIsAssignableFrom;

    if (kind == TypeKind.Enum) {
        type.prototype = type.prototype || {};
        for (var k of Object.keys(type))
            if (isNaN(parseInt(k)) && type[k] != null && !isNaN(parseInt(type[k])))
                type.prototype[k] = type[k];
    }
}

export function addAttribute(type: any, attr: any) {
    var md = ensureMetadata(type);
    md.attributes = md.attributes || [];
    md.attributes.push(attr);
}

export function setTypeName(target: Type, value: string) {
    if (!Object.hasOwnProperty.call(target, '__typeName')) {
        Object.defineProperty(target, '__typeName', {
            get: function() { return Object.prototype.hasOwnProperty.call(this, '__typeName$') ? this.__typeName$ : void 0; },
            set: function(v) { this.__typeName$ = v; }
        });
    }
    (target as any).__typeName = value;
}

export class ISlickFormatter {
}

registerType(ISlickFormatter, 'Serenity.ISlickFormatter', TypeKind.Interface);

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

        if (!Object.prototype.hasOwnProperty.call(root, k))
            continue;

        var obj = root[k];

        if ($.isArray(obj) ||
            root instanceof Date)
            continue;

        var t = typeof (obj);
        if (t == "string" || t == "number")
            continue;

        if (obj.__typeName && !obj.__register)
            continue;

        if ($.isFunction(obj) || (obj.__register && md?.kind == TypeKind.Enum)) {

            var md = obj.__metadata as TypeMetadata;

            if (!md?.interfaces &&
                obj.prototype.format &&
                k.substr(-9) == "Formatter") {
                md = ensureMetadata(obj);
                md.kind = TypeKind.Class;
                md.interfaces = [ISlickFormatter]
            }

            if (!md || md.kind == null) {
                var baseType = getBaseType(obj);
                if (baseType) {
                    var baseKind = baseType.__metadata?.kind;
                    if (baseKind != null) {
                        md = ensureMetadata(obj);
                        md.kind = baseKind;
                    }
                }
            }

            if (md != null && md.kind != null) {
                setTypeName(obj, pre + k)
                types[pre + k] = obj;
            }

            delete obj.__register;
        }
        
        if (limit > 0) 
            initializeTypes(obj, pre + k + ".", limit - 1);
    }
}

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