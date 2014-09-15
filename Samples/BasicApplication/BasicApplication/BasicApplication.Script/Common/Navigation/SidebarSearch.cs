namespace BasicApplication.Common
{
    using jQueryApi;
    using Serenity;

    public class SidebarSearch : Widget
    {
        private jQueryObject menuUL;

        public SidebarSearch(jQueryObject input, jQueryObject menuUL)
            : base(input)
        {
            var self = this;

            new QuickSearchInput(input, new QuickSearchInputOptions
            {
                OnSearch = (field, text) =>
                {
                    self.UpdateMatchFlags(text);
                }
            });

            this.menuUL = menuUL;
        }

        private void UpdateMatchFlags(string text)
        {
            var liList = menuUL.Find("li").RemoveClass("non-match");

            text = text.TrimToNull();
            if (text == null)
            {
                liList.Children("ul").Hide();
                liList.Show().RemoveClass("expanded");
                return;
            }

            var parts = text.Split(new char[] { ',', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < parts.Length; i++)
                parts[i] = Q.Externals.StripDiacritics(parts[i]).ToUpperCase().TrimToNull();

            var items = liList;

            items.Each((i, e) =>
            {
                var x = J(e);
                var title = Q.Externals.StripDiacritics((x.GetText() ?? "").ToUpperCase());


                foreach (var p in parts)
                {
                    if (p != null && !title.Contains(p))
                    {
                        x.AddClass("non-match");
                        break;
                    }
                }
            });

            var matchingItems = items.Not(".non-match");
            var visibles = matchingItems.Parents("li").Add(matchingItems);
            var nonVisibles = liList.Not(visibles);
            nonVisibles.Hide().AddClass("non-match");
            visibles.Show();

            if (visibles.Length <= 100)
            {
                liList.Children("ul").Show();
                liList.AddClass("expanded");
            }
        }
    }
}