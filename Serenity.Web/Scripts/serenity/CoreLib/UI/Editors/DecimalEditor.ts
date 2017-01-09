declare namespace Serenity {

    class DecimalEditor extends Widget<DecimalEditorOptions> {
        constructor(input: JQuery, opt?: DecimalEditorOptions);
        static defaultAutoNumericOptions(): any;
        value: number;
        get_isValid(): boolean;
    }

    interface DecimalEditorOptions {
        minValue?: string;
        maxValue?: string;
        decimals?: any;
        padDecimals?: any;
    }

}