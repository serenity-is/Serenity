import { DeleteRequest, DeleteResponse, RetrieveRequest, RetrieveResponse, SaveRequest, SaveResponse } from "@serenity-is/corelib";
import { EntityDialogWrapper, mockAdmin, mockDynamicData, mockFetch, mockRowLookup, typeText, unmockFetch } from "test-utils";
import { afterEach, beforeAll, describe, expect, it } from "vitest";
import { RegionRow, TerritoryForm, TerritoryRow, TerritoryService } from "../../Modules/ServerTypes/Demo";
import { TerritoryDialog } from "../../Modules/Territory/TerritoryDialog";

beforeAll(() => {
    mockDynamicData();
    mockAdmin();
    mockRowLookup(RegionRow, [{ RegionID: 1, RegionDescription: "Eastern" }]);
});

afterEach(() => {
    unmockFetch();
});

describe("TerritoryDialog", () => {
    it("wires up form, row and service", () => {
        const dialog = new TerritoryDialog({});
        expect(dialog["getFormKey"]()).toBe(TerritoryForm.formKey);
        expect(dialog["getRowDefinition"]()).toBe(TerritoryRow);
        expect(dialog["getService"]()).toBe(TerritoryService.baseUrl);
        dialog.destroy();
    });

    it("calls retrieve service on loadById", async () => {
        const fetchSpy = mockFetch({
            [TerritoryService.Methods.Retrieve]: (info) => {
                expect(info.data).toStrictEqual({ EntityId: "01581" } satisfies RetrieveRequest);
                return {
                    Entity: { TerritoryID: "01581", TerritoryDescription: "Westboro", RegionID: 1 }
                } satisfies RetrieveResponse<TerritoryRow>;
            }
        });
        const dlg = new EntityDialogWrapper(new TerritoryDialog());
        await new Promise((resolve, reject) => dlg.actual.loadByIdAndOpenDialog("01581", void 0, resolve, reject));
        expect(fetchSpy.requests.length).toBe(1);
        expect(dlg.actual.entityId).toBe("01581");
        expect(dlg.getForm(TerritoryForm).TerritoryDescription.value).toBe("Westboro");
    });

    it("calls create service on save button click for new mode", async () => {
        const dlg = new EntityDialogWrapper(new TerritoryDialog());
        dlg.actual.loadNewAndOpenDialog();
        const form = dlg.getForm(TerritoryForm);
        typeText(form.TerritoryID, "01581");
        typeText(form.TerritoryDescription, "Westboro");
        typeText(form.RegionID, "1");
        const fetchSpy = mockFetch({
            [TerritoryService.Methods.Create]: (info) => {
                expect((info.data as SaveRequest<TerritoryRow>).Entity).toMatchObject({ TerritoryID: "01581", TerritoryDescription: "Westboro", RegionID: "1" });
                return { EntityId: "01581" } satisfies SaveResponse;
            }
        });
        await dlg.clickSaveButton();
        expect(fetchSpy.requests.length).toBe(1);
    });

    it("calls update service on save button click for edit mode", async () => {
        const dlg = new EntityDialogWrapper(new TerritoryDialog());
        dlg.actual.loadEntityAndOpenDialog({ TerritoryID: "01581", TerritoryDescription: "Westboro", RegionID: 1 });
        typeText(dlg.getForm(TerritoryForm).TerritoryDescription, "Eastboro");
        const fetchSpy = mockFetch({
            [TerritoryService.Methods.Update]: (info) => {
                expect((info.data as SaveRequest<TerritoryRow>).EntityId).toBe("01581");
                expect((info.data as SaveRequest<TerritoryRow>).Entity).toMatchObject({ TerritoryID: "01581", TerritoryDescription: "Eastboro", RegionID: "1" });
                return { EntityId: "01581" } satisfies SaveResponse;
            }
        });
        await dlg.clickSaveButton();
        expect(fetchSpy.requests.length).toBe(1);
    });

    it("calls delete service on delete button click", async () => {
        const dlg = new EntityDialogWrapper(new TerritoryDialog());
        dlg.actual.loadEntityAndOpenDialog({ TerritoryID: "01581", TerritoryDescription: "Westboro", RegionID: 1 });
        const fetchSpy = mockFetch({
            [TerritoryService.Methods.Delete]: (info) => {
                expect(info.data).toStrictEqual({ EntityId: "01581" } satisfies DeleteRequest);
                return {} satisfies DeleteResponse;
            }
        });
        await dlg.clickDeleteButton();
        expect(fetchSpy.requests.length).toBe(1);
    });
});
