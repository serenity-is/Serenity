using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Serenity.Services;

public class ServiceEndpointActionModelConventionTests
{
    private class TestEndpoint : ServiceEndpoint
    {
        public void Action(ServiceRequest request, IDbConnection connection, IUnitOfWork uow)
        {
        }

        public void SingleRequest(ServiceRequest request)
        {
        }
    }

    private class NormalController
    {
        public void Action(ServiceRequest request)
        {
        }
    }

    private static ActionModel CreateActionModel(Type controllerType, string methodName)
    {
        var method = controllerType.GetMethod(methodName)!;
        var controller = new ControllerModel(controllerType.GetTypeInfo(), new List<object>());
        var action = new ActionModel(method, new List<object>()) { Controller = controller };
        foreach (var parameter in method.GetParameters())
            action.Parameters.Add(new ParameterModel(parameter, new List<object>()));
        return action;
    }

    [Fact]
    public void Apply_Ignores_Non_ServiceEndpoint_Controllers()
    {
        var action = CreateActionModel(typeof(NormalController), nameof(NormalController.Action));
        var parameter = action.Parameters[0];

        new ServiceEndpointActionModelConvention().Apply(action);

        Assert.Empty(action.Filters.OfType<JsonRequestAttribute>());
        Assert.Null(parameter.BindingInfo);
    }

    [Fact]
    public void Apply_Adds_JsonRequest_And_Body_Binding_For_ServiceRequest()
    {
        var action = CreateActionModel(typeof(TestEndpoint), nameof(TestEndpoint.SingleRequest));

        new ServiceEndpointActionModelConvention().Apply(action);

        var parameter = action.Parameters[0];
        Assert.Single(action.Filters.OfType<JsonRequestAttribute>());
        Assert.NotNull(parameter.BindingInfo);
        Assert.Equal(BindingSource.Body, parameter.BindingInfo!.BindingSource);
        Assert.Equal(typeof(ServiceEndpointNullModelBinder), parameter.BindingInfo.BinderType);
    }

    [Fact]
    public void Apply_Does_Not_Add_Duplicate_JsonRequest_Filter()
    {
        var action = CreateActionModel(typeof(TestEndpoint), nameof(TestEndpoint.SingleRequest));
        action.Filters.Add(new JsonRequestAttribute());

        new ServiceEndpointActionModelConvention().Apply(action);

        Assert.Single(action.Filters.OfType<JsonRequestAttribute>());
    }

    [Fact]
    public void Apply_Sets_Special_Binding_For_Connection_And_UnitOfWork()
    {
        var action = CreateActionModel(typeof(TestEndpoint), nameof(TestEndpoint.Action));

        new ServiceEndpointActionModelConvention().Apply(action);

        var connectionParameter = action.Parameters[1];
        var unitOfWorkParameter = action.Parameters[2];

        Assert.Equal(BindingSource.Special, connectionParameter.BindingInfo!.BindingSource);
        Assert.Equal(typeof(ServiceEndpointNullModelBinder), connectionParameter.BindingInfo.BinderType);
        Assert.Equal(BindingSource.Special, unitOfWorkParameter.BindingInfo!.BindingSource);
        Assert.Equal(typeof(ServiceEndpointNullModelBinder), unitOfWorkParameter.BindingInfo.BinderType);
    }

    [Fact]
    public void Apply_Does_Not_Override_Existing_BindingInfo()
    {
        var action = CreateActionModel(typeof(TestEndpoint), nameof(TestEndpoint.SingleRequest));
        var parameter = action.Parameters[0];
        var bindingInfo = new BindingInfo { BindingSource = BindingSource.Query };
        parameter.BindingInfo = bindingInfo;

        new ServiceEndpointActionModelConvention().Apply(action);

        Assert.Same(bindingInfo, parameter.BindingInfo);
    }
}
