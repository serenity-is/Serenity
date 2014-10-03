
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
    public partial class RegionRow
    {
        public Int32? RegionID { get; set; }
        public String RegionDescription { get; set; }
    
        [Imported, PreserveMemberCase]
        public static class Fields
        {
            [InlineConstant] public const string RegionID = "RegionID";
            [InlineConstant] public const string RegionDescription = "RegionDescription";
        }
    }
    
}

