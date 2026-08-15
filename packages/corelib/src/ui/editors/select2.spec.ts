import { describe, it, expect, vi } from "vitest";
import { Select2 } from "./select2";

describe("Select2 wrapper", () => {
    it("returns null instance for an uninitialized element", () => {
        const element = document.createElement("input");
        expect(Select2.getInstance(element as any)).toBeNull();
    });

    it("forwards operations to an underlying instance", () => {
        const element = document.createElement("input");
        const instance: any = {
            container: document.createElement("div"),
            dropdown: document.createElement("div"),
            data: vi.fn(() => ({ id: "1", text: "One" })),
            disable: vi.fn(),
            enable: vi.fn(),
            focus: vi.fn(),
            isFocused: vi.fn(() => true),
            opened: vi.fn(() => true),
            open: vi.fn(() => true),
            close: vi.fn(),
            positionDropdown: vi.fn(),
            readonly: vi.fn(),
            val: vi.fn(() => "1"),
            destroy: vi.fn()
        };
        (element as any).select2 = instance;
        const wrapper = Select2.getInstance(element as any)!;
        expect(wrapper.container).toBe(instance.container);
        expect(wrapper.dropdown).toBe(instance.dropdown);
        expect(wrapper.data).toEqual({ id: "1", text: "One" });
        wrapper.data = { id: "2", text: "Two" } as any;
        wrapper.disable();
        wrapper.enable(false);
        wrapper.focus();
        expect(wrapper.isFocused).toBe(true);
        expect(wrapper.opened).toBe(true);
        expect(wrapper.open()).toBe(true);
        wrapper.close();
        wrapper.positionDropdown();
        wrapper.readonly(true);
        expect(wrapper.search).toBeUndefined();
        expect(wrapper.val).toBe("1");
        wrapper.val = "2";
        wrapper.destroy();
        expect(instance.data).toHaveBeenCalled();
        expect(instance.disable).toHaveBeenCalled();
        expect(instance.enable).toHaveBeenCalledWith(false);
        expect(instance.destroy).toHaveBeenCalled();
    });

    it("exposes static formatting and matching defaults", () => {
        const defaults = Select2.defaults;
        expect(defaults.formatAjaxError!(undefined, undefined)).toBeTruthy();
        expect(defaults.formatInputTooLong!("abcd", 2)).toBeTruthy();
        expect(defaults.formatInputTooShort!("a", 3)).toBeTruthy();
        expect(defaults.formatLoadMore!(2)).toBeTruthy();
        expect(defaults.formatMatches!(1)).toBeTruthy();
        expect(defaults.formatMatches!(2)).toBeTruthy();
        expect(defaults.formatNoMatches!(undefined)).toBeTruthy();
        expect(defaults.formatSearching!()).toBeTruthy();
        expect(defaults.formatSelection!({ id: "1", text: "One" } as any, null, (x: string) => x)).toBe("One");
        expect(defaults.matcher!("caf", "Café", undefined)).toBe(true);
        expect(defaults.id!(undefined)).toBeNull();
        expect(defaults.id!({ id: "x" })).toBe("x");
        expect(Select2.stripDiacritics("café")).toBe("cafe");
        expect(Select2.ajaxDefaults.params.method).toBe("GET");
    });

    it("initializes and operates a single select", () => {
        const element = document.createElement("input");
        document.body.appendChild(element);
        const select = new Select2({
            element,
            placeholder: "Choose",
            allowClear: true,
            initSelection: (_element, callback) => callback(null),
            query: query => query.callback({ results: [{ id: "1", text: "One" }], more: false })
        });
        expect((element as any).select2).toBeTruthy();
        select.data = { id: "1", text: "One" } as any;
        select.val = "1";
        expect(typeof select.val).toBe("string");
        select.open();
        select.close();
        select.disable();
        select.enable();
        select.readonly(true);
        select.readonly(false);
        select.destroy();
    });

    it("initializes and operates a multiple select", () => {
        const element = document.createElement("input");
        document.body.appendChild(element);
        const select = new Select2({
            element,
            multiple: true,
            initSelection: (_element, callback) => callback([]),
            query: query => query.callback({ results: [
                { id: "1", text: "One" },
                { id: "2", text: "Two" }
            ], more: false })
        });
        select.data = [{ id: "1", text: "One" }, { id: "2", text: "Two" }] as any;
        select.val = ["1", "2"];
        expect(Array.isArray(select.val)).toBe(true);
        select.open();
        select.close();
        select.destroy();
    });

    it("processes single-select results and selection", () => {
        const element = document.createElement("input");
        document.body.appendChild(element);
        new Select2({
            element,
            initSelection: (_element, callback) => callback(null),
            query: query => query.callback({ results: [
                { id: "1", text: "One" },
                { id: "2", text: "Two" }
            ], more: false })
        });
        const internal: any = (element as any).select2;
        expect(() => internal.postprocessResults({ results: [{ id: "1", text: "One" }], more: false }, true)).not.toThrow();
        expect(() => internal.onSelect({ id: "1", text: "One" }, { noFocus: true })).not.toThrow();
        internal.destroy();
    });

    it("processes multiple-select results and selection", () => {
        const element = document.createElement("input");
        document.body.appendChild(element);
        new Select2({
            element,
            multiple: true,
            initSelection: (_element, callback) => callback([]),
            query: query => query.callback({ results: [
                { id: "1", text: "One" },
                { id: "2", text: "Two" }
            ], more: false })
        });
        const internal: any = (element as any).select2;
        expect(() => internal.postprocessResults({ results: [{ id: "1", text: "One" }], more: false }, true)).not.toThrow();
        expect(() => internal.onSelect({ id: "1", text: "One" }, { noFocus: true })).not.toThrow();
        internal.destroy();
    });

    it("initializes from a native select and queries options", () => {
        const select = document.createElement("select");
        const placeholder = document.createElement("option");
        placeholder.value = "";
        placeholder.textContent = "";
        const option = document.createElement("option");
        option.value = "1";
        option.textContent = "One";
        select.append(placeholder, option);
        document.body.appendChild(select);
        new Select2({ element: select, placeholderOption: "first" } as any);
        const internal: any = (select as any).select2;
        const callback = vi.fn();
        internal.opts.query({ term: "One", page: 1, matcher: () => true, callback });
        expect(callback).toHaveBeenCalled();
        expect(internal.optionToData(option).text).toBe("One");
        internal.destroy();
    });

    it("processes grouped native options and synchronizes source attributes", () => {
        const select = document.createElement("select");
        const placeholder = new Option("Choose", "");
        const group = document.createElement("optgroup");
        group.label = "Group";
        const option = new Option("One", "1");
        option.className = "important";
        option.setAttribute("locked", "locked");
        group.append(option);
        select.append(placeholder, group);
        document.body.appendChild(select);
        new Select2({ element: select, placeholder: "Choose" } as any);
        const internal: any = (select as any).select2;
        const groupData = internal.optionToData(group);
        groupData.children.push(internal.optionToData(option));
        expect(groupData.children[0].locked).toBe(true);

        const results = document.createElement("ul");
        internal.opts.populateResults.call(internal, results, [groupData], { term: "" });
        expect(results.querySelector(".select2-result-with-children")).toBeTruthy();
        select.disabled = true;
        internal.handleMonitorSync();
        expect(internal.container.classList.contains("select2-container-disabled")).toBe(true);
        select.disabled = false;
        internal.handleMonitorSync();
        internal.destroy();
    });

    it("validates native-select and option preparation rules", () => {
        const select = document.createElement("select");
        select.append(new Option("One", "1"));
        document.body.appendChild(select);
        expect(() => new Select2({ element: select, data: [] } as any)).toThrow("not allowed");
        const input = document.createElement("input");
        input.dataset.select2Tags = "[]";
        document.body.appendChild(input);
        expect(() => new Select2({ element: input, tags: [] } as any)).toThrow("tags specified");
        expect(() => new Select2({ element: document.createElement("input"), query: () => { }, createSearchChoicePosition: "middle" } as any)).toThrow("invalid createSearchChoicePosition");
    });

    it("initializes from local data and resolves selection", () => {
        const input = document.createElement("input");
        input.value = "2";
        document.body.appendChild(input);
        new Select2({ element: input, data: [
            [{ id: "1", text: "One" }],
            [{ id: "2", text: "Two" }]
        ] } as any);
        const internal: any = (input as any).select2;
        const callback = vi.fn();
        internal.opts.initSelection(input, callback);
        expect(callback).toHaveBeenCalledWith({ id: "2", text: "Two" });
        const results = vi.fn();
        internal.opts.query({ term: "Tw", matcher: () => true, callback: results });
        expect(results).toHaveBeenCalled();
        internal.destroy();
    });

    it("initializes tags and tokenizes values", () => {
        const input = document.createElement("input");
        document.body.appendChild(input);
        new Select2({ element: input, tags: ["one", { id: "two", text: "Two" }], tokenSeparators: [", "] } as any);
        const internal: any = (input as any).select2;
        const callback = vi.fn();
        const tokenizer = internal.opts.tokenizer;
        const remaining = tokenizer("one, two", [], callback, internal.opts);
        expect(callback).toHaveBeenCalled();
        expect(remaining).toBe("two");
        const choice = internal.opts.createSearchChoice("new");
        expect(choice.id).toBe("new");
        internal.destroy();
    });

    it("queries function-backed tags and skips duplicate tokens", () => {
        const input = document.createElement("input");
        document.body.appendChild(input);
        new Select2({
            element: input,
            tags: () => ["one", { id: "two", text: "Two" }],
            tokenSeparators: [", "]
        } as any);
        const internal: any = (input as any).select2;
        const callback = vi.fn();
        internal.opts.query({ term: "on", matcher: (term: string, text: string) => text.includes(term), callback });
        expect(callback).toHaveBeenCalledWith({ results: [{ id: "one", text: "one" }] });

        const selected = [{ id: "one", text: "one" }];
        const selectCallback = vi.fn();
        const remaining = internal.opts.tokenizer("one, new, ", selected, selectCallback, internal.opts);
        expect(selectCallback).toHaveBeenCalledTimes(1);
        expect(selectCallback).toHaveBeenCalledWith({ id: "new", text: "new" });
        expect(remaining).toBe("");
        internal.destroy();
    });

    it("supports custom width and CSS provider options", () => {
        const input = document.createElement("input");
        input.setAttribute("style", "width: 120px");
        document.body.appendChild(input);
        new Select2({
            element: input,
            query: query => query.callback({ results: [], more: false }),
            width: () => "50px",
            containerCssClass: () => "custom-container",
            dropdownCssClass: () => "custom-dropdown",
            providerOptions: () => ({ maximumInputLength: 10 })
        } as any);
        const internal: any = (input as any).select2;
        expect(internal.container.classList.contains("custom-container")).toBe(true);
        internal.destroy();
    });

    it("uses ajax options to create a delayed query adapter", async () => {
        vi.useFakeTimers();
        const base = await import("../../base");
        const serviceCall = vi.spyOn(base, "serviceCall").mockImplementation((options: any) => {
            options.onSuccess({ items: [{ id: "1", text: "One" }] });
            return Promise.resolve({});
        });
        const input = document.createElement("input");
        input.dataset.ajaxUrl = "/services/items";
        document.body.appendChild(input);
        new Select2({
            element: input,
            ajax: {
                url: (term: string, page: number) => "/items?term=" + term + "&page=" + page,
                quietMillis: 10,
                data: (term: string, page: number) => ({ term, page }),
                params: () => ({ headers: { "x-test": "1" } }),
                results: (response: any) => ({ results: response.items, more: false })
            }
        } as any);
        const internal: any = (input as any).select2;
        const callback = vi.fn();
        internal.opts.query({ term: "one", page: 2, context: {}, callback });
        vi.advanceTimersByTime(10);
        expect(serviceCall).toHaveBeenCalled();
        expect(callback).toHaveBeenCalledWith({ results: [{ id: "1", text: "One" }], more: false });
        serviceCall.mockRestore();
        vi.useRealTimers();
        internal.destroy();
    });

    it("handles dropdown mask mouse and touch events", () => {
        const input = document.createElement("input");
        document.body.appendChild(input);
        new Select2({
            element: input,
            selectOnBlur: true,
            initSelection: (_element, callback) => callback(null),
            query: query => query.callback({ results: [{ id: "1", text: "One" }], more: false })
        });
        const internal: any = (input as any).select2;
        const selectHighlighted = vi.spyOn(internal, "selectHighlighted").mockImplementation(() => { });
        const close = vi.spyOn(internal, "close").mockImplementation(() => { });
        internal.open();
        const mask = document.getElementById("select2-drop-mask")!;
        expect(mask).toBeTruthy();
        mask.dispatchEvent(new MouseEvent("mousedown", { bubbles: true, cancelable: true }));
        mask.dispatchEvent(new TouchEvent("touchstart", { bubbles: true, cancelable: true }));
        mask.dispatchEvent(new MouseEvent("click", { bubbles: true, cancelable: true }));
        expect(selectHighlighted).toHaveBeenCalled();
        expect(close).toHaveBeenCalled();
        internal.destroy();
    });

    it("renders minimum and maximum input length messages", () => {
        const shortInput = document.createElement("input");
        document.body.appendChild(shortInput);
        new Select2({
            element: shortInput,
            minimumInputLength: 3,
            initSelection: (_e, callback) => callback(null),
            query: query => query.callback({ results: [], more: false })
        } as any);
        const shortInternal: any = (shortInput as any).select2;
        shortInternal.open();
        expect(shortInternal.results.textContent).toContain("Controls.SelectEditor.InputTooShort");
        shortInternal.destroy();

        const longInput = document.createElement("input");
        document.body.appendChild(longInput);
        new Select2({
            element: longInput,
            maximumInputLength: 2,
            initSelection: (_e, callback) => callback(null),
            query: query => query.callback({ results: [], more: false })
        } as any);
        const longInternal: any = (longInput as any).select2;
        longInternal.open();
        longInternal.search.value = "long";
        longInternal.updateResults(false);
        expect(longInternal.results.textContent).toContain("Controls.SelectEditor.InputTooLong");
        longInternal.destroy();
    });

    it("creates tag choices and renders selection limits", () => {
        const input = document.createElement("input");
        document.body.appendChild(input);
        new Select2({
            element: input,
            tags: [],
            tokenSeparators: [", "],
            maximumSelectionSize: 1,
            createSearchChoice: (term: string) => ({ id: term, text: term }),
            formatSelectionTooBig: (size: number) => "Limit " + size,
            query: query => query.callback({ results: [{ id: "1", text: "One" }], more: false })
        } as any);
        const internal: any = (input as any).select2;
        internal.open();
        internal.search.value = "new";
        internal.updateResults(false);
        expect(internal.results.textContent).toContain("new");
        internal.addSelectedChoice({ id: "1", text: "One" });
        internal.updateResults(true);
        expect(internal.results.textContent).toContain("Limit 1");
        internal.close();
        internal.blur();
        internal.destroy();
    });

    it("renders ajax errors and no-match results", () => {
        const errorInput = document.createElement("input");
        document.body.appendChild(errorInput);
        new Select2({
            element: errorInput,
            initSelection: (_e, callback) => callback(null),
            query: query => query.callback({ hasError: true, errorInfo: "bad", results: [] }),
            formatAjaxError: () => "Ajax failed"
        } as any);
        const errorInternal: any = (errorInput as any).select2;
        errorInternal.open();
        errorInternal.search.value = "x";
        errorInternal.updateResults(false);
        expect(errorInternal.results.textContent).toContain("Ajax failed");
        errorInternal.destroy();

        const emptyInput = document.createElement("input");
        document.body.appendChild(emptyInput);
        new Select2({
            element: emptyInput,
            initSelection: (_e, callback) => callback(null),
            query: query => query.callback({ results: [], more: false })
        } as any);
        const emptyInternal: any = (emptyInput as any).select2;
        emptyInternal.open();
        emptyInternal.search.value = "x";
        emptyInternal.updateResults(false);
        expect(emptyInternal.results.textContent).toContain("No");
        emptyInternal.destroy();
    });

    it("validates formatter values used by updateResults", () => {
        const create = (formatNoMatches: any, open = true) => {
            const input = document.createElement("input");
            document.body.appendChild(input);
            new Select2({
                element: input,
                initSelection: (_e, callback) => callback(null),
                formatNoMatches,
                query: query => query.callback({ results: [], more: false })
            } as any);
            const internal: any = (input as any).select2;
            if (open)
                internal.open();
            internal.search.value = "x";
            return { input, internal };
        };

        const functionFormatter = create(() => "Function message");
        functionFormatter.internal.updateResults(false);
        expect(functionFormatter.internal.results.textContent).toContain("Function message");
        functionFormatter.internal.destroy();

        const stringFormatter = create("String message");
        stringFormatter.internal.updateResults(false);
        expect(stringFormatter.internal.results.textContent).toContain("String message");
        stringFormatter.internal.destroy();

        const falsyFormatter = create(null);
        expect(() => falsyFormatter.internal.updateResults(false)).not.toThrow();
        falsyFormatter.internal.destroy();

        expect(() => create(123)).toThrow("formatNoMatches must be a string, function, or falsy value");
    });

    it("renders load-more results", () => {
        vi.useFakeTimers();
        const input = document.createElement("input");
        document.body.appendChild(input);
        new Select2({
            element: input,
            initSelection: (_e, callback) => callback(null),
            query: query => query.callback({ results: [{ id: "1", text: "One" }], more: true }),
            formatLoadMore: () => "More"
        } as any);
        const internal: any = (input as any).select2;
        internal.open();
        internal.search.value = "o";
        internal.updateResults(false);
        expect(internal.results.querySelector(".select2-more-results")).toBeTruthy();
        vi.clearAllTimers();
        internal.destroy();
        vi.useRealTimers();
    });

    it("covers single-select result navigation and data APIs", () => {
        const input = document.createElement("input");
        document.body.appendChild(input);
        new Select2({
            element: input,
            initSelection: (_e, callback) => callback(null),
            query: query => query.callback({ results: [
                { id: "1", text: "One" },
                { id: "2", text: "Two", disabled: true }
            ], more: false })
        } as any);
        const internal: any = (input as any).select2;
        internal.open();
        internal.updateResults(true);
        expect(internal.findHighlightableChoices().length).toBeGreaterThanOrEqual(1);
        internal.highlight(0);
        internal.moveHighlight(1);
        internal.highlightUnderEvent({ target: internal.results.querySelector("li") } as any);
        internal.handleSearchFocus();
        internal.handleSearchBlur();
        internal.externalSearch("one");
        internal.selectHighlighted({ noFocus: true });
        internal.data({ id: "1", text: "One" }, true);
        internal.val(null, true);
        internal.cancel();
        internal.destroy();
    });

    it("handles single-select selection and container events", () => {
        const input = document.createElement("input");
        document.body.appendChild(input);
        new Select2({
            element: input,
            allowClear: true,
            placeholder: "Choose",
            initSelection: (_e, callback) => callback(null),
            query: query => query.callback({ results: [{ id: "1", text: "One" }], more: false })
        } as any);
        const internal: any = (input as any).select2;
        internal.onSelect({ id: "1", text: "One" }, { noFocus: true });
        const selection = internal.selection as HTMLElement;
        selection.dispatchEvent(new FocusEvent("focus", { bubbles: true }));
        selection.dispatchEvent(new MouseEvent("click", { bubbles: true, cancelable: true }));
        selection.dispatchEvent(new MouseEvent("click", { bubbles: true, cancelable: true }));
        selection.dispatchEvent(new Event("dragstart", { bubbles: true, cancelable: true }));
        const clear = selection.querySelector("abbr");
        clear?.dispatchEvent(new MouseEvent("click", { bubbles: true, cancelable: true }));

        const focusser = internal.focusser as HTMLInputElement;
        focusser.dispatchEvent(new FocusEvent("focus", { bubbles: true }));
        for (const key of ["Tab", "ArrowUp", "ArrowDown", "Enter", "Escape", "Delete", "Backspace"]) {
            focusser.dispatchEvent(new KeyboardEvent("keydown", { bubbles: true, cancelable: true, key }));
        }
        focusser.dispatchEvent(new FocusEvent("blur", { bubbles: true }));

        internal.open();
        internal.search.dispatchEvent(new KeyboardEvent("keydown", { bubbles: true, cancelable: true, isComposing: true }));
        for (const key of ["PageUp", "PageDown", "ArrowUp", "ArrowDown", "Enter", "Tab", "Escape"]) {
            internal.search.dispatchEvent(new KeyboardEvent("keydown", { bubbles: true, cancelable: true, key }));
        }
        internal.search.dispatchEvent(new FocusEvent("focus", { bubbles: true }));
        internal.search.dispatchEvent(new FocusEvent("blur", { bubbles: true }));
        internal.dropdown.dispatchEvent(new MouseEvent("mousedown", { bubbles: true }));
        internal.dropdown.dispatchEvent(new TouchEvent("touchstart", { bubbles: true }));
        internal.close();
        internal.destroy();
    });

    it("covers multi-select choices, values, and sorting", () => {
        const input = document.createElement("input");
        document.body.appendChild(input);
        new Select2({
            element: input,
            multiple: true,
            initSelection: (_e, callback) => callback([]),
            query: query => query.callback({ results: [
                { id: "1", text: "One" },
                { id: "2", text: "Two", locked: true }
            ], more: false })
        } as any);
        const internal: any = (input as any).select2;
        internal.addSelectedChoice({ id: "1", text: "One" });
        internal.addSelectedChoice({ id: "2", text: "Two", locked: true });
        expect(internal.data()).toHaveLength(2);
        internal.postprocessResults({ results: [], more: false }, false);
        internal.selectChoice(internal.selection.querySelector(".select2-search-choice"));
        internal.unselect(internal.selection.querySelector(".select2-search-choice"));
        internal.val(["2"], true);
        internal.data([{ id: "1", text: "One" }], true);
        internal.onSortStart();
        internal.onSortEnd();
        internal.handleSearchFocus();
        internal.handleSearchBlur();
        internal.cancel();
        internal.destroy();
    });

    it("handles multi-select choice and search interactions", () => {
        const input = document.createElement("input");
        document.body.appendChild(input);
        new Select2({
            element: input,
            multiple: true,
            initSelection: (_e, callback) => callback([]),
            query: query => query.callback({ results: [{ id: "1", text: "One" }, { id: "2", text: "Two" }], more: false })
        } as any);
        const internal: any = (input as any).select2;
        internal.addSelectedChoice({ id: "1", text: "One" });
        internal.addSelectedChoice({ id: "2", text: "Two" });
        const first = internal.selection.querySelector(".select2-search-choice") as HTMLElement;
        first.dispatchEvent(new MouseEvent("click", { bubbles: true, cancelable: true }));
        first.dispatchEvent(new MouseEvent("click", { bubbles: true, cancelable: true }));

        const search = internal.search as HTMLInputElement;
        search.value = "x";
        search.dispatchEvent(new Event("input", { bubbles: true }));
        search.dispatchEvent(new Event("paste", { bubbles: true }));
        internal.open();
        for (const key of ["ArrowLeft", "ArrowRight", "Backspace", "Delete", "Enter", "ArrowUp", "ArrowDown", "Tab", "Escape", "PageUp", "PageDown"]) {
            search.dispatchEvent(new KeyboardEvent("keydown", { bubbles: true, cancelable: true, key }));
        }
        search.dispatchEvent(new KeyboardEvent("keyup", { bubbles: true }));
        search.dispatchEvent(new FocusEvent("blur", { bubbles: true }));
        internal.container.dispatchEvent(new FocusEvent("focus", { bubbles: true }));
        internal.container.dispatchEvent(new MouseEvent("click", { bubbles: true, cancelable: true }));
        internal.destroy();
    });

    it("covers Select2 keyboard handlers", () => {
        const input = document.createElement("input");
        document.body.appendChild(input);
        new Select2({
            element: input,
            initSelection: (_e, callback) => callback(null),
            query: query => query.callback({ results: [{ id: "1", text: "One" }], more: false })
        } as any);
        const internal: any = (input as any).select2;
        internal.open();
        const search = internal.search;
        for (const key of ["ArrowUp", "ArrowDown", "Enter", "Tab", "Escape", "PageUp", "PageDown"]) {
            search.dispatchEvent(new KeyboardEvent("keydown", { bubbles: true, cancelable: true, key }));
        }
        const focusser = internal.focusser;
        for (const key of ["Tab", "ArrowDown", "Enter", "Delete", "Backspace", "Escape"]) {
            focusser.dispatchEvent(new KeyboardEvent("keydown", { bubbles: true, cancelable: true, key }));
        }
        internal.destroy();
    });

    it("emits keyup-change only when the value changes", () => {
        const input = document.createElement("input");
        document.body.appendChild(input);
        new Select2({
            element: input,
            initSelection: (_e, callback) => callback(null),
            query: query => query.callback({ results: [{ id: "1", text: "One" }], more: false })
        } as any);
        const internal: any = (input as any).select2;
        internal.open();

        const assertKeyupChange = (element: HTMLInputElement) => {
            const changed = vi.fn();
            element.addEventListener("keyup-change", changed);

            element.value = "without-keydown";
            element.dispatchEvent(new KeyboardEvent("keyup", { bubbles: true }));
            expect(changed).not.toHaveBeenCalled();

            element.value = "same";
            element.dispatchEvent(new KeyboardEvent("keydown", { bubbles: true }));
            element.dispatchEvent(new KeyboardEvent("keydown", { bubbles: true }));
            element.dispatchEvent(new KeyboardEvent("keyup", { bubbles: true }));
            expect(changed).not.toHaveBeenCalled();

            element.dispatchEvent(new KeyboardEvent("keydown", { bubbles: true }));
            element.value = "changed";
            element.dispatchEvent(new KeyboardEvent("keyup", { bubbles: true }));
            expect(changed).toHaveBeenCalledTimes(1);
        };

        assertKeyupChange(internal.search);
    internal.close();
        assertKeyupChange(internal.focusser);
    expect(internal.opened()).toBe(true);
        internal.destroy();
    });

    it("moves a long dropdown above the control when below space is insufficient", () => {
        const input = document.createElement("input");
        document.body.appendChild(input);
        new Select2({
            element: input,
            initSelection: (_e, callback) => callback(null),
            query: query => query.callback({ results: [], more: false })
        } as any);
        const internal: any = (input as any).select2;
        const originalHeight = window.innerHeight;
        Object.defineProperty(window, "innerHeight", { configurable: true, value: 300 });
        Object.defineProperty(internal.container, "offsetTop", { configurable: true, value: 250 });
        Object.defineProperty(internal.container, "offsetHeight", { configurable: true, value: 30 });
        Object.defineProperty(internal.container, "offsetWidth", { configurable: true, value: 120 });
        Object.defineProperty(internal.dropdown, "offsetHeight", { configurable: true, value: 100 });
        Object.defineProperty(internal.dropdown, "offsetWidth", { configurable: true, value: 120 });
        internal.container.getBoundingClientRect = () => ({ top: 250, left: 10, width: 120, height: 30 } as DOMRect);

        internal.positionDropdown();

        expect(internal.container.classList.contains("select2-drop-above")).toBe(true);
        expect(internal.dropdown.classList.contains("select2-drop-above")).toBe(true);
        expect(internal.dropdown.style.top).toBe("150px");
        Object.defineProperty(window, "innerHeight", { configurable: true, value: originalHeight });
        internal.destroy();
    });

    it("covers highlight scrolling and navigation branches", () => {
        const input = document.createElement("input");
        document.body.appendChild(input);
        new Select2({
            element: input,
            initSelection: (_e, callback) => callback(null),
            query: query => query.callback({ results: [
                { id: "1", text: "One" },
                { id: "2", text: "Two" },
                { id: "3", text: "Three", disabled: true }
            ], more: false })
        } as any);
        const internal: any = (input as any).select2;
        internal.open();
        internal.updateResults(true);
        internal.highlight(10);
        internal.highlight(-5);
        internal.highlight(0);
        internal.moveHighlight(1);
        internal.moveHighlight(-1);
        internal.highlightUnderEvent({ target: internal.results.querySelector(".select2-result-unselectable") } as any);
        internal.highlightUnderEvent({ target: internal.results.querySelector(".select2-result-selectable") } as any);
        internal.removeHighlight();
        internal.touchMoved();
        expect(internal._touchMoved).toBe(true);
        internal.clearTouchMoved();
        expect(internal.countSelectableResults()).toBeGreaterThanOrEqual(2);

        const child = internal.results.querySelectorAll(".select2-result-label")[1] as HTMLElement;
        const rect = (top: number, height: number) => ({ top, left: 0, right: 10, bottom: top + height, width: 10, height, x: 0, y: top, toJSON() { } } as DOMRect);
        child.getBoundingClientRect = () => rect(100, 20);
        internal.results.getBoundingClientRect = () => rect(0, 100);
        internal.highlight(1);
        child.getBoundingClientRect = () => rect(0, 20);
        internal.results.getBoundingClientRect = () => rect(100, 100);
        internal.highlight(1);
        internal.destroy();
    });

    it("renders searching state until a deferred query completes", async () => {
        const input = document.createElement("input");
        document.body.appendChild(input);
        let resolveQuery: (result: any) => void = () => { };
        new Select2({
            element: input,
            formatSearching: () => "Searching...",
            initSelection: (_e, callback) => callback(null),
            query: query => { resolveQuery = result => query.callback(result); }
        } as any);
        const internal: any = (input as any).select2;
        internal.open();
        internal.search.value = "x";
        internal.updateResults(false);
        expect(internal.results.textContent).toContain("Searching...");
        resolveQuery({ results: [{ id: "1", text: "One" }], more: false });
        await Promise.resolve();
        expect(internal.results.textContent).toContain("One");
        internal.destroy();
    });

    it("skips creating a duplicate tag choice", () => {
        const input = document.createElement("input");
        document.body.appendChild(input);
        new Select2({
            element: input,
            createSearchChoice: (term: string) => ({ id: term, text: term }),
            initSelection: (_e, callback) => callback(null),
            query: query => query.callback({ results: [{ id: "x", text: "x" }], more: false })
        } as any);
        const internal: any = (input as any).select2;
        internal.open();
        internal.search.value = "x";
        internal.updateResults(false);
        expect(internal.results.querySelectorAll(".select2-result").length).toBe(2);
        internal.destroy();
    });

    it("loads more results on demand", () => {
        vi.useFakeTimers();
        const input = document.createElement("input");
        document.body.appendChild(input);
        let calls = 0;
        new Select2({
            element: input,
            initSelection: (_e, callback) => callback(null),
            query: query => {
                calls++;
                query.callback({ results: [{ id: "1", text: "One" + calls }], more: calls === 1 });
            }
        } as any);
        const internal: any = (input as any).select2;
        internal.open();
        vi.advanceTimersByTime(30);
        expect(calls).toBe(2);
        vi.useRealTimers();
        internal.destroy();
    });

    it("positions an auto-width dropdown and flips its direction", () => {
        const input = document.createElement("input");
        document.body.appendChild(input);
        new Select2({
            element: input,
            dropdownAutoWidth: true,
            initSelection: (_e, callback) => callback(null),
            query: query => query.callback({ results: [], more: false })
        } as any);
        const internal: any = (input as any).select2;
        const originalHeight = window.innerHeight;
        Object.defineProperty(window, "innerHeight", { configurable: true, value: 1000 });
        Object.defineProperty(internal.container, "offsetHeight", { configurable: true, value: 30 });
        Object.defineProperty(internal.container, "offsetWidth", { configurable: true, value: 100 });
        Object.defineProperty(internal.dropdown, "offsetHeight", { configurable: true, value: 100 });
        Object.defineProperty(internal.dropdown, "offsetWidth", { configurable: true, value: 100 });
        Object.defineProperty(internal.results, "scrollHeight", { configurable: true, value: 200 });
        Object.defineProperty(internal.results, "clientHeight", { configurable: true, value: 100 });
        internal.container.getBoundingClientRect = () => ({ top: 0, left: 0, width: 100, height: 30 } as DOMRect);
        internal.positionDropdown();
        expect(internal.dropdown.classList.contains("select2-drop-auto-width")).toBe(true);

        internal.dropdown.classList.add("select2-drop-above");
        internal.container.classList.add("select2-drop-above");
        Object.defineProperty(window, "innerHeight", { configurable: true, value: 300 });
        internal.container.getBoundingClientRect = () => ({ top: 0, left: 0, width: 100, height: 30 } as DOMRect);
        internal.positionDropdown();
        expect(internal.dropdown.classList.contains("select2-drop-above")).toBe(false);
        Object.defineProperty(window, "innerHeight", { configurable: true, value: originalHeight });
        internal.destroy();
    });

    it("resolves the container width from several sources", () => {
        const make = (attrs: Record<string, string>, width: any) => {
            const input = document.createElement("input");
            for (const k in attrs) input.setAttribute(k, attrs[k]);
            document.body.appendChild(input);
            new Select2({
                element: input,
                width,
                initSelection: (_e, callback) => callback(null),
                query: query => query.callback({ results: [], more: false })
            } as any);
            const internal: any = (input as any).select2;
            const result = internal.container.style.width;
            internal.destroy();
            input.remove();
            return result;
        };
        expect(make({ style: "width: 120px" }, "copy")).toBe("120px");
        expect(make({}, "off")).toBe("");
        expect(make({}, "element")).toBe("auto");
        expect(make({}, "resolve")).toBe("auto");
        expect(make({}, () => "88px")).toBe("88px");
        expect(make({ style: "padding: 2px" }, "copy")).toBe("");
    });

    it("reads and writes values on a native single select and clears it", () => {
        const select = document.createElement("select");
        select.append(new Option("One", "1"), new Option("Two", "2"));
        document.body.appendChild(select);
        new Select2({ element: select } as any);
        const internal: any = (select as any).select2;
        internal.val("2");
        expect(select.value).toBe("2");
        internal.val("1", true);
        internal.clear();
        internal.destroy();
    });

    it("reorders multi-select local values and clears via val", () => {
        const input = document.createElement("input");
        input.value = "2,1";
        document.body.appendChild(input);
        new Select2({
            element: input,
            multiple: true,
            data: [
                [{ id: "1", text: "One" }],
                [{ id: "2", text: "Two" }]
            ]
        } as any);
        const internal: any = (input as any).select2;
        const cb = vi.fn();
        internal.opts.initSelection(input, cb);
        expect(cb.mock.calls[0][0].map((x: any) => x.id)).toEqual(["2", "1"]);
        internal.val(["1", "2"]);
        internal.val(null, true);
        internal.data([{ id: "x", text: "X" }, { id: "1", text: "One" }], true);
        internal.destroy();
    });

    it("detects touch devices when deciding whether to focus", () => {
        (window as any).ontouchstart = null;
        const input = document.createElement("input");
        document.body.appendChild(input);
        new Select2({
            element: input,
            minimumResultsForSearch: -1,
            initSelection: (_e, callback) => callback(null),
            query: query => query.callback({ results: [], more: false })
        } as any);
        const internal: any = (input as any).select2;
        expect(internal.opts.shouldFocusInput(internal)).toBe(false);
        internal.destroy();
        delete (window as any).ontouchstart;
    });

    it("adapts css classes from the source element", () => {
        const input = document.createElement("input");
        input.className = "custom-source";
        document.body.appendChild(input);
        new Select2({
            element: input,
            adaptContainerCssClass: (c: string) => "adapted-" + c,
            adaptDropdownCssClass: (c: string) => "drop-" + c,
            initSelection: (_e, callback) => callback(null),
            query: query => query.callback({ results: [], more: false })
        } as any);
        const internal: any = (input as any).select2;
        expect(internal.container.classList.contains("adapted-custom-source")).toBe(true);
        internal.destroy();
    });

    it("loads tags from a data attribute and restores tabindex on destroy", () => {
        const input = document.createElement("input");
        input.dataset.select2Tags = JSON.stringify([{ id: "a", text: "A" }]);
        input.setAttribute("tabindex", "7");
        input.autofocus = true;
        document.body.appendChild(input);
        new Select2({ element: input } as any);
        const internal: any = (input as any).select2;
        expect(internal.opts.tags.length).toBe(1);
        internal.destroy();
        expect(input.getAttribute("tabindex")).toBe("7");
        input.remove();

        const plain = document.createElement("input");
        document.body.appendChild(plain);
        new Select2({
            element: plain,
            initSelection: (_e, callback) => callback(null),
            query: query => query.callback({ results: [], more: false })
        } as any);
        const plainInternal: any = (plain as any).select2;
        plainInternal.destroy();
        expect(plain.hasAttribute("tabindex")).toBe(false);
        plain.remove();
    });

    it("filters stationary mouse movement events", () => {
        const input = document.createElement("input");
        document.body.appendChild(input);
        new Select2({
            element: input,
            initSelection: (_e, callback) => callback(null),
            query: query => query.callback({ results: [{ id: "1", text: "One" }], more: false })
        } as any);
        const internal: any = (input as any).select2;
        internal.open();
        const filtered = vi.fn();
        internal.results.addEventListener("mousemove-filtered", filtered);
        const move = (pageX: number, pageY: number) => {
            const event = new MouseEvent("mousemove", { bubbles: true });
            Object.defineProperty(event, "pageX", { value: pageX });
            Object.defineProperty(event, "pageY", { value: pageY });
            internal.results.dispatchEvent(event);
        };
        move(55, 66);
        expect(filtered).toHaveBeenCalledTimes(1);
        move(55, 66);
        expect(filtered).toHaveBeenCalledTimes(1);
        move(77, 66);
        expect(filtered).toHaveBeenCalledTimes(2);
        internal.destroy();
    });

    it("queries local data with text keys and children groups", () => {
        const input = document.createElement("input");
        document.body.appendChild(input);
        new Select2({
            element: input,
            data: {
                text: "label",
                results: [
                    [{ id: "g", label: "Group", children: [[{ id: "1", label: "One" }]] }]
                ]
            }
        } as any);
        const internal: any = (input as any).select2;
        const cb = vi.fn();
        internal.opts.query({ term: "Group", matcher: (t: string, text: string) => text.includes(t), callback: cb });
        expect(cb.mock.calls[0][0].results[0].id).toBe("g");
        internal.destroy();
    });

    it("resolves function-backed tags from the current value", () => {
        const input = document.createElement("input");
        input.value = "x";
        document.body.appendChild(input);
        new Select2({ element: input, tags: () => [{ id: "x", text: "X" }, "other"] } as any);
        const internal: any = (input as any).select2;
        const cb = vi.fn();
        internal.opts.initSelection(input, cb);
        expect(cb.mock.calls[0][0]).toEqual([{ id: "x", text: "X" }]);
        internal.destroy();
    });

    it("handles dropdown mouseup and touch-click selection", () => {
        const input = document.createElement("input");
        document.body.appendChild(input);
        new Select2({
            element: input,
            initSelection: (_e, callback) => callback(null),
            query: query => query.callback({ results: [{ id: "1", text: "One" }, { id: "2", text: "Two" }], more: false })
        } as any);
        const internal: any = (input as any).select2;
        internal.open();
        const selectable = internal.results.querySelector(".select2-result-selectable") as HTMLElement;
        selectable.dispatchEvent(new MouseEvent("mouseup", { bubbles: true }));
        internal._touchEvent = true;
        internal.dropdown.dispatchEvent(new MouseEvent("click", { bubbles: true }));
        expect(internal._touchEvent).toBe(false);
        internal.destroy();
    });

    it("handles multi-select keyboard removal and escape", () => {
        const input = document.createElement("input");
        document.body.appendChild(input);
        new Select2({
            element: input,
            multiple: true,
            initSelection: (_e, callback) => callback([]),
            query: query => query.callback({ results: [{ id: "1", text: "One" }], more: false })
        } as any);
        const internal: any = (input as any).select2;
        internal.addSelectedChoice({ id: "1", text: "One" });
        internal.addSelectedChoice({ id: "2", text: "Two" });
        const keydown = (key: string, extra: any = {}) => {
            internal.search.dispatchEvent(new KeyboardEvent("keydown", { bubbles: true, cancelable: true, key, ...extra }));
        };
        keydown("Backspace");
        keydown("ArrowLeft");
        internal.open();
        keydown("Escape");
        internal.destroy();
    });

    it("respects openOnEnter and modifier keys in multi-select", () => {
        const make = (openOnEnter: boolean) => {
            const input = document.createElement("input");
            document.body.appendChild(input);
            new Select2({
                element: input,
                multiple: true,
                openOnEnter,
                initSelection: (_e, callback) => callback([]),
                query: query => query.callback({ results: [], more: false })
            } as any);
            const internal: any = (input as any).select2;
            internal.search.dispatchEvent(new KeyboardEvent("keydown", { bubbles: true, cancelable: true, key: "Enter" }));
            return internal;
        };
        make(false).destroy();
        const alt = make(true);
        alt.search.dispatchEvent(new KeyboardEvent("keydown", { bubbles: true, cancelable: true, key: "Enter", altKey: true }));
        alt.destroy();
    });

    it("opens multi-select from container clicks and tokenized input", () => {
        const input = document.createElement("input");
        document.body.appendChild(input);
        new Select2({
            element: input,
            multiple: true,
            tags: ["a", "b"],
            tokenSeparators: [", "],
            initSelection: (_e, callback) => callback([])
        } as any);
        const internal: any = (input as any).select2;
        internal.open();
        internal.search.value = "a, b";
        internal.updateResults(false);
        expect(internal.search.value).toBe("b");
        internal.close();
        internal.selection.dispatchEvent(new MouseEvent("click", { bubbles: true, cancelable: true }));
        expect(internal.opened()).toBe(true);
        internal.destroy();
    });

    it("keeps multi-select open after selection with next search term", () => {
        const input = document.createElement("input");
        document.body.appendChild(input);
        new Select2({
            element: input,
            multiple: true,
            closeOnSelect: false,
            nextSearchTerm: () => "next",
            initSelection: (_e, callback) => callback([]),
            query: query => query.callback({ results: [{ id: "1", text: "One" }, { id: "2", text: "Two" }], more: false })
        } as any);
        const internal: any = (input as any).select2;
        internal.open();
        internal.onSelect({ id: "1", text: "One" }, {});
        expect(internal.search.value).toBe("next");
        internal.destroy();
    });

    it("unselects multi choices from their close buttons", () => {
        const input = document.createElement("input");
        document.body.appendChild(input);
        new Select2({
            element: input,
            multiple: true,
            initSelection: (_e, callback) => callback([]),
            query: query => query.callback({ results: [], more: false })
        } as any);
        const internal: any = (input as any).select2;
        internal.addSelectedChoice({ id: "1", text: "One" });
        const close = internal.selection.querySelector(".select2-search-choice-close") as HTMLElement;
        close.dispatchEvent(new FocusEvent("focus", { bubbles: true }));
        close.dispatchEvent(new MouseEvent("click", { bubbles: true, cancelable: true }));
        expect(internal.data()).toHaveLength(0);
        internal.destroy();
    });

    it("sets values on a native multi select", () => {
        const select = document.createElement("select");
        select.multiple = true;
        select.append(new Option("One", "1"), new Option("Two", "2"));
        document.body.appendChild(select);
        new Select2({ element: select } as any);
        const internal: any = (select as any).select2;
        internal.val(["1"], true);
        expect((select.querySelector('option[value="1"]') as HTMLOptionElement).selected).toBe(true);
        expect((select.querySelector('option[value="2"]') as HTMLOptionElement).selected).toBe(false);
        internal.destroy();
    });
});
