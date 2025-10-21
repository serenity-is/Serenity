
export interface TreeColumn {
    columns?: TreeColumn[];
    id?: string;
    visible?: boolean;
}

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
 *
 * @param {Array} treeColumns Array com levels of columns
 * @returns {{hasDepth: 'hasDepth', getTreeColumns: 'getTreeColumns', extractColumns: 'extractColumns', getDepth: 'getDepth',
 * getColumnsInDepth: 'getColumnsInDepth', getColumnsInGroup: 'getColumnsInGroup', visibleColumns: 'visibleColumns', filter: 'filter', reOrder: reOrder}}
 * @constructor
 */
export class TreeColumns {

    declare private treeColumns: TreeColumn[];

    constructor(treeColumns: TreeColumn[]) {
        this.treeColumns = treeColumns;
        this.init();
    }

    columnsById: { [key: string]: TreeColumn } = {};

    init() {
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

    hasDepth() {

        for (var i in this.treeColumns) {
            if (this.treeColumns[i].hasOwnProperty('columns')) {
                return true;
            }
        }

        return false;
    };

    getTreeColumns() {
        return this.treeColumns;
    };

    extractColumns() {
        return this.hasDepth() ? extractColumns(this.treeColumns) : this.treeColumns;
    };

    getDepth() {
        return getDepth(this.treeColumns);
    };

    getColumnsInDepth(depth: number) {
        return getColumnsInDepth(this.treeColumns, depth);
    };

    getColumnsInGroup = function (groups: any) {
        return extractColumns(groups);
    };

    visibleColumns() {
        return filter(this.cloneTreeColumns(), (column) => column.visible);
    };

    filter(condition: (col: TreeColumn) => boolean) {
        return filter(this.cloneTreeColumns(), condition);
    };

    reOrder(grid: TreeColumnsGrid) {
        return sort(this.treeColumns, grid);
    };

    getById(id: string) {
        return this.columnsById[id];
    }

    getInIds(ids: string[]) {
        return ids.map((id) => {
            return this.columnsById[id];
        });
    }
}
