import { describe, expect, it, vi } from "vitest";
import { ColumnFormat, CompatFormatter, formatterContext as ctx, Group, GroupTotals } from "../../src/core";
import { GroupItemMetadataProvider } from "../../src/data/groupitemmetadataprovider";

describe("GroupItemMetadataProvider.defaults", () => {
    it("has expected default values", () => {
        expect(GroupItemMetadataProvider.defaults.enableExpandCollapse).toBe(true);
        expect(GroupItemMetadataProvider.defaults.groupIndentation).toBe(15);
        expect(GroupItemMetadataProvider.defaults.indentDataRows).toBe(true);
        expect(GroupItemMetadataProvider.defaults.minGroupCellWidth).toBe(40);
        expect(GroupItemMetadataProvider.defaults.groupFocusable).toBe(true);
        expect(GroupItemMetadataProvider.defaults.totalsFocusable).toBe(false);
    });

    it("has expected default css classes", () => {
        expect(GroupItemMetadataProvider.defaults.groupCellCssClass).toBe("slick-group-cell");
        expect(GroupItemMetadataProvider.defaults.groupCssClass).toBe("slick-group");
        expect(GroupItemMetadataProvider.defaults.groupLevelPrefix).toBe("slick-group-level-");
        expect(GroupItemMetadataProvider.defaults.groupTitleCssClass).toBe("slick-group-title");
        expect(GroupItemMetadataProvider.defaults.totalsCssClass).toBe("slick-group-totals");
        expect(GroupItemMetadataProvider.defaults.toggleCssClass).toBe("slick-group-toggle");
        expect(GroupItemMetadataProvider.defaults.toggleCollapsedCssClass).toBe("collapsed");
        expect(GroupItemMetadataProvider.defaults.toggleExpandedCssClass).toBe("expanded");
    });

    it("hasSummaryType returns true if the column has summaryType and not 0 or -1", () => {
        expect(GroupItemMetadataProvider.defaults.hasSummaryType).toBeTruthy();
        expect(GroupItemMetadataProvider.defaults.hasSummaryType({})).toBeFalsy();
        expect(GroupItemMetadataProvider.defaults.hasSummaryType({ summaryType: null } as any)).toBeFalsy();
        expect(GroupItemMetadataProvider.defaults.hasSummaryType({ summaryType: -1 } as any)).toBeFalsy(); // -1 is assumed to be disabled
        expect(GroupItemMetadataProvider.defaults.hasSummaryType({ summaryType: 0 } as any)).toBeFalsy(); // 0 is assumed to be none
        expect(GroupItemMetadataProvider.defaults.hasSummaryType({ summaryType: 1 } as any)).toBeTruthy();
        expect(GroupItemMetadataProvider.defaults.hasSummaryType({ summaryType: 3 } as any)).toBeTruthy();
    });
});

describe("GroupItemMetadataProvider constructor", () => {
    it("uses options passed", () => {
        const old = GroupItemMetadataProvider.defaults.groupLevelPrefix;
        try {
            GroupItemMetadataProvider.defaults.groupLevelPrefix = "x-";
            const provider = new GroupItemMetadataProvider({ totalsCssClass: "y" });
            const options = provider.getOptions();
            expect(options.enableExpandCollapse).toBe(true);
            expect(options.groupLevelPrefix).toBe("x-");
            expect(options.totalsCssClass).toBe("y");
            expect(GroupItemMetadataProvider.defaults.totalsCssClass).toBe("slick-group-totals");
        }
        finally {
            GroupItemMetadataProvider.defaults.groupLevelPrefix = old;
        }
    });

    it("uses groupFormat if passed", () => {
        const groupFormat: ColumnFormat = () => "ok";
        const provider = new GroupItemMetadataProvider({ groupFormat });
        expect(provider.getOptions().groupFormat === groupFormat).toBe(true);
    });

    it("uses groupFormat if both groupFormat and compat groupFormatter passed", () => {
        const groupFormat: ColumnFormat = () => "ok";
        const groupFormatter: CompatFormatter = () => "ok";
        const provider = new GroupItemMetadataProvider({ groupFormat, groupFormatter });
        expect(provider.getOptions().groupFormat === groupFormat).toBe(true);
    });

    it("uses converted compat groupFormatter if passed", () => {
        const groupFormatter: CompatFormatter = () => "compat";
        const provider = new GroupItemMetadataProvider({ groupFormatter });
        expect((provider.getOptions() as any).groupFormatter === groupFormatter).toBe(true);
        expect(provider.getOptions().groupFormat as any !== groupFormatter).toBe(true);
        expect(provider.getOptions().groupFormat).toBeDefined();
        expect(provider.getOptions().groupFormat(ctx())).toBe("compat");
    });


    it("uses defaultGroupFormat if none passed", () => {
        const old = GroupItemMetadataProvider.defaultGroupFormat;
        try {
            GroupItemMetadataProvider.defaultGroupFormat = (ctx, o) => { opt = o; return ""; }
            let opt: any = null;
            const provider = new GroupItemMetadataProvider();
            const format = provider.getOptions().groupFormat;
            expect(format).toBeDefined();
            format(ctx());
            expect(opt === provider.getOptions()).toBe(true);
        }
        finally {
            GroupItemMetadataProvider.defaultGroupFormat = old;
        }
    });

    it("uses totalsFormat if passed", () => {
        const totalsFormat: ColumnFormat = () => "ok";
        const provider = new GroupItemMetadataProvider({ totalsFormat });
        expect(provider.getOptions().totalsFormat === totalsFormat).toBe(true);
    });

    it("uses totalsFormat if both totalsFormat and compat totalsFormatter passed", () => {
        const totalsFormat: ColumnFormat = () => "ok";
        const totalsFormatter: CompatFormatter = () => "ok";
        const provider = new GroupItemMetadataProvider({ totalsFormat, totalsFormatter });
        expect(provider.getOptions().totalsFormat === totalsFormat).toBe(true);
    });

    it("uses converted compat totalsFormatter if passed", () => {
        const totalsFormatter: CompatFormatter = () => "compat";
        const provider = new GroupItemMetadataProvider({ totalsFormatter });
        expect((provider.getOptions() as any).totalsFormatter === totalsFormatter).toBe(true);
        expect(provider.getOptions().totalsFormat as any !== totalsFormatter).toBe(true);
        expect(provider.getOptions().totalsFormat).toBeDefined();
        expect(provider.getOptions().totalsFormat(ctx())).toBe("compat");
    });

    it("uses defaultTotalsFormat if none passed", () => {
        const old = GroupItemMetadataProvider.defaultTotalsFormat;
        try {
            GroupItemMetadataProvider.defaultTotalsFormat = (ctx, o) => { called = true; return ""; }
            let called = false;
            const provider = new GroupItemMetadataProvider();
            const format = provider.getOptions().totalsFormat;
            expect(format).toBeDefined();
            format(ctx());
            expect(called).toBe(true);
        }
        finally {
            GroupItemMetadataProvider.defaultTotalsFormat = old;
        }
    });

});

