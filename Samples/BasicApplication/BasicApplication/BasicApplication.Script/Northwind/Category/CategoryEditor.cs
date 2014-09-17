namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;

    [Editor]
    public class CategoryEditor : LookupEditorBase<object, CategoryRow>
    {
        public CategoryEditor(jQueryObject select)
            : base(select, null)
        {
        }

        protected override string GetLookupKey()
        {
            return "Northwind.Category";
        }
    }
}