import * as corelib from "@serenity-is/corelib";
import { CustomerDialog, CustomerGrid, CustomerService } from "@serenity-is/demo.northwind";
import { EntityDialogWrapper, mockAdmin, mockDynamicData, mockFetch, mockGridSize, unmockFetch, waitUntil } from "test-utils";
import { afterAll, afterEach, beforeAll, describe, expect, it, vi } from "vitest";
import initPage, { SerialAutoNumberDialog, SerialAutoNumberGrid } from "../../Modules/Dialogs/SerialAutoNumber/SerialAutoNumberPage";

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

let lastOnSuccess: any;
function lastCallSpy() {
    return vi.spyOn(CustomerService, "GetNextNumber").mockImplementation(((request: any, onSuccess: any) => {
        lastOnSuccess = onSuccess;
        return Promise.resolve({}) as any;
    }) as any);
}

describe("SerialAutoNumberPage", () => {
    it("initializes the grid page", () => {
        const spy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(spy).toHaveBeenCalledWith(SerialAutoNumberGrid);
    });
});

describe("SerialAutoNumberGrid", () => {
    it("extends the customer grid and uses the custom dialog", () => {
        const grid = new SerialAutoNumberGrid({});
        expect(grid instanceof CustomerGrid).toBe(true);
        expect(grid["getDialogType"]()).toBe(SerialAutoNumberDialog);
        grid.destroy();
    });
});

describe("SerialAutoNumberDialog", () => {
    it("extends the customer dialog", () => {
        const dialog = new SerialAutoNumberDialog({});
        expect(dialog instanceof CustomerDialog).toBe(true);
        dialog.destroy();
    });

    it("gets the next number for a new record and applies the serial", async () => {
        const getNext = lastCallSpy();
        const wrapper = new EntityDialogWrapper(new SerialAutoNumberDialog({}));
        wrapper.actual.loadNewAndOpenDialog();
        await waitUntil(() => !!lastOnSuccess);

        expect(getNext).toHaveBeenCalledWith({ Prefix: "C", Length: 5 }, expect.any(Function));
        lastOnSuccess({ Serial: "C00001" });
        expect(wrapper.actual["form"].CustomerID.value).toBe("C00001");
        wrapper.actual.destroy();
    });

    it("gets the next number when a letter key is pressed", async () => {
        lastCallSpy();
        const wrapper = new EntityDialogWrapper(new SerialAutoNumberDialog({}));
        wrapper.actual.loadNewAndOpenDialog();
        await waitUntil(() => !!lastOnSuccess);

        const getNext = vi.spyOn(wrapper.actual as any, "getNextNumber");
        const el = wrapper.actual["form"].CustomerID.element[0] as HTMLInputElement;
        el.dispatchEvent(new KeyboardEvent("keyup", { key: "A", bubbles: true }));
        expect(getNext).toHaveBeenCalledTimes(1);
        el.dispatchEvent(new KeyboardEvent("keyup", { key: "1", bubbles: true }));
        expect(getNext).toHaveBeenCalledTimes(1);
        el.dispatchEvent(new KeyboardEvent("keyup", { key: "a", bubbles: true }));
        expect(getNext).toHaveBeenCalledTimes(2);
        wrapper.actual.destroy();
    });

    it("does not fetch a number for longer values", async () => {
        const getNext = lastCallSpy();
        const wrapper = new EntityDialogWrapper(new SerialAutoNumberDialog({}));
        wrapper.actual.loadNewAndOpenDialog();
        await waitUntil(() => !!lastOnSuccess);

        wrapper.actual["form"].CustomerID.value = "ALF";
        getNext.mockClear();
        wrapper.actual["getNextNumber"]();
        expect(getNext).not.toHaveBeenCalled();
        wrapper.actual.destroy();
    });

    it("uses the typed prefix when a single character is present", async () => {
        const getNext = lastCallSpy();
        const wrapper = new EntityDialogWrapper(new SerialAutoNumberDialog({}));
        wrapper.actual.loadNewAndOpenDialog();
        await waitUntil(() => !!lastOnSuccess);

        wrapper.actual["form"].CustomerID.value = "a";
        getNext.mockClear();
        wrapper.actual["getNextNumber"]();
        expect(getNext).toHaveBeenCalledWith({ Prefix: "A", Length: 5 }, expect.any(Function));
        lastOnSuccess({ Serial: "A00001" });
        expect(wrapper.actual["form"].CustomerID.value).toBe("A00001");
        wrapper.actual.destroy();
    });
});
