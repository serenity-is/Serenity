import { CheckboxCellEdit, DateCellEdit, FloatCellEdit, IntegerCellEdit, LongTextCellEdit, PercentCompleteCellEdit, TextCellEdit, YesNoSelectCellEdit } from "./editors";

export * from "./editors";

/**
 * Legacy namespace providing stable aliases for cell editors.
 * Prefers named imports from `"./editors"` when possible.
 */
export namespace Editors {
    /** Legacy alias for {@link TextCellEdit}. */
    export const Text: typeof TextCellEdit = TextCellEdit;
    /** Legacy alias for {@link IntegerCellEdit}. */
    export const Integer: typeof IntegerCellEdit = IntegerCellEdit;
    /** Legacy alias for {@link FloatCellEdit}. */
    export const Float: typeof FloatCellEdit = FloatCellEdit;
    /** Legacy alias for {@link DateCellEdit}. */
    export const Date: typeof DateCellEdit = DateCellEdit;
    /** Legacy alias for {@link YesNoSelectCellEdit}. */
    export const YesNoSelect: typeof YesNoSelectCellEdit = YesNoSelectCellEdit;
    /** Legacy alias for {@link CheckboxCellEdit}. */
    export const Checkbox: typeof CheckboxCellEdit = CheckboxCellEdit;
    /** Legacy alias for {@link PercentCompleteCellEdit}. */
    export const PercentComplete: typeof PercentCompleteCellEdit = PercentCompleteCellEdit;
    /** Legacy alias for {@link LongTextCellEdit}. */
    export const LongText: typeof LongTextCellEdit = LongTextCellEdit;
}
