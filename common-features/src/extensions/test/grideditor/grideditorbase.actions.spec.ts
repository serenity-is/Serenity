import * as corelib from "@serenity-is/corelib";
import { mockFetch, unmockFetch } from "test-utils";
import { GridEditorBase } from "../../Modules/GridEditor/GridEditorBase";
import { GridEditorDialog } from "../../Modules/GridEditor/GridEditorDialog";

class TestDialog extends GridEditorDialog<any> {
    override getIdProperty() { return "id"; }
    override getService() { return "Test/Entity"; }
    override loadEntityAndOpenDialog = vi.fn();
    override loadByIdAndOpenDialog = vi.fn();
}

class TestGridEditor extends GridEditorBase<any> {
    override getIdProperty() { return "id"; }
    override getService() { return "Test/Entity"; }
    override getDialogType() { return TestDialog; }
}

class EmptyEditor extends GridEditorBase<any> {
}

beforeEach(() => {
    mockFetch({ "*": () => ({}) });
});

afterEach(() => {
    unmockFetch();
    vi.restoreAllMocks();
});

describe("GridEditorBase service methods", () => {
    it("returns create, delete and update service methods", () => {
        const editor = new TestGridEditor({});
        expect(editor["getCreateServiceMethod"]()).toBe("Test/Entity/Create");
        expect(editor["getDeleteServiceMethod"]()).toBe("Test/Entity/Delete");
        expect(editor["getUpdateServiceMethod"]()).toBe("Test/Entity/Update");
        editor.destroy();
    });

    it("returns defaults for pager, title and grid can load", () => {
        const editor = new TestGridEditor({});
        expect(editor["usePager"]()).toBe(false);
        expect(editor["getInitialTitle"]()).toBeNull();
        expect(editor["getGridCanLoad"]()).toBe(false);
        editor.destroy();
    });

    it("allows loading in connected mode", () => {
        const editor = new TestGridEditor({});
        editor.connectedMode = true;
        expect(editor["getGridCanLoad"]()).toBe(true);
        editor.destroy();
    });
});

