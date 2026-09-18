import * as corelib from "@serenity-is/corelib";
import { EmployeeRow, OrderDialog, OrderGrid, ProductRow, ShipperRow } from "@serenity-is/demo.northwind";
import { mockAdmin, mockDynamicData, mockFetch, mockGridSize, mockRowLookup, unmockFetch } from "test-utils";
import initPage, { DefaultValuesInNewGrid } from "../../Modules/Dialogs/DefaultValuesInNewDialog/DefaultValuesInNewDialogPage";

beforeAll(() => {
    mockDynamicData();
    mockAdmin();
    mockGridSize();
    mockRowLookup(EmployeeRow, [
        { EmployeeID: 1, FullName: "Robert King" },
        { EmployeeID: 2, FullName: "Nancy Davolio" },
        { EmployeeID: 3, FullName: "Laura Callahan" }
    ]);
    mockRowLookup(ShipperRow, [
        { ShipperID: 1, CompanyName: "Speedy Express" },
        { ShipperID: 2, CompanyName: "United Package" }
    ]);
    mockRowLookup(ProductRow, [{ ProductID: 1, ProductName: "Chai", UnitPrice: 18 }]);
    mockFetch({ "*": () => ({ Entities: [], TotalCount: 0 }) });
});

afterAll(() => {
    unmockFetch();
});

afterEach(() => {
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

describe("DefaultValuesInNewDialogPage", () => {
    it("initializes the grid page", () => {
        const spy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(spy).toHaveBeenCalledWith(DefaultValuesInNewGrid);
    });
});

describe("DefaultValuesInNewGrid", () => {
    it("extends the order grid and adds extra buttons", () => {
        const grid = new DefaultValuesInNewGrid({});
        expect(grid instanceof OrderGrid).toBe(true);
        const buttons = grid["getButtons"]();
        expect(buttons.some((x: any) => x.cssClass == "add-button")).toBe(true);
        expect(buttons.some((x: any) => x.cssClass == "add-note-button")).toBe(true);
        grid.destroy();
    });

    it("sets default values when new item button is clicked", async () => {
        const grid = new DefaultValuesInNewGrid({});
        const editItem = vi.spyOn(grid as any, "editItem").mockImplementation((() => ({})) as any);
        await grid["addButtonClick"]();
        expect(editItem).toHaveBeenCalledWith(expect.objectContaining({
            CustomerID: "ANTON",
            EmployeeID: 1,
            ShipVia: 1
        }));
        expect((editItem.mock.calls[0][0] as any).RequiredDate).toBeTruthy();
        grid.destroy();
    });

    it("adds an order from the queen", async () => {
        const grid = new DefaultValuesInNewGrid({});
        const editItem = vi.spyOn(grid as any, "editItem").mockImplementation((() => ({})) as any);
        const button = grid["getButtons"]().find((x: any) => x.title == "Add Order from the Queen");
        await button.onClick(null);
        expect(editItem).toHaveBeenCalledWith(expect.objectContaining({
            CustomerID: "QUEEN",
            EmployeeID: 2,
            ShipVia: 2
        }));
        grid.destroy();
    });

    it("adds an order with a chai detail line", async () => {
        const grid = new DefaultValuesInNewGrid({});
        const initDialog = vi.spyOn(grid, "initDialog" as any).mockImplementation((() => ({})) as any);
        const load = vi.spyOn(OrderDialog.prototype, "loadEntityAndOpenDialog").mockImplementation((() => ({})) as any);
        const button = grid["getButtons"]().find((x: any) => x.title == "Add Order with 5 Chai by Laura");
        await button.onClick(null);
        expect(initDialog).toHaveBeenCalled();
        expect(load).toHaveBeenCalledWith(expect.objectContaining({
            CustomerID: "GOURL",
            EmployeeID: 3,
            DetailList: [expect.objectContaining({ ProductID: 1, Quantity: 5 })]
        }));
        grid.destroy();
    });
});
