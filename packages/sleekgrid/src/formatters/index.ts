import { formatterContext, type FormatterResult } from "../core";
import { CheckBoxFormatter, CheckmarkFormatter, PercentCompleteBarFormatter, PercentCompleteFormatter, YesNoFormatter } from "./formatters";

export * from "./formatters";

export namespace Formatters {
    export function PercentComplete(_row: number, _cell: number, value: any): FormatterResult {
        return PercentCompleteFormatter(formatterContext({ value }));
    }

    export function PercentCompleteBar(_row: number, _cell: number, value: any): FormatterResult {
        return PercentCompleteBarFormatter(formatterContext({ value }));
    }

    export function YesNo(_row: number, _cell: number, value: any): FormatterResult {
        return YesNoFormatter(formatterContext({ value }));
    }

    export function Checkbox(_row: number, _cell: number, value: any): FormatterResult {
        return CheckBoxFormatter(formatterContext({ value }));
    }

    export function Checkmark(_row: number, _cell: number, value: any): FormatterResult {
        return CheckmarkFormatter(formatterContext({ value }));
    }
}

