declare namespace Serenity {

    namespace EditorUtils {
        function getValue(editor: Serenity.Widget<any>): any;
        function saveValue(editor: Serenity.Widget<any>, item: PropertyItem, target: any): void;
        function setValue(editor: Serenity.Widget<any>, value: any): void;
        function loadValue(editor: Serenity.Widget<any>, item: PropertyItem, source: any): void;
        function setReadonly(elements: JQuery, isReadOnly: boolean): JQuery;
        function setReadOnly(widget: Serenity.Widget<any>, isReadOnly: boolean): void;
        function setRequired(widget: Serenity.Widget<any>, isRequired: boolean): void;
    }

    class PublicEditorTypes {
        static get_registeredTypes(): any;
    }

    class GoogleMap extends Widget<GoogleMapOptions> {
        constructor(container: JQuery, opt: GoogleMapOptions);
        get_map(): any;
    }

    interface GoogleMapOptions {
        latitude?: any;
        longitude?: any;
        zoom?: any;
        mapTypeId?: any;
        markerTitle?: string;
        markerLatitude?: any;
        markerLongitude?: any;
    }

    class BooleanEditor extends Widget<any> {
        constructor(input: JQuery);
        value: boolean;
    }

    interface RadioButtonEditorOptions {
        enumKey?: string;
        enumType?: any;
        lookupKey?: string;
    }

    class RadioButtonEditor extends Widget<RadioButtonEditorOptions> {
        constructor(input: JQuery);
        value: string;
    }

    interface EnumEditorOptions {
        enumKey?: string;
        enumType?: any;
        allowClear?: boolean;
    }

    interface HtmlContentEditorOptions {
        cols?: any;
        rows?: any;
    }

    class HtmlContentEditor extends Widget<HtmlContentEditorOptions> {
        constructor(textArea: JQuery, opt?: HtmlContentEditorOptions);
        instanceReady(x: any): void;
        getLanguage(): string;
        getConfig(): CKEditorConfig;
        value: string;
    }

    class HtmlNoteContentEditor extends HtmlContentEditor {
        constructor(textArea: JQuery, opt?: HtmlContentEditorOptions);
    }

    class HtmlReportContentEditor extends HtmlContentEditor {
        constructor(textArea: JQuery, opt?: HtmlContentEditorOptions);
    }

    interface ImageUploadEditorOptions {
        minWidth?: number;
        maxWidth?: number;
        minHeight?: number;
        maxHeight?: number;
        minSize?: number;
        maxSize?: number;
        originalNameProperty?: string;
        urlPrefix?: string;
        allowNonImage?: boolean;
    }

    class ImageUploadEditor extends Widget<ImageUploadEditorOptions> {
        entity: UploadedFile;
        toolbar: Toolbar;
        fileSymbols: JQuery;
        uploadInput: JQuery;
        constructor(div: JQuery, opt: ImageUploadEditorOptions);
        addFileButtonText(): string;
        getToolButtons(): ToolButton[];
        populate(): void;
        updateInterface(): void;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        value: UploadedFile;
    }

    interface MaskedEditorOptions {
        mask?: string;
        placeholder?: string;
    }

    class MaskedEditor extends Widget<MaskedEditorOptions> {
        constructor(input: JQuery, opt: MaskedEditorOptions);
        value: string;
    }

    class MultipleImageUploadEditor extends Widget<ImageUploadEditorOptions> {
        entities: UploadedFile[];
        toolbar: Toolbar;
        fileSymbols: JQuery;
        uploadInput: JQuery;
        constructor(div: JQuery, opt: ImageUploadEditorOptions);
        addFileButtonText(): string;
        getToolButtons(): ToolButton[];
        populate(): void;
        updateInterface(): void;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        value: UploadedFile[];
        get_jsonEncodeValue(): boolean;
        set_jsonEncodeValue(value: boolean): void;
    }

    interface PhoneEditorOptions {
        multiple?: boolean;
        internal?: boolean;
        mobile?: boolean;
        allowExtension?: boolean;
        allowInternational?: boolean;
    }

    class PhoneEditor extends Widget<PhoneEditorOptions> {
        constructor(input: JQuery, opt?: PhoneEditorOptions);
        validate(value: string): string;
        formatValue(): void;
        getFormattedValue(): string;
        value: string;
    }

    class EnumEditor extends Select2Editor<EnumEditorOptions, Select2Item> {
        constructor(hidden: JQuery, opt: EnumEditorOptions);
        updateItems(): void;
    }

    class Select2AjaxEditor<TOptions, TItem> extends Widget<TOptions> {
        pageSize: number;
        constructor(hidden: JQuery, opt: any);
        emptyItemText(): string;
        getService(): string;
        query(request: ListRequest, callback: (p1: ListResponse<any>) => void): void;
        executeQuery(options: ServiceOptions<ListResponse<any>>): void;
        queryByKey(key: string, callback: (p1: any) => void): void;
        executeQueryByKey(options: ServiceOptions<RetrieveResponse<any>>): void;
        getItemKey(item: any): string;
        getItemText(item: any): string;
        getTypeDelay(): number;
        getSelect2Options(): Select2Options;
        addInplaceCreate(title: string): void;
        inplaceCreateClick(e: any): void;
        get_select2Container(): JQuery;
        value: string;
    }

    class URLEditor extends StringEditor {
        constructor(input: JQuery);
    }

    class Recaptcha extends Widget<RecaptchaOptions> {
        constructor(div: JQuery, opt: RecaptchaOptions);
        get_value(): string;
        set_value(value: string): void;
    }

    interface RecaptchaOptions {
        siteKey?: string;
        language?: string;
    }
}