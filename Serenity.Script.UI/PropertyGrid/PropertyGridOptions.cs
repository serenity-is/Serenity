using jQueryApi;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Serializable]
    public class PropertyGridOptions
    {
        public PropertyGridOptions()
        {
            UseCategories = true;
            DefaultCategory = Texts.Controls.PropertyGrid.DefaultCategory;
        }

        public string IdPrefix { get; set; }
        public List<PropertyItem> Items { get; set; }
        public bool UseCategories { get; set; }
        public string CategoryOrder { get; set; }
        public string DefaultCategory { get; set; }
        public string LocalTextPrefix { get; set; }
        public PropertyGridMode Mode { get; set; }
    }

    public enum PropertyGridMode
    {
        Insert,
        Update
    }
}