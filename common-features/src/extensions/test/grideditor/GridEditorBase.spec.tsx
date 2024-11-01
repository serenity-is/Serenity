import { GridEditorBase } from "../../Modules/GridEditor/GridEditorBase";
import { mockFetch, unmockFetch } from "test-utils";

beforeAll(() => {
});

afterEach(() => {
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
});