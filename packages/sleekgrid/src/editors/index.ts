import { CheckboxCellEdit, DateCellEdit, FloatCellEdit, IntegerCellEdit, LongTextCellEdit, PercentCompleteCellEdit, TextCellEdit, YesNoSelectCellEdit } from "./editors";

export * from "./editors";

export namespace Editors {
    export const Text: typeof TextCellEdit = TextCellEdit;
    export const Integer: typeof IntegerCellEdit = IntegerCellEdit;
    export const Float: typeof FloatCellEdit = FloatCellEdit;
    export const Date: typeof DateCellEdit = DateCellEdit;
    export const YesNoSelect: typeof YesNoSelectCellEdit = YesNoSelectCellEdit;
    export const Checkbox: typeof CheckboxCellEdit = CheckboxCellEdit;
    export const PercentComplete: typeof PercentCompleteCellEdit = PercentCompleteCellEdit;
    export const LongText: typeof LongTextCellEdit = LongTextCellEdit;
}
