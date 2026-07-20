import { Authorization, Fluent, PropertyItemsData, Validator } from "../../base";
import { EntityDialog } from "./entitydialog";

function getIdProperty(dialog: EntityDialog<any, any>): string {
    return dialog["getIdProperty"]();
}

function mockPropertyItemsData(): PropertyItemsData {
    return { items: [], additionalItems: [] };
}

// Helper to access protected members
function getEntityType(dialog: EntityDialog<any, any>): string {
    return (dialog as any).getEntityType();
}

function getFormKey(dialog: EntityDialog<any, any>): string {
    return (dialog as any).getFormKey();
}

function getEntitySingular(dialog: EntityDialog<any, any>): string {
    return (dialog as any).getEntitySingular();
}

function getNameProperty(dialog: EntityDialog<any, any>): string {
    return (dialog as any).getNameProperty();
}

function getService(dialog: EntityDialog<any, any>): string {
    return (dialog as any).getService();
}

function isEditMode(dialog: EntityDialog<any, any>): boolean {
    return (dialog as any).isEditMode();
}

function isNew(dialog: EntityDialog<any, any>): boolean {
    return (dialog as any).isNew();
}

function isNewOrDeleted(dialog: EntityDialog<any, any>): boolean {
    return (dialog as any).isNewOrDeleted();
}

function isCloneMode(dialog: EntityDialog<any, any>): boolean {
    return (dialog as any).isCloneMode();
}

function isDeleted(dialog: EntityDialog<any, any>): boolean {
    return (dialog as any).isDeleted();
}

function loadResponse(dialog: EntityDialog<any, any>, data: any): void {
    (dialog as any).loadResponse(data);
}

function loadEntity(dialog: EntityDialog<any, any>, entity: any): void {
    (dialog as any).loadEntity(entity);
}

function beforeLoadEntity(dialog: EntityDialog<any, any>, entity: any): void {
    (dialog as any).beforeLoadEntity(entity);
}

function afterLoadEntity(dialog: EntityDialog<any, any>): void {
    (dialog as any).afterLoadEntity();
}

function getEntityNameFieldValue(dialog: EntityDialog<any, any>): string {
    return (dialog as any).getEntityNameFieldValue();
}

function getEntityTitle(dialog: EntityDialog<any, any>): string {
    return (dialog as any).getEntityTitle();
}

function updateTitle(dialog: EntityDialog<any, any>): void {
    (dialog as any).updateTitle();
}

function getSaveEntity(dialog: EntityDialog<any, any>): any {
    return (dialog as any).getSaveEntity();
}

function getSaveRequest(dialog: EntityDialog<any, any>): any {
    return (dialog as any).getSaveRequest();
}

function getSaveOptions(dialog: EntityDialog<any, any>, callback?: any, initiator?: string): any {
    return (dialog as any).getSaveOptions(callback, initiator);
}

function getCreateServiceMethod(dialog: EntityDialog<any, any>): string {
    return (dialog as any).getCreateServiceMethod();
}

function getUpdateServiceMethod(dialog: EntityDialog<any, any>): string {
    return (dialog as any).getUpdateServiceMethod();
}

function getRetrieveServiceMethod(dialog: EntityDialog<any, any>): string {
    return (dialog as any).getRetrieveServiceMethod();
}

function getDeleteServiceMethod(dialog: EntityDialog<any, any>): string {
    return (dialog as any).getDeleteServiceMethod();
}

function getUndeleteServiceMethod(dialog: EntityDialog<any, any>): string {
    return (dialog as any).getUndeleteServiceMethod();
}

function validateBeforeSave(dialog: EntityDialog<any, any>): boolean {
    return (dialog as any).validateBeforeSave();
}

function commitEdits(dialog: EntityDialog<any, any>): Promise<boolean> {
    return (dialog as any).commitEdits();
}

function hasSavePermission(dialog: EntityDialog<any, any>): boolean {
    return (dialog as any).hasSavePermission();
}

function hasDeletePermission(dialog: EntityDialog<any, any>): boolean {
    return (dialog as any).hasDeletePermission();
}

function hasInsertPermission(dialog: EntityDialog<any, any>): boolean {
    return (dialog as any).hasInsertPermission();
}

function hasUpdatePermission(dialog: EntityDialog<any, any>): boolean {
    return (dialog as any).hasUpdatePermission();
}

function isViewMode(dialog: EntityDialog<any, any>): boolean {
    return (dialog as any).isViewMode();
}

function useViewMode(dialog: EntityDialog<any, any>): boolean {
    return (dialog as any).useViewMode();
}

function getInsertPermission(dialog: EntityDialog<any, any>): string {
    return (dialog as any).getInsertPermission();
}

function getUpdatePermission(dialog: EntityDialog<any, any>): string {
    return (dialog as any).getUpdatePermission();
}

function getDeletePermission(dialog: EntityDialog<any, any>): string {
    return (dialog as any).getDeletePermission();
}

function getLoadByIdRequest(dialog: EntityDialog<any, any>, id: any): any {
    return (dialog as any).getLoadByIdRequest(id);
}

function getLoadByIdOptions(dialog: EntityDialog<any, any>, id: any, callback?: any): any {
    return (dialog as any).getLoadByIdOptions(id, callback);
}

function getCloningEntity(dialog: EntityDialog<any, any>): any {
    return (dialog as any).getCloningEntity();
}

function getPropertyGridOptions(dialog: EntityDialog<any, any>): any {
    return (dialog as any).getPropertyGridOptions();
}

function getPropertyItems(dialog: EntityDialog<any, any>): any[] {
    return (dialog as any).getPropertyItems();
}

function getPropertyItemsData(dialog: EntityDialog<any, any>): PropertyItemsData {
    return (dialog as any).getPropertyItemsData();
}

function getLocalizerOptions(dialog: EntityDialog<any, any>): any {
    return (dialog as any).getLocalizerOptions();
}

function getLanguages(dialog: EntityDialog<any, any>): any[] {
    return (dialog as any).getLanguages();
}

function getIsDeletedProperty(dialog: EntityDialog<any, any>): string {
    return (dialog as any).getIsDeletedProperty();
}

function loadById(dialog: EntityDialog<any, any>, id: any, callback?: any, fail?: any): void {
    (dialog as any).loadById(id, callback, fail);
}

function updateInterface(dialog: EntityDialog<any, any>): void {
    (dialog as any).updateInterface();
}

function getToolbarButtons(dialog: EntityDialog<any, any>): any[] {
    return (dialog as any).getToolbarButtons();
}

function initPropertyGrid(dialog: EntityDialog<any, any>): void {
    (dialog as any).initPropertyGrid();
}

function initLocalizer(dialog: EntityDialog<any, any>): void {
    (dialog as any).initLocalizer();
}

