
namespace Serenity {

    export namespace Decorators {
        function distinct(arr: any[]) {
            return arr.filter((item, pos) => arr.indexOf(item) === pos);
        }

        function merge(arr1: any[], arr2: any[]) {
            if (!arr1 || !arr2)
                return (arr1 || arr2 || []).slice();

            return distinct(arr1.concat(arr2));
        }

        function registerType(target: any, name: string, intf: any[]) {
            if (name != null) {
                target.__typeName = name;
                target.__assembly = ss.__assemblies['App'];
                target.__assembly.__types[name] = target;
            }
            else if (!target.__typeName)
                target.__register = true;

            if (intf)
                target.__interfaces = merge(target.__interfaces, intf);
        }

        export function registerClass(nameOrIntf?: string | any[], intf2?: any[]) {
            return function (target: Function) {
                if (typeof nameOrIntf == "string")
                    registerType(target, nameOrIntf, intf2);
                else
                    registerType(target, null, nameOrIntf);

                (target as any).__class = true;
            }
        }

        export function registerInterface(nameOrIntf?: string | any[], intf2?: any[]) {
            return function (target: Function) {

                if (typeof nameOrIntf == "string")
                    registerType(target, nameOrIntf, intf2);
                else
                    registerType(target, null, nameOrIntf);

                (target as any).__interface = true;
                (target as any).isAssignableFrom = function (type: any) {
                    return (ss as any).contains((ss as any).getInterfaces(type), this);
                };
            }
        }

        export function registerEditor(nameOrIntf?: string | any[], intf2?: any[]) {
            return registerClass(nameOrIntf, intf2);
        }
    }
}

namespace System.ComponentModel {

    @Serenity.Decorators.registerClass('System.DisplayNameAttribute')
    export class DisplayNameAttribute {
        constructor(public displayName: string) {
        }
    }
}

namespace Serenity {

    @Decorators.registerInterface('Serenity.ISlickFormatter')
    export class ISlickFormatter {
    }

    function Attr(name: string) {
        return Decorators.registerClass('Serenity.' + name + 'Attribute')
    }

    @Attr('Category')
    export class CategoryAttribute {
        constructor(public category: string) {
        }
    }

    @Attr('ColumnsKey')
    export class ColumnsKeyAttribute {
        constructor(public value: string) {
        }
    }


    @Attr('CssClass')
    export class CssClassAttribute {
        constructor(public cssClass: string) {
        }
    }

    @Attr('DefaultValue')
    export class DefaultValueAttribute {
        constructor(public value: any) {
        }
    }

    @Attr('DialogType')
    export class DialogTypeAttribute {
        constructor(public value: Function) {
        }
    }

    @Attr('Editor')
    export class EditorAttribute {
        constructor() { }
        key: string;
    }

    @Attr('EditorOption')
    export class EditorOptionAttribute {
        constructor(public key: string, public value: any) {
        }
    }

    @Decorators.registerClass('Serenity.EditorTypeAttributeBase')
    export class EditorTypeAttributeBase {
        constructor(public editorType: string) {
        }

        public setParams(editorParams: any): void {
        }
    }

    @Attr('EditorType')
    export class EditorTypeAttribute extends EditorTypeAttributeBase {
        constructor(editorType: string) {
            super(editorType);
        }
    }

    @Attr('Element')
    export class ElementAttribute {
        constructor(public value: string) {
        }
    }

    @Attr('EntityType')
    export class EntityTypeAttribute {
        constructor(public value: string) {
        }
    }

    @Attr('EnumKey')
    export class EnumKeyAttribute {
        constructor(public value: string) {
        }
    }

    @Attr('Flexify')
    export class FlexifyAttribute {
        constructor(public value = true) {
        }
    }

    @Attr('Filterable')
    export class FilterableAttribute {
        constructor(public value = true) {
        }
    }

    @Attr('FormKey')
    export class FormKeyAttribute {
        constructor(public value: string) {
        }
    }

    @Attr('GeneratedCode')
    export class GeneratedCodeAttribute {
        constructor(public origin?: string) {
        }
    }

    @Attr('Hidden')
    export class HiddenAttribute {
        constructor() {
        }
    }

    @Attr('Hint')
    export class HintAttribute {
        constructor(public hint: string) {
        }
    }

    @Attr('IdProperty')
    export class IdPropertyAttribute {
        constructor(public value: string) {
        }
    }

    @Attr('Insertable')
    export class InsertableAttribute {
        constructor(public value = true) {
        }
    }

    @Attr('IsActiveProperty')
    export class IsActivePropertyAttribute {
        constructor(public value: string) {
        }
    }

    @Attr('ItemName')
    export class ItemNameAttribute {
        constructor(public value: string) {
        }
    }

    @Attr('LocalTextPrefix')
    export class LocalTextPrefixAttribute {
        constructor(public value: string) {
        }
    }

    @Attr('Maximizable')
    export class MaximizableAttribute {
        constructor(public value = true) {
        }
    }

    @Attr('MaxLength')
    export class MaxLengthAttribute {
        constructor(public maxLength: number) {
        }
    }

    @Attr('NameProperty')
    export class NamePropertyAttribute {
        constructor(public value: string) {
        }
    }

    @Attr('OneWay')
    export class OneWayAttribute {
    }

    @Attr('Option')
    export class OptionAttribute {
        constructor() {
        }
    }

