import { describe, expect, it, vi } from "vitest";
import { SelectEditor } from "./selecteditor";
import { ComboboxEditor } from "./comboboxeditor";
import { Select2, type Select2Item } from "./select2";
import { addLocalText, registerEnum } from "../../base";
import { EnumEditor } from "./enumeditor";

const item = (id: string, text: string): Select2Item => ({ id, text });

// the texts of the widget are local texts, which the tests load for the ones they check
addLocalText({
    Tests: {
        ComboboxEditor: {
            Low: "Low priority",
            High: "High priority"
        }
    },
    Controls: {
        SelectEditor: {
            ClearAll: "Clear all",
            ClearAllMatching: "Clear all matching",
            ClearShown: "Clear shown",
            SelectAll: "Select all",
            SelectAllMatching: "Select all matching",
            SelectShown: "Select shown",
            SelectedItems: "{0} selected"
        }
    }
});

function createSingle(opt: any = {}) {
    const element = document.body.appendChild(document.createElement("input")) as HTMLInputElement;
    const select = new Select2({
        element,
        placeholder: "Choose",
        allowClear: true,
        initSelection: (_element, callback) => callback(null),
        query: query => query.callback({
            results: [item("1", "One"), item("2", "Two")],
            more: false
        }),
        ...opt
    });
    return { element, select, internal: (element as any).select2 };
}

function createChecklist(opt: any = {}) {
    const element = document.body.appendChild(document.createElement("input")) as HTMLInputElement;
    const select = new Select2({
        element,
        multiple: true,
        initSelection: (_element, callback) => callback([]),
        checkList: true,
        selectAll: { text: "Select all", clearText: "Clear all" },
        selectionSummary: (items, container) => {
            container.textContent = items.map(x => x.text).join(", ");
        },
        query: query => query.callback({
            results: [
                item("1", "One"),
                item("2", "Two"),
                item("3", "Three")
            ],
            more: false
        }),
        ...opt
    });
    return { element, select, internal: (element as any).select2 };
}

const click = (element: Element) => element.dispatchEvent(new Event("mouseup", { bubbles: true }));

describe("the options that imply a multiple editor", () => {
    const items = [["1", "One"], ["2", "Two"], ["3", "Three"]];
    const multiple = (options: any) => {
        const editor = new SelectEditor({ items, ...options } as any);
        const result = editor["isMultiple"]();
        editor.destroy();
        return result;
    };

    it("make an editor multiple on their own, as neither means anything without it", () => {
        expect(multiple({})).toBe(false);
        expect(multiple({ multiple: true })).toBe(true);
        expect(multiple({ checkList: true })).toBe(true);
        expect(multiple({ delimited: true })).toBe(true);
    });

    it("give a checklist the check boxes, the item that selects all and the summary", () => {
        const editor = new SelectEditor({ items, checkList: true } as any);
        const opt = editor["getComboboxOptions"]();
        expect(opt.multiple).toBe(true);
        expect(opt.checkList).toBe(true);
        expect(opt.selectAll).toBe(true);
        expect(typeof opt.selectionSummary).toBe("function");
        editor.destroy();
    });

    it("write the value of a checklist as a list, like a multiple editor does", () => {
        const editor = new SelectEditor({ items, checkList: true } as any);
        editor.value = "1,3";
        const target: any = {};
        editor.getEditValue({ name: "Values" } as any, target);
        expect(target.Values).toEqual(["1", "3"]);
        editor.destroy();

        const delimited = new SelectEditor({ items, delimited: true } as any);
        delimited.value = "1,3";
        const delimiterTarget: any = {};
        delimited.getEditValue({ name: "Values" } as any, delimiterTarget);
        expect(delimiterTarget.Values).toBe("1,3");
        delimited.destroy();
    });
});

describe("the texts of the members of an enum", () => {
    it("come from the key of a member, the one its enum implies, or the text that travels with the enum", () => {
        enum TestEnum {
            Plain = 1,
            Keyed = 2,
            Literal = 3
        }

        const name = "EnumTestTexts.TestEnum";
        registerEnum(TestEnum, name, {
            enumKey: "EnumTestTexts.TestEnum",
            enumTextKeys: { [TestEnum.Keyed]: "EnumTestTexts.Keyed", [TestEnum.Literal]: "" },
            enumTexts: { [TestEnum.Literal]: "Shown as it is" }
        });
        addLocalText({
            Enums: { EnumTestTexts: { TestEnum: { Plain: "Plain text" } } },
            EnumTestTexts: { Keyed: "Keyed text" }
        });

        const texts = (options: any) => {
            const editor = new EnumEditor({ enumType: TestEnum, ...options } as any);
            const result = Object.fromEntries(editor.items.map(x => [x.id, x.text]));
            editor.destroy();
            return result;
        };

        expect(texts({})).toEqual({
            // a member without a key uses the one its enum implies, i.e. Enums.<enumKey>.<Member>
            "1": "Plain text",
            // a member with a key of its own is localized by it
            "2": "Keyed text",
            // a member with an empty key is shown as it is, with the text that travels with the enum
            "3": "Shown as it is"
        });
    });
});

