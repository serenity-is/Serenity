import { isAssignableFromSymbol, isInstanceOfTypeSymbol } from "./symbols";
import { StringLiteral, TypeInfo, ensureTypeInfo, getTypeNameProp, getTypeRegistry, globalObject, interfaceIsAssignableFrom, internalRegisterType, merge, peekTypeInfo } from "./system-internal";
export { ensureTypeInfo, getTypeNameProp, getTypeRegistry, setTypeNameProp, peekTypeInfo, type StringLiteral } from "./system-internal";

export const nsSerenity: "Serenity." = "Serenity.";
export const SerenityNS: "Serenity" = "Serenity";

declare global {
  interface SymbolConstructor {
    readonly typeInfo: unique symbol;
  }
}


/**
 * Get the global object  (window in browsers, global in node)
 */
export function getGlobalObject(): any {
    return globalObject;
}

/**
 * Omit undefined properties from an object. Does not modify the original object.
 * This is useful when using Object.assign to avoid overwriting existing values with undefined
 * just like jQuery $.extend does.
 * @param x Object to omit undefined properties from
 * @returns New object without undefined properties
 */
export function omitUndefined(x: { [key: string]: any }) {
    if (x == null)
        return x;
    let obj = Object.create(null);
    Object.entries(x).forEach(([key, value]) => value !== void 0 && (obj[key] = value));
    return obj;
}

/**
 * Type alias for a function or object (enum).
 */
export type Type = Function | Object;

/**
 * Get a nested property from an object. Can be used to get nested properties from global object for example by separating names with dots.
 * @param from Object to get the property from
 * @param name Name of the property (dot-separated for nested properties)
 * @returns Value of the property or null if not found
 */
export function getNested(from: any, name: string) {
    const a = name.split('.');
    for (let i = 0; i < a.length; i++) {
        from = from[a[i]];
        if (from == null)
            return null;
    }
    return from;
}

/**
 * Get a type by name from the type registry, global object or a specific target.
 * @param name Name of the type
 * @param target Target object to search in (defaults to global object)
 * @returns The type or null if not found
 */
