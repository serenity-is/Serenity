using jQueryApi;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public abstract partial class EntityDialog<TEntity, TOptions> : TemplatedDialog<TOptions>
        where TEntity : class, new()
        where TOptions: class, new()
    {
        protected PropertyGrid localizationGrid;
        protected jQueryObject localizationButton;
        protected JsDictionary<string, object> localizationLastValue;
        protected JsDictionary<string, object> localizationPendingValue;
        public static Func<string[][]> defaultLanguageList;

        private void InitLocalizationGrid()
        {
            var pgDiv = this.ById("PropertyGrid");
            if (pgDiv.Length <= 0)
                return;

            #pragma warning disable 618
            var pgOptions = GetPropertyGridOptions();
            #pragma warning restore 618
            InitLocalizationGridCommon(pgOptions);
        }

        private Promise InitLocalizationGridAsync()
        {
            return Promise.Void.ThenAwait(() =>
            {
                var pgDiv = this.ById("PropertyGrid");
                if (pgDiv.Length <= 0)
                    return Promise.Void;

                return GetPropertyGridOptionsAsync()
                    .Then(pgOptions => 
                    { 
                        InitLocalizationGridCommon(pgOptions);
                    });
            });
        }

        private void InitLocalizationGridCommon(PropertyGridOptions pgOptions)
        {
            var pgDiv = this.ById("PropertyGrid");
            var anyLocalizable = false;
            foreach (var item in pgOptions.Items)
                if (item.Localizable == true)
                    anyLocalizable = true;

            if (!anyLocalizable)
                return;

            var localGridDiv = J("<div/>")
                .Attribute("id", this.IdPrefix + "LocalizationGrid")
                .Hide()
                .InsertAfter(pgDiv);

            pgOptions.IdPrefix = this.idPrefix + "Localization_";
            var items = new List<PropertyItem>();
            foreach (var item in pgOptions.Items)
            {
                string[][] langs = null;

                if (item.Localizable == true)
                {
                    var copy = jQuery.ExtendObject(new PropertyItem(), item);
                    copy.OneWay = true;
                    copy.ReadOnly = true;
                    copy.Required = false;
                    copy.DefaultValue = null;
                    items.Add(copy);

                    if (langs == null)
                    {
                        var langsTuple = GetLanguages();
                        langs = (langsTuple as object) as string[][];
                        if (langs == null || langs.Length == 0 || langs[0] == null || !Q.IsArray(langs[0]))
                            langs = langsTuple.Map(x => new string[] { x.Item1, x.Item2 }).ToArray();
                    }

                    foreach (var lang in langs)
                    {
                        copy = jQuery.ExtendObject(new PropertyItem(), item);
                        copy.Name = lang[0] + "$" + copy.Name;
                        copy.Title = lang[1];
                        copy.CssClass = string.Join(" ", copy.CssClass, "translation");
                        copy.Insertable = true;
                        copy.Updatable = true;
                        copy.OneWay = false;
                        copy.Required = false;
                        copy.Localizable = false;
                        copy.DefaultValue = null;
                        items.Add(copy);
                    }
                }
            }

            pgOptions.Items = items;
            localizationGrid = new PropertyGrid(localGridDiv, pgOptions).Init();

            localGridDiv.AddClass("s-LocalizationGrid");

            var self = this;
        }

        protected bool IsLocalizationMode
        {
            [ScriptName("isLocalizationMode")]
            get 
            {
                return
                    localizationButton != null &&
                    localizationButton.HasClass("pressed");
            }
        }

        private bool IsLocalizationModeAndChanged
        {
            [ScriptName("isLocalizationModeAndChanged")]
            get
            {
                if (!IsLocalizationMode)
                    return false;

                var newValue = GetLocalizationGridValue();

                return Q.ToJSON(localizationLastValue) != Q.ToJSON(newValue);
            }
        }

        private void LocalizationButtonClick()
        {
            if (IsLocalizationMode && !ValidateForm())
                return;

            if (IsLocalizationModeAndChanged)
            {
                var newValue = GetLocalizationGridValue();
                localizationLastValue = newValue;
                localizationPendingValue = newValue;
            }

            localizationButton.ToggleClass("pressed");
            UpdateInterface();

            if (IsLocalizationMode)
                LoadLocalization();
        }

        protected virtual List<Tuple<string, string>> GetLanguages()
        {
            if (defaultLanguageList != null)
                return defaultLanguageList().As<List<Tuple<string, string>>>() ?? new List<Tuple<string, string>>();

            return new List<Tuple<string, string>>();
        }

        private void LoadLocalization()
        {
            if (localizationLastValue == null && IsNew)
            {
                localizationGrid.Load(new JsDictionary<string, object>());
                SetLocalizationGridCurrentValues();
                localizationLastValue = GetLocalizationGridValue();
                return;
            }

            if (localizationLastValue != null)
            {
                localizationGrid.Load(localizationLastValue);
                SetLocalizationGridCurrentValues();
                return;
            }

            var self = this;
            var opt = new ServiceCallOptions<RetrieveResponse<TEntity>>();
            opt.Service = this.GetService() + "/Retrieve";
            opt.BlockUI = true;
            opt.Request = new RetrieveRequest
            {
                EntityId = this.EntityId,
                ColumnSelection = RetrieveColumnSelection.KeyOnly,
                IncludeColumns = new List<string> { "Localizations" }
            };
            
            opt.OnSuccess = response => 
            {
                var copy = jQuery.ExtendObject(new object().As<TEntity>(), self.Entity).As<JsDictionary<string, object>>();
                
                foreach (var language in response.Localizations.Keys)
                {
                    var entity = response.Localizations[language].As<JsDictionary<string, object>>();

                    foreach (var key in entity.Keys)
                        copy[language + "$" + key] = entity[key];
                }

                self.localizationGrid.Load(copy);
                SetLocalizationGridCurrentValues();

                localizationPendingValue = null;
                localizationLastValue = GetLocalizationGridValue();
            };

            Q.ServiceCall(opt);
        }

        private void SetLocalizationGridCurrentValues()
        {
            var valueByName = new JsDictionary<string, string>();

            this.localizationGrid.EnumerateItems((item, widget) =>
            {
                if (item.Name.IndexOf("$") < 0 && widget.Element.Is(":input"))
                {
                    valueByName[item.Name] = this.ById(item.Name).GetValue();
                    widget.Element.Value(valueByName[item.Name]);
                }
            });

            this.localizationGrid.EnumerateItems((item, widget) =>
            {
                var idx = item.Name.IndexOf("$");
                if (idx >= 0 && (widget.Element.Is(":input")))
                {
                    var hint = valueByName[item.Name.Substr(idx + 1)];
                    if (hint != null && hint.Length > 0)
                        widget.Element
                            .Attribute("title", hint)
                            .Attribute("placeholder", hint);
                }
            });

        }

        private JsDictionary<string, object> GetLocalizationGridValue()
        {
            var value = new JsDictionary<string, object>();
            localizationGrid.Save(value);
            foreach (var k in value.Keys)
                if (k.IndexOf('$') < 0)
                    value.Remove(k);
            return value;
        }

        private JsDictionary<string, TEntity> GetPendingLocalizations()
        {
            if (localizationPendingValue == null)
                return null;

            var result = new JsDictionary<string, TEntity>();

            string idField = GetIdProperty();

            var langsTuple = GetLanguages();
            var langs = (langsTuple as object) as string[][];
            if (langs == null || langs.Length == 0 || langs[0] == null || !Q.IsArray(langs[0]))
                langs = langsTuple.Map(x => new string[] { x.Item1, x.Item2 }).ToArray();

            foreach (var pair in langs)
            {
                var language = pair[0];

                var entity = new JsDictionary<string, object>();
                if (idField != null)
                    entity[idField] = this.EntityId;

                var prefix = language + "$";

                foreach (var k in localizationPendingValue.Keys)
                {
                    if (k.StartsWith(prefix))
                        entity[k.Substr(prefix.Length)] = localizationPendingValue[k];
                }

                result[language] = entity.As<TEntity>();
            }

            return result;
        }
    }
}