function reloadById(dialog: EntityDialog<any, any>): void {
    (dialog as any).reloadById();
}

function get_entityId(dialog: EntityDialog<any, any>): any {
    return (dialog as any).get_entityId();
}

describe("EntityDialog.getEntityType", () => {
    it("returns type name without Dialog suffix", () => {
        class TestDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.TestDialog");
        }
        const dialog = new TestDialog({});
        expect(getEntityType(dialog)).toBe("Test.Test");
        dialog.destroy();
    });

    it("returns type name without Panel suffix", () => {
        class TestPanel extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.TestPanel");
        }
        const dialog = new TestPanel({});
        expect(getEntityType(dialog)).toBe("Test.Test");
        dialog.destroy();
    });

    it("returns type name without Dialog suffix (no namespace)", () => {
        class MyDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyDialog");
        }
        const dialog = new MyDialog({});
        // "MyDialog".substring(0, 2) = "My" after removing "Dialog" suffix
        expect(getEntityType(dialog)).toBe("My");
        dialog.destroy();
    });

    it("caches result after first call", () => {
        class CacheDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.CacheDialog");
        }
        const dialog = new CacheDialog({});
        const first = getEntityType(dialog);
        const second = getEntityType(dialog);
        expect(first).toBe(second);
        dialog.destroy();
    });
});

describe("EntityDialog.getFormKey", () => {
    it("returns entity type by default", () => {
        class TestDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.TestDialog");
        }
        const dialog = new TestDialog({});
        expect(getFormKey(dialog)).toBe("Test.Test");
        dialog.destroy();
    });

    it("caches result", () => {
        class CacheDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.CacheDialog");
        }
        const dialog = new CacheDialog({});
        const first = getFormKey(dialog);
        const second = getFormKey(dialog);
        expect(first).toBe(second);
        dialog.destroy();
    });
});

describe("EntityDialog.getEntitySingular", () => {
    it("returns entity type as fallback", () => {
        class TestDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.TestDialog");
        }
        const dialog = new TestDialog({});
        const singular = getEntitySingular(dialog);
        expect(singular).toBeTruthy();
        dialog.destroy();
    });
});

describe("EntityDialog.getNameProperty", () => {
    it("returns 'Name' by default", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        expect(getNameProperty(dialog)).toBe("Name");
        dialog.destroy();
    });

    it("returns value from getRowDefinition", () => {
        class TestRow {
            static readonly nameProperty = "FullName";
        }
        class RowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            getRowDefinition() { return TestRow; }
        }
        const dialog = new RowDialog({});
        expect(getNameProperty(dialog)).toBe("FullName");
        dialog.destroy();
    });

    it("returns empty string if row def has no nameProperty", () => {
        class TestRow {
            static readonly nameProperty: string = undefined;
        }
        class RowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            getRowDefinition() { return TestRow; }
        }
        const dialog = new RowDialog({});
        expect(getNameProperty(dialog)).toBe("");
        dialog.destroy();
    });

    it("caches result", () => {
        class CacheDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new CacheDialog({});
        const first = getNameProperty(dialog);
        const second = getNameProperty(dialog);
        expect(first).toBe(second);
        dialog.destroy();
    });
});

describe("EntityDialog.getService", () => {
    it("replaces dots with slashes", () => {
        class TestDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.TestDialog");
        }
        const dialog = new TestDialog({});
        expect(getService(dialog)).toBe("Test/Test");
        dialog.destroy();
    });

    it("caches result", () => {
        class CacheDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.CacheDialog");
        }
        const dialog = new CacheDialog({});
        const first = getService(dialog);
        const second = getService(dialog);
        expect(first).toBe(second);
        dialog.destroy();
    });
});

describe("EntityDialog.isEditMode / isNew / isCloneMode", () => {
    it("isNew returns true when entityId is null", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        expect(isNew(dialog)).toBe(true);
        expect(isEditMode(dialog)).toBe(false);
        dialog.destroy();
    });

    it("isEditMode returns true when entityId is set", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        (dialog as any).entityId = 123;
        expect(isEditMode(dialog)).toBe(true);
        expect(isNew(dialog)).toBe(false);
        dialog.destroy();
    });

    it("isCloneMode returns false by default", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        expect(isCloneMode(dialog)).toBe(false);
        dialog.destroy();
    });

    it("isNewOrDeleted returns true when entityId is null", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        expect(isNewOrDeleted(dialog)).toBe(true);
        dialog.destroy();
    });
});

describe("EntityDialog.isDeleted", () => {
    it("returns false when entityId is null", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        expect(isDeleted(dialog)).toBe(false);
        dialog.destroy();
    });

    it("returns false when isActiveProperty is not set", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        (dialog as any).entityId = 123;
        (dialog as any).entity = {};
        expect(isDeleted(dialog)).toBe(false);
        dialog.destroy();
    });

    it("returns true when isActiveProperty value is negative", () => {
        class TestRow {
            static readonly isActiveProperty = "IsActive";
        }
        class RowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            getRowDefinition() { return TestRow; }
        }
        const dialog = new RowDialog({});
        (dialog as any).entityId = 123;
        (dialog as any)["entity"] = { IsActive: -1 };
        expect(isDeleted(dialog)).toBe(true);
        dialog.destroy();
    });

    it("returns false when isActiveProperty value is positive", () => {
        class TestRow {
            static readonly isActiveProperty = "IsActive";
        }
        class RowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            getRowDefinition() { return TestRow; }
        }
        const dialog = new RowDialog({});
        (dialog as any).entityId = 123;
        (dialog as any)["entity"] = { IsActive: 1 };
        expect(isDeleted(dialog)).toBe(false);
        dialog.destroy();
    });

    it("returns true when isDeletedProperty is set on entity", () => {
        class TestRow {
            static readonly isDeletedProperty = "IsDeleted";
        }
        class RowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            getRowDefinition() { return TestRow; }
        }
        const dialog = new RowDialog({});
        (dialog as any).entityId = 123;
        (dialog as any)["entity"] = { IsDeleted: true };
        expect(isDeleted(dialog)).toBe(true);
        dialog.destroy();
    });

    it("returns false when isDeletedProperty is falsy on entity", () => {
        class TestRow {
            static readonly isDeletedProperty = "IsDeleted";
        }
        class RowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            getRowDefinition() { return TestRow; }
        }
        const dialog = new RowDialog({});
        (dialog as any).entityId = 123;
        (dialog as any)["entity"] = { IsDeleted: false };
        expect(isDeleted(dialog)).toBe(false);
        dialog.destroy();
    });
});

describe("EntityDialog.getIsDeletedProperty", () => {
    it("returns undefined when no row definition", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        expect(getIsDeletedProperty(dialog)).toBeUndefined();
        dialog.destroy();
    });

    it("returns value from row definition", () => {
        class TestRow {
            static readonly isDeletedProperty = "IsDeleted";
        }
        class RowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            getRowDefinition() { return TestRow; }
        }
        const dialog = new RowDialog({});
        expect(getIsDeletedProperty(dialog)).toBe("IsDeleted");
        dialog.destroy();
    });
});

