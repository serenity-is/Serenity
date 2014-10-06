
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System.Collections.Generic;

    [ColumnsKey("Northwind.Customer"), IdProperty("ID"), NameProperty("CustomerID")]
    [DialogType(typeof(CustomerDialog)), LocalTextPrefix("Northwind.Customer"), Service("Northwind/Customer")]
    public class CustomerGrid : EntityGrid<CustomerRow>, IAsyncInit
    {
        private LookupEditor country;

        public CustomerGrid(jQueryObject container)
            : base(container)
        {
        }

        protected override void CreateToolbarExtensions()
        {
            base.CreateToolbarExtensions();

            country = Widget.Create<LookupEditor>(
                element: e => e.AppendTo(toolbar.Element)
                    .Attribute("placeholder",  "--- " + Q.Text("Db.Northwind.Customer.Country") + " ---"),
                options: new LookupEditorOptions 
                { 
                    LookupKey = "Northwind.CustomerCountry" 
                });

            country.Change(e => Refresh());
        }

        protected override bool OnViewSubmit()
        {
            if (!base.OnViewSubmit())
                return false;

            var req = (ListRequest)view.Params;
            req.EqualityFilter = req.EqualityFilter ?? new JsDictionary<string, object>();
            req.EqualityFilter["Country"] = country.Value;
            return true;
        }
    }
}