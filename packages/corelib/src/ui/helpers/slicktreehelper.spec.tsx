import { SlickTreeHelper } from "./slicktreehelper";

describe("SlickTreeHelper", () => {
    describe("filterCustom", () => {
        it("returns true for item with no parent", () => {
            const item = { name: "root" };
            const result = SlickTreeHelper.filterCustom(item, () => null);
            expect(result).toBe(true);
        });

        it("returns false if any parent is collapsed", () => {
            const parent = { name: "parent", _collapsed: true };
            const item = { name: "child" };
            const result = SlickTreeHelper.filterCustom(item, (x) =>
                x === item ? parent : null
            );
            expect(result).toBe(false);
        });

        it("returns true if no parent is collapsed", () => {
            const grandparent = { name: "gp", _collapsed: false };
            const parent = { name: "parent", _collapsed: false };
            const item = { name: "child" };
            let callCount = 0;
            const result = SlickTreeHelper.filterCustom(item, (x) => {
                callCount++;
                if (x === item) return parent;
                if (x === parent) return grandparent;
                return null;
            });
            expect(result).toBe(true);
            expect(callCount).toBe(3); // item, parent, grandparent
        });

        it("throws on circular reference", () => {
            const item: any = { name: "loop" };
            item.self = item;
            expect(() => {
                SlickTreeHelper.filterCustom(item, (x) => x.self);
            }).toThrow("Possible infinite loop");
        });
    });

    describe("filterById", () => {
        it("returns true when parent is not collapsed", () => {
            const items = [
                { id: 1, name: "parent" },
                { id: 2, name: "child", parentId: 1 }
            ];
            const view = {
                getItemById: (id: number) => items.find(x => x.id === id)
            };

            const result = SlickTreeHelper.filterById(items[1], view as any, (x: any) => x.parentId);
            expect(result).toBe(true);
        });

        it("returns false when parent is collapsed", () => {
            const items = [
                { id: 1, name: "parent", _collapsed: true },
                { id: 2, name: "child", parentId: 1 }
            ];
            const view = {
                getItemById: (id: number) => items.find(x => x.id === id)
            };

            const result = SlickTreeHelper.filterById(items[1], view as any, (x: any) => x.parentId);
            expect(result).toBe(false);
        });

        it("returns true when parentId is null", () => {
            const item = { id: 1, name: "root", parentId: null as any };
            const view = { getItemById: () => null };
            const result = SlickTreeHelper.filterById(item, view as any, (x: any) => x.parentId);
            expect(result).toBe(true);
        });
    });

    describe("setCollapsed", () => {
        it("sets _collapsed on all items", () => {
            const items = [
                { id: 1 },
                { id: 2 }
            ];
            SlickTreeHelper.setCollapsed(items, true);
            expect(items[0]).toHaveProperty("_collapsed", true);
            expect(items[1]).toHaveProperty("_collapsed", true);
        });

        it("handles null items", () => {
            expect(() => SlickTreeHelper.setCollapsed(null, true)).not.toThrow();
        });

        it("sets collapsed to false", () => {
            const items = [{ id: 1, _collapsed: true }];
            SlickTreeHelper.setCollapsed(items, false);
            expect(items[0]._collapsed).toBe(false);
        });
    });

    describe("setCollapsedFlag", () => {
        it("sets _collapsed on single item", () => {
            const item = { id: 1 };
            SlickTreeHelper.setCollapsedFlag(item, true);
            expect((item as any)._collapsed).toBe(true);
        });
    });

    describe("setIndents", () => {
        it("sets indent 0 for first item", () => {
            const items = [{ id: 1 }];
            SlickTreeHelper.setIndents(items, (x: any) => x.id, () => null);
            expect((items[0] as any)._indent).toBe(0);
        });

        it("increments indent when parentId matches previous item's id", () => {
            const items = [
                { id: 1 },
                { id: 2, parentId: 1 }
            ];
            SlickTreeHelper.setIndents(items,
                (x: any) => x.id,
                (x: any) => x.parentId
            );
            expect((items[0] as any)._indent).toBe(0);
            expect((items[1] as any)._indent).toBe(1);
        });

        it("resets indent when parentId is null", () => {
            const items = [
                { id: 1 },
                { id: 2, parentId: null }
            ];
            SlickTreeHelper.setIndents(items,
                (x: any) => x.id,
                (x: any) => x.parentId
            );
            expect((items[1] as any)._indent).toBe(0);
        });

        it("uses depth from depths map when parentId differs", () => {
            const items = [
                { id: 1 },
                { id: 2, parentId: 99 }
            ];
            SlickTreeHelper.setIndents(items,
                (x: any) => x.id,
                (x: any) => x.parentId
            );
            expect((items[1] as any)._indent).toBe(0);
        });

        it("sets collapsed flag when setCollapsed is provided", () => {
            const items = [{ id: 1 }];
            SlickTreeHelper.setIndents(items,
                (x: any) => x.id,
                () => null,
                true
            );
            expect((items[0] as any)._collapsed).toBe(true);
        });
    });

    describe("toggleClick", () => {
        it("does nothing if event target is not s-TreeToggle", () => {
            const e = { target: document.createElement("div"), preventDefault: vi.fn() };
            const view = { getItem: vi.fn(), updateItem: vi.fn() };
            SlickTreeHelper.toggleClick(e as any, 0, 0, view as any, vi.fn());
            expect(view.getItem).not.toHaveBeenCalled();
        });

        it("toggles _collapsed on s-TreeToggle click", () => {
            const target = document.createElement("span");
            target.classList.add("s-TreeToggle", "s-TreeCollapse");
            const e = { target, preventDefault: vi.fn() };
            const item = { id: 1, _collapsed: false };
            const view = {
                getItem: vi.fn(() => item),
                updateItem: vi.fn()
            };

            SlickTreeHelper.toggleClick(e as any, 0, 0, view as any, (x: any) => x.id);
            expect(item._collapsed).toBe(true);
            expect(view.updateItem).toHaveBeenCalledWith(1, item);
        });

        it("expands collapsed item on s-TreeExpand click", () => {
            const target = document.createElement("span");
            target.classList.add("s-TreeToggle", "s-TreeExpand");
            const e = { target, preventDefault: vi.fn() };
            const item = { id: 2, _collapsed: true };
            const view = {
                getItem: vi.fn(() => item),
                updateItem: vi.fn()
            };

            SlickTreeHelper.toggleClick(e as any, 0, 0, view as any, (x: any) => x.id);
            expect(item._collapsed).toBe(false);
        });

        it("does nothing if target is not s-TreeCollapse or s-TreeExpand", () => {
            const target = document.createElement("span");
            target.classList.add("s-TreeToggle");
            const e = { target, preventDefault: vi.fn() };
            const view = { getItem: vi.fn(), updateItem: vi.fn() };

            SlickTreeHelper.toggleClick(e as any, 0, 0, view as any, vi.fn());
            expect(view.getItem).not.toHaveBeenCalled();
        });

        it("collapses all siblings when shift is held", () => {
            const target = document.createElement("span");
            target.classList.add("s-TreeToggle", "s-TreeCollapse");
            const items = [
                { id: 1, _indent: 0, _collapsed: false },
                { id: 2, _indent: 1, _collapsed: false }
            ];
            const e = { target, preventDefault: vi.fn(), shiftKey: true };
            const view = {
                getItem: vi.fn(() => items[0]),
                updateItem: vi.fn(),
                getItems: vi.fn(() => items),
                setItems: vi.fn(),
                beginUpdate: vi.fn(),
                endUpdate: vi.fn()
            };

            SlickTreeHelper.toggleClick(e as any, 0, 0, view as any, (x: any) => x.id);
            expect(items[0]._collapsed).toBe(true);
            expect(view.beginUpdate).toHaveBeenCalled();
            expect(view.endUpdate).toHaveBeenCalled();
        });

        it("handles null item in toggleClick", () => {
            const target = document.createElement("span");
            target.classList.add("s-TreeToggle", "s-TreeCollapse");
            const e = { target, preventDefault: vi.fn() };
            const view = {
                getItem: vi.fn(() => null),
                updateItem: vi.fn()
            };

            SlickTreeHelper.toggleClick(e as any, 0, 0, view as any, (x: any) => x.id);
            expect(view.updateItem).not.toHaveBeenCalled();
        });

        it("handles default prevented event", () => {
            const e = { target: null, preventDefault: vi.fn(), defaultPrevented: true };
            const view = { getItem: vi.fn() };

            SlickTreeHelper.toggleClick(e as any, 0, 0, view as any, vi.fn());
            expect(view.getItem).not.toHaveBeenCalled();
        });
    });
});
