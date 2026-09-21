import { SubDialogHelper } from "@serenity-is/corelib";
import { mockDynamicData, mockFetch, mockGridSize, mockRowLookup, unmockFetch } from "test-utils";
import { afterAll, afterEach, beforeAll, describe, expect, it, vi } from "vitest";
import { CustomerOrderDialog } from "../../Modules/Customer/CustomerOrderDialog";
import { CustomerOrdersGrid } from "../../Modules/Customer/CustomerOrdersGrid";
import { CustomerRow, OrderColumns } from "../../Modules/ServerTypes/Demo";

beforeAll(() => {
    mockDynamicData();
    mockGridSize();
    mockRowLookup(CustomerRow, [{ CustomerID: "ALFKI", CompanyName: "Alfreds" }]);
    mockFetch({ "*": () => ({ Entities: [], TotalCount: 0 }) });
});

afterAll(() => {
    unmockFetch();
});

afterEach(() => {
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

describe("CustomerOrdersGrid", () => {
    it("uses the customer order dialog", () => {
        const grid = new CustomerOrdersGrid({});
        expect(grid["getDialogType"]()).toBe(CustomerOrderDialog);
        expect(grid["getInitialTitle"]()).toBeNull();
        grid.destroy();
    });

    it("tracks the customer id and grid can load", () => {
        const grid = new CustomerOrdersGrid({});
        expect(grid.customerID).toBeUndefined();
        expect(grid["getGridCanLoad"]()).toBe(false);
        grid.customerID = "ALFKI";
        expect(grid.customerID).toBe("ALFKI");
        expect(grid["getGridCanLoad"]()).toBe(true);
        grid.destroy();
    });

    it("filters out the customer company name column", () => {
        const grid = new CustomerOrdersGrid({});
        const columns = grid["createColumns"]();
        expect(columns.some(x => x.id === OrderColumns.Fields.CustomerCompanyName)).toBe(false);
        grid.destroy();
    });

    it("disables the add button without a customer id", () => {
        const grid = new CustomerOrdersGrid({});
        const addButton = grid["getButtons"]().find(x => x.action === "add");
        expect(addButton).toBeTruthy();
        expect((addButton.disabled as any)()).toBe(true);
        grid.customerID = "ALFKI";
        expect((addButton.disabled as any)()).toBe(false);
        grid.destroy();
    });

    it("does nothing on add button click without a customer id", () => {
        const grid = new CustomerOrdersGrid({});
        const edit = vi.spyOn(grid as any, "editItem").mockImplementation(() => { });
        grid["addButtonClick"]();
        expect(edit).not.toHaveBeenCalled();
        grid.destroy();
    });

    it("adds an order for the current customer", () => {
        const grid = new CustomerOrdersGrid({});
        grid.customerID = "ALFKI";
        const edit = vi.spyOn(grid as any, "editItem").mockImplementation(() => { });
        grid["addButtonClick"]();
        expect(edit).toHaveBeenCalledWith({ CustomerID: "ALFKI" });
        grid.destroy();
    });

    it("cascades the dialog to the parent dialog node", () => {
        const cascade = vi.spyOn(SubDialogHelper, "cascade").mockImplementation(() => { });
        const grid = new CustomerOrdersGrid({});
        vi.spyOn(Object.getPrototypeOf(Object.getPrototypeOf(CustomerOrdersGrid.prototype)), "initEntityDialog").mockImplementation(() => { });
        grid["initEntityDialog"]("CustomerOrder", { domNode: document.createElement("div") } as any);
        expect(cascade).toHaveBeenCalled();
        grid.destroy();
    });
});
