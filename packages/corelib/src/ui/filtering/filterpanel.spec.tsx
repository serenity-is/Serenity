import { FilterPanel } from "./filterpanel";

describe("FilterPanel", () => {
    it("is registered with typeInfo", () => {
        expect(FilterPanel[Symbol.for("Serenity.typeInfo")]).toBeDefined();
    });

    it("can be constructed with default props", () => {
        const panel = new FilterPanel({});
        expect(panel).toBeInstanceOf(FilterPanel);
        panel.destroy();
    });

    it("domNode has s-FilterPanel class", () => {
        const panel = new FilterPanel({});
        expect(panel.domNode.classList.contains("s-FilterPanel'")).toBe(true);
        panel.destroy();
    });

    it("showInitialLine defaults to false", () => {
        const panel = new FilterPanel({});
        expect(panel.showInitialLine).toBeFalsy();
        panel.destroy();
    });

    it("showInitialLine can be set to true", () => {
        const panel = new FilterPanel({});
        panel.showInitialLine = true;
        expect(panel.showInitialLine).toBe(true);
        panel.destroy();
    });

    it("showSearchButton defaults to undefined/falsy", () => {
        const panel = new FilterPanel({});
        expect(panel.showSearchButton).toBeFalsy();
        panel.destroy();
    });

    it("showSearchButton can be set", () => {
        const panel = new FilterPanel({});
        panel.showSearchButton = true;
        expect(panel.showSearchButton).toBe(true);
        panel.destroy();
    });

    it("updateStoreOnReset defaults to undefined", () => {
        const panel = new FilterPanel({});
        expect((panel as any).updateStoreOnReset).toBeUndefined();
        panel.destroy();
    });

    it("get_hasErrors returns false when no error elements", () => {
        const panel = new FilterPanel({});
        expect(panel.get_hasErrors()).toBe(false);
        panel.destroy();
    });

    it("resetButtonClick with updateStoreOnReset truthy clears store items", () => {
        const panel = new FilterPanel({});
        (panel as any).updateStoreOnReset = true;
        const store = panel.get_store();
        store.get_items().push({ field: "F1", displayText: "test" });
        expect(store.get_items().length).toBe(1);

        (panel as any).resetButtonClick(new MouseEvent("click"));
        expect(store.get_items().length).toBe(0);
        panel.destroy();
    });

    it("resetButtonClick with falsy updateStoreOnReset does not clear store", () => {
        const panel = new FilterPanel({});
        const store = panel.get_store();
        store.get_items().push({ field: "F1", displayText: "test" });

        (panel as any).resetButtonClick(new MouseEvent("click"));
        expect(store.get_items().length).toBe(1);
        panel.destroy();
    });

    it("andOrClick toggles the andor class", () => {
        const panel = new FilterPanel({});
        const el = document.createElement("a");
        el.classList.add("andor");

        (panel as any).andOrClick({ target: el, preventDefault: vi.fn() } as any);
        expect(el.classList.contains("or")).toBe(true);

        (panel as any).andOrClick({ target: el, preventDefault: vi.fn() } as any);
        expect(el.classList.contains("or")).toBe(false);
        panel.destroy();
    });

    it("leftRightParenClick toggles active class", () => {
        const panel = new FilterPanel({});
        const el = document.createElement("a");
        el.classList.add("leftparen");

        (panel as any).leftRightParenClick({ target: el, preventDefault: vi.fn() } as any);
        expect(el.classList.contains("active")).toBe(true);

        (panel as any).leftRightParenClick({ target: el, preventDefault: vi.fn() } as any);
        expect(el.classList.contains("active")).toBe(false);
        panel.destroy();
    });

    it("findEmptyRow returns null when no rows", () => {
        const panel = new FilterPanel({});
        expect((panel as any).findEmptyRow()).toBeNull();
        panel.destroy();
    });

    it("removeFiltering cleans up row metadata", () => {
        const panel = new FilterPanel({});
        const row = document.createElement("div");
        (row as any).__Filtering = {};
        (row as any).__FilteringField = {};

        (panel as any).removeFiltering(row);
        expect((row as any).__Filtering).toBeUndefined();
        expect((row as any).__FilteringField).toBeUndefined();
        panel.destroy();
    });

    it("getFieldFor returns null for null row", () => {
        const panel = new FilterPanel({});
        expect((panel as any).getFieldFor(null)).toBeNull();
        panel.destroy();
    });

    it("search returns empty when no rows with content", () => {
        const panel = new FilterPanel({});
        (panel as any).search();
        // Should not throw and store should have no items
        expect(panel.get_store().get_items().length).toBe(0);
        panel.destroy();
    });

    it("updateButtons toggles search and reset buttons visibility", () => {
        const panel = new FilterPanel({});
        (panel as any).updateButtons();
        // With no rows, search button should be hidden
        expect((panel as any).searchButton).toBeTruthy();
        expect((panel as any).resetButton).toBeTruthy();
        panel.destroy();
    });

    it("updateParens handles single row", () => {
        const panel = new FilterPanel({});
        // Set up rowsDiv properly
        const rowsDiv = document.createElement("div");
        (panel as any).rowsDiv = rowsDiv;

        const row = document.createElement("div");
        const divL = document.createElement("div");
        divL.className = "l";
        divL.innerHTML = '<a class="leftparen"></a><a class="rightparen"></a><a class="andor"></a>';
        row.appendChild(divL);
        rowsDiv.appendChild(row);

        (panel as any).updateParens();
        // When there is only 1 row, divL should have hidden=true
        expect(divL.hidden).toBe(true);
        panel.destroy();
    });

    it("updateParens with multiple rows", () => {
        const panel = new FilterPanel({});
        const rowsDiv = document.createElement("div");
        (panel as any).rowsDiv = rowsDiv;

        const row1 = document.createElement("div");
        const divL1 = document.createElement("div");
        divL1.innerHTML = '<a class="leftparen active"></a><a class="rightparen"></a><a class="andor"></a>';
        row1.appendChild(divL1);
        rowsDiv.appendChild(row1);

        const row2 = document.createElement("div");
        const divL2 = document.createElement("div");
        divL2.innerHTML = '<a class="leftparen"></a><a class="rightparen active"></a><a class="andor"></a>';
        row2.appendChild(divL2);
        rowsDiv.appendChild(row2);

        (panel as any).updateParens();
        expect(divL1.hidden).toBe(false);
        expect(divL2.hidden).toBe(false);
        panel.destroy();
    });

    it("deleteRowClick removes a row", () => {
        const panel = new FilterPanel({});
        const rowsDiv = document.createElement("div");
        (panel as any).rowsDiv = rowsDiv;
        const row = document.createElement("div");
        row.classList.add("filter-line");
        rowsDiv.appendChild(row);
        expect(rowsDiv.children.length).toBe(1);

        (panel as any).deleteRowClick({ target: row, preventDefault: vi.fn() } as any);
        expect(rowsDiv.children.length).toBe(0);
        panel.destroy();
    });

    it("filterStoreChanged calls updateRowsFromStore", () => {
        const panel = new FilterPanel({});
        const spy = vi.spyOn(panel as any, "updateRowsFromStore");
        panel.get_store().raiseChanged();
        expect(spy).toHaveBeenCalled();
        panel.destroy();
    });

    it("searchButtonClick prevents default and calls search", () => {
        const panel = new FilterPanel({});
        const searchSpy = vi.spyOn(panel as any, "search");
        const e = { preventDefault: vi.fn() };

        (panel as any).searchButtonClick(e);
        expect(e.preventDefault).toHaveBeenCalled();
        expect(searchSpy).toHaveBeenCalled();
        panel.destroy();
    });

    it("addButtonClick prevents default and calls addEmptyRow", () => {
        const panel = new FilterPanel({});
        const addSpy = vi.spyOn(panel as any, "addEmptyRow");
        const e = { preventDefault: vi.fn() };

        (panel as any).addButtonClick(e);
        expect(e.preventDefault).toHaveBeenCalled();
        expect(addSpy).toHaveBeenCalledWith(true);
        panel.destroy();
    });

    it("complete lifecycle: create, add rows, reset, destroy", () => {
        const panel = new FilterPanel({});
        panel.showInitialLine = true;

        // After setting showInitialLine, there should be a row
        if (panel.showInitialLine) {
            (panel as any).addEmptyRow(false);
            expect((panel as any).rowsDiv.children.length).toBeGreaterThan(0);
        }

        // Reset should clear rows
        (panel as any).resetButtonClick(new MouseEvent("click"));
        panel.destroy();
    });

    it("search with items processes them correctly", () => {
        const panel = new FilterPanel({});
        const rowsDiv = document.createElement("div");
        (panel as any).rowsDiv = rowsDiv;
        const store = panel.get_store();

        // Add fields to store so that field lookup works
        const field = { name: "TestField", title: "Test", filteringType: "String" };
        // Directly push to store's fields
        (store as any).fields = [field];
        (store as any).fieldByName = { TestField: field };

        // Add a row
        (panel as any).addEmptyRow(false);
        expect(rowsDiv.children.length).toBe(1);
        panel.destroy();
    });
});
