import { registerClass } from "../Decorators/Base";

function Attr(name: string) {
    return registerClass('Serenity.' + name + 'Attribute');
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

@Attr('Editor')
export class EditorAttribute {
    constructor() { 
    }
    key: string;
}

@Attr('EditorOption')
export class EditorOptionAttribute {
    constructor(public key: string, public value: any) {
    }
}

@registerClass('Serenity.EditorTypeAttributeBase')
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