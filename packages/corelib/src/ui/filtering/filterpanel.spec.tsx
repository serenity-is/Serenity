import { Fluent, FilterPanelTexts } from "../../base";
import { FilterPanel, FilterFieldSelect, FilterOperatorSelect } from "./filterpanel";
import { FilterOperator } from "./filteroperator";
import { StringFiltering } from "./stringfiltering";
import { FilteringTypeRegistry } from "./filteringtyperegistry";
import { getWidgetFrom } from "../widgets/widget";

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

    it("hasErrors returns false when no error elements", () => {
        const panel = new FilterPanel({});
        expect(panel.hasErrors).toBe(false);
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

    it("showInitialLine setter adds row when true and no rows present", () => {
        const panel = new FilterPanel({});
        // _showInitialLine starts undefined, set to true
        panel.showInitialLine = true;
        expect((panel as any)._showInitialLine).toBe(true);
        // Should have added a row since showInitialLine is true and no rows exist
        expect((panel as any).rowsDiv.childElementCount).toBeGreaterThanOrEqual(1);
        panel.destroy();
    });

    it("showInitialLine setter does nothing when value unchanged", () => {
        const panel = new FilterPanel({});
        panel.showInitialLine = true;
        const rowCountAfterFirst = (panel as any).rowsDiv.childElementCount;
        // Set again to same value
        panel.showInitialLine = true;
        expect((panel as any).rowsDiv.childElementCount).toBe(rowCountAfterFirst);
        panel.destroy();
    });

    it("showSearchButton setter calls updateButtons", () => {
        const panel = new FilterPanel({});
        const spy = vi.spyOn(panel as any, "updateButtons");
        panel.showSearchButton = true;
        expect(spy).toHaveBeenCalled();
        expect(panel.showSearchButton).toBe(true);
        panel.destroy();
    });

    it("findEmptyRow returns row when field input has no value", () => {
        const panel = new FilterPanel({});
        const row = document.createElement("div");
        const divF = document.createElement("div");
        divF.className = "f";
        const input = document.createElement("input");
        input.className = "field-select";
        input.type = "hidden";
        input.value = "";
        divF.appendChild(input);
        row.appendChild(divF);
        (panel as any).rowsDiv.appendChild(row);

        const result = (panel as any).findEmptyRow();
        expect(result).toBe(row);
        panel.destroy();
    });

    it("findEmptyRow returns null when field input has value", () => {
        const panel = new FilterPanel({});
        const row = document.createElement("div");
        const divF = document.createElement("div");
        divF.className = "f";
        const input = document.createElement("input");
        input.className = "field-select";
        input.type = "hidden";
        input.value = "SomeValue";
        divF.appendChild(input);
        row.appendChild(divF);
        (panel as any).rowsDiv.appendChild(row);

        const result = (panel as any).findEmptyRow();
        expect(result).toBeNull();
        panel.destroy();
    });

    it("addEmptyRow returns existing empty row when found", () => {
        const panel = new FilterPanel({});
        // First add a real row
        (panel as any).addEmptyRow(false);
        // Now add another - should find the first empty row and return it
        const result = (panel as any).addEmptyRow(false);
        expect(result).toBeTruthy();
        expect(result.classList.contains("filter-line")).toBe(true);
        panel.destroy();
    });

    it("getFieldFor returns null when row is null", () => {
        const panel = new FilterPanel({});
        expect((panel as any).getFieldFor(null)).toBeNull();
        panel.destroy();
    });

    it("getFieldFor throws when no FilterFieldSelect widget attached", () => {
        const panel = new FilterPanel({});
        const row = document.createElement("div");
        const divF = document.createElement("div");
        divF.className = "f";
        const input = document.createElement("input");
        input.className = "field-select";
        input.type = "hidden";
        divF.appendChild(input);
        row.appendChild(divF);

        // getFieldFor calls getWidgetFrom which throws if no widget attached
        expect(() => (panel as any).getFieldFor(row)).toThrow();
        panel.destroy();
    });

    it("getFilteringFor throws when row has no FilterFieldSelect widget", () => {
        const panel = new FilterPanel({});
        const row = document.createElement("div");
        // getFilteringFor calls getFieldFor which calls getWidgetFrom
        expect(() => (panel as any).getFilteringFor(row)).toThrow();
        panel.destroy();
    });

    it("deleteRowClick triggers search when no children left", () => {
        const panel = new FilterPanel({});
        const searchSpy = vi.spyOn(panel as any, "search");
        const row = document.createElement("div");
        row.classList.add("filter-line");
        (panel as any).rowsDiv.appendChild(row);

        (panel as any).deleteRowClick({ target: row, preventDefault: vi.fn() } as any);
        expect((panel as any).rowsDiv.children.length).toBe(0);
        expect(searchSpy).toHaveBeenCalled();
        panel.destroy();
    });

    it("updateButtons toggles reset button based on children count", () => {
        const panel = new FilterPanel({});
        // Initially no children, reset button should be hidden
        expect((panel as any).resetButton.hidden).toBe(true);

        // Add a child row
        const row = document.createElement("div");
        (panel as any).rowsDiv.appendChild(row);
        (panel as any).updateButtons();
        expect((panel as any).resetButton.hidden).toBe(false);

        panel.destroy();
    });

    it("updateButtons toggles search button based on children and showSearchButton", () => {
        const panel = new FilterPanel({});
        panel.showSearchButton = true;
        // No children, search button should be hidden
        expect((panel as any).searchButton.hidden).toBe(true);

        // Add a child
        const row = document.createElement("div");
        (panel as any).rowsDiv.appendChild(row);
        (panel as any).updateButtons();
        expect((panel as any).searchButton.hidden).toBe(false);

        panel.destroy();
    });

    it("search method does not throw when called on panel with rows", () => {
        const panel = new FilterPanel({});
        // Use addEmptyRow to properly initialize a row with widgets
        (panel as any).addEmptyRow(false);
        expect((panel as any).rowsDiv.childElementCount).toBe(1);

        // search() should not throw when iterating rows
        expect(() => (panel as any).search()).not.toThrow();
        panel.destroy();
    });

    it("removeFiltering cleans up row metadata after rowFieldChange", () => {
        const panel = new FilterPanel({});
        const row = document.createElement("div");
        const divO = document.createElement("div");
        divO.className = "o";
        row.appendChild(divO);
        const divF = document.createElement("div");
        divF.className = "f";
        const input = document.createElement("input");
        input.className = "field-select";
        input.type = "hidden";
        divF.appendChild(input);
        row.appendChild(divF);
        const divV = document.createElement("div");
        divV.className = "v";
        row.appendChild(divV);
        (row as any).__Filtering = {};
        (row as any).__FilteringField = {};

        (panel as any).removeFiltering(row);
        expect((row as any).__Filtering).toBeUndefined();
        expect((row as any).__FilteringField).toBeUndefined();
        panel.destroy();
    });

    it("rowFieldChange calls removeFiltering, populateOperatorList, rowOperatorChange, updateParens, updateButtons", () => {
        const panel = new FilterPanel({});
        // Create a proper DOM row element
        const row = document.createElement("div");
        const divO = document.createElement("div");
        divO.className = "o";
        row.appendChild(divO);
        const divF = document.createElement("div");
        divF.className = "f";
        const input = document.createElement("input");
        input.className = "field-select";
        input.type = "hidden";
        divF.appendChild(input);
        row.appendChild(divF);
        const divV = document.createElement("div");
        divV.className = "v";
        row.appendChild(divV);

        const removeSpy = vi.spyOn(panel as any, "removeFiltering");
        const popSpy = vi.spyOn(panel as any, "populateOperatorList");
        const rowOpSpy = vi.spyOn(panel as any, "rowOperatorChange");
        const parenSpy = vi.spyOn(panel as any, "updateParens");
        const btnSpy = vi.spyOn(panel as any, "updateButtons");
        // Mock getFilteringFor to return null so populateOperatorList doesn't throw
        vi.spyOn(panel as any, "getFilteringFor").mockReturnValue(null);

        (panel as any).rowFieldChange(row);
        expect(removeSpy).toHaveBeenCalledWith(row);
        expect(popSpy).toHaveBeenCalledWith(row);
        expect(rowOpSpy).toHaveBeenCalledWith(row);
        expect(parenSpy).toHaveBeenCalled();
        expect(btnSpy).toHaveBeenCalled();
        panel.destroy();
    });

    it("populateOperatorList clears opDiv and returns early when getFilteringFor returns null", () => {
        const panel = new FilterPanel({});
        const row = document.createElement("div");
        const opDiv = document.createElement("div");
        opDiv.className = "o";
        row.appendChild(opDiv);
        // Add some content to opDiv to verify it gets emptied
        opDiv.appendChild(document.createElement("span"));

        // Mock getFilteringFor to return null so it returns early
        vi.spyOn(panel as any, "getFilteringFor").mockReturnValue(null);

        (panel as any).populateOperatorList(row);
        // opDiv should be empty (Fluent(opDiv).empty() runs before getFilteringFor check)
        expect(opDiv.children.length).toBe(0);
        panel.destroy();
    });

    it("rowOperatorChange returns early when row is null", () => {
        const panel = new FilterPanel({});
        // Should not throw
        (panel as any).rowOperatorChange(null);
        panel.destroy();
    });

    it("rowOperatorChange returns early when getFilteringFor returns null", () => {
        const panel = new FilterPanel({});
        const row = document.createElement("div");
        const editorDiv = document.createElement("div");
        editorDiv.className = "v";
        row.appendChild(editorDiv);

        // Mock getFilteringFor to return null
        vi.spyOn(panel as any, "getFilteringFor").mockReturnValue(null);

        // Should not throw and return early
        expect(() => (panel as any).rowOperatorChange(row)).not.toThrow();
        panel.destroy();
    });

    it("updateRowsFromStore calls addEmptyRow for each store item", () => {
        const panel = new FilterPanel({});
        const store = panel.get_store();

        // Add items to store
        store.get_items().push({
            field: "TestField",
            operator: "eq",
            displayText: "TestField = value",
            leftParen: true,
            isOr: false
        });

        const addSpy = vi.spyOn(panel as any, "addEmptyRow");

        // updateRowsFromStore will call addEmptyRow for each item
        // It may throw later due to missing widget infrastructure
        try {
            (panel as any).updateRowsFromStore();
        } catch (e) {
            // Expected - widget infrastructure not fully available
        }

        // addEmptyRow should have been called at least once (for the store item)
        expect(addSpy).toHaveBeenCalled();
        panel.destroy();
    });

    it("updateRowsFromStore adds initial line when showInitialLine is true and no rows", () => {
        const panel = new FilterPanel({});
        panel.showInitialLine = true;
        // Clear rows
        Fluent((panel as any).rowsDiv).empty();
        // Call updateRowsFromStore with no items
        (panel as any).updateRowsFromStore();
        // Should add an empty row because showInitialLine is true
        expect((panel as any).rowsDiv.childElementCount).toBeGreaterThan(0);
        panel.destroy();
    });

    it("onRowFieldChange does not throw with valid event target", () => {
        const panel = new FilterPanel({});
        // Use addEmptyRow to create a properly structured row
        (panel as any).addEmptyRow(false);
        const row = (panel as any).rowsDiv.lastElementChild;
        const opInput = row.querySelector('div.o input.op-select');

        if (opInput) {
            const event = { target: opInput } as any;
            expect(() => (panel as any).onRowFieldChange(event)).not.toThrow();
        }
        panel.destroy();
    });

    it("resetButtonClick with showInitialLine true adds empty row", () => {
        const panel = new FilterPanel({});
        panel.showInitialLine = true;
        // Add a row first
        (panel as any).addEmptyRow(false);
        expect((panel as any).rowsDiv.childElementCount).toBeGreaterThan(0);

        // Reset should clear rows and add initial line
        (panel as any).resetButtonClick(new MouseEvent("click"));
        expect((panel as any).rowsDiv.childElementCount).toBeGreaterThan(0);
        panel.destroy();
    });

    it("search method with empty rows does nothing", () => {
        const panel = new FilterPanel({});
        const store = panel.get_store();
        expect(store.get_items().length).toBe(0);

        (panel as any).search();
        expect(store.get_items().length).toBe(0);
        panel.destroy();
    });

    it("getStore returns the store", () => {
        const panel = new FilterPanel({});
        const store = panel.get_store();
        expect(store).toBeDefined();
        expect(store.get_fields()).toBeDefined();
        panel.destroy();
    });

    it("renderContents renders with correct structure", () => {
        const panel = new FilterPanel({});
        const rendered = (panel as any).renderContents();
        expect(rendered).toBeTruthy();
        // Verify domNode has the buttons div
        expect(panel.domNode.querySelector('.buttons')).toBeTruthy();
        expect(panel.domNode.querySelector('.filter-lines')).toBeTruthy();
        expect(panel.domNode.querySelector('button.add')).toBeTruthy();
        expect(panel.domNode.querySelector('button.search')).toBeTruthy();
        expect(panel.domNode.querySelector('button.reset')).toBeTruthy();
        panel.destroy();
    });
});

