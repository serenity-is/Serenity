import * as corelib from "@serenity-is/corelib";
import { CustomerRow, CustomerService, OrderDialog, OrderGrid } from "@serenity-is/demo.northwind";
import { EntityDialogWrapper, mockAdmin, mockDynamicData, mockFetch, mockGridSize, mockRowLookup, unmockFetch } from "test-utils";
import { afterAll, afterEach, beforeAll, describe, expect, it, vi } from "vitest";
import initPage, { OtherFormInTabDialog, OtherFormInTabGrid } from "../../Modules/Dialogs/OtherFormInTab/OtherFormInTabPage";

beforeAll(() => {
    mockDynamicData();
    mockAdmin();
    mockGridSize();
    mockRowLookup(CustomerRow, [{ CustomerID: "ALFKI", CompanyName: "Alfreds" }]);
    mockFetch({ "*": () => ({ Entities: [], TotalCount: 0 }), [CustomerService.Methods.Retrieve]: () => ({ Entity: { CustomerID: "ALFKI", CompanyName: "Alfreds" } }) });
});

afterAll(() => {
    unmockFetch();
});

afterEach(async () => {
    await new Promise(resolve => setTimeout(resolve, 20));
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

function createWrapper() {
    return new EntityDialogWrapper(new OtherFormInTabDialog({}));
}

async function close(wrapper: EntityDialogWrapper<any>) {
    await new Promise(resolve => setTimeout(resolve, 20));
    wrapper.actual.destroy();
}

function mockRetrieve() {
    return vi.spyOn(CustomerService, "Retrieve").mockImplementation(((request: any, onSuccess: any) => {
        onSuccess({ Entity: { CustomerID: "ALFKI", CompanyName: "Alfreds" } });
        return Promise.resolve({}) as any;
    }) as any);
}

describe("OtherFormInTabPage", () => {
    it("initializes the grid page", () => {
        const spy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(spy).toHaveBeenCalledWith(OtherFormInTabGrid);
    });
});

describe("OtherFormInTabGrid", () => {
    it("extends the order grid and uses the custom dialog", () => {
        const grid = new OtherFormInTabGrid({});
        expect(grid instanceof OrderGrid).toBe(true);
        expect(grid["getDialogType"]()).toBe(OtherFormInTabDialog);
        grid.destroy();
    });
});

describe("OtherFormInTabDialog", () => {
    it("extends the order dialog and renders tabs", async () => {
        const wrapper = createWrapper();
        expect(wrapper.actual instanceof OrderDialog).toBe(true);
        expect(wrapper.actual["renderContents"]()).toBeTruthy();
        await close(wrapper);
    });

    it("exposes the customer id", async () => {
        const wrapper = createWrapper();
        wrapper.actual["form"].CustomerID.value = 5 as any;
        expect(wrapper.actual.customerId).toBe(5);
        await close(wrapper);
    });

    it("loads an empty customer grid when no customer is selected", async () => {
        const wrapper = createWrapper();
        const load = vi.spyOn(wrapper.actual["customerPropertyGrid"], "load");
        wrapper.actual["form"].CustomerID.value = null;
        wrapper.actual["form"].CustomerID.element.trigger("change");
        expect(load).toHaveBeenCalled();
        await close(wrapper);
    });

    it("retrieves the customer when a customer is selected", async () => {
        const wrapper = createWrapper();
        const retrieve = mockRetrieve();
        const load = vi.spyOn(wrapper.actual["customerPropertyGrid"], "load");
        wrapper.actual["form"].CustomerID.value = "ALFKI";
        wrapper.actual["form"].CustomerID.element.trigger("change");
        await new Promise(resolve => setTimeout(resolve, 20));
        expect(retrieve).toHaveBeenCalledWith({ EntityId: "ALFKI" }, expect.any(Function));
        expect(load).toHaveBeenCalledWith(expect.objectContaining({ CustomerID: "ALFKI" }));
        await close(wrapper);
    });

    it("disables the customer tab on load when there is no customer", async () => {
        const wrapper = createWrapper();
        mockRetrieve();
        const setDisabled = vi.spyOn(corelib.TabsExtensions, "setDisabled");
        wrapper.actual["loadEntity"]({ OrderID: 1 } as any);
        await new Promise(resolve => setTimeout(resolve, 20));
        expect(setDisabled).toHaveBeenCalledWith(wrapper.actual["tabs"], "Customer", true);
        await close(wrapper);
    });

    it("saves the customer details", async () => {
        const wrapper = createWrapper();
        wrapper.actual["form"].CustomerID.value = "ALFKI";
        vi.spyOn(wrapper.actual["customerValidator"], "form").mockReturnValue(true);
        const update = vi.spyOn(CustomerService, "Update").mockImplementation(((request: any, onSuccess: any) => {
            onSuccess({});
            return Promise.resolve({}) as any;
        }) as any);
        const reload = vi.spyOn(corelib, "reloadLookup").mockImplementation((() => ({})) as any);
        const notify = vi.spyOn(corelib, "notifySuccess").mockImplementation((() => ({})) as any);

        wrapper.actual["customerSaveClick"]();

        expect(update).toHaveBeenCalledWith(expect.objectContaining({ EntityId: "ALFKI" }), expect.any(Function));
        expect(reload).toHaveBeenCalled();
        expect(notify).toHaveBeenCalled();
        await close(wrapper);
    });

    it("does not save the customer when validation fails", async () => {
        const wrapper = createWrapper();
        wrapper.actual["form"].CustomerID.value = "ALFKI";
        vi.spyOn(wrapper.actual["customerValidator"], "form").mockReturnValue(false);
        const update = vi.spyOn(CustomerService, "Update").mockImplementation((() => Promise.resolve({})) as any);
        wrapper.actual["customerSaveClick"]();
        expect(update).not.toHaveBeenCalled();
        await close(wrapper);
    });

    it("does not save when there is no customer selected", async () => {
        const wrapper = createWrapper();
        wrapper.actual["form"].CustomerID.value = null;
        const update = vi.spyOn(CustomerService, "Update").mockImplementation((() => Promise.resolve({})) as any);
        wrapper.actual["customerSaveClick"]();
        expect(update).not.toHaveBeenCalled();
        await close(wrapper);
    });
});
