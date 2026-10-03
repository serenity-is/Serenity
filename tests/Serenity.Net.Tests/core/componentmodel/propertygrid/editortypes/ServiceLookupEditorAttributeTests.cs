namespace Serenity.ComponentModel;

public class ServiceLookupEditorAttributeTests
{
    private class TestRow
    {
    }

    [Module("Northwind")]
    private class ModuleRow
    {
    }

    private class EntitiesRow
    {
    }

    private class MyLookup
    {
    }

    private class CustomServiceLookupEditorAttribute : ServiceLookupEditorAttribute
    {
        public CustomServiceLookupEditorAttribute()
        {
            Service = "Custom/Items/List";
        }
    }

    [Fact]
    public void ProtectedParameterlessCtor_AllowsDerivedEditorAttribute()
    {
        var attr = new CustomServiceLookupEditorAttribute();

        Assert.Equal("Custom/Items/List", attr.Service);
    }

    [Fact]
    public void Ctor_WithServiceIdAndTextField_SetsOptions()
    {
        var attr = new ServiceLookupEditorAttribute("Northwind/Customer/List", "ID", "Name");

        var dict = new Dictionary<string, object?>();
        attr.SetParams(dict);

        Assert.Equal("Northwind/Customer/List", dict["service"]);
        Assert.Equal("ID", dict["idField"]);
        Assert.Equal("Name", dict["textField"]);
    }

    [Fact]
    public void Ctor_WithItemType_SetsItemType()
    {
        var attr = new ServiceLookupEditorAttribute(typeof(TestRow));
        Assert.Equal(typeof(TestRow), attr.ItemType);
    }

    [Fact]
    public void Ctor_WithNullItemType_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ServiceLookupEditorAttribute(null));
    }

    [Fact]
    public void AutoServiceFor_UsesModuleAttribute()
    {
        var service = ServiceLookupEditorAttribute.AutoServiceFor(typeof(ModuleRow));
        Assert.Equal("Northwind/Module/List", service);
    }

    [Fact]
    public void AutoServiceFor_RemovesEntitiesSuffix()
    {
        var service = ServiceLookupEditorAttribute.AutoServiceFor(typeof(EntitiesRow));
        Assert.Equal("ComponentModel/Entities/List", service);
    }

    [Fact]
    public void AutoServiceFor_RemovesLookupSuffix()
    {
        var service = ServiceLookupEditorAttribute.AutoServiceFor(typeof(MyLookup));
        Assert.Equal("ComponentModel/My/List", service);
    }

    [Fact]
    public void AutoServiceFor_UsesEndpointClassRoute()
    {
        var service = ServiceLookupEditorAttribute.AutoServiceFor(
            typeof(Serenity.Net.Tests.AutoServiceFor.DataSources.RouteOnlyRow));

        Assert.Equal("DataSources/RouteOnly/List", service);
    }

    [Fact]
    public void NavigationLink_UsesSharedRouteHelper()
    {
        var url = Serenity.Navigation.NavigationLinkAttribute.GetUrlFromController(
            typeof(Serenity.Net.Tests.AutoServiceFor.DataSources.RouteOnlyEndpoint), "List");

        Assert.Equal("~/Services/DataSources/RouteOnly/List", url);
    }

    [Fact]
    public void GetUrlFromController_ReturnsNullForMissingActionWhenThrowIfAbsentIsFalse()
    {
        var url = Serenity.Navigation.NavigationItemAttribute.GetUrlFromController(
            typeof(Serenity.Net.Tests.AutoServiceFor.DataSources.RouteOnlyEndpoint),
            "Missing", throwIfAbsent: false);

        Assert.Null(url);
    }

    [Fact]
    public void AutoServiceFor_PrefersListLookupAndUsesItsRoute()
    {
        var service = ServiceLookupEditorAttribute.AutoServiceFor(
            typeof(Serenity.Net.Tests.AutoServiceFor.DataSources.ConnectionRow));

        Assert.Equal("DataSources/Connection/Lookup", service);
    }

    [Fact]
    public void AutoServiceFor_UsesListLookupWhenItIsTheOnlyListMethod()
    {
        var service = ServiceLookupEditorAttribute.AutoServiceFor(
            typeof(Serenity.Net.Tests.AutoServiceFor.DataSources.LookupOnlyRow));

        Assert.Equal("DataSources/LookupOnly/ListLookup", service);
    }

    [Fact]
    public void TryGetServiceFromEndpoint_DoesNotAppendListToExplicitActionRoute()
    {
        var service = ServiceLookupEditorAttribute.TryGetServiceFromEndpoint(
            typeof(Serenity.Net.Tests.AutoServiceFor.DataSources.CustomActionEndpoint), "Search");

        Assert.Equal("DataSources/CustomAction/Search", service);
    }

    [Fact]
    public void AutoServiceFor_UsesAbsoluteUrlForNonServicesRoute()
    {
        var service = ServiceLookupEditorAttribute.AutoServiceFor(
            typeof(Serenity.Net.Tests.AutoServiceFor.Reports.CustomerRow));

        Assert.Equal("~/Reports/Customer/List", service);
    }

    [Fact]
    public void AutoServiceFor_RemovesEntitiesNamespaceBeforeFindingEndpoint()
    {
        var service = ServiceLookupEditorAttribute.AutoServiceFor(
            typeof(Serenity.Net.Tests.AutoServiceFor.Entities.CustomerRow));

        Assert.Equal("Customer/List", service);
    }

    [Fact]
    public void AutoServiceFor_FallsBackForComplexEndpointRoute()
    {
        var service = ServiceLookupEditorAttribute.AutoServiceFor(
            typeof(Serenity.Net.Tests.AutoServiceFor.Complex.ComplexRow));

        Assert.Equal("Fallback/Complex/List", service);
    }
}