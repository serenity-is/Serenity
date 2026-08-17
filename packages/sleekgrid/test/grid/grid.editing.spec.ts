import type { Editor, EditorClass, EditorOptions, ValidationResult } from "../../src/core/editing";
import type { Column } from "../../src/core";
import { SleekGrid } from "../../src/grid";

interface EditRow {
    value: string;
}

class TestEditor implements Editor {
    static validation: ValidationResult = { valid: true };
    readonly input: HTMLInputElement;
    private originalValue = "";
    private item: EditRow;

    constructor(options: EditorOptions) {
        this.input = document.createElement("input");
        options.container?.append(this.input);
    }

    destroy(): void {
        this.input.remove();
    }

    loadValue(item: EditRow): void {
        this.item = item;
        this.originalValue = item.value;
        this.input.value = item.value;
    }

    serializeValue(): string {
        return this.input.value;
    }

    applyValue(item: EditRow, value: string): void {
        item.value = value;
    }

    isValueChanged(): boolean {
        return this.input.value !== this.originalValue;
    }

    validate(): ValidationResult {
        return TestEditor.validation;
    }

    focus(): void {
        this.input.focus();
    }
}

const editor = TestEditor as unknown as EditorClass;
const columns: Column<EditRow>[] = [{ id: "value", field: "value", editor }];

function createGrid(data: EditRow[], options: Record<string, unknown> = {}) {
    const container = document.createElement("div");
    container.style.height = "300px";
    document.body.append(container);
    const grid = new SleekGrid(container, data, columns.map(column => ({ ...column })), {
        editable: true,
        renderAllRows: true,
        renderAllCells: true,
        ...options
    });
    return { container, grid };
}

describe("SleekGrid editing", () => {
    const grids: SleekGrid<EditRow>[] = [];

    beforeEach(() => {
        TestEditor.validation = { valid: true };
    });

    afterEach(() => {
        while (grids.length)
            grids.pop()?.destroy();
    });

    it("enters edit mode through setActiveCell and editActiveCell", () => {
        const data = [{ value: "before" }];
        const { grid } = createGrid(data);
        grids.push(grid);
        const beforeEdit = vi.fn();
        grid.onBeforeEditCell.subscribe(beforeEdit);

        grid.setActiveCell(0, 0);
        grid.editActiveCell();

        expect(grid.getActiveCell()).toEqual({ row: 0, cell: 0 });
        expect(grid.getCellEditor()).toBeInstanceOf(TestEditor);
        expect(beforeEdit).toHaveBeenCalledOnce();
        expect(grid.getEditorLock().isActive()).toBe(true);
    });

    it("commits changed values and emits cell change", () => {
        const data = [{ value: "before" }];
        const { grid } = createGrid(data);
        grids.push(grid);
        const changed = vi.fn();
        grid.onCellChange.subscribe(changed);
        grid.setActiveCell(0, 0);
        grid.editActiveCell();
        (grid.getCellEditor() as TestEditor).input.value = "after";

        expect(grid.commitCurrentEdit()).toBe(true);
        expect(data[0].value).toBe("after");
        expect(changed).toHaveBeenCalledOnce();
        expect(grid.getCellEditor()).toBeNull();
        expect(grid.getEditorLock().isActive()).toBe(false);
    });

    it("cancels edits without changing the item", () => {
        const data = [{ value: "before" }];
        const { grid } = createGrid(data);
        grids.push(grid);
        const destroyed = vi.fn();
        grid.onBeforeCellEditorDestroy.subscribe(destroyed);
        grid.setActiveCell(0, 0);
        grid.editActiveCell();
        (grid.getCellEditor() as TestEditor).input.value = "discarded";

        expect(grid.cancelCurrentEdit()).toBe(true);
        expect(data[0].value).toBe("before");
        expect(destroyed).toHaveBeenCalledOnce();
        expect(grid.getCellEditor()).toBeNull();
    });

    it("keeps invalid edits active and reports validation errors", () => {
        const data = [{ value: "before" }];
        const { grid } = createGrid(data);
        grids.push(grid);
        const validationError = vi.fn();
        grid.onValidationError.subscribe(validationError);
        TestEditor.validation = { valid: false, msg: "invalid" };
        grid.setActiveCell(0, 0);
        grid.editActiveCell();
        (grid.getCellEditor() as TestEditor).input.value = "bad";

        expect(grid.commitCurrentEdit()).toBe(false);
        expect(data[0].value).toBe("before");
        expect(validationError).toHaveBeenCalledOnce();
        expect(grid.getCellEditor()).toBeInstanceOf(TestEditor);
        expect(grid.getEditorLock().isActive()).toBe(true);
    });

    it("does not enter editing when the grid is not editable", () => {
        const { grid } = createGrid([{ value: "before" }], { editable: false });
        grids.push(grid);
        grid.setActiveCell(0, 0);

        expect(() => grid.editActiveCell()).toThrow();
    });
});
