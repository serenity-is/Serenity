namespace Serenity.CodeGenerator.Views
{
    using System;

    public class EntityPageIndex : RazorGenerator.Templating.RazorTemplateBase
    {
        public dynamic Model { get; set; }

        public override void Execute()
        {
            var dotModule = Model.Module == null ? "" : ("." + Model.Module);

            WriteLiteral(Environment.NewLine);
            WriteLiteral("@{");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("    ViewData[\"Title\"] = Serenity.LocalText.Get(\"Db");
            Write(dotModule);
            WriteLiteral(".");
            Write(Model.ClassName);
            WriteLiteral(".EntityPlural\");");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("}");
            WriteLiteral(Environment.NewLine);
            WriteLiteral(Environment.NewLine);
            WriteLiteral("<div id=\"GridDiv\"></div>");
            WriteLiteral(Environment.NewLine);
            WriteLiteral(Environment.NewLine);
            WriteLiteral("<script type=\"text/javascript\">");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("    jQuery(function () {");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("        new ");
            Write(Model.RootNamespace);
            Write(dotModule);
            WriteLiteral(".");
            Write(Model.ClassName);
            WriteLiteral("Grid($(\'#GridDiv\'), {}).init();");
            WriteLiteral(Environment.NewLine);
            WriteLiteral(Environment.NewLine);
            WriteLiteral("        Q.initFullHeightGridPage($(\'#GridDiv\'));");
            WriteLiteral(Environment.NewLine + "    });");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("</script>");
        }
    }
}