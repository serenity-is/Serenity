import * as corelib from "@serenity-is/corelib";
import { SupplierDialog, SupplierGrid } from "@serenity-is/demo.northwind";
import { EntityDialogWrapper, mockAdmin, mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import { afterAll, afterEach, beforeAll, describe, expect, it, vi } from "vitest";
import initPage, { ReadOnlyDialog, ReadOnlyGrid } from "../../Modules/Dialogs/ReadOnlyDialog/ReadOnlyDialogPage";

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

describe("ReadOnlyDialogPage", () => {
    it("initializes the grid page", () => {
        const spy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(spy).toHaveBeenCalledWith(ReadOnlyGrid);
    });
});

describe("ReadOnlyGrid", () => {
    it("extends the supplier grid, uses readonly dialog and removes add", () => {
        const grid = new ReadOnlyGrid({});
        expect(grid instanceof SupplierGrid).toBe(true);
        expect(grid["getDialogType"]()).toBe(ReadOnlyDialog);
        const buttons = grid["getButtons"]();
        expect(buttons.findIndex((x: any) => x.action == "add")).toBe(-1);
        grid.destroy();
    });
});

describe("ReadOnlyDialog", () => {
    it("extends the supplier dialog", () => {
        const dialog = new ReadOnlyDialog({});
        expect(dialog instanceof SupplierDialog).toBe(true);
        dialog.destroy();
    });

    it("removes save-and-close and apply-changes buttons", () => {
        const dialog = new ReadOnlyDialog({});
        const buttons = dialog["getToolbarButtons"]();
        expect(buttons.findIndex((x: any) => x.action == "save-and-close")).toBe(-1);
        expect(buttons.findIndex((x: any) => x.action == "apply-changes")).toBe(-1);
        dialog.destroy();
    });

    it("makes the dialog readonly and hides delete button", () => {
        const wrapper = new EntityDialogWrapper(new ReadOnlyDialog());
        wrapper.actual.loadEntityAndOpenDialog({ SupplierID: 1, CompanyName: "Exotic Liquids" });
        const hide = vi.spyOn(wrapper.actual["deleteButton"], "hide");
        wrapper.actual["updateInterface"]();
        expect(hide).toHaveBeenCalled();
        const inputs = wrapper.actual.domNode.querySelectorAll(".editor");
        inputs.forEach(el => expect((el as HTMLInputElement).readOnly || (el as HTMLInputElement).disabled).toBe(true));
        expect(wrapper.actual.domNode.querySelectorAll("sup").length).toBeGreaterThanOrEqual(0);
        wrapper.actual.destroy();
    });

    it("returns a view title in edit mode", () => {
        const wrapper = new EntityDialogWrapper(new ReadOnlyDialog());
        wrapper.actual.loadEntityAndOpenDialog({ SupplierID: 1, CompanyName: "Exotic Liquids" });
        expect(wrapper.actual["getEntityTitle"]()).toContain("Exotic Liquids");
        expect(wrapper.actual["getEntityTitle"]()).toContain("View ");
        wrapper.actual.destroy();
    });

    it("returns a fallback title in new mode", () => {
        const wrapper = new EntityDialogWrapper(new ReadOnlyDialog());
        wrapper.actual.loadNewAndOpenDialog();
        expect(wrapper.actual["getEntityTitle"]()).toContain("new record mode");
        wrapper.actual.destroy();
    });
});
