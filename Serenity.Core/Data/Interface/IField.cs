using System.Collections;
using System.Collections.Generic;

namespace Serenity.Data
{
    public interface IField
    {
        /// <summary>
        /// Column name</summary>
        string Name { get; }
        /// <summary>
        /// The expression (can be equal to name if no expression)</summary>
        string Expression { get; }
    }
}
