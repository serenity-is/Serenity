import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { PagerTexts } from "../../base";
import { SlickPager } from "./slickpager";

function createMockView() {
    const pagingInfo: any = { page: 1, rowsPerPage: 20, totalCount: 100, loading: false, error: null };
    const view: any = {
        loading: false,
        rowsPerPage: 20,
        pagingInfo,
        getPagingInfo: vi.fn(() => view.pagingInfo),
        setPagingOptions: vi.fn(),
        onPagingInfoChanged: { subscribe: vi.fn(), unsubscribe: vi.fn() },
        populate: vi.fn()
    };
    return view;
}

function createPager(view: any, options?: any): SlickPager<any> {
    const el = document.createElement("div");
    document.body.appendChild(el);
    return new SlickPager({ view, element: el, ...options } as any);
}

describe("SlickPager", () => {
    beforeEach(() => {
        document.body.innerHTML = "";
    });

    afterEach(() => {
        document.body.innerHTML = "";
        vi.restoreAllMocks();
    });

    describe("constructor", () => {
        it("throws when no view option is set", () => {
            const el = document.createElement("div");
            document.body.appendChild(el);
            expect(() => new SlickPager({ element: el } as any)).toThrow();
        });

        it("creates pager and renders into the element", () => {
            const view = createMockView();
            const pager = createPager(view);
            expect(pager).toBeInstanceOf(SlickPager);
            expect(pager.element.hasClass("s-SlickPager")).toBe(true);
            pager.destroy();
        });

        it("subscribes to onPagingInfoChanged", () => {
            const view = createMockView();
            const pager = createPager(view);
            expect(view.onPagingInfoChanged.subscribe).toHaveBeenCalled();
            pager.destroy();
        });

        it("uses onRowsPerPageChange option when provided", () => {
            const view = createMockView();
            const onRowsPerPageChange = vi.fn();
            const pager = createPager(view, { onRowsPerPageChange });
            const select = document.querySelector<HTMLSelectElement>(".slick-pg-size")!;
            select.value = "100";
            select.dispatchEvent(new Event("change"));
            expect(onRowsPerPageChange).toHaveBeenCalledWith(100);
            pager.destroy();
        });

        it("updates view paging options on rows per page change without callback", () => {
            const view = createMockView();
            const pager = createPager(view);
            const select = document.querySelector<HTMLSelectElement>(".slick-pg-size")!;
            select.value = "100";
            select.dispatchEvent(new Event("change"));
            expect(view.setPagingOptions).toHaveBeenCalledWith({ page: 1, rowsPerPage: 100 });
            pager.destroy();
        });
    });

    describe("_changePage", () => {
        it("navigates to first page", () => {
            const view = createMockView();
            view.pagingInfo.page = 2;
            const pager = createPager(view);
            pager._changePage("first");
            expect(view.setPagingOptions).toHaveBeenCalledWith({ page: 1 });
            pager.destroy();
        });

        it("returns false when already on first page", () => {
            const view = createMockView();
            view.pagingInfo.page = 1;
            const pager = createPager(view);
            expect(pager._changePage("first")).toBe(false);
            expect(view.setPagingOptions).not.toHaveBeenCalled();
            pager.destroy();
        });

        it("navigates to previous page", () => {
            const view = createMockView();
            view.pagingInfo.page = 2;
            const pager = createPager(view);
            pager._changePage("prev");
            expect(view.setPagingOptions).toHaveBeenCalledWith({ page: 1 });
            pager.destroy();
        });

        it("navigates to next page", () => {
            const view = createMockView();
            view.pagingInfo.page = 1;
            const pager = createPager(view);
            pager._changePage("next");
            expect(view.setPagingOptions).toHaveBeenCalledWith({ page: 2 });
            pager.destroy();
        });

        it("navigates to last page", () => {
            const view = createMockView();
            view.pagingInfo.page = 1;
            const pager = createPager(view);
            pager._changePage("last");
            expect(view.setPagingOptions).toHaveBeenCalledWith({ page: 5 });
            pager.destroy();
        });

        it("navigates from input value", () => {
            const view = createMockView();
            const pager = createPager(view);
            pager["currentPage"].value = "3";
            pager._changePage("input");
            expect(view.setPagingOptions).toHaveBeenCalledWith({ page: 3 });
            expect(pager["currentPage"].value).toBe("3");
            pager.destroy();
        });

        it("coerces invalid input to first page", () => {
            const view = createMockView();
            view.pagingInfo.page = 2;
            const pager = createPager(view);
            pager["currentPage"].value = "abc";
            pager._changePage("input");
            expect(view.setPagingOptions).toHaveBeenCalledWith({ page: 1 });
            expect(pager["currentPage"].value).toBe("1");
            pager.destroy();
        });

        it("coerces input below 1 to first page", () => {
            const view = createMockView();
            view.pagingInfo.page = 2;
            const pager = createPager(view);
            pager["currentPage"].value = "0";
            pager._changePage("input");
            expect(view.setPagingOptions).toHaveBeenCalledWith({ page: 1 });
            pager.destroy();
        });

        it("coerces input above last page to last page", () => {
            const view = createMockView();
            const pager = createPager(view);
            pager["currentPage"].value = "999";
            pager._changePage("input");
            expect(view.setPagingOptions).toHaveBeenCalledWith({ page: 5 });
            pager.destroy();
        });

        it("does nothing while view is loading", () => {
            const view = createMockView();
            view.loading = true;
            const pager = createPager(view);
            expect(pager._changePage("next")).toBe(true);
            expect(view.setPagingOptions).not.toHaveBeenCalled();
            pager.destroy();
        });

        it("does nothing when navigating prev on the first page", () => {
            const view = createMockView();
            view.pagingInfo.page = 1;
            const pager = createPager(view);
            expect(pager._changePage("prev")).toBe(false);
            expect(view.setPagingOptions).not.toHaveBeenCalled();
            pager.destroy();
        });

        it("does nothing when navigating next on the last page", () => {
            const view = createMockView();
            view.pagingInfo.page = 5;
            const pager = createPager(view);
            expect(pager._changePage("next")).toBe(false);
            expect(view.setPagingOptions).not.toHaveBeenCalled();
            pager.destroy();
        });

        it("does not call onChangePage when navigating prev on the first page", () => {
            const view = createMockView();
            view.pagingInfo.page = 1;
            const onChangePage = vi.fn();
            const pager = createPager(view, { onChangePage });
            pager._changePage("prev");
            expect(onChangePage).not.toHaveBeenCalled();
            pager.destroy();
        });

        it("calls onChangePage option instead of setPagingOptions", () => {
            const view = createMockView();
            view.pagingInfo.page = 2;
            const onChangePage = vi.fn();
            const pager = createPager(view, { onChangePage });
            pager._changePage("first");
            expect(onChangePage).toHaveBeenCalledWith(1);
            expect(view.setPagingOptions).not.toHaveBeenCalled();
            pager.destroy();
        });
    });

    describe("indeterminate paging", () => {
        function createIndeterminate(info: any) {
            const view = createMockView();
            view.pagingInfo = { page: 1, rowsPerPage: 20, totalCount: 21, loading: false, error: null, indeterminate: true, hasMore: true, ...info };
            return { view, pager: createPager(view, { indeterminatePaging: true }) };
        }

        it("has no last page button or total of the pages", () => {
            const { pager } = createIndeterminate({});
            expect(document.body.querySelector(".slick-pg-last")).toBeNull();
            expect(document.body.querySelector(".slick-pg-total")).toBeNull();
            pager.destroy();
        });

        it("shows the range, and only goes to the next page while there are more rows", () => {
            const { view, pager } = createIndeterminate({ page: 5, totalCount: 101 });
            pager._updatePager();
            expect(pager["stat"].textContent).toBe(PagerTexts.RangeStatus.replace(/{from}/, "81").replace(/{to}/, "100"));
            expect(pager["nextButton"].classList.contains("slick-pg-disabled")).toBe(false);

            pager._changePage("next");
            expect(view.setPagingOptions).toHaveBeenCalledWith({ page: 6 });

            view.setPagingOptions.mockClear();
            view.pagingInfo = { ...view.pagingInfo, page: 6, totalCount: 105, hasMore: false };
            pager._updatePager();
            expect(pager["stat"].textContent).toBe(PagerTexts.RangeStatus.replace(/{from}/, "101").replace(/{to}/, "105"));
            expect(pager["nextButton"].classList.contains("slick-pg-disabled")).toBe(true);

            pager._changePage("next");
            expect(view.setPagingOptions).not.toHaveBeenCalled();
            pager.destroy();
        });

        it("takes any page number, as the last page is not known", () => {
            const { view, pager } = createIndeterminate({});
            pager["currentPage"].value = "50";
            pager._changePage("input");
            expect(view.setPagingOptions).toHaveBeenCalledWith({ page: 50 });
            pager.destroy();
        });
    });

    describe("destroy", () => {
        it("removes its subscription to the view, so a view that outlives it does not update it", () => {
            const view = createMockView();
            const unsubscribe = vi.fn();
            view.onPagingInfoChanged = {
                subscribe: vi.fn((handler: () => void) => view.subscribed = handler),
                unsubscribe
            };
            const pager = createPager(view);
            const handler = view.subscribed;
            expect(handler).toBeTypeOf("function");

            pager.destroy();
            expect(unsubscribe).toHaveBeenCalledWith(handler);

            // the view may not have a pager anymore, so it should not call it
            expect(pager["pagingInfoChanged"]).toBeNull();
        });
    });

    describe("layout", () => {
        it("has the status at the start of the row, and the controls, with the reload, at the end", () => {
            const pager = createPager(createMockView(), { showFirstLast: true });
            const row = pager.domNode.querySelector(".slick-pg-in");
            expect(Array.from(row.children).map(x => x.className.split(" ")[0]))
                .toEqual(["slick-pg-info", "slick-pg-nav"]);

            const info = row.querySelector(".slick-pg-info");
            expect(Array.from(info.children).map(x => x.className.split(" ")[0]))
                .toEqual(["slick-pg-stat"]);

            // the extra content comes before the rows per page selector and the page buttons
            const nav = row.querySelector(".slick-pg-nav");
            expect(Array.from(nav.children).map(x => x.className.split(" ")[0] ?? x.className))
                .toEqual(["slick-pg-extra", "slick-pg-size", "slick-pg-reload", "slick-pg-first", "slick-pg-prev",
                    "slick-pg-control", "slick-pg-next", "slick-pg-last"]);
            expect((nav.querySelector(".slick-pg-extra") as HTMLElement).hidden).toBe(true);
            const size = nav.querySelector<HTMLSelectElement>(".slick-pg-size");
            expect(size.title).toBe(PagerTexts.RowsPerPage);
            expect(size.getAttribute("aria-label")).toBe(PagerTexts.RowsPerPage);
            expect(nav.querySelector(".slick-pg-rows-label")).toBeNull();
            expect(row.querySelector(".slick-pg-grp")).toBeNull();
            expect(pager.domNode.classList.contains("slick-pg-range")).toBe(false);
            pager.destroy();
        });

        it("leaves out the first and last page buttons by default", () => {
            const pager = createPager(createMockView());
            expect(pager.domNode.querySelector(".slick-pg-first")).toBeNull();
            expect(pager.domNode.querySelector(".slick-pg-last")).toBeNull();
            expect(pager.domNode.querySelector(".slick-pg-prev")).not.toBeNull();
            expect(pager.domNode.querySelector(".slick-pg-next")).not.toBeNull();
            expect(SlickPager.defaults.showFirstLast).toBe(false);
            pager.destroy();
        });

        it("shows the first and last page buttons when the option asks for them", () => {
            const pager = createPager(createMockView(), { showFirstLast: true });
            expect(pager.domNode.querySelector(".slick-pg-first")).not.toBeNull();
            expect(pager.domNode.querySelector(".slick-pg-last")).not.toBeNull();
            pager.destroy();
        });

        it("shows the first and last page buttons when the defaults ask for them", () => {
            SlickPager.defaults.showFirstLast = true;
            try {
                const pager = createPager(createMockView());
                expect(pager.domNode.querySelector(".slick-pg-first")).not.toBeNull();
                expect(pager.domNode.querySelector(".slick-pg-last")).not.toBeNull();

                // the option still wins over the defaults
                const without = createPager(createMockView(), { showFirstLast: false });
                expect(without.domNode.querySelector(".slick-pg-first")).toBeNull();
                pager.destroy();
                without.destroy();
            }
            finally {
                SlickPager.defaults.showFirstLast = false;
            }
        });

        it("has no page navigation and says so for a range view", () => {
            const view = createMockView();
            view.pagingInfo = { ...view.pagingInfo, pagingMode: "rows", rowRange: { from: 1, to: 20, total: 100 } };
            const pager = createPager(view);
            expect(pager.domNode.querySelector(".slick-pg-first")).toBeNull();
            expect(pager.domNode.querySelector(".slick-pg-current")).toBeNull();
            expect(pager.domNode.querySelector(".slick-pg-last")).toBeNull();
            expect(pager.domNode.classList.contains("slick-pg-range")).toBe(true);
            expect(pager.domNode.querySelector(".slick-pg-info .slick-pg-stat")).not.toBeNull();
            // the number of the rows it asks for at once is for the developer to set, so there is no page size
            expect(pager.domNode.querySelector(".slick-pg-nav .slick-pg-size")).toBeNull();
            pager.destroy();

            // unless the grid asks for it
            const withSize = createPager(view, { showRowsPerPage: true });
            expect(withSize.domNode.querySelector(".slick-pg-nav .slick-pg-size")).not.toBeNull();
            withSize.destroy();
        });

        it("shows the reload button by default, also when the grid has a refresh button of its own", () => {
            const grid = document.createElement("div");
            grid.className = "s-DataGrid";
            const toolbar = document.createElement("div");
            toolbar.className = "grid-toolbar";
            const refresh = document.createElement("div");
            refresh.className = "tool-button refresh-button icon-tool-button no-text";
            refresh.setAttribute("data-action", "refresh");
            toolbar.appendChild(refresh);
            grid.appendChild(toolbar);
            document.body.appendChild(grid);

            const el = document.createElement("div");
            grid.appendChild(el);
            const pager = new SlickPager({ view: createMockView(), element: el } as any);
            expect(pager.domNode.querySelector(".slick-pg-reload")).not.toBeNull();
            pager.destroy();
        });

        it("leaves the reload button out when the option says so", () => {
            const pager = createPager(createMockView(), { showReload: false });
            expect(pager.domNode.querySelector(".slick-pg-reload")).toBeNull();
            expect(pager.domNode.querySelector(".slick-pg-stat")).not.toBeNull();
            pager.destroy();
        });
    });

    describe("a view that shows a range of the rows", () => {
        function createRangeView(info: any, bindToGrid?: any) {
            const view = createMockView();
            view.pagingInfo = { page: 1, rowsPerPage: 20, totalCount: 0, loading: false, error: null, pagingMode: "rows", ...info };
            view.bindToGrid = bindToGrid;
            return view;
        }

        it("gets no page navigation, and binds the view to the grid it is given", () => {
            const bindToGrid = vi.fn();
            const view = createRangeView({ rowRange: { from: 1, to: 20, total: 500 } }, bindToGrid);
            const grid = {};
            const pager = createPager(view, { grid });
            expect(bindToGrid).toHaveBeenCalledWith(grid);
            expect(document.body.querySelector(".slick-pg-first")).toBeNull();
            expect(document.body.querySelector(".slick-pg-prev")).toBeNull();
            expect(document.body.querySelector(".slick-pg-current")).toBeNull();
            expect(document.body.querySelector(".slick-pg-next")).toBeNull();
            // it still has the reload and status of a pager, but not the rows per page
            expect(document.body.querySelector(".slick-pg-size")).toBeNull();
            expect(document.body.querySelector(".slick-pg-reload")).not.toBeNull();
            expect(pager["stat"].textContent).toBe(PagerTexts.PageStatus
                .replace(/{from}/, "1").replace(/{to}/, "20").replace(/{total}/, "500"));
            pager.destroy();
        });

        it("tells only the range when the total of the rows is not known", () => {
            const view = createRangeView({ rowRange: { from: 1, to: 40, total: undefined } });
            const pager = createPager(view);
            expect(pager["stat"].textContent).toBe(PagerTexts.RangeStatus
                .replace(/{from}/, "1").replace(/{to}/, "40"));
            pager.destroy();
        });

        it("tells that there are no rows before there are any", () => {
            const pager = createPager(createRangeView({ rowRange: undefined }));
            expect(pager["stat"].textContent).toBe(PagerTexts.NoRowStatus);
            pager.destroy();
        });

        it("tells of the range the view gives when it changes", () => {
            const view = createRangeView({ rowRange: { from: 1, to: 20, total: 500 } });
            const pager = createPager(view);
            const update = view.onPagingInfoChanged.subscribe.mock.calls[0][0];
            view.pagingInfo = { ...view.pagingInfo, rowRange: { from: 501, to: 520, total: 500 } };
            update();
            expect(pager["stat"].textContent).toBe(PagerTexts.PageStatus
                .replace(/{from}/, "501").replace(/{to}/, "520").replace(/{total}/, "500"));
            pager.destroy();
        });
    });

    describe("the extra content before the controls", () => {
        const extra = (pager: SlickPager<any>) => pager.domNode.querySelector<HTMLElement>(".slick-pg-extra");

        it("shows what the option gives, and appends a string as text, not as markup", () => {
            const pager = createPager(createMockView(), { getInfoText: () => "This page <b>rounded</b>" });
            const el = extra(pager);
            expect(el.hidden).toBe(false);
            expect(el.textContent).toBe("This page <b>rounded</b>");
            expect(el.querySelector("b")).toBeNull();
            pager.destroy();
        });

        it("shows an element or a fragment the option gives as it is", () => {
            const strong = document.createElement("strong");
            strong.textContent = "Total of 84 orders";
            const pager = createPager(createMockView(), { getInfoText: () => strong });
            expect(extra(pager).querySelector("strong")?.textContent).toBe("Total of 84 orders");
            pager.destroy();

            const fragment = document.createDocumentFragment();
            fragment.append("This page ", document.createElement("b"));
            const fromFragment = createPager(createMockView(), { getInfoText: () => fragment });
            expect(extra(fromFragment).querySelector("b")).not.toBeNull();
            fromFragment.destroy();
        });

        it("uses what the view gives when the option doesn't", () => {
            const view = createMockView();
            view.getPagingInfoText = () => "1-25 of 84 records";
            const pager = createPager(view);
            expect(extra(pager).textContent).toBe("1-25 of 84 records");
            pager.destroy();
        });

        it("leaves it out when there is nothing to show, and updates it when it changes", () => {
            const pager = createPager(createMockView());
            expect(extra(pager).hidden).toBe(true);

            const update = (getInfoText: any) => {
                (pager as any).options.getInfoText = getInfoText;
                pager._updatePager();
            };

            update(() => "first");
            expect(extra(pager).hidden).toBe(false);
            expect(extra(pager).textContent).toBe("first");

            update(() => "second");
            expect(extra(pager).textContent).toBe("second");

            update(() => null);
            expect(extra(pager).hidden).toBe(true);
            expect(extra(pager).textContent).toBe("");
            pager.destroy();
        });
    });

    describe("accessibility", () => {
        it("tells what its icon buttons do to a tooltip and a screen reader", () => {
            const pager = createPager(createMockView(), { showFirstLast: true });
            const label = (sel: string) => {
                const el = pager.domNode.querySelector<HTMLElement>(sel);
                return { title: el.title, aria: el.getAttribute("aria-label"), role: el.getAttribute("role") };
            };

            expect(label(".slick-pg-first")).toEqual({ title: PagerTexts.FirstPage, aria: PagerTexts.FirstPage, role: "button" });
            expect(label(".slick-pg-prev")).toEqual({ title: PagerTexts.PreviousPage, aria: PagerTexts.PreviousPage, role: "button" });
            expect(label(".slick-pg-next")).toEqual({ title: PagerTexts.NextPage, aria: PagerTexts.NextPage, role: "button" });
            expect(label(".slick-pg-last")).toEqual({ title: PagerTexts.LastPage, aria: PagerTexts.LastPage, role: "button" });
            expect(label(".slick-pg-reload")).toEqual({ title: PagerTexts.Refresh, aria: PagerTexts.Refresh, role: "button" });
            pager.destroy();
        });

        it("can be used from the keyboard", () => {
            const view = createMockView();
            view.pagingInfo = { ...view.pagingInfo, page: 3 };
            const pager = createPager(view);
            const next = pager.domNode.querySelector<HTMLElement>(".slick-pg-next");
            next.dispatchEvent(new KeyboardEvent("keydown", { key: "Enter", bubbles: true }));
            expect(view.setPagingOptions).toHaveBeenCalledWith({ page: 4 });
            pager.destroy();
        });

        it("tells the disabled state of a button", () => {
            const view = createMockView();
            view.pagingInfo = { ...view.pagingInfo, page: 1 };
            const pager = createPager(view);
            expect(pager.domNode.querySelector(".slick-pg-prev").getAttribute("aria-disabled")).toBe("true");
            expect(pager.domNode.querySelector(".slick-pg-next").getAttribute("aria-disabled")).toBe("false");
            pager.destroy();
        });
    });

    describe("the page box", () => {
        it("has no visible label, but tells what it is to the tooltip and the screen reader", () => {
            const pager = createPager(createMockView());
            const input = pager.domNode.querySelector<HTMLInputElement>(".slick-pg-current");
            expect(pager.domNode.querySelector(".slick-pg-pagetext")).toBeNull();
            expect(input.title).toBe(PagerTexts.Page);
            expect(input.getAttribute("aria-label")).toBe(PagerTexts.Page);
            pager.destroy();
        });
    });

    describe("disabled navigation buttons", () => {
        const disabled = (cls: string) =>
            document.querySelector(cls)?.classList.contains("slick-pg-disabled");

        it("dims the first and previous buttons on the first page", () => {
            const view = createMockView();
            view.pagingInfo = { ...view.pagingInfo, page: 1, rowsPerPage: 20, totalCount: 100 };
            const pager = createPager(view, { showFirstLast: true });
            expect(disabled(".slick-pg-first")).toBe(true);
            expect(disabled(".slick-pg-prev")).toBe(true);
            expect(disabled(".slick-pg-next")).toBe(false);
            expect(disabled(".slick-pg-last")).toBe(false);
            expect(document.querySelector(".slick-pg-prev").getAttribute("aria-disabled")).toBe("true");
            pager.destroy();
        });

        it("dims the next and last buttons on the last page", () => {
            const view = createMockView();
            view.pagingInfo = { ...view.pagingInfo, page: 5, rowsPerPage: 20, totalCount: 100 };
            const pager = createPager(view, { showFirstLast: true });
            expect(disabled(".slick-pg-first")).toBe(false);
            expect(disabled(".slick-pg-prev")).toBe(false);
            expect(disabled(".slick-pg-next")).toBe(true);
            expect(disabled(".slick-pg-last")).toBe(true);
            pager.destroy();
        });

        it("dims every button when there is nothing to page, or only one page", () => {
            const view = createMockView();
            view.pagingInfo = { ...view.pagingInfo, page: 1, rowsPerPage: 20, totalCount: 0 };
            const pager = createPager(view, { showFirstLast: true });
            expect(disabled(".slick-pg-first")).toBe(true);
            expect(disabled(".slick-pg-prev")).toBe(true);
            expect(disabled(".slick-pg-next")).toBe(true);
            expect(disabled(".slick-pg-last")).toBe(true);
            pager.destroy();
        });

        it("dims the next button while an indeterminate view has no more rows", () => {
            const view = createMockView();
            view.pagingInfo = { ...view.pagingInfo, page: 3, rowsPerPage: 20, totalCount: 45, indeterminate: true, hasMore: false };
            const pager = createPager(view, { indeterminatePaging: true, showFirstLast: true });
            expect(disabled(".slick-pg-next")).toBe(true);
            expect(disabled(".slick-pg-first")).toBe(false);
            pager.destroy();
        });

        it("updates the states as the page changes", () => {
            const view = createMockView();
            view.pagingInfo = { ...view.pagingInfo, page: 1, rowsPerPage: 20, totalCount: 100 };
            const pager = createPager(view, { showFirstLast: true });
            expect(disabled(".slick-pg-prev")).toBe(true);
            expect(disabled(".slick-pg-first")).toBe(true);

            const update = view.onPagingInfoChanged.subscribe.mock.calls[0][0];
            view.pagingInfo = { ...view.pagingInfo, page: 2 };
            update();
            expect(disabled(".slick-pg-prev")).toBe(false);
            expect(disabled(".slick-pg-first")).toBe(false);
            pager.destroy();
        });
    });

    describe("_updatePager", () => {
        it("updates status with normal paging info", () => {
            const view = createMockView();
            const pager = createPager(view);
            pager._updatePager();
            expect(pager["currentPage"].value).toBe("1");
            expect(pager["totalPages"].textContent).toBe("5");
            expect(pager["stat"].textContent).toBe(PagerTexts.PageStatus
                .replace(/{from}/, "1")
                .replace(/{to}/, "20")
                .replace(/{total}/, "100"));
            pager.destroy();
        });

        it("shows loading status while loading", () => {
            const view = createMockView();
            view.pagingInfo.loading = true;
            const pager = createPager(view);
            pager._updatePager();
            expect(pager["stat"].textContent).toBe(PagerTexts.LoadingStatus);
            pager.destroy();
        });

        it("shows error status when error is set", () => {
            const view = createMockView();
            view.pagingInfo.error = "boom";
            const pager = createPager(view);
            pager._updatePager();
            expect(pager["stat"].textContent).toBe("boom");
            pager.destroy();
        });

        it("shows no-row status when totalCount is zero", () => {
            const view = createMockView();
            view.pagingInfo.totalCount = 0;
            const pager = createPager(view);
            pager._updatePager();
            expect(pager["stat"].textContent).toBe(PagerTexts.NoRowStatus);
            pager.destroy();
        });

        it("clamps range to totalCount on last partial page", () => {
            const view = createMockView();
            view.pagingInfo.totalCount = 30;
            view.pagingInfo.page = 2;
            const pager = createPager(view);
            pager._updatePager();
            expect(pager["stat"].textContent).toBe(PagerTexts.PageStatus
                .replace(/{from}/, "21")
                .replace(/{to}/, "30")
                .replace(/{total}/, "30"));
            pager.destroy();
        });
    });

    describe("button handlers", () => {
        it("reload button calls view.populate", () => {
            const view = createMockView();
            const pager = createPager(view);
            const btn = document.querySelector<HTMLElement>(".slick-pg-reload")!;
            btn.dispatchEvent(new Event("click"));
            expect(view.populate).toHaveBeenCalled();
            pager.destroy();
        });

        it("first nav button changes page", () => {
            const view = createMockView();
            view.pagingInfo.page = 3;
            const pager = createPager(view, { showFirstLast: true });
            const btn = document.querySelector<HTMLElement>(".slick-pg-first")!;
            btn.dispatchEvent(new Event("click"));
            expect(view.setPagingOptions).toHaveBeenCalledWith({ page: 1 });
            pager.destroy();
        });

        describe("shift+click", () => {
            const shiftClick = (selector: string) =>
                document.querySelector<HTMLElement>(selector)!.dispatchEvent(new MouseEvent("click", { shiftKey: true }));

            it("on prev goes to the first page, and on next to the last, without the first and last buttons", () => {
                const view = createMockView();
                view.pagingInfo.page = 3; // 100 rows of 20 are 5 pages
                const pager = createPager(view);
                expect(document.querySelector(".slick-pg-first")).toBeNull();

                shiftClick(".slick-pg-prev");
                expect(view.setPagingOptions).toHaveBeenLastCalledWith({ page: 1 });
                shiftClick(".slick-pg-next");
                expect(view.setPagingOptions).toHaveBeenLastCalledWith({ page: 5 });
                pager.destroy();
            });

            it("goes one page with a plain click", () => {
                const view = createMockView();
                view.pagingInfo.page = 3;
                const pager = createPager(view);
                document.querySelector<HTMLElement>(".slick-pg-prev")!.dispatchEvent(new MouseEvent("click"));
                expect(view.setPagingOptions).toHaveBeenLastCalledWith({ page: 2 });
                pager.destroy();
            });

            it("goes one page when the first and last buttons are shown, as they do the rest", () => {
                const view = createMockView();
                view.pagingInfo.page = 3;
                const pager = createPager(view, { showFirstLast: true });
                shiftClick(".slick-pg-prev");
                expect(view.setPagingOptions).toHaveBeenLastCalledWith({ page: 2 });
                pager.destroy();
            });

            it("on next goes one page when the total is not known, as there is no last page", () => {
                const view = createMockView();
                view.pagingInfo = { ...view.pagingInfo, page: 2, indeterminate: true, hasMore: true };
                const pager = createPager(view, { indeterminatePaging: true });
                shiftClick(".slick-pg-next");
                expect(view.setPagingOptions).toHaveBeenLastCalledWith({ page: 3 });
                pager.destroy();
            });
        });

        it("Enter on current page input changes page", () => {
            const view = createMockView();
            const pager = createPager(view);
            const input = document.querySelector<HTMLInputElement>(".slick-pg-current")!;
            input.value = "2";
            input.dispatchEvent(new KeyboardEvent("keydown", { key: "Enter" }));
            expect(view.setPagingOptions).toHaveBeenCalledWith({ page: 2 });
            pager.destroy();
        });
    });
});
