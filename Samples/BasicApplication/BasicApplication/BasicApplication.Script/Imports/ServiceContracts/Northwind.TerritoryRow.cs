
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
    public partial class TerritoryRow
    {
        public Int32? ID { get; set; }
        public String TerritoryID { get; set; }
        public String TerritoryDescription { get; set; }
        public Int32? RegionID { get; set; }
        public String RegionDescription { get; set; }
    }
    
}

