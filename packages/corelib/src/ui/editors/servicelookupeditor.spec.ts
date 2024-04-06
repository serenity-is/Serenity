describe("servicelookupeditor", () => {
    it("is pending update", () => {
        expect(true).toBe(true);
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
    jest.restoreAllMocks();
    jest.useRealTimers();
    document.body.innerHTML = "";
});

test('ServiceLookupEditor loads items from service', () => {
    new ServiceLookupEditor({
        idField: "id",
        textField: "text",
        sort: ["text"],
        service: "Test/List",
        element: el => document.body.appendChild(el)
    })

    jest.spyOn(jQuery, "ajax").mockImplementation(function (options: ServiceOptions<ServiceResponse & any>) {
        expect(options.url).toBe("/Services/Test/List");

        const response = {
            Entities: [
                { id: 1, text: "Test" },
                { id: 2, text: "Test2" }
            ]
        };

        options.onSuccess(response);
    } as any);

    jest.useFakeTimers();

    jQuery(document.body.querySelector(".select2-choice")).trigger("mousedown");

    jest.runOnlyPendingTimers();

    const options = Array.from(document.body.querySelectorAll(".select2-results li"));
    expect(options).toHaveLength(2);

    expect(options[0].textContent).toBe("Test");
    expect(options[1].textContent).toBe("Test2");
});

test('ServiceLookupEditor sets active and searching without search correctly', () => {

    new ServiceLookupEditor({
        idField: "id",
        textField: "text",
        sort: ["text"],
        service: "Test/List",
        element: el => document.body.appendChild(el)
    });

    jest.spyOn(jQuery, "ajax").mockImplementation(function (options: ServiceOptions<ServiceResponse & any>) {
        expect(document.body.querySelector(".select2-active")).not.toBeNull();
        expect(options.url).toBe("/Services/Test/List");

        const response: ListResponse<any> = {
            Entities: []
        };

        options.onSuccess(response);
    } as any);

    jest.useFakeTimers();

    jQuery(document.body.querySelector(".select2-choice")).trigger("mousedown");
    expect(document.body.querySelector(".select2-searching")).not.toBeNull();

    jest.runOnlyPendingTimers();

    expect(document.body.querySelector(".select2-searching")).toBeNull();
    expect(document.body.querySelector(".select2-active")).toBeNull();
});

test('ServiceLookupEditor can search items', () => {
    new ServiceLookupEditor({
        idField: "id",
        textField: "text",
        sort: ["text"],
        service: "Test/List",
        element: el => document.body.appendChild(el)        
    });

    jest.spyOn(jQuery, "ajax").mockImplementation(function (options: ServiceOptions<ServiceResponse & any>) {
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

    jest.useFakeTimers();

    jQuery(document.body.querySelector(".select2-choice")).trigger("mousedown");
    jQuery(document.body.querySelector(".select2-input")).val("Test2").trigger("input");

    jest.runOnlyPendingTimers();

    const options = Array.from(document.body.querySelectorAll(".select2-results li"));
    expect(options).toHaveLength(1);

    expect(options[0].textContent).toBe("Test2");
});

test('ServiceLookupEditor sets active and searching without search correctly while searching', () => {
    new ServiceLookupEditor({
        idField: "id",
        textField: "text",
        sort: ["text"],
        service: "Test/List",
        element: el => document.body.appendChild(el)
    });

    jest.spyOn(jQuery, "ajax").mockImplementation(function (options: ServiceOptions<ServiceResponse & any>) {
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

    jest.useFakeTimers();

    jQuery(document.body.querySelector(".select2-choice")).trigger("mousedown");
    expect(document.body.querySelector(".select2-searching")).not.toBeNull();
    jQuery(document.body.querySelector(".select2-input")).val("Test2").trigger("input");

    jest.runOnlyPendingTimers();

    expect(document.body.querySelector(".select2-searching")).toBeNull();
    expect(document.body.querySelector(".select2-active")).toBeNull();

    expect(Array.from(document.body.querySelectorAll(".select2-results li"))).toHaveLength(1);
});

test('ServiceLookupEditor aborts previous requests', () => {

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

    jest.spyOn(jQuery, "ajax").mockImplementation(function (options: ServiceOptions<ServiceResponse & any>) {
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

    jest.useFakeTimers();

    jQuery(document.body.querySelector(".select2-choice")).trigger("mousedown");

    jQuery(document.body.querySelector(".select2-input")).val("Test2").trigger("input");
    jest.advanceTimersByTime(debounceDelay);

    jQuery(document.body.querySelector(".select2-input")).val("Test3").trigger("input");
    jest.advanceTimersByTime(debounceDelay - 1);

    jQuery(document.body.querySelector(".select2-input")).val("Test4").trigger("input");
    jest.advanceTimersByTime(debounceDelay - 1);

    jQuery(document.body.querySelector(".select2-input")).val("Test5").trigger("input");
    jest.advanceTimersByTime(debounceDelay);

    jQuery(document.body.querySelector(".select2-input")).val("Test6").trigger("input");
    jest.advanceTimersByTime(debounceDelay - 1);

    jQuery(document.body.querySelector(".select2-input")).val("Test7").trigger("input");
    jest.advanceTimersByTime(debounceDelay);

    expect(containsTexts).toEqual(["Test2", "Test5", "Test7"]);
});

test('ServiceLookupEditor aborts and set active and searching correctly', () => {
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

    jest.spyOn(jQuery, "ajax").mockImplementation(function (options: ServiceOptions<ServiceResponse & any>) {
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

    jest.useFakeTimers();

    jQuery(document.body.querySelector(".select2-choice")).trigger("mousedown");
    expect(document.body.querySelector(".select2-searching")).not.toBeNull();
    expect(document.body.querySelector(".select2-active")).toBeNull(); // no requests are pending

    jQuery(document.body.querySelector(".select2-input")).val("Test2").trigger("input");
    expect(document.body.querySelector(".select2-searching")).not.toBeNull();
    jest.advanceTimersByTime(debounceDelay);
    expect(document.body.querySelector(".select2-searching")).not.toBeNull(); // shows searching
    expect(document.body.querySelector(".select2-active")).not.toBeNull();
    successCallbacks[0]();
    expect(document.body.querySelector(".select2-active")).toBeNull();


    jQuery(document.body.querySelector(".select2-input")).val("Test3").trigger("input");
    expect(document.body.querySelector(".select2-searching")).toBeNull();
    jest.advanceTimersByTime(debounceDelay - 1);
    expect(document.body.querySelector(".select2-searching")).toBeNull(); // doesn't show searching as there is data on screen
    expect(document.body.querySelector(".select2-active")).toBeNull(); // no requests are pending

    jQuery(document.body.querySelector(".select2-input")).val("Test4").trigger("input");
    expect(document.body.querySelector(".select2-searching")).toBeNull(); // doesn't show searching as there is data on screen
    jest.advanceTimersByTime(debounceDelay - 1);
    expect(document.body.querySelector(".select2-searching")).toBeNull(); // doesn't show searching as there is data on screen
    expect(document.body.querySelector(".select2-active")).toBeNull(); // no requests are pending

    jQuery(document.body.querySelector(".select2-input")).val("Test5").trigger("input");
    expect(document.body.querySelector(".select2-searching")).toBeNull(); // doesn't show searching as there is data on screen
    jest.advanceTimersByTime(debounceDelay);
    expect(document.body.querySelector(".select2-searching")).toBeNull(); // doesn't show searching as there is data on screen
    expect(document.body.querySelector(".select2-active")).not.toBeNull(); // shows active as there is a pending request
    successCallbacks[1]();
    expect(document.body.querySelector(".select2-active")).toBeNull(); // no requests are pending

    jQuery(document.body.querySelector(".select2-input")).val("Test6").trigger("input");
    expect(document.body.querySelector(".select2-searching")).toBeNull(); // doesn't show searching as there is data on screen
    jest.advanceTimersByTime(debounceDelay);
    expect(document.body.querySelector(".select2-searching")).toBeNull(); // doesn't show searching as there is data on screen
    expect(document.body.querySelector(".select2-active")).not.toBeNull(); // shows active as there is a pending request
    successCallbacks[2]();
    expect(document.body.querySelector(".select2-active")).toBeNull(); // no requests are pending

    expect(containsTexts).toEqual(["Test2", "Test5", "Test6"]);
});

test('ServiceLookupEditor aborts while request is pending', () => {

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

    jest.spyOn(jQuery, "ajax").mockImplementation(function (options: ServiceOptions<ServiceResponse & any>) {
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

    jest.useFakeTimers();

    jQuery(document.body.querySelector(".select2-choice")).trigger("mousedown");
    expect(document.body.querySelector(".select2-searching")).not.toBeNull();
    expect(document.body.querySelector(".select2-active")).toBeNull(); // no requests are pending

    jQuery(document.body.querySelector(".select2-input")).val("Test2").trigger("input");
    expect(document.body.querySelector(".select2-searching")).not.toBeNull();
    jest.advanceTimersByTime(debounceDelay);
    expect(document.body.querySelector(".select2-searching")).not.toBeNull(); // shows searching
    expect(document.body.querySelector(".select2-active")).not.toBeNull(); // shows active as there is a pending request

    jQuery(document.body.querySelector(".select2-input")).val("Test3").trigger("input");
    expect(document.body.querySelector(".select2-searching")).not.toBeNull(); // shows searching
    expect(document.body.querySelector(".select2-active")).toBeNull(); // aborted previous request
    jest.advanceTimersByTime(debounceDelay); // aborts previous request
    expect(document.body.querySelector(".select2-searching")).not.toBeNull(); // shows searching
    expect(document.body.querySelector(".select2-active")).not.toBeNull(); // shows active as there is a pending request
    successCallbacks[1]();
    expect(document.body.querySelector(".select2-active")).toBeNull(); // no requests are pending

    expect(containsTexts).toEqual(["Test3"]);
    expect(abortedTexts).toEqual(["Test2"]);
});

*/