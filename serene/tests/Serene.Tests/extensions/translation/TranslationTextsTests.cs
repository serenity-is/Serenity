namespace Serenity.Extensions;

public class TranslationTextsTests
{
    [Fact]
    public void AllTextKeys_Are_Accessible()
    {
        Assert.NotNull(TranslationTexts.AllTextsAlreadyTranslated);
        Assert.NotNull(TranslationTexts.Assembly);
        Assert.NotNull(TranslationTexts.CopySuccessMessage);
        Assert.NotNull(TranslationTexts.CopyFailMessage);
        Assert.NotNull(TranslationTexts.CopySourceTranslations);
        Assert.NotNull(TranslationTexts.CopyTargetTranslations);
        Assert.NotNull(TranslationTexts.CustomText);
        Assert.NotNull(TranslationTexts.EntityPlural);
        Assert.NotNull(TranslationTexts.HasTranslation);
        Assert.NotNull(TranslationTexts.Key);
        Assert.NotNull(TranslationTexts.OverrideConfirmation);
        Assert.NotNull(TranslationTexts.SaveChangesButton);
        Assert.NotNull(TranslationTexts.SaveSuccessMessage);
        Assert.NotNull(TranslationTexts.SourceLanguage);
        Assert.NotNull(TranslationTexts.SourceTargetLanguageSame);
        Assert.NotNull(TranslationTexts.SourceText);
        Assert.NotNull(TranslationTexts.TargetLanguage);
        Assert.NotNull(TranslationTexts.TargetLanguageRequired);
        Assert.NotNull(TranslationTexts.TargetText);
        Assert.NotNull(TranslationTexts.TranslateAllText);
        Assert.NotNull(TranslationTexts.TranslateText);
        Assert.NotNull(TranslationTexts.TranslateTextConfirmation);
        Assert.NotNull(TranslationTexts.TranslateTextDisabled);
        Assert.NotNull(TranslationTexts.UserTranslated);
    }
}
