import * as corelib from "@serenity-is/corelib";
import { mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import { afterAll, afterEach, beforeAll, describe, expect, it, vi } from "vitest";
import { OrderGrid } from "../../Modules/Order/OrderGrid";
import initPage from "../../Modules/Order/OrderPage";

beforeAll(() => {
    mockDynamicData();
    mockGridSize();
    mockFetch({ "*": () => ({ Entities: [], TotalCount: 0 }) });
});

afterAll(() => {
    unmockFetch();
});

afterEach(() => {
    vi.restoreAllMocks();
});

describe("OrderPage", () => {
    it("initializes the grid page with query string state", () => {
        const setShippingState = vi.fn();
        const initSpy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({ set_shippingState: setShippingState } as any);
        vi.spyOn(corelib, "parseQueryString").mockReturnValue({ shippingState: "2" } as any);
        initPage();
        expect(initSpy).toHaveBeenCalledWith(OrderGrid);
        expect(setShippingState).toHaveBeenCalledWith(2);
    });

    it("skips setting the state when not in the query string", () => {
        const setShippingState = vi.fn();
        vi.spyOn(corelib, "gridPageInit").mockReturnValue({ set_shipping_state: setShippingState } as any);
        vi.spyOn(corelib, "parseQueryString").mockReturnValue({} as any);
        initPage();
        expect(setShippingState).not.toHaveBeenCalled();
    });
});
