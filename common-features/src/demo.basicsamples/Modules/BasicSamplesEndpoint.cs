using Microsoft.AspNetCore.Mvc;
using Serenity.Demo.Northwind;
using System.Data;

namespace Serenity.Demo.BasicSamples.Endpoints;

[ServiceAuthorize, Route("Services/Serenity.Demo.BasicSamples/[action]")]
[ConnectionKey(typeof(OrderRow))]
public class BasicSamplesEndpoint : ServiceEndpoint
{
    public OrdersByShipperResponse OrdersByShipper(IDbConnection connection, OrdersByShipperRequest request)
    {
        var fld = OrderRow.Fields;
        var year = DateTime.Today.Year;

        var response = new OrdersByShipperResponse();
        var shippers = connection.List<ShipperRow>(q => q.SelectTableFields().OrderBy(ShipperRow.Fields.CompanyName));
        response.ShipperKeys = shippers.Select(x => "s" + x.ShipperID.Value).ToList();
        response.ShipperLabels = shippers.Select(x => x.CompanyName).ToList();

        var monthExpr = new DatePartAttribute(DateParts.Month, fld.OrderDate.Expression)
            .ToString(connection.GetDialect());

        var byMonth = connection.Query(
            new SqlQuery()
                .From(fld)
                .Select(monthExpr, "Month")
                .Select(Sql.Count(), "Count")
                .Select(fld.ShipVia, "ShipVia")
                .GroupBy(monthExpr)
                .GroupBy(fld.ShipVia)
                .Where(
                    fld.OrderDate.IsNotNull() &
                    fld.ShipVia.IsNotNull()))
                .ToDictionary(x => new Tuple<int, int>((int)x.Month, (int)x.ShipVia), x => (int)x.Count);

        response.Values = [];
        var month = 0;
        for (var i = 0; i < 12; i++)
        {
            var d = new Dictionary<string, object>
            {
                ["Month"] = new DateTime(1999, (i + 1), 1)
                    .ToString("MMM", CultureInfo.CurrentCulture)
            };

            foreach (var p in shippers)
                d["s" + p.ShipperID] = byMonth.TryGetValue(
                    new Tuple<int, int>(month, p.ShipperID.Value), out int mc) ? mc : 0;

            response.Values.Add(d);

            if (++month > 12)
                month = 1;
        }

        return response;
    }
}
