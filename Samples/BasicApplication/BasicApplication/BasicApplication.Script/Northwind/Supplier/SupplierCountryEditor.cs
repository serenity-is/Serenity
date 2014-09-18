namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;

    [Editor]
    public class SupplierCountryEditor : LookupEditorBase<object, SupplierRow>
    {
        public SupplierCountryEditor(jQueryObject select)
            : base(select, null)
        {
        }

        protected override string GetLookupKey()
        {
            return "Northwind.SupplierCountry";
        }
    }
}