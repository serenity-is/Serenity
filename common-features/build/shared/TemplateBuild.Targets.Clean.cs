#if IsTemplateBuild
namespace Build;

public static partial class Shared
{
    public static partial class Targets
    {
        public static void Clean()
        {
            CleanDirectory(TemplateZipFolder, ensure: true);
            CleanDirectory(VSIXOutputFolder, ensure: true);
        }
    }
}
#endif