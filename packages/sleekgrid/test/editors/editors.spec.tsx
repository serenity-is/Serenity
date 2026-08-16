import { EventEmitter, type EditorOptions } from "../../src/core";
import {
    CheckboxCellEdit,
    DateCellEdit,
    FloatCellEdit,
    IntegerCellEdit,
    LongTextCellEdit,
    PercentCompleteCellEdit,
    TextCellEdit,
    YesNoSelectCellEdit
} from "../../src/editors/editors";
import { Editors } from "../../src/editors";

// ---------------------------------------------------------------------------
// helpers
// ---------------------------------------------------------------------------

function makeGrid(overrides: any = {}) {
    return {
        getActiveCell: () => ({ row: 0, cell: 0 }),
        navigateNext: vi.fn(() => true),
        navigatePrev: vi.fn(() => true),
        onCompositeEditorChange: new EventEmitter<any, any>(),
        getEditorFactory: () => null,
        ...overrides
    };
}

function makeArgs(overrides: any = {}): EditorOptions {
    const container = document.createElement("div");
    return {
        container,
        column: { field: "field1" },
        grid: makeGrid(),
        item: { field1: "initial" },
        commitChanges: vi.fn(),
        cancelChanges: vi.fn(),
        editorCellNavOnLRKeys: false,
        ...overrides
    } as unknown as EditorOptions;
}

/** Minimal fake jQuery that supports datepicker / slider / find().on() */
function installFakeJQuery() {
    const dpDiv = {
        stop: vi.fn(() => dpDiv),
        show: vi.fn(() => dpDiv),
        hide: vi.fn(() => dpDiv),
        css: vi.fn(() => dpDiv)
    };
    const calls: {
        datepickerOpts?: any;
        sliderOpts?: any;
        sliderValues: any[];
        buttonHandlers: ((e: any) => void)[];
    } = {
        sliderValues: [],
        buttonHandlers: []
    };

    const jq = (el: any) => ({
        datepicker: (arg?: any) => {
            if (arg && typeof arg === "object")
                calls.datepickerOpts = arg;
            return jq(el);
        },
        slider: (arg?: any, value?: any) => {
            if (arg && typeof arg === "object")
                calls.sliderOpts = arg;
            else if (arg === "value")
                calls.sliderValues.push(value);
            return jq(el);
        },
        find: () => ({
            on: (_evt: string, handler: any) => {
                calls.buttonHandlers.push(handler);
                return jq(el);
            }
        })
    });

    const $ = ((el: any) => jq(el)) as any;
    $.datepicker = { dpDiv };
    (globalThis as any).$ = $;
    return { calls, dpDiv };
}

function fireKey(el: EventTarget, key: string, init: KeyboardEventInit = {}) {
    el.dispatchEvent(new KeyboardEvent("keydown", { key, bubbles: true, cancelable: true, ...init }));
}

// ---------------------------------------------------------------------------
// BaseCellEdit / TextCellEdit
// ---------------------------------------------------------------------------

