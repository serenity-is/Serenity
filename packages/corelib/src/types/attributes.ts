import { registerClass } from "../base";

function attr(name: string) {
    return function (target: Function) {
        return registerClass(target, 'Serenity.' + name + 'Attribute');
    }
}

@attr('EnumKey')
export class EnumKeyAttribute {
    constructor(public value: string) {
    }
}

@attr('DisplayName')
export class DisplayNameAttribute {
    constructor(public displayName: string) {
    }
}

@attr('Category')
export class CategoryAttribute {
    constructor(public category: string) {
    }
}

@attr('ColumnsKey')
export class ColumnsKeyAttribute {
    constructor(public value: string) {
    }
}

@attr('CloseButton')
export class CloseButtonAttribute {
    constructor(public value = true) {
    }
}

@attr('CssClass')
export class CssClassAttribute {
    constructor(public cssClass: string) {
    }
}

@attr('DefaultValue')
export class DefaultValueAttribute {
    constructor(public value: any) {
    }
}

@attr('DialogType')
export class DialogTypeAttribute {
    constructor(public value: any) {
    }
}

@attr('EditorOption')
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

registerClass(EditorTypeAttributeBase, 'Serenity.EditorTypeAttributeBase');

@attr('EditorType')
export class EditorTypeAttribute extends EditorTypeAttributeBase {
    constructor(editorType: string) {
        super(editorType);
    }
}

@attr('Element')
export class ElementAttribute {
    constructor(public value: string) {
    }
}

@attr('EntityType')
export class EntityTypeAttribute {
    constructor(public value: string) {
    }
}

@attr('Filterable')
export class FilterableAttribute {
    constructor(public value = true) {
    }
}

@attr('Flexify')
export class FlexifyAttribute {
    constructor(public value = true) {
    }
}

@attr('FormKey')
export class FormKeyAttribute {
    constructor(public value: string) {
    }
}

@attr('GeneratedCode')
export class GeneratedCodeAttribute {
    constructor(public origin?: string) {
    }
}

@attr('Hidden')
export class HiddenAttribute {
    constructor() {
    }
}

@attr('Hint')
export class HintAttribute {
    constructor(public hint: string) {
    }
}

@attr('IdProperty')
export class IdPropertyAttribute {
    constructor(public value: string) {
    }
}

@attr('Insertable')
export class InsertableAttribute {
    constructor(public value = true) {
    }
}

@attr('IsActiveProperty')
export class IsActivePropertyAttribute {
    constructor(public value: string) {
    }
}

@attr('ItemName')
export class ItemNameAttribute {
    constructor(public value: string) {
    }
}

@attr('LocalTextPrefix')
export class LocalTextPrefixAttribute {
    constructor(public value: string) {
    }
}

@attr('Maximizable')
export class MaximizableAttribute {
    constructor(public value = true) {
    }
}

@attr('MaxLength')
export class MaxLengthAttribute {
    constructor(public maxLength: number) {
    }
}

@attr('NameProperty')
export class NamePropertyAttribute {
    constructor(public value: string) {
    }
}

@attr('OneWay')
export class OneWayAttribute {
}

@attr('Option')
export class OptionAttribute {
}

@attr('OptionsType')
export class OptionsTypeAttribute {
    constructor(public value: Function) {
    }
}

@attr('Panel')
export class PanelAttribute {
    constructor(public value = true) {
    }
}

@attr('Placeholder')
export class PlaceholderAttribute {
    constructor(public value: string) {
    }
}

@attr('ReadOnly')
export class ReadOnlyAttribute {
    constructor(public value = true) {
    }
}

@attr('Required')
export class RequiredAttribute {
    constructor(public isRequired = true) {
    }
}

@attr('Resizable')
export class ResizableAttribute {
    constructor(public value = true) {
    }
}

@attr('Responsive')
export class ResponsiveAttribute {
    constructor(public value = true) {
    }
}

@attr('Service')
export class ServiceAttribute {
    constructor(public value: string) {
    }
}

@attr('StaticPanel')
export class StaticPanelAttribute {
    constructor(public value = true) {
    }
}

@attr('Updatable')
export class UpdatableAttribute {
    constructor(public value = true) {
    }
}