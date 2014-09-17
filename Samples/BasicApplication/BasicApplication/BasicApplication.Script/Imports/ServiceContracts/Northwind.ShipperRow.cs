
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
    public partial class ShipperRow
    {
        public Int32? ShipperID { get; set; }
        public String CompanyName { get; set; }
        public String Phone { get; set; }
    }
    
}

