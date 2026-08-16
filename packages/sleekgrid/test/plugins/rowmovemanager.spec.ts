import { EditorLock } from "../../src/core/editing";
import { RowMoveManager, type ArgsMoveRows } from "../../src/plugins/rowmovemanager";
import { RowSelectionModel } from "../../src/plugins/rowselectionmodel";
import { SleekGrid } from "../../src/grid/sleekgrid";

function makeData(n: number) {
    const data: any[] = [];
    for (let i = 0; i < n; i++)
        data.push({ id: i, c1: "r" + i, c2: "c" + i });
    return data;
}

function makeCols(behavior = "selectAndMove") {
    return [
        { id: "c1", name: "c1", field: "c1", behavior },
        { id: "c2", name: "c2", field: "c2", behavior }
    ];
}

const created: { grid: SleekGrid; container: HTMLElement }[] = [];

function setup(n = 10, gridOptions: any = {}, managerOptions: any = {}, behavior = "selectAndMove") {
    const container = document.createElement("div");
    container.style.height = "100px";
    document.body.appendChild(container);
    const grid = new SleekGrid(container, makeData(n), makeCols(behavior), gridOptions);
    const selectionModel = new RowSelectionModel();
    grid.setSelectionModel(selectionModel);
    const manager = new RowMoveManager(managerOptions);
    grid.registerPlugin(manager);
    created.push({ grid, container });
    return { container, grid, selectionModel, manager };
}

function fireMouse(type: string, target: EventTarget, opts: any = {}) {
    const ev = new MouseEvent(type, { bubbles: true, cancelable: true, ...opts });
    if (opts.pageY !== undefined)
        Object.defineProperty(ev, "pageY", { value: opts.pageY, configurable: true });
    target.dispatchEvent(ev);
    return ev;
}

function startDrag(grid: SleekGrid, row = 0, cell = 0) {
    const cellNode = grid.getCellNode(row, cell);
    fireMouse("mousedown", cellNode, { clientX: 10, clientY: 10 });
}

function moveDrag(grid: SleekGrid, pageY: number, row = 0, cell = 0) {
    const cellNode = grid.getCellNode(row, cell);
    fireMouse("mousemove", cellNode, { clientX: 10, clientY: pageY, pageY });
}

function endDrag(grid: SleekGrid, row = 0, cell = 0) {
    const cellNode = grid.getCellNode(row, cell);
    fireMouse("mouseup", cellNode, { clientX: 10, clientY: 10 });
}

afterEach(() => {
    // clean up any drag listeners left attached to document.body mid-drag
    document.body.dispatchEvent(new MouseEvent("mouseup", { bubbles: true }));
    while (created.length) {
        const { grid, container } = created.pop();
        grid.destroy();
        container.remove();
    }
});

describe("RowMoveManager defaults", () => {
    it("cancelEditOnDrag defaults to false", () => {
        expect(RowMoveManager.defaults.cancelEditOnDrag).toBe(false);
    });
});

describe("RowMoveManager drag flow", () => {
    it("creates proxy and guide, moves rows, and notifies onMoveRows", () => {
        const { grid, manager } = setup();
        const moved: ArgsMoveRows[] = [];
        manager.onMoveRows.subscribe((_e, args) => moved.push(args));

        startDrag(grid);
        moveDrag(grid, 75);
        const canvas = grid.getCanvasNode();
        const proxy = canvas.querySelector(".slick-row-move-proxy") as HTMLElement;
        const guide = canvas.querySelector(".slick-row-move-guide") as HTMLElement;
        expect(proxy).toBeTruthy();
        expect(guide).toBeTruthy();
        expect(proxy.style.top).toBe("70px");
        expect(guide.style.top).toBe("90px");

        endDrag(grid);
        expect(canvas.querySelector(".slick-row-move-proxy")).toBeNull();
        expect(canvas.querySelector(".slick-row-move-guide")).toBeNull();
        expect(moved).toEqual([{ rows: [0], insertBefore: 3 }]);
    });

    it("selects the dragged row when nothing was selected", () => {
        const { grid, selectionModel } = setup();
        startDrag(grid);
        moveDrag(grid, 75);
        expect(selectionModel.getSelectedRows()).toEqual([0]);
        endDrag(grid);
    });

    it("keeps the existing selection when it contains the dragged row", () => {
        const { grid, selectionModel, manager } = setup();
        selectionModel.setSelectedRows([0, 2]);
        const moved: ArgsMoveRows[] = [];
        manager.onMoveRows.subscribe((_e, args) => moved.push(args));
        startDrag(grid);
        expect(selectionModel.getSelectedRows()).toEqual([0, 2]);
        moveDrag(grid, 75);
        endDrag(grid);
        expect(moved).toEqual([{ rows: [0, 2], insertBefore: 3 }]);
    });

    it("replaces the selection with the dragged row when it is not selected", () => {
        const { grid, selectionModel } = setup();
        selectionModel.setSelectedRows([2, 3]);
        startDrag(grid);
        moveDrag(grid, 75);
        expect(selectionModel.getSelectedRows()).toEqual([0]);
        endDrag(grid);
    });

    it("clamps insertBefore to the data length", () => {
        const { grid, manager } = setup(5);
        const moved: ArgsMoveRows[] = [];
        manager.onMoveRows.subscribe((_e, args) => moved.push(args));
        startDrag(grid);
        moveDrag(grid, 10000);
        endDrag(grid);
        expect(moved).toEqual([{ rows: [0], insertBefore: 5 }]);
    });

    it("clamps insertBefore to zero when dragging above the grid", () => {
        const { grid, manager } = setup();
        const moved: ArgsMoveRows[] = [];
        manager.onMoveRows.subscribe((_e, args) => moved.push(args));
        startDrag(grid);
        moveDrag(grid, -100);
        endDrag(grid);
        expect(moved).toEqual([{ rows: [0], insertBefore: 0 }]);
    });

    it("does not re-run the move check when insertBefore is unchanged", () => {
        const { grid, manager } = setup();
        let beforeCalls = 0;
        manager.onBeforeMoveRows.subscribe(() => { beforeCalls++; return true; });
        startDrag(grid);
        moveDrag(grid, 75); // insertBefore 3
        moveDrag(grid, 76); // insertBefore still 3
        expect(beforeCalls).toBe(1);
        endDrag(grid);
    });
});

