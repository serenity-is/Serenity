using Serenity.Localization;

namespace Serenity
{
    public static partial class Texts
    {
        public static readonly LocalTextPackage Package;

        static Texts()
        {
            Package = new LocalTextPackage().InitializeTextClass(typeof(Texts));
        }
    }
}