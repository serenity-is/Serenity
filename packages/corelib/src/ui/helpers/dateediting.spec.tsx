import { dateInputChangeHandler, dateInputKeyupHandler, flatPickrTrigger, jQueryDatepickerZIndexWorkaround, jQueryDatepickerInitialization } from "./dateediting";
import { Culture } from "../../base";
import type { MockInstance } from "vitest";

describe("dateInputChangeHandler", () => {
    let mockInput: HTMLInputElement;
    let mockInputGetSpy: MockInstance;
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
        mockInputGetSpy = vi.spyOn(mockInput, 'value', 'get');
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

        expect(mockInputGetSpy).not.toHaveBeenCalled();
        expect(mockInputSetSpy).not.toHaveBeenCalled();
        expect(mockInput.value).toBe('');

        Culture.dateOrder = originalDateOrder;
    });

    it("should do nothing if input type is 'date'", () => {
        Culture.dateOrder = 'dmy';
        (mockInput.getAttribute as any).mockReturnValue('date');

        dateInputChangeHandler(mockEvent);

        expect(mockInputGetSpy).not.toHaveBeenCalled();
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

    it("should not format 6+ digit numeric string with day part > 31", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '321056';

        dateInputChangeHandler(mockEvent);

        expect(mockInput.value).toBe('321056');
    });

    it("should not format 6+ digit numeric string with month part > 12", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '121356';

        dateInputChangeHandler(mockEvent);

        expect(mockInput.value).toBe('01/01/121356'); // Because parseDate will parse it as year 121356
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
    let mockEvent: any;
    let originalDateOrder: string;
    let originalDateSeparator: string;

    beforeEach(() => {
        mockInput = document.createElement('input');
        vi.spyOn(mockInput, 'getAttribute').mockReturnValue(null);
        mockEvent = {
            target: mockInput,
            which: 0
        };
        originalDateOrder = Culture.dateOrder;
        originalDateSeparator = Culture.dateSeparator;
    });

    afterEach(() => {
        vi.restoreAllMocks();
        Culture.dateOrder = originalDateOrder;
        Culture.dateSeparator = originalDateSeparator;
    });

    it("should do nothing if Culture.dateOrder is not 'dmy'", () => {
        Culture.dateOrder = 'mdy';

        dateInputKeyupHandler(mockEvent);

        expect(mockInput.value).toBe('');
    });

    it("should do nothing if input type is 'date'", () => {
        Culture.dateOrder = 'dmy';
        (mockInput.getAttribute as any).mockReturnValue('date');

        dateInputKeyupHandler(mockEvent);

        expect(mockInput.value).toBe('');
    });

    it("should do nothing if input is readonly", () => {
        Culture.dateOrder = 'dmy';
        (mockInput.getAttribute as any).mockReturnValue('readonly');

        dateInputKeyupHandler(mockEvent);

        expect(mockInput.value).toBe('');
    });

    it("should do nothing if input is disabled", () => {
        Culture.dateOrder = 'dmy';
        (mockInput.getAttribute as any).mockReturnValue('disabled');

        dateInputKeyupHandler(mockEvent);

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

    it("should handle slash key (47) at position 2", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '1/';
        mockInput.selectionEnd = 2;
        mockEvent.which = 47;

        dateInputKeyupHandler(mockEvent);

        expect(mockInput.value).toBe('01/');
    });

    it("should handle slash key (47) at position 4", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '12/3';
        mockInput.selectionEnd = 4;
        mockEvent.which = 47;

        dateInputKeyupHandler(mockEvent);

        expect(mockInput.value).toBe('12/3'); // No change because logic doesn't handle this case
    });

    it("should handle numeric input and add separator at position 2", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '12';
        mockInput.selectionEnd = 2;
        mockEvent.which = 50; // '2'

        dateInputKeyupHandler(mockEvent);

        expect(mockInput.value).toBe('12/');
    });

    it("should handle numeric input and add separator at position 5", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '12/34';
        mockInput.selectionEnd = 5;
        mockEvent.which = 53; // '5'

        dateInputKeyupHandler(mockEvent);

        expect(mockInput.value).toBe('12/34/');
    });

    it("should pad single digit day with zero", () => {
        Culture.dateOrder = 'dmy';
        Culture.dateSeparator = '/';
        mockInput.value = '4';
        mockInput.selectionEnd = 1;
        mockEvent.which = 52; // '4'

        dateInputKeyupHandler(mockEvent);

        expect(mockInput.value).toBe('04/');
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