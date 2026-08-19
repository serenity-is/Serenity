import { formatNumber, nsSerenity, parseInteger } from "../../base";
import { isTrimmedEmpty } from "../../compat";
import { IDoubleValue } from "../../interfaces";
import { AutoNumeric, type AutoNumericOptions } from "./autonumeric";
import { DecimalEditor } from "./decimaleditor";
import { EditorProps, EditorWidget } from "./editorwidget";

/**
 * Options for the {@link IntegerEditor}.
 */
export interface IntegerEditorOptions {
    /** Minimum allowed value. */
    minValue?: number;
    /** Maximum allowed value. */
    maxValue?: number;
    /** Whether negative values are allowed. */
    allowNegatives?: boolean;
}

/**
 * An editor that renders an integer input with AutoNumeric formatting.
 * @typeParam P - Widget props type.
 */
export class IntegerEditor<P extends IntegerEditorOptions = IntegerEditorOptions> extends EditorWidget<P> implements IDoubleValue {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity, [IDoubleValue]);

    static override createDefaultElement() { return <input type="text" /> as HTMLInputElement; }
    declare readonly domNode: HTMLInputElement;

    /**
     * Creates an integer editor.
     * @param props - Widget props.
     */
    constructor(props: EditorProps<P>) {
        super(props);

        this.domNode.classList.add('integerQ');
        this.initAutoNumeric();
    }

    /**
     * Cleans up the AutoNumeric instance.
     */
    override destroy() {
        AutoNumeric.destroy(this.domNode);
        super.destroy();
    }

    /**
     * Initializes the AutoNumeric instance.
     */
    protected initAutoNumeric() {
        AutoNumeric.init(this.domNode, this.getAutoNumericOptions());
    }

    /**
     * Returns the AutoNumeric options for this editor.
     * @returns AutoNumeric options.
     */
    protected getAutoNumericOptions(): AutoNumericOptions {
        var numericOptions: AutoNumericOptions = Object.assign({}, DecimalEditor.defaultAutoNumericOptions(), {
            vMin: this.options.minValue ?? (this.options.allowNegatives ? (this.options.maxValue != null ? ("-" + Math.abs(this.options.maxValue)) : '-2147483647') : '0'),
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

    /**
     * Returns the current integer value.
     * @returns The value, or null when empty.
     */
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

    /**
     * Returns the current integer value.
     * @returns The value, or null when empty.
     */
    get value(): number {
        return this.get_value();
    }

    /**
     * Sets the integer value.
     * @param value - The value to set.
     */
    set_value(value: number) {
        if (value == null || (value as any) === '')
            this.domNode.value = '';
        else if (AutoNumeric.hasInstance(this.domNode))
            AutoNumeric.setValue(this.domNode, value);
        else
            this.domNode.value = formatNumber(value);
    }

    /** Sets the integer value. */
    set value(v: number) {
        this.set_value(v);
    }

    /**
     * Whether the current value is valid.
     * @returns True when valid.
     */
    get_isValid(): boolean {
        return !isNaN(this.get_value());
    }
}