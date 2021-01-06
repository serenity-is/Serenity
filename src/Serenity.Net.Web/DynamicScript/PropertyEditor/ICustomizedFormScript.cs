using Serenity.ComponentModel;
using System.Collections.Generic;

namespace Serenity.Web.PropertyEditor
{
    public interface ICustomizedFormScript
    {
        void Customize(List<PropertyItem> input);
    }
}