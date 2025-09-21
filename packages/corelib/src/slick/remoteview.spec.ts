import { vi } from "vitest";
import { serviceCall } from "../base";
import { Aggregators } from "./aggregators";
import { RemoteView } from "./remoteview";

vi.mock("../base", async (importActual) => ({
    ...await importActual(),
    serviceCall: vi.fn(),
    localText: vi.fn((key) => key)
}));

vi.mock("@serenity-is/sleekgrid", () => ({
    EventEmitter: vi.fn().mockImplementation(() => ({
        subscribe: vi.fn(),
        unsubscribe: vi.fn(),
        notify: vi.fn()
    })),
    EventData: vi.fn(),
    GroupItemMetadataProvider: vi.fn().mockImplementation(() => ({
        getGroupRowMetadata: vi.fn(),
        getTotalsRowMetadata: vi.fn()
    })),
    Group: vi.fn().mockImplementation(() => ({
        value: null,
        level: 0,
        count: 0,
        rows: [],
        groups: [],
        collapsed: false,
        totals: null
    })),
    gridDefaults: {}
}));

describe("RemoteView", () => {
    describe("constructor", () => {
        it("initializes with default options", () => {
            const view = new RemoteView({});

            expect(view).toBeDefined();
            expect(view.getIdPropertyName()).toBeDefined();
            expect(view.getItems()).toEqual([]);
            expect(view.getLength()).toBe(0);
        });

        it("initializes with custom idField", () => {
            const view = new RemoteView({
                idField: "customId"
            });

            expect(view.getIdPropertyName()).toBe("customId");
        });

        it("initializes with autoLoad false by default", () => {
            const view = new RemoteView({});

            // Should not auto-load by default
            expect(view.getItems().length).toBe(0);
        });

        it("auto-loads when url and autoLoad are set", () => {
            const mockServiceCall = vi.fn();
            vi.mocked(serviceCall).mockImplementation(mockServiceCall);

            const view = new RemoteView({
                url: "/api/test",
                autoLoad: true
            });

            // Should have attempted to load
            expect(mockServiceCall).toHaveBeenCalled();
        });

        it("sets up event emitters", () => {
            const view = new RemoteView({});

            expect(view.onDataChanged).toBeDefined();
            expect(view.onDataLoading).toBeDefined();
            expect(view.onDataLoaded).toBeDefined();
            expect(view.onPagingInfoChanged).toBeDefined();
            expect(view.onRowCountChanged).toBeDefined();
            expect(view.onRowsChanged).toBeDefined();
            expect(view.onRowsOrCountChanged).toBeDefined();
            expect(view.onGroupExpanded).toBeDefined();
            expect(view.onGroupCollapsed).toBeDefined();
        });

        it("initializes paging info", () => {
            const view = new RemoteView({});

            const pagingInfo = view.getPagingInfo();
            expect(pagingInfo).toBeDefined();
            expect(pagingInfo.rowsPerPage).toBeDefined();
            expect(pagingInfo.page).toBeDefined();
            expect(pagingInfo.totalCount).toBeDefined();
            expect(pagingInfo.loading).toBeDefined();
            expect(pagingInfo.error).toBeDefined();
            expect(pagingInfo.dataView).toBe(view);
        });
    });

    describe("basic data operations", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({
                idField: "id"
            });
        });

        it("adds items", () => {
            const item1 = { id: 1, name: "Item 1" };
            const item2 = { id: 2, name: "Item 2" };

            view.addItem(item1);
            view.addItem(item2);

            expect(view.getItems()).toEqual([item1, item2]);
            expect(view.getLength()).toBe(2);
        });

        it("gets item by id", () => {
            const item = { id: 1, name: "Item 1" };
            view.addItem(item);

            expect(view.getItemById(1)).toBe(item);
            expect(view.getItemById(999)).toBeUndefined();
        });

        it("updates items", () => {
            const item = { id: 1, name: "Item 1" };
            view.addItem(item);

            const updatedItem = { id: 1, name: "Updated Item 1" };
            view.updateItem(1, updatedItem);

            expect(view.getItemById(1)).toBe(updatedItem);
            expect(view.getItems()).toEqual([updatedItem]);
        });

        it("deletes items", () => {
            const item1 = { id: 1, name: "Item 1" };
            const item2 = { id: 2, name: "Item 2" };
            view.addItem(item1);
            view.addItem(item2);

            view.deleteItem(1);

            expect(view.getItems()).toEqual([item2]);
            expect(view.getLength()).toBe(1);
            expect(view.getItemById(1)).toBeUndefined();
        });

        it("gets item by index", () => {
            const item1 = { id: 1, name: "Item 1" };
            const item2 = { id: 2, name: "Item 2" };
            view.addItem(item1);
            view.addItem(item2);

            expect(view.getItemByIdx(0)).toBe(item1);
            expect(view.getItemByIdx(1)).toBe(item2);
            expect(view.getItemByIdx(2)).toBeUndefined();
        });

        it("gets index by id", () => {
            const item1 = { id: 1, name: "Item 1" };
            const item2 = { id: 2, name: "Item 2" };
            view.addItem(item1);
            view.addItem(item2);

            expect(view.getIdxById(1)).toBe(0);
            expect(view.getIdxById(2)).toBe(1);
            expect(view.getIdxById(999)).toBeUndefined();
        });
    });

    describe("id uniqueness validation", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({
                idField: "id"
            });
        });

        it("throws error when setItems is called with duplicate ids", () => {
            const items = [
                { id: 1, name: "Item 1" },
                { id: 2, name: "Item 2" },
                { id: 1, name: "Duplicate Item" }
            ];

            expect(() => {
                view.setItems(items);
            }).toThrow("Each data element must implement a unique 'id' property. Object at index '0' has repeated identity value '1'");
        });

        it("throws error when setItems is called with missing id", () => {
            const items = [
                { id: 1, name: "Item 1" },
                { name: "Item without id" },
                { id: 3, name: "Item 3" }
            ];

            expect(() => {
                view.setItems(items);
            }).toThrow("Each data element must implement a unique 'id' property. Object at index '1' has no identity value");
        });
    });

    describe("filtering", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({
                idField: "id"
            });
            view.addItem({ id: 1, name: "Apple", category: "fruit" });
            view.addItem({ id: 2, name: "Banana", category: "fruit" });
            view.addItem({ id: 3, name: "Carrot", category: "vegetable" });
        });

        it("sets and gets filter", () => {
            const filter = (item: any) => item.category === "fruit";
            view.setFilter(filter);

            expect(view.getFilter()).toBe(filter);
        });

        it("applies filter to get filtered items", () => {
            const filter = (item: any) => item.category === "fruit";
            view.setFilter(filter);

            const filteredItems = view.getFilteredItems();
            expect(filteredItems.length).toBe(2);
            expect(filteredItems[0].name).toBe("Apple");
            expect(filteredItems[1].name).toBe("Banana");
        });

        it("returns all items when no filter is set", () => {
            const filteredItems = view.getFilteredItems();
            expect(filteredItems.length).toBe(3);
        });
    });

    describe("sorting", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({
                idField: "id"
            });
            view.addItem({ id: 1, name: "Charlie" });
            view.addItem({ id: 2, name: "Alice" });
            view.addItem({ id: 3, name: "Bob" });
        });

        it("sorts items ascending by default", () => {
            view.sort((a, b) => a.name.localeCompare(b.name));

            const items = view.getItems();
            expect(items[0].name).toBe("Alice");
            expect(items[1].name).toBe("Bob");
            expect(items[2].name).toBe("Charlie");
        });

        it("sorts items descending when specified", () => {
            view.sort((a, b) => a.name.localeCompare(b.name), false);

            const items = view.getItems();
            expect(items[0].name).toBe("Charlie");
            expect(items[1].name).toBe("Bob");
            expect(items[2].name).toBe("Alice");
        });

        it("sets local sort property", () => {
            expect(view.getLocalSort()).toBe(false);

            view.setLocalSort(true);
            expect(view.getLocalSort()).toBe(true);
        });

        it("sorts by single field when localSort is true", () => {
            const sortView = new RemoteView<any>({
                idField: "id",
                localSort: true,
                sortBy: "name"
            });

            const items = [
                { id: 1, name: "Charlie" },
                { id: 2, name: "Alice" },
                { id: 3, name: "Bob" }
            ];

            sortView.setItems(items);

            const sortedItems = sortView.getItems() as any[];
            expect(sortedItems[0].name).toBe("Alice");
            expect(sortedItems[1].name).toBe("Bob");
            expect(sortedItems[2].name).toBe("Charlie");
        });

        it("sorts by multiple fields when localSort is true", () => {
            const sortView = new RemoteView<any>({
                idField: "id",
                localSort: true,
                sortBy: ["category", "name"]
            });

            const items = [
                { id: 1, name: "Charlie", category: "B" },
                { id: 2, name: "Alice", category: "A" },
                { id: 3, name: "Bob", category: "A" },
                { id: 4, name: "David", category: "B" }
            ];

            sortView.setItems(items);

            const sortedItems = sortView.getItems() as any[];
            expect(sortedItems[0].name).toBe("Alice"); // A category first
            expect(sortedItems[1].name).toBe("Bob");   // A category second
            expect(sortedItems[2].name).toBe("Charlie"); // B category first
            expect(sortedItems[3].name).toBe("David");   // B category second
        });

        it("sorts descending when field ends with ' desc'", () => {
            const sortView = new RemoteView<any>({
                idField: "id",
                localSort: true,
                sortBy: "name desc"
            });

            const items = [
                { id: 1, name: "Charlie" },
                { id: 2, name: "Alice" },
                { id: 3, name: "Bob" }
            ];

            sortView.setItems(items);

            const sortedItems = sortView.getItems() as any[];
            expect(sortedItems[0].name).toBe("Charlie");
            expect(sortedItems[1].name).toBe("Bob");
            expect(sortedItems[2].name).toBe("Alice");
        });

        it("sorts multiple fields with mixed ascending and descending", () => {
            const sortView = new RemoteView<any>({
                idField: "id",
                localSort: true,
                sortBy: ["category", "name desc"]
            });

            const items = [
                { id: 1, name: "Charlie", category: "B" },
                { id: 2, name: "Alice", category: "A" },
                { id: 3, name: "Bob", category: "A" },
                { id: 4, name: "David", category: "B" }
            ];

            sortView.setItems(items);

            const sortedItems = sortView.getItems() as any[];
            expect(sortedItems[0].name).toBe("Bob");     // A category, Bob > Alice descending
            expect(sortedItems[1].name).toBe("Alice");   // A category, Alice second
            expect(sortedItems[2].name).toBe("David");   // B category, David > Charlie descending
            expect(sortedItems[3].name).toBe("Charlie"); // B category, Charlie last
        });
    });

    describe("grouping", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({
                idField: "id"
            });
        });

        it("sets and gets grouping info", () => {
            const groupInfo = [{
                getter: (item: any) => item.category,
                formatter: (g: any) => g.value,
                collapsed: false
            }];

            view.setGrouping(groupInfo);
            expect(view.getGrouping()).toEqual(groupInfo);
        });

        it("sets grouping info with single object", () => {
            const groupInfo = {
                getter: (item: any) => item.category,
                formatter: (g: any) => g.value,
                collapsed: false
            };

            view.setGrouping(groupInfo);
            const result = view.getGrouping();
            expect(result).toHaveLength(1);
            expect(result[0].getter).toBe(groupInfo.getter);
            expect(result[0].formatter).toBe(groupInfo.formatter);
            expect(result[0].collapsed).toBe(false);
            // Should have defaults merged in
            expect(result[0]).toHaveProperty('aggregateEmpty', false);
        });

        it("gets groups", () => {
            view.addItem({ id: 1, name: "Apple", category: "fruit" });
            view.addItem({ id: 2, name: "Banana", category: "fruit" });
            view.addItem({ id: 3, name: "Carrot", category: "vegetable" });

            expect(view.getLength()).toBe(3);

            const groupInfo = [{
                getter: (item: any) => item.category,
                formatter: (g: any) => g.value,
                collapsed: false
            }];
            view.setGrouping(groupInfo);

            const groups = view.getGroups();
            expect(groups.length).toBe(2);
            expect(groups[0].value).toBe("fruit");
            expect(groups[0].count).toBe(2);
            expect(groups[1].value).toBe("vegetable");
            expect(groups[1].count).toBe(1);
        });
    });

    describe("pagination", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({
                idField: "id",
                rowsPerPage: 2
            });
        });

        it("gets paging info with correct defaults", () => {
            const pagingInfo = view.getPagingInfo();
            expect(pagingInfo.rowsPerPage).toBe(2);
            expect(pagingInfo.page).toBe(1); // Page is 1-based, starts at 1
            expect(pagingInfo.totalCount).toBeDefined(); // Could be 0 or null depending on implementation
            expect(pagingInfo.loading).toBe(false);
            expect(pagingInfo.error).toBe(null);
        });
    });

    describe("update operations", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({
                idField: "id"
            });
        });

        it("begins and ends update", () => {
            // These methods should not throw errors
            expect(() => {
                view.beginUpdate();
                view.endUpdate();
            }).not.toThrow();
        });

        it("sets items directly", () => {
            const items = [
                { id: 1, name: "Item 1" },
                { id: 2, name: "Item 2" }
            ];

            view.setItems(items);

            expect(view.getItems()).toEqual(items);
            expect(view.getLength()).toBe(2);
        });

        it("inserts item at specific position", () => {
            view.addItem({ id: 1, name: "Item 1" });
            view.addItem({ id: 3, name: "Item 3" });

            view.insertItem(1, { id: 2, name: "Item 2" });

            const items = view.getItems();
            expect(items.length).toBe(3);
            expect(items[1].name).toBe("Item 2");
        });
    });

    describe("row mapping operations", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({
                idField: "id"
            });
            view.addItem({ id: 1, name: "Item 1" });
            view.addItem({ id: 2, name: "Item 2" });
            view.addItem({ id: 3, name: "Item 3" });
        });

        it("maps items to rows", () => {
            const items = view.getItems();
            const rows = view.mapItemsToRows([items[0], items[2]]);

            expect(rows).toEqual([0, 2]);
        });

        it("maps rows to ids", () => {
            const ids = view.mapRowsToIds([0, 2]);

            expect(ids).toEqual([1, 3]);
        });

        it("maps ids to rows", () => {
            const rows = view.mapIdsToRows([1, 3]);

            expect(rows).toEqual([0, 2]);
        });

        it("gets row by id", () => {
            expect(view.getRowById(1)).toBe(0);
            expect(view.getRowById(2)).toBe(1);
            expect(view.getRowById(999)).toBeUndefined();
        });

        it("gets row by item", () => {
            const item = view.getItems()[1];
            expect(view.getRowByItem(item)).toBe(1);
        });
    });

    describe("sorted operations", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({
                idField: "id"
            });
            view.addItem({ id: 1, name: "Charlie" });
            view.addItem({ id: 2, name: "Alice" });
            view.addItem({ id: 3, name: "Bob" });
        });

        it("adds item in sorted order", () => {
            view.sort((a, b) => a.name.localeCompare(b.name));

            view.sortedAddItem({ id: 4, name: "Aaron" });

            const items = view.getItems();
            expect(items[0].name).toBe("Aaron");
            expect(items[1].name).toBe("Alice");
            expect(items[2].name).toBe("Bob");
            expect(items[3].name).toBe("Charlie");
        });

        it("updates item in sorted order", () => {
            view.sort((a, b) => a.name.localeCompare(b.name));

            // Update Charlie to Aaron - should move to beginning
            view.sortedUpdateItem(1, { id: 1, name: "Aaron" });

            const items = view.getItems();
            expect(items[0].name).toBe("Aaron");
            expect(items[1].name).toBe("Alice");
            expect(items[2].name).toBe("Bob");
        });

        it("re-sorts items", () => {
            view.sort((a, b) => a.name.localeCompare(b.name));

            // Change an item without using sortedUpdate
            view.updateItem(2, { id: 2, name: "Zoe" });

            // Re-sort should fix the order
            view.reSort();

            const items = view.getItems();
            expect(items[0].name).toBe("Bob");
            expect(items[1].name).toBe("Charlie");
            expect(items[2].name).toBe("Zoe");
        });
    });

    describe("group expansion and collapse", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({
                idField: "id"
            });
            view.addItem({ id: 1, name: "Apple", category: "fruit" });
            view.addItem({ id: 2, name: "Banana", category: "fruit" });
            view.addItem({ id: 3, name: "Carrot", category: "vegetable" });

            const groupInfo = [{
                getter: (item: any) => item.category,
                formatter: (g: any) => g.value,
                collapsed: false
            }];
            view.setGrouping(groupInfo);
        });

        it("expands all groups", () => {
            // First collapse all
            view.collapseAllGroups(undefined);

            // Then expand all
            view.expandAllGroups(undefined);

            // Should not throw and groups should be expanded
            expect(view.getGroups().length).toBe(2);
        });

        it("collapses all groups", () => {
            view.collapseAllGroups(undefined);

            // Should not throw
            expect(view.getGroups().length).toBe(2);
        });

        it("expands all groups at specific level", () => {
            view.collapseAllGroups(0); // Collapse level 0
            view.expandAllGroups(0);   // Expand level 0

            // Should not throw
            expect(view.getGroups().length).toBe(2);
        });

        it("collapses all groups at specific level", () => {
            view.collapseAllGroups(0);

            // Should not throw
            expect(view.getGroups().length).toBe(2);
        });

        it("expands specific group", () => {
            view.collapseAllGroups(undefined);
            view.expandGroup(["fruit"]);

            // Should not throw
            expect(view.getGroups().length).toBe(2);
        });

        it("collapses specific group", () => {
            view.collapseGroup(["fruit"]);

            // Should not throw
            expect(view.getGroups().length).toBe(2);
        });
    });

    describe("summary and aggregation", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({
                idField: "id"
            });
            view.addItem({ id: 1, value: 10 });
            view.addItem({ id: 2, value: 20 });
            view.addItem({ id: 3, value: 30 });
        });

        it("sets summary options", () => {
            const aggregators = [
                new Aggregators.Sum("value")
            ];

            view.setSummaryOptions({
                aggregators: aggregators
            });

            // Should not throw
            expect(() => view.getGrandTotals()).not.toThrow();
        });

        it("gets grand totals", () => {
            const aggregators = [
                new Aggregators.Sum("value")
            ];

            view.setSummaryOptions({
                aggregators: aggregators
            });

            const totals = view.getGrandTotals();
            expect(totals).toBeDefined();
            expect(totals.sum).toBeDefined();
            expect(totals.sum.value).toBe(60);
        });
    });

    describe("data loading operations", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({
                idField: "id",
                url: "/api/test"
            });
        });

        it("adds data", () => {
            const data = {
                Entities: [
                    { id: 1, name: "Item 1" },
                    { id: 2, name: "Item 2" }
                ],
                TotalCount: 2
            };

            view.addData(data);

            expect(view.getItems().length).toBe(2);
            expect(view.getItems()[0].name).toBe("Item 1");
        });

        it("handles null data", () => {
            const result = view.addData(null);
            expect(result).toBe(false);
            expect(view['errorMessage']).toBeDefined();
            expect(view.getPagingInfo().error).toBeDefined();
        });

        it("handles undefined data", () => {
            const result = view.addData(undefined);
            expect(result).toBe(false);
            expect(view['errorMessage']).toBeDefined();
        });

        it("processes data with onProcessData callback", () => {
            const originalData = {
                Entities: [{ id: 1, name: "Item 1" }],
                TotalCount: 1
            };

            const processedData = {
                Entities: [{ id: 1, name: "Processed Item 1" }],
                TotalCount: 1
            };

            view.onProcessData = vi.fn(() => processedData);

            view.addData(originalData);

            expect(view.onProcessData).toHaveBeenCalledWith(originalData, view);
            expect(view.getItems()[0].name).toBe("Processed Item 1");
        });

        it("aborts previous loading request", () => {
            const mockServiceCall = vi.fn();
            vi.mocked(serviceCall).mockImplementation(mockServiceCall);

            // First populate call
            view.populate();
            const firstController = view['loading'] as AbortController;
            expect(firstController).toBeInstanceOf(AbortController);
            expect(firstController.signal.aborted).toBe(false);

            // Second populate call should abort the first
            view.populate();
            expect(firstController.signal.aborted).toBe(true);
            expect(mockServiceCall).toHaveBeenCalledTimes(2);
        });

        it("locks and unlocks populate", () => {
            const mockServiceCall = vi.fn();
            vi.mocked(serviceCall).mockImplementation(mockServiceCall);

            view.populateLock();
            expect(view['populateLocks']).toBe(1);
            expect(view['populateCalls']).toBe(0);

            // Call populate while locked - should increment populateCalls and not call service
            view.populate();
            expect(view['populateCalls']).toBe(1);
            expect(mockServiceCall).not.toHaveBeenCalled();

            view.populateUnlock();
            expect(view['populateLocks']).toBe(0);
            // Now populate should be called
            view.populate();
            expect(mockServiceCall).toHaveBeenCalled();
        });

        it("populates data", () => {
            // Mock the service call
            const mockServiceCall = vi.fn();
            vi.mocked(serviceCall).mockImplementation(mockServiceCall);

            view.populate();

            // Should attempt to call service
            expect(mockServiceCall).toHaveBeenCalled();
        });

        it("handles onSubmit callback returning false", () => {
            const mockServiceCall = vi.fn();
            vi.mocked(serviceCall).mockImplementation(mockServiceCall);

            view.onSubmit = vi.fn(() => false);
            const result = view.populate();

            expect(result).toBe(false);
            expect(mockServiceCall).not.toHaveBeenCalled();
            expect(view.onSubmit).toHaveBeenCalledWith(view);
        });

        it("handles onSubmit callback returning true", () => {
            const mockServiceCall = vi.fn();
            vi.mocked(serviceCall).mockImplementation(mockServiceCall);

            view.onSubmit = vi.fn(() => true);
            view.populate();

            expect(mockServiceCall).toHaveBeenCalled();
            expect(view.onSubmit).toHaveBeenCalledWith(view);
        });

        it("returns false when no URL is set", () => {
            const viewWithoutUrl = new RemoteView({
                idField: "id"
                // no url
            });

            const result = viewWithoutUrl.populate();
            expect(result).toBe(false);
        });

        it("handles onAjaxCall callback returning false", () => {
            const mockServiceCall = vi.fn();
            vi.mocked(serviceCall).mockImplementation(mockServiceCall);

            view.onAjaxCall = vi.fn(() => false);
            const result = view.populate();

            expect(result).toBe(false);
            expect(mockServiceCall).not.toHaveBeenCalled();
            expect(view.onAjaxCall).toHaveBeenCalled();
            expect(view['loading']).toBe(false);
        });

        it("handles onAjaxCall callback returning true", () => {
            const mockServiceCall = vi.fn();
            vi.mocked(serviceCall).mockImplementation(mockServiceCall);

            view.onAjaxCall = vi.fn(() => true);
            view.populate();

            expect(mockServiceCall).toHaveBeenCalled();
            expect(view.onAjaxCall).toHaveBeenCalled();
        });
    });

    describe("item metadata", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({
                idField: "id",
                getItemMetadata: (item: any, row: number) => ({
                    cssClasses: `row-${row}`,
                    focusable: true
                })
            });
        });

        it("gets item metadata", () => {
            view.addItem({ id: 1, name: "Item 1" });

            const metadata = view.getItemMetadata(0);
            expect(metadata).toBeDefined();
            expect(metadata.cssClasses).toBe("row-0");
            expect(metadata.focusable).toBe(true);
        });

        it("returns null for invalid row", () => {
            const metadata = view.getItemMetadata(999);
            expect(metadata).toBeNull();
        });
    });

    describe("grid synchronization", () => {
        let view: RemoteView<any>;
        let mockGrid: any;

        beforeEach(() => {
            view = new RemoteView({
                idField: "id"
            });
            view.addItem({ id: 1, name: "Item 1" });
            view.addItem({ id: 2, name: "Item 2" });

            mockGrid = {
                getSelectedRows: vi.fn(() => [0, 1]),
                setSelectedRows: vi.fn(),
                onSelectedRowsChanged: {
                    subscribe: vi.fn(),
                    unsubscribe: vi.fn()
                }
            };
        });

        it("syncs grid selection", () => {
            const event = view.syncGridSelection(mockGrid, false, false);

            expect(event).toBeDefined();
            expect(mockGrid.onSelectedRowsChanged.subscribe).toHaveBeenCalled();
        });

        it("syncs grid selection with preserve hidden", () => {
            view.addItem({ id: 1, name: "Item 1" });
            view.addItem({ id: 2, name: "Item 2" });
            view.addItem({ id: 3, name: "Item 3" });

            // Mock grid to return selected rows [0, 2] initially
            mockGrid.getSelectedRows = vi.fn(() => [0, 2]);
            mockGrid.setSelectedRows = vi.fn();

            view.syncGridSelection(mockGrid, true, false);

            // Should subscribe to grid selection changes
            expect(mockGrid.onSelectedRowsChanged.subscribe).toHaveBeenCalled();

            // Should subscribe to view row changes
            expect(view.onRowsChanged.subscribe).toHaveBeenCalled();
            expect(view.onRowCountChanged.subscribe).toHaveBeenCalled();

            // Test that grid selection changes trigger the callback
            mockGrid.getSelectedRows = vi.fn(() => [1]); // Now only row 1 is selected
            const gridSelectionCallback = mockGrid.onSelectedRowsChanged.subscribe.mock.calls[0][0];
            gridSelectionCallback({}, {});

            // The callback should execute without error
            expect(gridSelectionCallback).toBeDefined();
        });

        it("syncs grid cell CSS styles", () => {
            mockGrid.getCellCssStyles = vi.fn(() => ({}));
            mockGrid.registerPlugin = vi.fn();
            mockGrid.onCellCssStylesChanged = { subscribe: vi.fn() };

            view.syncGridCellCssStyles(mockGrid, "test-key");

            // Should not throw
            expect(() => view.syncGridCellCssStyles(mockGrid, "test-key")).not.toThrow();
        });
    });

    describe("group item metadata provider", () => {
        let view: RemoteView<any>;
        let mockProvider: any;

        beforeEach(() => {
            mockProvider = {
                getGroupRowMetadata: vi.fn(),
                getTotalsRowMetadata: vi.fn()
            };

            view = new RemoteView({
                idField: "id",
                groupItemMetadataProvider: mockProvider
            });
        });

        it("gets and sets group item metadata provider", () => {
            expect(view.getGroupItemMetadataProvider()).toBe(mockProvider);

            const newProvider = { ...mockProvider };
            view.setGroupItemMetadataProvider(newProvider);
            expect(view.getGroupItemMetadataProvider()).toBe(newProvider);
        });
    });

    describe("error handling", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({
                idField: "id"
            });
        });

        it("handles delete of non-existent item gracefully", () => {
            expect(() => view.deleteItem(999)).toThrow("Invalid id");
        });

        it("handles update of non-existent item gracefully", () => {
            expect(() => view.updateItem(999, { id: 999, name: "Test" })).toThrow("Invalid id");
        });

        it("throws error when updating item to null id", () => {
            view.addItem({ id: 1, name: "Item 1" });
            expect(() => view.updateItem(1, { id: null, name: "Updated Item" })).toThrow("Cannot update item to associate with a null id");
        });

        it("throws error when updating item to non-unique id", () => {
            view.addItem({ id: 1, name: "Item 1" });
            view.addItem({ id: 2, name: "Item 2" });
            expect(() => view.updateItem(1, { id: 2, name: "Updated Item" })).toThrow("Cannot update item to associate with a non-unique id");
        });

        it("handles invalid filter gracefully", () => {
            const invalidFilter = null;
            expect(() => view.setFilter(invalidFilter)).not.toThrow();
        });
    });

    describe("paging options", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({
                idField: "id",
                rowsPerPage: 10
            });
        });

        it("sets paging options", () => {
            view.setPagingOptions({
                rowsPerPage: 20,
                page: 2
            });

            const pagingInfo = view.getPagingInfo();
            expect(pagingInfo.rowsPerPage).toBe(20);
            expect(view.seekToPage).toBe(2); // page changes seekToPage, not immediate page
        });

        it("handles page changes", () => {
            view.setPagingOptions({ page: 3 });

            expect(view.seekToPage).toBe(3); // page changes seekToPage, not immediate page
        });

        it("handles page changes with zero rowsPerPage", () => {
            const viewNoPaging = new RemoteView({
                idField: "id",
                rowsPerPage: 0
            });

            viewNoPaging.setPagingOptions({ page: 5 });
            expect(viewNoPaging.seekToPage).toBe(1); // Should set to 1 when rowsPerPage is 0
        });

        it("handles page changes with totalCount", () => {
            view['totalCount'] = 100; // Set total count
            view.setPagingOptions({ page: 20 }); // Try to go beyond max pages

            // Max pages = ceil(100/10) + 1 = 11, so should be clamped to 11
            expect(view.seekToPage).toBe(11);
        });

        it("handles negative page numbers", () => {
            view.setPagingOptions({ page: -1 });
            expect(view.seekToPage).toBe(1); // Should clamp to 1
        });
    });

    describe("refresh hints", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({
                idField: "id"
            });
        });

        it("sets refresh hints", () => {
            const hints = [{ ignoreDiffsBefore: 5, ignoreDiffsAfter: 10 }];
            view.setRefreshHints(hints);

            // Should not throw - hints are internal
            expect(() => view.setRefreshHints(hints)).not.toThrow();
        });
    });
});