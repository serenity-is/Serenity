/// <reference types="react" />
/// <reference types="jquery" />
declare namespace Serenity {
    class IDialog {
    }
    interface IDialog {
        dialogOpen(asPanel?: boolean): void;
    }
}
declare namespace Serenity {
    class IBooleanValue {
    }
    interface IBooleanValue {
        get_value(): boolean;
        set_value(value: boolean): void;
    }
}
declare namespace Serenity {
    class IDoubleValue {
    }
    interface IDoubleValue {
        get_value(): any;
        set_value(value: any): void;
    }
}
declare namespace Serenity {
    class IStringValue {
    }
    interface IStringValue {
        get_value(): string;
        set_value(value: string): void;
    }
}
declare namespace Serenity {
    class IGetEditValue {
    }
    interface IGetEditValue {
        getEditValue(property: PropertyItem, target: any): void;
    }
}
declare namespace Serenity {
    class ISetEditValue {
    }
    interface ISetEditValue {
        setEditValue(source: any, property: PropertyItem): void;
    }
}
declare namespace Serenity {
    interface IReadOnly {
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
    }
    class IReadOnly {
    }
}
declare namespace Serenity {
    interface CreateWidgetParams<TWidget extends Widget<TOptions>, TOptions> {
        type?: new (element: JQuery, options?: TOptions) => TWidget;
        options?: TOptions;
        container?: JQuery;
        element?: (e: JQuery) => void;
        init?: (w: TWidget) => void;
    }
}
declare namespace System.ComponentModel {
    class DisplayNameAttribute {
        displayName: string;
        constructor(displayName: string);
    }
}
declare namespace Serenity {
    namespace Decorators {
        function registerEditor(nameOrIntf?: string | any[], intf2?: any[]): (target: Function) => void;
    }
    class CategoryAttribute {
        category: string;
        constructor(category: string);
    }
    class ColumnsKeyAttribute {
        value: string;
        constructor(value: string);
    }
    class CssClassAttribute {
        cssClass: string;
        constructor(cssClass: string);
    }
    class DefaultValueAttribute {
        value: any;
        constructor(value: any);
    }
    class DialogTypeAttribute {
        value: any;
        constructor(value: any);
    }
    class EditorAttribute {
        constructor();
        key: string;
    }
    class EditorOptionAttribute {
        key: string;
        value: any;
        constructor(key: string, value: any);
    }
    class EditorTypeAttributeBase {
        editorType: string;
        constructor(editorType: string);
        setParams(editorParams: any): void;
    }
    class EditorTypeAttribute extends EditorTypeAttributeBase {
        constructor(editorType: string);
    }
    class ElementAttribute {
        value: string;
        constructor(value: string);
    }
    class EntityTypeAttribute {
        value: string;
        constructor(value: string);
    }
    class FilterableAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class FormKeyAttribute {
        value: string;
        constructor(value: string);
    }
    class GeneratedCodeAttribute {
        origin?: string;
        constructor(origin?: string);
    }
    class HiddenAttribute {
        constructor();
    }
    class HintAttribute {
        hint: string;
        constructor(hint: string);
    }
    class IdPropertyAttribute {
        value: string;
        constructor(value: string);
    }
    class InsertableAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class IsActivePropertyAttribute {
        value: string;
        constructor(value: string);
    }
    class ItemNameAttribute {
        value: string;
        constructor(value: string);
    }
    class LocalTextPrefixAttribute {
        value: string;
        constructor(value: string);
    }
    class MaximizableAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class MaxLengthAttribute {
        maxLength: number;
        constructor(maxLength: number);
    }
    class NamePropertyAttribute {
        value: string;
        constructor(value: string);
    }
    class OneWayAttribute {
    }
    class OptionAttribute {
        constructor();
    }
    class OptionsTypeAttribute {
        value: Function;
        constructor(value: Function);
    }
    class PanelAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class PlaceholderAttribute {
        value: string;
        constructor(value: string);
    }
    class ReadOnlyAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class RequiredAttribute {
        isRequired: boolean;
        constructor(isRequired?: boolean);
    }
    class ResizableAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class ResponsiveAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class ServiceAttribute {
        value: string;
        constructor(value: string);
    }
    class UpdatableAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    namespace Decorators {
        function option(): (target: Object, propertyKey: string) => void;
        function registerFormatter(nameOrIntf?: string | any[], intf2?: any[]): (target: Function) => void;
        function dialogType(value: any): (target: Function) => void;
        function editor(key?: string): (target: Function) => void;
        function element(value: string): (target: Function) => void;
        function filterable(value?: boolean): (target: Function) => void;
        function itemName(value: string): (target: Function) => void;
        function maximizable(value?: boolean): (target: Function) => void;
        function optionsType(value: Function): (target: Function) => void;
        function panel(value?: boolean): (target: Function) => void;
        function resizable(value?: boolean): (target: Function) => void;
        function responsive(value?: boolean): (target: Function) => void;
        function service(value: string): (target: Function) => void;
    }
}
declare namespace Serenity {
    interface WidgetClass<TOptions = object> {
        new (element: JQuery, options?: TOptions): Widget<TOptions>;
        element: JQuery;
    }
    interface WidgetDialogClass<TOptions = object> {
        new (options?: TOptions): Widget<TOptions> & IDialog;
        element: JQuery;
    }
    type AnyWidgetClass<TOptions = object> = WidgetClass<TOptions> | WidgetDialogClass<TOptions>;
    class Widget<TOptions> extends React.Component<TOptions, any> {
        private static nextWidgetNumber;
        element: JQuery;
        protected options: TOptions;
        protected widgetName: string;
        protected uniqueName: string;
        constructor(element: JQuery, options?: TOptions);
        destroy(): void;
        protected addCssClass(): void;
        protected getCssClass(): string;
        static getWidgetName(type: Function): string;
        static elementFor<TWidget>(editorType: {
            new (...args: any[]): TWidget;
        }): JQuery;
        addValidationRule(eventClass: string, rule: (p1: JQuery) => string): JQuery;
        static create<TWidget extends Widget<TOpt>, TOpt>(params: CreateWidgetParams<TWidget, TOpt>): TWidget;
        init(action?: (widget: any) => void): this;
        private static __isWidgetType;
        props: Readonly<{
            children?: React.ReactNode;
        }> & Readonly<TOptions> & WidgetComponentProps<this>;
    }
    interface WidgetComponentProps<W extends Serenity.Widget<any>> {
        id?: string;
        name?: string;
        className?: string;
        maxLength?: number;
        placeholder?: string;
        setOptions?: any;
        required?: boolean;
        readOnly?: boolean;
        oneWay?: boolean;
        onChange?: (e: JQueryEventObject) => void;
        onChangeSelect2?: (e: JQueryEventObject) => void;
        value?: any;
        defaultValue?: any;
    }
    interface Widget<TOptions> {
        getGridField(): JQuery;
        change(handler: (e: JQueryEventObject) => void): void;
        changeSelect2(handler: (e: JQueryEventObject) => void): void;
    }
}
