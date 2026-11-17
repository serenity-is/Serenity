import * as corelib from "@serenity-is/corelib";
import { mockFetch, mockGridSize, unmockFetch } from "test-utils";
import { afterEach, beforeAll, beforeEach, describe, expect, it, vi } from "vitest";
import { ShipperGrid } from "../../Modules/Shipper/ShipperGrid";
import initPage from "../../Modules/Shipper/ShipperPage";

beforeAll(() => {
    mockGridSize();
});

beforeEach(() => {
    mockFetch({ "*": () => ({ Entities: [], TotalCount: 0 }) });
});

afterEach(() => {
    unmockFetch();
    vi.restoreAllMocks();
});

describe("ShipperPage", () => {
    it("initializes the grid page", () => {
        const spy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(spy).toHaveBeenCalledWith(ShipperGrid);
    });
});
