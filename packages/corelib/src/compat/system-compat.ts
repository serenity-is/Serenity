import { ensureTypeInfo, getBaseType, getTypeFullName, getGlobalTypeRegistry, isInstanceOfType, peekTypeInfo, Type } from "../base";

export type Dictionary<TItem> = { [key: string]: TItem };

/** @deprecated Use ?? operator */
export function coalesce(a: any, b: any): any {
    return a ?? b;
}

/** @deprecated Use a != null */
export function isValue(a: any): boolean {
    return a != null;
}

/** Extends an object with properties from another object similar to Object.assign.
 * @deprecated Use Object.assign
 */
export function extend<T = any>(a: T, b: T): T {
    return Object.assign(a, b);
}


/** Returns the current date without time part */
export let today = (): Date => {
    const d = new Date();
    return new Date(d.getFullYear(), d.getMonth(), d.getDate());
}

/**
 * Deep clones an object or value.
 * @param a The value to clone.
 * @param a2 An optional second value to merge into the clone.
 * @param a3 An optional third value to merge into the clone.
 * @returns A deep clone of the input value.
 */
export function deepClone<T = any>(a: T): T {

    // https://github.com/angus-c/just/blob/master/packages/collection-clone/index.js
    let result = a;
    const type = {}.toString.call(a).slice(8, -1);
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
        for (const key in a) {
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
        const flags = [];
        regExp.global && flags.push('g');
        regExp.ignoreCase && flags.push('i');
        regExp.multiline && flags.push('m');
        regExp.sticky && flags.push('y');
        regExp.unicode && flags.push('u');
        return flags.join('');
    }
}

/** Type member information, preserved for compatibility as used by legacy option decorator */
export interface TypeMember {
    name: string;
    kind: TypeMemberKind;
    attr?: any[];
    getter?: string;
    setter?: string;
}

/** Bitmask for type member kinds */
export enum TypeMemberKind {
    field = 4,
    property = 16
}

/** Gets type members including inherited ones. Optionally filters by member kinds.
 * @param type The type to get members for.
 * @param memberKinds Optional bitmask of TypeMemberKind to filter by.
 * @returns An array of TypeMember objects.
 * @remarks The members should be registered using addTypeMember function or option decorator.
 */
export function getTypeMembers(type: any, memberKinds?: TypeMemberKind): TypeMember[] {
    const result: TypeMember[] = [];
    do {
        const members = (peekTypeInfo(type) as any)?.["members"] as TypeMember[];
        if (members) {
            for (const member of members) {
                if (member &&
                    (memberKinds == null || (member.kind & memberKinds)) &&
                    !result.some(x => x.name === member.name))
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

/**
 * Adds a new member to a type or updates an existing member.
 * @param type The type to add the member to.
 * @param member The member information to add.
 * @returns The added or updated member.
 */
export function addTypeMember(type: any, member: TypeMember): TypeMember {
    if (!type)
        throw new Error("addTypeMember: type is null");

    const typeInfo = ensureTypeInfo(type);
    const members = ((typeInfo as any)["members"] ??= []) as TypeMember[];
    const existing = members.find(m => m.name === member.name);
    if (existing) {
        existing.kind ??= member.kind;
        member.attr && (existing.attr = merge(existing.attr, member.attr));
        member.getter != null && (existing.getter = member.getter);
        member.setter != null && (existing.setter = member.setter);
        return existing;
    }
    else {
        members.push(member);
        return member;
    }
}

/**
 * Gets all registered types.
 * @returns All registered types.
 */
export function getTypes(): any[] {
    const result = [];
    const types = getGlobalTypeRegistry();
    for (const t in types) {
        if (Object.prototype.hasOwnProperty.call(types, t))
            result.push(types[t]);
    }
    return result;
}

export function clearKeys(d: any) {
    for (const n in d) {
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