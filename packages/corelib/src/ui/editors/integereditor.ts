import { Fluent, formatNumber, getjQuery, parseInteger } from "@serenity-is/base";
import { Decorators } from "../../types/decorators";
import { IDoubleValue } from "../../interfaces";
import { extend, isTrimmedEmpty } from "../../q";
import { EditorProps, EditorWidget } from "../widgets/widget";
import { DecimalEditor } from "./decimaleditor";

export interface IntegerEditorOptions {
    minValue?: number;
    maxValue?: number;
    allowNegatives?: boolean;
}

@Decorators.registerEditor('Serenity.IntegerEditor', [IDoubleValue])
export class IntegerEditor<P extends IntegerEditorOptions = IntegerEditorOptions> extends EditorWidget<P> implements IDoubleValue {

    static override createDefaultElement() { return Fluent("input").attr("type", "text").getNode(); }
    declare readonly domNode: HTMLInputElement;

    constructor(props: EditorProps<P>) {
        super(props);

        this.domNode.classList.add('integerQ');
        var numericOptions = extend(DecimalEditor.defaultAutoNumericOptions(),
            {
                vMin: (this.options.minValue ?? this.options.allowNegatives ? (this.options.maxValue != null ? ("-" + this.options.maxValue) : '-2147483647') : '0'),
                vMax: (this.options.maxValue ?? 2147483647),
                aSep: null
            });

        let $ = getjQuery();
        if ($?.fn?.autoNumeric)
            $(this.domNode).autoNumeric?.(numericOptions);
    }

    get_value(): number {
        var val: string;
        let $ = getjQuery();
        if ($?.fn?.autoNumeric) {
            val = $(this.domNode).autoNumeric('get') as string;
            if (isTrimmedEmpty(val))
                return null;
            else
                return parseInt(val, 10);
        }
        else {
            val = this.domNode.value?.trim();
            if (!val)
                return null;
            return parseInteger(val)
        }
    }

    get value(): number {
        return this.get_value();
    }

    set_value(value: number) {
        let $ = getjQuery();
        if (value == null || (value as any) === '')
            this.domNode.value = '';
        else if ($?.fn?.autoNumeric)
            $(this.domNode).autoNumeric('set', value);
        else
            this.domNode.value = formatNumber(value);
    }

    set value(v: number) {
        this.set_value(v);
    }

    get_isValid(): boolean {
        return !isNaN(this.get_value());
    }
}