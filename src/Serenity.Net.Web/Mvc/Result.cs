using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Serenity.Services;

/// <summary>
/// An action result type containing a object
/// </summary>
/// <typeparam name="TResponse">Response data type</typeparam>
public class Result<TResponse> : ActionResult
{
    /// <summary>
    /// Content encoding
    /// </summary>
    public Encoding ContentEncoding { get; set; }

    /// <summary>
    /// Content type
    /// </summary>
    public string ContentType { get; set; }

    /// <summary>
    /// JSON serializer settings
    /// </summary>
    public JsonSerializerOptions SerializerOptions { get; set; }

    /// <summary>
    /// The data
    /// </summary>
    public TResponse Data { get; set; }

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="data">Data object</param>
    public Result(TResponse data)
    {
        Data = data;
        SerializerOptions = JSON.Defaults.Strict;
    }

    /// <inheritdoc/>
    public override void ExecuteResult(ActionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var response = context.HttpContext.Response;
        response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

        if (ContentEncoding != null)
            response.Headers.ContentEncoding = ContentEncoding.WebName;

        if (Data != null)
        {
            using var writer = new Utf8JsonWriter(response.Body);
            JsonSerializer.Serialize(writer, Data, SerializerOptions);
            writer.Flush();
        }
    }
}