describe("GridEditorBase save and delete", () => {
    it("inserts a new item in disconnected mode", async () => {
        const editor = new TestGridEditor({});
        editor["nextId"] = 1;
        const response = await editor["save"]({ request: { Entity: { name: "New" } } } as any);
        const items = editor.view.getItems() as any[];
        expect(items.length).toBe(1);
        expect(items[0].name).toBe("New");
        expect(items[0].id).toBe("`1");
        expect(response).toEqual({ EntityId: undefined });
        editor.destroy();
    });

    it("updates an existing item in disconnected mode", async () => {
        const editor = new TestGridEditor({});
        editor.view.setItems([{ id: 1, name: "Old" }] as any, true);
        const callback = vi.fn();
        await editor["save"]({ request: { EntityId: 1, Entity: { id: 1, name: "Updated" } } } as any, callback);
        const items = editor.view.getItems() as any[];
        expect(items[0].name).toBe("Updated");
        expect(callback).toHaveBeenCalled();
        editor.destroy();
    });

    it("merges into existing item when updating by id", async () => {
        const editor = new TestGridEditor({});
        editor.view.setItems([{ id: 1, name: "Old", extra: "keep" }] as any, true);
        await editor["save"]({ request: { EntityId: 1, Entity: { id: 1, name: "Updated" } } } as any);
        const items = editor.view.getItems() as any[];
        expect(items[0].extra).toBe("keep");
        editor.destroy();
    });

    it("does not save when validation fails", async () => {
        const editor = new TestGridEditor({});
        (editor as any).validateEntity = () => false;
        const response = await editor["save"]({ request: { Entity: { name: "New" } } } as any);
        expect(response).toBeUndefined();
        expect(editor.view.getItems().length).toBe(0);
        editor.destroy();
    });

    it("saves via service in connected mode", async () => {
        const serviceCall = vi.spyOn(corelib, "serviceCall").mockResolvedValue({ EntityId: 7 } as any);
        const editor = new TestGridEditor({});
        editor.connectedMode = true;
        const opt: any = { request: { Entity: { name: "New" } } };
        const response = await editor["save"](opt);
        expect(serviceCall).toHaveBeenCalled();
        expect(opt.service).toBe("Test/Entity/Create");
        expect(response).toEqual({ EntityId: 7 });
        editor.destroy();
    });

    it("uses update service in connected mode when id exists", async () => {
        vi.spyOn(corelib, "serviceCall").mockResolvedValue({ EntityId: 1 } as any);
        const editor = new TestGridEditor({});
        editor.connectedMode = true;
        const opt: any = { request: { EntityId: 1, Entity: { id: 1, name: "Updated" } } };
        await editor["save"](opt);
        expect(opt.service).toBe("Test/Entity/Update");
        editor.destroy();
    });

    it("merges into an existing item in connected mode", async () => {
        vi.spyOn(corelib, "serviceCall").mockResolvedValue({ EntityId: 1 } as any);
        const editor = new TestGridEditor({});
        editor.connectedMode = true;
        editor.view.setItems([{ id: 1, name: "Old", extra: "keep" }] as any, true);
        await editor["save"]({ request: { EntityId: 1, Entity: { id: 1, name: "New" } } } as any);
        expect(editor.view.getItemById(1)).toEqual(expect.objectContaining({ extra: "keep" }));
        editor.destroy();
    });

    it("stops a connected save when validation fails", async () => {
        const editor = new TestGridEditor({});
        editor.connectedMode = true;
        (editor as any).validateEntity = () => false;
        const response = await editor["save"]({ request: { EntityId: 1, Entity: { id: 1 } } } as any);
        expect(response).toBeUndefined();
        editor.destroy();
    });

    it("keeps explicit service option in connected mode", async () => {
        vi.spyOn(corelib, "serviceCall").mockResolvedValue({} as any);
        const editor = new TestGridEditor({});
        editor.connectedMode = true;
        const opt: any = { service: "Custom/Service", request: { Entity: {} } };
        await editor["save"](opt);
        expect(opt.service).toBe("Custom/Service");
        editor.destroy();
    });

    it("deletes an item in disconnected mode", async () => {
        const editor = new TestGridEditor({});
        editor.view.setItems([{ id: 1, name: "One" }] as any, true);
        const callback = vi.fn();
        const response = await editor["delete"]({ request: { EntityId: 1 } } as any, callback);
        expect(editor.view.getItems().length).toBe(0);
        expect(callback).toHaveBeenCalled();
        expect(response).toBeTruthy();
        editor.destroy();
    });

    it("deletes via service in connected mode", async () => {
        const serviceCall = vi.spyOn(corelib, "serviceCall").mockResolvedValue({} as any);
        const editor = new TestGridEditor({});
        editor.connectedMode = true;
        const opt: any = { request: { EntityId: 1 } };
        await editor["delete"](opt);
        expect(serviceCall).toHaveBeenCalled();
        expect(opt.service).toBe("Test/Entity/Delete");
        editor.destroy();
    });

    it("does not delete when deleteEntity returns false", async () => {
        const editor = new TestGridEditor({});
        (editor as any).deleteEntity = () => false;
        const response = await editor["delete"]({ request: { EntityId: 1 } } as any);
        expect(response).toBeUndefined();
        editor.destroy();
    });

    it("returns true from deleteEntity and validates true by default", () => {
        const editor = new TestGridEditor({});
        editor.connectedMode = true;
        expect(editor["deleteEntity"](5)).toBe(true);
        expect(editor["validateEntity"]({}, 5)).toBe(true);
        expect(editor["getNewEntity"]()).toEqual({});
        editor.destroy();
    });
});

