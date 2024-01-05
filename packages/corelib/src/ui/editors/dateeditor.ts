import sQuery from "@optionaldeps/squery";
import { Invariant, formatDate, isArrayLike, localText, parseISODateTime, stringFormat } from "@serenity-is/base";
import { dateInputChangeHandler, dateInputKeyupHandler, datePickerIconSvg as datePickerIconSvg_, flatPickrOptions, flatPickrTrigger, jQueryDatepickerInitialization, jQueryDatepickerZIndexWorkaround } from "@serenity-is/base-ui";
import { Decorators } from "../../decorators";
import { IReadOnly, IStringValue } from "../../interfaces";
import { addValidationRule, today } from "../../q";
import { EditorProps, EditorWidget } from "../widgets/widget";

export const datePickerIconSvg = datePickerIconSvg_;

export interface DateEditorOptions {
    yearRange?: string;
    minValue?: string;
    sqlMinMax?: boolean;
}

@Decorators.registerEditor('Serenity.DateEditor', [IStringValue, IReadOnly])
@Decorators.element('<input type="text"/>')
export class DateEditor<P extends DateEditorOptions = DateEditorOptions> extends EditorWidget<P> implements IStringValue, IReadOnly {

    private minValue: string;
    private maxValue: string;

    constructor(props: EditorProps<P>) {
        super(props);

        var input = sQuery(this.domNode);
        // @ts-ignore
        if (typeof flatpickr !== "undefined" && (DateEditor.useFlatpickr || !sQuery.fn.datepicker)) {
            // @ts-ignore
            flatpickr(input[0], DateEditor.flatPickrOptions(input));
        }
        else if ((sQuery.fn as any)?.datepicker) {
            (input as any).datepicker({
                showOn: 'button',
                beforeShow: (inp: any, inst: any) => {
                    if (input.hasClass('readonly') as any)
                        return false as any;
                    DateEditor.uiPickerZIndexWorkaround(input);
                    return true;
                },
                yearRange: (this.yearRange ?? '-100:+50')
            });

        }
        else {
            input.attr('type', 'date');
        }

        input.on('keyup.' + this.uniqueName, e => {
            if (e.which === 32 && !this.get_readOnly()) {
                if (this.get_valueAsDate() != today()) {
                    this.set_valueAsDate(today());
                    sQuery(this.domNode).trigger('change');
                }
            }
            else {
                DateEditor.dateInputKeyup(e as any);
            }
        });

        input.on('change.' + this.uniqueName, DateEditor.dateInputChange);

        addValidationRule(input, e1 => {
            var value = this.get_value();
            if (!value) {
                return null;
            }

            if (this.get_minValue() && Invariant.stringCompare(formatDate(value, 'yyyy-MM-dd'), formatDate(this.get_minValue(), 'yyyy-MM-dd')) < 0) {
                return stringFormat(localText('Validation.MinDate'), formatDate(this.get_minValue(), null));
            }

            if (this.get_maxValue() && Invariant.stringCompare(formatDate(value, 'yyyy-MM-dd'), formatDate(this.get_maxValue(), 'yyyy-MM-dd')) > 0) {
                return stringFormat(localText('Validation.MaxDate'), formatDate(this.get_maxValue(), null));
            }

            return null;
        }, this.uniqueName);

        this.set_sqlMinMax(true)
    }

    get_value(): string {
        var value = (sQuery(this.domNode).val() as string)?.trim();
        if (!value) {
            return null;
        }

        return formatDate(value, 'yyyy-MM-dd');
    }

    get value(): string {
        return this.get_value();
    }

    set_value(value: string) {
        if (value == null) {
            sQuery(this.domNode).val('');
        }
        else if (value.toLowerCase() === 'today' || value.toLowerCase() === 'now') {
            sQuery(this.domNode).val(formatDate(today(), this.domNode.getAttribute("type") === 'date' ? 'yyyy-MM-dd' : null));
        }
        else {
            sQuery(this.domNode).val(formatDate(value, this.domNode.getAttribute("type") === 'date' ? 'yyyy-MM-dd' : null));
        }
    }

    set value(v: string) {
        this.set_value(v);
    }

    private get_valueAsDate(): Date {
        if (!this.get_value())
            return null;

        return parseISODateTime(this.get_value());
    }

    get valueAsDate(): Date {
        return this.get_valueAsDate();
    }

    private set_valueAsDate(value: Date): void {
        if (value == null) {
            this.set_value(null);
        }

        this.set_value(formatDate(value, 'yyyy-MM-dd'));
    }

    set valueAsDate(v: Date) {
        this.set_valueAsDate(v);
    }

    get_readOnly(): boolean {
        return this.domNode.classList.contains('readonly');
    }

    set_readOnly(value: boolean): void {

        if (value !== this.get_readOnly()) {
            if (value) {
                sQuery(this.domNode).addClass('readonly').attr('readonly', 'readonly');
                sQuery(this.domNode).nextAll('.ui-datepicker-trigger').css('opacity', '0.1');
            }
            else {
                sQuery(this.domNode).removeClass('readonly').removeAttr('readonly');
                sQuery(this.domNode).nextAll('.ui-datepicker-trigger').css('opacity', '1');
            }
        }
    }

    @Decorators.option()
    public yearRange: string;

    @Decorators.option()
    get_minValue(): string {
        return this.minValue;
    }

    set_minValue(value: string) {
        this.minValue = value;
    }

    @Decorators.option()
    get_maxValue(): string {
        return this.maxValue;
    }

    set_maxValue(value: string): void {
        this.maxValue = value;
    }

    get_minDate(): Date {
        return parseISODateTime(this.get_minValue());
    }

    set_minDate(value: Date): void {
        this.set_minValue(formatDate(value, 'yyyy-MM-dd'));
    }

    @Decorators.option()
    get_maxDate(): Date {
        return parseISODateTime(this.get_maxValue());
    }

    set_maxDate(value: Date) {
        this.set_maxValue(formatDate(value, 'yyyy-MM-dd'));
    }

    @Decorators.option()
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

    static dateInputChange = function (e: Event) {
        dateInputChangeHandler(e);
    };

    static dateInputKeyup(e: KeyboardEvent) {
        dateInputKeyupHandler(e as any);
    };

    public static useFlatpickr: boolean;

    public static flatPickrOptions(input: JQuery) {
        return flatPickrOptions(function () {
            input.triggerHandler('change');
        });
    }

    public static flatPickrTrigger(input: JQuery): JQuery {
        if (!input.length)
            return;
        return sQuery(flatPickrTrigger(input[0] as HTMLInputElement)).insertAfter(input);
    }

    public static uiPickerZIndexWorkaround(el: HTMLElement | ArrayLike<HTMLElement>) {
        let input = isArrayLike(el) ? el[0] : el;
        if (!input)
            return;
        jQueryDatepickerZIndexWorkaround(input as HTMLInputElement, sQuery);
    }
}

typeof sQuery !== "undefined" && sQuery.fn && !jQueryDatepickerInitialization(sQuery) && sQuery(jQueryDatepickerInitialization);