import { Fluent, notifyError, Tooltip, Validator } from "../base";
import { validateOptions, ValidationHelper, validatorAbortHandler } from "./validation";

// Mock the imported functions
vi.mock("../base", () => ({
    Fluent: {
        trigger: vi.fn()
    },
    Tooltip: vi.fn().mockImplementation(function() {
        this.show = vi.fn().mockReturnThis();
        this.delayedDispose = vi.fn().mockReturnThis();
    }),
    Validator: {
        getInstance: vi.fn(),
        getHighlightTarget: vi.fn()
    },
    getjQuery: vi.fn().mockReturnValue(null),
    isArrayLike: vi.fn((obj) => obj && typeof obj.length === 'number' && !obj.tagName),
    localText: vi.fn((key) => key),
    notifyError: vi.fn(),
    FormValidationTexts: {
        InvalidFormMessage: "Validation.InvalidFormMessage"
    }
}));

// Mock DOM methods
const mockAppendChild = vi.fn();
const mockAppend = vi.fn();
const mockQuerySelector = vi.fn();
const mockQuerySelectorAll = vi.fn();
const mockClosest = vi.fn();
const mockClick = vi.fn();
const mockGetAttribute = vi.fn();

Object.defineProperty(document, 'querySelector', {
    writable: true,
    value: mockQuerySelector
});

Object.defineProperty(HTMLElement.prototype, 'appendChild', {
    writable: true,
    value: mockAppendChild
});

Object.defineProperty(HTMLElement.prototype, 'append', {
    writable: true,
    value: mockAppend
});

Object.defineProperty(HTMLElement.prototype, 'querySelector', {
    writable: true,
    value: mockQuerySelector
});

Object.defineProperty(HTMLElement.prototype, 'querySelectorAll', {
    writable: true,
    value: mockQuerySelectorAll
});

Object.defineProperty(HTMLElement.prototype, 'closest', {
    writable: true,
    value: mockClosest
});

Object.defineProperty(HTMLElement.prototype, 'getAttribute', {
    writable: true,
    value: mockGetAttribute
});

Object.defineProperty(HTMLElement.prototype, 'click', {
    writable: true,
    value: mockClick
});

afterEach(() => {
    vi.clearAllMocks();
    mockAppendChild.mockClear();
    mockAppend.mockClear();
    mockQuerySelector.mockClear();
    mockQuerySelectorAll.mockClear();
    mockClosest.mockClear();
    mockClick.mockClear();
    mockGetAttribute.mockClear();
});

describe("validatorAbortHandler", () => {
    it("removes abortHandler and sets submitHandler to return false", () => {
        const validator = {
            settings: {
                abortHandler: () => {},
                submitHandler: () => {}
            }
        };

        validatorAbortHandler(validator as any);

        expect(validator.settings.abortHandler).toBeUndefined();
        expect(validator.settings.submitHandler).toBeInstanceOf(Function);
        expect(validator.settings.submitHandler()).toBe(false);
    });
});

