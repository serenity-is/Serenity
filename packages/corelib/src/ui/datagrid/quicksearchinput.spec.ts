import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { QuickSearchInput } from "./quicksearchinput";

function createMountedInput(options?: any) {
    const container = document.createElement("div");
    document.body.appendChild(container);
    const inputEl = document.createElement("input");
    container.appendChild(inputEl);
    const input = new QuickSearchInput({ element: inputEl, ...options } as any);
    return { input, container };
}

describe("QuickSearchInput", () => {
    beforeEach(() => { document.body.innerHTML = ""; });
    afterEach(() => { document.body.innerHTML = ""; vi.restoreAllMocks(); });

    it("creates input and sets title and placeholder", () => {
        const { input } = createMountedInput();
        expect(input.domNode).toBeInstanceOf(HTMLInputElement);
        expect(input.domNode.title).toBeTruthy();
        expect(input.domNode.placeholder).toBeTruthy();
        input.destroy();
    });

    it("get_value trims the value", () => {
        const { input } = createMountedInput();
        input.domNode.value = "  abc  ";
        expect(input.get_value()).toBe("abc");
        input.destroy();
    });

    it("get_field returns undefined by default", () => {
        const { input } = createMountedInput();
        expect(input.get_field()).toBeUndefined();
        input.destroy();
    });

    it("set_field updates field and triggers checkIfValueChanged", () => {
        const { input } = createMountedInput({ fields: [{ name: "A", title: "Title A" }] });
        const checkSpy = vi.spyOn(input as any, "checkIfValueChanged").mockImplementation(() => { });
        const field = { name: "B", title: "Title B" };
        input.set_field(field);
        expect(input.get_field()).toBe(field);
        expect(checkSpy).toHaveBeenCalled();
        input.destroy();
    });

    it("restoreState sets value, lastValue and field", () => {
        const { input } = createMountedInput({ fields: [{ name: "A", title: "Title A" }] });
        const field = { name: "A", title: "Title A" };
        input.restoreState("  hi  ", field);
        expect(input.domNode.value).toBe("hi");
        expect(input.get_field()).toBe(field);
        input.destroy();
    });

    it("checkIfValueChanged ignores when ignore-change class present", () => {
        const search = vi.fn();
        const { input } = createMountedInput({ search });
        input.domNode.classList.add("ignore-change");
        input.domNode.value = "abc";
        input["checkIfValueChanged"]();
        expect(search).not.toHaveBeenCalled();
        input.destroy();
    });

    it("checkIfValueChanged triggers search after delay", () => {
        vi.useFakeTimers();
        const search = vi.fn();
        const { input } = createMountedInput({ search });
        input.domNode.value = "abc";
        input["checkIfValueChanged"]();
        vi.advanceTimersByTime(500);
        expect(search).toHaveBeenCalled();
        vi.useRealTimers();
        input.destroy();
    });

    it("searchNow calls beforeSearch then search option", () => {
        const beforeSearch = vi.fn();
        const search = vi.fn();
        const { input } = createMountedInput({ beforeSearch, search });
        input.domNode.value = "abc";
        input["searchNow"]("abc");
        expect(beforeSearch).toHaveBeenCalled();
        expect(search).toHaveBeenCalled();
        input.destroy();
    });

    it("searchNow uses deprecated onSearch option", () => {
        const onSearch = vi.fn();
        const { input } = createMountedInput({ onSearch } as any);
        input["searchNow"]("abc");
        expect(onSearch).toHaveBeenCalledWith(undefined, "abc", expect.any(Function));
        input.destroy();
    });

    it("searchNow calls done(true) when no handler", () => {
        const { input } = createMountedInput();
        expect(() => input["searchNow"]("abc")).not.toThrow();
        input.destroy();
    });

    it("execute-search event triggers searchNow", () => {
        const search = vi.fn();
        const { input } = createMountedInput({ search });
        input.domNode.value = "xyz";
        input.domNode.dispatchEvent(new Event("execute-search"));
        expect(search).toHaveBeenCalled();
        input.destroy();
    });

    it("adds shake effect when search reports no results", () => {
        const container = document.createElement("div");
        container.className = "s-QuickSearchBar";
        document.body.appendChild(container);
        const inputEl = document.createElement("input");
        container.appendChild(inputEl);

        const search = vi.fn((args: any) => args.done(false));
        const input = new QuickSearchInput({ element: inputEl, search } as any);
        const icon = container.querySelector<HTMLElement>(".quick-search-icon i")!;
        input["searchNow"]("abc");
        expect(icon.classList.contains("s-shake-effect")).toBe(true);
        input.destroy();
    });

    it("removes loading classes when search reports results", () => {
        const container = document.createElement("div");
        container.className = "s-QuickSearchBar";
        document.body.appendChild(container);
        const inputEl = document.createElement("input");
        container.appendChild(inputEl);
        const search = vi.fn((args: any) => args.done(true));
        const input = new QuickSearchInput({ element: inputEl, search } as any);
        input["searchNow"]("abc");
        expect(input.domNode.classList.contains("s-QuickSearchLoading")).toBe(false);
        input.destroy();
    });

    it("selecting a field from dropdown updates field", () => {
        const { input } = createMountedInput({ fields: [{ name: "A", title: "Title A" }, { name: "B", title: "Title B" }] });
        const anchors = input.domNode.parentElement?.querySelectorAll<HTMLAnchorElement>(".dropdown-item") ?? [];
        expect(anchors.length).toBe(2);
        (anchors[1] as HTMLAnchorElement).dispatchEvent(new Event("click"));
        expect(input.get_field()).toEqual({ name: "B", title: "Title B" });
        expect(input["fieldLink"].textContent).toBe("Title B");
        input.destroy();
    });
});
