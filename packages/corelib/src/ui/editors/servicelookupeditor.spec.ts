import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import * as base from "../../base";
import { ServiceLookupEditor } from "./servicelookupeditor";

function create(options: any = {}): any {
    return new ServiceLookupEditor({
        idField: "ID",
        textField: "Name",
        service: "Test/List",
        ...options
    } as any);
}

describe("ServiceLookupEditor", () => {
    let serviceCallSpy: any;

    beforeEach(() => {
        serviceCallSpy = vi.spyOn(base, "serviceCall").mockResolvedValue({ Entities: [] } as any);
    });

    afterEach(() => {
        serviceCallSpy.mockRestore();
    });

    it("resolves the service url from the service option", () => {
        const editor = create({ service: "Test/List" });
        expect(editor.getServiceUrl()).toBe("/Services/Test/List");
        editor.destroy();
    });

    it("throws when the service option is missing", () => {
        const editor = create({ service: null });
        expect(() => editor.getServiceUrl()).toThrow("ServiceLookupEditor requires 'service' option");
        editor.destroy();
    });

    it("returns the dialogType option as the dialog type key", () => {
        const editor = create({ dialogType: "My.Dialog" });
        expect(editor.getDialogTypeKey()).toBe("My.Dialog");
        editor.destroy();
    });

    it("derives a dialog type key from a two-part service path", () => {
        const editor = create({ dialogType: null });
        expect(editor.getDialogTypeKey()).toBe("Test.List");
        editor.destroy();
    });

    it("derives a dialog type key from a nested service path", () => {
        const editor = create({ dialogType: null, service: "Test/Sub/List" });
        expect(editor.getDialogTypeKey()).toBe("Test.Sub");
        editor.destroy();
    });

    it("derives a dialog type key from a services-rooted path", () => {
        const editor = create({ dialogType: null, service: "~/Services/Test/List" });
        expect(editor.getDialogTypeKey()).toBe("Test.List");
        editor.destroy();
    });

    it("includes the id and text fields in include columns", () => {
        const editor = create({ includeColumns: ["Col"] });
        expect(editor.getIncludeColumns()).toEqual(["Col", "ID", "Name"]);
        editor.destroy();
    });

    it("does not duplicate id and text fields in include columns", () => {
        const editor = create({ includeColumns: ["ID", "Name"] });
        expect(editor.getIncludeColumns()).toEqual(["ID", "Name"]);
        editor.destroy();
    });

    it("defaults the sort to the text field", () => {
        const editor = create({});
        expect(editor.getSort()).toEqual(["Name"]);
        editor.destroy();
    });

    it("uses an explicit sort option", () => {
        const editor = create({ sort: ["X"] });
        expect(editor.getSort()).toEqual(["X"]);
        editor.destroy();
    });

    it("returns an impossible criterion for cascade without a value", () => {
        const editor = create({ cascadeField: "Parent" });
        expect(editor.getCascadeCriteria()).toEqual(["0", "=", "1"]);
        editor.destroy();
    });

    it("builds a cascade criterion from a value", () => {
        const editor = create({ cascadeField: "Parent", cascadeValue: 5 });
        expect(editor.getCascadeCriteria()).toEqual([["Parent"], "=", 5]);
        editor.destroy();
    });

    it("returns null cascade criterion without a field or value", () => {
        const editor = create({});
        expect(editor.getCascadeCriteria()).toBeNull();
        editor.destroy();
    });

    it("builds a filter criterion from a value", () => {
        const editor = create({ filterField: "Kind", filterValue: "A" });
        expect(editor.getFilterCriteria()).toEqual([["Kind"], "=", "A"]);
        editor.destroy();
    });

    it("returns null filter criterion without a value", () => {
        const editor = create({});
        expect(editor.getFilterCriteria()).toBeNull();
        editor.destroy();
    });

    it("returns null id list criterion for null", () => {
        const editor = create({});
        expect(editor.getIdListCriteria(null)).toBeNull();
        editor.destroy();
    });

    it("returns an impossible criterion for an empty id list", () => {
        const editor = create({});
        expect(editor.getIdListCriteria([])).toEqual(["0", "=", "1"]);
        editor.destroy();
    });

    it("builds an id list criterion for values", () => {
        const editor = create({});
        const criterion = editor.getIdListCriteria(["1", "2"]);
        expect(JSON.stringify(criterion)).toContain("ID");
        expect(JSON.stringify(criterion)).toContain("1");
        expect(JSON.stringify(criterion)).toContain("2");
        editor.destroy();
    });

    it("throws when idField is missing for an id list", () => {
        const editor = create({ idField: null });
        expect(() => editor.getIdListCriteria(["1"])).toThrow("ServiceLookupEditor requires 'idField' option");
        editor.destroy();
    });

    it("combines cascade, filter, id list and option criteria", () => {
        const editor = create({
            criteria: [["X"], "=", 1],
            cascadeField: "P",
            cascadeValue: 2,
            filterField: "K",
            filterValue: "V"
        });
        const criteria = editor.getCriteria({ idList: ["5"] } as any);
        const json = JSON.stringify(criteria);
        expect(json).toContain("P");
        expect(json).toContain("K");
        expect(json).toContain("X");
        expect(json).toContain("5");
        editor.destroy();
    });

    it("builds a list request from the query", () => {
        const editor = create({
            containsField: "Contains",
            includeDeleted: true,
            equalityFilter: { A: 1 },
            excludeColumns: ["X"],
            columnSelection: 2
        });
        const request = editor.getListRequest({ searchTerm: "abc", skip: 10, take: 5, checkMore: true } as any);
        expect(request.ContainsText).toBe("abc");
        expect(request.Sort).toEqual(["Name"]);
        expect(request.ColumnSelection).toBe(2);
        expect(request.IncludeColumns).toEqual(["ID", "Name"]);
        expect(request.ExcludeColumns).toEqual(["X"]);
        expect(request.ContainsField).toBe("Contains");
        expect(request.EqualityFilter).toEqual({ A: 1 });
        expect(request.IncludeDeleted).toBe(true);
        expect(request.ExcludeTotalCount).toBe(true);
        expect(request.Skip).toBe(10);
        expect(request.Take).toBe(6);
        editor.destroy();
    });

    it("keeps take unchanged without checkMore and uses zero take by default", () => {
        const editor = create({});
        expect(editor.getListRequest({ take: 5 } as any).Take).toBe(5);
        expect(editor.getListRequest({} as any).Take).toBe(0);
        editor.destroy();
    });

    it("builds service call options with the request and signal", () => {
        const editor = create({});
        const signal = new AbortController().signal;
        const options = editor.getServiceCallOptions({ signal } as any);
        expect(options.service).toBe("/Services/Test/List");
        expect(options.signal).toBe(signal);
        expect(options.blockUI).toBe(false);
        expect(options.request).toBeDefined();
        editor.destroy();
    });

    it("reports an async source", () => {
        const editor = create({});
        expect(editor.hasAsyncSource()).toBe(true);
        editor.destroy();
    });

    it("allows search when no cascade field is set", () => {
        const editor = create({});
        expect(editor.canSearch(false)).toBe(true);
        editor.destroy();
    });

    it("blocks search until a cascade value is set", () => {
        const editor = create({ cascadeField: "Parent" });
        expect(editor.canSearch(false)).toBe(false);
        editor.options.cascadeValue = 3;
        expect(editor.canSearch(false)).toBe(true);
        editor.destroy();
    });

    it("resolves empty results when search is blocked", async () => {
        const editor = create({ cascadeField: "Parent" });
        const result = await editor.asyncSearch({} as any);
        expect(result).toEqual({ items: [], more: false });
        expect(serviceCallSpy).not.toHaveBeenCalled();
        editor.destroy();
    });

    it("loads items from the service", async () => {
        const editor = create({});
        serviceCallSpy.mockResolvedValue({ Entities: [{ ID: 1, Name: "One" }] } as any);
        const result = await editor.asyncSearch({ searchTerm: "o", skip: 0, take: 100, checkMore: true } as any);
        expect(serviceCallSpy).toHaveBeenCalledTimes(1);
        expect(result.items).toEqual([{ ID: 1, Name: "One" }]);
        expect(result.more).toBe(false);
        editor.destroy();
    });

    it("slices extra results and reports more when checking for more", async () => {
        const editor = create({});
        serviceCallSpy.mockResolvedValue({ Entities: [{ ID: 1, Name: "A" }, { ID: 2, Name: "B" }] } as any);
        const result = await editor.asyncSearch({ searchTerm: "x", take: 1, checkMore: true } as any);
        expect(result.items).toHaveLength(1);
        expect(result.items[0].ID).toBe(1);
        expect(result.more).toBe(true);
        editor.destroy();
    });

    it("returns null sort when no id or text field exists", () => {
        const editor = create({ idField: null, textField: null, sort: undefined });
        expect(editor.getSort()).toBeNull();
        editor.destroy();
    });

    it("handles a service response without entities", async () => {
        const editor = create({});
        serviceCallSpy.mockResolvedValue({} as any);
        const result = await editor.asyncSearch({ searchTerm: "x", take: 5, checkMore: true } as any);
        expect(result.items).toEqual([]);
        expect(result.more).toBe(false);
        editor.destroy();
    });
});

