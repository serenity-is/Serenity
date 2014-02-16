using System;

namespace Serenity.Data.Mapping
{
    public class AddJoinAttribute : Attribute
    {
        public AddJoinAttribute(string alias)
        {
            this.Alias = alias;
        }

        public String Alias { get; private set; }
    }
}