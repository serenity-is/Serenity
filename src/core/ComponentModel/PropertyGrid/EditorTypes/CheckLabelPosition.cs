namespace Serenity.ComponentModel;

/// <summary>
/// Where the caption of a checkbox is, e.g. of a <see cref="BooleanEditorAttribute"/> in a form field.
/// </summary>
public enum CheckLabelPosition
{
    /// <summary>
    /// The default: the caption is at the start (left in LTR) in the caption area, and the checkbox is after it, where the
    /// other editors start.
    /// </summary>
    Start = 0,

    /// <summary>
    /// The checkbox is first, at the start of the field, and the caption is after it, on the end side (right in LTR). It saves
    /// horizontal space, and lets several checkboxes share a row.
    /// </summary>
    End = 1,

    /// <summary>
    /// Like <see cref="End"/>, but the caption area before the checkbox is left empty, so the checkbox starts where the
    /// other editors do. It is for a checkbox that is alone in its row, so it lines up with the editors above it.
    /// </summary>
    EndAligned = 2
}
