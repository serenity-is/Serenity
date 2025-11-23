import { bindThis } from "@serenity-is/domwise";
import { Config, Fluent, FormValidationTexts, nsSerenity, resolveUrl, sanitizeHtml, Validator } from "../../base";
import { isTrimmedEmpty } from "../../compat";
import { IReadOnly, IStringValue } from "../../interfaces";
import { LazyLoadHelper } from "../helpers/lazyloadhelper";
import { EditorProps, EditorWidget } from "./editorwidget";
import { defaultTiptapFileHandlerConfig, getAllTiptapExtensions, getTiptapContent, TiptapToolbar, type TiptapModule, type TiptapToolbarHiddenOption } from "./htmlcontenteditor-tiptap";

export type HtmlContentEditorProvider = "ckeditor" | "tiptap";

export type { TiptapModule, TiptapToolbarHiddenOption } from "./htmlcontenteditor-tiptap";

export interface HtmlContentEditorOptions {
    cols?: number;
    rows?: number;
    editorProvider?: HtmlContentEditorProvider;
}

export interface CKEditorConfig {
}

export class HtmlContentEditor<P extends HtmlContentEditorOptions = HtmlContentEditorOptions> extends EditorWidget<P>
    implements IStringValue, IReadOnly {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity, [IStringValue, IReadOnly]);

    declare private _ckInstanceReady: boolean;
    declare readonly domNode: HTMLTextAreaElement;
    static tiptapModule: TiptapModule | (() => (TiptapModule | Promise<TiptapModule>));
    declare private tiptapEditor: any;
    declare private tiptapElement: HTMLElement;

    static override createDefaultElement() { return document.createElement("textarea"); }

    static defaultEditorProvider: HtmlContentEditorProvider;

    static readonly defaultOptions: Partial<HtmlContentEditorOptions> = {
    };

    constructor(props: EditorProps<P>) {
        super({
            editorProvider: HtmlContentEditor.defaultEditorProvider,
            ...HtmlContentEditor.defaultOptions,
            ...props
        });

        this.element.addClass("s-HtmlContentEditor");
        this._ckInstanceReady = false;

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
                    return FormValidationTexts.Required;
            }

            return null;
        });

        if (HtmlContentEditor.tiptapModule && this.options.editorProvider === "tiptap") {
            Promise.resolve(typeof HtmlContentEditor.tiptapModule === "function" ?
                HtmlContentEditor.tiptapModule() : HtmlContentEditor.tiptapModule).then((tiptap: TiptapModule) => {
                    if (!tiptap?.Editor)
                        return;

                    this.tiptapElement = document.createElement('div');
                    this.tiptapElement.id = "tiptap_" + (this.domNode?.id ?? this.uniqueName);
                    this.tiptapElement.dataset.editorProvider = "tiptap";
                    textArea.parentElement!.insertBefore(this.tiptapElement, textArea);
                    textArea.style.visibility = 'hidden';
                    for (const k of textArea.className.split(' ')) {
                        k && this.tiptapElement.classList.add(k);
                    }
                    textArea.classList.add("s-offscreen");

                    const extensions = this.getTiptapExtensions(tiptap)
                        .filter(ext => ext != null)
                        .map(ext => this.configureTiptapExtension(ext))
                        .filter(ext => ext != null);

                    this.tiptapEditor = new tiptap.Editor({
                        element: this.tiptapElement,
                        content: sanitizeHtml(textArea.value),
                        extensions: extensions,
                        injectNonce: Config.cspNonce
                    });

                    const toolbar = this.createTiptapToolbar(this.tiptapEditor, this.getTiptapToolbarHidden(this.tiptapEditor));
                    this.tiptapElement.prepend(toolbar);

                    this.tiptapEditor.on('update', () => {
                        const html = getTiptapContent(this.tiptapEditor);
                        textArea.value = sanitizeHtml(html);
                        Fluent.trigger(textArea, "change");
                    });

                    this.domNode.dataset.editorProvider = "tiptap";
                });
        }
        else if (this.options.editorProvider !== "tiptap") {
            HtmlContentEditor.includeCKEditor(() => {
                LazyLoadHelper.executeOnceWhenShown(this.domNode, () => {
                    if ((window as any)['CKEDITOR']) {
                        const config = (this as any).getConfig();
                        (window as any)['CKEDITOR'].replace(id, config);
                        this.domNode.dataset.editorProvider = "ckeditor";
                    }
                });
            });
        }
    }

    protected handleCKInstanceReady(x: any): void {
        this._ckInstanceReady = true;

        x.editor?.container?.$ && (x.editor.container.$.dataset.editorProvider = "ckeditor");
        this.domNode.classList.forEach((clss) => { x.editor.container.$?.classList.add(clss); });
        this.domNode.classList.add('select2-offscreen');
        this.domNode.style.display = 'block';

        // for validation to work
        x.editor.setData(this.domNode.value);
        x.editor.setReadOnly(this.get_readOnly());
    }

    protected handleCKEditorChange(e: any): void {
        e.editor.updateElement();
        Fluent.trigger(this.domNode, "change");

        if (this.triggerKeyupEvent) {
            const validator = Validator.getInstance(this.domNode.form);
            if (validator && validator?.settings?.onkeyup) {
                // trigger validation on keyup
                validator.settings.onkeyup(this.domNode, this.triggerKeyupEvent, validator)
            }
            this.triggerKeyupEvent = null;
        }
    }

    protected handleCKKey(e: any): void {
        this.triggerKeyupEvent = e?.data?.domEvent?.$;
    }

    protected getCKEditorLanguage(): string {
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

    private triggerKeyupEvent: KeyboardEvent;

    /** @deprecated Override and use getCKEditorConfig() */
    protected getConfig(): CKEditorConfig {
        return this.getCKEditorConfig();
    }

    protected getCKEditorConfig(): CKEditorConfig {
        const boundThis = bindThis(this);

        return {
            customConfig: '',
            language: this.getCKEditorLanguage(),
            bodyClass: 's-HtmlContentBody',
            versionCheck: false,
            on: {
                instanceReady: boundThis.handleCKInstanceReady,
                change: boundThis.handleCKEditorChange,
                key: boundThis.handleCKKey
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

    protected getCKEditorInstance() {
        const id = this.domNode.getAttribute("id");
        return (window as any)['CKEDITOR']?.instances?.[id];
    }

    protected configureTiptapExtension(extension: any): any {
        if (!extension || !extension.name)
            return extension;

        switch (extension.name) {
            case "fileHandler":
                return extension.configure(defaultTiptapFileHandlerConfig());

            case "textAlign":
                return extension.configure({
                    types: ["paragraph", "heading"],
                    alignments: ["left", "center", "right", "justify"]
                });
        }
        return extension;
    }

    protected getTiptapExtensions(tiptap: TiptapModule): any[] {
        return getAllTiptapExtensions(tiptap);
    }

    protected createTiptapToolbar(editor: any, hidden: TiptapToolbarHiddenOption): HTMLElement {
        return <TiptapToolbar editor={editor} hidden={hidden} /> as HTMLElement;
    }

    /** Can be overridden to hide some buttons even though they are registered in extensions */
    protected getTiptapToolbarHidden(editor: any): TiptapToolbarHiddenOption {
        return {
        };
    }

    override destroy(): void {
        if (this.editorProvider === "ckeditor") {
            const ckInstance = this.getCKEditorInstance();
            ckInstance && ckInstance.destroy(true);
        }
        else if (this.tiptapEditor) {
            this.tiptapEditor.destroy();
            delete this.tiptapEditor;
        }
        super.destroy();
    }

    get_value(): string {
        if (this.tiptapEditor) {
            return sanitizeHtml(getTiptapContent(this.tiptapEditor));
        }
        else if (this.editorProvider === "ckeditor") {
            const ckInstance = this.getCKEditorInstance();
            if (this._ckInstanceReady && ckInstance) {
                return sanitizeHtml(ckInstance.getData());
            }
        }
        return sanitizeHtml(this.domNode.value);
    }

    get value(): string {
        return this.get_value();
    }

    set_value(value: string): void {
        value = sanitizeHtml(value ?? '');

        this.domNode.value = value;

        if (this.tiptapEditor) {
            this.tiptapEditor?.commands.setContent(value);
            return;
        }

        if (this.editorProvider === "ckeditor") {
            const ckInstance = this.getCKEditorInstance();
            if (this._ckInstanceReady && ckInstance)
                ckInstance.setData(value);
        }
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

            if (this.tiptapEditor) {
                this.tiptapEditor.setEditable(!value);
                return;
            }

            if (this.editorProvider !== "ckeditor") {
                const ckInstance = this.getCKEditorInstance();
                if (this._ckInstanceReady && ckInstance)
                    ckInstance.setReadOnly(value);
            }
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
                return "https://cdnjs.cloudflare.com/ajax/libs/ckeditor/4.22.1/";
        }
        return path.endsWith('/') ? path : path + '/';
    }

    static includeCKEditor(then: () => void): void {
        if ((window as any)['CKEDITOR']) {
            return then();
        }

        var script = document.querySelector('#CKEditorScript');
        if (script) {
            return script.addEventListener("load", then);
        }
        const nonce =
            document.head?.querySelector('meta[name="csp-nonce"]')?.getAttribute('content') ??
            document.head?.querySelector('script[nonce]')?.getAttribute('nonce') ??
            document.head?.querySelector('style[nonce]')?.getAttribute('nonce');

        document.head.appendChild(
            <script type="text/javascript" id="CKEditorScript" async={false} onLoad={then} nonce={nonce}
                src={resolveUrl(HtmlContentEditor.getCKEditorBasePath() + 'ckeditor.js?v=' + HtmlContentEditor.CKEditorVer)}>
            </script>);
    };

    get editorProvider(): HtmlContentEditorProvider {
        return this.domNode?.dataset?.editorProvider as HtmlContentEditorProvider ?? null;
    }
}

/** Html content editor variant for notes with limited toolbar options, e.g. undo redo and bold / italic / underline for now */
export class HtmlNoteContentEditor<P extends HtmlContentEditorOptions = HtmlContentEditorOptions> extends HtmlContentEditor<P> {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity);

    constructor(props: EditorProps<P>) {
        super({
            editorProvider: HtmlNoteContentEditor.defaultEditorProvider,
            ...props,
        });
    }

    protected override getCKEditorConfig(): CKEditorConfig {
        var config = super.getCKEditorConfig();
        (config as any).removeButtons += ',Cut,Copy,Paste,BulletedList,NumberedList,' +
            'Indent,Outdent,SpecialChar,Subscript,Superscript,Styles,' +
            'PasteText,PasteFromWord,Strike,Link,Unlink,CreatePlaceholder,' +
            'Image,Table,HorizontalRule,Source,Maximize,Format,Font,FontSize,' +
            'Anchor,Blockquote,CreatePlaceholder,BGColor,JustifyLeft,JustifyCenter,' +
            'JustifyRight,JustifyBlock,Superscript,RemoveFormat';

        (config as any).removePlugins = 'elementspath,uploadimage,image2';
        return config;
    }

    protected override configureTiptapExtension(extension: any): any {
        extension = super.configureTiptapExtension(extension);

        switch (extension?.name) {
            case "starterKit":
                extension = extension.configure({
                    blockquote: false,
                    bulletList: false,
                    code: false,
                    codeBlock: false,
                    heading: false,
                    horizontalRule: false,
                    listItem: false,
                    listKeymap: false,
                    link: false,
                    orderedList: false,
                    strike: false
                });
                break;
        }

        return extension;
    }

    protected override getTiptapExtensions(tiptap: TiptapModule): any[] {
        return [tiptap.StarterKit];
    }
}

/** 
 * This is originally was intended to be a subset more compatible with reports,
 * which was necessary as most report rendering engines had limited HTML/CSS support. 
 * We will revisit this if needed in future.
 */
export class HtmlReportContentEditor<P extends HtmlContentEditorOptions = HtmlContentEditorOptions> extends HtmlContentEditor<P> {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity);

    constructor(props: EditorProps<P>) {
        super({
            editorProvider: HtmlReportContentEditor.defaultEditorProvider,
            ...props,
        });
    }

    protected override getCKEditorConfig(): CKEditorConfig {
        var config = super.getCKEditorConfig();
        (config as any).removeButtons += ',Cut,Copy,Paste,BulletedList,NumberedList,' +
            'Indent,Outdent,SpecialChar,Subscript,Superscript,Styles,' +
            'PasteText,PasteFromWord,Strike,Link,Unlink,CreatePlaceholder,' +
            'Image,Table,HorizontalRule,Source,Maximize,Format,Font,FontSize,' +
            'Anchor,Blockquote,CreatePlaceholder,BGColor,' +
            'JustifyBlock,Superscript,RemoveFormat';

        (config as any).removePlugins = 'elementspath,uploadimage,image2';
        return config;
    }

    protected override getTiptapToolbarHidden(editor: any): TiptapToolbarHiddenOption {
        return {
            alignmentJustify: true
        };
    }

    protected override configureTiptapExtension(extension: any): any {
        extension = super.configureTiptapExtension(extension);

        switch (extension?.name) {
            case "starterKit":
                extension = extension.configure({
                    blockquote: false,
                    bulletList: false,
                    code: false,
                    codeBlock: false,
                    heading: false,
                    horizontalRule: false,
                    listItem: false,
                    listKeymap: false,
                    link: false,
                    orderedList: false,
                    strike: false
                });

            case "textAlign":
                extension = extension.configure({
                    alignments: ["left", "center", "right"]
                });
                break;
        }

        return extension;
    }
}

