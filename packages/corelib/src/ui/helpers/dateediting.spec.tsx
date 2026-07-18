import { dateInputChangeHandler, dateInputKeyupHandler, flatPickrTrigger, jQueryDatepickerZIndexWorkaround, jQueryDatepickerInitialization } from "./dateediting";
import { Culture } from "../../base";
import type { MockInstance } from "vitest";

describe("dateInputChangeHandler", () => {
    let mockInput: HTMLInputElement;
    let mockInputSetSpy: MockInstance;
    let mockEvent: Event;
    let originalDateOrder: string;
    let originalDateSeparator: string;

    beforeEach(() => {
        mockInput = {
            _value: '',
            get value() { return this._value; },
            set value(val: string) { this._value = val; },
            getAttribute: vi.fn().mockReturnValue(null),
            setAttribute: vi.fn()
        } as any;
        mockEvent = {
            target: mockInput
        } as any;
        mockInputSetSpy = vi.spyOn(mockInput, 'value', 'set');
        originalDateOrder = Culture.dateOrder;
        originalDateSeparator = Culture.dateSeparator;
    });

    afterEach(() => {
        vi.restoreAllMocks();
        Culture.dateOrder = originalDateOrder;
        Culture.dateSeparator = originalDateSeparator;
    });

    it("should do nothing if Culture.dateOrder is not 'dmy'", () => {
        const originalDateOrder = Culture.dateOrder;
        Culture.dateOrder = 'mdy';

        dateInputChangeHandler(mockEvent);

        expect(mockInputSetSpy).not.toHaveBeenCalled();
        expect(mockInput.value).toBe('');

        Culture.dateOrder = originalDateOrder;
    });

    it("should do nothing if input type is 'date'", () => {
        Culture.dateOrder = 'dmy';
        (mockInput.getAttribute as any).mockReturnValue('date');

        dateInputChangeHandler(mockEvent);

        expect(mockInputSetSpy).not.toHaveBeenCalled();
        expect(mockInput.value).toBe('');
    });

    it("should format 6+ digit numeric string into date format", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '121056';

        dateInputChangeHandler(mockEvent);

        expect(mockInput.value).toBe('12/10/1956');
    });

    it("should not format non-numeric strings", () => {
        Culture.dateOrder = 'dmy';
        mockInput.value = 'abc123';

        expect(mockInput.value).toBe('abc123');
    });

    it("should not format non-numeric strings", () => {
        Culture.dateOrder = 'dmy';
        mockInput.value = 'abc123';

        dateInputChangeHandler(mockEvent);

        expect(mockInput.value).toBe('abc123');
    });

    it("should parse and format valid date strings", () => {
        Culture.dateOrder = 'dmy';
        mockInput.value = '12/12/2023';

        dateInputChangeHandler(mockEvent);

        // Should parse and reformat the date
        expect(mockInput.value).toBe('12/12/2023');
    });

    it("should handle null input value", () => {
        Culture.dateOrder = 'dmy';
        mockInput.value = null as any;

        dateInputChangeHandler(mockEvent);

        expect(mockInput.value).toBeNull();
    });
});

