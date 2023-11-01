import { Decorators } from "../../decorators";
import { IDoubleValue } from "../../interfaces";
import { extend, formatNumber, isTrimmedEmpty, parseInteger, trimToNull } from "../../q";
import { Widget } from "../widgets/widget";
import { DecimalEditor } from "./decimaleditor";

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
        var numericOptions = extend(DecimalEditor.defaultAutoNumericOptions(),
            {
                vMin: (this.options.minValue ?? this.options.allowNegatives ? (this.options.maxValue != null ? ("-" + this.options.maxValue) : '-2147483647') : '0'),
                vMax: (this.options.maxValue ?? 2147483647),
                aSep: null
            });

        if (($.fn as any).autoNumeric)
            (input as any).autoNumeric(numericOptions);
    }

    get_value(): number {
        if (($.fn as any).autoNumeric) {
            var val = (this.element as any).autoNumeric('get') as string;
            if (!!isTrimmedEmpty(val))
                return null;
            else 
                return parseInt(val, 10);
        } 
        else {
            var val = trimToNull(this.element.val());
            if (val == null)
                return null;
            return parseInteger(val)
        }

    }

    get value(): number {
        return this.get_value();
    }

    set_value(value: number) {
        if (value == null || (value as any) === '')
            this.element.val('');
        else if (($.fn as any).autoNumeric)
            (this.element as any).autoNumeric('set', value);
        else
            this.element.val(formatNumber(value));
    }

    set value(v: number) {
        this.set_value(v);
    }

    get_isValid(): boolean {
        return !isNaN(this.get_value());
    }
}