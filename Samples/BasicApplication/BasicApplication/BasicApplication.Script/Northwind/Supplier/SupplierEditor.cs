namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;

    [Editor]
    public class SupplierEditor : LookupEditorBase<object, SupplierRow>
    {
        public SupplierEditor(jQueryObject select)
            : base(select, null)
        {
        }

        protected override string GetLookupKey()
        {
            return "Northwind.Supplier";
        }
    }
}