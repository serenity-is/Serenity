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

    // https://github.com/angus-c/just/blob/master/packages/collection-clone/index.js
    var result = a;
    var type = {}.toString.call(a).slice(8, -1);
    if (type == 'Set') {
        return new Set([...(a as any)].map(value => deepClone(value))) as any;
    }
    if (type == 'Map') {
        return new Map([...(a as any)].map(kv => [deepClone(kv[0]), deepClone(kv[1])])) as any;
    }
    if (type == 'Date') {
        return new Date((a as any).getTime()) as any;
    }
    if (type == 'RegExp') {
        return RegExp((a as any).source, getRegExpFlags(a as any)) as any;
    }
    if (type == 'Array' || type == 'Object') {
        result = (Array.isArray(a) ? [] : {}) as any;
        for (var key in a) {
            // include prototype properties
            result[key] = deepClone(a[key]);
        }
    }
    // primitives and non-supported objects (e.g. functions) land here
    return result;
}

function getRegExpFlags(regExp: RegExp) {
    if (typeof (regExp.source as any).flags == 'string') {
        return (regExp.source as any).flags;
    } else {
        var flags = [];
        regExp.global && flags.push('g');
        regExp.ignoreCase && flags.push('i');
        regExp.multiline && flags.push('m');
        regExp.sticky && flags.push('y');
        regExp.unicode && flags.push('u');
        return flags.join('');
    }
}

interface TypeExt {
    __interface?: boolean;
    __interfaces?: any[];
    __isAssignableFrom?: (from: any) => boolean;
    __isInstanceOfType?: (instance: any) => boolean;
    __metadata?: TypeMetadata;
    __typeName?: string;
}

interface TypeMetadata {
    enumFlags?: boolean;
    attr?: any[];
    members?: TypeMember[];
}

export type Type = Function | Object;

export interface TypeMember {
    name: string;
    type: MemberType;
    attr?: any[];
    getter?: string;
    setter?: string;
}

function getTypeStore() {
    return getStateStore("__types");
}

export function getNested(from: any, name: string) {
    var a = name.split('.');
    for (var i = 0; i < a.length; i++) {
        from = from[a[i]];
        if (from == null)
            return null;
    }
    return from;
}

const globalObj: any = 
    (typeof globalThis !== "undefined" && globalThis) || 
    (typeof window !== "undefined" && window) || 
    (typeof self !== "undefined" && self) ||
    // @ts-ignore check for global
    (typeof global !== "undefined" && global) ||
    (function () { return this; })() || Function('return this')();

export function getGlobalThis(): any {
    return globalObj;
}

export function getType(name: string, target?: any): Type  {
    var type: any;
    const types = getTypeStore();
    if (target == null) {
        type = types[name];
        if (type != null || globalObj == void 0 || name === "Object")
            return type;

        target = globalObj;
    }

    type = getNested(target, name)
    if (typeof type !== 'function')
        return null;

    return type;
}

export function getTypeNameProp(type: Type): string {
    return (Object.prototype.hasOwnProperty.call(type, "__typeName") && (type as TypeExt).__typeName) || void 0;
}

export function setTypeNameProp(type: Type, value: string) {
    Object.defineProperty(type, "__typeName", { value, configurable: true });
}

