
namespace BasicApplication.Administration
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [ColumnsKey("Administration.Translation"), IdProperty("Key")]
    [LocalTextPrefix("Administration.Translation"), Service("Administration/Translation")]
    public class TranslationGrid : EntityGrid<TranslationItem>, IAsyncInit
    {
        private string searchText;
        private LookupEditor sourceLanguage;
        private LookupEditor targetLanguage;
        private string targetLanguageKey;
        private bool hasChanges;

        public TranslationGrid(jQueryObject container)
            : base(container)
        {
            this.element.On("keyup." + this.uniqueName + " change." + this.uniqueName, "input.custom-text", e =>
            {
                var value = J(e.Target).GetValue().TrimToNull();
                if (value == "")
                    value = null;
                this.view.GetItemById(J(e.Target).GetDataValue("key")).CustomText = value;
                hasChanges = true;
            });
        }

        protected override void OnClick(jQueryEvent e, int row, int cell)
        {
            base.OnClick(e, row, cell);

            if (e.IsDefaultPrevented())
                return;

            if (J(e.Target).HasClass("source-text"))
            {
                e.PreventDefault();
                var item = view.Rows[row];

                Action done = delegate
                {
                    item.CustomText = item.SourceText;
                    view.UpdateItem(item.Key, item);
                    hasChanges = true;
                };

                if (item.CustomText.IsTrimmedEmpty() || item.CustomText.TrimToEmpty() == item.SourceText.TrimToEmpty())
                {
                    done();
                    return;
                }

                Q.Confirm(Q.Text("Db.Administration.Translation.OverrideConfirmation"), done);
            }

            if (J(e.Target).HasClass("target-text"))
            {
                e.PreventDefault();
                var item = view.Rows[row];

                Action done = delegate
                {
                    item.CustomText = item.TargetText;
                    view.UpdateItem(item.Key, item);
                    hasChanges = true;
                };

                if (item.CustomText.IsTrimmedEmpty() || item.CustomText.TrimToEmpty() == item.TargetText.TrimToEmpty())
                {
                    done();
                    return;
                }

                Q.Confirm(Q.Text("Db.Administration.Translation.OverrideConfirmation"), done);
            }

        }

        protected override Promise<List<SlickColumn>> GetColumnsAsync()
        {
            var columns = new List<SlickColumn>();
            columns.Add(new SlickColumn { Field = "Key", Width = 300, Sortable = false });
            columns.Add(new SlickColumn
            {
                Field = "SourceText",
                Width = 300,
                Sortable = false,
                Format = ctx =>
                {
                    return jQuery.FromHtml("<a/>")
                        .AddClass("source-text")
                        .Text((string)ctx.Value ?? "")
                        .OuterHtml();
                }
            });
            columns.Add(new SlickColumn
            {
                Field = "CustomText",
                Width = 300,
                Sortable = false,
                Format = ctx =>
                {
                    return jQuery.FromHtml("<input/>")
                        .AddClass("custom-text")
                        .Attribute("value", (string)ctx.Value)
                        .Attribute("type", "text")
                        .Attribute("data-key", (string)ctx.Item.Key)
                        .OuterHtml();
                }
            });
            columns.Add(new SlickColumn
            {
                Field = "TargetText",
                Width = 300,
                Sortable = false,
                Format = ctx =>
                {
                    return jQuery.FromHtml("<a/>")
                        .AddClass("target-text")
                        .Text((string)ctx.Value ?? "")
                        .OuterHtml();
                }
            });

            return Promise.FromValue(columns);
        }

        protected override void CreateToolbarExtensions()
        {
            base.CreateToolbarExtensions();

            sourceLanguage = Widget.Create<LookupEditor>(
                element: e => e.AppendTo(toolbar.Element)
                    .Attribute("placeholder", "--- " + Q.Text("Db.Administration.Translation.SourceLanguage") + " ---"),
                options: new LookupEditorOptions
                {
                    LookupKey = "Administration.Language"
                });

            sourceLanguage.ChangeSelect2(e => 
            {
                if (hasChanges)
                {
                    SaveChanges(targetLanguageKey).Then(Refresh);
                }
                else
                    Refresh();

            });

            targetLanguage = Widget.Create<LookupEditor>(
                element: e => e.AppendTo(toolbar.Element)
                    .Attribute("placeholder", "--- " + Q.Text("Db.Administration.Translation.TargetLanguage") + " ---"),
                options: new LookupEditorOptions
                {
                    LookupKey = "Administration.Language"
                });

            targetLanguage.ChangeSelect2(e => 
            {
                if (hasChanges)
                {
                    SaveChanges(targetLanguageKey).Then(Refresh);
                }
                else
                    Refresh();
            });
        }

        protected virtual Promise SaveChanges(string language)
        {
            var translations = new JsDictionary<string, string>();
            foreach (var item in view.GetItems())
                translations[item.Key] = item.CustomText;

            return Promise.Resolve(TranslationService.Update(new TranslationUpdateRequest
            {
                TargetLanguageID = language,
                Translations = translations
            }, null)).Then(delegate()
            {
                hasChanges = false;
                Q.NotifySuccess("User translations in \"" + language + "\" language are saved to \"user.texts." + language + ".json\" " + 
                    "file under \"~/script/site/texts/user/\"");
            });
        }

        protected override bool OnViewSubmit()
        {
            var request = (TranslationListRequest)view.Params;
            request.SourceLanguageID = sourceLanguage.Value;
            targetLanguageKey = targetLanguage.Value ?? "";
            request.TargetLanguageID = targetLanguageKey;
            hasChanges = false;

            return base.OnViewSubmit();
        } 

        protected override List<ToolButton> GetButtons()
        {
            return new List<ToolButton>
            {
                new ToolButton 
                {
                    Title = "Save Changes",
                    OnClick = e => SaveChanges(targetLanguageKey).Then(Refresh),
                    CssClass = "apply-changes-button"
                }
            };
        }

        protected override void CreateQuickSearchInput()
        {
            GridUtils.AddQuickSearchInputCustom(toolbar.Element, (field, searchText) =>
            {
                this.searchText = searchText;
                this.view.SetItems(this.view.GetItems(), true);
            });
        }

        protected override bool OnViewFilter(TranslationItem item)
        {
            if (!base.OnViewFilter(item))
                return false;

            if (searchText.IsEmptyOrNull())
                return true;

            var searching = Q.Externals.StripDiacritics(searchText).ToLower();
            if (searching.IsEmptyOrNull())
                return true;

            if (Q.Externals.StripDiacritics(item.Key ?? "").ToLower().IndexOf(searching) >= 0)
                return true;

            if (Q.Externals.StripDiacritics(item.SourceText ?? "").ToLower().IndexOf(searching) >= 0)
                return true;

            if (Q.Externals.StripDiacritics(item.TargetText ?? "").ToLower().IndexOf(searching) >= 0)
                return true;

            if (Q.Externals.StripDiacritics(item.CustomText ?? "").ToLower().IndexOf(searching) >= 0)
                return true;

            return false;

        }

        protected override bool UsePager()
        {
            return false;
        }
    }
}