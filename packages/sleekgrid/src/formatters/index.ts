import { formatterContext, type FormatterResult } from "../core";
import { CheckBoxFormatter, CheckmarkFormatter, PercentCompleteBarFormatter, PercentCompleteFormatter, YesNoFormatter } from "./formatters";

export * from "./formatters";

/**
 * Legacy namespace exposing formatters with the old `(row, cell, value)` signature.
 * Each adapter wraps the modern `*Formatter(ctx)` via {@link formatterContext}.
 * @deprecated Prefer importing the named formatters from `"./formatters"` directly
 * and passing a {@link FormatterContext}.
 */
export namespace Formatters {
    /** Legacy adapter for {@link PercentCompleteFormatter}. @param _row - Unused row index. @param _cell - Unused cell index. @param value - Cell value (0–100). @returns Rendered percent text element. */
    export function PercentComplete(_row: number, _cell: number, value: any): FormatterResult {
        return PercentCompleteFormatter(formatterContext({ value }));
    }

    /** Legacy adapter for {@link PercentCompleteBarFormatter}. @param _row - Unused row index. @param _cell - Unused cell index. @param value - Cell value (0–100). @returns Rendered percent bar element. */
    export function PercentCompleteBar(_row: number, _cell: number, value: any): FormatterResult {
        return PercentCompleteBarFormatter(formatterContext({ value }));
    }

    /** Legacy adapter for {@link YesNoFormatter}. @param _row - Unused row index. @param _cell - Unused cell index. @param value - Truthy/falsy cell value. @returns `"Yes"` or `"No"`. */
    export function YesNo(_row: number, _cell: number, value: any): FormatterResult {
        return YesNoFormatter(formatterContext({ value }));
    }

    /** Legacy adapter for {@link CheckBoxFormatter}. @param _row - Unused row index. @param _cell - Unused cell index. @param value - Truthy/falsy cell value. @returns Checkbox icon element. */
    export function Checkbox(_row: number, _cell: number, value: any): FormatterResult {
        return CheckBoxFormatter(formatterContext({ value }));
    }

    /** Legacy adapter for {@link CheckmarkFormatter}. @param _row - Unused row index. @param _cell - Unused cell index. @param value - Truthy/falsy cell value. @returns Checkmark icon or empty string. */
    export function Checkmark(_row: number, _cell: number, value: any): FormatterResult {
        return CheckmarkFormatter(formatterContext({ value }));
    }
}

