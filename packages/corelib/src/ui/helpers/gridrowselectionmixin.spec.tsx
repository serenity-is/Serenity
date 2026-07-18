import { GridRowSelectionMixin } from "./gridrowselectionmixin";

describe("GridRowSelectionMixin", () => {
    let mockGrid: any;
    let gridObj: any;
    let viewObj: any;

    beforeEach(() => {
        gridObj = {
            onClick: { subscribe: vi.fn(), unsubscribe: vi.fn() },
            onHeaderClick: { subscribe: vi.fn(), unsubscribe: vi.fn() },
            updateRow: vi.fn()
        };
        viewObj = {
            getIdPropertyName: vi.fn(() => "id"),
            getItem: vi.fn(),
            getLength: vi.fn(() => 0),
            getItems: vi.fn(() => []),
            setItems: vi.fn(),
            onRowsChanged: { subscribe: vi.fn(), unsubscribe: vi.fn() }
        };
        mockGrid = {
            getGrid: vi.fn(() => gridObj),
            getView: vi.fn(() => viewObj),
            getElement: vi.fn(() => document.createElement("div"))
        };
    });

    it("constructor subscribes to grid events", () => {
        const mixin = new GridRowSelectionMixin(mockGrid);
        expect(mockGrid.getGrid().onClick.subscribe).toHaveBeenCalled();
        expect(mockGrid.getGrid().onHeaderClick.subscribe).toHaveBeenCalled();
    });

    it("destroy unsubscribes events and cleans up", () => {
        const mixin = new GridRowSelectionMixin(mockGrid);
        mixin.destroy();
        expect(mockGrid.getGrid().onClick.unsubscribe).toHaveBeenCalled();
        expect(mockGrid.getGrid().onHeaderClick.unsubscribe).toHaveBeenCalled();
        expect((mixin as any).grid).toBeUndefined();
    });

    it("destroy handles missing onRowsChanged gracefully", () => {
        const mixin = new GridRowSelectionMixin(mockGrid);
        (mixin as any).grid = {
            getGrid: () => gridObj,
            getView: () => ({ onRowsChanged: null })
        };
        expect(() => mixin.destroy()).not.toThrow();
    });

    it("updateSelectAll toggles checked class when all items selectable", () => {
        const element = document.createElement("div");
        const headerDiv = document.createElement("div");
        headerDiv.className = "select-all-header";
        const nameDiv = document.createElement("div");
        nameDiv.className = "slick-column-name";
        const button = document.createElement("a");
        button.className = "select-all-items";
        nameDiv.appendChild(button);
        headerDiv.appendChild(nameDiv);
        element.appendChild(headerDiv);

        mockGrid.getElement = vi.fn(() => element);
        mockGrid.getView = vi.fn(() => ({
            getIdPropertyName: vi.fn(() => "id"),
            getItems: vi.fn(() => [{ id: 1 }, { id: 2 }])
        }));

        const mixin = new GridRowSelectionMixin(mockGrid);
        (mixin as any).include = { "1": true, "2": true };

        mixin.updateSelectAll();
        expect(button.classList.contains("checked")).toBe(true);
    });

    it("updateSelectAll removes checked class when not all items selected", () => {
        const element = document.createElement("div");
        const headerDiv = document.createElement("div");
        headerDiv.className = "select-all-header";
        const nameDiv = document.createElement("div");
        nameDiv.className = "slick-column-name";
        const button = document.createElement("a");
        button.className = "select-all-items";
        button.classList.add("checked");
        nameDiv.appendChild(button);
        headerDiv.appendChild(nameDiv);
        element.appendChild(headerDiv);

        mockGrid.getElement = vi.fn(() => element);
        mockGrid.getView = vi.fn(() => ({
            getIdPropertyName: vi.fn(() => "id"),
            getItems: vi.fn(() => [{ id: 1 }, { id: 2 }])
        }));

        const mixin = new GridRowSelectionMixin(mockGrid);
        // Only one item selected out of two
        (mixin as any).include = { "1": true };

        mixin.updateSelectAll();
        expect(button.classList.contains("checked")).toBe(false);
    });

    it("updateSelectAll does nothing if no select-all button", () => {
        const mixin = new GridRowSelectionMixin(mockGrid);
        expect(() => mixin.updateSelectAll()).not.toThrow();
    });
});
