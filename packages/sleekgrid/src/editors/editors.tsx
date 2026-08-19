import { bindThis } from "@serenity-is/domwise";
import { Editor, EditorOptions, parsePx, Position, type ValidationResult } from "../core";

/**
 * Base class for inline cell editors. Manages the input element, default value
 * tracking and the standard {@link Editor} contract.
 */
abstract class BaseCellEdit {
    declare protected _input: HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement;
    declare protected _defaultValue: any;
    declare protected _args: EditorOptions;

    constructor(args: EditorOptions) {
        this._args = args;
        this.init();
    }

    /** Creates and attaches the editor DOM; called by the constructor. */
    abstract init(): void;

    /** Removes the editor input from the DOM. */
    destroy(): void {
        this._input.remove();
    }

    /** Focuses the editor's input element. */
    focus(): void {
        this._input.focus();
    }

    /**
     * Reads the current input value as a string.
     * @returns Raw string value from the input.
     */
    getValue(): string {
        return this._input.value;
    }

    /**
     * Writes a string value into the input.
     * @param val - Value to set; `null`/`undefined` becomes empty string.
     */
    setValue(val: string): void {
        this._input.value = val ?? '';
    }

    /**
     * Loads the item's field value into the editor and selects it.
     * @param item - Row data item whose field is being edited.
     */
    loadValue(item: any): void {
        this._defaultValue = item[this._args.column.field] ?? "";
        this._input.value = this._defaultValue;
        if ((this._input as any).select) {
            (this._input as any).defaultValue = this._defaultValue;
            (this._input as any).select();
        }
    }

    /**
     * Serializes the current input value for commit.
     * @returns String content of the input.
     */
    serializeValue(): any {
        return this._input.value;
    }

    /**
     * Writes the serialized value back to the data item's field.
     * @param item - Row data item to mutate.
     * @param state - Value returned by {@link BaseCellEdit.serializeValue}.
     */
    applyValue(item: any, state: any): void {
        item[this._args.column.field] = state;
    }

    /**
     * Tests whether the editor content differs from the loaded default.
     * @returns `true` if the value has changed.
     */
    isValueChanged(): boolean {
        return (!(this._input.value === "" && this._defaultValue == null)) && (this._input.value != this._defaultValue);
    }

    /**
     * Validates the current value using the column's `validator`, if any.
     * @returns Validation result; always valid when no validator is configured.
     */
    validate(): ValidationResult {
        if (this._args.column.validator) {
            var validationResults = this._args.column.validator(this._input.value, this._args);
            if (!validationResults.valid) {
                return validationResults;
            }
        }

        return {
            valid: true,
            msg: null
        };
    }
}

/**
 * Text editor backed by an `<input type="text">`. Honors `editorCellNavOnLRKeys`
 * for arrow-key navigation between cells.
 */
export class TextCellEdit extends BaseCellEdit {

    declare _input: HTMLInputElement;

    init(): void {
        const input = this._input = this._args.container.appendChild(<input type="text" class='editor-text slick-editor-text' /> as HTMLInputElement);
        input.addEventListener('keydown', this._args.editorCellNavOnLRKeys ? handleKeydownLRNav : handleKeydownLRNoNav);
        input.focus();
        input.select();

        addCompositeChangeListener(this, this._args, input);
    }
}

/**
 * Integer editor extending {@link TextCellEdit}. Serializes to `number` and
 * validates that the input is a valid integer.
 */
export class IntegerCellEdit extends TextCellEdit {

    serializeValue(): any {
        return parseInt(this._input.value, 10) || 0;
    }

    validate(): ValidationResult {
        if (isNaN(parseInt(this._input.value, 10))) {
            return {
                valid: false,
                msg: "Please enter a valid integer"
            };
        }

        return super.validate();
    }
}

/**
 * Float/decimal editor extending {@link TextCellEdit}. Supports fixed decimal
 * places via `column.editorFixedDecimalPlaces` or {@link FloatCellEdit.DefaultDecimalPlaces}.
 */
export class FloatCellEdit extends TextCellEdit {

    /** When `true`, empty input serializes to empty string rather than `0`. */
    static AllowEmptyValue = false;
    /** Default number of fixed decimal places when the column does not specify `editorFixedDecimalPlaces`. `null` means no rounding. */
    static DefaultDecimalPlaces: number = null;