describe('TextCellEdit', () => {
    it('creates a text input with the expected classes and focuses it', () => {
        const args = makeArgs();
        const editor = new TextCellEdit(args);
        const input = editor._input;
        expect(input.tagName).toBe("INPUT");
        expect(input.type).toBe("text");
        expect(input.classList.contains("editor-text")).toBe(true);
        expect(input.classList.contains("slick-editor-text")).toBe(true);
        expect(args.container.contains(input)).toBe(true);
    });

    it('getValue / setValue round-trip the input value', () => {
        const editor = new TextCellEdit(makeArgs());
        editor.setValue("hello");
        expect(editor.getValue()).toBe("hello");
        editor.setValue(null as any);
        expect(editor.getValue()).toBe("");
    });

    it('focus() focuses the input', () => {
        const args = makeArgs();
        document.body.appendChild(args.container);
        try {
            const editor = new TextCellEdit(args);
            editor.focus();
            expect(document.activeElement).toBe(editor._input);
        } finally {
            document.body.removeChild(args.container);
        }
    });

    it('destroy() removes the input', () => {
        const args = makeArgs();
        const editor = new TextCellEdit(args);
        editor.destroy();
        expect(args.container.contains(editor._input)).toBe(false);
    });

    it('loadValue sets the input value and default value', () => {
        const editor = new TextCellEdit(makeArgs({ item: { field1: "abc" } }));
        editor.loadValue({ field1: "abc" });
        expect((editor as any)._defaultValue).toBe("abc");
        expect(editor._input.value).toBe("abc");
        expect((editor._input as any).defaultValue).toBe("abc");
    });

    it('loadValue treats a missing value as empty string', () => {
        const editor = new TextCellEdit(makeArgs());
        editor.loadValue({ field1: null });
        expect((editor as any)._defaultValue).toBe("");
        expect(editor._input.value).toBe("");
    });

    it('serializeValue returns the input value', () => {
        const editor = new TextCellEdit(makeArgs());
        editor.setValue("xyz");
        expect(editor.serializeValue()).toBe("xyz");
    });

    it('applyValue writes the value onto the item', () => {
        const editor = new TextCellEdit(makeArgs());
        const item: any = {};
        editor.applyValue(item, "applied");
        expect(item.field1).toBe("applied");
    });

    it('isValueChanged detects changes', () => {
        const editor = new TextCellEdit(makeArgs());
        editor.loadValue({ field1: "abc" });
        expect(editor.isValueChanged()).toBe(false);
        editor.setValue("def");
        expect(editor.isValueChanged()).toBe(true);
    });

    it('isValueChanged returns false when value is empty and default is null', () => {
        const editor = new TextCellEdit(makeArgs());
        editor.loadValue({ field1: null });
        expect(editor.isValueChanged()).toBe(false);
    });

    it('validate returns the validator result when invalid', () => {
        const validator = vi.fn(() => ({ valid: false, msg: "nope" }));
        const editor = new TextCellEdit(makeArgs({ column: { field: "field1", validator } }));
        const result = editor.validate();
        expect(result.valid).toBe(false);
        expect(result.msg).toBe("nope");
        expect(validator).toHaveBeenCalledWith(editor._input.value, (editor as any)._args);
    });

    it('validate returns valid when no validator is set', () => {
        const editor = new TextCellEdit(makeArgs());
        expect(editor.validate()).toEqual({ valid: true, msg: null });
    });

    it('adds a keydown listener that navigates when editorCellNavOnLRKeys is set', () => {
        const args = makeArgs({ editorCellNavOnLRKeys: true });
        const editor = new TextCellEdit(args);
        const input = editor._input;
        input.value = "abc";
        input.selectionStart = 1;
        let later = 0;
        input.addEventListener("keydown", () => later++);
        fireKey(input, "Left");       // cursor > 0 -> stopImmediatePropagation
        expect(later).toBe(0);
        input.selectionStart = 0;
        fireKey(input, "Left");       // cursor == 0 -> no stop
        expect(later).toBe(1);
    });

    it('does not navigate on arrow keys when editorCellNavOnLRKeys is not set', () => {
        const args = makeArgs({ editorCellNavOnLRKeys: false });
        const editor = new TextCellEdit(args);
        const input = editor._input;
        input.value = "abc";
        input.selectionStart = 1;
        let later = 0;
        input.addEventListener("keydown", () => later++);
        fireKey(input, "Left");
        expect(later).toBe(0);
        fireKey(input, "Right");
        expect(later).toBe(0);
        fireKey(input, "ArrowRight");
        expect(later).toBe(0);
    });

    it('only stops Left/Right (not other keys) when LR nav is disabled', () => {
        const editor = new TextCellEdit(makeArgs({ editorCellNavOnLRKeys: false }));
        const input = editor._input;
        let later = 0;
        input.addEventListener("keydown", () => later++);
        fireKey(input, "Tab");
        expect(later).toBe(1);
    });
});

