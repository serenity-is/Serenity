using ServerTypingsTest;
using ServerTypingsTest.LocalTexts;
using ServerTypingsTest.TextsModule.Entities;

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
            Assert.Contains("export { SiteTexts } from \"./LocalTexts/SiteTexts\";", code);
        }

        [Fact]
        public void Generates_Nested_LocalTexts_Class_In_Its_Own_File_Under_Module_Directory()
        {
            var generator = CreateGenerator(typeof(SiteTexts));
            generator.LocalTexts = true;
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "LocalTexts/SiteTexts.ts").Text;

            Assert.Contains("import { proxyTexts } from \"@serenity-is/corelib\";", code);
            Assert.Contains("namespace texts", code);
            Assert.Contains("export declare namespace Site", code);
            Assert.Contains("export const Title: string;", code);
            Assert.Contains("export const SiteTexts: typeof texts.Site = /*#__PURE__*/ proxyTexts({}, 'Site.', {", code);
            Assert.Contains("Common: {}", code);
            Assert.DoesNotContain("Site: {", code);
        }

        [Fact]
        public void Texts_Ts_Does_Not_Contain_The_Tree_Without_A_Texts_Class_In_Compat_Mode()
        {
            var generator = CreateGenerator(typeof(SiteTexts));
            generator.LocalTexts = true;
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "Texts.ts").Text;

            Assert.DoesNotContain("proxyTexts", code);
            Assert.DoesNotContain("namespace texts", code);
            Assert.Equal("export { SiteTexts } from \"./LocalTexts/SiteTexts\";", code.Trim());
        }

        [Fact]
        public void Generates_Root_Namespace_Nested_LocalTexts_Class_Under_Texts_Directory()
        {
            var generator = CreateGenerator(typeof(RootLevelTexts));
            generator.LocalTexts = true;
            generator.ModuleReExports = true;
            var result = generator.Run();

            var code = Assert.Single(result, x => x.Filename == "Texts/RootLevelTexts.ts").Text;
            Assert.Contains("export const RootLevelTexts: typeof texts.Site.RootLevel = /*#__PURE__*/ proxyTexts({}, 'Site.RootLevel.', {}) as any;", code);
            Assert.Contains("import { proxyTexts } from \"@serenity-is/corelib\";", code);

            // no module re-export file is generated for the Texts directory, Texts.ts is the compat one
            var texts = Assert.Single(result, x => x.Filename == "Texts.ts").Text;
            Assert.Equal("export { RootLevelTexts } from \"./Texts/RootLevelTexts\";", texts.Trim());
        }

        [Fact]
        public void Does_Not_Generate_Texts_When_LocalTexts_Disabled()
        {
            var generator = CreateGenerator(typeof(SiteTexts));
            var result = generator.Run();
            Assert.DoesNotContain(result, x => x.Filename == "Texts.ts");
            Assert.DoesNotContain(result, x => x.Filename.EndsWith("Texts.ts", StringComparison.Ordinal));
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

            Assert.Contains("export { CustomTexts } from \"./LocalTexts/CustomTexts\";", code);
            Assert.Contains("export { UnprefixedTexts } from \"./LocalTexts/UnprefixedTexts\";", code);

            var custom = Assert.Single(result, x => x.Filename == "LocalTexts/CustomTexts.ts").Text;
            Assert.Contains("export const Something: string;", custom);
            Assert.Contains("export const CustomTexts: typeof texts.Custom = /*#__PURE__*/ proxyTexts({}, 'Custom.', {}) as any;", custom);

            // unprefixed class is an alias for the whole tree
            var unprefixed = Assert.Single(result, x => x.Filename == "LocalTexts/UnprefixedTexts.ts").Text;
            Assert.Contains("export const UnprefixedTexts: typeof texts = /*#__PURE__*/ proxyTexts({}, '', {", unprefixed);
            Assert.Contains("export const Something: string;", unprefixed);
            Assert.Contains("export const Title: string;", unprefixed);
        }

        [Fact]
        public void Unprefixed_Nested_LocalTexts_Class_Contains_Whole_Tree_But_Does_Not_Cover_Keys()
        {
            var generator = CreateGenerator(typeof(SiteTexts), typeof(UnprefixedSiteTexts), typeof(Texts),
                typeof(TextsModuleItemRow), typeof(OrphanPrefixRow));
            generator.LocalTexts = true;
            generator.TextsMode = ServerTypingsTextsMode.Remaining;
            var result = generator.Run();

            var code = Assert.Single(result, x => x.Filename == "LocalTexts/UnprefixedSiteTexts.ts").Text;
            Assert.Contains("export const UnprefixedSiteTexts: typeof texts = /*#__PURE__*/ proxyTexts({}, '', {", code);
            Assert.Contains("export const Unprefixed: string;", code);
            Assert.Contains("export const Title: string;", code);
            Assert.Contains("namespace TextsModule", code);

            // module Db texts and remaining texts are still determined by prefixed classes
            Assert.Single(result, x => x.Filename == "TextsModule/DbTexts.ts");
            var texts = Assert.Single(result, x => x.Filename == "Texts.ts").Text;
            Assert.Contains("namespace Orphan", texts);
            Assert.DoesNotContain("TextsModule", texts);
        }

        [Fact]
        public void Generates_Module_DbTexts_For_Db_Texts_Not_Covered_By_A_Class()
        {
            var generator = CreateGenerator(typeof(TextsModuleItemRow));
            generator.LocalTexts = true;
            generator.ModuleReExports = true;
            var result = generator.Run();

            var code = Assert.Single(result, x => x.Filename == "TextsModule/DbTexts.ts").Text;
            Assert.Contains("export const TextsModuleDbTexts: typeof texts.Db.TextsModule = /*#__PURE__*/ proxyTexts({}, 'Db.TextsModule.', {", code);
            Assert.Contains("TextsModuleItem: {}", code);
            Assert.Contains("export const EntityPlural: string;", code);

            var reExports = Assert.Single(result, x => x.Filename == "TextsModule.ts").Text;
            Assert.Contains("export * from \"./TextsModule/DbTexts\";", reExports);

            // module DbTexts are new, so they are not re-exported from compat Texts.ts
            var texts = Assert.Single(result, x => x.Filename == "Texts.ts").Text;
            Assert.DoesNotContain("TextsModuleDbTexts", texts);
        }

        [Fact]
        public void Maps_Db_Texts_To_Module_Directory_Case_Insensitively()
        {
            var generator = CreateGenerator(typeof(TextsModuleLowerCaseRow));
            generator.LocalTexts = true;
            var result = generator.Run();

            var code = Assert.Single(result, x => x.Filename == "TextsModule/DbTexts.ts").Text;
            Assert.Contains("export const TextsModuleDbTexts: typeof texts.Db.textsmodule = /*#__PURE__*/ proxyTexts({}, 'Db.textsmodule.', {", code);
        }

        [Fact]
        public void User_Defined_Db_Texts_Class_Takes_Precedence_Over_Module_DbTexts()
        {
            var generator = CreateGenerator(typeof(TextsModuleItemRow), typeof(ServerTypingsTest.TextsModule.TextsModuleDbTexts));
            generator.LocalTexts = true;
            var result = generator.Run();

            Assert.DoesNotContain(result, x => x.Filename == "TextsModule/DbTexts.ts");
            var code = Assert.Single(result, x => x.Filename == "TextsModule/TextsModuleDbTexts.ts").Text;
            Assert.Contains("export const TextsModuleDbTexts: typeof texts.Db.TextsModule = /*#__PURE__*/ proxyTexts({}, 'Db.TextsModule.', {", code);
            Assert.Contains("TextsModuleItem: {}", code);
        }

        [Fact]
        public void Class_With_Module_DbTexts_Name_Prevents_Module_DbTexts_Even_With_Another_Prefix()
        {
            var generator = CreateGenerator(typeof(TextsModuleItemRow), typeof(ServerTypingsTest.TextsModule.Other.TextsModuleDbTexts));
            generator.LocalTexts = true;
            var result = generator.Run();

            Assert.DoesNotContain(result, x => x.Filename == "TextsModule/DbTexts.ts");
        }

        [Fact]
        public void Db_Texts_Without_A_Matching_Module_Directory_Are_Not_Split()
        {
            var generator = CreateGenerator(typeof(OrphanPrefixRow), typeof(Texts));
            generator.LocalTexts = true;
            generator.TextsMode = ServerTypingsTextsMode.Remaining;
            var result = generator.Run();

            Assert.DoesNotContain(result, x => x.Filename.EndsWith("/DbTexts.ts", StringComparison.Ordinal));
            var code = Assert.Single(result, x => x.Filename == "Texts.ts").Text;
            Assert.Contains("namespace Orphan", code);
            Assert.Contains("export const Texts: typeof texts = /*#__PURE__*/ proxyTexts({}, '', {", code);
        }

        [Fact]
        public void Remaining_Mode_Does_Not_Generate_Texts_Ts_Without_A_Texts_Class()
        {
            var generator = CreateGenerator(typeof(SiteTexts), typeof(TextsModuleItemRow));
            generator.LocalTexts = true;
            generator.TextsMode = ServerTypingsTextsMode.Remaining;
            var result = generator.Run();

            Assert.DoesNotContain(result, x => x.Filename == "Texts.ts");
            Assert.Single(result, x => x.Filename == "LocalTexts/SiteTexts.ts");
            Assert.Single(result, x => x.Filename == "TextsModule/DbTexts.ts");
        }

        [Fact]
        public void Remaining_Mode_Texts_Only_Contains_Keys_Not_In_Other_Files()
        {
            var generator = CreateGenerator(typeof(SiteTexts), typeof(Texts), typeof(TextsModuleItemRow), typeof(OrphanPrefixRow));
            generator.LocalTexts = true;
            generator.TextsMode = ServerTypingsTextsMode.Remaining;
            var result = generator.Run();

            var code = Assert.Single(result, x => x.Filename == "Texts.ts").Text;
            Assert.Contains("namespace Orphan", code);
            Assert.DoesNotContain("namespace Site", code);
            Assert.DoesNotContain("TextsModule", code);
            Assert.DoesNotContain("export {", code);
        }

        [Fact]
        public void Remaining_Mode_Generates_Empty_Texts_When_Nothing_Remains()
        {
            var generator = CreateGenerator(typeof(SiteTexts), typeof(Texts));
            generator.LocalTexts = true;
            generator.TextsMode = ServerTypingsTextsMode.Remaining;
            var result = generator.Run();

            var code = Assert.Single(result, x => x.Filename == "Texts.ts").Text;
            Assert.Equal("export const Texts = {} as const;", code.Trim());
        }

        [Fact]
        public void Compat_Mode_Texts_Class_Contains_The_Full_Tree()
        {
            var generator = CreateGenerator(typeof(SiteTexts), typeof(Texts), typeof(TextsModuleItemRow));
            generator.LocalTexts = true;
            var result = generator.Run();

            var code = Assert.Single(result, x => x.Filename == "Texts.ts").Text;
            Assert.Contains("namespace Site", code);
            Assert.Contains("namespace TextsModule", code);
            Assert.Contains("export { SiteTexts } from \"./LocalTexts/SiteTexts\";", code);
        }

        [Fact]
        public void GeneratorConfig_Parses_TextsMode()
        {
            Assert.Equal(ServerTypingsTextsMode.Compat, new Serenity.CodeGenerator.GeneratorConfig.ServerTypingsConfig().GetTextsMode());
            Assert.Equal(ServerTypingsTextsMode.Remaining, new Serenity.CodeGenerator.GeneratorConfig.ServerTypingsConfig { TextsMode = "remaining" }.GetTextsMode());
            Assert.Equal(ServerTypingsTextsMode.Compat, new Serenity.CodeGenerator.GeneratorConfig.ServerTypingsConfig { TextsMode = "unknown" }.GetTextsMode());
        }
    }
}

