using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Serenity.Web;

public class FeatureBarrierAttributeTests
{
    private static (ActionConstraintContext context, IServiceProvider services) CreateConstraintContext(
        IFeatureToggles? featureToggles)
    {
        var services = new ServiceCollection();
        if (featureToggles != null)
            services.AddSingleton(featureToggles);
        var provider = services.BuildServiceProvider();

        var httpContext = new DefaultHttpContext { RequestServices = provider };
        return (new ActionConstraintContext
        {
            RouteContext = new RouteContext(httpContext)
        }, provider);
    }

    private static PageHandlerExecutingContext CreatePageExecutingContext(IFeatureToggles? featureToggles)
    {
        var services = new ServiceCollection();
        if (featureToggles != null)
            services.AddSingleton(featureToggles);
        var httpContext = new DefaultHttpContext { RequestServices = services.BuildServiceProvider() };
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
        var pageContext = new PageContext(actionContext) { ViewData = viewData };

        return new PageHandlerExecutingContext(pageContext, [], null,
            new Dictionary<string, object?>(), new object());
    }

    [Fact]
    public void Order_Is_Zero()
    {
        Assert.Equal(0, new FeatureBarrierAttribute("Feature").Order);
    }

    [Fact]
    public void Accept_Returns_True_When_FeatureToggles_Not_Registered()
    {
        var attribute = new FeatureBarrierAttribute("Feature");
        var (context, _) = CreateConstraintContext(null);

        Assert.True(attribute.Accept(context));
    }

    [Fact]
    public void Accept_Returns_True_When_Feature_Enabled()
    {
        var attribute = new FeatureBarrierAttribute("Feature");
        var (context, _) = CreateConstraintContext(new MockFeatureToggles
        {
            IsEnabledCallback = key => key == "Feature"
        });

        Assert.True(attribute.Accept(context));
    }

    [Fact]
    public void Accept_Returns_False_When_Feature_Disabled()
    {
        var attribute = new FeatureBarrierAttribute("Feature");
        var (context, _) = CreateConstraintContext(new MockFeatureToggles
        {
            IsEnabledCallback = _ => false
        });

        Assert.False(attribute.Accept(context));
    }

    [Fact]
    public void Accept_Returns_True_When_RequireAny_And_One_Enabled()
    {
        var attribute = new FeatureBarrierAttribute("A", "B") { RequireAny = true };
        var (context, _) = CreateConstraintContext(new MockFeatureToggles
        {
            IsEnabledCallback = key => key == "B"
        });

        Assert.True(attribute.Accept(context));
    }

    [Fact]
    public void OnPageHandlerExecuting_Does_Nothing_When_FeatureToggles_Not_Registered()
    {
        var attribute = new FeatureBarrierAttribute("Feature");
        var context = CreatePageExecutingContext(null);

        attribute.OnPageHandlerExecuting(context);

        Assert.Null(context.Result);
    }

    [Fact]
    public void OnPageHandlerExecuting_Sets_NotFound_When_Feature_Disabled()
    {
        var attribute = new FeatureBarrierAttribute("Feature");
        var context = CreatePageExecutingContext(new MockFeatureToggles
        {
            IsEnabledCallback = _ => false
        });

        attribute.OnPageHandlerExecuting(context);

        Assert.IsType<NotFoundResult>(context.Result);
    }

    [Fact]
    public void OnPageHandlerExecuting_Does_Nothing_When_Feature_Enabled()
    {
        var attribute = new FeatureBarrierAttribute("Feature");
        var context = CreatePageExecutingContext(new MockFeatureToggles
        {
            IsEnabledCallback = _ => true
        });

        attribute.OnPageHandlerExecuting(context);

        Assert.Null(context.Result);
    }

    [Fact]
    public void Constructor_With_Object_Features_Uses_Feature_Keys()
    {
        var attribute = new FeatureBarrierAttribute(TestFeature.SomeFeature);

        Assert.Equal(["SomeFeature"], attribute.Features);
    }

    [Fact]
    public void Constructor_With_No_Features_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new FeatureBarrierAttribute());
    }

    private enum TestFeature
    {
        SomeFeature
    }
}
