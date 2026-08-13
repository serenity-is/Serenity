import { Authorization, EntityGridTexts, Fluent } from "../../base";
import { Router } from "../../compat";
import { mockFetch } from "../../test/mocks";
import { DialogTypeRegistry } from "../../types/dialogtyperegistry";
import { EditorUtils } from "../editors/editorutils";
import { SubDialogHelper } from "../helpers/subdialoghelper";
import { Widget } from "../widgets/widget";
import { DataGrid } from "./datagrid";
import { EntityGrid } from "./entitygrid";

beforeEach(() => {
    mockFetch({
        "*": () => ({})
    });
});

function getIdProperty(grid: EntityGrid<any, any>): string {
    return grid["getIdProperty"]();
}

describe('EntityGrid.getIdProperty', () => {
    it('returns ID by default', () => {
        class DefaultGrid extends EntityGrid<any, any> {
        }

        var grid = new DefaultGrid({});
        expect(getIdProperty(grid)).toBe("ID");
    });

    it('can be overridden in subclass', () => {
        class SubClassGrid extends EntityGrid<any, any> {
            getIdProperty() { return "subClassId" };
        }

        var grid = new SubClassGrid({});
        expect(getIdProperty(grid)).toBe("subClassId");
    });

    it('returns value from getRowDefition()', () => {
        class TestRow {
            static readonly idProperty = "idForTestRow";
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getIdProperty(grid)).toBe("idForTestRow");
    });

    it("returns empty string if getRowDefition() doesn't have the value", () => {
        class TestRow {
            static readonly idProperty: string = undefined;
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getIdProperty(grid)).toBe("");
    });
});

function getIsActiveProperty(grid: EntityGrid<any, any>): string {
    return grid["getIsActiveProperty"]();
}

describe('EntityGrid.getIsActiveProperty', () => {
    it('returns empty by default', () => {
        class DefaultGrid extends EntityGrid<any, any> {
        }

        var grid = new DefaultGrid({});
        expect(getIsActiveProperty(grid)).toBe("");
    });

    it('can be overridden in subclass', () => {
        class SubClassGrid extends EntityGrid<any, any> {
            getIsActiveProperty() { return "subClassIsActive" };
        }

        var grid = new SubClassGrid({});
        expect(getIsActiveProperty(grid)).toBe("subClassIsActive");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly isActiveProperty = "activeForTestRow";
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getIsActiveProperty(grid)).toBe("activeForTestRow");
    });

    it("returns empty string if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly isActiveProperty: string = undefined;
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getIsActiveProperty(grid)).toBe("");
    });
});

function getLocalTextDbPrefix(grid: EntityGrid<any, any>): string {
    return grid["getLocalTextDbPrefix"]();
}

describe('EntityGrid.getLocalTextDbPrefix', () => {
    it('returns class identifier by default', () => {
        class DefaultGrid extends EntityGrid<any, any> {
        }

        var grid = new DefaultGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("Db.Default.");
    });

    it('returns class identifier based on typeInfo property', () => {
        class DefaultGrid extends EntityGrid<any, any> {
            static [Symbol.typeInfo] = this.registerClass('MyProject.TestModule.DefaultGrid');
        }

        var grid = new DefaultGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("Db.TestModule.Default.");
    });

    it('returns class identifier based on registration name', () => {
        class DefaultGrid extends EntityGrid<any, any> {
            static [Symbol.typeInfo] = this.registerClass('MyProject.MyModule.Some.DefaultGrid');
        }

        var grid = new DefaultGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("Db.MyModule.Some.Default.");
    });

    it('can be overridden in subclass via getLocalTextDbPrefix', () => {
        class SubClassGrid extends EntityGrid<any, any> {
            getLocalTextDbPrefix() { return "My.Prefix." };
        }

        var grid = new SubClassGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("My.Prefix.");
    });

    it('can be overridden in subclass via getLocalTextPrefix', () => {
        class SubClassGrid extends EntityGrid<any, any> {
            getLocalTextPrefix() { return "MySubClassPrefix" };
        }

        var grid = new SubClassGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("Db.MySubClassPrefix.");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly localTextPrefix = "prefixForTestRow";
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("Db.prefixForTestRow.");
    });

    it("returns empty string if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly localTextPrefix: string = undefined;
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("");
    });
});

