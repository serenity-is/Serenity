namespace Serenity {

    export namespace EditorTypeRegistry {
        let knownTypes: Q.Dictionary<Function>;

        export function get(key: string): Function {

            if (Q.isEmptyOrNull(key)) {
                throw new (ss as any).ArgumentNullException('key');
            }

            initialize();
            var editorType = knownTypes[key.toLowerCase()];
            if (editorType == null) {
                throw new ss.Exception(Q.format("Can't find {0} editor type!", key));
            }

            return editorType;

        }

        function initialize(): void {

            if (knownTypes != null)
                return;

            knownTypes = {};

            var assemblies = (ss as any).getAssemblies();
            for (var assembly of assemblies) {
                for (var type of (ss as any).getAssemblyTypes(assembly)) {

                    if (!(type.prototype instanceof Serenity.Widget)) {
                        continue;
                    }

                    if ((ss as any).isGenericTypeDefinition(type)) {
                        continue;
                    }

                    var fullName = (ss as any).getTypeFullName(type).toLowerCase();
                    knownTypes[fullName] = type;

                    var editorAttr = (ss as any).getAttributes(type, Serenity.EditorAttribute, false);
                    if (editorAttr != null && editorAttr.length > 0) {
                        var attrKey = editorAttr[0].key;
                        if (!Q.isEmptyOrNull(attrKey)) {
                            knownTypes[attrKey.toLowerCase()] = type;
                        }
                    }

                    for (var k of Q.Config.rootNamespaces) {
                        if (Q.startsWith(fullName, k.toLowerCase() + '.')) {
                            var kx = fullName.substr(k.length + 1).toLowerCase();
                            if (knownTypes[kx] == null) {
                                knownTypes[kx] = type;
                            }
                        }
                    }
                }
            }

            setTypeKeysWithoutEditorSuffix();
        }

        export function reset(): void {
            knownTypes = null;
        }

        function setTypeKeysWithoutEditorSuffix() {
            var suffix = 'editor';
            var keys = Object.keys(knownTypes);
            for (var k of keys) {

                if (!Q.endsWith(k, suffix))
                    continue;

                var p = k.substr(0, k.length - suffix.length);

                if (Q.isEmptyOrNull(p))
                    continue;

                if (knownTypes[p] != null)
                    continue;

                knownTypes[p] = knownTypes[k];
            }
        }
    }

    export namespace EditorUtils {

        export function getDisplayText(editor: Serenity.Widget<any>): string {

            var select2 = editor.element.data('select2');

            if (select2 != null) {
                var data = editor.element.select2('data');
                if (data == null)
                    return '';

                return Q.coalesce(data.text, '');
            }

            var value = getValue(editor);
            if (value == null) {
                return '';
            }

            if (typeof value === "string")
                return value;

            if (value instanceof Boolean)
                return (!!value ? Q.coalesce(Q.tryGetText('Controls.FilterPanel.OperatorNames.true'), 'True') :
                    Q.coalesce(Q.tryGetText('Controls.FilterPanel.OperatorNames.true'), 'False'));

            return value.toString();
        }

        var dummy: PropertyItem = { name: '_' };

        export function getValue(editor: Serenity.Widget<any>): any {
            var target = {};
            saveValue(editor, dummy, target);
            return target['_'];
        }

        export function saveValue(editor: Serenity.Widget<any>, item: PropertyItem, target: any): void {

            var getEditValue = (ss as any).safeCast(editor, Serenity.IGetEditValue);

            if (getEditValue != null) {
                getEditValue.getEditValue(item, target);
                return;
            }

            var stringValue = (ss as any).safeCast(editor, Serenity.IStringValue);
            if (stringValue != null) {
                target[item.name] = stringValue.get_value();
                return;
            }

            var booleanValue = (ss as any).safeCast(editor, Serenity.IBooleanValue);
            if (booleanValue != null) {
                target[item.name] = booleanValue.get_value();
                return;
            }

            var doubleValue = (ss as any).safeCast(editor, Serenity.IDoubleValue);
            if (doubleValue != null) {
                var value = doubleValue.get_value();
                target[item.name] = (isNaN(value) ? null : value);
                return;
            }

            if ((editor as any).getEditValue != null) {
                (editor as any).getEditValue(item, target);
                return;
            }

            if (editor.element.is(':input')) {
                target[item.name] = editor.element.val();
                return;
            }
        }

        export function setValue(editor: Serenity.Widget<any>, value: any): void {
            var source = { _: value };
            loadValue(editor, dummy, source);
        }

        export function loadValue(editor: Serenity.Widget<any>, item: PropertyItem, source: any): void {

            var setEditValue = (ss as any).safeCast(editor, Serenity.ISetEditValue);
            if (setEditValue != null) {
                setEditValue.setEditValue(source, item);
                return;
            }

            var stringValue = (ss as any).safeCast(editor, Serenity.IStringValue);
            if (stringValue != null) {
                var value = source[item.name];
                if (value != null) {
                    value = value.toString();
                }
                stringValue.set_value((ss as any).cast(value, String));
                return;
            }

            var booleanValue = (ss as any).safeCast(editor, Serenity.IBooleanValue);
            if (booleanValue != null) {
                var value1 = source[item.name];
                if (typeof (value1) === 'number') {
                    booleanValue.set_value(value1 > 0);
                }
                else {
                    booleanValue.set_value(!!value1);
                }
                return;
            }

            var doubleValue = (ss as any).safeCast(editor, Serenity.IDoubleValue);
            if (doubleValue != null) {
                var d = source[item.name];
                if (!!(d == null || (ss as any).isInstanceOfType(d, String) && Q.isTrimmedEmpty((ss as any).cast(d, String)))) {
                    doubleValue.set_value(null);
                }
                else if ((ss as any).isInstanceOfType(d, String)) {
                    doubleValue.set_value((ss as any).cast(Q.parseDecimal((ss as any).cast(d, String)), Number));
                }
                else if ((ss as any).isInstanceOfType(d, Boolean)) {
                    doubleValue.set_value((!!d ? 1 : 0));
                }
                else {
                    doubleValue.set_value((ss as any).cast(d, Number));
                }
                return;
            }

            if ((editor as any).setEditValue != null) {
                (editor as any).setEditValue(source, item);
                return;
            }

            if (editor.element.is(':input')) {
                var v = source[item.name];
                if (v == null) {
                    editor.element.val('');
                }
                else {
                    editor.element.val(v);
                }
                return;
            }
        }

        export function setReadonly(elements: JQuery, isReadOnly: boolean): JQuery {
            elements.each(function (index, el) {
                var elx = $(el);
                var type = elx.attr('type');
                if (elx.is('select') || type === 'radio' || type === 'checkbox') {
                    if (isReadOnly) {
                        elx.addClass('readonly').attr('disabled', 'disabled');
                    }
                    else {
                        elx.removeClass('readonly').removeAttr('disabled');
                    }
                }
                else if (isReadOnly) {
                    elx.addClass('readonly').attr('readonly', 'readonly');
                }
                else {
                    elx.removeClass('readonly').removeAttr('readonly');
                }
                return true;
            });
            return elements;
        }

        export function setReadOnly(widget: Serenity.Widget<any>, isReadOnly: boolean): void {

            var readOnly = (ss as any).safeCast(widget, Serenity.IReadOnly);

            if (readOnly != null) {
                readOnly.set_readOnly(isReadOnly);
            }
            else if (widget.element.is(':input')) {
                setReadonly(widget.element, isReadOnly);
            }
        }

        export function setRequired(widget: Serenity.Widget<any>, isRequired: boolean): void {
            var req = (ss as any).safeCast(widget, IValidateRequired);
            if (req != null) {
                req.set_required(isRequired);
            }
            else if (widget.element.is(':input')) {
                widget.element.toggleClass('required', !!isRequired);
            }
            var gridField = WX.getGridField(widget);
            var hasSupItem = gridField.find('sup').get().length > 0;
            if (isRequired && !hasSupItem) {
                $('<sup>*</sup>').attr('title', Q.text('Controls.PropertyGrid.RequiredHint'))
                    .prependTo(gridField.find('.caption')[0]);
            }
            else if (!isRequired && hasSupItem) {
                $(gridField.find('sup')[0]).remove();
            }
        }
    }

    export interface EmailEditorOptions {
        domain?: string;
        readOnlyDomain?: boolean;
    }

    @Decorators.registerEditor('Serenity.EmailEditor', [IStringValue, IReadOnly])
    @Decorators.element('<input type="text"/>')
    export class EmailEditor extends Widget<EmailEditorOptions> {

        constructor(input: JQuery, opt: EmailEditorOptions) {
            super(input, opt);

            EmailEditor.registerValidationMethods();

            input.addClass('emailuser').removeClass('flexify');

            var spanAt = $('<span/>').text('@').addClass('emailat').insertAfter(input);

            var domain = $('<input type="text"/>').addClass('emaildomain').addClass('flexify').insertAfter(spanAt);
            domain.bind('blur.' + this.uniqueName, function () {
                var validator = domain.closest('form').data('validator');
                if (validator != null) {
                    validator.element(input[0]);
                }
            });

            if (!Q.isEmptyOrNull(this.options.domain)) {
                domain.val(this.options.domain);
            }

            if (this.options.readOnlyDomain) {
                domain.attr('readonly', 'readonly').addClass('disabled').attr('tabindex', '-1');
            }

            input.bind('keypress.' + this.uniqueName, e => {
                if (e.which === 64) {
                    e.preventDefault();
                    if (!this.options.readOnlyDomain) {
                        domain.focus();
                        domain.select();
                    }
                }
            });

            domain.bind('keypress.' + this.uniqueName, function (e1) {
                if (e1.which === 64) {
                    e1.preventDefault();
                }
            });

            if (!this.options.readOnlyDomain) {
                input.change(e2 => this.set_value(input.val()));
            }
        }

        static registerValidationMethods(): void {
            if ($.validator.methods['emailuser'] != null)
                return;

            $.validator.addMethod('emailuser', function (value, element) {

                var domain = $(element).nextAll('.emaildomain');
                if (domain.length > 0 && domain.attr('readonly') == null) {

                    if (this.optional(element) && this.optional(domain[0])) {
                        return true;
                    }

                    value = value + '@' + domain.val();

                    if (Q.Config.emailAllowOnlyAscii) {
                        return (new RegExp("^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}" +
                            "[a-zA-Z0-9])?(?:\\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$"))
                            .test(value);
                    }

                    return (new RegExp("^((([a-z]|\\d|[!#\\$%&'\\*\\+\\-\\/=\\?\\^_`{\\|}~]|" +
                        "[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])+(\\.([a-z]|\\d|" +
                        "[!#\\$%&'\\*\\+\\-\\/=\\?\\^_`{\\|}~]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])+)*)|" +
                        "((\\x22)((((\\x20|\\x09)*(\\x0d\\x0a))?(\\x20|\\x09)+)?(([\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x7f]|" +
                        "\\x21|[\\x23-\\x5b]|[\\x5d-\\x7e]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(\\\\([\\x01-\\x09\\x0b\\x0c\\x0d-\\x7f]|" +
                        "[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF]))))*(((\\x20|\\x09)*(\\x0d\\x0a))?(\\x20|\\x09)+)?(\\x22)))@((([a-z]|\\d|" +
                        "[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(([a-z]|\\d|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])" +
                        "([a-z]|\\d|-|\\.|_|~|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])*([a-z]|\\d|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])))\\.)" +
                        "+(([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])([a-z]|\\d|-|\\.|_|~|" +
                        "[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])*([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])))$", 'i')).test(value);
                }
                else {

                    if (Q.Config.emailAllowOnlyAscii) {
                        return this.optional(element) || (new RegExp("^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+$")).test(value);
                    }

                    return this.optional(element) || (new RegExp("^((([a-z]|\\d|[!#\\$%&'\\*\\+\\-\\/=\\?\\^_`{\\|}~]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])+" +
                        "(\\.([a-z]|\\d|[!#\\$%&'\\*\\+\\-\\/=\\?\\^_`{\\|}~]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])+)*)|((\\x22)((((\\x20|\\x09)*" +
                        "(\\x0d\\x0a))?(\\x20|\\x09)+)?(([\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x7f]|\\x21|[\\x23-\\x5b]|[\\x5d-\\x7e]|[\\u00A0-\\uD7FF\\uF900-" +
                        "\\uFDCF\\uFDF0-\\uFFEF])|(\\\\([\\x01-\\x09\\x0b\\x0c\\x0d-\\x7f]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF]))))*(((\\x20|\\x09)*" +
                        "(\\x0d\\x0a))?(\\x20|\\x09)+)?(\\x22)))$", 'i')).test(value);
                }
            }, Q.text("Validation.Email"));
        }

        get_value(): string {
            var domain = this.element.nextAll('.emaildomain');
            var value = this.element.val();
            var domainValue = domain.val();
            if(Q.isEmptyOrNull(value)) {
                if (this.options.readOnlyDomain || Q.isEmptyOrNull(domainValue)) {
                    return '';
                }
                return '@' + domainValue;
            }
			return value + '@' + domainValue;
        }

        get value(): string {
            return this.get_value();
        }

        set_value(value: string): void {
            var domain = this.element.nextAll('.emaildomain');
            value = Q.trimToNull(value);
            if (value == null) {
                if (!this.options.readOnlyDomain) {
                    domain.val('');
                }
                this.element.val('');
            }
			else {
                var parts = value.split('@');
                if(parts.length > 1) {
                    if (!this.options.readOnlyDomain) {
                        domain.val(parts[1]);
                        this.element.val(parts[0]);
                    }
                    else if (!Q.isEmptyOrNull(this.options.domain)) {
                        if (parts[1] !== this.options.domain) {
                            this.element.val(value);
                        }
                        else {
                            this.element.val(parts[0]);
                        }
                    }
                    else {
                        this.element.val(parts[0]);
                    }
                }
				else {
                    this.element.val(parts[0]);
                }
            }
        }

        get_readOnly(): boolean {
            var domain = this.element.nextAll('.emaildomain');
            return !(this.element.attr('readonly') == null &&
                (!this.options.readOnlyDomain || domain.attr('readonly') == null));
        }

        set_readOnly(value: boolean): void {
            var domain = this.element.nextAll('.emaildomain');
            if(value) {
                this.element.attr('readonly', 'readonly').addClass('readonly');
                if (!this.options.readOnlyDomain) {
                    domain.attr('readonly', 'readonly').addClass('readonly');
                }
            }
			else {
                this.element.removeAttr('readonly').removeClass('readonly');
                if(!this.options.readOnlyDomain) {
                    domain.removeAttr('readonly').removeClass('readonly');
                }
            }
        }
    }

    export interface EnumEditorOptions {
        enumKey?: string;
        enumType?: any;
        allowClear?: boolean;
    }

    @Decorators.registerEditor('Serenity.EnumEditor')
    export class EnumEditor extends Select2Editor<EnumEditorOptions, Select2Item> {
        constructor(hidden: JQuery, opt: EnumEditorOptions) {
            super(hidden, opt);
            this.updateItems();
        }

        protected updateItems(): void {
            this.clearItems();

            var enumType = this.options.enumType || Serenity.EnumTypeRegistry.get(this.options.enumKey);
            var enumKey = this.options.enumKey;

            if (enumKey == null && enumType != null) {
                var enumKeyAttr = (ss as any).getAttributes(enumType, Serenity.EnumKeyAttribute, false);
                if (enumKeyAttr.length > 0) {
                    enumKey = enumKeyAttr[0].value;
                }
            }

            var values = (ss as any).Enum.getValues(enumType);
            for (var x of values) {
                var name = (ss as any).Enum.toString(enumType, x);
                this.addOption((ss as any).cast(x, (ss as any).Int32).toString(),
                    Q.coalesce(Q.tryGetText('Enums.' + enumKey + '.' + name), name), null, false);
            }
        }

        protected getSelect2Options(): Select2Options {
            var opt = super.getSelect2Options();
            opt.allowClear = Q.coalesce(this.options.allowClear, true);
            return opt;
        }
    }
}

declare namespace Serenity {

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

    interface RadioButtonEditorOptions {
        enumKey?: string;
        enumType?: any;
        lookupKey?: string;
    }

    class RadioButtonEditor extends Widget<RadioButtonEditorOptions> {
        constructor(input: JQuery, opt: RadioButtonEditorOptions);
        value: string;
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