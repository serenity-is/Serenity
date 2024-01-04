import sQuery from "@optionaldeps/squery";
import { formatNumber, parseInteger } from "@serenity-is/base";
import { Decorators } from "../../decorators";
import { IDoubleValue } from "../../interfaces";
import { extend, isTrimmedEmpty } from "../../q";
import { EditorWidget, EditorProps } from "../widgets/widget";
import { DecimalEditor } from "./decimaleditor";

export interface IntegerEditorOptions {
    minValue?: number;
    maxValue?: number;
    allowNegatives?: boolean;
}

@Decorators.registerEditor('Serenity.IntegerEditor', [IDoubleValue])
@Decorators.element('<input type="text"/>')
export class IntegerEditor<P extends IntegerEditorOptions = IntegerEditorOptions> extends EditorWidget<P> implements IDoubleValue {

    constructor(props: EditorProps<P>) {
        super(props);

        let input = sQuery(this.domNode);
        input.addClass('integerQ');
        var numericOptions = extend(DecimalEditor.defaultAutoNumericOptions(),
            {
                vMin: (this.options.minValue ?? this.options.allowNegatives ? (this.options.maxValue != null ? ("-" + this.options.maxValue) : '-2147483647') : '0'),
                vMax: (this.options.maxValue ?? 2147483647),
                aSep: null
            });

        if ((sQuery.fn as any).autoNumeric)
            (input as any).autoNumeric(numericOptions);
    }

    get_value(): number {
        var val: string;
        if ((sQuery.fn as any).autoNumeric) {
            val = (sQuery(this.domNode) as any).autoNumeric('get') as string;
            if (isTrimmedEmpty(val))
                return null;
            else
                return parseInt(val, 10);
        }
        else {
            val = (sQuery(this.domNode).val() as string)?.trim();
            if (!val)
                return null;
            return parseInteger(val)
        }

    }

    get value(): number {
        return this.get_value();
    }

    set_value(value: number) {
        if (value == null || (value as any) === '')
            sQuery(this.domNode).val('');
        else if ((sQuery.fn as any).autoNumeric)
            (sQuery(this.domNode) as any).autoNumeric('set', value);
        else
            sQuery(this.domNode).val(formatNumber(value));
    }

    set value(v: number) {
        this.set_value(v);
    }

    get_isValid(): boolean {
        return !isNaN(this.get_value());
    }
}