describe("GroupItemMetadataProvider.setOptions", () => {
    it("merges current options with passed ones", () => {
        const provider = new GroupItemMetadataProvider({ totalsCssClass: "y", groupCssClass: "z" });
        const options = provider.getOptions();
        expect(options.totalsCssClass).toBe("y");
        expect(options.groupCssClass).toBe("z");
        provider.setOptions({ totalsCssClass: "w"});
        expect(options.totalsCssClass).toBe("w");
        expect(options.groupCssClass).toBe("z");
    });
});

function mockEvent(args: any) {
    const ev = {
        stopImmediatePropagationCalls: 0,
        stopImmediatePropagation: function() {
            ev.stopImmediatePropagationCalls++;
        },
        preventDefaultCalls: 0,
        preventDefault: function() {
            ev.preventDefaultCalls++;
        },
        target: {
            classNames: ["slick-group-toggle"],
            classList: {
                contains: function(s: string) {
                    return ev.target?.classNames.indexOf(s) >= 0;
                },
                add: function(s: string) {
                    ev.target.classNames.push(s);
                },
                remove: function(s: string) {
                    const idx = ev.target.classNames.indexOf(s);
                    expect(idx >= 0).toBe(true);
                    ev.target.classNames.splice(idx, 1);
                }
            }
        },
        ...args
    }
    return ev;
}

function mockGrid() {
    const grid = {
        onClickList: <any[]>[],
        onClick: {
            subscribe: function(f: any) { grid.onClickList.push(f); },
            unsubscribe: function(f: any) {
                const idx = grid.onClickList.indexOf(f);
                expect(idx >= 0).toBe(true);
                grid.onClickList.splice(idx, 1);
            }
        } as any,
        onKeyDownList: <any[]>[],
        onKeyDown: {
            subscribe: function(f: any) { grid.onKeyDownList.push(f); },
            unsubscribe: function(f: any) {
                const idx = grid.onKeyDownList.indexOf(f);
                expect(idx >= 0).toBe(true);
                grid.onKeyDownList.splice(idx, 1);
            }
        } as any,
        getDataItemCalls: 0,
        getRenderedRangeCalls: 0,
        getRenderedRange: function() {
            grid.getRenderedRangeCalls++;
            return {
                top: 5,
                bottom: 13
            }
        },
        getDataItem: function(row: number) {
            grid.getDataItemCalls++;
            if (row < 0)
                return null;
            if (row === 1 || row == 3) {
                const group = new Group();
                group.groupingKey = "gk" + row;
                group.collapsed = row === 1;
                return group;
            }

            return {
                __row: row
            }
        },
        __data: {
            setRefreshHintsCalls: <any[]>[],
            setRefreshHints(obj: any) {
                grid.__data.setRefreshHintsCalls.push(obj);
            },
            collapseGroupCalls: <string[]>[],
            collapseGroup(key: string) {
                grid.__data.collapseGroupCalls.push(key);
            },
            expandGroupCalls: <string[]>[],
            expandGroup(key: string) {
                grid.__data.expandGroupCalls.push(key);
            },
            length: 999
        },
        getData: function() {
            return grid.__data;
        },
        getActiveCell: <any>null,
        getColumns: <any>null,
        getOptions: () => ({}),
        canCellBeActive: () => true,
        getColspan: () => 1,
        groupTotalsFormatter: <any>null
    };
    return grid as any;
}