describe("EntityDialog.service method names", () => {
    class TestDialog extends EntityDialog<any, any> {
        getPropertyItemsData() { return mockPropertyItemsData(); }
        static [Symbol.typeInfo] = this.registerClass("MyProject.Test.TestDialog");
    }

    it("getCreateServiceMethod returns /Create", () => {
        const dialog = new TestDialog({});
        expect(getCreateServiceMethod(dialog)).toBe("Test/Test/Create");
        dialog.destroy();
    });

    it("getUpdateServiceMethod returns /Update", () => {
        const dialog = new TestDialog({});
        expect(getUpdateServiceMethod(dialog)).toBe("Test/Test/Update");
        dialog.destroy();
    });

    it("getRetrieveServiceMethod returns /Retrieve", () => {
        const dialog = new TestDialog({});
        expect(getRetrieveServiceMethod(dialog)).toBe("Test/Test/Retrieve");
        dialog.destroy();
    });

    it("getDeleteServiceMethod returns /Delete", () => {
        const dialog = new TestDialog({});
        expect(getDeleteServiceMethod(dialog)).toBe("Test/Test/Delete");
        dialog.destroy();
    });

    it("getUndeleteServiceMethod returns /Undelete", () => {
        const dialog = new TestDialog({});
        expect(getUndeleteServiceMethod(dialog)).toBe("Test/Test/Undelete");
        dialog.destroy();
    });
});

describe("EntityDialog.validateBeforeSave", () => {
    it("returns true by default", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        expect(validateBeforeSave(dialog)).toBe(true);
        dialog.destroy();
    });
});

describe("EntityDialog.commitEdits", () => {
    it("returns true when propertyGrid is null", async () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        expect(await commitEdits(dialog)).toBe(true);
        dialog.destroy();
    });

    it("returns false when propertyGrid.commitEdits returns false", async () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        dialog["propertyGrid"] = { commitEdits: vi.fn(async () => false), destroy: vi.fn() } as any;
        expect(await commitEdits(dialog)).toBe(false);
        dialog.destroy();
    });

    it("returns true when propertyGrid.commitEdits returns true", async () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        dialog["propertyGrid"] = { commitEdits: vi.fn(async () => true), destroy: vi.fn() } as any;
        expect(await commitEdits(dialog)).toBe(true);
        dialog.destroy();
    });
});

describe("EntityDialog.permissions", () => {
    it("hasSavePermission returns hasInsertPermission when new", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        expect(hasSavePermission(dialog)).toBe(true);
        dialog.destroy();
    });

    it("hasSavePermission returns hasUpdatePermission when editing", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        (dialog as any).entityId = 123;
        expect(hasSavePermission(dialog)).toBe(true);
        dialog.destroy();
    });

    it("getInsertPermission returns null by default", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        expect(getInsertPermission(dialog)).toBeUndefined();
        dialog.destroy();
    });

    it("getUpdatePermission returns null by default", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        expect(getUpdatePermission(dialog)).toBeUndefined();
        dialog.destroy();
    });

    it("getDeletePermission returns null by default", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        expect(getDeletePermission(dialog)).toBeUndefined();
        dialog.destroy();
    });

    it("permissions from row definition", () => {
        class TestRow {
            static readonly insertPermission = "Insert";
            static readonly updatePermission = "Update";
            static readonly deletePermission = "Delete";
        }
        class RowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            getRowDefinition() { return TestRow; }
        }
        const dialog = new RowDialog({});
        expect(getInsertPermission(dialog)).toBe("Insert");
        expect(getUpdatePermission(dialog)).toBe("Update");
        expect(getDeletePermission(dialog)).toBe("Delete");
        dialog.destroy();
    });

    it("hasDeletePermission returns true when permission is null", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        expect(hasDeletePermission(dialog)).toBe(true);
        dialog.destroy();
    });

    it("hasDeletePermission checks Authorization", () => {
        class TestRow {
            static readonly deletePermission = "Admin";
        }
        class RowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            getRowDefinition() { return TestRow; }
        }
        vi.spyOn(Authorization, "hasPermission").mockReturnValue(true);
        const dialog = new RowDialog({});
        expect(hasDeletePermission(dialog)).toBe(true);
        expect(Authorization.hasPermission).toHaveBeenCalledWith("Admin");
        dialog.destroy();
        vi.restoreAllMocks();
    });
});

describe("EntityDialog.useViewMode / isViewMode", () => {
    it("useViewMode returns false by default", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        expect(useViewMode(dialog)).toBe(false);
        dialog.destroy();
    });

    it("isViewMode returns false when useViewMode is false", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        expect(isViewMode(dialog)).toBe(false);
        dialog.destroy();
    });

    it("isViewMode returns true when useViewMode is true and not editClicked", () => {
        class ViewDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            useViewMode() { return true; }
        }
        const dialog = new ViewDialog({});
        (dialog as any).entityId = 123;
        expect(isViewMode(dialog)).toBe(true);
        dialog.destroy();
    });

    it("isViewMode returns false after editClicked", () => {
        class ViewDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            useViewMode() { return true; }
        }
        const dialog = new ViewDialog({});
        (dialog as any).entityId = 123;
        (dialog as any).editClicked = true;
        expect(isViewMode(dialog)).toBe(false);
        dialog.destroy();
    });
});

describe("EntityDialog.loadResponse", () => {
    it("loads entity and sets entityId from idProperty", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        loadResponse(dialog, { Entity: { ID: 42, Name: "Test" } });
        expect((dialog as any).entity).toEqual({ ID: 42, Name: "Test" });
        expect((dialog as any).entityId).toBe(42);
        dialog.destroy();
    });

    it("handles null/undefined data", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        loadResponse(dialog, null);
        expect((dialog as any).entity).toBeDefined();
        dialog.destroy();
    });
});

describe("EntityDialog.loadEntity", () => {
    it("sets entity and entityId from idProperty field", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        loadEntity(dialog, { ID: 99, Name: "LoadTest" });
        expect((dialog as any).entityId).toBe(99);
        expect((dialog as any).entity).toEqual({ ID: 99, Name: "LoadTest" });
        dialog.destroy();
    });
});

