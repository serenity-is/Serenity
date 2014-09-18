namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;

    [Editor]
    public class CustomerEditor : LookupEditorBase<object, CustomerRow>
    {
        public CustomerEditor(jQueryObject select)
            : base(select, null)
        {
        }

        protected override string GetLookupKey()
        {
            return "Northwind.Customer";
        }
    }
}