    @Attr('OptionsType')
    export class OptionsTypeAttribute {
        constructor(public value: Function) {
        }
    }

    @Attr('Panel')
    export class PanelAttribute {
        constructor(public value = true) {
        }
    }

    @Attr('Placeholder')
    export class PlaceholderAttribute {
        constructor(public value: string) {
        }
    }

    @Attr('ReadOnly')
    export class ReadOnlyAttribute {
        constructor(public value = true) {
        }
    }

    @Attr('Required')
    export class RequiredAttribute {
        constructor(public isRequired = true) {
        }
    }

    @Attr('Resizable')
    export class ResizableAttribute {
        constructor(public value = true) {
        }
    }

    @Attr('Responsive')
    export class ResponsiveAttribute {
        constructor(public value = true) {
        }
    }

    @Attr('Service')
    export class ServiceAttribute {
        constructor(public value: string) {
        }
    }

    @Attr('Updatable')
    export class UpdatableAttribute {
        constructor(public value = true) {
        }
    }
}

namespace Serenity.Decorators {

    export function registerFormatter(nameOrIntf: string | any[] = [ISlickFormatter], intf2: any[] = [ISlickFormatter]) {
        return registerClass(nameOrIntf, intf2);
    }

    export function addAttribute(type: any, attr: any) {
        type.__metadata = type.__metadata || {};
        type.__metadata.attr = type.__metadata.attr || [];
        type.__metadata.attr.push(attr);
    }

    export function dialogType(value: Function) {
        return function (target: Function) {
            addAttribute(target, new DialogTypeAttribute(value));
        }
    }

    export function editor(key?: string) {
        return function (target: Function) {
            var attr = new EditorAttribute();
            if (key !== undefined)
                attr.key = key;
            addAttribute(target, attr);
        }
    }

    export function element(value: string) {
        return function (target: Function) {
            addAttribute(target, new ElementAttribute(value));
        }
    }

    export function enumKey(value: string) {
        return function (target: Function) {
            addAttribute(target, new EnumKeyAttribute(value));
        }
    }

    export function flexify(value = true) {
        return function (target: Function) {
            addAttribute(target, new FlexifyAttribute(value));
        }
    }

    export function registerEnum(target: any, enumKey?: string, name?: string) {
        if (!target.__enum) {
            Object.defineProperty(target, '__enum', {
                get: function () {
                    return true;
                }
            });

            target.prototype = target.prototype || {};
            for (var k of Object.keys(target))
                if (isNaN(Q.parseInteger(k)) && target[k] != null && !isNaN(Q.parseInteger(target[k])))
                    target.prototype[k] = target[k];

            if (name != null) {
                target.__typeName = name;
                target.__assembly = ss.__assemblies['App'];
                target.__assembly.__types[name] = target;
            }
            else if (!target.__typeName)
                target.__register = true;

            if (enumKey)
                addAttribute(target, new EnumKeyAttribute(enumKey));
        }
    }

    export function registerEnumType(target: any, name?: string, enumKey?: string) {
        registerEnum(target, Q.coalesce(enumKey, name), name);
    }

    export function filterable(value = true) {
        return function (target: Function) {
            addAttribute(target, new FilterableAttribute(value));
        }
    }

    export function itemName(value: string) {
        return function (target: Function) {
            addAttribute(target, new ItemNameAttribute(value));
        }
    }

    export function maximizable(value = true) {
        return function (target: Function) {
            addAttribute(target, new MaximizableAttribute(value));
        }
    }

    export function option() {
        return function (target: Object, propertyKey: string): void {

            var isGetSet = Q.startsWith(propertyKey, 'get_') || Q.startsWith(propertyKey, 'set_');
            var memberName = isGetSet ? propertyKey.substr(4) : propertyKey;

            var type: any = target.constructor;
            type.__metadata = type.__metadata || {};
            type.__metadata.members = type.__metadata.members || [];
            let member: any = undefined;
            for (var m of type.__metadata.members) {
                if (m.name == memberName) {
                    member = m;
                    break;
                }
            }

            if (!member) {
                member = {
                    attr: [new Serenity.OptionAttribute()],
                    name: memberName,
                    returnType: Object
                };

                if (isGetSet) {
                    member.type = 16;

                    member.getter = {
                        name: 'get_' + memberName,
                        type: 8,
                        sname: 'get_' + memberName,
                        returnType: Object,
                        params: []
                    };

                    member.setter = {
                        name: 'set_' + memberName,
                        type: 8,
                        sname: 'set_' + memberName,
                        returnType: Object,
                        params: [Object]
                    };
                }
                else {
                    member.type = 4;
                    member.sname = memberName;
                }

                type.__metadata.members.push(member);
            }
            else {
                member.attr = member.attr || [];
                member.attr.push(new OptionAttribute());
            }
        }
    }

    export function optionsType(value: Function) {
        return function (target: Function) {
            addAttribute(target, new OptionsTypeAttribute(value));
        }
    }

    export function panel(value = true) {
        return function (target: Function) {
            addAttribute(target, new PanelAttribute(value));
        }
    }

    export function resizable(value = true) {
        return function (target: Function) {
            addAttribute(target, new ResizableAttribute(value));
        }
    }

    export function responsive(value = true) {
        return function (target: Function) {
            addAttribute(target, new ResponsiveAttribute(value));
        }
    }

    export function service(value: string) {
        return function (target: Function) {
            addAttribute(target, new ServiceAttribute(value));
        }
    }
}