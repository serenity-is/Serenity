import { isAssignableFromSymbol, isInstanceOfTypeSymbol } from "./symbols";
import { StringLiteral, TypeInfo, ensureTypeInfo, getGlobalTypeRegistry, getTypeNameProp, globalObject, interfaceIsAssignableFrom, internalRegisterType, merge, peekTypeInfo } from "./system-internal";
export { ensureTypeInfo, getGlobalTypeRegistry, getTypeNameProp, peekTypeInfo, setTypeNameProp, type StringLiteral } from "./system-internal";

/**
 * @deprecated Use {@link getGlobalTypeRegistry} instead. Kept for backward compatibility.
 */
export const getTypeRegistry = getGlobalTypeRegistry;

/** Namespace prefix `"Serenity."` used when registering types with a fully-qualified name. */
export const nsSerenity: "Serenity." = "Serenity.";
/** Root namespace `"Serenity"` without trailing dot. */
export const SerenityNS: "Serenity" = "Serenity";

declare global {
    interface SymbolConstructor {
        readonly typeInfo: unique symbol;
    }
}

export { isAddRowSymbol } from "./symbols";

/**
 * Gets the global object (`window` in browsers, `global` in Node / workers).
 * @returns The global object instance.
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
 * Type alias for any runtime type representation.
 * In Serenity this is either a constructor function (class) or a plain object (enum).
 */
export type Type = Function | Object;

/**
 * Get a nested property from an object. Can be used to get nested properties from global object for example by separating names with dots.
 * @param from Object to get the property from
 * @param name Name of the property (dot-separated for nested properties)
 * @returns Value of the property or null if not found
 */