describe("the text keys of the items of an editor", () => {
    it("localize the text of an item by its key field, and show the text as it is when it has no key", () => {
        const editor = new SelectEditor({
            items: [["1", "One"], ["2", "Two"]],
            textKeyField: "textKey"
        } as any);
        editor.addOption("3", "Three", { textKey: "Tests.ComboboxEditor.Low" });
        editor.addOption("4", "Four", { textKey: "Tests.ComboboxEditor.Unknown" });

        const texts = Object.fromEntries(editor.items.map(x => [x.id, x.text]));
        // an item of its own gives the text, and the key of its source, which the editor localizes
        expect(texts["3"]).toBe("Low priority");
        // a key without a text shows the text of the item, which tells more than the key
        expect(texts["4"]).toBe("Four");
        // an item without a key shows its text
        expect(texts["2"]).toBe("Two");

        editor.destroy();
    });

    it("localize the text of an item by itself, when the text is its key", () => {
        const editor = new SelectEditor({
            items: [["low", "Tests.ComboboxEditor.Low"], ["high", "Tests.ComboboxEditor.High"], ["none", "None"]],
            textKeyField: "text"
        } as any);

        expect(editor.items.map(x => x.text)).toEqual(["Low priority", "High priority", "None"]);
        // the ids are what the values are, not the texts
        expect(editor.items.map(x => x.id)).toEqual(["low", "high", "none"]);

        editor.destroy();
    });

    it("give an item its text from its key alone, when it has no text of its own", () => {
        const editor = new SelectEditor({ items: [], textField: "text", textKeyField: "textKey" } as any);

        // an item that has only a key, e.g. one that a source gives, has the text of its key
        expect(editor["itemText"]({ textKey: "Tests.ComboboxEditor.Low" } as any)).toBe("Low priority");
        // without a text for the key, the key is the text, as it is readable enough
        expect(editor["itemText"]({ textKey: "Tests.ComboboxEditor.Unknown" } as any)).toBe("Tests.ComboboxEditor.Unknown");
        // without a key either, an item has no text
        expect(editor["itemText"]({} as any)).toBe("");
        expect(editor["itemText"]({ text: null, textKey: "Tests.ComboboxEditor.High" } as any)).toBe("High priority");

        editor.destroy();
    });

    it("localize the text where the editor reads it, so a search and a selection see the same text", () => {
        const editor = new SelectEditor({
            items: [["1", "One"]],
            textField: "text",
            textKeyField: "textKey"
        } as any);

        const mapped = editor["mapItem"]({ id: "3", text: "Three", textKey: "Tests.ComboboxEditor.High" } as any);
        expect(mapped.text).toBe("High priority");

        editor.destroy();
    });

    it("take the items of a select editor as objects, e.g. the ones that a form field gives", () => {
        const editor = new SelectEditor({
            items: [
                { id: "low", textKey: "Tests.ComboboxEditor.Low" },
                { id: "high", textKey: "Tests.ComboboxEditor.High", text: "High" },
                { id: "none", text: "None" },
                { id: "10" },
                { id: "20", text: "Twenty", icon: "fa fa-truck" }
            ],
            textKeyField: "textKey"
        } as any);

        expect(editor.items.map(x => x.id)).toEqual(["low", "high", "none", "10", "20"]);
        expect(editor.items.map(x => x.text)).toEqual([
            // a text key gives the text of an item, which falls back to the key when there is no text for it, and an
            // item whose text is not given has none, as the value of an item is not a fallback of the editor
            "Low priority", "High priority", "None", "", "Twenty"
        ]);
        expect(editor.items[4].icon).toBe("fa fa-truck");

        editor.destroy();
    });

    it("show the texts of an item as they are when the editor does not say where their keys are", () => {
        // e.g. a lookup, whose texts are data, and may look like keys
        const editor = new SelectEditor({
            items: [["low", "Tests.ComboboxEditor.Low"], ["high", "Tests.ComboboxEditor.High"]]
        } as any);

        expect(editor.items.map(x => x.text)).toEqual(["Tests.ComboboxEditor.Low", "Tests.ComboboxEditor.High"]);

        editor.destroy();
    });
});

describe("the CSS class of the items of an editor", () => {
    it("comes from the field that the editor gives, or from the item itself, and the results show it", () => {
        const byField = new SelectEditor({
            items: [],
            textField: "text",
            cssClassField: "CssClass"
        } as any);
        // the class comes from the field of the source item that the editor gives, e.g. a column of an item type
        byField.addOption("1", "One", { CssClass: "text-danger" });
        expect(byField.items[0].cssClass).toBe("text-danger");
        expect(byField["getComboboxOptions"]().formatResultCssClass?.({ cssClass: "text-danger" } as any))
            .toBe("text-danger");
        byField.destroy();

        // an item of its own gives the class as well, e.g. one that a form field gives
        const own = new SelectEditor({
            items: [{ id: "danger", text: "Danger", cssClass: "text-danger" }]
        } as any);
        expect(own.items[0].cssClass).toBe("text-danger");
        // an item without a class has none, which the editor passes as it is
        expect(own["getComboboxOptions"]().formatResultCssClass?.({} as any)).toBeUndefined();
        own.destroy();
    });
});

