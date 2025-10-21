import { FrozenLayout } from "../../src/layouts/frozenlayout";
import { mockLayoutHost } from "../mocks/mock-layout-host";

describe("FrozenLayout", () => {
    describe("headerrow visibility", () => {
        it("should not have headerrow in DOM when showHeaderRow is false", () => {
            const host = mockLayoutHost();
            host.opt.showHeaderRow = false;
            const layout = new FrozenLayout();
            layout.init(host);
            expect(host.container.querySelector(".slick-headerrow")).toBeNull();
        });

        it("should have headerrow in DOM when showHeaderRow is true", () => {
            const host = mockLayoutHost();
            host.opt.showHeaderRow = true;
            const layout = new FrozenLayout();
            layout.init(host);
            expect(host.container.querySelector(".slick-headerrow")).not.toBeNull();
        });

        it("should add/remove headerrow in DOM when showHeaderRow option is toggled", () => {
            const host = mockLayoutHost();
            host.opt.showHeaderRow = false;
            const layout = new FrozenLayout();
            layout.init(host);
            expect(host.container.querySelector(".slick-headerrow")).toBeNull();

            // toggle to true
            host.opt.showHeaderRow = true;
            expect(host.container.querySelector(".slick-headerrow")).not.toBeNull();

            // toggle back to false
            host.opt.showHeaderRow = false;
            expect(host.container.querySelector(".slick-headerrow")).toBeNull();
        });
    });

    describe("footerrow visibility", () => {
        it("should not have footerrow in DOM when showFooterRow is false", () => {
            const host = mockLayoutHost();
            host.opt.showFooterRow = false;
            const layout = new FrozenLayout();
            layout.init(host);
            expect(host.container.querySelector(".slick-footerrow")).toBeNull();
        });

        it("should have footerrow in DOM when showFooterRow is true", () => {
            const host = mockLayoutHost();
            host.opt.showFooterRow = true;
            const layout = new FrozenLayout();
            layout.init(host);
            expect(host.container.querySelector(".slick-footerrow")).not.toBeNull();
        });

        it("should add/remove footerrow in DOM when showFooterRow option is toggled", () => {
            const host = mockLayoutHost();
            host.opt.showFooterRow = false;
            const layout = new FrozenLayout();
            layout.init(host);
            expect(host.container.querySelector(".slick-footerrow")).toBeNull();

            // toggle to true
            host.opt.showFooterRow = true;
            expect(host.container.querySelector(".slick-footerrow")).not.toBeNull();

            // toggle back to false
            host.opt.showFooterRow = false;
            expect(host.container.querySelector(".slick-footerrow")).toBeNull();
        });
    });

    describe("top-panel visibility", () => {
        it("should not have top-panel in DOM when showTopPanel is false", () => {
            const host = mockLayoutHost();
            host.opt.showTopPanel = false;
            const layout = new FrozenLayout();
            layout.init(host);
            expect(host.container.querySelector(".slick-top-panel")).toBeNull();
        });

        it("should have top-panel in DOM when showTopPanel is true", () => {
            const host = mockLayoutHost();
            host.opt.showTopPanel = true;
            const layout = new FrozenLayout();
            layout.init(host);
            expect(host.container.querySelector(".slick-top-panel")).not.toBeNull();
        });

        it("should add/remove top-panel in DOM when showTopPanel option is toggled", () => {
            const host = mockLayoutHost();
            host.opt.showTopPanel = false;
            const layout = new FrozenLayout();
            layout.init(host);
            expect(host.container.querySelector(".slick-top-panel")).toBeNull();

            // toggle to true
            host.opt.showTopPanel = true;
            expect(host.container.querySelector(".slick-top-panel")).not.toBeNull();

            // toggle back to false
            host.opt.showTopPanel = false;
            expect(host.container.querySelector(".slick-top-panel")).toBeNull();
        });
    });
});
