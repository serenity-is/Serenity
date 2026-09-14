using ServerTypingsTest.LocalTexts;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void Generates_Texts_With_Nested_LocalTexts()
        {
            var generator = CreateGenerator(typeof(SiteTexts), typeof(Texts));
            generator.LocalTexts = true;
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "Texts.ts").Text;

            Assert.Contains("namespace texts", code);
            Assert.Contains("export declare namespace Site", code);
            Assert.Contains("export const Title: string;", code);
            Assert.Contains("export const Ok: string;", code);
            Assert.Contains("export const Texts: typeof texts", code);
            Assert.Contains("export const SiteTexts = Texts.Site;", code);
        }

        [Fact]
        public void Does_Not_Generate_Texts_When_LocalTexts_Disabled()
        {
            var generator = CreateGenerator(typeof(SiteTexts));
            var result = generator.Run();
            Assert.DoesNotContain(result, x => x.Filename == "Texts.ts");
        }

        [Fact]
        public void Generates_Texts_Using_LocalTextFilters_And_Unprefixed_Exports()
        {
            var fs = new MockFileSystem();
            fs.CreateDirectory("/app");
            fs.WriteAllText("/app/appsettings.json", /*lang=json*/ """
                { "LocalTextPackages": { "Custom": ["Custom"] } }
                """);

            var generator = CreateGenerator(typeof(SiteTexts), typeof(CustomTexts), typeof(UnprefixedTexts));
            generator.LocalTexts = true;
            generator.SetLocalTextFiltersFrom(fs, "/app/appsettings.json");
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "Texts.ts").Text;

            Assert.Contains("export const Something: string;", code);
            Assert.Contains("export const CustomTexts = Texts.Custom;", code);
            Assert.Contains("export const UnprefixedTexts = Texts;", code);
        }
    }
}

namespace ServerTypingsTest.LocalTexts
{
    [NestedLocalTexts(Prefix = "Site.")]
    public static class SiteTexts
    {
        public static LocalText Title = "Site Title";

        public static class Common_
        {
            public static LocalText Ok = "OK";
        }
    }

    [NestedLocalTexts]
    public static class Texts
    {
        public static LocalText MoreText = "More Text";
    }

    [NestedLocalTexts(Prefix = "Custom.")]
    public static class CustomTexts
    {
        public static LocalText Something = "Something";
    }

    [NestedLocalTexts]
    public static class UnprefixedTexts
    {
        public static LocalText Whatever = "Whatever";
    }
}
