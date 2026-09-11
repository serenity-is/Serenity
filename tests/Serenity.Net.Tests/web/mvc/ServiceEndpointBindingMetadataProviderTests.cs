using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Serenity.Services;

public class ServiceEndpointBindingMetadataProviderTests
{
    private static BindingMetadataProviderContext CreateContext(Type modelType)
    {
        var modelAttributes = (ModelAttributes)Activator.CreateInstance(typeof(ModelAttributes),
            BindingFlags.Instance | BindingFlags.NonPublic, null, [new List<object>()], null)!;

        return new BindingMetadataProviderContext(
            ModelMetadataIdentity.ForType(modelType),
            modelAttributes);
    }

    [Fact]
    public void CreateBindingMetadata_Throws_When_Context_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ServiceEndpointBindingMetadataProvider().CreateBindingMetadata(null!));
    }

    [Theory]
    [InlineData(typeof(IDbConnection))]
    [InlineData(typeof(IUnitOfWork))]
    public void CreateBindingMetadata_Disallows_Binding_For_Connection_Types(Type type)
    {
        var context = CreateContext(type);

        new ServiceEndpointBindingMetadataProvider().CreateBindingMetadata(context);

        Assert.False(context.BindingMetadata.IsBindingAllowed);
    }

    [Fact]
    public void CreateBindingMetadata_Does_Nothing_For_Other_Types()
    {
        var context = CreateContext(typeof(string));

        new ServiceEndpointBindingMetadataProvider().CreateBindingMetadata(context);

        Assert.True(context.BindingMetadata.IsBindingAllowed);
    }
}
