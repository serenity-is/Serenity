import { ColumnFormat, CompatFormatter, formatterContext as ctx, Group, GroupTotals, type ISleekGrid } from "../../src/core";
import { GroupItemMetadataProvider } from "../../src/data/groupitemmetadataprovider";

describe("GroupItemMetadataProvider.defaults", () => {
    it("has expected default values", () => {
        expect(GroupItemMetadataProvider.defaults.enableExpandCollapse).toBe(true);
        expect(GroupItemMetadataProvider.defaults.groupIndentation).toBe(15);
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
        var old = GroupItemMetadataProvider.defaults.groupLevelPrefix;
        try {
            GroupItemMetadataProvider.defaults.groupLevelPrefix = "x-";
            var provider = new GroupItemMetadataProvider({ totalsCssClass: "y" });
            var options = provider.getOptions();
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
        var groupFormat: ColumnFormat = () => "ok";
        var provider = new GroupItemMetadataProvider({ groupFormat });
        expect(provider.getOptions().groupFormat === groupFormat).toBe(true);
    });

    it("uses groupFormat if both groupFormat and compat groupFormatter passed", () => {
        var groupFormat: ColumnFormat = () => "ok";
        var groupFormatter: CompatFormatter = () => "ok";
        var provider = new GroupItemMetadataProvider({ groupFormat, groupFormatter });
        expect(provider.getOptions().groupFormat === groupFormat).toBe(true);
    });

    it("uses converted compat groupFormatter if passed", () => {
        var groupFormatter: CompatFormatter = () => "compat";
        var provider = new GroupItemMetadataProvider({ groupFormatter });
        expect(provider.getOptions().groupFormatter === groupFormatter).toBe(true);
        expect(provider.getOptions().groupFormat as any !== groupFormatter).toBe(true);
        expect(provider.getOptions().groupFormat).toBeDefined();
        expect(provider.getOptions().groupFormat(ctx())).toBe("compat");
    });


    it("uses defaultGroupFormat if none passed", () => {
        var old = GroupItemMetadataProvider.defaultGroupFormat;
        try {
            GroupItemMetadataProvider.defaultGroupFormat = (ctx, o) => { opt = o; return ""; }
            var opt: any = null;
            var provider = new GroupItemMetadataProvider();
            var format = provider.getOptions().groupFormat;
            expect(format).toBeDefined();
            format(ctx());
            expect(opt === provider.getOptions()).toBe(true);
        }
        finally {
            GroupItemMetadataProvider.defaultGroupFormat = old;
        }
    });

    it("uses totalsFormat if passed", () => {
        var totalsFormat: ColumnFormat = () => "ok";
        var provider = new GroupItemMetadataProvider({ totalsFormat });
        expect(provider.getOptions().totalsFormat === totalsFormat).toBe(true);
    });

    it("uses totalsFormat if both totalsFormat and compat totalsFormatter passed", () => {
        var totalsFormat: ColumnFormat = () => "ok";
        var totalsFormatter: CompatFormatter = () => "ok";
        var provider = new GroupItemMetadataProvider({ totalsFormat, totalsFormatter });
        expect(provider.getOptions().totalsFormat === totalsFormat).toBe(true);
    });

    it("uses converted compat totalsFormatter if passed", () => {
        var totalsFormatter: CompatFormatter = () => "compat";
        var provider = new GroupItemMetadataProvider({ totalsFormatter });
        expect(provider.getOptions().totalsFormatter === totalsFormatter).toBe(true);
        expect(provider.getOptions().totalsFormat as any !== totalsFormatter).toBe(true);
        expect(provider.getOptions().totalsFormat).toBeDefined();
        expect(provider.getOptions().totalsFormat(ctx())).toBe("compat");
    });

    it("uses defaultTotalsFormat if none passed", () => {
        var old = GroupItemMetadataProvider.defaultTotalsFormat;
        try {
            GroupItemMetadataProvider.defaultTotalsFormat = (ctx, o) => { called = true; return ""; }
            var called = false;
            var provider = new GroupItemMetadataProvider();
            var format = provider.getOptions().totalsFormat;
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
        var provider = new GroupItemMetadataProvider({ totalsCssClass: "y", groupCssClass: "z" });
        var options = provider.getOptions();
        expect(options.totalsCssClass).toBe("y");
        expect(options.groupCssClass).toBe("z");
        provider.setOptions({ totalsCssClass: "w"});
        expect(options.totalsCssClass).toBe("w");
        expect(options.groupCssClass).toBe("z");
    });
});

function mockEvent(args: any) {
    var ev = {
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
                    var idx = ev.target.classNames.indexOf(s);
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
    var grid = {
        onClickList: <any[]>[],
        onClick: {
            subscribe: function(f: any) { grid.onClickList.push(f); },
            unsubscribe: function(f: any) {
                var idx = grid.onClickList.indexOf(f);
                expect(idx >= 0).toBe(true);
                grid.onClickList.splice(idx, 1);
            }
        } as any,
        onKeyDownList: <any[]>[],
        onKeyDown: {
            subscribe: function(f: any) { grid.onKeyDownList.push(f); },
            unsubscribe: function(f: any) {
                var idx = grid.onKeyDownList.indexOf(f);
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
                var group = new Group();
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
        groupTotalsFormatter: <any>null
    };
    return grid as any;
}

describe("GroupItemMetadataProvider.init", () => {

    it("attaches to onClick", () => {
        var plugin = new GroupItemMetadataProvider();
        var grid = mockGrid();
        plugin.init(grid);
        expect(grid.onClickList.length).toBe(1);
        plugin.destroy();
        expect(grid.onClickList.length).toBe(0);
    });

    it("attaches to onKeyDown", () => {
        var plugin = new GroupItemMetadataProvider();
        var grid = mockGrid();
        plugin.init(grid);
        expect(grid.onKeyDownList.length).toBe(1);
        plugin.destroy();
        expect(grid.onKeyDownList.length).toBe(0);
    });

});

describe("GroupItemMetadataProvider.handleGridClick", () => {

    it("ignores when args does not include grid", () => {
        var plugin = new GroupItemMetadataProvider();
        var grid = mockGrid();
        var event = mockEvent({});
        plugin.handleGridClick(event);
        expect(grid.getDataItemCalls).toBe(0);
        expect(event.stopImmediatePropagationCalls).toBe(0);
        expect(event.preventDefaultCalls).toBe(0);
    });

    it("uses initializing grid when args does not include grid", () => {
        var plugin = new GroupItemMetadataProvider();
        var grid = mockGrid();
        var event = mockEvent({ row: -1 });
        plugin.init(grid);
        plugin.handleGridClick(event);
        expect(grid.getDataItemCalls).toBe(1);
        expect(event.stopImmediatePropagationCalls).toBe(0);
        expect(event.preventDefaultCalls).toBe(0);
    });

    it("ignores when no item for args.row", () => {
        var plugin = new GroupItemMetadataProvider();
        var grid = mockGrid();
        var event = mockEvent({ row: -1 });
        plugin.init(grid);
        plugin.handleGridClick(event);
        expect(grid.getDataItemCalls).toBe(1);
        expect(event.stopImmediatePropagationCalls).toBe(0);
        expect(event.preventDefaultCalls).toBe(0);
    });

    it("ignores when item at args.row is not an instance of Group", () => {
        var plugin = new GroupItemMetadataProvider();
        var grid = mockGrid();
        var event = mockEvent({ row: 333 });
        plugin.init(grid);
        plugin.handleGridClick(event);
        expect(grid.getDataItemCalls).toBe(1);
        expect(event.stopImmediatePropagationCalls).toBe(0);
        expect(event.preventDefaultCalls).toBe(0);
    });

    it("ignores when event target does not contain toggle class", () => {
        var plugin = new GroupItemMetadataProvider();
        var grid = mockGrid();
        var event = mockEvent({ row: 1 });
        event.target.classNames = ["xyz"];
        plugin.init(grid);
        plugin.handleGridClick(event);
        expect(grid.getDataItemCalls).toBe(1);
        expect(event.stopImmediatePropagationCalls).toBe(0);
        expect(event.preventDefaultCalls).toBe(0);
    });

    it("calls stopImmediatePropagation, preventDefault and setRefreshHints", () => {
        var plugin = new GroupItemMetadataProvider();
        var grid = mockGrid();
        var event = mockEvent({ row: 1 });
        plugin.init(grid);
        plugin.handleGridClick(event);
        expect(grid.getDataItemCalls).toBe(1);
        expect(event.stopImmediatePropagationCalls).toBe(1);
        expect(event.preventDefaultCalls).toBe(1);
        expect(grid.__data.setRefreshHintsCalls.length).toBe(1);
        expect(grid.__data.setRefreshHintsCalls[0]).toStrictEqual({ ignoreDiffsBefore: 5, ignoreDiffsAfter: 14 });
    });

    it("calls stopImmediatePropagation, preventDefault and setRefreshHints", () => {
        var plugin = new GroupItemMetadataProvider();
        var grid = mockGrid();
        var event = mockEvent({ row: 1 });
        plugin.init(grid);
        plugin.handleGridClick(event);
        expect(grid.getDataItemCalls).toBe(1);
        expect(event.stopImmediatePropagationCalls).toBe(1);
        expect(event.preventDefaultCalls).toBe(1);
        expect(grid.__data.setRefreshHintsCalls.length).toBe(1);
        expect(grid.__data.setRefreshHintsCalls[0]).toStrictEqual({ ignoreDiffsBefore: 5, ignoreDiffsAfter: 14 });
    });

    it("calls expandGroup if collapsed is true", () => {
        var plugin = new GroupItemMetadataProvider();
        var grid = mockGrid();
        var event = mockEvent({ row: 1 });
        plugin.init(grid);
        plugin.handleGridClick(event);
        expect(grid.__data.collapseGroupCalls.length).toBe(0);
        expect(grid.__data.expandGroupCalls.length).toBe(1);
        expect(grid.__data.expandGroupCalls[0]).toBe("gk1");
    });

    it("calls collapseGroup if collapsed is falsy", () => {
        var plugin = new GroupItemMetadataProvider();
        var grid = mockGrid();
        var event = mockEvent({ row: 3 });
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

    it("returns group and totals row metadata using configured options", () => {
        const provider = new GroupItemMetadataProvider({ groupRowTotals: true, groupFocusable: false, totalsFocusable: true });
        const group = new Group();
        group.level = 3;
        const totals = new GroupTotals();
        totals.group = group;

        const groupMetadata = provider.getGroupRowMetadata(group);
        const totalsMetadata = provider.getTotalsRowMetadata(totals);

        expect(groupMetadata.selectable).toBe(false);
        expect(groupMetadata.focusable).toBe(false);
        expect(groupMetadata.format).toBe(provider.getOptions().totalsFormat);
        expect(groupMetadata.columns?.[0]?.editor).toBeNull();
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
        grid.getActiveCell = () => ({ row: 1 });
        const provider = new GroupItemMetadataProvider();
        provider.init(grid);

        const expandEvent = mockEvent({ key: "+" });
        provider.handleGridKeyDown(expandEvent);
        expect(expandEvent.preventDefaultCalls).toBe(1);
        expect(grid.__data.expandGroupCalls).toEqual(["gk1"]);

        grid.getActiveCell = () => ({ row: 3 });
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

    it("handles missing optional data methods during keyboard expansion", () => {
        const grid = mockGrid();
        grid.getActiveCell = () => ({ row: 1 });
        grid.__data.setRefreshHints = undefined;
        grid.__data.expandGroup = undefined;
        const provider = new GroupItemMetadataProvider();
        provider.init(grid);

        expect(() => provider.handleGridKeyDown(mockEvent({ key: "+" }))).not.toThrow();
    });
});

describe("GroupItemMetadataProvider.groupCellPosition", () => {
    it("returns the default position until group row totals are enabled", () => {
        const provider = new GroupItemMetadataProvider();
        expect(provider.groupCellPosition()).toEqual({ cell: 0, colspan: "*" });
    });

    it("finds the first non-summary column and spans matching frozen columns", () => {
        const provider = new GroupItemMetadataProvider({ groupRowTotals: true });
        const grid = mockGrid();
        grid.getColumns = () => [
            { summaryType: 1, frozen: true },
            { id: "first", frozen: true },
            { id: "second", frozen: true },
            { id: "summary", summaryType: 1, frozen: true },
            { id: "main", frozen: false }
        ];
        provider.init(grid);

        expect(provider.groupCellPosition()).toEqual({ cell: 1, colspan: 1 });
    });

    it("enforces a minimum colspan when the next column is a summary or boundary", () => {
        const provider = new GroupItemMetadataProvider({ groupRowTotals: true });
        const grid = mockGrid();
        grid.getColumns = () => [
            { id: "first", frozen: true },
            { id: "summary", summaryType: 1, frozen: true }
        ];
        provider.init(grid);

        expect(provider.groupCellPosition()).toEqual({ cell: 0, colspan: 1 });
    });
});

