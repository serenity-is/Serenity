import { DeleteRequest, DeleteResponse, RetrieveRequest, RetrieveResponse, SaveRequest, SaveResponse } from "@serenity-is/corelib";
import { EntityDialogWrapper, mockAdmin, mockDynamicData, mockFetch, typeNumber, typeText, unmockFetch } from "test-utils";
import { RegionDialog } from "../../Modules/Region/RegionDialog";
import { RegionForm, RegionRow, RegionService } from "../../Modules/ServerTypes/Demo";

beforeAll(() => {
    mockDynamicData();
    mockAdmin();
});

afterEach(() => {
    unmockFetch();
});

describe("RegionDialog", () => {
    it("wires up form, row and service", () => {
        const dialog = new RegionDialog({});
        expect(dialog["getFormKey"]()).toBe(RegionForm.formKey);
        expect(dialog["getRowDefinition"]()).toBe(RegionRow);
        expect(dialog["getService"]()).toBe(RegionService.baseUrl);
        dialog.destroy();
    });

    it("calls retrieve service on loadById", async () => {
        const fetchSpy = mockFetch({
            [RegionService.Methods.Retrieve]: (info) => {
                expect(info.data).toStrictEqual({ EntityId: 1 } satisfies RetrieveRequest);
                return { Entity: { RegionID: 1, RegionDescription: "Eastern" } } satisfies RetrieveResponse<RegionRow>;
            }
        });
        const dlg = new EntityDialogWrapper(new RegionDialog());
        await new Promise((resolve, reject) => dlg.actual.loadByIdAndOpenDialog(1, void 0, resolve, reject));
        expect(fetchSpy.requests.length).toBe(1);
        expect(dlg.actual.entityId).toBe(1);
        expect(dlg.getForm(RegionForm).RegionDescription.value).toBe("Eastern");
    });

    it("calls create service on save button click for new mode", async () => {
        const dlg = new EntityDialogWrapper(new RegionDialog());
        dlg.actual.loadNewAndOpenDialog();
        const form = dlg.getForm(RegionForm);
        typeNumber(form.RegionID, 1);
        typeText(form.RegionDescription, "Eastern");
        const fetchSpy = mockFetch({
            [RegionService.Methods.Create]: (info) => {
                expect(info.data).toStrictEqual({
                    Entity: { RegionID: 1, RegionDescription: "Eastern" }
                } satisfies SaveRequest<RegionRow>);
                return { EntityId: 1 } satisfies SaveResponse;
            }
        });
        await dlg.clickSaveButton();
        expect(fetchSpy.requests.length).toBe(1);
    });

    it("calls update service on save button click for edit mode", async () => {
        const dlg = new EntityDialogWrapper(new RegionDialog());
        dlg.actual.loadEntityAndOpenDialog({ RegionID: 1, RegionDescription: "Eastern" });
        typeText(dlg.getForm(RegionForm).RegionDescription, "Western");
        const fetchSpy = mockFetch({
            [RegionService.Methods.Update]: (info) => {
                expect(info.data).toStrictEqual({
                    EntityId: 1,
                    Entity: { RegionID: 1, RegionDescription: "Western" }
                } satisfies SaveRequest<RegionRow>);
                return { EntityId: 1 } satisfies SaveResponse;
            }
        });
        await dlg.clickSaveButton();
        expect(fetchSpy.requests.length).toBe(1);
    });

    it("calls delete service on delete button click", async () => {
        const dlg = new EntityDialogWrapper(new RegionDialog());
        dlg.actual.loadEntityAndOpenDialog({ RegionID: 1, RegionDescription: "Eastern" });
        const fetchSpy = mockFetch({
            [RegionService.Methods.Delete]: (info) => {
                expect(info.data).toStrictEqual({ EntityId: 1 } satisfies DeleteRequest);
                return {} satisfies DeleteResponse;
            }
        });
        await dlg.clickDeleteButton();
        expect(fetchSpy.requests.length).toBe(1);
    });
});
