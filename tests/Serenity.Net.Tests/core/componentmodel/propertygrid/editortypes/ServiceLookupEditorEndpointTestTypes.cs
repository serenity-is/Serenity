using Microsoft.AspNetCore.Mvc;
using Serenity.ComponentModel;
using Serenity.Data;

namespace Serenity.Net.Tests.AutoServiceFor.DataSources
{
    [ConnectionKey("DataSources")]
    public class RouteOnlyRow : Row<RouteOnlyRow.RowFields>
    {
        public class RowFields : RowFieldsBase;
    }

    [ConnectionKey("DataSources")]
    public class ConnectionRow : Row<ConnectionRow.RowFields>
    {
        public class RowFields : RowFieldsBase;
    }

    [ConnectionKey("DataSources")]
    public class LookupOnlyRow : Row<LookupOnlyRow.RowFields>
    {
        public class RowFields : RowFieldsBase;
    }

    [ConnectionKey(typeof(RouteOnlyRow))]
    [Route("Services/DataSources/RouteOnly/[action]")]
    public class RouteOnlyEndpoint
    {
        public void List()
        {
        }
    }

    [ConnectionKey(typeof(ConnectionRow))]
    [Route("Services/DataSources/Connection/[action]")]
    public class ConnectionEndpoint
    {
        public void List()
        {
        }

        [Route("~/Services/DataSources/Connection/Lookup")]
        public void ListLookup()
        {
        }
    }

    [ConnectionKey(typeof(LookupOnlyRow))]
    [Route("Services/DataSources/LookupOnly/[action]")]
    public class LookupOnlyEndpoint
    {
        public void ListLookup()
        {
        }
    }

    [ConnectionKey(typeof(RouteOnlyRow))]
    [Route("Services/DataSources/CustomAction/[action]")]
    public class CustomActionEndpoint
    {
        public void Search()
        {
        }
    }
}

namespace Serenity.Net.Tests.AutoServiceFor.Reports
{
    [ConnectionKey("Reports")]
    public class CustomerRow : Row<CustomerRow.RowFields>
    {
        public class RowFields : RowFieldsBase;
    }

    [ConnectionKey(typeof(CustomerRow))]
    [Route("Reports/Customer/[action]")]
    public class CustomerEndpoint
    {
        public void List()
        {
        }
    }
}

namespace Serenity.Net.Tests.AutoServiceFor.Entities
{
    [ConnectionKey("Entities")]
    public class CustomerRow : Row<CustomerRow.RowFields>
    {
        public class RowFields : RowFieldsBase;
    }
}

namespace Serenity.Net.Tests.AutoServiceFor.Endpoints
{
    [ConnectionKey(typeof(Entities.CustomerRow))]
    [Route("Services/Customer")]
    public class CustomerEndpoint
    {
        public void List()
        {
        }
    }
}

namespace Serenity.Net.Tests.AutoServiceFor.Complex
{
    [ConnectionKey("Complex")]
    [Module("Fallback")]
    public class ComplexRow : Row<ComplexRow.RowFields>
    {
        public class RowFields : RowFieldsBase;
    }
}

namespace Serenity.Net.Tests.AutoServiceFor.Complex.Endpoints
{
    [ConnectionKey(typeof(Complex.ComplexRow))]
    [Route("Services/Complex/{tenant}/[action]")]
    public class ComplexEndpoint
    {
        public void List()
        {
        }
    }
}