export function getNested(from: any, name: string) {
    if (from == null || name == null)
        return null;
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
        type = getGlobalTypeRegistry()[name];
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
 * Gets the full name of a type including its namespace if any.
 * Prefers the registered `typeInfo.typeName` when available, otherwise falls back to the function's `name` property.
 * @param type - Type to get the full name of.
 * @returns Fully-qualified type name (e.g. `"Serenity.StringEditor"`).
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
        throw new Error("Can't get instance type of null or undefined!");

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

function isInterfaceType(type: any): type is InterfaceType {
    return typeof type === "function" && peekTypeInfo(type)?.typeKind === "interface";
}

function isAttributeClass(item: any): item is { new(): CustomAttribute } {
    return typeof item === "function" && isAssignableFrom(CustomAttribute, item);
}

function isAttributeFactory(item: any): item is (() => CustomAttribute) {
    return typeof item === "function" && item.isAttributeFactory === true;
}

function extractInterfaces(intfAndAttr: (InterfaceType | AttributeSpecifier)[]): InterfaceType[] {
    const result = intfAndAttr?.filter(isInterfaceType) as InterfaceType[];
    if (result && result.length === intfAndAttr.length)
        return intfAndAttr as InterfaceType[];
    return result;
}

function extractAttributes(intfAndAttr: (InterfaceType | AttributeSpecifier)[]): CustomAttribute[] {
    const result: CustomAttribute[] = [];
    if (!intfAndAttr)
        return result;

    for (const item of intfAndAttr) {
        if (isInterfaceType(item))
            continue;

        if (isInstanceOfType(item, CustomAttribute)) {
            result.push(item as CustomAttribute);
            continue;
        }

        if (isAttributeClass(item)) {
            result.push(new (item as { new(): CustomAttribute })());
            continue;
        }

        if (isAttributeFactory(item)) {
            result.push(item());
            continue;
        }

        throw new Error("Invalid attribute or interface specifier: " + item);
    }

    return result
}

/**
 * Register a class with the type system.
 * @param type Class type to register
 * @param name Name to register the class under
 * @param intfAndAttr Optional interfaces and attributes the class implements
 */
export function registerClass(type: any, name: string, intfAndAttr?: (InterfaceType | AttributeSpecifier)[]): void {
    internalRegisterType(type, name, extractInterfaces(intfAndAttr), "class");
    for (const attr of extractAttributes(intfAndAttr)) {
        addCustomAttribute(type, attr);
    }
}

/**
 * Base class for all Serenity custom attributes (metadata attached to types).
 * Attributes are stored on `typeInfo.customAttributes` and queried via
 * {@link getCustomAttribute} / {@link hasCustomAttribute}.
 */
export abstract class CustomAttribute {
    static [Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }
    declare private readonly isCustomAttribute: true;
}

/**
 * Attribute that overrides the lookup key under which an enum is registered in the global type registry.
 * By default the enum's full name is used as the key; this attribute allows an alternative key.
 */
export class EnumKeyAttribute extends CustomAttribute {
    static override[Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }

    /**
     * Creates a new enum-key attribute.
     * @param value - Alternative registry key for the enum (e.g. `"MyApp.MyEnum"`).
     */
    constructor(public value: string) {
        super();
    }
}

/**
 * Register an enum with the type system.
 * @param enumType Enum type to register
 * @param name Name to register the enum under
 * @param optOrEnumKey Alternative enum key or options for the enum registration, including alternative key and icon classes.
 */
export function registerEnum(enumType: any, name: string, optOrEnumKey?: string | {
    /** Enum key */
    enumKey?: string,
    /** Icon classes for enum values, numeric or string */
    enumIcons?: Record<string | number, string>,
    /** Whether the enum is a set of flags, like a [Flags] enum on the server side, whose values are OR-ed together */
    enumFlags?: boolean
}) {
    if (typeof enumType !== "object" || enumType == null)
        throw new Error("Enum type is required in registerEnum!");

    if (name && name.endsWith("."))
        throw new Error("Enum name cannot end with a dot in registerEnum!");

    internalRegisterType(enumType, name, undefined, "enum");
    const enumKey = typeof optOrEnumKey === "string" ? optOrEnumKey : optOrEnumKey?.enumKey;
    const iconClasses = optOrEnumKey != null && typeof optOrEnumKey === "object" ? optOrEnumKey?.enumIcons : undefined;
    if (enumKey && enumKey != name) {
        if (!hasCustomAttribute(enumType, EnumKeyAttribute, false))
            addCustomAttribute(enumType, new EnumKeyAttribute(enumKey));
        const typeStore = getGlobalTypeRegistry();
        if (!typeStore[enumKey])
            typeStore[enumKey] = enumType;
    }

    const typeInfo = ensureTypeInfo(enumType);
    if (typeInfo.enumKind == null)
        typeInfo.enumKind = hasStringMembers(enumType) ? "string" : "numeric";

    if (iconClasses)
        typeInfo.enumIcons = Object.assign(typeInfo.enumIcons ?? {}, iconClasses);

    if (optOrEnumKey != null && typeof optOrEnumKey === "object" && optOrEnumKey.enumFlags != null)
        typeInfo.enumFlags = !!optOrEnumKey.enumFlags;
}

/**
 * Checks whether an enum object has string members, i.e. whether it is generated from a StringEnum. A numeric TS
 * enum reverse-maps names to numbers, so an entry only counts when its value is a string and the entry for that
 * value is not a number.
 */
function hasStringMembers(enumType: any): boolean {
    if (enumType == null || typeof enumType !== "object")
        return false;

    for (const i in enumType) {
        if (isStringMember(enumType, i))
            return true;
    }

    return false;
}

/** Checks whether the entry is an own string member, not the reverse mapping of a numeric member */
function isStringMember(enumType: any, key: string): boolean {
    return Object.prototype.hasOwnProperty.call(enumType, key) &&
        typeof enumType[key] === "string" &&
        typeof enumType[enumType[key]] !== "number";
}

/** The number of bits in a value of a set of flags, which is more than one for a member combining others */
function bitCount(value: number): number {
    let count = 0;
    for (; value; value &= value - 1)
        count++;
    return count;
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
export function registerInterface(type: any, name: string, intf?: InterfaceType[]) {
    internalRegisterType(type, name, extractInterfaces(intf), "interface");
    Object.defineProperty(type, isAssignableFromSymbol, { value: interfaceIsAssignableFrom, configurable: true });
}

/**
 * Enum utilities
 */
export const Enum = {
    /**
     * Checks whether an enum has string members, i.e. whether it is generated from a StringEnum. The kind is
     * recorded when the enum is registered; before that, the members are inspected.
     * @param enumType Enum type
     * @returns True when the members of the enum are strings
     */
    isStringEnum: (enumType: any): boolean => {
        const kind = peekTypeInfo(enumType)?.enumKind;
        if (kind != null)
            return kind === "string";

        return hasStringMembers(enumType);
    },

    /**
     * Convert an enum value to a string containing enum names.
     * @param enumType Enum type
     * @param value Enum value
     */
    toString: (enumType: any, value: number | string): string => {
        if (value == null)
            return "";

        if (typeof value !== "number") {
            // for a string enum the value is stored as it is, so the name of its member is returned. Numeric enums
            // keep returning the value itself, as their editor values are strings of the numeric values
            if (typeof value === "string" && Enum.isStringEnum(enumType)) {
                const members = enumType;
                for (const i in members) {
                    if (isStringMember(members, i) && members[i] === value)
                        return i;
                }
            }
            return "" + value;
        }

        // a value that is a member, also one of a set of flags combining others like SourceAdapter, is its name, as in .NET
        const values = enumType;
        for (const i in values) {
            if (values[i] === value) {
                return i;
            }
        }

        if (value === 0 || !peekTypeInfo(enumType)?.enumFlags)
            return value.toString();

        // otherwise it is the fewest members that make it, the ones combining others first, as long as all their bits are in the
        // value and some are not covered yet, listed in their order, and the bits that no member has as a number
        const names = Object.keys(values).filter(name => typeof values[name] === "number" && values[name] > 0 &&
            (values[name] & value) === values[name]);
        let covered = 0;
        const picked = new Set<string>();
        for (const name of [...names].sort((a, b) => bitCount(values[b]) - bitCount(values[a]))) {
            if ((values[name] & ~covered) !== 0) {
                picked.add(name);
                covered |= values[name];
            }
        }

        const parts = names.filter(name => picked.has(name));
        const rest = value & ~covered;
        if (rest !== 0)
            parts.push(rest.toString());
        return parts.join(' | ');
    },

    /**
     * Get the icon class for a specific enum value.
     * @param enumType Enum type
     * @param value Enum value, numeric or string
     * @returns Icon class name or undefined if not found
     */
    getIconClass: (enumType: any, value: string | number): string | undefined => {
        const typeInfo = peekTypeInfo(enumType);
        return typeInfo?.enumIcons?.[value];
    },

    /**
     * Returns all values of an enum as an array, in their declaration order. A numeric enum returns its numeric
     * values, a string enum returns the string values of its members.
     * @param enumType - Enum object to enumerate.
     * @returns Array of enum values. It is typed as `any[]` by default, and can be narrowed with a type
     * argument, e.g. `Enum.getValues<WidgetCategory>(WidgetCategory)`.
     */
    getValues: <T = any>(enumType: any): T[] => {
        const parts: any[] = [];
        const values = enumType;
        if (values == null)
            return parts;

        if (Enum.isStringEnum(values)) {
            for (const i in values) {
                if (isStringMember(values, i))
                    parts.push(values[i]);
            }
            return parts;
        }

        for (const i in values) {
            if (Object.prototype.hasOwnProperty.call(values, i) &&
                typeof values[i] === "number")
                parts.push(values[i]);
        }
        return parts;
    },
}

/**
 * Check if a type is an enum. A type is considered an enum if it is not a function
 * and it's [Symbol.typeInfo].typeKind is "enum".
 * @param type Type to check
 * @returns True if the type is an enum
 */
export const isEnum = (type: any) => {
    return type != null && typeof type === "object" &&
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
 * Utility type that prevents TypeScript from inferring `T` from a specific position.
 * Prefers the inferred type from other positions. TypeScript 5.4+ provides a built-in `NoInfer<T>` that can be used instead.
 * @typeParam T - Type to block inference for.
 */
export type SNoInfer<T> = [T][T extends any ? 0 : never];

/**
 * Attribute that marks a class as a Serenity editor.
 * Added automatically by {@link registerEditor} / {@link editorTypeInfo}. Can also be applied manually via `classTypeInfo`.
 */
export class EditorAttribute extends CustomAttribute {
    static override[Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }
}

/**
 * Marker interface for SleekGrid / DataGrid formatters.
 * Formatters implementing this interface declare a `format(ctx)` method and are registered via {@link registerFormatter} / {@link formatterTypeInfo}.
 */
export abstract class ISlickFormatter {
    static [Symbol.typeInfo] = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

/**
 * Register a SleekGrid formatter.
 * @param type Formatter type
 * @param name Formatter name
 * @param intfAndAttr Optional attributes and interface(s) to implement
 */
export function registerFormatter(type: any, name: string, intfAndAttr?: (InterfaceType | AttributeSpecifier)[]): void {
    registerClass(type, name, merge([ISlickFormatter], intfAndAttr));
}

/**
 * Register an editor type. Adds EditorAttribute if not already present.
 * @param type Editor type
 * @param name Editor name
 * @param intfAndAttr Optional attributes and interface(s) to implement
 */
export function registerEditor(type: any, name: string, intfAndAttr?: (InterfaceType | AttributeSpecifier)[]) {
    registerClass(type, name, extractInterfaces(intfAndAttr));
    let attributes = extractAttributes(intfAndAttr);
    addCustomAttribute(type, new EditorAttribute());
    for (const attr of attributes) {
        if (getInstanceType(attr) !== EditorAttribute)
            addCustomAttribute(type, attr);
    }
}

/**
 * Attaches a custom attribute instance to a type's metadata.
 * JavaScript has no native attribute support, so Serenity stores attributes on `typeInfo.customAttributes`.
 * @param type - Target type (class / enum object) to attach the attribute to.
 * @param attr - Attribute instance to add.
 */
export function addCustomAttribute(type: any, attr: CustomAttribute) {
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
export function getCustomAttribute<TAttr extends CustomAttribute>(type: any, attrType: { new(...args: any[]): TAttr }, inherit: boolean = true): TAttr {
    if (!type || attrType == null)
        return null;

    do {
        let attrs = peekTypeInfo(type)?.customAttributes;
        if (attrs) {
            for (let i = attrs.length - 1; i >= 0; i--) {
                let attr = attrs[i];
                if (attr != null && isInstanceOfType(attr, attrType))
                    return attr as any;
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
export function hasCustomAttribute<TAttr extends CustomAttribute>(type: any, attrType: { new(...args: any[]): TAttr }, inherit: boolean = true): boolean {
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

/**
 * TypeInfo for a class. Used with `static override [Symbol.typeInfo] = classTypeInfo("...")` to embed the type name in declaration files (decorators are erased in `.d.ts`).
 * This is one of the helper types that are used to make the type name available in declaration files, unlike decorators that does not show in .d.ts files.
 * @typeParam TypeName - String-literal type of the fully-qualified class name.
 */
export type ClassTypeInfo<TypeName> = TypeInfo<TypeName>;
/**
 * TypeInfo for an editor class. Like {@link ClassTypeInfo} but automatically includes {@link EditorAttribute}.
 * This is one of the helper types that are used to make the type name available in declaration files, unlike decorators that does not show in .d.ts files.
 * @typeParam TypeName - String-literal editor type name.
 */
export type EditorTypeInfo<TypeName> = TypeInfo<TypeName>;
/**
 * TypeInfo for a formatter class. Like {@link ClassTypeInfo} but automatically includes {@link ISlickFormatter}.
 * This is one of the helper types that are used to make the type name available in declaration files, unlike decorators that does not show in .d.ts files.
 * @typeParam TypeName - String-literal formatter type name.
 */
export type FormatterTypeInfo<TypeName> = TypeInfo<TypeName>;
/**
 * TypeInfo for an interface. Used with `static [Symbol.typeInfo] = interfaceTypeInfo("...")`.
 * This is one of the helper types that are used to make the type name available in declaration files, unlike decorators that does not show in .d.ts files.
 * @typeParam TypeName - String-literal interface name.
 */
export type InterfaceTypeInfo<TypeName> = TypeInfo<TypeName>;

/**
 * Union of forms accepted where an attribute can be specified: an attribute instance, an attribute class (instantiated with `new`), or a factory function returning an attribute. Factories are marked with `isAttributeFactory === true`.
 */
export type AttributeSpecifier = CustomAttribute | ({ new(): CustomAttribute }) | (() => CustomAttribute);
/** Interface type — a constructor function carrying an {@link InterfaceTypeInfo} via `[Symbol.typeInfo]`. */
export type InterfaceType = Function & { [Symbol.typeInfo]: InterfaceTypeInfo<string> };

/**
 * Creates {@link ClassTypeInfo} for a class. Use as `static override [Symbol.typeInfo] = classTypeInfo("MyApp.MyClass")`.
 * @typeParam TypeName - String-literal fully-qualified type name.
 * @param typeName - Fully-qualified type name (e.g. `"MyApp.MyClass"`).
 * @param intfAndAttr - Optional interfaces and attributes the class implements / carries.
 * @returns A {@link ClassTypeInfo} object to assign to `[Symbol.typeInfo]`.
 */
export function classTypeInfo<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: (InterfaceType | AttributeSpecifier)[]): ClassTypeInfo<TypeName> {
    const typeInfo: TypeInfo<TypeName> = {
        typeKind: "class",
        typeName
    }

    const interfaces = extractInterfaces(intfAndAttr);
    if (interfaces?.length)
        typeInfo.interfaces = interfaces;

    const attrs = extractAttributes(intfAndAttr);
    if (attrs?.length)
        typeInfo.customAttributes = attrs;

    return typeInfo;
}

/**
 * Creates {@link EditorTypeInfo} for an editor class. Like {@link classTypeInfo} but automatically adds {@link EditorAttribute}.
 * @typeParam TypeName - String-literal editor type name.
 * @param typeName - Fully-qualified editor name (e.g. `"MyApp.MyEditor"`).
 * @param intfAndAttr - Optional interfaces and extra attributes.
 * @returns An {@link EditorTypeInfo} to assign to `[Symbol.typeInfo]`.
 */
export function editorTypeInfo<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: (InterfaceType | AttributeSpecifier)[]): EditorTypeInfo<TypeName> {
    const typeInfo: TypeInfo<TypeName> = {
        typeKind: "class",
        typeName
    };

    const interfaces = extractInterfaces(intfAndAttr);
    if (interfaces?.length)
        typeInfo.interfaces = interfaces;

    typeInfo.customAttributes = merge([new EditorAttribute()], extractAttributes(intfAndAttr).filter(x => getInstanceType(x) !== EditorAttribute));
    return typeInfo;
}

/**
 * Creates {@link FormatterTypeInfo} for a formatter class. Automatically includes {@link ISlickFormatter}.
 * @typeParam TypeName - String-literal formatter type name.
 * @param typeName - Fully-qualified formatter name (e.g. `"MyApp.MyFormatter"`).
 * @param intfAndAttr - Optional interfaces and attributes.
 * @returns A {@link FormatterTypeInfo} to assign to `[Symbol.typeInfo]`.
 */
export function formatterTypeInfo<TypeName>(typeName: StringLiteral<TypeName>, intfAndAttr?: (InterfaceType | AttributeSpecifier)[]): FormatterTypeInfo<TypeName> {
    const typeInfo: TypeInfo<TypeName> = {
        typeKind: "class",
        typeName,
        interfaces: merge([ISlickFormatter], extractInterfaces(intfAndAttr))
    };

    const attrs = extractAttributes(intfAndAttr);
    if (attrs?.length)
        typeInfo.customAttributes = attrs;

    return typeInfo;
}

/**
 * Creates {@link InterfaceTypeInfo} for an interface.
 * @typeParam TypeName - String-literal interface name.
 * @param typeName - Fully-qualified interface name (e.g. `"MyApp.IMyInterface"`).
 * @param intf - Optional base interfaces this interface extends.
 * @returns An {@link InterfaceTypeInfo} to assign to `[Symbol.typeInfo]`.
 */
export function interfaceTypeInfo<TypeName>(typeName: StringLiteral<TypeName>, intf?: InterfaceType[]): InterfaceTypeInfo<TypeName> {
    const typeInfo: TypeInfo<TypeName> = {
        typeKind: "interface",
        typeName
    }

    const interfaces = extractInterfaces(intf);
    if (interfaces?.length)
        typeInfo.interfaces = interfaces;

    return typeInfo;
}

/**
 * Registers a type that already has a `static [Symbol.typeInfo]` declaration.
 * Called automatically by the `static { registerType(this); }` block that follows the typeInfo declaration.
 * Validates that the typeInfo exists and has a `typeName`.
 * @param type - Class / interface object carrying `[Symbol.typeInfo]` and a `name` property.
 * @throws If `type` is null, lacks `[Symbol.typeInfo]`, or its `typeName` is empty.
 */
export function registerType(type: { [Symbol.typeInfo]: TypeInfo<any>, name: string }) {
    if (!type)
        throw new Error("registerType is called with null target!");

    // peekTypeInfo should auto handle registration
    let typeInfo: TypeInfo<any> = peekTypeInfo(type);
    if (!typeInfo)
        throw new Error(`registerType is called on type "${type.name}" that does not have a static [Symbol.typeInfo] property!`);

    if (!typeInfo.typeName)
        throw new Error(`registerType is called on type "${type.name}", but it's typeInfo property does not have a typeName!`);
}

declare const transformIncludeTypeNameSymbol: unique symbol;

/**
 * Marker interface used to include a basic interface type during code generation (sergen clienttypes).
 * Corresponding types are generated server-side as simple classes; this empty interface exists for typing only.
 */
export interface TransformInclude<TypeName = ""> {
    readonly [transformIncludeTypeNameSymbol]?: TypeName;
}

/**
 * Marker interface used to include a registered class in the lazy type loader during code generation
 * (sergen servertypings / clienttypes), so that it is loaded when it is first requested by its key from a type
 * registry, without a side-effect import. Useful for types that are only referenced by their keys, like the
 * editors of dynamically generated property items. Only the classes directly implementing it are included.
 * Each type gets its own chunk, unless a group name is passed, like `LazyLoadInclude<"Viewer">`, in which case
 * the types of a project with the same group are loaded together, resulting in fewer chunks.
 * This empty interface exists for typing only.
 */
export interface LazyLoadInclude<TGroup extends string = ""> {
}