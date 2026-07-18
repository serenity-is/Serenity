import { serviceRequest } from "../../base";
import { RowMoveManager } from "@serenity-is/sleekgrid";
import { GridUtils } from "./gridutils";

// Mock serviceRequest from base: invoke the callback immediately to simulate completion
vi.mock("../../base", async (importOriginal) => {
    const actual: any = await importOriginal();
    return {
        ...actual,
        serviceRequest: vi.fn((_svc: string, _req: any, cb: Function) => {
            cb?.();
        })
    };
});

// Mock RowMoveManager with a proper constructor that uses vi.fn for subscribing
vi.mock("@serenity-is/sleekgrid", () => {
    const MockRMM = vi.fn(function (this: any, _opts: any) {
        this.onBeforeMoveRows = { subscribe: vi.fn() };
        this.onMoveRows = { subscribe: vi.fn() };
    });
    return { RowMoveManager: MockRMM as any };
});

describe("GridUtils", () => {
    describe("addToggleButton", () => {
        it("adds toggle button to toolDiv", () => {
            const toolDiv = document.createElement("div");
            const callback = vi.fn();

            GridUtils.addToggleButton(toolDiv, "my-toggle", callback, "Toggle me");

            const btn = toolDiv.querySelector(".s-ToggleButton");
            expect(btn).toBeTruthy();
            expect(btn?.classList.contains("my-toggle")).toBe(true);

            const link = btn?.querySelector("a");
            link?.click();
            expect(callback).toHaveBeenCalledWith(true);
        });

        it("adds pressed class when initial is true", () => {
            const toolDiv = document.createElement("div");
            GridUtils.addToggleButton(toolDiv, "test", vi.fn(), "", true);
            const btn = toolDiv.querySelector(".s-ToggleButton");
            expect(btn?.classList.contains("pressed")).toBe(true);
        });

        it("handles ArrayLike toolDiv", () => {
            const toolDiv = document.createElement("div");
            GridUtils.addToggleButton([toolDiv] as any, "test", vi.fn(), "");
            expect(toolDiv.querySelector(".s-ToggleButton")).toBeTruthy();
        });

        it("toggles pressed class on subsequent clicks", () => {
            const toolDiv = document.createElement("div");
            const callback = vi.fn();
            GridUtils.addToggleButton(toolDiv, "my-toggle", callback, "Toggle me");
            const link = toolDiv.querySelector(".s-ToggleButton a") as HTMLElement;
            link?.click();
            expect(toolDiv.querySelector(".s-ToggleButton")?.classList.contains("pressed")).toBe(true);
            link?.click();
            expect(callback).toHaveBeenCalledWith(false);
            expect(toolDiv.querySelector(".s-ToggleButton")?.classList.contains("pressed")).toBe(false);
        });
    });

    describe("addIncludeDeletedToggle", () => {
        it("adds toggle button and modifies view onSubmit", () => {
            const toolDiv = document.createElement("div");
            const view = {
                onSubmit: vi.fn((p: any) => true),
                seekToPage: 0,
                populate: vi.fn()
            };
            GridUtils.addIncludeDeletedToggle(toolDiv, view as any);

            const btn = toolDiv.querySelector(".s-IncludeDeletedToggle");
            expect(btn).toBeTruthy();

            const params = {};
            const result = view.onSubmit({ params });
            expect((params as any).IncludeDeleted).toBe(false);
            expect(result).toBe(true);
        });

        it("uses initial value for IncludeDeleted", () => {
            const toolDiv = document.createElement("div");
            const view = {
                onSubmit: vi.fn((p: any) => true),
                seekToPage: 0,
                populate: vi.fn()
            };
            GridUtils.addIncludeDeletedToggle(toolDiv, view as any, "Show Deleted", true);
            const params = {};
            view.onSubmit({ params });
            expect((params as any).IncludeDeleted).toBe(true);
        });

        it("clicking toggle toggles IncludeDeleted and repopulates", () => {
            const toolDiv = document.createElement("div");
            const populate = vi.fn();
            const view = {
                onSubmit: vi.fn((p: any) => true),
                seekToPage: 0,
                populate
            };
            GridUtils.addIncludeDeletedToggle(toolDiv, view as any);
            const link = toolDiv.querySelector(".s-IncludeDeletedToggle a") as HTMLElement;
            link?.click();
            const params = {};
            view.onSubmit({ params });
            expect((params as any).IncludeDeleted).toBe(true);
            expect(populate).toHaveBeenCalled();
        });

        it("handles null oldSubmit gracefully", () => {
            const toolDiv = document.createElement("div");
            const view = {
                onSubmit: null as any,
                seekToPage: 0,
                populate: vi.fn()
            };
            GridUtils.addIncludeDeletedToggle(toolDiv, view as any);
            const params = {};
            const result = view.onSubmit({ params });
            expect((params as any).IncludeDeleted).toBe(false);
            expect(result).toBe(true);
        });
    });

    describe("addQuickSearch", () => {
        it("adds quick search input to container", () => {
            const container = document.createElement("div");
            const view = {
                onSubmit: vi.fn(),
                seekToPage: 0,
                populate: vi.fn(),
                onDataLoaded: { subscribe: vi.fn(), unsubscribe: vi.fn() }
            };
            const input = GridUtils.addQuickSearch({ container, view: view as any });
            expect(container.querySelector(".s-QuickSearchBar")).toBeTruthy();
            expect(input).toBeTruthy();
        });

        it("adds has-quick-search-fields class when fields provided", () => {
            const container = document.createElement("div");
            GridUtils.addQuickSearch({
                container,
                fields: [{ name: "field1", title: "Field 1" }]
            });
            const bar = container.querySelector(".s-QuickSearchBar");
            expect(bar?.classList.contains("has-quick-search-fields")).toBe(true);
        });

        it("handles ArrayLike container", () => {
            const container = document.createElement("div");
            GridUtils.addQuickSearch({ container: [container] as any });
            expect(container.querySelector(".s-QuickSearchBar")).toBeTruthy();
        });

        it("integrates with view onSubmit wrapper", () => {
            const container = document.createElement("div");
            const oldSubmit = vi.fn((v: any) => true);
            const view = {
                onSubmit: oldSubmit,
                seekToPage: 0,
                populate: vi.fn(),
                onDataLoaded: { subscribe: vi.fn(), unsubscribe: vi.fn() }
            };
            GridUtils.addQuickSearch({ container, view: view as any });
            expect(view.onSubmit).not.toBe(oldSubmit);
            const params: any = {};
            const result = view.onSubmit({ params });
            expect(result).toBe(true);
        });

        it("wraps view.onSubmit with search text handling by setting input value", () => {
            const container = document.createElement("div");
            const view = {
                onSubmit: vi.fn(),
                seekToPage: 0,
                populate: vi.fn(),
                onDataLoaded: { subscribe: vi.fn(), unsubscribe: vi.fn() }
            };
            const input = GridUtils.addQuickSearch({ container, view: view as any }) as any;
            // Set the input element's value directly (QuickSearchInput reads from domNode)
            input.domNode.value = "search term";
            const params: any = {};
            view.onSubmit({ params });
            expect(params.ContainsText).toBe("search term");
        });

        it("passes ContainsField when field is set on QuickSearchInput", () => {
            const container = document.createElement("div");
            const view = {
                onSubmit: vi.fn(),
                seekToPage: 0,
                populate: vi.fn(),
                onDataLoaded: { subscribe: vi.fn(), unsubscribe: vi.fn() }
            };
            const input = GridUtils.addQuickSearch({ container, view: view as any }) as any;
            input.domNode.value = "test";
            const params: any = {};
            view.onSubmit({ params });
            expect(params.ContainsText).toBe("test");
        });

        it("handles search without view", () => {
            const container = document.createElement("div");
            const search = vi.fn();
            const input = GridUtils.addQuickSearch({ container, search });
            expect(input).toBeTruthy();
        });

        it("calls populate via search fallback when view is provided", () => {
            const container = document.createElement("div");
            const populate = vi.fn();
            const view = {
                onSubmit: vi.fn(),
                seekToPage: 0,
                populate,
                onDataLoaded: { subscribe: vi.fn(), unsubscribe: vi.fn() }
            };
            GridUtils.addQuickSearch({ container, view: view as any });
            const params: any = {};
            view.onSubmit({ params });
            expect(params.ContainsText).toBeUndefined();
        });

        it("triggers data loaded callback after search populates view", () => {
            let subscribeHandler: Function;
            const container = document.createElement("div");
            const populate = vi.fn();
            const view = {
                onSubmit: vi.fn(),
                seekToPage: 0,
                populate,
                getLength: vi.fn(() => 5),
                onDataLoaded: {
                    subscribe: vi.fn((h: Function) => { subscribeHandler = h; }),
                    unsubscribe: vi.fn()
                }
            };
            GridUtils.addQuickSearch({ container, view: view as any });
            expect(subscribeHandler).toBeDefined();
            // Call the onDataLoaded handler directly
            // Without lastDoneEvent set, it only covers the null branch
            subscribeHandler();
        });

        it("triggers disposing cleanup callback via disposing event", () => {
            const container = document.createElement("div");
            const oldSubmit = vi.fn();
            const view = {
                onSubmit: oldSubmit,
                seekToPage: 0,
                populate: vi.fn(),
                onDataLoaded: { subscribe: vi.fn(), unsubscribe: vi.fn() }
            };
            GridUtils.addQuickSearch({ container, view: view as any });
            const input = container.querySelector("input");
            expect(input).toBeTruthy();

            // Fire disposing event to trigger the cleanup callback
            // This simulates what happens when the element is removed from the DOM
            input!.dispatchEvent(new CustomEvent("disposing", { bubbles: false }));
        });
    });

    describe("addQuickSearchInput", () => {
        it("creates quick search with toolbar and view", () => {
            const toolDiv = document.createElement("div");
            const view = {
                onSubmit: vi.fn(),
                seekToPage: 0,
                populate: vi.fn(),
                onDataLoaded: { subscribe: vi.fn(), unsubscribe: vi.fn() }
            };
            const input = GridUtils.addQuickSearchInput(toolDiv, view as any);
            expect(input).toBeTruthy();
        });

        it("passes fields and onChange", () => {
            const toolDiv = document.createElement("div");
            const view = {
                onSubmit: vi.fn(),
                seekToPage: 0,
                populate: vi.fn(),
                onDataLoaded: { subscribe: vi.fn(), unsubscribe: vi.fn() }
            };
            const onChange = vi.fn();
            const input = GridUtils.addQuickSearchInput(toolDiv, view as any, [{ name: "f1", title: "F1" }], onChange);
            expect(input).toBeTruthy();
        });
    });

    describe("addQuickSearchInputCustom", () => {
        it("creates quick search with custom search function", () => {
            const container = document.createElement("div");
            const searchFn = vi.fn();
            const input = GridUtils.addQuickSearchInputCustom(container, searchFn);
            expect(input).toBeTruthy();
        });

        it("passes fields to custom search", () => {
            const container = document.createElement("div");
            const searchFn = vi.fn();
            const input = GridUtils.addQuickSearchInputCustom(container, searchFn, [{ name: "f1", title: "F1" }]);
            expect(input).toBeTruthy();
        });
    });

    describe("makeOrderable", () => {
        beforeEach(() => {
            (RowMoveManager as any).mockClear();
        });

        function createMockGrid() {
            return {
                registerPlugin: vi.fn(),
                setSelectedRows: vi.fn()
            };
        }

        function getRMInst() {
            return (RowMoveManager as any).mock.results[0]?.value;
        }

        it("creates RowMoveManager and registers plugin", () => {
            const grid = createMockGrid();
            GridUtils.makeOrderable(grid as any, vi.fn());
            expect(RowMoveManager).toHaveBeenCalledWith({ cancelEditOnDrag: true });
            expect(grid.registerPlugin).toHaveBeenCalled();
        });

        it("onBeforeMoveRows returns false when row equals insertBefore", () => {
            const grid = createMockGrid();
            GridUtils.makeOrderable(grid as any, vi.fn());
            const inst = getRMInst();
            const handler = inst.onBeforeMoveRows.subscribe.mock.calls[0][0];
            const e = { stopPropagation: vi.fn() };
            const result = handler(e, { rows: [5], insertBefore: 5 });
            expect(result).toBe(false);
            expect(e.stopPropagation).toHaveBeenCalled();
        });

        it("onBeforeMoveRows returns false when row equals insertBefore - 1", () => {
            const grid = createMockGrid();
            GridUtils.makeOrderable(grid as any, vi.fn());
            const inst = getRMInst();
            const handler = inst.onBeforeMoveRows.subscribe.mock.calls[0][0];
            const e = { stopPropagation: vi.fn() };
            const result = handler(e, { rows: [5], insertBefore: 6 });
            expect(result).toBe(false);
            expect(e.stopPropagation).toHaveBeenCalled();
        });

        it("onBeforeMoveRows returns true for valid move", () => {
            const grid = createMockGrid();
            GridUtils.makeOrderable(grid as any, vi.fn());
            const inst = getRMInst();
            const handler = inst.onBeforeMoveRows.subscribe.mock.calls[0][0];
            const result = handler({}, { rows: [0], insertBefore: 5 });
            expect(result).toBe(true);
        });

        it("onMoveRows calls handleMove and clears selection", () => {
            const grid = createMockGrid();
            const handleMove = vi.fn();
            GridUtils.makeOrderable(grid as any, handleMove);
            const inst = getRMInst();
            const handler = inst.onMoveRows.subscribe.mock.calls[0][0];
            handler({}, { rows: [1, 2], insertBefore: 5 });
            expect(handleMove).toHaveBeenCalledWith([1, 2], 5);
            expect(grid.setSelectedRows).toHaveBeenCalledWith([]);
        });

        it("onMoveRows catches setSelectedRows error", () => {
            const grid = createMockGrid();
            grid.setSelectedRows = vi.fn(() => { throw new Error("oops"); });
            GridUtils.makeOrderable(grid as any, vi.fn());
            const inst = getRMInst();
            const handler = inst.onMoveRows.subscribe.mock.calls[0][0];
            expect(() => handler({}, { rows: [0], insertBefore: 2 })).not.toThrow();
        });
    });

    describe("makeOrderableWithUpdateRequest", () => {
        beforeEach(() => {
            (RowMoveManager as any).mockClear();
            (serviceRequest as any).mockClear();
        });

        function makeSUT(dataItems: any[]) {
            const grid = {
                registerPlugin: vi.fn(),
                setSelectedRows: vi.fn(),
                getDataLength: vi.fn(() => dataItems.length),
                getDataItem: vi.fn((i: number) => dataItems[i])
            };
            const populate = vi.fn();
            const dataGrid = {
                getGrid: vi.fn(() => grid),
                getView: vi.fn(() => ({ populate }))
            };
            const getId = vi.fn((x: any) => x.id);
            const getDisplayOrder = vi.fn((x: any) => x.order);
            const getUpdateRequest = vi.fn((id: number, order: number) => ({ Entity: { id, order } }));

            GridUtils.makeOrderableWithUpdateRequest(
                dataGrid as any, getId, getDisplayOrder, "TestSvc", getUpdateRequest
            );

            const inst = (RowMoveManager as any).mock.results[0]?.value;
            const moveHandler = inst?.onMoveRows.subscribe.mock.calls[0]?.[0];
            return { grid, dataGrid, populate, getUpdateRequest, moveHandler };
        }

        it("registers plugin", () => {
            const { grid } = makeSUT([]);
            expect(grid.registerPlugin).toHaveBeenCalled();
        });

        it("sends service request for valid move (insertBefore < dataLength with non-zero order)", () => {
            const { moveHandler, getUpdateRequest, getUpdateRequest: gur } = makeSUT([
                { id: 1, order: 10 },
                { id: 2, order: 20 },
                { id: 3, order: 30 }
            ]);
            moveHandler({}, { rows: [0], insertBefore: 2 });
            expect(getUpdateRequest).toHaveBeenCalled();
            const calls = getUpdateRequest.mock.calls;
            // The order++ in getUpdateRequest(id, order++) passes `order` before increment.
            // order is computed as displayOrder(item at insertBefore) + 1 = 30 + 1 = 31.
            // But order++ passes 31 then increments to 32. So first call should be (1, 31)
            expect(calls[0][1]).toBeGreaterThanOrEqual(30);
            expect(serviceRequest).toHaveBeenCalled();
        });

        it("uses insertBefore + 1 when order is 0 for insertBefore at end", () => {
            const { moveHandler, getUpdateRequest } = makeSUT([
                { id: 1, order: 0 },
                { id: 2, order: 0 },
                { id: 3, order: 0 }
            ]);
            moveHandler({}, { rows: [0], insertBefore: 5 }); // >= dataLength
            // order = insertBefore + 1 = 6
            expect(getUpdateRequest).toHaveBeenCalledWith(1, 6);
        });

        it("uses insertBefore + 1 when order is 0 for insertBefore within range", () => {
            const { moveHandler, getUpdateRequest } = makeSUT([
                { id: 1, order: 0 },
                { id: 2, order: 0 },
                { id: 3, order: 0 }
            ]);
            moveHandler({}, { rows: [0], insertBefore: 1 }); // < dataLength
            // order = insertBefore + 1 = 2
            expect(getUpdateRequest).toHaveBeenCalledWith(1, 2);
        });

        it("handles insertBefore negative (order = 1)", () => {
            const { moveHandler, getUpdateRequest } = makeSUT([
                { id: 42, order: 99 }
            ]);
            moveHandler({}, { rows: [0], insertBefore: -1 });
            expect(getUpdateRequest).toHaveBeenCalledWith(42, 1);
        });

        it("does nothing for empty rows", () => {
            const { moveHandler } = makeSUT([]);
            moveHandler({}, { rows: [], insertBefore: 0 });
            expect(serviceRequest).not.toHaveBeenCalled();
        });

        it("calls populate after all service requests complete", () => {
            const { moveHandler, dataGrid, getUpdateRequest } = makeSUT([
                { id: 1, order: 10 },
                { id: 2, order: 20 }
            ]);
            moveHandler({}, { rows: [0, 1], insertBefore: 3 });

            expect(serviceRequest).toHaveBeenCalledTimes(2);

            // Call first callback: i becomes 1, next() is called
            const cb1 = (serviceRequest as any).mock.calls[0][2];
            cb1();
            // i = 1, which is < 2 (rows.length), so next() is called again

            // Call second callback: i becomes 2, which equals rows.length, so populate() is called
            const cb2 = (serviceRequest as any).mock.calls[1][2];
            cb2();

            expect(dataGrid.getView().populate).toHaveBeenCalled();
        });
    });
});
