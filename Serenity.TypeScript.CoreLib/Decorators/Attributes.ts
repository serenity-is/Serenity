namespace Serenity {
    export class ColumnsKeyAttribute {
        constructor(public value: string) { }
    }

    export class DialogTypeAttribute {
        constructor(public value: Function) { }
    }

    export class EditorAttribute {
        constructor() { }
        key: string;
    }

    export class ElementAttribute {
        constructor(public value: string) { }
    }

    export class EntityTypeAttribute {
        constructor(public value: string) { }
    }

    export class EnumKeyAttribute {
        constructor(public value: string) { }
    }

    export class FlexifyAttribute {
        constructor(public value = true) { }
    }

    export class FormKeyAttribute {
        constructor(public value: string) { }
    }

    export class GeneratedCodeAttribute {
        constructor(public origin?: string) { }
    }

    export class IdPropertyAttribute {
        constructor(public value: string) { }
    }

    export class IsActivePropertyAttribute {
        constructor(public value: string) { }
    }

    export class ItemNameAttribute {
        constructor(public value: string) { }
    }

    export class LocalTextPrefixAttribute {
        constructor(public value: string) { }
    }

    export class MaximizableAttribute {
        constructor(public value = true) { }
    }

    export class NamePropertyAttribute {
        constructor(public value: string) { }
    }

    export class OptionAttribute {
        constructor() { }
    }

    export class OptionsTypeAttribute {
        constructor(public value: Function) { }
    }

    export class PanelAttribute {
        constructor(public value = true) { }
    }

    export class ResizableAttribute {
        constructor(public value = true) { }
    }

    export class ResponsiveAttribute {
        constructor(public value = true) { }
    }

    export class ServiceAttribute {
        constructor(public value: string) { }
    }
}

declare namespace Serenity {

    class DefaultValueAttribute {
        constructor(defaultValue: any);
        value: any;
    }

    class EditorOptionAttribute {
        constructor(key: string, value: any);
        key: string;
        value: any;
    }

    class EditorTypeAttribute extends EditorTypeAttributeBase {
        constructor(editorType: string);
    }

    class EditorTypeAttributeBase {
        constructor(type: string);
        setParams(editorParams: any): void;
        editorType: string;
    }

    class FilterableAttribute {
        constructor(value: boolean);
        value: boolean;
    }

    class HiddenAttribute {
    }

    class HintAttribute {
        constructor(hint: string);
        hint: string;
    }

    class InsertableAttribute {
        constructor(insertable?: boolean);
        value: boolean;
    }

    class MaxLengthAttribute {
        constructor(maxLength: number);
        maxLength: number;
    }

    class OneWayAttribute {
    }

    class PlaceholderAttribute {
        constructor(value: string);
        value: string;
    }

    class ReadOnlyAttribute {
        constructor(readOnly?: boolean);
        value: boolean;
    }

    class RequiredAttribute {
        constructor(isRequired: boolean);
        isRequired: boolean;
    }

    class UpdatableAttribute {
        constructor(updatable?: boolean);
        value: boolean;
    }
}