// ---------------------------------------------------------------------------
// IntegerCellEdit
// ---------------------------------------------------------------------------

describe('IntegerCellEdit', () => {
    it('serializes to an integer, defaulting to 0', () => {
        const editor = new IntegerCellEdit(makeArgs());
        editor.setValue("42");
        expect(editor.serializeValue()).toBe(42);
        editor.setValue("abc");
        expect(editor.serializeValue()).toBe(0);
        editor.setValue("");
        expect(editor.serializeValue()).toBe(0);
    });

    it('rejects non-integers in validate', () => {
        const editor = new IntegerCellEdit(makeArgs());
        editor.setValue("abc");
        expect(editor.validate().valid).toBe(false);
        expect(editor.validate().msg).toBe("Please enter a valid integer");
    });

    it('accepts valid integers in validate and delegates to the column validator', () => {
        const validator = vi.fn(() => ({ valid: false, msg: "custom" }));
        const editor = new IntegerCellEdit(makeArgs({ column: { field: "field1", validator } }));
        editor.setValue("12");
        const result = editor.validate();
        expect(result).toEqual({ valid: false, msg: "custom" });
    });
});

// ---------------------------------------------------------------------------
// FloatCellEdit
// ---------------------------------------------------------------------------

describe('FloatCellEdit', () => {
    afterEach(() => {
        FloatCellEdit.AllowEmptyValue = false;
        FloatCellEdit.DefaultDecimalPlaces = null as any;
    });

    it('serializes to a float, defaulting to 0', () => {
        const editor = new FloatCellEdit(makeArgs());
        editor.setValue("3.14");
        expect(editor.serializeValue()).toBe(3.14);
        editor.setValue("abc");
        expect(editor.serializeValue()).toBe(0);
    });

    it('returns empty string for empty values when AllowEmptyValue is set', () => {
        FloatCellEdit.AllowEmptyValue = true;
        const editor = new FloatCellEdit(makeArgs());
        editor.setValue("");
        expect(editor.serializeValue()).toBe("");
        editor.setValue("2.5");
        expect(editor.serializeValue()).toBe(2.5);
    });

    it('getDecimalPlaces falls back to the static DefaultDecimalPlaces', () => {
        const editor = new FloatCellEdit(makeArgs());
        FloatCellEdit.DefaultDecimalPlaces = 3;
        expect(editor.getDecimalPlaces()).toBe(3);
        FloatCellEdit.DefaultDecimalPlaces = null as any;
        expect(editor.getDecimalPlaces()).toBeNull();
    });

    it('getDecimalPlaces uses the column editorFixedDecimalPlaces when defined', () => {
        const editor = new FloatCellEdit(makeArgs({ column: { field: "field1", editorFixedDecimalPlaces: 2 } }));
        expect(editor.getDecimalPlaces()).toBe(2);
    });

    it('getDecimalPlaces returns 0 when explicitly zero', () => {
        const editor = new FloatCellEdit(makeArgs({ column: { field: "field1", editorFixedDecimalPlaces: 0 } }));
        expect(editor.getDecimalPlaces()).toBe(0);
    });

    it('loadValue formats to fixed decimals when requested', () => {
        const editor = new FloatCellEdit(makeArgs({ column: { field: "field1", editorFixedDecimalPlaces: 2 } }));
        editor.loadValue({ field1: 5 });
        expect((editor as any)._defaultValue).toBe("5.00");
        expect(editor._input.value).toBe("5.00");
    });

    it('loadValue keeps zero as zero when decimals are requested', () => {
        const editor = new FloatCellEdit(makeArgs({ column: { field: "field1", editorFixedDecimalPlaces: 2 } }));
        editor.loadValue({ field1: 0 });
        expect(editor._input.value).toBe("0.00");
    });

    it('loadValue treats null as empty string (bug fix)', () => {
        const editor = new FloatCellEdit(makeArgs({ column: { field: "field1", editorFixedDecimalPlaces: 2 } }));
        editor.loadValue({ field1: null });
        expect((editor as any)._defaultValue).toBe("");
        expect(editor._input.value).toBe("");
    });

    it('serializeValue respects decimal places', () => {
        const editor = new FloatCellEdit(makeArgs({ column: { field: "field1", editorFixedDecimalPlaces: 1 } }));
        editor.setValue("3.14159");
        expect(editor.serializeValue()).toBe(3.1);
    });

    it('rejects non-numbers in validate', () => {
        const editor = new FloatCellEdit(makeArgs());
        editor.setValue("abc");
        expect(editor.validate().valid).toBe(false);
        expect(editor.validate().msg).toBe("Please enter a valid number");
    });

    it('accepts valid numbers in validate and delegates to the column validator', () => {
        const validator = vi.fn(() => ({ valid: false, msg: "custom" }));
        const editor = new FloatCellEdit(makeArgs({ column: { field: "field1", validator } }));
        editor.setValue("3.14");
        expect(editor.validate()).toEqual({ valid: false, msg: "custom" });
        expect(validator).toHaveBeenCalledWith("3.14", (editor as any)._args);
    });
});

