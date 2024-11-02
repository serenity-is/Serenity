import { DeleteRequest, DeleteResponse, Dialog, RetrieveRequest, RetrieveResponse, SaveRequest, SaveResponse } from "@serenity-is/corelib";
import { EntityDialogWrapper, mockAdmin, mockDynamicData, mockFetch, typeText, unmockFetch } from "test-utils";
import { LanguageDialog } from "../../../Modules/Administration/Language/LanguageDialog";
import { LanguageForm, LanguageRow, LanguageService } from "../../../Modules/ServerTypes/Administration";

beforeAll(() => {
    mockDynamicData();
    mockAdmin();
});

afterEach(() => {
    unmockFetch();
});

describe("LanguageDialog", () => {
    it("can load new entity and open", () => {
        const dlg = new LanguageDialog();
        dlg.loadNewAndOpenDialog();
        const instance = Dialog.getInstance(dlg.element);
        expect(instance).toBeTruthy();
        expect(instance.getDialogNode()).toBeTruthy();
    });

    it("calls retrieve service on loadById", async () => {
        const fetchSpy = mockFetch({
            [LanguageService.Methods.Retrieve]: (info) => {
                expect(info.data).toStrictEqual({
                    EntityId: 7
                } satisfies RetrieveRequest);
                return {
                    Entity: {
                        Id: 7,
                        LanguageId: "en",
                        LanguageName: "English"
                    }
                } satisfies RetrieveResponse<LanguageRow>;
            }
        });
        const dlg = new EntityDialogWrapper(new LanguageDialog());
        await new Promise((resolve, reject) => dlg.actual.loadByIdAndOpenDialog(7, void 0, resolve, reject));
        expect(fetchSpy.requests.length).toBe(1);
        expect(dlg.actual.entityId).toBe(7);
        const form = dlg.getForm(LanguageForm);
        expect(form.LanguageId.value).toBe("en");
        expect(form.LanguageName.value).toBe("English");
    });

    it("calls create service on save button click for new mode", async () => {
        const dlg = new EntityDialogWrapper(new LanguageDialog());
        dlg.actual.loadNewAndOpenDialog();
        const form = dlg.getForm(LanguageForm);
        typeText(form.LanguageId, "en");
        typeText(form.LanguageName, "English");
        const fetchSpy = mockFetch({
            [LanguageService.Methods.Create]: (info) => {
                expect(info.data).toStrictEqual({
                    Entity: {
                        LanguageId: "en",
                        LanguageName: "English"
                    }
                } satisfies SaveRequest<LanguageRow>);
                return { EntityId: 1 } satisfies SaveResponse;
            }
        });
        await dlg.clickSaveButton();
        expect(fetchSpy.requests.length).toBe(1);
    });

    it("calls update service on save button click for edit mode", async () => {
        const dlg = new EntityDialogWrapper(new LanguageDialog());
        dlg.actual.loadEntityAndOpenDialog({
            Id: 7,
            LanguageId: "en",
            LanguageName: "English"
        });

        const form = dlg.getForm(LanguageForm);
        typeText(form.LanguageName, "UpdatedEnglish");

        const fetchSpy = mockFetch({
            [LanguageService.Methods.Update]: (info) => {
                expect(info.data).toStrictEqual({
                    EntityId: 7,
                    Entity: {
                        Id: 7,
                        LanguageId: "en",
                        LanguageName: "UpdatedEnglish"
                    }
                } satisfies SaveRequest<LanguageRow>);
                return { EntityId: 7 } satisfies SaveResponse;
            }
        });
        await dlg.clickSaveButton();
        expect(fetchSpy.requests.length).toBe(1);
    });

    it("calls delete service on delete button click", async () => {
        const dlg = new EntityDialogWrapper(new LanguageDialog());
        dlg.actual.loadEntityAndOpenDialog({
            Id: 7,
            LanguageId: "en",
            LanguageName: "English"
        });

        const fetchSpy = mockFetch({
            [LanguageService.Methods.Delete]: (info) => {
                expect(info.data).toStrictEqual({
                    EntityId: 7
                } satisfies DeleteRequest);
                return { } satisfies DeleteResponse;
            }
        });
        await dlg.clickDeleteButton();
        expect(fetchSpy.requests.length).toBe(1);
    });

});