[assembly: Serenity.Extensibility.ScriptRegistrar(typeof(BasicApplication.Northwind.DynamicScripts))]

namespace BasicApplication.Northwind
{
    using Entities;
    using Serenity.Data;
    using Serenity.Web;

    public static class DynamicScripts
    {
        public static IDynamicScript Category =
            new DbLookupScript<CategoryRow>(
                name: "Northwind.Category",
                getItems: cnn =>
                {
                    var fld = CategoryRow.Fields;
                    return cnn.List<CategoryRow>(q => q.Select(
                        fld.CategoryID,
                        fld.CategoryName));
                });

        public static IDynamicScript Customer =
            new DbLookupScript<CustomerRow>(
                name: "Northwind.Customer",
                getItems: cnn =>
                {
                    var fld = CustomerRow.Fields;
                    return cnn.List<CustomerRow>(q => q.Select(
                        fld.CustomerID,
                        fld.CompanyName));
                });

        public static IDynamicScript CustomerCountry =
            new DbLookupScript<CustomerRow>(
                name: "Northwind.CustomerCountry",
                getItems: cnn =>
                {
                    var fld = CustomerRow.Fields;
                    return cnn.List<CustomerRow>(q => q.Select(
                            fld.Country)
                        .Where(
                            new Criteria(fld.Country) != "" &
                            new Criteria(fld.Country).IsNotNull())
                        .Distinct(true));
                })
            {
                IdField = "Country",
                TextField = "Country"
            };

        public static IDynamicScript Region =
            new DbLookupScript<RegionRow>(
                name: "Northwind.Region",
                getItems: cnn =>
                {
                    var fld = RegionRow.Fields;
                    return cnn.List<RegionRow>(q => q.Select(
                        fld.RegionID,
                        fld.RegionDescription));
                });

        public static IDynamicScript Supplier =
            new DbLookupScript<SupplierRow>(
                name: "Northwind.Supplier",
                getItems: cnn =>
                {
                    var fld = SupplierRow.Fields;
                    return cnn.List<SupplierRow>(q => q.Select(
                        fld.SupplierID,
                        fld.CompanyName));
                });

        public static IDynamicScript SupplierCountry =
            new DbLookupScript<SupplierRow>(
                name: "Northwind.SupplierCountry",
                getItems: cnn =>
                {
                    var fld = SupplierRow.Fields;
                    return cnn.List<SupplierRow>(q => q.Select(
                            fld.Country)
                        .Where(
                            new Criteria(fld.Country) != "" &
                            new Criteria(fld.Country).IsNotNull())
                        .Distinct(true));
                })
            {
                IdField = "Country",
                TextField = "Country"
            };

        public static IDynamicScript Territory =
            new DbLookupScript<TerritoryRow>(
                name: "Northwind.Territory",
                getItems: cnn =>
                {
                    var fld = TerritoryRow.Fields;
                    return cnn.List<TerritoryRow>(q => q.Select(
                        fld.ID,
                        fld.TerritoryID,
                        fld.TerritoryDescription,
                        fld.RegionDescription));
                });
    }
}