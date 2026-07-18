import { CheckTreeEditorTexts } from "../../base";
import { Toolbar } from "../widgets/toolbar";
import { getWidgetFrom } from "../widgets/widgetutils";
import { GridSelectAllButtonHelper } from "./gridselectallbuttonhelper";

vi.mock("../widgets/widgetutils", () => ({
    getWidgetFrom: vi.fn()
}));

describe("GridSelectAllButtonHelper", () => {
    let mockToolbar: any;
    let mockFindButton: any;

    beforeEach(() => {
        mockFindButton = {
            toggleClass: vi.fn()
        };
        mockToolbar = {
            findButton: vi.fn(() => mockFindButton)
        };
        (getWidgetFrom as any).mockReturnValue(mockToolbar);
    });

    describe("define", () => {
        it("returns a ToolButton with select-all action", () => {
            const getGrid = vi.fn();
            const getId = vi.fn();
            const getSelected = vi.fn();
            const setSelected = vi.fn();

            const button = GridSelectAllButtonHelper.define(getGrid, getId, getSelected, setSelected);
            expect(button.action).toBe("select-all");
            expect(button.cssClass).toBe("select-all-button");
        });

        it("uses custom text when provided", () => {
            const button = GridSelectAllButtonHelper.define(
                vi.fn(), vi.fn(), vi.fn(), vi.fn(),
                "Check All"
            );
            expect(button.title).toBe("Check All");
        });

        it("uses CheckTreeEditorTexts.SelectAll when no custom text", () => {
            const button = GridSelectAllButtonHelper.define(
                vi.fn(), vi.fn(), vi.fn(), vi.fn()
            );
            // Default comes from CheckTreeEditorTexts.SelectAll which has a value
            expect(button.title).toBeTruthy();
        });

        describe("onClick", () => {
            it("selects all items when button is not checked", () => {
                const items = [
                    { id: 1, name: "Item 1" },
                    { id: 2, name: "Item 2" }
                ];
                const mockView = {
                    getItems: vi.fn(() => items),
                    beginUpdate: vi.fn(),
                    endUpdate: vi.fn(),
                    updateItem: vi.fn()
                };
                const getGrid = vi.fn(() => ({
                    getView: vi.fn(() => mockView)
                }));
                const getId = vi.fn((x: any) => x.id);
                const setSelected = vi.fn();
                const onClick = vi.fn();

                const button = GridSelectAllButtonHelper.define(getGrid as any, getId, vi.fn(() => false), setSelected, "Select All", onClick);
                const mockBtnEl = document.createElement("div");
                mockBtnEl.classList.add("select-all-button");
                const event = { target: mockBtnEl } as any;

                button.onClick(event);

                expect(mockView.beginUpdate).toHaveBeenCalled();
                expect(setSelected).toHaveBeenCalledTimes(2);
                expect(setSelected).toHaveBeenCalledWith(items[0], true);
                expect(setSelected).toHaveBeenCalledWith(items[1], true);
                expect(mockView.updateItem).toHaveBeenCalledWith(1, items[0]);
                expect(mockView.updateItem).toHaveBeenCalledWith(2, items[1]);
                expect(onClick).toHaveBeenCalled();
                expect(mockView.endUpdate).toHaveBeenCalled();
                expect(mockBtnEl.classList.contains("checked")).toBe(true);
            });

            it("deselects all items when button is checked", () => {
                const items = [
                    { id: 1, name: "Item 1" }
                ];
                const mockView = {
                    getItems: vi.fn(() => items),
                    beginUpdate: vi.fn(),
                    endUpdate: vi.fn(),
                    updateItem: vi.fn()
                };
                const getGrid = vi.fn(() => ({
                    getView: vi.fn(() => mockView)
                }));
                const getId = vi.fn((x: any) => x.id);
                const setSelected = vi.fn();

                const button = GridSelectAllButtonHelper.define(getGrid as any, getId, vi.fn(() => true), setSelected);
                const mockBtnEl = document.createElement("div");
                mockBtnEl.classList.add("select-all-button", "checked");
                const event = { target: mockBtnEl } as any;

                button.onClick(event);

                expect(setSelected).toHaveBeenCalledWith(items[0], false);
                expect(mockBtnEl.classList.contains("checked")).toBe(false);
            });

            it("handles no onClick callback", () => {
                const items = [{ id: 1 }];
                const mockView = {
                    getItems: vi.fn(() => items),
                    beginUpdate: vi.fn(),
                    endUpdate: vi.fn(),
                    updateItem: vi.fn()
                };
                const getGrid = vi.fn(() => ({
                    getView: vi.fn(() => mockView)
                }));
                const getId = vi.fn((x: any) => x.id);
                const setSelected = vi.fn();

                const button = GridSelectAllButtonHelper.define(getGrid as any, getId, vi.fn(() => false), setSelected);
                const mockBtnEl = document.createElement("div");
                mockBtnEl.classList.add("select-all-button");
                const event = { target: mockBtnEl } as any;

                expect(() => button.onClick(event)).not.toThrow();
            });

            it("handles event target without closest button", () => {
                const items = [{ id: 1 }];
                const mockView = {
                    getItems: vi.fn(() => items),
                    beginUpdate: vi.fn(),
                    endUpdate: vi.fn(),
                    updateItem: vi.fn()
                };
                const getGrid = vi.fn(() => ({
                    getView: vi.fn(() => mockView)
                }));
                const getId = vi.fn((x: any) => x.id);
                const setSelected = vi.fn();

                const button = GridSelectAllButtonHelper.define(getGrid as any, getId, vi.fn(() => false), setSelected);
                const event = { target: document.createElement("div") } as any;

                expect(() => button.onClick(event)).not.toThrow();
            });
        });
    });

    describe("update", () => {
        it("does nothing if no toolbar found", () => {
            const grid = {
                getElement: vi.fn(() => document.createElement("div")),
                getView: vi.fn(() => ({
                    getItems: vi.fn(() => [])
                }))
            };
            expect(() => GridSelectAllButtonHelper.update(grid as any, vi.fn())).not.toThrow();
        });

        it("adds checked class when all items are selected", () => {
            const element = document.createElement("div");
            const toolbar = document.createElement("div");
            toolbar.className = "s-Toolbar";
            element.appendChild(toolbar);

            const grid = {
                getElement: vi.fn(() => element),
                getView: vi.fn(() => ({
                    getItems: vi.fn(() => [{ id: 1 }, { id: 2 }])
                }))
            };

            GridSelectAllButtonHelper.update(grid as any, vi.fn(() => true));
            expect(mockFindButton.toggleClass).toHaveBeenCalledWith('checked', true);
        });

        it("removes checked class when not all items are selected", () => {
            const element = document.createElement("div");
            const toolbar = document.createElement("div");
            toolbar.className = "s-Toolbar";
            element.appendChild(toolbar);

            const grid = {
                getElement: vi.fn(() => element),
                getView: vi.fn(() => ({
                    getItems: vi.fn(() => [{ id: 1 }, { id: 2 }])
                }))
            };

            GridSelectAllButtonHelper.update(grid as any, vi.fn(() => false));
            expect(mockFindButton.toggleClass).toHaveBeenCalledWith('checked', false);
        });

        it("handles empty items list", () => {
            const element = document.createElement("div");
            const toolbar = document.createElement("div");
            toolbar.className = "s-Toolbar";
            element.appendChild(toolbar);

            const grid = {
                getElement: vi.fn(() => element),
                getView: vi.fn(() => ({
                    getItems: vi.fn(() => [])
                }))
            };

            GridSelectAllButtonHelper.update(grid as any, vi.fn(() => true));
            expect(mockFindButton.toggleClass).toHaveBeenCalledWith('checked', false);
        });
    });
});