describe("the placeholder of a combobox editor", () => {
    it("shows the icon of the placeholder, e.g. that of the field it filters, when nothing is selected", () => {
        const { select, internal } = createSingle({ placeholderIcon: "fa fa-tag" });

        select.open();
        select.close();

        expect(internal.selection.querySelector(".select2-chosen i")?.className).toContain("fa-tag");
        expect(internal.selection.querySelector(".select2-chosen").textContent.trim()).toBe("Choose");
        expect(internal.selection.classList.contains("select2-default")).toBe(true);

        select.destroy();
    });
});

describe("the items that select all the results of a checklist", () => {
    it("selects the results that are shown, and clears them when they are all selected", () => {
        const { select, internal } = createChecklist();
        select.open();

        const row = internal.results.querySelector(".select2-select-all");
        expect(row).toBeTruthy();
        expect(row.getAttribute("aria-label")).toBe("Select all");
        expect(row.classList.contains("select2-select-all-checked")).toBe(false);

        click(row.querySelector(".select2-result-label"));
        expect(select.val).toEqual(["1", "2", "3"]);
        expect(row.classList.contains("select2-select-all-checked")).toBe(true);
        expect(row.getAttribute("aria-label")).toBe("Clear all");
        expect(row.querySelector(".select2-result-label").textContent).toBe("Clear all");

        click(row.querySelector(".select2-result-label"));
        expect(select.val).toEqual([]);
        expect(row.getAttribute("aria-label")).toBe("Select all");

        select.destroy();
    });

    it("is partial when some of the results are selected", () => {
        const { select, internal } = createChecklist();
        select.open();

        select.data = [item("1", "One")];
        expect(internal.results.querySelector(".select2-select-all").classList.contains("select2-select-all-partial")).toBe(true);
        expect(internal.results.querySelector(".select2-select-all").getAttribute("aria-selected")).toBe("true");

        select.destroy();
    });

    it("unselects a result that is checked, which a checklist keeps in the list so that it can be clicked again", () => {
        const { select, internal } = createChecklist();
        select.data = [item("1", "One")];
        select.open();

        const row = [...(internal.results as HTMLElement).querySelectorAll<HTMLElement>(".select2-result")]
            .find(x => x.textContent === "One");
        expect(row.classList.contains("select2-selected")).toBe(true);

        click(row.querySelector(".select2-result-label"));
        expect(select.val).toEqual([]);
        expect(row.classList.contains("select2-selected")).toBe(false);

        select.destroy();
    });

    it("marks the results that are selected for a screen reader, e.g. the check boxes of a checklist", () => {
        const { select, internal } = createChecklist();
        select.open();
        select.data = [item("1", "One")];

        const selected = [...internal.results.querySelectorAll(".select2-result:not(.select2-select-all)")]
            .filter(x => x.getAttribute("aria-selected") === "true")
            .map(x => (x as any).select2data?.text);
        expect(selected).toEqual(["One"]);

        select.destroy();
    });
});

describe("the summary of the selection of a checklist", () => {
    it("is shown instead of a choice for each item, which keeps the items for data()", () => {
        const { element, select, internal } = createChecklist();
        select.data = [item("1", "One"), item("3", "Three")];

        expect(internal.selection.querySelectorAll(".select2-search-choice").length).toBe(0);
        expect(internal.selection.querySelector(".select2-selection-summary-content").textContent).toBe("One, Three");
        expect(element.value).toBe("1,3");
        expect((select.data as Select2Item[]).map(x => x.text)).toEqual(["One", "Three"]);

        select.destroy();
    });
});

describe("the options of the checklist of a select editor", () => {
    const items = Array.from({ length: 20 }, (_, i) => ["id" + i, "Item " + i]);

    it("shows the icon of the placeholder, and a search box only for a long list", () => {
        const editor = new SelectEditor({ items, placeholderIcon: "fa fa-tag" } as any);
        const opt = editor["getComboboxOptions"]();

        expect(opt.placeholderIcon).toBe("fa fa-tag");
        // a single editor is a combobox, which searches any list unless it says otherwise
        expect(opt.minimumResultsForSearch).toBeUndefined();
        expect(opt.selectAll).toBeUndefined();
        expect(opt.selectionSummary).toBeUndefined();

        editor.destroy();
    });

    it("gives a multiple editor the item that selects all the results, the check boxes and the summary, when asked", () => {
        const editor = new SelectEditor({ items, multiple: true, checkList: true, placeholderIcon: "fa fa-user" } as any);
        const opt = editor["getComboboxOptions"]();
        const result = opt.search({ searchTerm: null, skip: 0, take: 20 } as any);

        expect(opt.containerCssClass).toContain("select2-container-checklist");
        expect(opt.dropdownCssClass).toContain("select2-dropdown-checklist");
        expect(opt.checkList).toBe(true);
        // a checklist has no threshold of its own for the search box
        expect(opt.minimumResultsForSearch).toBeUndefined();
        // the select adds the item that selects all the results, with its texts, so the results are left as they are
        expect(opt.selectAll).toBe(true);
        expect(typeof opt.selectionSummary).toBe("function");
        expect(opt.placeholderIcon).toBe("fa fa-user");
        expect((result as any).items.length).toBe(20);

        editor.destroy();
    });

    it("renders a checklist with a check box for each result, and the item that selects all of them", () => {
        vi.useFakeTimers();
        try {
            const editor = new SelectEditor({ items: [["a", "A"], ["b", "B"]], multiple: true, checkList: true } as any);
            const internal: any = (editor.domNode as any).select2;

            internal.open();
            vi.advanceTimersByTime(0);

            expect(internal.results.querySelectorAll(".select2-result .select2-checkbox").length).toBe(3);
            const all = internal.results.querySelector(".select2-select-all");
            expect(all.querySelector(".select2-select-all-text").textContent).toBe("Select all");

            editor.destroy();
        }
        finally {
            vi.useRealTimers();
        }
    });

    it("keeps the options that the editor sets, e.g. its own threshold for the search box", () => {
        const editor = new SelectEditor({ items, multiple: true, checkList: true, minimumResultsForSearch: 3 } as any);
        const opt = editor["getComboboxOptions"]();
        expect(opt.minimumResultsForSearch).toBe(3);
        editor.destroy();
    });

    it("pages a source by the page size of the editor, e.g. a list service", () => {
        const editor = new SelectEditor({ items, pageSize: 8 } as any);
        const opt = editor["getComboboxOptions"]();
        expect(opt.pageSize).toBe(8);
        editor.destroy();

        const standard = new SelectEditor({ items } as any);
        expect(standard["getComboboxOptions"]().pageSize).toBe(100);
        standard.destroy();
    });

    it("is not used unless the editor asks for it", () => {
        const editor = new SelectEditor({ items, multiple: true } as any);
        const opt = editor["getComboboxOptions"]();
        const result = opt.search({ searchTerm: null, skip: 0, take: 20 } as any);

        expect(opt.containerCssClass).toBeUndefined();
        expect(opt.minimumResultsForSearch).toBeUndefined();
        expect(opt.selectAll).toBeUndefined();
        expect((result as any).items.length).toBe(20);

        editor.destroy();
    });
});

