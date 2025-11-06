import { ColumnFormat, CompatFormatter, formatterContext as ctx, Group, type ISleekGrid } from "../../src/core";
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

