import { ensureTypeInfo, getBaseType, getGlobalTypeRegistry, getTypeFullName, isInstanceOfType, peekTypeInfo, Type } from "../base";

/**
 * A plain-object dictionary mapping string keys to values of type `TItem`.
 * @deprecated Prefer {@link Record}`<string, TItem>` or {@link Map}`<string, TItem>` over this legacy alias.
 * @typeParam TItem - The type of each dictionary value.
 */
export type Dictionary<TItem> = { [key: string]: TItem };

/**
 * Returns the first argument if it is not `null`/`undefined`, otherwise the second argument.
 * @deprecated Use the nullish-coalescing operator `??` directly — e.g. `a ?? b`.
 * @param a - The preferred value; returned when it is not `null`/`undefined`.
 * @param b - The fallback value returned when `a` is `null`/`undefined`.
 * @returns `a` if `a != null`, otherwise `b`.
 */
export function coalesce(a: any, b: any): any {
    return a ?? b;
}

/**
 * Determines whether a value is neither `null` nor `undefined`.
 * @deprecated Use `a != null` (or `a !== null && a !== undefined`) directly.
 * @param a - The value to test.
 * @returns `true` if `a` is not `null` and not `undefined`.
 */
export function isValue(a: any): boolean {
    return a != null;
}

/**
 * Shallow-copies properties from `b` onto `a`, mutating `a` — equivalent to `Object.assign(a, b)`.
 * @deprecated Use {@link Object.assign} directly.
 * @typeParam T - The common object type.
 * @param a - The target object to extend (mutated and returned).
 * @param b - The source object whose own properties are copied onto `a`.
 * @returns The mutated target object `a`.
 */
export function extend<T = any>(a: T, b: T): T {
    return Object.assign(a, b);
}


/**
 * Returns the current local date with the time component zeroed to midnight.
 * @returns A `Date` representing today at 00:00:00 in the local time zone.
 */
export let today = (): Date => {
    const d = new Date();
    return new Date(d.getFullYear(), d.getMonth(), d.getDate());
}

/**
 * Deep clones an object or value.
 * @param a The value to clone.
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
    if (typeof (regExp as any).flags == 'string') {
        return (regExp as any).flags;
    } else {
        const flags = [];
        regExp.global && flags.push('g');
        regExp.ignoreCase && flags.push('i');
        regExp.multiline && flags.push('m');
        regExp.sticky && flags.push('y');
        regExp.unicode && flags.push('u');
        regExp.dotAll && flags.push('s');
        return flags.join('');
    }
}

/**
 * Describes a single type member collected via the legacy {@link addTypeMember} / option-decorator mechanism.
 * Preserved for backward compatibility; prefer {@link Symbol.metadata} / `Symbol.typeInfo` where possible.
 */
export interface TypeMember {
    /** Member name (field or property name). */
    name: string;
    /** Bitmask indicating the member kind (field vs. property). */
    kind: TypeMemberKind;
    /** Optional attribute/metadata objects attached to the member. */
    attr?: any[];
    /** Optional getter method name for property members. */
    getter?: string;
    /** Optional setter method name for property members. */
    setter?: string;
}

/**
 * Bitmask discriminating type-member kinds stored in {@link TypeMember.kind}.
 * Values are powers of two so they can be combined and filtered with bitwise operators.
 */
export enum TypeMemberKind {
    /** A plain field member. */
    field = 4,
    /** A property member (with optional getter/setter). */
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

/**
 * Removes all own enumerable properties from the given object.
 * @param d - The dictionary/object to clear. All own properties are deleted in place.
 */
export function clearKeys(d: any) {
    for (const n in d) {
        if (Object.prototype.hasOwnProperty.call(d, n))
            delete d[n];
    }
}

/**
 * Identity helper that preserves a property key's type, useful for type-safe `keyof` references.
 * @typeParam T - The type whose key is being referenced.
 * @param prop - A key of `T`.
 * @returns The same key, typed as `keyof T`.
 */
export function keyOf<T>(prop: keyof T) {
    return prop;
}

/**
 * Casts `instance` to `type`, throwing if the instance is not assignable to the target type.
 * @param instance - The value to cast; `null`/`undefined` is returned as-is.
 * @param type - The target {@link Type} to assert.
 * @returns `instance` typed as the target, if the runtime check passes.
 * @throws Error string when `instance` is not an instance of `type`.
 */
export function cast(instance: any, type: Type) {
    if (instance == null)
        return instance;
    else if (isInstanceOfType(instance, type))
        return instance;
    throw 'Cannot cast object to type ' + getTypeFullName(type);
}

/**
 * Attempts to cast `instance` to `type`, returning `null` on failure instead of throwing.
 * @param instance - The value to cast.
 * @param type - The target {@link Type} to test against.
 * @returns `instance` if it is an instance of `type`; otherwise `null`.
 */
export function safeCast(instance: any, type: Type) {
    return isInstanceOfType(instance, type) ? instance : null;
};