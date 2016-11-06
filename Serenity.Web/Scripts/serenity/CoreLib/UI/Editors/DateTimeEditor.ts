declare namespace Serenity {

    class DateTimeEditor extends Widget<DateTimeEditorOptions> {
        constructor(input: JQuery, opt: DateTimeEditorOptions);
        static roundToMinutes(date: Date, minutesStep: number): Date;
        value: string;
        valueAsDate: Date;
        get_minValue(): string;
        set_minValue(value: string): void;
        get_maxValue(): string;
        set_maxValue(value: string): void;
        get_minDate(): Date;
        set_minDate(value: Date): void;
        get_maxDate(): Date;
        set_maxDate(value: Date): void;
        get_sqlMinMax(): boolean;
        set_sqlMinMax(value: boolean): void;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
    }

    interface DateTimeEditorOptions {
        startHour?: any;
        endHour?: any;
        intervalMinutes?: any;
    }

}