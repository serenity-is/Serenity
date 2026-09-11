using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Serenity.Services;

public class ServiceEndpointApplicationModelProviderTests
{
    private class TestEndpoint : ServiceEndpoint
    {
        public void Action(ServiceRequest request)
        {
        }
    }

    private abstract class AbstractEndpoint : ServiceEndpoint
    {
        public void Action(ServiceRequest request)
        {
        }
    }

    private class NormalController
    {
        public void Action()
        {
        }
    }

    private static (ControllerModel controller, ActionModel action) CreateController(
        Type controllerType, string methodName)
    {
        var method = controllerType.GetMethod(methodName)!;
        var controller = new ControllerModel(controllerType.GetTypeInfo(), new List<object>());
        var action = new ActionModel(method, new List<object>()) { Controller = controller };
        foreach (var parameter in method.GetParameters())
            action.Parameters.Add(new ParameterModel(parameter, new List<object>()));
        controller.Actions.Add(action);
        return (controller, action);
    }

    private static ApplicationModelProviderContext CreateContext(params ControllerModel[] controllers)
    {
        var context = new ApplicationModelProviderContext(Array.Empty<TypeInfo>());
        foreach (var controller in controllers)
            context.Result.Controllers.Add(controller);
        return context;
    }

    [Fact]
    public void OnProvidersExecuting_Applies_Convention_To_Endpoints()
    {
        var (_, action) = CreateController(typeof(TestEndpoint), nameof(TestEndpoint.Action));
        var context = CreateContext(action.Controller);

        new ServiceEndpointApplicationModelProvider().OnProvidersExecuting(context);

        Assert.Single(action.Filters.OfType<JsonRequestAttribute>());
    }

    [Fact]
    public void OnProvidersExecuting_Skips_Abstract_Endpoints()
    {
        var (_, action) = CreateController(typeof(AbstractEndpoint), nameof(AbstractEndpoint.Action));
        var context = CreateContext(action.Controller);

        new ServiceEndpointApplicationModelProvider().OnProvidersExecuting(context);

        Assert.Empty(action.Filters.OfType<JsonRequestAttribute>());
    }

    [Fact]
    public void OnProvidersExecuting_Skips_Non_Endpoints()
    {
        var (_, action) = CreateController(typeof(NormalController), nameof(NormalController.Action));
        var context = CreateContext(action.Controller);

        new ServiceEndpointApplicationModelProvider().OnProvidersExecuting(context);

        Assert.Empty(action.Filters.OfType<JsonRequestAttribute>());
    }

    [Fact]
    public void Order_Is_Split_Between_ApiController_And_Authorization_Providers()
    {
        Assert.Equal(-950, new ServiceEndpointApplicationModelProvider().Order);
    }

    [Fact]
    public void OnProvidersExecuted_Does_Nothing()
    {
        new ServiceEndpointApplicationModelProvider().OnProvidersExecuted(null!);
    }
}
