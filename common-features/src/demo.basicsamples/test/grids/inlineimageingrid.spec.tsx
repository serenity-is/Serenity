import * as corelib from "@serenity-is/corelib";
import { ProductDialog, ProductRow, ProductService } from "@serenity-is/demo.northwind";
import { formatterContext } from "@serenity-is/sleekgrid";
import { mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import initPage, { InlineImageFormatter, InlineImageInGrid } from "../../Modules/Grids/InlineImageInGrid/InlineImageInGridPage";
import { InlineImageInGridColumns } from "../../Modules/ServerTypes/Demo";

beforeAll(() => {
    mockDynamicData();
    mockGridSize();
});

beforeEach(() => {
    mockFetch({ "*": () => ({ Entities: [], TotalCount: 0 }) });
});

afterEach(() => {
    unmockFetch();
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

describe("InlineImageInGridPage", () => {
    it("initializes the grid page", () => {
        const spy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(spy).toHaveBeenCalledWith(InlineImageInGrid);
    });
});

describe("InlineImageInGrid", () => {
    it("wires up columns, dialog, row and service", () => {
        const grid = new InlineImageInGrid({});
        expect(grid["getColumnsKey"]()).toBe(InlineImageInGridColumns.columnsKey);
        expect(grid["getDialogType"]()).toBe(ProductDialog);
        expect(grid["getRowDefinition"]()).toBe(ProductRow);
        expect(grid["getService"]()).toBe(ProductService.baseUrl);
        expect(grid["getSlickOptions"]().rowHeight).toBe(150);
        grid.destroy();
    });
});

describe("InlineImageFormatter", () => {
    it("returns empty string for empty values", () => {
        const formatter = new InlineImageFormatter();
        expect(formatter.format(formatterContext({ value: null }))).toBe("");
        expect(formatter.format(formatterContext({ value: "" }))).toBe("");
    });

    it("renders a link and image for a file value", () => {
        const result = new InlineImageFormatter().format(formatterContext({ value: "image.jpg" }));
        const div = document.createElement("div");
        div.append(result as any);
        const anchor = div.querySelector("a.inline-image");
        expect(anchor).toBeTruthy();
        expect(anchor.getAttribute("href")).toContain("upload/image.jpg");
        expect(div.querySelector("img")).toBeTruthy();
    });

    it("uses the fileProperty on the item", () => {
        const result = new InlineImageFormatter({ fileProperty: "ProductImage" })
            .format(formatterContext({ value: null, item: { ProductImage: "product.jpg" } }));
        const div = document.createElement("div");
        div.append(result as any);
        expect(div.querySelector("a.inline-image").getAttribute("href")).toContain("upload/product.jpg");
    });

    it("derives thumbnail name when thumb is set", () => {
        const result = new InlineImageFormatter({ thumb: true }).format(formatterContext({ value: "image.jpg" }));
        const div = document.createElement("div");
        div.append(result as any);
        expect(div.querySelector("a.inline-image").getAttribute("href")).toContain("upload/image_t.jpg");
    });

    it("adds the referenced field to the column", () => {
        const column: any = {};
        new InlineImageFormatter({ fileProperty: "ProductImage" }).initializeColumn(column);
        expect(column.referencedFields).toEqual(["ProductImage"]);

        const existing: any = { referencedFields: ["Other"] };
        new InlineImageFormatter({ fileProperty: "ProductImage" }).initializeColumn(existing);
        expect(existing.referencedFields).toEqual(["Other", "ProductImage"]);
    });

    it("does nothing to the column when there is no fileProperty", () => {
        const column: any = {};
        new InlineImageFormatter().initializeColumn(column);
        expect(column.referencedFields).toBeUndefined();
    });
});