export function getType(name: string, target?: any): Type {
    let type: any;
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

/**
 * Get the full name of a type (including namespace if any).
 * This returns the name from typeInfo.typeName if available (e.g. registered via decorators), 
 * otherwise tries to get the name from function's name property.
 * @param type Type to get the name of
 */
export function getTypeFullName(type: Type): string {
    return getTypeNameProp(type) || (type as any).name ||
        (type.toString().match(/^\s*function\s*([^\s(]+)/) || [])[1] || 'Object';
};

/**
 * Get the short name of a type (without namespace).
 * @param type Type to get the name of
 * @returns Short name of the type
 */
export function getTypeShortName(type: Type): string {
    const fullName = getTypeFullName(type);
    const bIndex = fullName?.indexOf('[');
    const nsIndex = fullName?.lastIndexOf('.', bIndex >= 0 ? bIndex : fullName.length);
    return nsIndex > 0 ? fullName.substring(nsIndex + 1) : fullName;
};

/**
 * Get the instance type of an object.
 * @param instance Object to get the instance type of
 * @returns The instance type or Object if not found
 */
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

/**
 * Check if a type is assignable from another type. A type is
 * assignable from another type if they are the same or if the other type
 * is derived from it. This also works for interfaces if they are registered
 * via registerInterface function or decorators.
 * @param target Target type or interface
 * @param fromType Type to check assignability from
 * @returns true if target is assignable from type
 */
export function isAssignableFrom(target: any, fromType: Type) {
    if (target === fromType || (fromType as any).prototype instanceof target)
        return true;

    if (typeof target[isAssignableFromSymbol] === 'function')
        return target[isAssignableFromSymbol](fromType);

    return false;
}

/**
 * Check if an instance is of a specific type.
 * @param instance Object to check
 * @param type Type to check against
 * @returns true if instance is of type
 */
export function isInstanceOfType(instance: any, type: Type) {
    if (instance == null)
        return false;

    if (typeof (type as any)[isInstanceOfTypeSymbol] === 'function')
        return (type as any)[isInstanceOfTypeSymbol](instance);

    return isAssignableFrom(type, getInstanceType(instance));
}

/**
 * Get the base type of a class or interface.
 * @param type Type to get the base type of
 * @returns The base type or null if not found
 */
export function getBaseType(type: any) {
    if (type == null ||
        type === Object ||
        !type.prototype ||
        (type as any)[Symbol.typeInfo]?.typeKind === "interface")
        return null;

    return Object.getPrototypeOf(type.prototype).constructor;
}

/**
 * Register a class with the type system.
 * @param type Class type to register
 * @param name Name to register the class under
 * @param intf Optional interfaces the class implements
 */
export function registerClass(type: any, name: string, intf?: any[]): void {
    internalRegisterType(type, name, intf, "class");
}

/**
 * Register an enum with the type system.
 * @param enumType Enum type to register
 * @param name Name to register the enum under
 * @param enumKey Optional key to use for the enum
 */
export function registerEnum(enumType: any, name: string, enumKey?: string) {
    if (typeof enumType !== "object" || enumType == null)
        throw "Enum type is required in registerEnum!";

    if (name && name.endsWith("."))
        throw "Enum name cannot end with a dot in registerEnum!";

    internalRegisterType(enumType, name, undefined, "enum");    
    if (enumKey && enumKey != name) {
        const typeStore = getTypeRegistry();
        if (!typeStore[enumKey])
            typeStore[enumKey] = enumType;
    }
}

/**
 * Register an interface with the type system. There is no runtime representation of interfaces
 * in JavaScript, so Serenity uses classes decorated with some special symbols to emulate
 * interfaces to some degree. This is used by the type system to support isAssignableFrom and 
 * isInstanceOfType functions for interfaces.
 * @param type Interface type to register
 * @param name Name to register the interface under
 * @param intf Optional interfaces the interface class implements
 */
export function registerInterface(type: any, name: string, intf?: any[]) {
    internalRegisterType(type, name, intf, "interface");
    Object.defineProperty(type, isAssignableFromSymbol, { value: interfaceIsAssignableFrom, configurable: true });
}

/**
 * Enum utilities
 */
export namespace Enum {
    /**
     * Convert an enum value to a string containing enum names.
     * @param enumType Enum type
     * @param value Enum value
     */
    export let toString = (enumType: any, value: number): string => {
        if (value == null)
            return "";

        if (typeof value !== "number")
            return "" + value;

        const values = enumType;
        if (value === 0 || !peekTypeInfo(enumType)?.enumFlags) {
            for (const i in values) {
                if (values[i] === value) {
                    return i;
                }
            }
            return value == null ? "" : value.toString();
        }
        else {
            const parts: string[] = [];
            for (const i in values) {
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

    /**
     * Get all numeric values of an enum as an array.
     * @param enumType 
     * @returns 
     */
    export let getValues = (enumType: any) => {
        const parts = [];
        const values = enumType;
        for (const i in values) {
            if (Object.prototype.hasOwnProperty.call(values, i) &&
                typeof values[i] === "number")
                parts.push(values[i]);
        }
        return parts;
    };
}

/**
 * Check if a type is an enum. A type is considered an enum if it is not a function
 * and it's [Symbol.typeInfo].typeKind is "enum".
 * @param type Type to check
 * @returns True if the type is an enum
 */
export const isEnum = (type: any) => {
    return typeof type === "object" &&
        (type[Symbol.typeInfo] as TypeInfo<string>)?.typeKind == "enum";
};

/**
 * Initialize a form type. This is used in the XYZForm.ts files that are generated
 * by the Serenity server typings code generator. It defines getters that call this.w() to
 * initialize form fields on the prototype of a form class.
 * @param typ Form type to initialize
 * @param nameWidgetPairs Array of name-widget pairs
 */
export function initFormType(typ: Function, nameWidgetPairs: any[]) {
    for (let i = 0; i < nameWidgetPairs.length - 1; i += 2) {
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

/**
 * Get a proxy for form fields. This proxy returns the field name for any property
 * accessed on it. This is used in form initialization to avoid having to declare
 * a variable for the fields type. There is no actual runtime check for field names,
 * so it is only used to provide intellisense and compile-time checks.
 * @returns A readonly record of form field names and same string values
 */
export function fieldsProxy<TRow>(): Readonly<Record<keyof TRow, string>> {
    return _fieldsProxy
}

/**
 * Check if an object is array-like. An object is considered array-like if it is
 * not null, is of type object, has a numeric length property and does not have
 * a nodeType property (to exclude DOM nodes).
 * @param obj Object to check
 * @returns True if the object is array-like
 */
export function isArrayLike(obj: any): obj is ArrayLike<any> {
    return typeof obj === "object" && obj != null && typeof obj.length === "number" && typeof obj.nodeType !== "number";
}

/**
 * Check if an object is Promise-like, meaning it is either a native Promise
 * or an object with then and catch methods (like jQuery Deferred).
 * @param obj Object to check
 * @returns True if the object is Promise-like
 */
export function isPromiseLike(obj: any): obj is PromiseLike<any> {
    return obj instanceof Promise || ((typeof obj === "object" && obj != null && typeof obj.then === "function" && typeof obj.catch === "function"));
}

/**
 * Utility type to prevent type inference in generic types.
 * TypeScript 5.4 has added a built-in NoInfer<T> type that can be used instead of this.
 */
export type SNoInfer<T> = [T][T extends any ? 0 : never];

/**
 * Attribute class for editors. This is used by the editorTypeInfo function
 * and registerEditor function to add EditorAttribute to editors.
 */
export class EditorAttribute { }
registerClass(EditorAttribute, 'Serenity.EditorAttribute');

/**
 * Marker interface for SlickGrid formatters.
 */
export class ISlickFormatter { }
registerInterface(ISlickFormatter, 'Serenity.ISlickFormatter');

/**
 * Register a SlickGrid formatter.
 * @param type Formatter type
 * @param name Formatter name
 * @param intfAndAttr Optional interface(s) to implement
 */
export function registerFormatter(type: any, name: string, intfAndAttr?: any[]): void {
    registerClass(type, name, merge([ISlickFormatter], intfAndAttr));
}

/**
 * Register an editor type. Adds EditorAttribute if not already present.
 * @param type Editor type
 * @param name Editor name
 * @param intf Optional interface(s) to implement
 */
export function registerEditor(type: any, name: string, intfAndAttr?: any[]) {
    registerClass(type, name, merge([new EditorAttribute()], intfAndAttr?.filter(x => typeof (x) !== "function" && x.prototype !== EditorAttribute.prototype)));
}

/**
 * Adds a custom attribute to a type. JavaScript does not have built-in support for attributes,
 * so Serenity uses a customAttributes array on typeInfo to store them. This is used by
 * decorators and some helper functions to add attributes to classes.
 * @param type 
 * @param attr 
 */
export function addCustomAttribute(type: any, attr: any) {
    let typeInfo = ensureTypeInfo(type);
    if (!typeInfo.customAttributes)
        typeInfo.customAttributes = [attr];
    else
        typeInfo.customAttributes.push(attr);
}

/**
 * Get a custom attribute of a type.
 * @param type Type to get the attribute from
 * @param attrType Attribute type to get
 * @param inherit Indicates whether to search in base types
 * @returns The custom attribute or null if not found
 */
export function getCustomAttribute<TAttr>(type: any, attrType: { new(...args: any[]): TAttr }, inherit: boolean = true): TAttr {
    if (!type || attrType == null)
        return null;

    do {
        let attrs = peekTypeInfo(type)?.customAttributes;
        if (attrs) {
            for (let i = attrs.length - 1; i >= 0; i--) {
                let attr = attrs[i];
                if (attr != null && isInstanceOfType(attr, attrType))
                    return attr;
            }
        }
    }
    while (inherit && (type = getBaseType(type)))
}

/**
 * Get whether a type has a specific custom attribute.
 * @param type Type to check
 * @param attrType Attribute type to check
 * @param inherit Indicates whether to search in base types
 * @returns True if the type has the attribute
 */
export function hasCustomAttribute<TAttr>(type: any, attrType: { new(...args: any[]): TAttr }, inherit: boolean = true): boolean {
    return getCustomAttribute(type, attrType, inherit) != null;
}

/**
 * Get all custom attributes of a type.
 * @param type Type to get the attributes from
 * @param attrType Attribute type to get. If not specified, all attributes are returned.
 * @param inherit Indicates whether to search in base types
 * @returns An array of custom attributes
 */
export function getCustomAttributes<TAttr>(type: any, attrType: { new(...args: any[]): TAttr }, inherit: boolean = true): TAttr[] {
    if (!type)
        return [];

    const allTypes = attrType === void 0;
    const result: any[] = [];
    do {
        let attrs = peekTypeInfo(type)?.customAttributes;
        if (attrs) {
            for (let i = attrs.length - 1; i >= 0; i--) {
                let attr = attrs[i];
                if (attr && (allTypes || (attrType && isInstanceOfType(attr, attrType)))) {
                    result.push(attr);
                }
            }
        }
    }
    while (inherit && (type = getBaseType(type)));
    return result;
};

export type { TypeInfo } from "./system-internal";

/** Class type information. This is used to make type name available in declaration files unlike decorators that does not show in .d.ts files. */
export type ClassTypeInfo<TypeName> = TypeInfo<TypeName>;
/** Editor type information. This is used to make type name available in declaration files unlike decorators that does not show in .d.ts files. */
export type EditorTypeInfo<TypeName> = TypeInfo<TypeName>;
/** Formatter type information. This is used to make type name available in declaration files unlike decorators that does not show in .d.ts files. */
export type FormatterTypeInfo<TypeName> = TypeInfo<TypeName>;
/** Interface type information. This is used to make type name available in declaration files unlike decorators that does not show in .d.ts files. */
export type InterfaceTypeInfo<TypeName> = TypeInfo<TypeName>;

export function classTypeInfo<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: any[]): ClassTypeInfo<TypeName> {
    const typeInfo: TypeInfo<TypeName> = {
        typeKind: "class",
        typeName
    }

    const interfaces = intfAndAttr?.filter(x => typeof (x) === "function");
    if (interfaces?.length)
        typeInfo.interfaces = interfaces;

    const attrs = intfAndAttr?.filter(x => typeof (x) !== "function")
    if (attrs?.length)
        typeInfo.customAttributes = attrs;

    return typeInfo;
}

export function editorTypeInfo<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: any[]): EditorTypeInfo<TypeName> {
    const typeInfo: TypeInfo<TypeName> = {
        typeKind: "class",
        typeName
    };

    const interfaces = intfAndAttr?.filter(x => typeof (x) === "function");
    if (interfaces?.length)
        typeInfo.interfaces = interfaces;

    typeInfo.customAttributes = merge([new EditorAttribute()], intfAndAttr?.filter(x => typeof (x) !== "function" && x.prototype !== EditorAttribute.prototype))
    return typeInfo;
}

export function formatterTypeInfo<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: any[]): FormatterTypeInfo<TypeName> {
    const typeInfo: TypeInfo<TypeName> = {
        typeKind: "class",
        typeName,
        interfaces: merge([ISlickFormatter], intfAndAttr?.filter(x => typeof (x) === "function"))
    };

    const attrs = intfAndAttr?.filter(x => typeof (x) !== "function");
    if (attrs?.length)
        typeInfo.customAttributes = attrs;

    return typeInfo;
}

export function interfaceTypeInfo<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: any[]): InterfaceTypeInfo<TypeName> {
    const typeInfo: TypeInfo<TypeName> = {
        typeKind: "interface",
        typeName
    }

    const interfaces = intfAndAttr?.filter(x => typeof (x) === "function");
    if (interfaces?.length)
        typeInfo.interfaces = interfaces;

    const attrs = intfAndAttr?.filter(x => typeof (x) !== "function")
    if (attrs?.length)
        typeInfo.customAttributes = attrs;

    return typeInfo;
}

export function registerType(type: { [Symbol.typeInfo]: TypeInfo<any>, name: string }) {
    if (!type)
        throw "registerType is called with null target!";

    // peekTypeInfo should auto handle registration
    let typeInfo: TypeInfo<any> = peekTypeInfo(type);
    if (!typeInfo)
        throw `registerType is called on type "${type.name}" that does not have a static [Symbol.typeInfo] property!`;

    if (!typeInfo.typeName)
        throw `registerType is called on type "${type.name}", but it's typeInfo property does not have a typeName!`;
}

export interface TransformInclude { }