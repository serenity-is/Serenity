import { addCustomAttribute, EditorAttribute, EnumKeyAttribute, ISlickFormatter, registerClass as regClass, registerEditor as regEditor, registerEnum as regEnum, registerFormatter as regFormatter, registerInterface as regIntf, registerType as regType, type InterfaceType } from "../base";
import { addTypeMember, TypeMemberKind } from "../compat";
import { AdvancedFilteringAttribute, CloseButtonAttribute, ElementAttribute, FilterableAttribute, MaximizableAttribute, OptionAttribute, PanelAttribute, ResizableAttribute, StaticPanelAttribute } from "./attributes";

/**
 * Legacy decorator helpers for Serenity type registration and widget attributes.
 * @deprecated Prefer direct `static [Symbol.typeInfo] = ...` and `static { registerType(this); }` patterns.
 */
export namespace Decorators {

    /** Legacy decorator that registers a type via `registerType`. @returns Class decorator. */
    export function registerType() {
        return function (target: Function & { [Symbol.typeInfo]: any }, _context?: any): void {
            if (_context && typeof _context.addInitializer === "function") {
                _context.addInitializer(() => { regType(target); });
            }
            else
                regType(target);
        }
    }

    /** Registers a class with an optional full name and interfaces. @param nameOrIntf - Full type name or interface list. @param intf2 - Additional interfaces. @returns Class decorator. */
    export function registerClass(nameOrIntf?: string | InterfaceType[], intf2?: InterfaceType[]) {
        return function (target: Function, _context?: any) {
            if (typeof nameOrIntf == "string")
                regClass(target, nameOrIntf, intf2);
            else
                regClass(target, null, nameOrIntf);
        }
    }

    /** Registers an interface. @param nameOrIntf - Full type name or interface list. @param intf2 - Additional interfaces. @returns Class decorator. */
    export function registerInterface(nameOrIntf?: string | InterfaceType[], intf2?: InterfaceType[]) {
        return function (target: Function, _context?: any) {

            if (typeof nameOrIntf == "string")
                regIntf(target, nameOrIntf, intf2);
            else
                regIntf(target, null, nameOrIntf);
        }
    }

    /** Registers an editor class. @param nameOrIntf - Full type name or interface list. @param intf2 - Additional interfaces. @returns Class decorator. */
    export function registerEditor(nameOrIntf?: string | InterfaceType[], intf2?: InterfaceType[]) {
        return function (target: Function, _context?: any) {
            if (typeof nameOrIntf == "string")
                regEditor(target, nameOrIntf, intf2);
            else
                regEditor(target, null, nameOrIntf);
        }
    }

    /** Registers an enum with optional keys. @param target - Enum object. @param enumKey - Legacy lookup key. @param name - Full type name. */
    export function registerEnum(target: any, enumKey?: string, name?: string) {
        regEnum(target, name, enumKey);
        if (enumKey)
            addCustomAttribute(target, new EnumKeyAttribute(enumKey));
    }

    /** @deprecated Use `registerEnum` instead. @param target - Enum object. @param name - Full type name. @param enumKey - Legacy lookup key. */
    export function registerEnumType(target: any, name?: string, enumKey?: string) {
        registerEnum(target, enumKey ?? name, name);
    }

    /** Registers a formatter class. @param nameOrIntf - Full type name or interface list (default `[ISlickFormatter]`). @param intf2 - Additional interfaces. @returns Class decorator. */
    export function registerFormatter(nameOrIntf: string | InterfaceType[] = [ISlickFormatter], intf2: InterfaceType[] = [ISlickFormatter]) {
        return function (target: Function, _context?: any) {
            if (typeof nameOrIntf == "string")
                regFormatter(target, nameOrIntf, intf2);
            else
                regFormatter(target, null, nameOrIntf);
        }
    }

    /** Attaches an {@link EnumKeyAttribute} to an enum. @param value - Lookup key for the enum. @returns Class decorator. */
    export function enumKey(value: string) {
        return function (target: Function, _context?: any) {
            addCustomAttribute(target, new EnumKeyAttribute(value));
        }
    }

    /** Marks a property/field as a reflective option (adds {@link OptionAttribute}). @returns Property decorator. */
    export function option() {
        return function (target: Object, propertyKey: string): void {

            const isGetSet = propertyKey?.startsWith('get_') || propertyKey?.startsWith('set_');
            const memberName = isGetSet ? propertyKey.substring(4) : propertyKey;

            addTypeMember(target.constructor, {
                name: memberName,
                attr: [new OptionAttribute()],
                kind: isGetSet ? TypeMemberKind.property : TypeMemberKind.field,
                getter: isGetSet ? ('get_' + memberName) : null,
                setter: isGetSet ? ('set_' + memberName) : null
            });
        }
    }

    /** Adds a {@link CloseButtonAttribute}. @param value - True to show close button (default `true`). @returns Class decorator. */
    export function closeButton(value = true) {
        return function (target: Function, _context?: any) {
            addCustomAttribute(target, new CloseButtonAttribute(value));
        }
    }

    /** Adds an {@link EditorAttribute}. @returns Class decorator. */
    export function editor() {
        return function (target: Function, _context?: any) {
            addCustomAttribute(target, new EditorAttribute());
        }
    }

    /** Adds an {@link ElementAttribute}. @param value - Element tag name. @returns Class decorator. */
    export function element(value: string) {
        return function (target: Function, _context?: any) {
            addCustomAttribute(target, new ElementAttribute(value));
        }
    }

    /** Adds an {@link AdvancedFilteringAttribute}. @param value - True to enable (default `true`). @returns Class decorator. */
    export function advancedFiltering(value = true) {
        return function (target: Function, _context?: any) {
            addCustomAttribute(target, new AdvancedFilteringAttribute(value));
        }
    }

    /** @deprecated Use `advancedFiltering` instead */
    export const filterable = advancedFiltering;

    /** Adds a {@link MaximizableAttribute}. @param value - True to allow maximizing (default `true`). @returns Class decorator. */
    export function maximizable(value = true) {
        return function (target: Function, _context?: any) {
            addCustomAttribute(target, new MaximizableAttribute(value));
        }
    }

    /** Adds a {@link PanelAttribute}. @param value - True to prefer panel mode (default `true`). @returns Class decorator. */
    export function panel(value = true) {
        return function (target: Function, _context?: any) {
            addCustomAttribute(target, new PanelAttribute(value));
        }
    }

    /** Adds a {@link ResizableAttribute}. @param value - True to allow resizing (default `true`). @returns Class decorator. */
    export function resizable(value = true) {
        return function (target: Function, _context?: any) {
            addCustomAttribute(target, new ResizableAttribute(value));
        }
    }

    /**
     * Deprecated as all dialogs are responsive.
     * @deprecated This is no longer used as all dialogs are responsive.
     */
    export function responsive(value = true) {
        return function (target: Function, _context?: any) {
        }
    }

    /** Adds a {@link StaticPanelAttribute}. @param value - True for static panel (default `true`). @returns Class decorator. */
    export function staticPanel(value: boolean = true) {
        return function (target: Function, _context?: any) {
            addCustomAttribute(target, new StaticPanelAttribute(value));
        }
    }
}

export { OptionAttribute };