describe("validateOptions", () => {
    it("returns default options when no options provided", () => {
        const result = validateOptions();

        expect(result.errorPlacement).toBeInstanceOf(Function);
        expect(result.submitHandler).toBeInstanceOf(Function);
        expect(result.invalidHandler).toBeInstanceOf(Function);
        expect(result.success).toBeInstanceOf(Function);
    });

    it("merges provided options with defaults", () => {
        const customOptions = {
            rules: { test: { required: true } },
            messages: { required: "Custom message" }
        };

        const result = validateOptions(customOptions);

        expect(result.rules).toEqual({ test: { required: true } });
        expect(result.messages).toEqual({ required: "Custom message" });
        expect(result.errorPlacement).toBeInstanceOf(Function); // default still present
    });

    it("errorPlacement finds field by data-vx-id", () => {
        const mockElement = {
            getAttribute: vi.fn().mockReturnValue("test-id"),
            closest: mockClosest,
            append: mockAppend
        };
        const mockLabel = document.createElement("div");

        mockQuerySelector.mockReturnValue(mockElement);

        const result = validateOptions();
        result.errorPlacement(mockLabel, mockElement as any, {} as any);

        expect(mockElement.getAttribute).toHaveBeenCalledWith("data-vx-id");
        expect(mockQuerySelector).toHaveBeenCalledWith("#test-id");
        expect(mockAppend).toHaveBeenCalledWith(mockLabel);
    });

    it("errorPlacement finds field by closest div.field", () => {
        const mockElement = { 
            getAttribute: vi.fn().mockReturnValue(null),
            closest: mockClosest
        };
        const mockField = { 
            querySelector: vi.fn().mockReturnValue(null), 
            append: mockAppend 
        };
        const mockLabel = document.createElement("div");

        mockQuerySelector.mockReturnValue(null);
        mockClosest.mockReturnValue(mockField);

        const result = validateOptions();
        result.errorPlacement(mockLabel, mockElement as any, {} as any);

        expect(mockClosest).toHaveBeenCalledWith("div.field");
        expect(mockField.querySelector).toHaveBeenCalledWith("div.vx");
        expect(mockAppend).toHaveBeenCalledWith(mockLabel);
    });

    it("success adds checked class to label", () => {
        const mockLabel = { classList: { add: vi.fn() } };

        const result = validateOptions();
        (result.success as any)(mockLabel);

        expect(mockLabel.classList.add).toHaveBeenCalledWith("checked");
    });

    it("submitHandler returns false", () => {
        const mockForm = document.createElement("form");
        const mockEvent = new Event("submit");
        const mockValidator = {} as any;

        const result = validateOptions();
        const returnValue = result.submitHandler(mockForm, mockEvent, mockValidator);

        expect(returnValue).toBe(false);
    });

    it("invalidHandler calls notifyError with localized message", () => {
        const mockValidator = {
            errorList: []
        };

        const result = validateOptions();
        result.invalidHandler({} as any, mockValidator as any);

        expect(notifyError).toHaveBeenCalledWith("Validation.InvalidFormMessage");
    });

    it("invalidHandler clicks collapsed category titles for each error", () => {
        const mockCategory = {
            querySelectorAll: vi.fn().mockReturnValue([
                { click: mockClick }
            ])
        };

        const mockElement = {
            closest: vi.fn((selector: string) => {
                if (selector === '.category.collapsed') {
                    return mockCategory;
                }
                return null; // no tab pane
            })
        };

        const mockValidator = {
            errorList: [
                { element: mockElement }
            ]
        };

        const result = validateOptions();
        result.invalidHandler({} as any, mockValidator as any);

        expect(mockElement.closest).toHaveBeenCalledWith('.category.collapsed');
        expect(mockClick).toHaveBeenCalled();
    });

    it("invalidHandler shows tooltip for first error", () => {
        const mockElement = {
            closest: vi.fn().mockReturnValue(null), // no collapsed category
            getAttribute: vi.fn().mockReturnValue(null) // no bs pane
        };

        const mockValidator = {
            errorList: [
                { element: mockElement, message: "Test error" }
            ]
        };

        const result = validateOptions();
        result.invalidHandler({} as any, mockValidator as any);

        expect(Tooltip).toHaveBeenCalledWith(mockElement, { title: "Test error" });
    });

    it("invalidHandler activates Bootstrap tab for first error", () => {
        const mockPane = {
            getAttribute: vi.fn().mockReturnValue("pane1")
        };

        const mockElement = {
            closest: vi.fn()
                .mockReturnValueOnce(null) // no collapsed category
                .mockReturnValueOnce(mockPane) // bs pane
        };

        const mockValidator = {
            errorList: [
                { element: mockElement, message: "Test error" }
            ]
        };

        mockQuerySelector.mockReturnValue({ click: mockClick });

        const result = validateOptions();
        result.invalidHandler({} as any, mockValidator as any);

        expect(mockElement.closest).toHaveBeenCalledWith('.tab-content>.tab-pane[id]:not(.active)');
        expect(mockPane.getAttribute).toHaveBeenCalledWith('id');
        expect(mockQuerySelector).toHaveBeenCalledWith('a[href="#pane1"]');
        expect(mockClick).toHaveBeenCalled();
    });

    it("invalidHandler activates jQuery UI tab for first error", () => {
        const mockUiPane = {
            getAttribute: vi.fn().mockReturnValue("ui-tab1")
        };

        const mockElement = {
            closest: vi.fn()
                .mockReturnValueOnce(null) // no collapsed category
                .mockReturnValueOnce(null) // no bs pane
                .mockReturnValue(mockUiPane) // ui pane
        };

        const mockValidator = {
            errorList: [
                { element: mockElement, message: "Test error" }
            ]
        };

        mockQuerySelector.mockReturnValue({ click: mockClick });

        const result = validateOptions();
        result.invalidHandler({} as any, mockValidator as any);

        expect(mockElement.closest).toHaveBeenCalledWith('.ui-tabs-panel[id]:not(.ui-tabs-panel-active)');
        expect(mockUiPane.getAttribute).toHaveBeenCalledWith('id');
        expect(mockQuerySelector).toHaveBeenCalledWith('a[href="#ui-tab1"]');
        expect(mockClick).toHaveBeenCalled();
    });
});