function getLocalTextPrefix(grid: EntityGrid<any, any>): string {
    return grid["getLocalTextPrefix"]();
}

describe('EntityGrid.getLocalTextPrefix', () => {
    it('returns class identifier by default', () => {
        class DefaultGrid extends EntityGrid<any, any> {
        }

        var grid = new DefaultGrid({});
        expect(getLocalTextPrefix(grid)).toBe("Default");
    });

    it('returns class identifier based on typeInfo property', () => {
        class DefaultGrid extends EntityGrid<any, any> {
            static [Symbol.typeInfo] = this.registerClass('MyProject.TestModule.DefaultGrid');
        }

        var grid = new DefaultGrid({});
        expect(getLocalTextPrefix(grid)).toBe("TestModule.Default");
    });

    it('returns class identifier based on registration name', () => {
        class DefaultGrid extends EntityGrid<any, any> {
            static [Symbol.typeInfo] = this.registerClass('MyProject.MyModule.Some.DefaultGrid');
        }

        var grid = new DefaultGrid({});
        expect(getLocalTextPrefix(grid)).toBe("MyModule.Some.Default");
    });

    it('can be overridden in subclass', () => {
        class SubClassGrid extends EntityGrid<any, any> {
            getLocalTextPrefix() { return "subClassPrefix" };
        }

        var grid = new SubClassGrid({});
        expect(getLocalTextPrefix(grid)).toBe("subClassPrefix");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly localTextPrefix = "prefixForTestRow";
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getLocalTextPrefix(grid)).toBe("prefixForTestRow");
    });

    it("returns undefined string if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly localTextPrefix: string = undefined;
        }

        class TestRowGrid extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getLocalTextPrefix(grid)).toBeUndefined();
    });
});

describe("EntityGrid service & entity type", () => {
    it("getService replaces dots with slashes and caches", () => {
        class G extends EntityGrid<any, any> {
            static [Symbol.typeInfo] = this.registerClass("MyProject.TestModule.DefaultGrid");
        }
        const grid = new G({});
        expect(grid["getService"]()).toBe("TestModule/Default");
        expect(grid["getService"]()).toBe("TestModule/Default");
        grid.destroy();
    });

    it("getServiceMethod returns List service", () => {
        class G extends EntityGrid<any, any> {
            static [Symbol.typeInfo] = this.registerClass("MyProject.TestModule.DefaultGrid");
        }
        const grid = new G({});
        expect(grid["getServiceMethod"]()).toBe("TestModule/Default/List");
        grid.destroy();
    });

    it("getServiceUrl resolves services url", () => {
        class G extends EntityGrid<any, any> {
            static [Symbol.typeInfo] = this.registerClass("MyProject.TestModule.DefaultGrid");
        }
        const grid = new G({});
        expect(grid["getServiceUrl"]()).toContain("TestModule/Default/List");
        grid.destroy();
    });

    it("getViewOptions sets url from service url", () => {
        class G extends EntityGrid<any, any> {
            static [Symbol.typeInfo] = this.registerClass("MyProject.TestModule.DefaultGrid");
        }
        const grid = new G({});
        const opt = grid["getViewOptions"]();
        expect(opt.url).toContain("TestModule/Default/List");
        grid.destroy();
    });

    it("getItemType returns entity type", () => {
        class G extends EntityGrid<any, any> {
            static [Symbol.typeInfo] = this.registerClass("MyProject.TestModule.DefaultGrid");
        }
        const grid = new G({});
        expect(grid["getItemType"]()).toBe("TestModule.Default");
        grid.destroy();
    });

    it("getEntityType strips Grid suffix and caches", () => {
        class G extends EntityGrid<any, any> {
            static [Symbol.typeInfo] = this.registerClass("MyProject.TestModule.DefaultGrid");
        }
        const grid = new G({});
        expect(grid["getEntityType"]()).toBe("TestModule.Default");
        expect(grid["getEntityType"]()).toBe("TestModule.Default");
        grid.destroy();
    });
});