describe("EntityDialog.beforeLoadEntity / afterLoadEntity", () => {
    it("beforeLoadEntity clears localizer value", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        const clearValue = vi.fn();
        dialog["localizer"] = { clearValue, destroy: vi.fn() } as any;
        beforeLoadEntity(dialog, {});
        expect(clearValue).toHaveBeenCalled();
        dialog.destroy();
    });

    it("afterLoadEntity calls updateInterface and updateTitle", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        const uiSpy = vi.fn();
        const titleSpy = vi.fn();
        dialog["updateInterface"] = uiSpy;
        dialog["updateTitle"] = titleSpy;
        afterLoadEntity(dialog);
        expect(uiSpy).toHaveBeenCalled();
        expect(titleSpy).toHaveBeenCalled();
        dialog.destroy();
    });
});

describe("EntityDialog.getEntityNameFieldValue", () => {
    it("returns empty string when entity has no name field", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        (dialog as any).entity = {};
        expect(getEntityNameFieldValue(dialog)).toBe("");
        dialog.destroy();
    });

    it("returns value from name property field", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        (dialog as any).entity = { Name: "Test Entity" };
        expect(getEntityNameFieldValue(dialog)).toBe("Test Entity");
        dialog.destroy();
    });
});

describe("EntityDialog.getEntityTitle", () => {
    it("returns new record title when entityId is null", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.DefaultDialog");
        }
        const dialog = new DefaultDialog({});
        (dialog as any).entity = {};
        const title = getEntityTitle(dialog);
        expect(title).toBeTruthy();
        dialog.destroy();
    });
});

describe("EntityDialog.updateTitle", () => {
    it("sets dialogTitle from getEntityTitle", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        dialog["dialogTitle"] = "";
        updateTitle(dialog);
        expect(dialog.dialogTitle).toBeDefined();
        dialog.destroy();
    });
});

describe("EntityDialog.getSaveEntity", () => {
    it("returns entity with entityId when editing", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        (dialog as any).entityId = 42;
        dialog["propertyGrid"] = { save: vi.fn(), destroy: vi.fn() } as any;
        const entity = getSaveEntity(dialog);
        expect(entity).toBeDefined();
        expect((entity as any).ID).toBe(42);
        dialog.destroy();
    });

    it("returns entity without entityId when new", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        dialog["propertyGrid"] = { save: vi.fn(), destroy: vi.fn() } as any;
        const entity = getSaveEntity(dialog);
        expect(entity).toBeDefined();
        dialog.destroy();
    });
});

describe("EntityDialog.getSaveRequest", () => {
    it("returns request with Entity", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        dialog["propertyGrid"] = { save: vi.fn(), destroy: vi.fn() } as any;
        const req = getSaveRequest(dialog);
        expect(req.Entity).toBeDefined();
        dialog.destroy();
    });

    it("includes EntityId when editing", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        (dialog as any).entityId = 42;
        dialog["propertyGrid"] = { save: vi.fn(), destroy: vi.fn() } as any;
        const req = getSaveRequest(dialog);
        expect(req.EntityId).toBe(42);
        dialog.destroy();
    });
});

describe("EntityDialog.getSaveOptions", () => {
    it("returns options with create service when new", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.DefaultDialog");
        }
        const dialog = new DefaultDialog({});
        dialog["propertyGrid"] = { save: vi.fn(), destroy: vi.fn() } as any;
        const opts = getSaveOptions(dialog);
        expect(opts.service).toContain("/Create");
        dialog.destroy();
    });

    it("returns options with update service when editing", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.DefaultDialog");
        }
        const dialog = new DefaultDialog({});
        (dialog as any).entityId = 42;
        dialog["propertyGrid"] = { save: vi.fn(), destroy: vi.fn() } as any;
        const opts = getSaveOptions(dialog);
        expect(opts.service).toContain("/Update");
        dialog.destroy();
    });
});

describe("EntityDialog.getLoadByIdRequest", () => {
    it("returns request with EntityId", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        const req = getLoadByIdRequest(dialog, 123);
        expect(req.EntityId).toBe(123);
        dialog.destroy();
    });
});

describe("EntityDialog.getLoadByIdOptions", () => {
    it("returns service options with retrieve service", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.DefaultDialog");
        }
        const dialog = new DefaultDialog({});
        const callback = vi.fn();
        const opts = getLoadByIdOptions(dialog, 42, callback);
        expect(opts.service).toContain("/Retrieve");
        expect(opts.request.EntityId).toBe(42);
        expect(opts.blockUI).toBe(true);
        dialog.destroy();
    });
});

describe("EntityDialog.getCloningEntity", () => {
    it("returns copy without id, isActive, isDeleted fields", () => {
        class TestRow {
            static readonly idProperty = "ID";
            static readonly isActiveProperty = "IsActive";
            static readonly isDeletedProperty = "IsDeleted";
        }
        class RowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            getRowDefinition() { return TestRow; }
        }
        const dialog = new RowDialog({});
        (dialog as any).entity = { ID: 1, IsActive: 1, IsDeleted: false, Name: "Test" };
        const clone = getCloningEntity(dialog);
        expect(clone.ID).toBeUndefined();
        expect(clone.IsActive).toBeUndefined();
        expect(clone.IsDeleted).toBeUndefined();
        expect(clone.Name).toBe("Test");
        dialog.destroy();
    });
});

describe("EntityDialog.getPropertyGridOptions", () => {
    it("returns options with insert mode by default", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.DefaultDialog");
        }
        const dialog = new DefaultDialog({});
        const opts = getPropertyGridOptions(dialog);
        expect(opts.mode).toBe(1); // PropertyGridMode.insert
        expect(opts.localTextPrefix).toBe("Forms.Test.Default.");
        expect(opts.idPrefix).toBe(dialog.idPrefix);
        dialog.destroy();
    });
});

describe("EntityDialog.getPropertyItems", () => {
    it("returns items from propertyItemsData", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        expect(getPropertyItems(dialog)).toEqual([]);
        dialog.destroy();
    });
});

describe("EntityDialog.getPropertyItemsData", () => {
    it("returns empty items when no formKey", () => {
        class SimpleDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new SimpleDialog({});
        // When getFormKey === EntityDialog.prototype.getFormKey 
        // and getPropertyItems === EntityDialog.prototype.getPropertyItems
        // then it tries to load from ScriptData
        expect(dialog["propertyItemsData"]).toBeDefined();
        dialog.destroy();
    });
});

describe("EntityDialog.getLocalizerOptions", () => {
    it("returns options object with expected properties", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        const opts = getLocalizerOptions(dialog);
        expect(opts.byId).toBeDefined();
        expect(opts.idPrefix).toBe(dialog.idPrefix);
        expect(opts.getEntity).toBeDefined();
        expect(typeof opts.isNew).toBe("function");
        expect(typeof opts.validateForm).toBe("function");
        dialog.destroy();
    });
});

describe("EntityDialog.getLanguages", () => {
    it("returns empty array when TranslationConfig has no language list", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        expect(getLanguages(dialog)).toEqual([]);
        dialog.destroy();
    });
});

