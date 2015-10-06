namespace Serenity.Data
{
    using System.Runtime.CompilerServices;

    [IgnoreNamespace]
    [Imported(ObeysTypeSystem = true)]
    [ScriptName("Array")]
    public class ValueCriteria : BaseCriteria
    {
        [InlineCode("({value})")]
        public ValueCriteria(object value)
        {
        }
        
        public object Value
        {
            [InlineCode("({this})")]
            get { return null; }
        }
    }
}