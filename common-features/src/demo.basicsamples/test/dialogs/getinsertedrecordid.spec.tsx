import * as corelib from "@serenity-is/corelib";
import { CategoryDialog, CategoryGrid, CategoryService } from "@serenity-is/demo.northwind";
import { EntityDialogWrapper, mockAdmin, mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import { afterAll, afterEach, beforeAll, describe, expect, it, vi } from "vitest";
import initPage, { GetInsertedRecordIdDialog, GetInsertedRecordIdGrid } from "../../Modules/Dialogs/GetInsertedRecordId/GetInsertedRecordIdPage";

beforeAll(() => {
    mockDynamicData();
    mockAdmin();
    mockGridSize();
    mockFetch({ "*": () => ({ Entities: [], TotalCount: 0 }) });
});

afterAll(() => {
    unmockFetch();
});

afterEach(() => {
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

describe("GetInsertedRecordIdPage", () => {
    it("initializes the grid page", () => {
        const spy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(spy).toHaveBeenCalledWith(GetInsertedRecordIdGrid);
    });
});

describe("GetInsertedRecordIdGrid", () => {
    it("uses the custom dialog type", () => {
        const grid = new GetInsertedRecordIdGrid({});
        expect(grid instanceof CategoryGrid).toBe(true);
        expect(grid["getDialogType"]()).toBe(GetInsertedRecordIdDialog);
        grid.destroy();
    });
});

describe("GetInsertedRecordIdDialog", () => {
    it("extends the category dialog", () => {
        const dialog = new GetInsertedRecordIdDialog({});
        expect(dialog instanceof CategoryDialog).toBe(true);
        dialog.destroy();
    });

    it("shows the inserted id and retrieves the record in new mode", () => {
        const wrapper = new EntityDialogWrapper(new GetInsertedRecordIdDialog());
        wrapper.actual.loadNewAndOpenDialog();
        const notifySuccess = vi.spyOn(corelib, "notifySuccess").mockImplementation((() => ({})) as any);
        const notifyInfo = vi.spyOn(corelib, "notifyInfo").mockImplementation((() => ({})) as any);
        const retrieve = vi.spyOn(CategoryService, "Retrieve").mockImplementation(((request: any, onSuccess: any) => {
            onSuccess({ Entity: { CategoryID: 5, CategoryName: "Beverages" } });
            return Promise.resolve({}) as any;
        }) as any);

        wrapper.actual["onSaveSuccess"]({ EntityId: 5 } as any, "save" as any);

        expect(notifySuccess).toHaveBeenCalledWith(expect.stringContaining("5"));
        expect(retrieve).toHaveBeenCalledWith({ EntityId: 5 }, expect.any(Function));
        expect(notifyInfo).toHaveBeenCalledWith(expect.stringContaining("Beverages"));
        wrapper.actual.destroy();
    });

    it("does nothing on update", () => {
        const wrapper = new EntityDialogWrapper(new GetInsertedRecordIdDialog());
        wrapper.actual.loadEntityAndOpenDialog({ CategoryID: 5, CategoryName: "Beverages" });
        const notifySuccess = vi.spyOn(corelib, "notifySuccess").mockImplementation((() => ({})) as any);
        wrapper.actual["onSaveSuccess"]({ EntityId: 5 } as any, "save" as any);
        expect(notifySuccess).not.toHaveBeenCalled();
        wrapper.actual.destroy();
    });
});
