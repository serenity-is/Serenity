using jQueryApi;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported]
    public class URLEditor : StringEditor
    {
        public URLEditor(jQueryObject input)
            : base(input)
        {
        }
    }
}