using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Serenity.Services;

/// <summary>
/// An action result type containing an object with a status code.
/// </summary>
/// <typeparam name="TResponse">The response data type.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="ResultWithStatus{TResponse}"/> class.
/// </remarks>
/// <param name="data">The data object.</param>
/// <param name="statusCode">The status code.</param>
public class ResultWithStatus<TResponse>(int statusCode, TResponse data) : StatusCodeResult(statusCode)
{
    /// <summary>
    /// Gets or sets the content encoding.
    /// </summary>
    public Encoding ContentEncoding { get; set; }

    /// <summary>
    /// Gets or sets the content type.
    /// </summary>
    public string ContentType { get; set; }

    /// <summary>
    /// Gets or sets the JSON serializer settings.
    /// </summary>
    public JsonSerializerOptions SerializerOptions { get; set; } = JSON.Defaults.Strict;

    /// <summary>
    /// Gets or sets the data.
    /// </summary>
    public TResponse Data { get; set; } = data;

    /// <inheritdoc/>
    public override async Task ExecuteResultAsync(ActionContext context)
    {
        await base.ExecuteResultAsync(context);

        ArgumentNullException.ThrowIfNull(context);

        var response = context.HttpContext.Response;
        response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

        if (ContentEncoding != null)
            response.Headers.ContentEncoding = ContentEncoding.WebName;

        if (Data != null)
            await JsonSerializer.SerializeAsync(response.Body, Data, SerializerOptions);
    }
}