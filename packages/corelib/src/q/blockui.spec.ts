
function mockJQuery(myJQuery: JQuery) {
    jest.unmock('@optionaldeps/jquery')
    jest.mock('@optionaldeps/jquery', () => {
        return {
            __esModule: true,
            default: myJQuery
        }
    });
}

describe("Q.blockUI", () => {

    beforeEach(() => {
        jest.unmock("@optionaldeps/jquery");
        jest.resetModules();
    })

    it("works without jQuery loaded", async () => {
        mockJQuery(undefined);
        const blockui = (await import("./blockui"));
        const blockUI = blockui.blockUI;
        const blockUndo = blockui.blockUndo;
        var $ = (await import('@optionaldeps/jquery')).default;
        expect(typeof $).toBe("undefined");
        blockUI({});
        try {
            var div = document.querySelector("div.blockUI.blockOverlay") as HTMLDivElement;
            expect(div).toBeDefined();
            expect(div.style?.length).toBeDefined();
            expect(div.style?.zIndex).toBe("2000");
        }
        finally {
            blockUndo();
        }
        div = document.querySelector("div.blockUI.blockOverlay") as HTMLDivElement;
        expect(div).toBeNull();
    });

    it("works with dummy jQuery but no blockUI", async () => {
        var myJQuery = function () {
        } as any;
        (myJQuery as any).fn = {};
        mockJQuery(myJQuery);
        const blockui = (await import("./blockui"));
        const blockUI = blockui.blockUI;
        const blockUndo = blockui.blockUndo;
        var $ = (await import('@optionaldeps/jquery')).default;
        expect(typeof $).toBe("function");

        blockUI({});
        try {
            var div = document.querySelector("div.blockUI.blockOverlay") as HTMLDivElement;
            expect(div).toBeDefined();
            expect(div.style?.length).toBeDefined();
            expect(div.style?.zIndex).toBe("2000");
        }
        finally {
            blockUndo();
        }
        div = document.querySelector("div.blockUI.blockOverlay") as HTMLDivElement;
        expect(div).toBeNull();
    });

    it("can use jQuery blockUI if available", async () => {
        var myJQuery = function () {
        } as any;
        (myJQuery as any).blockUI = function () { };
        (myJQuery as any).unblockUI = function () { };
        mockJQuery(myJQuery);
        var mockBlock = jest.spyOn(myJQuery, "blockUI");
        var mockUnblock = jest.spyOn(myJQuery, "unblockUI");
        const blockui = (await import("./blockui"));
        const blockUI = blockui.blockUI;
        const blockUndo = blockui.blockUndo;
        var $ = (await import('@optionaldeps/jquery')).default;
        expect(typeof $).toBe("function");
        expect(typeof ($ as any).blockUI).toBe("function");

        blockUI({});
        try {
            expect(mockBlock.mock.calls.length).toBe(1);
            expect(mockBlock.mock.calls[0]).toEqual([{
                "baseZ": 2000,
                "fadeOut": 0,
                "message": "",
                "overlayCSS": {
                    "cursor": "wait",
                    "opacity": "0.0",
                    "zIndex": 2000,
                }
            }]);
        }
        finally {
            blockUndo();
        }
        expect(mockBlock.mock.calls.length).toBe(1);
        expect(mockUnblock.mock.calls.length).toBe(1);
        expect(mockUnblock.mock.calls[0]).toEqual([{
            fadeOut: 0
        }]);
    });

    it("if already blocked it just increments counter", async () => {
        mockJQuery(undefined);
        const blockui = (await import("./blockui"));
        const blockUI = blockui.blockUI;
        const blockUndo = blockui.blockUndo;
        blockUI({});
        try {
            var div = document.querySelector("div.blockUI.blockOverlay") as HTMLDivElement;
            expect(div).toBeDefined();
            blockUI({});
            var div2 = document.querySelector("div.blockUI.blockOverlay") as HTMLDivElement;
            expect(document.querySelectorAll("div.blockUI").length).toBe(1);
            expect(div2 === div).toBe(true);
            blockUndo();
            var div3 = document.querySelector("div.blockUI.blockOverlay") as HTMLDivElement;
            expect(div3 === div).toBe(true);
            expect(document.querySelectorAll("div.blockUI").length).toBe(1);
        }
        finally {
            blockUndo();
        }
        div = document.querySelector("div.blockUI.blockOverlay") as HTMLDivElement;
        expect(div).toBeNull();
    });

    it("does not fail if blockUI div is manually removed", async () => {
        mockJQuery(undefined);
        const blockui = (await import("./blockui"));
        const blockUI = blockui.blockUI;
        const blockUndo = blockui.blockUndo;
        var $ = (await import('@optionaldeps/jquery')).default;
        expect(typeof $).toBe("undefined");
        blockUI({});
        try {
            var div = document.querySelector("div.blockUI.blockOverlay") as HTMLDivElement;
            expect(div).toBeDefined();
            expect(div.style?.length).toBeDefined();
            expect(div.style?.zIndex).toBe("2000");
            document.querySelector("div.blockUI.blockOverlay").remove();
        }
        finally {
            blockUndo();
        }
        div = document.querySelector("div.blockUI.blockOverlay") as HTMLDivElement;
        expect(div).toBeNull();
    });

    it("uses window.setTimeout if useTimeout is passed as true", async () => {
        mockJQuery(undefined);
        const blockui = (await import("./blockui"));
        const blockUI = blockui.blockUI;
        const blockUndo = blockui.blockUndo;
        var $ = (await import('@optionaldeps/jquery')).default;
        expect(typeof $).toBe("undefined");
        var oldTimeout = window.setTimeout;
        var timeoutCalls = 0;
        var timeoutMs;
        window.setTimeout = function(callback: Function, ms) {
            timeoutMs = ms;
            timeoutCalls++;
            callback();
            return 1;
        }
        blockUI({
            useTimeout: true
        });
        try {
            expect(timeoutCalls).toBe(1);
            expect(timeoutMs).toBe(0);
            var div = document.querySelector("div.blockUI.blockOverlay") as HTMLDivElement;
            expect(div).toBeDefined();
            expect(div.style?.length).toBeDefined();
            expect(div.style?.zIndex).toBe("2000");
            document.querySelector("div.blockUI.blockOverlay").remove();
        }
        finally {
            window.setTimeout = oldTimeout;
            blockUndo();
        }
        div = document.querySelector("div.blockUI.blockOverlay") as HTMLDivElement;
        expect(div).toBeNull();
    });

});

export {}