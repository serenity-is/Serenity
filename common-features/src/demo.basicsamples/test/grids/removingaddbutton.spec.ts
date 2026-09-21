import * as corelib from "@serenity-is/corelib";
import { SupplierGrid } from "@serenity-is/demo.northwind";
import { mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import { afterEach, beforeAll, beforeEach, describe, expect, it, vi } from "vitest";
import initPage, { RemovingAddButton } from "../../Modules/Grids/RemovingAddButton/RemovingAddButtonPage";

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

describe("RemovingAddButtonPage", () => {
    it("initializes the grid page", () => {
        const spy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(spy).toHaveBeenCalledWith(RemovingAddButton);
    });
});

describe("RemovingAddButton", () => {
    it("extends the supplier grid", () => {
        const grid = new RemovingAddButton({});
        expect(grid instanceof SupplierGrid).toBe(true);
        grid.destroy();
    });

    it("removes the add button from the toolbar", () => {
        const grid = new RemovingAddButton({});
        const buttons = grid["getButtons"]();
        expect(buttons.findIndex((x: any) => x.action == "add")).toBe(-1);
        grid.destroy();
    });
});
