import { formatterContext } from "../core";
import { CheckboxFormatter, CheckmarkFormatter, PercentCompleteBarFormatter, PercentCompleteFormatter, YesNoFormatter } from "./formatters";

export * from "./formatters";

export namespace Formatters {
    export function PercentComplete(_row: number, _cell: number, value: any) {
        return PercentCompleteFormatter(formatterContext({ value }));
    }

    export function PercentCompleteBar(_row: number, _cell: number, value: any) {
        return PercentCompleteBarFormatter(formatterContext({ value }));
    }

    export function YesNo(_row: number, _cell: number, value: any) {
        return YesNoFormatter(formatterContext({ value }));
    }

    export function Checkbox(_row: number, _cell: number, value: any) {
        return CheckboxFormatter(formatterContext({ value }));
    }

    export function Checkmark(_row: number, _cell: number, value: any) {
        return CheckmarkFormatter(formatterContext({ value }));
    }
}

