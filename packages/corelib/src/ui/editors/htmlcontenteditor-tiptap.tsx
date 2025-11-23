import { type Computed, type PropValue, useUpdatableComputed } from "@serenity-is/domwise";
import { faIcon, HtmlContentEditorTexts } from "../../base";

/* To avoid importing tiptap dependencies in corelib, we define minimal interfaces here */
export interface TiptapModule {
    Editor: any;
    StarterKit?: any;
    TextAlign?: any;
}

/* Internal TiptapModule typing */
export interface TiptapModuleInternal {
    Editor: { new(args: TiptapEditorArgs): TiptapEditor; };
    StarterKit?: { configure: (options: StarterKitOptions) => any; };
    TextAlign?: any;
}

export interface TiptapToolbarHiddenOption {
    alignment?: boolean;
    alignmentJustify?: boolean;
    blockquote?: boolean;
    boldItalicUnderline?: boolean;
    inlineCode?: boolean;
    headings?: boolean;
    image?: boolean;
    link?: boolean;
    listOptions?: boolean;
    strike?: boolean;
    superSubScript?: boolean;
    undoRedo?: boolean;
}

function TiptapButton(props: {
    title: string,
    active?: PropValue<boolean>,
    icon: string,
    onClick?: () => void,
    hidden?: PropValue<boolean>,
    disabled?: PropValue<boolean>
}) {
    return (
        <button class={{ "btn btn-link": true, active: props.active }} hidden={props.hidden} disabled={props.disabled}
            title={props.title} type="button" tabindex="-1" onClick={e => {
                e.preventDefault();
                props.onClick();
            }}>
            <i class={props.icon}></i>
        </button>
    );
}