namespace ServerTypingsTest
{
    [NestedLocalTexts(Prefix = "Site.RootLevel.")]
    public static class RootLevelTexts
    {
        public static LocalText Hello = "Hello";
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

    [NestedLocalTexts]
    public static class UnprefixedSiteTexts
    {
        public static class Site_
        {
            public static LocalText Unprefixed = "Unprefixed";
        }
    }
}

namespace ServerTypingsTest.TextsModule
{
    [NestedLocalTexts(Prefix = "Db.TextsModule.")]
    public static class TextsModuleDbTexts
    {
    }
}

namespace ServerTypingsTest.TextsModule.Other
{
    [NestedLocalTexts(Prefix = "Site.Other.")]
    public static class TextsModuleDbTexts
    {
        public static LocalText Other = "Other";
    }
}

namespace ServerTypingsTest.TextsModule.Entities
{
    [Module("TextsModule")]
    public class TextsModuleItemRow : Row<TextsModuleItemRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
        }
    }

    [LocalTextPrefix("textsmodule.Lower")]
    public class TextsModuleLowerCaseRow : Row<TextsModuleLowerCaseRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
        }
    }

    [LocalTextPrefix("Orphan.Thing")]
    public class OrphanPrefixRow : Row<OrphanPrefixRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
        }
    }
}
