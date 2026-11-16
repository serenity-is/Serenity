import { afterEach, describe, expect, it } from "vitest";
import { Fluent, type PropertyItem } from "../../base";
import { BooleanEditor } from "../editors/booleaneditor";
import { StringEditor } from "../editors/stringeditor";
import { PropertyGrid, PropertyGridMode } from "./propertygrid";

const grids: PropertyGrid[] = [];

afterEach(() => {
    grids.splice(0).forEach(x => x.destroy());
    document.body.innerHTML = "";
});

function createGrid(items: PropertyItem[], value?: any, mode?: PropertyGridMode): PropertyGrid {
    const grid = new PropertyGrid({ items, value, mode }).init();
    document.body.append(grid.domNode);
    grids.push(grid);
    return grid;
}

function field(grid: PropertyGrid, name: string): HTMLElement {
    return grid.domNode.querySelector<HTMLElement>(`.field[data-itemname="${name}"]`);
}

function isHidden(element: Element): boolean {
    return !!(element as HTMLElement).hidden || (element as HTMLElement).style.display === "none";
}

function editor<T>(grid: PropertyGrid, name: string): T {
    return grid.get_editors()[grid.get_items().findIndex(x => x.name === name)] as T;
}

/** Sets the value of a text editor as the user does, which the grid hears by the change event. */
function type(grid: PropertyGrid, name: string, value: string) {
    const input = editor<StringEditor>(grid, name);
    input.value = value;
    Fluent.trigger(input.domNode, "change");
}

function check(grid: PropertyGrid, name: string, value: boolean) {
    const box = editor<BooleanEditor>(grid, name);
    box.value = value;
    Fluent.trigger(box.domNode, "change");
}