    /**
     * Resolves the number of fixed decimal places to use.
     * @returns Number of places, or `null` when none is configured.
     */
    getDecimalPlaces(): number {
        // returns the number of fixed decimal places or null
        var rtn = this._args.column.editorFixedDecimalPlaces;
        if (typeof rtn === 'undefined') {
            rtn = FloatCellEdit.DefaultDecimalPlaces;
        }
        return (!rtn && rtn !== 0 ? null : rtn);
    }

    loadValue(item: any): void {
        this._defaultValue = item[this._args.column.field] ?? "";

        var decPlaces = this.getDecimalPlaces();
        if (decPlaces !== null
            && (this._defaultValue || this._defaultValue === 0)
            && (this._defaultValue as any).toFixed) {
            this._defaultValue = (this._defaultValue as any).toFixed(decPlaces);
        }

        this._input.value = this._defaultValue;
        this._input.defaultValue = this._defaultValue;
        this._input.select();
    }

    serializeValue(): any {
        var rtn = parseFloat(this._input.value) as any;
        if (FloatCellEdit.AllowEmptyValue) {
            if (!rtn && rtn !== 0)
                rtn = '';
        } else {
            rtn = rtn || 0;
        }

        var decPlaces = this.getDecimalPlaces();
        if (decPlaces !== null
            && (rtn || rtn === 0)
            && rtn.toFixed) {
            rtn = parseFloat(rtn.toFixed(decPlaces));
        }

        return rtn;
    }

    validate(): ValidationResult {
        if (isNaN(parseFloat(this._input.value))) {
            return {
                valid: false,
                msg: "Please enter a valid number"
            };
        }

        return super.validate();
    }
}

/**
 * Date editor extending {@link TextCellEdit} with a jQuery UI datepicker.
 * Manages calendar open/close and repositions via {@link DateCellEdit.position}.
 */
export class DateCellEdit extends TextCellEdit {
    private _calendarOpen = false;

    init(): void {
        super.init();

        // @ts-ignore
        (($ as any)(this._input) as any).datepicker({
            showOn: "button",
            buttonText: "",
            buttonImage: null,
            buttonImageOnly: false,
            beforeShow: () => {
                this._calendarOpen = true;
            },
            onClose: () => {
                this._calendarOpen = false;
                if (this._args.compositeEditorOptions)
                    triggerCompositeEditorChange(this, this._args);
            }
        });

        this._input.style.width = (parsePx(getComputedStyle(this._input).width) - (!this._args.compositeEditorOptions ? 18 : 28)) + 'px';
    }

    destroy(): void {
        // @ts-ignore
        ($ as any).datepicker.dpDiv.stop(true, true);
        // @ts-ignore
        (($ as any)(this._input) as any).datepicker("hide").datepicker('destroy')
        super.destroy();
    }

    show(): void {
        if (this._calendarOpen) {
            // @ts-ignore
            ($ as any).datepicker.dpDiv.stop(true, true).show();
        }
    };

    hide(): void {
        if (this._calendarOpen) {
            // @ts-ignore
            ($ as any).datepicker.dpDiv.stop(true, true).hide();
        }
    }

    position(position: Position): void {
        if (!this._calendarOpen) {
            return;
        }
        // @ts-ignore
        ($ as any).datepicker.dpDiv
            .css("top", position.top + 30)
            .css("left", position.left);
    }
}

/**
 * Two-option select editor mapping `"yes"`/`"no"` to boolean `true`/`false`.
 */
export class YesNoSelectCellEdit extends BaseCellEdit {

    declare _input: HTMLSelectElement;

    init(): void {
        this._args.container.appendChild(this._input = <select tabIndex="0" class="editor-yesno slick-editor-yesno">
            <option value="yes">Yes</option>
            <option value="no">No</option>
        </select> as HTMLSelectElement);

        this._input.focus();

        addCompositeChangeListener(this, this._args, this._input);
    }

    loadValue(item: any): void {
        this._input.value = (this._defaultValue = item[this._args.column.field]) ? "yes" : "no";
    }

    serializeValue(): any {
        return this._input.value === "yes";
    }

    isValueChanged(): boolean {
        return this._input.value !== (this._defaultValue ? "yes" : "no");
    }

    validate(): ValidationResult {
        return {
            valid: true,
            msg: null
        };
    }
}

/**
 * Checkbox editor backed by `<input type="checkbox">`. Supports `preClick` to
 * toggle on the click that activates the editor.
 */
export class CheckboxCellEdit extends BaseCellEdit {

