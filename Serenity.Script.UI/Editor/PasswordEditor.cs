using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor("Şifre")]
    public class PasswordEditor : StringEditor
    {
        public PasswordEditor(jQueryObject input)
            : base(input)
        {
            input.Attribute("type", "password");
        }
    }
}