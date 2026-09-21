import * as corelib from "@serenity-is/corelib";
import { OrderGrid } from "@serenity-is/demo.northwind";
import { mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import { afterEach, beforeAll, beforeEach, describe, expect, it, vi } from "vitest";
import initPage, { WrappedHeadersGrid } from "../../Modules/Grids/WrappedHeaders/WrappedHeadersPage";

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

describe("WrappedHeadersPage", () => {
    it("initializes the grid page", () => {
        const spy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(spy).toHaveBeenCalledWith(WrappedHeadersGrid);
    });
});

describe("WrappedHeadersGrid", () => {
    it("extends the order grid", () => {
        const grid = new WrappedHeadersGrid({});
        expect(grid instanceof OrderGrid).toBe(true);
        grid.destroy();
    });
});
