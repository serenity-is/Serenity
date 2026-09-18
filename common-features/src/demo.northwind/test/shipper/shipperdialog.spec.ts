import { DeleteRequest, DeleteResponse, RetrieveRequest, RetrieveResponse, SaveRequest, SaveResponse } from "@serenity-is/corelib";
import { EntityDialogWrapper, mockAdmin, mockDynamicData, mockFetch, typeText, unmockFetch } from "test-utils";
import { ShipperDialog } from "../../Modules/Shipper/ShipperDialog";
import { ShipperForm, ShipperRow, ShipperService } from "../../Modules/ServerTypes/Demo";

beforeAll(() => {
    mockDynamicData();
    mockAdmin();
});

afterEach(() => {
    unmockFetch();
});

describe("ShipperDialog", () => {
    it("wires up form, row and service", () => {
        const dialog = new ShipperDialog({});
        expect(dialog["getFormKey"]()).toBe(ShipperForm.formKey);
        expect(dialog["getRowDefinition"]()).toBe(ShipperRow);
        expect(dialog["getService"]()).toBe(ShipperService.baseUrl);
        dialog.destroy();
    });

    it("calls retrieve service on loadById", async () => {
        const fetchSpy = mockFetch({
            [ShipperService.Methods.Retrieve]: (info) => {
                expect(info.data).toStrictEqual({ EntityId: 1 } satisfies RetrieveRequest);
                return { Entity: { ShipperID: 1, CompanyName: "Speedy Express" } } satisfies RetrieveResponse<ShipperRow>;
            }
        });
        const dlg = new EntityDialogWrapper(new ShipperDialog());
        await new Promise((resolve, reject) => dlg.actual.loadByIdAndOpenDialog(1, void 0, resolve, reject));
        expect(fetchSpy.requests.length).toBe(1);
        expect(dlg.actual.entityId).toBe(1);
        expect(dlg.getForm(ShipperForm).CompanyName.value).toBe("Speedy Express");
    });

    it("calls create service on save button click for new mode", async () => {
        const dlg = new EntityDialogWrapper(new ShipperDialog());
        dlg.actual.loadNewAndOpenDialog();
        const form = dlg.getForm(ShipperForm);
        typeText(form.CompanyName, "Speedy Express");
        typeText(form.Phone, "5035559931");
        const fetchSpy = mockFetch({
            [ShipperService.Methods.Create]: (info) => {
                expect(info.data).toStrictEqual({
                    Entity: { CompanyName: "Speedy Express", Phone: "(503) 555-9931" }
                } satisfies SaveRequest<ShipperRow>);
                return { EntityId: 1 } satisfies SaveResponse;
            }
        });
        await dlg.clickSaveButton();
        expect(fetchSpy.requests.length).toBe(1);
    });

    it("calls update service on save button click for edit mode", async () => {
        const dlg = new EntityDialogWrapper(new ShipperDialog());
        dlg.actual.loadEntityAndOpenDialog({ ShipperID: 1, CompanyName: "Speedy Express", Phone: "5035559931" });
        typeText(dlg.getForm(ShipperForm).CompanyName, "United Package");
        const fetchSpy = mockFetch({
            [ShipperService.Methods.Update]: (info) => {
                expect((info.data as SaveRequest<ShipperRow>).EntityId).toBe(1);
                expect((info.data as SaveRequest<ShipperRow>).Entity).toMatchObject({ ShipperID: 1, CompanyName: "United Package" });
                return { EntityId: 1 } satisfies SaveResponse;
            }
        });
        await dlg.clickSaveButton();
        expect(fetchSpy.requests.length).toBe(1);
    });

    it("calls delete service on delete button click", async () => {
        const dlg = new EntityDialogWrapper(new ShipperDialog());
        dlg.actual.loadEntityAndOpenDialog({ ShipperID: 1, CompanyName: "Speedy Express" });
        const fetchSpy = mockFetch({
            [ShipperService.Methods.Delete]: (info) => {
                expect(info.data).toStrictEqual({ EntityId: 1 } satisfies DeleteRequest);
                return {} satisfies DeleteResponse;
            }
        });
        await dlg.clickDeleteButton();
        expect(fetchSpy.requests.length).toBe(1);
    });
});