describe("GridEditorBase value accessors", () => {
    it("returns items without temporary backtick ids", () => {
        const editor = new TestGridEditor({});
        editor.view.setItems([{ id: "`1", name: "New" }, { id: 2, name: "Existing" }] as any, true);
        const value = editor.value;
        expect(value[0].id).toBeUndefined();
        expect(value[0].name).toBe("New");
        expect(value[1].id).toBe(2);
        editor.destroy();
    });

    it("sets items and generates ids for new ones", () => {
        const editor = new TestGridEditor({});
        editor["nextId"] = 5;
        editor.value = [{ name: "New" }, { id: 9, name: "Existing" }] as any;
        const items = editor.view.getItems() as any[];
        expect(items[0].id).toBe("`5");
        expect(items[1].id).toBe(9);
        editor.destroy();
    });

    it("sets an empty array when value is null", () => {
        const editor = new TestGridEditor({});
        editor.view.setItems([{ id: 1 }] as any, true);
        editor.value = null;
        expect(editor.view.getItems().length).toBe(0);
        editor.destroy();
    });

    it("getEditValue and setEditValue work when disconnected", () => {
        const editor = new TestGridEditor({});
        editor.view.setItems([{ id: 1, name: "A" }] as any, true);
        const target: any = {};
        editor.getEditValue({ name: "value" } as any, target);
        expect(Array.isArray(target.value)).toBe(true);

        const source = { value: [{ id: 2, name: "B" }] };
        editor.setEditValue(source, { name: "value" } as any);
        expect((editor.view.getItems() as any[])[0].name).toBe("B");
        editor.destroy();
    });

    it("getEditValue and setEditValue are no-ops in connected mode", () => {
        const editor = new TestGridEditor({});
        editor.connectedMode = true;
        editor.view.setItems([{ id: 1, name: "A" }] as any, true);
        const target: any = {};
        editor.getEditValue({ name: "value" } as any, target);
        expect(target.value).toBeUndefined();
        editor.destroy();
    });
});

describe("GridEditorBase dialogs", () => {
    it("replaces add button click to open a new entity dialog", async () => {
        const editor = new TestGridEditor({});
        const dialog = new TestDialog({});
        const createSpy = vi.fn((_type: any, cb: any) => cb(dialog));
        (editor as any).createEntityDialog = createSpy;
        const buttons = editor["getButtons"]();
        const addButton = buttons.find(x => x.action === "add");
        expect(addButton).toBeTruthy();
        addButton.onClick({} as any);
        expect(createSpy).toHaveBeenCalled();
        expect(typeof dialog.onSave).toBe("function");
        expect(dialog.loadEntityAndOpenDialog).toHaveBeenCalled();
        await dialog.onSave({ request: { Entity: { name: "Added" } } } as any, () => { }, "save" as any);
        expect((editor.view.getItems() as any[])[0].name).toBe("Added");
        editor.destroy();
        dialog.destroy();
    });

    it("editItem loads by id in disconnected mode", () => {
        const editor = new TestGridEditor({});
        editor.view.setItems([{ id: 1, name: "One" }] as any, true);
        const dialog = new TestDialog({});
        (editor as any).createEntityDialog = (_type: any, cb: any) => cb(dialog);
        editor["editItem"](1);
        expect(dialog.loadEntityAndOpenDialog).toHaveBeenCalledWith(expect.objectContaining({ id: 1 }));
        expect(typeof dialog.onDelete).toBe("function");
        editor.destroy();
        dialog.destroy();
    });

    it("editItem loads an entity object in disconnected mode", () => {
        const editor = new TestGridEditor({});
        const dialog = new TestDialog({});
        (editor as any).createEntityDialog = (_type: any, cb: any) => cb(dialog);
        editor["editItem"]({ id: 3, name: "Three" });
        expect(dialog.loadEntityAndOpenDialog).toHaveBeenCalledWith(expect.objectContaining({ id: 3 }));
        editor.destroy();
        dialog.destroy();
    });

    it("editItem loads by id in connected mode", () => {
        const editor = new TestGridEditor({});
        editor.connectedMode = true;
        const dialog = new TestDialog({});
        (editor as any).createEntityDialog = (_type: any, cb: any) => cb(dialog);
        editor["editItem"](8);
        expect(dialog.loadByIdAndOpenDialog).toHaveBeenCalledWith(8);
        editor.destroy();
        dialog.destroy();
    });

    it("editItem loads an entity in connected mode", () => {
        const editor = new TestGridEditor({});
        editor.connectedMode = true;
        const dialog = new TestDialog({});
        (editor as any).createEntityDialog = (_type: any, cb: any) => cb(dialog);
        editor["editItem"]({ id: 8 });
        expect(dialog.loadEntityAndOpenDialog).toHaveBeenCalled();
        editor.destroy();
        dialog.destroy();
    });

    it("editItem handles null", () => {
        const editor = new TestGridEditor({});
        const dialog = new TestDialog({});
        (editor as any).createEntityDialog = (_type: any, cb: any) => cb(dialog);
        editor["editItem"](null);
        expect(dialog.loadEntityAndOpenDialog).toHaveBeenCalledWith(null);
        editor.destroy();
        dialog.destroy();
    });
});

