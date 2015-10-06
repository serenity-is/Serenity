namespace Serenity.Data
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text;

    [IgnoreNamespace]
    [Imported(ObeysTypeSystem = true)]
    [ScriptName("Array")]
    public class BinaryCriteria : BaseCriteria
    {
        [InlineCode("[{left}, {op}, {right}]")]
        public BinaryCriteria(BaseCriteria left, string op, BaseCriteria right)
        {
        }
    }
}