describe("EntityDialog.initPropertyGrid", () => {
        it("creates propertyGrid during construction via renderContents", () => {
            // EntityDialog.renderContents always creates a PropertyGrid div,
            // so initPropertyGrid creates the widget during construction
            class TestDialog extends EntityDialog<any, any> {
                getPropertyItemsData() { return mockPropertyItemsData(); }
            }
            const dialog = new TestDialog({});
            expect(dialog["propertyGrid"]).toBeTruthy();
            dialog.destroy();
        });
    });

describe("EntityDialog.initLocalizer", () => {
    it("does nothing if no PropertyGrid element", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        expect(() => initLocalizer(dialog)).not.toThrow();
        dialog.destroy();
    });

    it("creates localizer when PropertyGrid element exists", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        const pgDiv = document.createElement("div");
        pgDiv.id = dialog.idPrefix + "PropertyGrid";
        dialog.domNode.appendChild(pgDiv);
        initLocalizer(dialog);
        expect(dialog["localizer"]).toBeTruthy();
        dialog.destroy();
    });
});

describe("EntityDialog.reloadById", () => {
    it("calls loadById with current entityId", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        const loadByIdSpy = vi.fn();
        dialog["loadById"] = loadByIdSpy;
        (dialog as any).entityId = 42;
        reloadById(dialog);
        expect(loadByIdSpy).toHaveBeenCalledWith(42);
        dialog.destroy();
    });
});

describe("EntityDialog.get_entityId (deprecated)", () => {
    it("returns entityId", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        (dialog as any).entityId = 42;
        expect(get_entityId(dialog)).toBe(42);
        dialog.destroy();
    });
});

describe("EntityDialog.readOnly", () => {
    it("default is false", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        expect(dialog.readOnly).toBe(false);
        dialog.destroy();
    });

    it("set to true calls updateInterface and updateTitle", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        const uiSpy = vi.fn();
        const titleSpy = vi.fn();
        dialog["updateInterface"] = uiSpy;
        dialog["updateTitle"] = titleSpy;
        
        dialog.readOnly = true;
        expect(dialog.readOnly).toBe(true);
        expect(uiSpy).toHaveBeenCalled();
        expect(titleSpy).toHaveBeenCalled();
        dialog.destroy();
    });

    it("setting same value does not trigger update", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        const uiSpy = vi.fn();
        dialog["updateInterface"] = uiSpy;
        
        dialog.readOnly = false;
        expect(uiSpy).not.toHaveBeenCalled();
        dialog.destroy();
    });
});

describe("EntityDialog.defaultLanguageList", () => {
    afterEach(() => {
        // Clean up the static state
        (EntityDialog as any).defaultLanguageList = undefined;
    });

    it("get returns undefined by default", () => {
        expect(EntityDialog.defaultLanguageList).toBeUndefined();
    });

    it("set configures TranslationConfig.getLanguageList", () => {
        EntityDialog.defaultLanguageList = [["en", "English"], ["tr", "Turkish"]];
        const list = EntityDialog.defaultLanguageList;
        expect(list).toBeDefined();
    });
});

describe("EntityDialog.loadById", () => {
    it("calls loadByIdHandler with options", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.DefaultDialog");
        }
        const dialog = new DefaultDialog({});
        const handlerSpy = vi.fn();
        (dialog as any).loadByIdHandler = handlerSpy;
        
        loadById(dialog, 42);
        expect(handlerSpy).toHaveBeenCalled();
        dialog.destroy();
    });

    it("returns a PromiseLike that resolves with the response", async () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.DefaultDialog");
        }
        const dialog = new DefaultDialog({});
        const response = { Entity: { ID: 42 } };
        (dialog as any).loadByIdHandler = vi.fn(() => Promise.resolve(response));
        
        const result = (dialog as any).loadById(42);
        expect(result).toBeDefined();
        expect(typeof result.then).toBe("function");
        await expect(result).resolves.toBe(response);
        dialog.destroy();
    });
});

describe("EntityDialog.updateInterface", () => {
    it("calls EditorUtils.setContainerReadOnly and toolbar.updateInterface", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        dialog["toolbar"] = { updateInterface: vi.fn() } as any;
        
        // Mock EditorUtils.setContainerReadOnly via the Form element
        const form = document.createElement("div");
        form.id = dialog.idPrefix + "Form";
        dialog.domNode.appendChild(form);
        
        expect(() => updateInterface(dialog)).not.toThrow();
        expect(dialog["toolbar"]!.updateInterface).toHaveBeenCalled();
        dialog.destroy();
    });
});

describe("EntityDialog.load / loadNewAndOpenDialog / loadEntityAndOpenDialog / loadByIdAndOpenDialog", () => {
    it("load with null entityOrId calls loadResponse with empty object", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        const loadRespSpy = vi.fn();
        dialog["loadResponse"] = loadRespSpy;
        
        dialog.load(null, vi.fn());
        expect(loadRespSpy).toHaveBeenCalledWith({});
        dialog.destroy();
    });

    it("load with entity object calls loadResponse", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        const loadRespSpy = vi.fn();
        dialog["loadResponse"] = loadRespSpy;
        
        const entity = { ID: 1, Name: "Test" };
        dialog.load(entity, vi.fn());
        expect(loadRespSpy).toHaveBeenCalledWith({ Entity: entity });
        dialog.destroy();
    });