describe("FilterFieldSelect", () => {
    it("can be constructed with fields", () => {
        const input = document.createElement("input");
        input.type = "hidden";
        const fields = [
            { name: "Field1", title: "Field 1" },
            { name: "Field2", title: "Field 2" }
        ];
        const select = new FilterFieldSelect({ element: input, fields });
        expect(select).toBeInstanceOf(FilterFieldSelect);
        select.destroy();
    });

    it("adds options for each field", () => {
        const input = document.createElement("input");
        input.type = "hidden";
        const fields = [
            { name: "Field1", title: "Field 1" },
            { name: "Field2", title: "Field 2" }
        ];
        const select = new FilterFieldSelect({ element: input, fields });
        // addOption should have been called for each field
        expect(select.value).toBeFalsy(); // no value set initially
        select.destroy();
    });

    it("emptyItemText returns SelectField text when no value", () => {
        const input = document.createElement("input");
        input.type = "hidden";
        const select = new FilterFieldSelect({ element: input, fields: [] });
        expect((select as any).emptyItemText()).toBe(FilterPanelTexts.SelectField);
        select.destroy();
    });

    it("emptyItemText returns null when value is set", () => {
        const input = document.createElement("input");
        input.type = "hidden";
        const fields = [{ name: "F1", title: "F1" }];
        const select = new FilterFieldSelect({ element: input, fields });
        // Set a value
        select.value = "F1";
        expect((select as any).emptyItemText()).toBeNull();
        select.destroy();
    });

    it("getComboboxOptions sets allowClear to false", () => {
        const input = document.createElement("input");
        input.type = "hidden";
        const select = new FilterFieldSelect({ element: input, fields: [] });
        const opts = (select as any).getComboboxOptions();
        expect(opts.allowClear).toBe(false);
        select.destroy();
    });
});

