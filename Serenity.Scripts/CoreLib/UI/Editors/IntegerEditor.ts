namespace Serenity {

    export interface IntegerEditorOptions {
        minValue?: number;
		maxValue?: number;
		allowNegatives?: boolean;
    }

    @Decorators.registerEditor('Serenity.IntegerEditor', [IDoubleValue])
    @Decorators.element('<input type="text"/>')
    export class IntegerEditor extends Widget<IntegerEditorOptions> implements IDoubleValue {

        constructor(input: JQuery, opt?: IntegerEditorOptions) {
            super(input, opt);

            input.addClass('integerQ');
            var numericOptions = Q.extend(Serenity.DecimalEditor.defaultAutoNumericOptions(),
                {
					vMin: Q.coalesce(this.options.minValue, this.options.allowNegatives ? (this.options.maxValue != null ? ("-" + this.options.maxValue) : '-2147483647') : '0'),
                    vMax: Q.coalesce(this.options.maxValue, 2147483647),
                    aSep: null
                });

            if ($.fn.autoNumeric)
                (input as any).autoNumeric(numericOptions);
        }

        get_value(): number {
            if ($.fn.autoNumeric) {
                var val = (this.element as any).autoNumeric('get') as string;
                if (!!Q.isTrimmedEmpty(val))
                    return null;
                else 
                    return parseInt(val, 10);
            } 
            else {
                var val = Q.trimToNull(this.element.val());
                if (val == null)
                    return null;
                return Q.parseInteger(val)
            }

        }

        get value(): number {
            return this.get_value();
        }

        set_value(value: number) {
            if (value == null || (value as any) === '')
                this.element.val('');
            else if ($.fn.autoNumeric)
                (this.element as any).autoNumeric('set', value);
            else
                this.element.val(Q.formatNumber(value));
        }

        set value(v: number) {
            this.set_value(v);
        }

        get_isValid(): boolean {
            return !isNaN(this.get_value());
        }
    }
}