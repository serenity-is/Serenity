import * as corelib from "@serenity-is/corelib";
import { PropertyDialog } from "@serenity-is/corelib";
import { EntityDialogWrapper, mockAdmin, mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import { StaticTextBlockForm } from "../../Modules/ServerTypes/Demo";
import initPage, { StaticTextBlockDialog } from "../../Modules/Editors/StaticTextBlock/StaticTextBlockPage";

beforeAll(() => {
    mockDynamicData();
    mockAdmin();
    mockGridSize();
    mockFetch({ "*": () => ({ Entities: [], TotalCount: 0 }) });
});

afterAll(() => {
    unmockFetch();
});

afterEach(() => {
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

describe("StaticTextBlockPage", () => {
    it("opens the static text block dialog", () => {
        const open = vi.spyOn(StaticTextBlockDialog.prototype, "dialogOpen").mockImplementation((() => ({})) as any);
        initPage();
        expect(open).toHaveBeenCalled();
    });
});

describe("StaticTextBlockDialog", () => {
    it("extends the property dialog and wires the form", () => {
        const wrapper = new EntityDialogWrapper(new StaticTextBlockDialog({}) as any);
        expect(wrapper.actual instanceof PropertyDialog).toBe(true);
        expect(wrapper.actual["getFormKey"]()).toBe(StaticTextBlockForm.formKey);
        expect(wrapper.actual["dialogTitle"]).toBe("A form with static text blocks");
        wrapper.actual.destroy();
    });

    it("sets width and non modal dialog options", () => {
        const wrapper = new EntityDialogWrapper(new StaticTextBlockDialog({}) as any);
        const opt = wrapper.actual["getDialogOptions"]();
        expect(opt.width).toBe(650);
        expect(opt.modal).toBe(false);
        wrapper.actual.destroy();
    });

    it("loads the initial display field value", () => {
        const wrapper = new EntityDialogWrapper(new StaticTextBlockDialog({}) as any);
        const load = vi.spyOn(wrapper.actual["propertyGrid"], "load");
        wrapper.actual["loadInitialEntity"]();
        expect(load).toHaveBeenCalledWith(expect.objectContaining({
            DisplayFieldValue: expect.stringContaining("DisplayFieldValue")
        }));
        wrapper.actual.destroy();
    });
});
