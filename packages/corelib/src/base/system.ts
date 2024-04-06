import { implementedInterfacesSymbol, isAssignableFromSymbol, isInstanceOfTypeSymbol, isInterfaceTypeSymbol } from "./symbols";
import { StringLiteral, TypeInfo, ensureTypeInfo, getTypeNameProp, getTypeRegistry, globalObject, internalRegisterType, merge, peekTypeInfo, typeInfoProperty } from "./system-internal";
export { getTypeNameProp, getTypeRegistry, setTypeNameProp, typeInfoProperty, type StringLiteral } from "./system-internal";

export function getGlobalObject(): any {
    return globalObject;
}

export function omitUndefined(x: { [key: string]: any }) {
    if (x == null)
        return x;
    let obj = Object.create(null);
    Object.entries(x).forEach(([key, value]) => value !== void 0 && (obj[key] = value));
    return obj;
}

export type Type = Function | Object;

export function getNested(from: any, name: string) {
    var a = name.split('.');
    for (var i = 0; i < a.length; i++) {
        from = from[a[i]];
        if (from == null)
            return null;
    }
    return from;
}

export function getType(name: string, target?: any): Type {
    var type: any;
    if (target == null) {
        type = getTypeRegistry()[name];
        if (type != null || globalObject == void 0 || name === "Object")
            return type;

        target = globalObject;
    }

    type = getNested(target, name)
    if (typeof type !== 'function')
        return null;

    return type;
}

