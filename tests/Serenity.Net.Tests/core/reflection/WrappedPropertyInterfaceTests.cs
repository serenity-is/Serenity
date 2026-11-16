namespace Serenity.Reflection;

/// <summary>
/// A property inherits the attributes of the members of the interfaces it implements, like the ones of its base classes.
/// </summary>
public class WrappedPropertyInterfaceTests
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    private class TitleAttribute(string text) : Attribute
    {
        public string Text { get; } = text;
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    private class TagAttribute(string name) : Attribute
    {
        public string Name { get; } = name;
    }

    private class CombinedAttribute : Attribute, IIntrinsicPropertyAttributeProvider
    {
        [Title("Combined"), Tag("FromCombined")]
        public object PropertyAttributes { get; set; } = null!;
    }

    public interface IAddress
    {
        [Title("Street"), Tag("Address")]
        string? Street { get; set; }

        [Combined]
        string? City { get; set; }

        [Title("Country")]
        string? Country { get; }
    }

    public interface IOther
    {
        [Title("Other Street")]
        string? Street { get; set; }
    }

    private class Customer : IAddress
    {
        public string? Street { get; set; }

        [Title("Town"), Tag("Own")]
        public string? City { get; set; }

        public string? Country { get; set; }

        public string? Name { get; set; }
    }

    private class Ambiguous : IAddress, IOther
    {
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }

    private class Explicit : IAddress
    {
        [Title("Own Street")]
        public string? Street { get; set; }

        string? IAddress.City { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }
    }

    private class AddressBase
    {
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }

    private class InheritsAddress : AddressBase, IAddress
    {
    }

    private static WrappedProperty Get<T>(string name) => new(typeof(T).GetProperty(name)!);

    [Fact]
    public void Property_InheritsTheAttributesOfTheInterfaceMember_ItImplements()
    {
        var street = Get<Customer>(nameof(Customer.Street));

        Assert.Equal("Street", street.GetAttribute<TitleAttribute>()?.Text);
        Assert.Equal(["Address"], street.GetAttributes<TagAttribute>().Select(x => x.Name));
        Assert.Equal("Country", Get<Customer>(nameof(Customer.Country)).GetAttribute<TitleAttribute>()?.Text);
    }

    [Fact]
    public void OwnAttribute_WinsOverTheOneOfTheInterface_WhileTheOnesAllowingMultipleAreAdded()
    {
        var city = Get<Customer>(nameof(Customer.City));

        // the own Title wins over the one the combined attribute of the interface member has
        Assert.Equal("Town", city.GetAttribute<TitleAttribute>()?.Text);
        Assert.Equal(["Own", "FromCombined"], city.GetAttributes<TagAttribute>().Select(x => x.Name));
    }

    [Fact]
    public void IntrinsicAttributes_OfTheInterfaceMember_AreOnlyIncluded_WithTheIntrinsicOrigin()
    {
        var city = Get<InheritsAddress>(nameof(InheritsAddress.City));

        Assert.Equal("Combined", city.GetAttribute<TitleAttribute>()?.Text);
        Assert.Null(city.GetAttribute<TitleAttribute>(AttributeOrigin.Inherit));
        Assert.NotNull(city.GetAttribute<CombinedAttribute>(AttributeOrigin.Inherit));
    }

    [Fact]
    public void InterfaceAttributes_AreInherited_SoTheyAreNotIncluded_WithoutTheInheritOrigin()
    {
        var street = Get<Customer>(nameof(Customer.Street));

        Assert.Null(street.GetAttribute<TitleAttribute>(AttributeOrigin.Explicit));
        Assert.Null(street.GetAttribute<TitleAttribute>(AttributeOrigin.ExcludeInherit));
        Assert.Empty(street.GetAttributes<TagAttribute>(AttributeOrigin.Intrinsic));
    }

    [Fact]
    public void AllAttributes_IncludeTheOnesOfTheInterface_EvenWhenThePropertyHasSomeItself()
    {
        var city = Get<Customer>(nameof(Customer.City));

        var types = city.GetAttributes<Attribute>().Select(x => x.GetType()).ToList();
        Assert.Contains(typeof(CombinedAttribute), types);
        Assert.Equal(1, types.Count(x => x == typeof(TitleAttribute)));
    }

    [Fact]
    public void PropertyWithoutAnInterface_HasOnlyItsOwnAttributes()
    {
        Assert.Empty(Get<Customer>(nameof(Customer.Name)).GetAttributes<Attribute>());
        Assert.Empty(WrappedProperty.GetImplementedInterfaceProperties(typeof(Customer).GetProperty(nameof(Customer.Name))!));
    }

    [Fact]
    public void PropertyOfABaseClass_ImplementsTheInterfaceOfTheTypeItIsObtainedFrom()
    {
        Assert.Equal("Street", Get<InheritsAddress>(nameof(InheritsAddress.Street)).GetAttribute<TitleAttribute>()?.Text);
        // the base class doesn't implement the interface
        Assert.Null(Get<AddressBase>(nameof(AddressBase.Street)).GetAttribute<TitleAttribute>());
    }

    [Fact]
    public void ExplicitImplementation_IsNotThePublicProperty_WithTheSameName()
    {
        var city = Get<Explicit>(nameof(Explicit.City));

        Assert.Null(city.GetAttribute<TitleAttribute>());
        Assert.Empty(WrappedProperty.GetImplementedInterfaceProperties(typeof(Explicit).GetProperty(nameof(Explicit.City))!));
        Assert.Equal("Own Street", Get<Explicit>(nameof(Explicit.Street)).GetAttribute<TitleAttribute>()?.Text);
    }

    [Fact]
    public void TwoInterfaces_GivingAnAttributeOfTheSameType_AreAmbiguous()
    {
        var street = Get<Ambiguous>(nameof(Ambiguous.Street));

        Assert.Equal(2, WrappedProperty.GetImplementedInterfaceProperties(typeof(Ambiguous).GetProperty(nameof(Ambiguous.Street))!).Length);
        Assert.Throws<AmbiguousMatchException>(() => street.GetAttribute<TitleAttribute>());
    }

    [Fact]
    public void ImplementedInterfaceProperties_AreOrderedByTheNamesOfTheInterfaces()
    {
        var properties = WrappedProperty.GetImplementedInterfaceProperties(typeof(Ambiguous).GetProperty(nameof(Ambiguous.Street))!);

        Assert.Equal([typeof(IAddress), typeof(IOther)], properties.Select(x => x.DeclaringType));
    }
}
