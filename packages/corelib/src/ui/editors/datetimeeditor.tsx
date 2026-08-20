import { Culture, DateTimeEditorTexts, Fluent, FormValidationTexts, Invariant, addValidationRule, formatDate, formatISODateTimeUTC, getjQuery, nsSerenity, parseDate, parseISODateTime, round, setElementReadOnly, stringFormat, trunc } from "../../base";
import { addOption, today } from "../../compat";
import { IReadOnly, IStringValue } from "../../interfaces";
import { flatPickrTrigger } from "../helpers/dateediting";
import { DateEditor } from "./dateeditor";
import { EditorProps, EditorWidget } from "./editorwidget";

/**
 * An editor that renders a date-time input with a date picker and time select.
 * @typeParam P - Widget props type.
 */
export class DateTimeEditor<P extends DateTimeEditorOptions = DateTimeEditorOptions> extends EditorWidget<P> implements IStringValue, IReadOnly {

    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity, [IStringValue, IReadOnly]);

    /** Creates the default text input element for the date-time editor.
     * @returns The input element. */
    static override createDefaultElement() { return <input type="text" /> as HTMLInputElement; }
    /** The text input element that backs the editor. */
    declare readonly domNode: HTMLInputElement;

    declare private time: HTMLSelectElement;
    declare private lastSetValue: string;
    declare private lastSetValueGet: string;

    /**
     * Creates a date-time editor.
     * @param props - Widget props.
     */
    constructor(props: EditorProps<P>) {
        super(props);

        this.domNode.classList.add('s-DateTimeEditor');

        let $ = getjQuery();
        if (this.options.inputOnly) {
            this.domNode.classList.add('dateTimeQ');
            // just a basic input, usually read only display
        }
        // @ts-ignore
        else if (typeof flatpickr !== "undefined" && (DateEditor.useFlatpickr || !$?.fn?.datepicker || this.options.seconds)) {
            this.domNode.classList.add('dateTimeQ');
            // @ts-ignore
            flatpickr(this.domNode, this.getFlatpickrOptions());
            this.createFlatPickrTrigger();
        }
        else if ($?.fn?.datepicker) {
            this.domNode.classList.add('dateQ');

            let $ = getjQuery();
            $(this.domNode).datepicker({
                showOn: 'button',
                beforeShow: () => {
                    if (this.get_readOnly())
                        return false as any;
                    DateEditor.uiPickerZIndexWorkaround(this.domNode);
                    return true;
                },
                yearRange: (this.options.yearRange ?? '-100:+50')
            });

            Fluent.on(this.domNode, 'change.' + this.uniqueName, (e) => {
                this.lastSetValue = null;
                DateEditor.dateInputChange(e as any);
            });

            this.time = <select class="editor s-DateTimeEditor time" /> as HTMLSelectElement;
            var after = this.domNode.nextElementSibling as HTMLElement;
            if (after?.classList.contains("ui-datepicker-trigger")) {
                Fluent(this.time).insertAfter(after);
            }
            else {
                after = this.domNode.previousElementSibling as HTMLElement;
                if (after?.classList.contains("ui-datepicker-trigger")) {
                    Fluent(this.time).insertBefore(after);
                }
                else {
                    Fluent(this.time).insertAfter(this.domNode);
                }
            }

            Fluent.on(this.time, 'change', () => {
                this.lastSetValue = null;
                Fluent.trigger(this.domNode, 'change');
            });

            var timeOpt = DateTimeEditor.getTimeOptions(
                (this.options.startHour ?? 0), 0,
                (this.options.endHour ?? 23), 59,
                (this.options.intervalMinutes ?? 5));

            for (var t of timeOpt) {
                addOption(this.time, t, t);
            }

            addValidationRule(this.domNode, e1 => {
                var value = this.get_value();
                if (!value) {
                    return null;
                }

                if (this.get_minValue() && Invariant.stringCompare(value, this.get_minValue()) < 0) {
                    return stringFormat(FormValidationTexts.MinDate, formatDate(this.get_minValue(), null));
                }

                if (this.get_maxValue() && Invariant.stringCompare(value, this.get_maxValue()) > 0) {
                    return stringFormat(FormValidationTexts.MaxDate, formatDate(this.get_maxValue(), null));
                }

                return null;
            });
        }
        else {
            this.domNode.setAttribute('type', 'datetime');
            this.domNode.classList.add('dateTimeQ');
        }

        Fluent.on(this.domNode, 'keydown.' + this.uniqueName, (e: KeyboardEvent) => {
            if (this.get_readOnly() || e.key !== " ")
                return;

            var input = this.domNode as HTMLInputElement;
            if (input && !(input.value?.trim()?.length) ||
                input.selectionStart === 0 && input.selectionEnd === input.value?.length) {
                e.preventDefault();
                this.setToNow(true);
            }
        });

        Fluent.on(this.domNode, 'keyup.' + this.uniqueName, (e: KeyboardEvent) => {
            if (this.get_readOnly())
                return;

            var before = this.domNode.value;
            DateEditor.dateInputKeyup(e as any);
            if (before != this.domNode.value)
                this.lastSetValue = null;
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

        if (!this.options.inputOnly) {
            (this.time ?? this.domNode).after(
                <i class="inplace-button inplace-now" title={this.getInplaceNowText()} onClick={() => {
                    if (this.get_readOnly())
                        return;
                    this.setToNow(true);
                }}>
                    <b />
                </i>
            );
        }
    }

    /**
     * Sets the value to the current date and time.
     * @param triggerChange - When true, triggers a change event.
     */
    setToNow(triggerChange?: boolean) {
        this.lastSetValue = null;
        this.set_valueAsDate(new Date());
        triggerChange && Fluent.trigger(this.domNode, 'change');
    }

    /**
     * Cleans up the date-time picker instance.
     */
    override destroy() {
        if (this.domNode && (this.domNode as any)._flatpickr && (this.domNode as any)._flatpickr.destroy) {
            (this.domNode as any)._flatpickr.destroy();
        }
        super.destroy();
    }

    /**
     * Returns the flatpickr options for this editor.
     * @returns Flatpickr options.
     */
    getFlatpickrOptions(): any {
        var opt: any = {
            clickOpens: false,
            allowInput: true,
            enableTime: true,
            time_24hr: true,
            enableSeconds: !!this.options.seconds,
            minuteIncrement: this.options.intervalMinutes ?? 5,
            dateFormat: Culture.dateOrder.split('').join(Culture.dateSeparator).replace('y', 'Y') + " H:i" + (this.options.seconds ? ":S" : ""),
            onChange: () => {
                this.lastSetValue = null;
                //this.domNode && Fluent.trigger(this.domNode, 'change');
            },
            disable: [
                (d: Date) => this.get_readOnly() && formatDate(d, "d") != formatDate(this.domNode?.value, "d")
            ]
        }

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

    /**
     * Creates the flatpickr trigger button.
     * @returns The trigger element.
     */
    public createFlatPickrTrigger(): HTMLElement {
        if (!this.domNode)
            return;
        return Fluent(flatPickrTrigger(this.domNode)).insertAfter(this.domNode).getNode();
    }

    /**
     * Returns the current date-time value.
     * @returns The value, or null when empty.
     */
    get_value(): string {
        var value = this.domNode?.value?.trim();
        if (value != null && value.length === 0) {
            return null;
        }

        var result: string;
        if (this.time) {
            var datePart = formatDate(value, 'yyyy-MM-dd');
            var timePart = this.time.value;
            result = datePart + 'T' + timePart + ':00.000';
        }
        else
            result = formatDate(parseDate(this.domNode.value), "yyyy-MM-ddTHH:mm:ss.fff");

        if (this.options.useUtc)
            result = formatISODateTimeUTC(parseISODateTime(result));

        if (this.lastSetValue != null &&
            this.lastSetValueGet == result)
            return this.lastSetValue;

        return result;
    }

    /**
     * Returns the current date-time value.
     * @returns The value.
     */
    get value(): string {
        return this.get_value();
    }

    /**
     * Sets the date-time value.
     * @param value - The value to set.
     */
    set_value(value: string) {
        if (!value) {
            this.domNode.value = "";
            this.time && (this.time.value = "00:00");
        }
        else if (value.toLowerCase() === 'today') {
            if (this.time) {
                this.domNode.value = formatDate(today(), null);
                this.time.value = '00:00';
            }
            else {
                this.domNode.value = formatDate(value, this.getDisplayFormat());
            }
        }
        else {
            var val = ((value.toLowerCase() === 'now') ? new Date() : parseISODateTime(value));
            if (this.time) {
                val = DateTimeEditor.roundToMinutes(val, (this.options.intervalMinutes ?? 5));
                this.domNode.value = formatDate(val, null);
                this.time.value = formatDate(val, 'HH:mm');
            }
            else
                this.domNode.value = formatDate(val, this.getDisplayFormat());
        }

        this.lastSetValue = null;
        if (value && value.toLowerCase() != 'today' && value.toLowerCase() != 'now') {
            this.lastSetValueGet = this.get_value();
            this.lastSetValue = value;
        }
    }

    private getInplaceNowText(): string {
        return DateTimeEditorTexts.asTry().SetToNow ?? 'set to now';
    }

    private getDisplayFormat(): string {
        return (this.options.seconds ? Culture.dateTimeFormat : Culture.dateTimeFormat.replace(':ss', ''));
    }

    /** Sets the date-time value.
     * @param v - The date-time string to set. */
    set value(v: string) {
        this.set_value(v);
    }

    private get_valueAsDate(): Date {
        if (!this.get_value())
            return null;

        return parseISODateTime(this.get_value());
    }

    /**
     * Returns the current date-time value as a Date.
     * @returns The date-time value.
     */
    get valueAsDate() {
        return this.get_valueAsDate();
    }

    private set_valueAsDate(value: Date) {
        if (value == null) {
            this.set_value(null);
            return;
        }

        this.set_value(formatDate(value, 'yyyy-MM-ddTHH:mm' + (this.options.seconds ? ':ss' : '')));
    }

    /** Sets the date-time value as a Date.
     * @param value - The date-time value to set. */
    set valueAsDate(value: Date) {
        this.set_valueAsDate(value);
    }

    /**
     * Returns the minimum allowed date-time value.
     * @returns The minimum value.
     */
    get_minValue(): string {
        return this.options.minValue;
    }

    /**
     * Sets the minimum allowed date-time value.
     * @param value - The minimum value.
     */
    set_minValue(value: string) {
        this.options.minValue = value;
    }

    /**
     * Returns the maximum allowed date-time value.
     * @returns The maximum value.
     */
    get_maxValue(): string {
        return this.options.maxValue;
    }

    /**
     * Sets the maximum allowed date-time value.
     * @param value - The maximum value.
     */
    set_maxValue(value: string): void {
        this.options.maxValue = value;
    }

    /**
     * Returns the minimum allowed date-time as a Date.
     * @returns The minimum date-time.
     */
    get_minDate(): Date {
        return parseISODateTime(this.get_minValue());
    }

    /**
     * Sets the minimum allowed date-time as a Date.
     * @param value - The minimum date-time.
     */
    set_minDate(value: Date): void {
        this.set_minValue(formatDate(value, 'yyyy-MM-ddTHH:mm:ss'));
    }

    /**
     * Returns the maximum allowed date-time as a Date.
     * @returns The maximum date-time.
     */
    get_maxDate(): Date {
        return parseISODateTime(this.get_maxValue());
    }

    /**
     * Sets the maximum allowed date-time as a Date.
     * @param value - The maximum date-time.
     */
    set_maxDate(value: Date) {
        this.set_maxValue(formatDate(value, 'yyyy-MM-ddTHH:mm:ss'));
    }

    /**
     * Whether SQL min/max date-time bounds are applied.
     * @returns True when SQL bounds are set.
     */
    get_sqlMinMax(): boolean {
        return this.get_minValue() === '1753-01-01' && this.get_maxValue() === '9999-12-31';
    }

    /**
     * Sets whether SQL min/max date-time bounds are applied.
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

            let now = this.element.nextSibling(".inplace-now").getNode();
            now && ((now as HTMLElement).style.opacity = value ? "0.1" : "1");

            this.time && setElementReadOnly(this.time, value);
        }
    }

    /**
     * Rounds a date to the nearest minute step.
     * @param date - The date to round.
     * @param minutesStep - Step size in minutes.
     * @returns The rounded date with seconds and milliseconds zeroed. */
    static roundToMinutes(date: Date, minutesStep: number) {
        date = new Date(date.getTime());
        var m = trunc(round(date.getMinutes() / minutesStep) * minutesStep);
        date.setMinutes(m);
        date.setSeconds(0);
        date.setMilliseconds(0);
        return date;
    }

    /**
     * Generates a list of time strings at a fixed step.
     * @param fromHour - Start hour.
     * @param fromMin - Start minute.
     * @param toHour - End hour.
     * @param toMin - End minute.
     * @param stepMins - Step size in minutes.
     * @returns Array of "HH:mm" time strings. */
    static getTimeOptions = function (fromHour: number, fromMin: number,
        toHour: number, toMin: number, stepMins: number) {
        var list = [];
        if (toHour >= 23) {
            toHour = 23;
        }
        if (toMin >= 60) {
            toMin = 59;
        }
        var hour = fromHour;
        var min = fromMin;
        while (true) {
            if (hour > toHour || hour === toHour && min > toMin) {
                break;
            }
            var t = ((hour >= 10) ? '' : '0') + hour + ':' + ((min >= 10) ? '' : '0') + min;
            list.push(t);
            min += stepMins;
            if (min >= 60) {
                min -= 60;
                hour++;
            }
        }
        return list;
    };
}

/**
 * Options for the {@link DateTimeEditor}.
 */
export interface DateTimeEditorOptions {
    /** Starting hour for the time select (0-23). */
    startHour?: any;
    /** Ending hour for the time select (0-23). */
    endHour?: any;
    /** Interval in minutes between time options. */
    intervalMinutes?: any;
    /** Minimum allowed date-time as an ISO string. */
    minValue?: string;
    /** Maximum allowed date-time as an ISO string. */
    maxValue?: string;
    /** Year range for the date picker (e.g. "-100:+50"). */
    yearRange?: string;
    /** Whether to store and display values in UTC. */
    useUtc?: boolean;
    /** Whether to include seconds in the time picker. */
    seconds?: boolean;
    /** Whether to render as a plain input without picker UI. */
    inputOnly?: boolean;
    /** Whether to apply SQL min/max date bounds. */
    sqlMinMax?: boolean;
}