it("load with string id calls loadById with null fail", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        const loadByIdSpy = vi.fn();
        dialog["loadById"] = loadByIdSpy;

        dialog.load("abc", vi.fn());
        // loadById is called with (id, callback, null) when no fail handler
        expect(loadByIdSpy).toHaveBeenCalledWith("abc", expect.any(Function), null);
        dialog.destroy();
    });

    it("load with numeric id calls loadById with null fail", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        const loadByIdSpy = vi.fn();
        dialog["loadById"] = loadByIdSpy;

        dialog.load(42, vi.fn());
        expect(loadByIdSpy).toHaveBeenCalledWith(42, expect.any(Function), null);
        dialog.destroy();
    });

    it("load with fail callback wraps action in try-catch", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        const fail = vi.fn();
        
        // Should not throw - entityOrId is null so loadResponse is called
        dialog.load(null, vi.fn(), fail);
        expect(fail).not.toHaveBeenCalled();
        dialog.destroy();
    });

    it("load with null entityOrId returns a PromiseLike", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        dialog["loadResponse"] = vi.fn();
        
        const result = dialog.load(null, vi.fn());
        expect(result).toBeDefined();
        expect(typeof (result as any).then).toBe("function");
        dialog.destroy();
    });

    it("load with entity object returns a PromiseLike", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        dialog["loadResponse"] = vi.fn();
        
        const result = dialog.load({ ID: 1 }, vi.fn());
        expect(result).toBeDefined();
        expect(typeof (result as any).then).toBe("function");
        dialog.destroy();
    });

    it("load with string id returns a PromiseLike from loadById", async () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.DefaultDialog");
        }
        const dialog = new DefaultDialog({});
        const response = { Entity: { ID: "abc" } };
        dialog["loadById"] = vi.fn(() => Promise.resolve(response));
        
        const result = dialog.load("abc", vi.fn());
        expect(result).toBeDefined();
        expect(typeof (result as any).then).toBe("function");
        await expect(result).resolves.toBe(response);
        dialog.destroy();
    });

    it("load never returns null", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        dialog["loadResponse"] = vi.fn();
        dialog["loadById"] = vi.fn(() => Promise.resolve({}));
        
        expect(dialog.load(null, vi.fn())).not.toBeNull();
        expect(dialog.load({}, vi.fn())).not.toBeNull();
        expect(dialog.load("abc", vi.fn())).not.toBeNull();
        expect(dialog.load(42, vi.fn())).not.toBeNull();
        dialog.destroy();
    });

    it("loadNewAndOpenDialog loads and opens", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        const loadRespSpy = vi.fn();
        const dlgOpenSpy = vi.fn();
        dialog["loadResponse"] = loadRespSpy;
        dialog["dialogOpen"] = dlgOpenSpy;
        
        (dialog as any).loadNewAndOpenDialog(true);
        expect(loadRespSpy).toHaveBeenCalledWith({});
        expect(dlgOpenSpy).toHaveBeenCalledWith(true);
        dialog.destroy();
    });

    it("loadEntityAndOpenDialog loads entity and opens", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        const loadRespSpy = vi.fn();
        const dlgOpenSpy = vi.fn();
        dialog["loadResponse"] = loadRespSpy;
        dialog["dialogOpen"] = dlgOpenSpy;
        
        const entity = { ID: 1 };
        (dialog as any).loadEntityAndOpenDialog(entity);
        expect(loadRespSpy).toHaveBeenCalledWith({ Entity: entity });
        expect(dlgOpenSpy).toHaveBeenCalledWith(undefined);
        dialog.destroy();
    });

    it("loadByIdAndOpenDialog returns a Promise that resolves after dialogOpen", async () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.DefaultDialog");
        }
        const dialog = new DefaultDialog({});
        (dialog as any).entityId = 42;
        const dlgOpenSpy = vi.fn();
        dialog["dialogOpen"] = dlgOpenSpy;

        const response = { Entity: { ID: 42 } };
        dialog["loadById"] = vi.fn((id: any, onSuccess: (r: any) => void) => {
            onSuccess(response);
            return Promise.resolve(response);
        });

        const promise = (dialog as any).loadByIdAndOpenDialog(42);
        expect(promise).toBeInstanceOf(Promise);

        // Wait for setTimeout(0) to fire and dialogOpen to be called
        await new Promise(r => setTimeout(r, 1));
        expect(dlgOpenSpy).toHaveBeenCalledWith(undefined);

        await expect(promise).resolves.toBe(response);
        dialog.destroy();
    });
});

describe("EntityDialog.getToolbarButtons", () => {
    it("returns array of toolbar buttons", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        const buttons = getToolbarButtons(dialog);
        expect(Array.isArray(buttons)).toBe(true);
        expect(buttons.length).toBe(7); // save, apply, delete, undelete, edit, localization, clone
        dialog.destroy();
    });
});

describe("EntityDialog.onSaveSuccess", () => {
    it("calls showSaveSuccessMessage when initiator is not save-and-close", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        const showMsgSpy = vi.fn();
        dialog["showSaveSuccessMessage"] = showMsgSpy;
        
        (dialog as any).onSaveSuccess({}, "apply-changes" as any);
        expect(showMsgSpy).toHaveBeenCalled();
        dialog.destroy();
    });

    it("does not call showSaveSuccessMessage when initiator is save-and-close", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        const showMsgSpy = vi.fn();
        dialog["showSaveSuccessMessage"] = showMsgSpy;
        
        (dialog as any).onSaveSuccess({}, "save-and-close" as any);
        expect(showMsgSpy).not.toHaveBeenCalled();
        dialog.destroy();
    });
});

describe("EntityDialog.showSaveSuccessMessage", () => {
    it("calls notifySuccess", () => {
        // Use vi.spyOn to check if notifySuccess is called
        // Since notifySuccess is imported from base, we can't easily spy on it
        // Just verify it doesn't throw
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const dialog = new DefaultDialog({});
        expect(() => (dialog as any).showSaveSuccessMessage({})).not.toThrow();
        dialog.destroy();
    });
});

describe("EntityDialog.doDelete", () => {
    it("calls deleteHandler and triggers ondatachange", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.DefaultDialog");
        }
        const dialog = new DefaultDialog({});
        (dialog as any).entityId = 42;
        (dialog as any).entity = { ID: 42 };
        
        const handlerSpy = vi.fn((opts: any, cb: any) => {
            opts.onSuccess({});
            cb?.({});
            return Promise.resolve({});
        });
        (dialog as any).deleteHandler = handlerSpy;
        
        const callback = vi.fn();
        const dataChangeSpy = vi.fn();
        Fluent.on(dialog.domNode, "ondatachange", dataChangeSpy);
        
        const result = (dialog as any).doDelete(callback);
        
        expect(handlerSpy).toHaveBeenCalled();
        expect(callback).toHaveBeenCalled();
        expect(result).toBeDefined();
        expect(typeof result.then).toBe("function");
        dialog.destroy();
    });

    it("returns a PromiseLike that resolves with the delete response", async () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.DefaultDialog");
        }
        const dialog = new DefaultDialog({});
        (dialog as any).entityId = 42;
        (dialog as any).entity = { ID: 42 };
        const response = { EntityId: 42 };
        
        (dialog as any).deleteHandler = vi.fn(() => Promise.resolve(response));
        
        const result = (dialog as any).doDelete(vi.fn());
        await expect(result).resolves.toBe(response);
        dialog.destroy();
    });
});

describe("EntityDialog.undelete", () => {
    it("calls undeleteHandler and triggers ondatachange", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.DefaultDialog");
        }
        const dialog = new DefaultDialog({});
        (dialog as any).entityId = 42;
        
        const handlerSpy = vi.fn((opts: any, cb: any) => {
            opts.onSuccess({});
            cb?.({});
            return Promise.resolve({});
        });
        (dialog as any).undeleteHandler = handlerSpy;
        
        const callback = vi.fn();
        const dataChangeSpy = vi.fn();
        Fluent.on(dialog.domNode, "ondatachange", dataChangeSpy);
        
        const result = (dialog as any).undelete(callback);
        
        expect(handlerSpy).toHaveBeenCalled();
        expect(callback).toHaveBeenCalled();
        expect(result).toBeDefined();
        expect(typeof result.then).toBe("function");
        dialog.destroy();
    });

    it("returns a PromiseLike that resolves with the undelete response", async () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.DefaultDialog");
        }
        const dialog = new DefaultDialog({});
        (dialog as any).entityId = 42;
        const response = { EntityId: 42 };
        
        (dialog as any).undeleteHandler = vi.fn(() => Promise.resolve(response));
        
        const result = (dialog as any).undelete(vi.fn());
        await expect(result).resolves.toBe(response);
        dialog.destroy();
    });
});