// ---------------------------------------------------------------------------
// DateCellEdit (requires a fake jQuery datepicker)
// ---------------------------------------------------------------------------

describe('DateCellEdit', () => {
    let fake: ReturnType<typeof installFakeJQuery>;
    beforeEach(() => {
        fake = installFakeJQuery();
    });
    afterEach(() => {
        delete (globalThis as any).$;
    });

    it('initializes a datepicker on the input and sizes the input', () => {
        const args = makeArgs();
        const origGetComputedStyle = window.getComputedStyle;
        // jsdom reports 0px widths for detached inputs; emulate a real rendered width
        window.getComputedStyle = vi.fn(() => ({ width: "120px" })) as any;
        try {
            const editor = new DateCellEdit(args);
            expect(fake.calls.datepickerOpts).toBeTruthy();
            expect(typeof fake.calls.datepickerOpts.beforeShow).toBe("function");
            expect(typeof fake.calls.datepickerOpts.onClose).toBe("function");
            expect(editor._input.style.width).toBe("102px"); // 120 - 18
        } finally {
            window.getComputedStyle = origGetComputedStyle;
        }
    });

    it('show/hide/position are no-ops while the calendar is closed', () => {
        const editor = new DateCellEdit(makeArgs());
        editor.show();
        editor.hide();
        editor.position({ top: 10, left: 20 });
        expect(fake.dpDiv.show).not.toHaveBeenCalled();
        expect(fake.dpDiv.hide).not.toHaveBeenCalled();
        expect(fake.dpDiv.css).not.toHaveBeenCalled();
    });

    it('show/hide/position act on the datepicker div once the calendar is open', () => {
        const editor = new DateCellEdit(makeArgs());
        fake.calls.datepickerOpts!.beforeShow();
        editor.show();
        expect(fake.dpDiv.show).toHaveBeenCalled();
        editor.hide();
        expect(fake.dpDiv.hide).toHaveBeenCalled();
        editor.position({ top: 100, left: 200 });
        expect(fake.dpDiv.css).toHaveBeenCalledWith("top", 130);
        expect(fake.dpDiv.css).toHaveBeenCalledWith("left", 200);
    });

    it('onClose closes the calendar so subsequent show is a no-op', () => {
        const editor = new DateCellEdit(makeArgs());
        fake.calls.datepickerOpts!.beforeShow();
        fake.calls.datepickerOpts!.onClose();
        editor.show();
        expect(fake.dpDiv.show).not.toHaveBeenCalled();
    });

    it('onClose triggers a composite change when part of a composite editor', () => {
        const onCompositeEditorChange = new EventEmitter<any, any>();
        const grid = makeGrid({ onCompositeEditorChange });
        const formValues: any = {};
        const item: any = { field1: "2020-01-01" };
        const args = makeArgs({
            grid,
            item,
            compositeEditorOptions: { formValues }
        });
        const editor = new DateCellEdit(args);
        const notifySpy = vi.spyOn(onCompositeEditorChange, "notify");
        fake.calls.datepickerOpts!.onClose();
        expect(notifySpy).toHaveBeenCalled();
    });

    it('destroy stops and destroys the datepicker and removes the input', () => {
        const args = makeArgs();
        const editor = new DateCellEdit(args);
        editor.destroy();
        expect(fake.dpDiv.stop).toHaveBeenCalledWith(true, true);
        expect(args.container.contains(editor._input)).toBe(false);
    });
});

