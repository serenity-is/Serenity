using jQueryApi;
using jQueryApi.UI.Widgets;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity.Reporting
{
    public class ReportDialog : TemplatedDialog<ReportDialogOptions>
    {
        private List<PropertyItem> propertyItems;
        private PropertyGrid propertyGrid;
        private string reportKey;

        public ReportDialog(ReportDialogOptions opt)
            : base(opt)
        {
            if (opt.ReportKey != null)
                LoadReport(opt.ReportKey);
        }

        protected void CreatePropertyGrid()
        {
            if (propertyGrid != null)
            {
                this.ById("PropertyGrid").Html("").Attribute("class", "");
                propertyGrid = null;
            }

            propertyGrid = new PropertyGrid(this.ById("PropertyGrid"), new PropertyGridOptions
            {
                IdPrefix = this.IdPrefix,
                UseCategories = true,
                Items = propertyItems
            });
        }

        public void LoadReport(string reportKey)
        {
            Q.ServiceCall(new ServiceCallOptions<ReportRetrieveResponse>
            {
                Service = "Report/Retrieve",
                Request = new ReportRetrieveRequest
                {
                    ReportKey = reportKey
                },
                OnSuccess = response =>
                {
                    this.reportKey = response.ReportKey ?? reportKey;
                    this.propertyItems = response.Properties ?? new List<PropertyItem>();
                    this.Element.Dialog().Title = response.Title;
                    CreatePropertyGrid();
                    propertyGrid.Load(response.InitialSettings ?? new object());

                    this.toolbar.FindButton("print-preview-button").Toggle(!response.IsDataOnlyReport);
                    this.toolbar.FindButton("export-pdf-button").Toggle(!response.IsDataOnlyReport);
                    this.toolbar.FindButton("export-docx-button").Toggle(!response.IsDataOnlyReport);

                    this.DialogOpen();
                }
            });
        }

        protected void ExecuteReport(string targetFrame, string exportType)
        {
            if (!ValidateForm())
                return;

            var parameters = new object();
            propertyGrid.Save(parameters);

            Q.Externals.PostToService(new PostToServiceOptions
            {
                Service = "Report/Execute",
                Request = new ReportExecuteRequest
                {
                    ReportKey = reportKey,
                    DesignId = "Default",
                    ExportType = exportType,
                    Parameters = parameters
                },
                Target = targetFrame
            });
        }

        protected override List<ToolButton> GetToolbarButtons()
        {
            return new List<ToolButton> 
            {
                new ToolButton 
                { 
                    Title = "Önizleme", 
                    CssClass = "print-preview-button", 
                    OnClick = delegate 
                    {
                        ExecuteReport("_blank", null);
                    }
                },
                new ToolButton 
                {
                    Title = "PDF",
                    CssClass = "export-pdf-button",
                    OnClick = delegate 
                    {
                        ExecuteReport("", "Pdf");
                    }
                },
                new ToolButton 
                {
                    Title = "Excel",
                    CssClass = "export-xlsx-button",
                    OnClick = delegate 
                    {
                        ExecuteReport("", "Xlsx");
                    }
                },
                new ToolButton 
                {
                    Title = "Word",
                    CssClass = "export-docx-button",
                    OnClick = delegate 
                    {
                        ExecuteReport("", "Docx");
                    }
                }
            };
        }
    }

    [Imported, Serializable]
    public class ReportDialogOptions
    {
        public string ReportKey { get; set; }
    }
}