import { Validator } from "../../base";
import { HtmlContentEditor, HtmlNoteContentEditor, HtmlReportContentEditor } from "./htmlcontenteditor";

class TestHtmlContentEditor extends HtmlContentEditor<any> {
    protected override getTiptapExtensions() {
        return [];
    }

    protected override createTiptapToolbar() {
        return document.createElement("div");
    }

    getConfigForTest() {
        return this.getCKEditorConfig();
    }

    getConfigLegacy() {
        return this.getConfig();
    }

    getLanguageForTest() {
        return this.getCKEditorLanguage();
    }

    configureForTest(extension: any) {
        return this.configureTiptapExtension(extension);
    }
}

describe("HtmlContextEditor", () => {
    it('getCKEditorBasePath returns CDNJS url by default', function () {
        expect(HtmlContentEditor.getCKEditorBasePath()).toBe("https://cdnjs.cloudflare.com/ajax/libs/ckeditor/4.22.1/");
    });

    it('getCKEditorBasePath returns CKEDITOR_BASEPATH if set', function () {
        (window as any)["CKEDITOR_BASEPATH"] = "/ceb/";
        try {
            expect(HtmlContentEditor.getCKEditorBasePath()).toBe('/ceb/');
        }
        finally {
            delete (window as any)["CKEDITOR_BASEPATH"];
        }
    });

    it('getCKEditorBasePath returns HtmlContentEditor.CKEditorBasePath if set', function () {
        HtmlContentEditor.CKEditorBasePath = '~/myck/';
        try {
            expect(HtmlContentEditor.getCKEditorBasePath()).toBe('~/myck/');
        }
        finally {
            delete HtmlContentEditor.CKEditorBasePath;
        }
    });

    it('getCKEditorBasePath gives priority to HtmlContentEditor.CKEditorBasePath', function () {
        HtmlContentEditor.CKEditorBasePath = '~/myck2/';
        (window as any)["CKEDITOR_BASEPATH"] = "/ceb/";
        try {
            expect(HtmlContentEditor.getCKEditorBasePath()).toBe('~/myck2/');
        }
        finally {
            delete HtmlContentEditor.CKEditorBasePath;
            delete (window as any)["CKEDITOR_BASEPATH"];
        }
    });

    it('getCKEditorBasePath adds trailing slash to result for global var', function () {
        (window as any)["CKEDITOR_BASEPATH"] = "/ceb";
        try {
            expect(HtmlContentEditor.getCKEditorBasePath()).toBe('/ceb/');
        }
        finally {
            delete (window as any)["CKEDITOR_BASEPATH"];
        }
    });

    it('getCKEditorBasePath adds trailing slash to result for static member', function () {
        HtmlContentEditor.CKEditorBasePath = '~/myck';
        try {
            expect(HtmlContentEditor.getCKEditorBasePath()).toBe('~/myck/');
        }
        finally {
            delete HtmlContentEditor.CKEditorBasePath;
        }
    });

    it("initializes, synchronizes, and destroys a Tiptap editor", async () => {
        const updates: Record<string, () => void> = {};
        const tiptapEditor: any = {
            commands: { setContent: vi.fn() },
            isEditable: true,
            getJSON: () => ({ content: [{ content: [{ text: "content" }] }] }),
            getHTML: () => "<p>content</p>",
            on: vi.fn((event: string, callback: () => void) => updates[event] = callback),
            setEditable: vi.fn(),
            destroy: vi.fn()
        };
            const FakeEditor = function (this: any, options: any) {
                Object.assign(tiptapEditor, { options });
                return tiptapEditor;
            };
        const previousModule = HtmlContentEditor.tiptapModule;
        HtmlContentEditor.tiptapModule = { Editor: FakeEditor } as any;
        const textarea = document.createElement("textarea");
        textarea.value = "<p>initial</p>";
        document.body.appendChild(textarea);

        try {
            const editor: any = new TestHtmlContentEditor({ element: textarea, editorProvider: "tiptap" });
            await Promise.resolve();
            expect(textarea.dataset.editorProvider).toBe("tiptap");
            expect(editor.tiptapEditor.options.content).toBe("<p>initial</p>");
            expect(editor.get_value()).toBe("<p>content</p>");

            editor.set_value("<p>updated</p>");
            expect(textarea.value).toBe("<p>updated</p>");
            expect(tiptapEditor.commands.setContent).toHaveBeenCalledWith("<p>updated</p>");
            editor.set_readOnly(true);
            expect(textarea.disabled).toBe(true);
            expect(tiptapEditor.setEditable).toHaveBeenCalledWith(false);
            editor.set_readOnly(false);
            expect(textarea.disabled).toBe(false);
            updates.update();
            expect(editor.get_value()).toBe("<p>content</p>");
            editor.destroy();
            expect(tiptapEditor.destroy).toHaveBeenCalled();
        }
        finally {
            HtmlContentEditor.tiptapModule = previousModule;
            textarea.remove();
        }
    });

    it("builds CKEditor configuration and resolves language fallbacks", () => {
        const textarea = document.createElement("textarea");
        const editor: any = Object.create(TestHtmlContentEditor.prototype);
        editor.domNode = textarea;
        editor.options = { rows: 4 };
        const config: any = editor.getConfigForTest();
        expect(config.language).toBe("en");
        expect(config.height).toBe("80px");
        expect(config.on.instanceReady).toBeTypeOf("function");
        expect(config.contentsCss).toContain("site.htmlcontent.css");

        const previousCKEditor = (window as any).CKEDITOR;
        (window as any).CKEDITOR = { lang: { languages: { "tr": {}, "en": {} } } };
        try {
            document.documentElement.setAttribute("lang", "tr-TR");
            expect(editor.getLanguageForTest()).toBe("tr");
            document.documentElement.setAttribute("lang", "de");
            expect(editor.getLanguageForTest()).toBe("en");
        }
        finally {
            (window as any).CKEDITOR = previousCKEditor;
            document.documentElement.removeAttribute("lang");
        }
    });

    it("loads CKEditor through existing and newly created scripts", () => {
        const previousCKEditor = (window as any).CKEDITOR;
        const callback = vi.fn();
        (window as any).CKEDITOR = {};
        HtmlContentEditor.includeCKEditor(callback);
        expect(callback).toHaveBeenCalledTimes(1);
        delete (window as any).CKEDITOR;

        const existing = document.createElement("script");
        existing.id = "CKEditorScript";
        document.head.appendChild(existing);
        HtmlContentEditor.includeCKEditor(callback);
        existing.dispatchEvent(new Event("load"));
        expect(callback).toHaveBeenCalledTimes(2);
        existing.remove();

        HtmlContentEditor.includeCKEditor(callback);
        const created = document.querySelector<HTMLScriptElement>("#CKEditorScript")!;
        expect(created.src).toContain("ckeditor.js?v=4.22.1");
        created.remove();
        (window as any).CKEDITOR = previousCKEditor;
    });

    it("applies note and report editor configuration overrides", () => {
        const note: any = Object.create(HtmlNoteContentEditor.prototype);
        note.options = { rows: 0 };
        note.domNode = document.createElement("textarea");
        const noteConfig: any = note.getCKEditorConfig();
        expect(noteConfig.removeButtons).toContain("Cut,Copy,Paste");
        expect(noteConfig.removePlugins).toBe("elementspath,uploadimage,image2");

        const report: any = Object.create(HtmlReportContentEditor.prototype);
        report.options = { rows: 0 };
        report.domNode = document.createElement("textarea");
        const reportConfig: any = report.getCKEditorConfig();
        expect(reportConfig.removeButtons).toContain("JustifyBlock");
        expect(reportConfig.removePlugins).toBe("elementspath,uploadimage,image2");
        expect(report.getTiptapToolbarHidden(null).alignmentJustify).toBe(true);
        expect(report.configureTiptapExtension({ name: "textAlign", configure: vi.fn(() => "configured") })).toBe("configured");
    });

    it("creates the default textarea element and delegates config", () => {
        expect(HtmlContentEditor.createDefaultElement().tagName).toBe("TEXTAREA");
        const editor: any = Object.create(TestHtmlContentEditor.prototype);
        editor.domNode = document.createElement("textarea");
        editor.options = {};
        expect(editor.getConfigLegacy()).toEqual(editor.getCKEditorConfig());
    });

    it("configures the CKEditor constructor path and validates required", () => {
        const replaceSpy = vi.fn();
        (window as any).CKEDITOR = { replace: replaceSpy, instances: {}, lang: { languages: { en: {} } } };
        const textarea = document.createElement("textarea");
        textarea.name = "HtmlContent";
        Object.defineProperty(textarea, "offsetWidth", { value: 100, configurable: true });
        Object.defineProperty(textarea, "offsetHeight", { value: 100, configurable: true });
        const editor = new HtmlContentEditor({ element: textarea, cols: 40, rows: 5 } as any);
        expect(textarea.getAttribute("cols")).toBe("40");
        expect(textarea.getAttribute("rows")).toBe("5");
        expect(replaceSpy).toHaveBeenCalled();
        expect(textarea.dataset.editorProvider).toBe("ckeditor");

        const form = document.createElement("form");
        form.appendChild(textarea);
        document.body.appendChild(form);
        const validator = new Validator(form, {});
        textarea.classList.add("required");
        textarea.value = "";
        expect(validator.element(textarea)).toBe(false);
        textarea.value = "<p>x</p>";
        expect(validator.element(textarea)).toBe(true);
        editor.destroy();
        delete (window as any).CKEDITOR;
        form.remove();
    });

    it("handles CKEditor instance ready, change, and key events", () => {
        const editor: any = Object.create(HtmlContentEditor.prototype);
        editor.domNode = document.createElement("textarea");
        editor.domNode.classList.add("s-HtmlContentBody");
        editor.get_readOnly = () => false;
        editor.triggerKeyupEvent = null;
        const container = document.createElement("div");
        const fakeEditor = { container: { $: container }, setData: vi.fn(), setReadOnly: vi.fn(), updateElement: vi.fn() };
        editor.handleCKInstanceReady({ editor: fakeEditor });
        expect(editor._ckInstanceReady).toBe(true);
        expect(fakeEditor.setData).toHaveBeenCalledWith("");
        expect(fakeEditor.setReadOnly).toHaveBeenCalledWith(false);

        const keyEvent = { data: { domEvent: { $: {} } } };
        editor.handleCKKey(keyEvent);
        expect(editor.triggerKeyupEvent).toBe(keyEvent.data.domEvent.$);

        const form = document.createElement("form");
        form.appendChild(editor.domNode);
        document.body.appendChild(form);
        const onkeyup = vi.fn();
        new Validator(form, { onkeyup } as any);
        editor.handleCKEditorChange({ editor: fakeEditor });
        expect(fakeEditor.updateElement).toHaveBeenCalled();
        expect(onkeyup).toHaveBeenCalled();
        expect(editor.triggerKeyupEvent).toBeNull();
        form.remove();
    });

    it("reads, writes, and destroys through a fake CKEditor instance", () => {
        const textarea = document.createElement("textarea");
        textarea.id = "ed1";
        textarea.value = "<p>html</p>";
        const ckInstance = { getData: vi.fn(() => "<p>data</p>"), setData: vi.fn(), setReadOnly: vi.fn(), destroy: vi.fn() };
        (window as any).CKEDITOR = { instances: { ed1: ckInstance }, lang: { languages: { en: {} } } };
        const editor: any = Object.create(HtmlContentEditor.prototype);
        editor.domNode = textarea;
        editor.options = {};
        editor._ckInstanceReady = true;
        editor.domNode.dataset.editorProvider = "ckeditor";
        expect(editor.get_value()).toBe("<p>data</p>");
        editor.set_value("<p>new</p>");
        expect(ckInstance.setData).toHaveBeenCalledWith("<p>new</p>");
        editor.destroy();
        expect(ckInstance.destroy).toHaveBeenCalledWith(true);
        delete (window as any).CKEDITOR;
    });

    it("applies readonly through a fake CKEditor instance", () => {
        const textarea = document.createElement("textarea");
        textarea.id = "ed2";
        const ckInstance = { setReadOnly: vi.fn() };
        (window as any).CKEDITOR = { instances: { ed2: ckInstance }, lang: { languages: { en: {} } } };
        const editor: any = Object.create(HtmlContentEditor.prototype);
        editor.domNode = textarea;
        editor.options = {};
        editor._ckInstanceReady = true;
        editor.domNode.dataset.editorProvider = "ckeditor";
        expect(editor.get_readOnly()).toBe(false);
        editor.set_readOnly(true);
        expect(ckInstance.setReadOnly).toHaveBeenCalledWith(true);
        expect(textarea.hasAttribute("disabled")).toBe(true);
        editor.set_readOnly(false);
        expect(ckInstance.setReadOnly).toHaveBeenCalledWith(false);
        delete (window as any).CKEDITOR;
    });

    it("falls back to the textarea value without a provider", () => {
        const textarea = document.createElement("textarea");
        textarea.value = "<p>fallback</p>";
        const editor: any = Object.create(HtmlContentEditor.prototype);
        editor.domNode = textarea;
        expect(editor.get_value()).toBe("<p>fallback</p>");
        expect(editor.value).toBe("<p>fallback</p>");
        editor.value = "<p>via-prop</p>";
        expect(textarea.value).toBe("<p>via-prop</p>");
    });

    it("exposes base extension helpers and validates required content", () => {
        const editor: any = Object.create(HtmlContentEditor.prototype);
        const extension = { type: "extension" };
        const mark = { type: "mark" };
        expect(editor.getTiptapExtensions({ StarterKit: extension, Bold: mark })).toEqual([extension, mark]);
        const plain = { name: "plain" };
        expect(editor.configureTiptapExtension(plain)).toBe(plain);

        const form = document.createElement("form");
        const textarea = document.createElement("textarea");
        textarea.name = "Content";
        form.appendChild(textarea);
        document.body.appendChild(form);
        const htmlEditor = new TestHtmlContentEditor({ element: textarea } as any);
        const validator = new Validator(form, {});
        textarea.classList.add("required");
        textarea.value = "";
        expect(validator.element(textarea)).toBe(false);
        textarea.value = "<p>ok</p>";
        expect(validator.element(textarea)).toBe(true);
        htmlEditor.destroy();
        form.remove();
    });

    it("constructs note and report editor instances", () => {
        const noteTextarea = document.createElement("textarea");
        document.body.appendChild(noteTextarea);
        const note = new HtmlNoteContentEditor({ element: noteTextarea } as any);
        expect(note.domNode).toBe(noteTextarea);
        note.destroy();

        const reportTextarea = document.createElement("textarea");
        document.body.appendChild(reportTextarea);
        const report = new HtmlReportContentEditor({ element: reportTextarea } as any);
        expect(report.domNode).toBe(reportTextarea);
        report.destroy();
        noteTextarea.remove();
        reportTextarea.remove();
    });

    it("exposes the ckeditor instance and resolves exact language keys", () => {
        const textarea = document.createElement("textarea");
        textarea.id = "x";
        const ckInstance = {};
        (window as any).CKEDITOR = { instances: { x: ckInstance }, lang: { languages: { en: {}, tr: {} } } };
        const editor: any = Object.create(TestHtmlContentEditor.prototype);
        editor.domNode = textarea;
        editor.options = {};
        expect(editor.getCKEditorInstance()).toBe(ckInstance);
        document.documentElement.setAttribute("lang", "tr");
        expect(editor.getLanguageForTest()).toBe("tr");
        document.documentElement.removeAttribute("lang");
        delete (window as any).CKEDITOR;
    });

    it("configures note and report tiptap extensions", () => {
        const note: any = Object.create(HtmlNoteContentEditor.prototype);
        const kit = { name: "starterKit", configure: vi.fn(() => "configured") };
        expect(note.configureTiptapExtension(kit)).toBe("configured");
        expect(note.getTiptapExtensions({ StarterKit: "SK" })).toEqual(["SK"]);
        const report: any = Object.create(HtmlReportContentEditor.prototype);
        expect(report.configureTiptapExtension({ name: "starterKit", configure: vi.fn(() => ({ configure: vi.fn(() => ({})) })) })).toBeDefined();
        expect(report.configureTiptapExtension({ name: "textAlign", configure: vi.fn(() => ({ configure: vi.fn(() => ({})) })) })).toBeDefined();
    });

    it("builds a tiptap toolbar through the base editor", () => {
        const editor: any = Object.create(HtmlContentEditor.prototype);
        const fakeTiptap: any = {
            isEditable: true,
            isActive: () => false,
            can: () => new Proxy({}, { get: () => () => true }),
            chain: () => ({ focus: () => ({ run: () => true }) }),
            extensionManager: { extensions: [{ name: "textAlign" }] },
            on: vi.fn(),
            schema: { spec: { marks: new Map(), nodes: new Map() } },
            state: { selection: { node: null, empty: true } }
        };
        const el = editor.createTiptapToolbar(fakeTiptap, {}) as HTMLElement;
        expect(el.getAttribute("role")).toBe("toolbar");
    });

    it("ignores tiptap modules without an editor", async () => {
        const previousModule = HtmlContentEditor.tiptapModule;
        HtmlContentEditor.tiptapModule = {} as any;
        const textarea = document.createElement("textarea");
        document.body.appendChild(textarea);
        const editor = new TestHtmlContentEditor({ element: textarea, editorProvider: "tiptap" } as any);
        await Promise.resolve();
        expect(editor["tiptapEditor"]).toBeUndefined();
        editor.destroy();
        HtmlContentEditor.tiptapModule = previousModule;
        textarea.remove();
    });

    it("maps tiptap extensions through the configured pipeline", async () => {
        class ExtensionEditor extends TestHtmlContentEditor {
            protected override getTiptapExtensions() {
                return [
                    { name: "fileHandler", configure: vi.fn(() => "fh") },
                    { name: "textAlign", configure: vi.fn(() => "ta") },
                    null,
                    { name: "plain" }
                ] as any;
            }
        }
        const previousModule = HtmlContentEditor.tiptapModule;
        HtmlContentEditor.tiptapModule = {
            Editor: function (this: any, options: any) {
                Object.assign(this, options);
                this.on = vi.fn();
                this.destroy = vi.fn();
            }
        } as any;
        const textarea = document.createElement("textarea");
        document.body.appendChild(textarea);
        const editor: any = new ExtensionEditor({ element: textarea, editorProvider: "tiptap" } as any);
        await Promise.resolve();
        expect(editor.tiptapEditor).toBeTruthy();
        editor.destroy();
        HtmlContentEditor.tiptapModule = previousModule;
        textarea.remove();
    });
});