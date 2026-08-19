
/**
 * Column node that may contain nested children (`columns`).
 */
export interface TreeColumn {
    columns?: TreeColumn[];
    id?: string;
    visible?: boolean;
}

/**
 * Minimal grid surface consumed by {@link TreeColumns} for id-based reordering.
 */
export interface TreeColumnsGrid {
    getColumnIndex(id: string): number;
}

function filter(node: TreeColumn[], condition: (col: TreeColumn) => boolean) {

    return node.filter(function (column: TreeColumn) {
        var valid = condition(column);

        if (valid && column.columns)
            column.columns = filter(column.columns, condition);

        return valid && (!column.columns || column.columns.length);
    });
}

function sort(columns: TreeColumn[], grid: TreeColumnsGrid) {
    columns.sort((a, b) => {
        var indexA = getOrDefault(grid.getColumnIndex(a.id)),
            indexB = getOrDefault(grid.getColumnIndex(b.id));

        return indexA - indexB;
    })
        .forEach(function (column) {
            if (column.columns)
                sort(column.columns, grid);
        });
}

function getOrDefault(value: any) {
    return typeof value === 'undefined' ? -1 : value;
}

function getDepth(node: any): number {
    if (node.length) {
        for (var i in node) {
            return getDepth(node[i]);
        }
    }
    else if (node.columns)
        return 1 + getDepth(node.columns);
    else
        return 1;
}

function getColumnsInDepth(node: any, depth: number, current?: number) {
    var columns: TreeColumn[] = [];
    current = current || 0;

    if (depth == current) {

        if (node.length) {
            node.forEach(function (n: any) {
                if (n.columns) {
                    n.extractColumns = function () {
                        return extractColumns(n);
                    };
                }
            });
        }

        return node;
    }
    else {
        for (var i in node) {
            if (node[i].columns) {
                columns = columns.concat(getColumnsInDepth(node[i].columns, depth, current + 1));
            }
        }

        return columns;
    }
}

function extractColumns(node: any): TreeColumn[] {
    var result: TreeColumn[] = [];

    if (node.hasOwnProperty('length')) {

        for (var i = 0; i < node.length; i++)
            result = result.concat(extractColumns(node[i]));
    }
    else {
        if (node.hasOwnProperty('columns'))
            result = result.concat(extractColumns(node.columns));
        else
            return node;
    }

    return result;
}

/**
 * Helper for grouped/nested column trees. Supports depth queries, filtering,
 * flattening and id-order reordering.
 * @param treeColumns - Array of (possibly nested) column nodes.
 */
export class TreeColumns {

    declare private treeColumns: TreeColumn[];

    constructor(treeColumns: TreeColumn[]) {
        this.treeColumns = treeColumns;
        this.init();
    }

    /** Index of nodes by their `id`, populated during {@link TreeColumns.init}. */
    columnsById: { [key: string]: TreeColumn } = {};

    /**
     * Initializes `columnsById` by indexing the entire tree.
     */
    init(): void {
        this.mapToId(this.treeColumns);
    }

    private mapToId(columns: TreeColumn[]) {
        columns
            .forEach((column) => {
                this.columnsById[column.id] = column;

                if (column.columns)
                    this.mapToId(column.columns);
            });
    }

    private cloneTreeColumns() {
        return this.treeColumns.slice();
    }

    /**
     * Returns `true` when any top-level node has nested `columns`.
     * @returns Whether the tree has at least two levels.
     */
    hasDepth(): boolean {

        for (var i in this.treeColumns) {
            if (this.treeColumns[i].hasOwnProperty('columns')) {
                return true;
            }
        }

        return false;
    };

    /**
     * Returns the raw tree passed to the constructor.
     * @returns Original tree array.
     */
    getTreeColumns(): TreeColumn[] {
        return this.treeColumns;
    };

    /**
     * Flattens the tree to leaf columns (preserves order).
     * @returns Array of leaf nodes.
     */
    extractColumns(): TreeColumn[] {
        return this.hasDepth() ? extractColumns(this.treeColumns) : this.treeColumns;
    };

    /**
     * Returns the maximum nesting depth of the tree.
     * @returns Depth (1-based).
     */
    getDepth(): number {
        return getDepth(this.treeColumns);
    };

    /**
     * Returns nodes at the given `depth` level.
     * @param depth - Target depth (0-based).
     * @returns Nodes at that depth.
     */
    getColumnsInDepth(depth: number): TreeColumn[] {
        return getColumnsInDepth(this.treeColumns, depth);
    };

    /**
     * Returns leaf columns contained within the given group nodes.
     * @param groups - Group nodes to extract from.
     * @returns Leaf columns within those groups.
     */
    getColumnsInGroup = function (groups: any): TreeColumn[] {
        return extractColumns(groups);
    };

    /**
     * Returns a clone of the tree filtered to visible leaves (`visible !== false`).
     * @returns Visible columns, nested.
     */
    visibleColumns(): TreeColumn[] {
        return filter(this.cloneTreeColumns(), (column) => column.visible);
    };

    /**
     * Clones and filters the tree using `condition`, pruning parents whose
     * children are all filtered out.
     * @param condition - Predicate to test each node.
     * @returns Filtered tree.
     */
    filter(condition: (col: TreeColumn) => boolean): TreeColumn[] {
        return filter(this.cloneTreeColumns(), condition);
    };

    /**
     * Reorders the tree to match the column order of `grid`.
     * @param grid - Grid providing `getColumnIndex` for ordering.
     */
    reOrder(grid: TreeColumnsGrid): void {
        sort(this.treeColumns, grid);
    };

    /**
     * Looks up a column node by id.
     * @param id - Column id to find.
     * @returns Matching node, or `undefined`.
     */
    getById(id: string): TreeColumn {
        return this.columnsById[id];
    }

    /**
     * Looks up multiple column nodes by their ids.
     * @param ids - Column ids in desired order.
     * @returns Array of matching nodes (may contain `undefined` when missing).
     */
    getInIds(ids: string[]): TreeColumn[] {
        return ids.map((id) => {
            return this.columnsById[id];
        });
    }
}
