using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Serenity.Services;

/// <summary>
/// An action result type containing a object with status code
/// </summary>
/// <typeparam name="TResponse">Response data type</typeparam>
/// <remarks>
/// Creates a new instance of the class
/// </remarks>
/// <param name="data">Data object</param>
/// <param name="statusCode">Status code</param>
public class ResultWithStatus<TResponse>(int statusCode, TResponse data) : StatusCodeResult(statusCode)
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
    public JsonSerializerOptions SerializerOptions { get; set; } = JSON.Defaults.Strict;

    /// <summary>
    /// The data
    /// </summary>
    public TResponse Data { get; set; } = data;

    /// <inheritdoc/>
    public override void ExecuteResult(ActionContext context)
    {
        base.ExecuteResult(context);

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