import { blockUI, blockUndo } from "./blockui";

describe("blockUI", () => {

    it("blocks the page", async () => {
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

    it("if already blocked it just increments counter", async () => {
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

    it("ignores multiple blockUndo", () => {
        blockUI();
        let div = document.querySelector("div.blockUI.blockOverlay");
        expect(div).toBeDefined();
        blockUndo();
        div = document.querySelector("div.blockUI.blockOverlay");
        expect(div).toBe(null);
        blockUndo();
        div = document.querySelector("div.blockUI.blockOverlay");
        expect(div).toBe(null);
    });

});