import { signal } from "@serenity-is/domwise";
import { faIcon, FormValidationTexts, nsSerenity, sanitizeHtml } from "../../base";
import { isTrimmedEmpty } from "../../compat";
import { IReadOnly, IStringValue } from "../../interfaces";
import { EditorProps, EditorWidget } from "./editorwidget";

export interface HtmlTipTapEditorOptions {
    cols?: number;
    rows?: number;
}

export interface TipTapArgs {
    element: HTMLElement;
    extensions: any[];
    content?: string;
    editable?: boolean;
    textDirection?: "ltr" | "rtl" | "auto";
    injectCss?: boolean;
    injectNonce?: string;
    editorProps?: any;
    parseOptions?: any;
}

export interface TipTapExtension {
}

export interface TipTapModule {
    Editor: { new(args: TipTapArgs): TipTapInstance; };
    StarterKit?: TipTapExtension;
}

export interface TipTapInstance {
    schema: {
        spec: {
            nodes: Map<string, any>;
        }
    }
}

export class TipTapEditor<P extends HtmlTipTapEditorOptions = HtmlTipTapEditorOptions> extends EditorWidget<P>
    implements IStringValue, IReadOnly {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity, [IStringValue, IReadOnly]);

    declare readonly domNode: HTMLTextAreaElement;

    static override createDefaultElement() { return document.createElement("div"); }

    static tipTapModule: TipTapModule | (() => (TipTapModule | Promise<TipTapModule>));

    private tipTapElement: HTMLElement;

    constructor(props: EditorProps<P>) {
        super(props);

        this.element.addClass("s-HtmlContentEditor");

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

        if (TipTapEditor.tipTapModule) {
            Promise.resolve(typeof TipTapEditor.tipTapModule === "function" ?
                TipTapEditor.tipTapModule() : TipTapEditor.tipTapModule).then(tipTap => {
                    if (!tipTap?.Editor)
                        return;

                    this.tipTapElement = document.createElement('div');
                    this.tipTapElement.id = "tiptap_" + (this.domNode?.id ?? this.uniqueName);
                    textArea.parentElement!.insertBefore(this.tipTapElement, textArea);
                    textArea.style.visibility = 'hidden';
                    for (const k of textArea.className.split(' ')) {
                        k && this.tipTapElement.classList.add(k);
                    }
                    textArea.classList.add("s-offscreen");

                    const editor = new tipTap.Editor({
                        element: this.tipTapElement,
                        content: sanitizeHtml(textArea.value),
                        extensions: [tipTap.StarterKit].filter(x => x != null)
                    });

                    this.tipTapElement.prepend(TipTapToolbar({ editor }));
                });
        }
    }

    override destroy(): void {
        super.destroy();
    }

    get_value(): string {
        return null;
    }

    get value(): string {
        return this.get_value();
    }

    set_value(value: string): void {
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
        }
    }
}

export class TipTapNoteEditor extends TipTapEditor<HtmlTipTapEditorOptions> {
    constructor(props: EditorProps<HtmlTipTapEditorOptions>) {
        super(props);
    }
}

export interface TipTapToolbarHiddenOptions {
    undoRedo?: boolean;
    headings?: boolean;
    boldItalicUnderline?: boolean;
    strike?: boolean;
    blockquote?: boolean;
    link?: boolean;
    listOptions?: boolean;
    superSubScript?: boolean;
    alignment?: boolean;
    image?: boolean;
}

