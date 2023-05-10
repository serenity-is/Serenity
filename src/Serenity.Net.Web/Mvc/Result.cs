using System.IO;
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
    public JsonSerializerSettings SerializerSettings { get; set; }

    /// <summary>
    /// The data
    /// </summary>
    public TResponse Data { get; set; }

    /// <summary>
    /// Formatting
    /// </summary>
    public Formatting Formatting { get; set; }

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="data">Data object</param>
    public Result(TResponse data)
    {
        Data = data;
        SerializerSettings = JsonSettings.Strict;
    }

    /// <inheritdoc/>
    public override void ExecuteResult(ActionContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        var response = context.HttpContext.Response;
        response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

        if (ContentEncoding != null)
            response.Headers["Content-Encoding"] = ContentEncoding.WebName;
        if (Data != null)
        {
            JsonTextWriter writer = new(new StreamWriter(response.Body)) { Formatting = Formatting };
            JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
            serializer.Serialize(writer, Data);
            writer.Flush();
        }
    }
}