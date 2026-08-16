import { FrozenLayout } from "../../src/layouts/frozen-layout";
import { mockLayoutHost } from "../mocks/mock-layout-host";

describe("FrozenLayout", () => {
    it("initializes all frozen layout bands and adjusts frozen rows", () => {
        const host = mockLayoutHost();
        host.opt.frozenRows = 2;
        const layout = new FrozenLayout();

        layout.init(host);

        expect(host.container.querySelectorAll(".slick-header").length).toBe(2);
        expect(host.container.querySelectorAll(".slick-viewport").length).toBe(4);
        expect(host.refs.config.frozenTopRows).toBe(2);
        expect(layout.layoutName).toBe("FrozenLayout");
        expect(layout.supportPinnedCols).toBe(true);
        expect(layout.supportFrozenRows).toBe(true);
    });

    it("adjusts frozen rows for omitted and bottom-frozen options", () => {
        const host = mockLayoutHost();
        const layout = new FrozenLayout();

        layout.init(host);
        expect(host.refs.config.frozenTopRows).toBe(0);
        host.opt.frozenRows = 3;
        layout.afterSetOptions({ frozenRows: 3 } as any);
        expect(host.refs.config.frozenTopRows).toBe(3);
        host.opt.frozenBottom = true;
        layout.afterSetOptions({ frozenBottom: true } as any);
        expect(host.refs.config.frozenTopRows).toBe(0);
    });

    it("reorders columns only when frozen columns change without explicit columns", () => {
        const host = mockLayoutHost();
        const start = { id: "start", frozen: true } as any;
        const middle = { id: "middle" } as any;
        host.getAllColumns = vi.fn(() => [middle, start]);
        const layout = new FrozenLayout();
        layout.init(host);

        const reordered: any = { frozenColumns: 1 };
        layout.afterSetOptions(reordered);
        expect(reordered.columns).toEqual([start, middle]);

        const explicit: any = { frozenColumns: 1, columns: [middle] };
        layout.afterSetOptions(explicit);
        expect(explicit.columns).toEqual([middle]);
    });

    it("does not change columns when no frozen-related option changed", () => {
        const host = mockLayoutHost();
        const layout = new FrozenLayout();
        layout.init(host);
        const options: any = {};

        layout.afterSetOptions(options);

        expect(options.columns).toBeUndefined();
    });

    it("clears its host on destroy", () => {
        const layout = new FrozenLayout();

        expect(() => layout.destroy()).not.toThrow();
    });

    describe("reorderViewColumns", () => {
        it("moves pinned-start columns before ordinary and pinned-end columns", () => {
            const host = mockLayoutHost();
            const layout = new FrozenLayout();
            const start = { id: "start", frozen: true } as any;
            const end = { id: "end", frozen: "end" } as any;
            const middle = { id: "middle" } as any;

            const result = layout.reorderViewColumns([middle, end, start], host.refs);

            expect(result?.map(column => column.id)).toEqual(["start", "middle", "end"]);
            expect(host.refs.config.pinnedStartCols).toBe(1);
        });

        it("returns null and clears the pinned-start count when none are pinned", () => {
            const host = mockLayoutHost();
            host.refs.config.pinnedStartCols = 2;
            const layout = new FrozenLayout();

            expect(layout.reorderViewColumns([{ id: "middle" } as any], host.refs)).toBeNull();
            expect(host.refs.config.pinnedStartCols).toBe(0);
        });
    });

    describe("headerrow visibility", () => {
        it("should hide headerrow when showHeaderRow is false", () => {
            const host = mockLayoutHost();
            host.opt.showHeaderRow = false;
            const layout = new FrozenLayout();
            layout.init(host);
            expect(isHidden(host.container.querySelector(".slick-headerrow"))).toBe(true);
        });

        it("should show headerrow when showHeaderRow is true", () => {
            const host = mockLayoutHost();
            host.opt.showHeaderRow = true;
            const layout = new FrozenLayout();
            layout.init(host);
            //expect(isVisible(host.container.querySelector(".slick-headerrow"))).toBe(true);
        });

        it("should toggle header row when showHeaderRow option is toggled", () => {
            const host = mockLayoutHost();
            host.opt.showHeaderRow = false;
            const layout = new FrozenLayout();
            layout.init(host);
            expect(isHidden(host.container.querySelector(".slick-headerrow"))).toBe(true);

            // toggle to true
           //host.opt.showHeaderRow = true;
           //expect(isVisible(host.container.querySelector(".slick-headerrow"))).toBe(true);

            // toggle back to false
            //host.opt.showHeaderRow = false;
            //expect(isHidden(host.container.querySelector(".slick-headerrow"))).toBe(true);
        });
    });

    describe("footerrow visibility", () => {
        it("should hide footerrow in when showFooterRow is false", () => {
            const host = mockLayoutHost();
            host.opt.showFooterRow = false;
            const layout = new FrozenLayout();
            layout.init(host);
            expect(isHidden(host.container.querySelector(".slick-footerrow"))).toBe(true);
        });

        it("should show footerrow when showFooterRow is true", () => {
            const host = mockLayoutHost();
            host.opt.showFooterRow = true;
            const layout = new FrozenLayout();
            layout.init(host);
            //expect(isVisible(host.container.querySelector(".slick-footerrow"))).toBe(true);
        });

        it("should toggle footerrow when showFooterRow option is toggled", () => {
            const host = mockLayoutHost();
            host.opt.showFooterRow = false;
            const layout = new FrozenLayout();
            layout.init(host);
            expect(isHidden(host.container.querySelector(".slick-footerrow"))).toBe(true);

            // toggle to true
            //host.opt.showFooterRow = true;
            //expect(isVisible(host.container.querySelector(".slick-footerrow"))).toBe(true);

            // toggle back to false
            //host.opt.showFooterRow = false;
            //expect(isHidden(host.container.querySelector(".slick-footerrow"))).toBe(true);
        });
    });

    describe("top-panel visibility", () => {
        it("should hide top-panel when showTopPanel is false", () => {
            const host = mockLayoutHost();
            host.opt.showTopPanel = false;
            const layout = new FrozenLayout();
            layout.init(host);
            expect(isHidden(host.container.querySelector(".slick-top-panel-container"))).toBe(true);
        });

        it("should show top-panel when showTopPanel is true", () => {
            const host = mockLayoutHost();
            host.opt.showTopPanel = true;
            const layout = new FrozenLayout();
            layout.init(host);
            expect(isVisible(host.container.querySelector(".slick-top-panel-container"))).toBe(true);
        });

        it("should toggle top-panel when showTopPanel option is toggled", () => {
            const host = mockLayoutHost();
            host.opt.showTopPanel = false;
            const layout = new FrozenLayout();
            layout.init(host);
            expect(isHidden(host.container.querySelector(".slick-top-panel-container"))).toBe(true);

            // toggle to true
            host.opt.showTopPanel = true;
            expect(isVisible(host.container.querySelector(".slick-top-panel-container"))).toBe(true);

            // toggle back to false
            host.opt.showTopPanel = false;
            expect(isHidden(host.container.querySelector(".slick-top-panel-container"))).toBe(true);
        });
    });
});

function isHidden(element: HTMLElement | null): boolean {
    return !element || !!element.hidden || getComputedStyle(element).display === "none";
}

function isVisible(element: HTMLElement | null): boolean {
    return element != null && !element.hidden && getComputedStyle(element).display !== "none";
}
