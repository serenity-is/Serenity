using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Resets form css class to null, it maybe used to cancel a prior LabelWidth attribute with UntilNext = true
    /// </summary>
    public class ResetLabelWidthAttribute : LabelWidthAttribute
    {
        public ResetLabelWidthAttribute(string value)
            : base(null)
        {
        }
    }
}