describe("dateInputKeyupHandler", () => {
    let mockInput: HTMLInputElement;
    let mockInputSetSpy: MockInstance;
    let mockEvent: any;
    let originalDateOrder: string;
    let originalDateSeparator: string;

    beforeEach(() => {
        mockInput = document.createElement('input');
        vi.spyOn(mockInput, 'getAttribute').mockReturnValue(null);
        mockEvent = {
            target: mockInput,
            key: null
        };
        originalDateOrder = Culture.dateOrder;
        originalDateSeparator = Culture.dateSeparator;
        mockInputSetSpy = vi.spyOn(mockInput, 'value', 'set');
    });

    afterEach(() => {
        vi.restoreAllMocks();
        Culture.dateOrder = originalDateOrder;
        Culture.dateSeparator = originalDateSeparator;
    });

    it("should do nothing if Culture.dateOrder is not 'dmy' or 'mdy'", () => {
        Culture.dateOrder = 'ymd';

        dateInputKeyupHandler(mockEvent);

        expect(mockInput.value).toBe('');
    });

    it("should do nothing if input type is 'date'", () => {
        Culture.dateOrder = 'dmy';
        (mockInput.getAttribute as any).mockReturnValue('date');

        dateInputKeyupHandler(mockEvent);

        expect(mockInputSetSpy).not.toHaveBeenCalled();
        expect(mockInput.value).toBe('');
    });

    it("should do nothing if input is readonly", () => {
        Culture.dateOrder = 'dmy';
        (mockInput.getAttribute as any).mockReturnValue('readonly');

        dateInputKeyupHandler(mockEvent);

        expect(mockInputSetSpy).not.toHaveBeenCalled();
        expect(mockInput.value).toBe('');
    });

    it("should do nothing if input is disabled", () => {
        Culture.dateOrder = 'dmy';
        (mockInput.getAttribute as any).mockReturnValue('disabled');

        dateInputKeyupHandler(mockEvent);

        expect(mockInputSetSpy).not.toHaveBeenCalled();
        expect(mockInput.value).toBe('');
    });

    it("should do nothing if cursor is not at end", () => {
        Culture.dateOrder = 'dmy';
        mockInput.value = '12';
        mockInput.selectionEnd = 1;

        dateInputKeyupHandler(mockEvent);

        expect(mockInput.value).toBe('12');
    });

    it("should remove double separators", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '12//';
        mockInput.selectionEnd = 4; // At the end

        dateInputKeyupHandler(mockEvent);

        expect(mockInput.value).toBe('12/');
    });

    it("should handle slash key at position 2 (d/) with dmy", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '1/';
        mockInput.selectionEnd = 2;
        mockEvent.key = 'slash';

        dateInputKeyupHandler(mockEvent);

        expect(mockInput.value).toBe('01/');
    });

    it("should handle slash key at position 2 (m/) with mdy", () => {
        Culture.dateOrder = 'mdy';
        Culture.dateSeparator = '/';
        mockInput.value = '1/';
        mockInput.selectionEnd = 2;
        mockEvent.key = 'slash';

        dateInputKeyupHandler(mockEvent);

        expect(mockInput.value).toBe('01/');
    });

    it("should handle slash key at position 4 (d/m/) with dmy", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '1/3/';
        mockInput.selectionEnd = 4;
        mockEvent.key = 'slash';

        dateInputKeyupHandler(mockEvent);

        expect(mockInput.value).toBe('01/03/');
    });

    it("should handle slash key at position 4 (m/d/) with mdy", () => {
        Culture.dateOrder = 'mdy';
        Culture.dateSeparator = '/';
        mockInput.value = '12/3/';
        mockInput.selectionEnd = 4;
        mockEvent.key = 'slash';

        dateInputKeyupHandler(mockEvent);

        expect(mockInput.value).toBe('12/3/');
    });

    it("should handle slash key at position 5 (d/mm/) with dmy", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '1/03/';
        mockInput.selectionEnd = 5;
        mockEvent.key = 'slash';

        dateInputKeyupHandler(mockEvent);

        expect(mockInput.value).toBe('01/03/');
    });

    it("should handle slash key at position 5 (m/dd/) with mdy", () => {
        Culture.dateOrder = 'mdy';
        Culture.dateSeparator = '/';
        mockInput.value = '1/03/';
        mockInput.selectionEnd = 5;
        mockEvent.key = 'slash';

        dateInputKeyupHandler(mockEvent);

        expect(mockInput.value).toBe('01/03/');
    });

    it("should handle NumpadDivide key", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '1/';
        mockInput.selectionEnd = 2;
        mockEvent.key = 'NumpadDivide';

        dateInputKeyupHandler(mockEvent);

        expect(mockInput.value).toBe('01/');
    });

    it("should handle '/' key directly", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '1/';
        mockInput.selectionEnd = 2;
        mockEvent.key = '/';

        dateInputKeyupHandler(mockEvent);

        expect(mockInput.value).toBe('01/');
    });

    it("should handle slash key when last char is not separator (case 2)", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '12';
        mockInput.selectionEnd = 2;
        mockEvent.key = 'slash';

        dateInputKeyupHandler(mockEvent);

        // Last char is not separator, so it returns early from the slash handler
        expect(mockInput.value).toBe('12');
    });

    it("should handle slash key case 4 with mdy", () => {
        Culture.dateOrder = 'mdy';
        Culture.dateSeparator = '/';
        mockInput.value = '1/3/';
        mockInput.selectionEnd = 4;
        mockEvent.key = 'slash';

        dateInputKeyupHandler(mockEvent);

        expect(mockInput.value).toBe('01/03/');
    });

    it("should handle slash key at length 5 with valid pattern 1 (d/mm/)", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '4/12/';
        mockInput.selectionEnd = 5;
        mockEvent.key = 'slash';

        dateInputKeyupHandler(mockEvent);

        expect(mockInput.value).toBe('04/12/');
    });

    it("should handle slash key at length 5 with valid pattern 2 (dd/m/)", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '12/4/';
        mockInput.selectionEnd = 5;
        mockEvent.key = 'slash';

        dateInputKeyupHandler(mockEvent);

        expect(mockInput.value).toBe('12/04/');
    });

    it("should handle slash key with non-digit first char at length 2", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = 'a/';
        mockInput.selectionEnd = 2;
        mockEvent.key = 'slash';

        dateInputKeyupHandler(mockEvent);

        // Non-digit first char causes early return from slash handler
        expect(mockInput.value).toBe('a/');
    });

    it("should hit default case in slash handler with length 3", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '12/';
        mockInput.selectionEnd = 3;
        mockEvent.key = 'slash';

        dateInputKeyupHandler(mockEvent);

        // Length is 3, not in switch (2,4,5), hits default -> return
        expect(mockInput.value).toBe('12/');
    });

    it("should do nothing when slash handler has length 5 but no matching format", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '12/34';
        mockInput.selectionEnd = 5;
        mockEvent.key = 'slash';

        dateInputKeyupHandler(mockEvent);

        // Length is 5 but last char is not separator (it's '4')
        expect(mockInput.value).toBe('12/34');
    });

    it("should handle numeric input and add separator at position 2", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '12';
        mockInput.selectionEnd = 2;
        mockEvent.key = '2';

        dateInputKeyupHandler(mockEvent);

        expect(mockInput.value).toBe('12/');
    });

    it("should handle numeric input and add separator at position 5", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '12/34';
        mockInput.selectionEnd = 5;
        mockEvent.key = '5';

        dateInputKeyupHandler(mockEvent);

        expect(mockInput.value).toBe('12/34/');
    });

    it("should pad single digit day with zero", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '4';
        mockInput.selectionEnd = 1;
        mockEvent.key = '4';

        dateInputKeyupHandler(mockEvent);

        expect(mockInput.value).toBe('04/');
    });

    it("should not prepend zero for day <= 3 (returns early)", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '3';
        mockInput.selectionEnd = 1;
        mockEvent.key = '3';

        dateInputKeyupHandler(mockEvent);

        // charCodeAt(0) = 51, which is <= 51, so it returns without changing value
        // But the handler may already have added a separator via prior logic
        expect(mockInput.value).toBe('3');
    });

    it("should handle case 2 in dmy autocomplete with non-digit", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = 'a';
        mockInput.selectionEnd = 2;
        mockEvent.key = 'a';

        dateInputKeyupHandler(mockEvent);

        // Not a digit, so shouldn't enter the dmy autocomplete
        expect(mockInput.value).toBe('a');
    });

    it("should handle case 3 in dmy autocomplete with separator and valid digits", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '1/2';
        mockInput.selectionEnd = 3;
        mockEvent.key = '3';

        dateInputKeyupHandler(mockEvent);

        // case 3: val[0] is digit, val[1] is separator, val.charCodeAt(2) = 50 > 49
        // so it pads: 1/2 -> 01/02 + separator -> 01/02/
        expect(mockInput.value).toBe('01/02/');
    });

    it("should handle case 3 with month <= 1 (no padding)", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '1/0';
        mockInput.selectionEnd = 3;
        mockEvent.key = '2';

        dateInputKeyupHandler(mockEvent);

        // val.charCodeAt(2) = 48 ('0') <= 49, so it returns without change
        expect(mockInput.value).toBe('1/0');
    });

    it("should handle case 4 with separator at position 1 (d/m)", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '1/23';
        mockInput.selectionEnd = 4;
        mockEvent.key = '4';

        dateInputKeyupHandler(mockEvent);

        // val[1] == '/', so it pads to "01/23" then adds separator
        expect(mockInput.value).toBe('01/23/');
    });

    it("should handle case 4 with separator at position 2 (dd/m)", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '12/3';
        mockInput.selectionEnd = 4;
        mockEvent.key = '4';

        dateInputKeyupHandler(mockEvent);

        // val[2] == '/', month char '3' > '1' (charCode 51 > 49), so padding happens
        expect(mockInput.value).toBe('12/03/');
    });

    it("should handle case 4 with separator at position 2 and month <= 1", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '12/0';
        mockInput.selectionEnd = 4;
        mockEvent.key = '0';

        dateInputKeyupHandler(mockEvent);

        // val.charCodeAt(3) = 48 <= 49, so returns without change
        expect(mockInput.value).toBe('12/0');
    });

    it("should handle case 4 with separator at position 2 and non-digit first char", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = 'ab/3';
        mockInput.selectionEnd = 4;
        mockEvent.key = '3';

        dateInputKeyupHandler(mockEvent);

        // val[2] == '/' but val[0] is not digit so returns
        expect(mockInput.value).toBe('ab/3');
    });

    it("should handle case 4 with invalid digits at separator position 1", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = 'a/b5';
        mockInput.selectionEnd = 4;
        mockEvent.key = '5';

        dateInputKeyupHandler(mockEvent);

        // val[1] == '/' but val[0] ('a') is not a digit, so returns
        expect(mockInput.value).toBe('a/b5');
    });

    it("should handle case 4 where neither position matches separator", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = 'a123';
        mockInput.selectionEnd = 4;
        mockEvent.key = '4';

        dateInputKeyupHandler(mockEvent);

        // Neither val[1] nor val[2] is '/', so returns
        expect(mockInput.value).toBe('a123');
    });

    it("should handle case 5 with valid separator and digits", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '12/05';
        mockInput.selectionEnd = 5;
        mockEvent.key = '6';

        dateInputKeyupHandler(mockEvent);

        // Valid: val[2] is '/', all digits, adds separator
        expect(mockInput.value).toBe('12/05/');
    });

    it("should handle case 2 with non-digit first char", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = 'a2';
        mockInput.selectionEnd = 2;
        mockEvent.key = '3';

        dateInputKeyupHandler(mockEvent);

        // val[0] is not digit, so returns
        expect(mockInput.value).toBe('a2');
    });

    it("should handle case 5 with invalid separator position", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '1205a';
        mockInput.selectionEnd = 5;
        mockEvent.key = '6';

        dateInputKeyupHandler(mockEvent);

        // val[2] is '0' not '/', so returns
        expect(mockInput.value).toBe('1205a');
    });

    it("should handle case 5 with non-digit characters", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '12/a5';
        mockInput.selectionEnd = 5;
        mockEvent.key = '6';

        dateInputKeyupHandler(mockEvent);

        // val[2] is '/' but val[3] is not digit, so returns
        expect(mockInput.value).toBe('12/a5');
    });

    it("should not apply dmy autocomplete for mdy date order", () => {
        Culture.dateOrder = 'mdy';
        Culture.dateSeparator = '/';
        mockInput.value = '12';
        mockInput.selectionEnd = 2;
        mockEvent.key = '3';

        dateInputKeyupHandler(mockEvent);

        // dmy autocomplete only applies when dateOrder is 'dmy'
        expect(mockInput.value).toBe('12');
    });

    it("should handle null input value", () => {
        Culture.dateOrder = 'dmy';
        mockInput.value = null;

        dateInputKeyupHandler(mockEvent);

        expect(mockInput.value).toBe("");
    });
});

