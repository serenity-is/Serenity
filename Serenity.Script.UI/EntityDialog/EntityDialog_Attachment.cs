using System;
using System.Collections;

namespace Sinerji
{
    public partial class SiteEntityDialog : TemplatedDialog
    {
        private Query _attachmentPanel;

        protected Query AttachmentPanel
        {
            get { return _attachmentPanel; }
        }

        protected void RefreshAttachmentPanelIfRequired()
        {
            if (_attachmentPanel != null &&
                Utils.IsTrue(_attachmentPanel.Data("needsRefresh")))
            {
                _attachmentPanel.data("needsRefresh", false);
                _attachmentPanel.ntAttachmentPanel("refresh");
            }
        }

        protected virtual JsDictionary GetAttachmentPanelOptions()
        {
            return Utils.CreateDictionary(
                "entityTypeId", GetEntityType(),
                "entityId", (object)EntityId ?? null,
                "readOnly", this.EntityObject != null && this.IsEntityDeleted(this.EntityObject));
        }

        public virtual void SetAttachmentPanelOptions()
        {
            JsDictionary opt = GetAttachmentPanelOptions();
            foreach (string k in opt.Keys)
                _attachmentPanel.ntAttachmentPanel("option", k, opt[k]);
        }

        protected virtual void AfterLoadEntity_AttachmentPanel()
        {
            UpdateAttachmentHints(EntityObject);

            if (_attachmentPanel != null)
            {
                _attachmentPanel.ntAttachmentPanel("clearPending");
                SetAttachmentPanelOptions();
                _attachmentPanel.data("needsRefresh", true).triggerHandler("shown");
            }
        }

        protected virtual int GetAttachmentCount(object entity)
        {
            return Type.GetField(entity, "AttachmentCount").As<int>();
        }

        protected virtual string GetAttachmentTabText(int attCount)
        {
            if (attCount > 0)
                return "Dosya ve Notlar (" + attCount + ")";
            else
                return "Dosya ve Notlar";
        }

        private void UpdateAttachmentHints(object entity)
        {
            entity = entity ?? new JsDictionary();
            if (this._tabIndex != null && (object)this._tabIndex["Notes"] != null)
            {
                Query tab = this.ById("Tabs").children("ul").children().eq(this._tabIndex["Notes"]);
                Query spn = tab.find("span");
                Query inf = this.ById("AttachmentHint");

                int attCount = GetAttachmentCount(entity);
                spn.text(GetAttachmentTabText(attCount));

                if (tab.Is(".ui-state-disabled"))
                    inf.html("").hide();
                else
                {
                    inf.html("").show().removeClass("has-attachment");
                    string txt = "";
                    if (attCount > 0) {
                        inf.addClass("has-attachment");
                        txt = "<b>Dosya ve Notlar</b> sekmesinde bu kayda bağlı <b>" +
                            attCount + "</b> adet eklenti var.";
                    }
                    else
                        txt = "Kayda dosya veya not eklemek için <b>Dosya ve Notlar</b> sekmesini kullanınız.";

                    J.Query("<a/>")
                        .attr("href", "#")
                        .html(txt)
                        .appendTo(inf)
                        .click(delegate(jEvent e)
                    {
                        e.preventDefault();
                        tab.find("a").click();
                    });
                }
            }
        }

        protected virtual JsDictionary GetAttachmentPanelInitOptions()
        {
            return GetAttachmentPanelOptions();
        }

        protected virtual void InitAttachmentPanel()
        {
            Query div = this.ById("AttachmentPanel");
            if (div.length == 0)
                return;

            Utils.ExecuteOnceWhenShown(div, CreateAttachmentPanel);
        }

        protected virtual void CreateAttachmentPanel()
        {
            _attachmentPanel = this.ById("AttachmentPanel");
            _attachmentPanel.ntAttachmentPanel(GetAttachmentPanelInitOptions())
                .data("needsRefresh", false)
                .triggerHandler("arrange");

            Utils.ExecuteAgainWhenShown(_attachmentPanel, RefreshAttachmentPanelIfRequired, false);
        }

        protected AttachmentEntity[] GetPendingAttachments()
        {
            if ((!EntityId.HasValue ||
                 EntityId.As<long>() < 0) &&
                _attachmentPanel != null)
            {
                AttachmentEntity[] att = (AttachmentEntity[])_attachmentPanel.NtAttachmentPanel("pendingItems");
                if (att != null && att.Length > 0)
                    return att;
            }
            return null;
        }
    }
}