// ---------------------------------------------------------------------------
// YesNoSelectCellEdit
// ---------------------------------------------------------------------------

describe('YesNoSelectCellEdit', () => {
    it('creates a select with Yes/No options', () => {
        const editor = new YesNoSelectCellEdit(makeArgs());
        expect(editor._input.tagName).toBe("SELECT");
        expect(editor._input.querySelectorAll("option").length).toBe(2);
        expect((editor._input.querySelectorAll("option")[0] as HTMLOptionElement).value).toBe("yes");
        expect((editor._input.querySelectorAll("option")[1] as HTMLOptionElement).value).toBe("no");
    });

    it('loadValue maps truthy values to "yes" and falsy to "no"', () => {
        const editor = new YesNoSelectCellEdit(makeArgs());
        editor.loadValue({ field1: true });
        expect(editor._input.value).toBe("yes");
        editor.loadValue({ field1: false });
        expect(editor._input.value).toBe("no");
    });

    it('serializeValue returns a boolean', () => {
        const editor = new YesNoSelectCellEdit(makeArgs());
        editor.loadValue({ field1: true });
        expect(editor.serializeValue()).toBe(true);
        editor.loadValue({ field1: false });
        expect(editor.serializeValue()).toBe(false);
    });

    it('isValueChanged is false right after loadValue (bug fix)', () => {
        const editor = new YesNoSelectCellEdit(makeArgs());
        editor.loadValue({ field1: true });
        expect(editor.isValueChanged()).toBe(false);
        editor.loadValue({ field1: false });
        expect(editor.isValueChanged()).toBe(false);
    });

    it('isValueChanged is true after the user changes the selection', () => {
        const editor = new YesNoSelectCellEdit(makeArgs());
        editor.loadValue({ field1: true });
        editor._input.value = "no";
        expect(editor.isValueChanged()).toBe(true);
        editor._input.value = "yes";
        expect(editor.isValueChanged()).toBe(false);
    });

    it('validate always returns valid', () => {
        const editor = new YesNoSelectCellEdit(makeArgs());
        expect(editor.validate()).toEqual({ valid: true, msg: null });
    });
});

// ---------------------------------------------------------------------------
// CheckboxCellEdit
// ---------------------------------------------------------------------------

