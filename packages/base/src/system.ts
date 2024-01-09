import { typeRegistrySymbol, hookIsAssignableFromSymbol, hookIsInstanceOfTypeSymbol, typeDiscriminatorSymbol, implementedInterfacesSymbol, enumFlagsSymbol, typeAttributesSymbol } from "./symbols";

let globalObject: any =
    (typeof globalThis !== "undefined" && globalThis) ||
    (typeof window !== "undefined" && window) ||
    (typeof self !== "undefined" && self) ||
    // @ts-ignore check for global
    (typeof global !== "undefined" && global) || {};

export function getGlobalObject(): any {
    return globalObject;
}

export const typeNameProperty = "typeName";

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

function getTypeRegistry() {
    let typeRegistry = globalObject[typeRegistrySymbol];
    if (!typeRegistry)
        typeRegistry = globalObject[typeRegistrySymbol] = {};
    return typeRegistry;
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

export function getTypeNameProp(type: Type): string {
    return Object.prototype.hasOwnProperty.call(type, typeNameProperty) && (type as any)[typeNameProperty] || void 0;
}

export function setTypeNameProp(type: Type, value: string) {
    Object.defineProperty(type, typeNameProperty, { value, configurable: true, writable: true });
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

    if (typeof target[hookIsAssignableFromSymbol] === 'function')
        return target[hookIsAssignableFromSymbol](type);

    return false;
}

export function isInstanceOfType(instance: any, type: Type) {
    if (instance == null)
        return false;

    if (typeof (type as any)[hookIsInstanceOfTypeSymbol] === 'function')
        return (type as any)[hookIsInstanceOfTypeSymbol](instance);

    return isAssignableFrom(type, getInstanceType(instance));
}

export function getBaseType(type: any) {
    if (type == null ||
        type === Object ||
        !type.prototype ||
        (type as any)[typeDiscriminatorSymbol] === true)
        return null;

    return Object.getPrototypeOf(type.prototype).constructor;
}

function merge(arr1: any[], arr2: any[]) {
    if (!arr1 || !arr2)
        return (arr1 || arr2 || []).slice();

    function distinct(arr: any[]) {
        return arr.filter((item, pos) => arr.indexOf(item) === pos);
    }

    return distinct(arr1.concat(arr2));
}

function interfaceIsAssignableFrom(from: any) {
    return from != null &&
        Array.isArray((from as any)[implementedInterfacesSymbol]) &&
        (from as any)[implementedInterfacesSymbol].some((x: any) =>
            x === this ||
            (getTypeNameProp(this) &&
                x[typeDiscriminatorSymbol] &&
                getTypeNameProp(x) === getTypeNameProp(this)));
}

function registerType(type: any, name?: string, intf?: any[]) {
    if (name) {
        setTypeNameProp(type, name);
        getTypeRegistry()[name] = type;
    }
    else {
        name = getTypeNameProp(type);
        if (name) {
            getTypeRegistry()[name] = type;
        }
    }

    if (intf != null && intf.length)
        Object.defineProperty(type, implementedInterfacesSymbol, {
            value: merge(type[implementedInterfacesSymbol], intf),
            configurable: true
        });
}

export function registerClass(type: any, name: string, intf?: any[]) {
    registerType(type, name, intf);
    Object.defineProperty(type, typeDiscriminatorSymbol, { value: false, configurable: true });
}

export function registerEnum(type: any, name: string, enumKey?: string) {
    registerType(type, name, undefined);
    if (enumKey && enumKey != name) {
        const typeStore = getTypeRegistry();
        if (!typeStore[enumKey])
            typeStore[enumKey] = type;
    }
    Object.defineProperty(type, typeDiscriminatorSymbol, { value: null, configurable: true });
}

export function registerInterface(type: any, name: string, intf?: any[]) {
    registerType(type, name, intf);
    Object.defineProperty(type, typeDiscriminatorSymbol, { value: true, configurable: true });
    Object.defineProperty(type, hookIsAssignableFromSymbol, { value: interfaceIsAssignableFrom, configurable: true });
}

export namespace Enum {
    export let toString = (enumType: any, value: number): string => {
        if (value == null)
            return "";

        if (typeof value !== "number")
            return "" + value;

        var values = enumType;
        if (value === 0 || !enumType[enumFlagsSymbol]) {
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
        type[typeDiscriminatorSymbol] === null;
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

export function getjQuery(): any {
    // @ts-ignore
    return typeof jQuery === "function" ? jQuery : typeof $ === "function" && ($ as any).fn ? $ : undefined;
}

export type NoInfer<T> = [T][T extends any ? 0 : never];
type TypeName<T> = StringLiteral<T> | (string & {});
export type StringLiteral<T> = T extends string ? string extends T ? never : T : never;
export type EditorTypeName<T> = TypeName<T>;
export type FormatterTypeName<T> = TypeName<T>;
export type ClassTypeName<T> = TypeName<T>;

export class EditorAttribute { }
registerClass(EditorAttribute, 'Serenity.EditorAttribute');

export class ISlickFormatter { }
registerInterface(ISlickFormatter, 'Serenity.ISlickFormatter');

export function registerFormatter(type: any, name: string, intf?: any[]) {
    return registerClass(type, name, merge([ISlickFormatter], intf));
}

export function registerEditor(type: any, name: string, intf?: any[]) {
    registerClass(type, name, intf);
    addCustomAttribute(type, new EditorAttribute());
}

export function addCustomAttribute(type: any, attr: any) {
    if (!Object.prototype.hasOwnProperty.call(type, typeAttributesSymbol)) {
        type[typeAttributesSymbol] = [];
    }   
    let attributes = type[typeAttributesSymbol];
    if (!attributes)
        type[typeAttributesSymbol] = [attr];
    else
        attributes.push(attr);
}

export function getCustomAttribute<TAttr>(type: any, attrType: { new(...args: any[]): TAttr }, inherit: boolean = true): TAttr {
    if (!type || attrType == null)
        return null;

    do {
        let attrs = Object.prototype.hasOwnProperty.call(type, typeAttributesSymbol) ? type[typeAttributesSymbol] : null;
        if (!attrs) 
            return null;

        for (var i = attrs.length - 1; i >= 0; i--) {
            let attr = attrs[i];
            if (attr && isInstanceOfType(attr, attrType)) 
                return attr;
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
        let attrs = Object.prototype.hasOwnProperty.call(type, typeAttributesSymbol) ? type[typeAttributesSymbol] : null;
        if (attrs) {
            for (var i = attrs.length - 1; i >= 0; i--) {
                let attr = attrs[i];
                if (!attrType == null || (attr && isInstanceOfType(attr, attrType))) {
                    result.push(attr);
                }
            }
        }
    }
    while (inherit && (type = getBaseType(type)))
};

export { }