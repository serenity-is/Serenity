using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    /// <summary>
    /// Options for the printStackTrace function
    /// </summary>
    [Imported, Serializable]
    public class StackTraceOptions
    {
        [ScriptName("e")]
        public static object Error;
    }
}