describe('CheckboxCellEdit', () => {
    it('creates a checkbox input', () => {
        const editor = new CheckboxCellEdit(makeArgs());
        expect(editor._input.type).toBe("checkbox");
        expect(editor._input.classList.contains("editor-checkbox")).toBe(true);
        expect(editor._input.classList.contains("slick-editor-checkbox")).toBe(true);
    });

    it('loadValue sets the checked state from the item', () => {
        const editor = new CheckboxCellEdit(makeArgs());
        editor.loadValue({ field1: true });
        expect(editor._input.checked).toBe(true);
        expect((editor as any)._defaultValue).toBe(true);
        editor.loadValue({ field1: 0 });
        expect(editor._input.checked).toBe(false);
    });

    it('preClick toggles the checked state', () => {
        const editor = new CheckboxCellEdit(makeArgs());
        editor.loadValue({ field1: false });
        editor.preClick();
        expect(editor._input.checked).toBe(true);
        editor.preClick();
        expect(editor._input.checked).toBe(false);
    });

    it('serializeValue returns the checked state', () => {
        const editor = new CheckboxCellEdit(makeArgs());
        editor._input.checked = true;
        expect(editor.serializeValue()).toBe(true);
    });

    it('applyValue writes the value onto the item', () => {
        const editor = new CheckboxCellEdit(makeArgs());
        const item: any = {};
        editor.applyValue(item, true);
        expect(item.field1).toBe(true);
    });

    it('isValueChanged detects changes', () => {
        const editor = new CheckboxCellEdit(makeArgs());
        editor.loadValue({ field1: false });
        expect(editor.isValueChanged()).toBe(false);
        editor._input.checked = true;
        expect(editor.isValueChanged()).toBe(true);
    });

    it('validate always returns valid', () => {
        const editor = new CheckboxCellEdit(makeArgs());
        expect(editor.validate()).toEqual({ valid: true, msg: null });
    });
});

// ---------------------------------------------------------------------------
// PercentCompleteCellEdit (requires a fake jQuery slider)
// ---------------------------------------------------------------------------

describe('PercentCompleteCellEdit', () => {
    let fake: ReturnType<typeof installFakeJQuery>;
    beforeEach(() => {
        fake = installFakeJQuery();
    });
    afterEach(() => {
        delete (globalThis as any).$;
    });

    it('creates the percent picker and swaps the input classes', () => {
        const editor = new PercentCompleteCellEdit(makeArgs());
        expect(editor._input.classList.contains("editor-text")).toBe(false);
        expect(editor._input.classList.contains("editor-percentcomplete")).toBe(true);
        expect((editor as any)._picker.classList.contains("slick-editor-percentcomplete-picker")).toBe(true);
        expect((editor as any)._picker.querySelectorAll("button").length).toBe(3);
    });

    it('initializes the slider with a value', () => {
        const editor = new PercentCompleteCellEdit(makeArgs());
        expect(fake.calls.sliderOpts).toBeTruthy();
        expect(fake.calls.sliderOpts!.value).toBe(0);
        expect(typeof fake.calls.sliderOpts!.slide).toBe("function");
        expect(typeof fake.calls.sliderOpts!.stop).toBe("function");
    });

    it('loadValue keeps the slider in sync with the cell value (bug fix)', () => {
        const editor = new PercentCompleteCellEdit(makeArgs());
        editor.loadValue({ field1: 50 });
        expect(editor._input.value).toBe("50");
        expect(fake.calls.sliderValues).toContain(50);
    });

    it('slider slide updates the text input', () => {
        const editor = new PercentCompleteCellEdit(makeArgs());
        fake.calls.sliderOpts!.slide(null, { value: 75 });
        expect(editor._input.value).toBe("75");
    });

    it('slider stop triggers a composite change when part of a composite editor', () => {
        const onCompositeEditorChange = new EventEmitter<any, any>();
        const grid = makeGrid({ onCompositeEditorChange });
        const args = makeArgs({ grid, compositeEditorOptions: { formValues: {} } });
        const editor = new PercentCompleteCellEdit(args);
        const notifySpy = vi.spyOn(onCompositeEditorChange, "notify");
        fake.calls.sliderOpts!.stop();
        expect(notifySpy).toHaveBeenCalled();
    });

    it('button click sets the input and slider value', () => {
        const editor = new PercentCompleteCellEdit(makeArgs());
        const button = (editor as any)._picker.querySelector('button[data-val="50"]') as HTMLButtonElement;
        fake.calls.buttonHandlers[0]({ target: button });
        expect(editor._input.value).toBe("50");
        expect(fake.calls.sliderValues).toContain("50");
    });

    it('destroy removes the picker', () => {
        const args = makeArgs();
        const editor = new PercentCompleteCellEdit(args);
        editor.destroy();
        expect(args.container.contains((editor as any)._picker)).toBe(false);
        expect(args.container.contains(editor._input)).toBe(false);
    });
});

