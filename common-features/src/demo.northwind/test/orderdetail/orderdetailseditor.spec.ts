import * as corelib from "@serenity-is/corelib";
import { mockDynamicData, mockFetch, mockGridSize, mockRowLookup, unmockFetch } from "test-utils";
import { OrderDetailDialog } from "../../Modules/OrderDetail/OrderDetailDialog";
import { OrderDetailsEditor } from "../../Modules/OrderDetail/OrderDetailsEditor";
import { OrderDetailColumns, OrderDetailRow, OrderDetailService, ProductRow } from "../../Modules/ServerTypes/Demo";

beforeAll(() => {
    mockDynamicData();
    mockGridSize();
    mockRowLookup(ProductRow, [
        { ProductID: 1, ProductName: "Chai", UnitPrice: 18 },
        { ProductID: 2, ProductName: "Chang", UnitPrice: 19 }
    ]);
});

beforeEach(() => {
    mockFetch({ "*": () => ({ Entities: [], TotalCount: 0 }) });
});

afterEach(() => {
    unmockFetch();
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

describe("OrderDetailsEditor", () => {
    it("wires up columns, dialog, row and service", () => {
        const editor = new OrderDetailsEditor({});
        expect(editor["getColumnsKey"]()).toBe(OrderDetailColumns.columnsKey);
        expect(editor["getDialogType"]()).toBe(OrderDetailDialog);
        expect(editor["getRowDefinition"]()).toBe(OrderDetailRow);
        expect(editor["getService"]()).toBe(OrderDetailService.baseUrl);
        editor.destroy();
    });

    it("tracks the order id and connected mode", () => {
        const editor = new OrderDetailsEditor({});
        editor.orderId = 5;
        expect(editor.orderId).toBe(5);
        expect(editor.connectedMode).toBe(true);
        expect(editor["getGridCanLoad"]()).toBe(true);
        editor.orderId = null;
        expect(editor.connectedMode).toBe(false);
        expect(editor["getGridCanLoad"]()).toBe(false);
        editor.destroy();
    });

    it("creates new entities with the current order id", () => {
        const editor = new OrderDetailsEditor({});
        editor.orderId = 5;
        expect(editor["getNewEntity"]()).toEqual({ OrderID: 5 });
        editor.destroy();
    });

    it("warns when the same product is added twice", async () => {
        const alert = vi.spyOn(corelib, "alertDialog").mockImplementation((() => { }) as any);
        const editor = new OrderDetailsEditor({});
        editor.view.setItems([{ DetailID: 1, ProductID: 1 }] as any, true);
        await expect(editor["validateEntity"]({ ProductID: 1 } as any, 999)).resolves.toBe(false);
        expect(alert).toHaveBeenCalled();
        editor.destroy();
    });

    it("accepts any entity in connected mode", async () => {
        const editor = new OrderDetailsEditor({});
        editor.orderId = 5;
        await expect(editor["validateEntity"]({ ProductID: 2 } as any, null)).resolves.toBe(true);
        editor.destroy();
    });

    it("fills product name and line total in disconnected mode", async () => {
        const editor = new OrderDetailsEditor({});
        const row: any = { ProductID: 2, Quantity: 2, UnitPrice: 3, Discount: 1 };
        await expect(editor["validateEntity"](row, null)).resolves.toBe(true);
        expect(row.ProductName).toBe("Chang");
        expect(row.LineTotal).toBe(5);
        editor.destroy();
    });

    it("ignores clicks without an inline action", () => {
        const editor = new OrderDetailsEditor({});
        vi.spyOn(Object.getPrototypeOf(OrderDetailsEditor.prototype), "onClick").mockImplementation(() => { });
        (editor as any).itemAt = () => ({ DetailID: 3 });
        const del = vi.spyOn(editor as any, "delete").mockImplementation(() => { });
        const span = document.createElement("span");
        const e = new MouseEvent("click", { bubbles: true, cancelable: true });
        span.dispatchEvent(e);
        editor["onClick"](e, 0, 0);
        expect(del).not.toHaveBeenCalled();
        editor.destroy();
    });

    it("ignores prevented inline action clicks", () => {
        const editor = new OrderDetailsEditor({});
        vi.spyOn(Object.getPrototypeOf(OrderDetailsEditor.prototype), "onClick").mockImplementation(() => { });
        (editor as any).itemAt = () => ({ DetailID: 3 });
        const del = vi.spyOn(editor as any, "delete").mockImplementation(() => { });
        const anchor = document.createElement("a");
        anchor.className = "inline-action";
        anchor.setAttribute("data-action", "delete");
        const e = new MouseEvent("click", { bubbles: true, cancelable: true });
        anchor.dispatchEvent(e);
        e.preventDefault();
        editor["onClick"](e, 0, 0);
        expect(del).not.toHaveBeenCalled();
        editor.destroy();
    });

    it("populates the view in connected mode after delete success", () => {
        vi.spyOn(corelib, "confirmDialog").mockImplementation((_msg: any, onYes: any) => { onYes(); return null as any; });
        const editor = new OrderDetailsEditor({});
        editor.orderId = 5;
        vi.spyOn(Object.getPrototypeOf(OrderDetailsEditor.prototype), "onClick").mockImplementation(() => { });
        (editor as any).itemAt = () => ({ DetailID: 3 });
        vi.spyOn(editor as any, "delete").mockImplementation((opt: any) => { opt.onSuccess?.(); });
        const populate = vi.spyOn(editor.view, "populate").mockImplementation((() => { }) as any);
        const anchor = document.createElement("a");
        anchor.className = "inline-action";
        anchor.setAttribute("data-action", "delete");
        const e = new MouseEvent("click", { bubbles: true, cancelable: true });
        anchor.dispatchEvent(e);
        editor["onClick"](e, 0, 0);
        expect(populate).toHaveBeenCalled();
        editor.destroy();
    });

    it("queues a confirmed delete on inline action click", () => {
        const confirm = vi.spyOn(corelib, "confirmDialog").mockImplementation((_msg: any, onYes: any) => { onYes(); return null as any; });
        const editor = new OrderDetailsEditor({});
        vi.spyOn(Object.getPrototypeOf(OrderDetailsEditor.prototype), "onClick").mockImplementation(() => { });
        (editor as any).itemAt = () => ({ DetailID: 3 });
        const del = vi.spyOn(editor as any, "delete").mockImplementation(() => { });
        const anchor = document.createElement("a");
        anchor.className = "inline-action";
        anchor.setAttribute("data-action", "delete");
        const e = new MouseEvent("click", { bubbles: true, cancelable: true });
        anchor.dispatchEvent(e);
        editor["onClick"](e, 0, 0);
        expect(confirm).toHaveBeenCalled();
        expect(del).toHaveBeenCalled();
        editor.destroy();
    });
});
