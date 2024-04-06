import { Fluent, formatNumber, parseInteger } from "../../base";
import { IDoubleValue } from "../../interfaces";
import { isTrimmedEmpty } from "../../q";
import { Decorators } from "../../types/decorators";
import { EditorProps, EditorWidget } from "../widgets/widget";
import { AutoNumeric } from "./autonumeric";
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
            vMin: this.options.minValue ?? (this.options.allowNegatives ? (this.options.maxValue != null ? ("-" + this.options.maxValue) : '-2147483647') : '0'),
            vMax: this.options.maxValue ?? 2147483647,
            aSep: null
        });

        return numericOptions;
    }

    get_value(): number {
        var val: string;
        if (AutoNumeric.hasInstance(this.domNode)) {
            val = AutoNumeric.getValue(this.domNode);
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
        if (value == null || (value as any) === '')
            this.domNode.value = '';
        else if (AutoNumeric.hasInstance(this.domNode))
            AutoNumeric.setValue(this.domNode, value);
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