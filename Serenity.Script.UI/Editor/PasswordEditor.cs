using jQueryApi;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true), Editor, DisplayName("Şifre")]
    public class PasswordEditor : StringEditor
    {
        public PasswordEditor(jQueryObject input)
            : base(input)
        {
            input.Attribute("type", "password");
        }
    }
}