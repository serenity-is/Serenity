namespace Serene.Administration.Repositories;

public class TranslationRepositoryTests
{
    [FormScript("CategoryKeys")]
    private class FormWithCategories
    {
        [Category("Site.LegacyCategory")]
        public string? LegacyKey { get; set; }

        [CategoryName("Site.LiteralCategory")]
        public string? Literal { get; set; }

        [CategoryName("Invariant category", TextKey = "Site.ExplicitCategory")]
        public string? ExplicitKey { get; set; }

        [CategoryName(TextKey = "Site.KeyOnlyCategory")]
        public string? KeyOnly { get; set; }
    }

    [Fact]
    public void GetAllAvailableLocalTextKeys_Resolves_Form_Category_Attributes()
    {
        var repository = new TranslationRepository(new NullRequestContext(), null!,
            new LocalTextRegistry(), new MockTypeSource(typeof(FormWithCategories)));

        var keys = repository.GetAllAvailableLocalTextKeys();

        Assert.Contains("Site.LegacyCategory", keys);
        Assert.Contains("Forms.CategoryKeys.Categories.Site.LiteralCategory", keys);
        Assert.Contains("Site.ExplicitCategory", keys);
        Assert.Contains("Site.KeyOnlyCategory", keys);
    }
}