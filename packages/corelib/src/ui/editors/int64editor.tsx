import { nsSerenity, PropertyItem } from "../../base";
import { IGetEditValue } from "../../interfaces";
import { EditorProps, EditorWidget } from "./editorwidget";

/**
 * Options for the {@link Int64Editor}.
 */
export interface Int64EditorOptions {
    /** Minimum allowed value. */
    minValue?: number | bigint | string;
    /** Maximum allowed value. */
    maxValue?: number | bigint | string;
    /** Whether negative values are allowed. */
    allowNegatives?: boolean;
}

const maxInt64 = 9223372036854775807n;
const minInt64 = -9223372036854775808n;

/**
 * An editor that renders a Int64/long input.
 * @typeParam P - Widget props type.
 */
export class Int64Editor<P extends Int64EditorOptions = Int64EditorOptions> extends EditorWidget<P> implements IGetEditValue {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity, [IGetEditValue]);

    /** Creates the default text input element.
     * @returns The text input element. */
    static override createDefaultElement() { return <input type="text" /> as HTMLInputElement; }
    declare readonly domNode: HTMLInputElement;

    /**
     * Creates a Int64/long editor.
     * @param props - Widget props.
     */
    constructor(props: EditorProps<P>) {
        super(props);

        this.domNode.inputMode = "numeric";
        this.domNode.pattern = this.options.allowNegatives ? "-?[0-9]*" : "[0-9]*";
        this.options.maxValue = Int64Editor.convertToInt64(this.options.maxValue, "maxValue") ?? maxInt64;
        this.options.minValue = Int64Editor.convertToInt64(this.options.minValue, "minValue") ?? (this.options.allowNegatives ? minInt64 : 0n);
        this.domNode.setAttribute("min", this.options.minValue.toString());
        this.domNode.setAttribute("max", this.options.maxValue.toString());
        this.domNode.classList.add('int64');
    }

    static convertToInt64(value: boolean | string | number | bigint | null | undefined, argumentName = "value"): bigint | null {
        if (value == null || value === "")
            return null;

        let result: bigint;
        if (typeof value === "bigint")
            result = value;
        else if (typeof value === "number") {
            if (Math.trunc(value) !== value)
                throw new Error(`Cannot convert a decimal ${argumentName} (${value}) to Int64.`);

            if (value < Number.MIN_SAFE_INTEGER || value > Number.MAX_SAFE_INTEGER)
                throw new Error(`The '${argumentName}' (${value}) is outside the safe integer range. Pass it as a string or bigint.`);

            result = BigInt(value);
        }
        else if (typeof value === "string") {
            try {
                result = BigInt(value);
            }
            catch {
                throw new Error(`Cannot convert the '${argumentName}' ("${value}") to Int64.`);
            }
        }
        else if (typeof value === "boolean") {
            result = BigInt(value);
        }
        else
            throw new Error(`Invalid '${argumentName}' type for Int64 conversion ('${typeof value}')`);

        if (result < minInt64 || result > maxInt64)
            throw new Error(`The '${argumentName}' (${value}) is out of the Int64 range.`);

        return result;
    }

    getEditValue(property: PropertyItem, target: any): void {
        const value = this.get_value();
        if (value == null)
            target[property.name] = null;
        else if (value < Number.MIN_SAFE_INTEGER || value > Number.MAX_SAFE_INTEGER)
            target[property.name] = value.toString();
        else
            target[property.name] = Number(value);
    }

    /**
    * Returns the current Int64/long value.
    * @returns The value, or null when empty.
    */
    get_value(): bigint | null {
        try {
            return Int64Editor.convertToInt64(this.domNode?.value?.trim());
        }
        catch {
            return null;
        }
    }

    /**
     * Returns the current Int64/long value.
     * @returns The value, or null when empty.
     */
    get value(): bigint | null {
        return this.get_value();
    }

    /**
     * Sets the Int64/long value.
     * @param value - The value to set.
     */
    set_value(value: bigint | string | number | null) {
        this.domNode.value = Int64Editor.convertToInt64(value)?.toString() ?? "";
    }

    /** Sets the Int64/long value.
     * @param v - The Int64/long value to set. */
    set value(v: bigint | string | number | null) {
        this.set_value(v);
    }

    /**
     * Whether the current value is valid.
     * @returns True when valid.
     */
    get_isValid(): boolean {
        try {
            Int64Editor.convertToInt64(this.domNode?.value?.trim());
            return true;
        }
        catch {
            return false;
        }
    }
}