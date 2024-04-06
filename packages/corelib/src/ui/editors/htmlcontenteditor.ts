import { Fluent, localText, resolveUrl } from "../../base";
import { IReadOnly, IStringValue } from "../../interfaces";
import { isTrimmedEmpty } from "../../q";
import { Decorators } from "../../types/decorators";
import { LazyLoadHelper } from "../helpers/lazyloadhelper";
import { EditorProps, EditorWidget } from "../widgets/widget";

export interface HtmlContentEditorOptions {
    cols?: number;
    rows?: number;
}

export interface CKEditorConfig {
}    

@Decorators.registerEditor('Serenity.HtmlContentEditor', [IStringValue, IReadOnly])
export class HtmlContentEditor<P extends HtmlContentEditorOptions = HtmlContentEditorOptions> extends EditorWidget<P>
    implements IStringValue, IReadOnly {

    private _instanceReady: boolean;
    declare readonly domNode: HTMLTextAreaElement;

    static override createDefaultElement() { return document.createElement("textarea"); }

    constructor(props: EditorProps<P>) {
        super(props);

        this._instanceReady = false;

        let textArea = this.domNode;
        var id = textArea.getAttribute('id');
        if (isTrimmedEmpty(id)) {
            textArea.setAttribute('id', this.uniqueName);
            id = this.uniqueName;
        }

        if (this.options.cols != null)
            textArea.setAttribute('cols', "" + (this.options.cols ?? 0));

        if (this.options.rows != null)
            textArea.setAttribute('rows', "" + (this.options.rows ?? 0));

        this.addValidationRule(this.uniqueName, input => {
            if (input.classList.contains('required')) {
                if (!this.get_value()?.trim())
                    return localText('Validation.Required');
            }

            return null;
        });

        HtmlContentEditor.includeCKEditor(() => {
            LazyLoadHelper.executeOnceWhenShown(this.domNode, () => {
                var config = this.getConfig();
                (window as any)['CKEDITOR'] && (window as any)['CKEDITOR'].replace(id, config);
            });
        });
    }

    protected instanceReady(x: any): void {
        this._instanceReady = true;

        this.domNode.classList.forEach((clss) => { x.editor.container.$?.classList.add(clss); });
        this.domNode.classList.add('select2-offscreen');
        this.domNode.style.display = 'block';

        // for validation to work
        x.editor.setData(this.domNode.value);
        x.editor.setReadOnly(this.get_readOnly());
    }

    protected getLanguage(): string {
        if (!(window as any)['CKEDITOR'])
            return 'en';

        var CKEDITOR = (window as any)['CKEDITOR'];

        var lang = document.documentElement.getAttribute('lang')?.trim() || 'en';
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
            versionCheck: false,
            on: {
                instanceReady: (x: any) => this.instanceReady(x),
                change: (x1: any) => {
                    x1.editor.updateElement();
                    Fluent.trigger(this.domNode, "change");
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
        var id = this.domNode.getAttribute("id");
        return (window as any)['CKEDITOR']?.instances?.[id];
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
            return this.domNode.value;
        }
    }

    get value(): string {
        return this.get_value();
    }

    set_value(value: string): void {
        var instance = this.getEditorInstance();
        this.domNode.value = value ?? '';
        if (this._instanceReady && instance)
            instance.setData(value ?? '');
    }

    set value(v: string) {
        this.set_value(v);
    }

    get_readOnly(): boolean {
        return !!this.domNode.getAttribute("disabled");
    }

    set_readOnly(value: boolean) {

        if (this.get_readOnly() !== value) {
            if (value) {
                this.domNode.setAttribute('disabled', 'disabled');
            }
            else {
                this.domNode.removeAttribute('disabled');
            }

            var instance = this.getEditorInstance();
            if (this._instanceReady && instance)
                instance.setReadOnly(value);
        }
    }

    static CKEditorVer = "4.22.1";
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
        if (path.endsWith('/'))
            return path;
        return path + '/';
    }

    static includeCKEditor(then: () => void): void {
        if ((window as any)['CKEDITOR']) {
            return then();
        }

        var script = document.querySelector('#CKEditorScript');
        if (script) {
            return script.addEventListener("load", then);
        }

        Fluent("script").attr('type', 'text/javascript')
            .attr('id', 'CKEditorScript')
            .on("load", then)
            .attr("async", "false")
            .attr('src', resolveUrl(HtmlContentEditor.getCKEditorBasePath() + 'ckeditor.js?v=' +
                HtmlContentEditor.CKEditorVer))
            .appendTo(document.head);
    };
}

@Decorators.registerEditor('Serenity.HtmlNoteContentEditor')
export class HtmlNoteContentEditor<P extends HtmlContentEditorOptions = HtmlContentEditorOptions> extends HtmlContentEditor<P> {

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
export class HtmlReportContentEditor <P extends HtmlContentEditorOptions = HtmlContentEditorOptions> extends HtmlContentEditor<P> {

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