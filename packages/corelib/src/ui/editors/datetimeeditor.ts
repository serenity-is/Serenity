import { Culture, Fluent, Invariant, addValidationRule, formatDate, formatISODateTimeUTC, getjQuery, localText, parseDate, parseISODateTime, round, stringFormat, trunc, tryGetText } from "../../base";
import { IReadOnly, IStringValue } from "../../interfaces";
import { addOption, today } from "../../q";
import { Decorators } from "../../types/decorators";
import { flatPickrTrigger } from "../helpers/dateediting";
import { EditorProps, EditorWidget } from "../widgets/widget";
import { DateEditor } from "./dateeditor";
import { EditorUtils } from "./editorutils";

@Decorators.registerEditor('Serenity.DateTimeEditor', [IStringValue, IReadOnly])
export class DateTimeEditor<P extends DateTimeEditorOptions = DateTimeEditorOptions> extends EditorWidget<P> implements IStringValue, IReadOnly {

    static override createDefaultElement() { return Fluent("input").attr("type", "text").getNode(); }
    declare readonly domNode: HTMLInputElement;

    private time: HTMLSelectElement;
    private lastSetValue: string;
    private lastSetValueGet: string;

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

            this.time = Fluent("select").class('editor s-DateTimeEditor time').getNode();
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
                    return stringFormat(localText('Validation.MinDate'), formatDate(this.get_minValue(), null));
                }

                if (this.get_maxValue() && Invariant.stringCompare(value, this.get_maxValue()) > 0) {
                    return stringFormat(localText('Validation.MaxDate'), formatDate(this.get_maxValue(), null));
                }

                return null;
            });
        }
        else {
            this.domNode.setAttribute('type', 'datetime');
            this.domNode.classList.add('dateTimeQ');
        }

        Fluent.on(this.domNode, 'keyup.' + this.uniqueName, (e: KeyboardEvent) => {
            if (this.get_readOnly())
                return;

            if (this.time) {
                if (e.key === " ") {
                    if (this.get_valueAsDate() !== new Date()) {
                        this.set_valueAsDate(new Date());
                        Fluent.trigger(this.domNode, 'change');
                    }
                }
                else {
                    var before = this.domNode.value;
                    DateEditor.dateInputKeyup(e as any);
                    if (before != this.domNode.value)
                        this.lastSetValue = null;
                }
            }
        });

        this.set_sqlMinMax(true);

        if (!this.options.inputOnly) {
            Fluent("i").class("inplace-button inplace-now")
                .append(Fluent("b"))
                .attr('title', this.getInplaceNowText())
                .insertAfter(this.time).on("click", () => {
                    if (this.domNode.classList.contains('readonly')) {
                        return;
                    }
                    this.lastSetValue = null;
                    this.set_valueAsDate(new Date());
                    Fluent.trigger(this.domNode, 'change');
                });
        }
    }

    destroy() {
        if (this.domNode && (this.domNode as any)._flatpickr && (this.domNode as any)._flatpickr.destroy) {
            (this.domNode as any)._flatpickr.destroy();
        }
        super.destroy();
    }

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
                var modal = this.domNode.closest(".modal");
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

    get_value(): string {
        var value = this.domNode.value?.trim();
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

    get value(): string {
        return this.get_value();
    }

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
        return tryGetText('Controls.DateTimeEditor.SetToNow') ?? 'set to now';
    }

    private getDisplayFormat(): string {
        return (this.options.seconds ? Culture.dateTimeFormat : Culture.dateTimeFormat.replace(':ss', ''));
    }

    set value(v: string) {
        this.set_value(v);
    }

    private get_valueAsDate(): Date {
        if (!this.get_value())
            return null;

        return parseISODateTime(this.get_value());
    }

    get valueAsDate() {
        return this.get_valueAsDate();
    }

    private set_valueAsDate(value: Date) {
        if (value == null) {
            this.set_value(null);
        }

        this.set_value(formatDate(value, 'yyyy-MM-ddTHH:mm' + (this.options.seconds ? ':ss' : '')));
    }

    set valueAsDate(value: Date) {
        this.set_valueAsDate(value);
    }

    get_minValue(): string {
        return this.options.minValue;
    }

    set_minValue(value: string) {
        this.options.minValue = value;
    }

    get_maxValue(): string {
        return this.options.maxValue;
    }

    set_maxValue(value: string): void {
        this.options.maxValue = value;
    }

    get_minDate(): Date {
        return parseISODateTime(this.get_minValue());
    }

    set_minDate(value: Date): void {
        this.set_minValue(formatDate(value, 'yyyy-MM-ddTHH:mm:ss'));
    }

    get_maxDate(): Date {
        return parseISODateTime(this.get_maxValue());
    }

    set_maxDate(value: Date) {
        this.set_maxValue(formatDate(value, 'yyyy-MM-ddTHH:mm:ss'));
    }

    get_sqlMinMax(): boolean {
        return this.get_minValue() === '1753-01-01' && this.get_maxValue() === '9999-12-31';
    }

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

    get_readOnly(): boolean {
        return this.domNode.classList.contains('readonly') || this.domNode.getAttribute('readonly') != null;
    }

    set_readOnly(value: boolean): void {

        if (value !== this.get_readOnly()) {
            if (value) {
                this.domNode.classList.toggle('readonly', !!value);
                value ? this.domNode.setAttribute("readonly", "readonly") : this.domNode.removeAttribute("readonly");
                
                let trg = this.element.nextSibling(".ui-datepicker-trigger").getNode();
                trg && ((trg as HTMLElement).style.opacity = value ? "0.1" : "1");

                let now = this.element.nextSibling(".inplace-now").getNode();
                now && ((now as HTMLElement).style.opacity = value ? "0.1" : "1");
            }

            this.time && EditorUtils.setReadonly(this.time, value);
        }
    }

    static roundToMinutes(date: Date, minutesStep: number) {
        date = new Date(date.getTime());
        var m = trunc(round(date.getMinutes() / minutesStep) * minutesStep);
        date.setMinutes(m);
        date.setSeconds(0);
        date.setMilliseconds(0);
        return date;
    }

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

export interface DateTimeEditorOptions {
    startHour?: any;
    endHour?: any;
    intervalMinutes?: any;
    minValue?: string;
    maxValue?: string;
    yearRange?: string;
    useUtc?: boolean;
    seconds?: boolean;
    inputOnly?: boolean;
}