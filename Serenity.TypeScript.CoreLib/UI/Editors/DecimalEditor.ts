namespace Serenity {

    @Decorators.registerEditor('Serenity.DecimalEditor', [IDoubleValue])
    @Decorators.element('<input type="text"/>')
    export class DecimalEditor extends Widget<DecimalEditorOptions> implements IDoubleValue {

        constructor(input: JQuery, opt?: DecimalEditorOptions) {
            super(input, opt);

            input.addClass('decimalQ');
            var numericOptions = $.extend(Serenity.DecimalEditor.defaultAutoNumericOptions(), {
                vMin: Q.coalesce(this.options.minValue, '0.00'),
                vMax: Q.coalesce(this.options.maxValue, '999999999999.99')
            });

            if (this.options.decimals != null) {
                numericOptions.mDec = this.options.decimals;
            }

            if (this.options.padDecimals != null) {
                numericOptions.aPad = this.options.padDecimals;
            }

            (input as any).autoNumeric(numericOptions);
        }

        get_value(): number {
            var val = (this.element as any).autoNumeric('get');

            if (!!(val == null || val === ''))
                return null;

            return parseFloat(val);
        }

        get value(): number {
            return this.get_value();
        }

        set_value(value: number) {
            if (value == null || (value as any) === '') {
                this.element.val('');
            }
			else {
                (this.element as any).autoNumeric('set', value);
            }
        }

        set value(v: number) {
            this.set_value(v);
        }

        get_isValid(): boolean {
            return !isNaN(this.get_value());
        }

        static defaultAutoNumericOptions(): any {
            return {
                aDec: Q.Culture.decimalSeparator,
                altDec: ((Q.Culture.decimalSeparator === '.') ? ',' : '.'),
                aSep: ((Q.Culture.decimalSeparator === '.') ? ',' : '.'),
                aPad: true
            };
        }
    }

    export interface DecimalEditorOptions {
        minValue?: string;
        maxValue?: string;
        decimals?: any;
        padDecimals?: any;
    }

}