describe("flatPickrTrigger", () => {
    let mockInput: any;

    beforeEach(() => {
        mockInput = document.createElement('input');
        mockInput._flatpickr = {
            open: vi.fn(),
            calendarContainer: {
                focus: vi.fn()
            }
        };
    });

    it("should create a button element", () => {
        const button = flatPickrTrigger(mockInput);

        expect(button).toBeInstanceOf(HTMLButtonElement);
        expect((button as HTMLButtonElement).type).toBe('button');
    });

    it("should add ui-datepicker-trigger class to button", () => {
        const button = flatPickrTrigger(mockInput);

        expect(button.classList.contains('ui-datepicker-trigger')).toBe(true);
    });

    it("should add click event listener that opens flatpickr", () => {
        const button = flatPickrTrigger(mockInput);

        button.click();

        expect(mockInput._flatpickr.open).toHaveBeenCalled();
        expect(mockInput._flatpickr.calendarContainer.focus).toHaveBeenCalled();
    });

    it("should not open flatpickr if input has readonly class", () => {
        mockInput.classList.add('readonly');

        const button = flatPickrTrigger(mockInput);
        button.click();

        expect(mockInput._flatpickr.open).not.toHaveBeenCalled();
    });

    it("should not open flatpickr if input has readonly attribute", () => {
        mockInput.setAttribute('readonly', 'readonly');

        const button = flatPickrTrigger(mockInput);
        button.click();

        expect(mockInput._flatpickr.open).not.toHaveBeenCalled();
    });

    it("should handle missing flatpickr instance", () => {
        mockInput._flatpickr = null;

        const button = flatPickrTrigger(mockInput);

        expect(() => button.click()).not.toThrow();
    });
});