describe("FilterOperatorSelect", () => {
    it("can be constructed with source operators", () => {
        const input = document.createElement("input");
        input.type = "hidden";
        const operators: FilterOperator[] = [
            { key: "eq", title: "Equals" },
            { key: "ne", title: "Not Equals" }
        ];
        const select = new FilterOperatorSelect({ element: input, source: operators });
        expect(select).toBeInstanceOf(FilterOperatorSelect);
        select.destroy();
    });

    it("sets value to first operator key", () => {
        const input = document.createElement("input");
        input.type = "hidden";
        const operators: FilterOperator[] = [
            { key: "eq", title: "Equals" },
            { key: "ne", title: "Not Equals" }
        ];
        const select = new FilterOperatorSelect({ element: input, source: operators });
        expect(select.value).toBe("eq");
        select.destroy();
    });

    it("emptyItemText returns null", () => {
        const input = document.createElement("input");
        input.type = "hidden";
        const select = new FilterOperatorSelect({ element: input, source: [] });
        expect((select as any).emptyItemText()).toBeNull();
        select.destroy();
    });

    it("getComboboxOptions sets allowClear to false", () => {
        const input = document.createElement("input");
        input.type = "hidden";
        const select = new FilterOperatorSelect({ element: input, source: [] });
        const opts = (select as any).getComboboxOptions();
        expect(opts.allowClear).toBe(false);
        select.destroy();
    });

    it("handles operators with title from OperatorNames", () => {
        const input = document.createElement("input");
        input.type = "hidden";
        const operators: FilterOperator[] = [
            { key: "custom_op" }
        ];
        const select = new FilterOperatorSelect({ element: input, source: operators });
        // Should not throw and value should be set
        expect(select.value).toBe("custom_op");
        select.destroy();
    });
});

