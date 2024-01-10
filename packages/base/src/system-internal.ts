import { implementedInterfacesSymbol, isInterfaceTypeSymbol, typeRegistrySymbol } from "./symbols";

export const typeInfoProperty = "typeInfo";

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

export type TypeInfo<T> = {
    typeKind: "class" | "enum" | "interface" | "editor" | "formatter";
    typeName: StringLiteral<T> | (string & {});
    interfaces?: any[];
    customAttributes?: any[];
    enumFlags?: boolean;
    registered?: boolean;
}

export function getTypeRegistry() {
    let typeRegistry = globalObject[typeRegistrySymbol];
    if (!typeRegistry)
        typeRegistry = globalObject[typeRegistrySymbol] = {};
    return typeRegistry;
}

function autoRegisterViaTypeInfo(type: any): void {
    if (!Object.prototype.hasOwnProperty.call(type, typeInfoProperty))
        return;

    var typeInfo = type[typeInfoProperty] as TypeInfo<string>;
    if (!typeInfo || typeInfo.registered || !typeInfo.typeName)
        return;

    if (!getTypeRegistry()[typeInfo.typeName])
        getTypeRegistry()[typeInfo.typeName] = type;

    if (typeInfo.interfaces?.length &&
        !Object.prototype.hasOwnProperty.call(implementedInterfacesSymbol)) {
        Object.defineProperty(type, implementedInterfacesSymbol, {
            value: merge(type[implementedInterfacesSymbol], typeInfo.interfaces),
            configurable: true
        });
    }

    if (!Object.prototype.hasOwnProperty.call(isInterfaceTypeSymbol)) {
        if (typeInfo.typeKind === "class" || typeInfo.typeKind === "editor" || typeInfo.typeKind === "formatter") {
            Object.defineProperty(type, isInterfaceTypeSymbol, { value: false, configurable: true });
        }
        else if (typeInfo.typeKind === "enum") {
            Object.defineProperty(type, isInterfaceTypeSymbol, { value: null, configurable: true });
        }
        else if (typeInfo.typeKind === "interface") {
            Object.defineProperty(type, isInterfaceTypeSymbol, { value: true, configurable: true });
        }
    }

    typeInfo.registered = true;
    return;
}

export function internalRegisterType(type: any, typeName?: string, interfaces?: any[]): TypeInfo<string> {
    let typeInfo = ensureTypeInfo(type);

    if (typeName && typeName !== typeInfo.typeName)
        typeInfo.typeName = typeName;

    if (typeInfo.typeName)
        getTypeRegistry()[typeInfo.typeName] = type;

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
    if (!Object.prototype.hasOwnProperty.call(type, typeInfoProperty) ||
        !(typeInfo = type[typeInfoProperty])) {
        typeInfo = {} as any;
        Object.defineProperty(type, typeInfoProperty, { value: typeInfo, configurable: true });
        return typeInfo;
    }
    if (!typeInfo.registered)
        autoRegisterViaTypeInfo(type);
    return typeInfo;
}

export function peekTypeInfo(type: any): TypeInfo<string> {
    if (!type || !Object.prototype.hasOwnProperty.call(type, typeInfoProperty))
        return void 0;

    var typeInfo = type[typeInfoProperty];
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

