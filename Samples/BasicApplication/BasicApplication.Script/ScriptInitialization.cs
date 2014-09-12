using Serenity;
using System.Html;

namespace BasicApplication
{
    public static class ScriptInitialization
    {
        static ScriptInitialization()
        {
            Q.Config.RootNamespaces.Add("BasicApplication");
        }
    }
}