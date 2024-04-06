import { EditorAttribute, ISlickFormatter, addCustomAttribute, classTypeInfo, editorTypeInfo, formatterTypeInfo, interfaceTypeInfo, registerClass as regClass, registerEditor as regEditor, registerEnum as regEnum, registerFormatter as regFormatter, registerInterface as regIntf, registerType as regType, typeInfoProperty } from "../base";
import { MemberType, addTypeMember } from "../q";
import { CloseButtonAttribute, DialogTypeAttribute, ElementAttribute, EnumKeyAttribute, FilterableAttribute, ItemNameAttribute, MaximizableAttribute, OptionAttribute, OptionsTypeAttribute, PanelAttribute, ResizableAttribute, ResponsiveAttribute, ServiceAttribute, StaticPanelAttribute } from "./attributes";

export namespace Decorators {
    export const classType = classTypeInfo;
    export const editorType = editorTypeInfo;
    export const interfaceType = interfaceTypeInfo;
    export const formatterType = formatterTypeInfo;

    export function registerType() {
        return function(target: Function & { [typeInfoProperty]: any }): void {
            return regType(target);
        }
    }

    export function registerClass(nameOrIntf?: string | any[], intf2?: any[]) {
        return function (target: Function) {
            if (typeof nameOrIntf == "string")
                regClass(target, nameOrIntf, intf2);
            else
                regClass(target, null, nameOrIntf);
        }
    }

    export function registerInterface(nameOrIntf?: string | any[], intf2?: any[]) {
        return function (target: Function) {

            if (typeof nameOrIntf == "string")
                regIntf(target, nameOrIntf, intf2);
            else
                regIntf(target, null, nameOrIntf);
        }
    }

    export function registerEditor(nameOrIntf?: string | any[], intf2?: any[]) {
        return function (target: Function) {
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
        return function (target: Function) {
            if (typeof nameOrIntf == "string")
                regFormatter(target, nameOrIntf, intf2);
            else
                regFormatter(target, null, nameOrIntf);
        }
    }

    export function enumKey(value: string) {
        return function (target: Function) {
            addCustomAttribute(target, new EnumKeyAttribute(value));
        }
    }

    export function option() {
        return function (target: Object, propertyKey: string): void {

            var isGetSet = propertyKey?.startsWith('get_') || propertyKey?.startsWith('set_');
            var memberName = isGetSet ? propertyKey.substr(4) : propertyKey;

            addTypeMember(target.constructor, {
                name: memberName,
                attr: [new OptionAttribute()],
                type: isGetSet ? MemberType.property : MemberType.field,
                getter: isGetSet ? ('get_' + memberName) : null,
                setter: isGetSet ? ('set_' + memberName) : null
            });
        }
    }

    export function closeButton(value = true) {
        return function (target: Function) {
            addCustomAttribute(target, new CloseButtonAttribute(value));
        }
    }

    export function dialogType(value: any) {
        return function (target: Function) {
            addCustomAttribute(target, new DialogTypeAttribute(value));
        }
    }

    export function editor() {
        return function (target: Function) {
            var attr = new EditorAttribute();
            addCustomAttribute(target, attr);
        }
    }

    export function element(value: string) {
        return function (target: Function) {
            addCustomAttribute(target, new ElementAttribute(value));
        }
    }

    export function filterable(value = true) {
        return function (target: Function) {
            addCustomAttribute(target, new FilterableAttribute(value));
        }
    }

    export function itemName(value: string) {
        return function (target: Function) {
            addCustomAttribute(target, new ItemNameAttribute(value));
        }
    }

    export function maximizable(value = true) {
        return function (target: Function) {
            addCustomAttribute(target, new MaximizableAttribute(value));
        }
    }

    export function optionsType(value: Function) {
        return function (target: Function) {
            addCustomAttribute(target, new OptionsTypeAttribute(value));
        }
    }

    export function panel(value = true) {
        return function (target: Function) {
            addCustomAttribute(target, new PanelAttribute(value));
        }
    }

    export function resizable(value = true) {
        return function (target: Function) {
            addCustomAttribute(target, new ResizableAttribute(value));
        }
    }

    export function responsive(value = true) {
        return function (target: Function) {
            addCustomAttribute(target, new ResponsiveAttribute(value));
        }
    }

    export function service(value: string) {
        return function (target: Function) {
            addCustomAttribute(target, new ServiceAttribute(value));
        }
    }

    export function staticPanel(value: boolean = true) {
        return function (target: Function) {
            addCustomAttribute(target, new StaticPanelAttribute(value));
        }
    }
}

export { OptionAttribute };