describe("the item that selects all the results, which the select adds", () => {
    it("has the texts of the select editor by default", () => {
        const { select, internal } = createChecklist({ selectAll: true });
        select.open();

        const row = internal.results.querySelector(".select2-select-all");
        expect(row.querySelector(".select2-result-label").textContent).toBe("Select all");
        click(row.querySelector(".select2-result-label"));
        expect(row.querySelector(".select2-result-label").textContent).toBe("Clear all");

        select.destroy();
    });

    it("is added only when asked, and is not confused with an item without an id, e.g. the parent of a group", () => {
        const grouped = {
            results: [{ text: "Group", children: [item("1", "One"), item("2", "Two")] }],
            more: false
        };

        const { select, internal } = createChecklist({ selectAll: false, query: (q: any) => q.callback(grouped) });
        select.open();
        expect(internal.results.querySelector(".select2-select-all")).toBeNull();
        select.destroy();

        const withAll = createChecklist({ query: (q: any) => q.callback(grouped) });
        withAll.select.open();
        const rows = withAll.internal.results.querySelectorAll(".select2-select-all");
        expect(rows.length).toBe(1);
        expect(rows[0].textContent).toBe("Select all");
        click(rows[0].querySelector(".select2-result-label"));
        expect(withAll.select.val).toEqual(["1", "2"]);
        withAll.select.destroy();
    });

    it("is not added when there is no result that it can select", () => {
        const { select, internal } = createChecklist({
            query: (q: any) => q.callback({ results: [{ ...item("1", "One"), disabled: true }], more: false })
        });
        select.open();
        expect(internal.results.querySelector(".select2-select-all")).toBeNull();
        select.destroy();
    });
});

describe("a summary of the selection without the checklist option", () => {
    it("does not keep the selected results in the list to toggle them, which only a checklist does", () => {
        const highlightable = (checkList: boolean) => {
            const { select, internal } = createChecklist({ checkList, selectAll: false });
            select.data = [item("1", "One")];
            select.open();
            const texts = internal.findHighlightableChoices().map((x: HTMLElement) => x.textContent);
            select.destroy();
            return texts;
        };

        expect(highlightable(false)).toEqual(["Two", "Three"]);
        expect(highlightable(true)).toEqual(["One", "Two", "Three"]);
    });
});

