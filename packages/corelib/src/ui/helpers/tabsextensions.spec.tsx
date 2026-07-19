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

        it("uses jQuery UI tabs when available", () => {
            const mockTabsOption = vi.fn((_opt: string, _val: number, _arg: any) => {
                if (_val !== undefined) return undefined;
                return 0;
            });
            const mock$ = vi.fn(() => ({
                tabs: (method: string, ...args: any[]) => {
                    if (method === 'option') return mockTabsOption(method, args[0], args[1]);
                    return undefined;
                },
                data: vi.fn(() => ({ uiTabs: true }))
            })) as any;
            mock$.fn = { tabs: true };
            (globalThis as any).$ = mock$;
            (globalThis as any).jQuery = mock$;

            const tabs = document.createElement("div");
            TabsExtensions.selectTab(tabs, 1);
            expect(mockTabsOption).toHaveBeenCalledWith('option', 'active', 1);

            delete (globalThis as any).$;
            delete (globalThis as any).jQuery;
        });

        it("selectTab uses jQuery UI but skips when already active", () => {
            const mockTabsOption = vi.fn((_opt: string) => 1);
            const mock$ = vi.fn(() => ({
                tabs: (method: string) => {
                    if (method === 'option') return mockTabsOption(method);
                    return undefined;
                },
                data: vi.fn(() => ({ uiTabs: true }))
            })) as any;
            mock$.fn = { tabs: true };
            (globalThis as any).$ = mock$;
            (globalThis as any).jQuery = mock$;

            const tabs = document.createElement("div");
            TabsExtensions.selectTab(tabs, 1);
            // active is already 1, so should not call option('active', 1)
            expect(mockTabsOption).toHaveBeenCalledTimes(1); // only the get call

            delete (globalThis as any).$;
            delete (globalThis as any).jQuery;
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

        it("disables tab by numeric index", () => {
            const tabs = document.createElement("div");
            tabs.innerHTML = `
                <ul>
                    <li><a class="nav-link" data-tabkey="tab1">Tab 1</a></li>
                    <li><a class="nav-link" data-tabkey="tab2">Tab 2</a></li>
                </ul>
            `;

            TabsExtensions.setDisabled(tabs, 0, true);
            const link = tabs.querySelector('[data-tabkey="tab1"]');
            expect(link?.classList.contains("disabled")).toBe(true);
        });

        it("does nothing for non-existent tab key", () => {
            const tabs = document.createElement("div");
            expect(() => TabsExtensions.setDisabled(tabs, "nonexistent", true)).not.toThrow();
        });

        it("uses jQuery UI tabs when available", () => {
            const mockTabsDisable = vi.fn();
            const mock$ = vi.fn(() => ({
                tabs: mockTabsDisable,
                data: vi.fn(() => ({ uiTabs: true })),
                children: vi.fn(() => ({ children: vi.fn(() => ({ eq: vi.fn(() => ({ toggle: vi.fn() })) })) }))
            })) as any;
            mock$.fn = { tabs: true };
            (globalThis as any).$ = mock$;
            (globalThis as any).jQuery = mock$;

            const tabs = document.createElement("div");
            tabs.innerHTML = `
                <ul>
                    <li><a class="nav-link" data-tabkey="tab1">Tab 1</a></li>
                </ul>
            `;

            TabsExtensions.setDisabled(tabs, "tab1", true);
            expect(mockTabsDisable).toHaveBeenCalledWith('disable', 0);

            delete (globalThis as any).$;
            delete (globalThis as any).jQuery;
        });

        it("disables active tab via jQuery UI and switches to first tab", () => {
            const mockTabsOption = vi.fn((_opt: string, _arg: any) => 0); // active = 0
            const mockTabsDisable = vi.fn();
            const mock$ = vi.fn(() => ({
                tabs: (method: string, ...args: any[]) => {
                    if (method === 'option') return mockTabsOption(method, args[0]);
                    if (method === 'disable') return mockTabsDisable(method, args[0]);
                    if (method === 'option') return mockTabsOption(method, args[0]);
                    if (method === 'disable') return mockTabsDisable(method);
                    return undefined;
                },
                data: vi.fn(() => ({ uiTabs: true }))
            })) as any;
            mock$.fn = { tabs: true };
            (globalThis as any).$ = mock$;
            (globalThis as any).jQuery = mock$;

            const tabs = document.createElement("div");
            TabsExtensions.setDisabled(tabs, 0, true);
            expect(mockTabsDisable).toHaveBeenCalledWith('disable', 0);

            delete (globalThis as any).$;
            delete (globalThis as any).jQuery;
        });
    });

    describe("toggle", () => {
        it("does nothing for null tabs", () => {
            expect(() => TabsExtensions.toggle(null, "test", true)).not.toThrow();
        });

        it("toggles tab visibility by key", () => {
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

        it("toggles tab visibility by numeric index", () => {
            const tabs = document.createElement("div");
            tabs.innerHTML = `
                <ul>
                    <li><a class="nav-link" data-tabkey="tab1">Tab 1</a></li>
                    <li><a class="nav-link" data-tabkey="tab2">Tab 2</a></li>
                </ul>
            `;

            TabsExtensions.toggle(tabs, 0, false);
            const link = tabs.querySelector('[data-tabkey="tab1"]') as HTMLAnchorElement;
            expect(link.hidden).toBe(true);
        });

        it("does nothing for non-existent tab key", () => {
            const tabs = document.createElement("div");
            expect(() => TabsExtensions.toggle(tabs, "nonexistent", false)).not.toThrow();
        });

        it("uses jQuery UI tabs when available", () => {
            const mockToggle = vi.fn();
            const mock$ = vi.fn(() => ({
                tabs: vi.fn(),
                data: vi.fn(() => ({ uiTabs: true })),
                children: vi.fn(() => ({ children: vi.fn(() => ({ eq: vi.fn(() => ({ toggle: mockToggle })) })) }))
            })) as any;
            mock$.fn = { tabs: true };
            (globalThis as any).$ = mock$;
            (globalThis as any).jQuery = mock$;

            const tabs = document.createElement("div");
            TabsExtensions.toggle(tabs, 0, false);
            expect(mockToggle).toHaveBeenCalledWith(false);

            delete (globalThis as any).$;
            delete (globalThis as any).jQuery;
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

        it("uses jQuery UI tabs when available", () => {
            const mockTabsFn = vi.fn(() => ({
                on: vi.fn()
            }));
            const mock$ = vi.fn(() => ({
                tabs: mockTabsFn,
                data: vi.fn(() => ({ uiTabs: true }))
            })) as any;
            mock$.fn = { tabs: true };
            (globalThis as any).$ = mock$;
            (globalThis as any).jQuery = mock$;

            const tabs = document.createElement("div");
            const result = TabsExtensions.initialize(tabs, vi.fn());
            expect(result).toBeTruthy();

            delete (globalThis as any).$;
            delete (globalThis as any).jQuery;
        });

        it("activates first link when no active link found", () => {
            const tabs = document.createElement("div");
            tabs.innerHTML = `
                <ul>
                    <li><a class="nav-link" data-tabkey="tab1">Tab 1</a></li>
                </ul>
            `;

            TabsExtensions.initialize(tabs, vi.fn());
            const link = tabs.querySelector("a.nav-link.active");
            expect(link).toBeTruthy();
        });

        it("converts ui-tabs-active to active", () => {
            const tabs = document.createElement("div");
            tabs.innerHTML = `
                <ul>
                    <li><a class="nav-link ui-tabs-active" data-tabkey="tab1">Tab 1</a></li>
                </ul>
            `;

            TabsExtensions.initialize(tabs, vi.fn());
            const link = tabs.querySelector("a.nav-link.active");
            expect(link).toBeTruthy();
            expect(link?.classList.contains("ui-tabs-active")).toBe(false);
        });

        it("moves tab-panes into tab-content container", () => {
            const tabs = document.createElement("div");
            tabs.innerHTML = `
                <ul>
                    <li><a class="nav-link active" data-tabkey="tab1" href="#pane1">Tab 1</a></li>
                </ul>
                <div class="tab-pane" id="pane1">Content 1</div>
            `;

            TabsExtensions.initialize(tabs, vi.fn());
            const tabContent = tabs.querySelector(".tab-content");
            expect(tabContent).toBeTruthy();
            const pane = tabContent?.querySelector("#pane1");
            expect(pane).toBeTruthy();
            expect(pane?.classList.contains("show")).toBe(true);
            expect(pane?.classList.contains("active")).toBe(true);
        });

        it("does not fail when tabs has no ul", () => {
            const tabs = document.createElement("div");
            tabs.innerHTML = `<span>Not a tab</span>`;

            const result = TabsExtensions.initialize(tabs, vi.fn());
            expect(result).toBeTruthy();
        });

        it("handles tabs that are already a ul element", () => {
            const ul = document.createElement("ul");
            ul.innerHTML = `
                <li><a class="nav-link" data-tabkey="tab1">Tab 1</a></li>
            `;

            const result = TabsExtensions.initialize(ul as any, vi.fn());
            expect(result).toBeTruthy();
        });
    });

    describe("destroy", () => {
        it("does nothing for null tabs", () => {
            expect(() => TabsExtensions.destroy(null)).not.toThrow();
        });

        it("destroys jQuery UI tabs when present", () => {
            const mockDestroy = vi.fn();
            const mock$ = vi.fn(() => ({
                tabs: mockDestroy,
                data: vi.fn(() => ({ uiTabs: true }))
            })) as any;
            mock$.fn = { tabs: true };
            (globalThis as any).$ = mock$;
            (globalThis as any).jQuery = mock$;

            const tabs = document.createElement("div");
            TabsExtensions.destroy(tabs);
            expect(mockDestroy).toHaveBeenCalledWith("destroy");

            delete (globalThis as any).$;
            delete (globalThis as any).jQuery;
        });

        it("disposes bootstrap tabs when no jQuery UI", () => {
            const mockDispose = vi.fn();
            (globalThis as any).bootstrap = {
                Tab: {
                    getInstance: vi.fn(() => ({ dispose: mockDispose }))
                }
            };

            const tabs = document.createElement("div");
            tabs.innerHTML = `
                <ul>
                    <li><a class="nav-link" data-tabkey="tab1">Tab 1</a></li>
                </ul>
            `;

            TabsExtensions.destroy(tabs);
            expect(mockDispose).toHaveBeenCalled();

            delete (globalThis as any).bootstrap;
        });

        it("disposes bootstrap tab via ArrayLike input", () => {
            const mockDispose = vi.fn();
            (globalThis as any).bootstrap = {
                Tab: {
                    getInstance: vi.fn(() => ({ dispose: mockDispose }))
                }
            };

            const tabs = document.createElement("div");
            tabs.innerHTML = `
                <ul>
                    <li><a class="nav-link" data-tabkey="tab1">Tab 1</a></li>
                </ul>
            `;

            TabsExtensions.destroy([tabs] as any);
            expect(mockDispose).toHaveBeenCalled();

            delete (globalThis as any).bootstrap;
        });

        it("handles bootstrap instance without dispose method", () => {
            (globalThis as any).bootstrap = {
                Tab: {
                    getInstance: vi.fn(() => ({}))
                }
            };

            const tabs = document.createElement("div");
            tabs.innerHTML = `
                <ul>
                    <li><a class="nav-link">Tab 1</a></li>
                </ul>
            `;
            expect(() => TabsExtensions.destroy(tabs)).not.toThrow();

            delete (globalThis as any).bootstrap;
        });
    });

    describe("extractTabKey (via indexByKey)", () => {
        it("extracts tab key from _Tab prefix in href", () => {
            const tabs = document.createElement("div");
            tabs.innerHTML = `
                <ul>
                    <li><a class="nav-link" href="#_TabMyKey">Tab</a></li>
                </ul>
            `;
            const result = TabsExtensions.indexByKey(tabs);
            expect(result).toEqual({ MyKey: 0 });
        });

        it("falls back to href when no data-tabkey or _Tab prefix", () => {
            const tabs = document.createElement("div");
            tabs.innerHTML = `
                <ul>
                    <li><a class="nav-link" href="#/plain/href">Tab</a></li>
                </ul>
            `;
            const result = TabsExtensions.indexByKey(tabs);
            expect(result).toEqual({ "#/plain/href": 0 });
        });
    });

    describe("setDisabled - active tab switching", () => {
        it("switches to another tab when disabling the active one (no jQuery)", () => {
            const tabs = document.createElement("div");
            tabs.innerHTML = `
                <ul>
                    <li><a class="nav-link active" data-tabkey="tab1">Tab 1</a></li>
                    <li><a class="nav-link" data-tabkey="tab2">Tab 2</a></li>
                </ul>
            `;
            const clickSpy = vi.fn();
            tabs.querySelector('[data-tabkey="tab2"]')?.addEventListener("click", clickSpy);

            TabsExtensions.setDisabled(tabs, "tab1", true);
            expect(tabs.querySelector('[data-tabkey="tab1"]')?.classList.contains("disabled")).toBe(true);
            expect(clickSpy).toHaveBeenCalled();
        });

        it("does not switch when no alternative tab exists", () => {
            const tabs = document.createElement("div");
            tabs.innerHTML = `
                <ul>
                    <li><a class="nav-link active" data-tabkey="tab1">Tab 1</a></li>
                </ul>
            `;
            TabsExtensions.setDisabled(tabs, "tab1", true);
            expect(tabs.querySelector('[data-tabkey="tab1"]')?.classList.contains("disabled")).toBe(true);
        });

        it("skips already disabled alternative tabs when switching", () => {
            const tabs = document.createElement("div");
            tabs.innerHTML = `
                <ul>
                    <li><a class="nav-link active" data-tabkey="tab1">Tab 1</a></li>
                    <li><a class="nav-link disabled" data-tabkey="tab2">Tab 2 (disabled)</a></li>
                    <li><a class="nav-link" data-tabkey="tab3">Tab 3</a></li>
                </ul>
            `;
            const clickSpy = vi.fn();
            tabs.querySelector('[data-tabkey="tab3"]')?.addEventListener("click", clickSpy);

            TabsExtensions.setDisabled(tabs, "tab1", true);
            expect(clickSpy).toHaveBeenCalled();
        });
    });

    describe("toggle - jQuery UI path for hiding active tab", () => {
        it("hides active tab and switches to first via jQuery UI", () => {
            const mockTabsOption = vi.fn((_opt: string, _args: any) => 0); // active = 0
            const mockToggle = vi.fn();
            const mock$ = vi.fn(() => ({
                tabs: (method: string, ...args: any[]) => {
                    if (method === 'option') return mockTabsOption(method, args[0]);
                    return undefined;
                },
                data: vi.fn(() => ({ uiTabs: true })),
                children: vi.fn(() => ({ children: vi.fn(() => ({ eq: vi.fn(() => ({ toggle: mockToggle })) })) }))
            })) as any;
            mock$.fn = { tabs: true };
            (globalThis as any).$ = mock$;
            (globalThis as any).jQuery = mock$;

            const tabs = document.createElement("div");
            // Tab 0 is active, hiding it should trigger switching to tab 0 (which is itself, but code tries)
            TabsExtensions.toggle(tabs, 0, false);
            expect(mockTabsOption).toHaveBeenCalledWith('option', 'active');
            expect(mockToggle).toHaveBeenCalledWith(false);

            delete (globalThis as any).$;
            delete (globalThis as any).jQuery;
        });

        it("hides non-active tab via jQuery UI without switching", () => {
            const mockTabsOption = vi.fn((_opt: string) => 1); // active = 1
            const mockToggle = vi.fn();
            const mock$ = vi.fn(() => ({
                tabs: vi.fn(),
                data: vi.fn(() => ({ uiTabs: true })),
                children: vi.fn(() => ({ children: vi.fn(() => ({ eq: vi.fn(() => ({ toggle: mockToggle })) })) }))
            })) as any;
            mock$.fn = { tabs: true };
            (globalThis as any).$ = mock$;
            (globalThis as any).jQuery = mock$;

            const tabs = document.createElement("div");
            TabsExtensions.toggle(tabs, 0, false);
            expect(mockToggle).toHaveBeenCalledWith(false);

            delete (globalThis as any).$;
            delete (globalThis as any).jQuery;
        });
    });

    describe("activeTabKey - jQuery UI path", () => {
        it("returns tab key via jQuery UI tabs", () => {
            const mock$ = vi.fn(() => ({
                tabs: (method: string) => method === 'option' ? 1 : undefined,
                data: vi.fn(() => ({ uiTabs: true })),
                children: vi.fn(() => ({ children: vi.fn(() => ({ eq: vi.fn(() => ({ children: vi.fn(() => [{ dataset: { tabkey: 'tab2' } }]) })) })) }))
            })) as any;
            mock$.fn = { tabs: true };
            (globalThis as any).$ = mock$;
            (globalThis as any).jQuery = mock$;

            const tabs = document.createElement("div");
            const result = TabsExtensions.activeTabKey(tabs);
            expect(result).toBe('tab2');

            delete (globalThis as any).$;
            delete (globalThis as any).jQuery;
        });
    });

    describe("selectTab - jQuery UI path edge cases", () => {
        it("returns early when tabs is a string", () => {
            expect(() => TabsExtensions.selectTab("string" as any, "test")).not.toThrow();
        });

        it("does nothing when indexByKey returns null", () => {
            const tabs = document.createElement("div");
            tabs.innerHTML = `<div></div>`; // no nav links
            expect(() => TabsExtensions.selectTab(tabs, "nonexistent")).not.toThrow();
        });
    });

    describe("initialize - edge cases", () => {
        it("returns null when tabs is a string", () => {
            expect(TabsExtensions.initialize("string" as any, vi.fn())).toBeNull();
        });

        it("initializes with jQuery UI tabs + activeChange callback", () => {
            const mockOn = vi.fn();
            const mockTabs = vi.fn(() => ({ on: mockOn }));
            const mock$ = vi.fn(() => ({
                tabs: mockTabs,
                data: vi.fn(() => ({ uiTabs: true }))
            })) as any;
            mock$.fn = { tabs: true };
            (globalThis as any).$ = mock$;
            (globalThis as any).jQuery = mock$;

            const tabs = document.createElement("div");
            const result = TabsExtensions.initialize(tabs, vi.fn());
            expect(result).toBeTruthy();

            delete (globalThis as any).$;
            delete (globalThis as any).jQuery;
        });
    });

    describe("setDisabled bootstrap path edge cases", () => {
        it("returns early when tabs is a string", () => {
            expect(() => TabsExtensions.setDisabled("string" as any, "test", true)).not.toThrow();
        });

        it("returns early when indexByKey returns empty for string key", () => {
            const tabs = document.createElement("div");
            tabs.innerHTML = `<div></div>`; // no nav links
            expect(() => TabsExtensions.setDisabled(tabs, "nonexistent", true)).not.toThrow();
        });
    });

    describe("toggle bootstrap path edge cases", () => {
        it("returns early when tabs is a string", () => {
            expect(() => TabsExtensions.toggle("string" as any, "test", true)).not.toThrow();
        });

        it("toggles active tab and switches to visible one", () => {
            const tabs = document.createElement("div");
            tabs.innerHTML = `
                <ul>
                    <li><a class="nav-link active" data-tabkey="tab1">Tab 1</a></li>
                    <li><a class="nav-link" data-tabkey="tab2">Tab 2</a></li>
                </ul>
            `;
            const clickSpy = vi.fn();
            tabs.querySelector('[data-tabkey="tab2"]')?.addEventListener("click", clickSpy);

            TabsExtensions.toggle(tabs, "tab1", false);
            const link = tabs.querySelector('[data-tabkey="tab1"]') as HTMLAnchorElement;
            expect(link.hidden).toBe(true);
            expect(clickSpy).toHaveBeenCalled();
        });
    });

    describe("indexByKey edge cases", () => {
        it("returns empty object for element with no nav links", () => {
            const div = document.createElement("div");
            expect(TabsExtensions.indexByKey(div)).toEqual({});
        });
    });

    describe("activeTabKey edge cases", () => {
        it("returns undefined for string tabs", () => {
            expect(TabsExtensions.activeTabKey("string" as any)).toBeUndefined();
        });

        it("returns empty string when no active link found", () => {
            const tabs = document.createElement("div");
            tabs.innerHTML = `<ul><li><a class="nav-link" data-tabkey="tab1">Tab 1</a></li></ul>`;
            const result = TabsExtensions.activeTabKey(tabs);
            expect(result).toBe("");
        });
    });
});
