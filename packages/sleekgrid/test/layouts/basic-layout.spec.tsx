import { mockLayoutHost } from "../mocks/mock-layout-host";
import { BasicLayout } from "../../src/grid";

describe("BasicLayout", () => {
    describe("headerrow visibility", () => {
        it("should hide headerrow when showHeaderRow is false", () => {
            const host = mockLayoutHost();
            host.opt.showHeaderRow = false;
            const layout = new BasicLayout();
            layout.init(host);
            expect(isHidden(host.container.querySelector(".slick-headerrow"))).toBe(true);
        });

        it("should show headerrow when showHeaderRow is true", () => {
            const host = mockLayoutHost();
            host.opt.showHeaderRow = true;
            const layout = new BasicLayout();
            layout.init(host);
            expect(isVisible(host.container.querySelector(".slick-headerrow"))).toBe(true);
        });

        it("should toggle header row when showHeaderRow option is toggled", () => {
            const host = mockLayoutHost();
            host.opt.showHeaderRow = false;
            const layout = new BasicLayout();
            layout.init(host);
            expect(isHidden(host.container.querySelector(".slick-headerrow"))).toBe(true);

            // toggle to true
            host.opt.showHeaderRow = true;
            expect(isVisible(host.container.querySelector(".slick-headerrow"))).toBe(true);

            // toggle back to false
            host.opt.showHeaderRow = false;
            expect(isHidden(host.container.querySelector(".slick-headerrow"))).toBe(true);
        });
    });

    describe("footerrow visibility", () => {
        it("should hide footerrow in when showFooterRow is false", () => {
            const host = mockLayoutHost();
            host.opt.showFooterRow = false;
            const layout = new BasicLayout();
            layout.init(host);
            expect(isHidden(host.container.querySelector(".slick-footerrow"))).toBe(true);
        });

        it("should show footerrow when showFooterRow is true", () => {
            const host = mockLayoutHost();
            host.opt.showFooterRow = true;
            const layout = new BasicLayout();
            layout.init(host);
            expect(isVisible(host.container.querySelector(".slick-footerrow"))).toBe(true);
        });

        it("should toggle footerrow when showFooterRow option is toggled", () => {
            const host = mockLayoutHost();
            host.opt.showFooterRow = false;
            const layout = new BasicLayout();
            layout.init(host);
            expect(isHidden(host.container.querySelector(".slick-footerrow"))).toBe(true);

            // toggle to true
            host.opt.showFooterRow = true;
            expect(isVisible(host.container.querySelector(".slick-footerrow"))).toBe(true);

            // toggle back to false
            host.opt.showFooterRow = false;
            expect(isHidden(host.container.querySelector(".slick-footerrow"))).toBe(true);
        });
    });

    describe("top-panel visibility", () => {
        it("should hide top-panel when showTopPanel is false", () => {
            const host = mockLayoutHost();
            host.opt.showTopPanel = false;
            const layout = new BasicLayout();
            layout.init(host);
            expect(isHidden(host.container.querySelector(".slick-top-panel-container"))).toBe(true);
        });

        it("should show top-panel when showTopPanel is true", () => {
            const host = mockLayoutHost();
            host.opt.showTopPanel = true;
            const layout = new BasicLayout();
            layout.init(host);
            expect(isVisible(host.container.querySelector(".slick-top-panel-container"))).toBe(true);
        });

        it("should toggle top-panel when showTopPanel option is toggled", () => {
            const host = mockLayoutHost();
            host.opt.showTopPanel = false;
            const layout = new BasicLayout();
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
    return !element || element.hidden || getComputedStyle(element).display === "none";
}

function isVisible(element: HTMLElement | null): boolean {
    return element != null && !element.hidden && getComputedStyle(element).display !== "none";
}