function totalsGrouping(displayTotalsRow?: boolean | "groupheader", count = 1) {
    return Array.from({ length: count }, () => ({ displayTotalsRow }));
}

/** Makes the view show the totals of a group on a row of their own, so the group cell spans the row. */
function withTotalsRow(grid: any) {
    grid.getData = () => ({ getGrouping: () => totalsGrouping(true) } as any);
    return grid;
}

function keyGrid() {
    const grid = mockGrid();
    // a select column that the group cell skips, and a group cell that spans the rest of the row
    grid.getColumns = () => [
        { id: "__select__", minWidth: 27, maxWidth: 27 },
        { id: "name" },
        { id: "other" }
    ];
    grid.getColspan = (_row: number, cell: number) => cell === 1 ? 2 : 1;
    return grid;
}

describe("GroupItemMetadataProvider.init", () => {

    it("attaches to onClick", () => {
        const plugin = new GroupItemMetadataProvider();
        const grid = mockGrid();
        plugin.init(grid);
        expect(grid.onClickList.length).toBe(1);
        plugin.destroy();
        expect(grid.onClickList.length).toBe(0);
    });

    it("attaches to onKeyDown", () => {
        const plugin = new GroupItemMetadataProvider();
        const grid = mockGrid();
        plugin.init(grid);
        expect(grid.onKeyDownList.length).toBe(1);
        plugin.destroy();
        expect(grid.onKeyDownList.length).toBe(0);
    });

});

describe("GroupItemMetadataProvider.handleGridClick", () => {

    it("ignores when args does not include grid", () => {
        const plugin = new GroupItemMetadataProvider();
        const grid = mockGrid();
        const event = mockEvent({});
        plugin.handleGridClick(event);
        expect(grid.getDataItemCalls).toBe(0);
        expect(event.stopImmediatePropagationCalls).toBe(0);
        expect(event.preventDefaultCalls).toBe(0);
    });

    it("uses initializing grid when args does not include grid", () => {
        const plugin = new GroupItemMetadataProvider();
        const grid = mockGrid();
        const event = mockEvent({ row: -1 });
        plugin.init(grid);
        plugin.handleGridClick(event);
        expect(grid.getDataItemCalls).toBe(1);
        expect(event.stopImmediatePropagationCalls).toBe(0);
        expect(event.preventDefaultCalls).toBe(0);
    });

    it("ignores when no item for args.row", () => {
        const plugin = new GroupItemMetadataProvider();
        const grid = mockGrid();
        const event = mockEvent({ row: -1 });
        plugin.init(grid);
        plugin.handleGridClick(event);
        expect(grid.getDataItemCalls).toBe(1);
        expect(event.stopImmediatePropagationCalls).toBe(0);
        expect(event.preventDefaultCalls).toBe(0);
    });

    it("ignores when item at args.row is not an instance of Group", () => {
        const plugin = new GroupItemMetadataProvider();
        const grid = mockGrid();
        const event = mockEvent({ row: 333 });
        plugin.init(grid);
        plugin.handleGridClick(event);
        expect(grid.getDataItemCalls).toBe(1);
        expect(event.stopImmediatePropagationCalls).toBe(0);
        expect(event.preventDefaultCalls).toBe(0);
    });

    it("ignores when event target does not contain toggle class", () => {
        const plugin = new GroupItemMetadataProvider();
        const grid = mockGrid();
        const event = mockEvent({ row: 1 });
        event.target.classNames = ["xyz"];
        plugin.init(grid);
        plugin.handleGridClick(event);
        expect(grid.getDataItemCalls).toBe(1);
        expect(event.stopImmediatePropagationCalls).toBe(0);
        expect(event.preventDefaultCalls).toBe(0);
    });

    it("calls stopImmediatePropagation, preventDefault and setRefreshHints", () => {
        const plugin = new GroupItemMetadataProvider();
        const grid = mockGrid();
        const event = mockEvent({ row: 1 });
        plugin.init(grid);
        plugin.handleGridClick(event);
        expect(grid.getDataItemCalls).toBe(1);
        expect(event.stopImmediatePropagationCalls).toBe(1);
        expect(event.preventDefaultCalls).toBe(1);
        expect(grid.__data.setRefreshHintsCalls.length).toBe(1);
        expect(grid.__data.setRefreshHintsCalls[0]).toStrictEqual({ ignoreDiffsBefore: 5, ignoreDiffsAfter: 14 });
    });

    it("calls stopImmediatePropagation, preventDefault and setRefreshHints", () => {
        const plugin = new GroupItemMetadataProvider();
        const grid = mockGrid();
        const event = mockEvent({ row: 1 });
        plugin.init(grid);
        plugin.handleGridClick(event);
        expect(grid.getDataItemCalls).toBe(1);
        expect(event.stopImmediatePropagationCalls).toBe(1);
        expect(event.preventDefaultCalls).toBe(1);
        expect(grid.__data.setRefreshHintsCalls.length).toBe(1);
        expect(grid.__data.setRefreshHintsCalls[0]).toStrictEqual({ ignoreDiffsBefore: 5, ignoreDiffsAfter: 14 });
    });

    it("calls expandGroup if collapsed is true", () => {
        const plugin = new GroupItemMetadataProvider();
        const grid = mockGrid();
        const event = mockEvent({ row: 1 });
        plugin.init(grid);
        plugin.handleGridClick(event);
        expect(grid.__data.collapseGroupCalls.length).toBe(0);
        expect(grid.__data.expandGroupCalls.length).toBe(1);
        expect(grid.__data.expandGroupCalls[0]).toBe("gk1");
    });

    it("calls collapseGroup if collapsed is falsy", () => {
        const plugin = new GroupItemMetadataProvider();
        const grid = mockGrid();
        const event = mockEvent({ row: 3 });
        plugin.init(grid);
        plugin.handleGridClick(event);
        expect(grid.__data.expandGroupCalls.length).toBe(0);
        expect(grid.__data.collapseGroupCalls.length).toBe(1);
        expect(grid.__data.collapseGroupCalls[0]).toBe("gk3");
    });
});

