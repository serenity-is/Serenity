import * as corelib from "@serenity-is/corelib";
import { Fluent } from "@serenity-is/corelib";
import { CustomerDialog, OrderColumns, OrderDialog, OrderRow } from "@serenity-is/demo.northwind";
import { mockAdmin, mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import { afterAll, afterEach, beforeAll, describe, expect, it, vi } from "vitest";
import initPage, { CustomLinksInGrid } from "../../Modules/Grids/CustomLinksInGrid/CustomLinksInGridPage";

const order: OrderRow = {
    OrderID: 123,
    CustomerID: "ALFKI",
    CustomerCompanyName: "Alfreds",
    OrderDate: "2020-01-01",
    EmployeeID: 1,
    EmployeeFullName: "Nancy Davolio",
    ShipCountry: "USA"
};

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

function createGrid(): CustomLinksInGrid {
    const div = document.body.appendChild(document.createElement("div"));
    const grid = new CustomLinksInGrid({ element: div });
    grid.setItems([order]);
    return grid;
}

function clickOn(grid: CustomLinksInGrid, selector: string) {
    const target = grid.domNode.querySelector(selector) as HTMLElement;
    expect(target).toBeTruthy();
    const evt = new MouseEvent("click", { bubbles: true, cancelable: true });
    Object.defineProperty(evt, "target", { value: target });
    grid["onClick"](evt, 0, 0);
}

describe("CustomLinksInGridPage", () => {
    it("initializes the grid page", () => {
        const spy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(spy).toHaveBeenCalledWith(CustomLinksInGrid);
    });
});

describe("CustomLinksInGrid", () => {
    it("formats order columns as custom links", () => {
        const grid = createGrid();
        const columns = new OrderColumns(grid["createColumns"]());

        const div = document.createElement("div");
        div.append(columns.CustomerCompanyName.format({ value: "Alfreds" } as any) as any);
        expect(div.querySelector("a.customer-link")).toBeTruthy();

        div.innerHTML = "";
        div.append(columns.OrderDate.format({ value: "2020-01-01" } as any) as any);
        expect(div.querySelector("a.date-link")).toBeTruthy();

        div.innerHTML = "";
        div.append(columns.EmployeeFullName.format({ value: "Nancy" } as any) as any);
        expect(div.querySelector("a.employee-link")).toBeTruthy();

        div.innerHTML = "";
        div.append(columns.ShipCountry.format({ value: "USA" } as any) as any);
        expect(div.querySelector("a.ship-country-link")).toBeTruthy();

        grid.destroy();
    });

    it("opens a confirmation for customer links", () => {
        const grid = createGrid();
        const confirm = vi.spyOn(corelib, "confirmDialog").mockImplementation((() => ({})) as any);
        clickOn(grid, "a.customer-link");
        expect(confirm).toHaveBeenCalled();
        grid.destroy();
    });

    it("notifies for date links", () => {
        const grid = createGrid();
        const notify = vi.spyOn(corelib, "notifyInfo").mockImplementation((() => ({})) as any);
        clickOn(grid, "a.date-link");
        expect(notify).toHaveBeenCalledWith(expect.stringContaining("01/01/2020"));
        grid.destroy();
    });

    it("opens an order dialog for employee links", () => {
        const grid = createGrid();
        vi.spyOn(corelib, "notifySuccess").mockImplementation((() => ({})) as any);
        const load = vi.spyOn(OrderDialog.prototype, "loadEntityAndOpenDialog").mockImplementation((() => ({})) as any);
        clickOn(grid, "a.employee-link");
        expect(load).toHaveBeenCalledWith(expect.objectContaining({ CustomerID: "ALFKI", EmployeeID: 1 }));
        grid.destroy();
    });

    it("filters the grid for ship country links", () => {
        const grid = createGrid();
        vi.spyOn(corelib, "notifySuccess").mockImplementation((() => ({})) as any);
        const filter = { value: null } as any;
        vi.spyOn(grid, "findQuickFilter" as any).mockReturnValue(filter);
        const refresh = vi.spyOn(grid, "refresh").mockImplementation((() => ({})) as any);
        clickOn(grid, "a.ship-country-link");
        expect(filter.value).toBe("USA");
        expect(refresh).toHaveBeenCalled();
        grid.destroy();
    });

    it("asks for confirmation on edit links", () => {
        const grid = createGrid();
        grid["view"].getItemById = () => order;
        const confirm = vi.spyOn(corelib, "confirmDialog").mockImplementation((() => ({})) as any);
        grid["editItem"]("123");
        expect(confirm).toHaveBeenCalledWith(expect.stringContaining("123"), expect.any(Function));
        grid.destroy();
    });

    it("delegates non edit link calls to base grid", () => {
        const grid = createGrid();
        const load = vi.spyOn(OrderDialog.prototype, "load").mockImplementation((() => { }) as any);
        grid["editItem"](order);
        expect(load).toHaveBeenCalledWith(order, expect.any(Function));
        grid.destroy();
    });

    it("ignores clicks when the default is already prevented", () => {
        const grid = createGrid();
        vi.spyOn(Fluent, "isDefaultPrevented").mockReturnValue(true);
        const confirm = vi.spyOn(corelib, "confirmDialog").mockImplementation((() => ({})) as any);
        clickOn(grid, "a.customer-link");
        expect(confirm).not.toHaveBeenCalled();
        grid.destroy();
    });

    it("opens the customer dialog when confirmation is accepted", () => {
        const grid = createGrid();
        let yes: any;
        vi.spyOn(corelib, "confirmDialog").mockImplementation(((message: any, onYes: any) => { yes = onYes; return {}; }) as any);
        const load = vi.spyOn(CustomerDialog.prototype, "loadByIdAndOpenDialog").mockImplementation((() => ({})) as any);
        clickOn(grid, "a.customer-link");
        yes();
        expect(load).toHaveBeenCalledWith("ALFKI");
        grid.destroy();
    });

    it("opens the order dialog when customer confirmation is rejected", () => {
        const grid = createGrid();
        let options: any;
        vi.spyOn(corelib, "confirmDialog").mockImplementation(((message: any, onYes: any, o: any) => { options = o; return {}; }) as any);
        const load = vi.spyOn(OrderDialog.prototype, "loadByIdAndOpenDialog").mockImplementation((() => ({})) as any);
        clickOn(grid, "a.customer-link");
        options.onNo();
        expect(load).toHaveBeenCalledWith(123);
        grid.destroy();
    });

    it("opens the order dialog from the edit link confirmation", () => {
        const grid = createGrid();
        grid["view"].getItemById = () => order;
        let yes: any;
        vi.spyOn(corelib, "confirmDialog").mockImplementation(((message: any, onYes: any) => { yes = onYes; return {}; }) as any);
        const load = vi.spyOn(OrderDialog.prototype, "loadByIdAndOpenDialog").mockImplementation((() => ({})) as any);
        grid["editItem"]("123");
        yes();
        expect(load).toHaveBeenCalledWith(123);
        grid.destroy();
    });
});
