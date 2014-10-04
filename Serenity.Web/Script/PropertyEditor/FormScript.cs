using System;

namespace Serenity.Web
{
    public class FormScript : PropertyItemsScript
    {
        public FormScript(string name, Type formType)
            : base("Form." + CheckName(name), formType)
        {
        }
    }
}