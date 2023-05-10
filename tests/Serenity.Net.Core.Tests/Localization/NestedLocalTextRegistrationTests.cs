using Serenity.Localization;

namespace Serenity.Tests.Localization;

public class NestedLocalTextRegistrationTests
{
    [Fact]
    public void AddNestedTexts_ThrowsArgumentNull_IfNoLocalTextRegistry()
    {
        Assert.Throws<ArgumentNullException>(() =>
            NestedLocalTextRegistration.AddNestedTexts(registry: null, new MockTypeSource(typeof(TextsWithPrefixP))));
    }

    [Fact]
    public void AddNestedTexts_OnlyRunsOn_Types_From_TypeSource()
    {
        var registry = new MockLocalTextRegistry();
        registry.AddNestedTexts(new MockTypeSource(Array.Empty<Type>()));
        Assert.Empty(registry.AddedList);
    }

    [Fact]
    public void AddNestedTexts_Skips_Classes_Without_NestedLocalTextsAttribute()
    {
        var registry = new MockLocalTextRegistry();
        registry.AddNestedTexts(new MockTypeSource(typeof(NotNestedTexts), typeof(NestedTexts)));

        Assert.Empty(registry.AddedList.Where(x => x.key == "N.N1" || x.key == "NOT5"));
    }

    [Fact]
    public void AddNestedTexts_Uses_InvariantLanguage_And_NoPrefix_ByDefault()
    {
        var registry = new MockLocalTextRegistry();
        registry.AddNestedTexts(new MockTypeSource(typeof(NestedTexts)));
        Assert.Collection(registry.AddedList.OrderBy(x => x.key),
            x => Assert.Equal((LocalText.InvariantLanguageID, "Y.Y1", "1"), x),
            x => Assert.Equal((LocalText.InvariantLanguageID, "Y.Y2", "2"), x),
            x => Assert.Equal((LocalText.InvariantLanguageID, "Y.YI.Y3", "3"), x),
            x => Assert.Equal((LocalText.InvariantLanguageID, "Y4", "4"), x),
            x => Assert.Equal((LocalText.InvariantLanguageID, "Y4.Y5", "5"), x));
    }

    [Fact]
    public void AddNestedTexts_UsesLanguageIfSpecified()
    {
        var registry = new MockLocalTextRegistry();
        registry.AddNestedTexts(new MockTypeSource(typeof(TextsWithLanguageES)));

        Assert.Collection(registry.AddedList.OrderBy(x => x.key),
            x => Assert.Equal(("es", "Z.Z1", "1"), x),
            x => Assert.Equal(("es", "Z.Z2", "2"), x),
            x => Assert.Equal(("es", "Z.ZI.Z3", "3"), x),
            x => Assert.Equal(("es", "Z4", "4"), x));
    }

    [Fact]
    public void AddNestedTexts_UsesPrefixIfSpecified()
    {
        var registry = new MockLocalTextRegistry();
        registry.AddNestedTexts(new MockTypeSource(typeof(TextsWithPrefixP)));

        Assert.Collection(registry.AddedList.OrderBy(x => x.key),
            x => Assert.Equal((LocalText.InvariantLanguageID, "p.P.P1", "1"), x),
            x => Assert.Equal((LocalText.InvariantLanguageID, "p.P.P2", "2"), x),
            x => Assert.Equal((LocalText.InvariantLanguageID, "p.P.PI.P3", "3"), x),
            x => Assert.Equal((LocalText.InvariantLanguageID, "p.P4", "4"), x));
    }

    [Fact]
    public void AddNestedTexts_UsesLanguageAndPrefixIfSpecified()
    {
        var registry = new MockLocalTextRegistry();
        registry.AddNestedTexts(new MockTypeSource(typeof(TextsWithPrefixAndLanguage)));

        Assert.Collection(registry.AddedList.OrderBy(x => x.key),
            x => Assert.Equal(("jp", "x.X.X1", "1"), x),
            x => Assert.Equal(("jp", "x.X.X2", "2"), x),
            x => Assert.Equal(("jp", "x.X.XI.X3", "3"), x),
            x => Assert.Equal(("jp", "x.X4", "4"), x));
    }

    [Fact]
    public void AddNestedTexts_ReplacesLocalTextObjectsWithInitializedLocalTextObjects()
    {
        var registry = new MockLocalTextRegistry();
        registry.AddNestedTexts(new MockTypeSource(typeof(NestedTexts)));

        Assert.NotNull(((ILocalText)NestedTexts.Y.Y1).OriginalKey);
        Assert.NotNull(((ILocalText)NestedTexts.Y.Y2).OriginalKey);
        Assert.NotNull(((ILocalText)NestedTexts.Y.YI.Y3).OriginalKey);
        Assert.NotNull(((ILocalText)NestedTexts.Y4).OriginalKey);

        Assert.Equal("Y.Y1", NestedTexts.Y.Y1.Key);
        Assert.Equal("1", ((ILocalText)NestedTexts.Y.Y1).OriginalKey);
    }

    [Fact]
    public void AddNestedTexts_CanBeCalledMoreThanOnce()
    {
        var registry = new MockLocalTextRegistry();
        registry.AddNestedTexts(new MockTypeSource(typeof(NestedTexts)));
        registry.AddNestedTexts(new MockTypeSource(typeof(NestedTexts)));
    }

    internal static class NotNestedTexts
    {
        public static class N
        {
            public static LocalText N1 = "NOT5";
        }
    }

    [NestedLocalTexts]
    internal static class NestedTexts
    {
        public static class Y
        {
            public static LocalText Y1 = "1";
            public static LocalText Y2 = "2";

            public static class YI
            {
                public static LocalText Y3 = "3";
            }
        }

        public static LocalText Y4 = "4";

        // To prevent clash with Y4, append a underscore that will be removed when determining keys
        public class Y4_
        {
            public static LocalText Y5 = "5";
        }
    }

    [NestedLocalTexts(LanguageID = "es")]
    internal static class TextsWithLanguageES
    {
        public static class Z
        {
            public static LocalText Z1 = "1";
            public static LocalText Z2 = "2";

            public static class ZI
            {
                public static LocalText Z3 = "3";
            }
        }

        public static LocalText Z4 = "4";
    }

    [NestedLocalTexts(Prefix = "p.")]
    internal static class TextsWithPrefixP
    {
        public static class P
        {
            public static LocalText P1 = "1";
            public static LocalText P2 = "2";

            public static class PI
            {
                public static LocalText P3 = "3";
            }
        }

        public static LocalText P4 = "4";
    }

    [NestedLocalTexts(Prefix = "x.", LanguageID = "jp")]
    internal static class TextsWithPrefixAndLanguage
    {
        public static class X
        {
            public static LocalText X1 = "1";
            public static LocalText X2 = "2";

            public static class XI
            {
                public static LocalText X3 = "3";
            }
        }

        public static LocalText X4 = "4";
    }
}