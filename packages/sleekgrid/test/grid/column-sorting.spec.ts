import { EventEmitter } from "../../src/core/event";
import { columnSortHandler, sortToDesiredOrderAndKeepRest } from "../../src/grid/column-sorting";

function makeHeaderColumn(id: string, datasetC = "0"): HTMLElement {
    const node = document.createElement("div");
    node.className = "slick-header-column";
    node.dataset.c = datasetC;
    return node;
}

function makeSortContext(overrides: any = {}) {
    const onSort = new EventEmitter<any, any>();
    const ctx = {
        getColumnFromNode: vi.fn(() => ({ id: "c1", sortable: true, defaultSortAsc: true })),
        getEditorLock: () => ({ commitCurrentEdit: () => true }),
        getColumnById: vi.fn((id: string) => ({ id })),
        getSortColumns: () => [],
        setSortColumns: vi.fn(),
        onSort,
        getOptions: () => ({ multiColumnSort: false }),
        ...overrides
    };
    return ctx;
}

function clickOn(ctx: any, node: HTMLElement, init: MouseEventInit = {}): MouseEvent {
    const e = new MouseEvent("click", { bubbles: true, cancelable: true, ...init });
    node.dispatchEvent(e);
    columnSortHandler.call(ctx, e);
    return e;
}

