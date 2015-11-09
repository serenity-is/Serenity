using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// Describes a single cell from a Fusion Tables table.
    /// </summary>
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class FusionTablesCell
    {
        /// <summary>
        /// The name of the column in which the cell was located.
        /// </summary>
        [IntrinsicProperty]
        public string ColumnName
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The contents of the cell.
        /// </summary>
        [IntrinsicProperty]
        public string Value
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}