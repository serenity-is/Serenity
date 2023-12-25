import { Type, ensureMetadata, getBaseType, getInstanceType, getTypeFullName, getTypeNameProp, getTypeStore, isInstanceOfType, registerClass, registerInterface, setTypeNameProp } from "@serenity-is/base";

export type Dictionary<TItem> = { [key: string]: TItem };

/** @deprecated Use ?? operator */
export function coalesce(a: any, b: any): any {
    return a ?? b;
}

/** @deprecated Use a != null */
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

export interface TypeMember {
    name: string;
    type: MemberType;
    attr?: any[];
    getter?: string;
    setter?: string;
}

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
    var attr = (type as any).__metadata?.attr;
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

    var members = (type as any).__metadata?.members;
    if (members != null) {
        for (var m of members) {
            if (memberTypes & m.type)
                result.push(m);
        }
    }

    return result;
};

function merge(arr1: any[], arr2: any[]) {
    if (!arr1 || !arr2)
        return (arr1 || arr2 || []).slice();

    function distinct(arr: any[]) {
        return arr.filter((item, pos) => arr.indexOf(item) === pos);
    }        

    return distinct(arr1.concat(arr2));
}

export function addTypeMember(type: any, member: TypeMember): TypeMember {

    var name = member.name;
    var md = ensureMetadata(type) as any;
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

export function keyOf<T>(prop: keyof T) {
    return prop;
}

export function registerEditor(type: any, name: string, intf?: any[]) {
    registerClass(type, name, intf);
    addAttribute(type, new EditorAttribute());
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

export function cast(instance: any, type: Type) {
    if (instance == null)
        return instance;
    else if (isInstanceOfType(instance, type))
        return instance;
    throw 'Cannot cast object to type ' + getTypeFullName(type);
}

export function safeCast(instance: any, type: Type) {
    return isInstanceOfType(instance, type) ? instance : null;
};

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
              (obj as any).__interface !== undefined))) {
   
            // legacy formatter with registerClass
            if (typeof obj == "function" && 
                !obj.__interfaces && 
                obj.prototype?.format &&
                k.substr(-9) === "Formatter") {
                if ((obj as any).__interface === undefined)
                    Object.defineProperty(obj, "__interface", { value: false, configurable: true });
                (obj as any).__interfaces = [ISlickFormatter]
            }

            // only register types that should
            if ((obj as any).__interface !== undefined) {
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