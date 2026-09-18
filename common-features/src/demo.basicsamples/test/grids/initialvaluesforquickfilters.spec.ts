import * as corelib from "@serenity-is/corelib";
import { OrderGrid, OrderRow, OrderShippingState } from "@serenity-is/demo.northwind";
import { mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import initPage, { InitialValuesForQuickFilters } from "../../Modules/Grids/InitialValuesForQuickFilters/InitialValuesForQuickFiltersPage";

const fld = OrderRow.Fields;

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

        const endW = { valueAsDate: null as Date };
        const startW = {
            valueAsDate: null as Date,
            element: { nextSibling: () => ({ getWidget: () => endW }) }
        };
        const orderDate = filters.find((x: any) => x.field === fld.OrderDate);
        expect(orderDate).toBeTruthy();
        (orderDate as any).init(startW);
        expect(startW.valueAsDate).toEqual(new Date(2010, 4, 1));
        expect(endW.valueAsDate).toBeTruthy();
        expect((orderDate as any).init).toEqual(expect.any(Function));

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
