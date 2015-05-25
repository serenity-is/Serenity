using jQueryApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                if (item.Localizable)
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
                if (item.Localizable)
                {
                    var copy = jQuery.ExtendObject(new PropertyItem(), item);
                    copy.OneWay = true;
                    copy.ReadOnly = true;
                    copy.Required = false;
                    copy.DefaultValue = null;
                    items.Add(copy);

                    foreach (var lang in GetLanguages())
                    {
                        copy = jQuery.ExtendObject(new PropertyItem(), item);
                        copy.Name = lang.Item1 + "$" + copy.Name;
                        copy.Title = lang.Item2;
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

        private bool IsLocalizationMode
        {
            get 
            {
                return
                    localizationButton != null &&
                    localizationButton.HasClass("pressed");
            }
        }

        private bool IsLocalizationModeAndChanged
        {
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

        protected virtual IEnumerable<Tuple<string, string>> GetLanguages()
        {
            return new List<Tuple<string, string>>{};
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
            var opt = new ServiceCallOptions<RetrieveLocalizationResponse<TEntity>>();
            opt.Service = this.GetService() + "/RetrieveLocalization";
            opt.BlockUI = true;
            opt.Request = new RetrieveLocalizationRequest
            {
                EntityId = this.EntityId.Value
            };
            
            opt.OnSuccess = response => 
            {
                var copy = jQuery.ExtendObject(new TEntity(), self.Entity).As<JsDictionary<string, object>>();
                
                foreach (var language in response.Entities.Keys)
                {
                    var entity = response.Entities[language].As<JsDictionary<string, object>>();

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

            string idField = GetEntityIdField();

            foreach (var pair in GetLanguages())
            {
                var language = pair.Item1;

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