interface TiptapEditorArgs {
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

interface TiptapEditorChain {
    focus: () => TiptapEditorChain,
    setTextAlign: (align: TextAlign) => TiptapEditorChain,
    toggleTextAlign: (align: TextAlign) => TiptapEditorChain,
    toggleMark: (mark: string) => TiptapEditorChain,
    undo: () => TiptapEditorChain,
    redo: () => TiptapEditorChain,
    run: () => boolean
}

interface TiptapEditor {
    isActive: (name: string | { [key: string]: any }) => boolean,
    isEditable: boolean,
    can: () => {
        [command: string]: (arg?: string) => boolean;
    },
    chain: () => TiptapEditorChain,
    extensionManager: {
        extensions: Array<{ name: string }>;
    };
    on: (event: string, callback: () => void) => void;
    off: (event: string, callback: () => void) => void;
    destroy: () => void;
    getJSON: () => any;
    getHTML: () => string;
    schema: {
        spec: {
            marks: Map<string, any>;
            nodes: Map<string, any>;
        }
    },
    state: {
        selection: {
            node: any;
            empty: boolean;
        }
    }
}

type Editor = TiptapEditor;

interface StarterKitOptions {
    blockquote?: object | false;
    bold?: object | false;
    bulletList?: object | false;
    code?: object | false;
    codeBlock?: object | false;
    document?: false;
    dropcursor?: object | false;
    gapcursor?: false;
    hardBreak?: object | false;
    heading?: object | false;
    undoRedo?: object | false;
    horizontalRule?: object | false;
    italic?: object | false;
    listItem?: object | false;
    listKeymap?: object | false;
    link?: object | false;
    orderedList?: object | false;
    paragraph?: object | false;
    strike?: object | false;
    text?: object | false;
    underline?: object | false;
    trailingNode?: object | false;
}

export interface TiptapToolbarHiddenOption {
    alignment?: boolean;
    alignmentJustify?: boolean;
    blockquote?: boolean;
    boldItalicUnderline?: boolean;
    inlineCode?: boolean;
    headings?: boolean;
    image?: boolean;
    link?: boolean;
    listOptions?: boolean;
    strike?: boolean;
    superSubScript?: boolean;
    undoRedo?: boolean;
}

export function TiptapToolbar(props: {
    editor: TiptapEditor;
    hidden?: TiptapToolbarHiddenOption;
}) {
    const hidden = props?.hidden ?? {};
    const editor = props.editor;

    const { computed, update } = useUpdatableComputed();

    editor.on('transaction', update);

    const boldItalicUnderlineStrike = (["bold", "italic", "underline", "strike"] as Mark[]);
    const superSubScriptCode = (["superscript", "subscript", "code"] as Mark[]);
    const alignment = (["left", "center", "right", "justify"] as TextAlign[]);

    const hiddenUndoRedo = computed(() => hidden.undoRedo || !isEditable(editor));
    
    const hiddenMark: Record<Mark, Computed<boolean>> = {
        bold: computed(() => hidden.boldItalicUnderline || !showMark(editor, "bold")),
        italic: computed(() => hidden.boldItalicUnderline || !showMark(editor, "italic")),
        underline: computed(() => hidden.boldItalicUnderline || !showMark(editor, "underline")),
        strike: computed(() => hidden.strike || !showMark(editor, "strike")),
        code: computed(() => hidden.inlineCode || !showMark(editor, "code")),
        superscript: computed(() => hidden.superSubScript || !showMark(editor, "superscript")),
        subscript: computed(() => hidden.superSubScript || !showMark(editor, "subscript"))
    };

    const hiddenLeftCenterRight = computed(() => hidden.alignment || !showTextAlign(editor));
    const hiddenJustify = computed(() => hiddenLeftCenterRight.value || hidden.alignmentJustify);
    const hiddenBoldItalicUnderlineStrike = computed(() => boldItalicUnderlineStrike.every(key => hiddenMark[key].value));
    const hiddenSuperSubScriptCode = computed(() => superSubScriptCode.every(key => hiddenMark[key].value));
    const hiddenAlignmentAll = computed(() => hiddenLeftCenterRight.value && hiddenJustify.value);

    return (
        <div role="toolbar" title="toolbar" data-variant="fixed" class="btn-toolbar">
            <div class="btn-group" hidden={hiddenUndoRedo}>
                {(["undo", "redo"] as UndoRedoAction[]).map(key =>
                    <TiptapButton title={undoRedoTexts[key]} icon={undoRedoIcons[key]}
                        disabled={computed(() => !canUndoRedo(editor, key))}
                        hidden={hiddenUndoRedo}
                        onClick={() => execUndoRedo(editor, key)} />
                )}
            </div>

            <div class="btn-group" hidden={hiddenBoldItalicUnderlineStrike}>
                {boldItalicUnderlineStrike.map(key =>
                    <TiptapButton title={markTexts[key]} icon={markIcons[key]}
                        active={computed(() => isMarkActive(editor, key))}
                        disabled={computed(() => !canToggleMark(editor, key))}
                        hidden={hiddenMark[key]}
                        onClick={() => toggleMark(editor, key)} />
                )}
            </div>

            <div class="btn-group" hidden={hiddenSuperSubScriptCode}>
                {superSubScriptCode.map(key =>
                    <TiptapButton title={markTexts[key]} icon={markIcons[key]}
                        active={computed(() => isMarkActive(editor, key))}
                        disabled={computed(() => !canToggleMark(editor, key))}
                        hidden={hiddenMark[key]}
                        onClick={() => toggleMark(editor, key)} />
                )}
            </div>

            <div class="btn-group" hidden={hiddenAlignmentAll}>
                {alignment.map(key =>
                    <TiptapButton title={textAlignTexts[key]} icon={textAlignIcons[key]}
                        active={computed(() => isTextAlignActive(editor, key))}
                        disabled={computed(() => !canSetTextAlign(editor, key))}
                        hidden={key === "justify" ? hiddenJustify : hiddenLeftCenterRight}
                        onClick={() => {
                            if (isTextAlignActive(editor, key) && editor.chain().toggleTextAlign) {
                                editor.chain().focus().toggleTextAlign(key).run();
                            }
                            else {
                                setTextAlign(editor, key);
                            }
                        }} />)}
            </div>

            {/* todo add more buttons like below
            <div class="btn-group" hidden={hidden.headings && hidden.blockquote && hidden.listOptions}>
                <TiptapButton title={HtmlContentEditorTexts.FormatTextAsHeading} icon={faIcon("heading")} hidden={hidden.headings || !isNodeInSchema("heading", editor)} />
                <TiptapButton title={HtmlContentEditorTexts.ListOptions} icon={faIcon("list-ul")} hidden={hidden.listOptions || !isNodeInSchema("list", editor)} />
                <TiptapButton title={HtmlContentEditorTexts.Blockquote} icon={faIcon("quote-right")} hidden={hidden.blockquote || !isNodeInSchema("blockquote", editor)} />
            </div>
            <div class="btn-group">
                <button class="btn btn-outline-secondary" title="Link" type="button" tabindex="-1">
                    <i class={faIcon("link")}></i>
            </div>
            <button class="btn btn-outline-secondary" title="Add image" type="button" tabindex="-1">
                <i class={faIcon("image")}></i>
            </button>
            */}
        </div>)
}

// most of the following functions are adapted from https://github.com/ueberdosis/tiptap-ui-components

function isExtensionAvailable(editor: TiptapEditor, extensionNames: string | string[]): boolean {
    if (!editor) return false;
    const names = Array.isArray(extensionNames) ? extensionNames : [extensionNames];
    return names.some((name) => editor.extensionManager.extensions.some((ext) => ext.name === name));
}

const isMarkInSchema = (markName: string, editor: TiptapEditor): boolean => {
    if (!editor?.schema) return false;
    return editor.schema.spec.marks.get(markName) !== undefined;
}

const isNodeInSchema = (nodeName: string, editor: TiptapEditor): boolean => {
    if (!editor?.schema) return false;
    return editor.schema.spec.nodes.get(nodeName) !== undefined;
}

function isNodeTypeSelected(editor: TiptapEditor, types: string[] = []): boolean {
    if (!editor || !editor.state.selection) return false

    const { state } = editor
    const { selection } = state

    if (selection.empty) return false

    if (selection.node?.type?.name) {
        const node = selection.node;
        return node ? types.includes(node.type.name) : false
    }

    return false
}

type UndoRedoAction = "undo" | "redo"

const undoRedoTexts: Record<UndoRedoAction, string> = {
    undo: HtmlContentEditorTexts.Undo,
    redo: HtmlContentEditorTexts.Redo
};

const undoRedoIcons: Record<UndoRedoAction, string> = {
    undo: faIcon("undo"),
    redo: faIcon("redo")
};

function isEditable(editor: Editor): boolean {
    return !!(editor && editor.isEditable);
}

function canUndoRedo(editor: Editor, action: UndoRedoAction): boolean {
    if (!isEditable(editor) || isNodeTypeSelected(editor, ["image"])) return false;
    return action === "undo" ? editor.can().undo() : editor.can().redo();
}

function execUndoRedo(editor: Editor, action: UndoRedoAction): boolean {
    if (!isEditable(editor) || !canUndoRedo(editor, action)) return false;
    const chain = editor.chain().focus();
    return action === "undo" ? chain.undo().run() : chain.redo().run();
}

type Mark = "bold" | "italic" | "strike" | "code" | "underline" | "superscript" | "subscript"

const markTexts: Record<Mark, string> = {
    bold: HtmlContentEditorTexts.Bold,
    italic: HtmlContentEditorTexts.Italic,
    strike: HtmlContentEditorTexts.StrikeThrough,
    code: HtmlContentEditorTexts.InlineCode,
    underline: HtmlContentEditorTexts.Underline,
    superscript: HtmlContentEditorTexts.Superscript,
    subscript: HtmlContentEditorTexts.Subscript
};

const markIcons: Record<Mark, string> = {
    bold: faIcon("bold"),
    italic: faIcon("italic"),
    strike: faIcon("strikethrough"),
    code: faIcon("code"),
    underline: faIcon("underline"),
    superscript: faIcon("superscript"),
    subscript: faIcon("subscript")
};

function canToggleMark(editor: Editor, type: Mark): boolean {
    if (!isEditable(editor) || !isMarkInSchema(type, editor) || isNodeTypeSelected(editor, ["image"])) return false;
    return editor.can().toggleMark(type);
}

function isMarkActive(editor: Editor, type: Mark): boolean {
    if (!isEditable(editor)) return false;
    return editor.isActive(type);
}

function toggleMark(editor: Editor, type: Mark): boolean {
    if (!isEditable(editor) || !canToggleMark(editor, type)) return false;
    return editor.chain().focus().toggleMark(type).run();
}

function showMark(editor: Editor, type: Mark): boolean {
    return isEditable(editor) && isMarkInSchema(type, editor);
}

type TextAlign = "left" | "center" | "right" | "justify"

const textAlignTexts: Record<TextAlign, string> = {
    left: HtmlContentEditorTexts.AlignLeft,
    center: HtmlContentEditorTexts.AlignCenter,
    right: HtmlContentEditorTexts.AlignRight,
    justify: HtmlContentEditorTexts.AlignJustify
};

const textAlignIcons: Record<TextAlign, string> = {
    left: faIcon("align-left"),
    center: faIcon("align-center"),
    right: faIcon("align-right"),
    justify: faIcon("align-justify")
};

function canSetTextAlign(editor: TiptapEditor, align: TextAlign): boolean {
    if (!isEditable(editor)) return false;
    if (!isExtensionAvailable(editor, "textAlign") || isNodeTypeSelected(editor, ["image", "horizontalRule"])) return false;
    return editor.can().setTextAlign(align);
}

function hasSetTextAlign(commands: any): boolean {
    return "setTextAlign" in commands;
}

function isTextAlignActive(editor: TiptapEditor, align: TextAlign): boolean {
    if (!isEditable(editor)) return false;
    return editor.isActive({ textAlign: align });
}

function setTextAlign(editor: TiptapEditor, align: TextAlign): boolean {
    if (!isEditable(editor) || !canSetTextAlign(editor, align)) return false;
    const chain = editor.chain().focus();
    if (hasSetTextAlign(chain)) return chain.setTextAlign(align).run();
    return false;
}

function showTextAlign(editor: TiptapEditor): boolean {
    return isEditable(editor) && isExtensionAvailable(editor, "textAlign");
}

export function isTiptapContentEmpty(content: any): boolean {
    if (content === null) return true;
    if (typeof content === 'string') return content === '';
    if (Array.isArray(content)) return content.every(item => isTiptapContentEmpty(item));
    if (!content.content) return true;
    return content.content.every((item: any) => !item.content);
}

export function getTiptapContent(editor: TiptapEditor): string {
    if (isTiptapContentEmpty(editor.getJSON())) return '';
    return editor.getHTML();
}

export function getRestExtensions(tiptap: TiptapModule) {
    return Object.entries(tiptap).filter(([k, e]) => e && 
        e !== tiptap.StarterKit && 
        e !== tiptap.TextAlign && 
        /[A-Z]/.test(k[0]) && e && 
        (e.type === "extension" || e.type === "mark" || e.type === "node"))
        .map(([_, e]) => e)
}