import { mockFetch, unmockFetch } from "test-utils";
import { GridEditorBase } from "../../Modules/GridEditor/GridEditorBase";

beforeEach(() => {
    vi.useFakeTimers();
    mockFetch({ "*": () => ({}) });
});

afterEach(() => {
    vi.clearAllTimers();
    unmockFetch();
});

describe("GridEditorBase", () => {
    class EmptyEditor extends GridEditorBase<any> {
    }

    class ConnectedEditor extends EmptyEditor {
        getIdProperty() { return "theId" };
        getService() { return "/Service" };
    }

    it("has id property as __id by default", () => {
        const editor = new EmptyEditor({});
        expect(editor["getIdProperty"]()).toBe("__id");
    });

    it("returns id property from getRowDefinition() if available", () => {
        class Editor extends EmptyEditor {
            getRowDefinition() { return { idProperty: "theRowDefinitionId" } };
        }

        const editor = new Editor({});
        expect(editor["getIdProperty"]()).toBe("theRowDefinitionId");
    });

    it("does not fail if getRowDefinition() returns null", () => {
        class Editor extends EmptyEditor {
            getRowDefinition() { return null; };
        }

        const editor = new Editor({});
        expect(editor["getIdProperty"]()).toBe("__id");
    });

    it("throws for getDialogType() by default", () => {
        const editor = new EmptyEditor({});
        expect(() => editor["getDialogType"]()).toThrow();
    });

    it("throws when setting connectedMode to true if the id property is __id", () => {
        const editor = new EmptyEditor({});
        expect(() => editor.connectedMode = true).toThrow();
    });

    it("does not throw when setting connectedMode to false even if the id property is __id", () => {
        const editor = new EmptyEditor({});
        expect(() => editor.connectedMode = false).not.toThrow();
    });

    it("returns false from getGridCanLoad() by default", () => {
        const editor = new EmptyEditor({});
        expect(editor["getGridCanLoad"]()).toBe(false);
    });

    it("returns true from getGridCanLoad() for connectedMode", () => {
        const editor = new ConnectedEditor({});
        editor.connectedMode = true;
        expect(editor["getGridCanLoad"]()).toBe(true);
    });

    it("uses itemId method for id method", () => {
        const editor = new EmptyEditor({});
        editor.itemId = () => 42;
        expect((editor as any).id({})).toBe(42);
    });

    it("returns a value with backticks for nextId method", () => {
        const editor = new EmptyEditor({});
        editor["nextId"] = 42;
        expect(editor.getNextId()).toBe("`42");
    });

    it("setNewId sets the id to the nextId()", () => {
        const editor = new EmptyEditor({});
        editor["nextId"] = 42;
        const item = {};
        const item2 = editor.setNewId(item);
        expect(item).toBe(item2);
        expect(item["__id"]).toBe("`42");
    });
});