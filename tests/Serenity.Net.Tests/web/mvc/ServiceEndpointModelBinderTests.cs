namespace Serenity.Services;

public class ServiceEndpointModelBinderTests
{
    private sealed class TestModelBinderProviderContext(Type modelType) : ModelBinderProviderContext
    {
        private readonly ModelMetadata metadata = new EmptyModelMetadataProvider().GetMetadataForType(modelType);

        public override BindingInfo? BindingInfo => null;
        public override ModelMetadata Metadata => metadata;
        public override IModelMetadataProvider MetadataProvider => new EmptyModelMetadataProvider();
        public override IModelBinder CreateBinder(ModelMetadata metadata) => throw new NotImplementedException();
    }

    [Fact]
    public async Task NullModelBinder_Sets_Success_Result_With_Null()
    {
        var context = new DefaultModelBindingContext { ModelName = "test" };

        await ServiceEndpointNullModelBinder.Instance.BindModelAsync(context);

        Assert.True(context.Result.IsModelSet);
        Assert.Null(context.Result.Model);
    }

    [Fact]
    public void ModelBinderProvider_Returns_Null_Binder_For_Other_Types()
    {
        var provider = new ServiceEndpointModelBinderProvider();

        Assert.Null(provider.GetBinder(new TestModelBinderProviderContext(typeof(string))));
    }

    [Theory]
    [InlineData(typeof(IDbConnection))]
    [InlineData(typeof(IUnitOfWork))]
    public void ModelBinderProvider_Returns_Null_ModelBinder_For_Connection_Types(Type type)
    {
        var provider = new ServiceEndpointModelBinderProvider();

        Assert.Same(ServiceEndpointNullModelBinder.Instance,
            provider.GetBinder(new TestModelBinderProviderContext(type)));
    }

    [Fact]
    public void ModelBinderProvider_Throws_When_Context_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ServiceEndpointModelBinderProvider().GetBinder(null!));
    }
}
