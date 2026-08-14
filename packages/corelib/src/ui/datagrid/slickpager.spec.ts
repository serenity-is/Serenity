import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
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
        onPagingInfoChanged: { subscribe: vi.fn() },
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
            const pager = createPager(view);
            const btn = document.querySelector<HTMLElement>(".slick-pg-first")!;
            btn.dispatchEvent(new Event("click"));
            expect(view.setPagingOptions).toHaveBeenCalledWith({ page: 1 });
            pager.destroy();
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