describe("EntityGrid display names", () => {
    it("getDisplayName returns entity type fallback and caches", () => {
        class G extends EntityGrid<any, any> {
            static [Symbol.typeInfo] = this.registerClass("MyProject.TestModule.DefaultGrid");
        }
        const grid = new G({});
        const display = grid["getDisplayName"]();
        expect(display).toBe("TestModule.Default");
        expect(grid["getDisplayName"]()).toBe(display);
        grid.destroy();
    });

    it("getItemName returns entity type fallback and caches", () => {
        class G extends EntityGrid<any, any> {
            static [Symbol.typeInfo] = this.registerClass("MyProject.TestModule.DefaultGrid");
        }
        const grid = new G({});
        const item = grid["getItemName"]();
        expect(item).toBe("TestModule.Default");
        expect(grid["getItemName"]()).toBe(item);
        grid.destroy();
    });

    it("getAddButtonCaption returns new button text", () => {
        class G extends EntityGrid<any, any> {
            static [Symbol.typeInfo] = this.registerClass("MyProject.TestModule.DefaultGrid");
        }
        const grid = new G({});
        expect(grid["getAddButtonCaption"]()).toBe(EntityGridTexts.NewButton);
        grid.destroy();
    });
});

describe("EntityGrid permissions", () => {
    it("returns permissions from row definition", () => {
        class TestRow {
            static readonly insertPermission = "I";
            static readonly updatePermission = "U";
            static readonly deletePermission = "D";
        }
        class G extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }
        const grid = new G({});
        expect(grid["getInsertPermission"]()).toBe("I");
        expect(grid["getUpdatePermission"]()).toBe("U");
        expect(grid["getDeletePermission"]()).toBe("D");
        grid.destroy();
    });

    it("permissions are granted when no permission defined", () => {
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        expect(grid["hasInsertPermission"]()).toBe(true);
        expect(grid["hasUpdatePermission"]()).toBe(true);
        expect(grid["hasDeletePermission"]()).toBe(true);
        grid.destroy();
    });

    it("checks Authorization for permissions", () => {
        class TestRow {
            static readonly insertPermission = "I";
            static readonly updatePermission = "U";
            static readonly deletePermission = "D";
        }
        class G extends EntityGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }
        const grid = new G({});
        const spy = vi.spyOn(Authorization, "hasPermission").mockReturnValue(false);
        expect(grid["hasInsertPermission"]()).toBe(false);
        expect(grid["hasUpdatePermission"]()).toBe(false);
        expect(grid["hasDeletePermission"]()).toBe(false);
        expect(Authorization.hasPermission).toHaveBeenCalledTimes(3);
        spy.mockRestore();
        grid.destroy();
    });
});

