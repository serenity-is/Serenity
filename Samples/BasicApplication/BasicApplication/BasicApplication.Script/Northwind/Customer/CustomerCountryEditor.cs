namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;

    [Editor]
    public class CustomerCountryEditor : LookupEditorBase<object, CustomerRow>
    {
        public CustomerCountryEditor(jQueryObject select)
            : base(select, null)
        {
        }

        protected override string GetLookupKey()
        {
            return "Northwind.CustomerCountry";
        }
    }
}