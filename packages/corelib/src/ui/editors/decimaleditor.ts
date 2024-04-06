import { Culture, Fluent, formatNumber, parseDecimal } from "../../base";
import { IDoubleValue } from "../../interfaces";
import { Decorators } from "../../types/decorators";
import { EditorProps, EditorWidget } from "../widgets/widget";
import { AutoNumeric, AutoNumericOptions } from "./autonumeric";

export interface DecimalEditorOptions {
    minValue?: string;
    maxValue?: string;
    decimals?: any;
    padDecimals?: any;
    allowNegatives?: boolean;
}

@Decorators.registerEditor('Serenity.DecimalEditor', [IDoubleValue])
export class DecimalEditor<P extends DecimalEditorOptions = DecimalEditorOptions> extends EditorWidget<P> implements IDoubleValue {

    static override createDefaultElement() { return Fluent("input").attr("type", "text").getNode(); }
    declare readonly domNode: HTMLInputElement;

    constructor(props: EditorProps<P>) {
        super(props);

        this.domNode.classList.add('decimalQ');
        this.initAutoNumeric();
    }

    destroy() {
        AutoNumeric.destroy(this.domNode);
        super.destroy();
    }

    protected initAutoNumeric() {
        AutoNumeric.init(this.domNode, this.getAutoNumericOptions());
    }

    protected getAutoNumericOptions(): any {
        var numericOptions = Object.assign({}, DecimalEditor.defaultAutoNumericOptions(), {
            vMin: this.options.minValue ?? (this.options.allowNegatives ? (this.options.maxValue != null ? ("-" + this.options.maxValue) : '-999999999999.99') : '0.00'),
            vMax: this.options.maxValue ?? '999999999999.99'
        });

        if (this.options.decimals != null) {
            numericOptions.mDec = this.options.decimals;
        }

        if (this.options.padDecimals != null) {
            numericOptions.aPad = this.options.padDecimals;
        }

        return numericOptions;
    }

    get_value(): number {
        var val;
        if (AutoNumeric.hasInstance(this.domNode)) {
            val = AutoNumeric.getValue(this.domNode);

            if (!!(val == null || val === ''))
                return null;

            return parseFloat(val);
        }

        val = this.domNode.value;
        return parseDecimal(val);
    }

    get value(): number {
        return this.get_value();
    }

    set_value(value: number) {
        if (value == null || (value as any) === '') {
            this.domNode.value = '';
        }
        else if (AutoNumeric.hasInstance(this.domNode)) {
            AutoNumeric.setValue(this.domNode, value);
        }
        else
            this.domNode.value = formatNumber(value);
    }

    set value(v: number) {
        this.set_value(v);
    }

    get_isValid(): boolean {
        return !isNaN(this.get_value());
    }

    static defaultAutoNumericOptions(): AutoNumericOptions {
        return {
            aDec: Culture.decimalSeparator,
            altDec: ((Culture.decimalSeparator === '.') ? ',' : '.'),
            aSep: ((Culture.decimalSeparator === '.') ? ',' : '.'),
            aPad: true
        };
    }
}