/// <reference types="jquery.validation" />
declare namespace Q {
    function validatorAbortHandler(validator: any): void;
    function validateOptions(options: JQueryValidation.ValidationOptions): JQueryValidation.ValidationOptions;
}
declare namespace Serenity {
    namespace ValidationHelper {
        function asyncSubmit(form: JQuery, validateBeforeSave: () => boolean, submitHandler: () => void): boolean;
        function submit(form: JQuery, validateBeforeSave: () => boolean, submitHandler: () => void): boolean;
        function getValidator(element: JQuery): JQueryValidation.Validator;
    }
    namespace VX {
        var addValidationRule: typeof Q.addValidationRule;
        var removeValidationRule: typeof Q.removeValidationRule;
        function validateElement(validator: JQueryValidation.Validator, widget: Serenity.Widget<any>): boolean;
    }
}
declare namespace Serenity {
    class TemplatedPanel<TOptions> extends TemplatedWidget<TOptions> {
        constructor(container: JQuery, options?: TOptions);
        destroy(): void;
        protected tabs: JQuery;
        protected toolbar: Serenity.Toolbar;
        protected validator: JQueryValidation.Validator;
        protected isPanel: boolean;
        protected responsive: boolean;
        protected arrange(): void;
        protected getToolbarButtons(): ToolButton[];
        protected getValidatorOptions(): JQueryValidation.ValidationOptions;
        protected initTabs(): void;
        protected initToolbar(): void;
        protected initValidator(): void;
        protected resetValidation(): void;
        protected validateForm(): boolean;
    }
}
declare namespace Serenity {
    namespace TabsExtensions {
        function setDisabled(tabs: JQuery, tabKey: string, isDisabled: boolean): void;
        function toggle(tabs: JQuery, tabKey: string, visible: boolean): void;
        function activeTabKey(tabs: JQuery): string;
        function indexByKey(tabs: JQuery): any;
        function selectTab(tabs: JQuery, tabKey: string): void;
    }
}
declare namespace Serenity {
    namespace ReflectionOptionsSetter {
        function set(target: any, options: any): void;
    }
}
declare namespace Serenity {
    class PropertyGrid extends Widget<PropertyGridOptions> {
        private editors;
        private items;
        constructor(div: JQuery, opt: PropertyGridOptions);
        destroy(): void;
        private createItems;
        private createCategoryDiv;
        private categoryLinkClick;
        private determineText;
        private createField;
        private getCategoryOrder;
        private createCategoryLinks;
        get_editors(): Widget<any>[];
        get_items(): PropertyItem[];
        get_idPrefix(): string;
        get_mode(): PropertyGridMode;
        set_mode(value: PropertyGridMode): void;
        static loadEditorValue(editor: Serenity.Widget<any>, item: PropertyItem, source: any): void;
        static saveEditorValue(editor: Serenity.Widget<any>, item: PropertyItem, target: any): void;
        private static setReadOnly;
        private static setReadonly;
        private static setRequired;
        private static setMaxLength;
        load(source: any): void;
        save(target?: any): any;
        get value(): any;
        set value(val: any);
        private canModifyItem;
        updateInterface(): void;
        enumerateItems(callback: (p1: PropertyItem, p2: Serenity.Widget<any>) => void): void;
    }
    const enum PropertyGridMode {
        insert = 1,
        update = 2
    }
    interface PropertyGridOptions {
        idPrefix?: string;
        items?: PropertyItem[];
        useCategories?: boolean;
        categoryOrder?: string;
        defaultCategory?: string;
        localTextPrefix?: string;
        mode?: PropertyGridMode;
    }
}
declare namespace Serenity {
    class PropertyPanel<TItem, TOptions> extends TemplatedPanel<TOptions> {
        private _entity;
        private _entityId;
        constructor(container: JQuery, options?: TOptions);
        destroy(): void;
        protected initPropertyGrid(): void;
        protected loadInitialEntity(): void;
        protected getFormKey(): string;
        protected getPropertyGridOptions(): PropertyGridOptions;
        protected getPropertyItems(): PropertyItem[];
        protected getSaveEntity(): TItem;
        protected get_entity(): TItem;
        protected get_entityId(): any;
        protected set_entity(value: TItem): void;
        protected set_entityId(value: any): void;
        protected validateBeforeSave(): boolean;
        protected propertyGrid: Serenity.PropertyGrid;
    }
}
declare namespace Serenity {
}
