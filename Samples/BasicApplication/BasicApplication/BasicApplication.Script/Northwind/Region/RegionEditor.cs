namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;

    [Editor]
    public class RegionEditor : LookupEditorBase<object, RegionRow>
    {
        public RegionEditor(jQueryObject select)
            : base(select, null)
        {
        }

        protected override string GetLookupKey()
        {
            return "Northwind.Region";
        }
    }
}