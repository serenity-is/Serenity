import * as corelib from "@serenity-is/corelib";
import { OrderGrid, OrderRow, OrderShippingState } from "@serenity-is/demo.northwind";
import { mockFetch, mockGridSize, unmockFetch } from "test-utils";
import { afterEach, beforeAll, beforeEach, describe, expect, it, vi } from "vitest";
import initPage, { InitialValuesForQuickFilters } from "../../Modules/Grids/InitialValuesForQuickFilters/InitialValuesForQuickFiltersPage";

const fld = OrderRow.Fields;

beforeAll(() => {
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

describe("InitialValuesForQuickFiltersPage", () => {
    it("initializes the grid page", () => {
        const spy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(spy).toHaveBeenCalledWith(InitialValuesForQuickFilters);
    });
});

describe("InitialValuesForQuickFilters", () => {
    it("extends the order grid", () => {
        const grid = new InitialValuesForQuickFilters({});
        expect(grid instanceof OrderGrid).toBe(true);
        grid.destroy();
    });

    it("sets initial values for order date and shipping state quick filters", () => {
        const grid = new InitialValuesForQuickFilters({});
        const filters = grid["getQuickFilters"]();

        // the range is set through the state of the filter, as the editor of the range can be of any kind, e.g. a
        // single input one that the application gives to all of its date columns
        const orderDate = filters.find((x: any) => x.field === fld.OrderDate);
        expect(orderDate).toBeTruthy();
        const loadState = vi.fn();
        (orderDate as any).loadState = loadState;
        const widget = {};
        (orderDate as any).init(widget);
        expect(loadState).toHaveBeenCalledTimes(1);
        expect(loadState.mock.calls[0][0]).toBe(widget);
        const state = loadState.mock.calls[0][1];
        expect(state[0]).toBe("2010-05-01");
        expect(state[1]).toMatch(/^\d{4}-11-01$/);

        const shippingState = filters.find((x: any) => x.field === fld.ShippingState);
        expect(shippingState).toBeTruthy();
        const enumEditor = { value: null } as any;
        (shippingState as any).init(enumEditor);
        expect(enumEditor.value).toBe(OrderShippingState.NotShipped.toString());

        grid.destroy();
    });

    it("sets initial values of the ship via quick filter", () => {
        const grid = new InitialValuesForQuickFilters({});
        const shipVia = { values: null } as any;
        vi.spyOn(grid, "findQuickFilter" as any).mockImplementation((_t: any, field: any) => field === fld.ShipVia ? shipVia : null);
        grid["createQuickFilters"]();
        expect(shipVia.values).toEqual(["1", "2"]);
        grid.destroy();
    });
});
