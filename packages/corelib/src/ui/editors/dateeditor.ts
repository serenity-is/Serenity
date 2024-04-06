import { Culture, Fluent, Invariant, addValidationRule, formatDate, getjQuery, isArrayLike, localText, parseISODateTime, stringFormat } from "../../base";
import { IReadOnly, IStringValue } from "../../interfaces";
import { today } from "../../q";
import { Decorators } from "../../types/decorators";
import { dateInputChangeHandler, dateInputKeyupHandler, flatPickrTrigger, jQueryDatepickerInitialization, jQueryDatepickerZIndexWorkaround } from "../helpers/dateediting";
import { EditorProps, EditorWidget } from "../widgets/widget";

export interface DateEditorOptions {
    yearRange?: string;
    minValue?: string;
    maxValue?: string;
    sqlMinMax?: boolean;
}

@Decorators.registerEditor('Serenity.DateEditor', [IStringValue, IReadOnly])
export class DateEditor<P extends DateEditorOptions = DateEditorOptions> extends EditorWidget<P> implements IStringValue, IReadOnly {

    static override createDefaultElement() { return Fluent("input").attr("type", "text").getNode(); }
    declare readonly domNode: HTMLInputElement;

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

        Fluent.on(this.domNode, "keyup." + this.uniqueName, (e: KeyboardEvent) => {
            if (e.key === " " && !this.get_readOnly()) {
                if (this.get_valueAsDate() != today()) {
                    this.set_valueAsDate(today());
                    Fluent.trigger(this.domNode, 'change');
                }
            }
            else {
                DateEditor.dateInputKeyup(e as any);
            }
        });

        Fluent.on(this.domNode, 'change.' + this.uniqueName, DateEditor.dateInputChange);

        addValidationRule(this.domNode, () => {
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
        });

        this.set_sqlMinMax(true)
    }

    destroy() {
        if (this.domNode && (this.domNode as any)._flatpickr && (this.domNode as any)._flatpickr.destroy) {
            (this.domNode as any)._flatpickr.destroy();
        }
        super.destroy();
    }

    get_value(): string {
        var value = this.domNode.value?.trim();
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
            this.domNode.value = "";
        }
        else if (value.toLowerCase() === 'today' || value.toLowerCase() === 'now') {
            this.domNode.value = formatDate(today(), this.domNode.getAttribute("type") === 'date' ? 'yyyy-MM-dd' : null);
        }
        else {
            this.domNode.value = formatDate(value, this.domNode.getAttribute("type") === 'date' ? 'yyyy-MM-dd' : null);
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
        return this.domNode.classList.contains('readonly') || this.domNode.getAttribute('readonly') != null;
    }

    set_readOnly(value: boolean): void {

        if (value !== this.get_readOnly()) {
            this.domNode.classList.toggle('readonly', !!value);
            value ? this.domNode.setAttribute("readonly", "readonly") : this.domNode.removeAttribute("readonly");
            let trg = this.element.nextSibling(".ui-datepicker-trigger").getNode();
            trg && ((trg as HTMLElement).style.opacity = value ? "0.1" : "1");
        }
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
        this.set_minValue(formatDate(value, 'yyyy-MM-dd'));
    }

    get_maxDate(): Date {
        return parseISODateTime(this.get_maxValue());
    }

    set_maxDate(value: Date) {
        this.set_maxValue(formatDate(value, 'yyyy-MM-dd'));
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

    static dateInputChange = function (e: Event) {
        dateInputChangeHandler(e);
    };

    static dateInputKeyup(e: KeyboardEvent) {
        dateInputKeyupHandler(e as any);
    };

    public static useFlatpickr: boolean;

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

    public static uiPickerZIndexWorkaround(el: HTMLElement | ArrayLike<HTMLElement>) {
        let input = isArrayLike(el) ? el[0] : el;
        if (!input)
            return;
        jQueryDatepickerZIndexWorkaround(input as HTMLInputElement);
    }
}

!jQueryDatepickerInitialization() && Fluent.ready(jQueryDatepickerInitialization);