describe("GroupItemMetadataProvider formatters and metadata", () => {
    it("formats groups with escaped values and optional expansion markup", () => {
        const group = new Group();
        group.value = "<unsafe>";
        group.level = 2;
        group.collapsed = true;

        const plain = GroupItemMetadataProvider.defaultGroupFormat(ctx({ item: group }), {
            ...GroupItemMetadataProvider.defaults,
            enableExpandCollapse: false
        });
        const expanded = GroupItemMetadataProvider.defaultGroupFormat(ctx({ item: group }), {
            ...GroupItemMetadataProvider.defaults,
            groupIndentation: 10
        });

        expect(plain).toBe("<unsafe>");
        expect(expanded).toBeDefined();
        expect(group.formatValue).toBeUndefined();
    });

    it("indents the toggle with the level class instead of an inline style", () => {
        const group = new Group();
        group.level = 2;
        group.value = "x";

        const result = GroupItemMetadataProvider.defaultGroupFormat(ctx({ item: group }));
        const node = document.createElement("div");
        if (result instanceof Node)
            node.appendChild(result);
        else
            node.innerHTML = String(result);
        const html = node.innerHTML;

        expect(html).toContain("slick-group-toggle");
        expect(html).toContain("slick-group-level-2");
        // an inline style would be dropped by a content security policy
        expect(html).not.toContain("style=");
        expect(html).not.toContain("margin-left");
    });

    it("uses a group's formatValue when supplied", () => {
        const group = new Group();
        const formatValue = vi.fn(() => "formatted");
        group.formatValue = formatValue;

        expect(GroupItemMetadataProvider.defaultGroupFormat(ctx({ item: group }), {
            ...GroupItemMetadataProvider.defaults,
            enableExpandCollapse: false
        })).toBe("formatted");
        expect(formatValue).toHaveBeenCalledOnce();
    });

    it("selects totals formatters from the grid and column compatibility options", () => {
        const column = {} as any;
        const totals = new GroupTotals();
        const gridFormatter = vi.fn(() => () => "grid");
        const grid = { getTotalsFormatter: gridFormatter } as any;

        expect(GroupItemMetadataProvider.defaultTotalsFormat(ctx({ item: totals, column }), grid)).toBe("grid");
        expect(gridFormatter).toHaveBeenCalledWith(column);

        const compatFormatter = vi.fn(() => "compat");
        const compatColumn = { groupTotalsFormatter: compatFormatter } as any;
        expect(GroupItemMetadataProvider.defaultTotalsFormat(ctx({ item: totals, column: compatColumn }))).toBe("compat");
        expect(compatFormatter).toHaveBeenCalled();

        const formatColumn = { groupTotalsFormat: () => "format" } as any;
        expect(GroupItemMetadataProvider.defaultTotalsFormat(ctx({ item: totals, column: formatColumn }))).toBe("format");
        expect(GroupItemMetadataProvider.defaultTotalsFormat(ctx({ item: totals, column: {} as any }))).toBe("");
    });

    it("unwraps a wrapper item's totals before formatting", () => {
        const totals = new GroupTotals();
        const formatter = vi.fn((value: any) => value.item === totals ? "ok" : "wrong");
        const column = { groupTotalsFormat: formatter } as any;
        const wrapper = { totals } as any;

        expect(GroupItemMetadataProvider.defaultTotalsFormat(ctx({ item: wrapper, column }))).toBe("ok");
        expect(formatter).toHaveBeenCalledOnce();
    });

    it("puts the totals on the group row only when the grouping says so", () => {
        const provider = new GroupItemMetadataProvider();
        const grid = mockGrid();
        grid.getColumns = () => [{ summaryType: 1 }, { id: "name" }, { id: "other" }];
        grid.getData = () => ({ getGrouping: () => totalsGrouping("groupheader") } as any);
        provider.init(grid);

        const group = new Group();
        const metadata = provider.getGroupRowMetadata(group);

        expect(metadata.format).toBe(provider.getOptions().totalsFormat);
        // starts after the summary column, and spans the columns that are not summaries
        expect(metadata.columns[1].colspan).toBe(2);

        grid.getData = () => ({ getGrouping: () => totalsGrouping(true) } as any);
        // with a totals row of its own, the group cell covers the summary columns as well
        expect(provider.getGroupRowMetadata(group).format).toBeUndefined();
        expect(provider.getGroupRowMetadata(group).columns[0].colspan).toBe("*");
    });

    it("returns group and totals row metadata using configured options", () => {
        const provider = new GroupItemMetadataProvider({ groupFocusable: false, totalsFocusable: true });
        const grid = mockGrid();
        grid.getColumns = () => [{ summaryType: 1 }, { id: "name" }];
        grid.getData = () => ({ getGrouping: () => totalsGrouping("groupheader", 4) } as any);
        provider.init(grid);

        const group = new Group();
        group.level = 3;
        const totals = new GroupTotals();
        totals.group = group;

        const groupMetadata = provider.getGroupRowMetadata(group);
        const totalsMetadata = provider.getTotalsRowMetadata(totals);

        expect(groupMetadata.selectable).toBe(false);
        expect(groupMetadata.focusable).toBe(false);
        expect(groupMetadata.format).toBe(provider.getOptions().totalsFormat);
        // the group cell starts after the summary column
        expect(groupMetadata.columns?.[1]?.editor).toBeNull();
        expect(totalsMetadata.focusable).toBe(true);
        expect(totalsMetadata.cssClasses).toContain("slick-group-level-3");
    });
});