/*
import { ListResponse, ServiceResponse } from "../../base";
import { ServiceLookupEditor } from "./servicelookupeditor";

let oldWindowAlert: any;
beforeAll(() => {
    oldWindowAlert = window.alert;
    window.alert = () => { };
});

afterAll(() => {
    window.alert = oldWindowAlert;
});

afterEach(() => {
    vi.restoreAllMocks();
    vi.useRealTimers();
    document.body.innerHTML = "";
});

it('ServiceLookupEditor loads items from service', () => {
    new ServiceLookupEditor({
        idField: "id",
        textField: "text",
        sort: ["text"],
        service: "Test/List",
        element: el => document.body.appendChild(el)
    })

    vi.spyOn(jQuery, "ajax").mockImplementation(function (options: ServiceOptions<ServiceResponse & any>) {
        expect(options.url).toBe("/Services/Test/List");

        const response = {
            Entities: [
                { id: 1, text: "Test" },
                { id: 2, text: "Test2" }
            ]
        };

        options.onSuccess(response);
    } as any);

    vi.useFakeTimers();

    jQuery(document.body.querySelector(".select2-choice")).trigger("mousedown");

    vi.runOnlyPendingTimers();

    const options = Array.from(document.body.querySelectorAll(".select2-results li"));
    expect(options).toHaveLength(2);

    expect(options[0].textContent).toBe("Test");
    expect(options[1].textContent).toBe("Test2");
});

it('ServiceLookupEditor sets active and searching without search correctly', () => {

    new ServiceLookupEditor({
        idField: "id",
        textField: "text",
        sort: ["text"],
        service: "Test/List",
        element: el => document.body.appendChild(el)
    });

    vi.spyOn(jQuery, "ajax").mockImplementation(function (options: ServiceOptions<ServiceResponse & any>) {
        expect(document.body.querySelector(".select2-active")).not.toBeNull();
        expect(options.url).toBe("/Services/Test/List");

        const response: ListResponse<any> = {
            Entities: []
        };

        options.onSuccess(response);
    } as any);

    vi.useFakeTimers();

    jQuery(document.body.querySelector(".select2-choice")).trigger("mousedown");
    expect(document.body.querySelector(".select2-searching")).not.toBeNull();

    vi.runOnlyPendingTimers();

    expect(document.body.querySelector(".select2-searching")).toBeNull();
    expect(document.body.querySelector(".select2-active")).toBeNull();
});

it('ServiceLookupEditor can search items', () => {
    new ServiceLookupEditor({
        idField: "id",
        textField: "text",
        sort: ["text"],
        service: "Test/List",
        element: el => document.body.appendChild(el)        
    });

    vi.spyOn(jQuery, "ajax").mockImplementation(function (options: ServiceOptions<ServiceResponse & any>) {
        expect(options.url).toBe("/Services/Test/List");

        const containsText = options.request.ContainsText;

        const response = {
            Entities: [
                { id: 1, text: "Test" },
                { id: 2, text: "Test2" }
            ].filter(x => containsText == null || containsText === "" || x.text.indexOf(containsText) >= 0)
        };

        options.onSuccess(response);
    } as any);

    vi.useFakeTimers();

    jQuery(document.body.querySelector(".select2-choice")).trigger("mousedown");
    jQuery(document.body.querySelector(".select2-input")).val("Test2").trigger("input");

    vi.runOnlyPendingTimers();

    const options = Array.from(document.body.querySelectorAll(".select2-results li"));
    expect(options).toHaveLength(1);

    expect(options[0].textContent).toBe("Test2");
});

it('ServiceLookupEditor sets active and searching without search correctly while searching', () => {
    new ServiceLookupEditor({
        idField: "id",
        textField: "text",
        sort: ["text"],
        service: "Test/List",
        element: el => document.body.appendChild(el)
    });

    vi.spyOn(jQuery, "ajax").mockImplementation(function (options: ServiceOptions<ServiceResponse & any>) {
        expect(document.body.querySelector(".select2-active")).not.toBeNull();
        expect(options.url).toBe("/Services/Test/List");

        const containsText = options.request.ContainsText;
        const response = {
            Entities: [
                { id: 1, text: "Test" },
                { id: 2, text: "Test2" }
            ].filter(x => containsText == null || containsText === "" || x.text.indexOf(containsText) >= 0)
        };

        options.onSuccess(response);
    } as any);

    vi.useFakeTimers();

    jQuery(document.body.querySelector(".select2-choice")).trigger("mousedown");
    expect(document.body.querySelector(".select2-searching")).not.toBeNull();
    jQuery(document.body.querySelector(".select2-input")).val("Test2").trigger("input");

    vi.runOnlyPendingTimers();

    expect(document.body.querySelector(".select2-searching")).toBeNull();
    expect(document.body.querySelector(".select2-active")).toBeNull();

    expect(Array.from(document.body.querySelectorAll(".select2-results li"))).toHaveLength(1);
});

it('ServiceLookupEditor aborts previous requests', () => {

    const editor = new ServiceLookupEditor({
        idField: "id",
        textField: "text",
        sort: ["text"],
        service: "Test/List",
        element: el => document.body.appendChild(el)
    });

    const debounceDelay = editor["getTypeDelay"]?.();
    if (debounceDelay == null)
        throw new Error("getTypeDelay is null");

    let containsTexts: any[] = [];

    vi.spyOn(jQuery, "ajax").mockImplementation(function (options: ServiceOptions<ServiceResponse & any>) {
        expect(options.url).toBe("/Services/Test/List");
        containsTexts.push(options.request.ContainsText);

        const response = {
            Entities: [
                { id: 1, text: "Test" },
                { id: 2, text: "Test2" }
            ]
        };

        options.onSuccess(response);
    } as any);

    vi.useFakeTimers();

    jQuery(document.body.querySelector(".select2-choice")).trigger("mousedown");

    jQuery(document.body.querySelector(".select2-input")).val("Test2").trigger("input");
    vi.advanceTimersByTime(debounceDelay);

    jQuery(document.body.querySelector(".select2-input")).val("Test3").trigger("input");
    vi.advanceTimersByTime(debounceDelay - 1);

    jQuery(document.body.querySelector(".select2-input")).val("Test4").trigger("input");
    vi.advanceTimersByTime(debounceDelay - 1);

    jQuery(document.body.querySelector(".select2-input")).val("Test5").trigger("input");
    vi.advanceTimersByTime(debounceDelay);

    jQuery(document.body.querySelector(".select2-input")).val("Test6").trigger("input");
    vi.advanceTimersByTime(debounceDelay - 1);

    jQuery(document.body.querySelector(".select2-input")).val("Test7").trigger("input");
    vi.advanceTimersByTime(debounceDelay);

    expect(containsTexts).toEqual(["Test2", "Test5", "Test7"]);
});

it('ServiceLookupEditor aborts and set active and searching correctly', () => {
    const editor = new ServiceLookupEditor({
        idField: "id",
        textField: "text",
        sort: ["text"],
        service: "Test/List",
        element: el => document.body.appendChild(el)        
    });

    const debounceDelay = editor["getTypeDelay"]?.();
    if (debounceDelay == null)
        throw new Error("getTypeDelay is null");

    let containsTexts: any[] = [];
    let successCallbacks: (() => void)[] = [];

    vi.spyOn(jQuery, "ajax").mockImplementation(function (options: ServiceOptions<ServiceResponse & any>) {
        expect(options.url).toBe("/Services/Test/List");
        containsTexts.push(options.request.ContainsText);

        const response = {
            Entities: [
                { id: 1, text: "Test" },
                { id: 2, text: "Test2" }
            ]
        };

        successCallbacks.push(() => options.onSuccess(response));
    } as any);

    vi.useFakeTimers();

    jQuery(document.body.querySelector(".select2-choice")).trigger("mousedown");
    expect(document.body.querySelector(".select2-searching")).not.toBeNull();
    expect(document.body.querySelector(".select2-active")).toBeNull(); // no requests are pending

    jQuery(document.body.querySelector(".select2-input")).val("Test2").trigger("input");
    expect(document.body.querySelector(".select2-searching")).not.toBeNull();
    vi.advanceTimersByTime(debounceDelay);
    expect(document.body.querySelector(".select2-searching")).not.toBeNull(); // shows searching
    expect(document.body.querySelector(".select2-active")).not.toBeNull();
    successCallbacks[0]();
    expect(document.body.querySelector(".select2-active")).toBeNull();


    jQuery(document.body.querySelector(".select2-input")).val("Test3").trigger("input");
    expect(document.body.querySelector(".select2-searching")).toBeNull();
    vi.advanceTimersByTime(debounceDelay - 1);
    expect(document.body.querySelector(".select2-searching")).toBeNull(); // doesn't show searching as there is data on screen
    expect(document.body.querySelector(".select2-active")).toBeNull(); // no requests are pending

    jQuery(document.body.querySelector(".select2-input")).val("Test4").trigger("input");
    expect(document.body.querySelector(".select2-searching")).toBeNull(); // doesn't show searching as there is data on screen
    vi.advanceTimersByTime(debounceDelay - 1);
    expect(document.body.querySelector(".select2-searching")).toBeNull(); // doesn't show searching as there is data on screen
    expect(document.body.querySelector(".select2-active")).toBeNull(); // no requests are pending

    jQuery(document.body.querySelector(".select2-input")).val("Test5").trigger("input");
    expect(document.body.querySelector(".select2-searching")).toBeNull(); // doesn't show searching as there is data on screen
    vi.advanceTimersByTime(debounceDelay);
    expect(document.body.querySelector(".select2-searching")).toBeNull(); // doesn't show searching as there is data on screen
    expect(document.body.querySelector(".select2-active")).not.toBeNull(); // shows active as there is a pending request
    successCallbacks[1]();
    expect(document.body.querySelector(".select2-active")).toBeNull(); // no requests are pending

    jQuery(document.body.querySelector(".select2-input")).val("Test6").trigger("input");
    expect(document.body.querySelector(".select2-searching")).toBeNull(); // doesn't show searching as there is data on screen
    vi.advanceTimersByTime(debounceDelay);
    expect(document.body.querySelector(".select2-searching")).toBeNull(); // doesn't show searching as there is data on screen
    expect(document.body.querySelector(".select2-active")).not.toBeNull(); // shows active as there is a pending request
    successCallbacks[2]();
    expect(document.body.querySelector(".select2-active")).toBeNull(); // no requests are pending

    expect(containsTexts).toEqual(["Test2", "Test5", "Test6"]);
});

it('ServiceLookupEditor aborts while request is pending', () => {

    const editor = new ServiceLookupEditor({
        idField: "id",
        textField: "text",
        sort: ["text"],
        service: "Test/List",
        element: el => document.body.appendChild(el)
    });

    const debounceDelay = editor["getTypeDelay"]?.();
    if (debounceDelay == null)
        throw new Error("getTypeDelay is null");

    let containsTexts: any[] = [];
    let successCallbacks: (() => void)[] = [];
    let abortedTexts: any[] = [];

    vi.spyOn(jQuery, "ajax").mockImplementation(function (options: ServiceOptions<ServiceResponse & any>) {
        expect(options.url).toBe("/Services/Test/List");

        const response = {
            Entities: [
                { id: 1, text: "Test" },
                { id: 2, text: "Test2" }
            ]
        };

        successCallbacks.push(() => {
            containsTexts.push(options.request.ContainsText);
            options.onSuccess(response)
        });

        return {
            abort: () => {
                abortedTexts.push(options.request.ContainsText);
            }
        }
    } as any);

    vi.useFakeTimers();

    jQuery(document.body.querySelector(".select2-choice")).trigger("mousedown");
    expect(document.body.querySelector(".select2-searching")).not.toBeNull();
    expect(document.body.querySelector(".select2-active")).toBeNull(); // no requests are pending

    jQuery(document.body.querySelector(".select2-input")).val("Test2").trigger("input");
    expect(document.body.querySelector(".select2-searching")).not.toBeNull();
    vi.advanceTimersByTime(debounceDelay);
    expect(document.body.querySelector(".select2-searching")).not.toBeNull(); // shows searching
    expect(document.body.querySelector(".select2-active")).not.toBeNull(); // shows active as there is a pending request

    jQuery(document.body.querySelector(".select2-input")).val("Test3").trigger("input");
    expect(document.body.querySelector(".select2-searching")).not.toBeNull(); // shows searching
    expect(document.body.querySelector(".select2-active")).toBeNull(); // aborted previous request
    vi.advanceTimersByTime(debounceDelay); // aborts previous request
    expect(document.body.querySelector(".select2-searching")).not.toBeNull(); // shows searching
    expect(document.body.querySelector(".select2-active")).not.toBeNull(); // shows active as there is a pending request
    successCallbacks[1]();
    expect(document.body.querySelector(".select2-active")).toBeNull(); // no requests are pending

    expect(containsTexts).toEqual(["Test3"]);
    expect(abortedTexts).toEqual(["Test2"]);
});

*/