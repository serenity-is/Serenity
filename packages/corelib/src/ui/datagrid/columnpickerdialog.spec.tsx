import { ColumnPickerDialogTexts, Fluent } from "../../base";
import { ColumnPickerDialog } from "./columnpickerdialog";

afterEach(() => {
    Fluent(document.body).empty();
});

describe("ColumnPickerDialog", () => {
    it("should show all columns in the sleekGrid argument by calling getAllColumns", () => {
        const dialog = new ColumnPickerDialog({
            sleekGrid: {
                getAllColumns: () => [
                    { id: "A", name: "Column A" },
                    { id: "B", name: "Column B" },
                    { id: "C", name: "Column C" }
                ]
            } as any
        });

        dialog.dialogOpen();
        const shownKeys = dialog.element.findAll("li[data-key]").map(li => li.dataset.key);
        expect(shownKeys).toEqual(["A", "B", "C"]);
    });

    it("should show all columns in the dataGrid by calling getGrid().getAllColumns", () => {
        const dialog = new ColumnPickerDialog({
            dataGrid: {
                getGrid: () => ({
                    getAllColumns: () => [
                        { id: "X", name: "Column X" },
                        { id: "Y", name: "Column Y" },
                        { id: "Z", name: "Column Z" }
                    ]
                })
            } as any
        });

        dialog.dialogOpen();
        const shownKeys = dialog.element.findAll("li[data-key]").map(li => li.dataset.key);
        expect(shownKeys).toEqual(["X", "Y", "Z"]);
    });

    it("should show all columns passed via columns argument as an array", () => {
        const dialog = new ColumnPickerDialog({
            columns: [
                { id: "L", name: "Column L" },
                { id: "M", name: "Column M" },
                { id: "N", name: "Column N" }
            ]
        });

        dialog.dialogOpen();
        const shownKeys = dialog.element.findAll("li[data-key]").map(li => li.dataset.key);
        expect(shownKeys).toEqual(["L", "M", "N"]);
    });

    it("should show all columns passed via columns argument as a function", () => {
        const dialog = new ColumnPickerDialog({
            columns: () => [
                { id: "D", name: "Column D" },
                { id: "E", name: "Column E" },
                { id: "F", name: "Column F" }
            ]
        });

        dialog.dialogOpen();
        const shownKeys = dialog.element.findAll("li[data-key]").map(li => li.dataset.key);
        expect(shownKeys).toEqual(["D", "E", "F"]);
    });

    it("should show unchecked inputs for hidden columns (visible: false) and checked inputs for shown columns (visible: true or null)", () => {
        const dialog = new ColumnPickerDialog({
            columns: [
                { id: "1", name: "Column 1", visible: true },
                { id: "2", name: "Column 2", visible: false },
                { id: "3", name: "Column 3" },
                { id: "4", name: "Column 4", visible: false }
            ]
        });

        dialog.dialogOpen();
        const inputs = dialog.element.findAll<HTMLInputElement>("li[data-key] input");
        expect(inputs.length).toBe(4);
        expect(inputs[0].checked).toBe(true);
        expect(inputs[1].checked).toBe(false);
        expect(inputs[2].checked).toBe(true);
        expect(inputs[3].checked).toBe(false);
    });

    it("should filter columns based on search query", async () => {
        const dialog = new ColumnPickerDialog({
            columns: [
                { id: "A", name: "Apple Column" },
                { id: "B", name: "Banana Column" },
                { id: "C", name: "Cherry Column" }
            ]
        });

        dialog.dialogOpen();
        const searchInput = dialog.element.findAll<HTMLInputElement>(".s-QuickSearchBar input")[0];
        expect(searchInput).toBeTruthy();

        // Initially all should be visible
        let listItems = dialog.element.findAll("li[data-key]");
        expect(listItems.length).toBe(3);
        expect(listItems.every(li => !li.hidden)).toBe(true);

        // Search for "Apple"
        searchInput.value = "Apple";
        searchInput.dispatchEvent(new Event("execute-search"));

        await new Promise(resolve => setTimeout(resolve, 0));

        listItems = dialog.element.findAll("li[data-key]");
        expect(listItems.filter(li => !li.hidden).length).toBe(1);
        expect(listItems.find(li => li.dataset.key === "A" && !li.hidden)).toBeTruthy();
        expect(listItems.find(li => li.dataset.key === "B" && li.hidden)).toBeTruthy();
        expect(listItems.find(li => li.dataset.key === "C" && li.hidden)).toBeTruthy();
    });

    it("should toggle all visible columns when toggle all checkbox is clicked", () => {
        const dialog = new ColumnPickerDialog({
            columns: [
                { id: "A", name: "Column A", visible: true },
                { id: "B", name: "Column B", visible: false },
                { id: "C", name: "Column C", visible: true },
                { id: "D", name: "Column D", visible: true, togglable: false }
            ]
        });

        dialog.dialogOpen();
        const toggleAllCheckbox = dialog.element.findAll<HTMLInputElement>("#" + dialog.uniqueName + "_ToggleAll")[0];
        expect(toggleAllCheckbox).toBeTruthy();

        // Initially, toggle all should be unchecked because column B is hidden (unchecked)
        expect(toggleAllCheckbox.checked).toBe(false);

        // Check toggle all
        toggleAllCheckbox.click();
        expect(toggleAllCheckbox.checked).toBe(true);

        // Check that togglable columns are all checked
        const inputs = dialog.element.findAll<HTMLInputElement>("li[data-key] input.toggle-visibility");
        expect(inputs[0].checked).toBe(true); // A
        expect(inputs[1].checked).toBe(true); // B
        expect(inputs[2].checked).toBe(true); // C
        expect(inputs[3].checked).toBe(true);  // D (not togglable, should stay true)

        // Check toggle all again
        toggleAllCheckbox.click();
        expect(toggleAllCheckbox.checked).toBe(false);

        // All visible togglable columns should be checked
        expect(inputs[0].checked).toBe(false); // A
        expect(inputs[1].checked).toBe(false); // B
        expect(inputs[2].checked).toBe(false); // C
        expect(inputs[3].checked).toBe(true); // D (not togglable, should stay true)
    });

    it("should only toggle filtered columns when toggle all checkbox is clicked", async () => {
        const dialog = new ColumnPickerDialog({
            columns: [
                { id: "A", name: "Apple", visible: true },
                { id: "B", name: "Banana", visible: false },
                { id: "C", name: "Cherry", visible: true },
                { id: "D", name: "Dummy", visible: true }
            ]
        });

        dialog.dialogOpen();
        const searchInput = dialog.element.findAll<HTMLInputElement>(".s-QuickSearchBar input")[0];
        const toggleAllCheckbox = dialog.element.findAll<HTMLInputElement>("#" + dialog.uniqueName + "_ToggleAll")[0];

        // Search for "A" to filter only Apple and Banana
        searchInput.value = "A";
        searchInput.dispatchEvent(new Event("execute-search"));

        await new Promise(resolve => setTimeout(resolve, 0));

        // Check toggle all
        toggleAllCheckbox.click();
        expect(toggleAllCheckbox.checked).toBe(true);

        // Check that only filtered columns are toggled
        const inputs = dialog.element.findAll<HTMLInputElement>("li[data-key] input.toggle-visibility");
        expect(inputs[0].checked).toBe(true); // A
        expect(inputs[1].checked).toBe(true); // B
        expect(inputs[2].checked).toBe(true); // C (not filtered, should stay true)
        expect(inputs[3].checked).toBe(true); // D (not filtered, should stay true)

        // Uncheck toggle all
        toggleAllCheckbox.click();
        expect(toggleAllCheckbox.checked).toBe(false);

        // Check that only filtered columns are toggled
        expect(inputs[0].checked).toBe(false); // A
        expect(inputs[1].checked).toBe(false); // B
        expect(inputs[2].checked).toBe(true);  // C (not filtered, should stay true)
        expect(inputs[3].checked).toBe(true);  // D (not filtered, should stay true)
    });

    it("should restore defaults when restore defaults button is clicked", () => {
        const dialog = new ColumnPickerDialog({
            columns: [
                { id: "A", name: "Column A", visible: true },
                { id: "B", name: "Column B", visible: false },
                { id: "C", name: "Column C", visible: true },
                { id: "D", name: "Column D", visible: false }
            ],
            defaultOrder: ["D", "C", "B", "A"],
            defaultVisible: ["D", "C"]
        });

        dialog.dialogOpen();

        // Initially, columns should be in original order
        let listItems = dialog.element.findAll("li[data-key]");
        expect(listItems.map(li => li.dataset.key)).toEqual(["A", "B", "C", "D"]);

        // Initially, visibility should match column.visible
        let inputs = dialog.element.findAll<HTMLInputElement>("li[data-key] input.toggle-visibility");
        expect(inputs[0].checked).toBe(true);  // A
        expect(inputs[1].checked).toBe(false); // B
        expect(inputs[2].checked).toBe(true);  // C
        expect(inputs[3].checked).toBe(false); // D

        // Click restore defaults button
        const restoreButton = dialog.element.findAll("button#" + dialog.uniqueName + "_RestoreDefaults")[0];
        expect(restoreButton).toBeTruthy();
        restoreButton.click();

        // Columns should be reordered to default order
        listItems = dialog.element.findAll("li[data-key]");
        expect(listItems.map(li => li.dataset.key)).toEqual(["D", "C", "B", "A"]);

        // Visibility should match defaultVisible
        inputs = dialog.element.findAll<HTMLInputElement>("li[data-key] input.toggle-visibility");
        expect(inputs[0].checked).toBe(true);  // D
        expect(inputs[1].checked).toBe(true);  // C
        expect(inputs[2].checked).toBe(false); // B
        expect(inputs[3].checked).toBe(false); // A
    });

    it("should toggle individual column visibility when checkbox is clicked", () => {
        let toggledColumnId: string | undefined;

        const dialog = new ColumnPickerDialog({
            columns: [
                { id: "A", name: "Column A", visible: true },
                { id: "B", name: "Column B", visible: false },
                { id: "C", name: "Column C", visible: true }
            ],
            toggleColumn: (columnId: string, show?: boolean) => {
                toggledColumnId = columnId;
                // Update the column's visible state (simulate the default behavior)
                const column = dialog['colById'][columnId];
                if (column) {
                    column.visible = show ?? (column.visible === false);
                }
            }
        });

        dialog.dialogOpen();

        const inputs = dialog.element.findAll<HTMLInputElement>("li[data-key] input.toggle-visibility");

        // Initially
        expect(inputs[0].checked).toBe(true);  // A
        expect(inputs[1].checked).toBe(false); // B
        expect(inputs[2].checked).toBe(true);  // C

        // Click on column A's checkbox to hide it
        inputs[0].click();
        expect(toggledColumnId).toBe("A");
        expect(dialog['colById']['A'].visible).toBe(false);

        // Click on column B's checkbox to show it
        inputs[1].click();
        expect(toggledColumnId).toBe("B");
        expect(dialog['colById']['B'].visible).toBe(true);
    });

    it("should disable checkboxes for non-togglable columns", () => {
        const dialog = new ColumnPickerDialog({
            columns: [
                { id: "A", name: "Column A", visible: true, togglable: true },
                { id: "B", name: "Column B", visible: false, togglable: false },
                { id: "C", name: "Column C", visible: true }, // default togglable: true
                { id: "D", name: "Column D", visible: false, togglable: false }
            ]
        });

        dialog.dialogOpen();

        const inputs = dialog.element.findAll<HTMLInputElement>("li[data-key] input.toggle-visibility");

        // Check that togglable columns are enabled, non-togglable are disabled
        expect(inputs[0].disabled).toBe(false); // A (togglable: true)
        expect(inputs[1].disabled).toBe(true);  // B (togglable: false)
        expect(inputs[2].disabled).toBe(false); // C (default true)
        expect(inputs[3].disabled).toBe(true);  // D (togglable: false)

        // Check that non-togglable columns have correct checked state
        expect(inputs[0].checked).toBe(true);  // A
        expect(inputs[1].checked).toBe(false); // B
        expect(inputs[2].checked).toBe(true);  // C
        expect(inputs[3].checked).toBe(false); // D
    });

    it("should hide drag handles for non-movable columns", () => {
        const dialog = new ColumnPickerDialog({
            columns: [
                { id: "A", name: "Column A", movable: true },
                { id: "B", name: "Column B", movable: false },
                { id: "C", name: "Column C" }, // default movable: true
                { id: "D", name: "Column D", movable: false }
            ]
        });

        dialog.dialogOpen();

        const dragHandles = dialog.element.findAll("li[data-key] .drag-handle");

        // Check that drag handles are visible for movable columns, hidden for non-movable
        expect(dragHandles[0].style.visibility).toBe(""); // A (movable: true) - visible (no style set)
        expect(dragHandles[1].style.visibility).toBe("hidden"); // B (movable: false) - hidden
        expect(dragHandles[2].style.visibility).toBe(""); // C (default true) - visible (no style set)
        expect(dragHandles[3].style.visibility).toBe("hidden"); // D (movable: false) - hidden
    });

    it("should show pin indicators for pinned columns", () => {
        const mockGrid = {
            getOptions: () => ({ rtl: false }),
            getLayoutInfo: () => ({ supportPinnedCols: true, supportPinnedEnd: true })
        };

        const dialog = new ColumnPickerDialog({
            columns: [
                { id: "A", name: "Column A" },
                { id: "B", name: "Column B", frozen: true },
                { id: "C", name: "Column C", frozen: "end" },
                { id: "D", name: "Column D", frozen: true }
            ],
            sleekGrid: mockGrid as any
        });

        dialog.dialogOpen();

        const listItems = dialog.element.findAll("li[data-key]");

        // Check that pin indicators are present for pinned columns
        const pinIcons = listItems.map(li => li.querySelector("svg"));
        expect(pinIcons[0]).toBeNull(); // A - not pinned
        expect(pinIcons[1]).toBeTruthy(); // B - pinned left
        expect(pinIcons[2]).toBeTruthy(); // C - pinned right (rotated)
        expect(pinIcons[3]).toBeTruthy(); // D - pinned left

        // Check pin icon rotation for right-pinned columns
        expect(pinIcons[2]?.style.transform).toBe("rotate(270deg)"); // C - pinned right
        expect(pinIcons[1]?.style.transform).toBe(""); // B - pinned left (no rotation)
        expect(pinIcons[3]?.style.transform).toBe(""); // D - pinned left (no rotation)
    });

    it("should create tool button with correct properties", () => {
        const mockDataGrid = {
            element: document.createElement("div"),
            uniqueName: "testGrid"
        };

        const toolButton = ColumnPickerDialog.createToolButton(mockDataGrid as any);

        expect(toolButton).toBeDefined();
        expect(toolButton.hint).toBe(ColumnPickerDialogTexts.Title);
        expect(toolButton.action).toBe('column-picker');
        expect(toolButton.cssClass).toBe("column-picker-button");
        expect(toolButton.icon).toBeDefined();
        expect(typeof toolButton.onClick).toBe("function");

        // Test that onClick opens the dialog
        let dialogOpened = false;
        const originalOpenDialog = ColumnPickerDialog.openDialog;
        ColumnPickerDialog.openDialog = () => { dialogOpened = true; };

        toolButton.onClick({} as any);
        expect(dialogOpened).toBe(true);

        // Restore original method
        ColumnPickerDialog.openDialog = originalOpenDialog;
    });
});