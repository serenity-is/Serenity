using jQueryApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Serenity
{
    public abstract partial class EntityDialog<TEntity, TOptions> : TemplatedDialog<TOptions>
        where TEntity : class, new()
        where TOptions: class, new()
    {
        protected PropertyGrid localizationGrid;
        protected jQueryObject localizationSelect;

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

        private void InitLocalizationGrid(Action complete, Action<object> fail)
        {
            fail.TryCatch(delegate()
            {
                var pgDiv = this.ById("PropertyGrid");
                if (pgDiv.Length <= 0)
                {
                    complete();
                    return;
                }

                GetPropertyGridOptions(pgOptions =>
                {
                    fail.TryCatch(delegate()
                    {
                        InitLocalizationGridCommon(pgOptions);
                        complete();
                    })();
                }, fail);
            })();
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
                if (item.Localizable)
                {
                    var copy = jQuery.ExtendObject(new PropertyItem(), item);
                    copy.Insertable = true;
                    copy.Updatable = true;
                    copy.OneWay = false;
                    copy.Required = false;
                    copy.Localizable = false;
                    copy.DefaultValue = null;
                    items.Add(copy);
                }

            pgOptions.Items = items;
            localizationGrid = new PropertyGrid(localGridDiv, pgOptions);

            localGridDiv.AddClass("s-LocalizationGrid");

            var self = this;
            localizationSelect = J("<select/>")
                .AddClass("s-LocalizationSelect")
                .AppendTo(toolbar.Element)
                .Change((e) => self.LocalizationSelectChange(e));

            foreach (var k in GetLanguages())
                Q.AddOption(localizationSelect, k.Item1.ToString(), k.Item2);
        }

        private bool IsLocalizationMode
        {
            get 
            { 
                return 
                    IsEditMode && !IsCloneMode &&
                    localizationSelect != null &&
                    !localizationSelect.GetValue().IsEmptyOrNull();
            }
        }

        private void LocalizationSelectChange(jQueryEvent e)
        {
            UpdateInterface();

            if (IsLocalizationMode)
                LoadLocalization();
        }

        private static IEnumerable<Tuple<int, string>> GetLanguages()
        {
            return new List<Tuple<int, string>>
            {
                new Tuple<int, string>("".As<int>(), "Türkçe"),
                new Tuple<int, string>(1033, "English"),
                new Tuple<int, string>(3082, "Espanol")
            };
        }

        private void LoadLocalization()
        {
            var self = this;
            var opt = new ServiceCallOptions<RetrieveResponse<TEntity>>();
            opt.Service = this.GetService() + "/RetrieveLocalization";
            opt.BlockUI = true;
            opt.Request = new RetrieveLocalizationRequest
            {
                EntityId = this.EntityId.Value,
                CultureId = Int32.Parse(localizationSelect.GetValue(), 10)
            };
            
            opt.OnSuccess = response => 
            {
                var valueByName = new JsDictionary<string, string>();
                self.localizationGrid.Load(self.Entity);
                self.localizationGrid.EnumerateItems((item, widget) =>
                {
                    if (widget.Element.Is(":input"))
                        valueByName[item.Name] = widget.Element.GetValue();
                });

                self.localizationGrid.Load(response.Entity);

                self.localizationGrid.EnumerateItems((item, widget) =>
                {
                    if (widget.Element.Is(":input"))
                    {
                        var hint = valueByName[item.Name];
                        if (hint != null && hint.Length > 0)
                            widget.Element
                                .Attribute("title", "Türkçe Metin: " + hint)
                                .Attribute("placeholder", hint);
                    }
                });
            };

            Q.ServiceCall(opt);
        }

        private void SaveLocalization()
        {
            if (!ValidateForm())
                return;

            var opt = new ServiceCallOptions();
            opt.Service = this.GetService() + "/UpdateLocalization";
            opt.OnSuccess = delegate(ServiceResponse response)
            {
            };

            var entity = new TEntity();
            this.localizationGrid.Save(entity);

            string idField = GetEntityIdField();
            if (idField != null)
                entity.As<JsDictionary>()[idField] = this.EntityId;

            opt.Request = new UpdateLocalizationRequest<TEntity> 
            {
                CultureId = Int32.Parse(localizationSelect.GetValue(), 10),
                Entity = entity
            };

            Q.ServiceCall(opt);
        }
    }
}