describe("ValidationHelper", () => {
    describe("asyncSubmit", () => {
        let mockValidator: any;
        let mockForm: any;
        let validateBeforeSave: any;
        let submitHandler: any;

        beforeEach(() => {
            mockValidator = {
                settings: {
                    abortHandler: undefined,
                    submitHandler: undefined
                }
            };
            mockForm = document.createElement("form");
            validateBeforeSave = vi.fn().mockReturnValue(true);
            submitHandler = vi.fn();

            (Validator.getInstance as any).mockReturnValue(mockValidator);
        });

        it("returns false if abortHandler exists", () => {
            mockValidator.settings.abortHandler = () => {};

            const result = ValidationHelper.asyncSubmit(mockForm, validateBeforeSave, submitHandler);

            expect(result).toBe(false);
        });

        it("returns false if validateBeforeSave returns false", () => {
            validateBeforeSave.mockReturnValue(false);

            const result = ValidationHelper.asyncSubmit(mockForm, validateBeforeSave, submitHandler);

            expect(validateBeforeSave).toHaveBeenCalled();
            expect(result).toBe(false);
        });

        it("sets up handlers and triggers submit", () => {
            const result = ValidationHelper.asyncSubmit(mockForm, validateBeforeSave, submitHandler);

            expect(result).toBe(true);
            expect(mockValidator.settings.abortHandler).toBe(validatorAbortHandler);
            expect(typeof mockValidator.settings.submitHandler).toBe("function");
            expect(Fluent.trigger).toHaveBeenCalledWith(mockForm, "submit");
        });

        it("submitHandler calls provided submitHandler", () => {
            ValidationHelper.asyncSubmit(mockForm, validateBeforeSave, submitHandler);

            mockValidator.settings.submitHandler();

            expect(submitHandler).toHaveBeenCalled();
        });
    });

    describe("submit", () => {
        let mockValidator: any;
        let mockForm: any;
        let validateBeforeSave: any;
        let submitHandler: any;

        beforeEach(() => {
            mockValidator = {
                settings: {
                    abortHandler: undefined
                },
                form: vi.fn().mockReturnValue(true)
            };
            mockForm = document.createElement("form");
            validateBeforeSave = vi.fn().mockReturnValue(true);
            submitHandler = vi.fn();

            (Validator.getInstance as any).mockReturnValue(mockValidator);
        });

        it("returns false if abortHandler exists", () => {
            mockValidator.settings.abortHandler = () => {};

            const result = ValidationHelper.submit(mockForm, validateBeforeSave, submitHandler);

            expect(result).toBe(false);
        });

        it("returns false if validateBeforeSave returns false", () => {
            validateBeforeSave.mockReturnValue(false);

            const result = ValidationHelper.submit(mockForm, validateBeforeSave, submitHandler);

            expect(validateBeforeSave).toHaveBeenCalled();
            expect(result).toBe(false);
        });

        it("returns false if validator.form() returns false", () => {
            mockValidator.form.mockReturnValue(false);

            const result = ValidationHelper.submit(mockForm, validateBeforeSave, submitHandler);

            expect(mockValidator.form).toHaveBeenCalled();
            expect(result).toBe(false);
        });

        it("calls submitHandler and returns true on success", () => {
            const result = ValidationHelper.submit(mockForm, validateBeforeSave, submitHandler);

            expect(validateBeforeSave).toHaveBeenCalled();
            expect(mockValidator.form).toHaveBeenCalled();
            expect(submitHandler).toHaveBeenCalled();
            expect(result).toBe(true);
        });
    });

    describe("getValidator", () => {
        it("returns validator instance from Validator.getInstance", () => {
            const mockElement = document.createElement("input");
            const mockValidator = {};

            (Validator.getInstance as any).mockReturnValue(mockValidator);

            const result = ValidationHelper.getValidator(mockElement);

            expect(Validator.getInstance).toHaveBeenCalledWith(mockElement);
            expect(result).toBe(mockValidator);
        });
    });

    describe("validateElement", () => {
        it("calls validator.element with the element", () => {
            const mockElement = document.createElement("input");
            const mockValidator = {
                element: vi.fn()
            };

            (Validator.getInstance as any).mockReturnValue(mockValidator);

            ValidationHelper.validateElement(mockElement);

            expect(Validator.getInstance).toHaveBeenCalledWith(mockElement);
            expect(mockValidator.element).toHaveBeenCalledWith(mockElement);
        });
    });
});