describe("the defaults of the combobox editors", () => {
    it("name as many items in the summary of a checklist as the default says, unless the editor says otherwise", () => {
        const saved = ComboboxEditor.defaults.maxSummaryItems;
        ComboboxEditor.defaults.maxSummaryItems = 2;
        try {
            const items = [["1", "A"], ["2", "B"], ["3", "C"]];
            const editor = new SelectEditor({ items, multiple: true, checkList: true } as any);
            const container = document.createElement("div");
            editor["renderSelectionSummary"](items.map(([id, text]) => ({ id, text })), container);
            // the count of the rest is an element of its own, which is not cut with the names when they do not fit
            expect(container.querySelector(".select2-selection-summary-text").textContent).toBe("A, B");
            expect(container.querySelector(".select2-selection-summary-more").textContent).toBe("+1");
            expect(container.title).toBe("A, B, C");

            const own = new SelectEditor({ items, multiple: true, checkList: true, maxSummaryItems: 3 } as any);
            own["renderSelectionSummary"](items.map(([id, text]) => ({ id, text })), container);
            expect(container.textContent).toBe("A, B, C");
            expect(container.querySelector(".select2-selection-summary-more")).toBeNull();

            // -1 names them all against such a default
            const all = new SelectEditor({ items, multiple: true, checkList: true, maxSummaryItems: -1 } as any);
            all["renderSelectionSummary"](items.map(([id, text]) => ({ id, text })), container);
            expect(container.querySelector(".select2-selection-summary-text").textContent).toBe("A, B, C");
            expect(container.querySelector(".select2-selection-summary-more").textContent).toBe("(3)");

            // while null is not a value of its own, and takes the default, like an option that is not given
            const none = new SelectEditor({ items, multiple: true, checkList: true, maxSummaryItems: null } as any);
            none["renderSelectionSummary"](items.map(([id, text]) => ({ id, text })), container);
            expect(container.querySelector(".select2-selection-summary-text").textContent).toBe("A, B");
            expect(container.querySelector(".select2-selection-summary-more").textContent).toBe("+1");

            editor.destroy();
            own.destroy();
            all.destroy();
            none.destroy();
        }
        finally {
            ComboboxEditor.defaults.maxSummaryItems = saved;
        }
    });

    it("name every item in the summary by default, and show how many are selected after them", () => {
        const items = [["1", "A"], ["2", "B"], ["3", "C"], ["4", "D"], ["5", "E"]];
        const editor = new SelectEditor({ items, multiple: true, checkList: true } as any);
        const container = document.createElement("div");
        editor["renderSelectionSummary"](items.map(([id, text]) => ({ id, text })), container);

        // the styles cut the names that do not fit, so the whole list is there, and the count is the selection
        expect(ComboboxEditor.defaults.maxSummaryItems).toBe(-1);
        expect(container.querySelector(".select2-selection-summary-text").textContent).toBe("A, B, C, D, E");
        expect(container.querySelector(".select2-selection-summary-more").textContent).toBe("(5)");
        expect(container.title).toBe("A, B, C, D, E");

        // -1, which is the default, says the same, and so do the other negative values, which are reserved for
        // summaries to come
        for (const maxSummaryItems of [-1, -2]) {
            const explicit = new SelectEditor({ items, multiple: true, checkList: true, maxSummaryItems } as any);
            explicit["renderSelectionSummary"](items.map(([id, text]) => ({ id, text })), container);
            expect(container.querySelector(".select2-selection-summary-text").textContent).toBe("A, B, C, D, E");
            expect(container.querySelector(".select2-selection-summary-more").textContent).toBe("(5)");
            explicit.destroy();
        }

        editor.destroy();
    });

    it("show only how many are selected when maxSummaryItems is zero", () => {
        const items = [["1", "A"], ["2", "B"], ["3", "C"]];
        const editor = new SelectEditor({ items, multiple: true, checkList: true, maxSummaryItems: 0 } as any);
        const container = document.createElement("div");
        editor["renderSelectionSummary"](items.map(([id, text]) => ({ id, text })), container);

        expect(container.textContent).toBe("3 selected");
        // the names are still there for whoever looks at the tooltip
        expect(container.title).toBe("A, B, C");

        editor.destroy();
    });

    it("take a callback for the empty item text, which is called when the editor is created", () => {
        const saved = ComboboxEditor.defaults.emptyItemText;
        let text = "--select--";
        ComboboxEditor.defaults.emptyItemText = () => text;
        try {
            expect(new SelectEditor({ items: [] } as any)["emptyItemText"]()).toBe("--select--");
            text = "--choose--";
            expect(new SelectEditor({ items: [] } as any)["emptyItemText"]()).toBe("--choose--");
        }
        finally {
            ComboboxEditor.defaults.emptyItemText = saved;
        }
    });
});

