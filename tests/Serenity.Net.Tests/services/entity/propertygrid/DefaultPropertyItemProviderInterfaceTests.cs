namespace Serenity.PropertyGrid;

/// <summary>
/// The property items of a form have the attributes of the members of the interfaces its properties implement, so a group of
/// properties can be shared between forms with their attributes, and each form places them where it declares them.
/// </summary>
public class DefaultPropertyItemProviderInterfaceTests
{
    public interface IContact
    {
        [DisplayName("E-mail"), EmailAddressEditor, Placeholder("someone@example.com"), Hint("Where we write to")]
        string? Email { get; set; }

        [DisplayName("Phone"), MaxLength(20), VisibleWhen(nameof(Email))]
        string? Phone { get; set; }
    }

    [FormScript("Test.InterfaceForm")]
    private class ContactForm : IContact
    {
        public string? Name { get; set; }

        [Placeholder("Your phone")]
        public string? Phone { get; set; }

        public string? Email { get; set; }
    }

    private static PropertyItem[] GetItems(Type type)
    {
        var services = new ServiceCollection();
        services.AddOptions();
        return [.. new DefaultPropertyItemProvider(services.BuildServiceProvider(), new MockTypeSource(typeof(BasicPropertyProcessor)))
            .GetPropertyItemsFor(type, null)];
    }

    [Fact]
    public void Items_AreInTheOrderTheFormDeclaresThem()
    {
        Assert.Equal(["Name", "Phone", "Email"], GetItems(typeof(ContactForm)).Select(x => x.Name));
    }

    [Fact]
    public void Items_HaveTheTitlesEditorsHintsAndPlaceholdersOfTheInterfaceMembers()
    {
        var email = GetItems(typeof(ContactForm)).Single(x => x.Name == "Email");

        Assert.Equal("Forms.Test.InterfaceForm.Email", email.Title);
        Assert.Equal("EmailAddress", email.EditorType);
        Assert.Equal("Forms.Test.InterfaceForm.Email_Placeholder", email.Placeholder);
        Assert.Equal("Forms.Test.InterfaceForm.Email_Hint", email.Hint);
    }

    [Fact]
    public void OwnAttributes_OfTheForm_WinOverTheOnesOfTheInterface()
    {
        var phone = GetItems(typeof(ContactForm)).Single(x => x.Name == "Phone");

        Assert.Equal("Forms.Test.InterfaceForm.Phone_Placeholder", phone.Placeholder);
        Assert.Equal(20, phone.MaxLength);
        Assert.Equal("Email", Assert.Single(phone.VisibleWhen!).Field);
    }
}