export function getTypeFullName(type: Type): string {
    return getTypeNameProp(type) || (type as any).name ||
        (type.toString().match(/^\s*function\s*([^\s(]+)/) || [])[1] || 'Object';
};

export function getTypeShortName(type: Type): string {
    var fullName = getTypeFullName(type);
    var bIndex = fullName?.indexOf('[');
    var nsIndex = fullName?.lastIndexOf('.', bIndex >= 0 ? bIndex : fullName.length);
    return nsIndex > 0 ? fullName.substring(nsIndex + 1) : fullName;
};

export function getInstanceType(instance: any): any {
    if (instance == null)
        throw "Can't get instance type of null or undefined!";

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

    if (typeof target[isAssignableFromSymbol] === 'function')
        return target[isAssignableFromSymbol](type);

    return false;
}

export function isInstanceOfType(instance: any, type: Type) {
    if (instance == null)
        return false;

    if (typeof (type as any)[isInstanceOfTypeSymbol] === 'function')
        return (type as any)[isInstanceOfTypeSymbol](instance);

    return isAssignableFrom(type, getInstanceType(instance));
}

export function getBaseType(type: any) {
    if (type == null ||
        type === Object ||
        !type.prototype ||
        (type as any)[isInterfaceTypeSymbol] === true)
        return null;

    return Object.getPrototypeOf(type.prototype).constructor;
}

function interfaceIsAssignableFrom(from: any) {
    return from != null &&
        Array.isArray((from as any)[implementedInterfacesSymbol]) &&
        (from as any)[implementedInterfacesSymbol].some((x: any) =>
            x === this ||
            (getTypeNameProp(this) &&
                x[isInterfaceTypeSymbol] &&
                getTypeNameProp(x) === getTypeNameProp(this)));
}

export function registerClass(type: any, name: string, intf?: any[]) {
    internalRegisterType(type, name, intf);
    Object.defineProperty(type, isInterfaceTypeSymbol, { value: false, configurable: true });
}

export function registerEnum(type: any, name: string, enumKey?: string) {
    internalRegisterType(type, name, undefined);
    if (enumKey && enumKey != name) {
        const typeStore = getTypeRegistry();
        if (!typeStore[enumKey])
            typeStore[enumKey] = type;
    }
    Object.defineProperty(type, isInterfaceTypeSymbol, { value: null, configurable: true });
}

export function registerInterface(type: any, name: string, intf?: any[]) {
    internalRegisterType(type, name, intf);
    Object.defineProperty(type, isInterfaceTypeSymbol, { value: true, configurable: true });
    Object.defineProperty(type, isAssignableFromSymbol, { value: interfaceIsAssignableFrom, configurable: true });
}

export namespace Enum {
    export let toString = (enumType: any, value: number): string => {
        if (value == null)
            return "";

        if (typeof value !== "number")
            return "" + value;

        var values = enumType;
        if (value === 0 || !peekTypeInfo(enumType)?.enumFlags) {
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
                if (typeof values[i] !== "number")
                    continue;

                if (values[i] & value) {
                    parts.push(i);
                    value -= values[i];
                }
            }
            if (value != 0)
                parts.push(value.toString());
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

export const isEnum = (type: any) => {
    return typeof type !== "function" &&
        type[isInterfaceTypeSymbol] === null;
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

const _fieldsProxy = new Proxy({}, { get: (_, p) => p }) as any;

export function fieldsProxy<TRow>(): Readonly<Record<keyof TRow, string>> {
    return _fieldsProxy
}

export function isArrayLike(obj: any): obj is ArrayLike<any> {
    return typeof obj === "object" && obj != null && typeof obj.length === "number" && typeof obj.nodeType !== "number";
}

export function isPromiseLike(obj: any): obj is PromiseLike<any> {
    return obj instanceof Promise || ((typeof obj === "object" && obj != null && typeof obj.then === "function" && typeof obj.catch === "function"));
}

export type NoInfer<T> = [T][T extends any ? 0 : never];

export class EditorAttribute { }
registerClass(EditorAttribute, 'Serenity.EditorAttribute');

export class ISlickFormatter { }
registerInterface(ISlickFormatter, 'Serenity.ISlickFormatter');

export function registerFormatter(type: any, name: string, intf?: any[]) {
    return registerClass(type, name, merge([ISlickFormatter], intf));
}

export function registerEditor(type: any, name: string, intf?: any[]) {
    registerClass(type, name, intf);
    if (!peekTypeInfo(type).customAttributes?.some(x => getInstanceType(x) === x))
        addCustomAttribute(type, new EditorAttribute());
}

export function addCustomAttribute(type: any, attr: any) {
    let typeInfo = ensureTypeInfo(type);
    if (!typeInfo.customAttributes)
        typeInfo.customAttributes = [attr];
    else
        typeInfo.customAttributes.push(attr);
}

export function getCustomAttribute<TAttr>(type: any, attrType: { new(...args: any[]): TAttr }, inherit: boolean = true): TAttr {
    if (!type || attrType == null)
        return null;

    do {
        let attrs = peekTypeInfo(type)?.customAttributes;
        if (attrs) {
            for (var i = attrs.length - 1; i >= 0; i--) {
                let attr = attrs[i];
                if (attr != null && isInstanceOfType(attr, attrType))
                    return attr;
            }
        }
    }
    while (inherit && (type = getBaseType(type)))
}

export function hasCustomAttribute<TAttr>(type: any, attrType: { new(...args: any[]): TAttr }, inherit: boolean = true): boolean {
    return getCustomAttribute(type, attrType, inherit) != null;
}

export function getCustomAttributes<TAttr>(type: any, attrType: { new(...args: any[]): TAttr }, inherit: boolean = true): TAttr[] {
    if (!type)
        return [];

    var result: any[] = [];
    do {
        let attrs = peekTypeInfo(type)?.customAttributes;
        if (attrs) {
            for (var i = attrs.length - 1; i >= 0; i--) {
                let attr = attrs[i];
                if (attrType != null || (attr && isInstanceOfType(attr, attrType))) {
                    result.push(attr);
                }
            }
        }
    }
    while (inherit && (type = getBaseType(type)));
    return result;
};

export type { TypeInfo } from "./system-internal";
export type ClassTypeInfo<T> = TypeInfo<T>;
export type EditorTypeInfo<T> = TypeInfo<T>;
export type FormatterTypeInfo<T> = TypeInfo<T>;
export type InterfaceTypeInfo<T> = TypeInfo<T>;

export function classTypeInfo<T>(typeName: StringLiteral<T>, interfaces?: any[]): ClassTypeInfo<T> {
    return { typeKind: "class", typeName, interfaces: interfaces }
}

export function editorTypeInfo<T>(typeName: StringLiteral<T>, interfaces?: any[]): EditorTypeInfo<T> {
    return { typeKind: "editor", typeName, interfaces: interfaces, customAttributes: [new EditorAttribute()] }
}

export function formatterTypeInfo<T>(typeName: StringLiteral<T>, interfaces?: any[]): FormatterTypeInfo<T> {
    return { typeKind: "formatter", typeName, interfaces: merge([ISlickFormatter], interfaces) }
}

export function interfaceTypeInfo<T>(typeName: StringLiteral<T>, interfaces?: any[]): InterfaceTypeInfo<T> {
    return { typeKind: "interface", typeName, interfaces }
}

export function registerType(type: { [typeInfoProperty]: TypeInfo<any>, name: string }) {
    if (!type)
        throw "Decorators.register is called with null target!";

    // peekTypeInfo should auto handle registration
    let typeInfo: TypeInfo<any> = peekTypeInfo(type);
    if (!typeInfo)
        throw `Decorators.register is called on type "${type.name}" that does not have a static typeInfo property!`;

    if (!typeInfo.typeName)
        throw `Decorators.register is called on type "${type.name}", but it's typeInfo property does not have a typeName!`;
}
