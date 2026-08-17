import type { Column } from "../../src/core";
import { SleekGrid } from "../../src/grid";

interface EventRow {
    value: string;
}

const columns: Column<EventRow>[] = [{ id: "value", field: "value", name: "Value" }];

function createGrid(options: Record<string, unknown> = {}) {
    const container = document.createElement("div");
    container.style.height = "300px";
    document.body.append(container);
    const grid = new SleekGrid(container, [{ value: "one" }], columns.map(column => ({ ...column })), {
        renderAllRows: true,
        renderAllCells: true,
        ...options
    });
    return { container, grid };
}

describe("SleekGrid events and interaction", () => {
    const grids: SleekGrid<EventRow>[] = [];

    afterEach(() => {
        while (grids.length)
            grids.pop()?.destroy();
    });

    it("forwards cell click, double-click, and context-menu events", () => {
        const { grid } = createGrid();
        grids.push(grid);
        const cell = grid.getCellNode(0, 0);
        const click = vi.fn();
        const dblClick = vi.fn();
        const contextMenu = vi.fn();
        grid.onClick.subscribe(click);
        grid.onDblClick.subscribe(dblClick);
        grid.onContextMenu.subscribe(contextMenu);

        cell.dispatchEvent(new MouseEvent("click", { bubbles: true }));
        cell.dispatchEvent(new MouseEvent("dblclick", { bubbles: true }));
        cell.dispatchEvent(new MouseEvent("contextmenu", { bubbles: true }));

        expect(click).toHaveBeenCalledWith(expect.anything(), expect.objectContaining({ row: 0, cell: 0 }));
        expect(dblClick).toHaveBeenCalledWith(expect.anything(), expect.objectContaining({ row: 0, cell: 0 }));
        expect(contextMenu).toHaveBeenCalledWith(expect.anything(), expect.objectContaining({ grid }));
    });

    it("forwards header click, context-menu, and mouse boundary events", () => {
        const { grid } = createGrid();
        grids.push(grid);
        const header = grid.getHeaderColumn(0);
        const headerClick = vi.fn();
        const headerContext = vi.fn();
        const enter = vi.fn();
        const leave = vi.fn();
        grid.onHeaderClick.subscribe(headerClick);
        grid.onHeaderContextMenu.subscribe(headerContext);
        grid.onHeaderMouseEnter.subscribe(enter);
        grid.onHeaderMouseLeave.subscribe(leave);

        header.dispatchEvent(new MouseEvent("click", { bubbles: true }));
        header.dispatchEvent(new MouseEvent("contextmenu", { bubbles: true }));
        header.dispatchEvent(new MouseEvent("mouseenter", { bubbles: true }));
        header.dispatchEvent(new MouseEvent("mouseleave", { bubbles: true }));

        expect(headerClick).toHaveBeenCalledWith(expect.anything(), expect.objectContaining({ column: grid.getColumns()[0] }));
        expect(headerContext).toHaveBeenCalledWith(expect.anything(), expect.objectContaining({ column: grid.getColumns()[0] }));
        expect(enter).toHaveBeenCalledWith(expect.anything(), expect.objectContaining({ column: grid.getColumns()[0] }));
        expect(leave).toHaveBeenCalledWith(expect.anything(), expect.objectContaining({ column: grid.getColumns()[0] }));
    });

    it("forwards cell mouse enter and leave events", () => {
        const { grid } = createGrid();
        grids.push(grid);
        const cell = grid.getCellNode(0, 0);
        const enter = vi.fn();
        const leave = vi.fn();
        grid.onMouseEnter.subscribe(enter);
        grid.onMouseLeave.subscribe(leave);

        cell.dispatchEvent(new MouseEvent("mouseenter", { bubbles: true }));
        cell.dispatchEvent(new MouseEvent("mouseleave", { bubbles: true }));

        expect(enter).toHaveBeenCalledWith(expect.anything(), expect.objectContaining({ grid }));
        expect(leave).toHaveBeenCalledWith(expect.anything(), expect.objectContaining({ grid }));
    });

    it("forwards keyboard events and applies navigation cancellation", () => {
        const { grid } = createGrid();
        grids.push(grid);
        const cell = grid.getCellNode(0, 0);
        const keydown = vi.fn();
        grid.onKeyDown.subscribe(keydown);
        grid.setActiveCell(0, 0);
        const event = new KeyboardEvent("keydown", { key: "PageDown", bubbles: true, cancelable: true });

        cell.dispatchEvent(event);

        expect(keydown).toHaveBeenCalledWith(expect.anything(), expect.objectContaining({ row: 0, cell: 0 }));
        expect(event.defaultPrevented).toBe(true);
    });

    it("stops click processing when a subscriber stops immediate propagation", () => {
        const { grid } = createGrid();
        grids.push(grid);
        const cell = grid.getCellNode(0, 0);
        const click = vi.fn((event: { stopImmediatePropagation: () => void }) => event.stopImmediatePropagation());
        grid.onClick.subscribe(click);
        grid.setActiveCell(0, 0);
        cell.dispatchEvent(new MouseEvent("click", { bubbles: true }));

        expect(click).toHaveBeenCalledOnce();
        expect(grid.getActiveCell()).toEqual({ row: 0, cell: 0 });
    });

    it("routes handled keyboard commands to their navigation methods", () => {
        const { grid } = createGrid();
        grids.push(grid);
        const cell = grid.getCellNode(0, 0);
        grid.setActiveCell(0, 0);
        const methods = {
            navigateTop: vi.spyOn(grid, "navigateTop"),
            navigateRowStart: vi.spyOn(grid, "navigateRowStart").mockReturnValue(true),
            navigateBottom: vi.spyOn(grid, "navigateBottom"),
            navigateRowEnd: vi.spyOn(grid, "navigateRowEnd").mockReturnValue(true),
            navigatePageDown: vi.spyOn(grid, "navigatePageDown"),
            navigatePageUp: vi.spyOn(grid, "navigatePageUp"),
            navigateLeft: vi.spyOn(grid, "navigateLeft").mockReturnValue(true),
            navigateRight: vi.spyOn(grid, "navigateRight").mockReturnValue(true),
            navigateUp: vi.spyOn(grid, "navigateUp").mockReturnValue(true),
            navigateDown: vi.spyOn(grid, "navigateDown").mockReturnValue(true),
            navigateNext: vi.spyOn(grid, "navigateNext").mockReturnValue(true),
            navigatePrev: vi.spyOn(grid, "navigatePrev").mockReturnValue(true)
        };

        const commands: KeyboardEventInit[] = [
            { key: "Home", ctrlKey: true },
            { key: "Home" },
            { key: "End", ctrlKey: true },
            { key: "End" },
            { key: "PageDown" },
            { key: "PageUp" },
            { key: "ArrowLeft" },
            { key: "ArrowRight" },
            { key: "ArrowUp" },
            { key: "ArrowDown" },
            { key: "Tab" },
            { key: "Tab", shiftKey: true }
        ];
        for (const command of commands) {
            const event = new KeyboardEvent("keydown", { ...command, bubbles: true, cancelable: true });
            cell.dispatchEvent(event);
            expect(event.defaultPrevented).toBe(true);
        }

        expect(methods.navigateTop).toHaveBeenCalledOnce();
        expect(methods.navigateRowStart).toHaveBeenCalledOnce();
        expect(methods.navigateBottom).toHaveBeenCalledOnce();
        expect(methods.navigateRowEnd).toHaveBeenCalledOnce();
        expect(methods.navigatePageDown).toHaveBeenCalledOnce();
        expect(methods.navigatePageUp).toHaveBeenCalledOnce();
        expect(methods.navigateLeft).toHaveBeenCalledOnce();
        expect(methods.navigateRight).toHaveBeenCalledOnce();
        expect(methods.navigateUp).toHaveBeenCalledOnce();
        expect(methods.navigateDown).toHaveBeenCalledOnce();
        expect(methods.navigateNext).toHaveBeenCalledOnce();
        expect(methods.navigatePrev).toHaveBeenCalledOnce();
    });

    it("allows Escape to bubble when no edit is active", () => {
        const { grid } = createGrid();
        grids.push(grid);
        const cell = grid.getCellNode(0, 0);
        const event = new KeyboardEvent("keydown", { key: "Escape", bubbles: true, cancelable: true });

        cell.dispatchEvent(event);

        expect(event.defaultPrevented).toBe(false);
    });

    it("does not navigate when the key event is immediately stopped", () => {
        const { grid } = createGrid();
        grids.push(grid);
        const cell = grid.getCellNode(0, 0);
        const navigate = vi.spyOn(grid, "navigateTop");
        grid.onKeyDown.subscribe(event => event.stopImmediatePropagation());
        const key = new KeyboardEvent("keydown", { key: "Home", ctrlKey: true, bubbles: true, cancelable: true });

        cell.dispatchEvent(key);

        expect(navigate).not.toHaveBeenCalled();
        expect(key.defaultPrevented).toBe(true);
    });
});