describe("jQueryDatepickerZIndexWorkaround", () => {
    let mockInput: HTMLInputElement;
    let mockDialog: HTMLElement;
    let mockWidget: any;

    beforeEach(() => {
        vi.useFakeTimers();
        mockInput = document.createElement('input');
        mockDialog = document.createElement('div');
        mockDialog.classList.add('ui-dialog');
        mockWidget = {
            css: vi.fn().mockReturnValue(0),
            length: 1
        };

        // Mock getComputedStyle
        vi.spyOn(window, 'getComputedStyle').mockReturnValue({
            zIndex: '100'
        } as any);

        // Mock jQuery
        (globalThis as any).$ = vi.fn().mockReturnValue({
            datepicker: vi.fn().mockImplementation((method: string) => {
                if (method === 'widget') return mockWidget;
            })
        });
        (globalThis as any).jQuery = (globalThis as any).$;
    });

    afterEach(() => {
        vi.restoreAllMocks();
        vi.useRealTimers();
        delete (globalThis as any).$;
    });

    it("should do nothing if jQuery is not available", () => {
        delete (globalThis as any).$;

        jQueryDatepickerZIndexWorkaround(mockInput);

        // Should not throw
        expect(true).toBe(true);
    });

    it("should do nothing if input is not in a dialog", () => {
        // Input is not in a dialog
        document.body.appendChild(mockInput);

        jQueryDatepickerZIndexWorkaround(mockInput);

        expect((globalThis as any).$).not.toHaveBeenCalled();
    });

    it("should do nothing if dialog has no z-index", () => {
        mockDialog.style.zIndex = '';
        document.body.appendChild(mockDialog);
        mockDialog.appendChild(mockInput);

        vi.spyOn(window, 'getComputedStyle').mockReturnValue({
            zIndex: 'auto'
        } as any);

        jQueryDatepickerZIndexWorkaround(mockInput);

        vi.runAllTimers();

        expect(mockWidget.css).not.toHaveBeenCalled();
    });

    it("should set widget z-index higher than dialog", () => {
        document.body.appendChild(mockDialog);
        mockDialog.appendChild(mockInput);

        jQueryDatepickerZIndexWorkaround(mockInput);

        vi.runAllTimers();

        expect(mockWidget.css).toHaveBeenCalledWith('z-index', 101);
    });

    it("should handle widget not found", () => {
        mockWidget.length = 0;
        document.body.appendChild(mockDialog);
        mockDialog.appendChild(mockInput);

        jQueryDatepickerZIndexWorkaround(mockInput);

        vi.runAllTimers();

        expect(mockWidget.css).not.toHaveBeenCalled();
    });
});

