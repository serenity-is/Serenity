import * as corelib from "@serenity-is/corelib";
import { CustomerRow, CustomerService, OrderDialog, OrderGrid } from "@serenity-is/demo.northwind";
import { EntityDialogWrapper, mockAdmin, mockDynamicData, mockFetch, mockGridSize, mockRowLookup, unmockFetch } from "test-utils";
import { afterAll, afterEach, beforeAll, describe, expect, it, vi } from "vitest";
import initPage, { OtherFormInTabOneBarGrid, OtherFormOneBarDialog } from "../../Modules/Dialogs/OtherFormInTabOneBar/OtherFormInTabOneBarPage";

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
    return new EntityDialogWrapper(new OtherFormOneBarDialog({}));
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

describe("OtherFormInTabOneBarPage", () => {
    it("initializes the grid page", () => {
        const spy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(spy).toHaveBeenCalledWith(OtherFormInTabOneBarGrid);
    });
});

describe("OtherFormInTabOneBarGrid", () => {
    it("extends the order grid and uses the custom dialog", () => {
        const grid = new OtherFormInTabOneBarGrid({});
        expect(grid instanceof OrderGrid).toBe(true);
        expect(grid["getDialogType"]()).toBe(OtherFormOneBarDialog);
        grid.destroy();
    });
});

describe("OtherFormOneBarDialog", () => {
    it("extends the order dialog and renders contents", async () => {
        const wrapper = createWrapper();
        expect(wrapper.actual instanceof OrderDialog).toBe(true);
        expect(wrapper.actual["renderContents"]()).toBeTruthy();
        await close(wrapper);
    });

    it("exposes the customer id", async () => {
        const wrapper = createWrapper();
        wrapper.actual["form"].CustomerID.value = 7 as any;
        expect(wrapper.actual.customerId).toBe(7);
        await close(wrapper);
    });

    it("handles customer change with and without an id", async () => {
        const wrapper = createWrapper();
        const load = vi.spyOn(wrapper.actual["customerPropertyGrid"], "load");

        wrapper.actual["form"].CustomerID.value = null;
        wrapper.actual["form"].CustomerID.element.trigger("change");
        expect(load).toHaveBeenCalled();

        const retrieve = mockRetrieve();
        wrapper.actual["form"].CustomerID.value = "ALFKI";
        wrapper.actual["form"].CustomerID.element.trigger("change");
        await new Promise(resolve => setTimeout(resolve, 20));
        expect(retrieve).toHaveBeenCalled();
        await close(wrapper);
    });

    it("disables the customer tab on load", async () => {
        const wrapper = createWrapper();
        mockRetrieve();
        const setDisabled = vi.spyOn(corelib.TabsExtensions, "setDisabled");
        wrapper.actual["loadEntity"]({ OrderID: 1 } as any);
        await new Promise(resolve => setTimeout(resolve, 20));
        expect(setDisabled).toHaveBeenCalledWith(wrapper.actual["tabs"], "Customer", true);
        await close(wrapper);
    });

    it("saves only the order when there is no customer", async () => {
        const wrapper = createWrapper();
        wrapper.actual["form"].CustomerID.value = null;
        const saveOrder = vi.spyOn(wrapper.actual as any, "saveOrder").mockImplementation((() => ({})) as any);
        const onSuccess = vi.fn();
        wrapper.actual["saveCustomer"](() => { }, onSuccess);
        expect(onSuccess).toHaveBeenCalledWith(null);
        expect(saveOrder).not.toHaveBeenCalled();
        await close(wrapper);
    });

    it("skips saving when customer validation fails", async () => {
        const wrapper = createWrapper();
        wrapper.actual["form"].CustomerID.value = "ALFKI";
        vi.spyOn(wrapper.actual["customerValidator"], "form").mockReturnValue(false);
        const update = vi.spyOn(CustomerService, "Update").mockImplementation((() => Promise.resolve({})) as any);
        const onSuccess = vi.fn();
        wrapper.actual["saveCustomer"](() => { }, onSuccess);
        expect(update).not.toHaveBeenCalled();
        expect(onSuccess).not.toHaveBeenCalled();
        await close(wrapper);
    });

    it("saves the customer and notifies success", async () => {
        const wrapper = createWrapper();
        wrapper.actual["form"].CustomerID.value = "ALFKI";
        vi.spyOn(wrapper.actual["customerValidator"], "form").mockReturnValue(true);
        const update = vi.spyOn(CustomerService, "Update").mockImplementation(((request: any, onSuccess: any) => {
            onSuccess({ EntityId: "ALFKI" });
            return Promise.resolve({}) as any;
        }) as any);
        vi.spyOn(corelib, "reloadLookup").mockImplementation((() => ({})) as any);
        const onSuccess = vi.fn();

        wrapper.actual["saveCustomer"](() => { }, onSuccess);

        expect(update).toHaveBeenCalledWith(expect.objectContaining({ EntityId: "ALFKI" }), expect.any(Function));
        expect(onSuccess).toHaveBeenCalledWith({ EntityId: "ALFKI" });
        await close(wrapper);
    });

    it("saveAll saves the order when there is no customer", async () => {
        const wrapper = createWrapper();
        wrapper.actual["form"].CustomerID.value = null;
        const saveOrder = vi.spyOn(wrapper.actual as any, "saveOrder").mockImplementation((() => ({})) as any);
        wrapper.actual["saveAll"](() => { });
        expect(saveOrder).toHaveBeenCalled();
        await close(wrapper);
    });

    it("saveAll saves the customer then the order", async () => {
        const wrapper = createWrapper();
        wrapper.actual["form"].CustomerID.value = "ALFKI";
        vi.spyOn(wrapper.actual["customerValidator"], "form").mockReturnValue(true);
        vi.spyOn(CustomerService, "Update").mockImplementation(((request: any, onSuccess: any) => {
            onSuccess({ EntityId: "ALFKI" });
            return Promise.resolve({}) as any;
        }) as any);
        vi.spyOn(corelib, "reloadLookup").mockImplementation((() => ({})) as any);
        const saveOrder = vi.spyOn(wrapper.actual as any, "saveOrder").mockImplementation((() => ({})) as any);
        wrapper.actual["saveAll"](() => { });
        expect(saveOrder).toHaveBeenCalled();
        await close(wrapper);
    });

    it("save delegates to saveAll", async () => {
        const wrapper = createWrapper();
        const saveAll = vi.spyOn(wrapper.actual as any, "saveAll").mockImplementation((() => ({})) as any);
        wrapper.actual["save"](() => { });
        expect(saveAll).toHaveBeenCalled();
        await close(wrapper);
    });
});
