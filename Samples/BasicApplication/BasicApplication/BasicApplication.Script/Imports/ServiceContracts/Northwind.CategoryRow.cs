
namespace BasicApplication.Northwind
{
    using Serenity;
    using Serenity.ComponentModel;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    [Imported, Serializable, PreserveMemberCase]
    public partial class CategoryRow
    {
        public Int32? CategoryID { get; set; }
        public String CategoryName { get; set; }
        public String Description { get; set; }
        public byte[] Picture { get; set; }
    
        [Imported, PreserveMemberCase]
        public static class Fields
        {
            [InlineConstant] public const string CategoryID = "CategoryID";
            [InlineConstant] public const string CategoryName = "CategoryName";
            [InlineConstant] public const string Description = "Description";
            [InlineConstant] public const string Picture = "Picture";
        }
    }
    
}