describe("the search box of a checklist", () => {
    const key = (target: Element, key: string) => target.dispatchEvent(new KeyboardEvent("keydown", { key, bubbles: true, cancelable: true }));
    const type = (input: HTMLInputElement, text: string) => {
        input.value += text;
        input.dispatchEvent(new Event("input", { bubbles: true }));
    };

    it("is in the dropdown, as the control has nothing to type in, only an input that takes the focus", () => {
        const { select, internal } = createChecklist();

        expect(internal.selection.querySelector("input.select2-input")).toBeNull();
        const focusser = (internal.selection as HTMLElement).querySelector<HTMLInputElement>(".select2-focusser");
        expect(focusser.classList.contains("select2-offscreen")).toBe(true);
        expect(focusser.getAttribute("role")).toBe("combobox");

        select.open();
        expect(internal.search.closest(".select2-drop")).toBe(internal.dropdown);
        expect(internal.search.closest(".select2-search")).toBeTruthy();
        expect(focusser.getAttribute("aria-expanded")).toBe("true");

        select.destroy();
    });

    it("takes what is typed in the control, which opens the dropdown", () => {
        // a touch device, as jsdom looks like one, does not move the focus, see shouldFocusInput
        const { select, internal } = createChecklist({ shouldFocusInput: () => true });
        const focusser = internal.focusser as HTMLInputElement;
        focusser.focus();

        type(focusser, "t");
        expect(internal.opened()).toBe(true);
        expect(internal.search.value).toBe("t");
        expect(focusser.value).toBe("");
        expect(document.activeElement).toBe(internal.search);

        select.destroy();
    });

    it("is opened by the arrow keys, Enter or Space on the control, without typing them", () => {
        for (const k of ["ArrowDown", "ArrowUp", "Enter", " "]) {
            const { select, internal } = createChecklist();
            key(internal.focusser, k);
            expect(internal.opened()).toBe(true);
            expect(internal.search.value).toBe("");
            select.destroy();
        }
    });

    it("keeps what is typed, and the dropdown open, while results are checked and unchecked", () => {
        const { select, internal } = createChecklist();
        select.open();
        type(internal.search, "o");

        const row = () => [...(internal.results as HTMLElement).querySelectorAll<HTMLElement>(".select2-result")]
            .find(x => x.textContent === "One");
        click(row().querySelector(".select2-result-label"));
        expect(select.val).toEqual(["1"]);
        expect(internal.opened()).toBe(true);
        expect(internal.search.value).toBe("o");
        expect(row().getAttribute("aria-selected")).toBe("true");

        click(row().querySelector(".select2-result-label"));
        expect(select.val).toEqual([]);
        expect(internal.opened()).toBe(true);

        select.destroy();
    });

    it("ticks the highlighted result with Space, as a check box, and accepts the selection with Enter, which closes it", () => {
        const { select, internal } = createChecklist({ selectAll: false, shouldFocusInput: () => true });
        select.open();

        key(internal.search, "ArrowDown");
        key(internal.search, " ");
        expect(select.val).toEqual(["2"]);
        expect(internal.opened()).toBe(true);

        // the checked result is moved to by the arrow keys too, and unticked
        key(internal.search, "ArrowUp");
        key(internal.search, " ");
        expect(select.val).toEqual(["2", "1"]);
        key(internal.search, " ");
        expect(select.val).toEqual(["2"]);

        key(internal.search, "Enter");
        expect(internal.opened()).toBe(false);
        expect(select.val).toEqual(["2"]);
        expect(document.activeElement).toBe(internal.focusser);

        select.destroy();
    });

    it("closes with Escape, giving the focus back to the control", () => {
        const { select, internal } = createChecklist({ selectAll: false, shouldFocusInput: () => true });
        select.open();
        key(internal.search, "Escape");
        expect(internal.opened()).toBe(false);
        expect(internal.search.value).toBe("");
        expect(document.activeElement).toBe(internal.focusser);
        expect(internal.focusser.getAttribute("aria-expanded")).toBe("false");

        select.destroy();
    });

    it("ticks the item that selects all the results with Space, when nothing is typed", () => {
        const { select, internal } = createChecklist();
        select.open();

        // the item that selects all the results is the first one, which is highlighted on opening
        expect(internal.results.querySelector(".select2-highlighted").closest(".select2-result").classList.contains("select2-select-all")).toBe(true);
        key(internal.search, " ");
        expect(select.val).toEqual(["1", "2", "3"]);
        expect(internal.opened()).toBe(true);

        key(internal.search, " ");
        expect(select.val).toEqual([]);

        select.destroy();
    });

    it("ticks it with Space on a touch device too, where the search box does not get the focus on opening", () => {
        // jsdom looks like a touch device, so the search box is not focused, and the keys of a hardware keyboard come
        // from the document instead
        const { select, internal } = createChecklist();
        select.open();
        expect(document.activeElement).not.toBe(internal.search);

        const space = new KeyboardEvent("keydown", { key: " ", bubbles: true, cancelable: true });
        document.body.dispatchEvent(space);
        expect(space.defaultPrevented).toBe(true);
        expect(select.val).toEqual(["1", "2", "3"]);
        expect(internal.search.value).toBe("");
        expect(document.activeElement).not.toBe(internal.search);

        // another key is typed in the search box, which it focuses
        key(internal.search, "ArrowDown");
        const letter = new KeyboardEvent("keydown", { key: "t", bubbles: true, cancelable: true });
        document.body.dispatchEvent(letter);
        expect(letter.defaultPrevented).toBe(false);
        expect(document.activeElement).toBe(internal.search);

        select.destroy();
    });

    it("highlights no result after something is typed, so that Space is typed, until one is highlighted", () => {
        const { select, internal } = createChecklist({ selectAll: false });
        select.open();
        const highlighted = () => internal.results.querySelector(".select2-highlighted")?.textContent ?? null;
        expect(highlighted()).toBe("One");

        // what is typed is searched for, e.g. "Dairy Products", with no result highlighted for Space to tick
        type(internal.search, "o");
        expect(highlighted()).toBeNull();
        const typed = new KeyboardEvent("keydown", { key: " ", bubbles: true, cancelable: true });
        internal.search.dispatchEvent(typed);
        expect(typed.defaultPrevented).toBe(false);
        expect(select.val).toEqual([]);

        // Down goes to the first result, which Space then ticks
        key(internal.search, "ArrowDown");
        expect(highlighted()).toBe("One");
        const ticked = new KeyboardEvent("keydown", { key: " ", bubbles: true, cancelable: true });
        internal.search.dispatchEvent(ticked);
        expect(ticked.defaultPrevented).toBe(true);
        expect(select.val).toEqual(["1"]);
        expect(highlighted()).toBe("One");

        // typing again clears the highlight
        type(internal.search, "n");
        expect(highlighted()).toBeNull();

        // and an empty search highlights the first result again, as on opening
        internal.search.value = "";
        internal.search.dispatchEvent(new Event("input", { bubbles: true }));
        expect(highlighted()).toBe("One");

        select.destroy();
    });

    it("goes past the item that selects all the results with Down after something is typed, to the first one found", () => {
        const { select, internal } = createChecklist();
        select.open();
        const highlighted = () => internal.results.querySelector(".select2-highlighted")?.textContent ?? null;

        type(internal.search, "o");
        expect(highlighted()).toBeNull();
        key(internal.search, "ArrowDown");
        expect(highlighted()).toBe("One");
        key(internal.search, "ArrowUp");
        expect(highlighted()).toBe("Select all matching");

        select.destroy();
    });

    it("accepts the selection with Enter on a touch device too, where the keys come from the document", () => {
        const { select, internal } = createChecklist({ selectAll: false });
        select.open();
        document.body.dispatchEvent(new KeyboardEvent("keydown", { key: "ArrowDown", bubbles: true, cancelable: true }));
        document.body.dispatchEvent(new KeyboardEvent("keydown", { key: " ", bubbles: true, cancelable: true }));
        expect(select.val).toEqual(["2"]);
        document.body.dispatchEvent(new KeyboardEvent("keydown", { key: "Enter", bubbles: true, cancelable: true }));
        expect(internal.opened()).toBe(false);
        expect(select.val).toEqual(["2"]);
        select.destroy();
    });

    it("closes with Tab, without checking the highlighted result", () => {
        const { select, internal } = createChecklist({ selectAll: false, shouldFocusInput: () => true });
        select.open();
        key(internal.search, "Tab");
        expect(internal.opened()).toBe(false);
        expect(select.val).toEqual([]);
        expect(document.activeElement).toBe(internal.focusser);
        select.destroy();
    });

    it("is hidden for a short list, unless something is typed, as the one of a single select", () => {
        const short = createChecklist({ minimumResultsForSearch: 10 });
        short.select.open();
        expect(short.internal.dropdown.querySelector(".select2-search").classList.contains("select2-search-hidden")).toBe(true);
        short.select.close();

        type(short.internal.focusser, "t");
        expect(short.internal.dropdown.querySelector(".select2-search").classList.contains("select2-search-hidden")).toBe(false);
        short.select.destroy();

        const long = createChecklist({ minimumResultsForSearch: 2 });
        long.select.open();
        expect(long.internal.dropdown.querySelector(".select2-search").classList.contains("select2-search-hidden")).toBe(false);
        long.select.destroy();
    });

    it("does not check more results than the maximum selection size", () => {
        const { select, internal } = createChecklist({ selectAll: false, maximumSelectionSize: 1 });
        select.open();
        const rows = [...(internal.results as HTMLElement).querySelectorAll<HTMLElement>(".select2-result-label")];
        click(rows[0]);
        click(rows[1]);
        expect(select.val).toEqual(["1"]);
        select.destroy();
    });
});

