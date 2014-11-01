namespace BasicApplication.Common
{
    using BasicApplication.Administration;
    using jQueryApi;
    using Serenity;
    using System.Html;
    using System.Linq;

    public class LanguageSelection : LookupEditorBase<LanguageRow>, IAsyncInit
    {
        private string currentLanguage;

        public LanguageSelection(jQueryObject hidden, string currentLanguage)
            : base(hidden)
        {
            this.currentLanguage = currentLanguage ?? "en";
            this.Value = "en";

            var self = this;
            this.ChangeSelect2(e =>
            {
                jQuery.Instance.cookie("LanguagePreference", self.Value, new { path = Q.Config.ApplicationPath });
                Window.Location.Reload(true);
            });
        }

        protected override System.Promise<Lookup<LanguageRow>> GetLookupAsync()
        {
            return base.GetLookupAsync().ThenSelect(x =>
            {
                if (!x.Items.Any(z => z.LanguageId == currentLanguage))
                {
                    var idx = currentLanguage.LastIndexOf("-");
                    if (idx >= 0)
                    {
                        currentLanguage = currentLanguage.Substr(0, idx);
                        if (!x.Items.Any(z => z.LanguageId == currentLanguage))
                        {
                            currentLanguage = "en";
                        }
                    }
                    else
                        currentLanguage = "en";
                }

                return x;
            });
        }

        protected override System.Promise UpdateItemsAsync()
        {
            return base.UpdateItemsAsync().Then(() =>
            {
                this.Value = currentLanguage;
            });
        }

        protected override string GetLookupKey()
        {
            return "Administration.Language";
        }

        protected override string EmptyItemText()
        {
            return null;
        }
    }
}
