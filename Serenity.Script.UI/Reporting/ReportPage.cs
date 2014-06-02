namespace Serenity.Reporting
{
    using jQueryApi;

    public class ReportPage : Widget
    {
        public ReportPage(jQueryObject div)
            : base(div)
        {
            J(".report-link").Click(ReportLinkClick);

            J("div.line").Click(CategoryClick);

            var self = this;

            new QuickSearchInput(J("#QuickSearchInput"), new QuickSearchInputOptions
            {
                OnSearch = (field, text) =>
                {
                    self.UpdateMatchFlags(text);
                }
            });
        }

        private void UpdateMatchFlags(string text)
        {
            var liList = J("#ReportList").Find("li").RemoveClass("non-match");

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

            var reportItems = liList.Filter(".report-item");
            
            reportItems.Each((i, e) =>
            {
                var x = J(e);
                var title = Q.Externals.StripDiacritics((x.GetText() ?? "").ToUpperCase());

                foreach (var p in parts)
                    if (p != null && !title.Contains(p))
                    {
                        x.AddClass("non-match");
                        break;
                    }
            });
            
            var matchingItems = reportItems.Not(".non-match");
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

        private void CategoryClick(jQueryEvent e)
        {
            var li = J(e.Target).Closest("li");
            if (li.HasClass("expanded"))
            {
                li.Find("ul").Hide(EffectDuration.Fast);
                li.RemoveClass("expanded");
                li.Find("li").RemoveClass("expanded");
            }
            else
            {
                li.AddClass("expanded");
                li.Children("ul").Show(EffectDuration.Fast);
                if (li.Children("ul").Children("li").Length == 1 &&
                    !li.Children("ul").Children("li").HasClass("expanded"))
                    li.Children("ul").Children("li").Children(".line").Click();
            }
        }

        private void ReportLinkClick(jQueryEvent e)
        {
            e.PreventDefault();

            var dialog = new ReportDialog(new ReportDialogOptions
            {
                ReportKey = J(e.Target).GetDataValue("key").As<string>()
            });
        }
    }
}