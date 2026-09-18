import * as corelib from "@serenity-is/corelib";
import { OrderDialog } from "@serenity-is/demo.northwind";
import { mockAdmin, mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import initPage, { EntityDialogAsPanel } from "../../Modules/Dialogs/EntityDialogAsPanel/EntityDialogAsPanelPage";

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

function setupPanel() {
    const section = document.body.appendChild(document.createElement("section"));
    section.className = "content";
    const panel = document.createElement("div");
    panel.id = "PanelDiv";
    section.append(panel);
    return section;
}

function stubLoad() {
    const load = vi.spyOn(EntityDialogAsPanel.prototype, "load").mockImplementation(((entity: any, done: any) => {
        done?.();
    }) as any);
    const arrange = vi.spyOn(EntityDialogAsPanel.prototype, "arrange").mockImplementation((() => ({})) as any);
    return { load, arrange };
}

describe("EntityDialogAsPanelPage", () => {
    it("loads a new entity when there is no model", () => {
        setupPanel();
        const { load, arrange } = stubLoad();
        initPage(null);
        expect(load).toHaveBeenCalledWith({}, expect.any(Function));
        expect(arrange).toHaveBeenCalled();
    });

    it("loads the order id provided as model", () => {
        setupPanel();
        const { load } = stubLoad();
        initPage({ OrderID: 11068 });
        expect(load).toHaveBeenCalledWith({ OrderID: 11068 }, expect.any(Function));
    });

    it("switches to new record mode and loads by id via links", () => {
        const panel = setupPanel();
        const { load } = stubLoad();
        vi.spyOn(corelib, "notifySuccess").mockImplementation((() => ({})) as any);
        initPage(null);

        (panel.querySelector("#SwitchToNewRecordMode") as HTMLElement).click();
        expect(load).toHaveBeenCalledWith({}, expect.any(Function));

        (panel.querySelector("#LoadEntityWithId") as HTMLElement).click();
        expect(load).toHaveBeenCalledWith({ OrderID: 11048 }, expect.any(Function));
    });
});

describe("EntityDialogAsPanel", () => {
    it("extends the order dialog and hides delete/apply buttons", () => {
        const dialog = new EntityDialogAsPanel({});
        expect(dialog instanceof OrderDialog).toBe(true);
        const deleteHide = vi.spyOn(dialog["deleteButton"], "hide");
        const applyHide = vi.spyOn(dialog["applyChangesButton"], "hide");
        dialog["updateInterface"]();
        expect(deleteHide).toHaveBeenCalled();
        expect(applyHide).toHaveBeenCalled();
        dialog.destroy();
    });

    it("shows the save success message on save success", () => {
        const dialog = new EntityDialogAsPanel({});
        const show = vi.spyOn(dialog as any, "showSaveSuccessMessage").mockImplementation((() => ({})) as any);
        const response = { EntityId: 5 } as any;
        dialog["onSaveSuccess"](response, "save" as any);
        expect(show).toHaveBeenCalledWith(response, "save");
        dialog.destroy();
    });
});
