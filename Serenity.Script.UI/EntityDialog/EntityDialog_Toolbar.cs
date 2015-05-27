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
        protected jQueryObject saveAndCloseButton;
        protected jQueryObject applyChangesButton;
        protected jQueryObject deleteButton;
        protected jQueryObject undeleteButton;
        protected jQueryObject cloneButton;

        protected override void InitToolbar()
        {
            base.InitToolbar();

            if (this.toolbar == null)
                return;

            saveAndCloseButton = toolbar.FindButton("save-and-close-button");
            applyChangesButton = toolbar.FindButton("apply-changes-button");
            deleteButton = toolbar.FindButton("delete-button");
            undeleteButton = toolbar.FindButton("undo-delete-button");
            cloneButton = toolbar.FindButton("clone-button");
            localizationButton = toolbar.FindButton("localization-button");
        }

        protected virtual void ShowSaveSuccessMessage(ServiceResponse response)
        {
            Q.NotifySuccess(Texts.Controls.EntityDialog.SaveSuccessMessage);
        }

        protected override List<ToolButton> GetToolbarButtons()
        {
            List<ToolButton> list = new List<ToolButton>();

            var self = this;

            if (!isPanel)
            {
                list.Add(new ToolButton
                {
                    Title = Texts.Controls.EntityDialog.SaveButton,
                    CssClass = "save-and-close-button",
                    OnClick = delegate
                    {
                        self.Save(delegate(ServiceResponse response)
                        {
                            self.element.Dialog().Close();
                        });
                    }
                });
            }

            list.Add(new ToolButton
            {
                Title = isPanel ? Texts.Controls.EntityDialog.SaveButton : LocalText.Empty,
                Hint = isPanel ? Texts.Controls.EntityDialog.SaveButton : Texts.Controls.EntityDialog.ApplyChangesButton,
                CssClass = "apply-changes-button",
                OnClick = delegate
                {
                    self.Save(delegate(ServiceResponse response)
                    {
                        if (self.IsEditMode)
                            self.LoadById(self.EntityId.As<long>(), null);
                        else
                            self.LoadById(((object)(response.As<dynamic>().EntityId)).As<long>(), null);

                        ShowSaveSuccessMessage(response);
                    });
                }
            });

            if (!isPanel)
            {
                list.Add(new ToolButton
                {
                    Title = Texts.Controls.EntityDialog.DeleteButton,
                    CssClass = "delete-button",
                    OnClick = delegate
                    {
                        Q.Confirm(Texts.Controls.EntityDialog.DeleteConfirmation, delegate
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
                    Title = Texts.Controls.EntityDialog.UndeleteButton,
                    CssClass = "undo-delete-button",
                    OnClick = delegate
                    {
                        if (self.IsDeleted)
                        {
                            Q.Confirm(Texts.Controls.EntityDialog.UndeleteConfirmation, delegate()
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
                    Title = Texts.Controls.EntityDialog.LocalizationButton,
                    CssClass = "localization-button",
                    OnClick = delegate
                    {
                        this.LocalizationButtonClick();
                    }
                });

                list.Add(new ToolButton
                {
                    Title = Texts.Controls.EntityDialog.CloneButton,
                    CssClass = "clone-button",
                    OnClick = delegate
                    {
                        if (!self.IsEditMode)
                            return;

                        var cloneEntity = GetCloningEntity();
                        Widget.CreateOfType(
                            widgetType: this.GetType(),
                            options: new object(),
                            init: w => w.As<EntityDialog<TEntity, TOptions>>()
                                    .Cascade(this.element)
                                    .BubbleDataChange(this)
                                    .LoadEntityAndOpenDialog(cloneEntity));
                    }
                });
            }
            
            return list;
        }

        protected virtual TEntity GetCloningEntity()
        {
            var clone = new TEntity();
            clone = jQuery.Extend(clone, this.Entity).As<TEntity>().As<TEntity>();

            var idField = GetEntityIdField();
            if (!idField.IsEmptyOrNull())
                Script.Delete(clone, idField);

            var isActiveField = GetEntityIsActiveField();
            if (!isActiveField.IsEmptyOrNull())
                Script.Delete(clone, isActiveField);

            return clone;
        }

        protected virtual void UpdateInterface()
        {
            bool isDeleted = IsDeleted;
            bool isLocalizationMode = IsLocalizationMode;

            if (tabs != null)
                tabs.SetDisabled("Log", IsNewOrDeleted);

            if (propertyGrid != null)
                propertyGrid.Element.Toggle(!isLocalizationMode);

            if (localizationGrid != null)
                localizationGrid.Element.Toggle(isLocalizationMode);

            if (localizationButton != null)
            {
                localizationButton.Toggle(localizationGrid != null);

                localizationButton.Find(".button-inner").Text(IsLocalizationMode ? 
                    Texts.Controls.EntityDialog.LocalizationBack : 
                    Texts.Controls.EntityDialog.LocalizationButton);
            }

            if (isLocalizationMode)
            {
                if (this.toolbar != null)
                    this.toolbar.FindButton("tool-button").Not(".localization-hidden")
                        .AddClass(".localization-hidden").Hide();

                if (localizationButton != null)
                    localizationButton.Show();

                return;
            }

            this.toolbar.FindButton("localization-hidden")
                .RemoveClass("localization-hidden").Show();

            if (deleteButton != null)
                deleteButton.Toggle(IsEditMode && !isDeleted);

            if (undeleteButton != null)
                undeleteButton.Toggle(IsEditMode && isDeleted);

            if (saveAndCloseButton != null)
            {
                saveAndCloseButton.Toggle(!isDeleted);

                saveAndCloseButton.Find(".button-inner").Text(
                    IsNew ? Texts.Controls.EntityDialog.SaveButton : Texts.Controls.EntityDialog.UpdateButton);
            }

            if (applyChangesButton != null)
                applyChangesButton.Toggle(!isDeleted);

            if (cloneButton != null)
                cloneButton.Toggle(false);
        }
    }
}