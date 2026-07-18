import { GridUtils } from "./gridutils";

describe("GridUtils", () => {
    describe("addToggleButton", () => {
        it("adds toggle button to toolDiv", () => {
            const toolDiv = document.createElement("div");
            const callback = vi.fn();

            GridUtils.addToggleButton(toolDiv, "my-toggle", callback, "Toggle me");

            const btn = toolDiv.querySelector(".s-ToggleButton");
            expect(btn).toBeTruthy();
            expect(btn?.classList.contains("my-toggle")).toBe(true);

            // Click the link inside
            const link = btn?.querySelector("a");
            link?.click();
            expect(callback).toHaveBeenCalledWith(true);
        });

        it("adds pressed class when initial is true", () => {
            const toolDiv = document.createElement("div");
            GridUtils.addToggleButton(toolDiv, "test", vi.fn(), "", true);

            const btn = toolDiv.querySelector(".s-ToggleButton");
            expect(btn?.classList.contains("pressed")).toBe(true);
        });

        it("handles ArrayLike toolDiv", () => {
            const toolDiv = document.createElement("div");
            GridUtils.addToggleButton([toolDiv] as any, "test", vi.fn(), "");
            expect(toolDiv.querySelector(".s-ToggleButton")).toBeTruthy();
        });
    });

    describe("addIncludeDeletedToggle", () => {
        it("adds toggle button and modifies view onSubmit", () => {
            const toolDiv = document.createElement("div");
            const view = {
                onSubmit: vi.fn((p) => true),
                seekToPage: 0,
                populate: vi.fn()
            };

            GridUtils.addIncludeDeletedToggle(toolDiv, view as any);
            const btn = toolDiv.querySelector(".s-IncludeDeletedToggle");
            expect(btn).toBeTruthy();

            // The view's onSubmit should now set IncludeDeleted
            const params = {};
            const result = view.onSubmit({ params });
            expect((params as any).IncludeDeleted).toBe(false);
            expect(result).toBe(true);
        });

        it("uses initial value for IncludeDeleted", () => {
            const toolDiv = document.createElement("div");
            const view = {
                onSubmit: vi.fn((p) => true),
                seekToPage: 0,
                populate: vi.fn()
            };

            GridUtils.addIncludeDeletedToggle(toolDiv, view as any, "Show Deleted", true);
            const params = {};
            view.onSubmit({ params });
            expect((params as any).IncludeDeleted).toBe(true);
        });
    });

    describe("addQuickSearch", () => {
        it("adds quick search input to container", () => {
            const container = document.createElement("div");
            const view = {
                onSubmit: vi.fn(),
                seekToPage: 0,
                populate: vi.fn(),
                onDataLoaded: { subscribe: vi.fn(), unsubscribe: vi.fn() }
            };

            const input = GridUtils.addQuickSearch({
                container,
                view: view as any
            });

            expect(container.querySelector(".s-QuickSearchBar")).toBeTruthy();
            expect(input).toBeTruthy();
        });

        it("adds has-quick-search-fields class when fields provided", () => {
            const container = document.createElement("div");
            GridUtils.addQuickSearch({
                container,
                fields: [{ name: "field1", title: "Field 1" }]
            });

            const bar = container.querySelector(".s-QuickSearchBar");
            expect(bar?.classList.contains("has-quick-search-fields")).toBe(true);
        });

        it("handles ArrayLike container", () => {
            const container = document.createElement("div");
            GridUtils.addQuickSearch({
                container: [container] as any
            });

            expect(container.querySelector(".s-QuickSearchBar")).toBeTruthy();
        });
    });

    describe("addQuickSearchInput", () => {
        it("creates quick search with toolbar and view", () => {
            const toolDiv = document.createElement("div");
            const view = {
                onSubmit: vi.fn(),
                seekToPage: 0,
                populate: vi.fn(),
                onDataLoaded: { subscribe: vi.fn(), unsubscribe: vi.fn() }
            };

            const input = GridUtils.addQuickSearchInput(toolDiv, view as any);
            expect(input).toBeTruthy();
        });
    });

    describe("addQuickSearchInputCustom", () => {
        it("creates quick search with custom search function", () => {
            const container = document.createElement("div");
            const searchFn = vi.fn();

            const input = GridUtils.addQuickSearchInputCustom(container, searchFn);
            expect(input).toBeTruthy();
        });
    });
});
