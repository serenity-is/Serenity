import { describe, expect, it, vi } from "vitest";
import {
    defaultTiptapFileHandlerConfig,
    getAllTiptapExtensions,
    getTiptapContent,
    isTiptapContentEmpty,
    TiptapToolbar
} from "./htmlcontenteditor-tiptap";

describe("Tiptap content helpers", () => {
    it("detects empty Tiptap JSON content", () => {
        expect(isTiptapContentEmpty(null)).toBe(true);
        expect(isTiptapContentEmpty("")).toBe(true);
        expect(isTiptapContentEmpty([])).toBe(true);
        expect(isTiptapContentEmpty([{ type: "paragraph" }])).toBe(true);
        expect(isTiptapContentEmpty({ content: [{ type: "paragraph" }] })).toBe(true);
        expect(isTiptapContentEmpty({ content: [{ content: [{ text: "value" }] }] })).toBe(false);
    });

    it("returns empty or serialized Tiptap content", () => {
        const emptyEditor = { getJSON: () => ({ content: [{ type: "paragraph" }] }), getHTML: vi.fn() } as any;
        expect(getTiptapContent(emptyEditor)).toBe("");
        expect(emptyEditor.getHTML).not.toHaveBeenCalled();

        const editor = { getJSON: () => ({ content: [{ content: [{ text: "value" }] }] }), getHTML: () => "<p>value</p>" } as any;
        expect(getTiptapContent(editor)).toBe("<p>value</p>");
    });

    it("collects supported extension exports", () => {
        const extension = { type: "extension" };
        const mark = { type: "mark" };
        const node = { type: "node" };
        const result = getAllTiptapExtensions({ StarterKit: extension, Bold: mark, Image: node, lower: extension, Wrong: { type: "plugin" }, Empty: null } as any);
        expect(result).toEqual([extension, mark, node]);
    });

    it("configures image file drop and paste handlers", async () => {
        const readerInstances: any[] = [];
        class MockFileReader {
            result = "data:image/png;base64,abc";
            onload: (() => void) | null = null;
            constructor() { readerInstances.push(this); }
            readAsDataURL() { queueMicrotask(() => this.onload?.()); }
        }
        vi.stubGlobal("FileReader", MockFileReader);
        try {
            const config = defaultTiptapFileHandlerConfig();
            expect(config.allowedMimeTypes).toEqual(["image/jpeg", "image/gif", "image/png", "image/webp"]);
            const run = vi.fn(() => true);
            const chain = {
                insertContentAt: vi.fn().mockReturnThis(),
                focus: vi.fn().mockReturnThis(),
                run
            };
            const editor: any = { chain: () => chain, state: { selection: { anchor: 4 } } };
            const file = new Blob(["image"], { type: "image/png" });

            config.onDrop(editor, [file], 3);
            await Promise.resolve();
            expect(chain.insertContentAt).toHaveBeenCalledWith(3, expect.objectContaining({ type: "image" }));
            expect(run).toHaveBeenCalled();

            expect(config.onPaste(editor, [file], "<p>existing</p>")).toBeUndefined();
            config.onPaste(editor, [file], null);
            await Promise.resolve();
            expect(chain.insertContentAt).toHaveBeenCalledWith(4, expect.objectContaining({ type: "image" }));
            expect(readerInstances).toHaveLength(2);
        }
        finally {
            vi.unstubAllGlobals();
        }
    });

    it("renders toolbar commands for supported editor capabilities", () => {
        const chain: any = {
            focus: vi.fn().mockReturnThis(),
            insertContentAt: vi.fn().mockReturnThis(),
            setTextAlign: vi.fn().mockReturnThis(),
            toggleTextAlign: vi.fn().mockReturnThis(),
            toggleMark: vi.fn().mockReturnThis(),
            undo: vi.fn().mockReturnThis(),
            redo: vi.fn().mockReturnThis(),
            run: vi.fn(() => true)
        };
        const editor: any = {
            isEditable: true,
            isActive: vi.fn(() => false),
            can: () => new Proxy({}, { get: () => () => true }),
            chain: () => chain,
            extensionManager: { extensions: [{ name: "textAlign" }] },
            on: vi.fn(),
            schema: {
                spec: {
                    marks: new Map(["bold", "italic", "underline", "strike", "code", "superscript", "subscript"].map(x => [x, {}])),
                    nodes: new Map(["image", "horizontalRule"].map(x => [x, {}]))
                }
            },
            state: { selection: { node: null, empty: true } }
        };

        const toolbar = TiptapToolbar({ editor }) as HTMLElement;
        document.body.appendChild(toolbar);
        toolbar.querySelectorAll("button").forEach(button => button.click());
        expect(chain.undo).toHaveBeenCalled();
        expect(chain.redo).toHaveBeenCalled();
        expect(chain.toggleMark).toHaveBeenCalled();
        expect(chain.setTextAlign).toHaveBeenCalled();
        toolbar.remove();
    });

    it("guards toolbar commands for non-editable editors", () => {
        const chain = { focus: vi.fn().mockReturnThis(), run: vi.fn(() => true) } as any;
        const editor: any = {
            isEditable: false,
            isActive: vi.fn(() => false),
            can: () => ({ undo: () => false, redo: () => false, toggleMark: () => false, setTextAlign: () => false }),
            chain: () => chain,
            extensionManager: { extensions: [{ name: "textAlign" }] },
            on: vi.fn(),
            schema: { spec: { marks: new Map(["bold"].map(x => [x, {}])), nodes: new Map() } },
            state: { selection: { node: null, empty: true } }
        };
        const toolbar = TiptapToolbar({ editor }) as HTMLElement;
        document.body.appendChild(toolbar);
        toolbar.querySelectorAll("button").forEach(button => button.click());
        expect(chain.run).not.toHaveBeenCalled();
        toolbar.remove();
    });

    it("handles editors without a schema or selection", () => {
        const chain = { focus: vi.fn().mockReturnThis(), run: vi.fn(() => true) } as any;
        const editor: any = {
            isEditable: true,
            isActive: vi.fn(() => false),
            can: () => ({ undo: () => false, redo: () => false, toggleMark: () => false, setTextAlign: () => false }),
            chain: () => chain,
            extensionManager: { extensions: [{ name: "textAlign" }] },
            on: vi.fn(),
            state: {}
        };
        const toolbar = TiptapToolbar({ editor }) as HTMLElement;
        document.body.appendChild(toolbar);
        toolbar.querySelectorAll("button").forEach(button => button.click());
        toolbar.remove();
    });

    it("disables commands for a selected image node", () => {
        const chain: any = { focus: vi.fn().mockReturnThis(), run: vi.fn(() => true) };
        const editor: any = {
            isEditable: true,
            isActive: vi.fn(() => false),
            can: () => ({ undo: () => true, redo: () => true, toggleMark: () => true, setTextAlign: () => true }),
            chain: () => chain,
            extensionManager: { extensions: [{ name: "textAlign" }] },
            on: vi.fn(),
            schema: { spec: { marks: new Map(["bold"].map(x => [x, {}])), nodes: new Map(["image"].map(x => [x, {}])) } },
            state: { selection: { node: { type: { name: "image" } }, empty: false } }
        };
        const toolbar = TiptapToolbar({ editor }) as HTMLElement;
        document.body.appendChild(toolbar);
        toolbar.querySelectorAll("button").forEach(button => button.click());
        expect(chain.run).not.toHaveBeenCalled();
        toolbar.remove();
    });

    it("falls back gracefully when setTextAlign is unavailable", () => {
        const chain: any = { focus: vi.fn().mockReturnThis(), run: vi.fn(() => true) };
        const editor: any = {
            isEditable: true,
            isActive: vi.fn(() => true),
            can: () => ({ undo: () => false, redo: () => false, toggleMark: () => false, setTextAlign: () => true }),
            chain: () => chain,
            extensionManager: { extensions: [{ name: "textAlign" }] },
            on: vi.fn(),
            schema: { spec: { marks: new Map(["bold"].map(x => [x, {}])), nodes: new Map(["image"].map(x => [x, {}])) } },
            state: { selection: { node: null, empty: true } }
        };
        const toolbar = TiptapToolbar({ editor }) as HTMLElement;
        document.body.appendChild(toolbar);
        toolbar.querySelectorAll("button").forEach(button => button.click());
        expect(chain.run).not.toHaveBeenCalled();
        toolbar.remove();
    });
});