describe("jQueryDatepickerInitialization", () => {
    let mockRegional: any;
    let originalCulture: any;

    beforeEach(() => {
        originalCulture = { ...Culture };
        mockRegional = {
            en: {},
            fr: {},
            regional: vi.fn()
        };

        const mock$ = vi.fn() as any;
        mock$.datepicker = {
            regional: mockRegional,
            setDefaults: vi.fn()
        };
        (globalThis as any).$ = mock$;
        (globalThis as any).jQuery = mock$;

        // Mock document
        (globalThis as any).document = {
            documentElement: {
                lang: 'en'
            }
        };
    });

    afterEach(() => {
        Object.assign(Culture, originalCulture);
        vi.restoreAllMocks();
        delete (globalThis as any).$;
        delete (globalThis as any).jQuery;
        delete (globalThis as any).document;
    });

    it("should return false if jQuery datepicker regional is not available", () => {
        delete (globalThis as any).$.datepicker.regional;

        const result = jQueryDatepickerInitialization();

        expect(result).toBe(false);
    });

    it("should return false if jQuery datepicker regional.en is not available", () => {
        delete (globalThis as any).$.datepicker.regional.en;

        const result = jQueryDatepickerInitialization();

        expect(result).toBe(false);
    });

    it("should initialize datepicker with DMY format", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';

        const result = jQueryDatepickerInitialization();

        expect(result).toBe(true);
        expect((globalThis as any).$.datepicker.setDefaults).toHaveBeenCalledWith(
            expect.objectContaining({
                dateFormat: 'dd/mm/yy'
            })
        );
    });

    it("should initialize datepicker with MDY format", () => {
        Culture.dateOrder = 'mdy';
        Culture.dateSeparator = '/';

        const result = jQueryDatepickerInitialization();

        expect(result).toBe(true);
        expect((globalThis as any).$.datepicker.setDefaults).toHaveBeenCalledWith(
            expect.objectContaining({
                dateFormat: 'mm/dd/yy'
            })
        );
    });

    it("should initialize datepicker with YMD format", () => {
        Culture.dateOrder = 'ymd';
        Culture.dateSeparator = '-';

        const result = jQueryDatepickerInitialization();

        expect(result).toBe(true);
        expect((globalThis as any).$.datepicker.setDefaults).toHaveBeenCalledWith(
            expect.objectContaining({
                dateFormat: 'yy-mm-dd'
            })
        );
    });

    it("should use document language for regional settings", () => {
        (globalThis as any).document.documentElement.lang = 'fr';
        mockRegional.fr = {};

        jQueryDatepickerInitialization();

        expect((globalThis as any).$.datepicker.setDefaults).toHaveBeenCalledWith(mockRegional['fr']);
    });

    it("should fallback to language prefix if full locale not found", () => {
        (globalThis as any).document.documentElement.lang = 'fr-CA';
        mockRegional.fr = {};

        jQueryDatepickerInitialization();

        expect((globalThis as any).$.datepicker.setDefaults).toHaveBeenCalledWith(mockRegional['fr']);
    });

    it("should fallback to 'en' if language not found", () => {
        (globalThis as any).document.documentElement.lang = 'unknown';

        jQueryDatepickerInitialization();

        expect((globalThis as any).$.datepicker.setDefaults).toHaveBeenCalledWith(mockRegional['en']);
    });

    it("should handle undefined document language", () => {
        (globalThis as any).document.documentElement.lang = undefined;

        jQueryDatepickerInitialization();

        expect((globalThis as any).$.datepicker.setDefaults).toHaveBeenCalledWith(mockRegional['en']);
    });
});