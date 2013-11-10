using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public class ScriptContext
    {
        [InlineCode("$({p})")]
        protected static jQueryObject J(object p)
        {
            return null;
        }

        [InlineCode("$({p}, {context})")]
        protected static jQueryObject J(object p, object context)
        {
            return null;
        }
    }
}