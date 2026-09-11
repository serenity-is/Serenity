using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Serenity.Services;

public class JsonRequestAttributeTests
{
    private class MyRequest
    {
        public int? ID { get; set; }
        public string? Name { get; set; }
    }

    private class TestController
    {
        public void NoParams()
        {
        }

        public void OneRequest(MyRequest request)
        {
        }

        public void TwoParams(MyRequest request, string other)
        {
        }

        public void TwoNonRequest(MyRequest first, MyRequest second)
        {
        }
    }

    private static ActionExecutingContext CreateContext(HttpContext httpContext, string methodName)
    {
        var method = typeof(TestController).GetMethod(methodName)!;
        var actionDescriptor = new ControllerActionDescriptor
        {
            MethodInfo = method,
            ActionName = method.Name,
            Parameters = [.. method.GetParameters().Select(x => new ParameterDescriptor
            {
                Name = x.Name,
                ParameterType = x.ParameterType
            })]
        };

        var actionContext = new ActionContext(httpContext, new RouteData(), actionDescriptor);
        return new ActionExecutingContext(actionContext, [], new Dictionary<string, object?>(), new object());
    }

    private static async Task<bool> Invoke(ActionExecutingContext context, JsonRequestAttribute filter)
    {
        var nextCalled = false;
        await filter.OnActionExecutionAsync(context, () =>
        {
            nextCalled = true;
            return Task.FromResult(new ActionExecutedContext(context, [], context.Controller));
        });
        return nextCalled;
    }

