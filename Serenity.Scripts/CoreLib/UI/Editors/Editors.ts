namespace Serenity {

    import Option = Serenity.Decorators.option

    export namespace EditorTypeRegistry {
        let knownTypes: Q.Dictionary<WidgetClass>;

        export function get(key: string): WidgetClass {

            if (Q.isEmptyOrNull(key)) {
                throw new Q.ArgumentNullException('key');
            }

            initialize();

            var editorType = knownTypes[key.toLowerCase()];
            if (editorType == null) {

                var type = Q.getType(key) ?? Q.getType(key, globalObj);
                if (type != null) {
                    knownTypes[key.toLowerCase()] = type as any;
                    return type as any;
                }

                throw new Q.Exception(Q.format("Can't find {0} editor type!", key));
            }
            return editorType;

        }

        function initialize(): void {

            if (knownTypes != null)
                return;

            knownTypes = {};

            for (var type of Q.getTypes()) {

                if (!(type.prototype instanceof Serenity.Widget)) {
                    continue;
                }

                var fullName = Q.getTypeFullName(type).toLowerCase();
                knownTypes[fullName] = type;

                var editorAttr = Q.getAttributes(type, Serenity.EditorAttribute, false);
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

            setTypeKeysWithoutEditorSuffix();
        }

        export function reset(): void {
            knownTypes = null;
        }

        function setTypeKeysWithoutEditorSuffix() {
            var suffix = 'editor';
            var keys = Object.keys(knownTypes);
            for (var k of keys) {
                setWithoutSuffix(k, knownTypes[k]);
            }
        }

        function setWithoutSuffix(key: string, t: Serenity.WidgetClass) {
            var suffix = 'editor';

            if (!Q.endsWith(key, suffix))
                return;

            var p = key.substr(0, key.length - suffix.length);

            if (Q.isEmptyOrNull(p))
                return;

            if (knownTypes[p] != null)
                return;

            knownTypes[p] = knownTypes[key];
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

            var getEditValue = Q.safeCast(editor, Serenity.IGetEditValue);

            if (getEditValue != null) {
                getEditValue.getEditValue(item, target);
                return;
            }

            var stringValue = Q.safeCast(editor, Serenity.IStringValue);
            if (stringValue != null) {
                target[item.name] = stringValue.get_value();
                return;
            }

            var booleanValue = Q.safeCast(editor, Serenity.IBooleanValue);
            if (booleanValue != null) {
                target[item.name] = booleanValue.get_value();
                return;
            }

            var doubleValue = Q.safeCast(editor, Serenity.IDoubleValue);
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

            var setEditValue = Q.safeCast(editor, Serenity.ISetEditValue);
            if (setEditValue != null) {
                setEditValue.setEditValue(source, item);
                return;
            }

            var stringValue = Q.safeCast(editor, Serenity.IStringValue);
            if (stringValue != null) {
                var value = source[item.name];
                if (value != null) {
                    value = value.toString();
                }
                stringValue.set_value(Q.cast(value, String));
                return;
            }

            var booleanValue = Q.safeCast(editor, Serenity.IBooleanValue);
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

            var doubleValue = Q.safeCast(editor, Serenity.IDoubleValue);
            if (doubleValue != null) {
                var d = source[item.name];
                if (!!(d == null || Q.isInstanceOfType(d, String) && Q.isTrimmedEmpty(Q.cast(d, String)))) {
                    doubleValue.set_value(null);
                }
                else if (Q.isInstanceOfType(d, String)) {
                    doubleValue.set_value(Q.cast(Q.parseDecimal(Q.cast(d, String)), Number));
                }
                else if (Q.isInstanceOfType(d, Boolean)) {
                    doubleValue.set_value((!!d ? 1 : 0));
                }
                else {
                    doubleValue.set_value(Q.cast(d, Number));
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

            var readOnly = Q.safeCast(widget, Serenity.IReadOnly);

            if (readOnly != null) {
                readOnly.set_readOnly(isReadOnly);
            }
            else if (widget.element.is(':input')) {
                setReadonly(widget.element, isReadOnly);
            }
        }

        export function setRequired(widget: Serenity.Widget<any>, isRequired: boolean): void {
            var req = Q.safeCast(widget, IValidateRequired);
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

        export function setContainerReadOnly(container: JQuery, readOnly: boolean) {

            if (!readOnly) {

                if (!container.hasClass('readonly-container'))
                    return;

                container.removeClass('readonly-container').find(".editor.container-readonly")
                    .removeClass('container-readonly').each((i, e) => {
                        var w = $(e).tryGetWidget(Serenity.Widget);
                        if (w != null)
                            Serenity.EditorUtils.setReadOnly(w, false);
                        else
                            Serenity.EditorUtils.setReadonly($(e), false);
                    });

                return;
            }

            container.addClass('readonly-container').find(".editor")
                .not('.container-readonly')
                .each((i, e) => {
                    var w = $(e).tryGetWidget(Serenity.Widget);
                    if (w != null) {

                        if (w['get_readOnly']) {
                            if (w['get_readOnly']())
                                return;
                        }
                        else if ($(e).is('[readonly]') || $(e).is('[disabled]') || $(e).is('.readonly') || $(e).is('.disabled'))
                            return;

                        $(e).addClass('container-readonly');
                        Serenity.EditorUtils.setReadOnly(w, true);

                    }
                    else {
                        if ($(e).is('[readonly]') || $(e).is('[disabled]') || $(e).is('.readonly') || $(e).is('.disabled'))
                            return;

                        Serenity.EditorUtils.setReadonly($(e).addClass('container-readonly'), true);
                    }
                });
        }
    }

    function Editor(name: string, intf?: any[]) {
        return Decorators.registerEditor('Serenity.' + name + 'Editor', intf)
    }

    import Element = Decorators.element

    @Editor('Boolean', [IBooleanValue])
    @Element('<input type="checkbox"/>')
    export class BooleanEditor extends Widget<any> {

        constructor(input: JQuery) {
            super(input);

            input.removeClass("flexify");
        }

        public get value(): boolean {
            return this.element.is(":checked");
        }

        protected get_value(): boolean {
            return this.value;
        }

        public set value(value: boolean) {
            this.element.prop("checked", !!value);
        }

        protected set_value(value: boolean): void {
            this.value = value;
        }
    }

    @Editor('Decimal', [IDoubleValue])
    @Element('<input type="text"/>')
    export class DecimalEditor extends Widget<DecimalEditorOptions> implements IDoubleValue {

        constructor(input: JQuery, opt?: DecimalEditorOptions) {
            super(input, opt);

            input.addClass('decimalQ');
			var numericOptions = $.extend(Serenity.DecimalEditor.defaultAutoNumericOptions(), {
				vMin: Q.coalesce(this.options.minValue, this.options.allowNegatives ? (this.options.maxValue != null ? ("-" + this.options.maxValue) : '-999999999999.99') : '0.00'),
                vMax: Q.coalesce(this.options.maxValue, '999999999999.99')
            });

            if (this.options.decimals != null) {
                numericOptions.mDec = this.options.decimals;
            }

            if (this.options.padDecimals != null) {
                numericOptions.aPad = this.options.padDecimals;
            }

            (input as any).autoNumeric(numericOptions);
        }

        get_value(): number {
            var val = (this.element as any).autoNumeric('get');

            if (!!(val == null || val === ''))
                return null;

            return parseFloat(val);
        }

        get value(): number {
            return this.get_value();
        }

        set_value(value: number) {
            if (value == null || (value as any) === '') {
                this.element.val('');
            }
            else {
                (this.element as any).autoNumeric('set', value);
            }
        }

        set value(v: number) {
            this.set_value(v);
        }

        get_isValid(): boolean {
            return !isNaN(this.get_value());
        }

        static defaultAutoNumericOptions(): any {
            return {
                aDec: Q.Culture.decimalSeparator,
                altDec: ((Q.Culture.decimalSeparator === '.') ? ',' : '.'),
                aSep: ((Q.Culture.decimalSeparator === '.') ? ',' : '.'),
                aPad: true
            };
        }
    }

    export interface IntegerEditorOptions {
        minValue?: number;
		maxValue?: number;
		allowNegatives?: boolean;
    }

    @Editor('Integer', [IDoubleValue])
    @Element('<input type="text"/>')
    export class IntegerEditor extends Widget<IntegerEditorOptions> implements IDoubleValue {

        constructor(input: JQuery, opt?: IntegerEditorOptions) {
            super(input, opt);

            input.addClass('integerQ');
            var numericOptions = $.extend(Serenity.DecimalEditor.defaultAutoNumericOptions(),
                {
					vMin: Q.coalesce(this.options.minValue, this.options.allowNegatives ? (this.options.maxValue != null ? ("-" + this.options.maxValue) : '-2147483647') : '0'),
                    vMax: Q.coalesce(this.options.maxValue, 2147483647),
                    aSep: null
                });

            (input as any).autoNumeric(numericOptions);
        }

        get_value(): number {
            var val = (this.element as any).autoNumeric('get');
            if (!!Q.isTrimmedEmpty(val)) {
                return null;
            }
            else {
                return parseInt(val, 10);
            }
        }

        get value(): number {
            return this.get_value();
        }

        set_value(value: number) {
            if (value == null || (value as any) === '') {
                this.element.val('');
            }
            else {
                (this.element as any).autoNumeric('set', value);
            }
        }

        set value(v: number) {
            this.set_value(v);
        }

        get_isValid(): boolean {
            return !isNaN(this.get_value());
        }
    }


    export interface DecimalEditorOptions {
        minValue?: string;
        maxValue?: string;
        decimals?: any;
		padDecimals?: any;
		allowNegatives?: boolean;
    }

    export interface EmailEditorOptions {
        domain?: string;
        readOnlyDomain?: boolean;
    }

    @Editor('Email', [IStringValue, IReadOnly])
    @Element('<input type="text"/>')
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
            if (Q.isEmptyOrNull(value)) {
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
                if (parts.length > 1) {
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

        set value(v: string) {
            this.set_value(v);
        }

        get_readOnly(): boolean {
            var domain = this.element.nextAll('.emaildomain');
            return !(this.element.attr('readonly') == null &&
                (!this.options.readOnlyDomain || domain.attr('readonly') == null));
        }

        set_readOnly(value: boolean): void {
            var domain = this.element.nextAll('.emaildomain');
            if (value) {
                this.element.attr('readonly', 'readonly').addClass('readonly');
                if (!this.options.readOnlyDomain) {
                    domain.attr('readonly', 'readonly').addClass('readonly');
                }
            }
            else {
                this.element.removeAttr('readonly').removeClass('readonly');
                if (!this.options.readOnlyDomain) {
                    domain.removeAttr('readonly').removeClass('readonly');
                }
            }
        }
    }

    export interface EnumEditorOptions extends Select2CommonOptions {
        enumKey?: string;
        enumType?: any;
    }

    @Editor('Enum')
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
                var enumKeyAttr = Q.getAttributes(enumType, Serenity.EnumKeyAttribute, false);
                if (enumKeyAttr.length > 0) {
                    enumKey = enumKeyAttr[0].value;
                }
            }

            var values = Q.Enum.getValues(enumType);
            for (var x of values) {
                var name = Q.Enum.toString(enumType, x);
                this.addOption(parseInt(x, 10).toString(),
                    Q.coalesce(Q.tryGetText('Enums.' + enumKey + '.' + name), name), null, false);
            }
        }

        protected allowClear() {
            return Q.coalesce(this.options.allowClear, true);
        }
    }

    export interface GoogleMapOptions {
        latitude?: any;
        longitude?: any;
        zoom?: any;
        mapTypeId?: any;
        markerTitle?: string;
        markerLatitude?: any;
        markerLongitude?: any;
    }

    declare var google: any;

    @Editor('GoogleMap', [])
    @Element('<div/>')
    export class GoogleMap extends Widget<GoogleMapOptions> {

        private map: any;

        constructor(container: JQuery, opt: GoogleMapOptions) {
            super(container, opt);

            var center = new google.maps.LatLng(
                Q.coalesce(this.options.latitude, 0),
                Q.coalesce(this.options.longitude, 0));

            var mapOpt: any = new Object();
            mapOpt.center = center;
            mapOpt.mapTypeId = Q.coalesce(this.options.mapTypeId, 'roadmap');
            mapOpt.zoom = Q.coalesce(this.options.zoom, 15);
            mapOpt.zoomControl = true;
            this.map = new google.maps.Map(container[0], mapOpt);

            if (this.options.markerTitle != null) {
                var markerOpt: any = new Object();

                var lat = this.options.markerLatitude;
                if (lat == null) {
                    lat = Q.coalesce(this.options.latitude, 0);
                }
                var lon = this.options.markerLongitude;
                if (lon == null) {
                    lon = Q.coalesce(this.options.longitude, 0);
                }
                markerOpt.position = new google.maps.LatLng(lat, lon);
                markerOpt.map = this.map;
                markerOpt.title = this.options.markerTitle;
                markerOpt.animation = 2;
                new google.maps.Marker(markerOpt);
            }

            Serenity.LazyLoadHelper.executeOnceWhenShown(container, () => {
                google.maps.event.trigger(this.map, 'resize', []);
                this.map.setCenter(center);
                // in case it wasn't visible (e.g. in dialog)
            });
        }

        get_map(): any {
            return this.map;
        }
    }


    export interface HtmlContentEditorOptions {
        cols?: any;
        rows?: any;
    }

    @Editor('HtmlContent', [IStringValue, IReadOnly])
    @Element('<textarea/>')
    export class HtmlContentEditor extends Widget<HtmlContentEditorOptions>
        implements IStringValue, IReadOnly {

        private _instanceReady: boolean;

        constructor(textArea: JQuery, opt?: HtmlContentEditorOptions) {
            super(textArea, opt)

            this._instanceReady = false;
            HtmlContentEditor.includeCKEditor();

            var id = textArea.attr('id');
            if (Q.isTrimmedEmpty(id)) {
                textArea.attr('id', this.uniqueName);
                id = this.uniqueName;
            }

            if (this.options.cols != null)
                textArea.attr('cols', this.options.cols);

            if (this.options.rows != null)
                textArea.attr('rows', this.options.rows);

            this.addValidationRule(this.uniqueName, e => {
                if (e.hasClass('required')) {
                    var value = Q.trimToNull(this.get_value());
                    if (value == null)
                        return Q.text('Validation.Required');
                }

                return null;
            });

            Serenity.LazyLoadHelper.executeOnceWhenShown(this.element, () => {
                var config = this.getConfig();
                window['CKEDITOR'] && window['CKEDITOR'].replace(id, config);
            });
        }

        protected instanceReady(x: any): void {
            this._instanceReady = true;
            $(x.editor.container.$).addClass(this.element.attr('class'));
            this.element.addClass('select2-offscreen').css('display', 'block');

            // for validation to work
            x.editor.setData(this.element.val());
            x.editor.setReadOnly(this.get_readOnly());
        }

        protected getLanguage(): string {
            if (!window['CKEDITOR'])
                return 'en';

            var CKEDITOR = window['CKEDITOR'];

            var lang = Q.coalesce(Q.trimToNull($('html').attr('lang')), 'en');
            if (!!CKEDITOR.lang.languages[lang]) {
                return lang;
            }
            if (lang.indexOf(String.fromCharCode(45)) >= 0) {
                lang = lang.split(String.fromCharCode(45))[0];
            }
            if (!!CKEDITOR.lang.languages[lang]) {
                return lang;
            }
            return 'en';
        }

        protected getConfig(): CKEditorConfig {
            return {
                customConfig: '',
                language: this.getLanguage(),
                bodyClass: 's-HtmlContentBody',
                on: {
                    instanceReady: (x: any) => this.instanceReady(x),
                    change: (x1: any) => {
                        x1.editor.updateElement();
                        this.element.triggerHandler('change');
                    }
                },
                toolbarGroups: [
                    {
                        name: 'clipboard',
                        groups: ['clipboard', 'undo']
                    }, {
                        name: 'editing',
                        groups: ['find', 'selection', 'spellchecker']
                    }, {
                        name: 'insert',
                        groups: ['links', 'insert', 'blocks', 'bidi', 'list', 'indent']
                    }, {
                        name: 'forms',
                        groups: ['forms', 'mode', 'document', 'doctools', 'others', 'about', 'tools']
                    }, {
                        name: 'colors'
                    }, {
                        name: 'basicstyles',
                        groups: ['basicstyles', 'cleanup']
                    }, {
                        name: 'align'
                    }, {
                        name: 'styles'
                    }],
                removeButtons: 'SpecialChar,Anchor,Subscript,Styles',
                format_tags: 'p;h1;h2;h3;pre',
                removeDialogTabs: 'image:advanced;link:advanced',
                removePlugins: 'uploadimage,image2',
                contentsCss: Q.resolveUrl('~/Content/site/site.htmlcontent.css'),
                entities: false,
                entities_latin: false,
                entities_greek: false,
                autoUpdateElement: true,
                height: (this.options.rows == null || this.options.rows === 0) ? null :
                    ((this.options.rows * 20) + 'px')
            };
        }

        protected getEditorInstance() {
            var id = this.element.attr('id');
            return window['CKEDITOR'].instances[id];
        }

        destroy(): void {
            var instance = this.getEditorInstance();
            instance && instance.destroy(true);
            super.destroy();
        }

        get_value(): string {
            var instance = this.getEditorInstance();
            if (this._instanceReady && instance) {
                return instance.getData();
            }
            else {
                return this.element.val();
            }
        }

        get value(): string {
            return this.get_value();
        }

        set_value(value: string): void {
            var instance = this.getEditorInstance();
            this.element.val(value);
            if (this._instanceReady && instance)
                instance.setData(value);
        }

        set value(v: string) {
            this.set_value(v);
        }

        get_readOnly(): boolean {
            return !Q.isEmptyOrNull(this.element.attr('disabled'));
        }

        set_readOnly(value: boolean) {

            if (this.get_readOnly() !== value) {
                if (value) {
                    this.element.attr('disabled', 'disabled');
                }
                else {
                    this.element.removeAttr('disabled');
                }

                var instance = this.getEditorInstance();
                if (this._instanceReady && instance)
                    instance.setReadOnly(value);
            }
        }

        static CKEditorVer = "4.7.1";

        static includeCKEditor(): void {
            if (window['CKEDITOR']) {
                return;
            }

            var script = $('#CKEditorScript');
            if (script.length > 0) {
                return;
            }

            $('<script/>').attr('type', 'text/javascript')
                .attr('id', 'CKEditorScript')
                .attr('src', Q.resolveUrl('~/Scripts/CKEditor/ckeditor.js?v=' +
                    HtmlContentEditor.CKEditorVer))
                .appendTo(window.document.head);
        };
    }

    @Editor('HtmlNoteContent')
    export class HtmlNoteContentEditor extends HtmlContentEditor {
        constructor(textArea: JQuery, opt?: HtmlContentEditorOptions) {
            super(textArea, opt);
        }

        protected getConfig(): CKEditorConfig {
            var config = super.getConfig();
            (config as any).removeButtons += ',Cut,Copy,Paste,BulletedList,NumberedList,' +
                'Indent,Outdent,SpecialChar,Subscript,Superscript,Styles,PasteText,' +
                'PasteFromWord,Strike,Link,Unlink,CreatePlaceholder,Image,Table,' +
                'HorizontalRule,Source,Maximize,Format,Font,FontSize,Anchor,Blockquote,' +
                'CreatePlaceholder,BGColor,JustifyLeft,JustifyCenter,' +
                'JustifyRight,JustifyBlock,Superscript,RemoveFormat';

            (config as any).removePlugins = 'elementspath,uploadimage,image2';
            return config;
        }
    }

    @Editor('HtmlReportContent')
    export class HtmlReportContentEditor extends HtmlContentEditor {
        constructor(textArea: JQuery, opt?: HtmlContentEditorOptions) {
            super(textArea, opt);
        }

        protected getConfig(): CKEditorConfig {
            var config = super.getConfig();
            (config as any).removeButtons += ',Cut,Copy,Paste,BulletedList,NumberedList,' +
                'Indent,Outdent,SpecialChar,Subscript,Superscript,Styles,' +
                'PasteText,PasteFromWord,Strike,Link,Unlink,CreatePlaceholder,' +
                'Image,Table,HorizontalRule,Source,Maximize,Format,Font,FontSize,' +
                'Anchor,Blockquote,CreatePlaceholder,BGColor,JustifyLeft,JustifyCenter,' +
                'JustifyRight,JustifyBlock,Superscript,RemoveFormat';

            (config as any).removePlugins = 'elementspath,uploadimage,image2';
            return config;
        }
    }

    export interface ImageUploadEditorOptions {
        minWidth?: number;
        maxWidth?: number;
        minHeight?: number;
        maxHeight?: number;
        minSize?: number;
        maxSize?: number;
        originalNameProperty?: string;
        urlPrefix?: string;
        allowNonImage?: boolean;
        displayFileName?: boolean;
    }

    @Editor('ImageUpload', [IReadOnly])
    @Element('<div/>')
    export class ImageUploadEditor extends Widget<ImageUploadEditorOptions>
        implements IReadOnly, IGetEditValue, ISetEditValue {

        constructor(div: JQuery, opt: ImageUploadEditorOptions) {
            super(div, opt);

            div.addClass('s-ImageUploadEditor');
            if (Q.isEmptyOrNull(this.options.originalNameProperty))
                div.addClass('hide-original-name');

            this.toolbar = new Toolbar($('<div/>').appendTo(this.element), {
                buttons: this.getToolButtons()
            });

            var progress = $('<div><div></div></div>')
                .addClass('upload-progress')
                .prependTo(this.toolbar.element);

            var uio = this.getUploadInputOptions();
            this.uploadInput = UploadHelper.addUploadInput(uio);

            this.fileSymbols = $('<ul/>').appendTo(this.element);
            this.updateInterface();
        }

        protected getUploadInputOptions(): UploadInputOptions {
            return {
                container: this.toolbar.findButton('add-file-button'),
                zone: this.element,
                inputName: this.uniqueName,
                progress: this.progress,
                fileDone: (response, name, data) => {
                    if (!UploadHelper.checkImageConstraints(response, this.options)) {
                        return;
                    }

                    var newEntity: UploadedFile = {
                        OriginalName: name,
                        Filename: response.TemporaryFile
                    };

                    this.entity = newEntity;
                    this.populate();
                    this.updateInterface();
                }
            }
        }

        protected addFileButtonText(): string {
            return Q.text('Controls.ImageUpload.AddFileButton');
        }

        protected getToolButtons(): ToolButton[] {
            return [
                {
                    title: this.addFileButtonText(),
                    cssClass: 'add-file-button',
                    onClick: function () {
                    }
                },
                {
                    title: '',
                    hint: Q.text('Controls.ImageUpload.DeleteButtonHint'),
                    cssClass: 'delete-button',
                    onClick: () => {
                        this.entity = null;
                        this.populate();
                        this.updateInterface();
                    }
                }
            ];
        }

        protected populate(): void {
            var displayOriginalName = this.options.displayFileName ||
                !Q.isTrimmedEmpty(this.options.originalNameProperty);

            if (this.entity == null) {
                UploadHelper.populateFileSymbols(this.fileSymbols,
                    null, displayOriginalName, this.options.urlPrefix);
            }
            else {
                UploadHelper.populateFileSymbols(
                    this.fileSymbols, [this.entity], displayOriginalName,
                    this.options.urlPrefix);
            }
        }

        protected updateInterface(): void {
            var addButton = this.toolbar.findButton('add-file-button');
            var delButton = this.toolbar.findButton('delete-button');
            addButton.toggleClass('disabled', this.get_readOnly());
            delButton.toggleClass('disabled', this.get_readOnly() ||
                this.entity == null);
        }

        get_readOnly(): boolean {
            return this.uploadInput.attr('disabled') != null;
        }

        set_readOnly(value: boolean): void {
            if (this.get_readOnly() !== value) {
                if (value) {
                    (this.uploadInput.attr('disabled', 'disabled') as any).fileupload('disable');
                }
                else {
                    (this.uploadInput.removeAttr('disabled') as any).fileupload('enable');
                }
                this.updateInterface();
            }
        }

        get_value(): UploadedFile {
            if (this.entity == null) {
                return null;
            }
            var copy = $.extend({}, this.entity);
            return copy;
        }

        get value(): UploadedFile {
            return this.get_value();
        }

        set_value(value: UploadedFile): void {
            if (value != null) {
                if (value.Filename == null) {
                    this.entity = null;
                }
                else {
                    this.entity = $.extend({}, value);
                }
            }
            else {
                this.entity = null;
            }
            this.populate();
            this.updateInterface();
        }

        set value(v: UploadedFile) {
            this.set_value(v);
        }

        getEditValue(property: PropertyItem, target: any) {
            target[property.name] = this.entity == null ? null :
                Q.trimToNull(this.entity.Filename);
        }

        setEditValue(source: any, property: PropertyItem) {
            var value: UploadedFile = {};
            value.Filename = source[property.name];
            if (Q.isEmptyOrNull(this.options.originalNameProperty)) {

                if (this.options.displayFileName) {
                    var s = Q.coalesce(value.Filename, '');
                    var idx = Q.replaceAll(s, '\\', '/').lastIndexOf('/');
                    if (idx >= 0) {
                        value.OriginalName = s.substr(idx + 1);
                    }
                    else {
                        value.OriginalName = s;
                    }
                }
            }
            else {
                value.OriginalName = source[this.options.originalNameProperty];
            }

            this.set_value(value);
        }

        protected entity: UploadedFile;
        protected toolbar: Toolbar;
        protected progress: JQuery;
        protected fileSymbols: JQuery;
        protected uploadInput: JQuery;
    }

    @Editor('MultipleImageUpload', [IReadOnly])
    @Element('<div/>')
    export class MultipleImageUploadEditor extends Widget<ImageUploadEditorOptions>
        implements IReadOnly, IGetEditValue, ISetEditValue {

        private entities: UploadedFile[];
        private toolbar: Toolbar;
        private fileSymbols: JQuery;
        private uploadInput: JQuery;

        constructor(div: JQuery, opt: ImageUploadEditorOptions) {
            super(div, opt);

            this.entities = [];
            div.addClass('s-MultipleImageUploadEditor');
            var self = this;
            this.toolbar = new Toolbar($('<div/>').appendTo(this.element), {
                buttons: this.getToolButtons()
            });
            var progress = $('<div><div></div></div>')
                .addClass('upload-progress').prependTo(this.toolbar.element);

            var addFileButton = this.toolbar.findButton('add-file-button');

            this.uploadInput = Serenity.UploadHelper.addUploadInput({
                container: addFileButton,
                zone: this.element,
                inputName: this.uniqueName,
                progress: progress,
                fileDone: (response, name, data) => {
                    if (!UploadHelper.checkImageConstraints(response, this.options)) {
                        return;
                    }
                    var newEntity = { OriginalName: name, Filename: response.TemporaryFile };
                    self.entities.push(newEntity);
                    self.populate();
                    self.updateInterface();
                }
            });

            this.fileSymbols = $('<ul/>').appendTo(this.element);
            this.updateInterface();
        }

        protected addFileButtonText(): string {
            return Q.text('Controls.ImageUpload.AddFileButton');
        }

        protected getToolButtons(): ToolButton[] {
            return [{
                title: this.addFileButtonText(),
                cssClass: 'add-file-button',
                onClick: function () {
                }
            }];
        }

        protected populate(): void {
            Serenity.UploadHelper.populateFileSymbols(this.fileSymbols, this.entities,
                true, this.options.urlPrefix);

            this.fileSymbols.children().each((i, e) => {
                var x = i;
                $("<a class='delete'></a>").appendTo($(e).children('.filename'))
                    .click(ev => {
                        ev.preventDefault();
                        this.entities.splice(x, 1);
                        this.populate();
                    });
            });
        }

        protected updateInterface(): void {
            var addButton = this.toolbar.findButton('add-file-button');
            addButton.toggleClass('disabled', this.get_readOnly());
            this.fileSymbols.find('a.delete').toggle(!this.get_readOnly());
        }

        get_readOnly(): boolean {
            return this.uploadInput.attr('disabled') != null;
        }

        set_readOnly(value: boolean): void {
            if (this.get_readOnly() !== value) {
                if (value) {
                    (this.uploadInput.attr('disabled', 'disabled') as any).fileupload('disable');
                }
                else {
                    (this.uploadInput.removeAttr('disabled') as any).fileupload('enable');
                }
                this.updateInterface();
            }
        }

        get_value(): UploadedFile[] {
            return this.entities.map(function (x) {
                return $.extend({}, x);
            });
        }

        get value(): UploadedFile[] {
            return this.get_value();
        }

        set_value(value: UploadedFile[]) {
            this.entities = (value || []).map(function (x) {
                return $.extend({}, x);
            });
            this.populate();
            this.updateInterface();
        }

        set value(v: UploadedFile[]) {
            this.set_value(v);
        }

        getEditValue(property: PropertyItem, target: any) {
            if (this.jsonEncodeValue) {
                target[property.name] = $.toJSON(this.get_value());
            }
            else {
                target[property.name] = this.get_value();
            }
        }

        setEditValue(source: any, property: PropertyItem) {
            var val = source[property.name];
            if (Q.isInstanceOfType(val, String)) {
                var json = Q.coalesce(Q.trimToNull(val), '[]');
                if (Q.startsWith(json, '[') && Q.endsWith(json, ']')) {
                    this.set_value($.parseJSON(json));
                }
                else {
                    this.set_value([{
                        Filename: json,
                        OriginalName: 'UnknownFile'
                    }]);
                }
            }
            else {
                this.set_value(val as any);
            }
        }

        @Option()
        public jsonEncodeValue: boolean;
    }

    // http://digitalbush.com/projects/masked-input-plugin/
    @Editor('Masked', [IStringValue])
    @Element("<input type=\"text\"/>")
    export class MaskedEditor extends Widget<MaskedEditorOptions> {

        constructor(input: JQuery, opt?: MaskedEditorOptions) {
            super(input, opt);

            (input as any).mask(this.options.mask || '', {
                placeholder: Q.coalesce(this.options.placeholder, '_')
            });
        }

        public get value(): string {
            this.element.triggerHandler("blur.mask");
            return this.element.val();
        }

        protected get_value(): string {
            return this.value;
        }

        public set value(value: string) {
            this.element.val(value);
        }

        protected set_value(value: string): void {
            this.value = value;
        }
    }

    export interface MaskedEditorOptions {
        mask?: string;
        placeholder?: string;
    }

    @Editor('String', [IStringValue])
    @Element("<input type=\"text\"/>")
    export class StringEditor extends Widget<any> {

        constructor(input: JQuery) {
            super(input);
        }

        public get value(): string {
            return this.element.val();
        }

        protected get_value(): string {
            return this.value;
        }

        public set value(value: string) {
            this.element.val(value);
        }

        protected set_value(value: string): void {
            this.value = value;
        }
    }

    @Editor('EmailAddress')
    export class EmailAddressEditor extends Serenity.StringEditor {
        constructor(input: JQuery) {
            super(input);

            input.attr('type', 'email')
                .addClass('email');
        }
    }

    @Editor('Password')
    export class PasswordEditor extends StringEditor {
        constructor(input: JQuery) {
            super(input);

            input.attr('type', 'password');
        }
    }

    export interface RadioButtonEditorOptions {
        enumKey?: string;
        enumType?: any;
        lookupKey?: string;
    }

    @Editor('RadioButton', [IStringValue, IReadOnly])
    @Element('<div/>')
    export class RadioButtonEditor extends Widget<RadioButtonEditorOptions>
        implements IReadOnly {

        constructor(input: JQuery, opt: RadioButtonEditorOptions) {
            super(input, opt);

            if (Q.isEmptyOrNull(this.options.enumKey) &&
                this.options.enumType == null &&
                Q.isEmptyOrNull(this.options.lookupKey)) {
                return;
            }

            if (!Q.isEmptyOrNull(this.options.lookupKey)) {
                var lookup = Q.getLookup(this.options.lookupKey);
                for (var item of lookup.items) {
                    var textValue = item[lookup.textField];
                    var text = (textValue == null ? '' : textValue.toString());
                    var idValue = item[lookup.idField];
                    var id = (idValue == null ? '' : idValue.toString());
                    this.addRadio(id, text);
                }
            }
            else {
                var enumType = this.options.enumType || Serenity.EnumTypeRegistry.get(this.options.enumKey);
                var enumKey = this.options.enumKey;
                if (enumKey == null && enumType != null) {
                    var enumKeyAttr = Q.getAttributes(enumType, Serenity.EnumKeyAttribute, false);
                    if (enumKeyAttr.length > 0) {
                        enumKey = enumKeyAttr[0].value;
                    }
                }

                var values = Q.Enum.getValues(enumType);
                for (var x of values) {
                    var name = Q.Enum.toString(enumType, x);
                    this.addRadio(x.toString(), Q.coalesce(Q.tryGetText(
                        'Enums.' + enumKey + '.' + name), name));
                }
            }
        }

        protected addRadio(value: string, text: string) {
            var label = $('<label/>').text(text);
            $('<input type="radio"/>').attr('name', this.uniqueName)
                .attr('id', this.uniqueName + '_' + value)
                .attr('value', value).prependTo(label);
            label.appendTo(this.element);
        }

        get_value(): string {
            return this.element.find('input:checked').first().val();
        }

        get value(): string {
            return this.get_value();
        }

        set_value(value: string): void {
            if (value !== this.get_value()) {
                var inputs = this.element.find('input');
                var checks = inputs.filter(':checked');
                if (checks.length > 0) {
                    (checks[0] as HTMLInputElement).checked = false;
                }
                if (!Q.isEmptyOrNull(value)) {
                    checks = inputs.filter('[value=' + value + ']');
                    if (checks.length > 0) {
                        (checks[0] as HTMLInputElement).checked = true;
                    }
                }
            }
        }

        set value(v: string) {
            this.set_value(v);
        }

        get_readOnly(): boolean {
            return this.element.attr('disabled') != null;
        }

        set_readOnly(value: boolean): void {
            if (this.get_readOnly() !== value) {
                if (value) {
                    this.element.attr('disabled', 'disabled')
                        .find('input').attr('disabled', 'disabled');
                }
                else {
                    this.element.removeAttr('disabled')
                        .find('input').removeAttr('disabled');
                }
            }
        }

    }

    export interface RecaptchaOptions {
        siteKey?: string;
        language?: string;
    }

    @Decorators.registerEditor('Serenity.Recaptcha', [IStringValue])
    @Decorators.element("<div/>")
    export class Recaptcha extends Widget<RecaptchaOptions> implements IStringValue {
        constructor(div: JQuery, opt: RecaptchaOptions) {
            super(div, opt);

            this.element.addClass('g-recaptcha').attr('data-sitekey', this.options.siteKey);
            if (!!(window['grecaptcha'] == null && $('script#RecaptchaInclude').length === 0)) {
                var src = 'https://www.google.com/recaptcha/api.js';
                var lng = this.options.language;
                if (lng == null) {
                    lng = Q.coalesce($('html').attr('lang'), '');
                }
                src += '?hl=' + lng;
                $('<script/>').attr('id', 'RecaptchaInclude').attr('src', src).appendTo(document.body);
            }

            var valInput = $('<input />').insertBefore(this.element)
                .attr('id', this.uniqueName + '_validate').val('x');

            var gro = {};
            gro['visibility'] = 'hidden';
            gro['width'] = '0px';
            gro['height'] = '0px';
            gro['padding'] = '0px';

            var input = valInput.css(gro);
            var self = this;

            VX.addValidationRule(input, this.uniqueName, e => {
                if (Q.isEmptyOrNull(this.get_value())) {
                    return Q.text('Validation.Required');
                }
                return null;
            });
        }

        get_value(): string {
            return this.element.find('.g-recaptcha-response').val();
        }

        set_value(value: string): void {
            // ignore
        }
    }

    export interface TextAreaEditorOptions {
        cols?: number;
        rows?: number;
    }

    @Editor('TextArea', [IStringValue])
    @Element("<textarea />")
    export class TextAreaEditor extends Widget<TextAreaEditorOptions> {

        constructor(input: JQuery, opt?: TextAreaEditorOptions) {
            super(input, opt);

            if (this.options.cols !== 0) {
                input.attr('cols', Q.coalesce(this.options.cols, 80));
            }
            if (this.options.rows !== 0) {
                input.attr('rows', Q.coalesce(this.options.rows, 6));
            }
        }

        public get value(): string {
            return this.element.val();
        }

        protected get_value(): string {
            return this.value;
        }

        public set value(value: string) {
            this.element.val(value);
        }

        protected set_value(value: string): void {
            this.value = value;
        }
    }

    export interface TimeEditorOptions {
        noEmptyOption?: boolean;
        startHour?: any;
        endHour?: any;
        intervalMinutes?: any;
    }

    @Editor('Time', [IDoubleValue, IReadOnly])
    @Element("<select />")
    export class TimeEditor extends Widget<TimeEditorOptions> {

        private minutes: JQuery;

        constructor(input: JQuery, opt?: TimeEditorOptions) {
            super(input, opt);

            input.addClass('editor s-TimeEditor hour');

            if (!this.options.noEmptyOption) {
                Q.addOption(input, '', '--');
            }

            for (var h = (this.options.startHour || 0); h <= (this.options.endHour || 23); h++) {
                Q.addOption(input, h.toString(), ((h < 10) ? ('0' + h) : h.toString()));
            }

            this.minutes = $('<select/>').addClass('editor s-TimeEditor minute').insertAfter(input);
            this.minutes.change(() => this.element.trigger("change"));

            for (var m = 0; m <= 59; m += (this.options.intervalMinutes || 5)) {
                Q.addOption(this.minutes, m.toString(), ((m < 10) ? ('0' + m) : m.toString()));
            }
        }

        public get value(): number {
            var hour = Q.toId(this.element.val());
            var minute = Q.toId(this.minutes.val());
            if (hour == null || minute == null) {
                return null;
            }
            return hour * 60 + minute;
        }

        protected get_value(): number {
            return this.value;
        }

        public set value(value: number) {
            if (!value) {
                if (this.options.noEmptyOption) {
                    this.element.val(this.options.startHour);
                    this.minutes.val('0');
                }
                else {
                    this.element.val('');
                    this.minutes.val('0');
                }
            }
            else {
                this.element.val(Math.floor(value / 60).toString());
                this.minutes.val(value % 60);
            }
        }

        protected set_value(value: number): void {
            this.value = value;
        }

        get_readOnly(): boolean {
            return this.element.hasClass('readonly');
        }

        set_readOnly(value: boolean): void {

            if (value !== this.get_readOnly()) {
                if (value) {
                    this.element.addClass('readonly').attr('readonly', 'readonly');
                }
                else {
                    this.element.removeClass('readonly').removeAttr('readonly');
                }
                Serenity.EditorUtils.setReadonly(this.minutes, value);
            }
        }
    }

    @Editor('URL', [IStringValue])
    export class URLEditor extends StringEditor {

        constructor(input: JQuery) {
            super(input);

            input.addClass("url").attr("title", "URL should be entered in format: 'http://www.site.com/page'.");

            input.on("blur." + this.uniqueName, e => {
                var validator = input.closest("form").data("validator") as JQueryValidation.Validator;
                if (validator == null)
                    return;

                if (!input.hasClass("error"))
                    return;

                var value = Q.trimToNull(input.val());
                if (!value)
                    return;

                value = "http://" + value;

                if ($.validator.methods['url'].call(validator, value, input[0]) == true) {
                    input.val(value);
                    validator.element(input);
                }
            });
        }
    }

    @Editor('Select2Ajax', [IStringValue])
    @Element('<input type="hidden" />')
    export class Select2AjaxEditor<TOptions, TItem> extends Widget<TOptions> implements IStringValue {
        pageSize: number = 50;

        constructor(hidden: JQuery, opt: TOptions) {
            super(hidden, opt);

            var emptyItemText = this.emptyItemText();
            if (emptyItemText != null)
                hidden.attr("placeholder", emptyItemText);

            hidden.select2(this.getSelect2Options());

            hidden.attr("type", "text"); // jquery validate to work

            hidden.on("change." + this.uniqueName, (e, x) => {
                if (WX.hasOriginalEvent(e) || !x) {
                    if (ValidationHelper.getValidator(hidden) != null)
                        hidden.valid();
                }
            });
        }

        protected emptyItemText(): string {
            var txt = this.element.attr('placeholder');
            if (txt == null) {
                txt = Q.text('Controls.SelectEditor.EmptyItemText');
            }
            return txt;
        }

        protected getService(): string {
            throw new Error("Not implemented!");
        }

        protected query(request: ListRequest, callback: (p1: ListResponse<any>) => void): void {
            var options: Q.ServiceOptions<any> = {
                blockUI: false,
                service: this.getService() + '/List',
                request: request,
                onSuccess: function (response) {
                    callback(response);
                }
            };
            this.executeQuery(options);
        }

        protected executeQuery(options: ServiceOptions<ListResponse<any>>): void {
            Q.serviceCall(options);
        }

        protected queryByKey(key: string, callback: (p1: any) => void): void {
            var options: Q.ServiceOptions<any> = {
                blockUI: false,
                service: this.getService() + '/Retrieve',
                request: { EntityId: key },
                onSuccess: function (response) {
                    callback(response.Entity);
                }
            };
            this.executeQueryByKey(options);
        }

        protected executeQueryByKey(options: ServiceOptions<RetrieveResponse<any>>): void {
            Q.serviceCall(options);
        }

        protected getItemKey(item: any): string {
            return null;
        }

        protected getItemText(item: any): string {
            return null;
        }

        protected getTypeDelay(): number {
            return 500;
        }

        protected getSelect2Options(): Select2Options {
            var emptyItemText = this.emptyItemText();
            var queryTimeout = 0;
            return {
                minimumResultsForSearch: 10,
                placeHolder: (!Q.isEmptyOrNull(emptyItemText) ? emptyItemText : null),
                allowClear: Q.isValue(emptyItemText),
                query: query => {
                    var request = {
                        ContainsText: Q.trimToNull(query.term), Skip: (query.page - 1) * this.pageSize, Take: this.pageSize + 1
                    };

                    if (queryTimeout !== 0) {
                        window.clearTimeout(queryTimeout);
                    }

                    queryTimeout = window.setTimeout(() => {
                        this.query(request, response => {
                            query.callback({
                                results: response.Entities.slice(0, this.pageSize).map(x => {
                                    return { id: this.getItemKey(x), text: this.getItemText(x), source: x };
                                }), more: response.Entities.length >= this.pageSize
                            });
                        });
                    }, this.getTypeDelay());

                },
                initSelection: (element, callback) => {
                    var val = element.val();
                    if (Q.isEmptyOrNull(val)) {
                        callback(null);
                        return;
                    }
                    this.queryByKey(val, result => {
                        callback((result == null ? null : {
                            id: this.getItemKey(result),
                            text: this.getItemText(result),
                            source: result
                        }));
                    });
                }
            };
        }

        protected addInplaceCreate(title: string): void {
            var self = this;
            $('<a><b/></a>').addClass('inplace-button inplace-create')
                .attr('title', title).insertAfter(this.element).click(function (e) {
                    self.inplaceCreateClick(e);
                });

            this.get_select2Container().add(this.element)
                .addClass('has-inplace-button');
        }

        protected inplaceCreateClick(e: any): void {
        }

        protected get_select2Container(): JQuery {
            return this.element.prevAll('.select2-container');
        }

        get_value(): string {
            return Q.safeCast(this.element.select2('val'), String);
        }

        get value(): string {
            return this.get_value();
        }

        set_value(value: string) {
            if (value !== this.get_value()) {
                var el = this.element;
                el.select2('val', value);
                el.data('select2-change-triggered', true);
                el.triggerHandler('change', [true])
                el.data('select2-change-triggered', false);
            }
        }

        set value(v: string) {
            this.set_value(v);
        }
    }
}