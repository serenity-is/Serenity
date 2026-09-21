import * as corelib from "@serenity-is/corelib";
import { CustomerRow, CustomerService, OrderGrid, OrderRow, OrderService } from "@serenity-is/demo.northwind";
import { EntityDialogWrapper, mockAdmin, mockDynamicData, mockFetch, mockGridSize, mockRowLookup, unmockFetch } from "test-utils";
import { afterAll, afterEach, beforeAll, describe, expect, it, vi } from "vitest";
import initPage, { PopulateLinkedDataDialog, PopulateLinkedDataGrid } from "../../Modules/Dialogs/PopulateLinkedData/PopulateLinkedDataPage";
import { PopulateLinkedDataForm } from "../../Modules/ServerTypes/Demo";

beforeAll(() => {
    mockDynamicData();
    mockAdmin();
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

describe("PopulateLinkedDataPage", () => {
    it("initializes the grid page", () => {
        const init = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(init).toHaveBeenCalledWith(PopulateLinkedDataGrid);
    });
});

describe("PopulateLinkedDataGrid", () => {
    it("extends the order grid and uses the custom dialog", () => {
        const grid = new PopulateLinkedDataGrid({});
        expect(grid instanceof OrderGrid).toBe(true);
        expect(grid["getDialogType"]()).toBe(PopulateLinkedDataDialog);
        grid.destroy();
    });
});

describe("PopulateLinkedDataDialog", () => {
    it("wires up form, row and service", () => {
        const dialog = new PopulateLinkedDataDialog({});
        expect(dialog["getFormKey"]()).toBe(PopulateLinkedDataForm.formKey);
        expect(dialog["getRowDefinition"]()).toBe(OrderRow);
        expect(dialog["getService"]()).toBe(OrderService.baseUrl);
        dialog.destroy();
    });

    it("adds default order dialog css classes", () => {
        const dialog = new PopulateLinkedDataDialog({});
        const css = dialog["getCssClass"]();
        expect(css).toContain("s-OrderDialog");
        expect(css).toContain("s-Demo-Northwind-OrderDialog");
        dialog.destroy();
    });

    it("populates linked fields from customer details", () => {
        const wrapper = new EntityDialogWrapper(new PopulateLinkedDataDialog({}));
        wrapper.actual["setCustomerDetails"]({
            City: "Berlin",
            ContactName: "Maria",
            ContactTitle: "Sales",
            Country: "Germany",
            Fax: "030",
            Phone: "0302",
            Region: "West"
        });
        const form = wrapper.actual["form"] as PopulateLinkedDataForm;
        expect(form.CustomerCity.value).toBe("Berlin");
        expect(form.CustomerContactName.value).toBe("Maria");
        expect(form.CustomerContactTitle.value).toBe("Sales");
        expect(form.CustomerCountry.value).toBe("Germany");
        expect(form.CustomerFax.value).toBe("030");
        expect(form.CustomerPhone.value).toBe("0302");
        expect(form.CustomerRegion.value).toBe("West");
        wrapper.actual.destroy();
    });

    it("retrieves the customer on change", () => {
        const wrapper = new EntityDialogWrapper(new PopulateLinkedDataDialog({}));
        const form = wrapper.actual["form"] as PopulateLinkedDataForm;
        form.CustomerID.value = "ALFKI";
        const retrieve = vi.spyOn(CustomerService, "Retrieve").mockImplementation(((request: any, onSuccess: any) => {
            onSuccess({ Entity: { CustomerID: "ALFKI", City: "Berlin" } });
            return Promise.resolve({}) as any;
        }) as any);
        form.CustomerID.element.trigger("change");
        expect(retrieve).toHaveBeenCalledWith({ EntityId: "ALFKI" }, expect.any(Function));
        expect(form.CustomerCity.value).toBe("Berlin");
        wrapper.actual.destroy();
    });

    it("clears linked fields when the customer is emptied", () => {
        const wrapper = new EntityDialogWrapper(new PopulateLinkedDataDialog({}));
        const form = wrapper.actual["form"] as PopulateLinkedDataForm;
        form.CustomerID.value = null;
        const setDetails = vi.spyOn(wrapper.actual as any, "setCustomerDetails");
        form.CustomerID.element.trigger("change");
        expect(setDetails).toHaveBeenCalledWith({});
        wrapper.actual.destroy();
    });
});
