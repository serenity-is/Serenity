/// <reference types="jquery" />
declare namespace Serenity {
    class StringEditor extends Widget<any> {
        constructor(input: JQuery);
        get value(): string;
        protected get_value(): string;
        set value(value: string);
        protected set_value(value: string): void;
    }
}
declare namespace Serenity {
    class PasswordEditor extends StringEditor {
        constructor(input: JQuery);
    }
}
declare namespace Serenity {
    interface TextAreaEditorOptions {
        cols?: number;
        rows?: number;
    }
    class TextAreaEditor extends Widget<TextAreaEditorOptions> {
        constructor(input: JQuery, opt?: TextAreaEditorOptions);
        get value(): string;
        protected get_value(): string;
        set value(value: string);
        protected set_value(value: string): void;
    }
}
declare namespace Serenity {
    class BooleanEditor extends Widget<any> {
        get value(): boolean;
        protected get_value(): boolean;
        set value(value: boolean);
        protected set_value(value: boolean): void;
    }
}
declare namespace Serenity {
    interface DecimalEditorOptions {
        minValue?: string;
        maxValue?: string;
        decimals?: any;
        padDecimals?: any;
        allowNegatives?: boolean;
    }
    class DecimalEditor extends Widget<DecimalEditorOptions> implements IDoubleValue {
        constructor(input: JQuery, opt?: DecimalEditorOptions);
        get_value(): number;
        get value(): number;
        set_value(value: number): void;
        set value(v: number);
        get_isValid(): boolean;
        static defaultAutoNumericOptions(): any;
    }
}
declare namespace Serenity {
    interface IntegerEditorOptions {
        minValue?: number;
        maxValue?: number;
        allowNegatives?: boolean;
    }
    class IntegerEditor extends Widget<IntegerEditorOptions> implements IDoubleValue {
        constructor(input: JQuery, opt?: IntegerEditorOptions);
        get_value(): number;
        get value(): number;
        set_value(value: number): void;
        set value(v: number);
        get_isValid(): boolean;
    }
}
declare namespace Serenity {
    let datePickerIconSvg: string;
    class DateEditor extends Widget<any> implements IStringValue, IReadOnly {
        private minValue;
        private maxValue;
        private minDate;
        private maxDate;
        private sqlMinMax;
        constructor(input: JQuery);
        static useFlatpickr: boolean;
        static flatPickrOptions(input: JQuery): {
            clickOpens: boolean;
            allowInput: boolean;
            dateFormat: string;
            onChange: () => void;
        };
        get_value(): string;
        get value(): string;
        set_value(value: string): void;
        set value(v: string);
        private get_valueAsDate;
        get valueAsDate(): Date;
        private set_valueAsDate;
        set valueAsDate(v: Date);
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        yearRange: string;
        get_minValue(): string;
        set_minValue(value: string): void;
        get_maxValue(): string;
        set_maxValue(value: string): void;
        get_minDate(): Date;
        set_minDate(value: Date): void;
        get_maxDate(): Date;
        set_maxDate(value: Date): void;
        get_sqlMinMax(): boolean;
        set_sqlMinMax(value: boolean): void;
        static dateInputChange: (e: JQueryEventObject) => void;
        static flatPickrTrigger(input: JQuery): JQuery;
        static dateInputKeyup(e: JQueryEventObject): void;
    }
}
declare namespace Serenity {
    class DateTimeEditor extends Widget<DateTimeEditorOptions> implements IStringValue, IReadOnly {
        private minValue;
        private maxValue;
        private minDate;
        private maxDate;
        private sqlMinMax;
        private time;
        private lastSetValue;
        private lastSetValueGet;
        constructor(input: JQuery, opt?: DateTimeEditorOptions);
        getFlatpickrOptions(): any;
        get_value(): string;
        get value(): string;
        set_value(value: string): void;
        private getInplaceNowText;
        private getDisplayFormat;
        set value(v: string);
        private get_valueAsDate;
        get valueAsDate(): Date;
        private set_valueAsDate;
        set valueAsDate(value: Date);
        get_minValue(): string;
        set_minValue(value: string): void;
        get_maxValue(): string;
        set_maxValue(value: string): void;
        get_minDate(): Date;
        set_minDate(value: Date): void;
        get_maxDate(): Date;
        set_maxDate(value: Date): void;
        get_sqlMinMax(): boolean;
        set_sqlMinMax(value: boolean): void;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        static roundToMinutes(date: Date, minutesStep: number): Date;
        static getTimeOptions: (fromHour: number, fromMin: number, toHour: number, toMin: number, stepMins: number) => string[];
    }
    interface DateTimeEditorOptions {
        startHour?: any;
        endHour?: any;
        intervalMinutes?: any;
        yearRange?: string;
        useUtc?: boolean;
        seconds?: boolean;
        inputOnly?: boolean;
    }
}
declare namespace Serenity {
    interface TimeEditorOptions {
        noEmptyOption?: boolean;
        startHour?: any;
        endHour?: any;
        intervalMinutes?: any;
    }
    class TimeEditor extends Widget<TimeEditorOptions> {
        private minutes;
        constructor(input: JQuery, opt?: TimeEditorOptions);
        get value(): number;
        protected get_value(): number;
        set value(value: number);
        protected set_value(value: number): void;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
    }
}
declare namespace Serenity {
    class EmailAddressEditor extends Serenity.StringEditor {
        constructor(input: JQuery);
    }
}
declare namespace Serenity {
    interface EmailEditorOptions {
        domain?: string;
        readOnlyDomain?: boolean;
    }
    class EmailEditor extends Widget<EmailEditorOptions> {
        constructor(input: JQuery, opt: EmailEditorOptions);
        static registerValidationMethods(): void;
        get_value(): string;
        get value(): string;
        set_value(value: string): void;
        set value(v: string);
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
    }
}
declare namespace Serenity {
    class URLEditor extends StringEditor {
        constructor(input: JQuery);
    }
}
declare namespace Serenity {
    interface RadioButtonEditorOptions {
        enumKey?: string;
        enumType?: any;
        lookupKey?: string;
    }
    class RadioButtonEditor extends Widget<RadioButtonEditorOptions> implements IReadOnly {
        constructor(input: JQuery, opt: RadioButtonEditorOptions);
        protected addRadio(value: string, text: string): void;
        get_value(): string;
        get value(): string;
        set_value(value: string): void;
        set value(v: string);
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
    }
}
declare namespace Serenity {
    interface Select2CommonOptions {
        allowClear?: boolean;
        delimited?: boolean;
        minimumResultsForSearch?: any;
        multiple?: boolean;
    }
    interface Select2FilterOptions {
        cascadeFrom?: string;
        cascadeField?: string;
        cascadeValue?: any;
        filterField?: string;
        filterValue?: any;
    }
    interface Select2InplaceAddOptions {
        inplaceAdd?: boolean;
        inplaceAddPermission?: string;
        dialogType?: string;
        autoComplete?: boolean;
    }
    interface Select2EditorOptions extends Select2FilterOptions, Select2InplaceAddOptions, Select2CommonOptions {
    }
    interface Select2SearchPromise {
        abort?(): void;
        catch?(callback: () => void): void;
        fail?(callback: () => void): void;
    }
    interface Select2SearchQuery {
        searchTerm?: string;
        idList?: string[];
        skip?: number;
        take?: number;
        checkMore?: boolean;
    }
    interface Select2SearchResult<TItem> {
        items: TItem[];
        more: boolean;
    }
    class Select2Editor<TOptions, TItem> extends Widget<TOptions> implements Serenity.ISetEditValue, Serenity.IGetEditValue, Serenity.IStringValue, Serenity.IReadOnly {
        private _items;
        private _itemById;
        protected lastCreateTerm: string;
        constructor(hidden: JQuery, opt?: any);
        destroy(): void;
        protected hasAsyncSource(): boolean;
        protected asyncSearch(query: Select2SearchQuery, results: (result: Select2SearchResult<TItem>) => void): Select2SearchPromise;
        protected getTypeDelay(): any;
        protected emptyItemText(): any;
        protected getPageSize(): number;
        protected getIdField(): any;
        protected itemId(item: TItem): string;
        protected getTextField(): any;
        protected itemText(item: TItem): string;
        protected itemDisabled(item: TItem): boolean;
        protected mapItem(item: TItem): Select2Item;
        protected mapItems(items: TItem[]): Select2Item[];
        protected allowClear(): boolean;
        protected isMultiple(): boolean;
        protected getSelect2Options(): Select2Options;
        get_delimited(): boolean;
        get items(): Select2Item[];
        set items(value: Select2Item[]);
        protected get itemById(): Q.Dictionary<Select2Item>;
        protected set itemById(value: Q.Dictionary<Select2Item>);
        clearItems(): void;
        addItem(item: Select2Item): void;
        addOption(key: string, text: string, source?: any, disabled?: boolean): void;
        protected addInplaceCreate(addTitle: string, editTitle: string): void;
        protected useInplaceAdd(): boolean;
        protected isAutoComplete(): boolean;
        getCreateSearchChoice(getName: (z: any) => string): (s: string) => {
            id: string;
            text: string;
        };
        setEditValue(source: any, property: PropertyItem): void;
        getEditValue(property: PropertyItem, target: any): void;
        protected get_select2Container(): JQuery;
        protected get_items(): Select2Item[];
        protected get_itemByKey(): Q.Dictionary<Select2Item>;
        static filterByText<TItem>(items: TItem[], getText: (item: TItem) => string, term: string): TItem[];
        get_value(): any;
        get value(): string;
        set_value(value: string): void;
        set value(v: string);
        get selectedItem(): TItem;
        get selectedItems(): TItem[];
        protected get_values(): string[];
        get values(): string[];
        protected set_values(value: string[]): void;
        set values(value: string[]);
        protected get_text(): string;
        get text(): string;
        get_readOnly(): boolean;
        get readOnly(): boolean;
        private updateInplaceReadOnly;
        set_readOnly(value: boolean): void;
        set readOnly(value: boolean);
        protected getCascadeFromValue(parent: Serenity.Widget<any>): any;
        protected cascadeLink: Serenity.CascadedWidgetLink<Widget<any>>;
        protected setCascadeFrom(value: string): void;
        protected get_cascadeFrom(): string;
        get cascadeFrom(): string;
        protected set_cascadeFrom(value: string): void;
        set cascadeFrom(value: string);
        protected get_cascadeField(): any;
        get cascadeField(): string;
        protected set_cascadeField(value: string): void;
        set cascadeField(value: string);
        protected get_cascadeValue(): any;
        get cascadeValue(): any;
        protected set_cascadeValue(value: any): void;
        set cascadeValue(value: any);
        protected get_filterField(): string;
        get filterField(): string;
        protected set_filterField(value: string): void;
        set filterField(value: string);
        protected get_filterValue(): any;
        get filterValue(): any;
        protected set_filterValue(value: any): void;
        set filterValue(value: any);
        protected cascadeItems(items: TItem[]): TItem[];
        protected filterItems(items: TItem[]): TItem[];
        protected updateItems(): void;
        protected getDialogTypeKey(): string;
        protected createEditDialog(callback: (dlg: IEditDialog) => void): void;
        onInitNewEntity: (entity: TItem) => void;
        protected initNewEntity(entity: TItem): void;
        protected setEditDialogReadOnly(dialog: any): void;
        protected editDialogDataChange(): void;
        protected setTermOnNewEntity(entity: TItem, term: string): void;
        protected inplaceCreateClick(e: JQueryEventObject): void;
        openDialogAsPanel: boolean;
    }
}
declare namespace Serenity {
    class SelectEditor extends Select2Editor<SelectEditorOptions, Select2Item> {
        constructor(hidden: JQuery, opt?: SelectEditorOptions);
        getItems(): any[];
        protected emptyItemText(): any;
        updateItems(): void;
    }
    interface SelectEditorOptions extends Select2CommonOptions {
        items?: any[];
        emptyOptionText?: string;
    }
}
declare namespace Serenity {
    class DateYearEditor extends SelectEditor {
        constructor(hidden: JQuery, opt: DateYearEditorOptions);
        getItems(): any[];
    }
    interface DateYearEditorOptions extends SelectEditorOptions {
        minYear?: string;
        maxYear?: string;
        descending?: boolean;
    }
}
declare namespace Serenity {
    interface EnumEditorOptions extends Select2CommonOptions {
        enumKey?: string;
        enumType?: any;
    }
    class EnumEditor extends Select2Editor<EnumEditorOptions, Select2Item> {
        constructor(hidden: JQuery, opt: EnumEditorOptions);
        protected updateItems(): void;
        protected allowClear(): any;
    }
}
declare namespace Serenity {
    interface LookupEditorOptions extends Select2EditorOptions {
        lookupKey?: string;
        async?: boolean;
    }
    class LookupEditorBase<TOptions extends LookupEditorOptions, TItem> extends Select2Editor<TOptions, TItem> {
        constructor(input: JQuery, opt?: TOptions);
        hasAsyncSource(): boolean;
        destroy(): void;
        protected getLookupKey(): string;
        protected lookup: Q.Lookup<TItem>;
        protected getLookupAsync(): PromiseLike<Q.Lookup<TItem>>;
        protected getLookup(): Q.Lookup<TItem>;
        protected getItems(lookup: Q.Lookup<TItem>): TItem[];
        protected getIdField(): any;
        protected getItemText(item: TItem, lookup: Q.Lookup<TItem>): any;
        protected mapItem(item: TItem): Select2Item;
        protected getItemDisabled(item: TItem, lookup: Q.Lookup<TItem>): boolean;
        updateItems(): void;
        protected asyncSearch(query: Select2SearchQuery, results: (result: Select2SearchResult<TItem>) => void): Select2SearchPromise;
        protected getDialogTypeKey(): string;
        protected setCreateTermOnNewEntity(entity: TItem, term: string): void;
        protected editDialogDataChange(): void;
    }
    class LookupEditor extends LookupEditorBase<LookupEditorOptions, any> {
        constructor(hidden: JQuery, opt?: LookupEditorOptions);
    }
}
declare namespace Serenity {
    interface ServiceLookupEditorOptions extends Select2EditorOptions {
        service?: string;
        idField: string;
        textField: string;
        pageSize?: number;
        minimumResultsForSearch?: any;
        sort: string[];
        columnSelection?: Serenity.ColumnSelection;
        includeColumns?: string[];
        excludeColumns?: string[];
        includeDeleted?: boolean;
        containsField?: string;
        equalityFilter?: any;
        criteria?: any[];
    }
    class ServiceLookupEditorBase<TOptions extends ServiceLookupEditorOptions, TItem> extends Select2Editor<TOptions, TItem> {
        constructor(input: JQuery, opt?: TOptions);
        protected getDialogTypeKey(): string;
        protected getService(): string;
        protected getServiceUrl(): string;
        protected getIncludeColumns(): string[];
        protected getSort(): any[];
        protected getCascadeCriteria(): any[];
        protected getFilterCriteria(): any[];
        protected getIdListCriteria(idList: any[]): any[];
        protected getCriteria(query: Select2SearchQuery): any[];
        protected getListRequest(query: Select2SearchQuery): ListRequest;
        protected getServiceCallOptions(query: Select2SearchQuery, results: (result: Select2SearchResult<TItem>) => void): ServiceOptions<ListResponse<TItem>>;
        protected hasAsyncSource(): boolean;
        protected asyncSearch(query: Select2SearchQuery, results: (result: Select2SearchResult<TItem>) => void): Select2SearchPromise;
    }
    class ServiceLookupEditor extends ServiceLookupEditorBase<ServiceLookupEditorOptions, any> {
        constructor(hidden: JQuery, opt?: ServiceLookupEditorOptions);
    }
}
declare namespace Serenity {
    interface HtmlContentEditorOptions {
        cols?: any;
        rows?: any;
    }
    interface CKEditorConfig {
    }
    class HtmlContentEditor extends Widget<HtmlContentEditorOptions> implements IStringValue, IReadOnly {
        private _instanceReady;
        constructor(textArea: JQuery, opt?: HtmlContentEditorOptions);
        protected instanceReady(x: any): void;
        protected getLanguage(): string;
        protected getConfig(): CKEditorConfig;
        protected getEditorInstance(): any;
        destroy(): void;
        get_value(): string;
        get value(): string;
        set_value(value: string): void;
        set value(v: string);
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        static CKEditorVer: string;
        static includeCKEditor(): void;
    }
    class HtmlNoteContentEditor extends HtmlContentEditor {
        constructor(textArea: JQuery, opt?: HtmlContentEditorOptions);
        protected getConfig(): CKEditorConfig;
    }
    class HtmlReportContentEditor extends HtmlContentEditor {
        constructor(textArea: JQuery, opt?: HtmlContentEditorOptions);
        protected getConfig(): CKEditorConfig;
    }
}
declare namespace Serenity {
    class MaskedEditor extends Widget<MaskedEditorOptions> {
        constructor(input: JQuery, opt?: MaskedEditorOptions);
        get value(): string;
        protected get_value(): string;
        set value(value: string);
        protected set_value(value: string): void;
    }
    interface MaskedEditorOptions {
        mask?: string;
        placeholder?: string;
    }
}
declare namespace Serenity {
    interface RecaptchaOptions {
        siteKey?: string;
        language?: string;
    }
    class Recaptcha extends Widget<RecaptchaOptions> implements IStringValue {
        constructor(div: JQuery, opt: RecaptchaOptions);
        get_value(): string;
        set_value(value: string): void;
    }
}
declare namespace Serenity {
    interface FileUploadEditorOptions extends FileUploadConstraints {
        displayFileName?: boolean;
        urlPrefix?: string;
    }
    interface ImageUploadEditorOptions extends FileUploadEditorOptions {
    }
    class FileUploadEditor extends Widget<FileUploadEditorOptions> implements IReadOnly, IGetEditValue, ISetEditValue, IValidateRequired {
        constructor(div: JQuery, opt: FileUploadEditorOptions);
        protected getUploadInputOptions(): UploadInputOptions;
        protected addFileButtonText(): string;
        protected getToolButtons(): ToolButton[];
        protected populate(): void;
        protected updateInterface(): void;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        get_required(): boolean;
        set_required(value: boolean): void;
        get_value(): UploadedFile;
        get value(): UploadedFile;
        set_value(value: UploadedFile): void;
        set value(v: UploadedFile);
        getEditValue(property: PropertyItem, target: any): void;
        setEditValue(source: any, property: PropertyItem): void;
        protected entity: UploadedFile;
        protected toolbar: Toolbar;
        protected progress: JQuery;
        protected fileSymbols: JQuery;
        protected uploadInput: JQuery;
    }
    class ImageUploadEditor extends FileUploadEditor {
        constructor(div: JQuery, opt: ImageUploadEditorOptions);
    }
    class MultipleFileUploadEditor extends Widget<FileUploadEditorOptions> implements IReadOnly, IGetEditValue, ISetEditValue {
        private entities;
        private toolbar;
        private fileSymbols;
        private uploadInput;
        constructor(div: JQuery, opt: ImageUploadEditorOptions);
        protected addFileButtonText(): string;
        protected getToolButtons(): ToolButton[];
        protected populate(): void;
        protected updateInterface(): void;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        get_value(): UploadedFile[];
        get value(): UploadedFile[];
        set_value(value: UploadedFile[]): void;
        set value(v: UploadedFile[]);
        getEditValue(property: PropertyItem, target: any): void;
        setEditValue(source: any, property: PropertyItem): void;
        jsonEncodeValue: boolean;
    }
    class MultipleImageUploadEditor extends MultipleFileUploadEditor {
        constructor(div: JQuery, opt: ImageUploadEditorOptions);
    }
}
