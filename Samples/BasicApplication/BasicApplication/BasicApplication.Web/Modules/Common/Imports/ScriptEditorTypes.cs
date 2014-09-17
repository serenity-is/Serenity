
using Serenity;
using Serenity.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace BasicApplication.Northwind
{
    public partial class CategoryEditorAttribute : CustomEditorAttribute
    {
        public CategoryEditorAttribute()
            : base("Northwind.Category")
        {
        }
    }

    public partial class PhoneEditorAttribute : CustomEditorAttribute
    {
        public PhoneEditorAttribute()
            : base("Northwind.Phone")
        {
        }
    
        public Boolean Multiple
        {
            get { return GetOption<Boolean>("multiple"); }
            set { SetOption("multiple", value); }
        }
    }

    public partial class RegionEditorAttribute : CustomEditorAttribute
    {
        public RegionEditorAttribute()
            : base("Northwind.Region")
        {
        }
    }

    public partial class SupplierEditorAttribute : CustomEditorAttribute
    {
        public SupplierEditorAttribute()
            : base("Northwind.Supplier")
        {
        }
    }
}

