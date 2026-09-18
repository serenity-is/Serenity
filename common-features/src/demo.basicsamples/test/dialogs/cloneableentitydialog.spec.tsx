import * as corelib from "@serenity-is/corelib";
import { ProductColumns, ProductRow, ProductService } from "@serenity-is/demo.northwind";
import { EntityDialogWrapper, mockAdmin, mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import initPage, { CloneableEntityDialog, CloneableEntityGrid } from "../../Modules/Dialogs/CloneableEntityDialog/CloneableEntityDialogPage";

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

describe("CloneableEntityDialogPage", () => {
    it("initializes the grid page", () => {
        const spy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(spy).toHaveBeenCalledWith(CloneableEntityGrid);
    });
});

describe("CloneableEntityGrid", () => {
    it("wires up columns, dialog, row and service", () => {
        const grid = new CloneableEntityGrid({});
        expect(grid["getColumnsKey"]()).toBe(ProductColumns.columnsKey);
        expect(grid["getDialogType"]()).toBe(CloneableEntityDialog);
        expect(grid["getRowDefinition"]()).toBe(ProductRow);
        expect(grid["getService"]()).toBe(ProductService.baseUrl);
        grid.destroy();
    });
});

describe("CloneableEntityDialog", () => {
    it("clones with a suffix and clears non cloned fields", () => {
        const wrapper = new EntityDialogWrapper(new CloneableEntityDialog());
        wrapper.actual.loadEntityAndOpenDialog({
            ProductName: "Chai",
            ProductImage: "chai.jpg",
            UnitsInStock: 5,
            UnitsOnOrder: 2
        });

        const clone = wrapper.actual["getCloningEntity"]();
        expect(clone.ProductName).toBe("Chai (Clone)");
        expect(clone.ProductImage).toBeNull();
        expect(clone.UnitsInStock).toBe(0);
        expect(clone.UnitsOnOrder).toBe(0);
        wrapper.actual.destroy();
    });

    it("does not duplicate an existing clone suffix", () => {
        const wrapper = new EntityDialogWrapper(new CloneableEntityDialog());
        wrapper.actual.loadEntityAndOpenDialog({ ProductName: "Chai (Clone)" });
        expect(wrapper.actual["getCloningEntity"]().ProductName).toBe("Chai (Clone)");
        wrapper.actual.destroy();
    });

    it("adds the clone suffix when the name is missing", () => {
        const wrapper = new EntityDialogWrapper(new CloneableEntityDialog());
        wrapper.actual.loadEntityAndOpenDialog({ ProductID: 1 });
        expect(wrapper.actual["getCloningEntity"]().ProductName).toBe(" (Clone)");
        wrapper.actual.destroy();
    });

    it("shows the clone button in edit mode", () => {
        const wrapper = new EntityDialogWrapper(new CloneableEntityDialog());
        wrapper.actual.loadEntityAndOpenDialog({ ProductID: 1, ProductName: "Chai" });
        const toggle = vi.spyOn(wrapper.actual["cloneButton"], "toggle");
        wrapper.actual["updateInterface"]();
        expect(toggle).toHaveBeenCalledWith(true);
        wrapper.actual.destroy();
    });

    it("hides the clone button in new mode", () => {
        const wrapper = new EntityDialogWrapper(new CloneableEntityDialog());
        wrapper.actual.loadNewAndOpenDialog();
        const toggle = vi.spyOn(wrapper.actual["cloneButton"], "toggle");
        wrapper.actual["updateInterface"]();
        expect(toggle).toHaveBeenCalledWith(false);
        wrapper.actual.destroy();
    });
});