describe("GroupItemMetadataProvider keyboard and lifecycle edge cases", () => {
    it("ignores keyboard expansion when disabled or already in the requested state", () => {
        const grid = mockGrid();
        grid.getActiveCell = () => ({ row: 1 });
        const event = mockEvent({ grid, key: "+" });
        const provider = new GroupItemMetadataProvider({ enableExpandCollapse: false });

        provider.handleGridKeyDown(event);
        expect(event.preventDefaultCalls).toBe(0);

        const enabled = new GroupItemMetadataProvider();
        grid.getActiveCell = () => ({ row: 1, cell: 0 });
        const expandedEvent = mockEvent({ grid, key: "+" });
        grid.getDataItem = () => {
            const group = new Group();
            group.collapsed = false;
            return group;
        };
        enabled.handleGridKeyDown(expandedEvent);
        expect(expandedEvent.preventDefaultCalls).toBe(1);
        expect(grid.__data.expandGroupCalls).toHaveLength(0);
        expect(grid.__data.collapseGroupCalls).toHaveLength(0);
    });

    it("can be destroyed before initialization and merges new options", () => {
        const provider = new GroupItemMetadataProvider({ groupCssClass: "initial" });
        provider.destroy();
        provider.setOptions({ groupCssClass: "updated" });
        expect(provider.getOptions().groupCssClass).toBe("updated");
    });

    it("handles keyboard expansion and collapse through the initialized grid", () => {
        const grid = mockGrid();
        grid.getActiveCell = () => ({ row: 1, cell: 0 });
        const provider = new GroupItemMetadataProvider();
        provider.init(grid);

        const expandEvent = mockEvent({ key: "+" });
        provider.handleGridKeyDown(expandEvent);
        expect(expandEvent.preventDefaultCalls).toBe(1);
        expect(grid.__data.expandGroupCalls).toEqual(["gk1"]);

        grid.getActiveCell = () => ({ row: 3, cell: 0 });
        const collapseEvent = mockEvent({ key: "-" });
        provider.handleGridKeyDown(collapseEvent);
        expect(collapseEvent.preventDefaultCalls).toBe(1);
        expect(grid.__data.collapseGroupCalls).toEqual(["gk3"]);
    });

    it("ignores keyboard events without a grid, active cell, or group item", () => {
        const noGrid = new GroupItemMetadataProvider();
        expect(() => noGrid.handleGridKeyDown(mockEvent({ key: "+" }))).not.toThrow();

        const grid = mockGrid();
        const provider = new GroupItemMetadataProvider();
        provider.init(grid);
        grid.getActiveCell = () => null;
        provider.handleGridKeyDown(mockEvent({ key: "+" }));
        grid.getActiveCell = () => ({ row: 333 });
        provider.handleGridKeyDown(mockEvent({ key: "+" }));
        expect(grid.__data.expandGroupCalls).toHaveLength(0);
    });

    it("ignores the space key when the active cell is not the group cell", () => {
        // the select column is skipped, so the group cell starts at the second column
        const grid = keyGrid();
        const provider = new GroupItemMetadataProvider();
        provider.init(grid);

        grid.getActiveCell = () => ({ row: 1, cell: 0 });
        const space = mockEvent({ key: " " });
        provider.handleGridKeyDown(space);

        expect(space.preventDefaultCalls).toBe(0);
        expect(grid.__data.expandGroupCalls).toHaveLength(0);

        grid.getActiveCell = () => ({ row: 1, cell: 1 });
        const onGroupCell = mockEvent({ key: " " });
        provider.handleGridKeyDown(onGroupCell);

        expect(onGroupCell.preventDefaultCalls).toBe(1);
        expect(grid.__data.expandGroupCalls).toEqual(["gk1"]);
    });

    it("toggles the group on the space key", () => {
        const grid = keyGrid();
        const provider = new GroupItemMetadataProvider();
        provider.init(grid);

        // row 1 is a collapsed group and row 3 is an expanded one
        grid.getActiveCell = () => ({ row: 1, cell: 1 });
        provider.handleGridKeyDown(mockEvent({ key: " " }));
        expect(grid.__data.expandGroupCalls).toEqual(["gk1"]);

        grid.getActiveCell = () => ({ row: 3, cell: 1 });
        provider.handleGridKeyDown(mockEvent({ key: " " }));
        expect(grid.__data.collapseGroupCalls).toEqual(["gk3"]);

        // now it is collapsed, so the same key expands it again
        const collapsed = new Group();
        collapsed.groupingKey = "gk3";
        collapsed.collapsed = true;
        grid.getDataItem = () => collapsed;
        provider.handleGridKeyDown(mockEvent({ key: " " }));
        expect(grid.__data.expandGroupCalls).toEqual(["gk1", "gk3"]);
    });

    it("only acts on + and - in the direction they are for", () => {
        const grid = keyGrid();
        const provider = new GroupItemMetadataProvider();
        provider.init(grid);

        // row 3 is expanded, so + has nothing to do, but the key is still ours
        grid.getActiveCell = () => ({ row: 3, cell: 1 });
        const plus = mockEvent({ key: "+" });
        provider.handleGridKeyDown(plus);
        expect(plus.preventDefaultCalls).toBe(1);
        expect(grid.__data.expandGroupCalls).toHaveLength(0);

        // row 1 is collapsed, so - has nothing to do either
        grid.getActiveCell = () => ({ row: 1, cell: 1 });
        const minus = mockEvent({ key: "-" });
        provider.handleGridKeyDown(minus);
        expect(minus.preventDefaultCalls).toBe(1);
        expect(grid.__data.collapseGroupCalls).toHaveLength(0);
    });

    it("collapses on the left arrow at the start of a group row and expands on the right arrow at its end", () => {
        const grid = keyGrid();
        const provider = new GroupItemMetadataProvider();
        provider.init(grid);

        grid.getActiveCell = () => ({ row: 3, cell: 0 });
        const left = mockEvent({ key: "ArrowLeft" });
        provider.handleGridKeyDown(left);

        expect(left.preventDefaultCalls).toBe(1);
        expect(grid.__data.collapseGroupCalls).toEqual(["gk3"]);

        grid.getActiveCell = () => ({ row: 1, cell: 1 });
        const right = mockEvent({ key: "ArrowRight" });
        provider.handleGridKeyDown(right);

        expect(right.preventDefaultCalls).toBe(1);
        expect(grid.__data.expandGroupCalls).toEqual(["gk1"]);
    });

    it("leaves the arrow keys to the grid when they navigate to another cell", () => {
        const grid = keyGrid();
        const provider = new GroupItemMetadataProvider();
        provider.init(grid);

        // the left arrow goes to the first cell, and the right one goes to the collapsed group
        grid.getActiveCell = () => ({ row: 3, cell: 1 });
        const left = mockEvent({ key: "ArrowLeft" });
        provider.handleGridKeyDown(left);

        grid.getActiveCell = () => ({ row: 1, cell: 0 });
        const right = mockEvent({ key: "ArrowRight" });
        provider.handleGridKeyDown(right);

        expect(left.preventDefaultCalls).toBe(0);
        expect(right.preventDefaultCalls).toBe(0);
        expect(grid.__data.collapseGroupCalls).toHaveLength(0);
        expect(grid.__data.expandGroupCalls).toHaveLength(0);
    });

    it("swaps the arrow keys at the ends of the row in an rtl grid", () => {
        const grid = keyGrid();
        grid.getOptions = () => ({ rtl: true });
        const provider = new GroupItemMetadataProvider();
        provider.init(grid);

        grid.getActiveCell = () => ({ row: 3, cell: 1 });
        provider.handleGridKeyDown(mockEvent({ key: "ArrowLeft" }));

        grid.getActiveCell = () => ({ row: 1, cell: 0 });
        provider.handleGridKeyDown(mockEvent({ key: "ArrowRight" }));

        expect(grid.__data.collapseGroupCalls).toEqual(["gk3"]);
        expect(grid.__data.expandGroupCalls).toEqual(["gk1"]);
    });

    it("handles missing optional data methods during keyboard expansion", () => {
        const grid = mockGrid();
        grid.getActiveCell = () => ({ row: 1, cell: 0 });
        grid.__data.setRefreshHints = undefined;
        grid.__data.expandGroup = undefined;
        const provider = new GroupItemMetadataProvider();
        provider.init(grid);

        expect(() => provider.handleGridKeyDown(mockEvent({ key: "+" }))).not.toThrow();
    });
});

