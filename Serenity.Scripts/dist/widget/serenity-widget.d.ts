/// <reference types="react" />
/// <reference types="jquery" />
declare namespace Serenity {
    namespace Decorators {
        function registerClass(nameOrIntf?: string | any[], intf2?: any[]): (target: Function) => void;
        function registerInterface(nameOrIntf?: string | any[], intf2?: any[]): (target: Function) => void;
        function registerEditor(nameOrIntf?: string | any[], intf2?: any[]): (target: Function) => void;
        function addAttribute(type: any, attr: any): void;
    }
    class EnumKeyAttribute {
        value: string;
        constructor(value: string);
    }
    namespace Decorators {
        function enumKey(value: string): (target: Function) => void;
        function registerEnum(target: any, enumKey?: string, name?: string): void;
        function registerEnumType(target: any, name?: string, enumKey?: string): void;
    }
}
declare namespace System.ComponentModel {
    class DisplayNameAttribute {
        displayName: string;
        constructor(displayName: string);
    }
}
declare namespace Serenity {
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
    class IDialog {
    }
    interface IDialog {
        dialogOpen(asPanel?: boolean): void;
    }
}
declare namespace Serenity {
    class IEditDialog {
    }
    interface IEditDialog {
        load(entityOrId: any, done: () => void, fail: (p1: any) => void): void;
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
    interface IValidateRequired {
        get_required(): boolean;
        set_required(value: boolean): void;
    }
    class IValidateRequired {
    }
}
declare namespace Serenity {
    enum CaptureOperationType {
        Before = 0,
        Delete = 1,
        Insert = 2,
        Update = 3
    }
}
declare namespace Serenity {
    interface DataChangeInfo {
        type: string;
        entityId: any;
        entity: any;
    }
}
declare namespace Serenity.DialogTypeRegistry {
    function tryGet(key: string): WidgetDialogClass;
    function get(key: string): WidgetDialogClass;
}
declare namespace Serenity {
    namespace EditorTypeRegistry {
        function get(key: string): WidgetClass;
        function reset(): void;
    }
}
declare namespace Serenity {
    namespace EnumTypeRegistry {
        function tryGet(key: string): Function;
        function get(key: string): Function;
    }
}
declare namespace Serenity {
    namespace ReflectionUtils {
        function getPropertyValue(o: any, property: string): any;
        function setPropertyValue(o: any, property: string, value: any): void;
        function makeCamelCase(s: string): string;
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
        getGridField(): JQuery;
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
        change(handler: (e: JQueryEventObject) => void): void;
        changeSelect2(handler: (e: JQueryEventObject) => void): void;
    }
}
interface JQuery {
    getWidget<TWidget>(widgetType: {
        new (...args: any[]): TWidget;
    }): TWidget;
    tryGetWidget<TWidget>(widgetType: {
        new (...args: any[]): TWidget;
    }): TWidget;
}
declare namespace Serenity {
    class TemplatedWidget<TOptions> extends Widget<TOptions> {
        protected idPrefix: string;
        private static templateNames;
        constructor(container: JQuery, options?: TOptions);
        protected byId(id: string): JQuery;
        private byID;
        private static noGeneric;
        private getDefaultTemplateName;
        protected getTemplateName(): string;
        protected getFallbackTemplate(): string;
        protected getTemplate(): string;
    }
}
declare namespace Serenity {
    class CascadedWidgetLink<TParent extends Widget<any>> {
        private parentType;
        private widget;
        private parentChange;
        constructor(parentType: {
            new (...args: any[]): TParent;
        }, widget: Serenity.Widget<any>, parentChange: (p1: TParent) => void);
        private _parentID;
        bind(): TParent;
        unbind(): TParent;
        get_parentID(): string;
        set_parentID(value: string): void;
    }
}
declare namespace Serenity {
    namespace UploadHelper {
        function addUploadInput(options: UploadInputOptions): JQuery;
        function checkImageConstraints(file: UploadResponse, opt: FileUploadConstraints): boolean;
        function fileNameSizeDisplay(name: string, bytes: number): string;
        function fileSizeDisplay(bytes: number): string;
        function hasImageExtension(filename: string): boolean;
        function thumbFileName(filename: string): string;
        function dbFileUrl(filename: string): string;
        function colorBox(link: JQuery, options: any): void;
        function populateFileSymbols(container: JQuery, items: UploadedFile[], displayOriginalName?: boolean, urlPrefix?: string): void;
    }
    interface UploadedFile {
        Filename?: string;
        OriginalName?: string;
    }
    interface UploadInputOptions {
        container?: JQuery;
        zone?: JQuery;
        progress?: JQuery;
        inputName?: string;
        allowMultiple?: boolean;
        fileDone?: (p1: UploadResponse, p2: string, p3: any) => void;
    }
    interface UploadResponse {
        TemporaryFile: string;
        Size: number;
        IsImage: boolean;
        Width: number;
        Height: number;
    }
    interface FileUploadConstraints {
        minWidth?: number;
        maxWidth?: number;
        minHeight?: number;
        maxHeight?: number;
        minSize?: number;
        maxSize?: number;
        allowNonImage?: boolean;
        originalNameProperty?: string;
    }
}
declare namespace Serenity {
    interface ToolButton {
        title?: string;
        hint?: string;
        cssClass?: string;
        icon?: string;
        onClick?: any;
        htmlEncode?: any;
        hotkey?: string;
        hotkeyAllowDefault?: boolean;
        hotkeyContext?: any;
        separator?: (false | true | 'left' | 'right' | 'both');
        visible?: boolean | (() => boolean);
        disabled?: boolean | (() => boolean);
    }
    interface PopupMenuButtonOptions {
        menu?: JQuery;
        onPopup?: () => void;
        positionMy?: string;
        positionAt?: string;
    }
    class PopupMenuButton extends Widget<PopupMenuButtonOptions> {
        constructor(div: JQuery, opt: PopupMenuButtonOptions);
        destroy(): void;
    }
    interface PopupToolButtonOptions extends PopupMenuButtonOptions {
    }
    class PopupToolButton extends PopupMenuButton {
        constructor(div: JQuery, opt: PopupToolButtonOptions);
    }
    interface ToolbarOptions {
        buttons?: ToolButton[];
        hotkeyContext?: any;
    }
    class Toolbar extends Widget<ToolbarOptions> {
        constructor(div: JQuery, options: ToolbarOptions);
        destroy(): void;
        protected mouseTrap: any;
        protected createButton(container: JQuery, b: ToolButton): void;
        findButton(className: string): JQuery;
        updateInterface(): void;
    }
}
declare namespace Serenity {
    namespace EditorUtils {
        function getDisplayText(editor: Serenity.Widget<any>): string;
        function getValue(editor: Serenity.Widget<any>): any;
        function saveValue(editor: Serenity.Widget<any>, item: PropertyItem, target: any): void;
        function setValue(editor: Serenity.Widget<any>, value: any): void;
        function loadValue(editor: Serenity.Widget<any>, item: PropertyItem, source: any): void;
        function setReadonly(elements: JQuery, isReadOnly: boolean): JQuery;
        function setReadOnly(widget: Serenity.Widget<any>, isReadOnly: boolean): void;
        function setRequired(widget: Serenity.Widget<any>, isRequired: boolean): void;
        function setContainerReadOnly(container: JQuery, readOnly: boolean): void;
    }
}
declare namespace Serenity {
    namespace LazyLoadHelper {
        function executeOnceWhenShown(element: JQuery, callback: Function): void;
        function executeEverytimeWhenShown(element: JQuery, callback: Function, callNowIfVisible: boolean): void;
    }
}
declare namespace Serenity {
    namespace SubDialogHelper {
        function bindToDataChange(dialog: any, owner: Serenity.Widget<any>, dataChange: (p1: any, p2: DataChangeInfo) => void, useTimeout?: boolean): any;
        function triggerDataChange(dialog: Serenity.Widget<any>): any;
        function triggerDataChanged(element: JQuery): JQuery;
        function bubbleDataChange(dialog: any, owner: Serenity.Widget<any>, useTimeout?: boolean): any;
        function cascade(cascadedDialog: any, ofElement: JQuery): any;
        function cascadedDialogOffset(element: JQuery): any;
    }
}
declare namespace Serenity {
    class PrefixedContext {
        readonly idPrefix: string;
        constructor(idPrefix: string);
        byId(id: string): JQuery;
        w<TWidget>(id: string, type: {
            new (...args: any[]): TWidget;
        }): TWidget;
    }
}
declare namespace Serenity {
}
