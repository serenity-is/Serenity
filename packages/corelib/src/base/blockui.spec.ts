import { blockUI, blockUndo } from "./blockui";

describe("blockUI", () => {

    it("blocks the page", async () => {
        blockUI({});
        try {
            const div = document.querySelector("div.blockUI.blockOverlay") as HTMLDivElement;
            expect(div).toBeDefined();
            expect(div.style?.length).toBeDefined();
            expect(div.style?.zIndex).toBe("2000");
        }
        finally {
            blockUndo();
        }
        expect(document.querySelector("div.blockUI.blockOverlay")).toBeNull();
    });

    it("if already blocked it just increments counter", async () => {
        blockUI({});
        try {
            const div = document.querySelector("div.blockUI.blockOverlay") as HTMLDivElement;
            expect(div).toBeDefined();
            blockUI({});
            const div2 = document.querySelector("div.blockUI.blockOverlay") as HTMLDivElement;
            expect(document.querySelectorAll("div.blockUI").length).toBe(1);
            expect(div2 === div).toBe(true);
            blockUndo();
            const div3 = document.querySelector("div.blockUI.blockOverlay") as HTMLDivElement;
            expect(div3 === div).toBe(true);
            expect(document.querySelectorAll("div.blockUI").length).toBe(1);
        }
        finally {
            blockUndo();
        }
        expect(document.querySelector("div.blockUI.blockOverlay")).toBeNull();
    });

    it("does not fail if blockUI div is manually removed", async () => {
        blockUI({});
        try {
            const div = document.querySelector("div.blockUI.blockOverlay") as HTMLDivElement;
            expect(div).toBeDefined();
            expect(div.style?.length).toBeDefined();
            expect(div.style?.zIndex).toBe("2000");
            document.querySelector("div.blockUI.blockOverlay").remove();
        }
        finally {
            blockUndo();
        }
        expect(document.querySelector("div.blockUI.blockOverlay")).toBeNull();
    });

    it("uses window.setTimeout if useTimeout is passed as true", async () => {
        const oldTimeout = window.setTimeout;
        let timeoutCalls = 0;
        let timeoutMs: number;
        window.setTimeout = function (callback: Function, ms: number) {
            timeoutMs = ms;
            timeoutCalls++;
            callback();
            return 1;
        } as any;
        blockUI({
            useTimeout: true
        });
        try {
            expect(timeoutCalls).toBe(1);
            expect(timeoutMs).toBe(0);
            const div = document.querySelector("div.blockUI.blockOverlay") as HTMLDivElement;
            expect(div).toBeDefined();
            expect(div.style?.length).toBeDefined();
            expect(div.style?.zIndex).toBe("2000");
            document.querySelector("div.blockUI.blockOverlay").remove();
        }
        finally {
            window.setTimeout = oldTimeout;
            blockUndo();
        }
        expect(document.querySelector("div.blockUI.blockOverlay")).toBeNull();
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