describe("the search field of a multiple select", () => {
    it("has no width of its own, which the styles give it, so the control does not grow as the user types", () => {
        const element = document.body.appendChild(document.createElement("input")) as HTMLInputElement;
        const select = new Select2({
            element,
            multiple: true,
            initSelection: (_element, callback) => callback([]),
            query: query => query.callback({ results: [item("1", "One"), item("2", "Two")], more: false })
        });
        const internal = (element as any).select2;

        select.open();
        internal.search.value = "a long text that is typed";
        internal.search.dispatchEvent(new KeyboardEvent("keyup", { key: "t", bubbles: true }));
        expect(internal.search.style.width).toBe("");

        select.data = [item("1", "One")];
        select.close();
        expect(internal.search.style.width).toBe("");

        select.destroy();
    });
});

describe("the caret and the clear button of a checklist", () => {
    it("has a caret, as a single select, which opens and closes it", () => {
        const { select, internal } = createChecklist();
        const arrow = internal.container.querySelector(":scope > .select2-arrow");
        expect(arrow).toBeTruthy();

        arrow.dispatchEvent(new MouseEvent("click", { bubbles: true, cancelable: true }));
        expect(internal.opened()).toBe(true);
        arrow.dispatchEvent(new MouseEvent("click", { bubbles: true, cancelable: true }));
        expect(internal.opened()).toBe(false);

        select.destroy();
    });

    it("shows the clear button when something is selected and the select allows clearing it", () => {
        const { select, internal } = createChecklist({ allowClear: true });
        expect(internal.container.classList.contains("select2-allowclear")).toBe(false);

        select.data = [item("1", "One"), item("2", "Two")];
        expect(internal.container.classList.contains("select2-allowclear")).toBe(true);

        select.destroy();

        const noClear = createChecklist();
        noClear.select.data = [item("1", "One")];
        expect(noClear.internal.container.classList.contains("select2-allowclear")).toBe(false);
        noClear.select.destroy();
    });

    it("clears the whole selection with its clear button, without opening the dropdown", () => {
        const { element, select, internal } = createChecklist({ allowClear: true });
        select.data = [item("1", "One"), item("2", "Two")];
        const changed = vi.fn();
        element.addEventListener("change", changed);

        internal.container.querySelector(":scope > .select2-search-choice-close")
            .dispatchEvent(new MouseEvent("click", { bubbles: true, cancelable: true }));

        expect(select.val).toEqual([]);
        expect(select.data).toEqual([]);
        expect(internal.opened()).toBe(false);
        expect(changed).toHaveBeenCalled();
        expect(internal.container.classList.contains("select2-allowclear")).toBe(false);
        expect(internal.selection.querySelector(".select2-selection-summary-content").textContent).toBe("");

        select.destroy();
    });
});