describe("GroupItemMetadataProvider.groupCellPosition", () => {
    it("spans the whole row unless the totals of the group are on its row", () => {
        const provider = new GroupItemMetadataProvider();
        const grid = mockGrid();
        grid.getColumns = () => [{ id: "a" }, { id: "b" }, { id: "c" }];
        provider.init(grid);

        // a view that doesn't say where the totals are shown, the default of a grouping is a row of
        // their own, so the group cell spans the whole row
        expect(provider.getTotalsDisplay()).toBe(true);
        expect(provider.groupCellPosition()).toEqual({ cell: 0, colspan: "*" });

        // with the totals on the row of the group, but no summary columns, it still covers them all
        grid.getData = () => ({ getGrouping: () => totalsGrouping("groupheader") } as any);
        expect(provider.groupCellPosition()).toEqual({ cell: 0, colspan: 3 });

        grid.getData = () => ({ getGrouping: () => totalsGrouping(false) } as any);
        expect(provider.groupCellPosition()).toEqual({ cell: 0, colspan: "*" });
    });

    it("returns the default position when it has no grid or no columns", () => {
        const provider = new GroupItemMetadataProvider();
        expect(provider.groupCellPosition()).toEqual({ cell: 0, colspan: "*" });

        const withEmptyGrid = new GroupItemMetadataProvider();
        const grid = mockGrid();
        grid.getColumns = () => [];
        withEmptyGrid.init(grid);
        expect(withEmptyGrid.groupCellPosition()).toEqual({ cell: 0, colspan: "*" });
    });

    it("skips summary columns", () => {
        // the totals of the group are in the summary column, so the group cell starts after it
        const provider = new GroupItemMetadataProvider();
        const grid = mockGrid();
        grid.getColumns = () => [{ summaryType: 1 }, { id: "name" }];
        grid.getData = () => ({ getGrouping: () => totalsGrouping("groupheader") } as any);
        provider.init(grid);
        expect(provider.groupCellPosition()).toEqual({ cell: 1, colspan: 1 });
    });

    it("skips the columns marked with skipGroupCell", () => {
        const provider = new GroupItemMetadataProvider();
        const grid = withTotalsRow(mockGrid());
        grid.getColumns = () => [
            { id: "__select__", width: 200, skipGroupCell: true },
            { id: "__rowNumber__", width: 100, skipGroupCell: true },
            { id: "name" }
        ];
        provider.init(grid);

        expect(provider.groupCellPosition()).toEqual({ cell: 2, colspan: "*" });
        // the data rows of a group are indented in the same column as the group cell starts at
        const providerWithIndent = new GroupItemMetadataProvider({ indentDataRows: true });
        providerWithIndent.init(grid);
        expect(providerWithIndent.getDataRowMetadata(5, 2).columns[2]).toEqual({
            cssClass: "slick-group-data slick-group-level-2"
        });
    });

    it("skips narrow columns that have a fixed width, like a checkbox column", () => {
        const provider = new GroupItemMetadataProvider();
        const grid = withTotalsRow(mockGrid());
        grid.getColumns = () => [
            { id: "__select__", width: 27, minWidth: 27, maxWidth: 27, resizable: false },
            { id: "name", width: 200 }
        ];
        provider.init(grid);

        expect(provider.groupCellPosition()).toEqual({ cell: 1, colspan: "*" });
    });

    it("uses the declared width of a fixed column, so that resizing does not move the group cell", () => {
        const provider = new GroupItemMetadataProvider();
        const grid = withTotalsRow(mockGrid());
        const selectColumn: any = { id: "__select__", width: 90, minWidth: 27, maxWidth: 27 };
        grid.getColumns = () => [selectColumn, { id: "name" }];
        provider.init(grid);

        // minWidth is 27, which is the fixed width, so the current width does not matter
        expect(provider.groupCellPosition()).toEqual({ cell: 1, colspan: "*" });

        const noHeuristic = new GroupItemMetadataProvider({ minGroupCellWidth: 0 });
        noHeuristic.init(grid);
        expect(noHeuristic.groupCellPosition()).toEqual({ cell: 0, colspan: "*" });
    });

    it("does not skip a column whose minimum and maximum width are zero, as zero means not set", () => {
        const provider = new GroupItemMetadataProvider();
        const grid = withTotalsRow(mockGrid());
        grid.getColumns = () => [
            { id: "unset", width: 20, minWidth: 0, maxWidth: 0 },
            { id: "name" }
        ];
        provider.init(grid);

        expect(provider.groupCellPosition()).toEqual({ cell: 0, colspan: "*" });
        expect(provider.isGroupCellColumn(grid.getColumns()[0])).toBe(true);
    });

    it("does not skip a narrow column that is resizable, as its width is not fixed", () => {
        const provider = new GroupItemMetadataProvider();
        const grid = withTotalsRow(mockGrid());
        grid.getColumns = () => [{ id: "narrow", width: 20 }, { id: "name" }];
        provider.init(grid);

        expect(provider.groupCellPosition()).toEqual({ cell: 0, colspan: "*" });
    });

    it("keeps the default cell when every column is skipped", () => {
        const provider = new GroupItemMetadataProvider();
        const grid = mockGrid();
        grid.getColumns = () => [
            { id: "__select__", minWidth: 27, maxWidth: 27 },
            { id: "__rowNumber__", minWidth: 30, maxWidth: 30 }
        ];
        provider.init(grid);

        expect(provider.groupCellPosition()).toEqual({ cell: 0, colspan: "*" });
    });

    it("finds the first non-summary column and spans matching frozen columns", () => {
        const provider = new GroupItemMetadataProvider();
        const grid = mockGrid();
        grid.getData = () => ({ getGrouping: () => totalsGrouping("groupheader") } as any);
        grid.getColumns = () => [
            { summaryType: 1, frozen: true },
            { id: "first", frozen: true },
            { id: "second", frozen: true },
            { id: "summary", summaryType: 1, frozen: true },
            { id: "main", frozen: false }
        ];
        provider.init(grid);

        expect(provider.groupCellPosition()).toEqual({ cell: 1, colspan: 2 });
    });

    it("enforces a minimum colspan when the next column is a summary or boundary", () => {
        const provider = new GroupItemMetadataProvider();
        const grid = mockGrid();
        grid.getData = () => ({ getGrouping: () => totalsGrouping("groupheader") } as any);
        grid.getColumns = () => [
            { id: "first", frozen: true },
            { id: "summary", summaryType: 1, frozen: true }
        ];
        provider.init(grid);

        expect(provider.groupCellPosition()).toEqual({ cell: 0, colspan: 1 });
    });
});

