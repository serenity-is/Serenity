import {
    addAttribute, addTypeMember, MemberType, ISlickFormatter,
    registerClass as regClass, registerEditor as regEditor, registerInterface as regIntf, registerEnum as regEnum,
    startsWith,
    EditorAttribute
} from "@serenity-is/corelib/q";

function Attr(name: string) {
    return function (target: Function) {
        return regClass(target, 'Serenity.' + name + 'Attribute');
    }
}

@Attr('EnumKey')
export class EnumKeyAttribute {
    constructor(public value: string) {
    }
}

@Attr('DisplayName')
export class DisplayNameAttribute {
    constructor(public displayName: string) {
    }
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
    constructor(public value: any) {
    }
}

@Attr('EditorOption')
export class EditorOptionAttribute {
    constructor(public key: string, public value: any) {
    }
}

export class EditorTypeAttributeBase {
    constructor(public editorType: string) {
    }

    public setParams(editorParams: any): void {
    }
}

regClass(EditorTypeAttributeBase, 'Serenity.EditorTypeAttributeBase');

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

@Attr('Filterable')
export class FilterableAttribute {
    constructor(public value = true) {
    }
}

@Attr('Flexify')
export class FlexifyAttribute {
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

export namespace Decorators {

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
            addAttribute(target, new EnumKeyAttribute(enumKey));
    }

    export function registerEnumType(target: any, name?: string, enumKey?: string) {
        registerEnum(target, enumKey ?? name, name);
    }

    export function registerFormatter(nameOrIntf: string | any[] = [ISlickFormatter],
        intf2: any[] = [ISlickFormatter]) {
        return registerClass(nameOrIntf, intf2);
    }

    export function enumKey(value: string) {
        return function (target: Function) {
            addAttribute(target, new EnumKeyAttribute(value));
        }
    }

    export function option() {
        return function (target: Object, propertyKey: string): void {

            var isGetSet = startsWith(propertyKey, 'get_') || startsWith(propertyKey, 'set_');
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

    export function dialogType(value: any) {
        return function (target: Function) {
            addAttribute(target, new DialogTypeAttribute(value));
        }
    }

    export function editor() {
        return function (target: Function) {
            var attr = new EditorAttribute();
            addAttribute(target, attr);
        }
    }

    export function element(value: string) {
        return function (target: Function) {
            addAttribute(target, new ElementAttribute(value));
        }
    }

    export function filterable(value = true) {
        return function (target: Function) {
            addAttribute(target, new FilterableAttribute(value));
        }
    }

    export function flexify(value = true) {
        return function (target: Function) {
            addAttribute(target, new FlexifyAttribute(value));
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