describe("GridEditorBase connected mode and validation", () => {
    it("toggles connected mode and calls connectedModeChanged", () => {
        const editor = new TestGridEditor({});
        const changed = vi.spyOn(editor as any, "connectedModeChanged");
        editor.connectedMode = true;
        expect(editor.connectedMode).toBe(true);
        expect(changed).toHaveBeenCalled();
        editor.connectedMode = true;
        expect(changed).toHaveBeenCalledTimes(1);
        editor.connectedMode = false;
        expect(editor.connectedMode).toBe(false);
        expect(changed).toHaveBeenCalledTimes(2);
        editor.destroy();
    });

    it("throws when the id property is still __id", () => {
        const editor = new EmptyEditor({});
        expect(() => editor["checkConnectedModeSupport"]()).toThrow(/getIdProperty/);
        editor.destroy();
    });

    it("throws when no service method is overridden", () => {
        class NoServiceEditor extends GridEditorBase<any> {
            override getIdProperty() { return "id"; }
        }
        const editor = new NoServiceEditor({});
        expect(() => editor["checkConnectedModeSupport"]()).toThrow(/getService/);
        editor.destroy();
    });

    it("checkDialogType throws for non GridEditorDialog instances", () => {
        const editor = new TestGridEditor({});
        expect(() => editor["checkDialogType"]({})).toThrow(/subclass of GridEditorDialog/);
        editor.destroy();
    });

    it("checkDialogType throws for mismatching id property", () => {
        class OtherIdDialog extends GridEditorDialog<any> {
            override getIdProperty() { return "otherId"; }
        }
        const editor = new TestGridEditor({});
        const dialog = new OtherIdDialog({});
        expect(() => editor["checkDialogType"](dialog)).toThrow(/does not match/);
        editor.destroy();
        dialog.destroy();
    });

    it("checkDialogType throws for mismatching service in connected mode", () => {
        class OtherServiceDialog extends GridEditorDialog<any> {
            override getIdProperty() { return "id"; }
            override getService() { return "Other/Service"; }
        }
        const editor = new TestGridEditor({});
        editor.connectedMode = true;
        const dialog = new OtherServiceDialog({});
        expect(() => editor["checkDialogType"](dialog)).toThrow(/same service URL/);
        editor.destroy();
        dialog.destroy();
    });

    it("checkDialogType returns the dialog when valid", () => {
        const editor = new TestGridEditor({});
        const dialog = new TestDialog({});
        expect(editor["checkDialogType"](dialog)).toBe(dialog);
        editor.destroy();
        dialog.destroy();
    });

    it("updateInterface reflects connected mode", () => {
        const editor = new TestGridEditor({});
        const superUpdate = vi.spyOn(Object.getPrototypeOf(TestGridEditor.prototype), "updateInterface");
        editor["updateInterface"]();
        expect(editor.element.hasClass("connected-mode")).toBe(false);
        editor.connectedMode = true;
        expect(editor.element.hasClass("connected-mode")).toBe(true);
        editor.destroy();
    });
});