// ---------------------------------------------------------------------------
// LongTextCellEdit
// ---------------------------------------------------------------------------

describe('LongTextCellEdit', () => {
    function makeLongTextArgs(overrides: any = {}) {
        const base = makeArgs({
            item: { field1: "long text" },
            position: { top: 100, left: 200 },
            ...overrides
        });
        return base;
    }

    it('renders a detached textarea on document.body when not composite', () => {
        const editor = new LongTextCellEdit(makeLongTextArgs());
        expect((editor as any)._wrapper.parentElement).toBe(document.body);
        expect((editor as any)._wrapper.querySelectorAll("button").length).toBe(2);
        expect((editor as any)._wrapper.style.top).toBe("95px");
        expect((editor as any)._wrapper.style.left).toBe("195px");
        editor.destroy();
        expect(document.body.contains((editor as any)._wrapper)).toBe(false);
    });

    it('renders inside the container when composite and adds no buttons', () => {
        const args = makeLongTextArgs({ compositeEditorOptions: { formValues: {} } });
        const editor = new LongTextCellEdit(args);
        expect((editor as any)._wrapper.parentElement).toBe(args.container);
        expect((editor as any)._wrapper.querySelectorAll("button").length).toBe(0);
    });

    it('save() commits changes', () => {
        const args = makeLongTextArgs();
        const editor = new LongTextCellEdit(args);
        editor.save();
        expect(args.commitChanges).toHaveBeenCalled();
        editor.destroy();
    });

    it('cancel() restores the default value and cancels', () => {
        const args = makeLongTextArgs();
        const editor = new LongTextCellEdit(args);
        editor.loadValue({ field1: "long text" });
        editor._input.value = "changed";
        editor.cancel();
        expect(editor._input.value).toBe("long text");
        expect(args.cancelChanges).toHaveBeenCalled();
        editor.destroy();
    });

    it('ctrl+Enter saves', () => {
        const args = makeLongTextArgs();
        const editor = new LongTextCellEdit(args);
        fireKey(editor._input, "Enter", { ctrlKey: true });
        expect(args.commitChanges).toHaveBeenCalled();
        editor.destroy();
    });

    it('Escape cancels', () => {
        const args = makeLongTextArgs();
        const editor = new LongTextCellEdit(args);
        editor.loadValue({ field1: "long text" });
        editor._input.value = "changed";
        fireKey(editor._input, "Escape");
        expect(editor._input.value).toBe("long text");
        expect(args.cancelChanges).toHaveBeenCalled();
        editor.destroy();
    });

    it('Tab navigates next and Shift+Tab navigates prev', () => {
        const args = makeLongTextArgs();
        const editor = new LongTextCellEdit(args);
        fireKey(editor._input, "Tab");
        expect(args.grid.navigateNext).toHaveBeenCalled();
        fireKey(editor._input, "Tab", { shiftKey: true });
        expect(args.grid.navigatePrev).toHaveBeenCalled();
        editor.destroy();
    });

    it('navigates on arrow keys at the edges when LR nav is enabled', () => {
        const args = makeLongTextArgs({ editorCellNavOnLRKeys: true });
        const editor = new LongTextCellEdit(args);
        editor._input.value = "abcd";
        editor._input.selectionStart = 0;
        fireKey(editor._input, "ArrowLeft");
        expect(args.grid.navigatePrev).toHaveBeenCalled();
        editor._input.selectionStart = 3;
        fireKey(editor._input, "ArrowRight");
        expect(args.grid.navigateNext).toHaveBeenCalled();
        editor.destroy();
    });

    it('does not navigate on arrow keys when LR nav is disabled', () => {
        const args = makeLongTextArgs({ editorCellNavOnLRKeys: false });
        const editor = new LongTextCellEdit(args);
        editor._input.value = "abcd";
        editor._input.selectionStart = 0;
        fireKey(editor._input, "ArrowLeft");
        fireKey(editor._input, "ArrowRight");
        expect(args.grid.navigatePrev).not.toHaveBeenCalled();
        expect(args.grid.navigateNext).not.toHaveBeenCalled();
        editor.destroy();
    });

    it('hide() and show() toggle the wrapper visibility', () => {
        const editor = new LongTextCellEdit(makeLongTextArgs());
        editor.hide();
        expect((editor as any)._wrapper.hidden).toBe(true);
        editor.show();
        expect((editor as any)._wrapper.hidden).toBe(false);
        editor.destroy();
    });
});

