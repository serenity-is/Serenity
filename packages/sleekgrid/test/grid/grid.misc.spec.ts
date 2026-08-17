import type { CellStylesHash, Column, IDataView, ItemMetadata } from "../../src/core";
import { BasicLayout, FrozenLayout, SleekGrid } from "../../src/grid";

interface MiscRow {
    value: string;
}

const columns: Column<MiscRow>[] = [{ id: "value", field: "value", width: 100 }];

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

describe("SleekGrid miscellaneous public APIs", () => {
    const grids: SleekGrid<MiscRow>[] = [];

    afterEach(() => {
        while (grids.length)
            grids.pop()?.destroy();
    });

    it("flashes a rendered cell without requiring jQuery", () => {
        vi.useFakeTimers();
        const { grid } = createGrid();
        grids.push(grid);
        const cell = grid.getCellNode(0, 0);

        grid.flashCell(0, 0, 10);
        vi.advanceTimersByTime(10);
        expect(cell.classList.contains(grid.getOptions().cellFlashingCssClass)).toBe(true);
        vi.advanceTimersByTime(30);
        expect(cell.classList.contains(grid.getOptions().cellFlashingCssClass)).toBe(false);
        vi.useRealTimers();
    });

    it("gets and replaces data through the public data APIs", () => {
        const initial = [{ value: "one" }];
        const replacement = [{ value: "two" }, { value: "three" }];
        const { grid } = createGrid();
        grids.push(grid);

        grid.setData(initial);
        expect(grid.getData()).toBe(initial);
        expect(grid.getDataLength()).toBe(1);
        expect(grid.getDataItem(0)).toBe(initial[0]);
        grid.setData(replacement, true);
        expect(grid.getData()).toBe(replacement);
        expect(grid.getDataLength()).toBe(2);
        expect(grid.getDataItem(1)).toBe(replacement[1]);
    });

    it("manages cell CSS styles on rendered cells", () => {
        const { grid } = createGrid();
        grids.push(grid);
        const styles: CellStylesHash = { 0: { value: "highlight" } };
        const changed = vi.fn();
        grid.onCellCssStylesChanged.subscribe(changed);

        grid.addCellCssStyles("test", styles);
        expect(grid.getCellCssStyles("test")).toBe(styles);
        expect(grid.getCellNode(0, 0)?.classList.contains("highlight")).toBe(true);
        expect(() => grid.addCellCssStyles("test", styles)).toThrow();
        grid.removeCellCssStyles("test");
        expect(grid.getCellCssStyles("test")).toBeUndefined();
        expect(grid.getCellNode(0, 0)?.classList.contains("highlight")).toBe(false);
        expect(changed).toHaveBeenCalledTimes(2);
    });

    it("exposes layout, header, and panel information", () => {
        const layout = new BasicLayout();
        const { grid } = createGrid({ layoutEngine: layout, groupingPanel: true, createPreHeaderPanel: true });
        grids.push(grid);

        expect(grid.getLayoutInfo()).toMatchObject({ supportPinnedCols: undefined });
        expect(grid.getContainerNode()).toBeInstanceOf(HTMLElement);
        expect(grid.getUID()).toMatch(/^_sleekgrid_/);
        expect(grid.getHeader()).toBe(grid.getHeaderColumn(0)?.parentElement);
        expect(grid.getHeaderRow()).toBeDefined();
        expect(grid.getFooterRow()).toBeDefined();
        expect(grid.getGroupingPanel()).toBeDefined();
        expect(grid.getPreHeaderPanel()).toBeDefined();
    });

    it("updates panel and row visibility through public setters", () => {
        const { grid } = createGrid({ groupingPanel: true, createPreHeaderPanel: true });
        grids.push(grid);

        grid.setTopPanelVisibility(true);
        grid.setColumnHeaderVisibility(false);
        grid.setFooterRowVisibility(true);
        grid.setGroupingPanelVisibility(false);
        grid.setPreHeaderPanelVisibility(true);
        grid.setHeaderRowVisibility(false);

        expect(grid.getOptions().showColumnHeader).toBe(false);
        expect(grid.getOptions().showFooterRow).toBe(true);
        expect(grid.getOptions().showGroupingPanel).toBe(true);
        expect(grid.getOptions().showHeaderRow).toBe(false);
    });

    it("resolves cell, column, row, and event nodes through public helpers", () => {
        const { grid } = createGrid();
        grids.push(grid);
        const cell = grid.getCellNode(0, 0);
        const row = cell.parentElement;

        expect(grid.getCellFromNode(cell)).toBe(0);
        expect(grid.getCellFromNode(document.createElement("div"))).toBeNull();
        expect(grid.getColumnFromNode(cell)).toBe(grid.getColumns()[0]);
        expect(grid.getColumnFromNode(null)).toBeNull();
        expect(grid.getRowFromNode(row)).toBe(0);
        expect(grid.getRowFromNode(document.createElement("div"))).toBeNull();
        expect(grid.getCellFromEvent({ target: cell })).toEqual({ row: 0, cell: 0 });
        expect(grid.getCellFromEvent({ target: document.createElement("div") })).toBeNull();
        expect(grid.getCellNodeBox(0, 0)).toBeDefined();
        expect(grid.getCellNodeBox(-1, 0)).toBeNull();
    });

    it("uses metadata for colspans and formatter selection", () => {
        const format = () => "metadata";
        const metadata: ItemMetadata<MiscRow> = {
            format,
            columns: { value: { colspan: 2 } }
        };
        const data: IDataView<MiscRow> = {
            getLength: () => 1,
            getItem: () => ({ value: "one" }),
            getItemMetadata: () => metadata,
            getGrandTotals: () => ({})
        };
        const { grid } = createGrid();
        grids.push(grid);
        grid.setData(data);

        expect(grid.getColspan(0, 0)).toBe(2);
        expect(grid.getFormatter(0, grid.getColumns()[0])).toBe(format);
        expect(grid.getFormatterContext(0, 0).value).toBe("one");
        expect(grid.getDataItemValueForColumn({ value: "value" }, grid.getColumns()[0])).toBe("value");
    });

    it("reports frozen layout information and honors invalid lookup boundaries", () => {
        const { grid } = createGrid({ frozenColumns: 1, layoutEngine: new FrozenLayout() });
        grids.push(grid);

        expect(grid.getLayoutInfo()).toMatchObject({ pinnedStartCols: 0, supportPinnedCols: true });
        expect(grid.getHeaderColumn("missing")).toBeNull();
        expect(grid.getHeaderRowColumn("missing")).toBeUndefined();
        expect(grid.getFooterRowColumn("missing")).toBeNull();
        expect(grid.getCellNode(99, 0)).toBeNull();
        expect(grid.getCellFromPoint(0, 0)).toBeDefined();
    });
});