    declare _input: HTMLInputElement;

    init(): void {
        this._input = this._args.container.appendChild(<input type="checkbox" value="true" class="editor-checkbox slick-editor-checkbox" /> as HTMLInputElement);
        this._input.focus();

        addCompositeChangeListener(this, this._args, this._input);
    }

    loadValue(item: any): void {
        this._defaultValue = !!item[this._args.column.field];
        this._input.checked = !!this._defaultValue;
    }

    preClick(): void {
        this._input.checked = !this._input.checked;
    }

    serializeValue(): any {
        return this._input.checked;
    }

    applyValue(item: any, state: any): void {
        item[this._args.column.field] = state;
    }

    isValueChanged(): boolean {
        return this.serializeValue() !== this._defaultValue;
    }

    validate() {
        return {
            valid: true,
            msg: null as string
        }
    }
}

/**
 * Percent-complete editor combining {@link IntegerCellEdit} with a vertical
 * jQuery UI slider and preset buttons (0/50/100%).
 */
export class PercentCompleteCellEdit extends IntegerCellEdit {
    declare protected _picker: HTMLDivElement;
    declare protected _slider: HTMLDivElement;

    init(): void {
        super.init();
        this._input.classList.remove('editor-text', 'slick-editor-text');
        this._input.classList.add('editor-percentcomplete', 'slick-editor-percentcomplete');

        this._picker = this._args.container.appendChild(
            <div class="slick-editor-percentcomplete-picker">
                <div class="slick-editor-percentcomplete-helper">
                    <div class="slick-editor-percentcomplete-wrapper">
                        <div class="slick-editor-percentcomplete-slider" ref={el => this._slider = el} />
                        <div class="slick-editor-percentcomplete-buttons">
                            <button data-val={0}>Not started</button>
                            <button data-val={50}>In Progress</button>
                            <button data-val={100}>Complete</button>
                        </div>
                    </div>
                </div>
            </div> as HTMLDivElement);

        this._input.focus();
        this._input.select();

        // @ts-ignore
        (($ as any)(this._slider) as any).slider({
            orientation: "vertical",
            range: "min",
            value: this._defaultValue || 0,
            slide: (_: any, ui: any) => {
                this._input.value = ui.value;
            },
            stop: () => {
                if (this._args.compositeEditorOptions)
                    triggerCompositeEditorChange(this, this._args);
            }
        });

        // @ts-ignore
        ($ as any)(this._picker).find(".slick-editor-percentcomplete-buttons button")
            .on("click", (e: any) => {
                this._input.value = (e.target as HTMLButtonElement).dataset.val;
                // @ts-ignore
                (($ as any)(this._slider) as any).slider("value", (e.target as HTMLButtonElement).dataset.val);
            });
    }

    loadValue(item: any): void {
        super.loadValue(item);
        // keep the slider in sync with the cell value
        // @ts-ignore
        (($ as any)(this._slider) as any).slider("value", this._defaultValue);
    }

    destroy(): void {
        super.destroy();
        this._picker.remove();
    }
}

/*
* An example of a "detached" editor.
* The UI is added onto document BODY and .position(), .show() and .hide() are implemented.
* KeyDown events are also handled to provide handling for Tab, Shift-Tab, Esc and Ctrl-Enter.
*/
/**
 * Detached multi-line text editor using a floating `<textarea>` overlay.
 * Renders attached to `document.body` (or inline for composite editors) and
 * implements `show`/`hide`/`position` for the overlay lifecycle.
 */
export class LongTextCellEdit extends BaseCellEdit {

    declare _input: HTMLTextAreaElement;
    declare protected _container: HTMLElement;
    declare protected _wrapper: HTMLDivElement;

    init(): void {
        const isComposite = this._args.compositeEditorOptions;
        this._container = isComposite ? this._args.container : document.body;

        const boundThis = bindThis(this);
        this._wrapper = this._container.appendChild(<div class="large-editor-text slick-large-editor-text"
            style={{ zIndex: "10000", background: "white", padding: isComposite ? "0" : "5px", border: isComposite ? "0": "3px solid gray", borderRadius: "10px", position: isComposite ? "relative" : "absolute" }}>
            <textarea rows={5} style={{ background: "white", width: "250px", height: "80px", border: "0", outline: "0" }}
                onKeyDown={boundThis.handleKeyDown} ref={el => this._input = el} />
        </div> as HTMLDivElement);

        if (isComposite)
            addCompositeChangeListener(this, this._args, this._input)
        else {
            this._wrapper.appendChild(<div style={{ textAlign: "right" }}>
                <button onClick={boundThis.save}>Save</button>
                <button onClick={boundThis.cancel}>Cancel</button>
            </div>);

            this.position(this._args.position);
        }

        this._input.focus();
        this._input.select();
    }

