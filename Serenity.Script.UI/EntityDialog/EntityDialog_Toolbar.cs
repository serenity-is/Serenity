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
            Q.NotifySuccess(Q.Text("Controls.EntityDialog.SaveSuccessMessage"));
        }

        protected override List<ToolButton> GetToolbarButtons()
        {
            List<ToolButton> list = new List<ToolButton>();

            var self = this;

            if (!isPanel)
            {
                list.Add(new ToolButton
                {
                    Title = Q.Text("Controls.EntityDialog.SaveButton"),
                    CssClass = "save-and-close-button",
                    Hotkey = "alt+s",
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
                Title = isPanel ? Q.Text("Controls.EntityDialog.SaveButton") : "",
                Hint = isPanel ? Q.Text("Controls.EntityDialog.SaveButton") : Q.Text("Controls.EntityDialog.ApplyChangesButton"),
                CssClass = "apply-changes-button",
                Hotkey = "alt+a",
                OnClick = delegate
                {
                    self.Save(delegate(ServiceResponse response)
                    {
                        if (self.IsEditMode)
                            self.LoadById(response.As<SaveResponse>().EntityId ?? self.EntityId, null);
                        else
                            self.LoadById(response.As<SaveResponse>().EntityId, null);

                        ShowSaveSuccessMessage(response);
                    });
                }
            });

            if (!isPanel)
            {
                list.Add(new ToolButton
                {
                    Title = Q.Text("Controls.EntityDialog.DeleteButton"),
                    CssClass = "delete-button",
                    Hotkey = "alt+x",
                    OnClick = delegate
                    {
                        Q.Confirm(Q.Text("Controls.EntityDialog.DeleteConfirmation"), delegate
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
                    Title = Q.Text("Controls.EntityDialog.UndeleteButton"),
                    CssClass = "undo-delete-button",
                    OnClick = delegate
                    {
                        if (self.IsDeleted)
                        {
                            Q.Confirm(Q.Text("Controls.EntityDialog.UndeleteConfirmation"), delegate()
                            {
                                self.Undelete(delegate
                                {
                                    self.LoadById(self.EntityId, null);
                                });
                            });
                        }
                    }
                });

                list.Add(new ToolButton
                {
                    Title = Q.Text("Controls.EntityDialog.LocalizationButton"),
                    CssClass = "localization-button",
                    OnClick = delegate
                    {
                        this.LocalizationButtonClick();
                    }
                });

                list.Add(new ToolButton
                {
                    Title = Q.Text("Controls.EntityDialog.CloneButton"),
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
            var clone = new object().As<TEntity>();
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
                    Q.Text("Controls.EntityDialog.LocalizationBack") : 
                    Q.Text("Controls.EntityDialog.LocalizationButton"));
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
                    Q.Text(IsNew ? "Controls.EntityDialog.SaveButton" : "Controls.EntityDialog.UpdateButton"));
            }

            if (applyChangesButton != null)
                applyChangesButton.Toggle(!isDeleted);

            if (cloneButton != null)
                cloneButton.Toggle(false);
        }
    }
}