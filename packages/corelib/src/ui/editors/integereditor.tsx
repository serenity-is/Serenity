import { formatNumber, nsSerenity, parseInteger } from "../../base";
import { isTrimmedEmpty } from "../../compat";
import { IDoubleValue } from "../../interfaces";
import { AutoNumeric, type AutoNumericOptions } from "./autonumeric";
import { DecimalEditor } from "./decimaleditor";
import { EditorProps, EditorWidget } from "./editorwidget";

export interface IntegerEditorOptions {
    minValue?: number;
    maxValue?: number;
    allowNegatives?: boolean;
}

export class IntegerEditor<P extends IntegerEditorOptions = IntegerEditorOptions> extends EditorWidget<P> implements IDoubleValue {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity, [IDoubleValue]);

    static override createDefaultElement() { return <input type="text" /> as HTMLInputElement; }
    declare readonly domNode: HTMLInputElement;

    constructor(props: EditorProps<P>) {
        super(props);

        this.domNode.classList.add('integerQ');
        this.initAutoNumeric();
    }

    override destroy() {
        AutoNumeric.destroy(this.domNode);
        super.destroy();
    }

    protected initAutoNumeric() {
        AutoNumeric.init(this.domNode, this.getAutoNumericOptions());
    }

    protected getAutoNumericOptions(): AutoNumericOptions {
        var numericOptions: AutoNumericOptions = Object.assign({}, DecimalEditor.defaultAutoNumericOptions(), {
            vMin: this.options.minValue ?? (this.options.allowNegatives ? (this.options.maxValue != null ? ("-" + this.options.maxValue) : '-2147483647') : '0'),
            vMax: this.options.maxValue ?? 2147483647,
            aSep: null
        });

        for (const key of Object.keys(this.options)) {
            if (AutoNumeric.allowedSettingKeys.has(key)) {
                (numericOptions as any)[key] = (this.options as any)[key];
            }
        }

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