// ---------------------------------------------------------------------------
// Composite editor helpers (via TextCellEdit)
// ---------------------------------------------------------------------------

describe('composite editor change flow', () => {
    it('does not attach a change listener when not composite', () => {
        const editor = new TextCellEdit(makeArgs());
        const input = editor._input;
        let notifyCount = 0;
        (editor as any)._args.grid.onCompositeEditorChange.subscribe(() => notifyCount++);
        input.dispatchEvent(new Event("change"));
        expect(notifyCount).toBe(0);
    });

    it('triggers a composite change on input change when composite', () => {
        const onCompositeEditorChange = new EventEmitter<any, any>();
        const formValues: any = {};
        const item: any = { field1: "initial" };
        const grid = makeGrid({ onCompositeEditorChange });
        const args = makeArgs({ grid, item, compositeEditorOptions: { formValues } });

        let payload: any;
        onCompositeEditorChange.subscribe((_e: any, p: any) => (payload = p));

        const editor = new TextCellEdit(args);
        editor.setValue("updated");
        editor._input.dispatchEvent(new Event("change"));

        expect(payload).toBeTruthy();
        expect(payload.row).toBe(0);
        expect(payload.cell).toBe(0);
        expect(payload.item).toBe(item);
        expect(payload.formValues).toBe(formValues);
        expect(item.field1).toBe("updated");           // valid -> applied to item
        expect(formValues.field1).toBe("updated");     // always applied to formValues
    });

    it('still writes to formValues when validation fails', () => {
        const validator = vi.fn(() => ({ valid: false, msg: "bad" }));
        const onCompositeEditorChange = new EventEmitter<any, any>();
        const formValues: any = {};
        const item: any = { field1: "initial" };
        const grid = makeGrid({ onCompositeEditorChange });
        const args = makeArgs({
            grid,
            item,
            column: { field: "field1", validator },
            compositeEditorOptions: { formValues }
        });

        const editor = new TextCellEdit(args);
        editor.setValue("updated");
        editor._input.dispatchEvent(new Event("change"));

        expect(item.field1).toBe("initial");    // invalid -> NOT applied to item
        expect(formValues.field1).toBe("updated");  // still applied to formValues
    });
});

// ---------------------------------------------------------------------------
// Editors namespace
// ---------------------------------------------------------------------------

describe('Editors namespace', () => {
    it('exposes each editor class', () => {
        expect(Editors.Text).toBe(TextCellEdit);
        expect(Editors.Integer).toBe(IntegerCellEdit);
        expect(Editors.Float).toBe(FloatCellEdit);
        expect(Editors.Date).toBe(DateCellEdit);
        expect(Editors.YesNoSelect).toBe(YesNoSelectCellEdit);
        expect(Editors.Checkbox).toBe(CheckboxCellEdit);
        expect(Editors.PercentComplete).toBe(PercentCompleteCellEdit);
        expect(Editors.LongText).toBe(LongTextCellEdit);
    });
});
