import { Type, getBaseType, getTypeFullName, getTypeNameProp, getTypeRegistry, isInstanceOfType, setTypeNameProp } from "../base";

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

export enum MemberType {
    field = 4,
    property = 16
}

export function getMembers(type: any, memberTypes: MemberType): TypeMember[] {
    var result: TypeMember[] = [];

    if (!type)
        return [];

    var result: TypeMember[] = [];
    do {
        let members = Object.prototype.hasOwnProperty.call(type, typeMemberListSymbol) ? type[typeMemberListSymbol] : null;
        if (members) {
            for (var member of members) {
                if (member && (member.type & memberTypes) && !result.some(x => x.name === member.name))
                    result.push(member);
            }
        }
    }
    while ((type = getBaseType(type)))
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

const typeMemberListSymbol = "Serenity.typeMemberList";

export function addTypeMember(type: any, member: TypeMember): TypeMember {
    if (!Object.prototype.hasOwnProperty.call(type, typeMemberListSymbol)) {
        type[typeMemberListSymbol] = [];
    }
    var members = type[typeMemberListSymbol];
    if (!members)
        type[typeMemberListSymbol] = members = [];
    var name = member.name;

    let existing: TypeMember;
    for (var m of members) {
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
        members.push(member);
        return member;
    }
}

export function getTypes(from?: any): any[] {
    const types = getTypeRegistry();
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

export function keyOf<T>(prop: keyof T) {
    return prop;
}

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

// has to duplicate this for now
const isInterfaceTypeSymbol: unique symbol = Symbol.for("Serenity.isInterfaceType");

export function initializeTypes(root: any, pre: string, limit: number) {

    if (!root)
        return;

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
            (typeof obj != "function" && typeof obj != "object"))
            continue;

        // no explicit typeName, e.g. not registered with a name,
        // a function but not an html element, or registered without name
        if (!getTypeNameProp(obj) &&
            (obj as any)[isInterfaceTypeSymbol] !== void 0 &&
            ((typeof obj === "function" && typeof obj.nodeType !== "number") ||
             (Object.prototype.hasOwnProperty.call(obj, isInterfaceTypeSymbol) &&
              (obj as any)[isInterfaceTypeSymbol] !== undefined))) {
   
            setTypeNameProp(obj, pre + k);
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

export { };
