import { DeleteRequest, DeleteResponse, RetrieveRequest, RetrieveResponse, SaveRequest, SaveResponse } from "@serenity-is/corelib";
import { ReportHelper } from "@serenity-is/extensions";
import { EntityDialogWrapper, mockAdmin, mockDynamicData, mockFetch, mockRowLookup, typeText, unmockFetch } from "test-utils";
import { afterAll, afterEach, beforeAll, describe, expect, it, vi } from "vitest";
import { OrderDialog } from "../../Modules/Order/OrderDialog";
import { CustomerRow, OrderForm, OrderRow, OrderService } from "../../Modules/ServerTypes/Demo";

beforeAll(() => {
    mockDynamicData();
    mockAdmin();
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

describe("OrderDialog", () => {
    it("wires up form, row and service", () => {
        const dialog = new OrderDialog({});
        expect(dialog["getFormKey"]()).toBe(OrderForm.formKey);
        expect(dialog["getRowDefinition"]()).toBe(OrderRow);
        expect(dialog["getService"]()).toBe(OrderService.baseUrl);
        dialog.destroy();
    });

    it("adds a report toolbar button", () => {
        let options: any;
        const create = vi.spyOn(ReportHelper, "createToolButton").mockImplementation((o: any) => { options = o; return {} as any; });
        const dialog = new OrderDialog({});
        const buttons = dialog["getToolbarButtons"]();
        expect(buttons.length).toBeGreaterThan(0);
        expect(create).toHaveBeenCalledWith(expect.objectContaining({ reportKey: "Northwind.OrderDetail" }));
        (dialog as any).entityId = 7;
        expect(options.getParams()).toEqual({ OrderID: 7 });
        dialog.destroy();
    });

    it("toggles the report button based on edit mode", () => {
        const dialog = new OrderDialog({});
        (dialog as any).entity = {};
        (dialog as any).entityId = 42;
        expect(() => dialog["updateInterface"]()).not.toThrow();
        dialog.destroy();
    });

    it("passes the order id to the detail list on load", () => {
        const dialog = new OrderDialog({});
        (dialog as any).entity = {};
        (dialog as any).entityId = 42;
        expect(() => dialog["afterLoadEntity"]()).not.toThrow();
        expect(dialog["form"].DetailList.orderId).toBe(42);
        dialog.destroy();
    });

    it("calls retrieve service on loadById", async () => {
        const fetchSpy = mockFetch({
            [OrderService.Methods.Retrieve]: (info) => {
                expect(info.data).toStrictEqual({ EntityId: 1 } satisfies RetrieveRequest);
                return {
                    Entity: { OrderID: 1, CustomerID: "ALFKI", OrderDate: "2020-01-01" }
                } satisfies RetrieveResponse<OrderRow>;
            }
        });
        const dlg = new EntityDialogWrapper(new OrderDialog());
        await new Promise((resolve, reject) => dlg.actual.loadByIdAndOpenDialog(1, void 0, resolve, reject));
        expect(fetchSpy.requests.filter(r => r.url.includes("/Northwind/Order/Retrieve")).length).toBe(1);
        expect(dlg.actual.entityId).toBe(1);
        expect(dlg.getForm(OrderForm).CustomerID.value).toBe("ALFKI");
    });

    it("calls create service on save button click for new mode", async () => {
        const dlg = new EntityDialogWrapper(new OrderDialog());
        dlg.actual.loadNewAndOpenDialog();
        const form = dlg.getForm(OrderForm);
        typeText(form.CustomerID, "ALFKI");
        typeText(form.OrderDate, "2020-01-01");
        const fetchSpy = mockFetch({
            [OrderService.Methods.Create]: (info) => {
                expect((info.data as SaveRequest<OrderRow>).Entity).toMatchObject({ CustomerID: "ALFKI" });
                return { EntityId: 1 } satisfies SaveResponse;
            }
        });
        await dlg.clickSaveButton();
        expect(fetchSpy.requests.filter(r => r.url.includes("/Northwind/Order/Create")).length).toBe(1);
    });

    it("calls update service on save button click for edit mode", async () => {
        const dlg = new EntityDialogWrapper(new OrderDialog());
        dlg.actual.loadEntityAndOpenDialog({ OrderID: 1, CustomerID: "ALFKI", OrderDate: "2020-01-01" });
        typeText(dlg.getForm(OrderForm).ShipName, "Alfreds");
        const fetchSpy = mockFetch({
            [OrderService.Methods.Update]: (info) => {
                expect((info.data as SaveRequest<OrderRow>).EntityId).toBe(1);
                return { EntityId: 1 } satisfies SaveResponse;
            }
        });
        await dlg.clickSaveButton();
        expect(fetchSpy.requests.filter(r => r.url.includes("/Northwind/Order/Update")).length).toBe(1);
    });

    it("calls delete service on delete button click", async () => {
        const dlg = new EntityDialogWrapper(new OrderDialog());
        dlg.actual.loadEntityAndOpenDialog({ OrderID: 1, CustomerID: "ALFKI", OrderDate: "2020-01-01" });
        const fetchSpy = mockFetch({
            [OrderService.Methods.Delete]: (info) => {
                expect(info.data).toStrictEqual({ EntityId: 1 } satisfies DeleteRequest);
                return {} satisfies DeleteResponse;
            }
        });
        await dlg.clickDeleteButton();
        expect(fetchSpy.requests.filter(r => r.url.includes("/Northwind/Order/Delete")).length).toBe(1);
    });
});

