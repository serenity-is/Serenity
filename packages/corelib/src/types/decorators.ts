import { addCustomAttribute, EditorAttribute, EnumKeyAttribute, ISlickFormatter, registerClass as regClass, registerEditor as regEditor, registerEnum as regEnum, registerFormatter as regFormatter, registerInterface as regIntf, registerType as regType } from "../base";
import { addTypeMember, TypeMemberKind } from "../compat";
import { AdvancedFilteringAttribute, CloseButtonAttribute, ElementAttribute, FilterableAttribute, MaximizableAttribute, OptionAttribute, PanelAttribute, ResizableAttribute, StaticPanelAttribute } from "./attributes";

export namespace Decorators {

    export function registerType() {
        return function (target: Function & { [Symbol.typeInfo]: any }, _context?: any): void {
            if (_context && typeof _context.addInitializer === "function") {
                _context.addInitializer(() => { regType(target); });
            }
            else
                regType(target);
        }
    }

    export function registerClass(nameOrIntf?: string | any[], intf2?: any[]) {
        return function (target: Function, _context?: any) {
            if (typeof nameOrIntf == "string")
                regClass(target, nameOrIntf, intf2);
            else
                regClass(target, null, nameOrIntf);
        }
    }

    export function registerInterface(nameOrIntf?: string | any[], intf2?: any[]) {
        return function (target: Function, _context?: any) {

            if (typeof nameOrIntf == "string")
                regIntf(target, nameOrIntf, intf2);
            else
                regIntf(target, null, nameOrIntf);
        }
    }

    export function registerEditor(nameOrIntf?: string | any[], intf2?: any[]) {
        return function (target: Function, _context?: any) {
            if (typeof nameOrIntf == "string")
                regEditor(target, nameOrIntf, intf2);
            else
                regEditor(target, null, nameOrIntf);
        }
    }

    export function registerEnum(target: any, enumKey?: string, name?: string) {
        regEnum(target, name, enumKey);
        if (enumKey)
            addCustomAttribute(target, new EnumKeyAttribute(enumKey));
    }

    export function registerEnumType(target: any, name?: string, enumKey?: string) {
        registerEnum(target, enumKey ?? name, name);
    }

    export function registerFormatter(nameOrIntf: string | any[] = [ISlickFormatter], intf2: any[] = [ISlickFormatter]) {
        return function (target: Function, _context?: any) {
            if (typeof nameOrIntf == "string")
                regFormatter(target, nameOrIntf, intf2);
            else
                regFormatter(target, null, nameOrIntf);
        }
    }

    export function enumKey(value: string) {
        return function (target: Function, _context?: any) {
            addCustomAttribute(target, new EnumKeyAttribute(value));
        }
    }

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

    export function closeButton(value = true) {
        return function (target: Function, _context?: any) {
            addCustomAttribute(target, new CloseButtonAttribute(value));
        }
    }

    export function editor() {
        return function (target: Function, _context?: any) {
            addCustomAttribute(target, new EditorAttribute());
        }
    }

    export function element(value: string) {
        return function (target: Function, _context?: any) {
            addCustomAttribute(target, new ElementAttribute(value));
        }
    }

    export function advancedFiltering(value = true) {
        return function (target: Function, _context?: any) {
            addCustomAttribute(target, new AdvancedFilteringAttribute(value));
        }
    }

    /** @deprecated Use `advancedFiltering` instead */
    export const filterable = advancedFiltering;

    export function maximizable(value = true) {
        return function (target: Function, _context?: any) {
            addCustomAttribute(target, new MaximizableAttribute(value));
        }
    }

    export function panel(value = true) {
        return function (target: Function, _context?: any) {
            addCustomAttribute(target, new PanelAttribute(value));
        }
    }

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

    export function staticPanel(value: boolean = true) {
        return function (target: Function, _context?: any) {
            addCustomAttribute(target, new StaticPanelAttribute(value));
        }
    }
}

export { OptionAttribute };
