import { RemoteView } from "./remoteview";
import { vi } from "vitest";

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
});