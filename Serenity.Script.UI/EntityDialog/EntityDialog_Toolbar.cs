using jQueryApi;
using jQueryApi.UI.Widgets;
using Serenity;
using System;
using System.Collections.Generic;

namespace Serenity
{
    public abstract partial class EntityDialog<TEntity, TOptions> : TemplatedDialog<TOptions>
        where TEntity : class, new()
        where TOptions: class, new()
    {
        protected Toolbar toolbar;
        protected jQueryObject saveAndCloseButton;
        protected jQueryObject saveButton;
        protected jQueryObject deleteButton;
        protected jQueryObject undeleteButton;
        protected jQueryObject cloneButton;

        protected virtual void InitToolbar()
        {
            var toolbarDiv = this.ById("Toolbar");
            if (toolbarDiv.Length == 0)
                return;

            var opt = new ToolbarOptions
            {
                Buttons = GetToolbarButtons()
            };

            toolbar = new Toolbar(toolbarDiv, opt);

            saveAndCloseButton = toolbar.FindButton("save-and-close-button");
            saveButton = toolbar.FindButton("save-button");
            deleteButton = toolbar.FindButton("delete-button");
            undeleteButton = toolbar.FindButton("undo-delete-button");
            cloneButton = toolbar.FindButton("clone-button");
        }

        protected virtual List<ToolButton> GetToolbarButtons()
        {
            List<ToolButton> list = new List<ToolButton>();

            var self = this;

            list.Add(new ToolButton 
            {
                Title = "Kaydet ve Çık",
                CssClass = "save-and-close-button",
                OnClick = delegate
                {
                    self.Save(delegate(ServiceResponse response)
                    {
                        self.element.Dialog().Close();
                    });
                }
            });

            list.Add(new ToolButton
            {
                Title = "Kaydet",
                CssClass = "save-button",
                OnClick = delegate
                {
                    if (self.IsLocalizationMode)
                    {
                        self.SaveLocalization();
                        return;
                    }

                    self.Save(delegate(ServiceResponse response)
                    {
                        if (self.IsEditMode)
                            self.LoadById(self.EntityId.As<long>(), null);
                        else
                            self.LoadById(Type.GetField(response, "EntityId").As<long>(), null);

                        Q.NotifySuccess("Kayıt işlemi başarılı");
                    });
                }
            });

            list.Add(new ToolButton
            {
                Title = "Sil",
                CssClass = "delete-button",
                OnClick = delegate
                {
                    Q.Confirm("Kaydı silmek istiyor musunuz?", delegate
                    {
                        self.DoDelete(delegate
                        {
                            self.element.Dialog().Close();
                        });
                    });
                }
            });

            list.Add(new ToolButton
            {
                Title = "Geri Al",
                CssClass = "undo-delete-button",
                OnClick = delegate
                {
                    if (self.IsDeleted)
                    {
                        Q.Confirm("Kaydı geri almak istiyor musunuz?", delegate()
                        {
                            self.Undelete(delegate
                            {
                                self.LoadById(self.EntityId.As<long>(), null);
                            });
                        });
                    }
                }
            });

            list.Add(new ToolButton
            {
                Title = "Klonla",
                CssClass = "clone-button",
                OnClick = delegate
                {
                    if (!self.IsEditMode)
                        return;

                    var cloneEntity = GetCloningEntity();
                    var cloneDialog = Activator.CreateInstance(this.GetType(), new object()).As<EntityDialog<TEntity, TOptions>>();
                    cloneDialog.Cascade(this.element).LoadEntityAndOpenDialog(cloneEntity);
                }
            });
            
            return list;
        }

        protected virtual TEntity GetCloningEntity()
        {
            var clone = new TEntity();
            clone = jQuery.Extend(clone, this.Entity).As<TEntity>().As<TEntity>();

            var idField = entityIdField.Value;
            if (!idField.IsEmptyOrNull())
                Type.DeleteField(clone, idField);

            var isActiveField = entityIsActiveField.Value;
            if (!isActiveField.IsEmptyOrNull())
                Type.DeleteField(clone, isActiveField);

            return clone;
        }

        protected virtual void UpdateInterface()
        {
            bool isDeleted = IsDeleted;
            bool isLocalizationMode = IsLocalizationMode;

            if (deleteButton != null)
                deleteButton.Toggle(!isLocalizationMode && IsEditMode && !isDeleted);

            if (undeleteButton != null)
                undeleteButton.Toggle(!isLocalizationMode && IsEditMode && isDeleted);

            if (saveAndCloseButton != null)
                saveAndCloseButton.Toggle(!isLocalizationMode && !isDeleted);

            if (saveButton != null)
                saveButton.Toggle(isLocalizationMode || !isDeleted);

            cloneButton.Toggle(false);

            if (propertyGrid != null)
                propertyGrid.Element.Toggle(!isLocalizationMode);

            if (localizationGrid != null)
                localizationGrid.Element.Toggle(isLocalizationMode);

            if (localizationSelect != null)
                localizationSelect.Toggle(IsEditMode && !IsCloneMode);

            if (tabs != null)
                tabs.SetDisabled("Log", IsNewOrDeleted);
        }
    }
}