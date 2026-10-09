import { FormatterContext } from "@serenity-is/sleekgrid";
import { Culture, formatterTypeInfo, htmlEncode, nsSerenity, parseDecimal, registerType } from "../../base";
import { Formatter } from "../../slick";
import { NumberFormatter } from "./numberformatter";

const minSafe = BigInt(Number.MIN_SAFE_INTEGER);
const maxSafe = BigInt(Number.MAX_SAFE_INTEGER);

/**
 * Formats big integer values, including values outside the JavaScript safe integer range
 * (2^53) that arrive as strings, without losing precision.
 * @remarks Values that fit in the safe integer range (including numeric strings and small
 * bigints) are delegated to {@link NumberFormatter}. Only integers outside that range are
 * formatted from their `bigint` representation, so no digits are lost.
 */
export class BigIntFormatter implements Formatter {
    static [Symbol.typeInfo] = formatterTypeInfo(nsSerenity); static { registerType(this); }

    /**
     * Creates a BigIntFormatter.
     * @param props - Formatter options.
     * @param props.displayFormat - Number format string (default `"0"`).
     */
    constructor(public readonly props: { displayFormat?: string } = {}) {
        this.props ??= {};
    }

    /**
     * Formats the cell value as an integer string.
     * @param ctx - Formatter context containing the cell value.
     * @returns Formatted integer string.
     */
    format(ctx: FormatterContext): string {
        return ctx.escape(BigIntFormatter.formatValue(ctx.value, this.displayFormat));
    }

    /**
     * Static helper to format any big integer value (number, numeric string or bigint).
     * This does not HTML encode the return value.
     * @param value - Number, numeric string or bigint.
     * @param format - Format string (default `"0"`).
     * @returns Formatted string.
     */
    static formatValue(value: any, format?: string): string {
        if (value == null)
            return "";

        const big = BigIntFormatter.getAsBigIntIfUnsafe(value);
        if (big != null)
            return BigIntFormatter.formatBigInt(big, format);

        if (typeof value === "number" || typeof value === "bigint")
            return NumberFormatter.formatValue(value, format);

        if (typeof value === "string") {
            const num = parseDecimal(value);
            if (num != null && !isNaN(num))
                return NumberFormatter.formatValue(value, format);
        }

        return value.toString();
    }

    /**
     * Returns the value as a bigint only when it is an integer outside the safe JavaScript
     * integer range (where a number would lose precision); otherwise `null`, so the caller
     * can delegate to the number formatter.
     * @param value - Number, numeric string or bigint.
     * @returns The value as a bigint, or `null`.
     */
    static getAsBigIntIfUnsafe(value: any): bigint | null {
        if (typeof value === "bigint")
            return value < minSafe || value > maxSafe ? value : null;

        if (typeof value === "number") {
            if (!Number.isInteger(value))
                return null;

            return value < Number.MIN_SAFE_INTEGER || value > Number.MAX_SAFE_INTEGER ?
                BigInt(value) : null;
        }

        if (typeof value === "string") {
            const s = value.trim();
            if (!/^[+-]?\d+$/.test(s))
                return null;

            try {
                const big = BigInt(s);
                return big < minSafe || big > maxSafe ? big : null;
            }
            catch {
                return null;
            }
        }

        return null;
    }

    /**
     * Formats a bigint using .NET-style integer format strings and {@link Culture} settings.
     * @param value - Value to format.
     * @param format - Format specifier: `"g"`, `"d"`, `"n"`, `"x"`, or a custom pattern (`"#,##0"`, `"000"`, etc.). Defaults to `"0"`.
     * @returns The formatted integer string.
     */
    static formatBigInt(value: bigint, format?: string): string {
        format = (format ?? "0");

        const negative = value < 0n;
        const abs = negative ? -value : value;
        const neg = Culture?.negativeSign ?? "-";

        const fs = format.charAt(0);
        const precision = format.length > 1 ? parseInt(format.substring(1), 10) : -1;

        if (fs === "x" || fs === "X") {
            let s = abs.toString(16);
            if (fs === "X")
                s = s.toUpperCase();
            if (precision > 0)
                s = s.padStart(precision, "0");
            return (negative ? neg : "") + s;
        }

        let group: boolean;
        let pad = 0;

        if (fs === "n" || fs === "N")
            group = true;
        else if (fs === "d" || fs === "D") {
            group = false;
            pad = precision > 0 ? precision : 0;
        }
        else if (fs === "g" || fs === "G")
            group = false;
        else {
            // custom pattern, e.g. "#,##0"
            const intPart = format.indexOf(".") >= 0 ? format.substring(0, format.indexOf(".")) : format;
            group = intPart.indexOf(",") >= 0;
            const firstZero = intPart.indexOf("0");
            if (firstZero >= 0)
                pad = intPart.length - firstZero;
        }

        let s = abs.toString();
        if (pad > s.length)
            s = s.padStart(pad, "0");

        if (group) {
            const grp = Culture?.groupSeparator ?? ",";
            let grouped = "";
            let i = s.length;
            while (i > 3) {
                grouped = grp + s.substring(i - 3, i) + grouped;
                i -= 3;
            }
            s = s.substring(0, i) + grouped;
        }

        return (negative ? neg : "") + s;
    }

    /** Gets the number display format. @returns The display format string. */
    get displayFormat() { return this.props.displayFormat; }
    /**
     * Sets the number display format.
     * @param value - The display format string.
     */
    set displayFormat(value) { this.props.displayFormat = value; }
}
