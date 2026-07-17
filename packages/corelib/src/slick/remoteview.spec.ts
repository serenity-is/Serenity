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
    EventEmitter: vi.fn().mockImplementation(function() {
        const handlers: any[] = [];
        this.subscribe = vi.fn((handler: any) => {
            handlers.push(handler);
        });
        this.unsubscribe = vi.fn();
        this.notify = vi.fn(function(...args: any[]) {
            for (const handler of handlers) {
                handler(...args);
            }
        });
    }),
    EventDataWrapper: vi.fn(),
    GroupItemMetadataProvider: vi.fn().mockImplementation(function() {
        this.getGroupRowMetadata = vi.fn(),
        this.getTotalsRowMetadata = vi.fn()
    }),
    Group: vi.fn().mockImplementation(function() {
        this.__group = true;
        this.__nonDataRow = true;
        this.value = null;
        this.level = 0;
        this.count = 0;
        this.rows = [];
        this.groups = [];
        this.collapsed = false;
        this.totals = null;
        this.equals = vi.fn(function(other: any) {
            return this.value === other.value && this.level === other.level;
        });
    }),
    GroupTotals: vi.fn().mockImplementation(function() {
        this.__groupTotals = true;
        this.__nonDataRow = true;
        this.initialized = false;
        this.group = null;
        this.sum = {};
        this.avg = {};
        this.min = {};
        this.max = {};
        this.count = 0;
    }),
    gridDefaults: {},
    convertCompatFormatter: vi.fn((compatFn) => (ctx) => compatFn(0, 0, null, null, ctx.item))
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
            expect(result[0].aggregateEmpty ?? false).toBe(false);
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

        it("calculates totals lazily when accessing totals row", () => {
            view.addItem({ id: 1, name: "Apple", category: "fruit", price: 1 });
            view.addItem({ id: 2, name: "Banana", category: "fruit", price: 2 });
            view.addItem({ id: 3, name: "Carrot", category: "vegetable", price: 3 });

            const groupInfo = [{
                getter: (item: any) => item.category,
                formatter: (g: any) => g.value,
                collapsed: false,
                lazyTotalsCalculation: true,
                displayTotalsRow: true,
                aggregators: [new Aggregators.Sum('price')]
            }];
            view.setGrouping(groupInfo);

            // Get the rows - this should include totals rows
            const rows = view.getRows();
            expect(rows.length).toBeGreaterThan(3); // groups + data + totals

            // Find a totals row
            const totalsRow = rows.find(row => row.__groupTotals);
            expect(totalsRow).toBeDefined();
            expect(totalsRow.initialized).toBe(false);

            // Accessing the row should calculate totals
            const accessedRow = view.getItem(rows.indexOf(totalsRow));
            expect(accessedRow).toBe(totalsRow);
            expect(accessedRow.initialized).toBe(true);
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

        it("sets items in suspend mode", () => {
            const items = [
                { id: 1, name: "Item 1" },
                { id: 2, name: "Item 2" }
            ];

            vi.spyOn(view, 'refresh').mockImplementation(() => { });
            view.beginUpdate();
            view.setItems(items);
            // In suspend mode, items and rows should be set
            expect(view.getItems()).toEqual(items);
            expect(view.getLength()).toBe(2);
            expect(view.refresh).not.toHaveBeenCalled();
            view.endUpdate();
            // After endUpdate, should be refreshed again
            expect(view.getItems()).toEqual(items);
            expect(view.getLength()).toBe(2);
            expect(view.refresh).toHaveBeenCalled();
        });

        it("inserts item at specific position", () => {
            view.addItem({ id: 1, name: "Item 1" });
            view.addItem({ id: 3, name: "Item 3" });

            view.insertItem(1, { id: 2, name: "Item 2" });

            const items = view.getItems();
            expect(items.length).toBe(3);
            expect(items[1].name).toBe("Item 2");
        });

        it("updates item with id change", () => {
            view.addItem({ id: 1, name: "Item 1" });
            view.addItem({ id: 2, name: "Item 2" });

            // Update item 1 to have id 3
            view.updateItem(1, { id: 3, name: "Updated Item 1" });

            expect(view.getItemById(1)).toBeUndefined();
            expect(view.getItemById(3)).toEqual({ id: 3, name: "Updated Item 1" });
            expect(view.getItems()).toEqual([
                { id: 3, name: "Updated Item 1" },
                { id: 2, name: "Item 2" }
            ]);
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

        it("adds item at the end in sorted order", () => {
            view.sort((a, b) => a.name.localeCompare(b.name));

            view.sortedAddItem({ id: 4, name: "Zoe" });

            const items = view.getItems();
            expect(items[0].name).toBe("Alice");
            expect(items[1].name).toBe("Bob");
            expect(items[2].name).toBe("Charlie");
            expect(items[3].name).toBe("Zoe");
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

        it("throws error for invalid id in sortedUpdateItem", () => {
            view.sort((a, b) => a.name.localeCompare(b.name));

            expect(() => view.sortedUpdateItem(999, { id: 999, name: "Test" })).toThrow("Invalid or non-matching id");
        });

        it("throws error for non-matching id in sortedUpdateItem", () => {
            view.sort((a, b) => a.name.localeCompare(b.name));

            expect(() => view.sortedUpdateItem(1, { id: 2, name: "Test" })).toThrow("Invalid or non-matching id");
        });

        it("updates item without affecting sort order", () => {
            view.sort((a, b) => a.name.localeCompare(b.name));

            // Update Charlie to Charlie (same name) - should not affect sorting
            view.sortedUpdateItem(1, { id: 1, name: "Charlie", updated: true });

            const items = view.getItems();
            expect(items[0].name).toBe("Alice");
            expect(items[1].name).toBe("Bob");
            expect(items[2].name).toBe("Charlie"); // Same name
            expect(items[2].updated).toBe(true); // Check that other properties were updated
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
            view.collapseAllGroups();

            // Then expand all
            view.expandAllGroups();

            // Should not throw and groups should be expanded
            expect(view.getGroups().length).toBe(2);
        });

        it("collapses all groups", () => {
            view.collapseAllGroups();

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
            mockGrid.setCellCssStyles = vi.fn();

            let onCellCssStylesChangedCallback: any;
            mockGrid.onCellCssStylesChanged = {
                subscribe: vi.fn((callback) => onCellCssStylesChangedCallback = callback)
            };

            const onRowsOrCountChangedSpy = vi.spyOn(view.onRowsOrCountChanged, 'subscribe');

            view.syncGridCellCssStyles(mockGrid, "test-key");

            // Should have subscribed to events
            expect(mockGrid.onCellCssStylesChanged.subscribe).toHaveBeenCalled();
            expect(onRowsOrCountChangedSpy).toHaveBeenCalled();

            // Get the callback that was subscribed to onRowsOrCountChanged
            const onRowsOrCountChangedCallback = onRowsOrCountChangedSpy.mock.calls[0][0];

            // Trigger the onCellCssStylesChanged event to test the subFunc
            onCellCssStylesChangedCallback(null, { key: "test-key", hash: { 0: "new-style1", 1: "new-style2" } });

            // Trigger the onRowsOrCountChanged event to test the update function
            onRowsOrCountChangedCallback(null, {
                rowsDiff: [], previousRowCount: 2, currentRowCount: 2,
                rowCountChanged: false, rowsChanged: false, dataView: view
            });

            // Should have called setCellCssStyles with updated styles
            expect(mockGrid.setCellCssStyles).toHaveBeenCalledWith("test-key", expect.any(Object));
        });

        it("triggers update when row count changes after syncGridSelection", () => {
            mockGrid.getSelectedRows = vi.fn(() => [0, 1]);
            mockGrid.setSelectedRows = vi.fn();

            view.syncGridSelection(mockGrid, false, false);

            // Add an item which triggers onRowCountChanged
            view.addItem({ id: 3, name: "Item 3" });

            // The update function should have called setSelectedRows on the grid
            expect(mockGrid.setSelectedRows).toHaveBeenCalled();
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

    describe("formatGroupValue", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({ idField: "id" });
        });

        it("returns escaped value when ctx.item is null", () => {
            const result = (view as any)['formatGroupValue']({ item: null, escape: (v: any) => String(v) });
            // ctx.item?.value -> undefined -> String(undefined) -> "undefined"
            expect(result).toBe("undefined");
        });

        it("returns escaped value when ctx.item.level is null", () => {
            const result = (view as any)['formatGroupValue']({ item: { value: "test-val", level: null }, escape: (v: any) => String(v) });
            expect(result).toBe("test-val");
        });

        it("returns escaped value when groupingInfo is missing for level", () => {
            const result = (view as any)['formatGroupValue']({
                item: { value: "test-val", level: 0 },
                escape: (v: any) => String(v)
            });
            expect(result).toBe("test-val");
        });

        it("converts formatter and uses it when format is not set", () => {
            const viewWithGrouping = new RemoteView({ idField: "id" });
            const formatter = vi.fn(() => "formatted-value");
            viewWithGrouping.setGrouping([{
                getter: (item: any) => item.category,
                formatter: formatter,
                collapsed: false
            }]);

            (viewWithGrouping as any)['formatGroupValue']({
                item: { value: "test", level: 0 },
                escape: (v: any) => String(v)
            });

            expect(formatter).toHaveBeenCalledWith(expect.any(Object));
        });

        it("uses format when already set on groupingInfo", () => {
            const viewWithGrouping = new RemoteView({ idField: "id" });
            const formatFn = vi.fn(() => "already-formatted");
            viewWithGrouping.setGrouping([{
                getter: (item: any) => item.category,
                format: formatFn,
                collapsed: false
            }]);

            const result = (viewWithGrouping as any)['formatGroupValue']({
                item: { value: "test", level: 0 },
                escape: (v: any) => String(v)
            });

            expect(formatFn).toHaveBeenCalled();
            expect(result).toBe("already-formatted");
        });

        it("falls back to escaped value when no format or formatter", () => {
            const viewWithGrouping = new RemoteView({ idField: "id" });
            viewWithGrouping.setGrouping([{
                getter: (item: any) => item.category,
                collapsed: false
            }]);

            const result = (viewWithGrouping as any)['formatGroupValue']({
                item: { value: "fallback-val", level: 0 },
                escape: (v: any) => String(v)
            });

            expect(result).toBe("fallback-val");
        });

        it("handles item with value undefined", () => {
            const result = (view as any)['formatGroupValue']({
                item: { value: undefined, level: 0 },
                escape: (v: any) => String(v)
            });
            expect(result).toBe("undefined");
        });
    });

    describe("getItem with grouping", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({ idField: "id" });
        });

        it("calculates group totals when displayTotalsRow is false", () => {
            view.addItem({ id: 1, name: "Apple", category: "fruit", price: 10 });
            view.addItem({ id: 2, name: "Banana", category: "fruit", price: 20 });
            view.addItem({ id: 3, name: "Carrot", category: "vegetable", price: 30 });

            view.setGrouping([{
                getter: (item: any) => item.category,
                formatter: (g: any) => g.value,
                collapsed: false,
                displayTotalsRow: false,
                lazyTotalsCalculation: true,
                aggregators: [new Aggregators.Sum('price')]
            }]);

            const rows = view.getRows();
            // Find a group row
            const groupRow = rows.find(r => r.__group);
            expect(groupRow).toBeDefined();
            expect(groupRow.totals).toBeDefined();
            expect(groupRow.totals.initialized).toBe(false);

            // Access the group row to trigger totals calculation
            const groupRowIdx = rows.indexOf(groupRow);
            const accessed = view.getItem(groupRowIdx);
            expect(accessed.totals.initialized).toBe(true);
        });
    });

    describe("getItemMetadata with groups", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({
                idField: "id",
                groupItemMetadataProvider: {
                    getGroupRowMetadata: vi.fn(() => ({ cssClasses: "group-row" })),
                    getTotalsRowMetadata: vi.fn(() => ({ cssClasses: "totals-row" }))
                } as any
            });
        });

        it("returns metadata for group rows", () => {
            view.addItem({ id: 1, name: "Apple", category: "fruit" });
            view.addItem({ id: 2, name: "Banana", category: "fruit" });
            view.addItem({ id: 3, name: "Carrot", category: "vegetable" });

            view.setGrouping([{
                getter: (item: any) => item.category,
                formatter: (g: any) => g.value,
                collapsed: false
            }]);

            const rows = view.getRows();
            const groupRowIdx = rows.findIndex(r => r.__group);
            expect(groupRowIdx).not.toBe(-1);

            const metadata = view.getItemMetadata(groupRowIdx);
            expect(metadata).toEqual({ cssClasses: "group-row" });
        });

        it("returns metadata for totals rows", () => {
            view.addItem({ id: 1, name: "Apple", category: "fruit", price: 10 });
            view.addItem({ id: 2, name: "Banana", category: "fruit", price: 20 });
            view.addItem({ id: 3, name: "Carrot", category: "vegetable", price: 30 });

            view.setGrouping([{
                getter: (item: any) => item.category,
                formatter: (g: any) => g.value,
                collapsed: false,
                aggregators: [new Aggregators.Sum('price')]
            }]);

            const rows = view.getRows();
            const totalsRowIdx = rows.findIndex(r => r.__groupTotals);
            expect(totalsRowIdx).not.toBe(-1);

            const metadata = view.getItemMetadata(totalsRowIdx);
            expect(metadata).toEqual({ cssClasses: "totals-row" });
        });

        it("returns itemMetadataCallback result for regular rows", () => {
            const itemMetadataCallback = vi.fn((item: any, row: number) => ({
                cssClasses: `row-${row}`,
                selectable: true
            }));
            const viewWithCallback = new RemoteView({
                idField: "id",
                getItemMetadata: itemMetadataCallback
            });

            viewWithCallback.addItem({ id: 1, name: "Item 1" });
            const metadata = viewWithCallback.getItemMetadata(0);

            expect(itemMetadataCallback).toHaveBeenCalledWith({ id: 1, name: "Item 1" }, 0);
            expect(metadata).toEqual({ cssClasses: "row-0", selectable: true });
        });
    });

    describe("setItems coverage", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({ idField: "id" });
        });

        it("calls recalc when in suspend mode during setItems", () => {
            const items = [
                { id: 1, name: "A" },
                { id: 2, name: "B" }
            ];

            view.beginUpdate();
            expect(() => view.setItems(items)).not.toThrow();
            expect(view.getItems()).toEqual(items);
            view.endUpdate();
        });

        it("handles localSort during setItems", () => {
            const sortView = new RemoteView<any>({
                idField: "id",
                localSort: true,
                sortBy: "name"
            });

            const items = [
                { id: 1, name: "Z" },
                { id: 2, name: "A" },
                { id: 3, name: "M" }
            ];

            sortView.setItems(items);
            expect(sortView.getItems()[0].name).toBe("A");
            expect(sortView.getItems()[1].name).toBe("M");
            expect(sortView.getItems()[2].name).toBe("Z");
        });
    });

    describe("getSortComparer edge cases", () => {
        it("handles null entries in sortBy array", () => {
            const view = new RemoteView<any>({
                idField: "id",
                localSort: true,
                sortBy: ["name", null as any, "age"]
            });

            const items = [
                { id: 1, name: "C", age: 30 },
                { id: 2, name: "A", age: 20 },
                { id: 3, name: "B", age: 10 }
            ];

            view.setItems(items);
            expect(view.getItems()[0].name).toBe("A");
            expect(view.getItems()[1].name).toBe("B");
            expect(view.getItems()[2].name).toBe("C");
        });
    });

    describe("updateItem edge cases", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({ idField: "id" });
        });

        it("clears updated cache when id changes and old id was updated", () => {
            view.addItem({ id: 1, name: "A" });
            view.addItem({ id: 2, name: "B" });

            // First update to populate the updated cache
            view.updateItem(1, { id: 1, name: "A-modified" });

            // Now update with id change - should clear old id from updated cache
            expect(() => view.updateItem(1, { id: 3, name: "A-new-id" })).not.toThrow();
            expect(view.getItemById(1)).toBeUndefined();
            expect(view.getItemById(3)).toBeDefined();
            expect(view.getItemById(3).name).toBe("A-new-id");
        });
    });

    describe("addData coverage", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({
                idField: "id",
                url: "/api/test"
            });
        });

        it("calculates page from Skip and Take when both present", () => {
            const data = {
                Entities: [
                    { id: 1, name: "Item 1" },
                    { id: 2, name: "Item 2" }
                ],
                TotalCount: 50,
                Skip: 10,
                Take: 10
            };

            view.addData(data);
            // Page = ceil(10 / 10) + 1 = 2
            expect(view.getPagingInfo().page).toBe(2);
        });

        it("sets page to 1 when Skip is 0", () => {
            const data = {
                Entities: [
                    { id: 1, name: "Item 1" }
                ],
                TotalCount: 10,
                Skip: 0,
                Take: 10
            };

            view.addData(data);
            expect(view.getPagingInfo().page).toBe(1);
        });

        it("sets page to 1 when rowsPerPage is 0 and no Take", () => {
            const viewNoPaging = new RemoteView<any>({
                idField: "id",
                rowsPerPage: 0,
                url: "/api/test"
            });

            const data = {
                Entities: [
                    { id: 1, name: "Item 1" }
                ],
                TotalCount: 10,
                Skip: 10,
                Take: 0
            };

            viewNoPaging.addData(data);
            expect(viewNoPaging.getPagingInfo().page).toBe(1);
        });

        it("aborts previous loading and sets loading state", () => {
            const mockServiceCall = vi.fn();
            vi.mocked(serviceCall).mockImplementation(mockServiceCall);

            view.populate();
            const controller = view['loading'] as AbortController;
            expect(controller).toBeInstanceOf(AbortController);
            expect(controller.signal.aborted).toBe(false);

            // Populate again to abort the first
            view.populate();
            expect(controller.signal.aborted).toBe(true);
        });

        it("handles onProcessData returning falsy value", () => {
            view.onProcessData = vi.fn(() => undefined);

            const data = {
                Entities: [{ id: 1, name: "Item 1" }],
                TotalCount: 1
            };

            // When onProcessData returns undefined/null, the original data is used (undefined || data = data)
            const result = view.addData(data);
            expect(result).toBeUndefined(); // addData doesn't return anything in this path
            expect(view.getItems().length).toBe(1);
        });
    });

    describe("populate edge cases", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({
                idField: "id",
                url: "/api/test"
            });
        });

        it("handles sortBy as string in populate", () => {
            const viewWithStringSort = new RemoteView<any>({
                idField: "id",
                url: "/api/test",
                sortBy: "name"
            });

            const mockServiceCall = vi.fn();
            vi.mocked(serviceCall).mockImplementation(mockServiceCall);

            viewWithStringSort.populate();
            expect(mockServiceCall).toHaveBeenCalled();
            const callOptions = mockServiceCall.mock.calls[0][0];
            expect(callOptions.request.Sort).toEqual(["name"]);
        });

        it("handles seekToPage being 0", () => {
            const viewNoSeek = new RemoteView<any>({
                idField: "id",
                url: "/api/test",
                seekToPage: 0
            });

            const mockServiceCall = vi.fn();
            vi.mocked(serviceCall).mockImplementation(mockServiceCall);

            viewNoSeek.populate();
            expect(mockServiceCall).toHaveBeenCalled();
            // seekToPage should be set to 1
            expect(viewNoSeek.seekToPage).toBe(1);
        });

        it("includes Skip in request when seekToPage > 1", () => {
            const viewWithPage = new RemoteView<any>({
                idField: "id",
                url: "/api/test",
                rowsPerPage: 10,
                seekToPage: 3
            });

            const mockServiceCall = vi.fn();
            vi.mocked(serviceCall).mockImplementation(mockServiceCall);

            viewWithPage.populate();
            expect(mockServiceCall).toHaveBeenCalled();
            const callOptions = mockServiceCall.mock.calls[0][0];
            expect(callOptions.request.Skip).toBe(20); // (3-1) * 10
            expect(callOptions.request.Take).toBe(10);
        });

        it("includes params in request", () => {
            view.params = { additionalFilter: "test" };

            const mockServiceCall = vi.fn();
            vi.mocked(serviceCall).mockImplementation(mockServiceCall);

            view.populate();
            expect(mockServiceCall).toHaveBeenCalled();
            const callOptions = mockServiceCall.mock.calls[0][0];
            expect(callOptions.request.additionalFilter).toBe("test");
        });
    });

    describe("syncGridCellCssStyles coverage", () => {
        let view: RemoteView<any>;
        let mockGrid: any;

        beforeEach(() => {
            view = new RemoteView({ idField: "id" });
            view.addItem({ id: 1, name: "Item 1" });
            view.addItem({ id: 2, name: "Item 2" });

            mockGrid = {
                getCellCssStyles: vi.fn(() => ({
                    0: "style-1",
                    1: "style-2"
                })),
                setCellCssStyles: vi.fn(),
                onCellCssStylesChanged: {
                    subscribe: vi.fn(),
                    unsubscribe: vi.fn()
                }
            };
        });

        it("handles cleanup when args.hash is null", () => {
            let subscribedCallback: any;
            // Return empty hash initially so storeCellCssStyles doesn't access self before init
            mockGrid.getCellCssStyles = vi.fn(() => ({}));
            mockGrid.onCellCssStylesChanged.subscribe = vi.fn((cb) => {
                subscribedCallback = cb;
            });

            view.syncGridCellCssStyles(mockGrid, "test-key");

            // Trigger with a different key first - should return early
            subscribedCallback(null, { key: "other-key", hash: { 0: "style" } });
            expect(mockGrid.setCellCssStyles).not.toHaveBeenCalled();

            // Now trigger with no hash (cleanup)
            subscribedCallback(null, { key: "test-key", hash: null });
            expect(mockGrid.onCellCssStylesChanged.unsubscribe).toHaveBeenCalled();
        });

        it("preserves hidden selection when preserveHiddenOnSelectionChange is true and multiSelect is on", () => {
            mockGrid = {
                getSelectedRows: vi.fn(() => [0, 1]),
                setSelectedRows: vi.fn(),
                getOptions: vi.fn(() => ({ multiSelect: true })),
                onSelectedRowsChanged: {
                    subscribe: vi.fn(),
                    unsubscribe: vi.fn()
                }
            };

            view.syncGridSelection(mockGrid, true, true);

            const gridCallback = mockGrid.onSelectedRowsChanged.subscribe.mock.calls[0][0];

            // Simulate grid selection change
            mockGrid.getSelectedRows = vi.fn(() => [0]);
            expect(() => gridCallback(null, null)).not.toThrow();
        });
    });

    describe("sort edge cases", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({ idField: "id" });
            view.addItem({ id: 1, name: "Charlie" });
            view.addItem({ id: 2, name: "Alice" });
            view.addItem({ id: 3, name: "Bob" });
        });

        it("reverses items when ascending is false and then re-sorts", () => {
            view.sort((a, b) => a.name.localeCompare(b.name), false);

            const items = view.getItems();
            expect(items[0].name).toBe("Charlie");
            expect(items[1].name).toBe("Bob");
            expect(items[2].name).toBe("Alice");
        });

        it("enables localSort when setting to different value", () => {
            expect(view.getLocalSort()).toBe(false);
            view.setLocalSort(true);
            expect(view.getLocalSort()).toBe(true);

            // Set to same value should not trigger sort
            view.setLocalSort(true);
            expect(view.getLocalSort()).toBe(true);
        });
    });

    describe("extractGroups edge cases", () => {
        it("handles predefined values and string getter", () => {
            const view = new RemoteView<any>({ idField: "id" });
            view.addItem({ id: 1, name: "Apple", category: "fruit" });
            view.addItem({ id: 2, name: "Banana", category: "fruit" });
            view.addItem({ id: 3, name: "Carrot", category: "vegetable" });

            // Use string getter (not function) and predefined values
            view.setGrouping([{
                getter: "category",
                formatter: (g: any) => g.value,
                collapsed: false,
                predefinedValues: ["fruit", "vegetable", "meat"]
            }]);

            const groups = view.getGroups();
            expect(groups.length).toBe(3);
            expect(groups[0].value).toBe("fruit");
            expect(groups[0].count).toBe(2);
            expect(groups[1].value).toBe("meat");
            expect(groups[1].count).toBe(0);
            expect(groups[2].value).toBe("vegetable");
            expect(groups[2].count).toBe(1);
        });
    });

    describe("expandCollapseGroup with grouping delimiter", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({ idField: "id" });
            view.addItem({ id: 1, name: "Apple", category: "fruit", sub: "sweet" });
            view.addItem({ id: 2, name: "Banana", category: "fruit", sub: "sweet" });
            view.addItem({ id: 3, name: "Carrot", category: "vegetable", sub: "savory" });
        });

        it("handles expand with grouping key containing delimiter", () => {
            view.setGrouping([{
                getter: (item: any) => item.category,
                formatter: (g: any) => g.value,
                collapsed: false
            }]);

            // Collapse all first
            view.collapseAllGroups(0);

            // Try expanding with a key that includes the delimiter
            // The delimiter is ':|:' so we need to simulate it
            const groupingKey = "fruit";
            view.expandGroup([groupingKey]);

            // Should not throw and groups should still be accessible
            expect(view.getGroups().length).toBe(2);
        });
    });

    describe("getRowDiffs coverage", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({ idField: "id" });
        });

        it("handles refreshHints ignoreDiffsBefore and ignoreDiffsAfter", () => {
            view.addItem({ id: 1, name: "A" });
            view.addItem({ id: 2, name: "B" });
            view.addItem({ id: 3, name: "C" });

            view.setRefreshHints({ ignoreDiffsBefore: 1, ignoreDiffsAfter: 2 });

            // Trigger refresh
            view.addItem({ id: 4, name: "D" });

            // Should not throw
            expect(view.getLength()).toBe(4);
        });
    });

    describe("setPagingOptions coverage", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({
                idField: "id",
                rowsPerPage: 10,
                url: "/api/test"
            });
        });

        it("triggers populate when page changes", () => {
            const mockServiceCall = vi.fn();
            vi.mocked(serviceCall).mockImplementation(mockServiceCall);

            view.setPagingOptions({ page: 3 });
            expect(mockServiceCall).toHaveBeenCalled();
        });

        it("does not trigger populate when nothing changes", () => {
            const mockServiceCall = vi.fn();
            vi.mocked(serviceCall).mockImplementation(mockServiceCall);

            view.setPagingOptions({ rowsPerPage: 10 });
            // rowsPerPage is already 10, so no change
            expect(mockServiceCall).not.toHaveBeenCalled();
        });
    });

    describe("getGrandTotals coverage", () => {
        it("initializes grandTotals when not initialized", () => {
            const view = new RemoteView<any>({ idField: "id" });
            view.addItem({ id: 1, value: 10 });
            view.addItem({ id: 2, value: 20 });

            const aggregators = [new Aggregators.Sum("value")];
            view.setSummaryOptions({ aggregators });

            const totals = view.getGrandTotals();
            expect(totals.initialized).toBe(true);
            expect(totals.sum.value).toBe(30);
        });
    });

    describe("populateLock and populateUnlock edge cases", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({
                idField: "id",
                url: "/api/test"
            });
        });

        it("handles multiple populateLocks and unlocks", () => {
            const mockServiceCall = vi.fn();
            vi.mocked(serviceCall).mockImplementation(mockServiceCall);

            view.populateLock();
            view.populateLock();
            expect(view['populateLocks']).toBe(2);

            view.populate();
            expect(mockServiceCall).not.toHaveBeenCalled();

            view.populateUnlock();
            expect(view['populateLocks']).toBe(1);
            expect(mockServiceCall).not.toHaveBeenCalled(); // Still locked

            view.populateUnlock();
            expect(view['populateLocks']).toBe(0);
            // Should auto-trigger populate
            expect(mockServiceCall).toHaveBeenCalled();
        });

        it("handles populateUnlock when already unlocked", () => {
            expect(view['populateLocks']).toBe(0);
            view.populateUnlock(); // Should not throw
            expect(view['populateLocks']).toBe(0);
        });
    });

    describe("setGroupItemMetadataProvider coverage", () => {
        it("sets group item metadata provider", () => {
            const view = new RemoteView<any>({ idField: "id" });
            const provider = {
                getGroupRowMetadata: vi.fn(),
                getTotalsRowMetadata: vi.fn()
            } as any;

            view.setGroupItemMetadataProvider(provider);
            expect(view.getGroupItemMetadataProvider()).toBe(provider);
        });
    });

    describe("getItemMetadataCallback coverage", () => {
        it("gets and sets itemMetadataCallback", () => {
            const view = new RemoteView<any>({ idField: "id" });
            const callback = vi.fn();

            view.setItemMetadataCallback(callback);
            expect(view.getItemMetadataCallback()).toBe(callback);

            // Should call callback when getting metadata
            view.addItem({ id: 1, name: "Test" });
            const result = view.getItemMetadata(0);
            expect(callback).toHaveBeenCalledWith({ id: 1, name: "Test" }, 0);
        });

        it("returns null when no callback and no special row", () => {
            const view = new RemoteView<any>({ idField: "id" });
            view.addItem({ id: 1, name: "Test" });
            const result = view.getItemMetadata(0);
            expect(result).toBeNull();
        });
    });

    describe("idField getter", () => {
        it("returns the id property name", () => {
            const view = new RemoteView<any>({ idField: "myId" });
            expect(view.idField).toBe("myId");
        });
    });

    describe("recalc events", () => {
        it("has onRecalcRows event emitter", () => {
            const view = new RemoteView<any>({ idField: "id" });
            expect(view.onRecalcRows).toBeDefined();
            expect(() => view.onRecalcRows.subscribe(vi.fn())).not.toThrow();
        });
    });

    describe("refresh hints coverage", () => {
        let view: RemoteView<any>;

        beforeEach(() => {
            view = new RemoteView({ idField: "id" });
            view.addItem({ id: 1, name: "Apple", category: "fruit" });
            view.addItem({ id: 2, name: "Banana", category: "fruit" });
            view.addItem({ id: 3, name: "Carrot", category: "vegetable" });
            view.addItem({ id: 4, name: "Date", category: "fruit" });
        });

        it("handles isFilterNarrowing hint", () => {
            const filter = (item: any) => item.category === "fruit";
            view.setFilter(filter);

            // First filter to populate filterCache
            expect(view.getFilteredItems().length).toBe(3);

            // Now set isFilterNarrowing hint and change filter to be more narrow
            const narrowFilter = (item: any) => item.category === "fruit" && item.name.startsWith("A");
            view.setRefreshHints({ isFilterNarrowing: true });
            view.setFilter(narrowFilter);

            // Should use batchFilter on filteredItems
            expect(view.getFilteredItems().length).toBe(1);
            expect(view.getFilteredItems()[0].name).toBe("Apple");
        });

        it("handles isFilterExpanding hint", () => {
            // First set a narrow filter
            const narrowFilter = (item: any) => item.category === "fruit";
            view.setFilter(narrowFilter);
            expect(view.getFilteredItems().length).toBe(3);

            // Now expand the filter
            const broadFilter = (item: any) => true;
            view.setRefreshHints({ isFilterExpanding: true });
            view.setFilter(broadFilter);

            // Should use batchFilterWithCaching
            expect(view.getFilteredItems().length).toBe(4);
        });

        it("handles isFilterUnchanged hint", () => {
            // Set a filter
            const filter = (item: any) => item.category === "fruit";
            view.setFilter(filter);
            expect(view.getFilteredItems().length).toBe(3);

            // Now set isFilterUnchanged hint
            view.setRefreshHints({ isFilterUnchanged: true });
            const filter2 = (item: any) => item.category === "fruit";
            view.setFilter(filter2);

            // Should skip filtering
            expect(view.getFilteredItems().length).toBe(3);
        });
    });
});