describe("PropertyGrid conditions", () => {
    it("shows a field only while another one has one of the values, as it changes", () => {
        const grid = createGrid([
            { name: "Action", editorType: StringEditor },
            { name: "Url", editorType: StringEditor, visibleWhen: [{ field: "Action", values: [2, 3] }] }
        ], { Action: "1" });

        expect(isHidden(field(grid, "Url"))).toBe(true);
        type(grid, "Action", "2");
        expect(isHidden(field(grid, "Url"))).toBe(false);
        type(grid, "Action", "4");
        expect(isHidden(field(grid, "Url"))).toBe(true);
    });

    it("checks the values that are loaded, without events of the editors", () => {
        const grid = createGrid([
            { name: "Action", editorType: StringEditor },
            { name: "Url", editorType: StringEditor, visibleWhen: [{ field: "Action", values: [2] }] }
        ]);

        expect(isHidden(field(grid, "Url"))).toBe(true);
        grid.load({ Action: 2 });
        expect(isHidden(field(grid, "Url"))).toBe(false);
    });

    it("shows a field while a checkbox is checked, and another while a field is empty", () => {
        const grid = createGrid([
            { name: "ShowTrend", editorType: BooleanEditor },
            { name: "Style", editorType: StringEditor, visibleWhen: [{ field: "ShowTrend" }] },
            { name: "Column", editorType: StringEditor },
            { name: "Previous", editorType: StringEditor, visibleWhen: [{ field: "Column", isNot: true }] }
        ], { ShowTrend: false });

        expect(isHidden(field(grid, "Style"))).toBe(true);
        expect(isHidden(field(grid, "Previous"))).toBe(false);
        check(grid, "ShowTrend", true);
        type(grid, "Column", "Day");
        expect(isHidden(field(grid, "Style"))).toBe(false);
        expect(isHidden(field(grid, "Previous"))).toBe(true);
    });

    it("needs all the conditions of a field", () => {
        const grid = createGrid([
            { name: "A", editorType: StringEditor },
            { name: "B", editorType: StringEditor },
            { name: "C", editorType: StringEditor, visibleWhen: [{ field: "A" }, { field: "B", values: ["x"] }] }
        ], { A: "1" });

        expect(isHidden(field(grid, "C"))).toBe(true);
        type(grid, "B", "x");
        expect(isHidden(field(grid, "C"))).toBe(false);
    });

    it("hides the fields that depend on a field hidden by its condition", () => {
        const grid = createGrid([
            { name: "Column", editorType: StringEditor },
            { name: "ShowTrend", editorType: BooleanEditor, visibleWhen: [{ field: "Column" }] },
            { name: "Style", editorType: StringEditor, visibleWhen: [{ field: "ShowTrend" }] }
        ], { Column: "Day", ShowTrend: true });

        expect(isHidden(field(grid, "Style"))).toBe(false);
        type(grid, "Column", "");
        expect(isHidden(field(grid, "ShowTrend"))).toBe(true);
        expect(isHidden(field(grid, "Style"))).toBe(true);
    });

    it("never shows a field that is hidden otherwise", () => {
        const grid = createGrid([
            { name: "A", editorType: StringEditor },
            { name: "B", editorType: StringEditor, visible: false, visibleWhen: [{ field: "A" }] },
            { name: "C", editorType: StringEditor, hideOnInsert: true, visibleWhen: [{ field: "A" }] }
        ], { A: "1" }, PropertyGridMode.insert);

        type(grid, "A", "2");
        expect(isHidden(field(grid, "B"))).toBe(true);
        expect(isHidden(field(grid, "C"))).toBe(true);
    });

    it("hides a category whose fields are all hidden, and shows it again", () => {
        const grid = createGrid([
            { name: "Action", editorType: StringEditor, category: "Main" },
            { name: "Url", editorType: StringEditor, category: "Link", visibleWhen: [{ field: "Action", values: ["link"] }] },
            { name: "NewTab", editorType: BooleanEditor, category: "Link", visibleWhen: [{ field: "Action", values: ["link"] }] }
        ]);

        const category = grid.domNode.querySelector(`.category[data-category="Link"]`);
        expect(isHidden(category)).toBe(true);
        expect(isHidden(grid.domNode.querySelector(`.category[data-category="Main"]`))).toBe(false);
        type(grid, "Action", "link");
        expect(isHidden(category)).toBe(false);
    });

    it("hides a tab whose fields are all hidden", () => {
        const grid = createGrid([
            { name: "Action", editorType: StringEditor, tab: "General" },
            { name: "Url", editorType: StringEditor, tab: "Link", visibleWhen: [{ field: "Action", values: ["link"] }] }
        ]);

        const tabs = grid.domNode.querySelectorAll<HTMLElement>(".property-tabs > li");
        expect(isHidden(tabs[0])).toBe(false);
        expect(isHidden(tabs[1])).toBe(true);
        type(grid, "Action", "link");
        expect(isHidden(tabs[1])).toBe(false);
    });

    it("makes a field read only by its conditions, keeping one that is read only anyway", () => {
        const grid = createGrid([
            { name: "Locked", editorType: BooleanEditor },
            { name: "Code", editorType: StringEditor, readOnlyWhen: [{ field: "Locked" }] },
            { name: "Fixed", editorType: StringEditor, readOnly: true, readOnlyWhen: [{ field: "Locked" }] }
        ]);

        expect(editor<StringEditor>(grid, "Code").domNode.hasAttribute("readonly")).toBe(false);
        check(grid, "Locked", true);
        expect(editor<StringEditor>(grid, "Code").domNode.hasAttribute("readonly")).toBe(true);
        check(grid, "Locked", false);
        expect(editor<StringEditor>(grid, "Code").domNode.hasAttribute("readonly")).toBe(false);
        expect(editor<StringEditor>(grid, "Fixed").domNode.hasAttribute("readonly")).toBe(true);
    });

    it("makes a field required by its conditions, unless it is hidden", () => {
        const grid = createGrid([
            { name: "Action", editorType: StringEditor },
            {
                name: "Url", editorType: StringEditor, requiredWhen: [{ field: "Action", values: ["link"] }],
                visibleWhen: [{ field: "Action", values: ["link", "dialog"] }]
            }
        ]);

        const url = editor<StringEditor>(grid, "Url");
        const caption = () => field(grid, "Url").querySelector(".caption sup");
        type(grid, "Action", "dialog");
        expect(url.domNode.classList.contains("required")).toBe(false);
        type(grid, "Action", "link");
        expect(url.domNode.classList.contains("required")).toBe(true);
        expect(caption()).not.toBeNull();
        type(grid, "Action", "");
        expect(url.domNode.classList.contains("required")).toBe(false);
        expect(caption()).toBeNull();
    });

    it("keeps the value of a hidden field, unless it is cleared when hidden", () => {
        const grid = createGrid([
            { name: "Action", editorType: StringEditor },
            { name: "Url", editorType: StringEditor, visibleWhen: [{ field: "Action", values: ["link"] }] },
            { name: "Dialog", editorType: StringEditor, visibleWhen: [{ field: "Action", values: ["dialog"] }], clearWhenHidden: true }
        ], { Action: "link", Url: "/a", Dialog: "X" });

        const saved = grid.save();
        expect(saved.Url).toBe("/a");
        expect(saved.Dialog).toBeNull();

        type(grid, "Action", "dialog");
        expect(grid.save()).toMatchObject({ Url: "/a", Dialog: "X" });
    });

    it("leaves a field whose conditions didn't change as it is, e.g. one a dialog made read only", () => {
        const grid = createGrid([
            { name: "A", editorType: StringEditor },
            { name: "B", editorType: StringEditor },
            { name: "C", editorType: StringEditor, readOnlyWhen: [{ field: "A", values: ["x"] }] }
        ]);

        editor<StringEditor>(grid, "C").domNode.setAttribute("readonly", "readonly");
        type(grid, "B", "anything");
        expect(editor<StringEditor>(grid, "C").domNode.hasAttribute("readonly")).toBe(true);
    });

    it("stops listening when destroyed", () => {
        const grid = createGrid([
            { name: "A", editorType: StringEditor },
            { name: "B", editorType: StringEditor, visibleWhen: [{ field: "A" }] }
        ]);

        const node = grid.domNode;
        grid.destroy();
        grids.splice(grids.indexOf(grid), 1);
        expect(() => Fluent.trigger(node, "change")).not.toThrow();
    });
});