export function getTypeFullName(type: Type): string {
    return getTypeNameProp(type) || (type as any).name ||
        (type.toString().match(/^\s*function\s*([^\s(]+)/) || [])[1] || 'Object';
};

export function getTypeShortName(type: Type): string {
    var fullName = getTypeFullName(type);
    var bIndex = fullName.indexOf('[');
    var nsIndex = fullName.lastIndexOf('.', bIndex >= 0 ? bIndex : fullName.length);
    return nsIndex > 0 ? fullName.substr(nsIndex + 1) : fullName;
};

export function getInstanceType(instance: any): any {
    if (instance == null)
        throw new ArgumentNullException("instance", "Can't get instance type of null or undefined!");

    // Have to catch as constructor cannot be looked up on native COM objects
    try {
        return instance.constructor;
    }
    catch (ex) {
        return Object;
    }
};

export function isAssignableFrom(target: any, type: Type) {
    if (target === type || (type as any).prototype instanceof target)
        return true;

    if (typeof (target as TypeExt).__isAssignableFrom === 'function')
        return (target as TypeExt).__isAssignableFrom(type);

    return false;
};

export function isInstanceOfType(instance: any, type: Type) {
    if (instance == null)
        return false;

    if (typeof (type as TypeExt).__isInstanceOfType === 'function')
        return (type as TypeExt).__isInstanceOfType(instance);

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
        !type.prototype ||
        (type as TypeExt).__interface === true) {
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
    var attr = (type as TypeExt).__metadata?.attr;
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

export enum MemberType {
    field = 4,
    property = 16
}

export function getMembers(type: any, memberTypes: MemberType): TypeMember[] {
    var result: TypeMember[] = [];
    var b = getBaseType(type);
    if (b)
        result = getMembers(b, memberTypes & ~1);

    var members = (type as TypeExt).__metadata?.members;
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
    for (var m of md.members) {
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
    const types = getTypeStore();
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

const fallbackStore: any = {};


export function getStateStore(key?: string): any {

    let store: any;
    if (globalObj) {
        if (!globalObj.Q)
            globalObj.Q = {};

        store = globalObj.Q.__stateStore;
        if (!store)
            globalObj.Q.__stateStore = store = Object.create(null);
    }
    else
        store = fallbackStore;

    if (key == null)
        return store;

    var s = store[key];
    if (s == null)
        store[key] = s = Object.create(null);
    return s;
}

export namespace Enum {
    export let toString = (enumType: any, value: number): string => {
        var values = enumType;
        if (value === 0 || !((enumType as TypeExt).__metadata?.enumFlags)) {
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
        var values = enumType;
        for (var i in values) {
            if (Object.prototype.hasOwnProperty.call(values, i) &&
                typeof values[i] === "number")
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
        if (targets.length === 2) {
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
            if (targets.length === 2) {
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
    return typeof type !== "function" &&
        (type as TypeExt).__interface === null;
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

    if (!Object.prototype.hasOwnProperty.call(target, '__metadata') ||
        !(target as TypeExt).__metadata) {
        (target as TypeExt).__metadata = Object.create(null);
    }

    return (target as TypeExt).__metadata;
}

function distinct(arr: any[]) {
    return arr.filter((item, pos) => arr.indexOf(item) === pos);
}

const _fieldsProxy = new Proxy({}, { get: (_, p) => p }) as any;

export function fieldsProxy<TRow>(): Readonly<Record<keyof TRow, string>> {
    return _fieldsProxy
}

export function keyOf<T>(prop: keyof T) {
    return prop;
}

function merge(arr1: any[], arr2: any[]) {
    if (!arr1 || !arr2)
        return (arr1 || arr2 || []).slice();

    return distinct(arr1.concat(arr2));
}

function interfaceIsAssignableFrom(from: any) {
    return from != null && 
        Array.isArray((from as TypeExt).__interfaces) && 
        (from as TypeExt).__interfaces.some(x => 
            x === this ||
            (getTypeNameProp(this)?.length && 
                x.__interface &&
                getTypeNameProp(x) === this.__typeName));
}

function registerType(type: any, name: string, intf: any[]) {
    const types = getTypeStore();
    if (name && name.length) {
        setTypeNameProp(type, name);
        types[name] = type;
    }
    else if (getTypeNameProp(type as TypeExt)?.length)
        types[(type as TypeExt).__typeName] = type;

    if (intf != null && intf.length)
        Object.defineProperty(type, "__interfaces", {
            value: merge((type as TypeExt).__interfaces, intf),
            configurable: true
        });
}

export function registerClass(type: any, name: string, intf?: any[]) {
    registerType(type, name, intf);
    Object.defineProperty(type, "__interface", { value: false, configurable: true });
}

export function registerEditor(type: any, name: string, intf?: any[]) {
    registerClass(type, name, intf);
    addAttribute(type, new EditorAttribute());
}

export function registerEnum(type: any, name: string, enumKey?: string) {
    registerType(type, name, undefined);
    if (enumKey && enumKey != name) {
        const types = getTypeStore();
        if (!types[enumKey])
            types[enumKey] = type;
    }
    Object.defineProperty(type, "__interface", { value: null, configurable: true });
}

export function registerInterface(type: any, name: string, intf?: any[]) {
    registerType(type, name, intf);
    Object.defineProperty(type, "__interface", { value: true, configurable: true });
    Object.defineProperty(type, "__isAssignableFrom", { value: interfaceIsAssignableFrom, configurable: true });
}

export function addAttribute(type: any, attr: any) {
    var md = ensureMetadata(type);
    md.attr = md.attr || [];
    md.attr.push(attr);
}

export class ISlickFormatter {
}

registerInterface(ISlickFormatter, 'Serenity.ISlickFormatter');

export class EditorAttribute {
}

registerClass(EditorAttribute, 'Serenity.EditorAttribute');

export function initializeTypes(root: any, pre: string, limit: number) {

    if (!root)
        return;

    const types = getTypeStore();

    for (var k of Object.keys(root)) {
        if (k.charAt(0) < 'A' || 
            k.charAt(0) > 'Z' || 
            k.indexOf('$') >= 0 || 
            !Object.prototype.hasOwnProperty.call(root, k))
            continue;

        var obj = root[k];

        if (obj == void 0 ||
            Array.isArray(obj) ||
            obj instanceof Date ||
            (typeof obj != "function" &&
             typeof obj != "object"))
            continue;

        // no explict __typeName, e.g. not registered with a name,
        // a function but not an html element, or registered without name
        if (!getTypeNameProp(obj) && 
            ((typeof obj === "function" && typeof obj.nodeType !== "number") || 
             (Object.prototype.hasOwnProperty.call(obj, "__interface") &&
              (obj as TypeExt).__interface !== undefined))) {
   
            // legacy formatter with registerClass
            if (typeof obj == "function" && 
                !obj.__interfaces && 
                obj.prototype?.format &&
                k.substr(-9) === "Formatter") {
                if ((obj as TypeExt).__interface === undefined)
                    Object.defineProperty(obj, "__interface", { value: false, configurable: true });
                (obj as TypeExt).__interfaces = [ISlickFormatter]
            }

            // only register types that should
            if ((obj as TypeExt).__interface !== undefined) {
                setTypeNameProp(obj, pre + k);
                types[pre + k] = obj;
            }
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

export class ArgumentNullException extends Exception {
    constructor(paramName: string, message?: string) {
        super((message || 'Value cannot be null.') + '\nParameter name: ' + paramName);
        this.name = "ArgumentNullException";
    }
}

export class InvalidCastException extends Exception {
    constructor(message: string) {
        super(message);
        this.name = "InvalidCastException";
    }
}

export {}