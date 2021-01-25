using System;

namespace Serenity.Data
{
    /// <summary>
    /// IEnumTypeField
    /// </summary>
    public interface IEnumTypeField
    {
        /// <summary>
        /// Gets or sets the type of the enum.
        /// </summary>
        /// <value>
        /// The type of the enum.
        /// </value>
        Type EnumType { get; set; }
    }
}