describe("RowMoveManager veto", () => {
    it("hides the guide and does not move rows when onBeforeMoveRows returns false", () => {
        const { grid, manager } = setup();
        manager.onBeforeMoveRows.subscribe(() => false);
        const moved: ArgsMoveRows[] = [];
        manager.onMoveRows.subscribe((_e, args) => moved.push(args));

        startDrag(grid);
        const canvas = grid.getCanvasNode();
        moveDrag(grid, 75);
        const guide = canvas.querySelector(".slick-row-move-guide") as HTMLElement;
        expect(guide.style.top).toBe("-1000px");
        endDrag(grid);
        expect(moved).toEqual([]);
    });
});

describe("RowMoveManager editor lock", () => {
    it("cancels the current edit and aborts the drag when cancelEditOnDrag is set", () => {
        const editorLock = new EditorLock();
        let cancelled = 0;
        editorLock.activate({
            commitCurrentEdit: () => true,
            cancelCurrentEdit: () => { cancelled++; return true; }
        });
        const { grid, manager } = setup(10, { editorLock }, { cancelEditOnDrag: true });
        const moved: ArgsMoveRows[] = [];
        manager.onMoveRows.subscribe((_e, args) => moved.push(args));

        startDrag(grid);
        moveDrag(grid, 75);
        expect(cancelled).toBe(1);
        const canvas = grid.getCanvasNode();
        expect(canvas.querySelector(".slick-row-move-proxy")).toBeNull();
        endDrag(grid);
        expect(moved).toEqual([]);
    });

    it("aborts the drag when the editor lock is active without cancelEditOnDrag", () => {
        const editorLock = new EditorLock();
        editorLock.activate({
            commitCurrentEdit: () => true,
            cancelCurrentEdit: () => true
        });
        const { grid, manager } = setup(10, { editorLock });
        const moved: ArgsMoveRows[] = [];
        manager.onMoveRows.subscribe((_e, args) => moved.push(args));

        startDrag(grid);
        const canvas = grid.getCanvasNode();
        expect(canvas.querySelector(".slick-row-move-proxy")).toBeNull();
        moveDrag(grid, 75);
        endDrag(grid);
        expect(moved).toEqual([]);
    });
});

describe("RowMoveManager non-move columns", () => {
    it("aborts the drag when the column behavior is not move/selectAndMove", () => {
        const { grid, manager } = setup(10, {}, {}, "not-movable");
        const moved: ArgsMoveRows[] = [];
        manager.onMoveRows.subscribe((_e, args) => moved.push(args));

        startDrag(grid);
        const canvas = grid.getCanvasNode();
        expect(canvas.querySelector(".slick-row-move-proxy")).toBeNull();
        moveDrag(grid, 75);
        endDrag(grid);
        expect(moved).toEqual([]);
    });
});

describe("RowMoveManager destroy", () => {
    it("unsubscribes from grid drag events on destroy", () => {
        const { grid, manager } = setup();
        manager.destroy();
        startDrag(grid);
        const canvas = grid.getCanvasNode();
        expect(canvas.querySelector(".slick-row-move-proxy")).toBeNull();
        moveDrag(grid, 75);
        endDrag(grid);
    });
});
