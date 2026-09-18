import * as corelib from "@serenity-is/corelib";
import { CategoryRow, ProductColumns, ProductDialog, ProductRow, ProductService } from "@serenity-is/demo.northwind";
import { mockAdmin, mockDynamicData, mockFetch, mockGridSize, mockRowLookup, unmockFetch } from "test-utils";
import { LookupFilterByMultipleForm } from "../../Modules/ServerTypes/Demo";
import initPage, {
    LookupFilterByMultipleDialog,
    LookupFilterByMultipleGrid,
    ProduceSeafoodCategoryEditor
} from "../../Modules/Editors/LookupFilterByMultipleValues/LookupFilterByMultipleValuesPage";

beforeAll(() => {
    mockDynamicData();
    mockAdmin();
    mockGridSize();
    mockRowLookup(CategoryRow, [
        { CategoryID: 1, CategoryName: "Beverages" },
        { CategoryID: 2, CategoryName: "Produce" },
        { CategoryID: 3, CategoryName: "Seafood" },
        { CategoryID: 4, CategoryName: "Condiments" }
    ]);
    mockFetch({ "*": () => ({ Entities: [], TotalCount: 0 }) });
});

afterAll(() => {
    unmockFetch();
});

afterEach(() => {
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

describe("LookupFilterByMultipleValuesPage", () => {
    it("initializes the grid page", () => {
        const spy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(spy).toHaveBeenCalledWith(LookupFilterByMultipleGrid);
    });
});

describe("LookupFilterByMultipleGrid", () => {
    it("wires up columns, dialog, row and service", () => {
        const grid = new LookupFilterByMultipleGrid({});
        expect(grid["getColumnsKey"]()).toBe(ProductColumns.columnsKey);
        expect(grid["getDialogType"]()).toBe(LookupFilterByMultipleDialog);
        expect(grid["getRowDefinition"]()).toBe(ProductRow);
        expect(grid["getService"]()).toBe(ProductService.baseUrl);
        grid.destroy();
    });

    it("filters products by produce and seafood criteria", () => {
        const grid = new LookupFilterByMultipleGrid({});
        grid["view"].params = {} as any;
        grid["setViewParams"]();
        expect(grid["view"].params.Criteria).toBeTruthy();
        grid.destroy();
    });
});

describe("LookupFilterByMultipleDialog", () => {
    it("extends the product dialog and uses the custom form", () => {
        const dialog = new LookupFilterByMultipleDialog({});
        expect(dialog instanceof ProductDialog).toBe(true);
        expect(dialog["getFormKey"]()).toBe(LookupFilterByMultipleForm.formKey);
        dialog.destroy();
    });
});

describe("ProduceSeafoodCategoryEditor", () => {
    it("uses the category lookup key", () => {
        const editor = new ProduceSeafoodCategoryEditor({});
        expect(editor["getLookupKey"]()).toBe(CategoryRow.lookupKey);
        editor.destroy();
    });

    it("filters items to produce and seafood", () => {
        const editor = new ProduceSeafoodCategoryEditor({});
        const lookup = CategoryRow.getLookup();
        const items = editor["getItems"](lookup);
        expect(items.length).toBe(2);
        expect(items.map((x: any) => x.CategoryName).sort()).toEqual(["Produce", "Seafood"]);
        editor.destroy();
    });
});