describe("the keys that remove items from a checklist", () => {
    const key = (target: Element, key: string) => {
        const event = new KeyboardEvent("keydown", { key, bubbles: true, cancelable: true });
        target.dispatchEvent(event);
        return event;
    };
    const type = (input: HTMLInputElement, text: string) => {
        input.value += text;
        input.dispatchEvent(new Event("input", { bubbles: true }));
    };

    it("untick the last item that was ticked with Backspace, one by one, while it is closed", () => {
        const { select, internal } = createChecklist();
        select.data = [item("2", "Two"), item("1", "One")];

        expect(key(internal.focusser, "Backspace").defaultPrevented).toBe(true);
        expect(select.val).toEqual(["2"]);
        key(internal.focusser, "Backspace");
        expect(select.val).toEqual([]);
        expect(key(internal.focusser, "Backspace").defaultPrevented).toBe(false);

        select.destroy();
    });

    it("clear them all with Delete, only when the select allows clearing", () => {
        const { select, internal } = createChecklist({ allowClear: true });
        select.data = [item("1", "One"), item("2", "Two")];
        key(internal.focusser, "Delete");
        expect(select.val).toEqual([]);
        select.destroy();

        const noClear = createChecklist();
        noClear.select.data = [item("1", "One"), item("2", "Two")];
        expect(key(noClear.internal.focusser, "Delete").defaultPrevented).toBe(false);
        expect(noClear.select.val).toEqual(["1", "2"]);
        noClear.select.destroy();
    });

    it("act on the selection in the search box while nothing is typed, and edit what is typed otherwise", () => {
        const { select, internal } = createChecklist({ allowClear: true });
        select.data = [item("1", "One"), item("2", "Two"), item("3", "Three")];
        select.open();

        key(internal.search, "Backspace");
        expect(select.val).toEqual(["1", "2"]);
        expect(internal.opened()).toBe(true);
        // the check boxes follow
        const checked = () => [...(internal.results as HTMLElement).querySelectorAll(".select2-result[aria-selected=true]")]
            .map(x => x.textContent).filter(x => x !== "Select all" && x !== "Clear all");
        expect(checked()).toEqual(["One", "Two"]);

        type(internal.search, "o");
        expect(key(internal.search, "Backspace").defaultPrevented).toBe(false);
        expect(key(internal.search, "Delete").defaultPrevented).toBe(false);
        expect(select.val).toEqual(["1", "2"]);

        internal.search.value = "";
        key(internal.search, "Delete");
        expect(select.val).toEqual([]);
        expect(checked()).toEqual([]);

        select.destroy();
    });
});

describe("Up in a checklist with no highlight", () => {
    it("goes to the last result, after something is typed", () => {
        const { select, internal } = createChecklist();
        select.open();
        internal.search.value = "o";
        internal.search.dispatchEvent(new Event("input", { bubbles: true }));
        expect(internal.results.querySelector(".select2-highlighted")).toBeNull();

        internal.search.dispatchEvent(new KeyboardEvent("keydown", { key: "ArrowUp", bubbles: true, cancelable: true }));
        expect(internal.results.querySelector(".select2-highlighted")?.textContent).toBe("Three");

        select.destroy();
    });
});

describe("the text of the item that selects all the results", () => {
    const text = (internal: any) => internal.results.querySelector(".select2-select-all .select2-result-label").textContent;
    const click = (element: Element) => element.dispatchEvent(new Event("mouseup", { bubbles: true }));

    it("is \"Select all\" when nothing is typed, and \"Select all matching\" when something is, which it acts on", () => {
        const { select, internal } = createChecklist({
            query: (q: any) => q.callback({
                results: [item("1", "One"), item("2", "Two"), item("3", "Three")]
                    .filter(x => !q.term || x.text.toLowerCase().includes(q.term)),
                more: false
            })
        });
        select.open();
        expect(text(internal)).toBe("Select all");

        internal.search.value = "t";
        internal.search.dispatchEvent(new Event("input", { bubbles: true }));
        expect(text(internal)).toBe("Select all matching");

        click(internal.results.querySelector(".select2-select-all .select2-result-label"));
        expect(select.val).toEqual(["2", "3"]);
        expect(text(internal)).toBe("Clear all matching");

        select.destroy();
    });

    it("is \"Select shown\" when there are more results than the ones that are loaded, which it acts on", () => {
        const { select, internal } = createChecklist({
            query: (q: any) => q.callback({ results: [item("1", "One"), item("2", "Two")], more: true })
        });
        select.open();
        expect(text(internal)).toBe("Select shown");

        click(internal.results.querySelector(".select2-select-all .select2-result-label"));
        expect(select.val).toEqual(["1", "2"]);
        expect(text(internal)).toBe("Clear shown");

        select.destroy();
    });

    it("is given to the checklist of an editor whose source is asynchronous too, as its text tells what it selects", () => {
        const editor = new SelectEditor({ items: [["1", "One"]], multiple: true, checkList: true } as any);
        editor["hasAsyncSource"] = () => true;
        expect(editor["getComboboxOptions"]().selectAll).toBe(true);
        editor.destroy();
    });
});
