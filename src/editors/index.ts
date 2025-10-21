import { CheckboxCellEdit, DateCellEdit, FloatCellEdit, IntegerCellEdit, LongTextCellEdit, PercentCompleteCellEdit, TextCellEdit, YesNoSelectCellEdit } from "./editors";

export * from "./editors";

export namespace Editors {
    export const Text = TextCellEdit;
    export const Integer = IntegerCellEdit;
    export const Float = FloatCellEdit;
    export const Date = DateCellEdit;
    export const YesNoSelect = YesNoSelectCellEdit;
    export const Checkbox = CheckboxCellEdit;
    export const PercentComplete = PercentCompleteCellEdit;
    export const LongText = LongTextCellEdit;
}