describe('columnSortHandler', () => {
    it('does nothing when the click is on a resize handle', () => {
        const ctx = makeSortContext();
        const header = makeHeaderColumn("c1");
        const handle = document.createElement("div");
        handle.className = "slick-resizable-handle";
        header.appendChild(handle);
        clickOn(ctx, handle);
        expect(ctx.setSortColumns).not.toHaveBeenCalled();
    });

    it('does nothing when the click is outside a header column', () => {
        const ctx = makeSortContext();
        const other = document.createElement("div");
        clickOn(ctx, other);
        expect(ctx.setSortColumns).not.toHaveBeenCalled();
    });

    it('does nothing when the column is not sortable', () => {
        const ctx = makeSortContext({
            getColumnFromNode: () => ({ id: "c1", sortable: false })
        });
        clickOn(ctx, makeHeaderColumn("c1"));
        expect(ctx.setSortColumns).not.toHaveBeenCalled();
    });

    it('does nothing when the editor lock cannot commit', () => {
        const ctx = makeSortContext({
            getEditorLock: () => ({ commitCurrentEdit: () => false })
        });
        clickOn(ctx, makeHeaderColumn("c1"));
        expect(ctx.setSortColumns).not.toHaveBeenCalled();
    });

    it('adds a sort on a fresh click and fires onSort', () => {
        let sortCols: any[] = [];
        const ctx = makeSortContext({
            getSortColumns: () => sortCols,
            setSortColumns: (cols: any[]) => { sortCols = cols; }
        });
        let payload: any;
        ctx.onSort.subscribe((_e: any, p: any) => { payload = p; });

        clickOn(ctx, makeHeaderColumn("c1"));

        expect(sortCols).toEqual([{ columnId: "c1", sortAsc: true }]);
        expect(payload).toMatchObject({ multiColumnSort: false, sortCol: { id: "c1", sortable: true, defaultSortAsc: true }, sortAsc: true });
    });

    it('toggles the sort direction on a second click', () => {
        let sortCols: any[] = [{ columnId: "c1", sortAsc: true }];
        const ctx = makeSortContext({
            getSortColumns: () => sortCols,
            setSortColumns: (cols: any[]) => { sortCols = cols; }
        });
        clickOn(ctx, makeHeaderColumn("c1"));
        expect(sortCols).toEqual([{ columnId: "c1", sortAsc: false }]);
    });

    it('removes the sort when a single descending sort is clicked again', () => {
        let sortCols: any[] = [{ columnId: "c1", sortAsc: false }];
        const ctx = makeSortContext({
            getSortColumns: () => sortCols,
            setSortColumns: (cols: any[]) => { sortCols = cols; }
        });
        clickOn(ctx, makeHeaderColumn("c1"));
        expect(sortCols).toEqual([]);
    });

    it('uses the column defaultSortAsc for a fresh sort', () => {
        let sortCols: any[] = [];
        const ctx = makeSortContext({
            getColumnFromNode: () => ({ id: "c1", sortable: true, defaultSortAsc: false }),
            getSortColumns: () => sortCols,
            setSortColumns: (cols: any[]) => { sortCols = cols; }
        });
        clickOn(ctx, makeHeaderColumn("c1"));
        expect(sortCols).toEqual([{ columnId: "c1", sortAsc: false }]);
    });

    it('adds a second column with shift+click in multi-column sort mode', () => {
        let sortCols: any[] = [{ columnId: "c1", sortAsc: true }];
        const ctx = makeSortContext({
            getColumnFromNode: () => ({ id: "c2", sortable: true, defaultSortAsc: true }),
            getOptions: () => ({ multiColumnSort: true }),
            getSortColumns: () => sortCols,
            setSortColumns: (cols: any[]) => { sortCols = cols; }
        });
        clickOn(ctx, makeHeaderColumn("c2"), { shiftKey: true });
        expect(sortCols).toEqual([
            { columnId: "c1", sortAsc: true },
            { columnId: "c2", sortAsc: true }
        ]);
    });

    it('removes a column from the multi-sort with alt+click', () => {
        let sortCols: any[] = [
            { columnId: "c1", sortAsc: true },
            { columnId: "c2", sortAsc: true }
        ];
        const ctx = makeSortContext({
            getColumnFromNode: () => ({ id: "c1", sortable: true, defaultSortAsc: true }),
            getOptions: () => ({ multiColumnSort: true }),
            getSortColumns: () => sortCols,
            setSortColumns: (cols: any[]) => { sortCols = cols; }
        });
        clickOn(ctx, makeHeaderColumn("c1"), { altKey: true });
        expect(sortCols).toEqual([{ columnId: "c2", sortAsc: true }]);
    });

    it('fires a multi-column onSort payload', () => {
        let sortCols: any[] = [{ columnId: "c1", sortAsc: true }];
        const ctx = makeSortContext({
            getOptions: () => ({ multiColumnSort: true }),
            getSortColumns: () => sortCols,
            setSortColumns: (cols: any[]) => { sortCols = cols; }
        });
        let payload: any;
        ctx.onSort.subscribe((_e: any, p: any) => { payload = p; });
        clickOn(ctx, makeHeaderColumn("c1"));
        expect(payload.multiColumnSort).toBe(true);
        // clicking an already-sorted column toggles its direction
        expect(payload.sortCols).toEqual([{ sortCol: { id: "c1" }, sortAsc: false }]);
    });
});

describe('sortToDesiredOrderAndKeepRest', () => {
    const cols = [
        { id: "A" }, { id: "B" }, { id: "C" }, { id: "D" },
        { id: "E" }, { id: "F" }, { id: "G" }, { id: "H" }
    ];

    it('returns the columns unchanged for an empty id order', () => {
        expect(sortToDesiredOrderAndKeepRest(cols, [])).toBe(cols);
    });

    it('reorders visible columns while keeping invisible ones sticky', () => {
        const result = sortToDesiredOrderAndKeepRest(cols, ["G", "D", "F"]);
        // C stays sticky after B; G,H stick after G; D,E stick after D; then F
        expect(result.map(c => c.id)).toEqual(["A", "B", "C", "G", "H", "D", "E", "F"]);
    });

    it('handles a single desired column', () => {
        const result = sortToDesiredOrderAndKeepRest(cols, ["D"]);
        expect(result.map(c => c.id)).toEqual(["A", "B", "C", "D", "E", "F", "G", "H"]);
    });

    it('handles a desired order that starts with the first column', () => {
        const result = sortToDesiredOrderAndKeepRest(cols, ["A", "C"]);
        expect(result.map(c => c.id)).toEqual(["A", "B", "C", "D", "E", "F", "G", "H"]);
    });
});