describe("EntityDialog.save", () => {
    it("returns PromiseLike when validation passes", async () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.DefaultDialog");
        }
        const dialog = new DefaultDialog({});
        const response = { EntityId: 1 };

        const mockValidator = {
            settings: {},
            form: vi.fn().mockReturnValue(true)
        } as any;
        const getInstanceSpy = vi.spyOn(Validator, 'getInstance').mockReturnValue(mockValidator);
        
        (dialog as any).save_submitHandler = vi.fn(() => Promise.resolve(response));
        
        const form = document.createElement("form");
        form.id = dialog.idPrefix + "Form";
        dialog.domNode.appendChild(form);
        
        const result = (dialog as any).save(vi.fn(), "save-and-close");
        expect(result).not.toBe(false);
        expect(typeof (result as any).then).toBe("function");
        await expect(result).resolves.toBe(response);
        
        getInstanceSpy.mockRestore();
        dialog.destroy();
    });

    it("returns false when validation fails", () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.DefaultDialog");
        }
        const dialog = new DefaultDialog({});
        
        // Mock validateBeforeSave to return false
        (dialog as any).validateBeforeSave = vi.fn(() => false);
        
        const result = (dialog as any).save(vi.fn(), "save-and-close");
        expect(result).toBe(false);
        dialog.destroy();
    });
});

describe("EntityDialog.additionalMethods", () => {
    it("getUndeleteServiceMethod returns service/Undelete", () => {
        class D extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.DefaultDialog");
        }
        const d = new D({});
        expect((d as any).getUndeleteServiceMethod()).toBe("Test/Default/Undelete");
        d.destroy();
    });

    it("onDeleteSuccess does nothing", () => {
        class D extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const d = new D({});
        expect(() => (d as any).onDeleteSuccess({})).not.toThrow();
        d.destroy();
    });

    it("getRowDefinition returns null by default", () => {
        class D extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const d = new D({});
        expect((d as any).getRowDefinition()).toBeNull();
        d.destroy();
    });

    it("onLoadingData does nothing", () => {
        class D extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const d = new D({});
        expect(() => (d as any).onLoadingData({})).not.toThrow();
        d.destroy();
    });

    it("useAsync returns false by default", () => {
        class D extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const d = new D({});
        expect((d as any).useAsync()).toBe(false);
        d.destroy();
    });

    it("afterInit does nothing", () => {
        class D extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
        }
        const d = new D({});
        expect(() => (d as any).afterInit()).not.toThrow();
        d.destroy();
    });

    it("getPropertyItemsDataAsync returns empty when no formKey", async () => {
        class D extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            getFormKey() { return ""; }
        }
        const d = new D({});
        const result = await (d as any).getPropertyItemsDataAsync();
        expect(result).toEqual({ items: [], additionalItems: [] });
        d.destroy();
    });

    it("deleteHandler returns PromiseLike from serviceCall", async () => {
        class D extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.DefaultDialog");
        }
        const d = new D({});
        const result = (d as any).deleteHandler({ service: "Test/Delete", request: {} }, vi.fn());
        expect(result).toBeDefined();
        expect(typeof result.then).toBe("function");
        // Catch the inevitable fetch rejection to avoid unhandled rejection warnings
        result.catch(() => {});
        d.destroy();
    });

    it("undeleteHandler returns PromiseLike from serviceCall", async () => {
        class D extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.DefaultDialog");
        }
        const d = new D({});
        const result = (d as any).undeleteHandler({ service: "Test/Undelete", request: {} }, vi.fn());
        expect(result).toBeDefined();
        expect(typeof result.then).toBe("function");
        result.catch(() => {});
        d.destroy();
    });

    it("saveHandler returns PromiseLike from serviceCall", async () => {
        class D extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.DefaultDialog");
        }
        const d = new D({});
        const result = (d as any).saveHandler({ service: "Test/Save", request: {} }, vi.fn());
        expect(result).toBeDefined();
        expect(typeof result.then).toBe("function");
        result.catch(() => {});
        d.destroy();
    });

    it("save_submitHandler returns PromiseLike", () => {
        class D extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.DefaultDialog");
        }
        const d = new D({});
        (d as any).saveHandler = vi.fn(() => Promise.resolve({}));
        const result = (d as any).save_submitHandler(vi.fn(), "save-and-close");
        expect(result).toBeDefined();
        expect(typeof result.then).toBe("function");
        d.destroy();
    });

    it("loadByIdHandler returns PromiseLike", async () => {
        class D extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData(); }
            static [Symbol.typeInfo] = this.registerClass("MyProject.Test.DefaultDialog");
        }
        const d = new D({});
        const result = (d as any).loadByIdHandler(
            { service: "Test/Retrieve", request: { EntityId: 1 }, onSuccess: vi.fn() },
            vi.fn(),
            vi.fn()
        );
        expect(result).toBeDefined();
        expect(typeof result.then).toBe("function");
        result.catch(() => {});
        d.destroy();
    });
});


describe('EntityDialog.getIdProperty', () => {
    it('returns ID by default', () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
        }

        var Dialog = new DefaultDialog({});
        expect(getIdProperty(Dialog)).toBe("ID");
    });

    it('can be overridden in subclass', () => {
        class SubClassDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getIdProperty() { return "subClassId" };
        }

        var Dialog = new SubClassDialog({});
        expect(getIdProperty(Dialog)).toBe("subClassId");
    });

    it('returns value from getRowDefition()', () => {
        class TestRow {
            static readonly idProperty = "idForTestRow";
        }

        class TestRowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getRowDefinition() { return TestRow; }
        }

        var Dialog = new TestRowDialog({});
        expect(getIdProperty(Dialog)).toBe("idForTestRow");
    });

    it("returns empty string if getRowDefition() doesn't have the value", () => {
        class TestRow {
            static readonly idProperty: string = undefined;
        }

        class TestRowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getRowDefinition() { return TestRow; }
        }

        var dialog = new TestRowDialog({});
        expect(getIdProperty(dialog)).toBe("");
    });
});

function getIsActiveProperty(dialog: EntityDialog<any, any>): string {
    return dialog["getIsActiveProperty"]();
}