    private static DefaultHttpContext CreateHttpContext(string body, string contentType)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Method = "POST";
        httpContext.Request.ContentType = contentType;
        httpContext.Request.Body = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(body));
        return httpContext;
    }

    [Fact]
    public void Default_Properties()
    {
        var filter = new JsonRequestAttribute();

        Assert.Equal("request", filter.ParamName);
        Assert.True(filter.AllowGet);
        Assert.True(filter.AllowQuery);
        Assert.True(filter.AllowForm);
        Assert.True(JsonRequestAttribute.DefaultAllowGet);
        Assert.True(JsonRequestAttribute.DefaultAllowQuery);
        Assert.True(JsonRequestAttribute.DefaultAllowForm);
    }

    [Fact]
    public void Allow_Properties_Can_Be_Overridden()
    {
        var filter = new JsonRequestAttribute
        {
            AllowGet = false,
            AllowQuery = false,
            AllowForm = false
        };

        Assert.False(filter.AllowGet);
        Assert.False(filter.AllowQuery);
        Assert.False(filter.AllowForm);
    }

    [Fact]
    public async Task OnActionExecutionAsync_Calls_Next_When_No_Parameters()
    {
        var context = CreateContext(new DefaultHttpContext(), nameof(TestController.NoParams));

        Assert.True(await Invoke(context, new JsonRequestAttribute()));
    }

    [Fact]
    public async Task OnActionExecutionAsync_Calls_Next_When_ParamName_Is_Empty()
    {
        var context = CreateContext(new DefaultHttpContext(), nameof(TestController.OneRequest));

        Assert.True(await Invoke(context, new JsonRequestAttribute { ParamName = "" }));
    }

    [Fact]
    public async Task OnActionExecutionAsync_Throws_When_Multiple_Non_Matching_Parameters()
    {
        var context = CreateContext(new DefaultHttpContext(), nameof(TestController.TwoNonRequest));

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            Invoke(context, new JsonRequestAttribute()));
    }

    [Fact]
    public async Task OnActionExecutionAsync_Deserializes_Json_Body()
    {
        var httpContext = CreateHttpContext("""{"ID":5,"Name":"test"}""", "application/json; charset=utf-8");
        var context = CreateContext(httpContext, nameof(TestController.OneRequest));

        await Invoke(context, new JsonRequestAttribute());

        var request = Assert.IsType<MyRequest>(context.ActionArguments["request"]);
        Assert.Equal(5, request.ID);
        Assert.Equal("test", request.Name);
    }

    [Fact]
    public async Task OnActionExecutionAsync_Uses_Parameter_Named_ParamName_When_Multiple()
    {
        var httpContext = CreateHttpContext("""{"ID":7}""", "application/json");
        var context = CreateContext(httpContext, nameof(TestController.TwoParams));

        await Invoke(context, new JsonRequestAttribute());

        Assert.Equal(7, Assert.IsType<MyRequest>(context.ActionArguments["request"]).ID);
    }

    [Fact]
    public async Task OnActionExecutionAsync_Deserializes_Non_Utf8_Encoding()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Method = "POST";
        httpContext.Request.ContentType = "application/json";
        httpContext.Request.Headers.ContentEncoding = "utf-16";
        httpContext.Request.Body = new System.IO.MemoryStream(
            Encoding.Unicode.GetBytes("""{"ID":9}"""));
        var context = CreateContext(httpContext, nameof(TestController.OneRequest));

        await Invoke(context, new JsonRequestAttribute());

        Assert.Equal(9, Assert.IsType<MyRequest>(context.ActionArguments["request"]).ID);
    }

    [Fact]
    public async Task OnActionExecutionAsync_Deserializes_Form_Value()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Method = "POST";
        httpContext.Request.ContentType = "application/x-www-form-urlencoded";
        httpContext.Features.Set<IFormFeature>(new FormFeature(new FormCollection(
            new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                ["request"] = """{"ID":3}"""
            })));
        var context = CreateContext(httpContext, nameof(TestController.OneRequest));

        await Invoke(context, new JsonRequestAttribute());

        Assert.Equal(3, Assert.IsType<MyRequest>(context.ActionArguments["request"]).ID);
    }

    [Fact]
    public async Task OnActionExecutionAsync_Deserializes_Query_Value_For_Get()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Method = "GET";
        httpContext.Request.QueryString = new QueryString("?request=" +
            Uri.EscapeDataString("""{"ID":4}"""));
        var context = CreateContext(httpContext, nameof(TestController.OneRequest));

        await Invoke(context, new JsonRequestAttribute());

        Assert.Equal(4, Assert.IsType<MyRequest>(context.ActionArguments["request"]).ID);
    }

    [Fact]
    public async Task OnActionExecutionAsync_Does_Not_Deserialize_Get_When_AllowGet_False()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Method = "GET";
        httpContext.Request.QueryString = new QueryString("?request=" +
            Uri.EscapeDataString("""{"ID":4}"""));
        var context = CreateContext(httpContext, nameof(TestController.OneRequest));

        await Invoke(context, new JsonRequestAttribute { AllowGet = false });

        Assert.False(context.ActionArguments.ContainsKey("request"));
    }

    [Fact]
    public async Task OnActionExecutionAsync_Does_Nothing_When_Neither_Form_Nor_Query_Allowed()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Method = "GET";
        httpContext.Request.QueryString = new QueryString("?request=" +
            Uri.EscapeDataString("""{"ID":4}"""));
        var context = CreateContext(httpContext, nameof(TestController.OneRequest));

        await Invoke(context, new JsonRequestAttribute { AllowQuery = false, AllowForm = false });

        Assert.False(context.ActionArguments.ContainsKey("request"));
    }

    [Fact]
    public async Task OnActionExecutionAsync_Falls_Back_To_Request_ParamName_For_Query()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Method = "GET";
        httpContext.Request.QueryString = new QueryString("?request=" +
            Uri.EscapeDataString("""{"ID":6}"""));
        var context = CreateContext(httpContext, nameof(TestController.OneRequest));
        var filter = new JsonRequestAttribute { ParamName = "another" };

        await Invoke(context, filter);

        Assert.Equal(6, Assert.IsType<MyRequest>(context.ActionArguments["request"]).ID);
    }

    [Fact]
    public async Task OnActionExecutionAsync_Throws_When_Context_Is_Null()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            new JsonRequestAttribute().OnActionExecutionAsync(null!, () => Task.FromResult<ActionExecutedContext>(null!)));
    }

    [Fact]
    public async Task OnActionExecutionAsync_Throws_When_Next_Is_Null()
    {
        var context = CreateContext(new DefaultHttpContext(), nameof(TestController.NoParams));
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            new JsonRequestAttribute().OnActionExecutionAsync(context, null!));
    }

    [Fact]
    public void JsonFilter_Is_Obsolete_JsonRequestAttribute()
    {
#pragma warning disable CS0618
        Assert.Equal("request", new JsonFilter().ParamName);
#pragma warning restore CS0618
    }
}
