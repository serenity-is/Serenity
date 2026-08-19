import { Culture, Fluent, FormValidationTexts, Invariant, addValidationRule, formatDate, getjQuery, isArrayLike, nsSerenity, parseISODateTime, setElementReadOnly, stringFormat } from "../../base";
import { today } from "../../compat";
import { IReadOnly, IStringValue } from "../../interfaces";
import { dateInputChangeHandler, dateInputKeyupHandler, flatPickrTrigger, jQueryDatepickerInitialization, jQueryDatepickerZIndexWorkaround } from "../helpers/dateediting";
import { EditorProps, EditorWidget } from "./editorwidget";

/**
 * Options for the {@link DateEditor}.
 */
export interface DateEditorOptions {
    /** Year range for the date picker (e.g. "-100:+50"). */
    yearRange?: string;
    /** Minimum allowed date as a string. */
    minValue?: string;
    /** Maximum allowed date as a string. */
    maxValue?: string;
    /** Whether to apply SQL min/max date bounds. */
    sqlMinMax?: boolean;
}

/**
 * An editor that renders a date input with a date picker.
 * @typeParam P - Widget props type.
 */
export class DateEditor<P extends DateEditorOptions = DateEditorOptions> extends EditorWidget<P> implements IStringValue, IReadOnly {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity, [IStringValue, IReadOnly]);

    static override createDefaultElement() { return <input type="text" /> as HTMLInputElement; }
    declare readonly domNode: HTMLInputElement;

    /**
     * Creates a date editor.
     * @param props - Widget props.
     */
    constructor(props: EditorProps<P>) {
        super(props);

        let $ = getjQuery();
        // @ts-ignore
        if (typeof flatpickr !== "undefined" && (DateEditor.useFlatpickr || !$?.fn?.datepicker)) {
            var options = this.getFlatpickrOptions(this.domNode);
            // @ts-ignore
            flatpickr(this.domNode, options);
            this.createFlatPickrTrigger();
        }
        else if ($?.fn?.datepicker) {
            $(this.domNode).datepicker({
                showOn: 'button',
                beforeShow: (inp: any, inst: any) => {
                    if (this.get_readOnly())
                        return false as any;
                    DateEditor.uiPickerZIndexWorkaround(this.domNode);
                    return true;
                },
                yearRange: (this.options?.yearRange ?? '-100:+50')
            });

        }
        else {
            this.domNode.setAttribute('type', 'date');
        }

        Fluent.on(this.domNode, 'keydown.' + this.uniqueName, (e: KeyboardEvent) => {
            if (this.get_readOnly() || e.key !== " ") {
                return;
            }
            
            e.preventDefault();
            if (this.get_valueAsDate() != today()) {
                this.setToToday(true);
            }
        });

        Fluent.on(this.domNode, "keyup." + this.uniqueName, (e: KeyboardEvent) => {
            if (this.get_readOnly()) {
                return;
            }
            DateEditor.dateInputKeyup(e as any);
        });

        Fluent.on(this.domNode, 'change.' + this.uniqueName, DateEditor.dateInputChange);

        addValidationRule(this.domNode, () => {
            var value = this.get_value();
            if (!value) {
                return null;
            }

            if (this.get_minValue() && Invariant.stringCompare(formatDate(value, 'yyyy-MM-dd'), formatDate(this.get_minValue(), 'yyyy-MM-dd')) < 0) {
                return stringFormat(FormValidationTexts.MinDate, formatDate(this.get_minValue(), null));
            }

            if (this.get_maxValue() && Invariant.stringCompare(formatDate(value, 'yyyy-MM-dd'), formatDate(this.get_maxValue(), 'yyyy-MM-dd')) > 0) {
                return stringFormat(FormValidationTexts.MaxDate, formatDate(this.get_maxValue(), null));
            }

            return null;
        });

        if (this.options.sqlMinMax !== false) {
            const minValue = this.options.minValue;
            const maxValue = this.options.maxValue;
            this.set_sqlMinMax(true);
            if (minValue != null)
                this.set_minValue(minValue);
            if (maxValue != null)
                this.set_maxValue(maxValue);
        }
    }

    /**
     * Sets the value to today's date.
     * @param triggerChange - When true, triggers a change event.
     */
    setToToday(triggerChange?: boolean) {
        this.set_valueAsDate(today());
        triggerChange && Fluent.trigger(this.domNode, 'change');
    }    

    /**
     * Cleans up the date picker instance.
     */
    override destroy() {
        if (this.domNode && (this.domNode as any)._flatpickr) {
            (this.domNode as any)._flatpickr.destroy?.();
            delete (this.domNode as any)._flatpickr;
        }
        super.destroy();
    }

    /**
     * Returns the current date value in "yyyy-MM-dd" format.
     * @returns The date value, or null when empty.
     */
    get_value(): string {
        var value = this.domNode?.value?.trim();
        if (!value) {
            return null;
        }

        return formatDate(value, 'yyyy-MM-dd');
    }

    /**
     * Returns the current date value.
     * @returns The date value.
     */
    get value(): string {
        return this.get_value();
    }

    /**
     * Sets the date value.
     * @param value - The date value to set.
     */
    set_value(value: string) {
        if (value == null) {
            this.domNode.value = "";
        }
        else if (value.toLowerCase() === 'today' || value.toLowerCase() === 'now') {
            this.domNode.value = formatDate(today(), this.domNode.getAttribute("type") === 'date' ? 'yyyy-MM-dd' : null);
        }
        else {
            this.domNode.value = formatDate(value, this.domNode.getAttribute("type") === 'date' ? 'yyyy-MM-dd' : null);
        }
    }

    /** Sets the date value. */
    set value(v: string) {
        this.set_value(v);
    }

    private get_valueAsDate(): Date {
        if (!this.get_value())
            return null;

        return parseISODateTime(this.get_value());
    }

    /**
     * Returns the current date value as a Date.
     * @returns The date value.
     */
    get valueAsDate(): Date {
        return this.get_valueAsDate();
    }

    private set_valueAsDate(value: Date): void {
        if (value == null) {
            this.set_value(null);
            return;
        }

        this.set_value(formatDate(value, 'yyyy-MM-dd'));
    }

    /** Sets the date value as a Date. */
    set valueAsDate(v: Date) {
        this.set_valueAsDate(v);
    }

    /**
     * Returns whether the editor is read-only.
     * @returns True when read-only.
     */
    get_readOnly(): boolean {
        return this.domNode.classList.contains('readonly') || this.domNode.getAttribute('readonly') != null;
    }

    /**
     * Sets whether the editor is read-only.
     * @param value - True to enable read-only mode.
     */
    set_readOnly(value: boolean): void {

        if (value !== this.get_readOnly()) {
            setElementReadOnly(this.domNode, value);
            let trg = this.element.nextSibling(".ui-datepicker-trigger").getNode();
            trg && ((trg as HTMLElement).style.opacity = value ? "0.1" : "1");
        }
    }

    /**
     * Returns the minimum allowed date value.
     * @returns The minimum value.
     */
    get_minValue(): string {
        return this.options.minValue;
    }

    /**
     * Sets the minimum allowed date value.
     * @param value - The minimum value.
     */
    set_minValue(value: string) {
        this.options.minValue = value;
    }

    /**
     * Returns the maximum allowed date value.
     * @returns The maximum value.
     */
    get_maxValue(): string {
        return this.options.maxValue;
    }

    /**
     * Sets the maximum allowed date value.
     * @param value - The maximum value.
     */
    set_maxValue(value: string): void {
        this.options.maxValue = value;
    }

    /**
     * Returns the minimum allowed date as a Date.
     * @returns The minimum date.
     */
    get_minDate(): Date {
        return parseISODateTime(this.get_minValue());
    }

    /**
     * Sets the minimum allowed date as a Date.
     * @param value - The minimum date.
     */
    set_minDate(value: Date): void {
        this.set_minValue(formatDate(value, 'yyyy-MM-dd'));
    }

    /**
     * Returns the maximum allowed date as a Date.
     * @returns The maximum date.
     */
    get_maxDate(): Date {
        return parseISODateTime(this.get_maxValue());
    }

    /**
     * Sets the maximum allowed date as a Date.
     * @param value - The maximum date.
     */
    set_maxDate(value: Date) {
        this.set_maxValue(formatDate(value, 'yyyy-MM-dd'));
    }

    /**
     * Whether SQL min/max date bounds are applied.
     * @returns True when SQL bounds are set.
     */
    get_sqlMinMax(): boolean {
        return this.get_minValue() === '1753-01-01' && this.get_maxValue() === '9999-12-31';
    }

    /**
     * Sets whether SQL min/max date bounds are applied.
     * @param value - True to apply SQL bounds.
     */
    set_sqlMinMax(value: boolean) {
        if (value) {
            this.set_minValue('1753-01-01');
            this.set_maxValue('9999-12-31');
        }
        else {
            this.set_minValue(null);
            this.set_maxValue(null);
        }
    }

    /** Handles date input change events. */
    static dateInputChange = function (e: Event) {
        dateInputChangeHandler(e);
    };

    /** Handles date input keyup events. */
    static dateInputKeyup(e: KeyboardEvent) {
        dateInputKeyupHandler(e as any);
    };

    declare public static useFlatpickr: boolean;

    /**
     * Returns the flatpickr options for the given input.
     * @param input - The input element.
     * @returns Flatpickr options.
     */
    public getFlatpickrOptions(input: HTMLElement): any {
        var opt: any = {
            clickOpens: false,
            allowInput: true,
            dateFormat: Culture.dateOrder.split('').join(Culture.dateSeparator).replace('y', 'Y'),
            onChange: () => {
                //this.domNode && Fluent.trigger(this.domNode, 'change');
            },
            disable: [
                (d: Date) => this.get_readOnly() && formatDate(d, "d") != formatDate(this.domNode?.value, "d")
            ]
        };

        if (this.domNode.closest(".modal"))
            opt.appendTo = this.domNode.closest(".modal");
        else {
            setTimeout(() => {
                var modal = this.domNode?.closest(".modal");
                if (modal && !opt.static && !opt.appendTo && this.domNode &&
                    (this.domNode as any)._flatpickr &&
                    (this.domNode as any)._flatpickr.calendarContainer &&
                    (this.domNode as any)._flatpickr.calendarContainer.parentElement !== modal) {
                    modal.appendChild((this.domNode as any)._flatpickr.calendarContainer);
                }
            }, 0);
        }

        return opt;
    }

    public createFlatPickrTrigger(): HTMLElement {
        if (!this.domNode)
            return;
        return Fluent(flatPickrTrigger(this.domNode)).insertAfter(this.domNode).getNode();
    }

    public static uiPickerZIndexWorkaround(el: HTMLElement | ArrayLike<HTMLElement>) {
        let input = isArrayLike(el) ? el[0] : el;
        if (!input)
            return;
        jQueryDatepickerZIndexWorkaround(input as HTMLInputElement);
    }
}

!jQueryDatepickerInitialization() && Fluent.ready(jQueryDatepickerInitialization);