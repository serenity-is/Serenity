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

    function Editor(name: string, intf?: any[]) {
        return Decorators.registerEditor('Serenity.' + name + 'Editor', intf)
    }

    import Element = Decorators.element

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

    interface CKEditorConfig {
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
            var copy = Q.extend({}, this.entity);
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
                    this.entity = Q.extend({}, value);
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
                return Q.extend({}, x);
            });
        }

        get value(): UploadedFile[] {
            return this.get_value();
        }

        set_value(value: UploadedFile[]) {
            this.entities = (value || []).map(function (x) {
                return Q.extend({}, x);
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