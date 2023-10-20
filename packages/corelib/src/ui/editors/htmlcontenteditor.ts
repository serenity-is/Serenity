import { Decorators } from "../../decorators";
import { IReadOnly, IStringValue } from "../../interfaces";
import { endsWith, isEmptyOrNull, isTrimmedEmpty, resolveUrl, localText, trimToNull } from "../../q";
import { LazyLoadHelper } from "../helpers/lazyloadhelper";
import { Widget } from "../widgets/widget";

export interface HtmlContentEditorOptions {
    cols?: any;
    rows?: any;
}

export interface CKEditorConfig {
}    

@Decorators.registerEditor('Serenity.HtmlContentEditor', [IStringValue, IReadOnly])
@Decorators.element('<textarea/>')
export class HtmlContentEditor extends Widget<HtmlContentEditorOptions>
    implements IStringValue, IReadOnly {

    private _instanceReady: boolean;

    constructor(textArea: JQuery, opt?: HtmlContentEditorOptions) {
        super(textArea, opt)

        this._instanceReady = false;
        HtmlContentEditor.includeCKEditor();

        var id = textArea.attr('id');
        if (isTrimmedEmpty(id)) {
            textArea.attr('id', this.uniqueName);
            id = this.uniqueName;
        }

        if (this.options.cols != null)
            textArea.attr('cols', this.options.cols);

        if (this.options.rows != null)
            textArea.attr('rows', this.options.rows);

        this.addValidationRule(this.uniqueName, e => {
            if (e.hasClass('required')) {
                var value = trimToNull(this.get_value());
                if (value == null)
                    return localText('Validation.Required');
            }

            return null;
        });

        LazyLoadHelper.executeOnceWhenShown(this.element, () => {
            var config = this.getConfig();
            (window as any)['CKEDITOR'] && (window as any)['CKEDITOR'].replace(id, config);
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
        if (!(window as any)['CKEDITOR'])
            return 'en';

        var CKEDITOR = (window as any)['CKEDITOR'];

        var lang = (trimToNull($('html').attr('lang')) ?? 'en');
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
            contentsCss: resolveUrl('~/Content/site/site.htmlcontent.css'),
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
        return (window as any)['CKEDITOR'].instances[id];
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
        return !isEmptyOrNull(this.element.attr('disabled'));
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
    static CKEditorBasePath: string;

    static getCKEditorBasePath(): string {
        var path = HtmlContentEditor.CKEditorBasePath;
        if (path == null) {
            // @ts-ignore
            if (typeof CKEDITOR_BASEPATH !== "undefined" && CKEDITOR_BASEPATH)
                // @ts-ignore
                path = CKEDITOR_BASEPATH;
            else
                return "~/Serenity.Assets/Scripts/ckeditor/";
        }
        if (endsWith(path, '/'))
            return path;
        return path + '/';
    }

    static includeCKEditor(): void {
        if ((window as any)['CKEDITOR']) {
            return;
        }

        var script = $('#CKEditorScript');
        if (script.length > 0) {
            return;
        }

        $('<script/>').attr('type', 'text/javascript')
            .attr('id', 'CKEditorScript')
            .attr('src', resolveUrl(HtmlContentEditor.getCKEditorBasePath() + 'ckeditor.js?v=' +
                HtmlContentEditor.CKEditorVer))
            .appendTo(window.document.head);
    };
}

@Decorators.registerEditor('Serenity.HtmlNoteContentEditor')
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

@Decorators.registerEditor('Serenity.HtmlReportContentEditor')
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