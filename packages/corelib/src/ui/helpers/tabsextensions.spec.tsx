import { TabsExtensions } from "./tabsextensions";

describe("TabsExtensions", () => {
    describe("indexByKey", () => {
        it("returns empty object for null/undefined tabs", () => {
            expect(TabsExtensions.indexByKey(null)).toEqual({});
            expect(TabsExtensions.indexByKey(undefined as any)).toEqual({});
        });

        it("returns index by key for basic tabs", () => {
            const tabs = document.createElement("div");
            tabs.innerHTML = `
                <ul>
                    <li><a class="nav-link" data-tabkey="tab1">Tab 1</a></li>
                    <li><a class="nav-link" data-tabkey="tab2">Tab 2</a></li>
                </ul>
            `;

            const result = TabsExtensions.indexByKey(tabs);
            expect(result).toEqual({ tab1: 0, tab2: 1 });
        });

        it("handles ArrayLike input", () => {
            const tabs = document.createElement("div");
            tabs.innerHTML = `
                <ul>
                    <li><a class="nav-link" data-tabkey="first">First</a></li>
                </ul>
            `;

            const result = TabsExtensions.indexByKey([tabs] as any);
            expect(result).toEqual({ first: 0 });
        });
    });

    describe("selectTab", () => {
        it("does nothing for null tabs", () => {
            expect(() => TabsExtensions.selectTab(null, "test")).not.toThrow();
        });

        it("selects tab by index", () => {
            const tabs = document.createElement("div");
            tabs.innerHTML = `
                <ul>
                    <li><a class="nav-link" data-tabkey="tab1">Tab 1</a></li>
                    <li><a class="nav-link" data-tabkey="tab2">Tab 2</a></li>
                </ul>
            `;

            // Should not throw
            expect(() => TabsExtensions.selectTab(tabs, 1)).not.toThrow();
        });

        it("selects tab by key", () => {
            const tabs = document.createElement("div");
            tabs.innerHTML = `
                <ul>
                    <li><a class="nav-link" data-tabkey="tab1">Tab 1</a></li>
                    <li><a class="nav-link" data-tabkey="tab2">Tab 2</a></li>
                </ul>
            `;

            const clickHandler = vi.fn();
            tabs.querySelector('[data-tabkey="tab2"]')?.addEventListener("click", clickHandler);

            TabsExtensions.selectTab(tabs, "tab2");
            expect(clickHandler).toHaveBeenCalled();
        });

        it("does nothing for non-existent tab key", () => {
            const tabs = document.createElement("div");
            tabs.innerHTML = `
                <ul>
                    <li><a class="nav-link" data-tabkey="tab1">Tab 1</a></li>
                </ul>
            `;

            expect(() => TabsExtensions.selectTab(tabs, "nonexistent")).not.toThrow();
        });
    });

    describe("setDisabled", () => {
        it("does nothing for null tabs", () => {
            expect(() => TabsExtensions.setDisabled(null, "test", true)).not.toThrow();
        });

        it("disables tab by key", () => {
            const tabs = document.createElement("div");
            tabs.innerHTML = `
                <ul>
                    <li><a class="nav-link" data-tabkey="tab1">Tab 1</a></li>
                </ul>
            `;

            TabsExtensions.setDisabled(tabs, "tab1", true);
            const link = tabs.querySelector('[data-tabkey="tab1"]');
            expect(link?.classList.contains("disabled")).toBe(true);
        });

        it("does nothing for non-existent tab key", () => {
            const tabs = document.createElement("div");
            expect(() => TabsExtensions.setDisabled(tabs, "nonexistent", true)).not.toThrow();
        });
    });

    describe("toggle", () => {
        it("does nothing for null tabs", () => {
            expect(() => TabsExtensions.toggle(null, "test", true)).not.toThrow();
        });

        it("toggles tab visibility", () => {
            const tabs = document.createElement("div");
            tabs.innerHTML = `
                <ul>
                    <li><a class="nav-link" data-tabkey="tab1">Tab 1</a></li>
                </ul>
            `;

            TabsExtensions.toggle(tabs, "tab1", false);
            const link = tabs.querySelector('[data-tabkey="tab1"]') as HTMLAnchorElement;
            expect(link.hidden).toBe(true);
        });

        it("does nothing for non-existent tab key", () => {
            const tabs = document.createElement("div");
            expect(() => TabsExtensions.toggle(tabs, "nonexistent", false)).not.toThrow();
        });
    });

    describe("activeTabKey", () => {
        it("returns empty string for null tabs", () => {
            expect(TabsExtensions.activeTabKey(null)).toBeUndefined();
        });

        it("returns tab key from active nav-link", () => {
            const tabs = document.createElement("div");
            tabs.innerHTML = `
                <ul>
                    <li><a class="nav-link" data-tabkey="tab1">Tab 1</a></li>
                    <li><a class="nav-link active" data-tabkey="tab2">Tab 2</a></li>
                </ul>
            `;

            const result = TabsExtensions.activeTabKey(tabs);
            expect(result).toBe("tab2");
        });
    });

    describe("initialize", () => {
        it("returns null for null tabs", () => {
            expect(TabsExtensions.initialize(null, vi.fn())).toBeNull();
        });

        it("initializes tabs with bootstrap classes", () => {
            const tabs = document.createElement("div");
            tabs.innerHTML = `
                <ul>
                    <li><a class="nav-link" data-tabkey="tab1">Tab 1</a></li>
                    <li><a class="nav-link" data-tabkey="tab2">Tab 2</a></li>
                </ul>
            `;

            const result = TabsExtensions.initialize(tabs, vi.fn());
            expect(result).toBeTruthy();
            const ul = tabs.querySelector("ul");
            expect(ul?.classList.contains("nav")).toBe(true);
            expect(ul?.classList.contains("nav-tabs")).toBe(true);
        });

        it("does not add nav-tabs if already has nav-underline", () => {
            const tabs = document.createElement("div");
            tabs.innerHTML = `
                <ul class="nav-underline">
                    <li><a class="nav-link" data-tabkey="tab1">Tab 1</a></li>
                </ul>
            `;

            TabsExtensions.initialize(tabs, vi.fn());
            const ul = tabs.querySelector("ul");
            expect(ul?.classList.contains("nav-tabs")).toBe(false);
        });
    });
});
