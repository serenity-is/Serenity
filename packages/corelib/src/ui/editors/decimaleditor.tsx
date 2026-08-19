import { Culture, formatNumber, nsSerenity, parseDecimal } from "../../base";
import { IDoubleValue } from "../../interfaces";
import { AutoNumeric, AutoNumericOptions } from "./autonumeric";
import { EditorProps, EditorWidget } from "./editorwidget";

/**
 * Returns the negative form of an AutoNumeric min/max value, preserving the
 * string format (e.g. leading zeros and decimal places) instead of coercing
 * through Math.abs, which would turn "0000.00" into 0 and lose the format.
 */
function toNegativeValue(value: string | number): string {
    if (typeof value === 'string')
        return value.startsWith('-') ? value : '-' + value;
    return '-' + Math.abs(value);
}

/**
 * Options for the {@link DecimalEditor}.
 */
export interface DecimalEditorOptions {
    /** Minimum allowed value as a string. */
    minValue?: string;
    /** Maximum allowed value as a string. */
    maxValue?: string;
    /** Number of decimal places. */
    decimals?: any;
    /** Whether to pad decimals with zeros. */
    padDecimals?: any;
    /** Whether negative values are allowed. */
    allowNegatives?: boolean;
}

/**
 * An editor that renders a decimal input with AutoNumeric formatting.
 * @typeParam P - Widget props type.
 */
export class DecimalEditor<P extends DecimalEditorOptions = DecimalEditorOptions> extends EditorWidget<P> implements IDoubleValue {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity, [IDoubleValue]);

    static override createDefaultElement() { return <input type="text" /> as HTMLInputElement; }
    declare readonly domNode: HTMLInputElement;

    /**
     * Creates a decimal editor.
     * @param props - Widget props.
     */
    constructor(props: EditorProps<P>) {
        super(props);

        this.domNode.classList.add('decimalQ');
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
            vMin: this.options.minValue ?? (this.options.allowNegatives ? (this.options.maxValue != null ? toNegativeValue(this.options.maxValue) : '-999999999999.99') : '0.00'),
            vMax: this.options.maxValue ?? '999999999999.99'
        });

        if (this.options.decimals != null) {
            numericOptions.mDec = this.options.decimals;
        }

        if (this.options.padDecimals != null) {
            numericOptions.aPad = this.options.padDecimals;
        }

        for (const key of Object.keys(this.options)) {
            if (AutoNumeric.allowedSettingKeys.has(key)) {
                (numericOptions as any)[key] = (this.options as any)[key];
            }
        }

        return numericOptions;
    }

    /**
     * Returns the current decimal value.
     * @returns The value, or null when empty.
     */
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

    /**
     * Returns the current decimal value.
     * @returns The value, or null when empty.
     */
    get value(): number {
        return this.get_value();
    }

    /**
     * Sets the decimal value.
     * @param value - The value to set.
     */
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

    /** Sets the decimal value. */
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

    /**
     * Returns the default AutoNumeric options for decimal editors.
     * @returns AutoNumeric options.
     */
    static defaultAutoNumericOptions(): AutoNumericOptions {
        return {
            aDec: Culture.decimalSeparator,
            altDec: ((Culture.decimalSeparator === '.') ? ',' : '.'),
            aSep: ((Culture.decimalSeparator === '.') ? ',' : '.'),
            aPad: true
        };
    }   
}