describe("EntityGrid buttons", () => {
    it("getButtons returns toolbar buttons and add onClick works", () => {
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        const buttons = grid["getButtons"]() as any[];
        expect(Array.isArray(buttons)).toBe(true);
        expect(buttons.length).toBeGreaterThanOrEqual(3);
        const addButtonClickSpy = vi.spyOn(grid as any, "addButtonClick").mockImplementation(() => { });
        buttons[0].onClick({} as any);
        expect(addButtonClickSpy).toHaveBeenCalled();
        expect(typeof buttons[0].disabled).toBe("function");
        expect(buttons[0].disabled()).toBe(false);
        grid.destroy();
        addButtonClickSpy.mockRestore();
    });

    it("newRefreshButton onClick refreshes", () => {
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        const button = grid["newRefreshButton"](true);
        expect(button.title).toBeNull();
        expect(button.hint).toBe(EntityGridTexts.RefreshButton);
        const refreshSpy = vi.spyOn(grid as any, "refresh").mockImplementation(() => { });
        (button as any).onClick();
        expect(refreshSpy).toHaveBeenCalled();
        grid.destroy();
        refreshSpy.mockRestore();
    });

    it("newRefreshButton with text sets title", () => {
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        const button = grid["newRefreshButton"](false);
        expect(button.title).toBe(EntityGridTexts.RefreshButton);
        grid.destroy();
    });

    it("addButtonClick calls editItem", () => {
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        const editItemSpy = vi.spyOn(grid as any, "editItem").mockImplementation(() => { });
        grid["addButtonClick"]();
        expect(editItemSpy).toHaveBeenCalledWith(new Object());
        grid.destroy();
        editItemSpy.mockRestore();
    });
});

describe("EntityGrid filter bar", () => {
    it("setFilterBarVisibility toggles and lays out when store has no items", () => {
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        const domNode = document.createElement("div");
        grid["filterBar"] = { domNode, get_store: () => ({ get_items: () => [] } as any) } as any;
        const layoutSpy = vi.spyOn(grid as any, "layout").mockImplementation(() => { });
        grid["setFilterBarVisibility"]();
        expect(layoutSpy).toHaveBeenCalled();
        grid.destroy();
        layoutSpy.mockRestore();
    });

    it("createFilterBar calls super and sets visibility", () => {
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        const superSpy = vi.spyOn(DataGrid.prototype, "createFilterBar" as any).mockImplementation(() => { });
        const setVisSpy = vi.spyOn(grid as any, "setFilterBarVisibility").mockImplementation(() => { });
        grid["createFilterBar"]();
        expect(superSpy).toHaveBeenCalled();
        expect(setVisSpy).toHaveBeenCalled();
        grid.destroy();
        superSpy.mockRestore();
    });

    it("filterStoreChanged calls super and sets visibility", () => {
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        const superSpy = vi.spyOn(DataGrid.prototype, "filterStoreChanged" as any).mockImplementation(() => { });
        const setVisSpy = vi.spyOn(grid as any, "setFilterBarVisibility").mockImplementation(() => { });
        grid["filterStoreChanged"]();
        expect(superSpy).toHaveBeenCalled();
        expect(setVisSpy).toHaveBeenCalled();
        grid.destroy();
        superSpy.mockRestore();
    });
});

