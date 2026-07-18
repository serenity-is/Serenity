import { GridSelectAllButtonHelper } from "./gridselectallbuttonhelper";

describe("GridSelectAllButtonHelper", () => {
    describe("define", () => {
        it("returns a ToolButton with select-all action", () => {
            const getGrid = vi.fn();
            const getId = vi.fn();
            const getSelected = vi.fn();
            const setSelected = vi.fn();

            const button = GridSelectAllButtonHelper.define(getGrid, getId, getSelected, setSelected);
            // The default text comes from CheckTreeEditorTexts.SelectAll which is "Controls.CheckTreeEditor.SelectAll"
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
    });
});