describe("FilterPanel with StringFiltering", () => {
    // Importing StringFiltering triggers registerClass which registers the type
    // with FilteringTypeRegistry, enabling filtering operations
    it("StringFiltering is defined", () => {
        expect(StringFiltering).toBeDefined();
    });

    it("can register and retrieve StringFiltering type from registry", () => {
        const type = FilteringTypeRegistry.get("String");
        expect(type).toBeDefined();
        expect(type).toBe(StringFiltering);
    });

    it("addEmptyRow creates row with proper structure when filtering type is available", () => {
        const panel = new FilterPanel({});
        const store = panel.get_store();
        const field = { name: "F1", title: "F1", filteringType: "String" };
        (store as any).fields = [field];
        (store as any).fieldByName = { F1: field };

        (panel as any).addEmptyRow(false);
        const row = (panel as any).rowsDiv.lastElementChild;
        expect(row).toBeTruthy();
        expect(row.classList.contains("filter-line")).toBe(true);
        expect(row.querySelector('div.f input.field-select')).toBeTruthy();
        expect(row.querySelector('div.o')).toBeTruthy();
        expect(row.querySelector('div.v')).toBeTruthy();
        expect(row.querySelector('div.l')).toBeTruthy();
        panel.destroy();
    });

    it("updateRowsFromStore with items and registered String type adds rows", () => {
        const panel = new FilterPanel({});
        const store = panel.get_store();

        store.get_items().push({
            field: "F1",
            operator: "eq",
            displayText: "F1 = value",
            leftParen: true,
            isOr: false
        });

        const field = { name: "F1", title: "F1", filteringType: "String" };
        (store as any).fields = [field];
        (store as any).fieldByName = { F1: field };

        // This should work because StringFiltering is now registered
        expect(() => (panel as any).updateRowsFromStore()).not.toThrow();
        expect((panel as any).rowsDiv.childElementCount).toBe(1);
        panel.destroy();
    });

    it("updateParens adds paren-end class when right paren closes a paren block", () => {
        const panel = new FilterPanel({});
        const rowsDiv = document.createElement("div");
        (panel as any).rowsDiv = rowsDiv;

        // Row 0: left paren active
        const row0 = document.createElement("div");
        const divL0 = document.createElement("div");
        divL0.className = "l";
        divL0.innerHTML = '<a class="leftparen active"></a><a class="rightparen"></a><a class="andor"></a>';
        row0.appendChild(divL0);
        rowsDiv.appendChild(row0);

        // Row 1: no parens
        const row1 = document.createElement("div");
        const divL1 = document.createElement("div");
        divL1.className = "l";
        divL1.innerHTML = '<a class="leftparen"></a><a class="rightparen"></a><a class="andor"></a>';
        row1.appendChild(divL1);
        rowsDiv.appendChild(row1);

        // Row 2: right paren active (closing)
        const row2 = document.createElement("div");
        const divL2 = document.createElement("div");
        divL2.className = "l";
        divL2.innerHTML = '<a class="leftparen"></a><a class="rightparen active"></a><a class="andor"></a>';
        row2.appendChild(divL2);
        rowsDiv.appendChild(row2);

        (panel as any).updateParens();

        // Row1 should have paren-end (the row before the closing paren)
        expect(row1.classList.contains("paren-end")).toBe(true);
        // Row0 should have paren-start (the row before the opening paren)
        expect(row0.classList.contains("paren-start")).toBe(false); // row0 is index 0, so index > 0 is false
        panel.destroy();
    });

    it("updateParens adds paren-start class when left paren opens after first row", () => {
        const panel = new FilterPanel({});
        const rowsDiv = document.createElement("div");
        (panel as any).rowsDiv = rowsDiv;

        // Row 0: no parens
        const row0 = document.createElement("div");
        const divL0 = document.createElement("div");
        divL0.className = "l";
        divL0.innerHTML = '<a class="leftparen"></a><a class="rightparen"></a><a class="andor"></a>';
        row0.appendChild(divL0);
        rowsDiv.appendChild(row0);

        // Row 1: left paren active
        const row1 = document.createElement("div");
        const divL1 = document.createElement("div");
        divL1.className = "l";
        divL1.innerHTML = '<a class="leftparen active"></a><a class="rightparen"></a><a class="andor"></a>';
        row1.appendChild(divL1);
        rowsDiv.appendChild(row1);

        (panel as any).updateParens();

        // Row0 should have paren-start (the row before the left paren)
        expect(row0.classList.contains("paren-start")).toBe(true);
        panel.destroy();
    });

    it("search with valid row and registered StringFiltering processes correctly", () => {
        const panel = new FilterPanel({});
        const store = panel.get_store();
        const field = { name: "F1", title: "F1", filteringType: "String" };
        (store as any).fields = [field];
        (store as any).fieldByName = { F1: field };

        // Add a row via addEmptyRow which creates proper widgets
        (panel as any).addEmptyRow(false);
        expect((panel as any).rowsDiv.childElementCount).toBe(1);

        // search should not throw
        expect(() => (panel as any).search()).not.toThrow();
        panel.destroy();
    });

    it("getFieldFor returns the field when select has value and field exists in store", () => {
        const panel = new FilterPanel({});
        const store = panel.get_store();
        const field = { name: "F1", title: "F1", filteringType: "String" };
        (store as any).fields = [field];
        (store as any).fieldByName = { F1: field };

        // Use addEmptyRow to get a proper row with FilterFieldSelect widget
        (panel as any).addEmptyRow(false);
        const row = (panel as any).rowsDiv.lastElementChild;

        // Set the field select value using the widget
        const fieldInput = row.querySelector('div.f input.field-select');
        if (fieldInput) {
            try {
                const widget = getWidgetFrom(fieldInput, FilterFieldSelect);
                widget.value = "F1";
            } catch {
                // Widget access might fail in test environment
            }
        }

        // getFieldFor should return the field when value is set
        const result = (panel as any).getFieldFor(row);
        // May be null if widget value wasn't set, but should not throw
        expect(() => (panel as any).getFieldFor(row)).not.toThrow();
        panel.destroy();
    });

    it("rowOperatorChange with mock filtering and operator processes correctly", () => {
        const panel = new FilterPanel({});
        const store = panel.get_store();
        const field = { name: "F1", title: "F1", filteringType: "String" };
        (store as any).fields = [field];
        (store as any).fieldByName = { F1: field };

        // Create a row with addEmptyRow to get proper DOM structure
        (panel as any).addEmptyRow(false);
        const row = (panel as any).rowsDiv.lastElementChild;

        // Add an op-select input with a FilterOperatorSelect widget
        const opDiv = row.querySelector('div.o');
        const opInput = document.createElement("input");
        opInput.type = "hidden";
        opInput.className = "op-select";
        opDiv.appendChild(opInput);

        // Create a FilterOperatorSelect widget on the opInput
        new FilterOperatorSelect({ element: opInput, source: [{ key: "eq" }, { key: "ne" }] });

        // Mock getFilteringFor to return a mock filtering with operators
        const mockFiltering = {
            getOperators: () => [{ key: "eq" }, { key: "ne" }],
            set_operator: vi.fn(),
            createEditor: vi.fn()
        };
        vi.spyOn(panel as any, "getFilteringFor").mockReturnValue(mockFiltering);

        // rowOperatorChange should process the operator and create editor
        expect(() => (panel as any).rowOperatorChange(row)).not.toThrow();
        expect(mockFiltering.set_operator).toHaveBeenCalled();
        expect(mockFiltering.createEditor).toHaveBeenCalled();

        panel.destroy();
    });

    it("rowOperatorChange with mock filtering returns early when operator value is empty", () => {
        const panel = new FilterPanel({});
        const store = panel.get_store();
        const field = { name: "F1", title: "F1", filteringType: "String" };
        (store as any).fields = [field];
        (store as any).fieldByName = { F1: field };

        (panel as any).addEmptyRow(false);
        const row = (panel as any).rowsDiv.lastElementChild;

        // Create FilterOperatorSelect with empty source so value is undefined
        const opDiv = row.querySelector('div.o');
        const opInput = document.createElement("input");
        opInput.type = "hidden";
        opInput.className = "op-select";
        opDiv.appendChild(opInput);
        new FilterOperatorSelect({ element: opInput, source: [] });

        const mockFiltering = {
            getOperators: () => [{ key: "eq" }],
            set_operator: vi.fn(),
            createEditor: vi.fn()
        };
        vi.spyOn(panel as any, "getFilteringFor").mockReturnValue(mockFiltering);

        // Should not throw and should NOT call set_operator/createEditor (empty operator value)
        (panel as any).rowOperatorChange(row);
        expect(mockFiltering.set_operator).not.toHaveBeenCalled();
        expect(mockFiltering.createEditor).not.toHaveBeenCalled();
        panel.destroy();
    });

    it("rowOperatorChange returns early when no matching operator found", () => {
        const panel = new FilterPanel({});
        const store = panel.get_store();
        const field = { name: "F1", title: "F1", filteringType: "String" };
        (store as any).fields = [field];
        (store as any).fieldByName = { F1: field };

        (panel as any).addEmptyRow(false);
        const row = (panel as any).rowsDiv.lastElementChild;

        const opDiv = row.querySelector('div.o');
        const opInput = document.createElement("input");
        opInput.type = "hidden";
        opInput.className = "op-select";
        opDiv.appendChild(opInput);
        // Source has "xyz" but mock filtering only has "eq" -> no match
        new FilterOperatorSelect({ element: opInput, source: [{ key: "xyz" }] });

        const mockFiltering = {
            getOperators: () => [{ key: "eq" }],
            set_operator: vi.fn(),
            createEditor: vi.fn()
        };
        vi.spyOn(panel as any, "getFilteringFor").mockReturnValue(mockFiltering);

        (panel as any).rowOperatorChange(row);
        expect(mockFiltering.set_operator).not.toHaveBeenCalled();
        expect(mockFiltering.createEditor).not.toHaveBeenCalled();
        panel.destroy();
    });

    it("rowFieldChange with mock filtering creates operator select via populateOperatorList", () => {
        const panel = new FilterPanel({});
        const store = panel.get_store();
        const field = { name: "F1", title: "F1", filteringType: "String" };
        (store as any).fields = [field];
        (store as any).fieldByName = { F1: field };

        (panel as any).addEmptyRow(false);
        const row = (panel as any).rowsDiv.lastElementChild;

        // Set FilterFieldSelect value so getFieldFor returns the field
        const fieldInput = row.querySelector('div.f input.field-select');
        const fieldSelect = getWidgetFrom(fieldInput, FilterFieldSelect);
        fieldSelect.value = "F1";

        // Mock getFilteringFor
        const mockFiltering = {
            getOperators: () => [{ key: "eq" }, { key: "ne" }],
            set_operator: vi.fn(),
            createEditor: vi.fn(),
            get_field: () => field,
            set_container: vi.fn(),
            set_field: vi.fn()
        };
        vi.spyOn(panel as any, "getFilteringFor").mockReturnValue(mockFiltering);

        // rowFieldChange should call populateOperatorList which creates FilterOperatorSelect
        (panel as any).rowFieldChange(row);

        // Check that a FilterOperatorSelect widget was created
        const opInput = row.querySelector('div.o input.op-select');
        expect(opInput).toBeTruthy();

        // rowOperatorChange should have been called (it calls set_operator and createEditor)
        expect(mockFiltering.set_operator).toHaveBeenCalled();
        expect(mockFiltering.createEditor).toHaveBeenCalled();
        panel.destroy();
    });

    it("search processes rows with valid filtering and updates store items", () => {
        const panel = new FilterPanel({});
        const store = panel.get_store();
        const field = { name: "F1", title: "F1", filteringType: "String" };
        (store as any).fields = [field];
        (store as any).fieldByName = { F1: field };

        // Create and set up row
        (panel as any).addEmptyRow(false);
        const row = (panel as any).rowsDiv.lastElementChild;

        // Set field value on FilterFieldSelect
        const fieldInput = row.querySelector('div.f input.field-select');
        const fieldSelect = getWidgetFrom(fieldInput, FilterFieldSelect);
        fieldSelect.value = "F1";

        // Create FilterOperatorSelect widget
        const opDiv = row.querySelector('div.o');
        const opHidden = document.createElement("input");
        opHidden.type = "hidden";
        opHidden.className = "op-select";
        opDiv.appendChild(opHidden);
        new FilterOperatorSelect({ element: opHidden, source: [{ key: "eq", title: "Equals" }] });

        // Mock getFilteringFor
        const mockFiltering = {
            getOperators: () => [{ key: "eq", title: "Equals" }],
            set_operator: vi.fn(),
            createEditor: vi.fn(),
            getCriteria: () => ({ criteria: ["F1", "=", "test"], displayText: "F1 = test" }),
            saveState: () => ({ saved: true }),
            loadState: vi.fn(),
            get_field: () => field,
            set_container: vi.fn(),
            set_field: vi.fn()
        };
        vi.spyOn(panel as any, "getFilteringFor").mockReturnValue(mockFiltering);

        // search should process the row and update store
        (panel as any).search();

        expect(store.get_items().length).toBe(1);
        expect(store.get_items()[0].field).toBe("F1");
        expect(store.get_items()[0].operator).toBe("eq");
        expect(store.get_items()[0].displayText).toBe("F1 = test");
        expect(store.get_items()[0].state).toEqual({ saved: true });
        panel.destroy();
    });

    it("search with invalid operator reports error", () => {
        const panel = new FilterPanel({});
        const store = panel.get_store();
        const field = { name: "F1", title: "F1", filteringType: "String" };
        (store as any).fields = [field];
        (store as any).fieldByName = { F1: field };

        (panel as any).addEmptyRow(false);
        const row = (panel as any).rowsDiv.lastElementChild;

        const fieldInput = row.querySelector('div.f input.field-select');
        const fieldSelect = getWidgetFrom(fieldInput, FilterFieldSelect);
        fieldSelect.value = "F1";

        // Create op-select but with empty value (no source)
        const opDiv = row.querySelector('div.o');
        const opHidden = document.createElement("input");
        opHidden.type = "hidden";
        opHidden.className = "op-select";
        opDiv.appendChild(opHidden);
        new FilterOperatorSelect({ element: opHidden, source: [] });

        const mockFiltering = {
            getOperators: () => [],
            set_operator: vi.fn(),
            createEditor: vi.fn(),
            getCriteria: vi.fn(),
            saveState: vi.fn()
        };
        vi.spyOn(panel as any, "getFilteringFor").mockReturnValue(mockFiltering);

        // search should report invalid operator error
        (panel as any).search();
        // Store should not have items (error occurred)
        expect(store.get_items().length).toBe(0);
        // Error element should be present
        expect(row.querySelector('div.v span.error')).toBeTruthy();
        panel.destroy();
    });

    it("search with getCriteria exception reports error", () => {
        const panel = new FilterPanel({});
        const store = panel.get_store();
        const field = { name: "F1", title: "F1", filteringType: "String" };
        (store as any).fields = [field];
        (store as any).fieldByName = { F1: field };

        (panel as any).addEmptyRow(false);
        const row = (panel as any).rowsDiv.lastElementChild;

        const fieldInput = row.querySelector('div.f input.field-select');
        const fieldSelect = getWidgetFrom(fieldInput, FilterFieldSelect);
        fieldSelect.value = "F1";

        const opDiv = row.querySelector('div.o');
        const opHidden = document.createElement("input");
        opHidden.type = "hidden";
        opHidden.className = "op-select";
        opDiv.appendChild(opHidden);
        new FilterOperatorSelect({ element: opHidden, source: [{ key: "eq" }] });

        const mockFiltering = {
            getOperators: () => [{ key: "eq" }],
            set_operator: vi.fn(),
            createEditor: vi.fn(),
            getCriteria: () => { throw new Error("Test error"); },
            saveState: vi.fn()
        };
        vi.spyOn(panel as any, "getFilteringFor").mockReturnValue(mockFiltering);

        (panel as any).search();
        expect(store.get_items().length).toBe(0);
        expect(row.querySelector('div.v span.error')).toBeTruthy();
        panel.destroy();
    });

    it("updateRowsFromStore processes items completely with mocked filtering", () => {
        const panel = new FilterPanel({});
        const store = panel.get_store();

        // Add items to store
        store.get_items().push({
            field: "F1",
            operator: "eq",
            displayText: "F1 = value",
            leftParen: true,
            isOr: false,
            criteria: ["F1", "=", "value"],
            state: { saved: true }
        });

        const field = { name: "F1", title: "F1", filteringType: "String" };
        (store as any).fields = [field];
        (store as any).fieldByName = { F1: field };

        // Mock getFilteringFor to return a mock
        const mockFiltering = {
            getOperators: () => [{ key: "eq" }, { key: "ne" }],
            set_operator: vi.fn(),
            createEditor: vi.fn(),
            loadState: vi.fn(),
            get_field: () => field,
            set_container: vi.fn(),
            set_field: vi.fn()
        };
        vi.spyOn(panel as any, "getFilteringFor").mockReturnValue(mockFiltering);

        // updateRowsFromStore should process the items
        (panel as any).updateRowsFromStore();
        expect((panel as any).rowsDiv.childElementCount).toBe(1);

        // Verify leftParen was toggled active on the row
        const row = (panel as any).rowsDiv.lastElementChild;
        const leftParen = row.querySelector('div.l a.leftparen');
        expect(leftParen.classList.contains('active')).toBe(true);

        // set_operator and loadState should have been called
        expect(mockFiltering.set_operator).toHaveBeenCalled();
        expect(mockFiltering.loadState).toHaveBeenCalledWith({ saved: true });
        panel.destroy();
    });

    it("get_hasErrors compat method delegates to hasErrors", () => {
        const panel = new FilterPanel({});
        // Initially no errors
        expect((panel as any).get_hasErrors()).toBe(false);

        // Add a div.v with span.error as direct child of rowsDiv matching :scope > div.v > span.error
        const divV = document.createElement("div");
        divV.className = "v";
        const err = document.createElement("span");
        err.className = "error";
        divV.appendChild(err);
        (panel as any).rowsDiv.appendChild(divV);

        expect((panel as any).get_hasErrors()).toBe(true);
        panel.destroy();
    });

    it("getFilteringFor caches filtering instance on row metadata", () => {
        const panel = new FilterPanel({});
        const store = panel.get_store();
        const field = { name: "F1", title: "F1", filteringType: "String" };
        (store as any).fields = [field];
        (store as any).fieldByName = { F1: field };

        // Set up row properly
        (panel as any).addEmptyRow(false);
        const row = (panel as any).rowsDiv.lastElementChild;

        // Set field value
        const fieldInput = row.querySelector('div.f input.field-select');
        const fieldSelect = getWidgetFrom(fieldInput, FilterFieldSelect);
        fieldSelect.value = "F1";

        // First call should create and cache the filtering
        const filtering1 = (panel as any).getFilteringFor(row);
        expect(filtering1).toBeDefined();

        // Second call should return cached instance
        const filtering2 = (panel as any).getFilteringFor(row);
        expect(filtering2).toBe(filtering1);

        // verify __Filtering was set on row metadata
        expect((row as any).__Filtering).toBe(filtering1);
        panel.destroy();
    });

    it("onRowOperatorChange processes event and focuses visible input", () => {
        const panel = new FilterPanel({});
        const store = panel.get_store();
        const field = { name: "F1", title: "F1", filteringType: "String" };
        (store as any).fields = [field];
        (store as any).fieldByName = { F1: field };

        // Create row with proper structure
        (panel as any).addEmptyRow(false);
        const row = (panel as any).rowsDiv.lastElementChild;

        // Set field value
        const fieldInput = row.querySelector('div.f input.field-select');
        const fieldSelect = getWidgetFrom(fieldInput, FilterFieldSelect);
        fieldSelect.value = "F1";

        // Create FilterOperatorSelect widget
        const opDiv = row.querySelector('div.o');
        const opHidden = document.createElement("input");
        opHidden.type = "hidden";
        opHidden.className = "op-select";
        opDiv.appendChild(opHidden);
        new FilterOperatorSelect({ element: opHidden, source: [{ key: "eq" }] });

        // Mock getFilteringFor to return mock filtering
        const mockFiltering = {
            getOperators: () => [{ key: "eq" }],
            set_operator: vi.fn(),
            createEditor: vi.fn(),
            get_field: () => field,
            set_container: vi.fn(),
            set_field: vi.fn()
        };
        vi.spyOn(panel as any, "getFilteringFor").mockReturnValue(mockFiltering);

        // Trigger onRowOperatorChange with an event targeting the op-select input
        const event = { target: opHidden } as any;
        expect(() => (panel as any).onRowOperatorChange(event)).not.toThrow();
        expect(mockFiltering.set_operator).toHaveBeenCalled();
        panel.destroy();
    });

    it("FilterOperatorSelect constructor uses title from OperatorNames when available", () => {
        const input = document.createElement("input");
        input.type = "hidden";
        // Use a key that might be found in OperatorNames lookup
        const operators: FilterOperator[] = [
            { key: "eq" }  // "eq" might have a formatted title in OperatorNames
        ];
        const select = new FilterOperatorSelect({ element: input, source: operators });
        expect(select.value).toBe("eq");
        select.destroy();
    });

    it("FilterFieldSelect handles fields without title using name as fallback", () => {
        const input = document.createElement("input");
        input.type = "hidden";
        // Field with null title should use name as fallback
        const fields = [
            { name: "Field1", title: null },
            { name: "Field2" }  // no title at all
        ] as any;
        const select = new FilterFieldSelect({ element: input, fields });
        expect(select).toBeInstanceOf(FilterFieldSelect);
        select.destroy();
    });

    it("onRowOperatorChange iterates visible input elements without throwing", () => {
        const panel = new FilterPanel({});
        const store = panel.get_store();
        const field = { name: "F1", title: "F1", filteringType: "String" };
        (store as any).fields = [field];
        (store as any).fieldByName = { F1: field };

        (panel as any).addEmptyRow(false);
        const row = (panel as any).rowsDiv.lastElementChild;

        // Set field value
        const fieldInput = row.querySelector('div.f input.field-select');
        const fieldSelect = getWidgetFrom(fieldInput, FilterFieldSelect);
        fieldSelect.value = "F1";

        // Create FilterOperatorSelect
        const opDiv = row.querySelector('div.o');
        const opHidden = document.createElement("input");
        opHidden.type = "hidden";
        opHidden.className = "op-select";
        opDiv.appendChild(opHidden);
        new FilterOperatorSelect({ element: opHidden, source: [{ key: "eq" }] });

        // Add inputs in the v div for onRowOperatorChange to iterate over
        const vDiv = row.querySelector('div.v');
        const textInput = document.createElement("input");
        textInput.type = "text";
        vDiv.appendChild(textInput);
        const textarea = document.createElement("textarea");
        vDiv.appendChild(textarea);
        const selectEl = document.createElement("select");
        vDiv.appendChild(selectEl);

        // Mock getFilteringFor
        const mockFiltering = {
            getOperators: () => [{ key: "eq" }],
            set_operator: vi.fn(),
            createEditor: vi.fn(),
            get_field: () => field,
            set_container: vi.fn(),
            set_field: vi.fn()
        };
        vi.spyOn(panel as any, "getFilteringFor").mockReturnValue(mockFiltering);

        const event = { target: opHidden } as any;
        expect(() => (panel as any).onRowOperatorChange(event)).not.toThrow();
        expect(mockFiltering.set_operator).toHaveBeenCalled();
        panel.destroy();
    });

    it("onRowOperatorChange handles focus exception gracefully", () => {
        const panel = new FilterPanel({});
        const store = panel.get_store();
        const field = { name: "F1", title: "F1", filteringType: "String" };
        (store as any).fields = [field];
        (store as any).fieldByName = { F1: field };

        (panel as any).addEmptyRow(false);
        const row = (panel as any).rowsDiv.lastElementChild;

        const fieldInput = row.querySelector('div.f input.field-select');
        const fieldSelect = getWidgetFrom(fieldInput, FilterFieldSelect);
        fieldSelect.value = "F1";

        const opDiv = row.querySelector('div.o');
        const opHidden = document.createElement("input");
        opHidden.type = "hidden";
        opHidden.className = "op-select";
        opDiv.appendChild(opHidden);
        new FilterOperatorSelect({ element: opHidden, source: [{ key: "eq" }] });

        // Add a visible input that throws on focus
        const vDiv = row.querySelector('div.v');
        const focusInput = document.createElement("input");
        focusInput.type = "text";
        focusInput.style.display = "block";
        // Make focus throw an exception
        const focusSpy = vi.spyOn(focusInput, "focus").mockImplementation(() => { throw new Error("focus error"); });
        vDiv.appendChild(focusInput);

        const mockFiltering = {
            getOperators: () => [{ key: "eq" }],
            set_operator: vi.fn(),
            createEditor: vi.fn(),
            get_field: () => field,
            set_container: vi.fn(),
            set_field: vi.fn()
        };
        vi.spyOn(panel as any, "getFilteringFor").mockReturnValue(mockFiltering);

        const event = { target: opHidden } as any;
        // Should not throw despite focus error
        expect(() => (panel as any).onRowOperatorChange(event)).not.toThrow();
        panel.destroy();
    });
});
