namespace Serenity.Data
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text;

    [IgnoreNamespace]
    [Imported(ObeysTypeSystem = true)]
    [ScriptName("Array")]
    public class UnaryCriteria : BaseCriteria
    {
        [InlineCode("[{op}, {operand}]")]
        public UnaryCriteria(string op, BaseCriteria operand)
        {
        }
    }
}