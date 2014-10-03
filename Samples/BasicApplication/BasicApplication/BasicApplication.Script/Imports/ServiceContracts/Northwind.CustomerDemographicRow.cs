
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
    public partial class CustomerDemographicRow
    {
        public Int32? ID { get; set; }
        public String CustomerTypeID { get; set; }
        public String CustomerDesc { get; set; }
    
        [Imported, PreserveMemberCase]
        public static class Fields
        {
            [InlineConstant] public const string ID = "ID";
            [InlineConstant] public const string CustomerTypeID = "CustomerTypeID";
            [InlineConstant] public const string CustomerDesc = "CustomerDesc";
        }
    }
    
}

