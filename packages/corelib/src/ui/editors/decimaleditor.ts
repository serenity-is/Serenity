import { Culture, formatNumber, parseDecimal } from "@serenity-is/base";
import { Decorators } from "../../decorators";
import { IDoubleValue } from "../../interfaces";
import { extend } from "../../q";
import { EditorWidget, EditorProps } from "../widgets/widget";

export interface DecimalEditorOptions {
    minValue?: string;
    maxValue?: string;
    decimals?: any;
    padDecimals?: any;
    allowNegatives?: boolean;
}

@Decorators.registerEditor('Serenity.DecimalEditor', [IDoubleValue])
@Decorators.element('<input type="text"/>')
export class DecimalEditor<P extends DecimalEditorOptions = DecimalEditorOptions> extends EditorWidget<P> implements IDoubleValue {

    constructor(props: EditorProps<P>) {
        super(props);

        let input = $(this.domNode);
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
        var val;
        if (($.fn as any).autoNumeric) {
            val = ($(this.domNode) as any).autoNumeric('get');

            if (!!(val == null || val === ''))
                return null;

            return parseFloat(val);
        }

        val = $(this.domNode).val() as any;
        return parseDecimal(val);
    }

    get value(): number {
        return this.get_value();
    }

    set_value(value: number) {
        if (value == null || (value as any) === '') {
            $(this.domNode).val('');
        }
        else if (($.fn as any).autoNumeric) {
            ($(this.domNode) as any).autoNumeric('set', value);
        }
        else
            $(this.domNode).val(formatNumber(value));
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