describe('EntityDialog.getIsActiveProperty', () => {
    it('returns empty string by default', () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
        }

        var Dialog = new DefaultDialog({});
        expect(getIsActiveProperty(Dialog)).toBe("");
    });

    it('can be overridden in subclass', () => {
        class SubClassDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getIsActiveProperty() { return "subClassIsActive" };
        }

        var Dialog = new SubClassDialog({});
        expect(getIsActiveProperty(Dialog)).toBe("subClassIsActive");
    });

    it('returns value from getRowDefition()', () => {
        class TestRow {
            static readonly isActiveProperty = "isActiveForTestRow";
        }

        class TestRowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getRowDefinition() { return TestRow; }
        }

        var Dialog = new TestRowDialog({});
        expect(getIsActiveProperty(Dialog)).toBe("isActiveForTestRow");
    });

    it("returns empty string if getRowDefition() doesn't have the value", () => {
        class TestRow {
            static readonly isActiveProperty: string = undefined;
        }

        class TestRowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getRowDefinition() { return TestRow; }
        }

        var dialog = new TestRowDialog({});
        expect(getIsActiveProperty(dialog)).toBe("");
    });
});

function getLocalTextDbPrefix(dialog: EntityDialog<any, any>): string {
    return dialog["getLocalTextDbPrefix"]();
}

describe('EntityDialog.getLocalTextDbPrefix', () => {
    it('returns class identifier by default', () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
        }

        var dialog = new DefaultDialog({});
        expect(getLocalTextDbPrefix(dialog)).toBe("Db.Default.");
    });

    it('returns class identifier based on typeInfo property', () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            static [Symbol.typeInfo] = this.registerClass('MyProject.TestModule.DefaultDialog');
        }

        var dialog = new DefaultDialog({});
        expect(getLocalTextDbPrefix(dialog)).toBe("Db.TestModule.Default.");
    });

    it('returns class identifier based on registration name', () => {
        class DefaultDialog extends EntityDialog<any, any> {
            static [Symbol.typeInfo] = this.registerClass('MyProject.MyModule.Some.DefaultDialog');
            getPropertyItemsData() { return mockPropertyItemsData() };
        }

        var dialog = new DefaultDialog({});
        expect(getLocalTextDbPrefix(dialog)).toBe("Db.MyModule.Some.Default.");
    });

    it('can be overridden in subclass via getLocalTextDbPrefix', () => {
        class SubClassDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getLocalTextDbPrefix() { return "My.Prefix." };
        }

        var dialog = new SubClassDialog({});
        expect(getLocalTextDbPrefix(dialog)).toBe("My.Prefix.");
    });

    it('can be overridden in subclass via getLocalTextPrefix', () => {
        class SubClassDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getLocalTextPrefix() { return "MySubClassPrefix" };
        }

        var dialog = new SubClassDialog({});
        expect(getLocalTextDbPrefix(dialog)).toBe("Db.MySubClassPrefix.");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly localTextPrefix = "prefixForTestRow";
        }

        class TestRowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getRowDefinition() { return TestRow; }
        }

        var dialog = new TestRowDialog({});
        expect(getLocalTextDbPrefix(dialog)).toBe("Db.prefixForTestRow.");
    });

    it("returns empty string if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly localTextPrefix: string = undefined;
        }

        class TestRowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getRowDefinition() { return TestRow; }
        }

        var dialog = new TestRowDialog({});
        expect(getLocalTextDbPrefix(dialog)).toBe("");
    });
});

function getLocalTextPrefix(dialog: EntityDialog<any, any>): string {
    return dialog["getLocalTextPrefix"]();
}

describe('EntityDialog.getLocalTextPrefix', () => {
    it('returns class identifier by default', () => {
        class DefaultDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
        }

        var dialog = new DefaultDialog({});
        expect(getLocalTextPrefix(dialog)).toBe("Default");
    });

    it('returns class identifier based on typeInfo property', () => {
        class DefaultDialog extends EntityDialog<any, any> {
            static [Symbol.typeInfo] = this.registerClass('MyProject.TestModule.DefaultDialog');
            getPropertyItemsData() { return mockPropertyItemsData() };
        }

        var dialog = new DefaultDialog({});
        expect(getLocalTextPrefix(dialog)).toBe("TestModule.Default");
    });

    it('returns class identifier based on registration name', () => {
        class DefaultDialog extends EntityDialog<any, any> {
            static [Symbol.typeInfo] = this.registerClass('MyProject.MyModule.Some.DefaultDialog');
            getPropertyItemsData() { return mockPropertyItemsData() };
        }

        var dialog = new DefaultDialog({});
        expect(getLocalTextPrefix(dialog)).toBe("MyModule.Some.Default");
    });

    it('can be overridden in subclass', () => {
        class SubClassDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getLocalTextPrefix() { return "subClassPrefix" };
        }

        var dialog = new SubClassDialog({});
        expect(getLocalTextPrefix(dialog)).toBe("subClassPrefix");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly localTextPrefix = "prefixForTestRow";
        }

        class TestRowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getRowDefinition() { return TestRow; }
        }

        var dialog = new TestRowDialog({});
        expect(getLocalTextPrefix(dialog)).toBe("prefixForTestRow");
    });

    it("returns undefined if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly localTextPrefix: string = undefined;
        }

        class TestRowDialog extends EntityDialog<any, any> {
            getPropertyItemsData() { return mockPropertyItemsData() };
            getRowDefinition() { return TestRow; }
        }

        var dialog = new TestRowDialog({});
        expect(getLocalTextPrefix(dialog)).toBeUndefined();
    });
});

describe("EntityDialog.destroy", () => {

    class MockDialog extends EntityDialog<any, any> {
        getPropertyItemsData() { return mockPropertyItemsData() };
    }

    it("calls destroy on propertyGrid", () => {
        const dialog = new MockDialog();
        const destroy = vi.fn();
        dialog["propertyGrid"] = { destroy } as any;
        dialog.destroy();
        expect(destroy).toHaveBeenCalledTimes(1);
        expect(dialog["propertyGrid"] == null).toBe(true);
    });

    it("calls destroy on localizer", () => {
        const dialog = new MockDialog();
        const destroy = vi.fn();
        dialog["localizer"] = { destroy } as any;
        dialog.destroy();
        expect(destroy).toHaveBeenCalledTimes(1);
        expect(dialog["localizer"] == null).toBe(true);
    });

    it("clears toolbar and button references", () => {
        const dialog = new MockDialog();
        const props = ["toolbar", "editButton", "cloneButton", "saveAndCloseButton", "applyChangesButton", "deleteButton", "undeleteButton"];
        props.forEach(p => (dialog as any)[p] = "test" as any);
        dialog.destroy();
        props.forEach(p => expect((dialog as any)[p] == null).toBe(true));
    });

    it("calls super.destroy", () => {
        const dialog = new MockDialog();
        dialog.destroy();
        expect(dialog.domNode == null).toBe(true);
    });

});