namespace Serenity.Reporting
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Html;
    using jQueryApi;

    public class ReportDesignPanel : TemplatedWidget<ReportDesignPanelOptions>
    {
        public ReportDesignPanel(jQueryObject element, ReportDesignPanelOptions options)
            : base(element, options)
        {
            element.AddClass("s-ReportDesignPanel");

            this.ById("AddButton").Click(AddButtonClick);
            this.ById("EditButton").Click(EditButtonClick);

            this.Element.Find("a").Toggle(false);//Utils.HasRight("ReportDesign"));
        }

        public void AddButtonClick(jQueryEvent e)
        {
            e.PreventDefault();

            //if (String.IsNullOrEmpty(ReportKey))
            //    return;

            //var dialog = new ReportDesignDialog(new ReportDesignDialogOptions());
            //Utils.BindToDataChange(dialog.Element, delegate
            //{
            //    RefreshDesignList();
            //});
            //dialog.LoadDataAndOpenDialog(new RetrieveResponse
            //{
            //    Entity = new ReportDesignEntity
            //    {
            //        ReportKey = ReportKey,
            //        ReportDesign = this.ById("DesignList").children("option").length > 0 ? "" : "[Varsayılan]"
            //    }
            //});

        }

        public void EditButtonClick(jQueryEvent e)
        {
            e.PreventDefault();

            //var designId = SelectedDesignId;
            //if (designId == null)
            //    return;

            //var dialog = new ReportDesignDialog(new ReportDesignDialogOptions());
            //Utils.BindToDataChange(dialog.Element, delegate
            //{
            //    RefreshDesignList();
            //});
            //dialog.LoadByIdAndOpenDialog(designId.Value);

        }

        public void RefreshDesignList()
        {
            Q.ServiceCall(new ServiceCallOptions<ListResponse<ReportDesignItem>>
            {
                Service = "ReportDesign/List",
                Request = new ReportDesignListRequest
                {
                    ReportKey = this.ReportKey
                },
                OnSuccess = r =>
                {
                    SetDesignList(r.Entities);
                }
            });
        }

        public void SetDesignList(List<ReportDesignItem> designs)
        {
            var dl = this.ById("DesignList");
            Q.ClearOptions(dl);

            foreach (var design in designs)
                Q.AddOption(dl, design.DesignId, design.DesignId);
        }

        public String ReportKey { get; set; }

        public String SelectedDesignId
        {
            get { return this.ById("DesignList").GetValue().TrimToNull(); }
        }
    }

    [Imported, Serializable]
    public class ReportDesignPanelOptions
    {
    }
}