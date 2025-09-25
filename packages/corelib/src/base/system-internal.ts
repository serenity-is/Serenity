import { implementedInterfacesSymbol, isAssignableFromSymbol, typeRegistrySymbol } from "./symbols";

(Symbol as any).typeInfo ??= Symbol.for("Serenity.typeInfo");

export const globalObject: any =
    (typeof globalThis !== "undefined" && globalThis) ||
    (typeof window !== "undefined" && window) ||
    (typeof self !== "undefined" && self) ||
    // @ts-ignore check for global
    (typeof global !== "undefined" && global) || {};

export function merge(arr1: any[], arr2: any[]) {
    if (!arr1 || !arr2)
        return (arr1 || arr2 || []).slice();

    function distinct(arr: any[]) {
        return arr.filter((item, pos) => arr.indexOf(item) === pos);
    }

    return distinct(arr1.concat(arr2));
}

export type StringLiteral<T> = T extends string ? string extends T ? never : T : never;

/**
 * Type information for a registered type.
 */
export type TypeInfo<TypeName> = {
    /** Type kind, can be "class", "enum", "interface" */
    typeKind: "class" | "enum" | "interface";
    /** Registered type name */
    typeName: StringLiteral<TypeName> | (string & {});
    /** Implemented interfaces */
    interfaces?: any[];
    /** Custom attributes */
    customAttributes?: any[];
    /** Enum flags */
    enumFlags?: boolean;
    /** Registered flag */
    registered?: boolean;
}

export function getGlobalTypeRegistry(): { [key: string]: any } {
    let typeRegistry = globalObject[typeRegistrySymbol];
    if (!typeRegistry)
        typeRegistry = globalObject[typeRegistrySymbol] = {};
    return typeRegistry;
}

export function interfaceIsAssignableFrom(from: any) {
    return from != null &&
        Array.isArray((from as any)[implementedInterfacesSymbol]) &&
        (from as any)[implementedInterfacesSymbol].some((x: any) =>
            x === this ||
            (getTypeNameProp(this) &&
                peekTypeInfo(x)?.typeKind === "interface" &&
                getTypeNameProp(x) === getTypeNameProp(this)));
}

function autoRegisterViaTypeInfo(type: any): void {
    if (!Object.prototype.hasOwnProperty.call(type, Symbol.typeInfo))
        return;

    const typeInfo = type[Symbol.typeInfo] as TypeInfo<string>;
    if (!typeInfo || typeInfo.registered || !typeInfo.typeName)
        return;

    if (typeInfo.typeName.endsWith("."))
        typeInfo.typeName += type.name;

    if (!getGlobalTypeRegistry()[typeInfo.typeName])
        getGlobalTypeRegistry()[typeInfo.typeName] = type;

    if (typeInfo.interfaces?.length &&
        !Object.prototype.hasOwnProperty.call(implementedInterfacesSymbol)) {
        Object.defineProperty(type, implementedInterfacesSymbol, {
            value: merge(type[implementedInterfacesSymbol], typeInfo.interfaces),
            configurable: true
        });
    }

    if (typeInfo.typeKind === "interface" &&
        !Object.prototype.hasOwnProperty.call(type, isAssignableFromSymbol)) {
        Object.defineProperty(type, isAssignableFromSymbol, { value: interfaceIsAssignableFrom, configurable: true });
    }

    typeInfo.registered = true;
    return;
}

export function internalRegisterType(type: any, typeName?: string, interfaces?: any[], kind?: "class" | "enum" | "interface"): TypeInfo<string> {
    const typeInfo = ensureTypeInfo(type);
    if (kind)
        typeInfo.typeKind = kind;

    if (typeName && typeName !== typeInfo.typeName)
        typeInfo.typeName = typeName;

    if (typeInfo.typeName) {
        if (typeInfo.typeName.endsWith("."))
            typeInfo.typeName += type.name;

        getGlobalTypeRegistry()[typeInfo.typeName] = type;
    }

    if (interfaces?.length && typeInfo.interfaces !== interfaces) {
        interfaces = typeInfo.interfaces = merge(typeInfo.interfaces, interfaces);

        Object.defineProperty(type, implementedInterfacesSymbol, {
            value: merge(type[implementedInterfacesSymbol], interfaces),
            configurable: true
        });
    }

    typeInfo.registered = true;
    return typeInfo;
}

export function ensureTypeInfo(type: any): TypeInfo<string> {
    let typeInfo: TypeInfo<string>;
    if (!Object.prototype.hasOwnProperty.call(type, Symbol.typeInfo) ||
        !(typeInfo = type[Symbol.typeInfo])) {
        typeInfo = { typeKind: void 0 } as any;
        Object.defineProperty(type, Symbol.typeInfo, { value: typeInfo, configurable: true, writable: true });
        return typeInfo;
    }
    if (!typeInfo.registered)
        autoRegisterViaTypeInfo(type);
    return typeInfo;
}

export function peekTypeInfo(type: any): TypeInfo<string> {
    if (!type ||
        !Object.prototype.hasOwnProperty.call(type, Symbol.typeInfo))
        return void 0;

    const typeInfo = type[Symbol.typeInfo];
    if (typeInfo && !typeInfo.registered)
        autoRegisterViaTypeInfo(type);

    return typeInfo;
}

export function getTypeNameProp(type: any): string {
    return peekTypeInfo(type)?.typeName || void 0;
}

export function setTypeNameProp(type: any, value: string) {
    ensureTypeInfo(type).typeName = value;
    autoRegisterViaTypeInfo(type);
}
