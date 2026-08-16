import { TreeColumns, type TreeColumn } from "../../src/grid/tree-columns";

describe('TreeColumns', () => {
    // a: -> a1 (visible), a2 (hidden)
    // b: -> b1 (visible)
    // c: (visible, no children)
    function makeTree(): TreeColumn[] {
        return [
            { id: "a", visible: true, columns: [
                { id: "a1", visible: true },
                { id: "a2", visible: false }
            ] },
            { id: "b", visible: false, columns: [
                { id: "b1", visible: true }
            ] },
            { id: "c", visible: true }
        ];
    }

    describe('constructor / init / mapToId', () => {
        it('maps every column (including nested) into columnsById', () => {
            const tc = new TreeColumns(makeTree());
            expect(tc.getById("a")).toBeDefined();
            expect(tc.getById("a1")).toBeDefined();
            expect(tc.getById("a2")).toBeDefined();
            expect(tc.getById("b1")).toBeDefined();
            expect(tc.getById("c")).toBeDefined();
            expect(tc.getById("missing")).toBeUndefined();
        });

        it('keeps the original references in columnsById', () => {
            const tree = makeTree();
            const tc = new TreeColumns(tree);
            expect(tc.getById("a")).toBe(tree[0]);
            expect(tc.getById("a1")).toBe(tree[0].columns![0]);
        });
    });

    describe('getTreeColumns', () => {
        it('returns the original tree columns array', () => {
            const tree = makeTree();
            const tc = new TreeColumns(tree);
            expect(tc.getTreeColumns()).toBe(tree);
        });
    });

    describe('hasDepth', () => {
        it('returns true when any column has children', () => {
            expect(new TreeColumns(makeTree()).hasDepth()).toBe(true);
        });

        it('returns false for a flat list of columns', () => {
            const flat: TreeColumn[] = [{ id: "x" }, { id: "y" }];
            expect(new TreeColumns(flat).hasDepth()).toBe(false);
        });

        it('returns false for an empty list', () => {
            expect(new TreeColumns([]).hasDepth()).toBe(false);
        });
    });

    describe('extractColumns', () => {
        it('flattens nested columns into their leaf columns when there is depth', () => {
            const tc = new TreeColumns(makeTree());
            const extracted = tc.extractColumns();
            expect(extracted.map(c => c.id)).toEqual(["a1", "a2", "b1", "c"]);
        });

        it('returns the columns unchanged when there is no depth', () => {
            const flat: TreeColumn[] = [{ id: "x" }, { id: "y" }];
            const tc = new TreeColumns(flat);
            expect(tc.extractColumns()).toBe(flat);
        });
    });

    describe('getDepth', () => {
        it('returns the depth of the tree', () => {
            expect(new TreeColumns(makeTree()).getDepth()).toBe(2);
        });

        it('returns 1 for a flat list', () => {
            expect(new TreeColumns([{ id: "x" }]).getDepth()).toBe(1);
        });
    });

    describe('getColumnsInDepth', () => {
        it('returns the root columns for depth 0', () => {
            const tc = new TreeColumns(makeTree());
            const result = tc.getColumnsInDepth(0);
            expect(result.map(c => c.id)).toEqual(["a", "b", "c"]);
        });

        it('attaches an extractColumns helper to parent nodes at the requested depth', () => {
            const tc = new TreeColumns(makeTree());
            const roots = tc.getColumnsInDepth(0);
            const a = roots.find(c => c.id === "a")!;
            expect(typeof (a as any).extractColumns).toBe("function");
            const extracted = (a as any).extractColumns();
            expect(extracted.map((c: TreeColumn) => c.id)).toEqual(["a1", "a2"]);
        });

        it('returns the child columns for depth 1', () => {
            const tc = new TreeColumns(makeTree());
            const result = tc.getColumnsInDepth(1);
            expect(result.map(c => c.id)).toEqual(["a1", "a2", "b1"]);
        });

        it('returns an empty array for a depth deeper than the tree', () => {
            const tc = new TreeColumns(makeTree());
            expect(tc.getColumnsInDepth(5)).toEqual([]);
        });
    });

    describe('getColumnsInGroup', () => {
        it('extracts the leaf columns from a group', () => {
            const tc = new TreeColumns(makeTree());
            const groups = [
                { id: "g1", columns: [{ id: "x1" }, { id: "x2", columns: [{ id: "x2a" }] }] }
            ];
            const result = tc.getColumnsInGroup(groups);
            expect(result.map(c => c.id)).toEqual(["x1", "x2a"]);
        });
    });

    describe('visibleColumns', () => {
        it('keeps visible columns and drops hidden ones recursively', () => {
            const tc = new TreeColumns(makeTree());
            const result = tc.visibleColumns();
            expect(result.map(c => c.id)).toEqual(["a", "c"]);
            const a = result[0];
            expect(a.columns!.map(c => c.id)).toEqual(["a1"]);
        });
    });

    describe('filter', () => {
        it('keeps columns matching the condition and their matching descendants', () => {
            const tc = new TreeColumns(makeTree());
            const result = tc.filter(col => col.id === "a" || col.id === "a1");
            expect(result.map(c => c.id)).toEqual(["a"]);
            expect(result[0].columns!.map(c => c.id)).toEqual(["a1"]);
        });

        it('returns the same column object references for kept parents', () => {
            const tree = makeTree();
            const tc = new TreeColumns(tree);
            const result = tc.filter(col => col.id === "a" || col.id === "a1");
            expect(result[0]).toBe(tree[0]);
        });

        // NOTE (suspicious behavior, see plan): filter()/visibleColumns() use a
        // SHALLOW clone (slice()), so when a parent column is kept, its nested
        // .columns array is replaced on the shared object -> the ORIGINAL tree
        // is mutated. Documenting the actual behavior; the user may want to
        // decide whether a deep clone would be more correct.
        it('shallow-clone means keeping a parent replaces its nested columns on the original tree', () => {
            const tree = makeTree();
            const tc = new TreeColumns(tree);
            const result = tc.filter(col => col.id === "a" || col.id === "a1");
            expect(result.map(c => c.id)).toEqual(["a"]);
            expect(tree[0].columns!.map(c => c.id)).toEqual(["a1"]);
            // "b" was not kept, so its children stay untouched
            expect(tree[1].columns!.map(c => c.id)).toEqual(["b1"]);
        });
    });

    describe('reOrder', () => {
        it('sorts columns by their index in the grid', () => {
            const tc = new TreeColumns(makeTree());
            const grid = {
                getColumnIndex: (id: string) => {
                    const order = { a: 2, b: 0, c: 1 };
                    return order[id];
                }
            };
            tc.reOrder(grid);
            expect(tc.getTreeColumns().map(c => c.id)).toEqual(["b", "c", "a"]);
        });

        it('orders columns without a grid index to the front', () => {
            const tc = new TreeColumns([
                { id: "x" },
                { id: "y" }
            ]);
            const grid = {
                getColumnIndex: (id: string) => id === "y" ? 5 : undefined
            };
            tc.reOrder(grid);
            expect(tc.getTreeColumns().map(c => c.id)).toEqual(["x", "y"]);
        });

        it('recursively sorts nested columns', () => {
            const tc = new TreeColumns(makeTree());
            const grid = {
                getColumnIndex: (id: string) => {
                    const order = { a: 0, b: 1, c: 2, a1: 1, a2: 0 };
                    return order[id];
                }
            };
            tc.reOrder(grid);
            const a = tc.getTreeColumns()[0];
            expect(a.id).toBe("a");
            expect(a.columns!.map(c => c.id)).toEqual(["a2", "a1"]);
        });
    });

    describe('getInIds', () => {
        it('returns the columns for the given ids in order', () => {
            const tc = new TreeColumns(makeTree());
            const result = tc.getInIds(["c", "a1"]);
            expect(result.map(c => c && c.id)).toEqual(["c", "a1"]);
        });

        it('returns undefined entries for unknown ids', () => {
            const tc = new TreeColumns(makeTree());
            const result = tc.getInIds(["missing"]);
            expect(result[0]).toBeUndefined();
        });
    });
});
