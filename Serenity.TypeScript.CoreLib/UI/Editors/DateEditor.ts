declare namespace Serenity {

    class DateEditor extends Widget<any> {
        constructor(input: JQuery);
        static dateInputChange(e: any): void;
        static dateInputKeyup(e: any): void;
        static defaultAutoNumericOptions(): any;
        value: string;
        valueAsDate: Date;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
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
        yearRange: string;
    }

}