export function TipTapToolbar(props: {
    editor: any;
    hidden?: TipTapToolbarHiddenOptions;
}) {
    const hidden = props?.hidden ?? {};
    const editor = props.editor;
    const undoDisabled = signal<boolean>(!editor?.can().undo());
    const redoDisabled = signal<boolean>(!editor?.can().redo());

    function updateSignals() {
        undoDisabled.value = !editor.can().undo();
        redoDisabled.value = !editor.can().redo();
    }

    updateSignals();
    editor.on('transaction', () => {
        updateSignals();
    });

    return (
        <div role="toolbar" title="toolbar" data-variant="fixed" class="btn-toolbar">
            <div style="flex:1"></div>
            <div role="group" class="btn-group me-2" hidden={hidden.undoRedo}>
                <button class="btn btn-outline-light" title="Undo" type="button" tabindex="-1" 
                    disabled={undoDisabled} onClick={() => editor?.commands?.undo?.()}>
                    <i class={faIcon("undo")}></i>
                </button>
                <button class="btn btn-outline-light" title="Redo" type="button" tabindex="-1" 
                    disabled={redoDisabled} onClick={() => editor?.commands?.redo?.()}>
                    <i class={faIcon("redo")}></i>
                </button>
            </div>
            <div role="group" class="btn-group me-2" hidden={hidden.headings && hidden.blockquote && hidden.listOptions}>
                <button class="btn btn-outline-light" hidden={hidden.headings || !isNodeInSchema("heading", editor)} title="Format text as heading" type="button" tabindex="-1" aria-haspopup="menu">
                    <i class={faIcon("heading")}></i>
                </button>
                <button class="btn btn-outline-light" hidden={hidden.listOptions || !isNodeInSchema("list", editor)} title="List options" type="button" tabindex="-1" aria-haspopup="menu">
                    <i class={faIcon("list-ul")}></i>
                </button>
                <button class="btn btn-outline-light" hidden={hidden.blockquote || !isNodeInSchema("blockquote", editor)} title="Blockquote" type="button" tabindex="-1">
                    <i class={faIcon("quote-right")}></i>
                </button>
            </div>
            <div class="tiptap-separator" data-orientation="vertical" role="none"></div><div role="group" class="btn-group me-2">
                <button class="btn btn-outline-light" title="Bold" type="button" tabindex="-1">
                    <i class={faIcon("bold")}></i>
                </button>
                <button class="btn btn-outline-light" title="Italic" type="button" tabindex="-1">
                    <i class={faIcon("italic")}></i>
                </button>
                <button class="btn btn-outline-light" title="Underline" type="button" tabindex="-1">
                    <i class={faIcon("underline")}></i>
                </button>
                <button class="btn btn-outline-light" title="Strike" type="button" tabindex="-1">
                    <i class={faIcon("strikethrough")}></i>
                </button>
            </div>
            <div class="btn-group me-2">
                <button class="btn btn-outline-light" title="Link" type="button" tabindex="-1" aria-haspopup="dialog" aria-controls="radix-_R_3manpfiv5ubrb_">
                    <i class={faIcon("link")}></i>
                </button></div><div class="tiptap-separator" data-orientation="vertical" role="none"></div><div role="group" class="btn-group me-2">

                <button class="btn btn-outline-light" title="Superscript" type="button" tabindex="-1">
                    <i class={faIcon("superscript")}></i>
                </button>

                <button class="btn btn-outline-light" title="Subscript" type="button" tabindex="-1">
                    <i class={faIcon("subscript")}></i>
                </button>
            </div>

            <div class="tiptap-separator" data-orientation="vertical" role="none"></div>

            <div role="group" class="btn-group me-2">
                <button class="btn btn-outline-light" title="Align left" type="button" tabindex="-1">
                    <i class={faIcon("align-left")}></i>
                </button>

                <button class="btn btn-outline-light" title="Align center" type="button" tabindex="-1">
                    <i class={faIcon("align-center")}></i>
                </button>

                <button class="btn btn-outline-light" title="Align right" type="button" tabindex="-1">
                    <i class={faIcon("align-right")}></i>
                </button>

                <button class="btn btn-outline-light" title="Align justify" type="button" tabindex="-1">
                    <i class={faIcon("align-justify")}></i>
                </button></div><div class="tiptap-separator" data-orientation="vertical" role="none"></div><div role="group" class="btn-group">

                <button class="btn btn-outline-light" title="Add image" type="button" tabindex="-1">
                    <i class={faIcon("image")}></i>
                </button>
            </div>
            <div style="flex:1"></div>
        </div>)
}

function TipTapButton(props: {
    title: string;
    icon: string;
    onClick: () => void;
}) {
    return (
        <button class="btn btn-outline-light" title={props.title} type="button" tabindex="-1" onClick={e => {
            e.preventDefault();
            props.onClick();
        }}>
            <i class={props.icon}></i>
        </button>
    );
}

/**
 * Checks if a node exists in the editor schema
 * @param nodeName - The name of the node to check
 * @param editor - The editor instance
 * @returns boolean indicating if the node exists in the schema
 */
const isNodeInSchema = (
    nodeName: string,
    editor: TipTapInstance | null
): boolean => {
    if (!editor?.schema) return false
    return editor.schema.spec.nodes.get(nodeName) !== undefined
}