    /**
     * Handles overlay-specific keys: Ctrl+Enter to save, Esc to cancel, Tab/Shift+Tab
     * to navigate cells, and optionally Left/Right to navigate when at string bounds.
     * @param e - Keyboard event from the textarea.
     */
    handleKeyDown(e: KeyboardEvent): void {
        if (e.key === "Enter" && e.ctrlKey) {
            this.save();
        } else if (e.key === "Esc" || e.key == "Escape") {
            e.preventDefault();
            this.cancel();
        } else if (e.key === "Tab" && e.shiftKey) {
            e.preventDefault();
            this._args.grid.navigatePrev();
        } else if (e.key === "Tab") {
            e.preventDefault();
            this._args.grid.navigateNext();
        } else if (e.key === "Left" || e.key === "ArrowLeft" || e.key === "Right" || e.key === "ArrowRight") {
            if (this._args.editorCellNavOnLRKeys) {
                var cursorPosition = (e.target as HTMLInputElement).selectionStart;
                var textLength = (e.target as HTMLInputElement).value.length;
                if ((e.key === "Left" || e.key === "ArrowLeft") && cursorPosition === 0) {
                    this._args.grid.navigatePrev();
                }
                if ((e.key === "Right" || e.key === "ArrowRight") && cursorPosition >= textLength - 1) {
                    this._args.grid.navigateNext();
                }
            }
        }
    }

    /** Commits the current textarea value via the grid. */
    save(): void {
        this._args.commitChanges();
    };

    /**
     * Cancels editing, restoring the default value and notifying the grid.
     */
    cancel(): void {
        this._input.value = this._defaultValue;
        this._args.cancelChanges();
    }

    /** Hides the detached overlay wrapper. */
    hide(): void {
        this._wrapper.hidden = true;
    }

    /** Shows the detached overlay wrapper. */
    show(): void {
        this._wrapper.hidden = false;
    }

    /**
     * Positions the detached overlay relative to the cell bounds.
     * @param position - Pixel bounds of the target cell.
     */
    position(position: Position): void {
        this._wrapper.style.top = (position.top - 5) + 'px';
        this._wrapper.style.left = (position.left - 5) + 'px';
    }

    destroy(): void {
        this._wrapper.remove();
    }
}

function addCompositeChangeListener(editor: Editor, args: EditorOptions, input: HTMLElement) {
    if (!args.compositeEditorOptions)
        return;

    // don't show Save/Cancel when it's a Composite Editor and also trigger a onCompositeEditorChange event when input changes
    input.addEventListener("change", () => {
        triggerCompositeEditorChange(editor, args);
    });
}

function triggerCompositeEditorChange(editor: Editor, args: EditorOptions) {
    var activeCell = args.grid.getActiveCell();

    // when valid, we'll also apply the new value to the dataContext item object
    if (editor.validate().valid)
        editor.applyValue(args.item, editor.serializeValue());
    editor.applyValue(args.compositeEditorOptions.formValues, editor.serializeValue());
    args.grid.onCompositeEditorChange.notify({
        row: activeCell.row,
        cell: activeCell.cell,
        item: args.item,
        column: args.column,
        formValues: args.compositeEditorOptions.formValues
    });
}

/*
* Depending on the value of Grid option 'editorCellNavOnLRKeys', us
* Navigate to the cell on the left if the cursor is at the beginning of the input string
* and to the right cell if it's at the end. Otherwise, move the cursor within the text
*/
function handleKeydownLRNav(e: KeyboardEvent): void {
    var cursorPosition = this.selectionStart;
    var textLength = this.value.length;
    if (((e.key === "Left" || e.key === "ArrowLeft") && cursorPosition > 0) ||
        (e.key === "Right" || e.key === "ArrowRight") && cursorPosition < textLength - 1) {
        e.stopImmediatePropagation();
    }
}

function handleKeydownLRNoNav(e: KeyboardEvent) {
    if (e.key === "Left" || e.key === "ArrowLeft" || e.key === "Right" || e.key === "ArrowRight") {
        e.stopImmediatePropagation();
    }
}
