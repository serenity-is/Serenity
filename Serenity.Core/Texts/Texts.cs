namespace Serenity
{
    public static partial class Texts
    {
        public static readonly LocalText.Package Package;

        static Texts()
        {
            Package = new LocalText.Package().InitializeTextClass(typeof(Texts));
        }
    }
}