import * as corelib from "@serenity-is/corelib";
import { PropertyDialog } from "@serenity-is/corelib";
import { EntityDialogWrapper, mockAdmin, mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import { afterAll, afterEach, beforeAll, describe, expect, it, vi } from "vitest";
import initPage, { HardcodedValuesDialog, HardcodedValuesEditor } from "../../Modules/Editors/SelectWithHardcodedValues/SelectWithHardcodedValuesPage";
import { HardcodedValuesForm } from "../../Modules/ServerTypes/Demo";

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

function setupDom() {
    const usingWidget = document.createElement("div");
    usingWidget.id = "UsingWidgetCreate";
    usingWidget.innerHTML = "<label>Label</label>";
    document.body.append(usingWidget);
    const creating = document.createElement("div");
    creating.id = "CreatingOnInput";
    creating.innerHTML = "<input />";
    document.body.append(creating);
}

describe("SelectWithHardcodedValuesPage", () => {
    it("opens the dialog and creates the editors", () => {
        setupDom();
        const open = vi.spyOn(HardcodedValuesDialog.prototype, "dialogOpen").mockImplementation((() => ({})) as any);
        initPage();
        expect(open).toHaveBeenCalled();
        expect(document.querySelector("#UsingWidgetCreate label")).toBeTruthy();
        expect(document.querySelector("#CreatingOnInput input")).toBeTruthy();
    });
});

describe("HardcodedValuesEditor", () => {
    it("adds hardcoded options and items", () => {
        const input = document.body.appendChild(document.createElement("input"));
        const editor = new HardcodedValuesEditor({ element: input });
        const items = editor["get_items"]();
        expect(items.map(x => x.id)).toEqual(["key1", "key2", "key3", "key4"]);
        expect(items.find(x => x.id == "key4")?.disabled).toBe(true);
        editor.destroy();
    });
});

describe("HardcodedValuesDialog", () => {
    it("extends the property dialog and wires the form", () => {
        const wrapper = new EntityDialogWrapper(new HardcodedValuesDialog({}) as any);
        expect(wrapper.actual instanceof PropertyDialog).toBe(true);
        expect(wrapper.actual["getFormKey"]()).toBe(HardcodedValuesForm.formKey);
        expect(wrapper.actual["dialogTitle"]).toBe("Please select some value");
        expect(wrapper.actual["getDialogOptions"]().modal).toBe(false);
        wrapper.actual.destroy();
    });

    it("notifies when a value is selected", () => {
        const wrapper = new EntityDialogWrapper(new HardcodedValuesDialog({}) as any);
        const notify = vi.spyOn(corelib, "notifySuccess").mockImplementation((() => ({})) as any);
        const form = wrapper.actual["form"];
        form.SomeValue.element.val("key1");
        form.SomeValue.element.trigger("change");
        expect(notify).toHaveBeenCalledWith(expect.stringContaining("key1"));
        wrapper.actual.destroy();
    });
});