describe("EntityGrid dialog flow", () => {
    it("transferDialogReadOnly sets dialog readonly when grid readonly", () => {
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        grid["_readonly"] = true;
        const setReadOnlySpy = vi.spyOn(EditorUtils, "setReadOnly").mockImplementation(() => { });
        grid["transferDialogReadOnly"]({} as any);
        expect(setReadOnlySpy).toHaveBeenCalled();
        grid.destroy();
        setReadOnlySpy.mockRestore();
    });

    it("initDialog binds data change and routes dialog", () => {
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        const dialog = { domNode: document.createElement("div") };
        const bindSpy = vi.spyOn(SubDialogHelper, "bindToDataChange").mockImplementation(() => { });
        const transferSpy = vi.spyOn(grid as any, "transferDialogReadOnly").mockImplementation(() => { });
        const routeSpy = vi.spyOn(grid as any, "routeDialog").mockImplementation(() => { });
        grid["initDialog"](dialog as any);
        expect(bindSpy).toHaveBeenCalled();
        expect(transferSpy).toHaveBeenCalled();
        expect(routeSpy).toHaveBeenCalledWith(grid["getItemType"](), dialog);
        grid.destroy();
        bindSpy.mockRestore();
    });

    it("initEntityDialog routes with itemType when different", () => {
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        const dialog = { domNode: document.createElement("div") };
        const bindSpy = vi.spyOn(SubDialogHelper, "bindToDataChange").mockImplementation(() => { });
        const transferSpy = vi.spyOn(grid as any, "transferDialogReadOnly").mockImplementation(() => { });
        const routeSpy = vi.spyOn(grid as any, "routeDialog").mockImplementation(() => { });
        grid["initEntityDialog"]("OtherType", dialog as any);
        expect(routeSpy).toHaveBeenCalledWith("OtherType", dialog);
        grid.destroy();
        bindSpy.mockRestore();
    });

    it("routeDialog computes edit hash for different item type", () => {
        class G extends EntityGrid<any, any> {
            static [Symbol.typeInfo] = this.registerClass("MyProject.TestModule.DefaultGrid");
        }
        const grid = new G({});
        const dialog = { domNode: document.createElement("div"), entityId: 5 };
        const routerSpy = vi.spyOn(Router, "dialog").mockImplementation(() => { });
        grid["routeDialog"]("OtherType", dialog as any);
        const getHash = routerSpy.mock.calls[0][2];
        expect(getHash()).toBe("OtherType/edit/5");
        grid.destroy();
        routerSpy.mockRestore();
    });

    it("routeDialog computes new hash for matching type without entityId", () => {
        class G extends EntityGrid<any, any> {
            static [Symbol.typeInfo] = this.registerClass("MyProject.TestModule.DefaultGrid");
        }
        const grid = new G({});
        const dialog = { domNode: document.createElement("div"), entityId: null };
        const routerSpy = vi.spyOn(Router, "dialog").mockImplementation(() => { });
        grid["routeDialog"](grid["getItemType"](), dialog as any);
        const getHash = routerSpy.mock.calls[0][2];
        expect(getHash()).toBe("new");
        grid.destroy();
        routerSpy.mockRestore();
    });

    it("getDialogOptions returns empty object", () => {
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        expect(grid["getDialogOptions"]()).toEqual({});
        grid.destroy();
    });

    it("getDialogOptionsFor returns dialog options for matching type", () => {
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        const optsSpy = vi.spyOn(grid as any, "getDialogOptions").mockReturnValue({ foo: 1 });
        expect(grid["getDialogOptionsFor"](grid["getItemType"]())).toEqual({ foo: 1 });
        expect(grid["getDialogOptionsFor"]("Other")).toEqual({});
        grid.destroy();
        optsSpy.mockRestore();
    });

    it("getDialogTypeFor returns dialog type for matching type else registry", () => {
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        const type = class { };
        grid["getDialogType"] = () => type as any;
        expect(grid["getDialogTypeFor"](grid["getItemType"]())).toBe(type);
        const registrySpy = vi.spyOn(DialogTypeRegistry, "getOrLoad").mockReturnValue("RegType" as any);
        expect(grid["getDialogTypeFor"]("Other")).toBe("RegType");
        grid.destroy();
        registrySpy.mockRestore();
    });

    it("getDialogType returns and caches sync type", () => {
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        const registrySpy = vi.spyOn(DialogTypeRegistry, "getOrLoad").mockReturnValue("T" as any);
        expect(grid["getDialogType"]()).toBe("T");
        expect(grid["getDialogType"]()).toBe("T");
        expect(registrySpy).toHaveBeenCalledTimes(1);
        grid.destroy();
        registrySpy.mockRestore();
    });

    it("getDialogType resolves promise type and caches", async () => {
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        const registrySpy = vi.spyOn(DialogTypeRegistry, "getOrLoad").mockReturnValue(Promise.resolve("PT") as any);
        const p1 = grid["getDialogType"]();
        expect(typeof (p1 as any).then).toBe("function");
        expect(await p1).toBe("PT");
        expect(grid["getDialogType"]()).toBe("PT");
        grid.destroy();
        registrySpy.mockRestore();
    });

    it("createEntityDialog creates widget synchronously", () => {
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        const fakeType = class FakeDialog { };
        grid["getDialogTypeFor"] = () => fakeType as any;
        const widgetCreateSpy = vi.spyOn(Widget, "create").mockReturnValue({} as any);
        const initSpy = vi.spyOn(grid as any, "initEntityDialog").mockImplementation(() => { });
        const callback = vi.fn();
        const result = grid["createEntityDialog"]("X", callback);
        expect(widgetCreateSpy).toHaveBeenCalledWith({ type: fakeType, options: grid["getDialogOptionsFor"]("X") });
        expect(callback).toHaveBeenCalled();
        expect(result).toBeDefined();
        grid.destroy();
        widgetCreateSpy.mockRestore();
    });

    it("createEntityDialog handles promise dialog type", async () => {
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        const fakeType = class FakeDialog { };
        grid["getDialogTypeFor"] = () => Promise.resolve(fakeType) as any;
        const widgetCreateSpy = vi.spyOn(Widget, "create").mockReturnValue({} as any);
        const initSpy = vi.spyOn(grid as any, "initEntityDialog").mockImplementation(() => { });
        const callback = vi.fn();
        const result = grid["createEntityDialog"]("X", callback);
        expect(typeof (result as any).then).toBe("function");
        await result;
        expect(callback).toHaveBeenCalled();
        grid.destroy();
        widgetCreateSpy.mockRestore();
    });

    it("editItem creates and loads dialog", async () => {
        const safeCastSpy = vi.spyOn(await import("../../compat"), "safeCast").mockImplementation((d: any) => d);
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        const dialog = { load: vi.fn((id: any, cb: any) => cb()), dialogOpen: vi.fn() };
        grid["createEntityDialog"] = vi.fn((itemType: any, cb: any) => cb(dialog));
        grid["editItem"]({ ID: 1 });
        expect(dialog.load).toHaveBeenCalledWith({ ID: 1 }, expect.any(Function));
        expect(dialog.dialogOpen).toHaveBeenCalled();
        grid.destroy();
        safeCastSpy.mockRestore();
    });

    it("editItem throws when dialog does not implement IEditDialog", async () => {
        const safeCastSpy = vi.spyOn(await import("../../compat"), "safeCast").mockReturnValue(null);
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        grid["createEntityDialog"] = vi.fn((itemType: any, cb: any) => cb({}));
        expect(() => grid["editItem"]({ ID: 1 })).toThrow();
        grid.destroy();
        safeCastSpy.mockRestore();
    });

    it("editItemOfType delegates to editItem when matching type", async () => {
        const safeCastSpy = vi.spyOn(await import("../../compat"), "safeCast").mockImplementation((d: any) => d);
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        const dialog = { load: vi.fn((id: any, cb: any) => cb()), dialogOpen: vi.fn() };
        grid["createEntityDialog"] = vi.fn((itemType: any, cb: any) => cb(dialog));
        grid["editItemOfType"](grid["getItemType"](), { ID: 1 });
        expect(dialog.load).toHaveBeenCalled();
        grid.destroy();
        safeCastSpy.mockRestore();
    });

    it("editItemOfType loads dialog for other type", async () => {
        const safeCastSpy = vi.spyOn(await import("../../compat"), "safeCast").mockImplementation((d: any) => d);
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        const dialog = { load: vi.fn((id: any, cb: any) => cb()), dialogOpen: vi.fn() };
        grid["createEntityDialog"] = vi.fn((itemType: any, cb: any) => cb(dialog));
        grid["editItemOfType"]("OtherType", { ID: 2 });
        expect(grid["createEntityDialog"]).toHaveBeenCalledWith("OtherType", expect.any(Function));
        expect(dialog.load).toHaveBeenCalled();
        grid.destroy();
        safeCastSpy.mockRestore();
    });
});