describe("GroupItemMetadataProvider data row indentation", () => {
    it("indents data rows by default", () => {
        const provider = new GroupItemMetadataProvider();
        expect(provider.getDataRowMetadata(10, 2).columns[0].cssClass)
            .toBe("slick-group-data slick-group-level-2");
    });

    it("does not indent data rows when indentDataRows is disabled", () => {
        const provider = new GroupItemMetadataProvider({ indentDataRows: false });
        expect(provider.getDataRowMetadata(10, 2)).toBeNull();
    });

    it("returns null for rows that are not in a group", () => {
        const provider = new GroupItemMetadataProvider({ indentDataRows: true });
        expect(provider.getDataRowMetadata(0, 0)).toBeNull();
        expect(provider.getDataRowMetadata(0, undefined)).toBeNull();
    });

    it("marks the first cell with the level class and does not use an inline style", () => {
        const provider = new GroupItemMetadataProvider({ indentDataRows: true });
        const metadata = provider.getDataRowMetadata(10, 2);
        const cell = metadata.columns[0];

        expect(cell.cssClass).toBe("slick-group-data slick-group-level-2");
        // the inline style would be dropped by a content security policy
        expect(cell.style).toBeUndefined();
        // only the first cell is indented, the rest of the row stays aligned with the columns
        expect(Object.keys(metadata.columns)).toEqual(["0"]);
    });

    it("uses the configured css classes", () => {
        const provider = new GroupItemMetadataProvider({
            indentDataRows: true,
            dataCellCssClass: "my-indent",
            groupLevelPrefix: "lvl-"
        });
        const cell = provider.getDataRowMetadata(5, 3).columns[0];
        expect(cell.cssClass).toBe("my-indent lvl-3");
    });

    it("clamps levels deeper than the levels that have a css rule", () => {
        const provider = new GroupItemMetadataProvider({ indentDataRows: true });
        const cell = provider.getDataRowMetadata(5, GroupItemMetadataProvider.maxIndentLevel + 4).columns[0];
        expect(cell.cssClass).toBe("slick-group-data slick-group-level-8");
        expect(GroupItemMetadataProvider.clampIndentLevel(0)).toBe(0);
        expect(GroupItemMetadataProvider.clampIndentLevel(1)).toBe(1);
        expect(GroupItemMetadataProvider.clampIndentLevel(3)).toBe(3);
    });

    it("keeps the metadata it is given for the row", () => {
        const provider = new GroupItemMetadataProvider({ indentDataRows: true });
        const merged = provider.getDataRowMetadata(5, 2, {
            cssClasses: "deleted",
            columns: { 0: { cssClass: "c1", format: () => "a" } }
        } as any);

        expect(merged.cssClasses).toBe("deleted");
        expect(merged.columns[0].cssClass).toBe("c1 slick-group-data slick-group-level-2");
        expect(merged.columns[0].format).toBeTruthy();
    });
});

describe("GroupItemMetadataProvider.mergeMetadata", () => {
    it("returns the other side when one of them is empty", () => {
        const base: any = { cssClasses: "x" };
        expect(GroupItemMetadataProvider.mergeMetadata(base, null)).toBe(base);
        expect(GroupItemMetadataProvider.mergeMetadata(null, base)).toBe(base);
        expect(GroupItemMetadataProvider.mergeMetadata(null, null)).toBeNull();
    });

    it("concatenates row classes and merges per column metadata", () => {
        const merged = GroupItemMetadataProvider.mergeMetadata(
            { cssClasses: "deleted", columns: { 0: { cssClass: "c1", format: () => "a" } } },
            { cssClasses: "slick-group-2", columns: { 0: { cssClass: "c2", colspan: 2 } } });

        expect(merged.cssClasses).toBe("deleted slick-group-2");
        expect(merged.columns[0].cssClass).toBe("c1 c2");
        expect(merged.columns[0].colspan).toBe(2);
        expect(merged.columns[0].format).toBeTruthy();
    });
});
