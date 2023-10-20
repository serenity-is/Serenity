import { Decorators } from "../../decorators";
import { IDoubleValue } from "../../interfaces";
import { Culture, extend, formatNumber, parseDecimal } from "../../q";
import { Widget } from "../widgets/widget";

export interface DecimalEditorOptions {
    minValue?: string;
    maxValue?: string;
    decimals?: any;
    padDecimals?: any;
    allowNegatives?: boolean;
}

@Decorators.registerEditor('Serenity.DecimalEditor', [IDoubleValue])
@Decorators.element('<input type="text"/>')
export class DecimalEditor extends Widget<DecimalEditorOptions> implements IDoubleValue {

    constructor(input: JQuery, opt?: DecimalEditorOptions) {
        super(input, opt);

        input.addClass('decimalQ');
        var numericOptions = extend(DecimalEditor.defaultAutoNumericOptions(), {
            vMin: (this.options.minValue ?? (this.options.allowNegatives ? (this.options.maxValue != null ? ("-" + this.options.maxValue) : '-999999999999.99') : '0.00')),
            vMax: (this.options.maxValue ?? '999999999999.99')
        });

        if (this.options.decimals != null) {
            numericOptions.mDec = this.options.decimals;
        }

        if (this.options.padDecimals != null) {
            numericOptions.aPad = this.options.padDecimals;
        }

        if (($.fn as any).autoNumeric)
            (input as any).autoNumeric(numericOptions);
    }

    get_value(): number {
        if (($.fn as any).autoNumeric) {
            var val = (this.element as any).autoNumeric('get');

            if (!!(val == null || val === ''))
                return null;

            return parseFloat(val);
        }

        var val = this.element.val();
        return parseDecimal(val);
    }

    get value(): number {
        return this.get_value();
    }

    set_value(value: number) {
        if (value == null || (value as any) === '') {
            this.element.val('');
        }
        else if (($.fn as any).autoNumeric) {
            (this.element as any).autoNumeric('set', value);
        }
        else
            this.element.val(formatNumber(value));
    }

    set value(v: number) {
        this.set_value(v);
    }

    get_isValid(): boolean {
        return !isNaN(this.get_value());
    }

    static defaultAutoNumericOptions(): any {
        return {
            aDec: Culture.decimalSeparator,
            altDec: ((Culture.decimalSeparator === '.') ? ',' : '.'),
            aSep: ((Culture.decimalSeparator === '.') ? ',' : '.'),
            aPad: true
        };
    }
}