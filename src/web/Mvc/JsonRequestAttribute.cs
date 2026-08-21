using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Serenity.Services;

/// <summary>
/// An action filter for methods that accept JSON content via their
/// "request" arguments.
/// </summary>
public class JsonRequestAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonRequestAttribute"/> class.
    /// </summary>
    public JsonRequestAttribute()
    {
        ParamName = "request";
    }

    /// <inheritdoc/>
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        var request = context.HttpContext.Request;
        string method = request.Method ?? "";
        var prms = context.ActionDescriptor
            .Parameters
            .Where(x => !x.ParameterType.IsInterface);

        if (!prms.Any() || string.IsNullOrEmpty(ParamName))
        {
            await next();
            return;
        }

        if (prms.Count() != 1)
        {
            prms = prms.Where(x => x.Name == ParamName);

            if (prms.Count() != 1)
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture,
                    "Method {0} has {1} parameters. [JsonRequest] requires an action method with only one parameter," + 
                    "or a parameter with name '{2}'!",
                        ((ControllerActionDescriptor)context.ActionDescriptor).ActionName, 
                        context.ActionDescriptor.Parameters.Count,
                        ParamName));
        }

        var prm = prms.Single();

        if (method.Equals("POST", StringComparison.OrdinalIgnoreCase) ||
            method.Equals("PUT", StringComparison.OrdinalIgnoreCase))
        {
            if ((request.ContentType ?? string.Empty)
                .Contains("application/json", StringComparison.OrdinalIgnoreCase))
            {
                if (context.HttpContext.Request.Body.CanSeek)
                    context.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);

                var encoding = (string)context.HttpContext.Request.Headers.ContentEncoding ?? "utf-8";
                object obj;
                if (string.Equals(encoding, "utf-8", StringComparison.OrdinalIgnoreCase))
                {
                    obj = await JsonSerializer.DeserializeAsync(context.HttpContext.Request.Body, prm.ParameterType,
                        JSON.Defaults.Strict);
                }
                else
                {
                    using var sr = new StreamReader(context.HttpContext.Request.Body,
                        Encoding.GetEncoding(encoding)); 
                    obj = JsonSerializer.Deserialize(await sr.ReadToEndAsync(), prm.ParameterType, JSON.Defaults.Strict);
                }

                context.ActionArguments[prm.Name] = obj;
            }
            else 
            {
                var req = FromFormOrQuery(request, prm.Name);
                if (req != null)
                {
                    var obj = JsonSerializer.Deserialize(req, prm.ParameterType, JSON.Defaults.Strict);
                    context.ActionArguments[prm.Name] = obj;
                }
            }
        }
        else if (AllowGet && 
            method.Equals("GET", StringComparison.OrdinalIgnoreCase))
        {
            var req = FromFormOrQuery(request, prm.Name);
            if (req != null)
            {
                var obj = JsonSerializer.Deserialize(req, prm.ParameterType, JSON.Defaults.Strict);
                context.ActionArguments[prm.Name] = obj;
            }
        }

        await next();
    }

    private string FromFormOrQuery(HttpRequest request, string name)
    {
        var allowForm = AllowForm && request.HasFormContentType;
        var allowQuery = AllowQuery;
        if (!allowForm && !allowQuery)
            return null;

        string value;
        if (allowForm)
        {
            value = request.Form[name];
            if (value != null)
                return value;
        }

        if (allowQuery)
        {
            value = request.Query[name];
            if (value != null)
                return value;
        }

        if (name != ParamName && !string.IsNullOrEmpty(ParamName))
            return FromFormOrQuery(request, ParamName);

        if (name != "request")
            return FromFormOrQuery(request, "request");

        return null;
    }

    /// <summary>
    /// Gets or sets the parameter name for the <c>request</c> argument.
    /// </summary>
    public string ParamName { get; set; }

    private bool? allowGet;

    /// <summary>
    /// Gets or sets whether to allow this filter for GET requests.
    /// </summary>
    public bool AllowGet
    {
        get => allowGet ?? DefaultAllowGet;
        set => allowGet = value;
    }

    private bool? allowQuery;

    /// <summary>
    /// Gets or sets whether to allow getting the JSON body from the query string.
    /// </summary>
    public bool AllowQuery
    {
        get => allowQuery ?? DefaultAllowQuery;
        set => allowQuery = value;
    }

    private bool? allowForm;

    /// <summary>
    /// Gets or sets whether to allow getting the JSON body from the posted form.
    /// </summary>
    public bool AllowForm
    {
        get => allowForm ?? DefaultAllowForm;
        set => allowForm = value;
    }

    /// <summary>
    /// The default for <see cref="AllowGet"/>, which is <c>true</c>.
    /// </summary>
    public static bool DefaultAllowGet { get; set; } = true;

    /// <summary>
    /// The default for <see cref="AllowQuery"/>, which is <c>true</c>.
    /// </summary>
    public static bool DefaultAllowQuery { get; set; } = true;

    /// <summary>
    /// The default for <see cref="AllowForm"/>, which is <c>true</c>.
    /// </summary>
    public static bool DefaultAllowForm { get; set; } = true;
}

/// <summary>
/// Obsolete version of <see cref="JsonRequestAttribute"/> attribute.
/// </summary>
[Obsolete("Prefer [JsonRequestAttribute]")]
public class JsonFilter : JsonRequestAttribute
{
}