describe("EntityGrid.handleRoute", () => {
    it("routes to new record", () => {
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        const e = { preventDefault: vi.fn() } as any;
        const eventPropSpy = vi.spyOn(Fluent, "eventProp").mockReturnValue("new");
        const addBtnSpy = vi.spyOn(grid as any, "addButtonClick").mockImplementation(() => { });
        grid["handleRoute"](e);
        expect(e.preventDefault).toHaveBeenCalled();
        expect(addBtnSpy).toHaveBeenCalled();
        grid.destroy();
        eventPropSpy.mockRestore();
    });

    it("routes to edit item", () => {
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        const e = { preventDefault: vi.fn() } as any;
        const eventPropSpy = vi.spyOn(Fluent, "eventProp").mockImplementation((_e: any, key: string) => key === "route" ? "edit/123" : "");
        const editSpy = vi.spyOn(grid as any, "editItem").mockImplementation(() => { });
        grid["handleRoute"](e);
        expect(e.preventDefault).toHaveBeenCalled();
        expect(editSpy).toHaveBeenCalledWith("123");
        grid.destroy();
        eventPropSpy.mockRestore();
    });

    it("routes to edit item of type", () => {
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        const e = { preventDefault: vi.fn() } as any;
        const eventPropSpy = vi.spyOn(Fluent, "eventProp").mockImplementation((_e: any, key: string) => key === "route" ? "OtherType/edit/123" : "");
        const editSpy = vi.spyOn(grid as any, "editItemOfType").mockImplementation(() => { });
        grid["handleRoute"](e);
        expect(e.preventDefault).toHaveBeenCalled();
        expect(editSpy).toHaveBeenCalledWith("OtherType", "123");
        grid.destroy();
        eventPropSpy.mockRestore();
    });

    it("routes to new item of type", () => {
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        const e = { preventDefault: vi.fn() } as any;
        const eventPropSpy = vi.spyOn(Fluent, "eventProp").mockImplementation((_e: any, key: string) => key === "route" ? "OtherType/new" : "");
        const editSpy = vi.spyOn(grid as any, "editItemOfType").mockImplementation(() => { });
        grid["handleRoute"](e);
        expect(e.preventDefault).toHaveBeenCalled();
        expect(editSpy).toHaveBeenCalledWith("OtherType", null);
        grid.destroy();
        eventPropSpy.mockRestore();
    });

    it("ignores unknown routes", () => {
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        const e = { preventDefault: vi.fn() } as any;
        const eventPropSpy = vi.spyOn(Fluent, "eventProp").mockReturnValue("some/unknown/route");
        grid["handleRoute"](e);
        expect(e.preventDefault).not.toHaveBeenCalled();
        grid.destroy();
        eventPropSpy.mockRestore();
    });

    it("fixes hash after initial edit route", async () => {
        class G extends EntityGrid<any, any> { }
        const grid = new G({});
        const e = { preventDefault: vi.fn() } as any;
        const eventProps: any = { route: "MyType/edit/123", isInitial: true, parts: ["MyType", "edit", "123"], index: 0 };
        const eventPropSpy = vi.spyOn(Fluent, "eventProp").mockImplementation((_e: any, key: string) => eventProps[key]);
        const editSpy = vi.spyOn(grid as any, "editItemOfType").mockImplementation(() => { });
        const activeSpy = vi.spyOn(await import("../../base"), "getActiveRequests")
            .mockReturnValueOnce(0)
            .mockReturnValue(1);
        const defaultPreventedSpy = vi.spyOn(Fluent, "isDefaultPrevented").mockReturnValue(true);
        grid["handleRoute"](e);
        expect(e.preventDefault).toHaveBeenCalled();
        // Dispatch the ajaxStop handler to fix the hash
        document.dispatchEvent(new Event("ajaxStop"));
        expect(window.location.hash).toBe("#MyType/+/edit/+/123");
        grid.destroy();
        eventPropSpy.mockRestore();
        activeSpy.mockRestore();
        defaultPreventedSpy.mockRestore();
    });
});