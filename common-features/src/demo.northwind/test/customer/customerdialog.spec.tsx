import * as corelib from "@serenity-is/corelib";
import { DeleteRequest, DeleteResponse, RetrieveRequest, RetrieveResponse, SaveRequest, SaveResponse } from "@serenity-is/corelib";
import { DialogUtils } from "@serenity-is/extensions";
import { EntityDialogWrapper, mockAdmin, mockDynamicData, mockFetch, mockRowLookup, typeText } from "test-utils";
import { CustomerDialog } from "../../Modules/Customer/CustomerDialog";
import { CustomerForm, CustomerRow, CustomerService } from "../../Modules/ServerTypes/Demo";

beforeAll(() => {
    mockDynamicData();
    mockAdmin();
    mockRowLookup(CustomerRow, [{ CustomerID: "ALFKI", CompanyName: "Alfreds" }]);
    mockFetch({ "*": () => ({ Entities: [], TotalCount: 0 }) });
});

afterEach(() => {
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

describe("CustomerDialog", () => {
    it("wires up form, row and service", () => {
        const dialog = new CustomerDialog({});
        expect(dialog["getFormKey"]()).toBe(CustomerForm.formKey);
        expect(dialog["getRowDefinition"]()).toBe(CustomerRow);
        expect(dialog["getService"]()).toBe(CustomerService.baseUrl);
        dialog.destroy();
    });

    it("computes the save state", () => {
        const dialog = new CustomerDialog({});
        expect(typeof dialog.getSaveState()).toBe("string");
        vi.spyOn(dialog as any, "getSaveEntity").mockImplementation(() => { throw new Error("fail"); });
        expect(dialog.getSaveState()).toBeNull();
        dialog.destroy();
    });

    it("calls retrieve service on loadById", async () => {
        const fetchSpy = mockFetch({
            [CustomerService.Methods.Retrieve]: (info) => {
                expect(info.data).toStrictEqual({ EntityId: "ALFKI" } satisfies RetrieveRequest);
                return {
                    Entity: { CustomerID: "ALFKI", CompanyName: "Alfreds" }
                } satisfies RetrieveResponse<CustomerRow>;
            }
        });
        const dlg = new EntityDialogWrapper(new CustomerDialog({}));
        await new Promise((resolve, reject) => dlg.actual.loadByIdAndOpenDialog("ALFKI", void 0, resolve, reject));
        expect(fetchSpy.requests.filter(r => r.url.includes("/Customer/Retrieve")).length).toBe(1);
        expect(dlg.actual.entityId).toBe("ALFKI");
        expect(dlg.getForm(CustomerForm).CompanyName.value).toBe("Alfreds");
    });

    it("calls create service on save button click for new mode", async () => {
        const dlg = new EntityDialogWrapper(new CustomerDialog({}));
        dlg.actual.loadNewAndOpenDialog();
        const form = dlg.getForm(CustomerForm);
        typeText(form.CustomerID, "ALFKI");
        typeText(form.CompanyName, "Alfreds");
        const fetchSpy = mockFetch({
            [CustomerService.Methods.Create]: (info) => {
                expect((info.data as SaveRequest<CustomerRow>).Entity).toMatchObject({ CustomerID: "ALFKI", CompanyName: "Alfreds" });
                return { EntityId: "ALFKI" } satisfies SaveResponse;
            }
        });
        await dlg.clickSaveButton();
        expect(fetchSpy.requests.filter(r => r.url.includes("/Customer/Create")).length).toBe(1);
    });

    it("calls update service on save button click for edit mode", async () => {
        const dlg = new EntityDialogWrapper(new CustomerDialog({}));
        dlg.actual.loadEntityAndOpenDialog({ CustomerID: "ALFKI", CompanyName: "Alfreds" });
        typeText(dlg.getForm(CustomerForm).CompanyName, "Alfreds Futterkiste");
        const fetchSpy = mockFetch({
            [CustomerService.Methods.Update]: (info) => {
                expect((info.data as SaveRequest<CustomerRow>).EntityId).toBe("ALFKI");
                return { EntityId: "ALFKI" } satisfies SaveResponse;
            }
        });
        await dlg.clickSaveButton();
        expect(fetchSpy.requests.filter(r => r.url.includes("/Customer/Update")).length).toBe(1);
    });

    it("calls delete service on delete button click", async () => {
        const dlg = new EntityDialogWrapper(new CustomerDialog({}));
        dlg.actual.loadEntityAndOpenDialog({ CustomerID: "ALFKI", CompanyName: "Alfreds" });
        const fetchSpy = mockFetch({
            [CustomerService.Methods.Delete]: (info) => {
                expect(info.data).toStrictEqual({ EntityId: "ALFKI" } satisfies DeleteRequest);
                return {} satisfies DeleteResponse;
            }
        });
        await dlg.clickDeleteButton();
        expect(fetchSpy.requests.filter(r => r.url.includes("/Customer/Delete")).length).toBe(1);
    });

    it("reloads the customer lookup on save success", () => {
        const reload = vi.spyOn(corelib, "reloadLookup").mockImplementation((() => { }) as any);
        const dialog = new CustomerDialog({});
        dialog["onSaveSuccess"]({ EntityId: "ALFKI" } as any, "save" as any);
        expect(reload).toHaveBeenCalledWith("Northwind.Customer");
        dialog.destroy();
    });

    it("assigns the customer id to the orders grid on load", () => {
        const dialog = new CustomerDialog({});
        dialog["loadEntity"]({ CustomerID: "ALFKI" });
        expect(dialog["ordersGrid"].customerID).toBe("ALFKI");
        dialog.destroy();
    });

    it("registers pending change confirmation on dialog init", () => {
        let callback: any;
        const pending = vi.spyOn(DialogUtils, "pendingChangesConfirmation").mockImplementation((_el: any, cb: any) => { callback = cb; });
        const dialog = new CustomerDialog({});
        dialog["initDialog"]();
        expect(pending).toHaveBeenCalled();
        expect(typeof callback).toBe("function");
        expect(typeof callback()).toBe("boolean");
        dialog.destroy();
    });
});

