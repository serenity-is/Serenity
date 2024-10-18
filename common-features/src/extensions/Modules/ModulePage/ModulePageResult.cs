using Microsoft.AspNetCore.Mvc;

namespace Serenity.Web;

public class ModulePageResult : ViewResult
{
    public new ModulePageModel Model => (ModulePageModel)base.Model;
}