import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { Select2, type Select2Item } from "./select2";

const item = (id: string, text: string): Select2Item => ({ id, text });
const items = [item("1", "One"), item("2", "Two"), item("3", "Three")];

const key = (target: Element, key: string, init: KeyboardEventInit = {}) => {
    const event = new KeyboardEvent("keydown", { key, bubbles: true, cancelable: true, ...init });
    target.dispatchEvent(event);
    return event;
};

const type = (input: HTMLInputElement, text: string) => {
    input.value += text;
    input.dispatchEvent(new Event("input", { bubbles: true }));
};

let before: HTMLInputElement, after: HTMLInputElement;

beforeEach(() => {
    // jsdom has no layout, so the elements are made to look visible, as only those take the focus with Tab
    vi.spyOn(Element.prototype, "getClientRects").mockReturnValue([{}] as any);
    document.body.innerHTML = "";
    before = document.body.appendChild(document.createElement("input"));
    before.id = "before";
});

afterEach(() => {
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

function create(opt: any = {}) {
    const element = document.body.appendChild(document.createElement("input")) as HTMLInputElement;
    after = document.body.appendChild(document.createElement("input"));
    after.id = "after";
    const select = new Select2({
        element,
        // jsdom looks like a touch device, which would not move the focus to the search input
        shouldFocusInput: () => true,
        initSelection: (_element, callback) => callback(opt.multiple ? [] : null),
        query: query => query.callback({
            results: items.filter(x => !query.term || x.text.toLowerCase().includes(query.term.toLowerCase())),
            more: false
        }),
        ...opt
    });
    return { element, select, internal: (element as any).select2 };
}

describe("Tab from the dropdown of a single select", () => {
    it("does not select the result that happens to be highlighted, when the user did not point at it", () => {
        const { select, internal } = create();
        select.open();
        expect(internal.results.querySelector(".select2-highlighted")?.textContent).toBe("One");

        key(internal.search, "Tab");
        expect(select.val).toBe("");
        expect(internal.opened()).toBe(false);
    });

    it("keeps the value the select has, when it is opened and left", () => {
        const { select, internal } = create();
        select.data = items[1];
        select.open();
        key(internal.search, "Tab");
        expect(select.val).toBe("2");
    });

    it("selects the result that is found for what is typed", () => {
        const { select, internal } = create();
        select.open();
        type(internal.search, "thr");
        key(internal.search, "Tab");
        expect(select.val).toBe("3");
    });

    it("selects the result that is moved to with the arrow keys, but not one the mouse is over", () => {
        const { select, internal } = create();
        select.open();
        key(internal.search, "ArrowDown");
        key(internal.search, "Tab");
        expect(select.val).toBe("2");

        const other = create();
        other.select.open();
        const row = other.internal.results.querySelectorAll(".select2-result")[2];
        row.dispatchEvent(new MouseEvent("mousemove", { bubbles: true }));
        key(other.internal.search, "Tab");
        expect(other.select.val).toBe("");
    });

    it("moves the focus to the next element in one press, or to the previous one with Shift+Tab", () => {
        const { select, internal } = create();
        select.open();
        const tab = key(internal.search, "Tab");
        expect(tab.defaultPrevented).toBe(true);
        expect(document.activeElement).toBe(after);

        select.open();
        key(internal.search, "Tab", { shiftKey: true });
        expect(document.activeElement).toBe(before);
    });

    it("leaves Tab to the browser when there is no element after the control", () => {
        const { select, internal } = create();
        after.remove();
        select.open();
        const tab = key(internal.search, "Tab");
        expect(tab.defaultPrevented).toBe(false);
        expect(internal.opened()).toBe(false);
    });

    it("skips the elements that can not take the focus", () => {
        const { select, internal } = create();
        const disabled = document.createElement("input");
        disabled.disabled = true;
        const negative = document.createElement("input");
        negative.tabIndex = -1;
        after.before(disabled, negative);

        select.open();
        key(internal.search, "Tab");
        expect(document.activeElement).toBe(after);
    });
});

describe("Tab from the dropdown of a checklist", () => {
    it("ticks nothing, and moves the focus to the next element in one press", () => {
        const { select, internal } = create({ multiple: true, checkList: true, selectAll: true });
        select.open();
        key(internal.search, "ArrowDown");

        const tab = key(internal.search, "Tab");
        expect(tab.defaultPrevented).toBe(true);
        expect(select.val).toEqual([]);
        expect(internal.opened()).toBe(false);
        expect(document.activeElement).toBe(after);
    });
});

describe("Tab from the dropdown of a multiple select", () => {
    it("adds the highlighted result only when the user pointed at it, and leaves the focus to the browser", () => {
        const { select, internal } = create({ multiple: true });
        select.open();
        const tab = key(internal.search, "Tab");
        expect(select.val).toEqual([]);
        expect(internal.opened()).toBe(false);
        // the search input is in the control, so the browser moves the focus from it
        expect(tab.defaultPrevented).toBe(false);

        select.open();
        key(internal.search, "ArrowDown");
        key(internal.search, "Tab");
        expect(select.val).toEqual(["2"]);
    });
});
