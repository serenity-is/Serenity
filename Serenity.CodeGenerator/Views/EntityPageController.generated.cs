namespace Serenity.CodeGenerator.Views
{
    using System;

    public class EntityPageController : RazorGenerator.Templating.RazorTemplateBase
    {
        public dynamic Model { get; set; }

        public override void Execute()
        {
            var dotModule = Model.Module == null ? "" : ("." + Model.Module);
            var modulePath = Model.Module ?? Model.RootNamespace;
            var modulePrefix = (Model.Module == null ? "" : (Model.Module + "/"));

            WriteLiteral(Environment.NewLine);
            WriteLiteral(Environment.NewLine);
            WriteLiteral("[assembly:Serenity.Navigation.NavigationLink(int.MaxValue, \"");
            WriteLiteral(modulePrefix);
            WriteLiteral(Model.ClassName);
            WriteLiteral("\", typeof(");
            WriteLiteral(Model.RootNamespace);
            WriteLiteral(dotModule);
            WriteLiteral(".Pages.");
            WriteLiteral(Model.ClassName);
            WriteLiteral("Controller))]");
            WriteLiteral(Environment.NewLine);
            WriteLiteral(Environment.NewLine);
            WriteLiteral("namespace ");
            WriteLiteral(Model.RootNamespace);
            WriteLiteral(dotModule);
            WriteLiteral(".Pages");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("{");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("    using Serenity;");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("    using Serenity.Web;");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("    using Microsoft.AspNetCore.Mvc;");
            WriteLiteral(Environment.NewLine);
            WriteLiteral(Environment.NewLine);
            WriteLiteral("    [PageAuthorize(typeof(Entities.");
            WriteLiteral(Model.RowClassName);
            WriteLiteral("))]");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("    public class ");
            WriteLiteral(Model.ClassName);
            WriteLiteral("Controller : Controller");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("    {");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("        [Route(\"");
            WriteLiteral(modulePrefix);
            WriteLiteral(Model.ClassName);
            WriteLiteral("\")]");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("        public ActionResult Index()");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("        {");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("            return View(\"~/Modules/");
            WriteLiteral(modulePath);
            WriteLiteral("/");
            WriteLiteral(Model.ClassName);
            WriteLiteral("/");
            WriteLiteral(Model.ClassName);
            WriteLiteral("Index.cshtml\");");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("        }");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("    }");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("}");
        }
    }
}