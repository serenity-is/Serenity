declare namespace Serenity {

    namespace ReflectionOptionsSetter {
        function set(target: any, options: any): void;
    }

    namespace ReflectionUtils {
        function getPropertyValue(o: any, property: string): any;
        function setPropertyValue(o: any, property: string, value: any): void;
        function makeCamelCase(s: string): string;
    }

    const enum RetrieveColumnSelection {
        details = 0,
        keyOnly = 1,
        list = 2
    }

    class ScriptContext {
    }

    class PrefixedContext extends ScriptContext {
        constructor(prefix: string);
        w(id: string, type: Function): any;
    }

    class PasswordEditor extends StringEditor {
        constructor(input: JQuery);
    }

    class PersonNameEditor extends Widget<any> {
        constructor(input: JQuery);
        get_value(): string;
        set_value(value: string): void;
    }

    interface ToolbarOptions {
        buttons?: ToolButton[];
        hotkeyContext?: any;
    }

    class Toolbar extends Widget<ToolbarOptions> {
        constructor(div: JQuery, options: ToolbarOptions);
        findButton(className: string): JQuery;
    }

    class CheckListEditor extends Widget<CheckListEditorOptions> {
        constructor(div: JQuery, opt: CheckListEditorOptions);
        getItems(): CheckListItem[];
        updateItems(): void;
    }

    interface CheckListEditorOptions {
        items?: CheckListItem[];
        selectAllOptionText?: string;
    }

    class CheckListItem {
        id: string;
        text: string;
        parentId: string;
    }

    interface CheckTreeItem<TSource> {
        isSelected?: boolean;
        hideCheckBox?: boolean;
        isAllDescendantsSelected?: boolean;
        id?: string;
        text?: string;
        parentId?: string;
        children?: CheckTreeItem<TSource>[];
        source?: TSource;
    }

    const enum ColumnSelection {
        List = 0,
        KeyOnly = 1,
        Details = 2
    }

    class IntegerEditor extends Widget<IntegerEditorOptions> {
        constructor(input: JQuery, opt?: IntegerEditorOptions);
        value: number;
    }

    interface IntegerEditorOptions {
        minValue?: number;
        maxValue?: number;
    }

    interface HtmlContentEditorOptions {
    }

    interface GridPersistanceFlags {
        columnWidths?: boolean;
        columnVisibility?: boolean;
        sortColumns?: boolean;
        filterItems?: boolean;
        quickFilters?: boolean;
        quickFilterText?: boolean;
        quickSearch?: boolean;
        includeDeleted?: boolean;
    }

    interface PersistedGridColumn {
        id: string;
        width?: number;
        sort?: number;
        visible?: boolean;
    }

    interface PersistedGridSettings {
        columns?: PersistedGridColumn[];
        filterItems?: FilterLine[];
        quickFilters?: Q.Dictionary<any>;
        quickFilterText?: string;
        quickSearchField?: QuickSearchField;
        quickSearchText?: string;
        includeDeleted?: boolean;
    }

    interface SettingStorage {
        getItem(key: string): string;
        setItem(key: string, value: string): void;
    }

    interface CKEditorConfig {
    }

    enum CaptureOperationType {
        Before = 0,
        Delete = 1,
        Insert = 2,
        Update = 3
    }

    namespace CustomValidation {
        function registerValidationMethods(): void;
    }

    interface DialogButton {
        text: string;
        click: () => void;
    }

    interface EditorTypeInfo {
        type?: Function;
        displayName?: string;
        optionsType?: Function;
    }
}